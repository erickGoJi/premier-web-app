using System;

namespace api.premier.Models.ServiceRecord
{
    public class ServiceRecordAppDto
    {
        public int Id { get; set; }
        public string NumberServiceRecord { get; set; }
        public DateTime InitialAutho { get; set; }
        public DateTime InithialAuthoAcceptance { get; set; }
        public int PartnerId { get; set; }
        public int ClientId { get; set; }
    }
}
