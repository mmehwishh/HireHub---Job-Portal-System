using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.ViewModel
{
    public class JobViewModel
    {
        public int JobId { get; set; }

        [Required]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Required]
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }

        [Display(Name = "Requirements")]
        public string Requirements { get; set; }

        [Required]
        public string Location { get; set; }

        [Display(Name = "Salary Range")]
        public string SalaryRange { get; set; }

        [Required]
        [Display(Name = "Job Type")]
        public string JobType { get; set; } // Full-time, Part-time, Internship

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
        public string RequiredSkills { get; set; }
    }
}