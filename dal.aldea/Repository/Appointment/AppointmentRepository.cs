using biz.premier.Entities;
using biz.premier.Repository.Appointment;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.Appointment
{
    public class AppointmentRepository : GenericRepository<biz.premier.Entities.Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetAppointmentByServiceRecordId(int service_record_id)
        {
            var service = _context.Appointments
                .Include(i => i.AppointmentWorkOrderServices)
                .ThenInclude(i => i.WorkOrderService)
                .Select(e => new
                {
                    e.Id,
                    e.ServiceRecordId,
                    e.Date,
                    supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Name,
                    e.Status,
                    StatusName = e.StatusNavigation.Status,
                    e.CommentCancel,
                    avatar_supplier = 
                    _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo == null ||
                    _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo == "" ?
                    "images/users/avatar.png" : _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo,
                    assignee = e.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    avatar_assignee = 
                    e.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo == null ?
                    "images/users/avatar.png" : e.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo,
                    servicio = e.AppointmentWorkOrderServices.Any() ? _context.AppointmentWorkOrderServices.Where(x => x.AppointmentId == e.Id)
                         .Select(k => new
                         {
                             id = k.WorkOrderService.StandaloneServiceWorkOrders.Any().Equals(false)
                                 ? k.WorkOrderService.BundledServices.SingleOrDefault().Id
                                 : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault().Id,
                             //serviceNumber = (k.WorkOrderService.StandaloneServiceWorkOrders.Any() 
                             //? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber
                             //: k.WorkOrderService.BundledServices.FirstOrDefault().ServiceNumber)
                             //+ "-" + 
                             //(_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                             //  && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             //? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             //: k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName == "--" 
                             //? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                             //  && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             //? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             //: k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).IdServiceNavigation.Service 
                             //: _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                             //  && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             //? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             //: k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName),
                             serviceNumber = 
                             (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName == "--"
                             ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).IdServiceNavigation.Service
                             : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName)
                         }).ToList() : null,
                    StartTime = e.StartTime,
                    StartTimeZone = _context.CatCities.FirstOrDefault(x => x.IdCountry == e.ToNavigation.ProfileUsers.FirstOrDefault().Country).IdTimeZoneNavigation.TimeZone,
                    EndTime = e.EndTime,
                    EndTimeZone = _context.CatCities.FirstOrDefault(x => x.IdCountry == e.ToNavigation.ProfileUsers.FirstOrDefault().Country).IdTimeZoneNavigation.TimeZone,
                    location = e.AppointmentWorkOrderServices.Select(s => s.WorkOrderService.BundledServices.Any(a => a.WorkServicesId == s.WorkOrderServiceId)
                            ? s.WorkOrderService.BundledServices.FirstOrDefault().Location
                            : s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().Location).FirstOrDefault(),
                    document = _context.DocumentAppointments.Count(x => x.AppointmentId == e.Id),
                    documentAppointments = _context.DocumentAppointments.Where(x => x.AppointmentId == e.Id).ToList(),
                    start = e.Start.HasValue ? e.Start.Value : false,
                    ended = e.Ended.HasValue ? e.Ended.Value : false,
                    e.Report,
                    e.To,
                    e.From
                }).Where(y => y.ServiceRecordId == service_record_id).ToList();


            return new ObjectResult(service);
        }

        private string GetTimeZone(int userId)
        {
            var  user = _context.ProfileUsers.FirstOrDefault(x => x.UserId == userId);
            var _time = _context.CatCities.FirstOrDefault(x => x.IdCountry == user.Country).IdTimeZoneNavigation.TimeZone;

            return _time;
        }

        public ActionResult GetAllAppointment(int service_record_id)
        {
            var service = _context.Appointments
                .Include(i => i.AppointmentWorkOrderServices)
                .ThenInclude(i => i.WorkOrderService)
                .Select(e => new
                {
                    e.Id,
                    e.ServiceRecordId,
                    e.Date,
                    supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Name,
                    e.Status,
                    statusName = _context.CatStatusAppointments.FirstOrDefault(x => x.Id == e.Status).Status,
                    e.CommentCancel,
                    avatar_supplier =
                    _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo == null ||
                    _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo == "" ?
                    "images/users/avatar.png" : _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo,
                    assignee = e.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    avatar_assignee =
                    e.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo == null ?
                    "images/users/avatar.png" : e.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo,
                    servicio = e.AppointmentWorkOrderServices.Any() ? _context.AppointmentWorkOrderServices.Where(x => x.AppointmentId == e.Id)
                         .Select(k => new
                         {
                             id = k.WorkOrderService.StandaloneServiceWorkOrders.Any().Equals(false)
                                 ? k.WorkOrderService.BundledServices.SingleOrDefault().Id
                                 : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault().Id,
                             serviceNumber = 
                             (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName == "--"
                             ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).IdServiceNavigation.Service
                             : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName),
                         }).ToList() : null,
                    e.StartTime,
                    e.EndTime,
                    location = e.AppointmentWorkOrderServices.Select(s => s.WorkOrderService.BundledServices.Any(a => a.WorkServicesId == s.WorkOrderServiceId)
                            ? s.WorkOrderService.BundledServices.FirstOrDefault().Location
                            : s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().Location).FirstOrDefault(),
                    document = _context.DocumentAppointments.Count(x => x.AppointmentId == e.Id),
                    documentAppointments = _context.DocumentAppointments.Where(x => x.AppointmentId == e.Id).ToList(),
                    start = e.Start.HasValue ? e.Start.Value : false,
                    ended = e.Ended.HasValue ? e.Ended.Value : false,
                    e.Report,
                    e.To,
                    e.From
                }).Where(y => y.ServiceRecordId == service_record_id).ToList();


            return new ObjectResult(service);
        }

        public ActionResult SelectCustom(int key)
        {
            var query = _context.Appointments
                .Where(x => x.Id == key)
                .Include(i => i.DocumentAppointments)
                .Include(i => i.AppointmentWorkOrderServices)
                .Select(n => new
                {
                    n.Id,
                    n.Status,
                    n.ServiceRecordId,
                    workOrder = _context.StandaloneServiceWorkOrders.Any(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId) ?
                    _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).WorkOrderId :
                    _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).BundledServiceOrder.WorkOrderId,
                    n.Date,
                    StartTime = n.StartTime.Value.ToString("HH:mm"),
                    EndTime = n.EndTime.Value.ToString("HH:mm"),
                    n.Description,
                    n.CommentCancel,
                    service_line = _context.StandaloneServiceWorkOrders.Any(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId)
                    ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).WorkOrder.ServiceLineId
                    : _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).BundledServiceOrder.WorkOrder.ServiceLineId,
                    n.CreatedBy,
                    n.CreatedDate,
                    n.UpdateBy,
                    n.UpdatedDate,
                    n.AppointmentWorkOrderServices,
                    n.DocumentAppointments,
                    supplier = _context.AssignedServiceSupliers.Select(b => new
                    {
                        b.ServiceOrderServicesId,
                        Supplier = b.RelocationSupplierPartner.Supplier.Name == null ? b.ImmigrationSupplierPartner.Supplier.Name : b.RelocationSupplierPartner.Supplier.Name,
                        avatar = "/Files/Pets/2d0a3d38-a34e-4b80-b018-683d4e24bce9.jpg",
                        Location = (_context.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == n.Id).WorkOrderServiceId) == null
                                    ? _context.BundledServices.SingleOrDefault(x => x.WorkServicesId == n.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == n.Id).WorkOrderServiceId).Location
                                    : _context.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == n.Id).WorkOrderServiceId).Location) == null ? "N/A" : ""
                    }).Where(g => g.ServiceOrderServicesId == n.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == n.Id).WorkOrderServiceId).ToList(),
                    services = n.AppointmentWorkOrderServices
                    .Select(k => new
                    {
                        id = k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId) == null
                         ? k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).Id
                         : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).Id,
                        //idService = k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId) == null
                        // ? k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).ServiceId
                        // : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).ServiceId,
                        HomeFindingId = _context.HomeFindings.FirstOrDefault(y => y.WorkOrderServicesId == k.WorkOrderServiceId).Id,
                        SchoollingId = _context.SchoolingSearches.FirstOrDefault(y => y.WorkOrderServicesId == k.WorkOrderServiceId).Id,
                        serviceNumber = k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId) == null
                         ? k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).ServiceNumber
                         + "/" + k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).Service.Service
                         : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).ServiceNumber
                         + "/" +
                         k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).Service.Service
                         ,k.WorkOrderServiceId
                         ,workOrder = _context.StandaloneServiceWorkOrders.Any(x => x.WorkOrderServiceId == k.WorkOrderServiceId) ?
                                         _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).WorkOrderId :
                                         _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).BundledServiceOrder.WorkOrderId
                    }).ToList(),
                    n.To,
                    n.From
                }).ToList();
            return new ObjectResult(query);
        }

        public ActionResult GetAppointmentByIdApp(int key)
        {
            var query = _context.Appointments
                .Where(x => x.Id == key)
                .Include(i => i.DocumentAppointments)
                .Include(i => i.AppointmentWorkOrderServices)
                .Select(n => new
                {
                    n.Id,
                    n.Status,
                    n.ServiceRecordId,
                    workOrder = _context.StandaloneServiceWorkOrders.Any(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId) ?
                    _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).WorkOrderId :
                    _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).BundledServiceOrder.WorkOrderId,
                    n.Date,
                    StartTime = n.StartTime.Value.ToString("HH:mm"),
                    EndTime = n.EndTime.Value.ToString("HH:mm"),
                    n.Description,
                    n.CommentCancel,
                    service_line = _context.StandaloneServiceWorkOrders.Any(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId)
                    ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).WorkOrder.ServiceLineId
                    : _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == n.AppointmentWorkOrderServices.FirstOrDefault().WorkOrderServiceId).BundledServiceOrder.WorkOrder.ServiceLineId,
                    n.CreatedBy,
                    n.CreatedDate,
                    n.UpdateBy,
                    n.UpdatedDate,
                    n.AppointmentWorkOrderServices,
                    DocumentAppointments = n.DocumentAppointments.Count(),
                    services = n.AppointmentWorkOrderServices
                    .Select(k => new
                    {
                        id = k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId) == null
                         ? k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).Id
                         : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).Id,
                        //idService = k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId) == null
                        // ? k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).ServiceId
                        // : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).ServiceId,
                        HomeFinding = _context.HomeFindings.Where(y => y.WorkOrderServicesId == k.WorkOrderServiceId).Count(),
                        Schoolling = _context.SchoolingSearches.Where(y => y.WorkOrderServicesId == k.WorkOrderServiceId).Count(),
                        HomeFindingId = _context.HomeFindings.FirstOrDefault(y => y.WorkOrderServicesId == k.WorkOrderServiceId).Id,
                        SchoollingId = _context.SchoolingSearches.FirstOrDefault(y => y.WorkOrderServicesId == k.WorkOrderServiceId).Id,
                        serviceNumber = k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId) == null
                         ? k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).ServiceNumber
                         + "/" + k.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).Service.Service
                         : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).ServiceNumber
                         + "/" +
                         k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).Service.Service,
                        k.WorkOrderServiceId,
                        workOrder = _context.StandaloneServiceWorkOrders.Any(x => x.WorkOrderServiceId == k.WorkOrderServiceId) ?
                                         _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == k.WorkOrderServiceId).WorkOrderId :
                                         _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == k.WorkOrderServiceId).BundledServiceOrder.WorkOrderId
                    }).ToList(),
                    n.To,
                    ToNavigation = n.ToNavigation.ProfileUsers
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        Photo = d.Photo == null || d.Photo == "" ? "Files/assets/avatar.png" : d.Photo,
                        d.PhoneNumber,
                        d.VehicleConsultants
                    }),
                    n.From
                }).ToList();
            return new ObjectResult(query);
        }

        public ActionResult GetCountHousingSchoolAppointment(List<int> servicesRelated, DateTime appointmentDate)
        {

            var housingStand = _context.StandaloneServiceWorkOrders
                .Where(x => servicesRelated.Contains(x.WorkOrderServiceId.Value))
                .Join(_context.HomeFindings, a => a.WorkOrderServiceId, b => b.WorkOrderServicesId,
                        (a, b) => new
                        {
                            b.Id,
                            b.SupplierPartnerNavigation.ComercialName,
                            b.AuthoDate
                        })
                .Join(_context.HousingLists, c => c.Id, d => d.IdServiceDetail,
                        (c, d) => new
                        {
                            c.Id,
                            c.ComercialName,
                            c.AuthoDate,
                            d.VisitDate,
                            d.Address,
                            d.Zip,
                            d.WebSite,
                            d.Price,
                            d.Neighborhood

                        }).Where(j => j.VisitDate.Value.Date == appointmentDate.Date)
                .ToList();

            var housingBundle = _context.BundledServices
                .Where(x => servicesRelated.Contains(x.WorkServicesId.Value))
                .Join(_context.HomeFindings, a => a.WorkServicesId, b => b.WorkOrderServicesId,
                        (a, b) => new
                        {
                            b.Id,
                            b.SupplierPartnerNavigation.ComercialName,
                            b.AuthoDate
                        })
                .Join(_context.HousingLists, c => c.Id, d => d.IdServiceDetail,
                        (c, d) => new
                        {
                            c.Id,
                            c.ComercialName,
                            c.AuthoDate,
                            d.VisitDate,
                            d.Address,
                            d.Zip,
                            d.WebSite,
                            d.Price,
                            d.Neighborhood
                        }).Where(j => j.VisitDate.Value.Date == appointmentDate.Date)
                .ToList();

            var schoolingStand = _context.StandaloneServiceWorkOrders
                .Where(x => servicesRelated.Contains(x.WorkOrderServiceId.Value))
                .Join(_context.SchoolingSearches, a => a.WorkOrderServiceId, b => b.WorkOrderServicesId,
                        (a, b) => new
                        {
                            b.Id,
                            b.AuthoDate
                        })
                .Join(_context.SchoolsLists, c => c.Id, d => d.SchoolingSearchId,
                        (c, d) => new
                        {
                            c.Id,
                            c.AuthoDate,
                            d.VisitDate,
                            d.Address,
                            d.WebSite,
                            name = d.SupplierId == null ? "No information" : _context.SupplierPartnerProfileServices.FirstOrDefault(i =>  i.Id == d.SupplierId).ComercialName,
                            dependent = String.IsNullOrEmpty(d.DependentNavigation.Name) ? "No information" : d.DependentNavigation.Name
                        }).Where(j => j.VisitDate.Value.Date == appointmentDate.Date)
                .ToList();

            var schoolingBundle = _context.BundledServices
                .Where(x => servicesRelated.Contains(x.WorkServicesId.Value))
                .Join(_context.SchoolingSearches, a => a.WorkServicesId, b => b.WorkOrderServicesId,
                        (a, b) => new
                        {
                            b.Id,
                            b.AuthoDate
                        })
                .Join(_context.SchoolsLists, c => c.Id, d => d.SchoolingSearchId,
                        (c, d) => new
                        {
                            c.Id,
                            c.AuthoDate,
                            d.VisitDate,
                            d.Address,
                            d.WebSite,
                            name = d.SupplierId == null ? "No information" : _context.SupplierPartnerProfileServices.FirstOrDefault(i => i.Id == d.SupplierId).ComercialName,
                            dependent = String.IsNullOrEmpty(d.DependentNavigation.Name) ? "No information" : d.DependentNavigation.Name
                        }).Where(j => j.VisitDate.Value.Date == appointmentDate.Date)
                .ToList();
            return new ObjectResult(new { housing = housingStand.Union(housingBundle), schoolin = schoolingStand.Union(schoolingBundle) });
        }

        public ActionResult AddAppointmentHousing(int appointmentId, int housingId, bool action)
        {
            var query = _context.RelAppointmentHousingLists.FirstOrDefault(x => x.AppointmentId == appointmentId && x.HousingListId == housingId);
            RelAppointmentHousingList rel = new RelAppointmentHousingList();

            if (query == null)
            {
                if(action)
                {
                    rel.AppointmentId = appointmentId;
                    rel.HousingListId = housingId;

                    _context.RelAppointmentHousingLists.Add(rel);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (!action)
                {
                    _context.RelAppointmentHousingLists.Remove(query);
                    _context.SaveChanges();
                }
            }

            return new ObjectResult(query);
        }

        public ActionResult AddAppointmentSchooling(int appointmentId, int schoolingId, bool action)
        {
            var query = _context.RelAppointmentSchoolingLists.FirstOrDefault(x => x.AppointmentId == appointmentId && x.SchoolingListId == schoolingId);
            RelAppointmentSchoolingList rel = new RelAppointmentSchoolingList();

            if (query == null)
            {
                if (action)
                {
                    rel.AppointmentId = appointmentId;
                    rel.SchoolingListId = schoolingId;

                    _context.RelAppointmentSchoolingLists.Add(rel);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (!action)
                {
                    _context.RelAppointmentSchoolingLists.Remove(query);
                    _context.SaveChanges();
                }
            }

            return new ObjectResult(query);
        }

        public biz.premier.Entities.Appointment UpdateCustom(biz.premier.Entities.Appointment appointment)
        {
            var find = _context.AppointmentWorkOrderServices.Where(x => x.AppointmentId == appointment.Id).ToList();
            if (find != null)
            {
                foreach (var aws in find)
                {
                    _context.AppointmentWorkOrderServices.Remove(aws);
                    _context.SaveChanges();
                }
            }

            var consult = _context.Set<biz.premier.Entities.Appointment>()
                .Include(i => i.DocumentAppointments)
                .Single(x => x.Id == appointment.Id);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(appointment);
                foreach (var da in appointment.DocumentAppointments)
                {
                    var exist = consult.DocumentAppointments.Where(w => w.Id == da.Id).FirstOrDefault();
                    if (exist == null)
                    {
                        da.CreatedDate = DateTime.Now;
                        consult.DocumentAppointments.Add(da);
                    }
                    else
                    {
                        _context.Entry(exist).CurrentValues.SetValues(da);
                    }
                }
                
                foreach (var da in appointment.AppointmentWorkOrderServices)
                {
                    var exist = consult.AppointmentWorkOrderServices.Where(w => w.Id == da.Id).FirstOrDefault();
                    //_context.AppointmentWorkOrderServices.Remove(exist);
                    //_context.SaveChanges();
                    if (exist == null)
                    {
                        da.Id = 0;
                        _context.AppointmentWorkOrderServices.Add(da);
                        //consult.AppointmentWorkOrderServices.Add(da);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(exist).CurrentValues.SetValues(da);
                    }
                }
                _context.SaveChanges();
            }
            return appointment;
        }

        //App
        public ActionResult GetAppointmentByAssigneeId(int assigneeId)
        {
            //int count = 0;
            var service = _context.Appointments
                 .Select(e => new
                 {
                     //e.AppointmentWorkOrderServices.FirstOrDefault(x => x.AppointmentId == e.Id).WorkOrderServiceId,
                     e.Id,
                     userId = e.ServiceRecord.AssigneeInformations.FirstOrDefault(f => f.ServiceRecordId == e.ServiceRecordId).UserId,
                     assigneeId = e.ServiceRecord.AssigneeInformations.FirstOrDefault(f => f.ServiceRecordId == e.ServiceRecordId).Id,
                     e.ServiceRecordId,
                     //workOrderId = _context.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).WorkOrder == null
                     //? _context.BundledServices.SingleOrDefault(x => x.WorkServicesId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).BundledServiceOrder.WorkOrder.NumberWorkOrder
                     //: _context.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).WorkOrder.NumberWorkOrder,
                     e.Date,
                     supplier = _context.AssignedServiceSupliers.SingleOrDefault(x => x.ServiceOrderServicesId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).ImmigrationSupplierPartnerId == null
                     ? _context.AssignedServiceSupliers.SingleOrDefault(x => x.ServiceOrderServicesId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).RelocationSupplierPartner.Supplier.Name
                     : _context.AssignedServiceSupliers.SingleOrDefault(x => x.ServiceOrderServicesId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).ImmigrationSupplierPartner.Supplier.Name,
                     avatar_supplier = e.ServiceRecord.AssigneeInformations.FirstOrDefault(f => f.ServiceRecordId == e.ServiceRecordId).Photo,
                     servicios = e.AppointmentWorkOrderServices.Where(x => x.AppointmentId == e.Id).Select(d => new
                     {
                         id = d.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == d.WorkOrderServiceId) == null
                                    ? d.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == d.WorkOrderServiceId).Service.Id
                                    : d.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == d.WorkOrderServiceId).Service.Id,
                         servicio = d.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == d.WorkOrderServiceId) == null
                                    ? d.WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == d.WorkOrderServiceId).Service.Service
                                    : d.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == d.WorkOrderServiceId).Service.Service
                     }),
                     e.StartTime,
                     e.EndTime,
                     //location = e.AppointmentWorkOrderServices.FirstOrDefault(x => x.AppointmentId == e.Id).WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId) == null
                     //? e.AppointmentWorkOrderServices.FirstOrDefault(x => x.AppointmentId == e.Id).WorkOrderService.BundledServices.SingleOrDefault(x => x.WorkServicesId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).Location
                     //: e.AppointmentWorkOrderServices.FirstOrDefault(x => x.AppointmentId == e.Id).WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault(x => x.WorkOrderServiceId == e.AppointmentWorkOrderServices.FirstOrDefault(r => r.AppointmentId == e.Id).WorkOrderServiceId).Location,
                     document = _context.DocumentAppointments.Where(x => x.AppointmentId == e.Id).Count(),
                     e.Status
                 }).Where(y => y.userId == assigneeId).ToList();


            return new ObjectResult(service);
        }
        // WEB APP
        public ActionResult GetAppointmentByUser(int userId, int? serviceRecordId, int? status, DateTime? dateRange1, DateTime? dateRange2)
        {
            var appointments = _context.Appointments.Where(x => x.To == userId || x.From == userId).Select(s => new
            {
                s.Id,
                s.Date,
                DateView = s.Date.ToString("MM/dd/yyyy"),
                s.ServiceRecordId,
                s.Description,
                s.ServiceRecord.NumberServiceRecord,
                s.ServiceRecord.AssigneeInformations,
                s.ServiceRecord.Partner,
                s.Status,
                StatusId = s.Status.Value,
                StartTimeView = s.StartTime.Value.ToShortTimeString(),
                EndTimeView = s.EndTime.Value.ToShortTimeString(),
                s.StartTime,
                s.EndTime,
                mostrar_boton = s.Date.Date == DateTime.Now.Date ? true : false,
                s.AppointmentWorkOrderServices,
                s.DocumentAppointments,
                DocumentAppointmentsCount = s.DocumentAppointments.Count(),
                avatar_assignee = s.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo == null
                     ? "images/users/avatar.png"
                     : s.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo,
                avatar_consultor = s.ServiceRecord.ImmigrationSupplierPartners.Any() ? _context.ProfileUsers
                         .FirstOrDefault(x => x.Id == s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(k => k.ServiceRecordId == s.Id).SupplierId)
                         .Photo
                 : s.ServiceRecord.RelocationSupplierPartners.Any() ? _context.ProfileUsers
                     .FirstOrDefault(x => x.Id == s.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(k => k.ServiceRecordId == s.Id).SupplierId)
                     .Photo
                 : "images/users/avatar.png",
                city = s.ServiceRecord.ImmigrationSupplierPartners.Any()
                 ? s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.CityNavigation.City
                 : s.ServiceRecord.RelocationSupplierPartners.Any() ? s.ServiceRecord.RelocationSupplierPartners
                         .FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.CityNavigation.City
                 : "N/A",
                consultor = s.ServiceRecord.ImmigrationSupplierPartners.Any()
                     ? s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name + " " + s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName
                     : s.ServiceRecord.RelocationSupplierPartners.Any()
                         ? s.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.Name + " " + s.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.LastName
                         : "N/A",
                coordinador = s.ServiceRecord.ImmigrationCoodinators.Any()
                 ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name + " " + s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.LastName
                 : s.ServiceRecord.RelocationCoordinators.Any() ? s.ServiceRecord.RelocationCoordinators.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Coordinator.Name + " " + s.ServiceRecord.RelocationCoordinators.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Coordinator.LastName
                 : "N/A",
                location = s.ServiceRecord.ImmigrationSupplierPartners.Any()
                     ? s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCity.Name + ", " + s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                     : s.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCity.City + ", " + s.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                serviceName = s.AppointmentWorkOrderServices.Where(x => x.AppointmentId == s.Id).Select(_s => new
                {
                    service = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().Service.Service
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().Service.Service,
                    NumberService = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().ServiceNumber,
                    ServiceLine = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLine.ServiceLine
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
                }).ToList(),
                start = s.Start.HasValue ? s.Start.Value : false,
                ended = s.Ended.HasValue ? s.Ended.Value : false,
                s.Report,
                s.To,
                s.From
            }).OrderByDescending(x => x.Date).ToList();
            
            if(serviceRecordId != null)
            {
                appointments = appointments.Where(x => x.ServiceRecordId == serviceRecordId).ToList();
            }

            if(status != null)
            {
                appointments = appointments.Where(x => x.StatusId == status).ToList();
            }
            else // muiestra los activos por default
            {
                appointments = appointments.Where(x => x.StatusId == 1).ToList();
            }

            if (dateRange1.HasValue && dateRange2.HasValue)
            {
                appointments = appointments.Where(x => x.Date > dateRange1.Value && x.Date < dateRange2.Value).ToList();
            }

            // appointments = appointments.Where(w => w.Date >= DateTime.Now.AddDays(-5)).ToList(); estp no tiene sentipo por que choca con el filtro de fechas

            appointments.Take(10); 

            return new ObjectResult(appointments);
        }

        public ActionResult GetAppointmentByAssignee(int userId, int? status, DateTime? dateRange1, DateTime? dateRange2)
        {
            var sr = _context.ServiceRecords
                .Include(x => x.WorkOrders)
                .Include(x => x.AssigneeInformations)
                .SingleOrDefault(x => x.AssigneeInformations.FirstOrDefault().UserId == userId);

            var appointments = _context.Appointments.Where(x => x.ServiceRecordId == sr.Id).Select(s => new
            {
                s.Id,
                s.Date,
                DateView = s.Date.ToString("MM/dd/yyyy"),
                s.ServiceRecordId,
                s.Description,
                s.ServiceRecord.NumberServiceRecord,
                s.ServiceRecord.AssigneeInformations,
                s.ServiceRecord.Partner,
                s.Status,
                StatusId = s.Status.Value,
                StartTimeView = s.StartTime.Value.ToShortTimeString(),
                EndTimeView = s.EndTime.Value.ToShortTimeString(),
                s.StartTime,
                s.EndTime,
                mostrar_boton = s.Date.Date == DateTime.Now.Date ? true : false,
                s.AppointmentWorkOrderServices,
                s.DocumentAppointments,
                DocumentAppointmentsCount = s.DocumentAppointments.Count(),
                avatar_assignee = s.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo == null
                     ? "images/users/avatar.png"
                     : s.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo,
                avatar_consultor = s.ServiceRecord.ImmigrationSupplierPartners.Any() ? _context.ProfileUsers
                         .FirstOrDefault(x => x.Id == s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(k => k.ServiceRecordId == s.Id).SupplierId)
                         .Photo
                 : s.ServiceRecord.RelocationSupplierPartners.Any() ? _context.ProfileUsers
                     .FirstOrDefault(x => x.Id == s.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(k => k.ServiceRecordId == s.Id).SupplierId)
                     .Photo
                 : "images/users/avatar.png",
                city = s.ServiceRecord.ImmigrationSupplierPartners.Any()
                 ? s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.CityNavigation.City
                 : s.ServiceRecord.RelocationSupplierPartners.Any() ? s.ServiceRecord.RelocationSupplierPartners
                         .FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.CityNavigation.City
                 : "N/A",
                consultor = s.ServiceRecord.ImmigrationSupplierPartners.Any()
                     ? s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name + " " + s.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName
                     : s.ServiceRecord.RelocationSupplierPartners.Any()
                         ? s.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.Name + " " + s.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Supplier.LastName
                         : "N/A",
                coordinador = s.ServiceRecord.ImmigrationCoodinators.Any()
                 ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name + " " + s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.LastName
                 : s.ServiceRecord.RelocationCoordinators.Any() ? s.ServiceRecord.RelocationCoordinators.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Coordinator.Name + " " + s.ServiceRecord.RelocationCoordinators.FirstOrDefault(x => x.ServiceRecordId == s.ServiceRecordId).Coordinator.LastName
                 : "N/A",
                location = s.ServiceRecord.ImmigrationSupplierPartners.Any()
                     ? s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCity.Name + ", " + s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                     : s.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCity.City + ", " + s.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                serviceName = s.AppointmentWorkOrderServices.Where(x => x.AppointmentId == s.Id).Select(_s => new
                {
                    service = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().Service.Service
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().Service.Service,
                    NumberService = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().ServiceNumber,
                    ServiceLine = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLine.ServiceLine
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
                }).ToList(),
                start = s.Start.HasValue ? s.Start.Value : false,
                ended = s.Ended.HasValue ? s.Ended.Value : false,
                s.Report,
                s.To,
                s.From
            }).OrderByDescending(x => x.Date).ToList();

            if (status != null)
            {
                appointments = appointments.Where(x => x.StatusId == status).ToList();
            }
            else // muiestra los activos por default
            {
                appointments = appointments.Where(x => x.StatusId == 1).ToList();
            }

            if (dateRange1.HasValue && dateRange2.HasValue)
            {
                appointments = appointments.Where(x => x.Date > dateRange1.Value && x.Date < dateRange2.Value).ToList();
            }

            // appointments = appointments.Where(w => w.Date >= DateTime.Now.AddDays(-5)).ToList(); estp no tiene sentipo por que choca con el filtro de fechas

            appointments.Take(10);

            return new ObjectResult(appointments);
        }


        public biz.premier.Entities.ReportDay AddReport(int appointment, int user)
        {
            var bundled = _context.BundledServices.Include(i => i.BundledServiceOrder).ThenInclude(i => i.WorkOrder).ToList();
            var standalone = _context.StandaloneServiceWorkOrders.Include(i => i.WorkOrder).ToList();
            var servicesAppointments = _context.AppointmentWorkOrderServices.Where(x => x.AppointmentId == appointment).ToList();
            var servicesBundled = bundled.Where(x => servicesAppointments.Select(s => s.WorkOrderServiceId).Contains(x.WorkServicesId.Value))
                .Select(service => new
                {
                    service = service.Id,
                    workOrder = service.BundledServiceOrder.WorkOrderId,
                    ServiceRecord = service.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    serviceLine = service.BundledServiceOrder.WorkOrder.ServiceLineId
                }).ToList();
            var servicesStandalone = standalone.Where(x => servicesAppointments.Select(s => s.WorkOrderServiceId).Contains(x.WorkOrderServiceId.Value))
                .Select(service => new
                {
                    service = service.Id,
                    workOrder = service.WorkOrderId,
                    ServiceRecord = service.WorkOrder.ServiceRecordId,
                    serviceLine = service.WorkOrder.ServiceLineId
                }).ToList();
            var services = servicesBundled.Union(servicesStandalone).ToList();
            services = services.Where(x => x.service != 0).ToList();
            biz.premier.Entities.ReportDay reportDay = new biz.premier.Entities.ReportDay();
            reportDay.Id = 0;
            reportDay.ReportBy = user;
            reportDay.CreationDate = DateTime.Now;
            reportDay.CreatedBy = user;
            reportDay.CreatedDate = DateTime.Now;
            reportDay.WorkOrder = services.FirstOrDefault()?.workOrder.Value;
            reportDay.StartTime = DateTime.Now.TimeOfDay.ToString();
            reportDay.ReportNo = _context.ReportDays.Count(c =>
                c.WorkOrderNavigation.ServiceRecordId == (services.Count() == 0 ? 0 : services.FirstOrDefault().ServiceRecord.Value));
            reportDay.ServiceLine = services.FirstOrDefault()?.serviceLine.Value;
            reportDay.ServiceReportDays = new List<ServiceReportDay>();
            foreach (var service in services)
            {
                reportDay.ServiceReportDays.Add(new ServiceReportDay()
                {
                    Id = 0,
                    Service = service.service,
                    CreatedBy = user,
                    CreatedDate = DateTime.Now,
                    ReportDayId = 0
                });
            }

            _context.ReportDays.Add(reportDay);
            _context.SaveChanges();

            var appointmentFind = _context.Appointments.FirstOrDefault(x => x.Id == appointment);
            appointmentFind.Report = reportDay.Id;
            appointmentFind.Start = true;
            appointmentFind.Ended = false;
            _context.Appointments.Update(appointmentFind);
            _context.SaveChanges();

            return reportDay;
        }

        public biz.premier.Entities.ReportDay UpdateReport(int report, int appointment, int user)
        {
            biz.premier.Entities.ReportDay reportDay = _context.ReportDays.Include(i => i.ServiceReportDays).FirstOrDefault(x => x.Id == report);
            reportDay.ReportDate = DateTime.Now;
            reportDay.UpdateBy = user;
            reportDay.UpdatedDate = DateTime.Now;
            reportDay.EndTime = DateTime.Now.TimeOfDay.ToString();
            TimeSpan ts = TimeSpan.Parse(reportDay.StartTime);
            int totalTime = TimeSpan.Compare(DateTime.Now.TimeOfDay, ts);
            reportDay.TotalTime = totalTime.ToString();
            int parts = totalTime / reportDay.ServiceReportDays.Count;
            foreach (var service in reportDay.ServiceReportDays)
            {
                service.Time = parts.ToString();
                service.UpdateBy = user;
                service.UpdatedDate = DateTime.Now;
                _context.ServiceReportDays.Update(service);
            }

            _context.ReportDays.Update(reportDay);
            _context.SaveChanges();

            var appointmentFind = _context.Appointments.FirstOrDefault(x => x.Id == appointment);
            appointmentFind.Start = true;
            appointmentFind.Ended = true;
            appointmentFind.Status = 2;
            _context.Appointments.Update(appointmentFind);
            _context.SaveChanges();

            return reportDay;
        }

        public async Task<ActionResult> GetServicesByServiceRecord(int sr)
        {
            var bundled = await _context.BundledServices
                .Include(i => i.BundledServiceOrder)
                .ThenInclude(i => i.WorkOrder)
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId.Equals(sr))
                .Select(s => new
                {
                    id = s.WorkServicesId,
                    serviceNumber = $"{s.ServiceNumber} - {s.Category.Category}",
                })
                .ToListAsync();
            var standalone = await _context.StandaloneServiceWorkOrders
                .Include(i => i.WorkOrder)
                .Where(x => x.WorkOrder.ServiceRecordId.Equals(sr))
                .Select(s => new
                {
                    id = s.WorkOrderServiceId,
                    serviceNumber = $"{s.ServiceNumber} - {s.Category.Category}"
                })
                .ToListAsync();
            var services = standalone.Union(bundled).ToList();
            return new ObjectResult(services);

        }

        public async Task<ActionResult> GetServicesByServiceRecordAndServiceLine(int sr, int sl)
        {
            Object _object= new Object();
            if (sl == 3)
            {
                var bundled = await _context.BundledServices
               .Include(i => i.BundledServiceOrder)
               .ThenInclude(i => i.WorkOrder)
               .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId.Equals(sr))
               .Select(s => new
               {
                   id = s.WorkServicesId,
                   serviceNumber = $"{s.ServiceNumber} - {s.Category.Category}",
               })
               .ToListAsync();
                var standalone = await _context.StandaloneServiceWorkOrders
                    .Include(i => i.WorkOrder)
                    .Where(x => x.WorkOrder.ServiceRecordId.Equals(sr))
                    .Select(s => new
                    {
                        id = s.WorkOrderServiceId,
                        serviceNumber = $"{s.ServiceNumber} - {s.Category.Category}"
                    })
                    .ToListAsync();
                _object = standalone.Union(bundled).ToList();
            }
            else
            {
                var bundled = await _context.BundledServices
                .Include(i => i.BundledServiceOrder)
                .ThenInclude(i => i.WorkOrder)
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId.Equals(sr) && x.BundledServiceOrder.WorkOrder.ServiceLineId.Equals(sl))
                .Select(s => new
                {
                    id = s.WorkServicesId,
                    serviceNumber = $"{s.ServiceNumber} - {s.Category.Category}",
                })
                .ToListAsync();
                var standalone = await _context.StandaloneServiceWorkOrders
                    .Include(i => i.WorkOrder)
                    .Where(x => x.WorkOrder.ServiceRecordId.Equals(sr) && x.WorkOrder.ServiceLineId.Equals(sl))
                    .Select(s => new
                    {
                        id = s.WorkOrderServiceId,
                        serviceNumber = $"{s.ServiceNumber} - {s.Category.Category}"
                    })
                    .ToListAsync();
                _object = standalone.Union(bundled).ToList();                
            }

            return new ObjectResult(_object);
        }

        public Tuple<string, bool> IsAvailable(int user, DateTime date, DateTime StartTime, DateTime EndTime)
        {
            int day = (int)date.DayOfWeek;
            DateTime time1 = StartTime;
            DateTime time2 = EndTime;
            var roleId = _context.Users.FirstOrDefault(x => x.Id == user)?.RoleId;
            var availableList = _context.CalendarConsultantContactsConsultants
                .Where(x => x.ConsultantContactsConsultantNavigation.UserId == user && x.Day == day)
                .Select(s => new
                {
                    s.ConsultantContactsConsultantNavigation.Name,
                    available = s.Available.HasValue ? s.Available.Value : false,
                    hourStart = Convert.ToDateTime(s.HourStart),
                    hourEnd = Convert.ToDateTime(s.HourEnd),
                })
                .ToList();
            var available = availableList
                .Where(x => time1 >= x.hourStart
                             && time2 <= x.hourEnd)
                .Select(s =>
                    new Tuple<string, bool>(s.Name, s.available))
                .FirstOrDefault();
            // Esto estaba antes al parecer es para tarer la dispo de los consultores return roleId != 3 ? new Tuple<string, bool>("", true) : available;
            return roleId != 1000000 ? new Tuple<string, bool>("", true) : available;
        }

        private static bool EvaluateServiceLine(List<int> sr)
        {

            bool imm = false;
            bool relo = false;
            bool _return = false;

            for (int i = 0; i < sr.Count(); i++)
            {
                if (sr[i] == 1)
                {
                    imm = true;
                }

                if (sr[i] == 2)
                {
                    relo = true;
                }

            }

            if (imm && relo)
            {
                _return = true;
            }

            return _return;
        }
    }

}
