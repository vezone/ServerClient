using ClientUI.Models;
using CommonApp;
using SuperSocket.ClientEngine;
using System;
using System.Windows.Controls;
using WebSocket4Net;

namespace ClientUI.ViewModels
{
    class DialogsViewModel : ViewModelBase
    {
        RelayCommand SendMessageLogic;
        RelayCommand EditProfileLogic;
        RelayCommand ApplyProfileLogic;
        RelayCommand ChangeImageLogic;

        string m_MessagesView;
        string m_InputBoxText;
        static string m_AvatarImagePath;
        string m_Username;
        string m_NewUsername;

        public static WebSocket m_Socket;
        public static Client m_Client;
        public static ClientRequest m_Request;

        UserControl m_ContentPage;
        UserControl m_ProfileEditView;

        public DialogsViewModel()
        {
            AvatarImagePath = 
                @"E:\General\Programming\C_Sharp\Console\ServerClient\ServerApp\ClientUI\UIElements\avatar_default.jpg";
            m_Client = new Client();
            m_Client.Nickname = System.Configuration.ConfigurationManager.AppSettings["Nickname"];
            m_Request = new ClientRequest
            {
                Operation = OperationType.PostMessage,
                Client = m_Client
            };

            ProfileInfo.ProfileImagePath = AvatarImagePath;
            ProfileInfo.ProfileNickname = m_Client.Nickname;
            Username = m_Client.Nickname;

            string server_string = $"ws://127.0.0.1:1000";
            m_Socket = new WebSocket(server_string);
            m_Socket.Opened +=
                (object sender, EventArgs eventArgs) =>
                {
                    MessagesView += $"Подсоединение к серверу {server_string} как {m_Client.Nickname}\n";
                    m_Request.Operation = OperationType.Login;
                    m_Request.Client = m_Client;

                    MessagesView += $"Логин на сервере как {m_Client.Nickname}\n";
                    m_Socket.Send(m_Request.ToJson());
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
                m_Client.Message = InputBoxText;
                m_Request.Client = m_Client;
                m_Request.Operation = OperationType.PostMessage;
                if (m_Client.Message.Length > 0 &&
                    !string.IsNullOrWhiteSpace(m_Client.Message))
                {
                    MessagesView += "Me: " + InputBoxText + "\n";
                    m_Socket.Send(m_Request.ToJson());
                }
                InputBoxText = string.Empty;
            });

            EditProfileLogic = new RelayCommand((object obj) =>
            {
                //doing nothing for now
                if (ContentPage == null)
                {
                    m_ProfileEditView = new Views.Profile.ProfileEditView();
                }
                ContentPage = m_ProfileEditView;
            });

            ApplyProfileLogic = new RelayCommand((object obj) =>
            {
                //doing nothing for now
                m_Request.Client.Message = NewUsername;
                m_Request.Operation = OperationType.ChangedNickname;
                ProfileInfo.ProfileNickname = (NewUsername != null && NewUsername.Length > 0) ? NewUsername : ProfileInfo.ProfileNickname;
                ProfileInfo.ProfileImagePath = AvatarImagePath;
                Username = NewUsername;
                m_Socket.Send(m_Request.ToJson());
            });

            ChangeImageLogic = new RelayCommand((object obj) =>
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "*.jpg|*png";
                if (openFileDialog.ShowDialog() == true)
                {
                    AvatarImagePath = openFileDialog.FileName;
                    System.Windows.MessageBox.Show($"Path = {AvatarImagePath}");
                }
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

        public string AvatarImagePath
        {
            get
            {
                return m_AvatarImagePath;
            }
            set
            {
                m_AvatarImagePath = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get
            {
                return m_Username;
            }
            set
            {
                m_Username = value;
                OnPropertyChanged();
            }
        }

        public string NewUsername
        {
            get
            {
                return m_NewUsername;
            }
            set
            {
                if (value.Length > 0)
                {
                    m_NewUsername = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand btn_Send_Click
        {
            get
            {
                return SendMessageLogic;
            }
        }

        public RelayCommand btn_Edit_Click
        {
            get
            {
                return EditProfileLogic;
            }
        }

        public RelayCommand btn_Apply_Click
        {
            get
            {
                return ApplyProfileLogic;
            }
        }

        public RelayCommand btn_ChangeImage_Click
        {
            get
            {
                return ChangeImageLogic;
            }
        }
        

        public UserControl ContentPage
        {
            get
            {
                return m_ContentPage;
            }
            set
            {
                m_ContentPage = value;
                OnPropertyChanged();
            }
        }


    }
}
