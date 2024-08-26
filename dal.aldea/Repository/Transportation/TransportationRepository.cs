using biz.premier.Repository.Transportation;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Transportation
{
    public class TransportationRepository : GenericRepository<biz.premier.Entities.Transportation>, ITransportationRepository
    {
        public TransportationRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.Transportation GetCustom(int key)
        {
            var query = _context.Transportations
                .Include(i => i.DocumentTransportations)
                .Include(i => i.PaymentTransportations)
                .Include(i => i.ReminderTransportations)
                .Include(i => i.CommentTransportations)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.ExtensionTransportations)
                .Single(s => s.Id == key);
            return query;
        }

        public int get_id_country(int id_c)
        {
            int id_country = 0;
            if(id_c< 1000)// quiere decir que es pais de countries 
            {
                var country = _context.Countries.FirstOrDefault(c => c.Id == id_c);
                if(country!= null)
                {
                    var text_c = country.Name;
                    var cat_c = _context.CatCountries.FirstOrDefault(c => c.Name.ToLower() == text_c.ToLower());
                    if(cat_c != null)
                    {
                        id_country = cat_c.Id;
                    }
                }
            }

            return id_country;
        }

        public string get_city_name_by_countryid(int id_c)
        {
            string city = "No Encontrada";

                var obj_city = _context.CatCities.FirstOrDefault(c => c.IdCountry == id_c);
                if (obj_city != null)
                {
                    city = obj_city.City; 
                }

            return city;
        }

        public ActionResult GetTransportations(int applicatId, int service_order_id, int type_service)
        {

            type_service = get_id_country(type_service); //type es el id del país
            var city = get_city_name_by_countryid(type_service);

            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.CategoryId == 19 && x.DeliveredTo == applicatId && x.WorkOrderId == service_order_id && x.DeliveringIn == type_service)
                .Select(n => new
                {
                    n.ServiceNumber,
                    n.DeliveredToNavigation.Name,
                    n.DeliveredToNavigation.Relationship.Relationship,
                    n.WorkOrder.CreationDate,
                    n.Acceptance,
                    
                    country = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    // city = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCity.City,
                    city = city,
                    DocumentTransportations = _context.DocumentTransportations.Where(x => x.TransportationId == n.WorkOrderService.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderTransportations = _context.ReminderTransportations.Where(x => x.TransportationId == n.WorkOrderService.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentTransportations = _context.CommentTransportations.Where(x => x.TransportationId == n.WorkOrderService.Transportations.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    extensionTransportations = _context.ExtensionTransportations.Where(x => x.TransportationId == n.WorkOrderService.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    paymentTransportations = _context.PaymentTransportations.Where(x => x.TransportationId == n.WorkOrderService.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    transportService = _context.Transportations.Where(_x => _x.WorkOrderServicesId == n.WorkOrderServiceId)
                        //.Include(i => i.FamilyMemberTransportations)
                        .ToList()
                    //.Include(i => i.CommentTransportations).ThenInclude(i => i.User)
                    //n.WorkOrderService.Transportations
                }).ToList();


            var query = _context.BundledServices
                .Where(x => x.CategoryId == 19 && x.DeliveredTo == applicatId && x.BundledServiceOrder.WorkOrderId == service_order_id && x.DeliveringIn == type_service)
                .Select(n => new
                {
                    n.ServiceNumber,
                    n.DeliveredToNavigation.Name,
                    n.DeliveredToNavigation.Relationship.Relationship,
                    n.BundledServiceOrder.WorkOrder.CreationDate,
                    n.Acceptance,
                    country = n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    city = n.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.BundledServiceOrder.WorkOrder.ServiceRecordId).HostCity.City,
                    DocumentTransportations = _context.DocumentTransportations.Where(x => x.TransportationId == n.WorkServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderTransportations = _context.ReminderTransportations.Where(x => x.TransportationId == n.WorkServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentTransportations = _context.CommentTransportations.Where(x => x.TransportationId == n.WorkServices.Transportations.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    extensionTransportations = _context.ExtensionTransportations.Where(x => x.TransportationId == n.WorkServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    paymentTransportations = _context.PaymentTransportations.Where(x => x.TransportationId == n.WorkServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    transportService = _context.Transportations.Where(_x => _x.WorkOrderServicesId == n.WorkServicesId)
                        //.Include(i => i.FamilyMemberTransportations)
                        .ToList()
                    //_context.Transportations.Where(_x => n.WorkServices.Transportations.Contains(_x))
                    //.Include(i => i.CommentTransportations).ThenInclude(i => i.User)
                    //n.WorkServices.Transportations
                }).ToList();

            var union = new
            {
                standalone = standalone,
                bundle = query
            };

            return new ObjectResult(standalone.Union(query));
        }

        public ActionResult GetSingleTransportationById(int service_id, int country_id)
        {
            var Work_Order_Services_Id = _context.Transportations.FirstOrDefault(t => t.Id == service_id).WorkOrderServicesId;
           // country_id = get_id_country(country_id); // es el id del país
           // var city = get_city_name_by_countryid(country_id);

            var standalone = _context.Transportations.Where(s => s.WorkOrderServicesId == Work_Order_Services_Id)
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
                    //country = _context.CatCountries.FirstOrDefault(c=> c.Id == country_id).Name,
                    //country = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCountryNavigation.Name,
                    // city = n.WorkOrder.ServiceRecord.AssigneeInformations.SingleOrDefault(x => x.ServiceRecordId == n.WorkOrder.ServiceRecordId).HostCity.City,
                    //city = city,
                    DocumentTransportations = _context.DocumentTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    ReminderTransportations = _context.ReminderTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    CommentTransportations = _context.CommentTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).Include(i => i.User).ThenInclude(i => i.Role).ToList(),
                    extensionTransportations = _context.ExtensionTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    paymentTransportations = _context.PaymentTransportations.Where(x => x.TransportationId == n.WorkOrderServices.Transportations.Select(s => s.Id).FirstOrDefault()).ToList(),
                    transportPickups = _context.TransportPickups.Where(_x => _x.TransportationId == n.Id)
                        .Include(i => i.FamilyMemberTransportations)
                        .ToList()
                    //.Include(i => i.CommentTransportations).ThenInclude(i => i.User)
                    //n.WorkOrderService.Transportations
                }).ToList();
            
            if(standalone != null && standalone.Count > 0)
            {
                return new ObjectResult(standalone);
            }
            else
            {
                var query = _context.Transportations
                .Where(x => x.WorkOrderServicesId == Work_Order_Services_Id )
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


        public biz.premier.Entities.Transportation UpdateCustom(biz.premier.Entities.Transportation transportation, int key)
        {
            if (transportation == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Transportation>()
                .Include(i => i.CommentTransportations)
                .Include(i => i.TransportPickups)
                    .ThenInclude(i => i.FamilyMemberTransportations)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(transportation);
                foreach (var i in transportation.CommentTransportations)
                {
                    var comment = exist.CommentTransportations.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentTransportations.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }

                foreach (var i in transportation.TransportPickups)
                {
                    var _family = _context.FamilyMemberTransportations.Where(x => x.TransportService == i.Id).ToList();

                    foreach (var x in _family)
                    {
                        _context.FamilyMemberTransportations.Remove(x);
                        _context.SaveChanges();
                    }
                    foreach (var y in i.FamilyMemberTransportations)
                    {
                        if(y.TransportService > 0)
                            _context.FamilyMemberTransportations.Add(y);
                    }

                    _context.SaveChanges();
                }
            }
            UpdateStatusServiceRecord(exist.WorkOrderServicesId,exist.StatusId);
            return exist;
        }
    }
}
