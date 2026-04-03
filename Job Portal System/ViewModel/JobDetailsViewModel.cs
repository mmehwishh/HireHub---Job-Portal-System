using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class JobDetailsViewModel
    {
        public JOB Job { get; set; }
        public USER Employer { get; set; }
        public EMPLOYER Employerr_detail { get; set; }
        public List<JOB> RecommendedJobs { get; set; }
    }

}