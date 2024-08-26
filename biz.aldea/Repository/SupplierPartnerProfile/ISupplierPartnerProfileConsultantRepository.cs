using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace biz.premier.Repository.SupplierPartnerProfile
{
    public interface ISupplierPartnerProfileConsultantRepository : IGenericRepository<SupplierPartnerProfileConsultant>
    {
        Task<SupplierPartnerProfileConsultant> UpdatedCustom(SupplierPartnerProfileConsultant supplierPartnerProfileConsultant, int key);
        SupplierPartnerProfileConsultant GetCustom(int key);
        ActionResult GetConsultantContactsConsultants(int? supplierPartner, int country, int city, int serviceLine);
        ActionResult GetSupplierPartnerConsultant(int? country, int? city, int? serviceLine);
        ActionResult GetCompany();
        bool DeletePaymentInformation(int id);
        bool DeleteWireTransfer(int id);
    }
}
