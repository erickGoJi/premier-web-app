using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.SupplierPartnerProfileService
{
    public class SupplierPartnerProfileServiceDto
    {
        public int Id { get; set; }
        public int? Type { get; set; }
        public string Photo { get; set; }
        public string PhotoExtension { get; set; }
        public int? Status { get; set; }
        public int? ServiceLine { get; set; }
        public bool? Immigration { get; set; }
        public bool? Relocation { get; set; }
        public int? SupplierType { get; set; }
        public string ComercialName { get; set; }
        public string LegalName { get; set; }
        public bool? LuxuryVip { get; set; }
        public DateTime? SupplierSince { get; set; }
        public string About { get; set; }
        public decimal? AmountPerHour { get; set; }
        public int? Currency { get; set; }
        public int? CreditTerms { get; set; }
        public decimal? TaxesPercentage { get; set; }
        public string TaxeName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CatServiceLineDto ServiceLineNavigation { get; set; }
        public virtual CatSupplierPartnerProfileStatusDto StatusNavigation { get; set; }
        public virtual CatSupplierTypeDto SupplierTypeNavigation { get; set; }
        public virtual ICollection<AreasCoverageServiceDto> AreasCoverageServices { get; set; }
        public virtual ICollection<SupplierPartnerDetailDto> SupplierPartnerDetails { get; set; }
    }
}
