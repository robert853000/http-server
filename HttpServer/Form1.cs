using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HttpServer
{

    public partial class frmMain : Form, IfrmMain


    {

        public void AddMessageMethod(string methodstr)
        {
            OnMessage = methodstr;


        }
        public delegate void AddMessage(string methodstr);
        public AddMessage myDelegate;

        public string OnMessage
        {
            get { return txtlog.Text; }

            set {
                int lineCount = txtlog.Text.Split(new[] { '\n', 'r' }, StringSplitOptions.None).Length;
                //MessageBox.Show(lineCount.ToString());
                if (lineCount > 54)
                {
                    txtlog.Text = "";
                }
                txtlog.AppendText(value);
           
           }
        }
        Server server;
        public static frmMain _frmMain;

        public frmMain()
        {
            InitializeComponent();
            _frmMain = this;
            myDelegate = new AddMessage(AddMessageMethod);
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadSettings();


        }
        private void StartStop()
        {
            if (server == null || server.IsServerRunning == false)
            {

                try
                {
                    server = new Server(Properties.Settings.Default.ServerIP, Properties.Settings.Default.ServerPort, Properties.Settings.Default.GetUrl, Properties.Settings.Default.PostUrl, this);

                    server.Start();
                    OnMessage = "Server started: ";
                    OnMessage = "http://" + Properties.Settings.Default.ServerIP + ":" + Properties.Settings.Default.ServerPort + "/" + Properties.Settings.Default.GetUrl + "\r\n";



                }

                catch (Exception ex)
                {
                    OnMessage = "Error starting server: " + ex.Message + "\r\n";
                }
            }

            else
            {
                try
                {
                    server.Stop();
                    OnMessage = "Server stopped" + "\r\n";
                }

                catch (Exception ex)
                {
                    OnMessage = "Error stopping server: " + ex.Message + "\r\n";
                }

            }




        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            StartStop();
        }
        
        private void LoadSettings()
        {
            try
            { 
            txtPort.Text = Properties.Settings.Default.ServerPort.ToString();
            txtGetURL.Text = Properties.Settings.Default.GetUrl.ToString();
            txtPostURL.Text = Properties.Settings.Default.PostUrl.ToString();
            txtIP.Text = Properties.Settings.Default.ServerIP.ToString();
                txtlog.AppendText("Settings loaded." + "\r\n");
            }
            catch (Exception ex)
            {
                txtlog.AppendText("Error loading settings: " + ex.Message + "\r\n");
            }
            StartStop();
        }

        private void SaveSettings()
        {
            try
            {
                Properties.Settings.Default.ServerPort = int.Parse(txtPort.Text);
                Properties.Settings.Default.GetUrl = txtGetURL.Text;
                Properties.Settings.Default.PostUrl = txtPostURL.Text;
                Properties.Settings.Default.ServerIP = txtIP.Text;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                txtlog.AppendText("Settings saved." + "\r\n");
            }
            catch(Exception ex)
            {
                txtlog.AppendText("Error saving settings: " + ex.Message + "\r\n");
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            SaveSettings(); 
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (server == null || server.IsServerRunning == false)

                btnStartStop.Text = "Start Server";
            else
                btnStartStop.Text = "Stop Server";

        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
             if (server != null)
            {
                if (server.IsServerRunning)
                    server.Stop();

                server = null;

            }
        }

        private void txtlog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
