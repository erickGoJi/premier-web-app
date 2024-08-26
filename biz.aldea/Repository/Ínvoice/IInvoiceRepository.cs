using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Ínvoice
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        ActionResult GetSupplierInvoices(DateTime? range1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country);
        ActionResult GetRequestInvoice(int key);
        Invoice UpdateCustom(Invoice invoice);
        ActionResult GetThirdPartyInvoice(int sr);
        ActionResult ServiceInvoices(int sr, int? serviceLine, int? status);
    }
}
