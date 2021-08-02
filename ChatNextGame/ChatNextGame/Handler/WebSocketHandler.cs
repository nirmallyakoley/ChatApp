using ChatNextGame.BO;
using ChatNextGame.Connection;
using ChatNextGame.DBOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatNextGame.Handler
{
    public class WebSocketHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }
        private IDBOpeartion _dbOperation;
        public WebSocketHandler(ConnectionManager webSocketConnectionManager, IDBOpeartion dbOperation)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
            _dbOperation = dbOperation;
        }

        public  async Task OnConnected(WebSocket socket)
        {
            if (WebSocketConnectionManager.SocketCount() <= 20)
            {
                await WebSocketConnectionManager.AddSocket(socket);
                //var socketId = WebSocketConnectionManager.GetId(socket);
                //await SendMessageToAllAsync($"{socketId} is now connected");
            }
            else 
            {
                await SendMessageToAllAsync($"Chat Room is now full");
            }
        }

        public  async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),offset: 0,count: message.Length),messageType: WebSocketMessageType.Text, endOfMessage: true,cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

       
        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            if (message.Contains(":"))
            {
                string[] strPlit = message.Split(':');
                await SendMessageToAllAsync($"{strPlit[0]} said :{strPlit[1]}");

                //Store chat data//
                await StoreChatData(new UserMessage { Message= strPlit[1] , UserName= strPlit[0] });
                //
            }
            else
            {
                await SendMessageToAllAsync(message);
            }
        }
        private async Task StoreChatData(UserMessage userMessage)
        {
            await _dbOperation.AddtoDB(userMessage);
        }
    }
}
