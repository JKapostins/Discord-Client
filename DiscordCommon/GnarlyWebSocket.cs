using Discord.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using Windows.Storage.Streams;
using Discord;
using Windows.Foundation;
using System.IO;
using Windows.Web;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace DiscordCommon
{
    public delegate void WaitForPushCallBack();
    public class GnarlyWebSocket : IWebSocketEngine
    {
        public GnarlyWebSocket()
        {
            Socket = new MessageWebSocket();
            _sendQueue = new ConcurrentQueue<string>();
        }

        public event EventHandler<BinaryMessageEventArgs> BinaryMessage;
        public event EventHandler<TextMessageEventArgs> TextMessage;
        public MessageWebSocket Socket { get; private set; }

        public string Host { get; private set; } = null;
        public string LoginPacket { get; private set; } = null;

        public async Task Connect(string host, CancellationToken cancelToken)
        {
            Host = host;
            if (Socket == null)
            {
                Socket = new MessageWebSocket();
            }

            Socket.Control.MessageType = SocketMessageType.Utf8;
            Socket.SetRequestHeader("User-Agent", "Discord Client (Gnarlysoft LLC)");
            Socket.MessageReceived += MessageReceived;

            try
            {
                await Socket.ConnectAsync(new Uri(host));

                _messageWriter = new DataWriter(Socket.OutputStream);
            }
            catch (Exception e)
            {
                Socket.Dispose();
                Socket = null;
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    try
                    {
                        if (args.MessageType == SocketMessageType.Utf8)
                        {
                            string read = reader.ReadString(reader.UnconsumedBufferLength);
                            OnTextMessage(read);
                        }
                        else if (args.MessageType == SocketMessageType.Binary)
                        {
                            byte[] bytes = new byte[reader.UnconsumedBufferLength];
                            reader.ReadBytes(bytes);
                            OnBinaryMessage(bytes);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);

                switch (status)
                {
                    case WebErrorStatus.OperationCanceled:
                        break;

                    default:
                        break;
                }
            }
        }

        private Task SendAsync(CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    string json;
                    while (_sendQueue.TryDequeue(out json))
                    {
                        if (LoginPacket == null)
                        {
                            LoginPacket = json;
                        }

                        _messageWriter.WriteString(json);
                        try
                        {
                            // Send the data as one complete message.
                            await _messageWriter.StoreAsync();
                        }
                        catch (Exception ex)
                        {
                            WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);

                            switch (status)
                            {
                                case WebErrorStatus.OperationCanceled:
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    await Task.Delay(DiscordConfig.WebSocketQueueInterval, cancelToken).ConfigureAwait(false);
                }
            });
        }

        public Task Disconnect()
        {
            string ignored;
            while (_sendQueue.TryDequeue(out ignored)) { }

            CloseSocket();
            return Task.Delay(0);
        }

        public void QueueMessage(string message)
            => _sendQueue.Enqueue(message);

        public IEnumerable<Task> GetTasks(CancellationToken cancelToken)
            => new Task[] { /*ReceiveAsync(cancelToken),*/ SendAsync(cancelToken) };

        private void CloseSocket()
        {
            if (_messageWriter != null)
            {
                // In order to reuse the socket with another DataWriter, the socket's output stream needs to be detached.
                // Otherwise, the DataWriter's destructor will automatically close the stream and all subsequent I/O operations
                // invoked on the socket's output stream will fail with ObjectDisposedException.
                //
                // This is only added for completeness, as this sample closes the socket in the very next code block.
                _messageWriter.DetachStream();
                _messageWriter.Dispose();
                _messageWriter = null;
            }

            if (Socket != null)
            {
                try
                {
                    Socket.Close(1000, "Closed due to user request.");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                Socket = null;
            }
        }

        private void OnBinaryMessage(byte[] data)
            => BinaryMessage(this, new BinaryMessageEventArgs(data));
        private void OnTextMessage(string msg)
            => TextMessage(this, new TextMessageEventArgs(msg));

        private readonly ConcurrentQueue<string> _sendQueue;
        private DataWriter _messageWriter;
    }
}
