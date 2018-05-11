using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, int> onlineUsers = new Dictionary<string, int>();

        public Task<ConnectionOpenedResult> ConnectionOpened(string userId)
        {
            var joined = false;
            lock(onlineUsers)
            {
                if (onlineUsers.ContainsKey(userId))
                {
                    onlineUsers[userId] += 1;
                }
                else
                {
                    onlineUsers.Add(userId, 1);
                    joined = true;
                }
            }
            return Task.FromResult(new ConnectionOpenedResult { UserJoined = joined });
        }

        public Task<ConnectionClosedResult> ConnectionClosed(string userId)
        {
            var left = false;
            lock(onlineUsers)
            {
                if (onlineUsers.ContainsKey(userId))
                {
                    onlineUsers[userId] -= 1;
                    if (onlineUsers[userId] <= 0)
                    {
                        onlineUsers.Remove(userId);
                        left = true;
                    }
                }
            }
            return Task.FromResult(new ConnectionClosedResult { UserLeft = left });
        }

        public Task<string[]> GetOnlineUsers()
        {
            lock(onlineUsers)
            {
                return Task.FromResult(onlineUsers.Keys.ToArray());
            }
        }
    }

    public class ConnectionOpenedResult
    {
        public bool UserJoined { get; set; }
    }

    public class ConnectionClosedResult
    {
        public bool UserLeft { get; set; }
    }
}