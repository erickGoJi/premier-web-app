using api.premier.Models.PredicisionOrientation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestPayment
{
    public class ExtensionPredecisionOrientationDto
    {
        public int Id { get; set; }
        public int? PredecisionOrientationId { get; set; }
        public string RequestedBy { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTime? AuthoDate { get; set; }
        public DateTime? AuthoAcceptanceDate { get; set; }
        public int? Tine { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual PredecisionOrientationDto PredecisionOrientation { get; set; }
    }
}
