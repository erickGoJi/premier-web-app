using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Service
{
    public class DocumentServiceCountryDto
    {
        public int Id { get; set; }
        public int? ServiceCountry { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public int? Status { get; set; }
        public int? Privacy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Name { get; set; }

    }
    public class DocumentServiceCountrySelectDto
    {
        public int Id { get; set; }
        public int? ServiceCountry { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public int? Status { get; set; }
        public int? Privacy { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        public virtual CatPrivacyDto PrivacyNavigation { get; set; }
        public virtual CatDocumentStatusDto StatusNavigation { get; set; }

    }
}
