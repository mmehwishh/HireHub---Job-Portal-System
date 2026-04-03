using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class SavedJobViewModel
    {
        public int SavedJobID { get; set; }
        public int JobID { get; set; }
        public int savedSeekerId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public DateTime SavedAt { get; set; }
        public int Status { get; set; } // 1=Saved, 2=Applied, 3=Interview, etc.
        public string Notes { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsExpired => Deadline.HasValue && Deadline < DateTime.Now;
    }

}