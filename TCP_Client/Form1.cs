using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Client
{
    public partial class Form1 : Form
    {
        private string user_name;
        private string ip;
        private int port;
        private NetworkStream stream;
        private TcpClient client;
        public Form1()
        {
            InitializeComponent();
            client = new TcpClient();
            textBox1.Text = "127.0.0.1";
            textBox2.Text = "8080";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(client.Connected == false)
                client = new TcpClient();
            user_name = textBox3.Text;
            ip = textBox1.Text;
            int.TryParse(textBox2.Text, out port);

            try
            {
                client.Connect(ip, port);
                stream = client.GetStream();
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                richTextBox1.Text += "Вы вошли в чат" + Environment.NewLine;
            }
            catch
            {
                richTextBox1.Text += "Ошибка" + Environment.NewLine;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string message = textBox4.Text;
                textBox4.Text = "";
                byte[] data = Encoding.UTF8.GetBytes(user_name + "-> " + message + Environment.NewLine);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                richTextBox1.Text += "Вы не присоединились к чату" + Environment.NewLine;
            }
        }
        private void Disconnect()
        {
            stream.Close();
            client.Close();
        }
        private void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    string message = "";
                    byte[] data = new byte[256];
                    int actual_bytes = 0;
                    do
                    {
                        actual_bytes = stream.Read(data, 0, data.Length);
                        message += Encoding.UTF8.GetString(data, 0, actual_bytes);
                    }
                    while (stream.DataAvailable);
                    richTextBox1.Text += message + Environment.NewLine;
                }
                catch
                {
                    richTextBox1.Text += "Вы вышли из чата" + Environment.NewLine;
                    Disconnect();
                    break;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Disconnect();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }
    }
}
