using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class JobSeekerApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public int EmployerId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime? AppliedDate { get; set; }
        public string Status { get; set; }

        // Interview related fields
        public DateTime? InterviewDate { get; set; }
        public string InterviewLocation { get; set; }
        public string InterviewNotes { get; set; }
        public string RejectionFeedback { get; set; }
    }

}