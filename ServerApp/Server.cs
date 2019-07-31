using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonApp;
using Fleck;

namespace ServerApp
{
    class Server : IDisposable
    {
        private readonly object sync = new object();
        private readonly WebSocketServer m_Socket;
        private Dictionary<IWebSocketConnection, Client> m_Clients;

        public Server()
        {
            Console.Write("Server started\n");

            m_Clients = new Dictionary<IWebSocketConnection, Client>();
            m_Socket = new WebSocketServer($"ws://0.0.0.0:1000");
            ServerResponse response = new ServerResponse();
            m_Socket.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console
                    .WriteLine($"[Client: connected] " +
                    $"{socket.ConnectionInfo.ClientIpAddress}");
                };

                socket.OnMessage = (string message) =>
                {
                    var request = Json.FromJson<ClientRequest>(message);

                    switch (request.Operation)
                    {
                        case OperationType.Login:
                        {
                            if (m_Clients.ContainsValue(request.Client))
                            {
                                return;
                            }

                            m_Clients.Add(socket, request.Client);
                            response.Message = $"{request.Client.Nickname} connected to chat!";
                            response.ResponseType = ResponseType.Login;
                            foreach (var client in m_Clients)
                            {
                                if (client.Value.Nickname != request.Client.Nickname)
                                {
                                    client.Key.Send(response.ToJson());
                                }
                            }
                        }
                        break;
                        case OperationType.Logout:
                        {
                            lock (sync)
                            {
                                socket.Close();
                                m_Clients.Remove(socket);
                            }
                        }
                        break;
                        case OperationType.PostMessage:
                        {
                            string messageToSend = 
                                $"{request.Client.Nickname}: " +
                                $"{request.Client.Message}";
                            response.Message = messageToSend;
                            response.ResponseType = ResponseType.Message;
                            foreach (var client in m_Clients)
                            {
                                if (client.Value.Nickname != request.Client.Nickname)
                                {
                                    client.Key.Send(response.ToJson());
                                }
                            }
                        }
                        break;
                        default:
                            //Console.WriteLine(message);
                            break;
                    }
                
                };

                socket.OnError = (Exception exception) =>
                {
                    lock (sync)
                    {
                        Console
                            .WriteLine($"[Error: client disconnected] " +
                            $"{socket.ConnectionInfo.ClientIpAddress}");
                        if (m_Clients.ContainsKey(socket))
                        {
                            var clientToDelete = m_Clients[socket];
                            response.Message = $"{clientToDelete.Nickname} disconnected from chat!";
                            response.ResponseType = ResponseType.Logout;
                            m_Clients.Remove(socket);
                            foreach (var client in m_Clients)
                            {
                                client.Key.Send(response.ToJson());
                            }
                        }
                    }
                };

                socket.OnClose = () =>
                {
                    lock (sync)
                    {
                        Console
                        .WriteLine($"[Client: disconnected] " +
                        $"{socket.ConnectionInfo.ClientIpAddress}");

                        if (m_Clients.ContainsKey(socket))
                        {
                            //handling disconnecting of client
                        }

                    }

                };

            });
            

        }

        public void Run()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("30 sec!");
                    Thread.Sleep(30_000);
                }
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            foreach (var client in m_Clients)
            {
                client.Key.Close();
            }

            m_Socket.Dispose();
        }
    }
}
