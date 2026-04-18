using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Job_Portal_System;
using System.Data.Entity;
using Job_Portal_System.ViewModel;


namespace Job_Portal_System.Controllers
{
    public class ApplicationController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();

        // -----------------------------------------
        // Job Seeker: Apply for a job
        // -----------------------------------------


        // -----------------------------------------
        // Job Seeker: View my applications
        // -----------------------------------------
        public ActionResult MyApplications()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int seekerId = Convert.ToInt32(Session["UserId"]);

            var applications = db.APPLICATIONS
                .Include(a => a.JOB)
                .Include(a => a.JOB.EMPLOYER)
                .Include(a => a.INTERVIEW_SCHEDULES)
                .Where(a => a.seeker_id == seekerId)
                .OrderByDescending(a => a.applied_date)
                .Select(a => new JobSeekerApplicationViewModel
                {
                    ApplicationId = a.application_id,
                    JobTitle = a.JOB.job_title,
                    JobId = a.job_id,
                    CompanyName = a.JOB.EMPLOYER.company_name,
                    AppliedDate = a.applied_date,
                    Status = a.status,
                    EmployerId = a.JOB.employer_id,
                    InterviewDate = a.INTERVIEW_SCHEDULES.FirstOrDefault().interview_date,
                    InterviewLocation = a.INTERVIEW_SCHEDULES.FirstOrDefault().location_or_link,
                    InterviewNotes = a.INTERVIEW_SCHEDULES.FirstOrDefault().notes_for_seeker,
                    RejectionFeedback = a.RejectionFeedback
                })
                .ToList();

            return View(applications);
        }

        // -----------------------------------------
        // Employer: View applications for my jobs
        // -----------------------------------------
        public ActionResult JobApplications(int? jobId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (jobId == null)
                return RedirectToAction("ManageJobs"); // or any default page

            var applications = db.APPLICATIONS
                                 .Where(a => a.job_id == jobId)
                                 .Include(a => a.JOB)
                                 .Include(a => a.JOB_SEEKER_PROFILE)
                                 .Include(a => a.JOB_SEEKER_PROFILE.USER)
                                 .ToList();

            return View(applications);
        }




        // -----------------------------------------
        // Employer: Update application status
        // -----------------------------------------
        public ActionResult UpdateStatus(int id, string status)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var application = db.APPLICATIONS.FirstOrDefault(a => a.application_id == id);
            if (application == null)
                return HttpNotFound();

            // Update application status
            application.status = status;
            db.SaveChanges();

            // ---------- CREATE NOTIFICATION ----------
            if (status == "APPROVED")
            {
                NOTIFICATION note = new NOTIFICATION
                {
                    user_id = application.seeker_id,  // job seeker ID
                    message = "Your application for the job '" + application.JOB.job_title + "' has been approved!",
                    created_at = DateTime.Now,
                    read_status = "UNREAD"
                };

                db.NOTIFICATIONS.Add(note);
                db.SaveChanges();
            }
            else if (status == "REJECTED")
            {
                NOTIFICATION note = new NOTIFICATION
                {
                    user_id = application.seeker_id,
                    message = "Your application for the job '" + application.JOB.job_title + "' has been rejected.",
                    created_at = DateTime.Now,
                    read_status = "UNREAD"
                };

                db.NOTIFICATIONS.Add(note);
                db.SaveChanges();
            }
            // -----------------------------------------

            TempData["Success"] = "Application status updated!";
            return RedirectToAction("JobApplications", new { jobId = application.job_id });
        }


        // -----------------------------------------
        // Delete an application (Job Seeker)
        // -----------------------------------------
        public ActionResult Withdraw(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var application = db.APPLICATIONS.FirstOrDefault(a => a.application_id == id);
            if (application != null)
            {
                db.APPLICATIONS.Remove(application);
                db.SaveChanges();
                TempData["Success"] = "Application withdrawn successfully!";
            }

            return RedirectToAction("MyApplications");
        }

        public ActionResult Apply(int jobId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int seekerId = (int)Session["UserId"];

            // Check if already applied
            var alreadyApplied = db.APPLICATIONS
                .FirstOrDefault(a => a.job_id == jobId && a.seeker_id == seekerId);

            if (alreadyApplied != null)
            {
                TempData["Error"] = "You have already applied for this job.";
                return RedirectToAction("JobDetails", "Job", new { id = jobId });
            }

            APPLICATION newApp = new APPLICATION
            {
                job_id = jobId,
                seeker_id = seekerId,
                applied_date = DateTime.Now,
                status = "PENDING"
            };

            var user_id1 = db.JOBS.FirstOrDefault(j => j.job_id == jobId).employer_id;

            NOTIFICATION newnotf = new NOTIFICATION
            {
                user_id = user_id1,  // employer ID
                message = "A new application has been submitted for your job '" + db.JOBS.FirstOrDefault(j => j.job_id == jobId).job_title + "'.",
                created_at = DateTime.Now,
                read_status = "UNREAD"
            };
            db.NOTIFICATIONS.Add(newnotf);
            db.APPLICATIONS.Add(newApp);
            db.SaveChanges();

            TempData["Success"] = "Application submitted successfully!";
            return RedirectToAction("JobDetails", "Job", new { id = jobId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
