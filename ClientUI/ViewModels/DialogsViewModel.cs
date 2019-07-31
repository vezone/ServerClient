using CommonApp;
using SuperSocket.ClientEngine;
using System;
using WebSocket4Net;

namespace ClientUI.ViewModels
{
    class DialogsViewModel : ViewModelBase
    {
        RelayCommand SendMessageLogic;
        string m_MessagesView;
        string m_InputBoxText;

        public static WebSocket m_Socket;
        public static Client client;
        ClientRequest request = new ClientRequest
        {
            Operation = OperationType.PostMessage,
            Client = client
        };

        public DialogsViewModel()
        {

            client = new Client();
            client.Nickname = System.Configuration.ConfigurationManager.AppSettings["Nickname"];

            string server_string = $"ws://127.0.0.1:1000";
            m_Socket = new WebSocket(server_string);
            m_Socket.Opened +=
                (object sender, EventArgs eventArgs) =>
                {
                    MessagesView += $"Подсоединение к серверу {server_string} как {client.Nickname}\n";
                    ClientRequest request = new ClientRequest
                    {
                        Operation = OperationType.Login,
                        Client = client
                    };

                    MessagesView += $"Логин на сервере как {client.Nickname}\n";
                    m_Socket.Send(request.ToJson());
                    MessagesView += "Логин успешно выполнен.\n";
                };
            m_Socket.MessageReceived +=
                (object sender, MessageReceivedEventArgs messageReceivedEventArgs) =>
                {
                    ServerResponse response = messageReceivedEventArgs.Message.FromJson<ServerResponse>();

                    if (response.ResponseType == ResponseType.Message)
                    {
                        MessagesView += response.Message + "\n";
                    }
                    else if (response.ResponseType == ResponseType.Login)
                    {
                        MessagesView += response.Message + "\n";
                    }
                    else if (response.ResponseType == ResponseType.Logout)
                    {
                        MessagesView += response.Message + "\n";
                    }
                    else
                    {
                        MessagesView += "ELSE!\n";
                    }
                };
            m_Socket.Error += (object sender, ErrorEventArgs error) =>
            {
                MessagesView += "Error: " + error.Exception.Message + "\n";
            };

            m_Socket.Closed += (object sender, EventArgs eventArgs) =>
            {
                MessagesView += "Socket closed\n";
            };
            SendMessageLogic = new RelayCommand((object obj) =>
            {
                //Send message logic here
                client.Message = InputBoxText;
                request.Client = client;
                request.Operation = OperationType.PostMessage;
                if (client.Message.Length > 0 &&
                    !string.IsNullOrWhiteSpace(client.Message))
                {
                    MessagesView += "Me: " + InputBoxText + "\n";
                    m_Socket.Send(request.ToJson());
                }
                InputBoxText = string.Empty;
            });
            m_Socket.Open();
        }

        public string MessagesView
        {
            get
            {
                return m_MessagesView;
            }
            set
            {
                m_MessagesView = value;
                OnPropertyChanged();
            }
        }

        public string InputBoxText
        {
            get
            {
                return m_InputBoxText;
            }
            set
            {
                m_InputBoxText = value;
                OnPropertyChanged();
            }
        }
        
        public RelayCommand btn_Send_Click
        {
            get
            {
                return SendMessageLogic;
            }
        }
    }
}
