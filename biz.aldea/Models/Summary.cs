using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Models
{
    public class Summary
    {
        public string TimeService { get; set; }
        public List<ReportSummary> reportSummaries { get; set; }
    }
    public class ReportSummary
    {
        public string Service { get; set; }
        public string ServiceId { get; set; }
        public int TimeRemining { get; set; }
        public string Saved { get; set; }
    }
}
