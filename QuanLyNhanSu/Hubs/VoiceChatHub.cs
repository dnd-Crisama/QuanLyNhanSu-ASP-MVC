using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using QuanLyNhanSu.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QuanLyNhanSu.Hubs
{
    [HubName("voiceChatHub")]
    public class VoiceChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserState> connectedUsers = new ConcurrentDictionary<string, UserState>();
        private static readonly ConcurrentDictionary<string, HashSet<string>> channels = new ConcurrentDictionary<string, HashSet<string>>();

        public class UserState
        {
            public string Username { get; set; }
            public string CurrentChannel { get; set; }
            public bool IsMuted { get; set; }
            public bool IsDeafened { get; set; }
        }

        public override Task OnConnected()
        {
            var username = Context.QueryString["username"];
            var userState = new UserState
            {
                Username = username,
                CurrentChannel = null,
                IsMuted = false,
                IsDeafened = false
            };

            connectedUsers.TryAdd(Context.ConnectionId, userState);

            var currentUsers = connectedUsers.Values.Select(u => new {
                username = u.Username,
                channel = u.CurrentChannel,
                isMuted = u.IsMuted,
                isDeafened = u.IsDeafened
            }).ToList();

            Clients.Caller.initializeUserList(currentUsers);
            Clients.Others.userConnected(new
            {
                username = userState.Username,
                channel = userState.CurrentChannel,
                isMuted = userState.IsMuted,
                isDeafened = userState.IsDeafened
            });

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (connectedUsers.TryRemove(Context.ConnectionId, out UserState userState))
            {
                if (userState.CurrentChannel != null)
                {
                    LeaveChannel(userState.CurrentChannel);
                }
                Clients.All.userDisconnected(userState.Username);
            }
            return base.OnDisconnected(stopCalled);
        }

        public void JoinChannel(string channelName)
        {
            if (connectedUsers.TryGetValue(Context.ConnectionId, out UserState userState))
            {
                if (userState.CurrentChannel != null)
                {
                    LeaveChannel(userState.CurrentChannel);
                }

                var channelUsers = channels.GetOrAdd(channelName, new HashSet<string>());
                channelUsers.Add(Context.ConnectionId);
                userState.CurrentChannel = channelName;

                Groups.Add(Context.ConnectionId, channelName);

                Clients.All.userJoinedChannel(new
                {
                    username = userState.Username,
                    channel = channelName,
                    isMuted = userState.IsMuted,
                    isDeafened = userState.IsDeafened
                });
            }
        }

        public void LeaveChannel(string channelName)
        {
            if (connectedUsers.TryGetValue(Context.ConnectionId, out UserState userState))
            {
                if (channels.TryGetValue(channelName, out HashSet<string> channelUsers))
                {
                    channelUsers.Remove(Context.ConnectionId);
                    if (channelUsers.Count == 0)
                    {
                        channels.TryRemove(channelName, out _);
                    }
                }

                userState.CurrentChannel = null;
                Groups.Remove(Context.ConnectionId, channelName);

                Clients.All.userLeftChannel(new
                {
                    username = userState.Username,
                    channel = '0',
                    isMuted = userState.IsMuted,
                    isDeafened = userState.IsDeafened
                });
            }
        }

        public void ToggleMute()
        {
            if (connectedUsers.TryGetValue(Context.ConnectionId, out UserState userState))
            {
                userState.IsMuted = !userState.IsMuted;
                Clients.All.userStateChanged(new
                {
                    username = userState.Username,
                    channel = userState.CurrentChannel,
                    isMuted = userState.IsMuted,
                    isDeafened = userState.IsDeafened
                });
            }
        }

        public void ToggleDeafen()
        {
            if (connectedUsers.TryGetValue(Context.ConnectionId, out UserState userState))
            {
                userState.IsDeafened = !userState.IsDeafened;
                if (userState.IsDeafened)
                {
                    userState.IsMuted = true;
                }
                Clients.All.userStateChanged(new
                {
                    username = userState.Username,
                    channel = userState.CurrentChannel,
                    isMuted = userState.IsMuted,
                    isDeafened = userState.IsDeafened
                });
            }
        }

        public void BroadcastVoiceData(int[] voiceData)
        {
            if (connectedUsers.TryGetValue(Context.ConnectionId, out UserState userState))
            {
                if (userState?.CurrentChannel != null && !userState.IsMuted && voiceData != null && voiceData.Length > 0)
                {
                    Clients.OthersInGroup(userState.CurrentChannel).receiveVoiceData(userState.Username, voiceData);
                }
            }
        }

    }
}
