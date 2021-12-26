using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
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
        public string version = "1.1.0";
        private const string loopbackIP = "127.0.0.1";
        private const string defaultAdmin = "admin";
        private const string defaultPassword = "Kenwood";
        private const int defaultBaudRate = 9600;
        private const string defaultConnectivity = "Serial";

        public KenwoodTCPServerUI()
        {
            InitializeComponent();
            var serialPortNames = SerialPort.GetPortNames();
            int radioPort = 0;
            int tcpPort = 0;
            string serialPort = string.Empty;
            int baudRate = 0;
            string selectedConnectivity = string.Empty;
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

                if (string.IsNullOrEmpty(GetValue("RadioBaudRate", "Value")))
                {
                    WritePrivateProfileString("RadioBaudRate", "Value", defaultBaudRate.ToString(), this.iniFile);
                }

                if (string.IsNullOrEmpty(GetValue("RadioConnectivity", "Value")))
                {
                    WritePrivateProfileString("RadioConnectivity", "Value", defaultConnectivity.ToString(), this.iniFile);
                }

                minimizeOnStart = Convert.ToBoolean(string.IsNullOrEmpty(GetValue("MinimizeOnStart", "Value"))
                    ? "false" : (GetValue("MinimizeOnStart", "Value")));
                autoReConnect = Convert.ToBoolean(string.IsNullOrEmpty(GetValue("AutoReconnect", "Value"))
                    ? "false" : (GetValue("AutoReconnect", "Value")));

                callSign = GetValue("CallSign", "Value");

                tcpPort = Convert.ToInt32(GetValue("TCPPort", "Port"));
                radioPort = Convert.ToInt32(GetValue("RadioPort", "Port"));
                radioIpAddress = GetValue("RadioIP", "IP");
                userName = GetValue("UserName", "Value");
                password = GetValue("Password", "Value");
                radioType = GetValue("RadioType", "Value");
                serialPort = GetValue("RadioSerialPort", "Value");
                baudRate = Convert.ToInt32(GetValue("RadioBaudRate", "Value"));
                selectedConnectivity = GetValue("RadioConnectivity", "Value");
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
            if (serialPort == string.Empty)
            {
                RadioComPorts.Items.AddRange(serialPortNames);
            }
            else
            {
                RadioComPorts.Items.AddRange(serialPortNames);
                RadioComPorts.SelectedIndex = RadioComPorts.Items.IndexOf(serialPort);
            }

            if (baudRate == 0)
            {
                RadioBaudRate.SelectedIndex = RadioBaudRate.Items.IndexOf(defaultBaudRate);
            }
            else
            {
                RadioBaudRate.SelectedIndex = RadioBaudRate.Items.IndexOf(baudRate.ToString());
            }

            if (selectedConnectivity == string.Empty)
            {
                ConnectivityMode.SelectedIndex = ConnectivityMode.Items.IndexOf(defaultConnectivity);
                selectedConnectivity = defaultConnectivity;
            }
            else
            {
                ConnectivityMode.SelectedIndex = ConnectivityMode.Items.IndexOf(selectedConnectivity);
            }

            if (selectedConnectivity == defaultConnectivity)
            {
                if (RadioComPorts.SelectedIndex != -1)
                {
                    Task.Run(async () =>
                    {
                        Kenwood.RadioPort radioPrt = new Kenwood.RadioPort()
                        {
                            Comport = serialPort,
                            BaudRate = baudRate,
                            DataBits = 8,
                            DTR = "High",
                            RTS = "High",
                            Handshake = Handshake.RequestToSend,
                            Parity = Parity.None,
                            StopBits = StopBits.One,
                            ReadTimeout = 500,
                            WriteTimeout = 500
                        };


                        _kenwoodTcpServer = KenwoodSerialTcpServer.Create(radioPrt,
                                                                          radioType == "TS990" ? Kenwood.RadioType.TS990
                                                                                               : Kenwood.RadioType.TS890,
                                                                          Convert.ToInt32(TCPPortEntry.Text));
                        await _kenwoodTcpServer.InitializeAsync();
                    });
                }
            }
            else
            {
                if (RadioIpAddressEntry.Text != loopbackIP)
                {
                    Task.Run(async () =>
                    {

                        _kenwoodTcpServer = KenwoodTcpServer.Create(RadioIpAddressEntry.Text,
                                                                    radioPort,
                                                                    UserNameEntry.Text,
                                                                    PasswordEntry.Text,
                                                                    radioType == "TS990" ? Kenwood.RadioType.TS990
                                                                                         : Kenwood.RadioType.TS890,
                                                                    Convert.ToInt32(TCPPortEntry.Text));
                        await _kenwoodTcpServer.InitializeAsync();
                    });
                }

            }


        }

        [DllImport("kernel32", SetLastError = true)]
        private static extern int WritePrivateProfileString(
                                  string section,
                                  string key,
                                  string value,
                                  string fileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
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
                WritePrivateProfileString("CallSign", "Value", CallSign.Text, this.iniFile);
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
            var autoReconnect = sender as CheckBox;
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("AutoReconnect", "Value", AutoReconnectEntry.Checked.ToString(), this.iniFile);
            }

            if (autoReconnect.Checked)
            {
                _kenwoodTcpServer?.EnableAutoReConnect(true);
            }
            else
            {
                _kenwoodTcpServer?.EnableAutoReConnect(false);
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

        private void RadioComPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var radioComport = sender as ComboBox;
            var selectedPort = radioComport.SelectedItem.ToString();
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("RadioSerialPort", "Value", selectedPort, this.iniFile);
            }
        }

        private void RadioBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var radioBaudRate = sender as ComboBox;
            var selectedBaud = radioBaudRate.SelectedItem.ToString();
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("RadioBaudRate", "Value", selectedBaud, this.iniFile);
            }
        }

        private void ConnectivityMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            var connectivity = sender as ComboBox;
            var selectedConnectivity = connectivity.SelectedItem.ToString();
            if (File.Exists(iniFile))
            {
                WritePrivateProfileString("RadioConnectivity", "Value", selectedConnectivity, this.iniFile);
            }
        }
    }
}
