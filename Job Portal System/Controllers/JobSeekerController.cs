using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Job_Portal_System;
using Job_Portal_System.ViewModel;


namespace Job_Portal_System.Controllers
{
    public class JobSeekerController : Controller
    {
        JobDBEntities3 db = new JobDBEntities3();

        // ---------- HELPER METHOD ----------
        // Checks if the user is logged in
        private bool IsLoggedIn()
        {
            return Session["Email"] != null;
        }

        // ---------- OVERRIDE ON ACTION EXECUTION ----------
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Redirect if not logged in (existing)
            if (!IsLoggedIn())
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }

            // Populate common ViewBag items used by the Job Seeker layout
            try
            {
                if (Session["UserId"] != null)
                {
                    int currentUserId = Convert.ToInt32(Session["UserId"]);

                    // USER record (may be null in edge cases)
                    var user = db.USERS.Find(currentUserId);
                    var displayName = user != null ? (user.full_name ?? user.username) : (Session["UserName"] as string ?? "User");

                    // Derive initials if you want (first letters of first two name parts)
                    string initials = "";
                    if (!string.IsNullOrWhiteSpace(displayName))
                    {
                        var parts = displayName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        initials = parts.Length == 0 ? "AS" : (parts.Length == 1 ? parts[0].Substring(0, 1) : (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)));
                        initials = initials.ToUpperInvariant();
                    }

                    filterContext.Controller.ViewBag.UserName = displayName;
                    filterContext.Controller.ViewBag.UserInitials = initials;

                    // Unread message count (if you have a read flag replace this with unread-only filter)
                    var incomingCount = db.Messages.Count(m => m.ReceiverID == currentUserId);
                    filterContext.Controller.ViewBag.UnreadMessages = incomingCount;

                    // Unread notifications — your code uses read_status == "UNREAD"
                    var unreadNotifs = db.NOTIFICATIONS.Count(n => n.user_id == currentUserId && ((n.read_status ?? "").ToUpper() == "UNREAD"));
                    filterContext.Controller.ViewBag.UnreadNotifications = unreadNotifs;
                }
            }
            catch
            {
                // avoid throwing from layout population; fall back to defaults
                filterContext.Controller.ViewBag.UserName = filterContext.Controller.ViewBag.UserName ?? "User";
                filterContext.Controller.ViewBag.UserInitials = filterContext.Controller.ViewBag.UserInitials ?? "AS";
                filterContext.Controller.ViewBag.UnreadMessages = filterContext.Controller.ViewBag.UnreadMessages ?? 0;
                filterContext.Controller.ViewBag.UnreadNotifications = filterContext.Controller.ViewBag.UnreadNotifications ?? 0;
            }

            base.OnActionExecuting(filterContext);
        }

        // ----------- DASHBOARD ----------- //
        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int uid = Convert.ToInt32(Session["UserId"]);

            // Basic counts
            var totalApplications = db.APPLICATIONS.Count(a => a.seeker_id == uid);
            var savedJobsCount = db.SAVED_JOBS.Count(s => s.seeker_id == uid);
            var totalSkills = db.USER_SKILLS.Count(us => us.seeker_id == uid);
            var unreadNotifs = db.NOTIFICATIONS.Count(n => n.user_id == uid && ((n.read_status ?? "").ToUpper() == "UNREAD"));

            int unreadMessages = 0;
            try
            {
                // Replace with your real messages table if present
                // unreadMessages = db.Messages.Count(m => m.ReceiverID == uid && m.Read == false);
            }
            catch { unreadMessages = 0; }

            // Recent applications (for job seeker)
            var recentApplications = db.APPLICATIONS
                                       .Include(a => a.JOB)
                                       .Where(a => a.seeker_id == uid)
                                       .OrderByDescending(a => a.applied_date)
                                       .Take(6)
                                       .ToList();

            // Saved jobs (with job details)
            var savedJobs = db.SAVED_JOBS
                              .Include(s => s.JOB)
                              .Where(s => s.seeker_id == uid)
                              .OrderByDescending(s => s.saved_at)
                              .Take(6)
                              .ToList();

            // Recommended jobs based on user's skills (best-effort)
            var skillIds = db.USER_SKILLS
                             .Where(us => us.seeker_id == uid)
                             .Select(us => us.SkillID)
                             .ToList();

            var recommendedJobs = new List<JOB>();
            if (skillIds.Any())
            {
                recommendedJobs = db.JOB_SKILLS
                                    .Where(js => skillIds.Contains(js.skillID))
                                    .Select(js => js.JOB)
                                    .Distinct()
                                    .OrderByDescending(j => j.posted_date)
                                    .Take(8)
                                    .ToList();
            }

            // Recent jobs (global popular / latest) fallback
            var recentJobs = db.JOBS
                               .OrderByDescending(j => j.posted_date)
                               .Take(8)
                               .ToList();

            // Build view model
            var user = db.USERS.Find(uid);
            var vm = new JobSeekerDashboardViewModel
            {
                FullName = user?.full_name ?? (Session["UserName"] as string ?? "Job Seeker"),
                Initials = (user != null && !string.IsNullOrWhiteSpace(user.full_name)) ?
                           string.Concat(user.full_name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Take(2)
                                                     .Select(p => p[0])).ToUpper() : "JS",
                TotalApplications = totalApplications,
                SavedJobsCount = savedJobsCount,
                TotalSkills = totalSkills,
                UnreadNotifications = unreadNotifs,
                UnreadMessages = unreadMessages,
                RecentApplications = recentApplications,
                SavedJobs = savedJobs,
                RecommendedJobs = recommendedJobs.Any() ? recommendedJobs : recentJobs,
                RecentJobs = recentJobs
            };

            return View(vm);
        }


        // ----------- MY PROFILE ----------- //
        public ActionResult MyProfile()
{
    if (Session["Email"] == null)
        return RedirectToAction("Login", "Account");

    int uid = Convert.ToInt32(Session["UserId"]);
    string email = (Session["Email"] != null) ? Session["Email"].ToString() : null;


    var user = db.USERS.FirstOrDefault(u => u.email == email);
    var seekerProfile = db.JOB_SEEKER_PROFILE.FirstOrDefault(u => u.seeker_id == user.UserId);

    if (seekerProfile == null)
    {
        // User is logged in but has no profile yet
        return RedirectToAction("CreateProfile", "JobSeeker");
    }

    var vm = new JobSeekerProfileViewModel
    {
        FullName = user.full_name,
        Bio = seekerProfile.Bio,
        Resume = seekerProfile.resume_file,
        ProfileImage = seekerProfile.ProfilePicturePath,

        // New fields
        dob = seekerProfile.dob,
        gender = seekerProfile.gender,
        phone = seekerProfile.phone,
        address = seekerProfile.address,
        experience_level = seekerProfile.experience_level,
        education = seekerProfile.education
    };

    return View(vm);
}


        [HttpPost]
        public ActionResult MyProfile(JobSeekerProfileViewModel model, HttpPostedFileBase ResumeFile, HttpPostedFileBase ImageFile)
        {
            int uid = Convert.ToInt32(Session["UserId"]);
            var user = db.USERS.FirstOrDefault(u => u.UserId == uid);
            var seeker = db.JOB_SEEKER_PROFILE.FirstOrDefault(s => s.seeker_id == uid);

            if (user == null || seeker == null)
                return RedirectToAction("Login", "Account");

            // Update full name in USERS table
            user.full_name = model.FullName;

            // Update JobSeeker profile
            seeker.Bio = model.Bio;
            seeker.dob = model.dob;
            seeker.gender = model.gender;
            seeker.phone = model.phone;
            seeker.address = model.address;
            seeker.experience_level = model.experience_level;
            seeker.education = model.education;

            // Upload Resume
            if (ResumeFile != null && ResumeFile.ContentLength > 0)
            {
                string resumePath = "/Resumes/" + ResumeFile.FileName;
                ResumeFile.SaveAs(Server.MapPath(resumePath));
                seeker.resume_file = resumePath;
            }

            // Upload Profile Picture
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string imgPath = "/Images/" + ImageFile.FileName;
                ImageFile.SaveAs(Server.MapPath(imgPath));
                seeker.ProfilePicturePath = imgPath;
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


        public ActionResult ViewProfile()
{
    if (Session["Email"] == null)
        return RedirectToAction("Login", "Account");

    int uid = Convert.ToInt32(Session["UserId"]);
    string email = (Session["Email"] != null) ? Session["Email"].ToString() : null;


    var user = db.USERS.FirstOrDefault(u => u.email == email);
    var seekerProfile = db.JOB_SEEKER_PROFILE.FirstOrDefault(u => u.seeker_id == uid);

    if (seekerProfile == null)
    {
        return RedirectToAction("CreateProfile", "JobSeeker");
    }

    var vm = new JobSeekerProfileViewModel
    {
        FullName = user.full_name,
        Bio = seekerProfile.Bio,
        Resume = seekerProfile.resume_file,
        ProfileImage = seekerProfile.ProfilePicturePath,
        dob = seekerProfile.dob,
        gender = seekerProfile.gender,
        phone = seekerProfile.phone,
        address = seekerProfile.address,
        experience_level = seekerProfile.experience_level,
        education = seekerProfile.education
    };

    return View(vm);
}


        // ----------- SKILLS ----------- //
        public ActionResult ManageSkills()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int currentUserId = Convert.ToInt32(Session["UserId"]);

            // 1. Get User Skills
            var skills = db.USER_SKILLS
                            .Where(us => us.seeker_id == currentUserId)
                            .Join(db.Skills,
                                  us => us.SkillID,
                                  s => s.SkillID,
                                  (us, s) => s)
                            .ToList();

            // 2. Get Recommended Jobs based on these skills
            var skillIds = skills.Select(s => s.SkillID).ToList();

            var jobIds = db.JOB_SKILLS
                            .Where(js => skillIds.Contains(js.skillID))
                            .Select(js => js.job_id)
                            .Distinct()
                            .ToList();

            var recommendedJobs = db.JOBS
                                    .Where(j => jobIds.Contains(j.job_id))
                                    .ToList();

            // 3. Pass Jobs via ViewBag and Skills via Model
            ViewBag.RecommendedJobs = recommendedJobs;

            return View(skills);
        }

        [HttpPost]
        public ActionResult AddSkill(string SkillName)
        {

            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int currentUserId = Convert.ToInt32(Session["UserId"]);

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
            var alreadyHasSkill = db.USER_SKILLS.Any(us => us.seeker_id == currentUserId && us.SkillID == finalSkillId);
            if (!alreadyHasSkill)
            {
                USER_SKILLS u = new USER_SKILLS
                {
                    seeker_id = currentUserId,
                    SkillID = finalSkillId
                };
                db.USER_SKILLS.Add(u);
                db.SaveChanges();
            }
            return RedirectToAction("ManageSkills");
        }

        public ActionResult DeleteSkill(int id)
        {
            var uskill = db.USER_SKILLS.FirstOrDefault(u=> u.SkillID  == id);
            if (uskill != null)
            {
                db.USER_SKILLS.Remove(uskill);
                db.SaveChanges();

                var skill = db.Skills.Find(id);
                if (skill != null)
                {
                    db.Skills.Remove(skill);
                    db.SaveChanges();
                }
            }
            
            return RedirectToAction("ManageSkills");
        }
    }
}
