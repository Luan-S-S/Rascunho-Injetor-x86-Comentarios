using System.Diagnostics;

namespace Rascunho_Injetor_Dll
{
    public partial class Form1
    {

        // Gera uma sequência aleatória de caracteres para utilizar como título da janela.
        private string GenerateRandomWindowTitle()
        {
            string numers = "0123456789";
            string low = "abcdefghijklmnopqrstuvwxyz";
            string upp = low.ToUpper();

            string fullString = String.Concat(numers, low, upp);

            Random random = new Random();

            int tamName = random.Next(6, fullString.Length);

            char[] result = new char[tamName];

            for (int i = 0; i < tamName; i++)
            {
                result[i] = fullString[random.Next(fullString.Length)];
            }

            return new string(result);
        }

        // Atualiza a lista de processos disponíveis na interface.
        // São exibidos somente processos que possuem a mesma arquitetura do injetor.
        private void RefreshProcessList()
        {
            // Obtém os processos em execução, filtra pela arquitetura e ordena pelo nome.
            var availableProcesses = Process.GetProcesses().
                Where(proc => DllInjector.GetProcessArchitecture(proc) == Form1.CURRENT_ARCHITECTURE).
                OrderBy(proc => proc.ProcessName).ToList();


            comboBoxProcesses.DisplayMember = "ProcessName";
            comboBoxProcesses.ValueMember = "Id";
            comboBoxProcesses.DataSource = availableProcesses;
        }

        // Adiciona uma mensagem ao RichTextBox com horário e cor personalizados.
        public static void AppendLogMessage(RichTextBox rtb, string text, Color color)
        {
            rtb.SelectionStart = rtb.TextLength;
            rtb.SelectionLength = 0; // Posiciona o cursor no final do conteúdo.

            string timestamp = $"[{DateTime.Now:HH:mm:ss}] ";
            rtb.AppendText(timestamp); // Adiciona o horário da mensagem.

            rtb.SelectionColor = color; // Define a cor do texto da mensagem.

            rtb.AppendText(text + "\n");

            rtb.SelectionColor = rtb.ForeColor; // Restaura a cor padrão do controle.

            rtb.ScrollToCaret(); // Mantém a visualização posicionada na última mensagem.
        }

        // Limpa os campos relacionados à seleção do processo e da DLL.
        private void ResetSelectionFields()
        {
            //Relacionados ao processo
            comboBoxProcesses.SelectedIndex = -1;
            lblProcessStatus.Text = "";
            PID = 0;
            targetProcessName = "";
            lblProcessArchitecture.Text = "";

            //Relacionados a Dll
            txtSelectedDll.Text = "";
            lblDllStatus.Text = "";
            dllFileName = "";
            lblDllArchitecture.Text = "";

        }

        // Atualiza a visibilidade dos botões utilizados na procura automática.
        private void UpdateProcessSearchButtons(bool enable)
        {
            if (!enable)
            {
                // Durante a pesquisa, permite somente interromper a operação.
                btnStartAutoInject.Visible = false;
                btnStopSearch.Visible = true;
                return;
            }
            // Quando a pesquisa termina, permite iniciar novamente.
            btnStopSearch.Visible = false;
            btnStartAutoInject.Visible = true;
        }

        // Alterna entre o modo de seleção manual e o modo de pesquisa automática.
        private void SetAutomaticSearchMode(bool enable)
        {
            if (!enable)
            {
                // Exibe os controles utilizados na seleção manual.
                comboBoxProcesses.Visible = true;
                btnInject.Visible = true;

                // Oculta os controles utilizados na pesquisa automática.
                txtProcessName.Visible = false;
                btnStartAutoInject.Visible = false;
                return;
            }

            // Oculta os controles utilizados na seleção manual.
            comboBoxProcesses.Visible = false;
            btnInject.Visible = false;

            // Exibe os controles utilizados na pesquisa automática.
            txtProcessName.Visible = true;
            btnStartAutoInject.Visible = true;
        }

        // Registra uma exceção no log e apresenta uma janela com sua mensagem.
        private void ShowException(Exception ex)
        {
            AppendLogMessage(this.rtbLogs, "[-] -> " + ex.Message, Color.Red);
            MessageBox.Show(ex.Message);
        }

        // Procura continuamente pelo processo informado.
        // Quando encontra o processo, interrompe a pesquisa e inicia a injeção.
        private void SearchForTargetProcess(Object? sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(targetProcessName);

            if (processes.Length > 0)
            {
                processSearchTimer.Stop();
                PID = processes.FirstOrDefault().Id;
                InjectSelectedDll();
                UpdateProcessSearchButtons(true);
            }
        }
        private bool InjectSelectedDll() // Executa a injeção da DLL selecionada e trata os resultados da operação.
        {
            rtbLogs.Clear(); // Limpa o log anterior.

            bool injectionSucceeded = false;
            Color messageColor = Color.Red;
            // Mensagem padrão utilizada quando a injeção falha.
            string resultMessage = "[-] ----------------> Falha ao injetar Dll <------------------- [-]";

            try
            {
                // Cria o objeto responsável por gerenciar a injeção.
                dllInjector = new DllInjector(PID, dllFileName, this.rtbLogs);
                // Executa a injeção utilizando o método selecionado e a opção de UnHook.
                injectionSucceeded = dllInjector.InjectDll(selectedLoadMethod, chkUnhook.Checked);
            }
            catch (InvalidOperationException ex)
            {
                ShowException(ex); // Trata erros relacionados às validações realizadas pelo injetor.
            }

            catch (Exception ex)
            {
                ShowException(ex); // Trata outras exceções não previstas.
            }
            finally // Executa os procedimentos finais independentemente do resultado da injeção.
            {
                if (injectionSucceeded)
                {
                    resultMessage = "[+] => Dll Injetada com Sucesso! ";
                    messageColor = Color.Blue;
                    // Procura o endereço base da DLL carregada no processo remoto.
                    IntPtr moduleAddress = DllInjector.GetRemoteModuleAddress(Path.GetFileName(dllFileName), dllInjector.TargetProcess);
                    // Monta a mensagem contendo o endereço encontrado.
                    string moduleMessage = moduleAddress != IntPtr.Zero ? $"-> (Modulo: 0x{moduleAddress.ToString("X")})" : "[-] Falha ao obter modulo da DLL";

                    resultMessage = String.Concat(resultMessage, moduleMessage);

                }
                AppendLogMessage(this.rtbLogs, resultMessage, messageColor); // Registra o resultado final da injeção.
                ResetSelectionFields(); // Limpa os campos da interface.
                processSearchTimer.Stop(); // Garante que o Timer de pesquisa esteja parado.
                // Verifica se existe um handle válido do processo para liberar.
                if (dllInjector != null && dllInjector.ModuleProc != IntPtr.Zero)
                {
                    Kernel32.CloseHandle(dllInjector.ModuleProc); // Fecha o handle aberto por OpenProcess.
                    dllInjector.ModuleProc = IntPtr.Zero; // Zera a referência para evitar reutilização acidental do handle.
                    // CloseHandle fecha o handle no Windows, mas não altera automaticamente
                    // o valor armazenado na variável ModuleProc.
                }
            }
            // Encerra a aplicação somente quando a opção estiver habilitada
            // e a injeção tiver sido concluída com sucesso.
            if (chkCloseAutomatically.Checked && injectionSucceeded)
            {
                Application.Exit();
            }

            return injectionSucceeded;
        }
    }
}
