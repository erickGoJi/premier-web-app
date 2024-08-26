using biz.premier.Entities;
using biz.premier.Repository.AirportTransportationServices;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.AirportTransportationServices
{
    public class AirportTransportationServicesRepository : GenericRepository<AirportTransportationService>, IAirportTransportationServicesRepository
    {
        public AirportTransportationServicesRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetAirportTransportation(int applicatId, int service_order_id, int type_service)
        {
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.CategoryId == 20 && x.DeliveredTo == applicatId && x.WorkOrderId == service_order_id && x.DeliveringIn == type_service)
                .Select(n => new
                {
                    n.ServiceNumber,
                    n.DeliveredToNavigation.Name,
                    n.DeliveredToNavigation.Relationship.Relationship,
                    n.WorkOrder.CreationDate,
                    n.Acceptance,
                    country = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    city = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCity.City,
                    DocumentAirportTransportationServices = _context.DocumentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderService.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderAirportTransportationServices = _context.ReminderAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderService.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentAirportTransportationServices = _context.CommentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderService.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    ExtensionAirportTransportationServices = _context.ExtensionAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderService.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    PaymentAirportTransportationServices = _context.PaymentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderService.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    transportService = _context.AirportTransportationServices.Where(_x => _x.WorkOrderServicesId == n.WorkOrderServiceId)
                        //.Include(i => i.FamilyMemberTransportServices)
                        .ToList()
                }).ToList();
            var query = _context.BundledServices
                .Where(x => x.CategoryId == 20 && x.DeliveredTo == applicatId && x.BundledServiceOrder.WorkOrderId == service_order_id && x.DeliveringIn == type_service)
                .Select(n => new
                {
                    n.ServiceNumber,
                    n.DeliveredToNavigation.Name,
                    n.DeliveredToNavigation.Relationship.Relationship,
                    n.BundledServiceOrder.WorkOrder.CreationDate,
                    n.Acceptance,
                    country = n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    city = n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCity.City,
                    DocumentAirportTransportationServices = _context.DocumentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderAirportTransportationServices = _context.ReminderAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentAirportTransportationServices = _context.CommentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    ExtensionAirportTransportationServices = _context.ExtensionAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    PaymentAirportTransportationServices = _context.PaymentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    transportService = _context.AirportTransportationServices.Where(_x => _x.WorkOrderServicesId == n.WorkServicesId)
                        //.Include(i => i.FamilyMemberTransportServices)
                        .ToList()
                }).ToList();

            var union = new
            {
                standalone = standalone,
                bundle = query
            };

            return new ObjectResult(standalone.Union(query));
        }

        public ActionResult GetSingleAirportTransportationServicesById(int service_id)
        {
            var Work_Order_Services_Id = _context.AirportTransportationServices.FirstOrDefault(t => t.Id == service_id).WorkOrderServicesId;
            // country_id = get_id_country(country_id); // es el id del país
            // var city = get_city_name_by_countryid(country_id);

            var standalone = _context.AirportTransportationServices.Where(s => s.WorkOrderServicesId == Work_Order_Services_Id)
                .Select(n => new
                {
                    n.Id,
                    n.StatusId,
                    workOrderServicesId = n.WorkOrderServicesId,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Name,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrderServiceId,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().DeliveredToNavigation.Relationship.Relationship,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.CreationDate,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().Acceptance,
                    n.Budget,
                    n.TotalTimeAllowed,
                    n.TotalTimeAllowedId,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().Coordination,
                    n.ServiceCompletionDate,
                    //country = _context.CatCountries.FirstOrDefault(c=> c.Id == country_id).Name,
                    //country = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    // city = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCity.City,
                    //city = city,
                    DocumentAirportTransportationServices = _context.DocumentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderAirportTransportationServices = _context.ReminderAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentAirportTransportationServices = _context.CommentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    ExtensionAirportTransportationServices = _context.ExtensionAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    PaymentAirportTransportationServices = _context.PaymentAirportTransportationServices.Where(x => x.AirportTransportationServicesId == n.WorkOrderServices.AirportTransportationServices.Select(s => s.Id).FirstOrDefault()).ToList(),
                    AirportTransportPickup = _context.AirportTransportPickups.Where(_x => _x.TransportationId == n.Id)
                        .Include(i => i.FamilyMemberTransportServices)
                        .ToList()
                    //.Include(i => i.CommentTransportations).ThenInclude(i => i.User)
                    //n.WorkOrderService.Transportations
                }).ToList();

            if (standalone != null && standalone.Count > 0)
            {
                return new ObjectResult(standalone);
            }
            else
            {
                var query = _context.Transportations
                .Where(x => x.WorkOrderServicesId == Work_Order_Services_Id)
                .Select(n => new
                {
                    n.Id,
                    n.StatusId,
                    workOrderServicesId = n.WorkOrderServicesId,
                    n.WorkOrderServices.BundledServices.FirstOrDefault().ServiceNumber,
                    n.WorkOrderServices.BundledServices.FirstOrDefault().DeliveredToNavigation.Name,
                    n.WorkOrderServices.BundledServices.FirstOrDefault().DeliveredToNavigation.Relationship.Relationship,
                    n.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.CreationDate,
                    n.WorkOrderServices.BundledServices.FirstOrDefault().Acceptance,
                    n.Budget,
                    n.TotalTimeAllowed,
                    n.TotalTimeAllowedId,
                    n.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().Coordination,
                    // country = _context.CatCountries.FirstOrDefault(c => c.Id == country_id).Name,
                    // country = n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    // city = city,
                    //city = n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCity.City,
                    DocumentTransportations = _context.DocumentTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderTransportations = _context.ReminderTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentTransportations = _context.CommentTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    extensionTransportations = _context.ExtensionTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    paymentTransportations = _context.PaymentTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    transportPickups = _context.TransportPickups.Where(_x => _x.TransportationId == n.Id)
                        .Include(i => i.FamilyMemberTransportations)
                        .ToList()
                    //_context.Transportations.Where(_x => n.WorkServices.Transportations.Contains(_x))
                    //.Include(i => i.CommentTransportations).ThenInclude(i => i.User)
                    //n.WorkServices.Transportations
                }).ToList();
                return new ObjectResult(query);
            }
        }

        public AirportTransportationService GetCustom(int key)
        {
            var query = _context.AirportTransportationServices
                .Include(i => i.DocumentAirportTransportationServices)
                .Include(i => i.ReminderAirportTransportationServices)
                .Include(i => i.PaymentAirportTransportationServices)
                //.Include(i => i.FamilyMemberTransportServices)
                .Include(i => i.CommentAirportTransportationServices)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.ExtensionAirportTransportationServices)
                .Single(s => s.Id == key);
            return query;
        }

        public AirportTransportationService UpdateCustom(AirportTransportationService airportTransportationService, int key)
        {
            if (airportTransportationService == null)
                return null;

            var exist = _context.Set<biz.premier.Entities.AirportTransportationService>()
                .Include(i => i.CommentAirportTransportationServices)
                .Include(i => i.AirportTransportPickups)
                    .ThenInclude(i => i.FamilyMemberTransportServices)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(airportTransportationService);
                foreach (var i in airportTransportationService.CommentAirportTransportationServices)
                {
                    var comment = exist.CommentAirportTransportationServices.Where(x => x.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentAirportTransportationServices.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                foreach (var i in airportTransportationService.AirportTransportPickups)
                {
                    var _family = _context.FamilyMemberTransportServices.Where(x => x.TransportService == i.Id).ToList();

                    foreach (var x in _family)
                    {
                        _context.FamilyMemberTransportServices.Remove(x);
                        _context.SaveChanges();
                    }
                    foreach (var y in i.FamilyMemberTransportServices)
                    {
                        if (y.TransportService > 0)
                            _context.FamilyMemberTransportServices.Add(y);
                    }

                    i.TpAuthoDate = DateTime.Now;
                    i.TpAuthoAcceptanceDate = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            UpdateStatusServiceRecord(exist.WorkOrderServicesId, exist.StatusId);
            return exist;
        }
    }
}
