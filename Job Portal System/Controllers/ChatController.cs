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
            string userRole = (Session["UserRole"] as string) ?? string.Empty;

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

            if (userRole.Equals("Employer", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Chat/EmployerIndex.cshtml", history);
            }
            else
            {
                // Default to job seeker inbox for any non-employer role
                return View("~/Views/Chat/JobSeekerIndex.cshtml", history);
            }
        }

        public ActionResult Inbox()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int currentUserId = (int)Session["UserId"];

            // Ensure we read the correct session key: AccountController stores role in "UserRole"
            string userRole = (Session["UserRole"] as string) ?? string.Empty;

            // If session didn't contain role, try to fetch from DB and cache it
            if (string.IsNullOrWhiteSpace(userRole))
            {
                var user = db.USERS.Find(currentUserId);
                if (user != null)
                {
                    userRole = user.role ?? string.Empty;
                    Session["UserRole"] = userRole;
                }
            }

            var userIds = db.Messages
                .Where(m => m.SenderID == currentUserId || m.ReceiverID == currentUserId)
                .Select(m => m.SenderID == currentUserId ? m.ReceiverID : m.SenderID)
                .Distinct()
                .ToList();

            var conversations = db.USERS
                .Where(u => userIds.Contains(u.UserId))
                .ToList();

            // Decide view based on actual role string saved at login
            if (userRole.Equals("Employer", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Chat/EmployerInbox.cshtml", conversations);
            }
            else
            {
                // Default to job seeker inbox for any non-employer role
                return View("~/Views/Chat/JobSeekerInbox.cshtml", conversations);
            }
        }

    }


}