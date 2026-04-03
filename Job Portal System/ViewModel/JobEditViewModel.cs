using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class JobEditViewModel
    {
        public JOB Job { get; set; }
        public List<Skill> Emp_job_skills { get; set; }
    }

}