using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Job_Portal_System;

namespace Job_Portal_System.Controllers
{
    public class AccountController : Controller
    {
        private JobDBEntities3 db = new JobDBEntities3();


        // ---------- LOGIN ----------
        public ActionResult Login()
        {
            // If user is already logged in, redirect to dashboard
            if (Session["Email"] != null)
            {
                string userRole = Session["UserRole"] != null ? Session["UserRole"].ToString() : null;

                if (userRole == "Employer")
                    return RedirectToAction("Dashboard", "Employeer");
                else
                    return RedirectToAction("Dashboard", "JobSeeker");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.USERS.FirstOrDefault(u =>
                (u.email == email || u.username == email) && u.password_hash == password);

            if (user != null)
            {
                // Set session variables
                Session["UserId"] = user.UserId;
                Session["UserName"] = user.full_name;
                Session["Email"] = user.email;
                Session["UserRole"] = user.role;

                // Set authentication cookie
                FormsAuthentication.SetAuthCookie(user.username, false);

                // Redirect based on role
                if (user.role == "Employer")
                    return RedirectToAction("Dashboard", "Employeer");
                else
                    return RedirectToAction("Dashboard", "JobSeeker");
            }

            ViewBag.Error = "Invalid Email or password!";
            return View();
        }

        // ---------- LOGOUT ----------
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // ---------- REGISTER ----------
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(USER user, string confirmPassword, string selectedRole)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(user.username) ||
                string.IsNullOrWhiteSpace(user.email) ||
                string.IsNullOrWhiteSpace(user.password_hash) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(selectedRole))
            {
                ViewBag.Error = "Please fill in all the required fields.";
                return View();
            }

            if (user.password_hash != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            // Check if username or email already exists
            var existUser = db.USERS.FirstOrDefault(u => u.username == user.username || u.email == user.email);
            if (existUser != null)
            {
                ViewBag.Error = "Username or Email already exists.";
                return View();
            }

            // Optional: hash the password
            // user.password_hash = BCrypt.Net.BCrypt.HashPassword(user.password_hash);

            user.created_at = DateTime.Now;
            user.role = selectedRole;

            // Add to USERS table
            db.USERS.Add(user);
            db.SaveChanges(); // this saves and generates UserId

            // Depending on role, create a related record
            if (selectedRole == "Employer" || selectedRole == "Employeer")
            {
                var employer = new EMPLOYER
                {
                    employer_id = user.UserId, // same as UserId
                    company_name = "",
                    industry = "",
                    company_description = "",
                    website = "",
                    location = "",
                    companyLogo = ""
                };
                db.EMPLOYERS.Add(employer);
            }
            else if (selectedRole == "Job Seeker")
            {
                var seeker = new JOB_SEEKER_PROFILE
                {
                    seeker_id = user.UserId, // same as UserId
                    dob = null,
                    gender = "",
                    phone = "",
                    address = "",
                    experience_level = "",
                    education = "",
                    resume_file = "",
                    ProfilePicturePath = "",
                    Bio = ""
                };
                db.JOB_SEEKER_PROFILE.Add(seeker);
            }

            db.SaveChanges();

            ViewBag.Message = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }


        // ---------- SETTINGS ----------
        public ActionResult Settings()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Login");
            }

            string email = Session["Email"].ToString();
            var user = db.USERS.FirstOrDefault(x => x.email == email);

            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }
    }
}
