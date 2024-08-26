using System;

namespace api.premier.Models.ClientPartnerProfile
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public string PremierSalesForce { get; set; }
        public string Activity { get; set; }
        public DateTime Date { get; set; }

        public virtual ClientPartnerProfileDto IdClientPartnerProfileNavigation { get; set; }
    }
}