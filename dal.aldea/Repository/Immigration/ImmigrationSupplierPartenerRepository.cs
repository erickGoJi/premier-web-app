using biz.premier.Repository.Immigration;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Immigration
{
    public class ImmigrationSupplierPartenerRepository : GenericRepository<ImmigrationSupplierPartner>, IImmigrationSupplierPartenerRepository
    {
        public ImmigrationSupplierPartenerRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetAssignedServices(int id)
        {
            var query = _context.WorkOrders.Where(x => x.ServiceRecordId == id)
                .Select(s => new
                {
                    s.Id,
                    StandaloneServices = _context.StandaloneServiceWorkOrders.Where(x => x.ServiceTypeId == 2),
                    PackageServices = _context.BundledServicesWorkOrders
                }).ToList();
            return new ObjectResult(query);
        }

        public int GetCountryById(int id)
        {
            var nameCountry = _context.Countries.Any(x => x.Id == id)
                ? _context.Countries.FirstOrDefault(x => x.Id == id).Name
                : _context.CatCountries.FirstOrDefault(x => x.Id == id).Name;

            var nameCountryId = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == nameCountry.ToLower()).Id;
            
            return nameCountryId;
        }

        public ActionResult GetSuppplierPartnerList(int id)
        {
            var supplier = _context.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == id);
            if(supplier != null)
            {
                var union = _context.AssignedServiceSupliers
                .Where(x => supplier.Select(s => s.Id).Contains(x.ImmigrationSupplierPartnerId.Value))
                .Select(s => new ServicesAssigned
                {
                    Id = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().WorkServicesId
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrderServiceId,
                    ServiceNumber = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().ServiceNumber
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
                    ServiceId = s.ServiceOrderServices.BundledServices.Any()
                    ? s.ServiceOrderServices.BundledServices.FirstOrDefault().Id
                    : s.ServiceOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().Id,
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
                    SupplierId = s.ImmigrationSupplierPartnerId
                }).ToList();

                var unionBundle = (from a in _context.BundledServices
                                   join d in _context.CatServices on a.ServiceId equals d.Id
                                   join e in _context.WorkOrders on a.BundledServiceOrder.WorkOrderId equals e.Id
                                   where e.ServiceRecordId == id && e.ServiceLineId == 1
                                   select new ServicesAssigned
                                   {
                                       Id = a.WorkServicesId,
                                       ServiceNumber = a.ServiceNumber,
                                       ServiceId = a.Id,
                                       CreationDate = e.CreationDate,
                                       Value = a.ServiceId.Value,
                                       Service = d.Service,
                                       Assignee = false,
                                       SupplierId = (int?)0,
                                       StatusId = a.StatusId

                                   }).ToList();
                var unionStandAlone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == id && x.WorkOrder.ServiceLineId == 1)
                    .Select(s => new ServicesAssigned
                    {
                        Id = s.WorkOrderServiceId,
                        ServiceNumber = s.ServiceNumber,
                        ServiceId = s.Id,
                        CreationDate = s.WorkOrder.CreationDate,
                        Value = s.ServiceId.Value,
                        Service = s.Service.Service,
                        Assignee = false,
                        SupplierId = (int?)0,
                        StatusId = s.StatusId
                    }).ToList();
                var unionNoAssigned = unionBundle.Union(unionStandAlone).ToList();

                foreach (var i in union)
                {
                    var l = unionNoAssigned.SingleOrDefault(x => x.ServiceId == i.ServiceId);
                    if (l != null)
                        unionNoAssigned.Remove(l);
                }
                var unionAll = union.Union(unionNoAssigned).ToList();
                int totals = unionAll.Count(x => x.Assignee == true);
                var query = _context.ImmigrationSupplierPartners
                    .Where(x => x.ServiceRecordId == id && x.StatusId != 4)
                    .Select(s => new SupplierAssignedServices
                    {
                        Id = s.Id,
                        SupplierType = s.SupplierType.Name,
                        SupplierCompany = s.SupplierCompany.ComercialName,
                        Supplier = s.Supplier.Name,
                        AssignedDate = s.AssignedDate,
                        AcceptedDate = s.AcceptedDate,
                        ServiceRecordId = s.ServiceRecordId,
                        Total = 0, // unionAll.Count(x => x.Assignee.Equals(true) && x.SupplierId == s.Id),
                        UnionAll = null, // unionAll.Where(x => x.SupplierId == s.Id || x.Assignee.Equals(false)).ToList(),
                        Status = s.StatusId.HasValue ? s.Status.Status.Trim() : "Pending Accepted",
                        StatusId = s.StatusId,
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
                    }).ToList();

                if(query.Count() > 0)
                {
                    foreach (var a in query)
                    {
                        a.Total = unionAll.Count(x => x.Assignee.Equals(true) && x.SupplierId == a.Id);
                        a.UnionAll = unionAll.Where(x => x.SupplierId == a.Id || x.Assignee.Equals(false)).ToList();
                    }

                    return new ObjectResult(query);
                }
                else
                {

                    return new ObjectResult(unionAll.Where(x => x.StatusId != 4));

                }
            }
            else
            {
                
                return new ObjectResult(null);
            }
            
        }

        private class SupplierAssignedServices
        {
            public int Id { get; set; }
            public string SupplierType { get; set; }
            public string SupplierCompany { get; set; }
            public string Supplier { get; set; }
            public string Status { get; set; }
            public int? StatusId { get; set; }
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
            public int? ServiceId { get; set; }
            public string Service { get; set; }
            public bool Assignee { get; set; }
            public DateTime? CreationDate { get; set; }
            public int? Value { get; set; }
            public int? StatusId { get; set; }
        }

    }
}
