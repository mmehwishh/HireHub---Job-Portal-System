using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Job_Portal_System;
using Job_Portal_System.ViewModel;
using System.IO;


namespace Job_Portal_System.Controllers
{
    public class EmployeerController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();

        // GET: Employer Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Account");

            string email = (Session["Email"] != null) ? Session["Email"].ToString() : null;
            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;

            var employer = db.EMPLOYERS.FirstOrDefault(e => e.employer_id == uid);

            if (employer == null)
            {
                return RedirectToAction("CreateProfile", "Employeer");
            }

            return View(employer);
        }


        public ActionResult DownloadResume(int seekerId)
        {
            var seeker = db.JOB_SEEKER_PROFILE.FirstOrDefault(s => s.seeker_id == seekerId);

            if (seeker == null || string.IsNullOrEmpty(seeker.resume_file))
                return HttpNotFound("Resume not found.");

            string filePath = Server.MapPath(seeker.resume_file);

            if (!System.IO.File.Exists(filePath))
                return HttpNotFound("File missing from server.");

            string fileName = Path.GetFileName(filePath);

            return File(filePath, "application/pdf");
        }

        public ActionResult ViewResume(int seekerId)
        {
            var seeker = db.JOB_SEEKER_PROFILE.FirstOrDefault(s => s.seeker_id == seekerId);

            if (seeker == null || string.IsNullOrEmpty(seeker.resume_file))
                return HttpNotFound("Resume not found.");

            string filePath = Server.MapPath(seeker.resume_file);

            if (!System.IO.File.Exists(filePath))
                return HttpNotFound("File missing from server.");

            // PDF ko browser mein 'inline' (preview) karne ka logic
            // 'inline' batata hai browser ko ke file ko isi tab/frame mein render karo
            Response.AppendHeader("Content-Disposition", "inline; filename=" + Path.GetFileName(filePath));

            return File(filePath, "application/pdf");
        }


        // GET: View Employer Profile
        public ActionResult MyProfile()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Account");

            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;

            var user = db.USERS.FirstOrDefault(u => u.UserId == uid);
            var employer = db.EMPLOYERS.FirstOrDefault(e => e.employer_id == uid);

            if (employer == null)
                return RedirectToAction("CreateProfile", "Employeer");

            var vm = new EmployerProfileViewModel
            {
                FullName = user.full_name,
                CompanyName = employer.company_name,
                Industry = employer.industry,
                CompanyDescription = employer.company_description,
                Website = employer.website,
                Location = employer.location,
                 CompanyLogo = string.IsNullOrEmpty(employer.companyLogo) 
                      ? Url.Content("~/Images/CompanyLogos/default-logo.webp") 
                      : employer.companyLogo
            };

            return View(vm);
        }

        // POST: Update Employer Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile(EmployerProfileViewModel model, HttpPostedFileBase CompanyLogoFile)
        {
            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;

            var user = db.USERS.FirstOrDefault(u => u.UserId == uid);
            var employer = db.EMPLOYERS.FirstOrDefault(e => e.employer_id == uid);

            if (user == null || employer == null)
                return RedirectToAction("Login", "Account");

            // Update USER table
            user.full_name = model.FullName;

            // Update EMPLOYER table
            employer.company_name = model.CompanyName;
            employer.industry = model.Industry;
            employer.company_description = model.CompanyDescription;
            employer.website = model.Website;
            employer.location = model.Location;

            if (CompanyLogoFile != null && CompanyLogoFile.ContentLength > 0) 
            { 
                string fileName = Path.GetFileNameWithoutExtension(CompanyLogoFile.FileName); 
                string ext = Path.GetExtension(CompanyLogoFile.FileName); 
                string newFileName = string.Format("{0}_{1}{2}", fileName, DateTime.Now.Ticks, ext); 
                string logoPath = "/Images/CompanyLogos/" + newFileName; 
                CompanyLogoFile.SaveAs(Server.MapPath(logoPath)); employer.companyLogo = logoPath; 
            }


            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return RedirectToAction("MyProfile");
        }


        // GET: Create Employer Profile
        public ActionResult CreateProfile()
        {
            if (Session["Email"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        // POST: Create Employer Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProfile(EmployerProfileViewModel model)
        {
            int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;
            var user = db.USERS.FirstOrDefault(u => u.UserId == uid);

            if (user == null)
                return RedirectToAction("Login", "Account");

            EMPLOYER employer = new EMPLOYER
            {
                employer_id = uid,
                company_name = model.CompanyName,
                industry = model.Industry,
                company_description = model.CompanyDescription,
                website = model.Website,
                location = model.Location
            };

            db.EMPLOYERS.Add(employer);
            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        public JsonResult RejectApplication(int id, string feedback)
        {
            try
            {
                var app = db.APPLICATIONS.Find(id);
                if (app != null)
                {
                    app.status = "REJECTED";
                    // Make sure you added 'RejectionFeedback' column to your APPLICATIONS table
                    app.RejectionFeedback = feedback;

                    var user_job = db.JOBS.FirstOrDefault(j => j.job_id == app.job_id);
                    if (user_job != null)
                    {
                        var job_title = user_job.job_title;
                        var userNotf = new NOTIFICATION { 
                            user_id = app.seeker_id,
                            message = $"Thank you for applying to {job_title}. The employer has decided to move forward with other candidates at this time.",
                            created_at = DateTime.Now,
                            read_status = "UNREAD",

                        };
                        db.NOTIFICATIONS.Add(userNotf);
                        db.SaveChanges();

                    }

                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Application not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public JsonResult GetInterview(int appId)
        {
            var interview = db.INTERVIEW_SCHEDULES
                .Where(i => i.application_id == appId)
                .Select(i => new
                {
                    i.interview_type,
                    i.interview_date,
                    i.location_or_link,
                    i.interviewer_name,
                    i.interviewer_email,
                    i.notes_for_seeker,
                    i.internal_notes,
                    i.interview_status
                })
                .FirstOrDefault();

            return Json(interview, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ScheduleInterview(
            int appId,
            string type,
            DateTime date,
            string location,
            string interviewer,
            string email,
            string notes,
            string internalNotes,
            string status)
        {
            if (date == null)
            {
                return Json(new { success = false, message = "Invalid Date format." });
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var app = db.APPLICATIONS.Find(appId);

                    if (app == null)
                        return Json(new { success = false, message = "Application not found" });

                    app.status = "Interview Scheduled";

                    var existingInterview = db.INTERVIEW_SCHEDULES
                        .FirstOrDefault(i => i.application_id == appId);

                    if (existingInterview != null)
                    {
                        if (existingInterview.created_at == null)
                        {
                            existingInterview.created_at = DateTime.Now;
                        }
                        existingInterview.interview_type = type;
                        existingInterview.interview_date = date;
                        existingInterview.location_or_link = location;
                        existingInterview.interviewer_name = interviewer;
                        existingInterview.interviewer_email = email;
                        existingInterview.notes_for_seeker = notes;
                        existingInterview.internal_notes = internalNotes;
                        existingInterview.interview_status = status;
                        existingInterview.updated_at = DateTime.Now;
                    }
                    else
                    {
                        var interview = new INTERVIEW_SCHEDULES
                        {
                            application_id = appId,
                            interview_type = type,
                            interview_date = date,
                            location_or_link = location,
                            interviewer_name = interviewer,
                            interviewer_email = email,
                            notes_for_seeker = notes,
                            internal_notes = internalNotes,
                            interview_status = status,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now
                        };

                        db.INTERVIEW_SCHEDULES.Add(interview);
                    }

                    var user_job = db.JOBS.FirstOrDefault(j => j.job_id == app.job_id);
                    int uid = (Session["UserId"] != null) ? Convert.ToInt32(Session["UserId"]) : 0;
                    var employeerDetail = db.EMPLOYERS.FirstOrDefault(c => c.employer_id == uid);
                    if (user_job != null)
                    {
                        var job_title = user_job.job_title;
                        var userNotf = new NOTIFICATION
                        {
                            user_id = app.seeker_id,
                            message = $"You’ve been invited for an interview! {employeerDetail.company_name} has scheduled a session for you on {existingInterview.created_at}.",
                            created_at = DateTime.Now,
                            read_status = "UNREAD",

                        };
                        db.NOTIFICATIONS.Add(userNotf);

                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        // Dispose
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
