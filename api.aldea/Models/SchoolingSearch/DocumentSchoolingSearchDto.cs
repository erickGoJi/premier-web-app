using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SchoolingSearch
{
    public class DocumentSchoolingSearchDto
    {
        public int Id { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public int? Privacy { get; set; }
        public int? Relationship { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public string Comment { get; set; }
        public int SchoolingSearchId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual SchoolingSearchDto SchoolingSearch { get; set; }
    }
    public class DocumentSchoolingSearchSelectDto
    {
        public int Id { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public int? Privacy { get; set; }
        public int? Relationship { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public string Comment { get; set; }
        public int SchoolingSearchId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCountryDto CountryOriginNavigation { get; set; }
        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        public virtual DependentInformationDto RelationshipNavigation { get; set; }
        public virtual SchoolingSearchDto SchoolingSearch { get; set; }
    }

}
