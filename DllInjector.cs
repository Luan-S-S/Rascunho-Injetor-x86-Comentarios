using System.Diagnostics;

namespace Rascunho_Injetor_Dll
{
    // Responsável por gerenciar o processo de injeção de uma DLL em um processo externo.
    // Realiza validações de arquitetura, obtenção de handles e execução do método
    // de carregamento selecionado.
    class DllInjector
    {
        // Inicializa o injetor, identificando o processo alvo, validando o caminho
        // da DLL e verificando a compatibilidade entre as arquiteturas envolvidas.
        public DllInjector(int pid, string pathDll, RichTextBox rTxBLogs)
        {
            // Atribuído primeiro porque EnsureSuccess utiliza RTxBLogs para registrar mensagens.
            this.RTxBLogs = rTxBLogs;

            // Verifica se um processo foi selecionado.
            EnsureSuccess(pid != 0, $"[+] -> Processo Selecionado Encotrado!", "Processo não foi Selecionado. PID = 0");

            // Obtém o objeto Process correspondente ao PID informado.
            TargetProcess = Process.GetProcessById(pid);

            // Define o caminho da DLL que será utilizada na injeção.
            DllPath = pathDll;

            // Identifica a arquitetura da DLL através da leitura de seu cabeçalho PE.
            DllArchitecture = GetPEArchitecture(DllPath);

            // Identifica a arquitetura do processo alvo utilizando IsWow64Process2.
            ProcessArchitecture = GetProcessArchitecture(TargetProcess);

            // Verifica se o injetor, a DLL e o processo possuem arquiteturas compatíveis.
            ValidateArchitectures(Form1.CURRENT_ARCHITECTURE, DllArchitecture, ProcessArchitecture);
        }


        public Process TargetProcess // Representa o processo que receberá a DLL.
        {
            get
            {
                return _targetProcess;
            }
            private set
            {
                EnsureSuccess(value != null, $"[*] -> Processo Alvo        :      {value?.ProcessName}  (PID: {value?.Id})", "Processo não encontrado");
                _targetProcess = value;
            }
        }

        public string DllPath // Armazena o caminho completo da DLL selecionada.
        {
            get
            {
                return _dllPath;
            }
            private set
            {
                EnsureSuccess(!string.IsNullOrEmpty(value), $"[*] -> Caminho da Dll     :      {value}\n", "Dll Selecionada Invalida!");
                _dllPath = value;
            }
        }

        // Handle do processo remoto.
        // É utilizado pelas operações de manipulação de memória e criação de threads.
        public IntPtr ModuleProc { get; set; } = IntPtr.Zero;

        // Handle retornado pelo método de carregamento da DLL.
        // Representa o resultado da operação de carregamento no processo remoto.
        public IntPtr ProcessHandle { get; private set; } = IntPtr.Zero;

        // Referência ao RichTextBox utilizado para registrar o andamento da injeção.
        public RichTextBox RTxBLogs { get; set; }
        public string DllArchitecture { get; private set; } // Arquitetura identificada no arquivo PE da DLL.
        public string ProcessArchitecture { get; private set; } // Arquitetura identificada no processo alvo.

        private string _dllPath;
        private Process _targetProcess;

        // Identifica a arquitetura de um processo utilizando a API IsWow64Process2.
        public static string GetProcessArchitecture(Process process)
        {
            // Obtém um handle do processo com permissão para consultar informações limitadas.
            IntPtr hProcess = Kernel32.OpenProcess(Kernel32.PROCESS_QUERY_LIMITED_INFORMATION, false, process.Id);
            if (hProcess == IntPtr.Zero) return "???";

            // Consulta a arquitetura do processo e a arquitetura nativa do sistema.
            bool sucess = Kernel32.IsWow64Process2(hProcess, out ushort processMachine, out ushort nativeMachine);

            Kernel32.CloseHandle(hProcess);

            if (!sucess) return "???";

            // IMAGE_FILE_MACHINE_UNKNOWN indica que o processo utiliza a arquitetura nativa.
            if (processMachine == Kernel32.IMAGE_FILE_MACHINE_UNKNOWN)
            {
                switch (nativeMachine)
                {
                    case (ushort)Kernel32.IMAGE_FILE_MACHINE_I386:
                        return Form1.X86;
                    case (ushort)Kernel32.IMAGE_FILE_MACHINE_AMD64:
                        return Form1.X64;
                    case (ushort)Kernel32.IMAGE_FILE_MACHINE_ARM64:
                        return Form1.ARM64;
                }
                return "Arquitetura Desconhecida";
            }

            // Quando processMachine possui outro valor, indica uma arquitetura
            // diferente da arquitetura nativa, como um processo executado via WOW64.
            switch (processMachine)
            {
                case (ushort)Kernel32.IMAGE_FILE_MACHINE_I386:
                    return Form1.X86;
                case (ushort)Kernel32.IMAGE_FILE_MACHINE_ARM:
                    return Form1.ARM;
            }
            return "Arquitetura Desconhecida";
        }

        // Identifica a arquitetura de um arquivo executável ou DLL através
        // da leitura dos campos Machine do cabeçalho PE.
        public static string GetPEArchitecture(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // 1. Offset para o cabeçalho PE (está no offset 0x3C do DOS header)
                fs.Seek(0x3C, SeekOrigin.Begin);
                int peOffset = br.ReadInt32();

                // 2. Ir até o cabeçalho PE
                fs.Seek(peOffset, SeekOrigin.Begin);
                uint peHead = br.ReadUInt32();

                if (peHead != 0x4550)// "PE\0\0"
                {
                    return "Arquivo não é um PE válido.";
                }

                // 3. Ler Machine (2 bytes)
                ushort arch = br.ReadUInt16();

                switch (arch)
                {
                    case (int)Kernel32.IMAGE_FILE_MACHINE_I386://0x014c:
                        return Form1.X86;
                    case (int)Kernel32.IMAGE_FILE_MACHINE_AMD64://0x8664:
                        return Form1.X64;
                    case (int)Kernel32.IMAGE_FILE_MACHINE_ARM64://0xAA64:
                        return Form1.ARM64;
                }

                return "Arquitetura Desconhecida";
            }
        }

        // Procura um módulo carregado dentro de um processo e retorna seu endereço base.
        public static IntPtr GetRemoteModuleAddress(string moduleName, Process process)
        {
            IntPtr module = IntPtr.Zero;

            foreach (ProcessModule mod in process.Modules)
            {
                if (mod.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    module = mod.BaseAddress;
                    break;
                }

            }
            return module;
        }

        // Compara as arquiteturas do injetor, da DLL e do processo alvo.
        // A injeção somente é permitida quando todas são compatíveis.
        public void ValidateArchitectures(string archCurr, string archDll, string archProc)
        {
            Form1.AppendLogMessage(RTxBLogs, $"[*] -> Arquitetura Proc.  :       {archProc}", archProc == archCurr ? Color.Green : Color.Red);
            Form1.AppendLogMessage(RTxBLogs, $"[*] -> Arquitetura DLL.   :       {archDll}", archDll == archCurr ? Color.Green : Color.Red);

            EnsureSuccess(archDll == archCurr && archProc == archCurr, "[+] -> Arquiteturas Compativeis com a injeção\n", "Arquitetura da dll Não Permitida para este injetor");
        }

        // Centraliza a validação de operações e o registro de mensagens.
        // Quando a condição é falsa, lança uma exceção com a mensagem informada.
        public void EnsureSuccess(bool result, string msgSucess, string msgFailed)
        {
            if (!result) throw new InvalidOperationException(msgFailed);
            Form1.AppendLogMessage(RTxBLogs, msgSucess, Color.Green);
        }

        // Executa o processo de injeção utilizando o método de carregamento selecionado.
        // Também permite restaurar os bytes originais da API utilizada, caso solicitado.
        public bool InjectDll(string typeLoadLibrary, bool restoreApiBytes)
        {
            // Procura o método de carregamento correspondente ao nome informado.
            // O Dictionary associa o nome da API ao respectivo delegate.
            EnsureSuccess(LoadDll.LoadMethods.TryGetValue(typeLoadLibrary, out DllLoadMethod loadMethod), $"[*] -> Type Load API       :       {typeLoadLibrary}\n", "Nome da API de injeção Incorreto!");

            // Abre o processo alvo solicitando os direitos necessários para as operações
            // de criação de thread e manipulação de memória.
            ModuleProc = Kernel32.OpenProcess(Kernel32.PROCESS_CREATE_THREAD |
            Kernel32.PROCESS_QUERY_INFORMATION |
            Kernel32.PROCESS_VM_OPERATION |
            Kernel32.PROCESS_VM_READ |
            Kernel32.PROCESS_VM_WRITE, false, TargetProcess.Id);

            EnsureSuccess(ModuleProc != IntPtr.Zero, $"[+] -> Handle do processo obtida com sucesso: 0x{ModuleProc.ToString("X")}\n", "Falha ao obter Handle de: " + TargetProcess.ProcessName);

            // Executa o procedimento de restauração dos bytes da API selecionada,
            // caso a opção tenha sido habilitada na interface.
            if (restoreApiBytes)
            {
                bool unHooked = UnHooking.UnHook(ModuleProc, TargetProcess.Id, UnHooking.APIModule[typeLoadLibrary], typeLoadLibrary, UnHooking.NUM_BYTES, out UnHooking.OldProtect, UnHooking.OldBytes);
                EnsureSuccess(unHooked, $"[+] -> UnHook da API: ({typeLoadLibrary}). Realizado com Sucesso!\n" +
                    $"                  [*] -> Bytes Antigos: |{String.Join("|", UnHooking.OldBytes.Select(b => b.ToString("X2")))}\n" +
                    $"                  [*] -> Proteção antiga: {(UnHooking.MemoryProtection)UnHooking.OldProtect}\n", $"Falha ao realizar UnHook na API {typeLoadLibrary} do Processo: {this.TargetProcess.ProcessName}");
            }

            // Executa o método de carregamento selecionado.
            ProcessHandle = loadMethod(ModuleProc, DllPath, out uint errorCode);

            // Obtém a lista de erros correspondente ao método de carregamento utilizado.
            InjectionErrors errors = new InjectionErrors(typeLoadLibrary);

            // Percorre os erros possíveis e registra as etapas concluídas.
            // Quando o código retornado corresponde a um erro, EnsureSuccess lança uma exceção.
            errors.ErrorList.ForEach(err => EnsureSuccess((uint)err.errorCode != errorCode, "[+] -> " + err.SuccessMessage, err.Description));

            return ProcessHandle != IntPtr.Zero && errorCode == 0;
        }
    }
}


