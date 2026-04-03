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
            if (!IsLoggedIn())
            {
                filterContext.Result = RedirectToAction("Login", "Account");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        // ----------- DASHBOARD ----------- //
        public ActionResult Dashboard()
        {
            var stats = new
            {
                TotalApplications = db.APPLICATIONS.Count(),
                SavedJobs = db.SAVED_JOBS.Count(),
                TotalSkills = db.Skills.Count()
            };

            return View(stats);
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
