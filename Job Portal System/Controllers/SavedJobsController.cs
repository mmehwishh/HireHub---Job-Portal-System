using Job_Portal_System;
using Job_Portal_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Job_Portal_System.Controllers
{
    public class SavedJobsController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();

        // GET: /SavedJobs/
        public ActionResult Index()
        {
            var saved_jobs = db.SAVED_JOBS.Include(s => s.JOB_SEEKER_PROFILE).Include(s => s.JOB);
            return View(saved_jobs.ToList());
        }

        // GET: /SavedJobs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAVED_JOBS saved_jobs = db.SAVED_JOBS.Find(id);
            if (saved_jobs == null)
            {
                return HttpNotFound();
            }
            return View(saved_jobs);
        }

        // GET: /SavedJobs/Create
        public ActionResult Create()
        {
            ViewBag.seeker_id = new SelectList(db.JOB_SEEKER_PROFILE, "seeker_id", "gender");
            ViewBag.job_id = new SelectList(db.JOBS, "job_id", "job_title");
            return View();
        }

        // POST: /SavedJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="seeker_id,job_id,saved_at")] SAVED_JOBS saved_jobs)
        {
            if (ModelState.IsValid)
            {
                db.SAVED_JOBS.Add(saved_jobs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.seeker_id = new SelectList(db.JOB_SEEKER_PROFILE, "seeker_id", "gender", saved_jobs.seeker_id);
            ViewBag.job_id = new SelectList(db.JOBS, "job_id", "job_title", saved_jobs.job_id);
            return View(saved_jobs);
        }

        // GET: /SavedJobs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAVED_JOBS saved_jobs = db.SAVED_JOBS.Find(id);
            if (saved_jobs == null)
            {
                return HttpNotFound();
            }
            ViewBag.seeker_id = new SelectList(db.JOB_SEEKER_PROFILE, "seeker_id", "gender", saved_jobs.seeker_id);
            ViewBag.job_id = new SelectList(db.JOBS, "job_id", "job_title", saved_jobs.job_id);
            return View(saved_jobs);
        }

        // POST: /SavedJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="seeker_id,job_id,saved_at")] SAVED_JOBS saved_jobs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(saved_jobs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.seeker_id = new SelectList(db.JOB_SEEKER_PROFILE, "seeker_id", "gender", saved_jobs.seeker_id);
            ViewBag.job_id = new SelectList(db.JOBS, "job_id", "job_title", saved_jobs.job_id);
            return View(saved_jobs);
        }

        // GET: /SavedJobs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAVED_JOBS saved_jobs = db.SAVED_JOBS.Find(id);
            if (saved_jobs == null)
            {
                return HttpNotFound();
            }
            return View(saved_jobs);
        }

        // POST: /SavedJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SAVED_JOBS saved_jobs = db.SAVED_JOBS.Find(id);
            db.SAVED_JOBS.Remove(saved_jobs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SavedJobsList()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int uid = Convert.ToInt32(Session["UserId"]);

            var savedJobsData = db.SAVED_JOBS
                .Include(s => s.JOB)
                .Include(s => s.JOB.EMPLOYER) // Eager loading for company name
                .Where(s => s.seeker_id == uid)
                .OrderByDescending(s => s.saved_at)
                .Select(s => new SavedJobViewModel // Mapping to ViewModel
                {
                    //SavedJobID = s.id, // Assuming your PK is 'id'
                    JobID = s.job_id,
                    savedSeekerId = s.seeker_id,
                    JobTitle = s.JOB.job_title,
                    CompanyName = s.JOB.EMPLOYER.company_name,
                    Location = s.JOB.location,
                    SavedAt = s.saved_at ?? DateTime.Now,
                    Status = s.Status ?? 1, // Default to 1 (Saved) if null
                    Notes = s.Notes,
                    Deadline = s.JOB.expiry_date // Assuming you have a deadline column in JOBS
                })
                .ToList();

            

            // Check karein 'jobsWithNotes.Count' kitna hai?
            // Yahan breakpoint lagayen aur check karein 'firstNote' mein value hai ya nahi

            return View(savedJobsData);
        }

        [HttpPost]
        public JsonResult UpdateNotes(int id, string notes)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Session expired. Please login again." });
            try
            {
                // 1. Record dhoondne ke liye 'Where' ka istemal karein
                int uid = Convert.ToInt32(Session["UserId"]);
                var entry = db.SAVED_JOBS.FirstOrDefault(x => x.job_id == id && x.seeker_id == uid);

                if (entry != null)
                {
                   entry.Notes = notes;

                    // 2. Composite key tables mein state manually change karni parti hai
                    db.Entry(entry).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Job record not found in database." });
            }
            catch (Exception ex)
            {
                // Agar koi error aaye (e.g. Database connection), toh ye message dikhayega
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult RemoveSavedJob(int jobId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int uid = Convert.ToInt32(Session["UserId"]);

            // Find the saved job entry
            var saved = db.SAVED_JOBS
                          .FirstOrDefault(s => s.seeker_id == uid && s.job_id == jobId);

            if (saved != null)
            {
                db.SAVED_JOBS.Remove(saved);
                db.SaveChanges();
            }

            TempData["Success"] = "Job removed from saved list.";

            return RedirectToAction("SavedJobsList");
        }




        public ActionResult SaveJob(int jobId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int seekerId = Convert.ToInt32(Session["UserId"]);

            // Check if job already saved
            var alreadySaved = db.SAVED_JOBS
                                 .FirstOrDefault(s => s.job_id == jobId && s.seeker_id == seekerId);

            if (alreadySaved != null)
            {
                TempData["Error"] = "Job is already saved.";
                return RedirectToAction("JobDetails", "Job", new { id = jobId });
            }

            // Save job
            SAVED_JOBS save = new SAVED_JOBS
            {
                job_id = jobId,
                seeker_id = seekerId,
                saved_at = DateTime.Now
            };

            db.SAVED_JOBS.Add(save);
            db.SaveChanges();

            TempData["Success"] = "Job saved successfully!";
            return RedirectToAction("JobDetails", "Job", new { id = jobId });
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
