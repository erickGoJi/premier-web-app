using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.DependentInformations
{
    public class DocumentDependentInformationDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public int? Relationship { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public string Comment { get; set; }
        public int? DependentInformation { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual DependentInformationDto DependentInformationNavigation { get; set; }
    }

    public class DocumentDependentInformationSelectDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public int? DocumentType { get; set; }
        public int? Relationship { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public string Comment { get; set; }
        public int? DependentInformation { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual CatCountryDto CountryOriginNavigation { get; set; }
        public virtual DependentInformationDto DependentInformationNavigation { get; set; }
    }
}
