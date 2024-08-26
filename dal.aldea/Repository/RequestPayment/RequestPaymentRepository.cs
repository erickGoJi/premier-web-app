using biz.premier.Repository.RequestPayment;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.RequestPayment
{
    public class RequestPaymentRepository : GenericRepository<biz.premier.Entities.RequestPayment>, IRequestPaymentRepository
    {
        public RequestPaymentRepository(Db_PremierContext context) : base(context) { }
        public List<biz.premier.Entities.Payment> AddPaymentConcept(List<biz.premier.Entities.Payment> paymentConcept)
        {
            biz.premier.Entities.RequestPayment payment = new biz.premier.Entities.RequestPayment();
            payment.WorkOrderServicesId = paymentConcept.FirstOrDefault().WorkOrderServices;
            payment.Urgent = paymentConcept.FirstOrDefault().Urgent;
            payment.Recurrence = paymentConcept.FirstOrDefault().Recurrence;
            payment.Status = 1;
            payment.RecurrenceRequestPayments = new List<biz.premier.Entities.RecurrenceRequestPayment>();
            if (paymentConcept.FirstOrDefault().RecurrencePaymentConcepts.Any())
            {
                foreach (var i in paymentConcept.FirstOrDefault().RecurrencePaymentConcepts)
                {
                    biz.premier.Entities.RecurrenceRequestPayment recurrence = new biz.premier.Entities.RecurrenceRequestPayment();
                    recurrence.Never = i.Never;
                    recurrence.Period = i.Period;
                    recurrence.RepeatEvery = i.RepeatEvery;
                    recurrence.RequestPayment = 0;
                    recurrence.CreatedBy = i.CreatedBy;
                    recurrence.CreatedDate = DateTime.Now;
                    recurrence.Date = i.Date;
                    recurrence.StartDate = i.StartDate;
                    recurrence.EndDate = i.EndDate;
                    recurrence.RepeatTheRecurrenceRequestPayments = new List<biz.premier.Entities.RepeatTheRecurrenceRequestPayment>();
                    foreach (var _i in i.RepeatThePaymentConcepts)
                    {
                        biz.premier.Entities.RepeatTheRecurrenceRequestPayment repeat = new biz.premier.Entities.RepeatTheRecurrenceRequestPayment();
                        repeat.Day = _i.Day;
                        repeat.Recurrence = _i.Recurrence;
                        recurrence.RepeatTheRecurrenceRequestPayments.Add(repeat);
                    }
                    payment.RecurrenceRequestPayments.Add(recurrence);
                }
            }
            payment.Payments = new List<biz.premier.Entities.Payment>();
            foreach(var i in paymentConcept)
            {
                payment.Payments.Add(i);
            }
            payment.CreatedBy = paymentConcept.FirstOrDefault().CreatedBy;
            payment.CreatedDate = DateTime.Now;
            _context.RequestPayments.Add(payment);
            _context.SaveChanges();
            return paymentConcept;
        }
        public biz.premier.Entities.Payment GetPaymentConceptById(int paymentConcept)
        {
            var service = _context.Set<biz.premier.Entities.Payment>()
                .Include(i => i.Concepts)
                .Include(i => i.CommentPaymentConcepts)
                    .ThenInclude(i => i.User)
                        .ThenInclude(i => i.UserType)
                .Include(i => i.CommentPaymentConcepts)
                    .ThenInclude(i => i.User)
                        .ThenInclude(i => i.Role)
                .Include(i => i.DocumentPaymentConcepts)
                .Include(i => i.RecurrencePaymentConcepts)
                    .ThenInclude(i => i.RepeatThePaymentConcepts)
                .Include(i => i.RecurrencePaymentConcepts)
                    .ThenInclude(i => i.PeriodNavigation)
                .Include(i => i.WireTransferServicePaymentConcepts)
                .Include(i => i.WireTransferPaymentConcepts)
                    .ThenInclude(i => i.AccountTypeNavigation)
                .Include(i => i.WireTransferPaymentConcepts)
                    .ThenInclude(i => i.CurrencyNavigation)
                .Include(i => i.CurrencyAdvanceFeeNavigation)
                .Include(i => i.CurrencyManagementFeeNavigation)
                .Include(i => i.CurrencyPaymentAmountNavigation)
                .Include(i => i.CurrencyWireFeeNavigation)
                .SingleOrDefault(s => s.Id == paymentConcept);
            return service;
        }
        public ActionResult GetRequestPaymentById(int requestPaymentId)
        {
            var requestPayment = _context.RequestPayments
                .Include(i => i.DocumentRequestPayments)
                    .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.DocumentRequestPayments)
                    .ThenInclude(i => i.CountryOriginNavigation)
                .Include(i => i.CommentRequestPayments)
                    .ThenInclude(i => i.User)
                        .ThenInclude(i => i.UserType)
                .Include(i => i.CommentRequestPayments)
                    .ThenInclude(i => i.User)
                        .ThenInclude(i => i.Role)
                .Include(i => i.RecurrenceRequestPayments)
                    .ThenInclude(i => i.RepeatTheRecurrenceRequestPayments)
                .Include(i => i.Payments)
                .SingleOrDefault(s => s.Id == requestPaymentId);
            var payments = _context.Payments.Where(x => x.RequestPayment == requestPaymentId).Select(s => new
            {
                s.Id,
                s.Desciption,
                ammount = s.PaymentAmount.ToString() + " " + s.CurrencyPaymentAmountNavigation.Currency,
                ManagementFee = s.IfSupplierPartner.Value == true 
                    ? s.ManagementFee.ToString() + " " + s.CurrencyManagementFeeNavigation.Currency
                    : s.PaymentAmount.ToString() + " " + s.CurrencyPaymentAmountNavigation.Currency,
                WireFee = s.IfSupplierPartner.Value == true ? 
                    s.WireFee.ToString() + " " + s.CurrencyWireFeeNavigation.Currency
                    : s.PaymentAmount.ToString() + " " + s.CurrencyPaymentAmountNavigation.Currency,
                AdvenceFee = s.IfSupplierPartner.Value == true 
                    ? s.AdvenceFee.ToString() + " " + s.CurrencyAdvanceFeeNavigation.Currency
                    : s.PaymentAmount.ToString() + " " + s.CurrencyPaymentAmountNavigation.Currency,
                service = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().ServiceNumber
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
                s.RecurrencePaymentConcepts.FirstOrDefault().PeriodNavigation.Recurrence,
                s.Urgent
            }).ToList();
            var requestedPayments = new
            {
                ammountSubTotal = _context.Payments.Where(x => x.RequestPayment == requestPaymentId).Select(s => s.PaymentAmount).Sum(),
                ManagementFeeSubTotal = _context.Payments.Where(x => x.RequestPayment == requestPaymentId).Select(s => Convert.ToDecimal(s.ManagementFee)).Sum(),
                WireFeeSubTotal = _context.Payments.Where(x => x.RequestPayment == requestPaymentId).Select(s => Convert.ToDecimal(s.WireFee)).Sum(),
                AdvanceFeeSubTotal = _context.Payments.Where(x => x.RequestPayment == requestPaymentId).Select(s => Convert.ToDecimal(s.AdvenceFee)).Sum(),
                payments = payments
            };
            var documentsPaymentConcept = _context.DocumentPaymentConcepts
                    .Include(i => i.DocumentTypeNavigation)
                    .Include(i => i.CountryOriginNavigation)
                    .Where(x => x.PaymentConceptNavigation.RequestPayment == requestPaymentId)
                    .ToList();
            var RequestPayment = new
            {
                requestPayment,
                requestedPayments,
                documentsPaymentConcept,
                nextReminderDate = GetNextReminderDate(requestPaymentId)
            };
            return new ObjectResult(RequestPayment);
        }
        public ActionResult GetPaymentsRelated(int requestPaymentId)
        {
            var paymentsRelated = _context.Payments.Where(x => x.RequestPayment == requestPaymentId).Select(s => new
            {
                s.Id,
                s.Desciption,
                ammount = s.PaymentAmount.ToString() + " " + s.CurrencyPaymentAmountNavigation.Currency,
                service = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().ServiceNumber
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
            }).ToList();
            return new ObjectResult(paymentsRelated);
        }
        public ActionResult GetRequestedPayment(int workOrderServices)
        {
            var payments = _context.Payments.Where(x => x.WorkOrderServices == workOrderServices).Select(s => new
            {
                s.Id,
                requestPaymentId = s.RequestPaymentNavigation.Id,
                s.Desciption,
                ammount = s.PaymentAmount.ToString() + " " + s.CurrencyPaymentAmountNavigation.Currency,
                ManagementFee = s.ManagementFee.Length > 0 && s.CurrencyManagementFee.HasValue 
                    ? s.ManagementFee.ToString() + " " + s.CurrencyManagementFeeNavigation.Currency
                    : "N/A",
                WireFee = s.WireFee.Length > 0 && s.CurrencyWireFee.HasValue 
                    ? s.WireFee.ToString() + " " + s.CurrencyWireFeeNavigation.Currency
                    : "N/A",
                AdvenceFee = s.AdvenceFee.Length > 0 && s.CurrencyAdvanceFee.HasValue 
                    ? s.AdvenceFee.ToString() + " " + s.CurrencyAdvanceFeeNavigation.Currency
                    : "N/A",
                service = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().ServiceNumber
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
                s.RecurrencePaymentConcepts.FirstOrDefault().PeriodNavigation.Recurrence,
                s.Urgent
            }).ToList();
            var requestedPayments = new
            {
                ammountSubTotal = _context.Payments.Where(x => x.WorkOrderServices == workOrderServices).Select(s => s.PaymentAmount).Sum(),
                ManagementFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == workOrderServices).Select(s => Convert.ToDecimal(s.ManagementFee)).Sum(),
                WireFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == workOrderServices).Select(s => Convert.ToDecimal(s.WireFee)).Sum(),
                AdvanceFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == workOrderServices).Select(s => Convert.ToDecimal(s.AdvenceFee)).Sum(),
                payments = payments
            };
            return new ObjectResult(requestedPayments);
        }
        public biz.premier.Entities.RequestPayment UpdateCustom(biz.premier.Entities.RequestPayment requestPayment, int key)
        {
            if (requestPayment == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.RequestPayment>()
                .Include(i => i.CommentRequestPayments)
                .Include(i => i.DocumentRequestPayments)
                .Include(i => i.Payments)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(requestPayment);
                foreach (var i in requestPayment.CommentRequestPayments)
                {
                    var comments = exist.CommentRequestPayments.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        exist.CommentRequestPayments.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(i);
                    }
                }
                foreach (var i in requestPayment.DocumentRequestPayments)
                {
                    var document = exist.DocumentRequestPayments.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (document == null)
                    {
                        exist.DocumentRequestPayments.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(i);
                    }
                }
                foreach (var i in requestPayment.Payments)
                {
                    var payment = exist.Payments.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (payment == null)
                    {
                        exist.Payments.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(payment).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            return exist;
        }
        public biz.premier.Entities.Payment UpdatePaymentConceptCustom(biz.premier.Entities.Payment paymentConceptDto)
        {
            if (paymentConceptDto == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Payment>()
                .Include(i => i.CommentPaymentConcepts)
                .Include(i => i.DocumentPaymentConcepts)
                .Include(i => i.RecurrencePaymentConcepts)
                    .ThenInclude(i => i.RepeatThePaymentConcepts)
                .Include(i => i.WireTransferServicePaymentConcepts)
                .Include(i => i.WireTransferPaymentConcepts)
                .Single(s => s.Id == paymentConceptDto.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(paymentConceptDto);
                foreach (var i in paymentConceptDto.CommentPaymentConcepts)
                {
                    var comments = exist.CommentPaymentConcepts.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        exist.CommentPaymentConcepts.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(i);
                    }
                }
                foreach (var i in paymentConceptDto.DocumentPaymentConcepts)
                {
                    var document = exist.DocumentPaymentConcepts.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (document == null)
                    {
                        exist.DocumentPaymentConcepts.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(i);
                    }
                }
                foreach (var i in paymentConceptDto.RecurrencePaymentConcepts)
                {
                    var recurrence = exist.RecurrencePaymentConcepts.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (recurrence == null)
                    {
                        exist.RecurrencePaymentConcepts.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(recurrence).CurrentValues.SetValues(i);
                        recurrence.RepeatThePaymentConcepts.Clear();
                        _context.SaveChanges();
                        foreach (var o in i.RepeatThePaymentConcepts)
                        {
                            recurrence.RepeatThePaymentConcepts.Add(o);
                        }
                        _context.SaveChanges();
                    }
                }

                exist.WireTransferServicePaymentConcepts.Clear();
                _context.SaveChanges();
                foreach (var o in paymentConceptDto.WireTransferServicePaymentConcepts)
                {
                    exist.WireTransferServicePaymentConcepts.Add(o);
                }
                _context.SaveChanges();

                foreach (var i in paymentConceptDto.WireTransferPaymentConcepts)
                {
                    var wireTransfer = exist.WireTransferPaymentConcepts.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (wireTransfer == null)
                    {
                        exist.WireTransferPaymentConcepts.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(wireTransfer).CurrentValues.SetValues(i);
                    }
                }


                _context.SaveChanges();
            }
            return exist;
        }
        public ActionResult GetFinance(int sr)
        {
            //Random rnd = new Random();
            //var serviceInvoices = _context.PaymentConcepts.Where(x => x.ServiceRecord == sr).Select(s => new
            //{
            //    serviceLine = s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.Any()
            //        ? s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLine.ServiceLine
            //        : s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
            //    workOrder = s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.Any()
            //        ? s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.NumberWorkOrder
            //        : s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.NumberWorkOrder,
            //    serviceId = s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.Any()
            //        ? s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber
            //        : s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().ServiceNumber,
            //    supplierPartner = s.SupplierPartnerNavigation.ComercialName,
            //    s.DueDate,
            //    s.InvoiceDate,
            //    s.RequestPaymentNavigation.StatusNavigation.Status,
            //    closedSale = rnd.Next(1, 100).ToString() + "%"
            //}).ToList();
            var financeBundled = _context.RequestPayments
                .Where(x => x.WorkOrderServices.BundledServices.Select(s => s.BundledServiceOrder).Where(y => y.WorkOrder.ServiceRecordId == sr).Any())
                .Select(s => new
                {
                    s.Id,
                    ammountSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => _s.PaymentAmount).Sum(),
                    ManagementFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => Convert.ToDecimal(_s.ManagementFee)).Sum(),
                    WireFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => Convert.ToDecimal(_s.WireFee)).Sum(),
                    AdvanceFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => Convert.ToDecimal(_s.AdvenceFee)).Sum(),
                    fundingRequestedDate = s.FundingRequestDate,
                    recurrent = s.Recurrence,
                    status = s.StatusNavigation.Status
                }).ToList();
            var financeStandalone = _context.RequestPayments
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.Where(y => y.WorkOrder.ServiceRecordId == sr).Any())
                .Select(s => new
                {
                    s.Id,
                    ammountSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => _s.PaymentAmount).Sum(),
                    ManagementFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => Convert.ToDecimal(_s.ManagementFee)).Sum(),
                    WireFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => Convert.ToDecimal(_s.WireFee)).Sum(),
                    AdvanceFeeSubTotal = _context.Payments.Where(x => x.WorkOrderServices == s.WorkOrderServicesId).Select(_s => Convert.ToDecimal(_s.AdvenceFee)).Sum(),
                    fundingRequestedDate = s.FundingRequestDate,
                    recurrent = s.Recurrence,
                    status = s.StatusNavigation.Status
                }).ToList();
            return new ObjectResult(financeBundled.Union(financeStandalone));
        }
        public bool DeletePaymentConcept(int pc, bool allPaymentConcept)
        {
            bool isSuccess = false;
            var find = _context.Payments.SingleOrDefault(x => x.Id == pc);
            if (allPaymentConcept && find != null)
            {
                var allPaymentConcepts = _context.Payments.Where(x => x.RequestPayment == find.RequestPayment && x.DueDate >= find.DueDate).ToList();
                foreach(var i in allPaymentConcepts)
                {
                    _context.Payments.Remove(i);
                }
                _context.SaveChanges();
                isSuccess = true;
            }else if(find != null)
            {
                _context.Payments.Remove(find);
                _context.SaveChanges();
                isSuccess = true;
            }
            return isSuccess;
        }
        public DateTime? GetNextReminderDate(int requestPayment)
        {
            DateTime dateNow = DateTime.Now;
            var paymentConcepts = _context.Payments
                .Where(x => x.RequestPayment == requestPayment)
                .Select(s => s.DueDate)
                .ToList()
                .OrderByDescending(o => o);
            var nextDate = paymentConcepts.Where(x => x >= dateNow ).FirstOrDefault();
            return nextDate;
        }

        public ActionResult GetSupplierInvoices(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            var invoicesRequestPayment = _context.Invoices.Select(s => new
            {
                id = s.Id,
                serviceRecord = s.ServiceRecordNavigation.NumberServiceRecord,
                serviceRecordId = s.ServiceRecord,
                serviceLineId = _context.CatServiceLines.Where(x => x.Id == s.ServiceInvoices.FirstOrDefault().ServiceLine).FirstOrDefault().Id,
                serviceLine = _context.CatServiceLines.Where(x => x.Id == s.ServiceInvoices.FirstOrDefault().ServiceLine).FirstOrDefault().ServiceLine,
                invoiceNo = s.InoviceNo,
                invoiceDate = s.CreatedDate,
                invoiceType = s.InvoiceTypeNavigation.Type,
                invoiceTypeId = s.InvoiceType,
                amount = s.ServiceInvoices.Sum(_s => _s.AmountToInvoice),
                supplierPartner = _context.ProfileUsers.Where(x => x.Id == s.Consultant).Select(a => $"{a.Name} {a.LastName} {a.MotherLastName}").FirstOrDefault(),
                supplierPartnerId = s.Consultant,
                dueDate = s.ServiceInvoices.FirstOrDefault().DueDate,
                currency = _context.ProfileUsers.Where(x => x.Id == s.Consultant)
                    .Select(a => a.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency).FirstOrDefault(),
                country = s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                countryId = s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCountryId,
                status = s.ServiceInvoices.FirstOrDefault().StatusNavigation.Status,
                statusId = s.ServiceInvoices.FirstOrDefault().Status,
                asignee = s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                client = s.ServiceRecordNavigation.Client.Name,
                clientId = s.ServiceRecordNavigation.ClientId,
                partner = s.ServiceRecordNavigation.Partner.Name,
                partnerId = s.ServiceRecordNavigation.PartnerId,
                data = s.Id,
            }).ToList();


            //if (renge1.HasValue && range2.HasValue)
            //    invoicesRequestPayment = invoicesRequestPayment.Where(x => x.requestedDate > renge1.Value && x.requestedDate < range2.Value).ToList();
            if (status.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.statusId == status.Value).ToList();
            if (status.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.statusId == status.Value).ToList();
            if (serviceLine.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (invoiceType.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.invoiceTypeId == invoiceType.Value).ToList();
            //if (coordinator.HasValue)
            //    invoices = invoices.Where(x => x.coordinatorId == coordinator.Value).ToList();
            if (partner.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.partnerId == partner.Value).ToList();
            if (client.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.clientId == client.Value).ToList();
            if (country.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.countryId == country.Value).ToList();
            return new ObjectResult(invoicesRequestPayment);
        }

        public ActionResult GetThirdPartyExpenses(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            var invoicesRequestPayment = _context.Payments.Select(s => new
            {
                id = s.Id,
                status = s.RequestPaymentNavigation.StatusNavigation.Status,
                statusId = s.RequestPaymentNavigation.Status,
                serviceLineId = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLineId.Value
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLineId.Value,
                serviceLine = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLine.ServiceLine,
                serviceRecord = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.NumberServiceRecord
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.NumberServiceRecord,
                serviceRecordId = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId.Value
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId.Value,
                asignee = _context.AssigneeInformations.Where(x => x.ServiceRecordId == s.ServiceRecord.Value).FirstOrDefault().AssigneeName,
                partner = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.Partner.Name
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.Partner.Name,
                partnerId = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.PartnerId
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.PartnerId,
                client = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.Client.Name
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.Client.Name,
                clientId = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.ClientId,
                requestedDate = s.CreatedDate.Value,
                invoiceType = s.RequestPaymentNavigation.StatusNavigation.Status,
                invoiceTypeId = s.RequestPaymentNavigation.Status,
                country = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().DeliveringInNavigation.Name
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringInNavigation.Name,
                countryId = s.RequestPaymentNavigation.WorkOrderServices.BundledServices.Any()
                    ? s.RequestPaymentNavigation.WorkOrderServices.BundledServices.FirstOrDefault().DeliveringIn
                    : s.RequestPaymentNavigation.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringIn,
                dueDate = s.PaymentDate.Value,
                amount = s.PaymentAmount.Value,
                description = s.Desciption,
                data = s.Id,
                currency = s.CurrencyPaymentAmountNavigation.Currency,
            }).ToList();


            if (renge1.HasValue && range2.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.requestedDate > renge1.Value && x.requestedDate < range2.Value).ToList();
            if (status.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.statusId == status.Value).ToList();
            if (status.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.statusId == status.Value).ToList();
            if (serviceLine.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (invoiceType.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.invoiceTypeId == invoiceType.Value).ToList();
            //if (coordinator.HasValue)
            //    invoices = invoices.Where(x => x.coordinatorId == coordinator.Value).ToList();
            if (partner.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.partnerId == partner.Value).ToList();
            if (client.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.clientId == client.Value).ToList();
            if (country.HasValue)
                invoicesRequestPayment = invoicesRequestPayment.Where(x => x.countryId == country.Value).ToList();
            return new ObjectResult(invoicesRequestPayment);
        }

        public ActionResult GetInvoicesService(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType,
            int? coordinator, int? partner, int? client, int? country)
        {
            var invoices = _context.Invoices.Where(x => x.InvoiceType == 1).Select(s => new
            {
                id = s.Id,
                serviceRecord = s.ServiceRecordNavigation.NumberServiceRecord,
                serviceRecordId = s.ServiceRecord,
                serviceLineId = _context.CatServiceLines.Where(x => x.Id == s.ServiceInvoices.FirstOrDefault().ServiceLine).FirstOrDefault().Id,
                serviceLine = _context.CatServiceLines.Where(x => x.Id == s.ServiceInvoices.FirstOrDefault().ServiceLine).FirstOrDefault().ServiceLine,
                invoiceNo = s.InoviceNo,
                invoiceDate = s.CreatedDate,
                invoiceType = s.InvoiceTypeNavigation.Type,
                invoiceTypeId = s.InvoiceType,
                amount = s.ServiceInvoices.Sum(_s => _s.AmountToInvoice),
                supplierPartner = _context.ProfileUsers.Where(x => x.Id == s.Consultant).Select(a => $"{a.Name} {a.LastName} {a.MotherLastName}").FirstOrDefault(),
                supplierPartnerId = s.Consultant,
                dueDate = s.ServiceInvoices.FirstOrDefault().DueDate,
                currency = _context.ProfileUsers.Where(x => x.Id == s.Consultant)
                    .Select(a => a.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency).FirstOrDefault(),
                country = s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                countryId = s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCountryId,
                status = s.ServiceInvoices.FirstOrDefault().StatusNavigation.Status,
                statusId = s.ServiceInvoices.FirstOrDefault().Status,
                asignee = s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                client = s.ServiceRecordNavigation.Client.Name,
                clientId = s.ServiceRecordNavigation.ClientId,
                partner = s.ServiceRecordNavigation.Partner.Name,
                partnerId = s.ServiceRecordNavigation.PartnerId,
                data = s.Id,
            }).ToList();


            if (renge1.HasValue && range2.HasValue)
                invoices = invoices.Where(x => x.invoiceDate >= renge1.Value && x.invoiceDate <= range2.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (serviceLine.HasValue)
                invoices = invoices.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (invoiceType.HasValue)
                invoices = invoices.Where(x => x.invoiceTypeId == invoiceType.Value).ToList();
            //if (coordinator.HasValue)
            //    invoices = invoices.Where(x => x.coordinatorId == coordinator.Value).ToList();
            if (partner.HasValue)
                invoices = invoices.Where(x => x.partnerId == partner.Value).ToList();
            if (client.HasValue)
                invoices = invoices.Where(x => x.clientId == client.Value).ToList();
            if (country.HasValue)
                invoices = invoices.Where(x => x.countryId == country.Value).ToList();
            return new ObjectResult(invoices);
        }

        public ActionResult GetFee(int key)
        {
            var fees = _context.Payments.Where(x => x.RequestPayment == key).Select(s => new
            {
                s.Id,
                s.Desciption,
                s.ManagementFee,
                s.CurrencyManagementFee,
                s.WireFee,
                s.CurrencyWireFee,
                s.AdvenceFee,
                s.CurrencyAdvanceFee
            }).ToList();
            return new ObjectResult(fees);
        }
    }
}
