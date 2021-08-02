using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChattingClient
{

    public class Program
    {
        static string name;
        public static async Task Main(string[] args)
        {

            Console.Write("Enter your name :");
            name = Console.ReadLine();
            if (ValidateUser(name))
            {              
              await RunWebSockets();
            }
        }

        private static async Task FetchOldMessage(string name)
        {
            //connect DB and populate old message//
            await Task.Run(()=> { Console.WriteLine("Old Messages......."); });
        }

        private static bool ValidateUser(string name)
        {
            //Validate user logic//
            return true;
        }

        private static async Task RunWebSockets()
        {
            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);
           
            Console.WriteLine("Connected!");
            await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{name} joined chat room")), WebSocketMessageType.Text, true, CancellationToken.None);

            ////populate old messages///
            await FetchOldMessage(name);
            //---------------------------//

            var sending = Task.Run(async () =>
            {
                string line;
                while ((line = Console.ReadLine()) != null && line != String.Empty)
                {
                    string msg = $"{name}:{line}";
                    var bytes = Encoding.UTF8.GetBytes(msg);
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    if (line == "CLOSE")
                    {
                        break;
                    }
                }

                if (line == "CLOSE")
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing Socket", CancellationToken.None);
                }
            });

            var receiving = Receiving(client);

            await Task.WhenAll(sending, receiving);
        }

        private static async Task Receiving(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
    }
}
