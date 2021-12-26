
namespace KenwoodTCPServerGUI
{
    partial class KenwoodTCPServerUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KenwoodTCPServerUI));
            this.RadioIPAddress = new System.Windows.Forms.Label();
            this.RadioPort = new System.Windows.Forms.Label();
            this.UserName = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.Label();
            this.RadioIpAddressEntry = new System.Windows.Forms.TextBox();
            this.RadioPortEntry = new System.Windows.Forms.TextBox();
            this.UserNameEntry = new System.Windows.Forms.TextBox();
            this.PasswordEntry = new System.Windows.Forms.TextBox();
            this.AutoReconnectEntry = new System.Windows.Forms.CheckBox();
            this.Info = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.MinimizeOnStart = new System.Windows.Forms.CheckBox();
            this.Exit = new System.Windows.Forms.Button();
            this.TCPPortEntry = new System.Windows.Forms.TextBox();
            this.TCPPort = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.TcpSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CallSign = new System.Windows.Forms.TextBox();
            this.RadioTypeEntry = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.RadioComPorts = new System.Windows.Forms.ComboBox();
            this.RadioBaudRate = new System.Windows.Forms.ComboBox();
            this.ConnectivityMode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // RadioIPAddress
            // 
            this.RadioIPAddress.AutoSize = true;
            this.RadioIPAddress.Location = new System.Drawing.Point(255, 23);
            this.RadioIPAddress.Name = "RadioIPAddress";
            this.RadioIPAddress.Size = new System.Drawing.Size(89, 13);
            this.RadioIPAddress.TabIndex = 0;
            this.RadioIPAddress.Text = "Radio IP Address";
            // 
            // RadioPort
            // 
            this.RadioPort.AutoSize = true;
            this.RadioPort.Location = new System.Drawing.Point(255, 50);
            this.RadioPort.Name = "RadioPort";
            this.RadioPort.Size = new System.Drawing.Size(57, 13);
            this.RadioPort.TabIndex = 1;
            this.RadioPort.Text = "Radio Port";
            // 
            // UserName
            // 
            this.UserName.AutoSize = true;
            this.UserName.Location = new System.Drawing.Point(255, 77);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(60, 13);
            this.UserName.TabIndex = 2;
            this.UserName.Text = "User Name";
            // 
            // Password
            // 
            this.Password.AutoSize = true;
            this.Password.Location = new System.Drawing.Point(255, 104);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(53, 13);
            this.Password.TabIndex = 3;
            this.Password.Text = "Password";
            // 
            // RadioIpAddressEntry
            // 
            this.RadioIpAddressEntry.Location = new System.Drawing.Point(369, 20);
            this.RadioIpAddressEntry.Name = "RadioIpAddressEntry";
            this.RadioIpAddressEntry.Size = new System.Drawing.Size(116, 20);
            this.RadioIpAddressEntry.TabIndex = 5;
            // 
            // RadioPortEntry
            // 
            this.RadioPortEntry.Location = new System.Drawing.Point(369, 47);
            this.RadioPortEntry.Name = "RadioPortEntry";
            this.RadioPortEntry.Size = new System.Drawing.Size(116, 20);
            this.RadioPortEntry.TabIndex = 6;
            this.RadioPortEntry.Text = "60000";
            // 
            // UserNameEntry
            // 
            this.UserNameEntry.Location = new System.Drawing.Point(369, 74);
            this.UserNameEntry.Name = "UserNameEntry";
            this.UserNameEntry.Size = new System.Drawing.Size(116, 20);
            this.UserNameEntry.TabIndex = 7;
            // 
            // PasswordEntry
            // 
            this.PasswordEntry.Location = new System.Drawing.Point(369, 101);
            this.PasswordEntry.Name = "PasswordEntry";
            this.PasswordEntry.Size = new System.Drawing.Size(116, 20);
            this.PasswordEntry.TabIndex = 8;
            // 
            // AutoReconnectEntry
            // 
            this.AutoReconnectEntry.AutoSize = true;
            this.AutoReconnectEntry.Checked = true;
            this.AutoReconnectEntry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoReconnectEntry.Location = new System.Drawing.Point(618, 42);
            this.AutoReconnectEntry.Name = "AutoReconnectEntry";
            this.AutoReconnectEntry.Size = new System.Drawing.Size(104, 17);
            this.AutoReconnectEntry.TabIndex = 9;
            this.AutoReconnectEntry.Text = "Auto Reconnect";
            this.AutoReconnectEntry.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoReconnectEntry.UseVisualStyleBackColor = true;
            this.AutoReconnectEntry.CheckedChanged += new System.EventHandler(this.AutoReconnectEntry_CheckedChanged);
            // 
            // Info
            // 
            this.Info.AutoSize = true;
            this.Info.Location = new System.Drawing.Point(553, 234);
            this.Info.Name = "Info";
            this.Info.Size = new System.Drawing.Size(181, 13);
            this.Info.TabIndex = 10;
            this.Info.Text = "KENWOOD TCP Server by VU3ESV";
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(553, 256);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(45, 13);
            this.VersionLabel.TabIndex = 11;
            this.VersionLabel.Text = "Version:";
            // 
            // MinimizeOnStart
            // 
            this.MinimizeOnStart.AutoSize = true;
            this.MinimizeOnStart.Location = new System.Drawing.Point(618, 65);
            this.MinimizeOnStart.Name = "MinimizeOnStart";
            this.MinimizeOnStart.Size = new System.Drawing.Size(108, 17);
            this.MinimizeOnStart.TabIndex = 13;
            this.MinimizeOnStart.Text = "Minimize On Start";
            this.MinimizeOnStart.UseVisualStyleBackColor = true;
            this.MinimizeOnStart.CheckedChanged += new System.EventHandler(this.MinimizeOnStart_CheckedChanged);
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(618, 119);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(104, 20);
            this.Exit.TabIndex = 12;
            this.Exit.Text = "Exit";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // TCPPortEntry
            // 
            this.TCPPortEntry.Location = new System.Drawing.Point(369, 127);
            this.TCPPortEntry.Name = "TCPPortEntry";
            this.TCPPortEntry.Size = new System.Drawing.Size(116, 20);
            this.TCPPortEntry.TabIndex = 16;
            this.TCPPortEntry.Text = "7355";
            // 
            // TCPPort
            // 
            this.TCPPort.AutoSize = true;
            this.TCPPort.Location = new System.Drawing.Point(255, 130);
            this.TCPPort.Name = "TCPPort";
            this.TCPPort.Size = new System.Drawing.Size(50, 13);
            this.TCPPort.TabIndex = 15;
            this.TCPPort.Text = "TCP Port";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDoubleClick);
            // 
            // TcpSave
            // 
            this.TcpSave.Location = new System.Drawing.Point(618, 90);
            this.TcpSave.Name = "TcpSave";
            this.TcpSave.Size = new System.Drawing.Size(104, 20);
            this.TcpSave.TabIndex = 14;
            this.TcpSave.Text = "Save";
            this.TcpSave.UseVisualStyleBackColor = true;
            this.TcpSave.Click += new System.EventHandler(this.TcpSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(540, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Station";
            // 
            // CallSign
            // 
            this.CallSign.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.CallSign.Location = new System.Drawing.Point(618, 16);
            this.CallSign.Name = "CallSign";
            this.CallSign.Size = new System.Drawing.Size(100, 20);
            this.CallSign.TabIndex = 18;
            this.CallSign.Text = "CALLSIGN";
            // 
            // RadioTypeEntry
            // 
            this.RadioTypeEntry.FormattingEnabled = true;
            this.RadioTypeEntry.Items.AddRange(new object[] {
            "TS990",
            "TS890"});
            this.RadioTypeEntry.Location = new System.Drawing.Point(369, 155);
            this.RadioTypeEntry.Name = "RadioTypeEntry";
            this.RadioTypeEntry.Size = new System.Drawing.Size(116, 21);
            this.RadioTypeEntry.TabIndex = 19;
            this.RadioTypeEntry.SelectedIndexChanged += new System.EventHandler(this.RadioTypeEntry_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "RadioType";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::KenwoodTCPServerGUI.Properties.Resources.KenwoodIcon;
            this.pictureBox1.Location = new System.Drawing.Point(248, 219);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(261, 54);
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Baud Rate";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Radio ComPort";
            // 
            // RadioComPorts
            // 
            this.RadioComPorts.FormattingEnabled = true;
            this.RadioComPorts.Location = new System.Drawing.Point(95, 24);
            this.RadioComPorts.Name = "RadioComPorts";
            this.RadioComPorts.Size = new System.Drawing.Size(116, 21);
            this.RadioComPorts.TabIndex = 24;
            this.RadioComPorts.SelectedIndexChanged += new System.EventHandler(this.RadioComPorts_SelectedIndexChanged);
            // 
            // RadioBaudRate
            // 
            this.RadioBaudRate.FormattingEnabled = true;
            this.RadioBaudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "56000",
            "57600",
            "115200",
            "128000",
            "256000"});
            this.RadioBaudRate.Location = new System.Drawing.Point(95, 51);
            this.RadioBaudRate.Name = "RadioBaudRate";
            this.RadioBaudRate.Size = new System.Drawing.Size(116, 21);
            this.RadioBaudRate.TabIndex = 25;
            this.RadioBaudRate.SelectedIndexChanged += new System.EventHandler(this.RadioBaudRate_SelectedIndexChanged);
            // 
            // ConnectivityMode
            // 
            this.ConnectivityMode.FormattingEnabled = true;
            this.ConnectivityMode.Items.AddRange(new object[] {
            "Serial",
            "LAN"});
            this.ConnectivityMode.Location = new System.Drawing.Point(95, 162);
            this.ConnectivityMode.Name = "ConnectivityMode";
            this.ConnectivityMode.Size = new System.Drawing.Size(116, 21);
            this.ConnectivityMode.TabIndex = 26;
            this.ConnectivityMode.SelectedIndexChanged += new System.EventHandler(this.ConnectivityMode_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Connectivity";
            // 
            // KenwoodTCPServerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 276);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ConnectivityMode);
            this.Controls.Add(this.RadioBaudRate);
            this.Controls.Add(this.RadioComPorts);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RadioTypeEntry);
            this.Controls.Add(this.CallSign);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TCPPortEntry);
            this.Controls.Add(this.TCPPort);
            this.Controls.Add(this.TcpSave);
            this.Controls.Add(this.MinimizeOnStart);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.Info);
            this.Controls.Add(this.AutoReconnectEntry);
            this.Controls.Add(this.PasswordEntry);
            this.Controls.Add(this.UserNameEntry);
            this.Controls.Add(this.RadioPortEntry);
            this.Controls.Add(this.RadioIpAddressEntry);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.UserName);
            this.Controls.Add(this.RadioPort);
            this.Controls.Add(this.RadioIPAddress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KenwoodTCPServerUI";
            this.Text = "KENWOOD TCPServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label RadioIPAddress;
        private System.Windows.Forms.Label RadioPort;
        private System.Windows.Forms.Label UserName;
        private System.Windows.Forms.Label Password;
        private System.Windows.Forms.TextBox RadioIpAddressEntry;
        private System.Windows.Forms.TextBox RadioPortEntry;
        private System.Windows.Forms.TextBox UserNameEntry;
        private System.Windows.Forms.TextBox PasswordEntry;
        private System.Windows.Forms.CheckBox AutoReconnectEntry;
        private System.Windows.Forms.Label Info;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.CheckBox MinimizeOnStart;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.TextBox TCPPortEntry;
        private System.Windows.Forms.Label TCPPort;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button TcpSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CallSign;
        private System.Windows.Forms.ComboBox RadioTypeEntry;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox RadioComPorts;
        private System.Windows.Forms.ComboBox RadioBaudRate;
        private System.Windows.Forms.ComboBox ConnectivityMode;
        private System.Windows.Forms.Label label3;
    }
}

