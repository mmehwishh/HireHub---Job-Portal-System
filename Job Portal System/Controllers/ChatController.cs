using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Job_Portal_System.Controllers
{
    public class ChatController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();
        public ActionResult Index(int receiverId)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");

            int senderId = (int)Session["UserId"];

            // History load karna taake purani baatein nazar aayein
            var history = db.Messages
                .Where(m => (m.SenderID == senderId && m.ReceiverID == receiverId) ||
                            (m.SenderID == receiverId && m.ReceiverID == senderId))
                .OrderBy(m => m.Timestamp)
                .ToList();

            // View ko batana ke dusra banda kaun hai
            var receiver = db.USERS.Find(receiverId);
            ViewBag.ReceiverName = receiver.full_name;
            ViewBag.ReceiverID = receiverId;

            return View(history);
        }

        public ActionResult Inbox()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int currentUserId = (int)Session["UserId"];

            // Un logon ki IDs nikalna jinse baat hui hai
            var userIds = db.Messages
                .Where(m => m.SenderID == currentUserId || m.ReceiverID == currentUserId)
                .Select(m => m.SenderID == currentUserId ? m.ReceiverID : m.SenderID)
                .Distinct()
                .ToList();

            // Un users ki details (names) nikalna
            var conversations = db.USERS
                .Where(u => userIds.Contains(u.UserId))
                .ToList();

            return View(conversations);
        }

    }


}