using api.premier.Models.Catalogos;
using api.premier.Models.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Invoice
{
    public class DocumentInvoiceDto
    {
        public int Id { get; set; }
        public int? Type { get; set; }
        public int? Invoice { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuingAuthority { get; set; }
        public int? CountryOrigin { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatCountryDto CountryOriginNavigation { get; set; }
        //public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        //public virtual InvoiceDto InvoiceNavigation { get; set; }
    }
}
