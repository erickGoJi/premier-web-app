using biz.premier.Entities;
using biz.premier.Models;
using biz.premier.Repository.ServiceOrder;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.ServiceOrder
{
    public class WorkOrderRepository : GenericRepository<biz.premier.Entities.WorkOrder>, IWorkOrderRepository
    {
        public WorkOrderRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetOrderByWo(int so)
        {
            var exist = _context.Set<biz.premier.Entities.WorkOrder>()
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(y => y.WorkOrder.ServiceRecord)
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(_i => _i.BundledServices)
                        .ThenInclude(r => r.Service).ThenInclude(c => c.ServiceLocations)
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(_i => _i.BundledServices)
                            .ThenInclude(i => i.DeliveredToNavigation)
                                .ThenInclude(i => i.Relationship)
                .Include(i => i.StandaloneServiceWorkOrders)
                    .ThenInclude(y => y.WorkOrder.ServiceRecord)
                .Include(i => i.StandaloneServiceWorkOrders)
                    .ThenInclude(r => r.Service).ThenInclude(c => c.ServiceLocations)
                //.Where(x => x.IdClientPartnerProfile == _context.WorkOrders.FirstOrDefault(wo => wo.Id == so).ServiceRecord.PartnerId))
                .Include(i => i.StandaloneServiceWorkOrders)
                    .ThenInclude(i => i.DeliveredToNavigation)
                        .ThenInclude(i => i.Relationship)
                .Where(x => x.Id == so).ToList();

            var set = new HashSet<BundledService>();
            foreach (var list in exist.FirstOrDefault().BundledServicesWorkOrders)
            {
                foreach (var item in list.BundledServices)
                {
                    set.Add(item);
                }
            }

            var result = set.Count;
            var consult = new
            {
                workOrder = exist.Select(f => new
                {
                    f.Id,
                    f.NumberWorkOrder,
                    f.CreationDate,
                    f.ServiceLineId,
                    f.ServiceRecordId,
                    f.StatusId,
                    f.CreatedBy,
                    f.CreatedDate,
                    f.UpdateBy,
                    f.UpdatedDate,
                    f.ServiceRecord.PartnerId,
                    StandaloneServiceWorkOrders = f.StandaloneServiceWorkOrders.Select(h => new
                    {
                        h?.Id,
                        h?.ServiceNumber,
                        h?.WorkOrderId,
                        h?.DeliveredTo,
                        h?.DeliveringIn,
                        h?.ServiceId,
                        h?.ServiceTypeId,
                        h?.Location,
                        h?.CategoryId,
                        h?.Autho,
                        h?.Acceptance,
                        h?.Coordination,
                        h?.AuthoTime,
                        h?.ProjectedFee,
                        h?.StatusId,
                        h?.CreatedBy,
                        h?.CreatedDate,
                        h?.UpdateBy,
                        h?.UpdatedDate,
                        h?.WorkOrderServiceId,
                        h?.InvoiceSupplier,
                        h?.BillingHour,
                        h?.Category,
                        h?.Service.Service,
                        nickname = h.Service.ServiceLocations.FirstOrDefault(k => k.IdClientPartnerProfile == f.ServiceRecord.ClientId)?.NickName == "--"
                        ? h.Service.Service : h.Service.ServiceLocations.FirstOrDefault(k => k.IdClientPartnerProfile == f.ServiceRecord.ClientId)?.NickName
                    }),
                    BundledServicesWorkOrders = f.BundledServicesWorkOrders.Select(g => new
                    {
                        g.Id,
                        g.WorkOrderId,
                        g.TotalTime,
                        g.ProjectedFee,
                        g.Package,
                        g.CreatedBy,
                        g.CreatedDate,
                        g.UpdateBy,
                        g.UpdatedDate,
                        BundledServices = g.BundledServices.Select(j => new {
                            j.Id,
                            j.ServiceNumber,
                            j.DeliveredTo,
                            j.DeliveringIn,
                            j.ServiceId,
                            j.ServiceTypeId,
                            j.Location,
                            j.CategoryId,
                            j.Autho,
                            j.Acceptance,
                            j.StatusId,
                            j.CreatedBy,
                            j.CreatedDate,
                            j.UpdateBy,
                            j.UpdatedDate,
                            j.WorkServicesId,
                            j.InvoiceSupplier,
                            j.BillingHour,
                            j.Service.Service,
                            j.Category,
                            nickname = j.Service.ServiceLocations.FirstOrDefault(k => k.IdClientPartnerProfile == f.ServiceRecord.PartnerId).NickName == "--"
                            ? j.Service.Service : j.Service.ServiceLocations.FirstOrDefault(k => k.IdClientPartnerProfile == f.ServiceRecord.PartnerId).NickName
                        })

                    })

                }),
                totalServices = exist.FirstOrDefault().StandaloneServiceWorkOrders.Count + result,
                standalone = exist.FirstOrDefault().StandaloneServiceWorkOrders.Sum(s => double.Parse(s.ProjectedFee)),
                bundled = exist.FirstOrDefault().BundledServicesWorkOrders.Sum(s => double.Parse(s.ProjectedFee)),
                projectFee = exist.FirstOrDefault().BundledServicesWorkOrders.Sum(s => double.Parse(s.ProjectedFee)) + exist.FirstOrDefault().StandaloneServiceWorkOrders.Sum(s => double.Parse(s.ProjectedFee))
            };
            return new ObjectResult(consult);
        }

        public ActionResult GetOrder(int so)
        {
            //service  = _context.ServiceLocations.FirstOrDefault(x => x.IdService == n.ServiceId && x.IdClientPartnerProfile == q.ServiceRecord.PartnerId).NickName,
            var exist = _context.Set<biz.premier.Entities.WorkOrder>()
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(_i => _i.BundledServices)
                        .ThenInclude(r => r.Service).ThenInclude(c => c.ServiceLocations)
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(_i => _i.BundledServices)
                            .ThenInclude(i => i.DeliveredToNavigation)
                                .ThenInclude(i => i.Relationship)
                .Include(i => i.StandaloneServiceWorkOrders)
                    .ThenInclude(r => r.Service).ThenInclude(c => c.ServiceLocations)
                //.Where(x => x.IdClientPartnerProfile == _context.WorkOrders.FirstOrDefault(wo => wo.Id == so).ServiceRecord.PartnerId))
                .Include(i => i.StandaloneServiceWorkOrders)
                    .ThenInclude(i => i.DeliveredToNavigation)
                        .ThenInclude(i => i.Relationship)
                .Single(x => x.Id == so);
            var set = new HashSet<BundledService>();
            foreach (var list in exist.BundledServicesWorkOrders)
            {
                foreach (var item in list.BundledServices)
                {
                    set.Add(item);
                }
            }

            var result = set.Count;
            var consult = new
            {
                workOrder = exist,
                totalServices = exist.StandaloneServiceWorkOrders.Count + result,
                standalone = exist.StandaloneServiceWorkOrders.Sum(s => double.Parse(s.ProjectedFee)),
                bundled = exist.BundledServicesWorkOrders.Sum(s => double.Parse(s.ProjectedFee)),
                projectFee = exist.BundledServicesWorkOrders.Sum(s => double.Parse(s.ProjectedFee)) + exist.StandaloneServiceWorkOrders.Sum(s => double.Parse(s.ProjectedFee))
            };
            return new ObjectResult(consult);
        }

        public biz.premier.Entities.WorkOrder GetWorkOrder(int wo)
        {
            var exist = _context.Set<biz.premier.Entities.WorkOrder>()
                .Include(i => i.BundledServicesWorkOrders)
                .ThenInclude(_i => _i.BundledServices)
                .Include(i => i.StandaloneServiceWorkOrders)
                .SingleOrDefault(x => x.Id == wo);
            return exist;
        }

        public class ListServiceOrders
        {
            public int ServiceStatus { get; set; }
        }

        public ActionResult GetOrders(int so, int? supplierPartner)
        {
            int[] _wo = _context.Payments.Where(x => x.SupplierPartner == supplierPartner).Select(s => s.WorkOrder.Value).ToArray().Length == 0
                ? _context.Payments.Where(x => x.Id == supplierPartner).Select(s => s.WorkOrder.Value).ToArray()
                : _context.Payments.Where(x => x.SupplierPartner == supplierPartner).Select(s => s.WorkOrder.Value).ToArray();


            var exist = _context.Set<biz.premier.Entities.WorkOrder>().Where(x => x.ServiceRecordId == so)
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(_i => _i.BundledServices)
                .Include(i => i.StandaloneServiceWorkOrders)
                .Include(i => i.ServiceLine)
                .Include(i => i.Status)
                .Include(i => i.ServiceRecord).ThenInclude(i => i.ImmigrationSupplierPartners)
                .Include(i => i.ServiceRecord).ThenInclude(i => i.RelocationSupplierPartners)
                .ToList();
            if (exist.Any())
            {
                var query = supplierPartner.HasValue ? exist
                    .Where(x => (x.ServiceRecord.ImmigrationSupplierPartners.Select(s => s.SupplierId).Contains(supplierPartner)
                        || x.ServiceRecord.RelocationSupplierPartners.Select(s => s.SupplierId).Contains(supplierPartner)))
                    .Select(q => new
                    {
                        q.Id,
                        serviceOrderId = q.NumberWorkOrder,
                        servicesLine = q.ServiceLine.ServiceLine,
                        services = q.BundledServicesWorkOrders.Any() && q.StandaloneServiceWorkOrders.Any() ?
                        (q.BundledServicesWorkOrders.Sum(d => d.BundledServices.Count) + q.StandaloneServiceWorkOrders.Count).ToString()
                        : q.StandaloneServiceWorkOrders.Any() ? q.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber : q.BundledServicesWorkOrders.Sum(d => d.BundledServices.Count).ToString(),
                        autho = q.CreatedDate,
                        status = q.StatusId.HasValue ? q.Status.Status : "N/A",
                        completeImm = ServiceCompletePercentageImm(_context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == so).Select(s => new ListServiceOrders
                        {
                            ServiceStatus = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = 0
                            }).FirstOrDefault().StatusId
                        }).ToList()),
                        completeRelo = ServiceCompletePercentageRelo(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == so).Select(s => new ListServiceOrders
                        {
                            ServiceStatus = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = 0
                            }).FirstOrDefault().StatusId
                        }).ToList()),
                        servicesData = _context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrderId == q.Id).Select(n => new
                        {
                            id = n.WorkServicesId,
                            n.Service.Service,
                            n.ServiceNumber,
                            n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                            n.CategoryId,
                            n.Autho,
                            services = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 5 ? _context.CatCategories
                              .Where(x => x.Id == 5)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 6 ? _context.CatCategories
                              .Where(x => x.Id == 6)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 8 ? n.WorkServices.Renewals
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 9 ? n.WorkServices.Notifications
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 16 ? n.WorkServices.Departures
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 19 ? _context.CatCategories
                              .Where(x => x.Id == 19)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 20 ? _context.CatCategories
                              .Where(x => x.Id == 20)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 23 ? n.WorkServices.HomeSales
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 26 ? n.WorkServices.Others
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 28 ? n.WorkServices.Others
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.WorkServices.WorkPermits
                                  .Select(r => new
                                  {
                                      Id = 0
                                  }).ToList()
                        }).ToList().Union(_context.StandaloneServiceWorkOrders
                            .Where(x => x.WorkOrderId == q.Id)
                            .Select(n => new
                            {
                                id = n.WorkOrderServiceId,
                                n.Service.Service,
                                n.ServiceNumber,
                                n.WorkOrder.NumberWorkOrder,
                                n.CategoryId,
                                n.Autho,
                                services = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 5 ? _context.CatCategories
                                .Where(x => x.Id == 5)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 6 ? _context.CatCategories
                                .Where(x => x.Id == 6)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 16 ? n.WorkOrderService.Departures
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 19 ? _context.CatCategories
                                .Where(x => x.Id == 19)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 20 ? _context.CatCategories
                                .Where(x => x.Id == 20)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 26 ? n.WorkOrderService.Others
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 28 ? n.WorkOrderService.Others
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.WorkOrderService.WorkPermits
                                .Select(r => new
                                {
                                    Id = 0
                                }).ToList()
                            }).ToList()).ToList()
                    }).ToList()
                : exist.Select(q => new
                {
                    q.Id,
                    serviceOrderId = q.NumberWorkOrder,
                    servicesLine = q.ServiceLine.ServiceLine,
                    services = q.BundledServicesWorkOrders.Any() && q.StandaloneServiceWorkOrders.Any() ?
                        (q.BundledServicesWorkOrders.Sum(d => d.BundledServices.Count) + q.StandaloneServiceWorkOrders.Count).ToString()
                        : q.StandaloneServiceWorkOrders.Any() ? q.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber : q.BundledServicesWorkOrders.Sum(d => d.BundledServices.Count).ToString(),
                    autho = q.CreatedDate,
                    status = q.StatusId.HasValue ? q.Status.Status : "N/A",
                    completeImm = ServiceCompletePercentageImm(_context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == so).Select(s => new ListServiceOrders
                    {
                        ServiceStatus = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = 0
                            }).FirstOrDefault().StatusId
                    }).ToList()),
                    completeRelo = ServiceCompletePercentageRelo(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == so).Select(s => new ListServiceOrders
                    {
                        ServiceStatus = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault().StatusId
                        : s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                StatusId = 0
                            }).FirstOrDefault().StatusId
                    }).ToList()),
                    servicesData = _context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrderId == q.Id).Select(n => new
                    {
                        id = n.WorkServicesId,
                        n.Service.Service,
                        n.ServiceNumber,
                        n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        n.CategoryId,
                        n.Autho,
                        services = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 5 ? _context.CatCategories
                              .Where(x => x.Id == 5)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 6 ? _context.CatCategories
                              .Where(x => x.Id == 6)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 8 ? n.WorkServices.Renewals
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 9 ? n.WorkServices.Notifications
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 16 ? n.WorkServices.Departures
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 19 ? _context.CatCategories
                              .Where(x => x.Id == 19)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 20 ? _context.CatCategories
                              .Where(x => x.Id == 20)
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                              .Select(r => new
                              {
                                  r.Id
                              }).ToList()
                              : n.CategoryId == 23 ? n.WorkServices.HomeSales
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 26 ? n.WorkServices.Others
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.CategoryId == 28 ? n.WorkServices.Others
                                  .Select(r => new
                                  {
                                      r.Id
                                  }).ToList()
                              : n.WorkServices.WorkPermits
                              .Select(r => new
                              {
                                  Id = 0
                              }).ToList()
                    }).ToList().Union(_context.StandaloneServiceWorkOrders
                            .Where(x => x.WorkOrderId == q.Id)
                            .Select(n => new
                            {
                                id = n.WorkOrderServiceId,
                                n.Service.Service,
                                n.ServiceNumber,
                                n.WorkOrder.NumberWorkOrder,
                                n.CategoryId,
                                n.Autho,
                                services = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 5 ? _context.CatCategories
                                .Where(x => x.Id == 5)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 6 ? _context.CatCategories
                                .Where(x => x.Id == 6)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 16 ? n.WorkOrderService.Departures
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 19 ? _context.CatCategories
                                .Where(x => x.Id == 19)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 20 ? _context.CatCategories
                                .Where(x => x.Id == 20)
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                                : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 26 ? n.WorkOrderService.Others
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.CategoryId == 28 ? n.WorkOrderService.Others
                                    .Select(r => new
                                    {
                                        r.Id
                                    }).ToList()
                                : n.WorkOrderService.WorkPermits
                                .Select(r => new
                                {
                                    Id = 0
                                }).ToList()
                            }).ToList()).ToList()
                }).ToList();
                return new ObjectResult(query);
            }
            else
            {
                return new ObjectResult(null);
            }
        }

        private string ServiceCompletePercentageImm(List<ListServiceOrders> immigration)
        {
            var services = immigration;
            int allServices = services.Count(x => x.ServiceStatus == 4 || x.ServiceStatus == 5);
            decimal completeServices = allServices != 0 ? allServices * 100 / services.Count() : 0;
            var res = Math.Abs(completeServices);
            return res.ToString("P");
        }

        private string ServiceCompletePercentageRelo(List<ListServiceOrders> relocation)
        {
            var services = relocation;
            int allServices = services.Count(x => x.ServiceStatus == 4 || x.ServiceStatus == 5);
            decimal completeServices = allServices != 0 ? allServices * 100 / services.Count() : 0;
            var res = Math.Abs(completeServices);
            return res.ToString("P");
        }

        public biz.premier.Entities.WorkOrder UpdateCustom(biz.premier.Entities.WorkOrder order, int key)
        {
            if (order == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.WorkOrder>()
                .Include(i => i.StandaloneServiceWorkOrders)
                .Include(i => i.BundledServicesWorkOrders)
                    .ThenInclude(_i => _i.BundledServices)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(order);
                foreach (var package in order.StandaloneServiceWorkOrders)
                {
                    var existingPackage = exist.StandaloneServiceWorkOrders.Where(p => p.Id == package.Id).FirstOrDefault();
                    if (existingPackage == null)
                    {
                        exist.StandaloneServiceWorkOrders.Add(package);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingPackage).CurrentValues.SetValues(package);
                    }
                }
                foreach (var package in order.BundledServicesWorkOrders)
                {
                    var existingPackage = exist.BundledServicesWorkOrders.Where(p => p.Id == package.Id).FirstOrDefault();
                    //existingPackage.BundledServices = e 
                    if (existingPackage == null)
                    {
                        exist.BundledServicesWorkOrders.Add(package);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingPackage).CurrentValues.SetValues(package);
                        foreach (var i in package.BundledServices)
                        {
                            var _exist = existingPackage.BundledServices.Where(x => x.Id == i.Id).FirstOrDefault();
                            if (_exist == null)
                            {
                                existingPackage.BundledServices.Add(i);
                                _context.SaveChanges();
                            }
                            else
                            {
                              //  _context.Entry(_exist).CurrentValues.SetValues(i);
                            }
                        }
                    }
                }
                _context.SaveChanges();
            }
            return exist;
        }

        public int last_id()
        {

            var lastId = _context.WorkOrders.OrderByDescending(x => x.Id).FirstOrDefault();
            int num = lastId != null ? lastId.Id + 1 : 1;
            return num;
        }

        public int lastIdServiceOrderServices()
        {
            var lastId = _context.StandaloneServiceWorkOrders.Select(s => s.Id).DefaultIfEmpty().OrderByDescending(x => x).FirstOrDefault();
            int num = lastId != 0 ? lastId + 1 : 1;
            return num;
        }


        public ActionResult GetServiceStandalone(int service_order_id)
        {
            var service = _context.StandaloneServiceWorkOrders
                .Where(e => e.WorkOrderId == service_order_id && e.ServiceTypeId == 1)
                .Join(_context.CatRelationships, e => e.DeliveredTo, a => a.Id, (e, a) => new
                {
                    e.Id,
                    Authodate = e.WorkOrder.CreatedDate,
                    DeleveredTo = a.Relationship,
                    DeliviringTo = e.DeliveringInNavigation.Name,
                    e.WorkOrder.NumberWorkOrder,
                    e.Service.Service,
                    e.Location,
                    Acceptance_date = e.Acceptance.Value.ToShortDateString(),
                    e.Coordination,
                    AuthoTime = e.AuthoTime == null ? 0 : e.AuthoTime,
                    e.ProjectedFee
                }).ToList();

            return new ObjectResult(service);
        }

        public ActionResult GetServicePackage(int service_order_id)
        {
            var service = _context.BundledServices
                .Where(e => e.BundledServiceOrder.WorkOrderId == service_order_id)
                .Join(_context.CatRelationships, e => e.DeliveredTo, a => a.Id, (e, a) => new
                {
                    e.Id,
                    e.BundledServiceOrder.TotalTime,
                    e.BundledServiceOrder.ProjectedFee,
                    Authodate = e.Autho,
                    DeleveredTo = a.Relationship,
                    DeliviringTo = e.DeliveringInNavigation.Name,
                    e.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    e.Service.Service,
                    e.Location,
                    Acceptance_date = e.Acceptance.Value.ToShortDateString(),
                }).ToList();

            return new ObjectResult(service);
        }

        public ActionResult GetServiceAllService(int serviceLineId, int serviceRecordId, int? service_type_id, int? status_id, DateTime? date_in, DateTime? date_last)
        {

            //var service = _context.WorkOrders
            //    .Include(i => i.BundledServicesWorkOrders)
            //    .ThenInclude(_i => _i.BundledServices)
            //    .Include(i => i.StandaloneServiceWorkOrders)
            //    .Where(x => x.ServiceLineId == serviceLineId && x.ServiceRecordId == serviceRecordId)
            //    .Select(e => new
            //    {
            //        e.Id,
            //        e.ServiceRecordId,
            //        e.NumberWorkOrder,
            //        Service = e.StandaloneServiceWorkOrders.FirstOrDefault().Service.Service.Any()
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().Service.Service : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().Service.Service,
            //        ServiceNumber = e.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber.Any()
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().ServiceNumber,
            //        program = e.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber.Any()
            //        ? "Standalone" : "Bundled",
            //        Category = e.StandaloneServiceWorkOrders.FirstOrDefault().Category.Category.Any()
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().Category.Category : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().ServiceNumber,
            //        Location = e.StandaloneServiceWorkOrders.FirstOrDefault().Location.Any()
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().Location : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().Location,
            //        deliveredTo = e.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Name.Any()
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Name + " / " + e.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Relationship.Relationship
            //        : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().DeliveredToNavigation.Name + " / " + e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().DeliveredToNavigation.Relationship.Relationship,
            //        Autho = e.StandaloneServiceWorkOrders.FirstOrDefault().Autho.HasValue
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().Autho : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().Autho,
            //        e.ServiceRecord.ClosedDate,
            //        e.Status.Status,
            //        e.StatusId,
            //        coordinator = serviceLineId == 1 ? _context.ImmigrationCoodinators.SingleOrDefault(x => x.ServiceRecordId == e.ServiceRecordId).Coordinator.Name
            //        + " " + _context.ImmigrationCoodinators.SingleOrDefault(x => x.ServiceRecordId == e.ServiceRecordId).Coordinator.LastName
            //        : _context.RelocationCoordinators.SingleOrDefault(x => x.ServiceRecordId == e.ServiceRecordId).Coordinator.Name 
            //        + " " + _context.ImmigrationCoodinators.SingleOrDefault(x => x.ServiceRecordId == e.ServiceRecordId).Coordinator.LastName,
            //        ProjectedFee = e.StandaloneServiceWorkOrders.FirstOrDefault().ProjectedFee.Any()
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().ProjectedFee : e.BundledServicesWorkOrders.FirstOrDefault().ProjectedFee,
            //        ServiceTypeId = e.StandaloneServiceWorkOrders.FirstOrDefault().ServiceTypeId == null
            //        ? e.StandaloneServiceWorkOrders.FirstOrDefault().ServiceTypeId : e.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().ServiceTypeId
            //    }).ToList();

            var service = _context.Set<biz.premier.Entities.StandaloneServiceWorkOrder>()
               .Where(x => x.WorkOrder.ServiceLineId == serviceLineId && x.WorkOrder.ServiceRecordId == serviceRecordId).Select(s => new
               {
                   s.WorkOrder.Id,
                   s.ServiceTypeId,
                   s.StatusId,
                   s.WorkOrder.ServiceLineId,
                   s.WorkOrder.NumberWorkOrder,
                   s.Service.Service,
                   s.ServiceNumber,
                   program = "Standalone",
                   s.Category.Category,
                   s.Location,
                   deliveredTo = s.DeliveredToNavigation.Name + " / " + s.DeliveredToNavigation.Relationship.Relationship,
                   s.Autho,
                   s.WorkOrder.ServiceRecord.ClosedDate,
                   s.Status.Status,
                   coordinator = serviceLineId == 1 ? _context.ImmigrationCoodinators.SingleOrDefault(x => x.ServiceRecordId == s.WorkOrder.ServiceRecordId).Coordinator.Name
                   : _context.RelocationCoordinators.SingleOrDefault(x => x.ServiceRecordId == s.WorkOrder.ServiceRecordId).Coordinator.Name,
                   supplier = serviceLineId == 1 ?
                   (_context.ImmigrationSupplierPartners.Count() > 1
                   ? _context.ImmigrationSupplierPartners.SingleOrDefault(x => x.ServiceRecordId == s.WorkOrder.ServiceRecordId).Supplier.Name
                   : "Not assigned")
                   : (_context.RelocationSupplierPartners.Count() > 1
                   ? _context.RelocationSupplierPartners.SingleOrDefault(x => x.ServiceRecordId == s.WorkOrder.ServiceRecordId).Supplier.Name
                   : "Not assigned"),
                   s.ProjectedFee
               }).ToList();
            var bundle = _context.Set<biz.premier.Entities.BundledService>()
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLineId && x.BundledServiceOrder.WorkOrder.ServiceRecordId == serviceRecordId).Select(s => new
                {
                    s.BundledServiceOrder.WorkOrder.Id,
                    s.ServiceTypeId,
                    s.StatusId,
                    s.BundledServiceOrder.WorkOrder.ServiceLineId,
                    s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    s.Service.Service,
                    s.ServiceNumber,
                    program = "Bundle",
                    s.Category.Category,
                    s.Location,
                    deliveredTo = s.DeliveredToNavigation.Name + " / " + s.DeliveredToNavigation.Relationship.Relationship,
                    s.Autho,
                    s.BundledServiceOrder.WorkOrder.ServiceRecord.ClosedDate,
                    s.Status.Status,
                    coordinator = serviceLineId == 1 ? _context.ImmigrationCoodinators.SingleOrDefault(x => x.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Coordinator.Name
                   : _context.RelocationCoordinators.SingleOrDefault(x => x.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Coordinator.Name,
                    supplier = serviceLineId == 1 ?
                   (_context.ImmigrationSupplierPartners.Count() > 1
                   ? _context.ImmigrationSupplierPartners.Count().ToString()
                   : _context.ImmigrationSupplierPartners.SingleOrDefault(x => x.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Supplier.Name)
                   : (_context.RelocationSupplierPartners.Count() > 1
                   ? _context.RelocationSupplierPartners.Count().ToString()
                   : _context.RelocationSupplierPartners.SingleOrDefault(x => x.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Supplier.Name),
                    s.BundledServiceOrder.ProjectedFee
                }).ToList().Union(service);

            if (service_type_id != 0 && service_type_id != null)
            {
                service = service.Where(x => x.ServiceTypeId == service_type_id).ToList();
            }

            if (status_id != 0 && status_id != null)
            {
                service = service.Where(x => x.StatusId == status_id).ToList();
            }

            if (date_in != null && date_last != null)
            {
                service = service.Where(x => x.Autho.Value.Date >= date_in.Value.Date && x.Autho.Value.Date <= date_last.Value.Date).ToList();
            }

            //if (date_in.Value.Date != Convert.ToDateTime("01/01/1900").Date && date_last.Value.Date != Convert.ToDateTime("01/01/1900").Date)
            //{
            //    service = service.Where(x => x.Autho.Value.Date >= date_in.Value.Date && x.Autho.Value.Date <= date_last.Value.Date).ToList();
            //}

            return new ObjectResult(service);
            //return null;
        }
        public class InvoiceWO
        {
            public int? WorkOrderId { get; set; }
            public int Id { get; set; }
            public string NumberWorkOrder { get; set; }
            public string Service { get; set; }
            public List<string> Services { get; set; }
            public string Status { get; set; }
            public string Program { get; set; }
            public int TypeProgram { get; set; }
            public int? Total { get; set; }
            public decimal AmountPerHour { get; set; }
            public decimal TotalFee { get; set; }
            public bool To_Invoice { get; set; }
            public string Available { get; set; }
            public string Invoiced { get; set; }
            public string hoursToInvoice { get; set; }
            public string amountToInvoice { get; set; }
            public string pendingFee { get; set; }
            public string comments { get; set; }
            public int? invoiceId { get; set; }
            public List<CommentInvoice> commentInvoices { get; set; }
            public biz.premier.Entities.Invoice _Invoice { get; set; }
            public List<biz.premier.Entities.Invoice> Invoices { get; set; }
            public List<AdditionalExpense> additionalExpenses { get; set; }
        }
        public ActionResult GetrequestInvoice(int[] so, int? invoice, int? supplierPartner)
        {
            List<InvoiceWO> relocation = new List<InvoiceWO>();
            List<InvoiceWO> immigration = new List<InvoiceWO>();

            List<InvoiceWO> relocationInvoice = new List<InvoiceWO>();
            List<InvoiceWO> immigrationInvoice = new List<InvoiceWO>();
            if (!invoice.HasValue)
            {
                if (supplierPartner.HasValue)
                {
                    //////////////////////////////////////////////////////////////////////////////////
                    /// INVOICE ALREADY EXISTS
                    ///

                    // INVOICE
                    var invoiced = _context.ServiceInvoices
                        .Where(x => so.Contains(x.WorkOrder.Value) && x.InvoiceNavigation.InvoiceType == 2)
                        .Select(s => new
                        {
                            workOrder = s.WorkOrder.Value,
                            s.Invoice,
                            s.Service,
                            CommentInvoices = s.InvoiceNavigation.CommentInvoices.ToList(),
                            AdditionalExpenses = s.InvoiceNavigation.AdditionalExpenses.ToList(),
                            s.InvoiceNavigation,
                            s,
                            s.InvoiceNavigation.DocumentInvoices
                        })
                        .ToArray()
                        .Distinct();
                    var amountPerHour = _context.ProfileUsers
                        .Include(i => i.AreasCoverageNavigation).ThenInclude(i => i.SupplierPartnerProfileConsultantNavigation)
                        .FirstOrDefault(x => x.Id == supplierPartner.Value).AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.AmountPerHour.Value;
                    if (invoiced.Any())
                    {
                        // Relocation Standalone  
                        int numberRelocationStandalone;
                        var relocation_standalone_Invoice = _context.StandaloneServiceWorkOrders
                            .Include(i => i.Service)
                            .Include(i => i.Status)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.AdditionalExpenses)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Service = n.Service.Service + "/" + n.ServiceNumber,
                                Status = n.Status.Status,
                                Program = n.Coordination == true ? "Coordination" : "Standalone",
                                Total = n.AuthoTime,
                                AmountPerHour = amountPerHour,
                                Available = int.TryParse(GetSummaryHoursToInvoice(
                                        n.WorkOrder.ServiceRecord.Invoices
                                            .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                            .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                            invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray())
                                    , out numberRelocationStandalone
                                    ) ? Convert.ToString(n.AuthoTime - numberRelocationStandalone) : "N/A",
                                Invoiced = n.Coordination == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                        .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id)).ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                hoursToInvoice = n.Coordination == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x =>
                                        x.WorkOrder == n.WorkOrderId.Value
                                        && x.Service == n.Id
                                        && x.Status != 4
                                        && x.Status != 6)
                                    .HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                amountToInvoice = Convert.ToString(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service)).Select(s => s.AmountToInvoice).Sum()),
                                pendingFee = "",// Convert.ToString(
                                                // amountPerHour -
                                                // (n.WorkOrder.ServiceRecord.Invoices.Where(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                                //    .FirstOrDefault().ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                                //    invoiced.Select(s => s.Service).Contains(x.Service)).Select(s => s.AmountToInvoice).Sum())),
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();
                        // Relocation Bundled
                        int numberRelocationBundled;
                        var relocation_bundled_Invoice = _context.BundledServicesWorkOrders
                            .Include(i => i.WorkOrder)
                            .ThenInclude(i => i.Status)
                            .Include(i => i.BundledServices).ThenInclude(i => i.Service)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                                Status = n.WorkOrder.Status.Status,
                                Program = n.Package == true ? "Package" : "Bundled",
                                Total = Convert.ToInt32(n.TotalTime),
                                AmountPerHour = amountPerHour,
                                Available = int.TryParse(GetSummaryHoursToInvoice(
                                        n.WorkOrder.ServiceRecord.Invoices
                                            .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id)).ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray())
                                    , out numberRelocationStandalone
                                    ) ? Convert.ToString(Convert.ToInt32(n.TotalTime) - numberRelocationStandalone) : "N/A",
                                Invoiced = n.Package == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).FirstOrDefault().HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices
                                        .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id)).ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                hoursToInvoice = n.Package == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).FirstOrDefault().HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices.Where(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .FirstOrDefault().ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                amountToInvoice = Convert.ToString(n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id)).ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.AmountToInvoice).Sum()),
                                pendingFee = "",
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .FirstOrDefault().ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();
                        // Immigration Standalone
                        var immigration_standalone_Invoice = _context.StandaloneServiceWorkOrders
                            .Include(i => i.Service)
                            .Include(i => i.Status)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Service = n.Service.Service + "/" + n.ServiceNumber,
                                Status = n.Status.Status,
                                Program = n.Coordination == true ? "Coordination" : "Standalone",
                                Total = n.AuthoTime,
                                AmountPerHour = amountPerHour,
                                To_Invoice = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .ServiceInvoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value)
                                                         && invoiced.Select(s => s.Service).Contains(x.Service.Value)
                                                         && x.Status != 4 && x.Status != 6)
                                    .ToInvoice.Value,
                                amountToInvoice = Convert.ToString(
                                    _context.ServiceInvoices
                                    .FirstOrDefault(x =>
                                        invoiced.Select(s => s.Invoice).Contains(x.Id)
                                        && x.WorkOrder == n.WorkOrderId
                                        && x.Service == n.Id
                                        && x.Status != 4
                                        && x.Status != 6)
                                    .AmountToInvoice),
                                pendingFee = "",
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x =>
                                        x.WorkOrder == n.WorkOrderId.Value
                                        && x.Service == n.Id
                                        && x.Status != 4
                                        && x.Status != 6
                                        )
                                    .Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();
                        // Immigration Bundled
                        var immigration_bundled_Invoice = _context.BundledServicesWorkOrders
                            .Include(i => i.WorkOrder)
                            .ThenInclude(i => i.Status)
                            .Include(i => i.BundledServices).ThenInclude(i => i.Service)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                                Status = n.WorkOrder.Status.Status,
                                Program = n.Package == true ? "Coordination" : "Standalone",
                                Total = Convert.ToInt32(n.TotalTime),
                                AmountPerHour = amountPerHour,
                                To_Invoice = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .ServiceInvoices
                                    .FirstOrDefault(x =>
                                        invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value)
                                        && invoiced.Select(s => s.Service).Contains(x.Service.Value)
                                        && x.Status != 4
                                        && x.Status != 6)
                                    .ToInvoice.Value,
                                amountToInvoice = Convert.ToString(
                                    _context.ServiceInvoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id) && x.WorkOrder == n.WorkOrderId && x.Service == n.Id && x.Status != 4 && x.Status != 6)
                                    .AmountToInvoice),
                                pendingFee = "",// Convert.ToString(Convert.ToDecimal(n.ProjectedFee) - _context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices
                                    .FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6)
                                    .Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();

                        relocationInvoice.AddRange(relocation_standalone_Invoice.Union(relocation_bundled_Invoice));
                        immigrationInvoice.AddRange(immigration_standalone_Invoice.Union(immigration_bundled_Invoice));
                        /////////////////////////
                        /// END
                        ////////////////////////
                    }

                    // Relocation Standalone  
                    var relocation_standalone = _context.StandaloneServiceWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2
                            && (x.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Select(s => s.SupplierId).Contains(supplierPartner) || x.WorkOrder.ServiceRecord.RelocationSupplierPartners.Select(s => s.SupplierId).Contains(supplierPartner)))
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 1,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Service = n.Service.Service + "/" + n.ServiceNumber,
                            Status = n.Status.Status,
                            Program = n.Coordination == true ? "Coordination" : "Standalone",
                            Total = n.AuthoTime,
                            AmountPerHour = amountPerHour,
                            Available = n.AuthoTime.ToString(),
                            Invoiced = "0",
                            hoursToInvoice = "0",
                            amountToInvoice = "0",
                            pendingFee = "",
                            comments = ""
                        }).ToList();
                    // Relocation Bundled
                    var relocation_bundled = _context.BundledServicesWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 2,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                            Status = n.WorkOrder.Status.Status,
                            Program = n.Package == true ? "Package" : "Bundled",
                            Total = Int32.Parse(n.TotalTime),
                            AmountPerHour = amountPerHour,
                            Available = n.TotalTime,
                            Invoiced = "0",
                            hoursToInvoice = "0",
                            amountToInvoice = "0",
                            pendingFee = "",
                            comments = ""
                        }).ToList();
                    // Immigration Standalone
                    var immigration_standalone = _context.StandaloneServiceWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 1,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Service = n.Service.Service + "/" + n.ServiceNumber,
                            Status = n.Status.Status,
                            Program = n.Coordination == true ? "Coordination" : "Standalone",
                            Total = n.AuthoTime,
                            AmountPerHour = amountPerHour,
                            To_Invoice = false,
                            amountToInvoice = "0",
                            pendingFee = "",
                            comments = ""
                        }).ToList();
                    // Immigration Bundled
                    var immigration_bundled = _context.BundledServicesWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 2,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                            Status = n.WorkOrder.Status.Status,
                            Program = n.Package == true ? "Coordination" : "Standalone",
                            Total = Int32.Parse(n.TotalTime),
                            AmountPerHour = amountPerHour,
                            To_Invoice = false,
                            amountToInvoice = "0",
                            pendingFee = "",
                            comments = ""
                        }).ToList();
                    ///////////////////////////////////////////////////////////////////////
                    /// UNION LISTS
                    relocation.AddRange(relocation_standalone.Union(relocation_bundled));
                    immigration.AddRange(immigration_standalone.Union(immigration_bundled));

                    if (invoiced.Any())
                    {
                        foreach (var i in relocationInvoice)
                        {
                            var l = relocation.SingleOrDefault(x => x.Id == i.Id);
                            if (l != null)
                                relocation.Remove(l);
                        }
                        relocation.AddRange(relocationInvoice);
                        foreach (var i in immigrationInvoice)
                        {
                            var l = immigration.SingleOrDefault(x => x.Id == i.Id);
                            if (l != null)
                                immigration.Remove(l);
                        }
                        immigration.AddRange(immigrationInvoice);

                    }
                }
                else if (!supplierPartner.HasValue)
                {
                    /* INVOICE ALREADY EXISTS ? */

                    // INVOICE
                    var invoiced = _context.ServiceInvoices
                        .Where(x => so.Contains(x.WorkOrder.Value) && x.InvoiceNavigation.InvoiceType == 1)
                        .Select(s => new
                        {
                            workOrder = s.WorkOrder.Value,
                            s.Invoice,
                            s.Service,
                            CommentInvoices = s.InvoiceNavigation.CommentInvoices.ToList(),
                            AdditionalExpenses = s.InvoiceNavigation.AdditionalExpenses.ToList(),
                            s.InvoiceNavigation,
                            s,
                            s.InvoiceNavigation.DocumentInvoices
                        })
                        .ToArray()
                        .Distinct();
                    // var amountPerHour = _context.ProfileUsers
                    //     .Include(i => i.AreasCoverageNavigation).ThenInclude(i => i.SupplierPartnerProfileConsultantNavigation)
                    //     .FirstOrDefault(x => x.Id == supplierPartner.Value).AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.AmountPerHour.Value;
                    if (invoiced.Any())
                    {
                        // Relocation Standalone  
                        int numberRelocationStandalone;
                        var relocation_standalone_Invoice = _context.StandaloneServiceWorkOrders
                            .Include(i => i.Service)
                            .Include(i => i.Status)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.AdditionalExpenses)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Service = n.Service.Service + "/" + n.ServiceNumber,
                                Status = n.Status.Status,
                                Program = n.Coordination == true ? "Coordination" : "Standalone",
                                Total = n.AuthoTime,
                                pendingFee = n.ProjectedFee,
                                Available = int.TryParse(GetSummaryHoursToInvoice(
                                        n.WorkOrder.ServiceRecord.Invoices
                                            .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                            .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                            invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray())
                                    , out numberRelocationStandalone
                                    ) ? Convert.ToString(n.AuthoTime - numberRelocationStandalone) : "N/A",
                                Invoiced = n.Coordination == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                        .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id)).ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                hoursToInvoice = n.Coordination == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x =>
                                        x.WorkOrder == n.WorkOrderId.Value
                                        && x.Service == n.Id
                                        && x.Status != 4
                                        && x.Status != 6)
                                    .HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                amountToInvoice = Convert.ToString(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service)).Select(s => s.AmountToInvoice).Sum()),
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();
                        // Relocation Bundled
                        int numberRelocationBundled;
                        var relocation_bundled_Invoice = _context.BundledServicesWorkOrders
                            .Include(i => i.WorkOrder)
                            .ThenInclude(i => i.Status)
                            .Include(i => i.BundledServices).ThenInclude(i => i.Service)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                                Status = n.WorkOrder.Status.Status,
                                Program = n.Package == true ? "Package" : "Bundled",
                                Total = Convert.ToInt32(n.TotalTime),
                                Available = int.TryParse(GetSummaryHoursToInvoice(
                                        n.WorkOrder.ServiceRecord.Invoices
                                            .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id)).ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray())
                                    , out numberRelocationStandalone
                                    ) ? Convert.ToString(Convert.ToInt32(n.TotalTime) - numberRelocationStandalone) : "N/A",
                                Invoiced = n.Package == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices
                                        .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id)).ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                hoursToInvoice = n.Package == true
                                    ? n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).HourInvoice
                                    : GetSummaryHoursToInvoice(n.WorkOrder.ServiceRecord.Invoices.FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                        .ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.HourInvoice).ToArray()),
                                amountToInvoice = Convert.ToString(n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id)).ServiceInvoices.Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value) &&
                                        invoiced.Select(s => s.Service).Contains(x.Service) && x.Status != 4 && x.Status != 6).Select(s => s.AmountToInvoice).Sum()),
                                pendingFee = n.ProjectedFee,
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6).Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();
                        // Immigration Standalone
                        var immigration_standalone_Invoice = _context.StandaloneServiceWorkOrders
                            .Include(i => i.Service)
                            .Include(i => i.Status)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Service = n.Service.Service + "/" + n.ServiceNumber,
                                Status = n.Status.Status,
                                Program = n.Coordination == true ? "Coordination" : "Standalone",
                                Total = n.AuthoTime,
                                To_Invoice = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .ServiceInvoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value)
                                                         && invoiced.Select(s => s.Service).Contains(x.Service.Value)
                                                         && x.Status != 4 && x.Status != 6)
                                    .ToInvoice.Value,
                                amountToInvoice = Convert.ToString(
                                    _context.ServiceInvoices
                                    .FirstOrDefault(x =>
                                        invoiced.Select(s => s.Invoice).Contains(x.Id)
                                        && x.WorkOrder == n.WorkOrderId
                                        && x.Service == n.Id
                                        && x.Status != 4
                                        && x.Status != 6)
                                    .AmountToInvoice),
                                pendingFee = n.ProjectedFee,
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices.FirstOrDefault(x =>
                                        x.WorkOrder == n.WorkOrderId.Value
                                        && x.Service == n.Id
                                        && x.Status != 4
                                        && x.Status != 6
                                        )
                                    .Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();
                        // Immigration Bundled
                        var immigration_bundled_Invoice = _context.BundledServicesWorkOrders
                            .Include(i => i.WorkOrder)
                            .ThenInclude(i => i.Status)
                            .Include(i => i.BundledServices).ThenInclude(i => i.Service)
                            .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                            .Where(x => invoiced.Select(s => s.workOrder).Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1 && x.WorkOrder.ServiceRecord.Invoices.Any())
                            .Select(n => new InvoiceWO
                            {
                                WorkOrderId = n.WorkOrderId,
                                Id = n.Id,
                                NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                                Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                                Status = n.WorkOrder.Status.Status,
                                Program = n.Package == true ? "Coordination" : "Standalone",
                                Total = Convert.ToInt32(n.TotalTime),
                                To_Invoice = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id))
                                    .ServiceInvoices
                                    .FirstOrDefault(x =>
                                        invoiced.Select(s => s.workOrder).Contains(x.WorkOrder.Value)
                                        && invoiced.Select(s => s.Service).Contains(x.Service.Value)
                                        && x.Status != 4
                                        && x.Status != 6)
                                    .ToInvoice.Value,
                                amountToInvoice = Convert.ToString(
                                    _context.ServiceInvoices
                                    .FirstOrDefault(x => invoiced.Select(s => s.Invoice).Contains(x.Id) && x.WorkOrder == n.WorkOrderId && x.Service == n.Id && x.Status != 4 && x.Status != 6)
                                    .AmountToInvoice),
                                pendingFee = n.ProjectedFee,
                                invoiceId = n.WorkOrder.ServiceRecord.Invoices
                                    .FirstOrDefault(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ServiceInvoices
                                    .FirstOrDefault(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id && x.Status != 4 && x.Status != 6)
                                    .Invoice,
                                Invoices = n.WorkOrder.ServiceRecord.Invoices
                                    .Where(x => invoiced.Select(w => w.Invoice.Value).Contains(x.Id))
                                    .ToList(),
                            }).ToList();

                        relocationInvoice.AddRange(relocation_standalone_Invoice.Union(relocation_bundled_Invoice));
                        immigrationInvoice.AddRange(immigration_standalone_Invoice.Union(immigration_bundled_Invoice));
                        /* END */
                    }

                    // Relocation Standalone  
                    var relocation_standalone = _context.StandaloneServiceWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 1,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Service = n.Service.Service + "/" + n.ServiceNumber,
                            Status = n.Status.Status,
                            Program = n.Coordination == true ? "Coordination" : "Standalone",
                            Total = n.AuthoTime,
                            pendingFee = n.ProjectedFee,
                            Available = n.AuthoTime.ToString(),
                            Invoiced = "0",
                            hoursToInvoice = "0",
                            amountToInvoice = "0",
                            comments = ""
                        }).ToList();
                    // Relocation Bundled
                    var relocation_bundled = _context.BundledServicesWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 2,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                            Status = n.WorkOrder.Status.Status,
                            Program = n.Package == true ? "Package" : "Bundled",
                            Total = Int32.Parse(n.TotalTime),
                            pendingFee = n.ProjectedFee,
                            Available = n.TotalTime,
                            Invoiced = "0",
                            hoursToInvoice = "0",
                            amountToInvoice = "0",
                            comments = ""
                        }).ToList();
                    // Immigration Standalone
                    var immigration_standalone = _context.StandaloneServiceWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 1,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Service = n.Service.Service + "/" + n.ServiceNumber,
                            Status = n.Status.Status,
                            Program = n.Coordination == true ? "Coordination" : "Standalone",
                            Total = n.AuthoTime,
                            To_Invoice = false,
                            amountToInvoice = "0",
                            pendingFee = n.ProjectedFee,
                            comments = ""
                        }).ToList();
                    // Immigration Bundled
                    var immigration_bundled = _context.BundledServicesWorkOrders
                        .Where(x => so.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1)
                        .Select(n => new InvoiceWO
                        {
                            TypeProgram = 2,
                            WorkOrderId = n.WorkOrderId,
                            Id = n.Id,
                            NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                            Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                            Status = n.WorkOrder.Status.Status,
                            Program = n.Package == true ? "Coordination" : "Standalone",
                            Total = Int32.Parse(n.TotalTime),
                            To_Invoice = false,
                            amountToInvoice = "0",
                            pendingFee = n.ProjectedFee,
                            comments = ""
                        }).ToList();
                    ///////////////////////////////////////////////////////////////////////
                    /// UNION LISTS
                    relocation.AddRange(relocation_standalone.Union(relocation_bundled));
                    immigration.AddRange(immigration_standalone.Union(immigration_bundled));

                    if (invoiced.Any())
                    {
                        foreach (var i in relocationInvoice)
                        {
                            var l = relocation.SingleOrDefault(x => x.Id == i.Id);
                            if (l != null)
                                relocation.Remove(l);
                        }
                        relocation.AddRange(relocationInvoice);
                        foreach (var i in immigrationInvoice)
                        {
                            var l = immigration.SingleOrDefault(x => x.Id == i.Id);
                            if (l != null)
                                immigration.Remove(l);
                        }
                        immigration.AddRange(immigrationInvoice);
                    }
                }
            }
            else if (invoice.HasValue)
            {
                // INVOICE
                int[] invoiced = _context.ServiceInvoices.Where(x => x.Invoice == invoice.Value).Select(s => s.WorkOrder.Value).ToArray();

                var invoicedsAll = _context.ServiceInvoices.Where(x => x.Invoice == invoice.Value).ToList();
                // Relocation Standalone  
                int numberRelocationStandalone;
                var relocation_standalone = _context.StandaloneServiceWorkOrders
                    .Include(i => i.Service)
                    .Include(i => i.Status)
                    .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                    .Where(x => invoiced.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2)
                    .Select(n => new InvoiceWO
                    {
                        WorkOrderId = n.WorkOrderId,
                        Id = n.Id,
                        NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                        Service = n.Service.Service + "/" + n.ServiceNumber,
                        Status = n.Status.Status,
                        Program = n.Coordination == true ? "Coordination" : "Standalone",
                        Total = n.AuthoTime,
                        //ProjectedFee = n.ProjectedFee,
                        Available = int.TryParse(
                            n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().HourInvoice
                            , out numberRelocationStandalone
                            ) ? Convert.ToString(n.AuthoTime - numberRelocationStandalone) : "N/A",
                        Invoiced = n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().HourInvoice,
                        hoursToInvoice = n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().HourInvoice,
                        amountToInvoice = Convert.ToString(_context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        pendingFee = Convert.ToString(Convert.ToDecimal(n.ProjectedFee) - _context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        //invoice = n.WorkOrder.ServiceRecord.Invoices
                        //    .Where(x => x.Id == invoice.Value)
                        //    .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().Id
                    }).ToList();
                // Relocation Bundled
                int numberRelocationBundled;
                var relocation_bundled = _context.BundledServicesWorkOrders
                    .Include(i => i.WorkOrder)
                    .ThenInclude(i => i.Status)
                    .Include(i => i.BundledServices).ThenInclude(i => i.Service)
                    .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                    .Where(x => invoiced.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 2)
                    .Select(n => new InvoiceWO
                    {
                        WorkOrderId = n.WorkOrderId,
                        Id = n.Id,
                        NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                        Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                        Status = n.WorkOrder.Status.Status,
                        Program = n.Package == true ? "Package" : "Bundled",
                        Total = Convert.ToInt32(n.TotalTime),
                        //ProjectedFee = n.ProjectedFee,
                        Available = Int32.TryParse(
                            n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value && x.ServiceInvoices.Select(a => a.WorkOrder).Contains(n.WorkOrderId.Value) && x.ServiceInvoices.Select(a => a.Service).Contains(n.Id))
                            .FirstOrDefault().ServiceInvoices.FirstOrDefault().HourInvoice
                            , out numberRelocationBundled
                            ) ? (Int32.Parse(n.TotalTime) - numberRelocationBundled).ToString() : "N/A",
                        Invoiced = n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().HourInvoice,
                        hoursToInvoice = n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().HourInvoice,
                        amountToInvoice = Convert.ToString(_context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        pendingFee = Convert.ToString(Convert.ToDecimal(n.ProjectedFee) - n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        //invoice = n.WorkOrder.ServiceRecord.Invoices
                        //    .Where(x => x.Id == invoice.Value)
                        //    .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().Id
                    }).ToList();
                // Immigration Standalone
                var immigration_standalone = _context.StandaloneServiceWorkOrders
                    .Include(i => i.Service)
                    .Include(i => i.Status)
                    .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                    .Where(x => invoiced.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1)
                    .Select(n => new InvoiceWO
                    {
                        WorkOrderId = n.WorkOrderId,
                        Id = n.Id,
                        NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                        Service = n.Service.Service + "/" + n.ServiceNumber,
                        Status = n.Status.Status,
                        Program = n.Coordination == true ? "Coordination" : "Standalone",
                        Total = n.AuthoTime,
                        //ProjectedFee = n.ProjectedFee,
                        To_Invoice = n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().ToInvoice.Value,
                        amountToInvoice = Convert.ToString(_context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        pendingFee = Convert.ToString(decimal.Parse(n.ProjectedFee.ToString()) - _context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        //invoice = n.WorkOrder.ServiceRecord.Invoices
                        //    .Where(x => x.Id == invoice.Value)
                        //    .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().Id
                    }).ToList();
                // Immigration Bundled
                var immigration_bundled = _context.BundledServicesWorkOrders
                    .Include(i => i.WorkOrder)
                    .ThenInclude(i => i.Status)
                    .Include(i => i.BundledServices).ThenInclude(i => i.Service)
                    .Include(i => i.WorkOrder).ThenInclude(i => i.ServiceRecord).ThenInclude(i => i.Invoices).ThenInclude(i => i.ServiceInvoices)
                    .Where(x => invoiced.Contains(x.WorkOrderId.Value) && x.WorkOrder.ServiceLineId == 1)
                    .Select(n => new InvoiceWO
                    {
                        WorkOrderId = n.WorkOrderId,
                        Id = n.Id,
                        NumberWorkOrder = n.WorkOrder.NumberWorkOrder,
                        Services = n.BundledServices.Select(s => s.Service.Service + " " + s.ServiceNumber).ToList(),
                        Status = n.WorkOrder.Status.Status,
                        Program = n.Package == true ? "Coordination" : "Standalone",
                        Total = Convert.ToInt32(n.TotalTime),
                        // ProjectedFee = n.ProjectedFee,
                        To_Invoice = n.WorkOrder.ServiceRecord.Invoices
                            .Where(x => x.Id == invoice.Value)
                            .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().ToInvoice.Value,
                        amountToInvoice = Convert.ToString(_context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        pendingFee = Convert.ToString(Convert.ToDecimal(n.ProjectedFee) - _context.ServiceInvoices.Where(x => x.Invoice == invoice.Value && x.WorkOrder == n.WorkOrderId && x.Service == n.Id).FirstOrDefault().AmountToInvoice),
                        //invoice = n.WorkOrder.ServiceRecord.Invoices
                        //    .Where(x => x.Id == invoice.Value)
                        //    .FirstOrDefault().ServiceInvoices.Where(x => x.WorkOrder == n.WorkOrderId.Value && x.Service == n.Id).FirstOrDefault().Id
                    }).ToList();
                ///////////////////////////////////////////////////////////////////////
                /// UNION LISTS
                ///////////////////////////////////////////////////////////////////
                relocation.AddRange(relocation_standalone.Union(relocation_bundled));
                immigration.AddRange(immigration_standalone.Union(immigration_bundled));
            }
            var invoiceData = invoice.HasValue ? _context.Invoices.Where(x => x.Id == invoice.Value).Select(s => new
            {
                s.Id,
                client = s.ServiceRecordNavigation.Client.Name,
                s.ServiceRecord,
                s.InvoiceType,
                supplierPartner = _context.ProfileUsers.SingleOrDefault(d => d.Id == s.Consultant),
                statusId = s.ServiceInvoices.FirstOrDefault().Status,
                //s.StatusNavigation.Status,
                workOrderInvoices = s.ServiceInvoices,
                documents = s.DocumentInvoices.Select(n => new
                {
                    n.Id,
                    n.DocumentTypeNavigation.DocumentType,
                    n.UpdatedDate,
                    n.IssueDate,
                    n.ExpirationDate,
                    n.IssuingAuthority,
                    n.CountryOriginNavigation.Name
                }).ToList(),
                comments = s.CommentInvoices.Select(n => new
                {
                    n.Id,
                    n.Comment,
                    name = $"{n.CreatedByNavigation.Name} {n.CreatedByNavigation.LastName} {n.CreatedByNavigation.MotherLastName}",
                    title = n.CreatedByNavigation.Role.Role,
                    created_date = n.CreatedDate,
                    title2 = n.CreatedByNavigation.UserType.Type,
                    avatar = n.CreatedByNavigation.Avatar
                }).ToList()
            }).FirstOrDefault() : null;
            var client = _context.WorkOrders.Where(x => x.Id == so.FirstOrDefault()).Select(s => s.ServiceRecord.Client.Name).FirstOrDefault();
            return new ObjectResult(new { relocation, immigration, invoiceData, client });
        }
        public static string GetSummaryHoursToInvoice(string[] hoursInvoice)
        {
            string result = "";
            int outResult = 0;
            foreach (var i in hoursInvoice)
            {
                int getResult = outResult == 0 ? 0 : outResult;
                int.TryParse(i, out outResult);
                outResult = outResult != 0 ? getResult + outResult : getResult;
            }
            result = outResult != 0 ? outResult.ToString() : "";
            return result;
        }
        public biz.premier.Entities.Invoice AddInvoice(biz.premier.Entities.Invoice Dto)
        {
            _context.Invoices.Add(Dto);
            _context.SaveChanges();
            return Dto;
        }

        public ActionResult GetDocumentation(int category, int applicatId, int service_order_id, int type_service)
        {
            var standalone = _context.Set<biz.premier.Entities.StandaloneServiceWorkOrder>()
                .Where(x => x.CategoryId == category && x.DeliveredTo == applicatId && x.WorkOrderId == service_order_id && x.DeliveringIn == type_service)
                .Select(n => new
                {
                    n.WorkOrder.CreationDate,
                    n.Acceptance,
                    n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCity.City,
                    // DocumentDocumentManagements = _context.DocumentDocumentManagements.Where(x => x.DocumentManagementId == n.WorkOrderService.DocumentManagements.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderDocumentManagements = _context.ReminderDocumentManagements.Where(x => x.DocumentManagementId == n.WorkOrderService.DocumentManagements.Select(s => s.Id).FirstOrDefault()).ToList(),
                    // DocumentLocalDocumentations = _context.DocumentLocalDocumentations.Where(x => x.LocalDocumentationId == n.WorkOrderService.LocalDocumentations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderLocalDocumentations = _context.ReminderLocalDocumentations.Where(x => x.LocalDocumentationId == n.WorkOrderService.LocalDocumentations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentDocumentManagements = _context.CommentDocumentManagements.Where(x => x.DocumentManagementId == n.WorkOrderService.DocumentManagements.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    CommentLocalDocumentations = _context.CommentLocalDocumentations.Where(x => x.LocalDocumentationId == n.WorkOrderService.LocalDocumentations.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    DocumentManagement = n.WorkOrderService.DocumentManagements.Count() > 0 ? n.WorkOrderService.DocumentManagements
                    .Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.StandaloneServiceWorkOrders.Where(_x => _x.WorkOrderServiceId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        l.ProjectFee,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentDocumentManagements = _context.DocumentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        // ReminderDocumentManagements = _context.ReminderDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        // CommentDocumentManagements = _context.CommentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).Include(i => i.User).ToList(),
                    })
                    : n.WorkOrderService.DocumentManagements.Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.StandaloneServiceWorkOrders.Where(_x => _x.WorkOrderServiceId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        l.ProjectFee,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentDocumentManagements = _context.DocumentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        // ReminderDocumentManagements = _context.ReminderDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        // CommentDocumentManagements = _context.CommentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).Include(i => i.User).ToList(),
                    }),
                    LocalDocumentation = n.WorkOrderService.LocalDocumentations.Count() > 0 ? n.WorkOrderService.LocalDocumentations
                    .Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.StandaloneServiceWorkOrders.Where(_x => _x.WorkOrderServiceId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentLocalDocumentations = _context.DocumentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        //ReminderLocalDocumentations = _context.ReminderLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        // CommentLocalDocumentations = _context.CommentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).Include(i => i.User).ToList(),
                    })
                    : n.WorkOrderService.LocalDocumentations.Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.StandaloneServiceWorkOrders.Where(_x => _x.WorkOrderServiceId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentLocalDocumentations = _context.DocumentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        //ReminderLocalDocumentations = _context.ReminderLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        // CommentLocalDocumentations = _context.CommentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).Include(i => i.User).ToList(),
                    }),
                }).ToList();
            var query = _context.Set<biz.premier.Entities.BundledService>()
                .Where(x => x.CategoryId == category && x.DeliveredTo == applicatId && x.BundledServiceOrder.WorkOrderId == service_order_id && x.DeliveringIn == type_service)
                .Select(n => new
                {
                    n.BundledServiceOrder.WorkOrder.CreationDate,
                    n.Acceptance,
                    n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCity.City,
                    // DocumentDocumentManagements = _context.DocumentDocumentManagements.Where(x => x.DocumentManagementId == n.WorkServices.DocumentManagements.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderDocumentManagements = _context.ReminderDocumentManagements.Where(x => x.DocumentManagementId == n.WorkServices.DocumentManagements.Select(s => s.Id).FirstOrDefault()).ToList(),
                    // DocumentLocalDocumentations = _context.DocumentLocalDocumentations.Where(x => x.LocalDocumentationId == n.WorkServices.LocalDocumentations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderLocalDocumentations = _context.ReminderLocalDocumentations.Where(x => x.LocalDocumentationId == n.WorkServices.LocalDocumentations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentDocumentManagements = _context.CommentDocumentManagements.Where(x => x.DocumentManagementId == n.WorkServices.DocumentManagements.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ToList(),
                    CommentLocalDocumentations = _context.CommentLocalDocumentations.Where(x => x.LocalDocumentationId == n.WorkServices.LocalDocumentations.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ToList(),
                    DocumentManagement = n.WorkServices.DocumentManagements.Count() > 0 ? n.WorkServices.DocumentManagements
                    .Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.BundledServices.Where(_x => _x.WorkServicesId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        l.ProjectFee,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentDocumentManagements = _context.DocumentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        //ReminderDocumentManagements = _context.ReminderDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        // CommentDocumentManagements = _context.CommentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).Include(i => i.User).ToList(),
                    })
                    : n.WorkServices.DocumentManagements.Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.BundledServices.Where(_x => _x.WorkServicesId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        l.ProjectFee,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentDocumentManagements = _context.DocumentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        //ReminderDocumentManagements = _context.ReminderDocumentManagements.Where(x => x.DocumentManagementId == l.Id).ToList(),
                        // CommentDocumentManagements = _context.CommentDocumentManagements.Where(x => x.DocumentManagementId == l.Id).Include(i => i.User).ToList(),
                    }),
                    LocalDocumentation = n.WorkServices.LocalDocumentations.Count() > 0 ? n.WorkServices.LocalDocumentations
                    .Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.BundledServices.Where(_x => _x.WorkServicesId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentLocalDocumentations = _context.DocumentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        //ReminderLocalDocumentations = _context.ReminderLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        // CommentLocalDocumentations = _context.CommentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).Include(i => i.User).ToList(),
                    })
                    : n.WorkServices.LocalDocumentations.Select(l => new
                    {
                        l.Id,
                        l.WorkOrderServicesId,
                        l.AuthoDate,
                        l.AuthoAcceptanceDate,
                        l.StatusId,
                        l.HostCountryId,
                        l.HostCityId,
                        l.ApplicantId,
                        ApplicantName = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Name,
                        Relationship = _context.DependentInformations.Where(_x => _x.Id == l.ApplicantId).FirstOrDefault().Relationship.Relationship,
                        l.WorkOrderServices.BundledServices.Where(_x => _x.WorkServicesId == l.WorkOrderServicesId).FirstOrDefault().ServiceNumber,
                        l.Service.Service,
                        applicant = _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name == null ? "N/A"
                        : _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Relationship.Relationship
                        + "/" + _context.DependentInformations.FirstOrDefault(x => x.Id == l.ApplicantId).Name,
                        l.Name,
                        l.ServiceCompletionDate,
                        Country = l.HostCountry.Name,
                        City = l.HostCity.City,
                        l.ServiceId,
                        l.DocumentTypeId,
                        l.SupplierId,
                        l.AppointmentDate,
                        l.DocumentCollectionStartDate,
                        l.DocumentCollectionCompletionDate,
                        l.ApplicationSubmissionDate,
                        l.ApplicationApprovalDate,
                        l.DocumentDeliveryDate,
                        l.DocumentExpirationDate,
                        l.Comment,
                        DocumentLocalDocumentations = _context.DocumentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        ProjectedFee = l.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                            ? l.WorkOrderServices.StandaloneServiceWorkOrders.SingleOrDefault(s => s.WorkOrderServiceId == l.WorkOrderServicesId).ProjectedFee
                            : l.WorkOrderServices.BundledServices.SingleOrDefault(s => s.WorkServicesId == l.WorkOrderServicesId).BundledServiceOrder.ProjectedFee
                        //ReminderLocalDocumentations = _context.ReminderLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).ToList(),
                        // CommentLocalDocumentations = _context.CommentLocalDocumentations.Where(x => x.LocalDocumentationId == l.Id).Include(i => i.User).ToList(),
                    }),

                }).ToList();

            var union = new
            {
                standalone = standalone,
                bundle = query
            };

            return new ObjectResult(union);
            //return null;
        }

        public int lastIdBundleService()
        {
            int num = _context.BundledServices.Select(s => s.Id).DefaultIfEmpty().OrderByDescending(x => x).FirstOrDefault() + 1;

            return num;
        }

        public Boolean DeleteStandStandaloneServiceWorkOrder(int key)
        {
            var cosnult = _context.StandaloneServiceWorkOrders.Find(key);
            if (cosnult == null)
                return false;
            _context.StandaloneServiceWorkOrders.Remove(cosnult);
            _context.SaveChanges();
            return true;
        }

        public Boolean DeleteBundledService(int key)
        {
            var cosnult = _context.BundledServices.Find(key);
            if (cosnult == null)
                return false;
            _context.BundledServices.Remove(cosnult);
            _context.SaveChanges();
            return true;
        }

        public Tuple<string, string> GetName(int key)
        {
            var query = _context.DependentInformations
                .Include(i => i.Relationship)
                .SingleOrDefault(x => x.Id == key);
            Tuple<string, string> t = new Tuple<string, string>(query.Name, query.Relationship.Relationship);
            return t;
        }

        public ActionResult GetPackage(int wo)
        {
            var query = _context.BundledServicesWorkOrders
                .Include(i => i.BundledServices)
                .Where(x => x.WorkOrderId == wo).Select(s => new
                {
                    s.Id,
                    s.WorkOrder.ServiceLine.ServiceLine,
                    s.WorkOrder.NumberWorkOrder,
                    s.ProjectedFee,
                    s.TotalTime,
                    remainignTime = Math.Abs(_context.ReportDays.Where(_x => _x.WorkOrder == s.Id).Sum(_s => Convert.ToInt32(_s.TotalTime)) - int.Parse(s.TotalTime)),
                    s.Package,
                    services = s.BundledServices.Select(_s => new
                    {
                        WorkOrderId = s.Id,
                        DeliveredToId = _s.DeliveredTo,
                        DeliveredInId = _s.DeliveringIn,
                        _s.CategoryId,
                        Status = _s.CategoryId == 1 ? _s.WorkServices.EntryVisas
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 2 ? _s.WorkServices.WorkPermits
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 3 ? _s.WorkServices.VisaDeregistrations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 4 ? _s.WorkServices.ResidencyPermits
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 5 ? "N/A"
                            : _s.CategoryId == 6 ? "N/A"
                            : _s.CategoryId == 7 ? _s.WorkServices.CorporateAssistances
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 8 ? _s.WorkServices.Renewals
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 9 ? _s.WorkServices.Notifications
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 10 ? _s.WorkServices.LegalReviews
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 12 ? _s.WorkServices.PredecisionOrientations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 13 ? _s.WorkServices.AreaOrientations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 14 ? _s.WorkServices.SettlingIns
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 15 ? _s.WorkServices.SchoolingSearches
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 16 ? _s.WorkServices.Departures
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 17 ? _s.WorkServices.TemporaryHousingCoordinatons
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 18 ? _s.WorkServices.RentalFurnitureCoordinations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 19 ? _s.WorkServices.Transportations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 20 ? _s.WorkServices.AreaOrientations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 21 ? _s.WorkServices.HomeFindings
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 22 ? _s.WorkServices.LeaseRenewals
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 23 ? _s.WorkServices.HomeSales
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 24 ? _s.WorkServices.HomePurchases
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 25 ? _s.WorkServices.PropertyManagements
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 26 ? _s.WorkServices.Others
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 27 ? _s.WorkServices.TenancyManagements
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 28 ? _s.WorkServices.Others
                                .FirstOrDefault().Status.Status
                            : "N/A",
                        serviceName = _s.Service.Service,
                        _s.ServiceNumber,
                        _s.Autho,
                        _s.DeliveredToNavigation.Name,
                        _s.WorkServicesId,
                        service = _s.CategoryId == 1 ? _s.WorkServices.EntryVisas
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 2 ? _s.WorkServices.WorkPermits
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 3 ? _s.WorkServices.VisaDeregistrations
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 4 ? _s.WorkServices.ResidencyPermits
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 5 ? _context.CatCategories
                                .Where(x => x.Id == 5).Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 6 ? _context.CatCategories
                            .Where(x => x.Id == 6)
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 7 ? _s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 8 ? _s.WorkServices.Renewals
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 9 ? _s.WorkServices.Notifications
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 10 ? _s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 12 ? _s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 13 ? _s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 14 ? _s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 15 ? _s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 16 ? _s.WorkServices.Departures
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 17 ? _s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 18 ? _s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 19 ? _s.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 20 ? _s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 21 ? _s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 22 ? _s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 23 ? _s.WorkServices.HomeSales
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 24 ? _s.WorkServices.HomePurchases
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 25 ? _s.WorkServices.PropertyManagements
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 26 ? _s.WorkServices.Others
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 27 ? _s.WorkServices.TenancyManagements
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 28 ? _s.WorkServices.Others
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                Id = 0
                            }).ToList(),
                        id_server = _s.CategoryId == 5 ? _context.DocumentManagements
                                .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(5)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                                         && f.ApplicantId == _s.DeliveredTo).Id
                            : _s.CategoryId == 6 ? _context.LocalDocumentations
                                .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(6)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                                         && f.ApplicantId == _s.DeliveredTo).Id
                            //: _s.CategoryId == 19 ? _context.Transportations
                            //    .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(19)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                            //            && f.ApplicantId == _s.DeliveredTo).Id
                            //: _s.CategoryId == 20 ? _context.AirportTransportationServices
                            //    .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(20)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                            //            && f.ApplicantId == _s.DeliveredTo).Id
                            : _s.Id,
                        timeSpent = Math.Abs(_context.ServiceReportDays.Where(x => x.ReportDay.WorkOrder.Value == s.Id && x.Service.Value == _s.Id).Sum(sigma => Convert.ToInt32(sigma.Time))),
                        remainingTime = Math.Abs(_context.ServiceReportDays.Where(x => x.ReportDay.WorkOrder.Value == s.Id && x.Service.Value == _s.Id).Sum(sigma => Convert.ToInt32(sigma.Time)) - int.Parse(s.TotalTime)),
                        requestedExtraTime = Math.Abs(_context.RequestAdditionalTimes.Where(x => x.WorkOrder.Value == s.WorkOrderId && x.Service.Value == _s.Id).Sum(sigma => sigma.RequestTime.Value))
                    })
                }).ToList();
            return new ObjectResult(query);
        }

        public ActionResult GetBundledServiceByBundleId(int bundle_swo_id)
        {
            var query = _context.BundledServicesWorkOrders
                .Include(i => i.BundledServices)
                .Where(x => x.Id == bundle_swo_id).Select(s => new
                {
                    s.Id,
                    s.WorkOrder.ServiceLine.ServiceLine,
                    s.WorkOrder.NumberWorkOrder,
                    s.ProjectedFee,
                    s.TotalTime,
                    totalTimeReport = _context.ReportDays.Where(_x => _x.WorkOrder == s.WorkOrderId).Sum(_s => Convert.ToInt32(_s.TotalTime)),
                    remainignTime = Math.Abs(int.Parse(s.TotalTime) - _context.ReportDays.Where(_x => _x.WorkOrder == s.WorkOrderId).Sum(_s => Convert.ToInt32(_s.TotalTime))),
                    s.Package,
                    services = s.BundledServices.Select(_s => new
                    {
                        WorkOrderId = s.Id,
                        DeliveredToId = _s.DeliveredTo,
                        DeliveredInId = _s.DeliveringIn,
                        _s.CategoryId,
                        Status = _s.CategoryId == 1 ? _s.WorkServices.EntryVisas
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 2 ? _s.WorkServices.WorkPermits
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 3 ? _s.WorkServices.VisaDeregistrations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 4 ? _s.WorkServices.ResidencyPermits
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 5 ? "N/A"
                            : _s.CategoryId == 6 ? "N/A"
                            : _s.CategoryId == 7 ? _s.WorkServices.CorporateAssistances
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 8 ? _s.WorkServices.Renewals
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 9 ? _s.WorkServices.Notifications
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 10 ? _s.WorkServices.LegalReviews
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 12 ? _s.WorkServices.PredecisionOrientations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 13 ? _s.WorkServices.AreaOrientations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 14 ? _s.WorkServices.SettlingIns
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 15 ? _s.WorkServices.SchoolingSearches
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 16 ? _s.WorkServices.Departures
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 17 ? _s.WorkServices.TemporaryHousingCoordinatons
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 18 ? _s.WorkServices.RentalFurnitureCoordinations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 19 ? _s.WorkServices.Transportations
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 20 ? _s.WorkServices.AirportTransportationServices
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 21 ? _s.WorkServices.HomeFindings
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 22 ? _s.WorkServices.LeaseRenewals
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 23 ? _s.WorkServices.HomeSales
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 24 ? _s.WorkServices.HomePurchases
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 25 ? _s.WorkServices.PropertyManagements
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 26 ? _s.WorkServices.Others
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 27 ? _s.WorkServices.TenancyManagements
                                .FirstOrDefault().Status.Status
                            : _s.CategoryId == 28 ? _s.WorkServices.Others
                                .FirstOrDefault().Status.Status
                            : "N/A",
                        serviceName = _context.ServiceLocations.SingleOrDefault(o => o.IdService == _s.ServiceId
                                && o.IdClientPartnerProfile == _s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                                && o.IdServiceLine == _s.BundledServiceOrder.WorkOrder.ServiceLineId).NickName == "--" ? _s.Service.Service
                                : _context.ServiceLocations.SingleOrDefault(o => o.IdService == _s.ServiceId
                                && o.IdClientPartnerProfile == _s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                                && o.IdServiceLine == _s.BundledServiceOrder.WorkOrder.ServiceLineId).NickName,
                        _s.ServiceNumber,
                        _s.Autho,
                        _s.DeliveredToNavigation.Name,
                        _s.WorkServicesId,
                        service = _s.CategoryId == 1 ? _s.WorkServices.EntryVisas
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 2 ? _s.WorkServices.WorkPermits
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 3 ? _s.WorkServices.VisaDeregistrations
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 4 ? _s.WorkServices.ResidencyPermits
                                .Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 5 ? _context.CatCategories
                                .Where(x => x.Id == 5).Select(r => new { r.Id }).ToList()
                            : _s.CategoryId == 6 ? _context.CatCategories
                            .Where(x => x.Id == 6)
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 7 ? _s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 8 ? _s.WorkServices.Renewals
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 9 ? _s.WorkServices.Notifications
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 10 ? _s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 12 ? _s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 13 ? _s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 14 ? _s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 15 ? _s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 16 ? _s.WorkServices.Departures
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 17 ? _s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 18 ? _s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 19 ? _s.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 20 ? _s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 21 ? _s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 22 ? _s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                            : _s.CategoryId == 23 ? _s.WorkServices.HomeSales
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 24 ? _s.WorkServices.HomePurchases
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 25 ? _s.WorkServices.PropertyManagements
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 26 ? _s.WorkServices.Others
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 27 ? _s.WorkServices.TenancyManagements
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.CategoryId == 28 ? _s.WorkServices.Others
                                .Select(r => new
                                {
                                    r.Id
                                }).ToList()
                            : _s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                Id = 0
                            }).ToList(),
                        id_server = _s.CategoryId == 5 ? _context.DocumentManagements
                                .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(5)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                                         && f.ApplicantId == _s.DeliveredTo).Id
                            : _s.CategoryId == 6 ? _context.LocalDocumentations
                                .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(6)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                                         && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                                         && f.ApplicantId == _s.DeliveredTo).Id
                            //: _s.CategoryId == 19 ? _context.Transportations
                            //    .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(19)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                            //            && f.ApplicantId == _s.DeliveredTo).Id
                            //: _s.CategoryId == 20 ? _context.AirportTransportationServices
                            //    .FirstOrDefault(f => f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrder.ServiceRecordId).Contains(s.WorkOrder.ServiceRecordId)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.CategoryId).Contains(20)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.DeliveringIn).Contains(_s.DeliveringIn)
                            //            && f.WorkOrderServices.BundledServices.Select(q => q.BundledServiceOrder.WorkOrderId).Contains(_s.BundledServiceOrder.WorkOrderId)
                            //            && f.ApplicantId == _s.DeliveredTo).Id
                            : _s.Id,
                        timeSpent = Math.Abs(_context.ServiceReportDays.Where(x => x.ReportDay.WorkOrder.Value == s.Id && x.Service.Value == _s.Id).Sum(sigma => Convert.ToInt32(sigma.Time))),
                        remainingTime = Math.Abs(_context.ServiceReportDays.Where(x => x.ReportDay.WorkOrder.Value == s.Id && x.Service.Value == _s.Id).Sum(sigma => Convert.ToInt32(sigma.Time)) - int.Parse(s.TotalTime)),
                        requestedExtraTime = Math.Abs(_context.RequestAdditionalTimes.Where(x => x.WorkOrder.Value == s.WorkOrderId && x.Service.Value == _s.Id).Sum(sigma => sigma.RequestTime.Value))
                    })
                }).ToList();
            return new ObjectResult(query);
        }


        public Tuple<bool, string> AddAdditionalTime(int wo, int service, int time, int user, int userRequest)
        {
            string serviceNumber = "";
            bool val = false;
            var isBundled = _context.BundledServices.Include(i => i.BundledServiceOrder).SingleOrDefault(x => x.Id == service && x.BundledServiceOrder.WorkOrderId == wo);
            var isStandalone = _context.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderId == wo && x.Id == service);
            if (isBundled != null)
            {
                serviceNumber = isBundled.ServiceNumber;
                val = AdditionalTime(isBundled.WorkServicesId.Value, time, user, userRequest);
                isBundled.BundledServiceOrder.TotalTime = isBundled.BundledServiceOrder.TotalTime + time;
                _context.BundledServicesWorkOrders.Update(isBundled.BundledServiceOrder);
            }
            else if (isStandalone != null)
            {
                serviceNumber = isStandalone.ServiceNumber;
                val = AdditionalTime(isStandalone.WorkOrderServiceId.Value, time, user, userRequest);
                isStandalone.AuthoTime = isStandalone.AuthoTime + time;
                _context.StandaloneServiceWorkOrders.Update(isStandalone);
            }
            _context.SaveChanges();
            Tuple<bool, string> t = new Tuple<bool, string>(true, serviceNumber);
            return t;
        }
        public bool AdditionalTime(int wos, int time, int user, int userRequest)
        {
            bool success = false;
            var service = _context.WorkOrderServices
                .Include(i => i.VisaDeregistrations)
                .Include(i => i.AreaOrientations)
                .Include(i => i.SettlingIns)
                .Include(i => i.SchoolingSearches)
                .Include(i => i.Departures)
                .Include(i => i.TemporaryHousingCoordinatons)
                .Include(i => i.RentalFurnitureCoordinations)
                .Include(i => i.Transportations)
                .Include(i => i.AirportTransportationServices)
                .Include(i => i.HomeFindings)
                .SingleOrDefault(x => x.Id == wos);
            var _user = _context.Users.SingleOrDefault(x => x.Id == user);
            if (service.VisaDeregistrations.Count > 0)
            {
                // Some stuff
            }
            else if (service.AreaOrientations.Count > 0)
            {
                ExtensionAreaOrientation extensionAreaOrientation = new ExtensionAreaOrientation();
                extensionAreaOrientation.AreaOrientationId = service.AreaOrientations.FirstOrDefault().Id;
                extensionAreaOrientation.RequestedBy = user;
                extensionAreaOrientation.CreatedBy = user;
                extensionAreaOrientation.CreatedDate = DateTime.Now;
                extensionAreaOrientation.AuthoAcceptanceDate = DateTime.Now;
                extensionAreaOrientation.AuthoDate = DateTime.Now;
                extensionAreaOrientation.AuthorizedBy = userRequest;
                extensionAreaOrientation.Time = time;
                _context.ExtensionAreaOrientations.Add(extensionAreaOrientation);
                success = true;
            }
            else if (service.SettlingIns.Count > 0)
            {
                ExtensionSettlingIn extensionSettlingIn = new ExtensionSettlingIn();
                extensionSettlingIn.SettlingInId = service.SettlingIns.FirstOrDefault().Id;
                extensionSettlingIn.RequestedBy = user;
                extensionSettlingIn.CreatedBy = user;
                extensionSettlingIn.CreatedDate = DateTime.Now;
                extensionSettlingIn.AuthoAcceptanceDate = DateTime.Now;
                extensionSettlingIn.AuthoDate = DateTime.Now;
                extensionSettlingIn.AuthorizedBy = userRequest;
                extensionSettlingIn.Time = time;
                _context.ExtensionSettlingIns.Add(extensionSettlingIn);
                success = true;
            }
            else if (service.SchoolingSearches.Count > 0)
            {
                ExtensionSchoolingSearch extensionSchoolingSearch = new ExtensionSchoolingSearch();
                extensionSchoolingSearch.SchoolingSearchId = service.SchoolingSearches.FirstOrDefault().Id;
                extensionSchoolingSearch.RequestedBy = user;
                extensionSchoolingSearch.CreatedBy = user;
                extensionSchoolingSearch.CreatedDate = DateTime.Now;
                extensionSchoolingSearch.AuthoAcceptanceDate = DateTime.Now;
                extensionSchoolingSearch.AuthoDate = DateTime.Now;
                extensionSchoolingSearch.AuthorizedBy = userRequest;
                extensionSchoolingSearch.Time = time;
                _context.ExtensionSchoolingSearches.Add(extensionSchoolingSearch);
                success = true;
            }
            else if (service.Departures.Count > 0)
            {
                ExtensionDeparture extensionDeparture = new ExtensionDeparture();
                extensionDeparture.DepartureId = service.Departures.FirstOrDefault().Id;
                extensionDeparture.RequestedBy = user;
                extensionDeparture.CreatedBy = user;
                extensionDeparture.CreatedDate = DateTime.Now;
                extensionDeparture.AuthoAcceptanceDate = DateTime.Now;
                extensionDeparture.AuthoDate = DateTime.Now;
                extensionDeparture.AuthorizedBy = userRequest;
                extensionDeparture.Time = time;
                _context.ExtensionDepartures.Add(extensionDeparture);
                success = true;
            }
            else if (service.TemporaryHousingCoordinatons.Count > 0)
            {
                ExtensionTemporaryHousingCoordinaton extensionTemporaryHousingCoordinaton = new ExtensionTemporaryHousingCoordinaton();
                extensionTemporaryHousingCoordinaton.TemporaryHousingCoordinationId = service.TemporaryHousingCoordinatons.FirstOrDefault().Id;
                extensionTemporaryHousingCoordinaton.RequestedBy = user;
                extensionTemporaryHousingCoordinaton.CreatedBy = user;
                extensionTemporaryHousingCoordinaton.CreatedDate = DateTime.Now;
                extensionTemporaryHousingCoordinaton.AuthoAcceptanceDate = DateTime.Now;
                extensionTemporaryHousingCoordinaton.AuthoDate = DateTime.Now;
                extensionTemporaryHousingCoordinaton.AuthorizedBy = userRequest;
                extensionTemporaryHousingCoordinaton.Time = time;
                _context.ExtensionTemporaryHousingCoordinatons.Add(extensionTemporaryHousingCoordinaton);
                success = true;
            }
            else if (service.RentalFurnitureCoordinations.Count > 0)
            {
                ExtensionRentalFurnitureCoordination extensionRentalFurnitureCoordination = new ExtensionRentalFurnitureCoordination();
                extensionRentalFurnitureCoordination.RentalFurnitureCoordinationId = service.RentalFurnitureCoordinations.FirstOrDefault().Id;
                extensionRentalFurnitureCoordination.RequestedBy = user;
                extensionRentalFurnitureCoordination.CreatedBy = user;
                extensionRentalFurnitureCoordination.CreatedDate = DateTime.Now;
                extensionRentalFurnitureCoordination.AuthoAcceptanceDate = DateTime.Now;
                extensionRentalFurnitureCoordination.AuthoDate = DateTime.Now;
                extensionRentalFurnitureCoordination.AuthorizedBy = userRequest;
                extensionRentalFurnitureCoordination.Time = time;
                _context.ExtensionRentalFurnitureCoordinations.Add(extensionRentalFurnitureCoordination);
                success = true;
            }
            else if (service.Transportations.Count > 0)
            {
                ExtensionTransportation extensionTransportation = new ExtensionTransportation();
                extensionTransportation.TransportationId = service.Transportations.FirstOrDefault().Id;
                extensionTransportation.RequestedBy = user;
                extensionTransportation.CreatedBy = user;
                extensionTransportation.CreatedDate = DateTime.Now;
                extensionTransportation.AuthoAcceptanceDate = DateTime.Now;
                extensionTransportation.AuthoDate = DateTime.Now;
                extensionTransportation.AuthorizedBy = userRequest;
                extensionTransportation.Time = time;
                _context.ExtensionTransportations.Add(extensionTransportation);
                success = true;
            }
            else if (service.AirportTransportationServices.Count > 0)
            {
                ExtensionAirportTransportationService extensionAirportTransportationService = new ExtensionAirportTransportationService();
                extensionAirportTransportationService.AirportTransportationServicesId = service.AirportTransportationServices.FirstOrDefault().Id;
                extensionAirportTransportationService.RequestedBy = user;
                extensionAirportTransportationService.CreatedBy = user;
                extensionAirportTransportationService.CreatedDate = DateTime.Now;
                extensionAirportTransportationService.AuthoAcceptanceDate = DateTime.Now;
                extensionAirportTransportationService.AuthoDate = DateTime.Now;
                extensionAirportTransportationService.AuthorizedBy = userRequest;
                extensionAirportTransportationService.Time = time;
                _context.ExtensionAirportTransportationServices.Add(extensionAirportTransportationService);
                success = true;
            }
            else if (service.HomeFindings.Count > 0)
            {
                ExtensionHomeFinding extensionHomeFinding = new ExtensionHomeFinding();
                extensionHomeFinding.HomeFindingId = service.HomeFindings.FirstOrDefault().Id;
                extensionHomeFinding.RequestedBy = user;
                extensionHomeFinding.CreatedBy = user;
                extensionHomeFinding.CreatedDate = DateTime.Now;
                extensionHomeFinding.AuthoAcceptanceDate = DateTime.Now;
                extensionHomeFinding.AuthoDate = DateTime.Now;
                extensionHomeFinding.AuthorizedBy = userRequest;
                extensionHomeFinding.Time = time;
                _context.ExtensionHomeFindings.Add(extensionHomeFinding);
                success = true;
            }
            _context.SaveChanges();
            return success;
        }

        public ActionResult GetCommentsHistory(int sr)
        {
            if (sr == 0)
                return null;
            var consultHistory = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr)
                .Select(s => new
                {
                    s.WorkOrder.NumberWorkOrder,
                    s.Category.Category,
                    s.ServiceNumber,
                    s.CreatedDate,
                    s.WorkOrder.ServiceLine.ServiceLine,
                    nameAndReply = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentsEntryVisas.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentsEntryVisas.FirstOrDefault().User.Name
                                + " " + _s.CommentsEntryVisas.FirstOrDefault().User.LastName
                                + " " + _s.CommentsEntryVisas.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentsEntryVisas.Count
                        })
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentsWorkPermits.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentsWorkPermits.FirstOrDefault().User.Name
                                + " " + _s.CommentsWorkPermits.FirstOrDefault().User.LastName
                                + " " + _s.CommentsWorkPermits.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentsWorkPermits.Count
                        })
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentVisaDeregistrations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentVisaDeregistrations.FirstOrDefault().User.Name
                                + " " + _s.CommentVisaDeregistrations.FirstOrDefault().User.LastName
                                + " " + _s.CommentVisaDeregistrations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentVisaDeregistrations.Count
                        })
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentResidencyPermits.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentResidencyPermits.FirstOrDefault().User.Name
                                + " " + _s.CommentResidencyPermits.FirstOrDefault().User.LastName
                                + " " + _s.CommentResidencyPermits.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentResidencyPermits.Count
                        })
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentDocumentManagements.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentDocumentManagements.FirstOrDefault().User.Name
                                + " " + _s.CommentDocumentManagements.FirstOrDefault().User.LastName
                                + " " + _s.CommentDocumentManagements.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentDocumentManagements.Count
                        })
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentLocalDocumentations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentLocalDocumentations.FirstOrDefault().User.Name
                                + " " + _s.CommentLocalDocumentations.FirstOrDefault().User.LastName
                                + " " + _s.CommentLocalDocumentations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentLocalDocumentations.Count
                        })
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentCorporateAssistances.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentCorporateAssistances.FirstOrDefault().User.Name
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.LastName
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentCorporateAssistances.Count
                        })
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentRenewals.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentRenewals.FirstOrDefault().User.Name
                                + " " + _s.CommentRenewals.FirstOrDefault().User.LastName
                                + " " + _s.CommentRenewals.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentRenewals.Count
                        })
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentNotifications.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentNotifications.FirstOrDefault().User.Name
                                + " " + _s.CommentNotifications.FirstOrDefault().User.LastName
                                + " " + _s.CommentNotifications.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentNotifications.Count
                        })
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentLegalReviews.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentLegalReviews.FirstOrDefault().User.Name
                                + " " + _s.CommentLegalReviews.FirstOrDefault().User.LastName
                                + " " + _s.CommentLegalReviews.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentLegalReviews.Count
                        })
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentPredecisionOrientations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentPredecisionOrientations.FirstOrDefault().User.Name
                                + " " + _s.CommentPredecisionOrientations.FirstOrDefault().User.LastName
                                + " " + _s.CommentPredecisionOrientations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentPredecisionOrientations.Count
                        })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentAreaOrientations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentAreaOrientations.FirstOrDefault().User.Name
                                + " " + _s.CommentAreaOrientations.FirstOrDefault().User.LastName
                                + " " + _s.CommentAreaOrientations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentAreaOrientations.Count
                        })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentSettlingIns.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentSettlingIns.FirstOrDefault().User.Name
                                + " " + _s.CommentSettlingIns.FirstOrDefault().User.LastName
                                + " " + _s.CommentSettlingIns.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentSettlingIns.Count
                        })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentSchoolingSearches.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentSchoolingSearches.FirstOrDefault().User.Name
                                + " " + _s.CommentSchoolingSearches.FirstOrDefault().User.LastName
                                + " " + _s.CommentSchoolingSearches.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentSchoolingSearches.Count
                        })
                        : s.CategoryId == 16 ? s.WorkOrderService.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentCorporateAssistances.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentCorporateAssistances.FirstOrDefault().User.Name
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.LastName
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentCorporateAssistances.Count
                        })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentTemporaryHosuings.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentTemporaryHosuings.FirstOrDefault().User.Name
                                + " " + _s.CommentTemporaryHosuings.FirstOrDefault().User.LastName
                                + " " + _s.CommentTemporaryHosuings.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentTemporaryHosuings.Count
                        })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentRentalFurnitureCoordinations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentRentalFurnitureCoordinations.FirstOrDefault().User.Name
                                + " " + _s.CommentRentalFurnitureCoordinations.FirstOrDefault().User.LastName
                                + " " + _s.CommentRentalFurnitureCoordinations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentRentalFurnitureCoordinations.Count
                        })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentTransportations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentTransportations.FirstOrDefault().User.Name
                                + " " + _s.CommentTransportations.FirstOrDefault().User.LastName
                                + " " + _s.CommentTransportations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentTransportations.Count
                        })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentAirportTransportationServices.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentAirportTransportationServices.FirstOrDefault().User.Name
                                + " " + _s.CommentAirportTransportationServices.FirstOrDefault().User.LastName
                                + " " + _s.CommentAirportTransportationServices.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentAirportTransportationServices.Count
                        })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId && x.CommentHomeFindings.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentHomeFindings.FirstOrDefault().User.Name
                                + " " + _s.CommentHomeFindings.FirstOrDefault().User.LastName
                                + " " + _s.CommentHomeFindings.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentHomeFindings.Count
                        })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentLeaseRenewals.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentLeaseRenewals.FirstOrDefault().CreationByNavigation.Name
                                + " " + _s.CommentLeaseRenewals.FirstOrDefault().CreationByNavigation.LastName
                                + " " + _s.CommentLeaseRenewals.FirstOrDefault().CreationByNavigation.MotherLastName,
                            reply = _s.CommentLeaseRenewals.Count
                        }) : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentHomeSales.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentHomeSales.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentHomeSales.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentHomeSales.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentHomeSales.Count
                        }) : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentHomePurchases.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentHomePurchases.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentHomePurchases.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentHomePurchases.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentHomePurchases.Count
                        }) : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentPropertyManagements.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentPropertyManagements.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentPropertyManagements.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentPropertyManagements.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentPropertyManagements.Count
                        }) : s.CategoryId == 26 ? s.WorkOrderService.Others
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentOthers.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentOthers.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentOthers.Count
                        }) : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentTenancyManagements.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentTenancyManagements.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentTenancyManagements.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentTenancyManagements.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentTenancyManagements.Count
                        }) : s.CategoryId == 28 ? s.WorkOrderService.Others
                        .Where(x => x.WorkOrderServices == s.WorkOrderServiceId && x.CommentOthers.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentOthers.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentOthers.Count
                        })
                        : null,
                    comments = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentsEntryVisas.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentsWorkPermits.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentVisaDeregistrations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentResidencyPermits.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentDocumentManagements.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentLocalDocumentations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentCorporateAssistances.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentRenewals.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentNotifications.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentLegalReviews.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentPredecisionOrientations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentAreaOrientations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentSettlingIns.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentSchoolingSearches.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 16 ? s.WorkOrderService.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentCorporateAssistances.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentTemporaryHosuings.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentRentalFurnitureCoordinations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentTransportations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentAirportTransportationServices.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                        .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                        .Select(r => new
                        {
                            comment = r.CommentHomeFindings.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : null
                }).ToList();

            var consultHistories = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr)
                .Select(s => new
                {
                    s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    s.Category.Category,
                    s.ServiceNumber,
                    s.CreatedDate,
                    s.BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
                    nameAndReply = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentsEntryVisas.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentsEntryVisas.FirstOrDefault().User.Name
                                + " " + _s.CommentsEntryVisas.FirstOrDefault().User.LastName
                                + " " + _s.CommentsEntryVisas.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentsEntryVisas.Count
                        })
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentsWorkPermits.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentsWorkPermits.FirstOrDefault().User.Name
                                + " " + _s.CommentsWorkPermits.FirstOrDefault().User.LastName
                                + " " + _s.CommentsWorkPermits.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentsWorkPermits.Count
                        })
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentVisaDeregistrations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentVisaDeregistrations.FirstOrDefault().User.Name
                                + " " + _s.CommentVisaDeregistrations.FirstOrDefault().User.LastName
                                + " " + _s.CommentVisaDeregistrations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentVisaDeregistrations.Count
                        })
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentResidencyPermits.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentResidencyPermits.FirstOrDefault().User.Name
                                + " " + _s.CommentResidencyPermits.FirstOrDefault().User.LastName
                                + " " + _s.CommentResidencyPermits.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentResidencyPermits.Count
                        })
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentDocumentManagements.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentDocumentManagements.FirstOrDefault().User.Name
                                + " " + _s.CommentDocumentManagements.FirstOrDefault().User.LastName
                                + " " + _s.CommentDocumentManagements.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentDocumentManagements.Count
                        })
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentLocalDocumentations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentLocalDocumentations.FirstOrDefault().User.Name
                                + " " + _s.CommentLocalDocumentations.FirstOrDefault().User.LastName
                                + " " + _s.CommentLocalDocumentations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentLocalDocumentations.Count
                        })
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentCorporateAssistances.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentCorporateAssistances.FirstOrDefault().User.Name
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.LastName
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentCorporateAssistances.Count
                        })
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentRenewals.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentRenewals.FirstOrDefault().User.Name
                                + " " + _s.CommentRenewals.FirstOrDefault().User.LastName
                                + " " + _s.CommentRenewals.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentRenewals.Count
                        })
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentNotifications.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentNotifications.FirstOrDefault().User.Name
                                + " " + _s.CommentNotifications.FirstOrDefault().User.LastName
                                + " " + _s.CommentNotifications.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentNotifications.Count
                        })
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentLegalReviews.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentLegalReviews.FirstOrDefault().User.Name
                                + " " + _s.CommentLegalReviews.FirstOrDefault().User.LastName
                                + " " + _s.CommentLegalReviews.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentLegalReviews.Count
                        })
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentPredecisionOrientations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentPredecisionOrientations.FirstOrDefault().User.Name
                                + " " + _s.CommentPredecisionOrientations.FirstOrDefault().User.LastName
                                + " " + _s.CommentPredecisionOrientations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentPredecisionOrientations.Count
                        })
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentAreaOrientations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentAreaOrientations.FirstOrDefault().User.Name
                                + " " + _s.CommentAreaOrientations.FirstOrDefault().User.LastName
                                + " " + _s.CommentAreaOrientations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentAreaOrientations.Count
                        })
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentSettlingIns.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentSettlingIns.FirstOrDefault().User.Name
                                + " " + _s.CommentSettlingIns.FirstOrDefault().User.LastName
                                + " " + _s.CommentSettlingIns.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentSettlingIns.Count
                        })
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentSchoolingSearches.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentSchoolingSearches.FirstOrDefault().User.Name
                                + " " + _s.CommentSchoolingSearches.FirstOrDefault().User.LastName
                                + " " + _s.CommentSchoolingSearches.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentSchoolingSearches.Count
                        })
                        : s.CategoryId == 16 ? s.WorkServices.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentCorporateAssistances.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentCorporateAssistances.FirstOrDefault().User.Name
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.LastName
                                + " " + _s.CommentCorporateAssistances.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentCorporateAssistances.Count
                        })
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentTemporaryHosuings.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentTemporaryHosuings.FirstOrDefault().User.Name
                                + " " + _s.CommentTemporaryHosuings.FirstOrDefault().User.LastName
                                + " " + _s.CommentTemporaryHosuings.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentTemporaryHosuings.Count
                        })
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentRentalFurnitureCoordinations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentRentalFurnitureCoordinations.FirstOrDefault().User.Name
                                + " " + _s.CommentRentalFurnitureCoordinations.FirstOrDefault().User.LastName
                                + " " + _s.CommentRentalFurnitureCoordinations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentRentalFurnitureCoordinations.Count
                        })
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentTransportations.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentTransportations.FirstOrDefault().User.Name
                                + " " + _s.CommentTransportations.FirstOrDefault().User.LastName
                                + " " + _s.CommentTransportations.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentTransportations.Count
                        })
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentAirportTransportationServices.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentAirportTransportationServices.FirstOrDefault().User.Name
                                + " " + _s.CommentAirportTransportationServices.FirstOrDefault().User.LastName
                                + " " + _s.CommentAirportTransportationServices.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentAirportTransportationServices.Count
                        })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId && x.CommentHomeFindings.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentHomeFindings.FirstOrDefault().User.Name
                                + " " + _s.CommentHomeFindings.FirstOrDefault().User.LastName
                                + " " + _s.CommentHomeFindings.FirstOrDefault().User.MotherLastName,
                            reply = _s.CommentHomeFindings.Count
                        }) : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentLeaseRenewals.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentLeaseRenewals.FirstOrDefault().CreationByNavigation.Name
                                + " " + _s.CommentLeaseRenewals.FirstOrDefault().CreationByNavigation.LastName
                                + " " + _s.CommentLeaseRenewals.FirstOrDefault().CreationByNavigation.MotherLastName,
                            reply = _s.CommentLeaseRenewals.Count
                        }) : s.CategoryId == 23 ? s.WorkServices.HomeSales
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentHomeSales.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentHomeSales.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentHomeSales.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentHomeSales.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentHomeSales.Count
                        }) : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentHomePurchases.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentHomePurchases.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentHomePurchases.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentHomePurchases.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentHomePurchases.Count
                        }) : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentPropertyManagements.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentPropertyManagements.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentPropertyManagements.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentPropertyManagements.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentPropertyManagements.Count
                        }) : s.CategoryId == 26 ? s.WorkServices.Others
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentOthers.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentOthers.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentOthers.Count
                        }) : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentTenancyManagements.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentTenancyManagements.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentTenancyManagements.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentTenancyManagements.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentTenancyManagements.Count
                        }) : s.CategoryId == 28 ? s.WorkServices.Others
                        .Where(x => x.WorkOrderServices == s.WorkServicesId && x.CommentOthers.Any())
                        .Select(_s => new
                        {
                            name = _s.CommentOthers.FirstOrDefault().CreatedByNavigation.Name
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.LastName
                                + " " + _s.CommentOthers.FirstOrDefault().CreatedByNavigation.MotherLastName,
                            reply = _s.CommentOthers.Count
                        })
                        : null,
                    comments = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentsEntryVisas.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentsWorkPermits.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentVisaDeregistrations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentResidencyPermits.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentDocumentManagements.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentLocalDocumentations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentCorporateAssistances.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentRenewals.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentNotifications.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentLegalReviews.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentPredecisionOrientations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentAreaOrientations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentSettlingIns.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentSchoolingSearches.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 16 ? s.WorkServices.CorporateAssistances
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentCorporateAssistances.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentTemporaryHosuings.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentRentalFurnitureCoordinations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentTransportations.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentAirportTransportationServices.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                        .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                        .Select(r => new
                        {
                            comment = r.CommentHomeFindings.Select(_r => new
                            {
                                name = _r.User.Name + " " + _r.User.LastName + " " + _r.User.MotherLastName,
                                title = _r.User.UserType.Type,
                                _r.User.Avatar,
                                _r.Reply,
                                _r.CreatedDate,
                            })
                        })
                        : null
                }).ToList().Union(consultHistory).OrderBy(x => x.CreatedDate);
            consultHistories = consultHistories.Where(x => x.nameAndReply.Any() && x.nameAndReply.Any(a => a.reply > 1)).ToList()
                .OrderBy(x => x.CreatedDate);
            return new ObjectResult(consultHistories);
        }

        public ActionResult GetDeliverTo(int key)
        {
            var deliverTo = _context.WorkOrderServices.Where(x => x.Id == key).Select(s => new
            {
                name = s.BundledServices.Any()
                    ? s.BundledServices.FirstOrDefault().DeliveredToNavigation.Name
                    : s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Name,
                phone = s.BundledServices.Any()
                    ? _context.AssigneeInformations.FirstOrDefault(f =>
                                f.Id == s.BundledServices.FirstOrDefault().DeliveredToNavigation.AssigneeInformationId)
                            .MobilePhone
                    : _context.AssigneeInformations.FirstOrDefault(f =>
                            f.Id == s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation
                                .AssigneeInformationId).MobilePhone,
                country = s.BundledServices.Any() ? s.BundledServices.FirstOrDefault().DeliveringInNavigation.Name
                    : s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringInNavigation.Name,
                avatar = s.BundledServices.Any()
                    ? s.BundledServices.FirstOrDefault().DeliveredToNavigation.RelationshipId != 7
                        ? s.BundledServices.FirstOrDefault().DeliveredToNavigation.Photo
                        : _context.AssigneeInformations.FirstOrDefault(f => f.Id == s.BundledServices.FirstOrDefault().DeliveredToNavigation.AssigneeInformationId)
                            .Photo
                    : s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.RelationshipId != 7
                        ? s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Photo
                        : _context.AssigneeInformations.FirstOrDefault(f => f.Id == s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.AssigneeInformationId)
                            .Photo,
                relationship = s.BundledServices.Any()
                    ? s.BundledServices.FirstOrDefault().DeliveredToNavigation.Relationship.Relationship
                    : s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Relationship.Relationship,
                relationshipId = s.BundledServices.Any()
                ? s.BundledServices.FirstOrDefault().DeliveredToNavigation.RelationshipId
                : s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.RelationshipId
            }).FirstOrDefault();
            return new ObjectResult(deliverTo);
        }

        public bool Delete(biz.premier.Entities.WorkOrder order)
        {
            bool isSuccess = false;
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderId == order.Id)
                .Select(s => new
                {
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                        .FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .FirstOrDefault().StatusId
                        : (int?)null,
                });
            var bundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrderId == order.Id)
                .Select(s => new
                {
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                        .FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.CorporateAssistances
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .FirstOrDefault().StatusId
                        : (int?)null,
                });
            var services = standalone.Union(bundled).ToList();
            if (services.All(a => a.status == 1))
            {
                _context.WorkOrders.Remove(order);
                _context.SaveChanges();
                isSuccess = true;
            }

            return isSuccess;
        }

        public ActionResult GetInvoiceList(int user, DateTime? range1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            var invoices = _context.ServiceInvoices.Select(s => new
            {
                id = s.Invoice,
                status = s.StatusNavigation.Status,
                statusId = s.Status,
                serviceLineId = s.WorkOrderNavigation.ServiceLineId,
                serviceLine = s.WorkOrderNavigation.ServiceLine.ServiceLine,
                serviceRecord = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
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
                    ? s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                requestedDate = s.InvoiceNavigation.CreatedDate,
                invoiceType = s.InvoiceNavigation.InvoiceTypeNavigation.Type,
                invoiceTypeId = s.InvoiceNavigation.InvoiceType,
                country = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any()
                    ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 1).Any()
                        ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 1).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name)
                        : s.WorkOrderNavigation.BundledServicesWorkOrders.Where(x => x.BundledServices.Select(a => a.DeliveringIn).Contains(1)).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name)
                    : s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 2).Any()
                        ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 2).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name)
                        : s.WorkOrderNavigation.BundledServicesWorkOrders.Where(x => x.BundledServices.Select(a => a.DeliveringIn).Contains(2)).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name),
                countryId = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any()
                    ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 1).Any()
                        ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 1).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId).FirstOrDefault().Value
                        : s.WorkOrderNavigation.BundledServicesWorkOrders.Where(x => x.BundledServices.Select(a => a.DeliveringIn).Contains(1)).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry).FirstOrDefault().Value
                    : s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 2).Any()
                        ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.DeliveringIn == 2).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry).FirstOrDefault().Value
                        : s.WorkOrderNavigation.BundledServicesWorkOrders.Where(x => x.BundledServices.Select(a => a.DeliveringIn).Contains(2)).Select(n => n.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry).FirstOrDefault().Value,
                dueDate = DateTime.Today,
                amount = s.AmountToInvoice,
                description = s.InvoiceNavigation.Comments
            }).ToList();
            if (range1.HasValue && range2.HasValue)
                invoices = invoices.Where(x => x.requestedDate.Value > range1.Value && x.requestedDate.Value < range2.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (serviceLine.HasValue)
                invoices = invoices.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (invoiceType.HasValue)
                invoices = invoices.Where(x => x.invoiceTypeId == invoiceType.Value).ToList();
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

        public ActionResult GetRequestCenter(DateTime? range1, DateTime? range2, int? status, int? serviceLine, int? requestType, int? coordinator, int? partner, int? client, int? country)
        {
            var invoices = _context.ServiceInvoices.Where(x => x.InvoiceNavigation.InvoiceType == 2 && x.Status == 2).Select(s => new
            {
                id = s.Invoice.Value,
                status = s.StatusNavigation.Status,
                statusId = s.Status,
                serviceLineId = s.WorkOrderNavigation.ServiceLineId.Value,
                serviceLine = s.WorkOrderNavigation.ServiceLine.ServiceLine,
                serviceRecord = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                serviceRecordId = s.InvoiceNavigation.ServiceRecord.Value,
                asignee = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                partnerId = s.InvoiceNavigation.ServiceRecordNavigation.PartnerId,
                client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                clientId = s.InvoiceNavigation.ServiceRecordNavigation.ClientId,
                //coordinator = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                //    ? $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.LastName} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.MotherLastName}"
                //    : $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.Name} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.LastName} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.MotherLastName}",
                //coordinatorId = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                //    ? s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().CoordinatorId.Value
                //    : s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().CoordinatorId.Value,
                requestedDate = s.InvoiceNavigation.CreatedDate.Value,
                request_type = "SPP Invoice",
                request_id = s.InvoiceNavigation.InvoiceType,
                country = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).HomeCountry.Name,
                countryId = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).HomeCountry.Id,
                dueDate = DateTime.Today,
                amount = s.AmountToInvoice.Value,
                description = s.InvoiceNavigation.Comments,
                data = s.Invoice.Value,
                currency = _context.ProfileUsers
                    .Where(x => x.Id == s.InvoiceNavigation.Consultant)
                    .Select(a => a.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency).FirstOrDefault(),
            }).ToList();

            var invoiceServices = _context.ServiceInvoices.Where(x => x.InvoiceNavigation.InvoiceType == 1 && x.Status == 2).Select(s => new
            {
                id = s.Invoice.Value,
                status = s.StatusNavigation.Status,
                statusId = s.Status,
                serviceLineId = s.WorkOrderNavigation.ServiceLineId.Value,
                serviceLine = s.WorkOrderNavigation.ServiceLine.ServiceLine,
                serviceRecord = s.InvoiceNavigation.ServiceRecordNavigation.NumberServiceRecord,
                serviceRecordId = s.InvoiceNavigation.ServiceRecord.Value,
                asignee = s.InvoiceNavigation.ServiceRecordNavigation.AssigneeInformations.FirstOrDefault().AssigneeName,
                partner = s.InvoiceNavigation.ServiceRecordNavigation.Partner.Name,
                partnerId = s.InvoiceNavigation.ServiceRecordNavigation.PartnerId,
                client = s.InvoiceNavigation.ServiceRecordNavigation.Client.Name,
                clientId = s.InvoiceNavigation.ServiceRecordNavigation.ClientId,
                //coordinator = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                //    ? $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.LastName} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().Coordinator.MotherLastName}"
                //    : $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.Name} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.LastName} " +
                //    $"{s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().Coordinator.MotherLastName}",
                //coordinatorId = s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.Any()
                //    ? s.InvoiceNavigation.ServiceRecordNavigation.ImmigrationCoodinators.FirstOrDefault().CoordinatorId.Value
                //    : s.InvoiceNavigation.ServiceRecordNavigation.RelocationCoordinators.FirstOrDefault().CoordinatorId.Value,
                requestedDate = s.InvoiceNavigation.CreatedDate.Value,
                request_type = "Service Invoice",
                request_id = s.InvoiceNavigation.InvoiceType,
                country = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).HomeCountry.Name,
                countryId = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).HomeCountry.Id,
                dueDate = DateTime.Today,
                amount = s.AmountToInvoice.Value,
                description = s.InvoiceNavigation.Comments,
                data = s.Invoice.Value,
                currency = _context.ProfileUsers
                    .Where(x => x.Id == s.InvoiceNavigation.Consultant)
                    .Select(a => a.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency).FirstOrDefault(),
            }).ToList();

            var invoicesRequestPayment = _context.RequestPayments
                .Where(x => x.Status.Value == 1).Select(s => new
                {
                    id = s.Id,
                    status = s.StatusNavigation.Status,
                    statusId = s.Status,
                    serviceLineId = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLineId.Value
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLineId.Value,
                    serviceLine = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLine.ServiceLine,
                    serviceRecord = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.NumberServiceRecord
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.NumberServiceRecord,
                    serviceRecordId = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId.Value
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId.Value,
                    asignee = _context.AssigneeInformations.Where(x => x.ServiceRecordId == s.Payments.FirstOrDefault().ServiceRecord.Value).FirstOrDefault().AssigneeName,
                    partner = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.Partner.Name
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.Partner.Name,
                    partnerId = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.PartnerId
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.PartnerId,
                    client = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.Client.Name
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.Client.Name,
                    clientId = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.ClientId,
                    requestedDate = s.CreatedDate.Value,
                    request_type = "Third Party Invoice",
                    request_id = s.Status,
                    country = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().DeliveringInNavigation.Name
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringInNavigation.Name,
                    countryId = s.WorkOrderServices.BundledServices.Any()
                    ? s.WorkOrderServices.BundledServices.FirstOrDefault().DeliveringIn.Value
                    : s.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringIn.Value,
                    dueDate = s.PaymentDate.Value,
                    amount = s.Payments.FirstOrDefault().PaymentAmount.Value,
                    description = s.Description,
                    data = s.Id,
                    currency = s.Payments.FirstOrDefault().CurrencyPaymentAmountNavigation.Currency,
                }).ToList();

            invoices = invoices.Union(invoicesRequestPayment).Union(invoiceServices).ToList();

            if (range1.HasValue && range2.HasValue)
                invoices = invoices.Where(x => x.requestedDate > range1.Value && x.requestedDate < range2.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (status.HasValue)
                invoices = invoices.Where(x => x.statusId == status.Value).ToList();
            if (serviceLine.HasValue)
                invoices = invoices.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (requestType.HasValue)
                invoices = invoices.Where(x => x.request_id == requestType.Value).ToList();
            //if (coordinator.HasValue)
            //    invoices = invoices.Where(x => x.coordinatorId == coordinator.Value).ToList();
            if (partner.HasValue)
                invoices = invoices.Where(x => x.partnerId == partner.Value).ToList();
            if (client.HasValue)
                invoices = invoices.Where(x => x.clientId == client.Value).ToList();
            if (country.HasValue)
                invoices = invoices.Where(x => x.countryId == country.Value).ToList();
            return new ObjectResult(invoices.OrderBy(o => o.id));

        }

        public biz.premier.Entities.Invoice UpdateInvoice(biz.premier.Entities.Invoice Dto)
        {
            if (Dto == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Invoice>()
                .Include(i => i.CommentInvoices)
                .Include(i => i.DocumentInvoices)
                .Include(i => i.ServiceInvoices)
                .SingleOrDefault(s => s.Id == Dto.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(Dto);
                // Comments
                foreach (var comment in Dto.CommentInvoices)
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
                foreach (var document in Dto.DocumentInvoices)
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
                foreach (var workOrder in Dto.ServiceInvoices)
                {
                    var existingWorkOrder = exist.ServiceInvoices.Where(p => p.Id == workOrder.Id).FirstOrDefault();
                    if (existingWorkOrder == null)
                    {
                        exist.ServiceInvoices.Add(workOrder);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingWorkOrder).CurrentValues.SetValues(workOrder);
                    }
                }

                _context.SaveChanges();
            }
            return exist;
        }

        public ActionResult GetSupplierPartnerInvoices(int sr)
        {
            var invoices = _context.ServiceInvoices.Where(x => x.InvoiceNavigation.ServiceRecord == sr && x.InvoiceNavigation.InvoiceType == 2).Select(s => new
            {
                id = s.Invoice,
                status = s.StatusNavigation.Status,
                serviceLine = s.WorkOrderNavigation.ServiceLine.ServiceLine,
                workOrder = s.WorkOrderNavigation.NumberWorkOrder,
                serviceID = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any()
                    ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.Where(x => x.Id == s.Service).FirstOrDefault().ServiceNumber
                    : s.WorkOrderNavigation.BundledServicesWorkOrders.SingleOrDefault(x => x.BundledServices.Select(a => a.Id).Contains(s.Service.Value)).BundledServices.FirstOrDefault().ServiceNumber,
                dueDate = DateTime.Today,
                invoiceDate = s.InvoiceNavigation.CreatedDate,
                requestedDate = s.InvoiceNavigation.CreatedDate,
                invoiceType = s.AmountToInvoice,
                closedDale = DateTime.Today
            }).ToList();
            return new ObjectResult(invoices);
        }
        private Random gen = new Random();
        DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        //Traductor de pais de la tabla countries a la tabla cat_country que es la tabla que tiene los paises en donde premier realiza servicios
        public int GetCountryToCountry(int id)
        {
            var nameCountry = _context.Countries.Any(x => x.Id == id)
                ? _context.Countries.FirstOrDefault(x => x.Id == id).Name
                : _context.CatCountries.FirstOrDefault(x => x.Id == id).Name;

            var nameCountryId = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == nameCountry.ToLower()).Id;

            return nameCountryId;
        }
    }
}
