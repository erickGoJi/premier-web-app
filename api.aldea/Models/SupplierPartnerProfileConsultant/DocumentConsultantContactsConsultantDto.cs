using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileConsultant
{
    public class DocumentConsultantContactsConsultantDto
    {
        public int Id { get; set; }
        public int? ConsultantContactsService { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Location { get; set; }
        public int? Privacy { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual ConsultantContactsConsultantDto ConsultantContactsServiceNavigation { get; set; }
        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        public virtual CatPrivacyDto PrivacyNavigation { get; set; }
        public virtual CatDocumentStatusDto StatusNavigation { get; set; }
    }
}
