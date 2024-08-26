using biz.premier.Entities;
using biz.premier.Repository.Report;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Report
{
    public class ReportRepository : GenericRepository<biz.premier.Entities.Report>, IReportRepository
    {
        public ReportRepository(Db_PremierContext context) : base(context) { }

        public List<Filter> AddOrEditFilters(List<Filter> filters, int report)
        {
            if (filters.Count() < 0)
                return null;
            var exist = _context.Filters.Where(x => x.Report == report).ToList();
            //_context.Entry(exist).re.SetValues(record);

            if (exist.Any())
            {
                foreach(var i in exist)
                {
                    _context.Filters.Remove(i);
                }
                _context.SaveChanges();
            }
            foreach(var f in filters)
            {
                _context.Filters.Add(f);
            }
            _context.SaveChanges();
            return filters;
        }

        public List<Filter> AddFiltersOpertionals(List<Filter> filters, int report, int reportType)
        {
            if (filters.Count() < 0)
                return null;
            var exist = _context.Filters.Where(x => x.Report == report).ToList();
            //_context.Entry(exist).re.SetValues(record);

            if (exist.Any())
            {
                foreach (var i in exist)
                {
                    _context.Filters.Remove(i);
                }
                _context.SaveChanges();
            }
            foreach (var f in filters)
            {
                _context.Filters.Add(f);
            }

            var reportS = _context.Reports.SingleOrDefault(x => x.Id == report);
            reportS.ReportType = reportType;
            _context.Reports.Update(reportS);

            _context.SaveChanges();
            return filters;
        }

        public List<Column> AddOrEditColumns(List<Column> columns, int report)
        {
            if (columns.Count() < 0)
                return null;
            var exist = _context.Columns.Where(x => x.Report == report).ToList();
            if (exist.Any())
            {
                foreach (var i in exist)
                {
                    _context.Columns.Remove(i);
                }
                _context.SaveChanges();
            }
            foreach (var c in columns)
            {
                _context.Columns.Add(c);
            }
            _context.SaveChanges();
            return columns;
        }
        public class FullSystemContacts
        {
            public int? id { get; set; }
            public int? user_id { get; set; }
            public string user { get; set; }
            public string role { get; set; }
            public int? title_id { get; set; }
            public string title { get; set; }
            public string company { get; set; }
            public int? company_id { get; set; }
            public string office { get; set; }
            public int? office_id { get; set; }
            public string country { get; set; }
            public int? country_id { get; set; }
            public string city { get; set; }
            public int? city_id { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public int reportType { get; set; }
        }
        public class OperationalReports
        {
            public int Id { get; set; }
            public string ServiceRecordNo { get; set; }
            public bool? Vip { get; set; }
            public string Status { get; set; }
            public int? StatusId { get; set; }
            public DateTime? AuthoDate { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string Partner { get; set; }
            public string Client { get; set; }
            public string AssigneeName { get; set; }
            public int Services { get; set; }
            public string SupplierPartner { get; set; }
            public string Invoice { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public string InvoiceType { get; set; }
            public string Description { get; set; }
            public Decimal Amount { get; set; }
            public int? StatusInvoiceId { get; set; }
            public string StatusInvoice { get; set; }
            public decimal? AmmountSubTotal { get; set; }
            public decimal? ManagementFeeSubTotal { get; set; }
            public decimal? WireFeeSubTotal { get; set; }
            public decimal? AdvanceFeeSubTotal { get; set; }
            public DateTime? FundingRequestedDate { get; set; }
            public bool? Recurrent { get; set; }
            public int? StatusThirdPartyId { get; set; }
            public string StatusThirdParty { get; set; }
        }
        public class table
        {
            public string name { get; set; }
            public int id { get; set; }
            public ICollection<FullSystemContacts> contacts { get; set; }
            public ICollection<OperationalReports> operationals { get; set; }
            public ICollection<Filter> filters1 { get; set; }
            public ICollection<Column> columns1 { get; set; }
        }
        public ActionResult GetCustom(int user, int report)
        {
            List<table> tablesAll = new List<table>();
            var listAll = new List<dynamic>();
            var tables = _context.Reports
                .Include(i => i.Columns)
                    .ThenInclude(i => i.ColumnsNavigation)
                .Include(i => i.Filters)
                    .ThenInclude(i => i.Filter1Navigation)
                .Where(x => x.CreatedBy == user);
            if (report == 2)
            {
                foreach (var i in tables)
                {
                    if(i.ReportType == null)
                    {
                        table tab = new table();
                        var premierContacts = _context.Offices
                            .Select(c => new FullSystemContacts
                            {
                                id = c.ConsultantNavigation.Id,
                                user_id = c.ConsultantNavigation.User.Id,
                                user = c.ConsultantNavigation.User.Name + " " + c.ConsultantNavigation.User.LastName,
                                role = c.ConsultantNavigation.User.Role.Role,
                                title_id = c.ConsultantNavigation.Title.Value,
                                title = c.ConsultantNavigation.TitleNavigation.Title,
                                company = c.ConsultantNavigation.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.ComercialName,
                                company_id = c.ConsultantNavigation.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.Id,
                                office = c.Office1Navigation.Office,
                                office_id = c.Office1Navigation.Id,
                                country_id = c.ConsultantNavigation.PersonalInformation.CountryNavigation.Id,
                                country = c.ConsultantNavigation.PersonalInformation.CountryNavigation.Name,
                                city_id = c.ConsultantNavigation.PersonalInformation.CityNavigation.Id,
                                city = c.ConsultantNavigation.PersonalInformation.CityNavigation.City,
                                phone = c.ConsultantNavigation.PhoneNumber,
                                email = c.ConsultantNavigation.Email,
                                reportType = 1
                            }).ToList();
                        var premierContactsAdministrative = _context.AdministrativeContactsConsultants
                            .Select(c => new FullSystemContacts
                            {
                                id = c.Id,
                                user_id = 0,
                                user = c.ContactName,
                                role = "",
                                title_id = 0,
                                title = c.Title,
                                company = c.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.ComercialName,
                                company_id = c.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.Id,
                                office = "",
                                office_id = 0,
                                country_id = c.CityNavigation.IdCountry,
                                country = c.CityNavigation.IdCountryNavigation.Name,
                                city_id = c.CityNavigation.Id,
                                city = c.CityNavigation.City,
                                phone = c.PhoneNumber,
                                email = c.Email,
                                reportType = 1
                            }).ToList();
                        var externalPremierContacts = _context.ConsultantContactsServices
                            .Select(c => new FullSystemContacts
                            {
                                id = c.Id,
                                user_id = 0,
                                user = c.ContactName,
                                role = "",
                                title_id = 0,
                                title = c.Title,
                                company = c.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.ComercialName,
                                company_id = c.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.Id,
                                office = "",
                                office_id = 0,
                                country_id = c.CityNavigation.IdCountry,
                                country = c.CityNavigation.IdCountryNavigation.Name,
                                city_id = c.CityNavigation.Id,
                                city = c.CityNavigation.City,
                                phone = c.PhoneNumber,
                                email = c.Email,
                                reportType = 2
                            }).ToList();
                        var externalPremierContactsAdministrative = _context.AdministrativeContactsServices
                            .Select(c => new FullSystemContacts
                            {
                                id = c.Id,
                                user_id = 0,
                                user = c.ContactName,
                                role = "",
                                title_id = 0,
                                title = c.Title,
                                company = c.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.ComercialName,
                                company_id = c.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.Id,
                                office = "",
                                office_id = 0,
                                country_id = c.CityNavigation.IdCountry,
                                country = c.CityNavigation.IdCountryNavigation.Name,
                                city_id = c.CityNavigation.Id,
                                city = c.CityNavigation.City,
                                phone = c.PhoneNumber,
                                email = c.Email,
                                reportType = 2
                            }).ToList();
                        var fullSystemContact = premierContacts.Union(premierContactsAdministrative)
                            .Union(externalPremierContacts)
                            .Union(externalPremierContactsAdministrative)
                            .OrderByDescending(o => o.id)
                            .ToList();
                        if (i.Filters.Where(s => s.Filter1 == 1).SingleOrDefault() != null)
                        {
                            fullSystemContact = fullSystemContact.Where(x => x.title_id == i.Filters.Where(s => s.Filter1 == 1).SingleOrDefault().Value).ToList();
                        }
                        if (i.Filters.Where(s => s.Filter1 == 2).SingleOrDefault() != null)
                        {
                            fullSystemContact = fullSystemContact.Where(x => x.country_id == i.Filters.Where(s => s.Filter1 == 2).SingleOrDefault().Value).ToList();
                        }
                        if (i.Filters.Where(s => s.Filter1 == 3).SingleOrDefault() != null)
                        {
                            fullSystemContact = fullSystemContact.Where(x => x.city_id == i.Filters.Where(s => s.Filter1 == 3).SingleOrDefault().Value).ToList();
                        }
                        if (i.Filters.Where(s => s.Filter1 == 4).SingleOrDefault() != null)
                        {
                            fullSystemContact = fullSystemContact.Where(x => x.company_id == i.Filters.Where(s => s.Filter1 == 4).SingleOrDefault().Value).ToList();
                        }
                        if (i.Filters.Where(s => s.Filter1 == 5).SingleOrDefault() != null)
                        {
                            fullSystemContact = fullSystemContact.Where(x => x.office_id == i.Filters.Where(s => s.Filter1 == 5).SingleOrDefault().Value).ToList();
                        }
                        if (i.Filters.Where(s => s.Filter1 == 6).SingleOrDefault() != null)
                        {
                            fullSystemContact = fullSystemContact.Where(x => x.reportType == i.Filters.Where(s => s.Filter1 == 6).SingleOrDefault().Value).ToList();
                        }
                        tab.id = i.Id;
                        tab.name = i.Name;
                        tab.columns1 = i.Columns;
                        tab.filters1 = i.Filters;
                        tab.contacts = fullSystemContact;
                        tablesAll.Add(tab);
                    }
                }
            }
            else if(report == 1)
            {
                foreach(var t in tables)
                {
                    table tab = new table();
                    // REPORT TYPE: NONE AND ONLY SERVICE RECORDS
                    if (t.ReportType == 1 || t.ReportType == 6)
                    {
                        var onlySR = _context.ServiceRecords.Select(s => new OperationalReports
                        {
                            Id = s.Id,
                            ServiceRecordNo = s.NumberServiceRecord,
                            Vip = !s.Vip.HasValue ? false : s.Vip.Value,
                            Status = s.Status.Status,
                            StatusId = !s.StatusId.HasValue ? 0 : s.StatusId.Value,
                            AuthoDate = s.InitialAutho,
                            Country = s.AssigneeInformations.Count != 0 ?
                            s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name : "",
                            //countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                            //countryHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCountryId : 0,
                            City = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
                            //cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                            //cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                            Partner = s.Partner.Name,
                            //partnerId = s.PartnerId,
                            Client = s.Client.Name,
                            //clientId = s.ClientId,
                            AssigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName,
                            Services = s.WorkOrders.Count != 0 ? s.WorkOrders.Count : 0,
                            SupplierPartner = s.ImmigrationSupplierPartners.Count != 0 ?
                                s.ImmigrationSupplierPartners.FirstOrDefault().SupplierType.Name :
                                (s.RelocationSupplierPartners.Count != 0 ? s.RelocationSupplierPartners.FirstOrDefault().SupplierType.Name : "")
                        }).ToList();
                        tab.id = t.Id;
                        tab.name = t.Name;
                        tab.columns1 = t.Columns;
                        tab.filters1 = t.Filters;
                        tab.operationals = onlySR;
                        tablesAll.Add(tab);
                    }
                    // REPORT TYPE: INVOICE
                    else if(t.ReportType == 2)
                    {
                        var onlySR = _context.ServiceInvoices.Select(s => new OperationalReports
                        {
                            Id = s.InvoiceNavigation.ServiceRecord.Value,
                            ServiceRecordNo = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                            Vip = !s.InvoiceNavigation.ServiceRecordNavigation.Vip.HasValue ? false : s.InvoiceNavigation.ServiceRecordNavigation.Vip.Value,
                            Status = s.InvoiceNavigation.ServiceRecordNavigation.Status.Status,
                            StatusId = !s.InvoiceNavigation.ServiceRecordNavigation.StatusId.HasValue ? 0 : s.InvoiceNavigation.ServiceRecordNavigation.StatusId.Value,
                            AuthoDate = s.InvoiceNavigation.ServiceRecordNavigation.InitialAutho,
                            Country = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0 ?
                            s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name : "",
                            //countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                            //countryHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCountryId : 0,
                            City = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0 
                                ? s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCity.City 
                                : "",
                            //cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                            //cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                            Partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                            //partnerId = s.PartnerId,
                            Client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                            //clientId = s.ClientId,
                            AssigneeName = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                            Services = s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count : 0,
                            SupplierPartner = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.Count != 0 ?
                                s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().SupplierType.Name :
                                (s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().SupplierType.Name : ""),
                            Invoice = s.Id.ToString(),
                            Amount = s.AmountToInvoice.Value,
                            Description = s.InvoiceNavigation.Comments,
                            StatusInvoice = s.StatusNavigation.Status,
                            StatusInvoiceId = s.Status,
                        }).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 7).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusId == t.Filters.Where(q => q.Filter1 == 7).SingleOrDefault().Value).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 9).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.AuthoDate > t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().FirstDate.Value &&
                                x.AuthoDate < t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().SecondDate.Value).ToList();
                        tab.id = t.Id;
                        tab.name = t.Name;
                        tab.columns1 = t.Columns;
                        tab.filters1 = t.Filters;
                        tab.operationals = onlySR;
                        tablesAll.Add(tab);
                    }
                    // REPORT TYPE: Supplier invoice
                    else if (t.ReportType == 3)
                    {
                        var onlySR = _context.ServiceInvoices.Where(x => x.InvoiceNavigation.InvoiceType == 2).Select(s => new OperationalReports
                        {
                            Id = s.InvoiceNavigation.ServiceRecord.Value,
                            ServiceRecordNo = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                            Vip = !s.InvoiceNavigation.ServiceRecordNavigation.Vip.HasValue ? false : s.InvoiceNavigation.ServiceRecordNavigation.Vip.Value,
                            Status = s.InvoiceNavigation.ServiceRecordNavigation.Status.Status,
                            StatusId = !s.InvoiceNavigation.ServiceRecordNavigation.StatusId.HasValue ? 0 : s.InvoiceNavigation.ServiceRecordNavigation.StatusId.Value,
                            AuthoDate = s.InvoiceNavigation.ServiceRecordNavigation.InitialAutho,
                            Country = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0 ?
                            s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name : "",
                            //countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                            //countryHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCountryId : 0,
                            City = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0
                                ? s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCity.City
                                : "",
                            //cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                            //cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                            Partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                            //partnerId = s.PartnerId,
                            Client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                            //clientId = s.ClientId,
                            AssigneeName = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                            Services = s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count : 0,
                            SupplierPartner = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.Count != 0 ?
                                s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().SupplierType.Name :
                                (s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().SupplierType.Name : ""),
                            Invoice = s.Id.ToString(),
                            Amount = s.AmountToInvoice.Value,
                            Description = s.InvoiceNavigation.Comments,
                            StatusInvoice = s.StatusNavigation.Status,
                            StatusInvoiceId = s.Status,
                        }).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 7).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusId == t.Filters.Where(q => q.Filter1 == 7).SingleOrDefault().Value).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 8).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusInvoiceId == 1).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 9).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.AuthoDate > t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().FirstDate.Value &&
                                x.AuthoDate < t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().SecondDate.Value).ToList();
                        tab.id = t.Id;
                        tab.name = t.Name;
                        tab.columns1 = t.Columns;
                        tab.filters1 = t.Filters;
                        tab.operationals = onlySR;
                        tablesAll.Add(tab);
                    }
                    // REPORT TYPE: Third Party Invoice-Supplier Invoice 
                    else if (t.ReportType == 4)
                    {
                        var onlySR = _context.ServiceInvoices.Select(s => new OperationalReports
                        {
                            Id = s.InvoiceNavigation.ServiceRecord.Value,
                            ServiceRecordNo = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                            Vip = !s.InvoiceNavigation.ServiceRecordNavigation.Vip.HasValue ? false : s.InvoiceNavigation.ServiceRecordNavigation.Vip.Value,
                            Status = s.InvoiceNavigation.ServiceRecordNavigation.Status.Status,
                            StatusId = !s.InvoiceNavigation.ServiceRecordNavigation.StatusId.HasValue ? 0 : s.InvoiceNavigation.ServiceRecordNavigation.StatusId.Value,
                            AuthoDate = s.InvoiceNavigation.ServiceRecordNavigation.InitialAutho,
                            Country = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0 ?
                            s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name : "",
                            //countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                            //countryHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCountryId : 0,
                            City = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0
                                ? s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCity.City
                                : "",
                            //cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                            //cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                            Partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                            //partnerId = s.PartnerId,
                            Client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                            //clientId = s.ClientId,
                            AssigneeName = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                            Services = s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count : 0,
                            SupplierPartner = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.Count != 0 ?
                                s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().SupplierType.Name :
                                (s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().SupplierType.Name : ""),
                            Invoice = s.Id.ToString(),
                            Amount = s.AmountToInvoice.Value,
                            Description = s.InvoiceNavigation.Comments,
                            StatusInvoice = s.StatusNavigation.Status,
                            StatusInvoiceId = s.Status,
                            AmmountSubTotal = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => _s.PaymentAmount).Sum(),
                            ManagementFeeSubTotal = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => Convert.ToDecimal(_s.ManagementFee)).Sum(),
                            WireFeeSubTotal = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => Convert.ToDecimal(_s.WireFee)).Sum(),
                            AdvanceFeeSubTotal = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => Convert.ToDecimal(_s.AdvenceFee)).Sum(),
                            FundingRequestedDate = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.FundingRequestDate,
                            Recurrent = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.Recurrence,
                            StatusThirdParty = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.StatusNavigation.Status,
                            StatusThirdPartyId = _context.Payments.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.Status,
                        }).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 7).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusId == t.Filters.Where(q => q.Filter1 == 7).SingleOrDefault().Value).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 8).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusInvoiceId == 1).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 9).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.AuthoDate > t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().FirstDate.Value &&
                                x.AuthoDate < t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().SecondDate.Value).ToList();
                        tab.id = t.Id;
                        tab.name = t.Name;
                        tab.columns1 = t.Columns;
                        tab.filters1 = t.Filters;
                        tab.operationals = onlySR;
                        tablesAll.Add(tab);
                    }
                    // REPORT TYPE: Invoice-Supplier Invoice
                    else if (t.ReportType == 5)
                    {
                        var onlySR = _context.ServiceInvoices.Select(s => new OperationalReports
                        {
                            Id = s.InvoiceNavigation.ServiceRecord.Value,
                            ServiceRecordNo = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                            Vip = !s.InvoiceNavigation.ServiceRecordNavigation.Vip.HasValue ? false : s.InvoiceNavigation.ServiceRecordNavigation.Vip.Value,
                            Status = s.InvoiceNavigation.ServiceRecordNavigation.Status.Status,
                            StatusId = !s.InvoiceNavigation.ServiceRecordNavigation.StatusId.HasValue ? 0 : s.InvoiceNavigation.ServiceRecordNavigation.StatusId.Value,
                            AuthoDate = s.InvoiceNavigation.ServiceRecordNavigation.InitialAutho,
                            Country = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0 ?
                            s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name : "",
                            //countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                            //countryHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCountryId : 0,
                            City = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.Count != 0
                                ? s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().HostCity.City
                                : "",
                            //cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                            //cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                            Partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                            //partnerId = s.PartnerId,
                            Client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                            //clientId = s.ClientId,
                            AssigneeName = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                            Services = s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.WorkOrders.Count : 0,
                            SupplierPartner = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.Count != 0 ?
                                s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationSupplierPartners.FirstOrDefault().SupplierType.Name :
                                (s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.Count != 0 ? s.InvoiceNavigation.ServiceRecordNavigation.RelocationSupplierPartners.FirstOrDefault().SupplierType.Name : ""),
                            Invoice = s.Id.ToString(),
                            Amount = s.AmountToInvoice.Value,
                            Description = s.InvoiceNavigation.Comments,
                            StatusInvoice = s.StatusNavigation.Status,
                            StatusInvoiceId = s.Status,
                            //AmmountSubTotal = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => _s.PaymentAmount).Sum(),
                            //ManagementFeeSubTotal = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => Convert.ToDecimal(_s.ManagementFee)).Sum(),
                            //WireFeeSubTotal = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => Convert.ToDecimal(_s.WireFee)).Sum(),
                            //AdvanceFeeSubTotal = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).Select(_s => Convert.ToDecimal(_s.AdvenceFee)).Sum(),
                            //FundingRequestedDate = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.FundingRequestDate,
                            //Recurrent = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.Recurrence,
                            //StatusThirdParty = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.StatusNavigation.Status,
                            //StatusThirdPartyId = _context.PaymentConcepts.Where(x => x.ServiceRecord == s.InvoiceNavigation.ServiceRecord.Value && x.WorkOrder == s.WorkOrder).FirstOrDefault().RequestPaymentNavigation.Status,
                        }).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 7).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusId == t.Filters.Where(q => q.Filter1 == 7).SingleOrDefault().Value).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 8).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.StatusInvoiceId == 1).ToList();
                        if (t.Filters.Where(x => x.Filter1 == 9).SingleOrDefault() != null)
                            onlySR = onlySR = onlySR.Where(x => x.AuthoDate > t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().FirstDate.Value &&
                                x.AuthoDate < t.Filters.Where(q => q.Filter1 == 9).SingleOrDefault().SecondDate.Value).ToList();
                        tab.id = t.Id;
                        tab.name = t.Name;
                        tab.columns1 = t.Columns;
                        tab.filters1 = t.Filters;
                        tab.operationals = onlySR;
                        tablesAll.Add(tab);
                    }
                }
            }

            return new ObjectResult(tablesAll) ;

        }

        public ActionResult ReturnReport()
        {
            var reportStandAlone = _context.StandaloneServiceWorkOrders.Select(s => new
            {
                s.WorkOrder.ServiceRecord.NumberServiceRecord,
                service = s.Category.Category,
                s.WorkOrder.NumberWorkOrder,
                s.Acceptance,
                packageOrCoordination = s.Coordination,
                s.Location,
                Country = s.DeliveringInNavigation.Name,
                s.WorkOrder.ServiceLine.ServiceLine,
                s.WorkOrder.ServiceLineId,
                s.WorkOrder.ServiceRecord.Vip,
                Partner = s.WorkOrder.ServiceRecord.Partner.Name,
                Client = s.WorkOrder.ServiceRecord.Client.Name,
                s.WorkOrder.ServiceRecord.PartnerId,
                s.WorkOrder.ServiceRecord.ClientId,
                Coordinator = s.WorkOrder.ServiceLineId == 1 ? s.WorkOrder.ServiceRecord.ImmigrationCoodinators.Select(q => new
                {
                    q.Accepted,
                    Coordinator = $"{q.Coordinator.Name} {q.Coordinator.LastName} {q.Coordinator.MotherLastName}",
                    q.Status.Status,
                    q.StatusId,
                    q.Assigned
                }).ToList() : s.WorkOrder.ServiceRecord.RelocationCoordinators.Select(q => new
                {
                    q.Accepted,
                    Coordinator = $"{q.Coordinator.Name} {q.Coordinator.LastName} {q.Coordinator.MotherLastName}",
                    q.Status.Status,
                    q.StatusId,
                    q.Assigned
                }).ToList(),
                SupplierPartner = s.WorkOrder.ServiceLineId == 1 ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Select(q => new
                {
                    q.AcceptedDate,
                    q.AssignedDate,
                    SupplierPartner = $"{q.Supplier.Name} {q.Supplier.LastName} {q.Supplier.MotherLastName}",
                    COmpany = q.SupplierCompany.ComercialName,
                    q.Status.Status,
                    q.StatusId
                }).ToList() : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.Select(q => new
                {
                    q.AcceptedDate,
                    q.AssignedDate,
                    SupplierPartner = $"{q.Supplier.Name} {q.Supplier.LastName} {q.Supplier.MotherLastName}",
                    COmpany = q.SupplierCompany.ComercialName,
                    q.Status.Status,
                    q.StatusId
                }).ToList(),
                AssigneeInformation = s.WorkOrder.ServiceRecord.AssigneeInformations.Select(n => new
                {
                    n.AssigneeName,
                    n.Birth,
                    n.Pets,
                    n.DependentInformation,
                    n.Sex.Sex,
                    HomeCity = n.HomeCity.Name,
                    HomeCountry = n.HomeCountry.Name,
                    n.Email,
                    HostCity = n.HostCity.City,
                    HostCountry = n.HostCountryNavigation.Name
                }).FirstOrDefault(),
                RequestPayments = s.WorkOrderService.RequestPayments.Select(q => new
                {
                    q.InvoiceDate,
                    q.InvoiceNo,
                    q.Description,
                    q.PaymentDate,
                    q.StatusNavigation.Status,
                    q.Urgent,
                    Payments = q.Payments.Select(p => new
                    {
                        ammount = p.PaymentAmount.ToString() + " " + p.CurrencyPaymentAmountNavigation.Currency,
                        ManagementFee = p.ManagementFee.Length > 0 && p.CurrencyManagementFee.HasValue 
                            ? p.ManagementFee.ToString() + " " + p.CurrencyManagementFeeNavigation.Currency
                            : "N/A",
                        WireFee = p.WireFee.Length > 0 && p.CurrencyWireFee.HasValue 
                            ? p.WireFee.ToString() + " " + p.CurrencyWireFeeNavigation.Currency
                            : "N/A",
                        AdvenceFee = p.AdvenceFee.Length > 0 && p.CurrencyAdvanceFee.HasValue 
                            ? p.AdvenceFee.ToString() + " " + p.CurrencyAdvanceFeeNavigation.Currency
                            : "N/A",
                        p.Urgent
                    }).ToList()
                }).ToList(),
                status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 16 ? s.WorkOrderService.Departures
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 26 ? s.WorkOrderService.Others
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.WorkOrderService.WorkPermits
                    .Select(r => new
                    {
                        Id = 0,
                        Status = ""
                    }),
            }).ToList();
            
            var reportBundled = _context.BundledServices.Select(s => new
            {
                s.BundledServiceOrder.WorkOrder.ServiceRecord.NumberServiceRecord,
                service = s.Category.Category,
                s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                s.Acceptance,
                packageOrCoordination = s.BundledServiceOrder.Package,
                s.Location,
                Country = s.DeliveringInNavigation.Name,
                s.BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
                s.BundledServiceOrder.WorkOrder.ServiceLineId,
                s.BundledServiceOrder.WorkOrder.ServiceRecord.Vip,
                Partner = s.BundledServiceOrder.WorkOrder.ServiceRecord.Partner.Name,
                Client = s.BundledServiceOrder.WorkOrder.ServiceRecord.Client.Name,
                s.BundledServiceOrder.WorkOrder.ServiceRecord.PartnerId,
                s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId,
                Coordinator = s.BundledServiceOrder.WorkOrder.ServiceLineId == 1 ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationCoodinators.Select(q => new
                {
                    q.Accepted,
                    Coordinator = $"{q.Coordinator.Name} {q.Coordinator.LastName} {q.Coordinator.MotherLastName}",
                    q.Status.Status,
                    q.StatusId,
                    q.Assigned
                }).ToList() : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationCoordinators.Select(q => new
                {
                    q.Accepted,
                    Coordinator = $"{q.Coordinator.Name} {q.Coordinator.LastName} {q.Coordinator.MotherLastName}",
                    q.Status.Status,
                    q.StatusId,
                    q.Assigned
                }).ToList(),
                SupplierPartner = s.BundledServiceOrder.WorkOrder.ServiceLineId == 1 ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Select(q => new
                {
                    q.AcceptedDate,
                    q.AssignedDate,
                    SupplierPartner = $"{q.Supplier.Name} {q.Supplier.LastName} {q.Supplier.MotherLastName}",
                    COmpany = q.SupplierCompany.ComercialName,
                    q.Status.Status,
                    q.StatusId
                }).ToList() : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.Select(q => new
                {
                    q.AcceptedDate,
                    q.AssignedDate,
                    SupplierPartner = $"{q.Supplier.Name} {q.Supplier.LastName} {q.Supplier.MotherLastName}",
                    COmpany = q.SupplierCompany.ComercialName,
                    q.Status.Status,
                    q.StatusId
                }).ToList(),
                AssigneeInformation = s.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.Select(n => new
                {
                    n.AssigneeName,
                    n.Birth,
                    n.Pets,
                    n.DependentInformation,
                    n.Sex.Sex,
                    HomeCity = n.HomeCity.Name,
                    HomeCountry = n.HomeCountry.Name,
                    n.Email,
                    HostCity = n.HostCity.City,
                    HostCountry = n.HostCountryNavigation.Name
                }).FirstOrDefault(),
                RequestPayments = s.WorkServices.RequestPayments.Select(q => new
                {
                    q.InvoiceDate,
                    q.InvoiceNo,
                    q.Description,
                    q.PaymentDate,
                    q.StatusNavigation.Status,
                    q.Urgent,
                    Payments = q.Payments.Select(p => new
                    {
                        ammount = p.PaymentAmount.ToString() + " " + p.CurrencyPaymentAmountNavigation.Currency,
                        ManagementFee = p.ManagementFee.Length > 0 && p.CurrencyManagementFee.HasValue 
                            ? p.ManagementFee.ToString() + " " + p.CurrencyManagementFeeNavigation.Currency
                            : "N/A",
                        WireFee = p.WireFee.Length > 0 && p.CurrencyWireFee.HasValue 
                            ? p.WireFee.ToString() + " " + p.CurrencyWireFeeNavigation.Currency
                            : "N/A",
                        AdvenceFee = p.AdvenceFee.Length > 0 && p.CurrencyAdvanceFee.HasValue 
                            ? p.AdvenceFee.ToString() + " " + p.CurrencyAdvanceFeeNavigation.Currency
                            : "N/A",
                        p.Urgent
                    }).ToList()
                }).ToList(),
                status = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 8 ? s.WorkServices.Renewals
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 9 ? s.WorkServices.Notifications
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 16 ? s.WorkServices.Departures
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 19 ? s.WorkServices.Transportations
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                    .Select(r => new
                    {
                        r.Id,
                        r.Status.Status,
                    })
                    : s.CategoryId == 23 ? s.WorkServices.HomeSales
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.CategoryId == 26 ? s.WorkServices.Others
                        .Select(r => new
                        {
                            r.Id,
                            r.Status.Status,
                        })
                    : s.WorkServices.WorkPermits
                    .Select(r => new
                    {
                        Id = 0,
                        Status = ""
                    }),
            }).ToList();

            var sr = reportBundled.Union(reportStandAlone);
            return new ObjectResult(sr);

        }
    }
}
