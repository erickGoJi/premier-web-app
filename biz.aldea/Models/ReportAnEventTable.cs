using System;

namespace biz.premier.Models
{
    public class ReportAnEventTable
    {
        public int Id { get; set; }
        public DateTime IssueReportDate { get; set; }
        public DateTime IssueCloseDate { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int Photos { get; set; }
        public string QuoteApproval { get; set; }
    }
}