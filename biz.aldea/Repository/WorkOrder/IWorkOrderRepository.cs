using biz.premier.Entities;
using biz.premier.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ServiceOrder
{
    public interface IWorkOrderRepository : IGenericRepository<Entities.WorkOrder>
    {
        Entities.WorkOrder UpdateCustom(Entities.WorkOrder order, int key);
        Entities.WorkOrder GetWorkOrder(int wo);
        ActionResult GetOrderByWo(int so);
        ActionResult GetOrders(int sr, int? supplierPartner);
        ActionResult GetOrder(int so);
        ActionResult GetrequestInvoice(int[] so, int? invoice, int? supplierPartner);
        Invoice AddInvoice(Invoice Dto);
        Invoice UpdateInvoice(Invoice Dto);
        ActionResult GetInvoiceList(int user, DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country);
        ActionResult GetRequestCenter(DateTime? range1, DateTime? range2, int? status, int? serviceLine, int? requestType, int? coordinator, int? partner, int? client, int? country);
        ActionResult GetSupplierPartnerInvoices(int sr);
        int last_id();
        int lastIdServiceOrderServices();
        int lastIdBundleService();
        ActionResult GetServiceStandalone(int service_order_id);
        ActionResult GetServicePackage(int service_order_id);
        ActionResult GetServiceAllService(int serviceLineId, int serviceRecordId, int? service_type_id, int? status_id, DateTime? date_in, DateTime? date_last);
        ActionResult GetDocumentation(int category, int applicatId, int service_order_id, int type_service);
        Boolean DeleteStandStandaloneServiceWorkOrder(int key);
        Boolean DeleteBundledService(int key);
        Tuple<string, string> GetName(int key);
        ActionResult GetPackage(int wo);
        Tuple<bool, string> AddAdditionalTime(int wo, int service, int time, int user, int userRequest);
        ActionResult GetCommentsHistory(int sr);
        ActionResult GetDeliverTo(int key);
        bool Delete(Entities.WorkOrder order);
        int GetCountryToCountry(int id);
        ActionResult GetBundledServiceByBundleId(int bundle_swo_id);
    }
}
