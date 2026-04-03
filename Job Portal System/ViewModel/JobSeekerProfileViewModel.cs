using System;

namespace Job_Portal_System.ViewModel
{
    public class JobSeekerProfileViewModel
    {
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Resume { get; set; }
        public string ProfileImage { get; set; }

        // New fields
        public DateTime? dob { get; set; }
        public string gender { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string experience_level { get; set; }
        public string education { get; set; }
    }
}
