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

            return File(filePath, "application/octet-stream", fileName);
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
