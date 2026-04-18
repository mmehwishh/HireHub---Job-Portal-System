using System;
using System.Collections.Generic;
using Job_Portal_System;

namespace Job_Portal_System.ViewModel
{
    public class JobSeekerDashboardViewModel
    {
        public string FullName { get; set; }
        public string Initials { get; set; }

        public int TotalApplications { get; set; }
        public int SavedJobsCount { get; set; }
        public int TotalSkills { get; set; }
        public int UnreadNotifications { get; set; }
        public int UnreadMessages { get; set; }

        public List<JOB> RecommendedJobs { get; set; } = new List<JOB>();
        public List<JOB> RecentJobs { get; set; } = new List<JOB>();
        public List<APPLICATION> RecentApplications { get; set; } = new List<APPLICATION>();
        public List<SAVED_JOBS> SavedJobs { get; set; } = new List<SAVED_JOBS>();
    }
}