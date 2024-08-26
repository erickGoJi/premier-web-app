using biz.premier.Repository.RequestAdditionalTime;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using biz.premier.Entities;

namespace dal.premier.Repository.RequestAdditionalTime
{
    public class RequestAdditionalTimeRepository : GenericRepository<biz.premier.Entities.RequestAdditionalTime>, IRequestAdditionalTimeRepository
    {
        public RequestAdditionalTimeRepository(Db_PremierContext context) : base(context) { }

        public Tuple<bool, string> ValidateAdditionalTime(List<biz.premier.Entities.RequestAdditionalTime> requestAdditionalTimes)
        {
            bool isValidate = true;
            string service = "";
            foreach (var i in requestAdditionalTimes)
            {
                var isPackage = _context.BundledServices.Include(_i => _i.BundledServiceOrder).SingleOrDefault(x => x.Id == i.Service);
                var isCoordination = _context.StandaloneServiceWorkOrders.SingleOrDefault(x => x.Id == i.Service);
                if (isPackage != null && isPackage.BundledServiceOrder.Package.Value == true)
                {
                    isValidate = false;
                    service = $"Service No Valid { isPackage.ServiceNumber }";
                    break;
                }
                else if (isCoordination != null && isCoordination.Coordination.Value) 
                {
                    isValidate = false;
                    service = $"Service No Valid { isCoordination.ServiceNumber }";
                    break;
                }

            }
            Tuple<bool, string> t = new Tuple<bool, string>(isValidate, service);
            return t;
        }

        public bool AddExtension(int workOrderServices, DateTime creation, int categoryId, int user, int time, int requested)
        {
            bool success = false;
            switch (categoryId)
            {
                case (12):
                    var pre_decision =
                        _context.PredecisionOrientations.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionPredecisionOrientations.Add(new ExtensionPredecisionOrientation()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Tine = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        PredecisionOrientationId = pre_decision.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (13):
                    var area =
                        _context.AreaOrientations.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionAreaOrientations.Add(new ExtensionAreaOrientation()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        AreaOrientationId = area.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (14):
                    var settling =
                        _context.SettlingIns.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionSettlingIns.Add(new ExtensionSettlingIn()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        SettlingInId = settling.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (15):
                    var schoolingSearch =
                        _context.SchoolingSearches.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionSchoolingSearches.Add(new ExtensionSchoolingSearch()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        SchoolingSearchId = schoolingSearch.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (16):
                    var departure =
                        _context.Departures.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionDepartures.Add(new ExtensionDeparture()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        DepartureId = departure.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (17):
                    var temporaryHousing =
                        _context.TemporaryHousingCoordinatons.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionTemporaryHousingCoordinatons.Add(new ExtensionTemporaryHousingCoordinaton()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        TemporaryHousingCoordinationId = temporaryHousing.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (18):
                    var rentalFurnitureCoordination =
                        _context.RentalFurnitureCoordinations.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionRentalFurnitureCoordinations.Add(new ExtensionRentalFurnitureCoordination()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        RentalFurnitureCoordinationId = rentalFurnitureCoordination.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (19):
                    var transportation =
                        _context.Transportations.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionTransportations.Add(new ExtensionTransportation()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        TransportationId = transportation.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (20):
                    var airportTransportationService =
                        _context.AirportTransportationServices.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionAirportTransportationServices.Add(new ExtensionAirportTransportationService()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        AirportTransportationServicesId = airportTransportationService.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                case (21):
                    var homeFinding =
                        _context.HomeFindings.FirstOrDefault(x =>
                            x.WorkOrderServicesId == workOrderServices);
                    _context.ExtensionHomeFindings.Add(new ExtensionHomeFinding()
                    {
                        AuthoAcceptanceDate = DateTime.Now,
                        Time = time,
                        AuthoDate = creation,
                        AuthorizedBy = user,
                        CreatedBy = user,
                        CreatedDate = DateTime.Now,
                        HomeFindingId = homeFinding.Id,
                        RequestedBy = requested,
                        Id = 0
                    });
                    break;
                // case (22):
                //     var leaseRenewal =
                //         _context.LeaseRenewals.FirstOrDefault(x =>
                //             x.WorkOrderServices == workOrderServices);
                //     _context.extension.Add(new ExtensionPredecisionOrientation()
                //     {
                //         AuthoAcceptanceDate = DateTime.Now,
                //         Tine = time,
                //         AuthoDate = creation,
                //         AuthorizedBy = user,
                //         CreatedBy = user,
                //         CreatedDate = DateTime.Now,
                //         PredecisionOrientationId = leaseRenewal.Id,
                //         RequestedBy = requested,
                //         Id = 0
                //     });
                //     break;
                default:
                    break;
            }

            _context.SaveChanges();
            return true;
        }
    }
}
