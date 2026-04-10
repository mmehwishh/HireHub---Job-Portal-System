using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System
{
    public class ChatHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void SendMessage(int senderId, int receiverId, string message)
        {
            // 1. DB mein save karein
            using (var db = new JobDBEntities3())
            {
                var msg = new Message
                {
                    SenderID = senderId,
                    ReceiverID = receiverId,
                    MessageText = message,
                    Timestamp = DateTime.Now
                };
                db.Messages.Add(msg);
                db.SaveChanges();
            }

            // 2. Real-time push (Abhi ke liye clients handle karenge ke message kis ka hai)
            Clients.All.addNewMessageToPage(senderId, receiverId, message);
        }
    }
}