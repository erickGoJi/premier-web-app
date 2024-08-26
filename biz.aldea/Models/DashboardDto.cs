using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Models
{
    public class DashboardDto
    {
        public int id { get; set; }
        public string numberServiceRecord { get; set; }
        public bool vip { get; set; }
        public bool confidentialMove { get; set; }
        public bool urgent { get; set; }
        public string status { get; set; }
        public int statusId { get; set; }
        public DateTime initialAutho { get; set; }
        public int serviceLineId { get; set; }
        public int workOrders { get; set; }
        public string serviceLine { get; set; }
        public string homeCountry { get; set; }
        public string hostCountry { get; set; }
        public string homeCity { get; set; }
        public string hostCity { get; set; }
        public int homeCountryId { get; set; }
        public int hostCountryId { get; set; }
        public int homeCityId { get; set; }
        public int hostCityId { get; set; }
        public string name { get; set; }
        public int partnerId { get; set; }
        public string partnerAvatar { get; set; }
        public string client { get; set; }
        public int clientId { get; set; }
        public string clientAvatar { get; set; }
        public string assigneeName { get; set; }
        public int total_services { get; set; }
    }
}
