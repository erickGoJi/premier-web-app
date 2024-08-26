using biz.premier.Repository.HousingSpecification;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace dal.premier.Repository.HousingSpecification
{
    public class HousingSpecification : GenericRepository<biz.premier.Entities.HousingSpecification>, IHousingSpecification
    {
        public HousingSpecification(Db_PremierContext context) : base(context) { 
        
        }

        public biz.premier.Entities.HousingSpecification AddRelhousingAmenitie(biz.premier.Entities.HousingSpecification housingAmenitie)
        {
            //biz.premier.Entities.RelHousingAmenitie relHousingAmenitie = housingAmenitie;
            biz.premier.Entities.HousingSpecification _housingAmenitie = new biz.premier.Entities.HousingSpecification();
            _housingAmenitie.WorkOrderServices = housingAmenitie.WorkOrderServices;
            _housingAmenitie.TypeService = housingAmenitie.TypeService;
            _housingAmenitie.AreaInterest = housingAmenitie.AreaInterest;
            _housingAmenitie.PropertyTypeId = housingAmenitie.PropertyTypeId;
            _housingAmenitie.Bedroom = housingAmenitie.Bedroom;
            _housingAmenitie.Bathroom = housingAmenitie.Bathroom;
            _housingAmenitie.Size = housingAmenitie.Size;
            _housingAmenitie.MetricId = housingAmenitie.MetricId;
            _housingAmenitie.DesiredCommuteTime = housingAmenitie.DesiredCommuteTime;
            _housingAmenitie.Budget = housingAmenitie.Budget;
            _housingAmenitie.CurrencyId = housingAmenitie.CurrencyId;
            _housingAmenitie.ContractTypeId = housingAmenitie.ContractTypeId;
            _housingAmenitie.IntendedStartDate = housingAmenitie.IntendedStartDate;
            _housingAmenitie.AdditionalComments = housingAmenitie.AdditionalComments;
            _housingAmenitie.ParkingSpace = housingAmenitie.ParkingSpace;

            _context.HousingSpecifications.Add(_housingAmenitie);
            _context.SaveChanges();
            biz.premier.Entities.RelHousingAmenitie _relHousingAmenitie = new biz.premier.Entities.RelHousingAmenitie();
            foreach(var i in housingAmenitie.RelHousingAmenities)
            {
                if(_context.RelHousingAmenities.FirstOrDefault(x => x.AmenitieId == i.AmenitieId && x.HousingSpecificationId == i.HousingSpecificationId) == null)
                {
                    _relHousingAmenitie.AmenitieId = i.AmenitieId;
                    _relHousingAmenitie.HousingSpecificationId = _housingAmenitie.Id;

                    _context.RelHousingAmenities.Add(_relHousingAmenitie);
                    _context.SaveChanges();
                }
            }
            //for (var i = 0; i < housingAmenitie.RelHousingAmenities.Count; i++)
            //{
            //    if (_context.RelHousingAmenities.FirstOrDefault(x => x.AmenitieId == housingAmenitie.RelHousingAmenities[i].AmenitieId && x.HousingSpecificationId == housingAmenitie.RelHousingAmenities[i].HousingSpecificationId) == null) {
            //        _relHousingAmenitie.AmenitieId = housingAmenitie.RelHousingAmenities[i].AmenitieId;
            //        _relHousingAmenitie.HousingSpecificationId = _housingAmenitie.Id; 
            //            // housingAmenitie.RelHousingAmenities[i].HousingSpecificationId;

            //        _context.RelHousingAmenities.Add(_relHousingAmenitie);
            //        //_context.SaveChanges();
            //    }                
            //}

            
            return housingAmenitie;
        }

        public ActionResult GetHousingSpecification(int sr)
        {
            var housingsBundled = _context.HousingSpecifications
                .Where(x => x.WorkOrderServicesNavigation.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr)
                .Select(s => new
                {
                    s.Id,
                    s.WorkOrderServicesNavigation.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    s.WorkOrderServicesNavigation.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.NumberServiceRecord,
                    s.TypeServiceNavigation.TypeHousing,
                    s.TypeService,
                    s.WorkOrderServicesNavigation.BundledServices.FirstOrDefault().Service.Service,
                    s.CreatedDate,
                    country = _context.AssigneeInformations.Where(x => x.ServiceRecordId == sr).FirstOrDefault().HostCountryNavigation.Name,
                    city = _context.AssigneeInformations.Where(x => x.ServiceRecordId == sr).FirstOrDefault().HostCity.City,
                    s.ContractType.ContractType,
                    s.Budget,
                    s.WorkOrderServices
                });
            var housingsStandAlone = _context.HousingSpecifications
                .Where(x => x.WorkOrderServicesNavigation.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr)
                .Select(s => new
                {
                    s.Id,
                    s.WorkOrderServicesNavigation.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.NumberWorkOrder,
                    s.WorkOrderServicesNavigation.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.NumberServiceRecord,
                    s.TypeServiceNavigation.TypeHousing,
                    s.TypeService,
                    s.WorkOrderServicesNavigation.StandaloneServiceWorkOrders.FirstOrDefault().Service.Service,
                    s.CreatedDate,
                    country = _context.AssigneeInformations.Where(x => x.ServiceRecordId == sr).FirstOrDefault().HostCountryNavigation.Name,
                    city = _context.AssigneeInformations.Where(x => x.ServiceRecordId == sr).FirstOrDefault().HostCity.City,
                    s.ContractType.ContractType,
                    s.Budget,
                    s.WorkOrderServices
                }); ;
            var housings = housingsBundled.Union(housingsStandAlone);
            return new ObjectResult(housings);
        }


        public ActionResult GetHousingSpecificationBySR(int sr)
        {
            var housingsBundled = _context.HousingSpecifications
                .Include(a=> a.RelHousingAmenities)
                .Where(x => x.WorkOrderServicesNavigation.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr);

            var housingsStandAlone = _context.HousingSpecifications
                 .Include(a => a.RelHousingAmenities)
                .Where(x => x.WorkOrderServicesNavigation.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
           
            var housings = housingsBundled.Union(housingsStandAlone);
            return new ObjectResult(housings);
        }

        public int GetWorkOrderServiceId(int wos, int service)
        {
            int value = 0;
            var preDecisionStandalone = _context.PredecisionOrientations
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == wos);
            var preDecisionBundled = _context.PredecisionOrientations
                .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == wos).Union(preDecisionStandalone);
            if (preDecisionBundled.Any() && service == 1)
                value = preDecisionBundled.FirstOrDefault().WorkOrderServicesId;

            var homeFindingStandalone = _context.HomeFindings
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == wos);
            var homeFindingBundled = _context.HomeFindings
                .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == wos).Union(homeFindingStandalone);
            if (homeFindingBundled.Any() && service == 2)
                value = homeFindingBundled.FirstOrDefault().WorkOrderServicesId;

            var areaOrientationStandalone = _context.AreaOrientations
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == wos);
            var areaOrientationBundled = _context.AreaOrientations
                .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == wos).Union(areaOrientationStandalone);
            if (areaOrientationBundled.Any() && service == 3)
                value = areaOrientationBundled.FirstOrDefault().WorkOrderServicesId;

            return value;
        }

        public Tuple<string, string> GetWorkOrder(int wos, int service)
        {
            Tuple<string, string> value = new Tuple<string, string>("", "");
            bool is_alone = false;
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == wos)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId,
                    nwo = s.WorkOrder.NumberWorkOrder,
                    sn = s.ServiceNumber
                }).ToList();

            if (standalone.Count > 0)
            {
                 is_alone = true;
                return value = new Tuple<string, string>(standalone.FirstOrDefault().nwo, standalone.FirstOrDefault().sn);
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == wos).Include(s=> s.BundledServiceOrder)
                .Select(s => new
                {
                    country = s.DeliveringIn ,
                    type = s.ServiceId,
                    nwo = s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    sn = s.ServiceNumber

                }).ToList();

                if (bundled.Count > 0)
                {
                    is_alone = false;
                    return value = new Tuple<string, string>(bundled.FirstOrDefault().nwo, bundled.FirstOrDefault().sn);
                }
                else
                {
                    return value = new Tuple<string, string>("", "");
                }
            }


            
            var preDecisionStandalone = _context.PredecisionOrientations
                .Where(x => x.WorkOrderServicesId == wos)
                .Include(i => i.WorkOrderServices).ThenInclude(i => i.StandaloneServiceWorkOrders).ThenInclude(i => i.WorkOrder);
            if (preDecisionStandalone.Any() && service == 1)
                return value = new Tuple<string, string>(preDecisionStandalone.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.NumberWorkOrder,
                    preDecisionStandalone.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber);

            var preDecisionBundled = _context.PredecisionOrientations
                .Where(x => x.WorkOrderServicesId == wos)
                .Include(i => i.WorkOrderServices).ThenInclude(i => i.BundledServices).ThenInclude(i => i.BundledServiceOrder).ThenInclude(i => i.WorkOrder); 
            if (preDecisionBundled.Any() && service == 1)
                return value = new Tuple<string, string>(preDecisionBundled.FirstOrDefault().WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    preDecisionBundled.FirstOrDefault().WorkOrderServices.BundledServices.FirstOrDefault().ServiceNumber);

            if(is_alone)
            {
                var homeFindingStandalone = _context.HomeFindings
                                .Where(x => x.WorkOrderServicesId == wos)
                                .Include(i => i.WorkOrderServices).ThenInclude(i => i.StandaloneServiceWorkOrders).ThenInclude(i => i.WorkOrder);
                if (homeFindingStandalone.Any() && service == 2)
                    return value = new Tuple<string, string>(homeFindingStandalone.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.NumberWorkOrder,
                        homeFindingStandalone.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber);
            }
            else
            {
                var homeFindingBundled = _context.HomeFindings
                .Where(x => x.WorkOrderServicesId == wos)
                .Include(i => i.WorkOrderServices).ThenInclude(i => i.BundledServices).ThenInclude(i => i.BundledServiceOrder).ThenInclude(i => i.WorkOrder);
                if (homeFindingBundled.Any() && service == 2)
                    return value = new Tuple<string, string>(homeFindingBundled.FirstOrDefault().WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        homeFindingBundled.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber);
            }
            

            var areaOrientationStandalone = _context.AreaOrientations
                .Where(x => x.WorkOrderServicesId == wos)
                .Include(i => i.WorkOrderServices).ThenInclude(i => i.StandaloneServiceWorkOrders).ThenInclude(i => i.WorkOrder);
            if (areaOrientationStandalone.Any() && service == 3)
                return value = new Tuple<string, string>(areaOrientationStandalone.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.NumberWorkOrder,
                    areaOrientationStandalone.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber);

            var areaOrientationBundled = _context.AreaOrientations
                .Where(x => x.WorkOrderServicesId == wos)
                .Include(i => i.WorkOrderServices).ThenInclude(i => i.BundledServices).ThenInclude(i => i.BundledServiceOrder).ThenInclude(i => i.WorkOrder); ;
            if (areaOrientationBundled.Any() && service == 3)
                return value = new Tuple<string, string>(areaOrientationBundled.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.NumberWorkOrder,
                    areaOrientationBundled.FirstOrDefault().WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber);

            return new Tuple<string, string>("", "");
        }

        public biz.premier.Entities.HousingSpecification UpdateRelhousingAmenitie(biz.premier.Entities.HousingSpecification housingAmenitie)
        {
            if (housingAmenitie == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.HousingSpecification>()
                .Include(i => i.RelHousingAmenities)
                .SingleOrDefault(s => s.Id == housingAmenitie.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(housingAmenitie);
                foreach (var i in housingAmenitie.RelHousingAmenities)
                {
                    var amenitie = exist.RelHousingAmenities.SingleOrDefault(p => p.AmenitieId == i.AmenitieId);
                    if (amenitie == null)
                    {
                        exist.RelHousingAmenities.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(amenitie).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            return housingAmenitie;
        }
    }
}
