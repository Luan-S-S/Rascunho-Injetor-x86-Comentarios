using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;
namespace Rascunho_Injetor_Dll
{

    public partial class Form1 : Form
    {
        // Identificadores utilizados para representar as arquiteturas suportadas.
        public const string X86 = "x86";
        public const string X64 = "x64";
        public const string ARM = "ARM";
        public const string ARM64 = "ARM64";
        public static string CURRENT_ARCHITECTURE = ""; // Armazena a arquitetura do processo atual do próprio injetor.

        int PID; // PID do processo selecionado para receber a DLL.
        // Caminho da DLL, método de carregamento selecionado e nome do processo alvo.
        string dllFileName, selectedLoadMethod, targetProcessName;
        DllInjector dllInjector; // Instância responsável por executar a injeção.
        Timer processSearchTimer; // Timer utilizado para pesquisar automaticamente o processo alvo.


        public Form1()
        {
            // Solicita a habilitação do privilégio SeDebugPrivilege.
            // Esse privilégio pode permitir acesso a processos que normalmente
            // possuem restrições adicionais de segurança.
            InitializeComponent();
            if (!SeDebugPrivilege.SetEnabled(true)) //Habilitando Privilegio de debug
                MessageBox.Show("Falha ao Habilitar privilégios de Debug");

            this.Text = GenerateRandomWindowTitle(); // Define um título aleatório para a janela.

            rbtnLoadLibraryA.Checked = true; // Define LoadLibraryA como método de carregamento padrão.
            selectedLoadMethod = LoadDll.LOAD_LIBRARY_A;

            processSearchTimer = new Timer() { Interval = 100 };
            processSearchTimer.Tick += SearchForTargetProcess;

            CURRENT_ARCHITECTURE = Environment.Is64BitProcess ? X64 : X86; // Identifica a arquitetura do próprio injetor.

        }

        // Evento disparado quando o usuário solicita a injeção manualmente.
        private void InjectButton_Click(object sender, EventArgs e)
        {
            InjectSelectedDll();
        }

        // Evento responsável por iniciar a procura automática de um processo.
        private void AutoInjectButton_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrWhiteSpace(targetProcessName))
            {
                ShowException(new InvalidOperationException("Processo não Selecionado"));
                return;
            }

            AppendLogMessage(this.rtbLogs, $"Aguardando abertura do processo: {targetProcessName}...", Color.Black);

            processSearchTimer.Start();

            UpdateProcessSearchButtons(false);
        }

        // Interrompe a pesquisa automática pelo processo alvo.
        private void StopSearchingButton_Click(object sender, EventArgs e)
        {
            processSearchTimer.Stop();
            UpdateProcessSearchButtons(true);
            AppendLogMessage(this.rtbLogs, $"Cancelada a procura pelo processo: {targetProcessName}", Color.Black);
            targetProcessName = "";
            lblProcessStatus.Text = "";
            PID = 0;
        }

        // Abre a janela de seleção de arquivos para permitir que o usuário escolha uma DLL.
        private void SelectDllButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecione uma DLL";
                openFileDialog.Filter = "Bibliotecas DLL (*.dll)|*.dll";
                openFileDialog.Multiselect = false; // Permite selecionar somente um arquivo.


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dllFileName = openFileDialog.FileName; // Armazena o caminho completo da DLL.
                    // Obtém somente o nome do arquivo, sem o diretório.
                    string selectedDllPath = Path.GetFileName(dllFileName);

                    txtSelectedDll.Text = selectedDllPath; // Atualiza o campo visual com o nome da DLL.

                    lblDllStatus.Text = "Dll Selecionada";
                    // Identifica e apresenta a arquitetura da DLL selecionada.
                    lblDllArchitecture.Text = "Arch: " + DllInjector.GetPEArchitecture(dllFileName);
                }
            }
        }

        // Obtém o processo selecionado pelo usuário ou registra o nome e ID
        // do processo que deverá ser pesquisado automaticamente.
        private void SelectProcessButton_Click(object sender, EventArgs e)
        {
            // Verifica se o modo de seleção manual está ativo
            // e se existe um processo selecionado no ComboBox.
            if (comboBoxProcesses.Visible && comboBoxProcesses.SelectedItem is Process selectedProcess)
            {
                PID = selectedProcess.Id; // Armazena o PID e o nome do processo.
                targetProcessName = selectedProcess.ProcessName;

                // Identifica a arquitetura do processo selecionado e Atualiza o status visual.
                lblProcessArchitecture.Text = "Arch: " + DllInjector.GetProcessArchitecture(selectedProcess);
                lblProcessStatus.ForeColor = Color.Green;
                lblProcessStatus.Text = "Processo Selecionado";
                return;
            }
            // Verifica se o modo de pesquisa automática está ativo
            // e se o usuário informou um nome de processo válido.
            if (txtProcessName.Visible && !String.IsNullOrWhiteSpace(txtProcessName.Text))
            {
                // Armazena o nome do processo que será procurado e  Atualiza o status visual.
                targetProcessName = txtProcessName.Text.Trim();
                lblProcessStatus.ForeColor = Color.Green;
                lblProcessStatus.Text = "Processo Selecionado";
            }
        }

        // Evento disparado quando o usuário abre a lista de processos.
        // Atualiza as informações e recarrega os processos disponíveis.
        private void ProcessComboBox_DropDown(object sender, EventArgs e)
        {
            // Limpa a seleção anterior.
            PID = 0;
            targetProcessName = "";
            lblProcessStatus.Text = "";
            lblProcessArchitecture.Text = "";
            RefreshProcessList(); // Atualiza a lista de processos.

        }

        // Evento disparado quando o usuário habilita ou desabilita
        // o modo de pesquisa automática.
        private void AutoInjectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            processSearchTimer.Stop(); // Interrompe qualquer pesquisa que esteja em andamento.
            // Limpa as informações do processo anterior.
            lblProcessStatus.Text = "";
            lblProcessArchitecture.Text = "";
            targetProcessName = "";
            PID = 0;
            // Atualiza os controles visuais de acordo com o modo selecionado.
            SetAutomaticSearchMode(chkAutoInject.Checked); 
        }

        // Evento disparado quando o usuário altera o método de carregamento selecionado.
        private void LoadMethodRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender; // Obtém o RadioButton que disparou o evento.

            if (!radioButton.Checked) return; // Ignora o evento quando o botão foi desmarcado.

            if (radioButton == rbtnLoadLibraryA) // Define LoadLibraryA como método selecionado.
            {
                selectedLoadMethod = LoadDll.LOAD_LIBRARY_A;
                return;
            }
            if (radioButton == rbtnLoadLibraryW) // Define LoadLibraryW como método selecionado.
            {
                selectedLoadMethod = LoadDll.LOAD_LIBRARY_W;
                return;
            }
            if (radioButton == rbtnLdrLoadDll) // Define LdrLoadDll como método selecionado.
            {
                selectedLoadMethod = LoadDll.LDR_LOAD_DLL;
                return;
            }

        }
    }
}

