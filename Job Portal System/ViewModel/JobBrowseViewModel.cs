using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class JobBrowseViewModel
    {
        public JOB Job { get; set; }
        public bool IsApplied { get; set; }
        public bool IsSaved { get; set; }
    }

}