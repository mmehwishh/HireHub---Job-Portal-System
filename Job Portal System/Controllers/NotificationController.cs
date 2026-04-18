using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Job_Portal_System;

namespace Job_Portal_System.Controllers
{
    public class NotificationController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();

        // GET: /Notification/
        //public ActionResult Index()
        //{
          //  var notifications = db.NOTIFICATIONS.Include(n => n.USER);
         //   return View(notifications.ToList());
        //}

        public ActionResult Index()
        {
            var notes = db.NOTIFICATIONS.ToList();
            return View(notes);
        }


        // GET: /Notification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NOTIFICATION notification = db.NOTIFICATIONS.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: /Notification/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.USERS, "UserId", "full_name");
            return View();
        }

        // POST: /Notification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="notification_id,user_id,message,created_at,read_status")] NOTIFICATION notification)
        {
            if (ModelState.IsValid)
            {
                db.NOTIFICATIONS.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.USERS, "UserId", "full_name", notification.user_id);
            return View(notification);
        }

        // GET: /Notification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NOTIFICATION notification = db.NOTIFICATIONS.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.USERS, "UserId", "full_name", notification.user_id);
            return View(notification);
        }

        // POST: /Notification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="notification_id,user_id,message,created_at,read_status")] NOTIFICATION notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.USERS, "UserId", "full_name", notification.user_id);
            return View(notification);
        }

        // GET: /Notification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NOTIFICATION notification = db.NOTIFICATIONS.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: /Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NOTIFICATION notification = db.NOTIFICATIONS.Find(id);
            db.NOTIFICATIONS.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult JobSeekerNotifications()
        {
            int uid = Convert.ToInt32(Session["UserId"]); // get logged-in Job Seeker ID
            if (uid <= 0 )
                return RedirectToAction("Login", "Account");

            // Get notifications for this user, newest first
            var notes = db.NOTIFICATIONS
                          .Where(n => n.user_id == uid)
                          .OrderByDescending(n => n.created_at)
                          .ToList();

            return View(notes);
        }

        [HttpPost]
        public ActionResult JobSeekerMarkAsRead(int id)
        {
            var note = db.NOTIFICATIONS.Find(id);
            if (note != null)
            {
                note.read_status = "READ";
                db.SaveChanges();
            }
            return RedirectToAction("JobSeekerNotifications");
        }


        //EMPLOYER NOTIFICATIONS
        public ActionResult EmployeerNotifications()
        {
            int uid;
            if (!int.TryParse(Convert.ToString(Session["UserId"]), out uid) || uid <= 0)
                return RedirectToAction("Login", "Account");

            var notes = db.NOTIFICATIONS
                          .Where(n => n.user_id == uid)
                          .OrderByDescending(n => n.created_at)
                          .ToList();

            return View(notes);
        }

        [HttpPost]
        public JsonResult EmployeerMarkAsRead(int id)
        {
            var note = db.NOTIFICATIONS.Find(id);
            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;
            if (note != null && note.user_id == uid)
            {
                note.read_status = "READ";
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult EmployeerMarkAsUnread(int id)
        {
            var note = db.NOTIFICATIONS.Find(id);
            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;
            if (note != null && note.user_id == uid)
            {
                note.read_status = "UNREAD";
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult EmployeerMarkAllRead()
        {
            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;
            if (uid <= 0) return Json(new { success = false });
            var notes = db.NOTIFICATIONS.Where(n => n.user_id == uid && ((n.read_status ?? "").ToUpper() == "UNREAD")).ToList();
            foreach (var n in notes) n.read_status = "READ";
            db.SaveChanges();
            return Json(new { success = true });
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
