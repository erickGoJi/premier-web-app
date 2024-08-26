using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.HomeFinding
{
    public class DocumentHomeFindingDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public int? Relationship { get; set; }
        public int? Privacy { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public string Comment { get; set; }
        public int HomeFindingId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //public virtual HomeFindingDto HomeFinding { get; set; }
    }
    public partial class DocumentHomeFindingSelectDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public int? DocumentType { get; set; }
        public int? Relationship { get; set; }
        public int? Privacy { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public string Comment { get; set; }
        public int HomeFindingId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCountryDto CountryOriginNavigation { get; set; }
        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        //public virtual HomeFindingDto HomeFinding { get; set; }
        public virtual DependentInformationDto RelationshipNavigation { get; set; }
    }
}
