using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using QuanLyNhanSu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
namespace QuanLyNhanSu.Hubs
{
    [HubName("chatHub")]
    public class Chat : Hub
    {
        public async Task SendMessage(string conversationId, string userId, string senderName,
            string senderAvatar, string content, string imagePath, DateTime sentAt, int senderGender)
        {
            await Clients.Group(conversationId).receiveMessage(
                conversationId,
                userId,
                senderName,
                senderAvatar,
                content,
                imagePath,
                sentAt.ToString("o"), // ISO 8601 format for consistent date handling
                senderGender
            );
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.Add(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.Remove(Context.ConnectionId, conversationId);
        }
    }
}