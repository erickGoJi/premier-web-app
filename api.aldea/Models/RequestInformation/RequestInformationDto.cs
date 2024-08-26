using api.premier.Models.RequestInformationDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestInformation
{
    public class RequestInformationDto
    {
        public RequestInformationDto()
        {
            RequestInformationDocuments = new HashSet<RequestInformationDocumentDto>();
        }
        public int Id { get; set; }
        public string SentTo { get; set; }
        public List<string> SentsTo { get; set; }
        public int? AuthorizedBy { get; set; }
        public bool? NeedsAssessment { get; set; }
        public bool? ImmigrationProfile { get; set; }
        public bool? HousingSpecification { get; set; }
        public int? House { get; set; }
        public string Comment { get; set; }
        public int? ServiceRecordId { get; set; }
        public DateTime? Due { get; set; }
        public virtual ICollection<RequestInformationDocumentDto> RequestInformationDocuments { get; set; }
    }
    public class RequestInformationSelectDto
    {
        public int Id { get; set; }
        public string SentTo { get; set; }
        public int? AuthorizedBy { get; set; }
        public bool? NeedsAssessment { get; set; }
        public bool? ImmigrationProfile { get; set; }
        public bool? HousingSpecification { get; set; }
        public int? House { get; set; }
        public int? ServiceRecordId { get; set; }
        public DateTime? Due { get; set; }
        public virtual ICollection<RequestInformationDocumentDto> RequestInformationDocuments { get; set; }
        public virtual ICollection<TypeHousingAvailible> HousingAvailible { get; set; }
    }
    public class TypeHousingAvailible
    {
        public int Id { get; set; }
        public string TypeHousing { get; set; }
        public bool availible { get; set; }
        public int? WorkOrderServices { get; set; }
    }
}
