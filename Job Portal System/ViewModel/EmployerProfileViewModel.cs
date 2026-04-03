using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class EmployerProfileViewModel
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string CompanyDescription { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }

        public string CompanyLogo { get; set; }
    }

}