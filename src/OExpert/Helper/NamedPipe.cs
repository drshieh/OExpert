using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace OExpert
{
    public class PipeServer
    {
        public string sPipeName = "OExpert";
        public bool isRunning = true;

        //---------------------------------------------------------------------
        public void StartServer(string _appPipe)
        {
            if (!String.IsNullOrEmpty(_appPipe)) sPipeName = _appPipe;
            StartServerX();
        }

        //---------------------------------------------------------------------
        public void StartServerX()
        {
            // Start the server in a background task
            Task.Run(() => RunServerAsync());
        }

        //---------------------------------------------------------------------
        public async Task RunServerAsync()
        {
            Console.WriteLine("Named Pipe Server started. Listening for clients...");
            isRunning = true;

            while (isRunning)
            {
                var server = new NamedPipeServerStream(
                    sPipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                try
                {
                    // Asynchronously wait for connection (non-blocking)
                    Console.WriteLine("Waiting for connection...");
                    await server.WaitForConnectionAsync();
                    Console.WriteLine($"Client connected at {DateTime.Now:T}");

                    // Handle client in separate task without blocking
                    _ = HandleClientAsync(server);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server error: {ex.Message}");
                    server.Dispose();
                    isRunning = false;
                }
            }
        }

        //---------------------------------------------------------------------
        public void EndServerX()
        {
            isRunning = false;
            Console.WriteLine("EndServerX");
        }

        //---------------------------------------------------------------------
        private async Task HandleClientAsync(NamedPipeServerStream server)
        {
            try
            {
                using (var reader = new StreamReader(server))
                using (var writer = new StreamWriter(server) { AutoFlush = true })
                {
                    while (true)
                    {
                        // Read with timeout to prevent permanent blocking
                        var readTask = reader.ReadLineAsync();
                        if (await Task.WhenAny(readTask, Task.Delay(5000)) != readTask)
                        {
                            if (!server.IsConnected) break;
                            continue; // Timeout check
                        }

                        string request = await readTask;
                        if (string.IsNullOrEmpty(request)) break;

                        Console.WriteLine($"Received: {request}");

                        // Process request
                        await writer.WriteLineAsync($"ACK: {DateTime.Now:T}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                if (server.IsConnected)
                {
                    server.Disconnect();
                    Console.WriteLine("Client disconnected");
                }
                server.Dispose();
            }
        }
    }

    //===================================================================================
    public class PipeClient
    {
        public string sPipeName = "ChineseMD_TC";

        //---------------------------------------------------------------------
        public void ClientSend(string _pipeName, string sMessage)
        {
            if (!String.IsNullOrEmpty(_pipeName)) sPipeName = _pipeName;
            
            Task.Run(() => RunClientAsync(sMessage));
            Thread.Sleep(300); // Stagger connections

            Console.WriteLine("Clients Sent: " + sMessage);
        }

        //---------------------------------------------------------------------
        public async Task RunClientAsync(string sMsg)
        {
            using (var client = new NamedPipeClientStream(".", sPipeName, PipeDirection.InOut))
            {
                try
                {
                    await client.ConnectAsync(3000);
                    Console.WriteLine("Message:" + sMsg);

                    using (var writer = new StreamWriter(client) { AutoFlush = true })
                    using (var reader = new StreamReader(client))
                    {
                            string message = sMsg;
                            await writer.WriteLineAsync(message);
                            Console.WriteLine("Sent: " + message);

                            string response = await reader.ReadLineAsync();
                            Console.WriteLine("Received: " + response);

                            await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    string sErrMsg = "ERROR: " + ex.Message + "\r\n " + sPipeName;
                    Console.WriteLine(sErrMsg);
                }
            }
        }
    }
}
