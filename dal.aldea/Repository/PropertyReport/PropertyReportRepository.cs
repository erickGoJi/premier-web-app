using biz.premier.Repository.PropertyReport;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.PropertyReport
{
    public class PropertyReportRepository : GenericRepository<biz.premier.Entities.PropertyReport>, IPropertyReportRepository
    {
        public PropertyReportRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.PhotosInventory FindphotoInventory(int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.PhotosInventory>()
                .SingleOrDefault(s => s.Id == key);
            return exist;
        }

        public List<biz.premier.Entities.PropertyReport> GetCustom(int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.PropertyReport>()
                .Include(i => i.Attendees)
                .Include(i => i.KeyInventories)
                .Include(i => i.PropertyReportSections)
                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                //.Include(i => i.CreatedByNavigation)
                .Include(i => i.PropertyReportSections)
                    .ThenInclude(i => i.SectionInventories)
                    .ThenInclude(i => i.PhotosInventories)
                .Where(s => s.HousingList == key).ToList();
            return exist;
        }

        public List<biz.premier.Entities.StatusPropertyReport> GetAllStatusPropertyReport()
        {
            
             
            var exist = _context.StatusPropertyReports.ToList();
            return exist;
        }

        public biz.premier.Entities.PropertyReport UpdateCustom(biz.premier.Entities.PropertyReport propertyReport, int key)
        {
            if (propertyReport == null)
                return null;

            var current = _context.PropertyReports.Update(propertyReport);
            _context.SaveChanges();


            var exist = _context.PropertyReports
                .Include(i => i.PropertyReportSections)
                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                .Include(i => i.PropertyReportSections)
                    .ThenInclude(i => i.SectionInventories)
                        .ThenInclude(i => i.PhotosInventories)
                .Include(i => i.KeyInventories)
                .Include(i => i.Attendees)
                .SingleOrDefault(s => s.Id == key);

            //if (exist != null)
            //{
            //    _context.Entry(exist).CurrentValues.SetValues(propertyReport);
            //    foreach (var a in propertyReport.PropertyReportSections)
            //    {
            //        var propertyReportSections = exist.PropertyReportSections.Where(x => x.Id == a.Id).FirstOrDefault();
            //        if (propertyReportSections == null)
            //        {
            //            exist.PropertyReportSections.Add(a);
            //        }
            //        else
            //        {
            //            _context.Entry(propertyReportSections).CurrentValues.SetValues(a);

            //            foreach (var i in a.PhotosPropertyReportSections)
            //            {
            //                var _exist = propertyReportSections.PhotosPropertyReportSections.Where(x => x.Id == i.Id).FirstOrDefault();
            //                if (_exist == null)
            //                {
            //                    propertyReportSections.PhotosPropertyReportSections.Add(i);
            //                    _context.SaveChanges();
            //                }
            //                else
            //                {
            //                    _context.Entry(_exist).CurrentValues.SetValues(i);
            //                }
            //            }

            //            foreach (var i in a.SectionInventories)
            //            {
            //                var _exist = propertyReportSections.SectionInventories.Where(x => x.Id == i.Id).FirstOrDefault();
            //                if (_exist == null)
            //                {
            //                    propertyReportSections.SectionInventories.Add(i);
            //                    _context.SaveChanges();
            //                }
            //                else
            //                {
            //                    _context.Entry(_exist).CurrentValues.SetValues(i);
            //                    foreach (var d in _exist.PhotosInventories)
            //                    {
            //                        var __exist = _exist.PhotosInventories.Where(x => x.Id == d.Id).FirstOrDefault();
            //                        if (__exist == null)
            //                        {
            //                            _exist.PhotosInventories.Add(d);
            //                            _context.SaveChanges();
            //                        }
            //                        else
            //                        {
            //                            _context.Entry(__exist).CurrentValues.SetValues(d);
            //                        }
            //                    }
            //                }
            //            }

            //        }
            //    }

            //    foreach (var i in propertyReport.Attendees)
            //    {
            //        var _exist = exist.Attendees.Where(x => x.Id == i.Id).FirstOrDefault();
            //        if (_exist == null)
            //        {
            //            exist.Attendees.Add(i);
            //            _context.SaveChanges();
            //        }
            //        else
            //        {
            //            _context.Entry(_exist).CurrentValues.SetValues(i);
            //        }
            //    }

            //    foreach (var i in propertyReport.KeyInventories)
            //    {
            //        var _exist = exist.KeyInventories.Where(x => x.Id == i.Id).FirstOrDefault();
            //        if (_exist == null)
            //        {
            //            exist.KeyInventories.Add(i);
            //            _context.SaveChanges();
            //        }
            //        else
            //        {
            //            _context.Entry(_exist).CurrentValues.SetValues(i);
            //        }
            //    }

            //    _context.SaveChanges();
            //}

            return exist;
        }


        public biz.premier.Entities.PropertyReportSection UpdateCustomSection(biz.premier.Entities.PropertyReportSection propertyReportSection, int key)
        {
            if (propertyReportSection == null)
                return null;

            var current = _context.PropertyReportSections.Update(propertyReportSection);
            _context.SaveChanges();


            var exist = _context.PropertyReportSections
                    .Include(ti => ti.PhotosPropertyReportSections)
                    .Include(i => i.SectionInventories)
                        .ThenInclude(i => i.PhotosInventories)
                .SingleOrDefault(s => s.Id == key);
            return exist;
        }

        public ActionResult GetsupplierPartner(int? supplier, int? supplier_company, int sr)
        {
            var query = _context.Payments
                .Where(x => x.ServiceRecord == sr)
                .Select(x => new
                {
                    SupplierType = x.SupplierPartner.HasValue ? x.SupplierPartnerNavigation.SupplierTypeNavigation.SupplierType : "N/A",
                    SupplierType_id = x.SupplierPartnerNavigation.SupplierType,
                    supplier_company = x.SupplierPartner.HasValue 
                        ? _context.CatSupplierCompanies.FirstOrDefault(c => c.Int == x.SupplierPartnerNavigation.SupplierType).SupplierCompany 
                        : "N/A",
                    supplier_id = x.Supplier,
                    supplier = x.IfSupplierPartner.Value.Equals(true) ? x.SupplierNavigation.ContactName : x.SupplierName, 
                        // _context.ProfileUsers
                        // .FirstOrDefault(y => y.ImmigrationSupplierPartners.Any(f => f.ServiceRecord.WorkOrders.Any(q=> q.Id == x.WorkOrder))
                        //                      || y.RelocationSupplierPartners.Any(f => f.ServiceRecord.WorkOrders.Any(q=> q.Id == x.WorkOrder))).Name,
                    date = x.CreatedDate,
                    Assigned_service = _context.RequestPayments.Count(s => s.WorkOrderServicesId == x.WorkOrderServices),
                    x.RequestPaymentNavigation.StatusNavigation.Status,
                    StatusId = x.RequestPaymentNavigation.Status,
                    Assigned_service_detail = _context.RequestPayments.Where(s => s.WorkOrderServicesId == x.WorkOrderServices).Select(g => new
                    {
                        g.Id,
                        g.WorkOrderServicesId,
                        Service_name = g.WorkOrderServices.StandaloneServiceWorkOrders.Any() 
                        ? g.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault(d=>d.WorkOrderServiceId == g.WorkOrderServicesId).Service.Service
                        : g.WorkOrderServices.BundledServices.Any()
                        ? g.WorkOrderServices.BundledServices.FirstOrDefault(d => d.WorkServicesId == g.WorkOrderServicesId).Service.Service
                        : "--",
                        Service_id = g.WorkOrderServices.StandaloneServiceWorkOrders.Any()
                        ? g.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault(d => d.WorkOrderServiceId == g.WorkOrderServicesId).ServiceId
                        : g.WorkOrderServices.BundledServices.Any()
                        ? g.WorkOrderServices.BundledServices.FirstOrDefault(d => d.WorkServicesId == g.WorkOrderServicesId).ServiceId
                        : 0,
                        x.PaymentDate,
                        accepted = true
                    }).ToList()
                }).ToList();

            if (supplier != null) {
                if (supplier != 0)
                {
                    query = query.Where(x => x.SupplierType_id == supplier).ToList();   
                }
            }

            if (supplier_company != null) {
                if (supplier_company != 0)
                {
                    query = query.Where(x => x.supplier_id == supplier_company).ToList();   
                }
            }

            return new ObjectResult(query);
        }
    }
}
