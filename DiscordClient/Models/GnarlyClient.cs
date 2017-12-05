using DiscordCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;

namespace Discord
{
    class GnarlyClient
    {
        public static GnarlyClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GnarlyClient();
                }
                return _instance;
            }
        }

        public bool Connect(string token, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (Connected == false)
                {
                    try
                    {
                        var task = DiscordClient.Connect(token, TokenType.User);
                        task.Wait();
                        RegisterEvents();

                    }
                    catch (Exception e) // Failed to connect
                    {
                        errorMessage = e.Message;
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
            return DiscordClient.State == ConnectionState.Connected;
        }

        /// <summary>
        /// Connects to the discord servers
        /// </summary>
        /// <param name="username">The username for the discord server</param>
        /// <param name="password">The password for the user</param>
        /// <returns>True if connection succeded, false otherwise</returns>
        public bool Connect(string username, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (Connected == false)
                {
                    //RegisterBackgroundTasks();
                    try
                    {
                        var task = DiscordClient.Connect(username, password);
                        task.Wait();
                        RegisterEvents();

                    }
                    catch (Exception e)// Failed to connect
                    {
                        errorMessage = e.Message;
                    }
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }

            return DiscordClient.State == ConnectionState.Connected;
        }

        public async void Disconnect()
        {
            try
            {
                UnregisterEvents();
                await DiscordClient.Disconnect();
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        public DiscordClient DiscordClient { get; private set; }
        public bool Connected { get { return DiscordClient.State == ConnectionState.Connected; } }
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<UserUpdatedEventArgs> UserUpdated = delegate { };

        public MessageWebSocket NativeSocket
        {
            get { return _gnarlySocket.Socket; }
        }

        public string ConnectionHost
        {
            get { return _gnarlySocket.Host; }
        }

        public string LoginPacket
        {
            get { return _gnarlySocket.LoginPacket; }
        }

        private GnarlyClient()
        {
            try
            {
                _gnarlySocket = new GnarlyWebSocket();
                DiscordClient = new DiscordClient(_gnarlySocket);
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
            //RegisterNetworkChangeTask();
        }

        private void RegisterEvents()
        {
            DiscordClient.UserUpdated += DiscordClient_UserUpdated;
            DiscordClient.MessageReceived += DiscordClient_MessageReceived;
        }

        private void UnregisterEvents()
        {
            DiscordClient.UserUpdated -= DiscordClient_UserUpdated;
            DiscordClient.MessageReceived -= DiscordClient_MessageReceived;
        }

        private void DiscordClient_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                if (MessageReceived != null)
                {
                    OnEvent(MessageReceived, new MessageEventArgs(e.Message));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void DiscordClient_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            try
            {
                if (UserUpdated != null)
                {
                    OnEvent(UserUpdated, new UserUpdatedEventArgs(e.Before, e.After));
                }
            }
            catch (Exception exception)
            {
                App.LogUnhandledError(exception);
            }
        }

        private void OnEvent<T>(EventHandler<T> handler, T eventArgs)
        {
            try { handler(this, eventArgs); }
            catch (Exception ex)
            {
                App.LogUnhandledError(new Exception("Error calling event: " + ex.Message));
            }
        }
        private void OnEvent(EventHandler handler)
        {
            try { handler(this, EventArgs.Empty); }
            catch (Exception ex)
            {
                App.LogUnhandledError(new Exception("Error calling event: " + ex.Message));
            }
        }

        private static GnarlyClient _instance;
        private GnarlyWebSocket _gnarlySocket;
    }
}
