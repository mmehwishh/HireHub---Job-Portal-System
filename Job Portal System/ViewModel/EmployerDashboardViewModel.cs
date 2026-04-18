using System;
using System.Collections.Generic;
using Job_Portal_System;

namespace Job_Portal_System.ViewModel
{
    public class EmployerDashboardViewModel
    {
        public EMPLOYER Employer { get; set; }

        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplicants { get; set; }

        public int UnreadNotifications { get; set; }
        public int UnreadMessages { get; set; }

        public List<JOB> RecentJobs { get; set; } = new List<JOB>();
        public List<APPLICATION> RecentApplications { get; set; } = new List<APPLICATION>();
    }
}