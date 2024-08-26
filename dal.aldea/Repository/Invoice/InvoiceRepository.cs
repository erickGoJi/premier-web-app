using biz.premier.Repository.Ínvoice;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using biz.premier.Entities;

namespace dal.premier.Repository.Invoice
{
    public class InvoiceRepository : GenericRepository<biz.premier.Entities.Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetRequestInvoice(int key)
        {
            var requestInvoice = _context.Invoices
                .Include(x => x.ServiceInvoices)
                .Where(x => x.Id == key).Select(s => new
                {
                    s.Id,
                    s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                    supplierPartner = s.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                    ? $"{s.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name} " +
                    $"{s.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.LastName} " +
                    $"{s.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.MotherLastName}"
                    : $"{s.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.Name} " +
                    $"{s.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.LastName} " +
                    $"{s.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.MotherLastName}",
                    consultantId = s.Consultant,
                    consultant = s.Consultant.HasValue ? s.ServiceRecordNavigation.ImmigrationSupplierPartners.Any()
                        ? $"{s.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name} " +
                        $"{s.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName} " +
                        $"{s.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}"
                        : $"{s.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().Supplier.Name} " +
                        $"{s.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().Supplier.LastName} " +
                        $"{s.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}"
                    : "",
                    partner = s.ServiceRecordNavigation.Partner.Name,
                    partnerId = s.ServiceRecordNavigation.PartnerId,
                    client = s.ServiceRecordNavigation.Client.Name,
                    clientId = s.ServiceRecordNavigation.ClientId,
                    office = s.Office,
                    contact = s.Contact,
                    email = s.ContactNavigation.Email,
                    phoneNumber = s.ContactNavigation.PhoneNumber,
                    city = s.ServiceRecordNavigation.ImmigrationSupplierPartners.Any()
                    ? s.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().SupplierTypeId == 1
                        ? s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCity.City
                    : s.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().SupplierTypeId == 2
                        ? s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCity.City
                        : s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                    country = s.ServiceRecordNavigation.ImmigrationSupplierPartners.Any()
                    ? s.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().SupplierTypeId == 1
                        ? s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                        : s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                    : s.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().SupplierTypeId == 2
                        ? s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                        : s.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                    total = s.AdditionalExpenses.Any()
                    ? (s.AdditionalExpenses.Select(a => a.Amount).Sum() + s.ServiceInvoices.Select(a => a.AmountToInvoice).Sum())
                    : s.ServiceInvoices.Select(a => a.AmountToInvoice).Sum(),
                    currency = _context.ProfileUsers
                    .Where(x => x.Id == s.Consultant)
                    .Select(a => a.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency).FirstOrDefault(),
                    services = s.ServiceInvoices.Select(a => new
                    {
                        serviceLine = a.WorkOrderNavigation.ServiceLine.ServiceLine,
                        workOrder = a.WorkOrderNavigation.NumberWorkOrder,
                        serviceID = a.TypeService == 1
                            ? a.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(q => q.Id == a.Service).Select(e => $"{e.Service.Service}/{e.ServiceNumber}").ToList()
                            : a.WorkOrderNavigation.BundledServicesWorkOrders.Where(q => q.Id == a.Service).FirstOrDefault().BundledServices.Select(e => $"{e.Service.Service}/{e.ServiceNumber}").ToList(),
                        amountToInvoice = a.AmountToInvoice,
                        hoursToInvocie = a.HourInvoice
                    }).ToList(),
                    additionalExpense = s.AdditionalExpenses.Select(q => new
                    {
                        requested = q.Requested,
                        concept = q.Concept,
                        amount = q.Amount,
                        currency = q.CurrencyNavigation.Currency
                    }).ToList(),

                    comments = s.Comments,
                    commentInvoices = s.CommentInvoices.Select(q => new
                    {
                        q.Id,
                        q.Invoice,
                        q.Comment,
                        q.CreatedBy,
                        title = q.CreatedByNavigation.UserType.Type,
                        photo = q.CreatedByNavigation.Avatar,
                        q.CreatedDate
                    }).ToList(),
                    paymentNumber = s.PaymentNumber,
                    document = s.DocumentInvoices.Select(q => new
                    {
                        id = q.Id,
                        q.Type,
                        uploadedDate = q.CreatedDate,
                        documet = q.FilePath,
                    }).ToList(),
                    invoiceObjectGeneral = s,
                }).FirstOrDefault();
            var paymentMethods = requestInvoice.consultantId.HasValue ? _context.AreasCoverageConsultants.Where(x => x.ProfileUsers.Select(a => a.Id).Contains(requestInvoice.consultantId.Value)).Select(q => new
            {
                wireTransfer = q.PaymentInformationConsultants.FirstOrDefault().WireTransferConsultants.Select(x => new
                {
                    x.Id,
                    x.AccountTypeNavigation.AccountType,
                    x.AccountHoldersName,
                    x.BankName,
                    x.AccountNumber,
                    x.InternationalPaymentAcceptance
                }).ToList(),
                creditCards = q.PaymentInformationConsultants.FirstOrDefault().CreditCardPaymentInformationConsultants.Select(x => new
                {
                    x.CreditCardNavigation.CreditCard,
                    id = x.CreditCard,
                }).ToList(),
                ifCheck = q.PaymentInformationConsultants.FirstOrDefault().Checks,
                PayToOrderOf = q.PaymentInformationConsultants.FirstOrDefault().PayToOrderOf,
                ifCash = q.PaymentInformationConsultants.FirstOrDefault().Cash,
                IfCashComment = q.PaymentInformationConsultants.FirstOrDefault().Comment,
            }).FirstOrDefault() : null;
            var data = new
            {
                paymentMethods = paymentMethods,
                requestInvoice
            };
            return new ObjectResult(data);
        }

        public ActionResult GetSupplierInvoices(DateTime? range1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            var invoices = _context.ServiceInvoices.Where(x => x.InvoiceNavigation.InvoiceType == 2).Select(s => new
            {
                id = s.Invoice.Value,
                status = s.StatusNavigation.Status,
                statusId = s.Status,
                serviceLineId = s.WorkOrderNavigation.ServiceLineId.Value,
                serviceLine = s.WorkOrderNavigation.ServiceLine.ServiceLine,
                serviceRecord = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                serviceRecordId = s.InvoiceNavigation.ServiceRecord,
                asignee = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                partnerId = s.InvoiceNavigation.ServiceRecordNavigation.PartnerId,
                client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                clientId = s.InvoiceNavigation.ServiceRecordNavigation.ClientId,
                coordinator = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                    ? $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name} " +
                    $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.LastName} " +
                    $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.MotherLastName}"
                    : $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.Name} " +
                    $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.LastName} " +
                    $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.MotherLastName}",
                coordinatorId = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                    ? s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().CoordinatorId.Value
                    : s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().CoordinatorId.Value,
                requestedDate = s.InvoiceNavigation.CreatedDate.Value,
                request_type = s.InvoiceNavigation.InvoiceTypeNavigation.Type,
                invoiceType = s.InvoiceNavigation.InvoiceType,
                country = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).HomeCountry.Name,
                countryId = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).HomeCountry.Id,
                dueDate = DateTime.Today,
                amount = s.AmountToInvoice.Value,
                currency = _context.ProfileUsers
                    .Where(x => x.Id == s.InvoiceNavigation.Consultant)
                    .Select(a => a.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency).FirstOrDefault(),
                description = s.InvoiceNavigation.Comments,
                s.Invoice,
            }).ToList();

            if (range1.HasValue && range2.HasValue)
                invoices = invoices.Where(x => x.requestedDate > range1.Value && x.requestedDate < range2.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (serviceLine.HasValue)
                invoices = invoices.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (invoiceType.HasValue)
                invoices = invoices.Where(x => x.invoiceType == invoiceType.Value).ToList();
            if (coordinator.HasValue)
                invoices = invoices.Where(x => x.coordinatorId == coordinator.Value).ToList();
            if (partner.HasValue)
                invoices = invoices.Where(x => x.partnerId == partner.Value).ToList();
            if (client.HasValue)
                invoices = invoices.Where(x => x.clientId == client.Value).ToList();
            if (country.HasValue)
                invoices = invoices.Where(x => x.countryId == country.Value).ToList();
            return new ObjectResult(invoices);
        }

        public biz.premier.Entities.Invoice UpdateCustom(biz.premier.Entities.Invoice invoice)
        {
            if (invoice == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Invoice>()
                .Include(i => i.CommentInvoices)
                .Include(i => i.DocumentInvoices)
                .Include(i => i.ServiceInvoices)
                    .ThenInclude(x => x.StatusNavigation)
                .SingleOrDefault(s => s.Id == invoice.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(invoice);
                // Comments
                foreach (var comment in invoice.CommentInvoices)
                {
                    var existingComments = exist.CommentInvoices.Where(p => p.Id == comment.Id).FirstOrDefault();
                    if (existingComments == null)
                    {
                        exist.CommentInvoices.Add(comment);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingComments).CurrentValues.SetValues(comment);
                    }
                }
                // Documents
                foreach (var document in invoice.DocumentInvoices)
                {
                    var existingDocuments = exist.DocumentInvoices.Where(p => p.Id == document.Id).FirstOrDefault();
                    if (existingDocuments == null)
                    {
                        exist.DocumentInvoices.Add(document);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingDocuments).CurrentValues.SetValues(document);
                    }
                }
                // Work Order Invoices
                foreach (var workOrder in exist.ServiceInvoices)
                {
                    foreach (var status in invoice.ServiceInvoices)
                    {
                        switch (status.Status)
                        {
                            case 3:
                                var credit = _context.ImmigrationSupplierPartners.Any()
                                ? _context.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == exist.ServiceRecord)
                                .Join(_context.ProfileUsers, a => a.SupplierId, e => e.Id
                                , (a, e) => new
                                {
                                    e.AreasCoverage
                                })
                                .Join(_context.AreasCoverageConsultants, a => a.AreasCoverage, e => e.Id
                                , (a, e) => new
                                {
                                    e.SupplierPartnerProfileConsultantNavigation.CreditTermsNavigation.CreditTerm
                                }).FirstOrDefault()
                                : _context.RelocationSupplierPartners.Where(x => x.ServiceRecordId == exist.ServiceRecord)
                                .Join(_context.ProfileUsers, a => a.SupplierId, e => e.Id
                                , (a, e) => new
                                {
                                    e.AreasCoverage
                                })
                                .Join(_context.AreasCoverageConsultants, a => a.AreasCoverage, e => e.Id
                                , (a, e) => new
                                {
                                    e.SupplierPartnerProfileConsultantNavigation.CreditTermsNavigation.CreditTerm
                                }).FirstOrDefault();

                                workOrder.DueDate = DateTime.Now.AddDays(Convert.ToInt32(credit.CreditTerm.Split(' ')[0]));
                                workOrder.Status = status.Status;
                                break;
                            case 4:
                                workOrder.Status = status.Status;
                                workOrder.DueDate = null;
                                break;
                            case 6:
                                workOrder.Status = status.Status;
                                workOrder.DueDate = null;
                                break;
                            default:
                                break;
                        }
                    }

                    workOrder.Status = workOrder.Status == 1 ? 2 : workOrder.Status;
                    _context.ServiceInvoices.Update(workOrder);
                }
                //foreach (var status in invoice.ServiceInvoices)
                //{
                //    foreach (var workOrder in exist.ServiceInvoices)
                //    {
                //        if (status.Status == 3)
                //        {

                //            workOrder.DueDate = DateTime.Now.AddDays(Convert.ToInt32(workOrder.StatusNavigation.Status.Split(' ')[0]));
                //        }

                //        workOrder.Status = workOrder.Status == 1 ? 2 : workOrder.Status;
                //        _context.ServiceInvoices.Update(workOrder);
                //    }
                //}
                _context.SaveChanges();
            }
            return exist;
        }

        public ActionResult GetThirdPartyInvoice(int sr)
        {
            var workOrderStandaloneServiceWorkOrders = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == sr)
                .Select(s=> new { s.WorkOrderServiceId.Value}).ToList();
            var workOrdersBundledServicesWorkOrders = _context.BundledServicesWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == sr)
                .Select(s=> new { s.WorkOrderId.Value}).ToList();
            var workOrders = workOrdersBundledServicesWorkOrders.Union(workOrderStandaloneServiceWorkOrders).ToList();
            var thirdPartyInvoices = _context.RequestPayments
                .Where(x => workOrders.Select(s => s.Value).Contains(x.WorkOrderServicesId.Value)).Select(s => new
                {
                    s.Id,
                    invoiceNo = s.InvoiceNo.Length > 0 ? s.InvoiceNo : "-",
                    invoiceDate = s.InvoiceDate.HasValue ? s.InvoiceDate.ToString() : "-",
                    s.Description,
                    amount = s.Payments.Sum(_s => _s.PaymentAmount),
                    s.Payments.FirstOrDefault().CurrencyPaymentAmount,
                    nextReminderDate = s.Payments.Select(q=>q.DueDate).FirstOrDefault(x => x >= DateTime.Now),
                    s.CountryNavigation.Name,
                    country = s.WorkOrderServices.BundledServices.Any() 
                        ? s.WorkOrderServices.BundledServices.FirstOrDefault().DeliveringInNavigation.Name
                        : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringInNavigation.Name,
                    s.StatusNavigation.Status
                }).ToList();
            return new ObjectResult(thirdPartyInvoices);
        }

        public ActionResult ServiceInvoices(int sr, int? serviceLine, int? status)
        {
            var invoices = _context.ServiceInvoices.Where(x => x.InvoiceNavigation.ServiceRecord == sr).Select(s => new
            {
                s.InvoiceNavigation.Id,
                invoiceNo = s.InvoiceNavigation.InoviceNo.Length > 0 ? s.InvoiceNavigation.InoviceNo : "-",
                premierInvoiceDate = s.CreatedDate,
                serviceLince = s.WorkOrderNavigation.ServiceLine.ServiceLine,
                serviceLineId = s.ServiceLine,
                workOder = s.WorkOrderNavigation.NumberWorkOrder,
                dueDate = s.DueDate,
                invoicedFee = s.AmountToInvoice,
                status = s.StatusNavigation.Status,
                statusId = s.Status
            }).ToList();
            if (serviceLine.HasValue)
                invoices = invoices.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            return new ObjectResult(invoices);
        }

        private DateTime? GetNextReminderDate(int requestPayment)
        {
            DateTime dateNow = DateTime.Now;
            var paymentConcepts = _context.Payments
                .Where(x => x.RequestPayment == requestPayment)
                .Select(s => s.DueDate)
                .ToList()
                .OrderByDescending(o => o);
            return paymentConcepts.FirstOrDefault(x => x >= dateNow);
        }

        private string GetCountryName(int workOrderServices)
        {
            var stand = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrderServiceId == workOrderServices).Select(s =>s.DeliveringInNavigation.Name);
            var bundled = _context.BundledServices.Where(x => x.WorkServicesId == workOrderServices).Select(s =>s.DeliveringInNavigation.Name);
            return stand.Any() ? stand.Select(s => s).FirstOrDefault() : bundled.Select(s => s).FirstOrDefault();
        }

        private decimal GetAmount(int rp)
        {
            var payment = _context.Payments
                .Where(x => x.RequestPayment == rp)
                .Sum(s => s.PaymentAmount);
            var concept = _context.Concepts
                .Where(x => x.PaymentNavigation.RequestPayment == rp)
                .Sum(s=>s.Amount);
            return new decimal((double) (payment.Value + concept.Value));
        }
    }
}
