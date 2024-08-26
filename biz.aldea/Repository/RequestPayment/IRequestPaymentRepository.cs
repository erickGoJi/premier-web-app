using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.RequestPayment
{
    public interface IRequestPaymentRepository : IGenericRepository<Entities.RequestPayment>
    {
        Entities.RequestPayment UpdateCustom(Entities.RequestPayment requestPayment, int key);
        List<Entities.Payment> AddPaymentConcept(List<Entities.Payment> paymentConcept);
        ActionResult GetRequestedPayment(int workOrderServices);
        Entities.Payment UpdatePaymentConceptCustom(Entities.Payment paymentConceptDto);
        Entities.Payment GetPaymentConceptById(int paymentConcept);
        ActionResult GetPaymentsRelated(int requestPaymentId);
        ActionResult GetRequestPaymentById(int requestPaymentId);
        ActionResult GetFinance(int sr);
        bool DeletePaymentConcept(int pc, bool allPaymentConcept);
        ActionResult GetSupplierInvoices(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country);
        ActionResult GetThirdPartyExpenses(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country);
        ActionResult GetInvoicesService(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country);
        ActionResult GetFee(int key);
    }
}
