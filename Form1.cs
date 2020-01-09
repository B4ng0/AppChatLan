using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace AppChat
{
    public partial class Chat : Form
    {
        Socket sck;
        EndPoint epLocal, epRomote;
        public Chat()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork , SocketType.Dgram , ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            TextLocalIP.Text = getLocalIp();
            TextClientIp.Text= getLocalIp();
        }
        private String getLocalIp()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if(ip.AddressFamily== AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }

            }
            return "127.0.0.1";
        }
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult , ref epRomote);
                if (size>0) {
                    byte[] receiveData = new byte[1464];
                    receiveData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receiveData);
                    ListMessages.Items.Add("Friend : "+receivedMessage);
                }
                byte[] Buffer = new byte[1500];
                sck.BeginReceiveFrom(Buffer , 0 , Buffer.Length , SocketFlags.None , ref epRomote , new AsyncCallback(MessageCallBack) , Buffer);
                

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(TextLocalIP.Text), Convert.ToInt32(TextLocalPort.Text));
                sck.Bind(epLocal);
                epRomote= new IPEndPoint(IPAddress.Parse(TextClientIp.Text), Convert.ToInt32(TextClientPort.Text));
                sck.Connect(epRomote);
                byte[] Buffer = new byte[1500];
                sck.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref epRomote, new AsyncCallback(MessageCallBack), Buffer);
                btnStart.Text = "Connected";
                btnStart.Enabled = false;
                btnSend.Enabled = true;
                TextMessage.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TextMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void Btn_hide_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Btn_minimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void Btn_maximize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void Btn_close_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.ExitThread();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg  = new byte[1500];
                msg = enc.GetBytes(TextMessage.Text);
                sck.Send(msg);
                ListMessages.Items.Add("Me :"+ TextMessage.Text);
                TextMessage.Clear();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
