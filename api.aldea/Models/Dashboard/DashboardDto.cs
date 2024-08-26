using System;

namespace api.premier.Models.Dashboard
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

        //"serviceLine": "Relocation",
        //"homeCountry": "COLOMBIA",
        //"hostCountry": "Mexico",
        //"homeCity": "Bogotá",
        //"hostCity": "Mexico City",
        //"homeCountryId": 82,
        //"hostCountryId": 1075,
        //"homeCityId": 452466,
        //"hostCityId": 1061,
        //"name": "Partner International",
        //"partnerId": 1183,
        //"partnerAvatar": "Files/ClientProfile/465fd303-e765-4dfb-a439-b1c46103e974.png",
        //"client": "Frank & Copernico Lab",
        //"clientId": 1184,
        //"clientAvatar": "Files/ClientProfile/5c8dce47-d872-4340-8e2b-5cd5f2493165.png",
        //"assigneeName": "Joaquin Quezada",
    }
}
