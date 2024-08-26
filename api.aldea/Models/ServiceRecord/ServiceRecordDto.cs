using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ServiceRecord
{
    public class ServiceRecordDto
    {
        public int Id { get; set; }
        public string NumberServiceRecord { get; set; }
        public DateTime InitialAutho { get; set; }
        public DateTime InithialAuthoAcceptance { get; set; }
        public int PartnerId { get; set; }
        public int ClientId { get; set; }
        public string ClientFileNumber { get; set; }
        public int? AuthorizedByImmigration { get; set; }
        public int? AuthorizedByRelocation { get; set; }
        public bool? CopyOnEmailImmigration { get; set; }
        public bool? CopyOnEmailRelocation { get; set; }
        public bool? SpocImmigration { get; set; }
        public bool? SpocRelocation { get; set; }
        public bool? Vip { get; set; }
        public bool? Urgent { get; set; }
        public bool? ConfidentialMove { get; set; }
        public string SpecialIntructions { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Office { get; set; }
    }
}
