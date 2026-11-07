namespace Rascunho_Injetor_Dll
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSelectDll = new Button();
            txtSelectedDll = new TextBox();
            comboBoxProcesses = new ComboBox();
            btnSelectProcess = new Button();
            lblDllStatus = new Label();
            lblProcessStatus = new Label();
            btnInject = new Button();
            lblDllArchitecture = new Label();
            lblProcessArchitecture = new Label();
            rtbLogs = new RichTextBox();
            chkAutoInject = new CheckBox();
            txtProcessName = new TextBox();
            btnStartAutoInject = new Button();
            btnStopSearch = new Button();
            rbtnLoadLibraryA = new RadioButton();
            rbtnLoadLibraryW = new RadioButton();
            rbtnLdrLoadDll = new RadioButton();
            grpLoadingApis = new GroupBox();
            chkUnhook = new CheckBox();
            chkCloseAutomatically = new CheckBox();
            grpLoadingApis.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectDll
            // 
            btnSelectDll.Location = new Point(25, 25);
            btnSelectDll.Margin = new Padding(3, 1, 3, 1);
            btnSelectDll.Name = "btnSelectDll";
            btnSelectDll.Size = new Size(108, 23);
            btnSelectDll.TabIndex = 0;
            btnSelectDll.Text = "Selecionar Dll";
            btnSelectDll.UseVisualStyleBackColor = true;
            btnSelectDll.Click += SelectDllButton_Click;
            // 
            // txtSelectedDll
            // 
            txtSelectedDll.Enabled = false;
            txtSelectedDll.Location = new Point(155, 29);
            txtSelectedDll.Margin = new Padding(3, 1, 3, 1);
            txtSelectedDll.Name = "txtSelectedDll";
            txtSelectedDll.Size = new Size(306, 23);
            txtSelectedDll.TabIndex = 1;
            // 
            // comboBoxProcesses
            // 
            comboBoxProcesses.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxProcesses.FormattingEnabled = true;
            comboBoxProcesses.Location = new Point(153, 79);
            comboBoxProcesses.Margin = new Padding(1);
            comboBoxProcesses.Name = "comboBoxProcesses";
            comboBoxProcesses.Size = new Size(309, 23);
            comboBoxProcesses.TabIndex = 2;
            comboBoxProcesses.DropDown += ProcessComboBox_DropDown;
            // 
            // btnSelectProcess
            // 
            btnSelectProcess.Location = new Point(18, 79);
            btnSelectProcess.Margin = new Padding(1);
            btnSelectProcess.Name = "btnSelectProcess";
            btnSelectProcess.Size = new Size(125, 23);
            btnSelectProcess.TabIndex = 3;
            btnSelectProcess.Text = "Selecionar Processo";
            btnSelectProcess.UseVisualStyleBackColor = true;
            btnSelectProcess.Click += SelectProcessButton_Click;
            // 
            // lblDllStatus
            // 
            lblDllStatus.AutoSize = true;
            lblDllStatus.BackColor = Color.White;
            lblDllStatus.ForeColor = Color.Green;
            lblDllStatus.Location = new Point(251, 11);
            lblDllStatus.Margin = new Padding(1, 0, 1, 0);
            lblDllStatus.Name = "lblDllStatus";
            lblDllStatus.Size = new Size(0, 15);
            lblDllStatus.TabIndex = 4;
            // 
            // lblProcessStatus
            // 
            lblProcessStatus.AutoSize = true;
            lblProcessStatus.BackColor = Color.White;
            lblProcessStatus.ForeColor = Color.Green;
            lblProcessStatus.Location = new Point(206, 63);
            lblProcessStatus.Margin = new Padding(1, 0, 1, 0);
            lblProcessStatus.Name = "lblProcessStatus";
            lblProcessStatus.Size = new Size(0, 15);
            lblProcessStatus.TabIndex = 5;
            // 
            // btnInject
            // 
            btnInject.Location = new Point(251, 166);
            btnInject.Margin = new Padding(1);
            btnInject.Name = "btnInject";
            btnInject.Size = new Size(78, 26);
            btnInject.TabIndex = 6;
            btnInject.Text = "Injetar";
            btnInject.UseVisualStyleBackColor = true;
            btnInject.Click += InjectButton_Click;
            // 
            // lblDllArchitecture
            // 
            lblDllArchitecture.AutoSize = true;
            lblDllArchitecture.Location = new Point(481, 33);
            lblDllArchitecture.Name = "lblDllArchitecture";
            lblDllArchitecture.Size = new Size(0, 15);
            lblDllArchitecture.TabIndex = 11;
            // 
            // lblProcessArchitecture
            // 
            lblProcessArchitecture.AutoSize = true;
            lblProcessArchitecture.Location = new Point(479, 86);
            lblProcessArchitecture.Name = "lblProcessArchitecture";
            lblProcessArchitecture.Size = new Size(0, 15);
            lblProcessArchitecture.TabIndex = 12;
            // 
            // rtbLogs
            // 
            rtbLogs.Location = new Point(10, 288);
            rtbLogs.Name = "rtbLogs";
            rtbLogs.ReadOnly = true;
            rtbLogs.Size = new Size(532, 170);
            rtbLogs.TabIndex = 13;
            rtbLogs.Text = "";
            // 
            // chkAutoInject
            // 
            chkAutoInject.AutoSize = true;
            chkAutoInject.Location = new Point(196, 112);
            chkAutoInject.Margin = new Padding(3, 2, 3, 2);
            chkAutoInject.Name = "chkAutoInject";
            chkAutoInject.Size = new Size(211, 19);
            chkAutoInject.TabIndex = 16;
            chkAutoInject.Text = "Auto Injetar ao Encotrar o Processo";
            chkAutoInject.UseVisualStyleBackColor = true;
            chkAutoInject.CheckedChanged += AutoInjectCheckBox_CheckedChanged;
            // 
            // txtProcessName
            // 
            txtProcessName.Location = new Point(153, 80);
            txtProcessName.Margin = new Padding(3, 2, 3, 2);
            txtProcessName.Name = "txtProcessName";
            txtProcessName.PlaceholderText = "Digite o nome do processo...";
            txtProcessName.Size = new Size(288, 23);
            txtProcessName.TabIndex = 17;
            txtProcessName.Visible = false;
            // 
            // btnStartAutoInject
            // 
            btnStartAutoInject.Location = new Point(232, 166);
            btnStartAutoInject.Margin = new Padding(3, 2, 3, 2);
            btnStartAutoInject.Name = "btnStartAutoInject";
            btnStartAutoInject.Size = new Size(115, 26);
            btnStartAutoInject.TabIndex = 18;
            btnStartAutoInject.Text = "Auto Injetar";
            btnStartAutoInject.UseVisualStyleBackColor = true;
            btnStartAutoInject.Visible = false;
            btnStartAutoInject.Click += AutoInjectButton_Click;
            // 
            // btnStopSearch
            // 
            btnStopSearch.BackColor = Color.Red;
            btnStopSearch.Location = new Point(232, 166);
            btnStopSearch.Margin = new Padding(3, 2, 3, 2);
            btnStopSearch.Name = "btnStopSearch";
            btnStopSearch.Size = new Size(115, 26);
            btnStopSearch.TabIndex = 19;
            btnStopSearch.Text = "Cancelar Procura";
            btnStopSearch.UseVisualStyleBackColor = false;
            btnStopSearch.Visible = false;
            btnStopSearch.Click += StopSearchingButton_Click;
            // 
            // rbtnLoadLibraryA
            // 
            rbtnLoadLibraryA.AutoSize = true;
            rbtnLoadLibraryA.Location = new Point(12, 32);
            rbtnLoadLibraryA.Margin = new Padding(3, 2, 3, 2);
            rbtnLoadLibraryA.Name = "rbtnLoadLibraryA";
            rbtnLoadLibraryA.Size = new Size(95, 19);
            rbtnLoadLibraryA.TabIndex = 0;
            rbtnLoadLibraryA.TabStop = true;
            rbtnLoadLibraryA.Text = "LoadLibraryA";
            rbtnLoadLibraryA.UseVisualStyleBackColor = true;
            rbtnLoadLibraryA.CheckedChanged += LoadMethodRadioButton_CheckedChanged;
            // 
            // rbtnLoadLibraryW
            // 
            rbtnLoadLibraryW.AutoSize = true;
            rbtnLoadLibraryW.Location = new Point(12, 62);
            rbtnLoadLibraryW.Margin = new Padding(3, 2, 3, 2);
            rbtnLoadLibraryW.Name = "rbtnLoadLibraryW";
            rbtnLoadLibraryW.Size = new Size(98, 19);
            rbtnLoadLibraryW.TabIndex = 1;
            rbtnLoadLibraryW.TabStop = true;
            rbtnLoadLibraryW.Text = "LoadLibraryW";
            rbtnLoadLibraryW.UseVisualStyleBackColor = true;
            rbtnLoadLibraryW.CheckedChanged += LoadMethodRadioButton_CheckedChanged;
            // 
            // rbtnLdrLoadDll
            // 
            rbtnLdrLoadDll.AutoSize = true;
            rbtnLdrLoadDll.Location = new Point(12, 92);
            rbtnLdrLoadDll.Margin = new Padding(3, 2, 3, 2);
            rbtnLdrLoadDll.Name = "rbtnLdrLoadDll";
            rbtnLdrLoadDll.Size = new Size(82, 19);
            rbtnLdrLoadDll.TabIndex = 2;
            rbtnLdrLoadDll.TabStop = true;
            rbtnLdrLoadDll.Text = "LdrLoadDll";
            rbtnLdrLoadDll.UseVisualStyleBackColor = true;
            rbtnLdrLoadDll.CheckedChanged += LoadMethodRadioButton_CheckedChanged;
            // 
            // grpLoadingApis
            // 
            grpLoadingApis.Controls.Add(chkUnhook);
            grpLoadingApis.Controls.Add(rbtnLoadLibraryA);
            grpLoadingApis.Controls.Add(rbtnLdrLoadDll);
            grpLoadingApis.Controls.Add(rbtnLoadLibraryW);
            grpLoadingApis.Location = new Point(6, 112);
            grpLoadingApis.Margin = new Padding(3, 2, 3, 2);
            grpLoadingApis.Name = "grpLoadingApis";
            grpLoadingApis.Padding = new Padding(3, 2, 3, 2);
            grpLoadingApis.Size = new Size(177, 170);
            grpLoadingApis.TabIndex = 20;
            grpLoadingApis.TabStop = false;
            grpLoadingApis.Text = "APIs de Carregamento";
            // 
            // chkUnhook
            // 
            chkUnhook.AutoSize = true;
            chkUnhook.Location = new Point(38, 127);
            chkUnhook.Margin = new Padding(3, 2, 3, 2);
            chkUnhook.Name = "chkUnhook";
            chkUnhook.Size = new Size(70, 19);
            chkUnhook.TabIndex = 3;
            chkUnhook.Text = "UnHook";
            chkUnhook.UseVisualStyleBackColor = true;
            // 
            // chkCloseAutomatically
            // 
            chkCloseAutomatically.AutoSize = true;
            chkCloseAutomatically.Location = new Point(196, 143);
            chkCloseAutomatically.Margin = new Padding(3, 2, 3, 2);
            chkCloseAutomatically.Name = "chkCloseAutomatically";
            chkCloseAutomatically.Size = new Size(212, 19);
            chkCloseAutomatically.TabIndex = 21;
            chkCloseAutomatically.Text = "Fechar Automaticamente ao injetar";
            chkCloseAutomatically.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.Disable;
            BackColor = SystemColors.ButtonFace;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(564, 470);
            Controls.Add(chkCloseAutomatically);
            Controls.Add(grpLoadingApis);
            Controls.Add(btnStopSearch);
            Controls.Add(btnStartAutoInject);
            Controls.Add(txtProcessName);
            Controls.Add(chkAutoInject);
            Controls.Add(rtbLogs);
            Controls.Add(lblProcessArchitecture);
            Controls.Add(lblDllArchitecture);
            Controls.Add(btnInject);
            Controls.Add(lblProcessStatus);
            Controls.Add(lblDllStatus);
            Controls.Add(btnSelectProcess);
            Controls.Add(comboBoxProcesses);
            Controls.Add(txtSelectedDll);
            Controls.Add(btnSelectDll);
            ForeColor = SystemColors.ActiveCaptionText;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(3, 1, 3, 1);
            Name = "Form1";
            Text = "Form1";
            grpLoadingApis.ResumeLayout(false);
            grpLoadingApis.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnSelectDll;
        private TextBox txtSelectedDll;
        private ComboBox comboBoxProcesses;
        private Button btnSelectProcess;
        private Label lblDllStatus;
        private Label lblProcessStatus;
        private Button btnInject;
        private Label lblDllArchitecture;
        private Label lblProcessArchitecture;
        private RichTextBox rtbLogs;
        private CheckBox chkAutoInject;
        private TextBox txtProcessName;
        private Button btnStartAutoInject;
        private Button btnStopSearch;
        private RadioButton rbtnLoadLibraryA;
        private RadioButton rbtnLoadLibraryW;
        private RadioButton rbtnLdrLoadDll;
        private GroupBox grpLoadingApis;
        private CheckBox chkCloseAutomatically;
        private CheckBox chkUnhook;
    }
}
