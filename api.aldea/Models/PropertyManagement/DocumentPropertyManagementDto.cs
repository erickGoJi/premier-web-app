using System;
using api.premier.Models.Catalogos;
using api.premier.Models.DependentInformations;

namespace api.premier.Models.PropertyManagement
{
    public class DocumentPropertyManagementDto
    {
        public int Id { get; set; }
        public int PropertyManagement { get; set; }
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
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatCountryDto CountryOriginNavigation { get; set; }
        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        public virtual PropertyManagementDto PropertyManagementNavigation { get; set; }
        public virtual DependentInformationDto RelationshipNavigation { get; set; }
    }
}