using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using KenwoodTCP;

namespace KenwoodTCPServerGUI
{
    public partial class KenwoodTCPServerUI : Form
    {

        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        private IKenwoodTcpServer _kenwoodTcpServer;
        private readonly string iniFile = Path.GetDirectoryName(Application.ExecutablePath) + "\\KenwoodTCPServer.ini";
        public string version = "1.0.0";
        private const string loopbackIP = "127.0.0.1";
        private const string defaultAdmin = "admin";
        private const string defaultPassword = "Kenwood";

        public KenwoodTCPServerUI()
        {
            InitializeComponent();
            int radioPort = 0;
            int tcpPort = 0;
            string radioIpAddress = string.Empty;
            bool minimizeOnStart = false;
            bool autoReConnect = false;
            string callSign = string.Empty;
            string userName = string.Empty;
            string password = string.Empty;
            string radioType = string.Empty;
            VersionLabel.Text = VersionLabel.Text + this.version;

            WritePrivateProfileString("Kenwood TCPServer Information", "Kenwood TCPServer Version", this.version, this.iniFile);

            if (File.Exists(iniFile))
            {
                if (string.IsNullOrEmpty(GetValue("RadioIP", "IP")))
                {
                    WritePrivateProfileString("RadioIP", "IP", RadioIpAddressEntry.Text, this.iniFile);
                }

                if (string.IsNullOrEmpty(GetValue("RadioPort", "Port")))
                {
                    WritePrivateProfileString("RadioPort", "Port", RadioPortEntry.Text, this.iniFile);
                }

                if (string.IsNullOrEmpty(GetValue("TCPPort", "Port")))
                {
                    WritePrivateProfileString("TCPPort", "Port", TCPPortEntry.Text, this.iniFile);
                }

                if (string.IsNullOrEmpty(GetValue("UserName", "Value")))
                {
                    WritePrivateProfileString("UserName", "Value", UserNameEntry.Text, this.iniFile);
                }

                if (string.IsNullOrEmpty(GetValue("Password", "Value")))
                {
                    WritePrivateProfileString("Password", "Value", PasswordEntry.Text, this.iniFile);
                }

                minimizeOnStart = Convert.ToBoolean(string.IsNullOrEmpty(GetValue("MinimizeOnStart", "Value"))
                    ? "false" : (GetValue("MinimizeOnStart", "Value")));
                autoReConnect = Convert.ToBoolean(string.IsNullOrEmpty(GetValue("AutoReconnect", "Value"))
                    ? "false" : (GetValue("AutoReconnect", "Value")));

                callSign = (GetValue("CallSign", "Value"));

                tcpPort = Convert.ToInt32(GetValue("TCPPort", "Port"));
                radioPort = Convert.ToInt32(GetValue("RadioPort", "Port"));
                radioIpAddress = GetValue("RadioIP", "IP");
                userName = GetValue("UserName", "Value");
                password = GetValue("Password", "Value");
                radioType = GetValue("RadioType", "Value");
            }

            RadioIpAddressEntry.Text = radioIpAddress == string.Empty ? loopbackIP : radioIpAddress;
            RadioPortEntry.Text = radioPort == 0 ? "60000" : radioPort.ToString();
            TCPPortEntry.Text = tcpPort == 0 ? "7355" : tcpPort.ToString();
            UserNameEntry.Text = userName == string.Empty ? defaultAdmin : userName;
            PasswordEntry.Text = password == string.Empty ? defaultPassword : password;
            RadioTypeEntry.SelectedIndex = radioType == "TS990" ? 0 : 1;
            MinimizeOnStart.Checked = minimizeOnStart;
            AutoReconnectEntry.Checked = autoReConnect;
            CallSign.Text = !string.IsNullOrEmpty(callSign) ? callSign : CallSign.Text;

            if (RadioIpAddressEntry.Text != loopbackIP)
            {
                Task.Run(async () =>
                {
                    _kenwoodTcpServer = KenwoodTcpServer.Create(RadioIpAddressEntry.Text,
                    radioPort,
                    UserNameEntry.Text,
                    PasswordEntry.Text,
                    Kenwood.RadioType.TS990,
                    Convert.ToInt32(TCPPortEntry.Text));

                    await _kenwoodTcpServer.InitializeAsync();
                });
            }
        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern int WritePrivateProfileString(
                                  string section,
                                  string key,
                                  string value,
                                  string fileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
          string section,
          string key,
          string defaultValue,
          StringBuilder result,
          int size,
          string fileName);

        public string GetValue(string section, string entry)
        {
            StringBuilder result = new StringBuilder(1000);
            GetPrivateProfileString(section, entry, "", result, 1000, this.iniFile);
            return result.ToString();
        }

        private void TcpSave_Click(object sender, EventArgs e)
        {
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("RadioIP", "IP", RadioIpAddressEntry.Text, this.iniFile);
                WritePrivateProfileString("RadioPort ", "Port", RadioPortEntry.Text, this.iniFile);
                WritePrivateProfileString("TCPPort", "Port", TCPPortEntry.Text, this.iniFile);
                WritePrivateProfileString("UserName", "Value", UserNameEntry.Text, this.iniFile);
                WritePrivateProfileString("Password", "Value", PasswordEntry.Text, this.iniFile);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            notifyIcon1.Visible = true;
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
            }
        }

        private void MinimizeOnStart_CheckedChanged(object sender, EventArgs e)
        {
            var minimizeOnStart = sender as CheckBox;
            if (minimizeOnStart.Checked == true)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("MinimizeOnStart", "Value", MinimizeOnStart.Checked.ToString(), this.iniFile);
            }
        }

        private void AutoReconnectEntry_CheckedChanged(object sender, EventArgs e)
        {
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("AutoReconnect", "Value", AutoReconnectEntry.Checked.ToString(), this.iniFile);
            }
        }

        private void RadioTypeEntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            var radioSelection = sender as ComboBox;
            var selectedRadio = radioSelection.SelectedItem.ToString();
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("RadioType", "Value", selectedRadio, this.iniFile);
            }
        }
    }
}
