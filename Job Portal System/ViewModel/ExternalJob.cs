using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Job_Portal_System.ViewModel
{
    public class ExternalJob
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class AdzunaResponse
    {
        public List<JobResult> results { get; set; }
    }

    public class JobResult
    {
        public string title { get; set; }
        public Company company { get; set; }
        public Location location { get; set; }
        public string redirect_url { get; set; }
    }

    public class Company
    {
        public string display_name { get; set; }
    }

    public class Location
    {
        public string display_name { get; set; }
    }

}