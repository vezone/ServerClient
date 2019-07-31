using System;
using CommonApp;
using System.Linq;
using WebSocket4Net;
using System.Windows.Forms;
using System.Drawing;
using SuperSocket.ClientEngine;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        public static WebSocket m_Socket;
        public static Client client;
        ClientRequest request = new ClientRequest
        {
            Operation = OperationType.PostMessage,
            Client = client
        };

        public Form1()
        {
            InitializeComponent();
            
            client = new Client();
            client.Nickname = System.Configuration.ConfigurationManager.AppSettings["Nickname"];

            string server_string = $"ws://127.0.0.1:1000";
            m_Socket = new WebSocket(server_string);
            m_Socket.Opened +=
                (object sender, EventArgs eventArgs) =>
                {
                    richTextBox1.AppendText($"Подсоединение к серверу {server_string} как {client.Nickname}\n");
                    ClientRequest request = new ClientRequest
                    {
                        Operation = OperationType.Login,
                        Client = client
                    };

                    richTextBox1.AppendText($"Логин на сервере как {client.Nickname}\n");
                    m_Socket.Send(request.ToJson());
                    richTextBox1.AppendText("Логин успешно выполнен.\n");
                };
            m_Socket.MessageReceived +=
                (object sender, MessageReceivedEventArgs messageReceivedEventArgs) =>
                {
                    ServerResponse response = messageReceivedEventArgs.Message.FromJson<ServerResponse>();

                    if (response.ResponseType == ResponseType.Message)
                    {
                        richTextBox1.AppendText(response.Message + "\n");
                    }
                    else if (response.ResponseType == ResponseType.Login)
                    {
                        richTextBox1.AppendText(response.Message + "\n");
                    }
                    else if (response.ResponseType == ResponseType.Logout)
                    {
                        richTextBox1.AppendText(response.Message + "\n");
                    }
                    else
                    {
                        richTextBox1.AppendText("ELSE!\n");
                    }
                };
            m_Socket.Error += (object sender, ErrorEventArgs error) =>
            {
                richTextBox1.AppendText("Error: " + error.Exception.Message + "\n");
            };

            m_Socket.Closed += (object sender, EventArgs eventArgs) =>
            {
                richTextBox1.AppendText("Socket closed\n");
            };
            m_Socket.Open();
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Message = textBox1.Text;
            request.Client = client;
            request.Operation = OperationType.PostMessage;
            if (client.Message.Length > 0)
            {
                richTextBox1.AppendText($"Me: {textBox1.Text}\n");
                m_Socket.Send(request.ToJson());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
