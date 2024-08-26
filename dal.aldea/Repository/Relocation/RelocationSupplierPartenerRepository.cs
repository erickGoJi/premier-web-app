using biz.premier.Entities;
using biz.premier.Repository.Relocation;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Relocation
{
    public class RelocationSupplierPartenerRepository : GenericRepository<RelocationSupplierPartner>, IRelocationSupplierPartenerRepository
    {
        public RelocationSupplierPartenerRepository(Db_PremierContext context): base(context) { }

        public ActionResult GetAssignedServices(int id)
        {
            var query = _context.WorkOrders.Where(x => x.ServiceRecordId == id)
                .Select(s => new
                {
                    s.StandaloneServiceWorkOrders,
                    s.BundledServicesWorkOrders
                }).ToList();
            return new ObjectResult(query);
        }

        
        public List<AssignedServiceSuplier> GetAssignedServiceSuplierById(int id, int sr_id) {
            var union = new List<AssignedServiceSuplier>(); //_context.AssignedServiceSupliers.Where(x => x.RelocationSupplierPartnerId == id).ToList();


            var supplier = _context.RelocationSupplierPartners.Where(x => x.ServiceRecordId == sr_id);

            if (supplier != null)
            {
                union = _context.AssignedServiceSupliers
                .Where(x => supplier.Select(s => s.Id).Contains(x.RelocationSupplierPartnerId.Value) && x.RelocationSupplierPartnerId == id).ToList();
            }

             return union;
        }

        public ActionResult GetSuppplierPartnerList(int id, int? countryId)
        {
            var supplier = _context.RelocationSupplierPartners
                .Include(x => x.Supplier)
                .Where(x => x.ServiceRecordId == id);
            if (supplier != null)
            {
                var union = _context.AssignedServiceSupliers
                .Where(x => supplier.Select(s => s.Id).Contains(x.RelocationSupplierPartnerId.Value))
                .Select(s => new ServicesAssigned
                {
                    Id = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().WorkServicesId
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrderServiceId,
                    ServiceNumber = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().ServiceNumber
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
                    CreationDate = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.CreationDate
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.CreationDate,
                    Value = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().ServiceId.Value
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId.Value,
                    Service = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().Service.Service
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().Service.Service,
                    Assignee = true,
                    SupplierId = s.RelocationSupplierPartnerId
                }).ToList();
                var unionBundle = (from a in _context.BundledServices
                                   join d in _context.CatServices on a.ServiceId equals d.Id
                                   join e in _context.WorkOrders on a.BundledServiceOrder.WorkOrderId equals e.Id
                                   where e.ServiceRecordId == id && e.ServiceLineId == 2
                                   select new ServicesAssigned
                                   {
                                       Id = a.WorkServicesId,
                                       ServiceNumber = a.ServiceNumber,
                                       CreationDate = e.CreationDate,
                                       Value = a.ServiceId.Value,
                                       Service = d.Service,
                                       Assignee = false,
                                       SupplierId = (int?)0,
                                       StatusId = a.StatusId,
                                       CountryId = a.DeliveringIn
                                   }).ToList();
                var unionStandAlone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == id && x.WorkOrder.ServiceLineId == 2)
                    .Select(s => new ServicesAssigned
                    {
                        Id = s.WorkOrderServiceId,
                        ServiceNumber = s.ServiceNumber,
                        CreationDate = s.WorkOrder.CreationDate,
                        Value = s.ServiceId.Value,
                        Service = s.Service.Service,
                        Assignee = false,
                        SupplierId = (int?)0,
                        StatusId = s.StatusId,
                        CountryId = s.DeliveringIn
                    }).ToList();
                var unionNoAssigned = unionBundle.Union(unionStandAlone).ToList();
                foreach (var i in union)
                {
                    var l = unionNoAssigned.SingleOrDefault(x => x.ServiceNumber == i.ServiceNumber);
                    if (l != null)
                        unionNoAssigned.Remove(l);
                }
                var unionAll = union.Union(unionNoAssigned).ToList();
                var query = _context.RelocationSupplierPartners
                    .Where(x => x.ServiceRecordId == id)
                    .Select(s => new SupplierAssignedServices
                    {
                        Id = s.Id,
                        SupplierType = s.SupplierType.Name,
                        SupplierCompany = s.SupplierCompany.ComercialName,
                        Supplier = s.Supplier.Name,
                        AssignedDate = s.AssignedDate,
                        AcceptedDate = s.AcceptedDate,
                        ServiceRecordId = s.ServiceRecordId.Value,
                        Total = 0,
                        UnionAll = null,
                        Status = s.StatusId.HasValue ? s.Status.Status : "Pending Accepted",
                        StatusId = s.StatusId.Value,
                        CountryId = s.Supplier.Country,
                        Supplier_Detail = (from a in _context.ProfileUsers
                                           join u in _context.Users on a.UserId equals u.Id
                                           where a.Id == s.SupplierId
                                           select new SupplierDetail
                                           {
                                               Id = a.Id,
                                               Photo = a.Photo == null || a.Photo == "" ? "Files/assets/avatar.png" : a.Photo,
                                               Email = u.Email, // "email@premier.com",
                                               Name = $"{a.Name} {a.LastName} {a.MotherLastName}",
                                               Mobile = u.MobilePhone, // "55555555555",
                                               Phone = a.PhoneNumber, // "000000000000",
                                               Supplier_Since = u.CreatedDate // "01/01/1900"
                                           }).ToList()
                    }).Where(x => x.StatusId != 4).ToList();

                //if(countryId != null)
                //{
                //    query = query.Where(x => x.CountryId == countryId).ToList();
                //}

                if(query.Count() > 0)
                {
                    if (countryId == null)
                    {
                        foreach (var a in query)
                        {
                            a.Total = unionAll.Count(x => x.Assignee.Equals(true) && x.SupplierId == a.Id);
                            a.UnionAll = unionAll.Where(x => x.SupplierId == a.Id || x.Assignee.Equals(false)).ToList();
                        }
                    }
                    else
                    {
                        var _country = _context.CatCountries.FirstOrDefault(x => x.Id == countryId).Id;
                        if (_country == null)
                        {
                            var __country = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == _context.Countries.FirstOrDefault(a => a.Id == countryId).Name.ToLower()).Id;
                            _country = __country;
                        }

                        foreach (var a in query)
                        {
                            a.Total = unionAll.Count(x => x.Assignee.Equals(true) && x.SupplierId == a.Id);
                            a.UnionAll = unionAll.Where(x => x.SupplierId == a.Id || x.Assignee.Equals(false) && x.CountryId == _country).ToList();
                        }
                    }
                    

                    return new ObjectResult(query);
                }
                else
                {

                    if (countryId == null)
                    {
                        return new ObjectResult(unionAll);
                    }
                    else
                    {
                        var _country = _context.CatCountries.FirstOrDefault(x => x.Id == countryId)?.Id;
                        if(_country == null)
                        {
                            var __country = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == _context.Countries.FirstOrDefault(a => a.Id == countryId).Name.ToLower()).Id;
                            _country = __country;
                        } 
                        return new ObjectResult(unionAll = unionAll.Where(x => x.CountryId == _country).ToList());
                    }
                    
                }
                
            }
            else
            {
                return new ObjectResult(null);
            }
        }

        public int GetCountryById(int id)
        {
            var nameCountry = _context.Countries.Any(x => x.Id == id)
                ? _context.Countries.FirstOrDefault(x => x.Id == id).Name
                : _context.CatCountries.FirstOrDefault(x => x.Id == id).Name;

            var nameCountryId = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == nameCountry.ToLower()).Id;

            return nameCountryId;
        }

        private class SupplierAssignedServices
        {
            public int Id { get; set; }
            public string SupplierType { get; set; }
            public string SupplierCompany { get; set; }
            public string Supplier { get; set; }
            public string Status { get; set; }
            public int StatusId { get; set; }
            public int? CountryId { get; set; }
            public List<SupplierDetail> Supplier_Detail { get; set; }
            public List<ServicesAssigned> UnionAll { get; set; }
            public DateTime? AssignedDate { get; set; }
            public DateTime? AcceptedDate { get; set; }
            public int ServiceRecordId { get; set; }
            public int Total { get; set; }
        }
        private class SupplierDetail
        {
            public int Id { get; set; }
            public string Photo { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Phone { get; set; }
            public DateTime? Supplier_Since { get; set; }
        }
        private class ServicesAssigned
        {
            public int? Id { get; set; }
            public int? SupplierId { get; set; }
            public string ServiceNumber { get; set; }
            public string Service { get; set; }
            public bool Assignee { get; set; }
            public DateTime? CreationDate { get; set; }
            public int? Value { get; set; }
            public int? StatusId { get; set; }
            public int? CountryId { get; set; }
        }
    }
}
