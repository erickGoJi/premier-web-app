using biz.premier.Repository.ExperienceSurvey;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.ExperienceSurvey
{
    public class ExperienceSurveyRepository : GenericRepository<biz.premier.Entities.ExperienceSurvey>, IExperienceSurveyRepository
    {
        public ExperienceSurveyRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.ExperienceSurvey SelectCustom(int key)
        {
            var survey = _context.Set<biz.premier.Entities.ExperienceSurvey>()
                .Include(i => i.ExperienceSurveySuppliers)
                    .ThenInclude(_i => _i.ExperienceSurvey)
                .SingleOrDefault(s => s.Id == key);
            if (survey == null)
                return null;
            return survey;
        }

        public bool CompleteSrByServiceLine(int sr, bool complete, int serviceLine)
        {
            bool isComplete = false;
            var record = _context.ServiceRecords.Find(sr);
            if (complete)
            {
                if (serviceLine == 1)
                {
                    record.ImmigrationClosed = complete;
                }
                else
                {
                    record.RelocationClosed = complete;
                }

                _context.ServiceRecords.Update(record);
                _context.SaveChanges();
                isComplete = true;
            }

            return isComplete;
        }

        public int ReturnCoordinator(int sr, int serviceLine)
        {
            var coordinator = _context.ServiceRecords.Where(x => x.Id == sr).Select(s => new
            {
                user = serviceLine == 1
                    ? s.ImmigrationCoodinators.FirstOrDefault().Coordinator.UserId
                    : s.RelocationCoordinators.FirstOrDefault().Coordinator.UserId
            }).FirstOrDefault();
            return coordinator.user.Value;
        }

        public string ReturnService(int workOrderServices, int sr)
        {
            string isComplete = "";
            var service = _context.WorkOrderServices.Where(x => x.Id == workOrderServices).Select(s => new
            {
                description = s.StandaloneServiceWorkOrders.Any()
                    ? $"{s.StandaloneServiceWorkOrders.FirstOrDefault().Category.Category} / {s.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber}"
                    : $"{s.BundledServices.FirstOrDefault().Category.Category} / {s.BundledServices.FirstOrDefault().ServiceNumber}",
                workOrder = s.StandaloneServiceWorkOrders.Any()
                    ? s.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrderId.Value
                    : s.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrderId.Value
            }).FirstOrDefault();
            var statusBundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.Id == service.workOrder).Select(s => new
                {
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                StatusId = 0
                            }).FirstOrDefault(),
                }).ToList();
            var statusStandalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrderId == service.workOrder).Select(s => new
                {
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                StatusId = r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.StatusId
                            }).FirstOrDefault()
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                StatusId = r.StatusId.Value
                            }).FirstOrDefault()
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                StatusId = 0
                            }).FirstOrDefault(),
                }).ToList();
            var status = statusStandalone.Union(statusBundled).ToList();
            var statusServices = new int[] { 3, 4, 5 };
            if (status.Any(a => statusServices.Contains(a.status.StatusId)))
            {
                isComplete = "Work Order Complete";
            }
            return service.description + isComplete;
        }

        public biz.premier.Entities.ExperienceSurvey UpdateCustom(biz.premier.Entities.ExperienceSurvey experienceSurvey, int key)
        {
            if (experienceSurvey == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.ExperienceSurvey>()
                .Include(i => i.ExperienceSurveySuppliers)
                .SingleOrDefault(s => s.Id == key);
            if(exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(experienceSurvey);
                foreach (var i in experienceSurvey.ExperienceSurveySuppliers)
                {
                    var supplier = exist.ExperienceSurveySuppliers.FirstOrDefault(_i => _i.Id == i.Id);
                    if (supplier == null)
                    {
                        exist.ExperienceSurveySuppliers.Add(i);
                    }
                    else
                    {
                        _context.Entry(supplier).CurrentValues.SetValues(i);
                    }
                }
                exist.UpdatedDate = DateTime.Now;
                _context.SaveChanges();
            }
            return exist;
        }
    }
}
