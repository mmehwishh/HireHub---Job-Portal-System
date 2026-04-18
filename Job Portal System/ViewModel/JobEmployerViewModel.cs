using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Job_Portal_System.ViewModel
{
    public class JobEmployerViewModel
    {
        public JOB JobData { get; set; }
        public EMPLOYER EmpData { get; set; }
    }
}