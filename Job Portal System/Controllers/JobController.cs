using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Job_Portal_System;
using Job_Portal_System.ViewModel;

namespace Job_Portal_System.Controllers
{
    public class JobController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();

        // GET: /Job/
        public ActionResult Index()
        {
            var jobs = db.JOBS.Include(j => j.EMPLOYER);
            return View(jobs.ToList());
        }

        // GET: /Job/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JOB job = db.JOBS.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: /Job/Create
        public ActionResult Create()
        {
            ViewBag.employer_id = new SelectList(db.EMPLOYERS, "employer_id", "company_name");
            return View();
        }

        // POST: /Job/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="job_id,employer_id,job_title,job_description,requirements,location,salary_range,job_type,posted_date,expiry_date,status")] JOB job)
        {
            if (ModelState.IsValid)
            {
                db.JOBS.Add(job);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.employer_id = new SelectList(db.EMPLOYERS, "employer_id", "company_name", job.employer_id);
            return View(job);
        }

        // GET: /Job/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JOB job = db.JOBS.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            ViewBag.employer_id = new SelectList(db.EMPLOYERS, "employer_id", "company_name", job.employer_id);
            return View(job);
        }

        // POST: /Job/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include="job_id,employer_id,job_title,job_description,requirements,location,salary_range,job_type,posted_date,expiry_date,status")] JOB job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageJobs");
            }
            ViewBag.employer_id = new SelectList(db.EMPLOYERS, "employer_id", "company_name", job.employer_id);

            return RedirectToAction("ManageJobs");
        }

        // GET: /Job/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JOB job = db.JOBS.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: /Job/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JOB job = db.JOBS.Find(id);
            db.JOBS.Remove(job);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult BrowseJobs(string search, string location, string company, int? page)
        {
            // 1. Use a View Model to avoid passing the raw Database Entity to the View
            // 2. Set up pagination parameters
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var jobsQuery = db.JOBS
                .Include(j => j.EMPLOYER) // Eager loading: avoids N+1 query issues
                .AsQueryable();

            // 3. Efficient Filtering
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Trim() handles accidental spaces in search inputs
                string term = search.Trim();
                jobsQuery = jobsQuery.Where(x => x.job_title.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                jobsQuery = jobsQuery.Where(x => x.location.Contains(location.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(company))
            {
                jobsQuery = jobsQuery.Where(x => x.EMPLOYER.company_name.Contains(company.Trim()));
            }

            // 4. Always Order before Paging
            var results = jobsQuery
                .OrderByDescending(x => x.posted_date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return the list along with any metadata needed for the UI
            return View(results);
        }

        public ActionResult JobDetails(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int seekerId = Convert.ToInt32(Session["UserId"]);
            var job_data = db.JOBS.FirstOrDefault(j => j.job_id == id);
            if (job_data == null) return HttpNotFound();
            var employer_data = db.USERS.FirstOrDefault(u => u.UserId == job_data.employer_id);
            var employer_complete = db.EMPLOYERS.FirstOrDefault(e => e.employer_id == employer_data.UserId);

            var myViewModel = new JobDetailsViewModel { 
                    Job = job_data,
                    Employer = employer_data,
                    Employerr_detail = employer_complete
            };
    

            //if (jobWithUser == null)
            //    return HttpNotFound();

            // Check if already applied
            bool alreadyApplied = db.APPLICATIONS
                .Any(a => a.job_id == id && a.seeker_id == seekerId);

            ViewBag.HasApplied = alreadyApplied;

            return View(myViewModel);
        }


        public ActionResult PostJob()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostJob(JobViewModel model)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                JOB job = new JOB
                {
                    employer_id = Convert.ToInt32(Session["UserId"]),
                    job_title = model.JobTitle,
                    job_description = model.JobDescription,
                    requirements = model.Requirements,
                    location = model.Location,
                    salary_range = model.SalaryRange,
                    job_type = model.JobType,
                    posted_date = DateTime.Now,
                    expiry_date = model.ExpiryDate,
                    status = "ACTIVE"
                };

                db.JOBS.Add(job);
                db.SaveChanges(); 

                
                if (!string.IsNullOrEmpty(model.RequiredSkills))
                {
                    var skillNames = model.RequiredSkills.Split(',')
                                        .Select(s => s.Trim())
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .ToList();

                    foreach (var name in skillNames)
                    {
                        var existingSkill = db.Skills.FirstOrDefault(s => s.skillName.ToLower() == name.ToLower());

                        int currentSkillId;

                        if (existingSkill == null)
                        {
                            var newSkill = new Skill { skillName = name };
                            db.Skills.Add(newSkill);
                            currentSkillId = newSkill.SkillID;

                            JOB_SKILLS js = new JOB_SKILLS
                            {
                                job_id = job.job_id,
                                skillID = currentSkillId
                            };
                            db.JOB_SKILLS.Add(js);
                        }
                        else
                        {
                            currentSkillId = existingSkill.SkillID;
                            JOB_SKILLS js = new JOB_SKILLS
                            {
                                job_id = job.job_id,
                                skillID = currentSkillId
                            };
                            db.JOB_SKILLS.Add(js);
                        }

                    }

                    db.SaveChanges();
                }

                return RedirectToAction("ManageJobs");
            }

            return View(model);
        }

        // GET: Manage Jobs
        public ActionResult ManageJobs()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int uid = Convert.ToInt32(Session["UserId"]);
            var jobs = db.JOBS
                         .Where(j => j.employer_id == uid)
                         .OrderByDescending(j => j.posted_date)
                         .ToList();

            return View(jobs);
        }

        public ActionResult EditJob(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var job = db.JOBS.FirstOrDefault(j => j.job_id == id);
            if (job == null)
                return HttpNotFound();

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }


            // 1. Get User Skills
            var skills_job = db.JOB_SKILLS
                            .Where(us => us.job_id == id)
                            .Join(db.Skills,
                                  us => us.skillID,
                                  s => s.SkillID,
                                  (us, s) => s)
                            .ToList();
            var vm = new JobEditViewModel
            {
                Job = job,
                Emp_job_skills = skills_job
            };

            return View(vm);   
        }


        // POST: Edit Job
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Bind(Prefix = "Job")] lagane se MVC ko pata chal jayega ke 
        // form se "Job.Something" wala data pick karna hai
        public ActionResult EditJob([Bind(Prefix = "Job")] JOB jobData)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var job = db.JOBS.FirstOrDefault(j => j.job_id == jobData.job_id);
                if (job == null)
                    return HttpNotFound();

                // Data Update
                job.job_title = jobData.job_title;
                job.job_description = jobData.job_description;
                job.requirements = jobData.requirements;
                job.location = jobData.location;
                job.salary_range = jobData.salary_range;
                job.job_type = jobData.job_type;
                job.expiry_date = jobData.expiry_date;

                db.SaveChanges();

                // Wapis isi page par redirect karega
                return RedirectToAction("EditJob", new { id = jobData.job_id });
            }

            // Agar model state valid nahi hai tou VM dubara populate karein
            var vm = new JobEditViewModel
            {
                Job = jobData,
                Emp_job_skills = db.JOB_SKILLS
                        .Where(us => us.job_id == jobData.job_id)
                        .Join(db.Skills, us => us.skillID, s => s.SkillID, (us, s) => s)
                        .ToList()
            };
            return View(vm);
        }


        [HttpPost]
        public ActionResult AddSkill(string SkillName , int current_job_id)
        {

            var check_skill = db.Skills
                .FirstOrDefault(st => st.skillName == SkillName);
            int finalSkillId;

            if (check_skill == null)
            {
                Skill s = new Skill { skillName = SkillName };
                db.Skills.Add(s);
                db.SaveChanges();
                finalSkillId = s.SkillID;

            }
            else
            {
                finalSkillId = check_skill.SkillID;
            }
            var alreadyHasSkill = db.JOB_SKILLS.Any(us => us.job_id == current_job_id && us.skillID == finalSkillId);
            if (!alreadyHasSkill)
            {
                JOB_SKILLS u = new JOB_SKILLS
                {
                    job_id = current_job_id,
                    skillID = finalSkillId
                };
                db.JOB_SKILLS.Add(u);
                db.SaveChanges();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }


        // GET: Delete Job
        public ActionResult DeleteJob(int id)
        {
            var job = db.JOBS.FirstOrDefault(j => j.job_id == id);
            if (job != null)
            {
                var all_applications = db.APPLICATIONS.Where(j => j.job_id == id).ToList();
                if (all_applications.Any())
                {
                    db.APPLICATIONS.RemoveRange(all_applications);
                }

                db.JOBS.Remove(job);
                db.SaveChanges();
            }

            return RedirectToAction("ManageJobs");
        }

        public ActionResult DeleteSkill(int skillId, int jobId)
        {
            var uskill = db.JOB_SKILLS.FirstOrDefault(u => u.job_id == jobId && u.skillID == skillId);
            if (uskill != null)
            {
                db.JOB_SKILLS.Remove(uskill);
                db.SaveChanges();

                //var skill = db.Skills.Find(skillId);
                //if (skill != null)
                //{
                //    db.Skills.Remove(skill);
                //    db.SaveChanges();
                //}
            }

            return RedirectToAction("ManageJobs");
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
