using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.SupplierPartnerProfile
{
    public interface ISupplierPartnerProfileServiceRepository : IGenericRepository<SupplierPartnerProfileService>
    {
        SupplierPartnerProfileService UpdatedCustom(SupplierPartnerProfileService supplierPartnerProfileService, int key);
        SupplierPartnerProfileService GetCustom(int key);
        ActionResult GetSupplierPartners(int? supplierCategory, int? partnerType, int? country, int? city, int? status);
        string HashPassword(string password);
        ActionResult GetSupplierPartnerServiceByServices(int workOrderService, int? supplierType, int? serviceLine);

        ActionResult GetServiceProviderByServiceId(int workOrderService);

        ActionResult GetServProvByServiceTypeCountry(int workOrderService, int type);
        ActionResult GetAdministrativeContactsServiceBySupplierPartner(int workOrderService, int supplierPartner);
        ActionResult GetConsultantContactsService(int? supplierPartner, int? supplierType);

        ActionResult GetAdmintContactsServiceProv(int supplierPartner, int workOrderService);
        ActionResult GetSupplierPartnerServiceInvoice(int sr);
        ActionResult GetSupplierPartnersBySR(int sr);
        bool DeleteAdministrativeContact(int id);
        bool DeleteConsultantContact(int id);
        bool DeletePaymentInformation(int id);
        bool DeleteWireTransfer(int id);
    }
}
