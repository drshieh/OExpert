using OExpert.UILibraryWPF;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OExpert
{
    public class XTcpServer
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private Task _serverTask;
        private bool _isRunning;
        public string sRequest;
        public string sResponse;

        //--------------------------------------------------------
        public async void StartServer(int port)
        {
            await StartServerX(port);  // Listen on port 9000
        }

        //--------------------------------------------------------
        public async Task StartServerX(int port)
        {
            if (_isRunning) return;

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _isRunning = true;

            _serverTask = Task.Run(() => RunServer(_cts.Token), _cts.Token);
        }

        //--------------------------------------------------------
        public void RestartServer(int port)
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _isRunning = true;

            _serverTask = Task.Run(() => RunServer(_cts.Token), _cts.Token);
        }

        //--------------------------------------------------------
        public bool IsServerRunning()
        {
            return _isRunning;
        }

        //--------------------------------------------------------
        private async Task RunServer(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _ = HandleClientAsync(client, ct);
                }
                catch (ObjectDisposedException)
                {
                    // Listener stopped
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    // Stopped by cancellation
                    break;
                }
            }
        }

        //--------------------------------------------------------
        private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
        {
            try
            {
                using (client)
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] buffer = new byte[1024000];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // Process message here (update UI using Dispatcher)
                        sRequest = message;
                        MainWindow.tp.Dispatcher.Invoke(() => RunServerActionCB());

                        // Send response
                        byte[] response = Encoding.UTF8.GetBytes(sResponse);
                        await stream.WriteAsync(response, 0, response.Length);
                    }
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException ||
                                      ex is ObjectDisposedException)
            {
                // Connection closed
            }
        }

        //--------------------------------------------------------
        public void StopServerX()
        {
            StopServer();
        }

        //--------------------------------------------------------
        public async Task StopServer()
        {
            if (!_isRunning) return;

            _cts?.Cancel();
            _listener?.Stop();

            try
            {
                await (_serverTask ?? Task.CompletedTask);
            }
            catch (OperationCanceledException) { /* Expected during shutdown */ }
            finally
            {
                _isRunning = false;
                _cts?.Dispose();
            }
        }

        //--------------------------------------------------------
        public void Dispose() => StopServer().Wait();

        //-----------------------------------------------------------------------------
        private void RunServerActionCB()
        {
            sResponse = MainWindow.tp.RunServerAction(sRequest);
        }

    }


    //===================================================================================
    public class XTcpClient
    {
        //--------------------------------------------------------
        public async Task<string> SendMessage(string ipAddress, int port, string message)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, port);

                    using (NetworkStream stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(data, 0, data.Length);

                        byte[] buffer = new byte[1024000];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: TcpClient SendMessage : " + ex.Message);
                return ex.Message;
            }
        }

    }
}
