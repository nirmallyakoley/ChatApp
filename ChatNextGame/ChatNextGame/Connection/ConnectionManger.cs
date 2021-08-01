using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatNextGame.Connection
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _socketCollection = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string id)
        {
            return _socketCollection.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _socketCollection;
        }

        public string GetId(WebSocket socket)
        {
            return _socketCollection.FirstOrDefault(p => p.Value == socket).Key;
        }
        public async Task AddSocket(WebSocket socket)
        {
           await Task.Run(() =>_socketCollection.TryAdd(CreateConnectionId(), socket));
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            _socketCollection.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the ConnectionManager",
                                    cancellationToken: CancellationToken.None);
        }

        public int SocketCount()
        {
            return _socketCollection.Count;
        }
        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}