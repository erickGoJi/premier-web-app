using biz.premier.Entities;
using biz.premier.Models;
using biz.premier.Paged;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dal.premier.Repository.ServiceRecord
{
    public class ServiceRecordRepository : GenericRepository<biz.premier.Entities.ServiceRecord>,
        IServiceRecordRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;

        public ServiceRecordRepository(Db_PremierContext context, IConfiguration config, IEmailService emailService) :
            base(context)
        {
            _config = config;
            _emailservice = emailService;
        }

        public biz.premier.Entities.ServiceRecord AddNewService(biz.premier.Entities.ServiceRecord record)
        {
            var ids = _context.ServiceRecords.OrderByDescending(x => x.Id).FirstOrDefault();
            var num = ids != null ? ids.Id + 1 : 1;

            if (record.AssigneeInformations.Count > 0)
            {
                record.AssigneeInformations.FirstOrDefault().CreatedDate = DateTime.Now;
                record.AssigneeInformations.FirstOrDefault().CreatedBy = record.CreatedBy;
            }

            // biz.premier.Entities.ServiceRecord serviceRecord = record;
            // record.CreatedDate = DateTime.Now;
            record.NumberServiceRecord = "SR-" + num;
            _context.ServiceRecords.Add(record);
            _context.SaveChanges();

            var workOrder = record.WorkOrders
                               .Where(x => x.ServiceRecordId == record.Id).ToList();

            for (int i = 0; i < workOrder.Count(); i++)
            {
                var standAlone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderId == workOrder[i].Id).ToList();

                var bundle = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrderId == workOrder[i].Id).ToList();

                if (standAlone != null)
                {
                    for (int x = 0; x < standAlone.Count(); x++)
                    {

                        standAlone[x].StatusId = 1;
                        change_detail_status_byWos_id(standAlone[x].WorkOrderServiceId.Value, standAlone[x].ServiceId.Value, 1);
                        _context.StandaloneServiceWorkOrders.Update(standAlone[x]);
                    }
                }

                if (bundle != null)
                {

                    for (int y = 0; y < bundle.Count(); y++)
                    {
                        bundle[y].StatusId = 1;
                        change_detail_status_byWos_id(bundle[y].WorkServicesId.Value, standAlone[y].ServiceId.Value, 1);
                        _context.BundledServices.Update(bundle[y]);
                    }
                }
            }

            return record;
        }

        public string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public PagedList<biz.premier.Entities.ServiceRecord> GetAllPagedCustom(int pageNumber, int pageSize, bool? vip,
            int? client,
            int? partner, int? country, int? city, int? supplier)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;

            int totalRows = _context.Set<biz.premier.Entities.ServiceRecord>().Count();
            List<biz.premier.Entities.ServiceRecord> result = _context.Set<biz.premier.Entities.ServiceRecord>()
                .Include(i => i.Partner).Include(i => i.WorkOrders).Include(i => i)
                .Include(i => i.Partner).Include(i => i.Client)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();

            if (vip.HasValue)
            {
                result = result.Where(x => x.Vip == vip.Value).ToList();
            }

            if (client.HasValue)
            {
                result = result.Where(x => x.ClientId == client.Value).ToList();
            }

            if (partner.HasValue)
            {
                result = result.Where(x => x.PartnerId == partner).ToList();
            }

            return new PagedList<biz.premier.Entities.ServiceRecord>(totalRows, result);
        }

        public ActionResult GetProfile(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            var query = _context.ServiceRecords.Select(s => new
            {
                s.Id,
                serviceRecordNo = s.NumberServiceRecord,
                name = s.AssigneeInformations.FirstOrDefault().AssigneeName,
                email = s.AssigneeInformations.FirstOrDefault().Email,
                partner = s.Partner.Name,
                client = s.Client.Name,
                vip = s.Vip.Value,
                country = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                    : "",
                city_home = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name : "",
                city = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
                serviceLine = s.WorkOrders.Select(_s => _s.ServiceLine.ServiceLine).ToList(),
                closed = s.ClosedDate.HasValue ? s.ClosedDate.Value.ToString(" dd/MM/yyyy ") : "~"
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new ObjectResult(query);
        }

        public ActionResult GetServiceRecord(bool? vip, int? client, int? partner,
            int? country, int? city, int? supplier,
            DateTime? startDate, DateTime? endDate, string sr, int? status, int? coordiantor, string serviceLine,
            int user)
        {
            var profileId = _context.ProfileUsers.SingleOrDefault(x => x.UserId == user);

            //var serviceLine = 

            var ServiceRecord = _context.ServiceRecords.Select(s => new
            {
                s.Id,
                WorkOrders = s.WorkOrders.Count(),
                avatar_assignee = s.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).Photo,
                serviceRecordNo = s.NumberServiceRecord,
                vip = !s.Vip.HasValue ? false : s.Vip.Value,
                urgent = !s.Urgent.HasValue ? false : s.Urgent.Value,
                s.ConfidentialMove,
                status = s.Status,
                statusId = !s.StatusId.HasValue ? 0 : s.StatusId.Value,
                autho = s.InitialAutho,
                closed = s.ClosedDate,
                s.Office,
                AssigneeInformations = s.AssigneeInformations
                    .Select(c => new
                    {
                        c.Id,
                        c.PetsNavigation,
                        DependentInformations = c.DependentInformations.Select(f => new {
                            f.Name,
                            f.Phone,
                            f.Photo,
                            f.Relationship.Relationship
                        }).Where(r => r.Relationship != "Assignee").ToList()

                    }).ToList(),
                serviceLine = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                homeCountry = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                    : "",
                country = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                    : "",
                countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                countryHomeId = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountryId
                    : 0,
                city = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
                cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                cityHomeName = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name : "N/A",
                partner = s.Partner.Name,
                partnerId = s.PartnerId,
                client = s.Client.Name,
                clientId = s.ClientId,
                clientAvatar = s.Client.Photo == null || s.Client.Photo == "" ? "Files/assets/avatar.png" : s.Client.Photo,
                assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName,
                //services = s.WorkOrders.Count != 0
                //    ? s.StandaloneServiceWorkOrders.Where(x =>
                //            x.WorkOrder.ServiceRecordId == s.ServiceRecordId && x.WorkOrder.ServiceLineId == s.ServiceLineId)
                //        .Select(_s => new
                //        {
                //            id = _s.WorkOrderServiceId,
                //            _s.Service.Service,
                //            _s.ServiceNumber,
                //            _s.WorkOrder.NumberWorkOrder,
                //            _s.CategoryId,
                //            _s.Autho
                //        }).Union(_context.BundledServices
                //            .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id &&
                //                        x.BundledServiceOrder.WorkOrder.ServiceLineId == s.ServiceLineId).Select(_s => new
                //                        {
                //                            id = _s.WorkServicesId,
                //                            _s.Service.Service,
                //                            _s.ServiceNumber,
                //                            _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                //                            _s.CategoryId,
                //                            _s.Autho
                //                        })).ToList()
                //    : null,
                standaloneServices = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho,
                        country = _s.DeliveringInNavigation.Name,
                        serviceLine = _s.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                        && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                        && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                       && f.IdService == _s.ServiceId).NickName
                    }).ToList(),
                bundledService = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id)
                    .Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho,
                        country = _s.DeliveringInNavigation.Name,
                        serviceLine = _s.BundledServiceOrder.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                       && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                       && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                      && f.IdService == _s.ServiceId).NickName
                    }).ToList(),
                pending_accept = _context.RelocationSupplierPartners.Where(r => r.ServiceRecordId == s.Id && r.AcceptedDate == null && r.SupplierId == profileId.Id)
                .Select(y => y.AssignedServiceSupliers.Where(d => d.RelocationSupplierPartnerId == profileId.Id)).Count(),

                timeRemaining = 0,
                coordinatorImm = s.ImmigrationCoodinators
                .Where(x => x.StatusId != 4).Select(c => new
                {
                    Id = c.CoordinatorId,
                    c.Coordinator.Name,
                    c.CoordinatorType.CoordinatorType,
                    coordinatorUser = c.Coordinator.User.Id,
                    avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                    coordinatorAcceptance = c.StatusId == 2 ? false : true
                }),
                coordinatorRelo = s.RelocationCoordinators
                .Where(x => x.StatusId != 4).Select(c => new
                {
                    Id = c.CoordinatorId,
                    c.Coordinator.Name,
                    c.CoordinatorType.CoordinatorType,
                    coordinatorUser = c.Coordinator.User.Id,
                    avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                    coordinatorAcceptance = c.StatusId == 2 ? false : true
                }),
                coordinatorimmId = s.ImmigrationCoodinators.Select(c => c.CoordinatorId),
                coordinatorreloId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
                supplierConsultantImm = s.ImmigrationSupplierPartners
                .Where(x => x.StatusId != 4).Select(c => new
                {
                    Id = c.SupplierId,
                    c.Supplier.Name,
                    supplierConsultant_Id = c.Supplier.UserId,
                    consultantAssigned = c.StatusId == 2 ? false : true,
                    supplierConsultantId = c.SupplierTypeId,
                    country = c.SupplierType.Name,
                    avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar
                }),
                supplierConsultantRelo = s.RelocationSupplierPartners
                .Where(x => x.StatusId != 4).Select(c => new
                {
                    Id = c.SupplierId,
                    c.Supplier.Name,
                    supplierConsultant_Id = c.Supplier.UserId,
                    consultantAssigned = c.StatusId == 2 ? false : true,
                    supplierConsultantId = c.SupplierTypeId,
                    country = c.SupplierType.Name,
                    avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar
                }),
                supplierImmId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                supplierReloId = s.RelocationSupplierPartners.Select(c => c.SupplierId)
            }).ToList();

            //var ReloServiceRecord = _context.ServiceRecords.Select(s => new
            //{
            //    s.Id,
            //    WorkOrders = s.WorkOrders.Count(),
            //    avatar_assignee = s.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).Photo,
            //    serviceRecordNo = s.NumberServiceRecord,
            //    vip = !s.Vip.HasValue ? false : s.Vip.Value,
            //    urgent = !s.Urgent.HasValue ? false : s.Urgent.Value,
            //    status = s.Status,
            //    statusId = !s.StatusId.HasValue ? 0 : s.StatusId.Value,
            //    autho = s.InitialAutho,
            //    closed = s.ClosedDate,
            //    country = s.AssigneeInformations.Count != 0
            //        ? s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
            //        : "",
            //    countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
            //    countryHomeId = s.AssigneeInformations.Count != 0
            //        ? s.AssigneeInformations.FirstOrDefault().HomeCountryId
            //        : 0,
            //    city = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
            //    cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
            //    cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
            //    cityHomeName = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name : "N/A",
            //    partner = s.Partner.Name,
            //    partnerId = s.PartnerId,
            //    client = s.Client.Name,
            //    clientId = s.ClientId,
            //    clientAvatar = s.Client.Photo == null || s.Client.Photo == "" ? "Files/assets/avatar.png" : s.Client.Photo,
            //    assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName,
            //    //services = s.WorkOrders.Count != 0
            //    //    ? s.StandaloneServiceWorkOrders.Where(x =>
            //    //            x.WorkOrder.ServiceRecordId == s.ServiceRecordId && x.WorkOrder.ServiceLineId == s.ServiceLineId)
            //    //        .Select(_s => new
            //    //        {
            //    //            id = _s.WorkOrderServiceId,
            //    //            _s.Service.Service,
            //    //            _s.ServiceNumber,
            //    //            _s.WorkOrder.NumberWorkOrder,
            //    //            _s.CategoryId,
            //    //            _s.Autho
            //    //        }).Union(_context.BundledServices
            //    //            .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id &&
            //    //                        x.BundledServiceOrder.WorkOrder.ServiceLineId == s.ServiceLineId).Select(_s => new
            //    //                        {
            //    //                            id = _s.WorkServicesId,
            //    //                            _s.Service.Service,
            //    //                            _s.ServiceNumber,
            //    //                            _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
            //    //                            _s.CategoryId,
            //    //                            _s.Autho
            //    //                        })).ToList()
            //    //    : null,
            //    standaloneServices = _context.StandaloneServiceWorkOrders
            //        .Where(x => x.WorkOrder.ServiceRecordId == s.Id && x.WorkOrder.ServiceLineId == 2).Select(_s => new
            //        {
            //            id = _s.WorkOrderServiceId,
            //            _s.Service.Service,
            //            _s.ServiceNumber,
            //            _s.WorkOrder.NumberWorkOrder,
            //            _s.CategoryId,
            //            _s.Autho
            //        }).ToList(),
            //    bundledService = _context.BundledServices
            //        .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id &&
            //                    x.BundledServiceOrder.WorkOrder.ServiceLineId == 2)
            //        .Select(_s => new
            //        {
            //            id = _s.WorkServicesId,
            //            _s.Service.Service,
            //            _s.ServiceNumber,
            //            _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
            //            _s.CategoryId,
            //            _s.Autho
            //        }).ToList(),
            //    timeRemaining = 0,
            //    coordinator = s.RelocationCoordinators.Select(c => new
            //    {
            //        Id = c.CoordinatorId,
            //        c.Coordinator.Name,
            //        c.CoordinatorType,
            //        coordinatorUser = c.Coordinator.User.Id,
            //        coordinatorAcceptance = c.StatusId == 2 ? false : true
            //    }),
            //    coordinatorId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
            //    supplierConsultant = s.RelocationSupplierPartners.Select(c => new
            //    {
            //        Id = c.SupplierId,
            //        c.Supplier.Name,
            //        supplierConsultant_Id = c.Supplier.UserId,
            //        consultantAssigned = c.StatusId == 2 ? false : true,
            //        supplierConsultantId = c.SupplierTypeId,
            //        country = c.SupplierType.Name,
            //        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar


            //    }),
            //    supplierId = s.RelocationSupplierPartners.Select(c => c.SupplierId),
            //    serviceLine = 2,
            //    serviceLineName = "Relocation"
            //}).ToList();

            //ImmServiceRecord = ImmServiceRecord.Where(x => (x.standaloneServices.Count() > 0) || (x.bundledService.Count() > 0)).ToList();
            //ReloServiceRecord = ReloServiceRecord.Where(x => (x.standaloneServices.Count() > 0) || (x.bundledService.Count() > 0)).ToList();
            //var ServiceRecord = ImmServiceRecord.Union(ReloServiceRecord);
            //Query por Role
            int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;
            switch (role)
            {
                case 1: // Manager
                    ServiceRecord = ServiceRecord
                        .Where(x => x.Office == profileId.ResponsablePremierOffice).ToList();
                    break;
                case 2: // Coordinator
                    ServiceRecord = ServiceRecord.Where(x => x.coordinatorimmId.Contains(profileId.Id) || x.coordinatorreloId.Contains(profileId.Id)).ToList();
                    break;
                case 3: // Cosultor
                    ServiceRecord = ServiceRecord.Where(x => x.supplierImmId.Contains(profileId.Id) || x.supplierReloId.Contains(profileId.Id)).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            // Filters
            if (vip.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.vip == vip.Value).ToList();
            }

            if (client.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.clientId == client).ToList();
            }

            if (partner.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.partnerId == partner).ToList();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                ServiceRecord = ServiceRecord
                    .Where(x => x.autho.Date >= startDate.Value.Date && x.autho.Date <= endDate.Value.Date).ToList();
            }

            if (sr != null)
            {
                ServiceRecord = ServiceRecord.Where(x => x.serviceRecordNo.Contains(sr.ToUpper())).ToList();
            }

            if (supplier.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.supplierImmId.Contains(supplier) || x.supplierReloId.Contains(supplier)).ToList();
            }

            if (country.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.countryId == country || x.countryHomeId == country).ToList();
            }

            if (city.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.cityId == city).ToList();
            }

            if (status.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.statusId == status).ToList();
            }

            if (coordiantor.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.coordinatorimmId.Contains(coordiantor) || x.coordinatorreloId.Contains(coordiantor)).ToList();
            }

            if (serviceLine != null)
            {
                ServiceRecord = ServiceRecord.Where(x => x.serviceLine?.ToLower() == serviceLine.ToString().ToLower()).ToList();
            }

            //serviceRecordsList = serviceRecordsList.Where(x => x.services.FirstOrDefault().id != null).ToList();
            //immigration = immigration.OrderByDescending(d => d.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            //ServiceRecord = ServiceRecord.OrderByDescending(x => x.vip.Equals(true) || x.urgent.Equals(true))
            //    .ThenByDescending(o => o.Id).ToList();
            ServiceRecord = ServiceRecord.OrderByDescending(x => x.Id)
              .ThenByDescending(o => o.vip.Equals(true)).ToList();
            //immigration = immigration.OrderByDescending(x => GetServiceRecordByIdAppx.autho).ToList();

            return new ObjectResult(ServiceRecord);
        }

        public ActionResult GetServiceRecordListApp(int? partner, DateTime? startDate, DateTime? endDate, int? status, bool? pending_acceptance_services, bool? pending_activity_reports, int user)
        {
            var profileId = _context.ProfileUsers.SingleOrDefault(x => x.UserId == user);

            var ServiceRecord = _context.ServiceRecords
                .Include(x => x.AssigneeInformations)
                .Select(s => new
            {
                s.Id,
                WorkOrders = s.WorkOrders.Count(),
                InitialArrival = s.AssigneeInformations.FirstOrDefault().InitialArrival,
                avatar_assignee = s.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == s.Id).Photo,
                serviceRecordNo = s.NumberServiceRecord,
                vip = !s.Vip.HasValue ? false : s.Vip.Value,
                urgent = !s.Urgent.HasValue ? false : s.Urgent.Value,
                s.ConfidentialMove,
                status = s.Status,
                statusId = !s.StatusId.HasValue ? 0 : s.StatusId.Value,
                autho = s.InitialAutho,
                closed = s.ClosedDate,
                s.Office,
                serviceLine = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                AssigneeInformations = s.AssigneeInformations
                    .Select(c => new
                    {
                        c.Id,
                        c.PetsNavigation,
                        DependentInformations = c.DependentInformations.Select(f => new {
                            f.Name,
                            f.Phone,
                            f.Photo,
                            f.Relationship.Relationship
                        }).Where(r => r.Relationship != "Assignee").ToList()

                    }).ToList(),
                homeCountry = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                    : "",
                country = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                    : "",
                countryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                countryHomeId = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountryId
                    : 0,
                city = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
                cityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                cityHomeId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                cityHomeName = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name : "N/A",
                partner = s.Partner.Name,
                partnerId = s.PartnerId,
                userid = profileId.UserId,
                client = s.Client.Name,
                clientId = s.ClientId,
                clientAvatar = s.Client.Photo == null || s.Client.Photo == "" ? "Files/assets/avatar.png" : s.Client.Photo,
                assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName,
                timeRemaining = 0,
                standaloneServices = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count() +
                    _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count(),
                coordinatorImm = s.ImmigrationCoodinators
                .Where(x => x.StatusId != 4).Select(c => new
                {
                    Id = c.CoordinatorId,
                    c.Coordinator.Name,
                    c.CoordinatorType.CoordinatorType,
                    coordinatorUser = c.Coordinator.User.Id,
                    avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                    coordinatorAcceptance = c.StatusId == 2 ? false : true
                }),
                coordinatorRelo = s.RelocationCoordinators
                .Where(x => x.StatusId != 4).Select(c => new
                {
                    Id = c.CoordinatorId,
                    c.Coordinator.Name,
                    c.CoordinatorType.CoordinatorType,
                    coordinatorUser = c.Coordinator.User.Id,
                    avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                    coordinatorAcceptance = c.StatusId == 2 ? false : true
                }),
                supplierImm = s.ImmigrationSupplierPartners
                    .Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdSupplier = c.Id,
                        Id = c.SupplierId,
                        UserId = c.Supplier.UserId,
                        c.Supplier.Name,
                        c.Supplier.User.Email,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        Accepted = c.StatusId == null ? 2 : c.StatusId,
                        total_services = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()
                        +
                        _context.BundledServices
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()
                    }),
                supplierRelo = s.RelocationSupplierPartners
                    .Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdSupplier = c.Id,
                        Id = c.SupplierId,
                        UserId = c.Supplier.UserId,
                        c.Supplier.Name,
                        c.Supplier.User.Email,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        Accepted = c.StatusId == null ? 2 : c.StatusId,
                        total_services = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()
                        +
                        _context.BundledServices
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()

                    }),
                coordinatorimmId = s.ImmigrationCoodinators.Select(c => c.CoordinatorId),
                coordinatorreloId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
                supplierImmId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                supplierReloId = s.RelocationSupplierPartners.Select(c => c.SupplierId),
                pending_acceptance_services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count()
                    +
                                              _context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count(),
                pending_activity_reports = s.WorkOrders.FirstOrDefault().ReportDays.Where(x => x.ReportDate == null).Count()
            }).ToList();

            //Query por Role
            int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;
            switch (role)
            {
                case 1: // Manager
                    ServiceRecord = ServiceRecord
                        .Where(x => x.Office == profileId.ResponsablePremierOffice).ToList();
                    break;
                case 2: // Coordinator
                    ServiceRecord = ServiceRecord.Where(x => x.coordinatorimmId.Contains(profileId.Id) || x.coordinatorreloId.Contains(profileId.Id)).ToList();
                    break;
                case 3: // Cosultor
                    ServiceRecord = ServiceRecord.Where(x => x.supplierImmId.Contains(profileId.Id) || x.supplierReloId.Contains(profileId.Id)
                    && x?.supplierRelo?.FirstOrDefault()?.total_services > 0).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            // Filters
            if (partner.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.partnerId == partner).ToList();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                ServiceRecord = ServiceRecord
                    .Where(x => x.autho.Date >= startDate.Value.Date && x.autho.Date <= endDate.Value.Date).ToList();
            }

            if (status.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.statusId == status).ToList();
            }

            if (pending_acceptance_services.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.pending_acceptance_services > 0).ToList();
            }

            if (pending_activity_reports.HasValue)
            {
                ServiceRecord = ServiceRecord.Where(x => x.pending_activity_reports > 0).ToList();
            }

            ServiceRecord = ServiceRecord.OrderByDescending(x => x.Id)
              .ThenByDescending(o => o.vip.Equals(true)).ToList();

            return new ObjectResult(ServiceRecord);
        }

        public string GetServiceLineName(int idServiceLine)
        {
            string _serviceLine = "N/S";
            var serviceLine = _context.ServiceRecords
                .Select(s => new
                {
                    s.Id
                })
                .Join(_context.WorkOrders, a => a.Id, b => b.ServiceRecordId,
                        (a, b) => new
                        {
                            a.Id,
                            b.ServiceLineId,
                            b.ServiceLine.ServiceLine
                        })
                .Where(x => x.Id == idServiceLine).ToList();

            for (int i = 0; i < serviceLine.Count(); i++)
            {
                if (serviceLine[i].ServiceLine == "Immigration")
                {
                    _serviceLine = "I";
                }

                if (serviceLine[i].ServiceLine == "Relocation")
                {
                    _serviceLine = "/R";
                }
            }

            return _serviceLine;
        }

        public int getHomeCountry(int id)
        {
            var country = _context.Countries.FirstOrDefault(x => x.Id == id).Name;

            return _context.CatCountries.Any(x => x.Name.ToLower() == country.ToLower())
                    ? _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == country.ToLower()).Id : 0;
        }

        public biz.premier.Entities.ServiceRecord SelectCustom(int key, int user)
        {
            var service = _context.Set<biz.premier.Entities.ServiceRecord>()
                .Include(i => i.AssigneeInformations)
                .ThenInclude(ti => ti.PetsNavigation)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(ti => ti.NationalityAssigneeInformations)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.DependentInformations)
                .ThenInclude(i => i.LanguageDependentInformations)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.LanguagesSpokens)
                .Include(i => i.ImmigrationCoodinators)
                .ThenInclude(i => i.Status)
                .Include(i => i.ImmigrationCoodinators)
                .ThenInclude(i => i.Coordinator)
                .Include(i => i.ImmigrationSupplierPartners)
                .Include(i => i.RelocationCoordinators)
                .ThenInclude(i => i.Status)
                .Include(i => i.RelocationCoordinators)
                .ThenInclude(i => i.Coordinator)
                .Include(i => i.RelocationSupplierPartners)
                .Include(i => i.Status)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.Sex)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.DependentInformations)
                .Include(i => i.Follows)
                .Include(i => i.WorkOrders)
                .Include(i => i.ImmigrationProfiles)
                .ThenInclude(i => i.AssigmentInformation)
                .Include(i => i.ImmigrationProfiles)
                .ThenInclude(i => i.DependentImmigrationInfos)
                .Include(i => i.ImmigrationProfiles)
                .ThenInclude(i => i.EducationalBackgrounds)
                .Include(i => i.ImmigrationProfiles)
                .ThenInclude(i => i.LenguageProficiencies)
                .ThenInclude(i => i.Language)
                .Include(i => i.OfficeNavigation)
                .Single(s => s.Id == key);
            if (service == null)
                return null;
            return service;
        }

        public ActionResult GetServiceRecordByIdApp(int sr)
        {
            var ServiceRecord = _context.ServiceRecords.Select(c => new
            {
                c.Id,
                assigneeName = c.AssigneeInformations.FirstOrDefault().AssigneeName == null ||
                c.AssigneeInformations.FirstOrDefault().AssigneeName == ""
                ? "Files/assets/avatar.png" : c.AssigneeInformations.FirstOrDefault().AssigneeName,
                avatarAssignee = c.AssigneeInformations.FirstOrDefault().Photo,
                InitialArrival = c.AssigneeInformations.FirstOrDefault().InitialArrival,
                client = c.Client.Name,
                partener = c.Partner.Name,
                status = c.Status.Status,
                statusId = c.StatusId,
                c.Vip,
                c.SpocImmigration,
                c.SpocRelocation,
                c.ConfidentialMove,
                serviceLine = EvaluateServiceLine(c.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : c.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                nationality = c.AssigneeInformations.FirstOrDefault().NationalityAssigneeInformations
                .Where(x => x.AssigneeInformationId == c.AssigneeInformations.FirstOrDefault().Id)
                .Select(x => new
                {
                    x.Id,
                    x.AssigneeInformationId,
                    x.Nationality.Name
                }).ToList(),
                homeCountry = c.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                hostCountry = c.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                host_city = c.AssigneeInformations.FirstOrDefault().HostCity.City,
                home_city = c.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                email = c.AssigneeInformations.FirstOrDefault().Email,
                telephone = c.AssigneeInformations.FirstOrDefault().MobilePhone,
                appointment = c.Appointments.Count(),
                activityReport = c.WorkOrders.FirstOrDefault().ReportDays.Count(),
                conversationId = _context.Conversations.FirstOrDefault(ch => ch.ServiceRecordId == c.Id).Id,
                relocation = c.RelocationCoordinators
                    .Where(x => x.StatusId != 4).Select(s => new
                    {
                        IdCoodinator = s.Id,
                        Id = s.CoordinatorId,
                        s.Coordinator.Name,
                        avatar = s.Coordinator.User.Avatar == null || s.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : s.Coordinator.User.Avatar,
                        Accepted = s.StatusId,
                        s.CoordinatorType.CoordinatorType,
                        s.Coordinator.PhoneNumber
                    }),
                immigration = c.ImmigrationCoodinators
                    .Where(x => x.StatusId != 4).Select(s => new
                    {
                        IdCoodinator = s.Id,
                        Id = s.CoordinatorId,
                        s.Coordinator.Name,
                        avatar = s.Coordinator.User.Avatar == null || s.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : s.Coordinator.User.Avatar,
                        Accepted = s.StatusId,
                        s.CoordinatorType.CoordinatorType,
                        s.Coordinator.PhoneNumber
                    })
            }).Where(x => x.Id == sr).ToList();

            return new ObjectResult(ServiceRecord);
        }

        public ActionResult GetServicesApp(int service_record_id, int? status, int? serviceLine, int? user)
        {
            var sr = _context.ServiceRecords
                .Include(x => x.WorkOrders)
                .Include(x => x.AssigneeInformations)
                .SingleOrDefault(x => x.Id == service_record_id);

            //int profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user).Id;

            var _Bundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLine
                            && x.BundledServiceOrder.WorkOrder.ServiceRecordId == service_record_id)
                .Select(n => new
                {
                    n.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    n.Status.Status,

                    supplierId = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.UserId,

                    supplierName = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.Name
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.Name,

                    Avatar = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.Photo
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.Photo,

                    Authodate_consultant = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.AssignedDate
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.AssignedDate,

                    n.ServiceTypeId,
                    ServiceType = "Bundle", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.BundledServiceOrder.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,

                    n.BundledServiceOrder.ProjectedFee,
                    n.BundledServiceOrder.WorkOrderId,
                    n.CategoryId,

                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    location = n.Location,
                    arrival = sr.AssigneeInformations.FirstOrDefault().InitialArrival,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.BundledServiceOrder.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.BundledServiceOrder.WorkOrder.ServiceLineId).NickName,

                    authoTime = n.BundledServiceOrder.TotalTime.ToString(),

                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - Convert.ToDecimal(n.BundledServiceOrder.TotalTime))
                        : Convert.ToDecimal(n.BundledServiceOrder.TotalTime),
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    id_server = n.Id,
                    WorkOrderServiceId = n.WorkServicesId,
                    // END
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkServices.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkServices.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkServices.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkServices.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkServices.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkServices.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkServices.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkServices.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkServices.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                })
                .ToList();

            var _Standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceLineId == serviceLine && x.WorkOrder.ServiceRecordId == service_record_id)
                .Select(n => new
                {
                    n.WorkOrder.ServiceRecordId,
                    n.WorkOrder.NumberWorkOrder,
                    n.Status.Status,

                    supplierId = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.UserId,

                    supplierName = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.Name
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.Name,

                    Avatar = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.Photo
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.Photo,
                   

                    Authodate_consultant = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.AssignedDate
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.AssignedDate,

                    n.ServiceTypeId,
                    ServiceType = "Standalone", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,

                    n.ProjectedFee,
                    n.WorkOrderId,
                    n.CategoryId,

                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    location = n.Location,
                    arrival = sr.AssigneeInformations.FirstOrDefault().InitialArrival,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName,

                    authoTime = n.AuthoTime.Value.ToString(),

                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - n.AuthoTime.Value)
                        : n.AuthoTime.Value,
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    id_server = n.Id,
                    WorkOrderServiceId = n.WorkOrderServiceId,
                    // END

                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                })
                .ToList();

            if (status.HasValue)
            {
                _Bundled = _Bundled.Where(x => x.StatusId == status.Value).ToList();
                _Standalone = _Standalone.Where(x => x.StatusId == status.Value).ToList();
            }

            if (user != null)
            {
                _Bundled = _Bundled.Where(x => x.supplierId == user.Value).ToList();
                _Standalone = _Standalone.Where(x => x.supplierId == user.Value).ToList();
            }

            return new ObjectResult(new
            {
                services = _Bundled.Union(_Standalone)
            });
        }

        public ActionResult GetServicesAppAssignee(int? status, int? serviceLine, int? user)
        {
            var sr = _context.ServiceRecords
                .Include(x => x.WorkOrders)
                .Include(x => x.AssigneeInformations)
                .SingleOrDefault(x => x.AssigneeInformations.FirstOrDefault().UserId == user);

            //int profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user).Id;

            var _Bundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLine
                            && x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr.Id)
                .Select(n => new
                {
                    n.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    n.Status.Status,

                    supplierId = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.UserId,

                    supplierName = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.Name
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.Name,

                    Avatar = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.Photo
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.Photo,

                    Authodate_consultant = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.AssignedDate
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.AssignedDate,

                    n.ServiceTypeId,
                    ServiceType = "Bundle", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.BundledServiceOrder.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,

                    n.BundledServiceOrder.ProjectedFee,
                    n.BundledServiceOrder.WorkOrderId,
                    n.CategoryId,

                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    location = n.Location,
                    arrival = sr.AssigneeInformations.FirstOrDefault().InitialArrival,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.BundledServiceOrder.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.BundledServiceOrder.WorkOrder.ServiceLineId).NickName,

                    authoTime = n.BundledServiceOrder.TotalTime.ToString(),

                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - Convert.ToDecimal(n.BundledServiceOrder.TotalTime))
                        : Convert.ToDecimal(n.BundledServiceOrder.TotalTime),
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    id_server = n.Id,
                    WorkOrderServiceId = n.WorkServicesId,
                    // END
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkServices.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkServices.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkServices.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkServices.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkServices.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkServices.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkServices.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkServices.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkServices.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                })
                .ToList();

            var _Standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceLineId == serviceLine && x.WorkOrder.ServiceRecordId == sr.Id)
                .Select(n => new
                {
                    n.WorkOrder.ServiceRecordId,
                    n.WorkOrder.NumberWorkOrder,
                    n.Status.Status,

                    supplierId = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.UserId,

                    supplierName = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.Name
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.Name,

                    Avatar = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.Photo
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.Photo,


                    Authodate_consultant = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.AssignedDate
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.AssignedDate,

                    n.ServiceTypeId,
                    ServiceType = "Standalone", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,

                    n.ProjectedFee,
                    n.WorkOrderId,
                    n.CategoryId,

                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    location = n.Location,
                    arrival = sr.AssigneeInformations.FirstOrDefault().InitialArrival,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName,

                    authoTime = n.AuthoTime.Value.ToString(),

                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - n.AuthoTime.Value)
                        : n.AuthoTime.Value,
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    id_server = n.Id,
                    WorkOrderServiceId = n.WorkOrderServiceId,
                    // END

                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                })
                .ToList();

            if (status.HasValue)
            {
                _Bundled = _Bundled.Where(x => x.StatusId == status.Value).ToList();
                _Standalone = _Standalone.Where(x => x.StatusId == status.Value).ToList();
            }

            //if (user != null)
            //{
            //    _Bundled = _Bundled.Where(x => x.supplierId == user.Value).ToList();
            //    _Standalone = _Standalone.Where(x => x.supplierId == user.Value).ToList();
            //}

            return new ObjectResult(new
            {
                services = _Bundled.Union(_Standalone)
            });
        }

        public ActionResult GetAssigneeInformationApp(int service_record_id)
        {
            var assignee = _context.AssigneeInformations
                .Where(x => x.ServiceRecordId == service_record_id)
                .Select(x => new
                {
                    x.Id,
                    x.Photo,
                    x.AssigneeName,
                    x.Age,
                    x.NationalityAssigneeInformations,
                    x.HomeCountryId,
                    HomeCountry = x.HomeCountry.Name,
                    x.Email,
                    DependentInformations = x.DependentInformations
                     .Where(r => r.RelationshipId != 7)
                    .Select(c => new
                    {
                        c.Photo,
                        c.Name,
                        c.Age,
                        c.Relationship.Relationship,
                        c.RelationshipId,
                        c.NationalityId,
                        Nationality = _context.Nationalities.FirstOrDefault(z => z.Id == c.NationalityId).Nationality1,
                        c.Email
                    }).ToList(),
                    PetsNavigation = x.PetsNavigation.Select(c => new
                    {
                        c.Photo,
                        c.Name,
                        c.Age,
                        c.BreedId,
                        breed = _context.CatBreeds.FirstOrDefault(z => z.Id == Convert.ToInt32(c.BreedId)).Breed,
                        c.Size.Size,
                        c.Weight
                    }).ToList(),
                    HousingSpec = _context.HousingSpecifications
                    .Select(c => new
                    {
                        c.Id,
                        c.ContractType.ContractType,
                        c.PropertyType.PropertyType,
                        c.Budget,
                        c.Bedroom,
                        c.Size
                    }).OrderByDescending(g => g.Id).Take(1).ToList()
                });

            return new ObjectResult(assignee);
        }

        public AssigneeInformation GetAssigneeInformationAppAll(int service_record_id)
        {
            var assignee = _context.AssigneeInformations
                .Where(x => x.ServiceRecordId == service_record_id)
                .Include(d => d.DependentInformations)
                .Include(p => p.PetsNavigation).ToList();

            return assignee[0];
        }


        public string returnClient(int id)
        {
            return _context.ServiceRecords.Include(x => x.Client).FirstOrDefault(x => x.Id == id).Client.Name;
        }

        public string returnPartner(int id)
        {
            return _context.ServiceRecords.Include(x => x.Partner).FirstOrDefault(x => x.Id == id).Partner.Name;
        }

        public string returnHomeCountry(int id)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(_context.Countries.FirstOrDefault(x => x.Id == id).Name.ToLower());
        }

        public string returnHomeCity(int id)
        {
            return _context.Cities.FirstOrDefault(x => x.Id == id).Name;
        }

        private static string ReturnServiceLine(bool immigration, bool relocation)
        {
            string serviceLine = "";
            if (immigration == true && relocation == true)
            {
                serviceLine = "Relocation & Immigration";
            }
            else if (immigration == true && relocation == false)
            {
                serviceLine = "Immigration";
            }
            else if (immigration == false && relocation == true)
            {
                serviceLine = "Relocation";
            }
            else
            {
                serviceLine = "N/A";
            }

            return serviceLine;
        }

        public biz.premier.Entities.ServiceRecord UpdateCustom(biz.premier.Entities.ServiceRecord record, int key)
        {
            if (record == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.ServiceRecord>()
                .Include(i => i.AssigneeInformations)
                .ThenInclude(ti => ti.PetsNavigation)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.DependentInformations)
                .ThenInclude(i => i.LanguageDependentInformations)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.User)
                .Include(i => i.AssigneeInformations)
                .ThenInclude(i => i.LanguagesSpokens)
                .Include(i => i.ImmigrationCoodinators)
                .Include(i => i.ImmigrationSupplierPartners)
                .Include(i => i.RelocationCoordinators)
                .Include(i => i.RelocationSupplierPartners)
                .Single(s => s.Id == key);

            record.NumberServiceRecord =
                exist.NumberServiceRecord != null ? exist.NumberServiceRecord : $"SR-{exist.Id}";

            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(record);

                foreach (var a in record.AssigneeInformations)
                {
                    var assigneeInfo = exist.AssigneeInformations.Where(w => w.Id == a.Id);
                    if (assigneeInfo == null)
                    {
                        exist.AssigneeInformations.Add(a);
                    }
                    else
                    {
                        _context.Entry(assigneeInfo.FirstOrDefault()).CurrentValues.SetValues(a);
                        foreach (var p in a.PetsNavigation)
                        {
                            var pet = assigneeInfo.FirstOrDefault().PetsNavigation.Where(w => w.Id == p.Id)
                                .FirstOrDefault();
                            if (pet == null)
                            {
                                exist.AssigneeInformations.FirstOrDefault().PetsNavigation.Add(p);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(pet).CurrentValues.SetValues(p);
                            }
                        }

                        foreach (var d in a.DependentInformations)
                        {
                            var dependent = assigneeInfo.FirstOrDefault().DependentInformations.Where(w => w.Id == d.Id)
                                .FirstOrDefault();
                            if (dependent == null)
                            {
                                exist.AssigneeInformations.FirstOrDefault().DependentInformations.Add(d);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(dependent).CurrentValues.SetValues(d);
                                dependent.LanguageDependentInformations.Clear();
                                _context.SaveChanges();
                                foreach (var o in d.LanguageDependentInformations)
                                {
                                    dependent.LanguageDependentInformations.Add(o);
                                }

                                _context.SaveChanges();
                            }
                        }

                        exist.AssigneeInformations.FirstOrDefault().LanguagesSpokens.Clear();
                        _context.SaveChanges();
                        foreach (var o in a.LanguagesSpokens)
                        {
                            exist.AssigneeInformations.FirstOrDefault().LanguagesSpokens.Add(o);
                        }

                        // exist.AssigneeInformations.FirstOrDefault().NationalityAssigneeInformations.Clear();

                        var nats = _context.NationalityAssigneeInformations.Where(s => s.AssigneeInformationId == a.Id).ToList();

                        foreach (var o in nats)
                        {
                            o.AssigneeInformationId = null;
                            _context.NationalityAssigneeInformations.Update(o);
                            _context.SaveChanges();
                           // o.Id = 0;
                          //  exist.AssigneeInformations.FirstOrDefault().NationalityAssigneeInformations.Add(o);
                        }

                       
                        foreach (var o in a.NationalityAssigneeInformations)
                        {
                            o.Id = 0;
                            exist.AssigneeInformations.FirstOrDefault().NationalityAssigneeInformations.Add(o);
                        }
                        _context.SaveChanges();
                    }
                }

                foreach (var ic in record.ImmigrationCoodinators)
                {
                    var coordinator = exist.ImmigrationCoodinators.Where(i => i.Id == ic.Id).FirstOrDefault();
                    if (coordinator == null)
                    {
                        ic.StatusId = 2;
                        exist.ImmigrationCoodinators.Add(ic);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(coordinator).CurrentValues.SetValues(ic);
                    }
                }

                foreach (var isp in record.ImmigrationSupplierPartners)
                {
                    var supplierPartners =
                        exist.ImmigrationSupplierPartners.Where(i => i.Id == isp.Id).FirstOrDefault();
                    if (supplierPartners == null)
                    {
                        isp.StatusId = 2;
                        exist.ImmigrationSupplierPartners.Add(isp);
                    }
                    else
                    {
                        _context.Entry(supplierPartners).CurrentValues.SetValues(isp);
                    }
                }

                foreach (var rc in record.RelocationCoordinators)
                {
                    var relocationCoordinators =
                        exist.RelocationCoordinators.Where(i => i.Id == rc.Id).FirstOrDefault();
                    if (relocationCoordinators == null)
                    {
                        rc.StatusId = 2;
                        exist.RelocationCoordinators.Add(rc);
                    }
                    else
                    {
                        _context.Entry(relocationCoordinators).CurrentValues.SetValues(rc);
                    }
                }

                foreach (var rsp in record.RelocationSupplierPartners)
                {
                    var relocationSupplierPartners =
                        exist.RelocationSupplierPartners.Where(i => i.Id == rsp.Id).FirstOrDefault();
                    if (relocationSupplierPartners == null)
                    {
                        rsp.StatusId = 2;
                        exist.RelocationSupplierPartners.Add(rsp);
                    }
                    else
                    {
                        _context.Entry(relocationSupplierPartners).CurrentValues.SetValues(rsp);
                    }
                }


                record.UpdatedDate = DateTime.Now;
                _context.SaveChanges();
            }

            return exist;
        }

        public string UploadImageBase64(string image)
        {
            //string ruta;

            var filePath = Environment.CurrentDirectory;
            var extension = "jpg";
            var _guid = Guid.NewGuid();
            var path = "/Files/Pets/" + _guid + "." + extension;

            var bytes = Convert.FromBase64String(image);
            using (var imageFile = new FileStream(filePath + path, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            return path;
        }

        public ActionResult GetApplicant(int sr)
        {
            var query = _context.DependentInformations.Where(x => x.AssigneeInformation.ServiceRecordId.Value == sr)
                .Select(s => new
                {
                    dependentId = s.Id,
                    Name = s.RelationshipId == 7 ? s.AssigneeInformation.User.Name : s.Name,
                    s.Relationship.Relationship,
                    s.RelationshipId
                }).ToList();
            if (query.Any())
            {
                return new ObjectResult(query);
            }
            else
            {
                return new EmptyResult();
            }
        }

        public ActionResult GetChildrenBySrId(int sr)
        {
            var query = _context.DependentInformations.Where(x => x.AssigneeInformation.ServiceRecordId.Value == sr)
                .Include(s => s.LanguageDependentInformations)
                    .ThenInclude(s => s.LanguageNavigation)
                .Include(s => s.SchoolingInformations)
                .Where(c => c.RelationshipId == 2)
                .Select(s => new
                {
                    id = s.SchoolingInformations.FirstOrDefault().Id,
                    dependentId = s.Id,
                    active = s.SchoolingInformations.Any(x => x.Active.Value),
                    s.Name,
                    s.Sex,
                    s.Age,
                    s.CurrentGrade,
                    Grade = _context.CatGradeSchoolings.FirstOrDefault(x => x.Id == s.CurrentGrade).Grade,
                    s.Photo,
                    s.AditionalComments,
                    s.NationalityId,
                    nationality = _context.Nationalities.FirstOrDefault(x => x.Id == s.NationalityId).Nationality1,
                    s.Birth,
                    LanguageDependentInformations = s.LanguageDependentInformations,
                    s.Relationship.Relationship,
                    s.SchoolsLists,
                    s.RelationshipId
                }).ToList();
            if (query.Any())
            {
                return new ObjectResult(query);
            }
            else
            {
                return new EmptyResult();
            }
        }


        public ActionResult GetServices(int service_record_id, int type, int? status, int? deliverTo, int? serviceType,
            int? program, int? userId)
        {
            var assignee = _context.AssigneeInformations.Where(f => f.ServiceRecordId == service_record_id).Select(s =>
                new
                {
                    host = s.HostCountry.Value,
                    home = _context.CatCountries.Any(x => x.Name.ToLower() == s.HomeCountry.Name.ToLower())
                    ? _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == s.HomeCountry.Name.ToLower()).Id : 0
                }).FirstOrDefault();
            var sr = _context.ServiceRecords.Include(x => x.WorkOrders).SingleOrDefault(x => x.Id == service_record_id);

            var _homeBundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == type
                            && x.BundledServiceOrder.WorkOrder.ServiceRecordId == service_record_id
                            && x.DeliveringIn == assignee.home)
                .Select(n => new
                {
                    n.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    n.Status.Status,
                    supplier = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.UserId,
                    n.ServiceTypeId,
                    ServiceType = n.BundledServiceOrder.Package.Value ? "Package" : "Bundled", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.BundledServiceOrder.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,
                    n.BundledServiceOrder.ProjectedFee,
                    n.BundledServiceOrder.WorkOrderId,
                    n.CategoryId,
                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    service_name = n.BundledServiceOrder.BundledServices.Count().ToString(),
                    authoTime = n.BundledServiceOrder.TotalTime,
                    timeRemaining = _context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id).Any()
                        ? Math.Abs(_context.ServiceReportDays.Where(_x =>
                                       _x.Service.Value == n.Id).Sum(_s => Convert.ToDecimal(_s.Time)) -
                                   int.Parse(n.BundledServiceOrder.TotalTime))
                        : int.Parse(n.BundledServiceOrder.TotalTime),
                    // NUMBER SERVER START
                    number_server = n.BundledServiceOrder.BundledServices.Count().ToString(),
                    // END
                    // ID SERVER START
                    id_server = n.Id,
                    // END
                    n.Location,
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = _context.BundledServicesWorkOrders
                        .Where(x => x.WorkOrderId == n.BundledServiceOrder.WorkOrderId)
                        .Select(r => new
                        {
                            Id = r.BundledServices.Count()
                        }).ToList(),
                    StatusService = "N/A",
                    statusServiceId = 1,
                    dialog_type = n.CategoryId,
                    bundled = n.BundledServiceOrderId
                })
                .ToList();

            var _hostBundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == type
                            && x.BundledServiceOrder.WorkOrder.ServiceRecordId == service_record_id
                            && x.DeliveringIn == assignee.host)
                .Select(n => new
                {
                    n.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    n.Status.Status,
                    supplier = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.UserId,
                    n.ServiceTypeId,
                    ServiceType = n.BundledServiceOrder.Package.Value ? "Package" : "Bundled", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.BundledServiceOrder.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,
                    n.BundledServiceOrder.ProjectedFee,
                    n.BundledServiceOrder.WorkOrderId,
                    n.CategoryId,
                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    service_name = n.BundledServiceOrder.BundledServices.Count().ToString(),
                    authoTime = n.BundledServiceOrder.TotalTime,
                    timeRemaining = _context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id).Any()
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - int.Parse(n.BundledServiceOrder.TotalTime))
                        : int.Parse(n.BundledServiceOrder.TotalTime),
                    // NUMBER SERVER START
                    number_server = n.BundledServiceOrder.BundledServices.Count().ToString(),
                    // END
                    // ID SERVER START
                    id_server = n.Id,
                    // END
                    n.Location,
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = _context.BundledServicesWorkOrders
                        .Where(x => x.WorkOrderId == n.BundledServiceOrder.WorkOrderId)
                        .Select(r => new
                        {
                            Id = r.BundledServices.Count()
                        }).ToList(),
                    StatusService = "N/A",
                    statusServiceId = 1,
                    dialog_type = n.CategoryId,
                    bundled = n.BundledServiceOrderId
                })
                .ToList();

            var _homeStandalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceLineId == type && x.WorkOrder.ServiceRecordId == service_record_id &&
                            x.DeliveringIn == assignee.home)
                .Select(n => new
                {
                    n.WorkOrder.ServiceRecordId,
                    n.WorkOrder.NumberWorkOrder,
                    n.Status.Status,
                    supplier = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.UserId,
                    n.ServiceTypeId,
                    ServiceType = "Standalone", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,
                    n.ProjectedFee,
                    n.WorkOrderId,
                    n.CategoryId,
                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName,
                    authoTime = n.AuthoTime.Value.ToString(),
                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - n.AuthoTime.Value)
                        : n.AuthoTime.Value,
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    //number_server = n.CategoryId == 5 ? _context.DocumentManagements
                    //        .Count(f => f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s =>
                    //                        _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                        .Contains(5)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                    //                        .Contains(assignee.home)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                        .Contains(n.WorkOrderId)
                    //                    && f.ApplicantId == n.DeliveredTo).ToString()
                    //    : n.CategoryId == 6 ? _context.LocalDocumentations
                    //        .Count(f => f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s =>
                    //                        _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                        .Contains(6)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                    //                        .Contains(assignee.home)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                        .Contains(n.WorkOrderId)
                    //                    && f.ApplicantId == n.DeliveredTo).ToString()
                    //    : n.CategoryId == 19 ? _context.Transportations
                    //        .Count(f => f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s =>
                    //                        _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                        .Contains(19)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                    //                        .Contains(assignee.home)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                        .Contains(n.WorkOrderId)
                    //                    && f.ApplicantId == n.DeliveredTo).ToString()
                    //    : n.CategoryId == 20 ? _context.AirportTransportationServices
                    //        .Count(f => f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s =>
                    //                        _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                        .Contains(20)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                    //                        .Contains(assignee.home)
                    //                    && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                        .Contains(n.WorkOrderId)
                    //                    && f.ApplicantId == n.DeliveredTo).ToString()
                    //    : n.ServiceNumber,
                    // END
                    // ID SERVER START
                    id_server = n.CategoryId == 5 ? _context.DocumentManagements
                            .FirstOrDefault(f => f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s =>
                                                     _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                                                 && f.WorkOrderServices.StandaloneServiceWorkOrders
                                                     .Select(_s => _s.CategoryId).Contains(5)
                                                 && f.WorkOrderServices.StandaloneServiceWorkOrders
                                                     .Select(_s => _s.DeliveringIn).Contains(assignee.home)
                                                 && f.WorkOrderServices.StandaloneServiceWorkOrders
                                                     .Select(_s => _s.WorkOrderId).Contains(n.WorkOrderId)
                                                 && f.ApplicantId == n.DeliveredTo).Id
                        : n.CategoryId == 6 ? _context.LocalDocumentations
                            .FirstOrDefault(f => f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s =>
                                                     _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                                                 && f.WorkOrderServices.StandaloneServiceWorkOrders
                                                     .Select(_s => _s.CategoryId).Contains(6)
                                                 && f.WorkOrderServices.StandaloneServiceWorkOrders
                                                     .Select(_s => _s.DeliveringIn).Contains(assignee.home)
                                                 && f.WorkOrderServices.StandaloneServiceWorkOrders
                                                     .Select(_s => _s.WorkOrderId).Contains(n.WorkOrderId)
                                                 && f.ApplicantId == n.DeliveredTo).Id
                        //: n.CategoryId == 19 ? _context.Transportations
                        //    .FirstOrDefault(f =>
                        //        f.WorkOrderServices.StandaloneServiceWorkOrders
                        //            .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                        //        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                        //            .Contains(19)
                        //        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                        //            .Contains(assignee.home)
                        //        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                        //            .Contains(n.WorkOrderId)
                        //        && f.ApplicantId == n.DeliveredTo).Id
                        //: n.CategoryId == 20 ? _context.AirportTransportationServices
                        //    .FirstOrDefault(f =>
                        //        f.WorkOrderServices.StandaloneServiceWorkOrders
                        //            .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                        //        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                        //            .Contains(20)
                        //        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                        //            .Contains(assignee.home)
                        //        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                        //            .Contains(n.WorkOrderId)
                        //        && f.ApplicantId == n.DeliveredTo).Id
                        : n.Id,
                    // END
                    n.Location,
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        //: n.CategoryId == 19 ? _context.CatCategories
                        //    .Where(x => x.Id == 19)
                        //    .Select(r => new
                        //    {
                        //        r.Id
                        //    }).ToList()
                        //: n.CategoryId == 20 ? _context.CatCategories
                        //    .Where(x => x.Id == 20)
                        //    .Select(r => new
                        //    {
                        //        r.Id
                        //    }).ToList()
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                }).ToList();

            var _hostStandalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceLineId == type && x.WorkOrder.ServiceRecordId == service_record_id &&
                            x.DeliveringIn == assignee.host)
                .Select(n => new
                {
                    n.WorkOrder.ServiceRecordId,
                    n.WorkOrder.NumberWorkOrder,
                    n.Status.Status,
                    supplier = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.UserId,
                    n.ServiceTypeId,
                    ServiceType = "Standalone", // n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,
                    n.ProjectedFee,
                    n.WorkOrderId,
                    n.CategoryId,
                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName,
                    authoTime = n.AuthoTime.Value.ToString(),
                    timeRemaining = _context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id).Any()
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - n.AuthoTime.Value)
                        : n.AuthoTime.Value,
                    // NUMBER SERVER START
                    number_server = n.ServiceNumber,
                    //number_server = n.CategoryId == 5 ? _context.DocumentManagements
                    //        .Count(f => (f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                         .Contains(5)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.DeliveringIn).Contains(assignee.host)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                         .Contains(n.WorkOrderId)
                    //                     && f.ApplicantId == n.DeliveredTo)
                    //                    || (f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrder.ServiceRecordId)
                    //                            .Contains(service_record_id)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.CategoryId)
                    //                            .Contains(5)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.DeliveringIn)
                    //                            .Contains(assignee.host)
                    //                        && f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrderId)
                    //                            .Contains(n.WorkOrderId))
                    //        ).ToString()
                    //    : n.CategoryId == 6 ? _context.LocalDocumentations
                    //        .Count(f => (f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                         .Contains(6)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.DeliveringIn).Contains(assignee.host)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                         .Contains(n.WorkOrderId)
                    //                     && f.ApplicantId == n.DeliveredTo)
                    //                    || (f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrder.ServiceRecordId)
                    //                            .Contains(service_record_id)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.CategoryId)
                    //                            .Contains(6)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.DeliveringIn)
                    //                            .Contains(assignee.host)
                    //                        && f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrderId)
                    //                            .Contains(n.WorkOrderId))
                    //        ).ToString()
                    //    : n.CategoryId == 19 ? _context.Transportations
                    //        .Count(f => (f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                         .Contains(19)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.DeliveringIn).Contains(assignee.host)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                         .Contains(n.WorkOrderId)
                    //                     && f.ApplicantId == n.DeliveredTo)
                    //                    || (f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrder.ServiceRecordId)
                    //                            .Contains(service_record_id)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.CategoryId)
                    //                            .Contains(19)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.DeliveringIn)
                    //                            .Contains(assignee.host)
                    //                        && f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrderId)
                    //                            .Contains(n.WorkOrderId))
                    //        ).ToString()
                    //    : n.CategoryId == 20 ? _context.AirportTransportationServices
                    //        .Count(f => (f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                    //                         .Contains(20)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders
                    //                         .Select(_s => _s.DeliveringIn).Contains(assignee.host)
                    //                     && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                    //                         .Contains(n.WorkOrderId)
                    //                     && f.ApplicantId == n.DeliveredTo)
                    //                    || (f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrder.ServiceRecordId)
                    //                            .Contains(service_record_id)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.CategoryId)
                    //                            .Contains(20)
                    //                        && f.WorkOrderServices.BundledServices.Select(_s => _s.DeliveringIn)
                    //                            .Contains(assignee.host)
                    //                        && f.WorkOrderServices.BundledServices
                    //                            .Select(_s => _s.BundledServiceOrder.WorkOrderId)
                    //                            .Contains(n.WorkOrderId))
                    //        ).ToString()
                    //    : n.ServiceNumber,
                    // END
                    // ID SERVER START
                    id_server = n.CategoryId == 5 ? _context.DocumentManagements
                            .Where(f => f.WorkOrderServices.StandaloneServiceWorkOrders
                                            .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                                        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                                            .Contains(5)
                                        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                                            .Contains(assignee.host)
                                        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                                            .Contains(n.WorkOrderId)
                                        && f.ApplicantId == n.DeliveredTo).FirstOrDefault().Id
                        : n.CategoryId == 6 ? _context.LocalDocumentations
                            .Where(f => f.WorkOrderServices.StandaloneServiceWorkOrders
                                            .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                                        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                                            .Contains(6)
                                        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                                            .Contains(assignee.host)
                                        && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.WorkOrderId)
                                            .Contains(n.WorkOrderId)
                                        && f.ApplicantId == n.DeliveredTo).FirstOrDefault().Id
                        //: n.CategoryId == 19 ? _context.Transportations
                        //    .Where(f => f.WorkOrderServices.StandaloneServiceWorkOrders
                        //                    .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                        //                && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                        //                    .Contains(19)
                        //                && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                        //                    .Contains(assignee.host)
                        //                && f.WorkOrderServices.StandaloneServiceWorkOrders
                        //                    .Select(_s => _s.WorkOrderServiceId).Contains(n.WorkOrderServiceId)
                        //                && f.ApplicantId == n.DeliveredTo).FirstOrDefault().Id
                        //: n.CategoryId == 20 ? _context.AirportTransportationServices
                        //    .Where(f => f.WorkOrderServices.StandaloneServiceWorkOrders
                        //                    .Select(_s => _s.WorkOrder.ServiceRecordId).Contains(service_record_id)
                        //                && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.CategoryId)
                        //                    .Contains(20)
                        //                && f.WorkOrderServices.StandaloneServiceWorkOrders.Select(_s => _s.DeliveringIn)
                        //                    .Contains(assignee.host)
                        //                && f.WorkOrderServices.StandaloneServiceWorkOrders
                        //                    .Select(_s => _s.WorkOrderServiceId).Contains(n.WorkOrderServiceId)
                        //                && f.ApplicantId == n.DeliveredTo).FirstOrDefault().Id
                        : n.Id,
                    // END
                    n.Location,
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
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
                       : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 21 ? n.WorkOrderService.LeaseRenewals
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? "N/A"
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                }).ToList();

            var Home = _homeBundled.GroupBy(g => g.bundled).Select(g => g.First()).ToList().Union(_homeStandalone)
                .ToList();
            var Host = _hostBundled.GroupBy(g => g.bundled).Select(g => g.First()).ToList().Union(_hostStandalone)
                .ToList();

            var _Home_management = Home.Where(s => s.CategoryId == 5).GroupBy(x => x.WorkOrderId).Select(g => g.First())
                .ToList();
            var _Home_document = Home.Where(s => s.CategoryId == 6).GroupBy(x => x.WorkOrderId).Select(g => g.First())
                .ToList();
            //var _Home_Transporation = Home.Where(s => s.CategoryId == 19).GroupBy(x => x.WorkOrderId)
            //    .Select(g => g.First()).ToList();
            //var _Home_Airport_Transporation = Home.Where(s => s.CategoryId == 20).GroupBy(x => x.WorkOrderId)
            //    .Select(g => g.First()).ToList();

            var _Host_management = Host
                .Where(s => s.CategoryId == 5)
                .GroupBy(x => x.WorkOrderId)
                .Select(g => g.First())
                .ToList();
            var _Host_document = Host.Where(s => s.CategoryId == 6).GroupBy(x => x.WorkOrderId).Select(g => g.First())
                .ToList();
            //var _Host_Transporation = Host.Where(s => s.CategoryId == 19).GroupBy(x => x.WorkOrderId)
            //    .Select(g => g.First()).ToList();
            //var _Host_Airport_Transporation = Host.Where(s => s.CategoryId == 20).GroupBy(x => x.WorkOrderId)
            //    .Select(g => g.First()).ToList();

            //Home = Home.Where(x => x.CategoryId != 5 && x.CategoryId != 6 && x.CategoryId != 19 && x.CategoryId != 20)
            //    .ToList();
            //Home = Home.Where(x => x.CategoryId != 5 && x.CategoryId != 6)
            //    .ToList();

            //Home = Home.Union(_Home_management.Union(_Home_document).Union(_Home_Transporation)
            //    .Union(_Home_Airport_Transporation)).ToList();

            //Host = Host.Where(x => x.CategoryId != 5 && x.CategoryId != 6 && x.CategoryId != 19 && x.CategoryId != 20)
            //    .ToList();
            //Host = Host.Where(x => x.CategoryId != 5 && x.CategoryId != 6)
            //    .ToList();

            //Host = Host.Union(_Host_management.Union(_Host_document).Union(_Host_Transporation)
            //    .Union(_Host_Airport_Transporation)).ToList();

            if (status.HasValue)
            {
                Host = Host.Where(x => x.StatusId == status.Value).ToList();
                Home = Home.Where(x => x.StatusId == status.Value).ToList();
            }

            if (deliverTo.HasValue)
            {
                Host = Host.Where(x => x.DeliveredToId == deliverTo).ToList();
                Home = Home.Where(x => x.DeliveredToId == deliverTo).ToList();
            }

            if (serviceType.HasValue)
            {
                Host = Host.Where(x => x.CategoryId == serviceType.Value).ToList();
                Home = Home.Where(x => x.CategoryId == serviceType.Value).ToList();
            }

            if (program.HasValue)
            {
                Host = Host.Where(x => x.ServiceTypeId == program.Value).ToList();
                Home = Home.Where(x => x.ServiceTypeId == program.Value).ToList();
            }

            if (_context.Users.FirstOrDefault(x => x.Id == userId).RoleId == 3)
            {
                if (userId.HasValue)
                {
                    Host = Host.Where(x => x.supplier == userId.Value).ToList();
                    Home = Home.Where(x => x.supplier == userId.Value).ToList();
                }
            }

            return new ObjectResult(new
            {
                Home,
                Host
            });
        }

        public List<DependentInformation> GetDependets(int key)
        {
            var query = _context.DependentInformations
                .Where(x => x.AssigneeInformation.ServiceRecordId == key)
                .OrderBy(x => x.Name)
                .ToList();
            return query;
        }

        public List<LeaseSignator> GetSignatorsbyId(int key)
        {
            var query = _context.LeaseSignators
                .Where(x => x.Id == (key == 0 ? x.Id : key ))
                .OrderBy(x => x.Signator)
                .ToList();

            return query;
        }


        public List<SecurityDeposit> GetSecurityDeposit(int key)
        {
            var query = _context.SecurityDeposits
                .Where(x => x.Id == (key == 0 ? x.Id : key))
                .OrderBy(x => x.Name)
                .ToList();

            return query;
        }

        public List<InitialRentPayment> GetInitialRentPayment(int key)
        {
            var query = _context.InitialRentPayments
                .Where(x => x.Id == (key == 0 ? x.Id : key))
                .OrderBy(x => x.Name)
                .ToList();

            return query;
        }

        public List<OngoingRentPayment> GetOngoingRentPayment(int key)
        {
            var query = _context.OngoingRentPayments
                .Where(x => x.Id == (key == 0 ? x.Id : key))
                .OrderBy(x => x.Name)
                .ToList();

            return query;
        }

        public List<RealtorCommission> GetRealtorCommission(int key)
        {
            var query = _context.RealtorCommissions
                .Where(x => x.Id == (key == 0 ? x.Id : key))
                .OrderBy(x => x.Name)
                .ToList();

            return query;
        }





        public Tuple<bool, string> CompleteService(int key, int serviceLine)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "Service RecorService Record Was Not Found.");
            bool isImmigrationComplete = false;
            bool isRelocationComplete = false;
            var service = _context.ServiceRecords
                .SingleOrDefault(s => s.Id == key);
            if (service != null)
            {
                var statusServices = new int[] { 3, 4, 5 };
                var statusWO = new int[] { 3 };
                var work = _context.WorkOrders.Where(x => x.ServiceRecordId == service.Id).ToList();
                work = work.Where(x => !statusWO.Contains(x.StatusId.Value)).ToList();

                if (work.Any())
                {
                    var serviceLineRelocation = work.Where(x => x.ServiceLineId == 2).ToList();
                    var serviceLineImmigration = work.Where(x => x.ServiceLineId == 1).ToList();

                    #region Relocation

                    var relocationBundled = _context.BundledServices
                        .Where(x => serviceLineRelocation.Select(s => s.Id)
                            .Contains(x.BundledServiceOrder.WorkOrderId.Value))
                        .Select(s => new
                        {
                            status = s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 16 ? s.WorkServices.Departures
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 19 ? s.WorkServices.Transportations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId.Value
                                    }).FirstOrDefault()
                                : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
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
                                : s.WorkServices.PredecisionOrientations
                                    .Select(r => new
                                    {
                                        StatusId = 0
                                    }).FirstOrDefault()
                        }).ToList();
                    var relocationStandalone = _context.StandaloneServiceWorkOrders
                        .Where(x => serviceLineRelocation.Select(s => s.Id).Contains(x.WorkOrderId.Value))
                        .Select(s => new
                        {
                            status = s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 16 ? s.WorkOrderService.Departures
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId.Value
                                    }).FirstOrDefault()
                                : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
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
                                : s.WorkOrderService.PredecisionOrientations
                                    .Select(r => new
                                    {
                                        StatusId = 0
                                    }).FirstOrDefault()
                        }).ToList();
                    var relocation = relocationBundled.Union(relocationStandalone).ToList();

                    #endregion

                    #region Immigration

                    var immigrationBundled = _context.BundledServices
                        .Where(x => serviceLineImmigration.Select(s => s.Id)
                            .Contains(x.BundledServiceOrder.WorkOrderId.Value))
                        .Select(s => new
                        {
                            status = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
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
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
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
                                        StatusId = r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 28 ? s.WorkServices.Others
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
                    var immigrationStandalone = _context.StandaloneServiceWorkOrders
                        .Where(x => serviceLineImmigration.Select(s => s.Id).Contains(x.WorkOrderId.Value))
                        .Select(s => new
                        {
                            status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId
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
                                        StatusId = r.StatusId
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
                                        r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                                    .Select(r => new
                                    {
                                        r.StatusId
                                    }).FirstOrDefault()
                                : s.CategoryId == 28 ? s.WorkOrderService.Others
                                    .Select(r => new
                                    {
                                        StatusId = r.StatusId.Value
                                    }).FirstOrDefault()
                                : s.WorkOrderService.WorkPermits
                                    .Select(r => new
                                    {
                                        StatusId = 0
                                    }).FirstOrDefault()
                        }).ToList();
                    var immigration = immigrationBundled.Union(immigrationStandalone).ToList();

                    #endregion

                    if (immigration.Any())
                    {
                        isImmigrationComplete = !immigration.Any(x => !statusServices.Contains(x.status.StatusId));
                    }
                    else
                    {
                        isImmigrationComplete = true;
                    }

                    if (relocation.Any())
                    {
                        isRelocationComplete = !relocation.Any(x => !statusServices.Contains(x.status.StatusId));
                    }
                    else
                    {
                        isRelocationComplete = true;
                    }

                    if (isImmigrationComplete == true && isRelocationComplete == true)
                    {
                        if (serviceLine == 1)
                        {
                            foreach (var workOrder in serviceLineImmigration)
                            {
                                workOrder.StatusId = 3;
                                _context.WorkOrders.Update(workOrder);
                            }

                            _context.SaveChanges();
                        }
                        else if (serviceLine == 2)
                        {
                            foreach (var workOrder in serviceLineRelocation)
                            {
                                workOrder.StatusId = 3;
                                _context.WorkOrders.Update(workOrder);
                            }

                            _context.SaveChanges();
                        }

                        service.StatusId = 7;
                        Update(service, service.Id);
                        tuple = new Tuple<bool, string>(true, "Service Record Complete.");
                    }
                    else if (isImmigrationComplete == true && isRelocationComplete == false)
                    {
                        if (serviceLine == 1)
                        {
                            foreach (var workOrder in serviceLineImmigration)
                            {
                                workOrder.StatusId = 3;
                                _context.WorkOrders.Update(workOrder);
                            }

                            _context.SaveChanges();
                        }

                        tuple = new Tuple<bool, string>(true, "Work Orders Complete but not Closed Service Record.");
                    }
                    else if (isImmigrationComplete == false && isRelocationComplete == true)
                    {
                        if (serviceLine == 2)
                        {
                            foreach (var workOrder in serviceLineRelocation)
                            {
                                workOrder.StatusId = 3;
                                _context.WorkOrders.Update(workOrder);
                            }

                            _context.SaveChanges();
                        }

                        tuple = new Tuple<bool, string>(true, "Work Orders Complete but not Closed Service Record.");
                    }
                    else if (isImmigrationComplete == false && isRelocationComplete == false)
                    {
                        tuple = new Tuple<bool, string>(false, "Services not complete.");
                    }

                }
            }
            else
            {
                return tuple;
            }

            return tuple;
        }

        public List<biz.premier.Entities.WorkOrder> GetWorkOrdersByServiceLine(int sr, int sl)
        {
            var consult = _context.Set<biz.premier.Entities.WorkOrder>()
                .Where(x => x.ServiceRecordId == sr && x.ServiceLineId == sl).ToList();
            return consult;
        }

        public ActionResult GetServiceByWorkOrder(int wo, int idUser)
        {
            //var consult = _context.Set<biz.premier.Entities.WorkOrder>()
            //    .Include(i => i.StandaloneServiceWorkOrders)
            //    .Include(i => i.BundledServicesWorkOrders)
            //    .ThenInclude(i => i.BundledServices)
            //    .Where(x => x.Id == wo).ToList();

            var userId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == idUser).Id;
            var standalone = _context.Set<biz.premier.Entities.StandaloneServiceWorkOrder>()
                .Where(x => x.WorkOrderId == wo).Select(s => new
                {
                    s.Id,
                    Location = s.Location,
                    ServiceNumber = s.ServiceNumber + "-" + (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                        && f.IdService == s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                        && f.IdService == s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).NickName),
                    service = s.WorkOrderServiceId,
                    assignee = s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    partner = s.WorkOrder.ServiceRecord.Partner.Name,
                    client = s.WorkOrder.ServiceRecord.Client.Name,
                    supplier = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().Supplier.Name,
                    supplierId = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Id
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().Supplier.Id,
                    accepted = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().StatusId
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().StatusId,
                    category = s.CategoryId,
                    serviceType = s.ServiceType.Service,
                    s.ServiceTypeId,
                    TotalTime = s.AuthoTime.Value,
                    s.WorkOrderId,
                    WorkOrderServiceId = s.WorkOrderServiceId,
                    serviceID = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                r.Id
                            })
                        //: s.CategoryId == 7 s.WorkOrderService.CorporateAssistances
                        //.Select(r => new
                        //{
                        //    r.Id
                        //})
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 28 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                Id = 0
                            })
                }).Where(x => x.supplierId == userId && x.accepted == 1).ToList();
            var bundle = _context.Set<biz.premier.Entities.BundledService>()
                .Where(x => x.BundledServiceOrder.WorkOrderId == wo).Select(s => new
                {
                    s.Id,
                    Location = s.Location,
                    ServiceNumber = s.ServiceNumber + "/" + s.Service.Service,
                    service = s.WorkServicesId,
                    assignee = s.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault()
                        .AssigneeName,
                    partner = s.BundledServiceOrder.WorkOrder.ServiceRecord.Partner.Name,
                    client = s.BundledServiceOrder.WorkOrder.ServiceRecord.Client.Name,
                    supplier = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault()
                            .Supplier.Name
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault()
                            .Supplier.Name,
                    supplierId = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Id
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().Supplier.Id,
                    accepted = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().StatusId
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().StatusId,
                    category = s.CategoryId,
                    serviceType = s.ServiceType.Service,
                    s.ServiceTypeId,
                    TotalTime = Convert.ToInt32(s.BundledServiceOrder.TotalTime),
                    s.BundledServiceOrder.WorkOrderId,
                    WorkOrderServiceId = s.WorkServicesId,
                    serviceID = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                r.Id
                            })
                        //: s.CategoryId == 7 s.WorkServices.CorporateAssistances
                        //.Select(r => new
                        //{
                        //    r.Id
                        //})
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.CategoryId == 28 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                r.Id
                            })
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                Id = 0
                            })
                }).Where(x => x.supplierId == userId && x.accepted == 1).ToList().Union(standalone);
            return new ObjectResult(bundle.OrderBy(x => x.ServiceNumber));
        }

        public ActionResult GetServiceByServiceLine(int sr, int sl, int idUser)
        {

            var userId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == idUser).Id;
            var standalone = _context.Set<biz.premier.Entities.StandaloneServiceWorkOrder>()
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == sl).Select(s => new
                {
                    s.Id,
                    Location = s.Location,
                    ServiceNumber = (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                        && f.IdService == s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                        && f.IdService == s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).NickName),
                    service = s.WorkOrderServiceId,
                    to = _context.DependentInformations.FirstOrDefault(x => x.Id == s.DeliveredTo).Relationship.Relationship,
                    tipo = "standalone",
                    school_id = _context.SchoolingSearches.Any(x => x.WorkOrderServicesId == s.WorkOrderServiceId) ?
                    _context.SchoolingSearches.FirstOrDefault(x => x.WorkOrderServicesId == s.WorkOrderServiceId).Id :
                    0,
                    assignee = s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    partner = s.WorkOrder.ServiceRecord.Partner.Name,
                    client = s.WorkOrder.ServiceRecord.Client.Name,
                    supplier = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Name
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Name,
                    supplierId = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id,
                    accepted = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId,
                    category = s.CategoryId,
                    serviceType = s.ServiceType.Service,
                    s.ServiceTypeId,
                    TotalTime = s.AuthoTime.Value,
                    TotalRemaining = _context.ServiceReportDays.FirstOrDefault(f => f.Service == s.Id) == null
                    ? s.AuthoTime.Value : _context.ServiceReportDays.OrderByDescending(x => x.Id).FirstOrDefault(f => f.Service == s.Id).TimeReminder,
                    s.WorkOrderId,
                    WorkOrderServiceId = s.WorkOrderServiceId,
                    s.StatusId

                })
                .Join(_context.AssignedServiceSupliers, x => x.WorkOrderServiceId, y => y.ServiceOrderServicesId,
                (x, y) => new
                {
                    x.Id,
                    x.Location,
                    x.ServiceNumber,
                    x.service,
                    x.to,
                    x.tipo,
                    x.school_id,
                    x.assignee,
                    x.partner,
                    x.client,
                    x.supplier,
                    x.supplierId,
                    x.accepted,
                    x.category,
                    x.serviceType,
                    x.ServiceTypeId,
                    x.TotalTime,
                    x.TotalRemaining,
                    x.WorkOrderId,
                    x.WorkOrderServiceId,
                    x.StatusId
                }).ToList();
            var bundle = _context.Set<biz.premier.Entities.BundledService>()
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == sl).Select(s => new
                {
                    s.Id,
                    Location = s.Location,
                    ServiceNumber = s.ServiceNumber + "-" + (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                      && f.IdService == s.ServiceId).NickName),
                    service = s.WorkServicesId,
                    to = _context.DependentInformations.FirstOrDefault(x => x.Id == s.DeliveredTo).Relationship.Relationship,
                    tipo = "bundle",
                    school_id = _context.SchoolingSearches.Any(x => x.WorkOrderServicesId == s.WorkServicesId) ?
                    _context.SchoolingSearches.FirstOrDefault(x => x.WorkOrderServicesId == s.WorkServicesId).Id :
                    0,
                    assignee = s.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault()
                        .AssigneeName,
                    partner = s.BundledServiceOrder.WorkOrder.ServiceRecord.Partner.Name,
                    client = s.BundledServiceOrder.WorkOrder.ServiceRecord.Client.Name,
                    supplier = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId)
                            .Supplier.Name
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId)
                            .Supplier.Name,
                    supplierId = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id,
                    accepted = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId,
                    category = s.CategoryId,
                    serviceType = s.ServiceType.Service,
                    s.ServiceTypeId,
                    TotalTime = Convert.ToInt32(s.BundledServiceOrder.TotalTime),
                    TotalRemaining = _context.ServiceReportDays.FirstOrDefault(f => f.Service == s.Id) == null
                    ? Convert.ToInt32(s.BundledServiceOrder.TotalTime)
                    : _context.ServiceReportDays.OrderByDescending(x => x.Id).FirstOrDefault(f => f.Service == s.Id).TimeReminder,
                    s.BundledServiceOrder.WorkOrderId,
                    WorkOrderServiceId = s.WorkServicesId,
                    s.StatusId

                })
                .Join(_context.AssignedServiceSupliers, x => x.WorkOrderServiceId, y => y.ServiceOrderServicesId,
                (x, y) => new
                {
                    x.Id,
                    x.Location,
                    x.ServiceNumber,
                    x.service,
                    x.to,
                    x.tipo,
                    x.school_id,
                    x.assignee,
                    x.partner,
                    x.client,
                    x.supplier,
                    x.supplierId,
                    x.accepted,
                    x.category,
                    x.serviceType,
                    x.ServiceTypeId,
                    x.TotalTime,
                    x.TotalRemaining,
                    x.WorkOrderId,
                    x.WorkOrderServiceId,
                    x.StatusId
                }).ToList();

            var union = standalone.Union(bundle);
            return new ObjectResult(union.OrderBy(x => x.ServiceNumber));
        }

        public ActionResult GetServiceByServiceLineApp(int sr, int sl, int idUser)
        {

            var userId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == idUser).Id;
            var standalone = _context.Set<biz.premier.Entities.StandaloneServiceWorkOrder>()
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == sl).Select(s => new
                {
                    s.Id,
                    Location = s.Location,
                    ServiceNumber = s.ServiceNumber,
                    ServiceName = (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                        && f.IdService == s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                        && f.IdService == s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).NickName),
                    service = s.WorkOrderServiceId,
                    to = _context.DependentInformations.FirstOrDefault(x => x.Id == s.DeliveredTo).Relationship.Relationship,
                    tipo = "standalone",
                    school_id = _context.SchoolingSearches.Any(x => x.WorkOrderServicesId == s.WorkOrderServiceId) ?
                    _context.SchoolingSearches.FirstOrDefault(x => x.WorkOrderServicesId == s.WorkOrderServiceId).Id :
                    0,
                    assignee = s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    partner = s.WorkOrder.ServiceRecord.Partner.Name,
                    client = s.WorkOrder.ServiceRecord.Client.Name,
                    supplier = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Name
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Name,
                    supplierId = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id,
                    accepted = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId
                        : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId,
                    category = s.CategoryId,
                    serviceType = s.ServiceType.Service,
                    s.ServiceTypeId,
                    TotalTime = s.AuthoTime.Value,
                    TotalRemaining = _context.ServiceReportDays.FirstOrDefault(f => f.Service == s.Id) == null
                    ? s.AuthoTime.Value : _context.ServiceReportDays.OrderByDescending(x => x.Id).FirstOrDefault(f => f.Service == s.Id).TimeReminder,
                    s.WorkOrderId,
                    WorkOrderServiceId = s.WorkOrderServiceId,
                    s.StatusId

                })
                .Join(_context.AssignedServiceSupliers, x => x.WorkOrderServiceId, y => y.ServiceOrderServicesId,
                (x, y) => new
                {
                    x.Id,
                    x.Location,
                    x.ServiceName,
                    x.ServiceNumber,
                    x.service,
                    x.to,
                    x.tipo,
                    x.school_id,
                    x.assignee,
                    x.partner,
                    x.client,
                    x.supplier,
                    x.supplierId,
                    x.accepted,
                    x.category,
                    x.serviceType,
                    x.ServiceTypeId,
                    x.TotalTime,
                    x.TotalRemaining,
                    x.WorkOrderId,
                    x.WorkOrderServiceId,
                    x.StatusId
                }).ToList();
            var bundle = _context.Set<biz.premier.Entities.BundledService>()
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == sl).Select(s => new
                {
                    s.Id,
                    Location = s.Location,
                    ServiceName = (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                       && f.IdService == s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.BundledServiceOrder.WorkOrder.ServiceRecord.ClientId
                      && f.IdService == s.ServiceId).NickName),
                    ServiceNumber = s.ServiceNumber,
                    service = s.WorkServicesId,
                    to = _context.DependentInformations.FirstOrDefault(x => x.Id == s.DeliveredTo).Relationship.Relationship,
                    tipo = "bundle",
                    school_id = _context.SchoolingSearches.Any(x => x.WorkOrderServicesId == s.WorkServicesId) ?
                    _context.SchoolingSearches.FirstOrDefault(x => x.WorkOrderServicesId == s.WorkServicesId).Id :
                    0,
                    assignee = s.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault()
                        .AssigneeName,
                    partner = s.BundledServiceOrder.WorkOrder.ServiceRecord.Partner.Name,
                    client = s.BundledServiceOrder.WorkOrder.ServiceRecord.Client.Name,
                    supplier = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId)
                            .Supplier.Name
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId)
                            .Supplier.Name,
                    supplierId = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).Supplier.Id,
                    accepted = s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any(x => x.Supplier.Id == userId)
                        ? s.BundledServiceOrder.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId
                        : s.BundledServiceOrder.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault(x => x.Supplier.Id == userId).StatusId,
                    category = s.CategoryId,
                    serviceType = s.ServiceType.Service,
                    s.ServiceTypeId,
                    TotalTime = Convert.ToInt32(s.BundledServiceOrder.TotalTime),
                    TotalRemaining = _context.ServiceReportDays.FirstOrDefault(f => f.Service == s.Id) == null
                    ? Convert.ToInt32(s.BundledServiceOrder.TotalTime)
                    : _context.ServiceReportDays.OrderByDescending(x => x.Id).FirstOrDefault(f => f.Service == s.Id).TimeReminder,
                    s.BundledServiceOrder.WorkOrderId,
                    WorkOrderServiceId = s.WorkServicesId,
                    s.StatusId

                })
                .Join(_context.AssignedServiceSupliers, x => x.WorkOrderServiceId, y => y.ServiceOrderServicesId,
                (x, y) => new
                {
                    x.Id,
                    x.Location,
                    x.ServiceName,
                    x.ServiceNumber,
                    x.service,
                    x.to,
                    x.tipo,
                    x.school_id,
                    x.assignee,
                    x.partner,
                    x.client,
                    x.supplier,
                    x.supplierId,
                    x.accepted,
                    x.category,
                    x.serviceType,
                    x.ServiceTypeId,
                    x.TotalTime,
                    x.TotalRemaining,
                    x.WorkOrderId,
                    x.WorkOrderServiceId,
                    x.StatusId
                }).ToList();

            var union = standalone.Union(bundle);
            return new ObjectResult(union.OrderBy(x => x.ServiceNumber));
        }

        public bool AcceptOrRejectCoordinator(int coordinator, bool accepted, int sr)
        {
            Boolean _value_step = true;
            var _coordinator = _context.ProfileUsers.FirstOrDefault(x => x.UserId == coordinator).Id;
            var immigration = _context.ImmigrationCoodinators
                .SingleOrDefault(s => s.CoordinatorId == _coordinator && s.ServiceRecordId == sr);

            var _service_record =
                       _context.ServiceRecords.SingleOrDefault(x => x.Id == immigration.ServiceRecordId);

            if (immigration != null)
            {
                if (accepted)
                {
                    immigration.Accepted = DateTime.Now;
                    immigration.StatusId = 1;

                    var status_supplier_partner_imm =
                       _context.ImmigrationSupplierPartners.Where(x =>
                           x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    var status_supplier_partner_relo =
                        _context.RelocationSupplierPartners.Where(x =>
                            x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    var status_coordinator_relo =
                        _context.RelocationCoordinators.Where(x =>
                            x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    for (int i = 0; i < status_supplier_partner_imm.Count(); i++)
                    {
                        if (status_supplier_partner_imm != null)
                        {
                            if (status_supplier_partner_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_supplier_partner_relo.Count(); i++)
                    {
                        if (status_supplier_partner_relo != null)
                        {
                            if (status_supplier_partner_relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_coordinator_relo.Count(); i++)
                    {
                        if (status_coordinator_relo != null)
                        {
                            if (status_coordinator_relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    if (_value_step)
                    {
                        _service_record.StatusId = 2;
                        _context.ServiceRecords.Update(_service_record);

                        var ImmigrationSupplierPartnerId = _context.ImmigrationSupplierPartners
                            .Where(x => x.ServiceRecordId == _service_record.Id).ToList();

                        for (int z = 0; z < ImmigrationSupplierPartnerId.Count(); z++)
                        {
                            var assigment = _context.AssignedServiceSupliers
                                .Where(x => x.ImmigrationSupplierPartnerId == ImmigrationSupplierPartnerId[z].Id).ToList();

                            for (int i = 0; i < assigment.Count(); i++)
                            {
                                var standAlone = _context.StandaloneServiceWorkOrders
                                    .FirstOrDefault(x => x.WorkOrderServiceId == assigment[i].ServiceOrderServicesId);

                                var bundle = _context.BundledServices
                                    .FirstOrDefault(x => x.WorkServicesId == assigment[i].ServiceOrderServicesId);

                                if (standAlone != null)
                                {
                                    standAlone.StatusId = 39;
                                    change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 39);
                                    _context.StandaloneServiceWorkOrders.Update(standAlone);
                                }

                                if (bundle != null)
                                {
                                    bundle.StatusId = 39;
                                    change_detail_status_byWos_id(bundle.WorkServicesId.Value, standAlone.ServiceId.Value, 39);
                                    _context.BundledServices.Update(bundle);
                                }
                            }
                        }
                    }

                    if (!_value_step)
                    {
                        _service_record.StatusId = 18;
                        _context.ServiceRecords.Update(_service_record);
                    }
                }
                else
                {
                    immigration.Accepted = Convert.ToDateTime("1900/01/01");
                    immigration.StatusId = 3;

                    _service_record.StatusId = 18;
                    _context.ServiceRecords.Update(_service_record);
                }

                _context.ImmigrationCoodinators.Update(immigration);
                _context.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool AccpetOrRejectRelocationCoordinator(int coordinator, bool accepted, int sr)
        {
            Boolean _value_step = true;
            var _coordinator = _context.ProfileUsers.FirstOrDefault(x => x.UserId == coordinator).Id;
            var relocation = _context.RelocationCoordinators
                .SingleOrDefault(s => s.CoordinatorId == _coordinator && s.ServiceRecordId == sr);

            var _service_record =
                       _context.ServiceRecords.SingleOrDefault(x => x.Id == relocation.ServiceRecordId);

            if (relocation != null)
            {
                if (accepted)
                {
                    relocation.Accepted = DateTime.Now;
                    relocation.StatusId = 1;

                    var status_supplier_partner_relo =
                        _context.RelocationSupplierPartners.Where(x =>
                            x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    var status_supplier_partner_imm =
                       _context.ImmigrationSupplierPartners.Where(x =>
                           x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    var status_coordinator_imm =
                      _context.ImmigrationCoodinators.Where(x =>
                          x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    for (int i = 0; i < status_supplier_partner_relo.Count(); i++)
                    {
                        if (status_supplier_partner_relo != null)
                        {
                            if (status_supplier_partner_relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_supplier_partner_imm.Count(); i++)
                    {
                        if (status_supplier_partner_imm != null)
                        {
                            if (status_supplier_partner_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_coordinator_imm.Count(); i++)
                    {
                        if (status_coordinator_imm != null)
                        {
                            if (status_coordinator_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    if (_value_step)
                    {
                        _service_record.StatusId = 2;
                        _context.ServiceRecords.Update(_service_record);

                        var relocationSupplierPartners = _context.RelocationSupplierPartners
                            .Where(x => x.ServiceRecordId == _service_record.Id).ToList();

                        for (int z = 0; z < relocationSupplierPartners.Count(); z++)
                        {
                            var assigment = _context.AssignedServiceSupliers
                                .Where(x => x.RelocationSupplierPartnerId == relocationSupplierPartners[z].Id).ToList();

                            for (int i = 0; i < assigment.Count(); i++)
                            {
                                var standAlone = _context.StandaloneServiceWorkOrders
                                    .FirstOrDefault(x => x.WorkOrderServiceId == assigment[i].ServiceOrderServicesId);

                                var bundle = _context.BundledServices
                                    .FirstOrDefault(x => x.WorkServicesId == assigment[i].ServiceOrderServicesId);

                                if (standAlone != null)
                                {
                                    standAlone.StatusId = 39;
                                    change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 39);
                                    _context.StandaloneServiceWorkOrders.Update(standAlone);
                                }

                                if (bundle != null)
                                {
                                    bundle.StatusId = 39;
                                    change_detail_status_byWos_id(bundle.WorkServicesId.Value, standAlone.ServiceId.Value, 39);
                                    _context.BundledServices.Update(bundle);
                                }
                            }
                        }
                    }


                    if (!_value_step)
                    {
                        _service_record.StatusId = 18;
                        _context.ServiceRecords.Update(_service_record);
                    }

                }
                else
                {
                    relocation.Accepted = Convert.ToDateTime("1900/01/01");
                    relocation.StatusId = 3;

                    _service_record.StatusId = 18;
                    _context.ServiceRecords.Update(_service_record);
                }

                _context.RelocationCoordinators.Update(relocation);

                _context.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool AccpetImmigrationSupplierPartner(int supplier, bool accepted, int sr)
        {
            Boolean _value_step = true;
            var _supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == supplier).Id;
            var immigration = _context.ImmigrationSupplierPartners
                .SingleOrDefault(s => s.SupplierId == _supplier && s.ServiceRecordId == sr);

            var _service_record =
                        _context.ServiceRecords.SingleOrDefault(x => x.Id == immigration.ServiceRecordId);

            if (immigration != null)
            {
                if (accepted)
                {
                    immigration.AcceptedDate = DateTime.Now;
                    immigration.StatusId = 1;

                    var status_coordinator_imm =
                        _context.ImmigrationCoodinators.Where(x =>
                            x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    var status_coordinator_Relo =
                       _context.RelocationCoordinators.Where(x =>
                           x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    var status_supplier_Relo =
                      _context.RelocationSupplierPartners.Where(x =>
                          x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    for (int i = 0; i < status_coordinator_imm.Count(); i++)
                    {
                        if (status_coordinator_imm != null)
                        {
                            if (status_coordinator_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_coordinator_Relo.Count(); i++)
                    {
                        if (status_coordinator_Relo != null)
                        {
                            if (status_coordinator_Relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_supplier_Relo.Count(); i++)
                    {
                        if (status_supplier_Relo != null)
                        {
                            if (status_supplier_Relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    if (_value_step)
                    {
                        _service_record.StatusId = 2;
                        _context.ServiceRecords.Update(_service_record);

                        var assigment = _context.AssignedServiceSupliers.Where(x => x.ImmigrationSupplierPartnerId == immigration.Id).ToList();

                        for (int i = 0; i < assigment.Count(); i++)
                        {
                            var standAlone = _context.StandaloneServiceWorkOrders
                                .FirstOrDefault(x => x.WorkOrderServiceId == assigment[i].ServiceOrderServicesId);

                            var bundle = _context.BundledServices
                                .FirstOrDefault(x => x.WorkServicesId == assigment[i].ServiceOrderServicesId);

                            if (standAlone != null)
                            {
                                standAlone.StatusId = 39;
                                change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 39);
                                _context.StandaloneServiceWorkOrders.Update(standAlone);
                            }

                            if (bundle != null)
                            {
                                bundle.StatusId = 39;
                                change_detail_status_byWos_id(bundle.WorkServicesId.Value, standAlone.ServiceId.Value, 39);
                                _context.BundledServices.Update(bundle);
                            }
                        }
                    }


                    if (!_value_step)
                    {
                        _service_record.StatusId = 18;
                        _context.ServiceRecords.Update(_service_record);
                    }
                }
                else
                {
                    immigration.AcceptedDate = Convert.ToDateTime("1900/01/01");
                    immigration.StatusId = 3;

                    _service_record.StatusId = 18;
                    _context.ServiceRecords.Update(_service_record);
                }

                _context.ImmigrationSupplierPartners.Update(immigration);

                _context.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool AccpetImmigrationSupplierPartnerIndividual(int supplier, bool accepted, int sr, int ServiceOrderServicesId)
        {
            Boolean _value_step = true;
            var _supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == supplier).Id;
            var immigration = _context.ImmigrationSupplierPartners
                .SingleOrDefault(s => s.SupplierId == _supplier && s.ServiceRecordId == sr);

            var _service_record =
                        _context.ServiceRecords.SingleOrDefault(x => x.Id == immigration.ServiceRecordId);

            if (immigration != null)
            {
                if (accepted)
                {
                    immigration.AcceptedDate = DateTime.Now;
                    immigration.StatusId = 1;

                    var status_coordinator_imm =
                        _context.ImmigrationCoodinators.Where(x =>
                            x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    var status_coordinator_Relo =
                       _context.RelocationCoordinators.Where(x =>
                           x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    var status_supplier_Relo =
                      _context.RelocationSupplierPartners.Where(x =>
                          x.ServiceRecordId == immigration.ServiceRecordId).ToList();

                    for (int i = 0; i < status_coordinator_imm.Count(); i++)
                    {
                        if (status_coordinator_imm != null)
                        {
                            if (status_coordinator_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_coordinator_Relo.Count(); i++)
                    {
                        if (status_coordinator_Relo != null)
                        {
                            if (status_coordinator_Relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_supplier_Relo.Count(); i++)
                    {
                        if (status_supplier_Relo != null)
                        {
                            if (status_supplier_Relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    if (_value_step)
                    {
                        _service_record.StatusId = 2;
                        _context.ServiceRecords.Update(_service_record);

                        var standAlone = _context.StandaloneServiceWorkOrders
                                .FirstOrDefault(x => x.WorkOrderServiceId == ServiceOrderServicesId);

                        var bundle = _context.BundledServices
                            .FirstOrDefault(x => x.WorkServicesId == ServiceOrderServicesId);

                        if (standAlone != null)
                        {
                            standAlone.StatusId = 39;
                            change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 39);
                            _context.StandaloneServiceWorkOrders.Update(standAlone);
                        }

                        if (bundle != null)
                        {
                            bundle.StatusId = 39;
                            change_detail_status_byWos_id(bundle.WorkServicesId.Value, standAlone.ServiceId.Value, 39);
                            _context.BundledServices.Update(bundle);
                        }
                    }


                    if (!_value_step)
                    {
                        _service_record.StatusId = 18;
                        _context.ServiceRecords.Update(_service_record);
                    }
                }
                else
                {
                    immigration.AcceptedDate = Convert.ToDateTime("1900/01/01");
                    immigration.StatusId = 3;

                    _service_record.StatusId = 18;
                    _context.ServiceRecords.Update(_service_record);
                }

                _context.ImmigrationSupplierPartners.Update(immigration);

                _context.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool AccpetRelocationSupplierPartner(int supplier, bool accepted, int sr)
        {
            Boolean _value_step = true;
            var _supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == supplier).Id;
            var relocation = _context.RelocationSupplierPartners
                .SingleOrDefault(s => s.SupplierId == _supplier && s.ServiceRecordId == sr && s.StatusId != 4);

            var _service_record =
                       _context.ServiceRecords.SingleOrDefault(x => x.Id == relocation.ServiceRecordId);

            if (relocation != null)
            {
                if (accepted)
                {
                    relocation.AcceptedDate = DateTime.Now;
                    relocation.StatusId = 1;

                    var status_coordinator_relo =
                        _context.RelocationCoordinators.Where(x =>
                            x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    var status_coordinator_imm =
                       _context.ImmigrationCoodinators.Where(x =>
                           x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    var status_supplier_imm =
                       _context.ImmigrationSupplierPartners.Where(x =>
                           x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    for (int i = 0; i < status_coordinator_relo.Count(); i++)
                    {
                        if (status_coordinator_relo != null)
                        {
                            if (status_coordinator_relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_coordinator_imm.Count(); i++)
                    {
                        if (status_coordinator_imm != null)
                        {
                            if (status_coordinator_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_supplier_imm.Count(); i++)
                    {
                        if (status_supplier_imm != null)
                        {
                            if (status_supplier_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    if (_value_step)
                    {
                        _service_record.StatusId = 2;
                        _context.ServiceRecords.Update(_service_record);

                        var assigment = _context.AssignedServiceSupliers.Where(x => x.RelocationSupplierPartnerId == relocation.Id).ToList();

                        for (int i = 0; i < assigment.Count(); i++)
                        {
                            var standAlone = _context.StandaloneServiceWorkOrders
                                .FirstOrDefault(x => x.WorkOrderServiceId == assigment[i].ServiceOrderServicesId);

                            var bundle = _context.BundledServices
                                .FirstOrDefault(x => x.WorkServicesId == assigment[i].ServiceOrderServicesId);

                            if (standAlone != null)
                            {
                                standAlone.StatusId = 39;
                                change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 39);
                                _context.StandaloneServiceWorkOrders.Update(standAlone);
                            }

                            if (bundle != null)
                            {
                                bundle.StatusId = 39;
                                change_detail_status_byWos_id(bundle.WorkServicesId.Value, bundle.ServiceId.Value, 39);
                                _context.BundledServices.Update(bundle);
                            }
                        }
                    }


                    if (!_value_step)
                    {
                        _service_record.StatusId = 18;
                        _context.ServiceRecords.Update(_service_record);
                    }
                }
                else
                {
                    relocation.AcceptedDate = Convert.ToDateTime("1900/01/01");
                    relocation.StatusId = 3;

                    _service_record.StatusId = 18;
                    _context.ServiceRecords.Update(_service_record);
                }

                _context.RelocationSupplierPartners.Update(relocation);

                _context.SaveChanges();
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool AccpetRelocationSupplierPartnerIndividual(int supplier, bool accepted, int sr, int ServiceOrderServicesId)
        {
            Boolean _value_step = true;
            var _supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == supplier).Id;
            var relocation = _context.RelocationSupplierPartners
                .SingleOrDefault(s => s.SupplierId == _supplier && s.ServiceRecordId == sr);

            var _service_record =
                       _context.ServiceRecords.SingleOrDefault(x => x.Id == relocation.ServiceRecordId);

            if (relocation != null)
            {
                if (accepted)
                {
                    relocation.AcceptedDate = DateTime.Now;
                    relocation.StatusId = 1;

                    var status_coordinator_relo =
                        _context.RelocationCoordinators.Where(x =>
                            x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    var status_coordinator_imm =
                       _context.ImmigrationCoodinators.Where(x =>
                           x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    var status_supplier_imm =
                       _context.ImmigrationSupplierPartners.Where(x =>
                           x.ServiceRecordId == relocation.ServiceRecordId).ToList();

                    for (int i = 0; i < status_coordinator_relo.Count(); i++)
                    {
                        if (status_coordinator_relo != null)
                        {
                            if (status_coordinator_relo[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_coordinator_imm.Count(); i++)
                    {
                        if (status_coordinator_imm != null)
                        {
                            if (status_coordinator_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    for (int i = 0; i < status_supplier_imm.Count(); i++)
                    {
                        if (status_supplier_imm != null)
                        {
                            if (status_supplier_imm[i].StatusId == null)
                            {
                                _value_step = false;
                            }
                        }
                    }

                    if (_value_step)
                    {
                        _service_record.StatusId = 2;
                        _context.ServiceRecords.Update(_service_record);

                        //var assigment = _context.AssignedServiceSupliers.Where(x => x.RelocationSupplierPartnerId == relocation.Id).ToList();

                        var standAlone = _context.StandaloneServiceWorkOrders
                                .FirstOrDefault(x => x.WorkOrderServiceId == ServiceOrderServicesId);

                        var bundle = _context.BundledServices
                            .FirstOrDefault(x => x.WorkServicesId == ServiceOrderServicesId);

                        if (standAlone != null)
                        {
                            standAlone.StatusId = 39;
                            change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 39);
                            _context.StandaloneServiceWorkOrders.Update(standAlone);
                        }

                        if (bundle != null)
                        {
                            bundle.StatusId = 39;
                            change_detail_status_byWos_id(bundle.WorkServicesId.Value, bundle.ServiceId.Value, 39);
                            _context.BundledServices.Update(bundle);
                        }
                    }


                    if (!_value_step)
                    {
                        _service_record.StatusId = 18;
                        _context.ServiceRecords.Update(_service_record);
                    }
                }
                else
                {
                    relocation.AcceptedDate = Convert.ToDateTime("1900/01/01");
                    relocation.StatusId = 3;

                    _service_record.StatusId = 18;
                    _context.ServiceRecords.Update(_service_record);

                    var standAlone = _context.StandaloneServiceWorkOrders
                                .FirstOrDefault(x => x.WorkOrderServiceId == ServiceOrderServicesId);

                    var bundle = _context.BundledServices
                        .FirstOrDefault(x => x.WorkServicesId == ServiceOrderServicesId);

                    if (standAlone != null)
                    {
                        standAlone.StatusId = 3;
                        change_detail_status_byWos_id(standAlone.WorkOrderServiceId.Value, standAlone.ServiceId.Value, 3);
                        _context.StandaloneServiceWorkOrders.Update(standAlone);
                    }

                    if (bundle != null)
                    {
                        bundle.StatusId = 3;
                        change_detail_status_byWos_id(bundle.WorkServicesId.Value, bundle.ServiceId.Value, 3);
                        _context.BundledServices.Update(bundle);
                    }
                }


                _context.RelocationSupplierPartners.Update(relocation);
                _context.SaveChanges();

                var relocation_valid = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.RelocationSupplierPartnerId,
                            stand.StatusId
                        }).Where(w => w.imm_relo == relocation.Id).ToList();
                
                
                for(var i = 0; i < relocation_valid.Count; i++)
                {
                    if (relocation_valid[i].StatusId == 38 || relocation_valid[i].StatusId == 3)
                    {
                        relocation.StatusId = null;
                        break;
                    }
                }

                if (relocation.StatusId == null)
                {
                    _context.RelocationSupplierPartners.Update(relocation);
                    _context.SaveChanges();
                }

            }
            else
            {
                return false;
            }

            return true;
        }

        public List<biz.premier.Models.DashboardDto> GetDashboardTest(int user, string serviceLine, int? country, int? city, int? partner, int? client,
            int? coordinator, int? supplier, int? status, DateTime? rangeDate1, DateTime? rangeDate2)
        {
            int profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user).Id;

            var service_record = _context.ServiceRecords
                .Select(s => new biz.premier.Models.DashboardDto
                {
                    id = s.Id,
                    numberServiceRecord = s.NumberServiceRecord,
                    vip = s.Vip.Value,
                    confidentialMove = s.ConfidentialMove.Value,
                    urgent = s.Urgent.Value,
                    status = "",
                    statusId = s.StatusId.Value,
                    initialAutho = s.InitialAutho,
                    serviceLineId = s.WorkOrders.FirstOrDefault().ServiceLineId.Value,
                    workOrders = s.WorkOrders.Count(),
                    serviceLine = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                    total_services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count()
                    + _context.BundledServices.Where(w => w.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count(),
                    homeCountry = s.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                    hostCountry = s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                    homeCity = s.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                    hostCity = s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    homeCountryId = s.AssigneeInformations.FirstOrDefault().HomeCountryId.Value,
                    hostCountryId = s.AssigneeInformations.FirstOrDefault().HostCountry.Value,
                    homeCityId = s.AssigneeInformations.FirstOrDefault().HomeCityId.Value,
                    hostCityId = s.AssigneeInformations.FirstOrDefault().HostCityId.Value,
                    name = s.Partner.Name,
                    partnerId = s.PartnerId.Value,
                    partnerAvatar = s.Partner.Photo,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    clientAvatar = s.Client.Photo == null || s.Client.Photo == "" ? "Files/assets/avatar.png" : s.Client.Photo,
                    assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName,
                    //AssigneeInformations = s.AssigneeInformations
                    //.Select(c => new
                    //{
                    //    c.Id,
                    //    c.PetsNavigation,
                    //    DependentInformations = c.DependentInformations.Select(f => new {
                    //        f.Name,
                    //        f.Phone,
                    //        f.Photo,
                    //        f.Relationship.Relationship
                    //    }).Where(r => r.Relationship != "Assignee").ToList()

                    //}).ToList(),
                    //coordinatorImm = s.ImmigrationCoodinators
                    //.Where(x => x.StatusId != 4).Select(c => new
                    //{
                    //    IdCoodinator = c.Id,
                    //    Id = c.CoordinatorId,
                    //    c.Coordinator.Name,
                    //    avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                    //    Accepted = c.StatusId,
                    //    c.CoordinatorType.CoordinatorType,
                    //}),
                    //coordinatorRelo = s.RelocationCoordinators
                    //.Where(x => x.StatusId != 4).Select(c => new
                    //{
                    //    IdCoodinator = c.Id,
                    //    Id = c.CoordinatorId,
                    //    c.Coordinator.Name,
                    //    avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                    //    Accepted = c.StatusId,
                    //    c.CoordinatorType.CoordinatorType,
                    //}),
                    //coordinatorimmId = s.ImmigrationCoodinators.Select(c => c.CoordinatorId),
                    //coordinatorreloId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
                    //supplierImm = s.ImmigrationSupplierPartners
                    //.Where(x => x.StatusId != 4).Select(c => new
                    //{
                    //    IdSupplier = c.Id,
                    //    Id = c.SupplierId,
                    //    UserId = c.Supplier.UserId,
                    //    c.Supplier.Name,
                    //    c.Supplier.User.Email,
                    //    country = c.SupplierType.Name,
                    //    avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                    //    Accepted = c.StatusId == null ? 2 : c.StatusId,
                    //    total_services = _context.StandaloneServiceWorkOrders
                    //    .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                    //    (stand, assig) => new
                    //    {
                    //        imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                    //    }).Where(w => w.imm_relo == c.Id).Count()
                    //    +
                    //    _context.BundledServices
                    //    .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                    //    (stand, assig) => new
                    //    {
                    //        imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                    //    }).Where(w => w.imm_relo == c.Id).Count()
                    //}),
                    //supplierRelo = s.RelocationSupplierPartners
                    //.Where(x => x.StatusId != 4).Select(c => new
                    //{
                    //    IdSupplier = c.Id,
                    //    Id = c.SupplierId,
                    //    UserId = c.Supplier.UserId,
                    //    c.Supplier.Name,
                    //    c.Supplier.User.Email,
                    //    country = c.SupplierType.Name,
                    //    avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                    //    Accepted = c.StatusId == null ? 2 : c.StatusId,
                    //    total_services = _context.StandaloneServiceWorkOrders
                    //    .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                    //    (stand, assig) => new
                    //    {
                    //        imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                    //    }).Where(w => w.imm_relo == c.Id).Count()
                    //    +
                    //    _context.BundledServices
                    //    .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                    //    (stand, assig) => new
                    //    {
                    //        imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                    //    }).Where(w => w.imm_relo == c.Id).Count()

                    //}),
                    //supplierimmId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                    //supplierreloId = s.RelocationSupplierPartners.Select(c => c.SupplierId),
                    //total_services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count()
                    //+ _context.BundledServices.Where(w => w.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count()

                }).ToList();

            service_record = service_record.Where(x => x.total_services > 0).ToList();
            ////relo_service_record = relo_service_record.Where(x => (x.standalone.Count() > 0) || (x.bundled.Count() > 0)).ToList();
            ////var service_record = imm_service_record.Union(relo_service_record).ToList();

            ////Query por Role
            //int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;

            //switch (role)
            //{
            //    case 2: // Coordinator
            //        service_record = service_record.Where(a => a.coordinatorimmId.Contains(profileId) || a.coordinatorreloId.Contains(profileId)).ToList();
            //        break;
            //    case 3: // Supplier
            //        service_record = service_record.Where(a => a.supplierimmId.Contains(profileId) || a.supplierreloId.Contains(profileId)).ToList();
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}

            //if (serviceLine != null)
            //    service_record = service_record.Where(x => x?.serviceLine?.ToLower() == serviceLine.ToString().ToLower()).ToList();
            //if (country.HasValue)
            //    service_record = service_record.Where(x => x.homeCountryId == country.Value || x.hostCountryId == country).ToList();
            //if (city.HasValue)
            //    service_record = service_record.Where(x => x.homeCityId == city.Value || x.hostCityId == city).ToList();
            //if (partner.HasValue)
            //    service_record = service_record.Where(x => x.PartnerId == partner.Value).ToList();
            //if (client.HasValue)
            //    service_record = service_record.Where(x => x.ClientId == client.Value).ToList();
            //if (coordinator.HasValue)
            //    service_record = service_record.Where(x => x.coordinatorimmId.Contains(coordinator) || x.coordinatorreloId.Contains(coordinator)).ToList();
            //if (supplier.HasValue)
            //    service_record = service_record.Where(x => x.supplierimmId.Contains(supplier) || x.supplierreloId.Contains(supplier)).ToList();
            //if (status.HasValue)
            //    service_record = service_record.Where(x => x.StatusId == status.Value).ToList();
            //if (status == 99)
            //    service_record = service_record.Where(x => x.Vip == true).ToList();
            //if (rangeDate1.HasValue && rangeDate2.HasValue)
            //    service_record = service_record
            //        .Where(x => x.InitialAutho >= rangeDate1.Value && x.InitialAutho <= rangeDate2.Value)
            //        .ToList();

            return service_record;
        }

        public ActionResult GetDashboard(int user, string serviceLine, int? country, int? city, int? partner, int? client,
            int? coordinator, int? supplier, int? status, DateTime? rangeDate1, DateTime? rangeDate2)
        {
            int profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user).Id;

            var service_record = _context.ServiceRecords
                .Select(s => new
                {
                    Id = s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    s.ConfidentialMove,
                    s.Urgent,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    s.WorkOrders.FirstOrDefault().ServiceLineId,
                    WorkOrders = s.WorkOrders.Count(),
                    serviceLine = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                    homeCountry = s.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                    hostCountry = s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                    homeCity = s.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                    hostCity = s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    homeCountryId = s.AssigneeInformations.FirstOrDefault().HomeCountryId,
                    hostCountryId = s.AssigneeInformations.FirstOrDefault().HostCountry,
                    homeCityId = s.AssigneeInformations.FirstOrDefault().HomeCityId,
                    hostCityId = s.AssigneeInformations.FirstOrDefault().HostCityId,
                    s.Partner.Name,
                    s.PartnerId,
                    partnerAvatar = s.Partner.Photo,
                    client = s.Client.Name,
                    s.ClientId,
                    clientAvatar = s.Client.Photo == null || s.Client.Photo == "" ? "Files/assets/avatar.png" : s.Client.Photo,
                    s.AssigneeInformations.FirstOrDefault().AssigneeName,
                    AssigneeInformations = s.AssigneeInformations
                    .Select(c => new
                    {
                        c.Id,
                        c.PetsNavigation,
                        DependentInformations = c.DependentInformations.Select(f => new { 
                            f.Name,
                            f.Phone,
                            f.Photo,
                            f.Relationship.Relationship
                        }).Where(r => r.Relationship != "Assignee").ToList()
                       
                    }).ToList(),
                    coordinatorImm = s.ImmigrationCoodinators
                    .Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdCoodinator = c.Id,
                        Id = c.CoordinatorId,
                        c.Coordinator.Name,
                        avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                        Accepted = c.StatusId,
                        c.CoordinatorType.CoordinatorType,
                    }),
                    coordinatorRelo = s.RelocationCoordinators
                    .Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdCoodinator = c.Id,
                        Id = c.CoordinatorId,
                        c.Coordinator.Name,
                        avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                        Accepted = c.StatusId,
                        c.CoordinatorType.CoordinatorType,
                    }),
                    coordinatorimmId = s.ImmigrationCoodinators.Select(c => c.CoordinatorId),
                    coordinatorreloId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
                    supplierImm = s.ImmigrationSupplierPartners
                    .Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdSupplier = c.Id,
                        Id = c.SupplierId,
                        UserId = c.Supplier.UserId,
                        c.Supplier.Name,
                        c.Supplier.User.Email,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        Accepted = c.StatusId == null ? 2 : c.StatusId,
                        total_services = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()
                        +
                        _context.BundledServices
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()
                    }),
                    supplierRelo = s.RelocationSupplierPartners
                    .Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdSupplier = c.Id,
                        Id = c.SupplierId,
                        UserId = c.Supplier.UserId,
                        c.Supplier.Name,
                        c.Supplier.User.Email,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        Accepted = c.StatusId == null ? 2 : c.StatusId,
                        total_services = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()
                        +
                        _context.BundledServices
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == c.Id).Count()

                    }),
                    supplierimmId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                    supplierreloId = s.RelocationSupplierPartners.Select(c => c.SupplierId),
                    total_services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count()
                    + _context.BundledServices.Where(w => w.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count()
        
                }).ToList();

            service_record = service_record.Where(x => x.total_services > 0).ToList();
            //relo_service_record = relo_service_record.Where(x => (x.standalone.Count() > 0) || (x.bundled.Count() > 0)).ToList();
            //var service_record = imm_service_record.Union(relo_service_record).ToList();

            //Query por Role
            int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;

            switch (role)
            {
                case 2: // Coordinator
                    service_record = service_record.Where(a => a.coordinatorimmId.Contains(profileId) || a.coordinatorreloId.Contains(profileId)).ToList();
                    break;
                case 3: // Supplier
                    service_record = service_record.Where(a => a.supplierimmId.Contains(profileId) || a.supplierreloId.Contains(profileId)
                    && a.supplierRelo.FirstOrDefault().total_services > 0).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            if (serviceLine != null)
                service_record = service_record.Where(x => x?.serviceLine?.ToLower() == serviceLine.ToString().ToLower()).ToList();
            if (country.HasValue)
                service_record = service_record.Where(x => x.homeCountryId == country.Value || x.hostCountryId == country).ToList();
            if (city.HasValue)
                service_record = service_record.Where(x => x.homeCityId == city.Value || x.hostCityId == city).ToList();
            if (partner.HasValue)
                service_record = service_record.Where(x => x.PartnerId == partner.Value).ToList();
            if (client.HasValue)
                service_record = service_record.Where(x => x.ClientId == client.Value).ToList();
            if (coordinator.HasValue)
                service_record = service_record.Where(x => x.coordinatorimmId.Contains(coordinator) || x.coordinatorreloId.Contains(coordinator)).ToList();
            if (supplier.HasValue)
                service_record = service_record.Where(x => x.supplierimmId.Contains(supplier) || x.supplierreloId.Contains(supplier)).ToList();
            if (status.HasValue)
                service_record = service_record.Where(x => x.StatusId == status.Value).ToList();
            if (status == 99)
                service_record = service_record.Where(x => x.Vip == true).ToList();
            if (rangeDate1.HasValue && rangeDate2.HasValue)
                service_record = service_record
                    .Where(x => x.InitialAutho >= rangeDate1.Value && x.InitialAutho <= rangeDate2.Value)
                    .ToList();

            var counts = new
            {
                calendar = GetCalendarCount(),
                activityItems = ActivityItemsCount(user),
                reminders = GetRemindersCount(user),
                notifications = GetNotificationsCount(user),
                escalation = GetEscalationCount(user),
                arrivals = GetArrivalsCount(user, role, profileId),
                calls = GetCallsCount(user),
                following = GetFollowingCount(user),
                coordinators = GetCoordinatorsCount()
            };
            var serviceRecords = new
            {
                active = 0,//service_record.Distinct().Count(x => x.StatusId == 2),
                inprogress = 0,//service_record.Distinct().Count(x => x.StatusId == 3),
                //onHold = service_record.Distinct().Count(x => x.StatusId == 4),
                pendingAssigned = 0,//service_record.Distinct().Count(x => x.StatusId == 1),
                vip = service_record.Distinct().Count(x => x.Vip == true),
                pendngAcceptance = 0,//service_record.Distinct().Count(x => x.StatusId == 18),
                board = service_record.Distinct().OrderByDescending(o => o.Id),
                counts
            };
            return new ObjectResult(serviceRecords);
        }

        public ActionResult GetDashboardApp(int user)
        {
            int? profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user)?.Id;
            int? countryId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user)?.Country.Value;

            var service_record = _context.ServiceRecords
                .Select(s => new
                {
                    Id = s.Id,
                    s.Status.Status,
                    s.StatusId,
                    assigneeId = s.AssigneeInformations.FirstOrDefault().UserId,
                    coordinatorimmId = s.ImmigrationCoodinators.Select(c => c.CoordinatorId),
                    coordinatorreloId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
                    supplierimmId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                    supplierreloId = s.RelocationSupplierPartners.Select(c => c.SupplierId),
                    total_services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count()
                    + _context.BundledServices.Where(w => w.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count()

                }).Where(x => x.total_services > 0 && x.supplierimmId.Contains(profileId) || x.supplierreloId.Contains(profileId)).ToList();


            //Query por Role
            //int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;

            //switch (role)
            //{
            //    case 2: // Coordinator
            //        service_record = service_record.Where(a => a.total_services > 0 && a.coordinatorimmId.Contains(profileId) || a.coordinatorreloId.Contains(profileId)).ToList();
            //        break;
            //    case 3: // Supplier
            //        service_record = service_record.Where(x => x.total_services > 0 && x.supplierimmId.Contains(profileId) || x.supplierreloId.Contains(profileId)).ToList();
            //        break;
            //    case 4:
            //        service_record = service_record.Where(x => x.assigneeId == user).ToList();
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}

            var _report = _context.ReportDays
                .Include(x => x.ServiceReportDays)
                .Select(c => new {
                    c.Id,
                    c.ReportNo,
                    avatar = c.ReportByNavigation.ProfileUsers.FirstOrDefault().Photo,
                    ReportBy = c.ReportByNavigation.ProfileUsers.FirstOrDefault().Name,
                    c.CreationDate,
                    c.ReportDate,
                    c.ServiceLine,
                    c.WorkOrder,
                    c.StartTime,
                    c.EndTime,
                    c.TotalTime,
                    c.Activity,
                    c.Conclusion,
                    c.CreatedBy,
                    c.CreatedDate,
                    c.UpdateBy,
                    c.UpdatedDate,
                    country = c.ReportByNavigation.ProfileUsers.FirstOrDefault().CountryNavigation.Name,
                    ServiceCount = c.ServiceReportDays.Count()
                }).ToList();
            //for (int i = 0; i < service_record.Count(); i++)
            //{

            //}

            var _notification = _context.NotificationSystems
                //.Include(x => x.UserFromNavigation)
                .Include(x => x.NotificationTypeNavigation)
                .Where(c => c.UserTo == user && c.View == false && c.Archive == false).ToList();

            var _messages = _context.Messages.Where(x => x.UserId != user && x.Status == false).ToList();
            
            var _appointment = _context.Appointments.Where(c => c.To == user && c.Status == 1 && c.Date == DateTime.Now.Date).ToList();

            var counts = new
            {
                notificationscount = _notification.Count(),
                notifications = _notification,
                messages = _messages,
                appointments = _appointment,

                // pendingReports = _report,//ActivityItemsCount(user),
                textNotification = _notification.Count() > 0 ? _notification.LastOrDefault()?.NotificationTypeNavigation?.Type
                + (_notification.Count() - 1 > 0 ? " + " + (_notification.Count() - 1).ToString() + " Notifications More" : "")
               : "No new Notifications."
               , textMessages = _messages.Count() > 0 ? _messages.LastOrDefault()?.Message1
               + (_messages.Count() - 1 > 0 ? " + " + (_messages.Count() - 1).ToString() + " Messages More" : "")
               : "No new Messages.",
                textAppointments = _appointment.Count() > 0 ? _appointment.LastOrDefault()?.Description
               + (_appointment.Count() - 1 > 0 ? " + " + (_appointment.Count() - 1) + " Appointments More" : "")
               : "No Meetings today.",
                //arrivals = _context.ServiceRecords.Where(x => x.RelocationSupplierPartners.FirstOrDefault().SupplierId
                //           == profileId && x.AssigneeInformations.FirstOrDefault().InitialArrival >= DateTime.Now).ToList()//GetArrivalsCount(user, role, profileId)
                supplier = _context.ProfileUsers.Where(x => x.User.RoleId == 3 && x.Country == countryId)
               .Select(s => new
               {
                   s.Id,
                   s.Photo,
                   s.Name,
                   country = s.CountryNavigation.Name,
                   city = s.CityNavigation.City,
                   s.SupplierTypeNavigation.Type,
                   s.Status
               }).ToList(),

                arrivals = _context.ServiceRecords
               .Select(s => new
               {
                   avatar = s.AssigneeInformations.FirstOrDefault().Photo == null || s.AssigneeInformations.FirstOrDefault().Photo == "" ? "Files/assets/avatar.png" : s.AssigneeInformations.FirstOrDefault().Photo,
                   s.AssigneeInformations.FirstOrDefault().AssigneeName,
                   city = s.AssigneeInformations.FirstOrDefault().HostCity.City,
                   serviceLine = "Relo",
                   total_services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Count()
                   + _context.BundledServices.Where(w => w.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Count(),
                   arrivalDate = s.AssigneeInformations.FirstOrDefault().InitialArrival,
                   client = s.Client.Name,
                   coordinatorId = s.ImmigrationCoodinators.Any()
                   ? s.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                   : s.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                   supplierId = s.ImmigrationSupplierPartners.Any()
                   ? s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId
                   : s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
               }).Where(x => x.supplierId == profileId && x.arrivalDate >= DateTime.Now).ToList()//GetArrivalsCount(user, role, profileId)
            };

           // var counts_ = 0;

            var serviceRecords = new
            {
                pendngAcceptance = service_record.Distinct().Count(x => x.StatusId == 18),
                inprogress = service_record.Distinct().Count(x => x.StatusId == 3),
                all = service_record.Count(),
            };
            return new ObjectResult(new { serviceRecords = serviceRecords, counts = counts });
        }

        public ActionResult GetDashboardAppAssigne(int user, int? serviceLine)
        {
            var sr = _context.ServiceRecords
                .Include(x => x.WorkOrders)
                .Include(x => x.AssigneeInformations)
                .SingleOrDefault(x => x.AssigneeInformations.FirstOrDefault().UserId == user);

            //int profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user).Id;

            var _Bundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLine
                            && x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr.Id)
                .Select(n => new
                {
                    n.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    n.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    n.Status.Status,

                    supplierId = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.UserId,

                    supplierName = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.Name
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.Name,

                    Avatar = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.Supplier.Photo
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.Supplier.Photo,

                    Authodate_consultant = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .RelocationSupplierPartner.AssignedDate
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkServicesId)
                    .ImmigrationSupplierPartner.AssignedDate,

                    n.ServiceTypeId,
                    ServiceType = "Bundle", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.BundledServiceOrder.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,

                    n.BundledServiceOrder.ProjectedFee,
                    n.BundledServiceOrder.WorkOrderId,
                    n.CategoryId,

                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    location = n.Location,
                    arrival = sr.AssigneeInformations.FirstOrDefault().InitialArrival,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.BundledServiceOrder.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.BundledServiceOrder.WorkOrder.ServiceLineId).NickName,

                    authoTime = n.BundledServiceOrder.TotalTime.ToString(),

                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - Convert.ToDecimal(n.BundledServiceOrder.TotalTime))
                        : Convert.ToDecimal(n.BundledServiceOrder.TotalTime),
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    id_server = n.Id,
                    WorkOrderServiceId = n.WorkServicesId,
                    // END
                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkServices.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkServices.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkServices.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkServices.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkServices.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkServices.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkServices.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkServices.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkServices.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkServices.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkServices.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkServices.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkServices.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkServices.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkServices.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkServices.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkServices.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkServices.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkServices.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkServices.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkServices.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkServices.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkServices.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkServices.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkServices.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkServices.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkServices.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkServices.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkServices.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                })
                .ToList();

            var _Standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceLineId == serviceLine && x.WorkOrder.ServiceRecordId == sr.Id)
                .Select(n => new
                {
                    n.WorkOrder.ServiceRecordId,
                    n.WorkOrder.NumberWorkOrder,
                    n.Status.Status,

                    supplierId = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.UserId
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.UserId,

                    supplierName = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.Name
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.Name,

                    Avatar = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.Supplier.Photo
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.Supplier.Photo,


                    Authodate_consultant = _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartnerId == null
                    ? _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .RelocationSupplierPartner.AssignedDate
                    : _context.AssignedServiceSupliers.FirstOrDefault(x => x.ServiceOrderServicesId == n.WorkOrderServiceId)
                    .ImmigrationSupplierPartner.AssignedDate,

                    n.ServiceTypeId,
                    ServiceType = "Standalone", //n.ServiceType.Service,
                    n.StatusId,
                    Authodate = n.WorkOrder.CreationDate.Value,
                    Acceptance = n.Acceptance.HasValue ? n.Acceptance : DateTime.Now,

                    n.ProjectedFee,
                    n.WorkOrderId,
                    n.CategoryId,

                    DeliveredToId = n.DeliveredTo,
                    Country = n.DeliveringInNavigation.Name,
                    location = n.Location,
                    arrival = sr.AssigneeInformations.FirstOrDefault().InitialArrival,
                    service_name = _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                    && o.IdClientPartnerProfile == sr.ClientId
                    && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName == "--" ? n.Service.Service
                    : _context.ServiceLocations.SingleOrDefault(o => o.IdService == n.ServiceId
                   && o.IdClientPartnerProfile == sr.ClientId
                   && o.IdServiceLine == n.WorkOrder.ServiceLineId).NickName,

                    authoTime = n.AuthoTime.Value.ToString(),

                    timeRemaining = _context.ServiceReportDays.Any(_x => _x.Service.Value == n.Id)
                        ? Math.Abs(_context.ServiceReportDays.Where(_x => _x.Service.Value == n.Id)
                            .Sum(_s => Convert.ToDecimal(_s.Time)) - n.AuthoTime.Value)
                        : n.AuthoTime.Value,
                    // NUMBER SERVER
                    number_server = n.ServiceNumber,
                    id_server = n.Id,
                    WorkOrderServiceId = n.WorkOrderServiceId,
                    // END

                    DeliveredTo = _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name + "/" + _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Relationship.Relationship == null ? "N/A" :
                        _context.DependentInformations
                            .FirstOrDefault(x => x.Id == n.DeliveredTo).Name,
                    service_change = n.ServiceType.Service,
                    serviceInvoice = n.InvoiceSupplier.HasValue ? n.InvoiceSupplier.Value : false,
                    service = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
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
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Id
                            }).ToList()
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
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
                            }).ToList(),
                    StatusService = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 5 ? "N/A"
                        : n.CategoryId == 6 ? "N/A"
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Status
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Status
                        : "N/A",
                    statusServiceId = n.CategoryId == 1 ? n.WorkOrderService.EntryVisas
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 2 ? n.WorkOrderService.WorkPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 3 ? n.WorkOrderService.VisaDeregistrations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 4 ? n.WorkOrderService.ResidencyPermits
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 5 ? 1
                        : n.CategoryId == 6 ? 1
                        : n.CategoryId == 7 ? n.WorkOrderService.CorporateAssistances
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 8 ? n.WorkOrderService.Renewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 9 ? n.WorkOrderService.Notifications
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 10 ? n.WorkOrderService.LegalReviews
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 12 ? n.WorkOrderService.PredecisionOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 13 ? n.WorkOrderService.AreaOrientations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 14 ? n.WorkOrderService.SettlingIns
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 15 ? n.WorkOrderService.SchoolingSearches
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 16 ? n.WorkOrderService.Departures
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 17 ? n.WorkOrderService.TemporaryHousingCoordinatons
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 18 ? n.WorkOrderService.RentalFurnitureCoordinations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 19 ? n.WorkOrderService.Transportations
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 20 ? n.WorkOrderService.AirportTransportationServices
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 21 ? n.WorkOrderService.HomeFindings
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 22 ? n.WorkOrderService.LeaseRenewals
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 23 ? n.WorkOrderService.HomeSales
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 24 ? n.WorkOrderService.HomePurchases
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 25 ? n.WorkOrderService.PropertyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 26 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 27 ? n.WorkOrderService.TenancyManagements
                            .FirstOrDefault().Status.Id
                        : n.CategoryId == 28 ? n.WorkOrderService.Others
                            .FirstOrDefault().Status.Id
                        : 1,
                    dialog_type = n.CategoryId,
                    bundled = (int?)null
                }).ToList();

            var services = _Bundled.Union(_Standalone);
            //Query por Role
            //int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;

            //switch (role)
            //{
            //    case 2: // Coordinator
            //        service_record = service_record.Where(a => a.total_services > 0 && a.coordinatorimmId.Contains(profileId) || a.coordinatorreloId.Contains(profileId)).ToList();
            //        break;
            //    case 3: // Supplier
            //        service_record = service_record.Where(x => x.total_services > 0 && x.supplierimmId.Contains(profileId) || x.supplierreloId.Contains(profileId)).ToList();
            //        break;
            //    case 4:
            //        service_record = service_record.Where(x => x.assigneeId == user).ToList();
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}

            var _notification = _context.NotificationSystems
                //.Include(x => x.UserFromNavigation)
                .Include(x => x.NotificationTypeNavigation)
                .Where(c => c.UserTo == user && c.View == false && c.Archive == false).ToList();

            var _messages = _context.Messages.Where(x => x.UserId != user && x.Status == false).ToList();

            var _appointment = _context.Appointments.Where(c => c.ServiceRecordId == sr.Id && c.Status == 1 && c.Date == DateTime.Now.Date).ToList();

            //Agregar suplier immigration cuando se haga la parte de immigration
            var _supplier = _context.RelocationSupplierPartners
                    .Where(x => x.ServiceRecordId == sr.Id)
               .Select(s => new
               {
                   s.Supplier.User.ProfileUsers.FirstOrDefault().Id,
                   s.Supplier.User.ProfileUsers.FirstOrDefault().Photo,
                   s.Supplier.User.ProfileUsers.FirstOrDefault().Name,
                   country = s.Supplier.User.ProfileUsers.FirstOrDefault().CountryNavigation.Name,
                   city = s.Supplier.User.ProfileUsers.FirstOrDefault().CityNavigation.City,
                   s.Supplier.User.ProfileUsers.FirstOrDefault().SupplierTypeNavigation.Type,
                   s.Supplier.User.ProfileUsers.FirstOrDefault().Status
               }).ToList();

            var counts = new
                {
                    //notificationscount = _notification.Count(),
                    notifications = _notification,
                    messages = _messages,
                    appointments = _appointment,
                    supplier = _supplier,

                    // pendingReports = _report,//ActivityItemsCount(user),
                    textNotification = _notification.Count() > 0 ? _notification.LastOrDefault()?.NotificationTypeNavigation?.Type
                + (_notification.Count() - 1 > 0 ? " + " + (_notification.Count() - 1).ToString() + " Notifications More" : "")
               : "No new Notifications."
               ,
                    textMessages = _messages.Count() > 0 ? _messages.LastOrDefault()?.Message1
               + (_messages.Count() - 1 > 0 ? " + " + (_messages.Count() - 1).ToString() + " Messages More" : "")
               : "No new Messages.",
                    textAppointments = _appointment.Count() > 0 ? _appointment.LastOrDefault()?.Description
               + (_appointment.Count() - 1 > 0 ? " + " + (_appointment.Count() - 1) + " Appointments More" : "")
               : "No Meetings today."
                };

                // var counts_ = 0;

                var serviceRecords = new
                {
                    inprogress = services.Distinct().Count(x => x.StatusId == 2),
                    complete = services.Distinct().Count(x => x.StatusId == 4),
                    all = services.Count(),
                };
                return new ObjectResult(new { serviceRecords = serviceRecords, counts = counts });
            
        }

        public ActionResult GetInboxHome(int userId)
        {
            var messages = _context.Messages.Where(x => x.ConversationNavigation.UserTo == userId || x.ConversationNavigation.UserReciver == userId
                || x.ConversationNavigation.UserGroups.Select(u => u.UserReciver).Contains(userId))
                .Select(s => new
                {
                    iduser = s.UserId,
                    name = s.User.Name,
                    lastname = s.User.LastName,
                    photo = s.User.Avatar,
                    conversationId = s.Conversation,
                    lastMessage = s.Message1,
                    lastMessageTime = s.Time,
                    s.ConversationNavigation.Groups,
                    s.Status
                }).Where(x => userId != x.iduser && x.Status == false).ToList();
            var notifications_read = _context.NotificationSystems
                .Where(c => c.UserTo == userId && c.View == true)
                .Select(s => new
                {
                    s.Id,
                    s.NotificationTypeNavigation.Type,
                    serviceRecord = s.ServiceRecord.HasValue ? s.ServiceRecordNavigation.NumberServiceRecord : "",
                    fromId = s.UserFrom,
                    fromProfile = s.UserFromNavigation.ProfileUsers.FirstOrDefault().Id,
                    toId = s.UserTo,
                    roleFromId = s.UserFromNavigation.RoleId,
                    roleToId = s.UserToNavigation.RoleId,
                    from = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Name + " " + s.UserFromNavigation.LastName + " " + s.UserFromNavigation.MotherLastName
                    : "N/A",
                    fromAvatar = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Avatar
                    : "N/A",
                    s.Description,
                    s.Time,
                    s.CreatedDate,
                    s.Color,
                    s.NotificationType,
                    serviceRecordId = s.ServiceRecord,
                    clientName = s.ServiceRecordNavigation.Client.Name,
                    s.ServiceRecordNavigation.Vip,
                    s.ServiceRecordNavigation.Urgent,
                    s.ServiceRecordNavigation.ConfidentialMove,
                    assignee = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().AssigneeName,
                    serviceRecordCountry = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCountry.Name,
                    serviceRecordCountryHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCountryNavigation.Name,
                    serviceRecordCityHome = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCity.Name,
                    serviceRecordCityHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCity.City,
                    s.Archive,
                    s.View
                }).Where(c => c.toId == userId && c.View == true).Take(20).ToList();
            var unread_notifications = _context.NotificationSystems
                .Select(s => new
                {
                    s.Id,
                    s.NotificationTypeNavigation.Type,
                    serviceRecord = s.ServiceRecord.HasValue ? s.ServiceRecordNavigation.NumberServiceRecord : "",
                    fromId = s.UserFrom,
                    fromProfile = s.UserFromNavigation.ProfileUsers.FirstOrDefault().Id,
                    toId = s.UserTo,
                    roleFromId = s.UserFromNavigation.RoleId,
                    roleToId = s.UserToNavigation.RoleId,
                    from = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Name + " " + s.UserFromNavigation.LastName + " " + s.UserFromNavigation.MotherLastName
                    : "N/A",
                    fromAvatar = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Avatar
                    : "N/A",
                    s.Description,
                    s.Time,
                    s.CreatedDate,
                    s.Color,
                    s.NotificationType,
                    serviceRecordId = s.ServiceRecord,
                    clientName = s.ServiceRecordNavigation.Client.Name,
                    s.ServiceRecordNavigation.Vip,
                    s.ServiceRecordNavigation.Urgent,
                    s.ServiceRecordNavigation.ConfidentialMove,
                    assignee = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().AssigneeName,
                    serviceRecordCountry = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCountry.Name,
                    serviceRecordCountryHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCountryNavigation.Name,
                    serviceRecordCityHome = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCity.Name,
                    serviceRecordCityHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCity.City,
                    s.Archive,
                    s.View
                }).Where(c => c.toId == userId && c.View == false).ToList();

            return new ObjectResult(new { messages, notifications_read, unread_notifications });
        }

        public ActionResult GetChangeNotificationsToRead(int userId)
        {
            var unread_notifications = _context.NotificationSystems
                .Where(c => c.UserTo == userId && c.View == false).ToList();

            for(var i = 0; i < unread_notifications.Count(); i++)
            {
                unread_notifications[i].View = true;
                _context.NotificationSystems.Update(unread_notifications[i]);
            }

            _context.SaveChanges();
            return new ObjectResult(unread_notifications);
        }

        public ActionResult GetReportDayApp(int id_ReporDay)
        {
            var report = _context.ReportDays
                .Where(c => c.Id == id_ReporDay)
                .Select(s => new
                {
                    s.Id,
                    s.ReportNo,
                    supplier = s.ReportBy,
                    supplierName = _context.ProfileUsers.FirstOrDefault(x  => x.UserId == s.ReportBy).Name,
                    s.CreatedDate,
                    s.ReportDate,
                    s.StartTime,
                    s.EndTime,
                    s.TotalTime,
                    standaloneServices = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderId == s.WorkOrder)
                    .Select(_s => new
                    {
                        _s.Id,
                        _s.ServiceNumber,
                        _s.Autho,
                        _s.AuthoTime,
                        used = _context.ServiceReportDays.FirstOrDefault(x => x.Service == _s.Id).Time,
                        TimeReminder = _context.ServiceReportDays.FirstOrDefault(x => x.Service == _s.Id).TimeReminder,
                        serviceName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
                        && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
                        && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
                       && f.IdService == _s.ServiceId).NickName
                     })
                     .Join(_context.ServiceReportDays, stan => stan.Id, rport => rport.Service,
                        (stan, rport) => new
                        {
                            Id = stan.Id,
                            report_day_id = rport.ReportDayId,
                            stan.ServiceNumber,
                            stan.Autho,
                            stan.AuthoTime,
                            used = rport.Time,
                            rport.TimeReminder,
                            stan.serviceName
                        }).Where(x => x.report_day_id == s.Id).ToList(),
                    bundledService = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrderId == s.WorkOrder)
                    .Select(_s => new
                    {
                        _s.Id,
                        _s.ServiceNumber,
                        _s.Autho,
                        TotalTime = _s.BundledServiceOrder.TotalTime,
                        used = s.ServiceReportDays.FirstOrDefault().Time,
                        TimeReminder = _context.ServiceReportDays.FirstOrDefault(x => x.Service == _s.Id).TimeReminder,
                        serviceName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
                        && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
                        && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
                       && f.IdService == _s.ServiceId).NickName
                    })
                    .Join(_context.ServiceReportDays, stan => stan.Id, rport => rport.Service,
                        (stan, rport) => new
                        {
                            stan.Id,
                            report_day_id = rport.ReportDayId,
                            stan.ServiceNumber,
                            stan.Autho,
                            stan.TotalTime,
                            used = rport.Time,
                            rport.TimeReminder,
                            stan.serviceName
                        }).Where(x => x.report_day_id == s.Id).ToList(),
                    s.Activity,
                    s.Conclusion
                    //ServiceReportDays = s.ServiceReportDays.Select(d => new
                    //{
                    //    serviceNumber = _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == d.Service).ServiceNumber + ""
                    //    type = _context.StandaloneServiceWorkOrders.Any(x => x.Id == d.Service) ? "Stand Alone Services"
                    //    : "Bundled Services",
                    //    Total_Autho = _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == d.Service).AuthoTime,
                    //    used = d.Time,
                    //    d.TimeReminder
                    //}).ToList()
                })
                .ToList();

            return new ObjectResult(report);
        }

        public ActionResult GetMessageNotificatiosApp(int userId)
        {
            int messages = _context.Messages.Where(x => x.ConversationNavigation.UserTo == userId || x.ConversationNavigation.UserReciver == userId
                || x.ConversationNavigation.UserGroups.Select(u => u.UserReciver).Contains(userId))
                .Where(x => userId != x.UserId && x.Status == false).Count();
            
            int unread_notifications = _context.NotificationSystems
                .Where(c => c.UserTo == userId && c.View == false).Count();

            return new ObjectResult(messages + unread_notifications);
        }

        public ActionResult GetMessageHome(int userId)
        {
            var serviceRecord = _context.Messages
                .Where(s => s.UserId == userId)
                .Select(x => new
                {
                    x.Id,
                    x.UserId,
                    x.Message1,
                    x.Conversation,
                    x.Time,
                    x.Status
                }).ToList();

            return new ObjectResult(serviceRecord);
        }

        public ActionResult getServicesBySupplierAdmin(int sr, int? userId)
        {
            int _profile = 0;
            if(userId != null)
            {
                _profile = _context.ProfileUsers.FirstOrDefault(x => x.UserId == userId).Id;
            }

            var serviceRecord = _context.ServiceRecords
                .Where(s => s.Id == sr).Select(x => new
                {
                    x.Id,
                    statusCoordinator = x.RelocationCoordinators.Any() 
                    ? x.RelocationCoordinators.FirstOrDefault(f => f.ServiceRecordId == sr && f.CoordinatorId == _profile).StatusId
                    : x.ImmigrationCoodinators.FirstOrDefault(f => f.ServiceRecordId == sr && f.CoordinatorId == _profile).StatusId,
                    standalone = _context.StandaloneServiceWorkOrders.Select(stand => new
                    {
                        stand.Id,
                        stand.ServiceNumber,
                        stand.ServiceId,
                        stand.WorkOrder.NumberWorkOrder,
                        stand.WorkOrder.ServiceRecordId,
                        stand.WorkOrderServiceId,
                        country = stand.DeliveringInNavigation.Name,
                        status = x.RelocationSupplierPartners.Any() 
                        ? x.RelocationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id == 
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkOrderServiceId).RelocationSupplierPartnerId).Status.Status 
                        : x.ImmigrationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkOrderServiceId).ImmigrationSupplierPartnerId.Value).Status.Status,
                        assigned = x.RelocationSupplierPartners.Any()
                        ? x.RelocationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkOrderServiceId).RelocationSupplierPartnerId.Value).Supplier.Name
                        : x.ImmigrationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkOrderServiceId).ImmigrationSupplierPartnerId.Value).Supplier.Name,
                        serviceLine = stand.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == x.ClientId
                            && f.IdService == stand.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == x.ClientId
                            && f.IdService == stand.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == x.ClientId
                            && f.IdService == stand.ServiceId).NickName
                    }).Where(w => w.ServiceRecordId == sr).ToList(),
                    //&& w.SupplierId == s.ServiceRecord.ImmigrationSupplierPartners.SingleOrDefault(C => C.SupplierId == profileId).SupplierId.Value).ToList(),
                    bundled = _context.BundledServices.Select(stand => new
                    {
                        stand.Id,
                        stand.ServiceNumber,
                        stand.ServiceId,
                        stand.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        stand.BundledServiceOrder.WorkOrder.ServiceRecordId,
                        stand.WorkServicesId,
                        country = stand.DeliveringInNavigation.Name,
                        status = x.RelocationSupplierPartners.Any()
                        ? x.RelocationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkServicesId).RelocationSupplierPartnerId).StatusId == null ? 2 :
                        x.RelocationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkServicesId).RelocationSupplierPartnerId).StatusId
                        : x.ImmigrationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkServicesId).ImmigrationSupplierPartnerId.Value).StatusId == null ? 2
                        : x.ImmigrationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkServicesId).ImmigrationSupplierPartnerId.Value).StatusId,
                        assigned = x.RelocationSupplierPartners.Any()
                        ? x.RelocationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkServicesId).RelocationSupplierPartnerId.Value).Supplier.Name
                        : x.ImmigrationSupplierPartners.FirstOrDefault(y => y.ServiceRecordId == sr && y.Id ==
                        _context.AssignedServiceSupliers.FirstOrDefault(u => u.ServiceOrderServicesId == stand.WorkServicesId).ImmigrationSupplierPartnerId.Value).Supplier.Name,
                        serviceLine = stand.BundledServiceOrder.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == x.ClientId
                            && f.IdService == stand.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == x.ClientId
                            && f.IdService == stand.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == x.ClientId
                            && f.IdService == stand.ServiceId).NickName
                    }).Where(w => w.ServiceRecordId == sr)
                //&& w.SupplierId == s.ServiceRecord.ImmigrationSupplierPartners.SingleOrDefault(C => C.SupplierId == profileId).SupplierId.Value)
                .ToList()
                });

            return new ObjectResult(serviceRecord);
        }

        public ActionResult getServicesBySupplier(int sr, int idUser)
        {
            int profileId = _context.ProfileUsers.FirstOrDefault(x => x.UserId == idUser).Id;

            var serviceRecord = _context.ServiceRecords
                .Where(x => x.Id == sr).Select(s => new
                {
                    s.Id,
                    supplierImm = s.ImmigrationSupplierPartners.Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdSupplier = c.Id,
                        standalone = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            stand.Id,
                            stand.ServiceNumber,
                            stand.ServiceId,
                            serviceType = "standalone",
                            stand.Status.Status,
                            stand.WorkOrder.NumberWorkOrder,
                            AuthoTime = stand.AuthoTime,
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId,
                            stand.WorkOrderServiceId,
                            nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName
                        }).Where(w => w.imm_relo == c.Id && c.SupplierId == profileId).ToList(),
                        bundled = _context.BundledServices
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            stand.Id,
                            stand.ServiceNumber,
                            stand.ServiceId,
                            serviceType = "bundled",
                            stand.Status.Status,
                            stand.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                            AuthoTime = stand.BundledServiceOrder.TotalTime,
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId,
                            stand.BundledServiceOrderId,
                            nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName
                        }).Where(w => w.imm_relo == c.Id && c.SupplierId == profileId).ToList()
                    }),
                    supplierRelo = s.RelocationSupplierPartners.Where(x => x.StatusId != 4).Select(c => new
                    {
                        IdSupplier = c.Id,
                        standalone = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            stand.Id,
                            stand.ServiceNumber,
                            stand.ServiceId,
                            serviceType = "standalone",
                            stand.Status.Status,
                            stand.WorkOrder.NumberWorkOrder,
                            AuthoTime = stand.AuthoTime,
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId,
                            stand.WorkOrderServiceId,
                            nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName
                        }).Where(w => w.imm_relo == c.Id && c.SupplierId == profileId).ToList(),
                        bundled = _context.BundledServices
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            stand.Id,
                            stand.ServiceNumber,
                            stand.ServiceId,
                            serviceType = "bundled",
                            stand.Status.Status,
                            stand.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                            AuthoTime = stand.BundledServiceOrder.TotalTime,
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId,
                            stand.BundledServiceOrderId,
                            nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.ClientId
                            && f.IdService == stand.ServiceId).NickName
                        }).Where(w => w.imm_relo == c.Id && c.SupplierId == profileId).ToList()
                    })
                });

            return new ObjectResult(serviceRecord);
        }

        public int GetServicesSupplierCount(int id)
        {
            int standalone = _context.StandaloneServiceWorkOrders
                        .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                        (stand, assig) => new
                        {
                            imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                        }).Where(w => w.imm_relo == id).Count();
            int bundled = _context.BundledServices
                     .Join(_context.AssignedServiceSupliers, stand => stand.WorkServicesId, assig => assig.ServiceOrderServicesId,
                     (stand, assig) => new
                     {
                         imm_relo = assig.ImmigrationSupplierPartnerId == null ? assig.RelocationSupplierPartnerId : assig.ImmigrationSupplierPartnerId
                     }).Where(w => w.imm_relo == id).Count();

            return standalone + bundled;
        }

        public int GetCoordinatorsCount()
        {
            int relocation = _context.RelocationCoordinators.Count();
            int immigration = _context.ImmigrationCoodinators.Count();
            return relocation + immigration;
        }

        public int GetFollowingCount(int user)
        {
            int calls = _context.Follows.Count(c => c.UserId == user);
            return calls;
        }

        public int GetCallsCount(int user)
        {
            int calls = _context.Calls.Count();
            return calls;
        }

        public int GetEscalationCount(int user)
        {
            int scalate = _context.ScalateServices.Count(c => c.UserFromId == user || c.UserToId == user);
            return scalate;
        }

        private int GetArrivalsCount(int user, int role, int profile)
        {
            var services = _context.ServiceRecords.Select(s => new
            {
                arrival = s.AssigneeInformations.FirstOrDefault().InitialArrival,
                coordinatorId = s.ImmigrationCoodinators.Any()
                    ? s.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                supplierId = s.ImmigrationSupplierPartners.Any()
                    ? s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId
                    : s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
            }).ToList();
            switch (role)
            {
                case 2: // Coordinator
                    services = services.Where(x => x.coordinatorId == profile).ToList();
                    break;
                case 3: // Supplier
                    services = services.Where(x => x.supplierId == profile).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            int arrival = services.Count(c => c.arrival >= DateTime.Now);
            return arrival;
        }

        public int GetNotificationsCount(int user)
        {
            int calendar =
                _context.NotificationSystems.Count(c => c.UserTo == user && c.View == false);
            return calendar;
        }

        public int GetMessagesCount(int user)
        {
            int calendar =
                _context.Messages.Count(c => c.ConversationNavigation.UserReciver == user && c.Status == false);
            return calendar;
        }

        public int GetAppointmentCount(int user)
        {
            int calendar =
                _context.Appointments.Count(c => c.To == user && c.Status == 1);
            return calendar;
        }

        public bool ValidateEmailDependent(string email)
        {
            bool result = false;
            var _email = _context.DependentInformations.ToList();
            var _emailassignee = _context.AssigneeInformations.FirstOrDefault(x => x.Email == email).Email;

            for (int i = 0; i < _email.Count(); i++)
            {
                if (_email[i].Email == email)
                {
                    result = true;
                }
            }

            if(_emailassignee == email)
            {
                result = true;
            }

            return result;
        }

        public int GetUserBySupplier(int user)
        {

            //int supplierId = _context.ImmigrationSupplierPartners.Any(X => X.Id == user)
            //    ? _context.ImmigrationSupplierPartners.FirstOrDefault(X => X.Id == user).SupplierId.Value
            //    : _context.RelocationSupplierPartners.FirstOrDefault(X => X.Id == user).SupplierId.Value;
            //int supplier =
            //    _context.ProfileUsers.FirstOrDefault(c => c.Id == supplierId).UserId.Value;
            return user;
        }

        public List<int> GetUserByCoordinatorImm(int sr)
        {
            List<int> coordinador =
                _context.ImmigrationCoodinators.Where(y => y.ServiceRecordId == sr).Select(c => c.CoordinatorId.Value).ToList();

            List<int> users = _context.ProfileUsers.Where(x => coordinador.Contains(x.Id)).Select(x => x.UserId.Value).ToList();
            return users;
        }

        public List<int> GetUserByCoordinatorRelo(int sr)
        {
            List<int> coordinador =
                _context.RelocationCoordinators.Where(y => y.ServiceRecordId == sr).Select(c => c.CoordinatorId.Value).ToList();

            List<int> users = _context.ProfileUsers.Where(x => coordinador.Contains(x.Id)).Select(x => x.UserId.Value).ToList();
            return users;
        }

        public int GetCalendarCount()
        {
            int calendar = _context.Appointments.Count();
            return calendar;
        }

        private int ActivityItemsCount(int user)
        {
            var range1 = DateTime.Today.AddDays(-20);
            var range2 = DateTime.Today.AddDays(20);
            return _context.Tasks.Count(c =>
                (c.TaskFrom == user || c.TaskTo == user) && (c.DueDate > range1 && c.DueDate < range2));
        }

        private int GetRemindersCount(int user)
        {
            var remindersAll = _context.ReminderSchoolingSearches
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderHomeFindings
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderDepartures
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderPredecisionOrientations
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderAreaOrientations
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderSettlingIns
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderTransportations
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderAirportTransportationServices
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderLeaseRenewals
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderOthers
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderTemporaryHousingCoordinatons
                .Where(x => x.CreatedBy == user).Count()
                +
                _context.ReminderRentalFurnitureCoordinations
                .Where(x => x.CreatedBy == user).Count();

            return remindersAll;
        }

        private class RecurringEvent
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Color { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public DateTime? Date { get; set; }
            public RRule rrule { get; set; }

        }

        private class RRule
        {
            public string Freq { get; set; }
            public int? ByMonth { get; set; }
            public int? ByMonthDay { get; set; }
            public string Byweekday { get; set; }
        }

        private static string ReturnDay(int day)
        {
            string daySelected = "";
            switch (day)
            {
                case (1):
                    daySelected = "[RRule.MO]";
                    break;
                case (2):
                    daySelected = "[RRule.TU]";
                    break;
                case (3):
                    daySelected = "[RRule.WE]";
                    break;
                case (4):
                    daySelected = "[RRule.TH]";
                    break;
                case (5):
                    daySelected = "[RRule.FR]";
                    break;
                case (6):
                    daySelected = "[RRule.SA]";
                    break;
                case (7):
                    daySelected = "[RRule.SU]";
                    break;
            }

            return daySelected;
        }

        public ActionResult GetCalendar(int user, int? serviceLine, int? country, int? city, int? partner, int? client,
            int? coordinator, int? supplier, DateTime? rangeDate1, DateTime? rangeDate2)
        {
            var scheduleUser =
                _context.CalendarConsultantContactsConsultants.Where(x =>
                        x.ConsultantContactsConsultantNavigation.UserId == user)
                    .Select(s => new RecurringEvent()
                    {
                        Id = s.Id,
                        Title = s.Available.Value.Equals(true) ? "Available" : "No Available",
                        StartTime = s.HourStart,
                        EndTime = s.HourEnd,
                        Date = s.Date,
                        rrule = new RRule()
                        {
                            Freq = "RRule.WEEKLY",
                            Byweekday = ReturnDay(s.Day.Value)
                        },
                        Color = s.Available.Value.Equals(true) ? "#A64295" : "#595959"
                    });
            var user_profile = _context.ProfileUsers.Include(a => a.User).FirstOrDefault(x => x.UserId == user);

            var calendar = _context.Appointments.Select(s => new
            {
                s.Id,
                s.To,
                s.From,
                s.StartTime,
                s.EndTime,
                s.Date,
                assignee = s.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                s.ServiceRecord.Partner.Name,
                s.ServiceRecord.PartnerId,
                client = s.ServiceRecord.Client.Name,
                s.ServiceRecord.ClientId,
                coordinator = s.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                supplier = s.ToNavigation.ProfileUsers.SingleOrDefault(x => x.UserId == s.To).Id,
                suppliername = s.ToNavigation.ProfileUsers.SingleOrDefault(x => x.UserId == s.To).User.Name,
                s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCity,
                s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCityId,
                s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId,
                s.ServiceRecordId,
                services = s.AppointmentWorkOrderServices.Select(_s => new
                {
                    category = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().Category.Category
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().Category.Category,
                    serviceNumber = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().ServiceNumber,
                    serviceLine = _s.WorkOrderService.StandaloneServiceWorkOrders.Any()
                        ? _s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceLineId
                        : _s.WorkOrderService.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder
                            .ServiceLineId
                }).ToList()
            }).ToList();

            if (user_profile != null)
                if (user_profile.User.RoleId == 3 || user_profile.User.RoleId == 2)
                    calendar = calendar.Where(x => x.To == user || x.From == user).ToList();
            if (serviceLine.HasValue)
                calendar = calendar.Where(x => x.services.Any(_x => _x.serviceLine == serviceLine.Value)).ToList();
            if (country.HasValue)
                calendar = calendar.Where(x => x.HomeCountryId == country.Value).ToList();
            if (city.HasValue)
                calendar = calendar.Where(x => x.HomeCityId == city.Value).ToList();
            if (partner.HasValue)
                calendar = calendar.Where(x => x.PartnerId == partner.Value).ToList();
            if (client.HasValue)
                calendar = calendar.Where(x => x.ClientId == client.Value).ToList();
            if (coordinator.HasValue)
                calendar = calendar.Where(x => x.coordinator == coordinator.Value).ToList();
            if (supplier.HasValue)
                calendar = calendar.Where(x => x.supplier == supplier.Value).ToList();
            if (rangeDate1.HasValue && rangeDate2.HasValue)
                calendar = calendar.Where(x => x.Date > rangeDate1.Value && x.Date > rangeDate2.Value).ToList();
            var dataCalendar = new
            {
                calendar,
                scheduleUser
            };
            return new ObjectResult(dataCalendar);
        }

        public ActionResult GetFollowing(int user, int? sr, int? coordinator)
        {
            var followings = _context.Follows.Where(x => x.UserId == user).Select(s => new
            {
                s.Id,
                s.ServiceRecordId,
                s.ServiceRecord.NumberServiceRecord,
                s.ServiceRecord.Vip,
                s.ServiceRecord.Status.Status,
                s.ServiceRecord.InitialAutho,
                s.ServiceRecord.Partner.Name,
                client = s.ServiceRecord.Client.Name,
                coordinator = s.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name
                    : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Name,
                coordinatorId = s.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                coordinatorInfo = new
                {
                    s.User.Name,
                    s.User.LastName,
                    s.User.MotherLastName,
                    s.User.MobilePhone,
                    s.User.Avatar,
                    s.User.Email,
                    s.User.CreatedDate
                },
                s.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                services = _context.StandaloneServiceWorkOrders.Count(x =>
                               x.WorkOrder.ServiceRecordId == s.ServiceRecordId) +
                           _context.BundledServices.Count(x =>
                               x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId),
                immigration = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Select(q => new
                    {
                        q.Id,
                        q.Category.Category,
                        q.ServiceNumber,
                        q.Location
                    }).ToList(),
                relocation = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Select(q => new
                    {
                        q.Id,
                        q.Category.Category,
                        q.ServiceNumber,
                        q.Location
                    }).ToList(),
                servicesData = s.ServiceRecord.WorkOrders.Count != 0
                    ? _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.ServiceRecordId)
                        .Select(_s => new
                        {
                            id = _s.WorkOrderServiceId,
                            _s.Service.Service,
                            _s.ServiceNumber,
                            _s.WorkOrder.NumberWorkOrder,
                            _s.CategoryId,
                            _s.Autho,
                            _s.WorkOrder.ServiceLine.ServiceLine
                        }).Union(_context.BundledServices
                            .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId)
                            .Select(_s => new
                            {
                                id = _s.WorkServicesId,
                                _s.Service.Service,
                                _s.ServiceNumber,
                                _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                                _s.CategoryId,
                                _s.Autho,
                                _s.BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine
                            })).ToList()
                    : null,
            }).ToList();
            if (sr.HasValue)
                followings = followings.Where(x => x.ServiceRecordId == sr.Value).ToList();
            if (coordinator.HasValue)
                followings = followings.Where(x => x.coordinatorId == coordinator.Value).ToList();
            var srList = followings.Select(s => new
            {
                s.Id,
                s.NumberServiceRecord
            }).ToList();
            var coordinatorList = followings.Select(s => new
            {
                s.coordinatorId,
                s.coordinator
            }).ToList();
            var res = new
            {
                followings = followings,
                coordinators = coordinatorList,
                serviceRecords = srList
            };
            return new ObjectResult(followings);
        }

        public ActionResult GetFollowingCoordinators(int user)
        {
            var coordinators = _context.Follows.Select(s => new
            {
                coordinator = s.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name
                    : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Name,
                coordinatorId = s.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().CoordinatorId,
            }).Distinct().ToList();
            return new ObjectResult(coordinators);
        }

        public ActionResult GetFollowingsSr(int user)
        {
            var followings = _context.Follows.Select(s => new
            {
                s.Id,
                s.ServiceRecordId,
                s.ServiceRecord.NumberServiceRecord
            }).ToList();

            return new ObjectResult(followings);
        }

        public ActionResult GetCoordinators(int user, int? country, int? city, int? serviceLine, int? office)
        {
            var coordinatorImmigration = _context.ImmigrationCoodinators.Select(s => new
            {
                s.Id,
                s.CoordinatorId,
                profileId = s.Coordinator.Id,
                coordinator = s.Coordinator.Name,
                s.Coordinator.Email,
                s.Coordinator.Photo,
                s.Coordinator.PhoneNumber,
                serviceLineId = 1,
                serviceLine = "Immigration",
                countryId = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId,
                country = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                cityId = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCityId,
                city = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                officeId = s.ServiceRecord.Office,
                office = s.ServiceRecord.OfficeNavigation.Office,
                services = _context.StandaloneServiceWorkOrders.Where(x =>
                                   x.WorkOrder.ServiceLineId == 1 && x.WorkOrder.ServiceRecordId == s.ServiceRecordId)
                               .Count()
                           + _context.BundledServicesWorkOrders
                               .Where(x => x.WorkOrder.ServiceLineId == 1 &&
                                           x.WorkOrder.ServiceRecordId == s.ServiceRecordId)
                               .Select(_s => _s.BundledServices).Count(),
                active = _context.StandaloneServiceWorkOrders.Where(x =>
                             x.WorkOrder.ServiceLineId == 1 && x.StatusId == 3 &&
                             x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                         + _context.BundledServices.Where(x =>
                             x.BundledServiceOrder.WorkOrder.ServiceLineId == 1 && x.StatusId == 3 &&
                             x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count(),
                inProgress = _context.StandaloneServiceWorkOrders.Where(x =>
                                 x.WorkOrder.ServiceLineId == 1 && x.StatusId == 4 &&
                                 x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                             + _context.BundledServices.Where(x =>
                                 x.BundledServiceOrder.WorkOrder.ServiceLineId == 1 && x.StatusId == 4 &&
                                 x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count(),
                onHold = _context.StandaloneServiceWorkOrders.Where(x =>
                             x.WorkOrder.ServiceLineId == 1 && x.StatusId == 5 &&
                             x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                         + _context.BundledServices.Where(x =>
                             x.BundledServiceOrder.WorkOrder.ServiceLineId == 1 && x.StatusId == 5 &&
                             x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count(),
                pendingBilling = _context.StandaloneServiceWorkOrders.Where(x =>
                                     x.WorkOrder.ServiceLineId == 1 && x.StatusId == 1 &&
                                     x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                                 + _context.BundledServices.Where(x =>
                                     x.BundledServiceOrder.WorkOrder.ServiceLineId == 1 && x.StatusId == 1 &&
                                     x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
            }).ToList();
            var coordinatorRelocation = _context.RelocationCoordinators.Select(s => new
            {
                s.Id,
                s.CoordinatorId,
                profileId = s.Coordinator.Id,
                coordinator = s.Coordinator.Name,
                s.Coordinator.Email,
                s.Coordinator.Photo,
                s.Coordinator.PhoneNumber,
                serviceLineId = 2,
                serviceLine = "Relocation",
                countryId = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId,
                country = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                cityId = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCityId,
                city = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                officeId = s.ServiceRecord.Office,
                office = s.ServiceRecord.OfficeNavigation.Office,
                services = _context.StandaloneServiceWorkOrders.Where(x =>
                                   x.WorkOrder.ServiceLineId == 2 && x.WorkOrder.ServiceRecordId == s.ServiceRecordId)
                               .Count()
                           + _context.BundledServicesWorkOrders
                               .Where(x => x.WorkOrder.ServiceLineId == 2 &&
                                           x.WorkOrder.ServiceRecordId == s.ServiceRecordId)
                               .Select(_s => _s.BundledServices).Count(),
                active = _context.StandaloneServiceWorkOrders.Where(x =>
                             x.WorkOrder.ServiceLineId == 2 && x.StatusId == 3 &&
                             x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                         + _context.BundledServices.Where(x =>
                             x.BundledServiceOrder.WorkOrder.ServiceLineId == 2 && x.StatusId == 3 &&
                             x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count(),
                inProgress = _context.StandaloneServiceWorkOrders.Where(x =>
                                 x.WorkOrder.ServiceLineId == 2 && x.StatusId == 4 &&
                                 x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                             + _context.BundledServices.Where(x =>
                                 x.BundledServiceOrder.WorkOrder.ServiceLineId == 2 && x.StatusId == 4 &&
                                 x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count(),
                onHold = _context.StandaloneServiceWorkOrders.Where(x =>
                             x.WorkOrder.ServiceLineId == 2 && x.StatusId == 5 &&
                             x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                         + _context.BundledServices.Where(x =>
                             x.BundledServiceOrder.WorkOrder.ServiceLineId == 2 && x.StatusId == 5 &&
                             x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count(),
                pendingBilling = _context.StandaloneServiceWorkOrders.Where(x =>
                                     x.WorkOrder.ServiceLineId == 2 && x.StatusId == 1 &&
                                     x.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
                                 + _context.BundledServices.Where(x =>
                                     x.BundledServiceOrder.WorkOrder.ServiceLineId == 2 && x.StatusId == 1 &&
                                     x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.ServiceRecordId).Count()
            }).ToList();
            var coordinator = coordinatorImmigration.Union(coordinatorRelocation).ToList();
            if (country.HasValue)
                coordinator = coordinator.Where(x => x.countryId == country.Value).ToList();
            if (city.HasValue)
                coordinator = coordinator.Where(x => x.cityId == city.Value).ToList();
            if (serviceLine.HasValue)
                coordinator = coordinator.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (office.HasValue)
                coordinator = coordinator.Where(x => x.officeId == office.Value).ToList();
            return new ObjectResult(coordinator);
        }

        public ActionResult GetEscalation(int user, bool? status, DateTime? rangeDate1, DateTime? rangeDate2,
            int? level, int? client, int? supplierPartner, int? city,
            int? partner, int? country, int? serviceLine, int? office)
        {
            var escalation = _context.ScalateServices.Select(s => new
            {
                s.Id,
                s.WorkOrder.ServiceRecord.NumberServiceRecord,
                s.WorkOrder.ServiceRecord.Vip,
                escalations = _context.ScalateServices.Count(x => x.ServiceRecordId == s.WorkOrder.ServiceRecordId),
                s.Status,
                s.EscalationLevel,
                s.WorkOrder.ServiceRecord.InitialAutho,
                s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                s.WorkOrder.ServiceRecord.PartnerId,
                s.WorkOrder.ServiceRecord.Partner.Name,
                Client = s.WorkOrder.ServiceRecord.Client.Name,
                s.WorkOrder.ServiceRecord.ClientId,
                supplierId = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                    ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().SupplierId
                    : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                supplier = s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.Any()
                    ? s.WorkOrder.ServiceRecord.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name
                    : s.WorkOrder.ServiceRecord.RelocationSupplierPartners.FirstOrDefault().Supplier.Name,
                coordinatorId = s.WorkOrder.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.WorkOrder.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.WorkOrder.ServiceRecord.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                coordinator = s.WorkOrder.ServiceRecord.ImmigrationCoodinators.Any()
                    ? s.WorkOrder.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name
                    : s.WorkOrder.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Name,
                HomeCity = s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCityId,
                // services = s.WorkOrder.StandaloneServiceWorkOrders.Count() + _context.BundledServices.Where(_x => _x.BundledServiceOrder.WorkOrderId == s.WorkOrderId).Count()
                s.WorkOrder.ServiceLine.ServiceLine,
                s.WorkOrder.ServiceLineId,
                country = s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId,
                office = s.WorkOrder.ServiceRecord.Office.Value,
                partner = s.WorkOrder.ServiceRecord.Partner.Name
            }).ToList();
            if (status.HasValue)
                escalation = escalation.Where(x => x.Status == status.Value).ToList();
            if (rangeDate1.HasValue && rangeDate2.HasValue)
                escalation = escalation
                    .Where(x => x.InitialAutho >= rangeDate1.Value && x.InitialAutho <= rangeDate2.Value).ToList();
            if (level.HasValue)
                escalation = escalation.Where(x => x.EscalationLevel == level.Value).ToList();
            if (client.HasValue)
                escalation = escalation.Where(x => x.ClientId == client.Value).ToList();
            if (supplierPartner.HasValue)
                escalation = escalation.Where(x => x.supplierId == supplierPartner.Value).ToList();
            if (city.HasValue)
                escalation = escalation.Where(x => x.HomeCityId == city.Value).ToList();
            if (partner.HasValue)
                escalation = escalation.Where(x => x.PartnerId == partner.Value).ToList();
            if (country.HasValue)
                escalation = escalation.Where(x => x.HomeCountryId == country.Value).ToList();
            if (serviceLine.HasValue)
                escalation = escalation.Where(x => x.ServiceLineId == serviceLine.Value).ToList();
            if (office.HasValue)
                escalation = escalation.Where(x => x.office == office.Value).ToList();
            var escalations = new
            {
                level1 = escalation.Count(x => x.EscalationLevel == 1),
                level2 = escalation.Count(x => x.EscalationLevel == 2),
                level3 = escalation.Count(x => x.EscalationLevel == 3),
                level4 = escalation.Count(x => x.EscalationLevel == 4),
                level5 = escalation.Count(x => x.EscalationLevel == 5),
                escalation
            };
            return new ObjectResult(escalations);
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

        public async Task<ActionResult> GetPendingAuthorizations(int user, int? country, int? city, int? service_line,
            int? sr)
        {
            var pendingAssigment = _context.ServiceRecords
                //.Where(x => !x.ImmigrationSupplierPartners.Any() || !x.RelocationSupplierPartners.Any())
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    s.InithialAuthoAcceptance,
                    Partner = s.Partner.Name,
                    client = s.Client.Name,
                    seviceLineId = s.WorkOrders.Select(f => f.ServiceLineId).ToList(),
                    s.AssigneeInformations.FirstOrDefault().AssigneeName,
                    s.AssigneeInformations.FirstOrDefault().InitialArrival,
                    services = _context.StandaloneServiceWorkOrders.Count(x => x.WorkOrder.ServiceRecordId == s.Id)
                               + _context.BundledServicesWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id)
                                   .Select(q => q.BundledServices).Count(),
                    serviceLine = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                    assigment = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList())
                    ? (s.ImmigrationSupplierPartners.Count() > 0 && s.RelocationSupplierPartners.Count() > 0 ? true : false)
                    : (s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine == "Immigration" ? (s.ImmigrationSupplierPartners.Count() > 0 ? true : false) : (s.RelocationSupplierPartners.Count() > 0 ? true : false)),
                    homeCountry = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                    : "",
                    hostCountry = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                    : "",
                    hostCountryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                    homeCountryId = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountryId
                    : 0,
                    hostCity = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
                    hostCityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                    homeCityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                    homeCity = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name : "N/A",
                    coordinatorImm = s.ImmigrationCoodinators.Select(c => new
                    {
                        Id = c.CoordinatorId,
                        c.Coordinator.Name,
                        c.CoordinatorType.CoordinatorType,
                        coordinatorUser = c.Coordinator.User.Id,
                        avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                        Accepted = c.Accepted == null ? "pending" : c.Accepted != null && c.Accepted != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.Accepted == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A",
                        coordinatorAcceptance = c.StatusId == 2 ? false : true
                    }),
                    coordinatorRelo = s.RelocationCoordinators.Select(c => new
                    {
                        Id = c.CoordinatorId,
                        c.Coordinator.Name,
                        c.CoordinatorType.CoordinatorType,
                        coordinatorUser = c.Coordinator.User.Id,
                        avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                        Accepted = c.Accepted == null ? "pending" : c.Accepted != null && c.Accepted != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.Accepted == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A",
                        coordinatorAcceptance = c.StatusId == 2 ? false : true
                    }),
                    coordinatorimmId = s.ImmigrationCoodinators.Select(c => c.CoordinatorId),
                    coordinatorreloId = s.RelocationCoordinators.Select(c => c.CoordinatorId),
                    supplierConsultantImm = s.ImmigrationSupplierPartners.Select(c => new
                    {
                        Id = c.SupplierId,
                        c.Supplier.Name,
                        supplierConsultant_Id = c.Supplier.UserId,
                        consultantAssigned = c.StatusId == 2 ? false : true,
                        supplierConsultantId = c.SupplierTypeId,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        Accepted = c.AcceptedDate == null ? "pending" : c.AcceptedDate != null && c.AcceptedDate != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.AcceptedDate == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A"
                    }),
                    supplierConsultantRelo = s.RelocationSupplierPartners.Select(c => new
                    {
                        Id = c.SupplierId,
                        c.Supplier.Name,
                        supplierConsultant_Id = c.Supplier.UserId,
                        consultantAssigned = c.StatusId == 2 ? false : true,
                        supplierConsultantId = c.SupplierTypeId,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        Accepted = c.AcceptedDate == null ? "pending" : c.AcceptedDate != null && c.AcceptedDate != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.AcceptedDate == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A",
                    }),
                    supplierId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                    coordinatorStatus = s.ImmigrationCoodinators.Select(x => x.StatusId).Union(s.RelocationCoordinators.Select(x => x.StatusId)),
                    consultantStatus = s.ImmigrationSupplierPartners.Select(x => x.StatusId).Union(s.RelocationSupplierPartners.Select(x => x.StatusId)),
                    s.Urgent,
                    standaloneServices = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho,
                        country = _s.DeliveringInNavigation.Name,
                        serviceLine = _s.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                        && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                        && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                       && f.IdService == _s.ServiceId).NickName
                    }).ToList(),
                    bundledService = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id)
                    .Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho,
                        country = _s.DeliveringInNavigation.Name,
                        serviceLine = _s.BundledServiceOrder.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                       && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                       && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                      && f.IdService == _s.ServiceId).NickName
                    }).ToList()
                }).ToList();

            pendingAssigment = pendingAssigment.Where(x => x.assigment == false || x.consultantStatus.Contains(3)).ToList();

            var pendingAcceptance = _context.ServiceRecords
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    s.InithialAuthoAcceptance,
                    Partner = s.Partner.Name,
                    client = s.Client.Name,
                    seviceLineId = s.WorkOrders.Select(f => f.ServiceLineId).ToList(),
                    s.AssigneeInformations.FirstOrDefault().AssigneeName,
                    s.AssigneeInformations.FirstOrDefault().InitialArrival,
                    services = _context.StandaloneServiceWorkOrders.Count(x => x.WorkOrder.ServiceRecordId == s.Id)
                               + _context.BundledServicesWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id)
                                   .Select(q => q.BundledServices).Count(),
                    serviceLine = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList()) ? "I/R" : s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine,
                    assigment = EvaluateServiceLine(s.WorkOrders.Select(f => f.ServiceLineId.Value).ToList())
                    ? (s.ImmigrationSupplierPartners.Count() > 0 && s.RelocationSupplierPartners.Count() > 0 ? true : false)
                    : (s.WorkOrders.FirstOrDefault().ServiceLine.ServiceLine == "Immigration" ? (s.ImmigrationSupplierPartners.Count() > 0 ? true : false) : (s.RelocationSupplierPartners.Count() > 0 ? true : false)),
                    homeCountry = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountry.Name
                    : "",
                    hostCountry = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name
                    : "",
                    hostCountryId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCountry : 0,
                    homeCountryId = s.AssigneeInformations.Count != 0
                    ? s.AssigneeInformations.FirstOrDefault().HomeCountryId
                    : 0,
                    hostCity = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCity.City : "",
                    hostCityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HostCityId : 0,
                    homeCityId = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCityId : 0,
                    homeCity = s.AssigneeInformations.Count != 0 ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name : "N/A",
                    coordinatorImm = s.ImmigrationCoodinators.Select(c => new
                    {
                        Id = c.CoordinatorId,
                        c.Coordinator.Name,
                        c.CoordinatorType.CoordinatorType,
                        coordinatorUser = c.Coordinator.User.Id,
                        avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                        Accepted = c.Accepted == null ? "pending" : c.Accepted != null && c.Accepted != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.Accepted == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A",
                        coordinatorAcceptance = c.StatusId
                    }),
                    coordinatorRelo = s.RelocationCoordinators.Select(c => new
                    {
                        Id = c.CoordinatorId,
                        c.Coordinator.Name,
                        c.CoordinatorType.CoordinatorType,
                        coordinatorUser = c.Coordinator.User.Id,
                        avatar = c.Coordinator.User.Avatar == null || c.Coordinator.User.Avatar == "" ? "Files/assets/avatar.png" : c.Coordinator.User.Avatar,
                        Accepted = c.Accepted == null ? "pending" : c.Accepted != null && c.Accepted != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.Accepted == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A",
                        coordinatorAcceptance = c.StatusId
                    }),
                    supplierConsultantImm = s.ImmigrationSupplierPartners.Select(c => new
                    {
                        Id = c.SupplierId,
                        c.Supplier.Name,
                        supplierConsultant_Id = c.Supplier.UserId,
                        consultantAssigned = c.StatusId == 2 ? false : true,
                        supplierConsultantId = c.SupplierTypeId,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        supplierAcceptance = c.StatusId,
                        Accepted = c.AcceptedDate == null ? "pending" : c.AcceptedDate != null && c.AcceptedDate != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.AcceptedDate == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A"
                    }),
                    supplierConsultantRelo = s.RelocationSupplierPartners.Select(c => new
                    {
                        Id = c.SupplierId,
                        c.Supplier.Name,
                        supplierConsultant_Id = c.Supplier.UserId,
                        consultantAssigned = c.StatusId == 2 ? false : true,
                        supplierConsultantId = c.SupplierTypeId,
                        country = c.SupplierType.Name,
                        avatar = c.Supplier.User.Avatar == null || c.Supplier.User.Avatar == "" ? "Files/assets/avatar.png" : c.Supplier.User.Avatar,
                        supplierAcceptance = c.StatusId,
                        Accepted = c.AcceptedDate == null ? "pending" : c.AcceptedDate != null && c.AcceptedDate != Convert.ToDateTime("1900/01/01")
                        ? "accepted" : c.AcceptedDate == Convert.ToDateTime("1900/01/01") ? "reject" : "N/A",
                    }),
                    supplierId = s.ImmigrationSupplierPartners.Select(c => c.SupplierId),
                    s.Urgent,
                    standaloneServices = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho,
                        country = _s.DeliveringInNavigation.Name,
                        serviceLine = _s.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                        && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                        && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                       && f.IdService == _s.ServiceId).NickName
                    }).ToList(),
                    bundledService = _context.BundledServices
                    .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id)
                    .Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho,
                        country = _s.DeliveringInNavigation.Name,
                        serviceLine = _s.BundledServiceOrder.WorkOrder.ServiceLineId,
                        nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                       && f.IdService == _s.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                       && f.IdService == _s.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.PartnerId
                      && f.IdService == _s.ServiceId).NickName
                    }).ToList(),
                    coordinatorStatusImm = s.ImmigrationCoodinators.Select(x => !x.StatusId.HasValue ? 2 : x.StatusId),
                    coordinatorStatusRelo = s.RelocationCoordinators.Select(x => !x.StatusId.HasValue ? 2 : x.StatusId),
                    consultantStatusImm = s.ImmigrationSupplierPartners.Select(x => !x.StatusId.HasValue ? 2 : x.StatusId),
                    consultantStatusRelo = s.RelocationSupplierPartners.Select(x => !x.StatusId.HasValue ? 2 : x.StatusId)
                }).ToList();

            pendingAcceptance = pendingAcceptance.Where(x => !x.consultantStatusImm.Contains(3) || !x.consultantStatusRelo.Contains(3)).ToList();

            var _pendingAcceptanceImm = pendingAcceptance.Where(x => x.coordinatorStatusImm.Any() && x.consultantStatusImm.Any()).ToList();
            var _pendingAcceptanceRelo = pendingAcceptance.Where(x => x.coordinatorStatusRelo.Any() && x.consultantStatusRelo.Any()).ToList();

            _pendingAcceptanceImm.Union(_pendingAcceptanceRelo);
            _pendingAcceptanceImm = _pendingAcceptanceImm.Where(x => x.coordinatorStatusImm.Contains(2) || x.coordinatorStatusRelo.Contains(2) || x.consultantStatusImm.Contains(2) || x.consultantStatusRelo.Contains(2)).ToList();
            //pendingAcceptance = pendingAcceptance.Where(x => x.coordinatorStatusImm.Contains(2)).ToList();
            //pendingAcceptance = pendingAcceptance.Where(x => x.coordinatorStatusRelo.Contains(2)).ToList();
            //pendingAcceptance = pendingAcceptance.Where(x => x.consultantStatusImm.Contains(2)).ToList();
            //pendingAcceptance = pendingAcceptance.Where(x => x.consultantStatusRelo.Contains(2)).ToList();


            if (country.HasValue)
            {
                _pendingAcceptanceImm = pendingAcceptance.Where(x => x.homeCountryId == country.Value || x.hostCountryId == country.Value).ToList();
                pendingAssigment = pendingAssigment.Where(x => x.homeCountryId == country.Value || x.hostCountryId == country.Value).ToList();
            }

            if (city.HasValue)
            {
                _pendingAcceptanceImm = pendingAcceptance.Where(x => x.homeCityId == city.Value || x.hostCityId == city.Value).ToList();
                pendingAssigment = pendingAssigment.Where(x => x.homeCityId == city.Value || x.hostCityId == city.Value).ToList();
            }

            if (service_line.HasValue)
            {
                if (service_line == 1)
                {
                    _pendingAcceptanceImm = pendingAcceptance.Where(x => x.serviceLine.ToLower() == "Immigration".ToLower()).ToList();
                    pendingAssigment = pendingAssigment.Where(x => x.serviceLine.ToLower() == "Immigration".ToLower()).ToList();
                }

                if (service_line == 2)
                {
                    _pendingAcceptanceImm = pendingAcceptance.Where(x => x.serviceLine.ToLower() == "Relocation".ToLower()).ToList();
                    pendingAssigment = pendingAssigment.Where(x => x.serviceLine.ToLower() == "Relocation".ToLower()).ToList();
                }

            }

            if (sr.HasValue)
            {
                _pendingAcceptanceImm = pendingAcceptance.Where(x => x.Id == sr.Value).ToList();
                pendingAssigment = pendingAssigment.Where(x => x.Id == sr.Value).ToList();
            }

            var pendingAuthorizations = new
            {
                pendingAcceptance = _pendingAcceptanceImm.Count(),
                pendingAssignments = pendingAssigment.Count(),
                pendingTotal = _pendingAcceptanceImm.Count() + pendingAssigment.Count(),
                pendingAuthorizations = new
                {
                    Acceptance = _pendingAcceptanceImm.OrderByDescending(x => x.Urgent),
                    Assigment = pendingAssigment.OrderByDescending(x => x.Urgent)
                },
            };
            return new ObjectResult(pendingAuthorizations);
        }

        public ActionResult GetUpcomingArrivals(int user, DateTime? rangeDate1, DateTime? rangeDate2, int? city,
            int? partner, int? client, int? coordinator, int? supplierPartner)
        {
            rangeDate1 = rangeDate1.HasValue ? rangeDate1.Value : DateTime.Now;
            rangeDate2 = rangeDate2.HasValue ? rangeDate2.Value : DateTime.Now.AddYears(2);
            var upcomingArrivals = _context.ServiceRecords.Select(s => new
            {
                s.Id,
                s.NumberServiceRecord,
                s.Vip,
                s.AssigneeInformations.FirstOrDefault().InitialArrival,
                s.AssigneeInformations.FirstOrDefault().AssigneeName,
                s.Partner.Name,
                s.PartnerId,
                client = s.Client.Name,
                s.ClientId,
                coordinator = s.ImmigrationCoodinators.Any()
                    ? s.ImmigrationCoodinators.FirstOrDefault().Coordinator.Name
                    : s.RelocationCoordinators.FirstOrDefault().Coordinator.Name,
                coordinatorId = s.ImmigrationCoodinators.Any()
                    ? s.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : s.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                supplier = s.ImmigrationSupplierPartners.Any()
                    ? s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name
                    : s.RelocationSupplierPartners.FirstOrDefault().Supplier.Name,
                supplierId = s.ImmigrationSupplierPartners.Any()
                    ? s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId
                    : s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                location = s.AssigneeInformations.FirstOrDefault().HostCity.City,
                s.AssigneeInformations.FirstOrDefault().HostCityId,
                services = _context.StandaloneServiceWorkOrders.Count(x => x.WorkOrder.ServiceRecordId == s.Id)
                           + _context.BundledServicesWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id)
                               .Select(_s => _s.BundledServices).Count(),
                servicesData = s.WorkOrders.Count != 0
                    ? _context.StandaloneServiceWorkOrders.Where(x =>
                            x.WorkOrder.ServiceRecordId == s.Id && x.WorkOrder.ServiceLineId == 1)
                        .Select(_s => new
                        {
                            id = _s.WorkOrderServiceId,
                            _s.Service.Service,
                            _s.ServiceNumber,
                            _s.WorkOrder.NumberWorkOrder,
                            _s.CategoryId,
                            _s.Autho
                        }).Union(_context.BundledServices
                            .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id &&
                                        x.BundledServiceOrder.WorkOrder.ServiceLineId == 1).Select(_s => new
                                        {
                                            id = _s.WorkServicesId,
                                            _s.Service.Service,
                                            _s.ServiceNumber,
                                            _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                                            _s.CategoryId,
                                            _s.Autho
                                        })).ToList()
                    : null,
                serviceLine = WhichServiceline(s.ImmigrationCoodinators.Any(), s.RelocationCoordinators.Any())
            }).ToList();
            //Query por Role
            int? role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;
            int? profileId = _context.ProfileUsers.SingleOrDefault(x => x.UserId == user).Id;
            switch (role)
            {
                case 2: // Coordinator
                    upcomingArrivals = upcomingArrivals.Where(x => x.coordinatorId == profileId).ToList();
                    break;
                case 3: // Supplier
                    upcomingArrivals = upcomingArrivals.Where(x => x.supplierId == profileId).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            if (rangeDate1.HasValue && rangeDate2.HasValue)
                upcomingArrivals = upcomingArrivals
                    .Where(x => x.InitialArrival >= rangeDate1.Value && x.InitialArrival <= rangeDate2.Value).ToList();
            if (city.HasValue)
                upcomingArrivals = upcomingArrivals.Where(x => x.HostCityId == city.Value).ToList();
            if (partner.HasValue)
                upcomingArrivals = upcomingArrivals.Where(x => x.PartnerId == partner.Value).ToList();
            if (client.HasValue)
                upcomingArrivals = upcomingArrivals.Where(x => x.ClientId == client.Value).ToList();
            if (coordinator.HasValue)
                upcomingArrivals = upcomingArrivals.Where(x => x.coordinatorId == coordinator.Value).ToList();
            if (supplierPartner.HasValue)
                upcomingArrivals = upcomingArrivals.Where(x => x.supplierId == supplierPartner.Value).ToList();
            return new ObjectResult(upcomingArrivals.OrderByDescending(x => x.InitialArrival));
        }

        private static string WhichServiceline(bool immigration, bool relocation)
        {
            string serviceLine = "";
            if (immigration.Equals(true) && relocation.Equals(true))
            {
                serviceLine = "R & I";
            }
            else if (immigration.Equals(true) && relocation.Equals(false))
            {
                serviceLine = "I";
            }
            else if (immigration.Equals(false) && relocation.Equals(true))
            {
                serviceLine = "R";
            }
            return serviceLine;
        }

        public ActionResult GetCalls(int user, string caller, int? sr, int? wo, int? service)
        {
            var calls = _context.Calls.Select(s => new
            {
                s.Id,
                s.ServiceRecordId,
                s.ServiceRecord.NumberServiceRecord,
                s.WorkOrderId,
                s.WorkOrder.NumberWorkOrder,
                s.ServiceId,
                service = s.Service.BundledServices.Any() ? s.Service.BundledServices.FirstOrDefault().ServiceNumber
                                                          : s.Service.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber,
                callerId = s.Caller,
                caller = s.CallerNavigation.Name + " " + s.CallerNavigation.LastName + " " + s.CallerNavigation.MotherLastName,
                calleeId = s.Calle,
                callee = s.CalleNavigation.Name + " " + s.CalleNavigation.LastName + " " + s.CalleNavigation.MotherLastName,
                s.Date,
                s.Time,
                s.DurationNavigation.Duration,
                escalate = s.Escalate.HasValue ? s.Escalate : false
            }).ToList();
            if (caller != null)
                calls = calls.Where(x => x.caller.StartsWith(caller) || x.callee.StartsWith(caller)).ToList();
            if (sr.HasValue)
                calls = calls.Where(x => x.ServiceRecordId == sr.Value).ToList();
            if (wo.HasValue)
                calls = calls.Where(x => x.WorkOrderId == wo.Value).ToList();
            if (service.HasValue)
                calls = calls.Where(x => x.ServiceId == service.Value).ToList();
            return new ObjectResult(calls);
        }

        public ActionResult GetworkOrderBySR(int sr)
        {
            var wo = _context.WorkOrders.Where(x => x.ServiceRecordId == sr).Select(s => new
            {
                s.Id,
                s.NumberWorkOrder,
                s.ServiceLineId,
                s.ServiceLine.ServiceLine
            }).ToList();
            return new ObjectResult(wo);
        }

        public List<biz.premier.Entities.WorkOrder> GetWorkOrderByServiceRecord(int sr)
        {
            var wo = _context.WorkOrders
                .Include(d => d.StandaloneServiceWorkOrders)
                .Include(d => d.BundledServicesWorkOrders)
                .Where(x => x.ServiceRecordId == sr).ToList();
                //.Select(s => new WorkOrderEstatus
                //{

                //    Id = s.Id,
                //    StandaloneServiceWorkOrders = s.StandaloneServiceWorkOrders.Select(a => new
                //    {
                //        a.Id,
                //        a.StatusId,
                //        a.DeliveredTo,
                //        a.DeliveringIn
                //    }).ToList(),
                //    //BundledServicesWorkOrders = (BundledServicesWorkOrders)s.BundledServicesWorkOrders.ToList()
                //    //StandaloneServiceWorkOrders = s.StandaloneServiceWorkOrders
                //    //.Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                //    //    (stand, assig) => new
                //    //    {
                //    //        Id = stand.Id,
                //    //        StatusId = stand.StatusId.Value,
                //    //        DeliveredTo = stand.DeliveredTo,
                //    //        DeliveringIn = stand.DeliveringIn
                //    //    }).ToList()
                //    //BundledServicesWorkOrders = (ICollection<BundledServicesWorkOrder>)s.BundledServicesWorkOrders
                //    //.Join(_context.BundledServices, bunWork => bunWork.Id, bunService => bunService.BundledServiceOrderId,
                //    //    (bunWork, bunService) => new
                //    //    {
                //    //        Id = bunService.Id,
                //    //        StatusId = bunService.StatusId.Value,
                //    //        DeliveredTo = bunService.DeliveredTo,
                //    //        DeliveringIn = bunService.DeliveringIn,
                //    //        ServiceOrderServicesId = bunService.WorkServicesId
                //    //    })
                //    //.Join(_context.AssignedServiceSupliers, stand => stand.ServiceOrderServicesId, assig => assig.ServiceOrderServicesId,
                //    //    (stand, assig) => new
                //    //    {
                //    //        Id = stand.Id,
                //    //        StatusId = stand.StatusId,
                //    //        DeliveredTo = stand.DeliveredTo,
                //    //        DeliveringIn = stand.DeliveringIn,
                //    //        ServiceOrderServicesId = stand.ServiceOrderServicesId
                //    //    }).ToList()
                //}).ToList();
    
            return wo;
        }

        //public List<StandaloneServiceWorkOrder> GetStandAloneByWorkOrder(int workOrder)
        //{
        //    var standAlone = _context.StandaloneServiceWorkOrders
        //                   .Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
        //                   (stand, assig) => StandaloneServiceWorkOrder
        //                   {
        //                       stand
                              
        //                   }).Where(x => x.stand.WorkOrderId == workOrder).ToList();

        //    //var bundle = workOrder[i].BundledServicesWorkOrders
        //    //    .Where(x => x.WorkOrderId == workOrder[i].Id).ToList();
        //    return standAlone;
        //}
        public Boolean IsAssigned(int swo)
        {
            return _context.AssignedServiceSupliers.Any(x => x.ServiceOrderServicesId == swo);
        }
        public ActionResult GetReminders(int user, DateTime? rangeDate1, DateTime? rangeDate2, int? city, int? sr, int? sl, int? wo)
        {

            var remindersAll = _context.ReminderSchoolingSearches
                .Where(x => x.CreatedBy == user)
               .Join(_context.SchoolingSearches, remi => remi.SchoolingSearchId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                      && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                      && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                     && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveringIn
               })
               .Union(
                _context.ReminderHomeFindings
                .Where(x => x.CreatedBy == user)
               .Join(_context.HomeFindings, remi => remi.HomeFindingId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(
                _context.ReminderDepartures
                .Where(x => x.CreatedBy == user)
               .Join(_context.Departures, remi => remi.DepartaureId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(
                _context.ReminderPredecisionOrientations
                .Where(x => x.CreatedBy == user)
               .Join(_context.PredecisionOrientations, remi => remi.PredecisionOrientationId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(
                _context.ReminderAreaOrientations
                .Where(x => x.CreatedBy == user)
               .Join(_context.AreaOrientations, remi => remi.AreaOrientationId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(
                _context.ReminderSettlingIns
                .Where(x => x.CreatedBy == user)
               .Join(_context.SettlingIns, remi => remi.SettlingInId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(
                _context.ReminderTransportations
                .Where(x => x.CreatedBy == user)
               .Join(_context.Transportations, remi => remi.TransportationId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(
                _context.ReminderAirportTransportationServices
                .Where(x => x.CreatedBy == user)
               .Join(_context.AirportTransportationServices, remi => remi.AirportTransportationServicesId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(_context.ReminderLeaseRenewals
                .Where(x => x.CreatedBy == user)
               .Join(_context.LeaseRenewals, remi => remi.LeaseRenewal, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  WorkOrderServicesId = scho.WorkOrderServices.Value,
                  remi.CreatedBy,
                  AuthoDate = scho.AuthoDate.Value
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(_context.ReminderOthers
                .Where(x => x.CreatedBy == user)
               .Join(_context.Others, remi => remi.Other, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  WorkOrderServicesId = scho.WorkOrderServices.Value,
                  remi.CreatedBy,
                  AuthoDate = scho.AuthoDate.Value
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(_context.ReminderTemporaryHousingCoordinatons
                .Where(x => x.CreatedBy == user)
               .Join(_context.TemporaryHousingCoordinatons, remi => remi.TemporaryHousingCoordinationId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  WorkOrderServicesId = scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  AuthoDate = scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .Union(_context.ReminderRentalFurnitureCoordinations
                .Where(x => x.CreatedBy == user)
               .Join(_context.RentalFurnitureCoordinations, remi => remi.RentalFurnitureCoordinationId, scho => scho.Id
              , (remi, scho) => new
              {
                  scho.Id,
                  WorkOrderServicesId = scho.WorkOrderServicesId,
                  remi.CreatedBy,
                  AuthoDate = scho.AuthoDate
              })
               .Join(_context.StandaloneServiceWorkOrders, a => a.WorkOrderServicesId, b => b.WorkOrderServiceId,
               (a, b) => new
               {
                   a.Id,
                   b.WorkOrder.ServiceRecordId,
                   b.WorkOrder.ServiceLineId,
                   b.WorkOrderId,
                   a.WorkOrderServicesId,
                   b.WorkOrder.ServiceRecord.NumberServiceRecord,
                   b.WorkOrder.ServiceLine.ServiceLine,
                   nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                  && f.IdService == b.ServiceId).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == b.WorkOrder.ServiceRecord.ClientId
                 && f.IdService == b.ServiceId).NickName,
                   a.AuthoDate,
                   b.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                   partnerName = b.WorkOrder.ServiceRecord.Partner.Name,
                   clientName = b.WorkOrder.ServiceRecord.Client.Name,
                   city = b.Location,
                   cityId = b.DeliveredTo
               }))
               .ToList();

            if (rangeDate1.HasValue && rangeDate2.HasValue)
                remindersAll = remindersAll.Where(x => x.AuthoDate > rangeDate1.Value && x.AuthoDate < rangeDate2.Value).ToList();
            if (city.HasValue)
                remindersAll = remindersAll.Where(x => x.cityId == city.Value).ToList();
            if (sr.HasValue)
                remindersAll = remindersAll.Where(x => x.ServiceRecordId == sr.Value).ToList();
            if (sl.HasValue)
                remindersAll = remindersAll.Where(x => x.ServiceLineId == sl.Value).ToList();
            if (wo.HasValue)
                remindersAll = remindersAll.Where(x => x.WorkOrderId == wo.Value).ToList();
            //Query por Role

            return new ObjectResult(remindersAll); 
        }

        public ActionResult AddReminder(int user, int service, DateTime? reminderDate, int category, string comment)
        {
            switch (category)
            {
                case (1):
                    ReminderEntryVisa reminderEntryVisa = new ReminderEntryVisa();
                    reminderEntryVisa.CreatedBy = user;
                    reminderEntryVisa.CreatedDate = DateTime.Now;
                    reminderEntryVisa.EntryVisaId = service;
                    reminderEntryVisa.ReminderComments = comment;
                    reminderEntryVisa.ReminderDate = reminderDate.Value;
                    _context.ReminderEntryVisas.Add(reminderEntryVisa);
                    break;
                case (2):
                    ReminderWorkPermit reminderWorkPermit = new ReminderWorkPermit();
                    reminderWorkPermit.CreatedBy = user;
                    reminderWorkPermit.CreatedDate = DateTime.Now;
                    reminderWorkPermit.WorkPermitId = service;
                    reminderWorkPermit.ReminderComments = comment;
                    reminderWorkPermit.ReminderDate = reminderDate.Value;
                    _context.ReminderWorkPermits.Add(reminderWorkPermit);
                    break;
                case (3):
                    ReminderVisaDeregistration reminderVisaDeregistration = new ReminderVisaDeregistration();
                    reminderVisaDeregistration.CreatedBy = user;
                    reminderVisaDeregistration.CreatedDate = DateTime.Now;
                    reminderVisaDeregistration.VisaDeregistrationId = service;
                    reminderVisaDeregistration.ReminderComments = comment;
                    reminderVisaDeregistration.ReminderDate = reminderDate.Value;
                    _context.ReminderVisaDeregistrations.Add(reminderVisaDeregistration);
                    break;
                case (4):
                    ReminderResidencyPermit reminderResidencyPermit = new ReminderResidencyPermit();
                    reminderResidencyPermit.CreatedBy = user;
                    reminderResidencyPermit.CreatedDate = DateTime.Now;
                    reminderResidencyPermit.ResidencyPermitId = service;
                    reminderResidencyPermit.ReminderComments = comment;
                    reminderResidencyPermit.ReminderDate = reminderDate.Value;
                    _context.ReminderResidencyPermits.Add(reminderResidencyPermit);
                    break;
                case (5):
                    ReminderDocumentManagement reminderDocumentManagement = new ReminderDocumentManagement();
                    reminderDocumentManagement.CreatedBy = user;
                    reminderDocumentManagement.CreatedDate = DateTime.Now;
                    reminderDocumentManagement.DocumentManagementId = service;
                    reminderDocumentManagement.ReminderComments = comment;
                    reminderDocumentManagement.ReminderDate = reminderDate.Value;
                    _context.ReminderDocumentManagements.Add(reminderDocumentManagement);
                    break;
                case (6):
                    ReminderLocalDocumentation reminderLocalDocumentation = new ReminderLocalDocumentation();
                    reminderLocalDocumentation.CreatedBy = user;
                    reminderLocalDocumentation.CreatedDate = DateTime.Now;
                    reminderLocalDocumentation.LocalDocumentationId = service;
                    reminderLocalDocumentation.ReminderComments = comment;
                    reminderLocalDocumentation.ReminderDate = reminderDate.Value;
                    _context.ReminderLocalDocumentations.Add(reminderLocalDocumentation);
                    break;
                case (7):
                    RemiderCorporateAssistance remiderCorporateAssistance = new RemiderCorporateAssistance();
                    remiderCorporateAssistance.CreatedBy = user;
                    remiderCorporateAssistance.CreatedDate = DateTime.Now;
                    remiderCorporateAssistance.CorporateAssistanceId = service;
                    remiderCorporateAssistance.ReminderComments = comment;
                    remiderCorporateAssistance.ReminderDate = reminderDate.Value;
                    _context.RemiderCorporateAssistances.Add(remiderCorporateAssistance);
                    break;
                case (8):
                    ReminderRenewal reminderRenewal = new ReminderRenewal();
                    reminderRenewal.CreatedBy = user;
                    reminderRenewal.CreatedDate = DateTime.Now;
                    reminderRenewal.RenewalId = service;
                    reminderRenewal.ReminderComments = comment;
                    reminderRenewal.ReminderDate = reminderDate.Value;
                    _context.ReminderRenewals.Add(reminderRenewal);
                    break;
                case (9):
                    ReminderNotification reminderNotification = new ReminderNotification();
                    reminderNotification.CreatedBy = user;
                    reminderNotification.CreatedDate = DateTime.Now;
                    reminderNotification.NotificationId = service;
                    reminderNotification.ReminderComments = comment;
                    reminderNotification.ReminderDate = reminderDate.Value;
                    _context.ReminderNotifications.Add(reminderNotification);
                    break;
                case (10):
                    ReminderLegalReview reminderLegalReview = new ReminderLegalReview();
                    reminderLegalReview.CreatedBy = user;
                    reminderLegalReview.CreatedDate = DateTime.Now;
                    reminderLegalReview.LegalReviewId = service;
                    reminderLegalReview.ReminderComments = comment;
                    reminderLegalReview.ReminderDate = reminderDate.Value;
                    _context.ReminderLegalReviews.Add(reminderLegalReview);
                    break;
                case (12):
                    ReminderPredecisionOrientation reminderPredecisionOrientation = new ReminderPredecisionOrientation();
                    reminderPredecisionOrientation.CreatedBy = user;
                    reminderPredecisionOrientation.CreatedDate = DateTime.Now;
                    reminderPredecisionOrientation.PredecisionOrientationId = service;
                    reminderPredecisionOrientation.ReminderComments = comment;
                    reminderPredecisionOrientation.ReminderDate = reminderDate.Value;
                    _context.ReminderPredecisionOrientations.Add(reminderPredecisionOrientation);
                    break;
                case (13):
                    ReminderAreaOrientation reminderAreaOrientation = new ReminderAreaOrientation();
                    reminderAreaOrientation.CreatedBy = user;
                    reminderAreaOrientation.CreatedDate = DateTime.Now;
                    reminderAreaOrientation.AreaOrientationId = service;
                    reminderAreaOrientation.ReminderComments = comment;
                    reminderAreaOrientation.ReminderDate = reminderDate.Value;
                    _context.ReminderAreaOrientations.Add(reminderAreaOrientation);
                    break;
                case (14):
                    ReminderSettlingIn reminderSettlingIn = new ReminderSettlingIn();
                    reminderSettlingIn.CreatedBy = user;
                    reminderSettlingIn.CreatedDate = DateTime.Now;
                    reminderSettlingIn.SettlingInId = service;
                    reminderSettlingIn.ReminderComments = comment;
                    reminderSettlingIn.ReminderDate = reminderDate.Value;
                    _context.ReminderSettlingIns.Add(reminderSettlingIn);
                    break;
                case (15):
                    ReminderSchoolingSearch reminderSchoolingSearch = new ReminderSchoolingSearch();
                    reminderSchoolingSearch.CreatedBy = user;
                    reminderSchoolingSearch.CreatedDate = DateTime.Now;
                    reminderSchoolingSearch.SchoolingSearchId = service;
                    reminderSchoolingSearch.ReminderComments = comment;
                    reminderSchoolingSearch.ReminderDate = reminderDate.Value;
                    _context.ReminderSchoolingSearches.Add(reminderSchoolingSearch);
                    break;
                case (16):
                    ReminderDeparture reminderDeparture = new ReminderDeparture();
                    reminderDeparture.CreatedBy = user;
                    reminderDeparture.CreatedDate = DateTime.Now;
                    reminderDeparture.DepartaureId = service;
                    reminderDeparture.ReminderComments = comment;
                    reminderDeparture.ReminderDate = reminderDate.Value;
                    _context.ReminderDepartures.Add(reminderDeparture);
                    break;
                case (17):
                    ReminderTemporaryHousingCoordinaton reminderTemporaryHousingCoordinaton = new ReminderTemporaryHousingCoordinaton();
                    reminderTemporaryHousingCoordinaton.CreatedBy = user;
                    reminderTemporaryHousingCoordinaton.CreatedDate = DateTime.Now;
                    reminderTemporaryHousingCoordinaton.TemporaryHousingCoordinationId = service;
                    reminderTemporaryHousingCoordinaton.ReminderComments = comment;
                    reminderTemporaryHousingCoordinaton.ReminderDate = reminderDate.Value;
                    _context.ReminderTemporaryHousingCoordinatons.Add(reminderTemporaryHousingCoordinaton);
                    break;
                case (18):
                    ReminderRentalFurnitureCoordination reminderRentalFurnitureCoordination = new ReminderRentalFurnitureCoordination();
                    reminderRentalFurnitureCoordination.CreatedBy = user;
                    reminderRentalFurnitureCoordination.CreatedDate = DateTime.Now;
                    reminderRentalFurnitureCoordination.RentalFurnitureCoordinationId = service;
                    reminderRentalFurnitureCoordination.ReminderComments = comment;
                    reminderRentalFurnitureCoordination.ReminderDate = reminderDate.Value;
                    _context.ReminderRentalFurnitureCoordinations.Add(reminderRentalFurnitureCoordination);
                    break;
                case (19):
                    ReminderTransportation reminderTransportation = new ReminderTransportation();
                    reminderTransportation.CreatedBy = user;
                    reminderTransportation.CreatedDate = DateTime.Now;
                    reminderTransportation.TransportationId = service;
                    reminderTransportation.ReminderComments = comment;
                    reminderTransportation.ReminderDate = reminderDate.Value;
                    _context.ReminderTransportations.Add(reminderTransportation);
                    break;
                case (20):
                    ReminderAirportTransportationService reminderAirportTransportationService = new ReminderAirportTransportationService();
                    reminderAirportTransportationService.CreatedBy = user;
                    reminderAirportTransportationService.CreatedDate = DateTime.Now;
                    reminderAirportTransportationService.AirportTransportationServicesId = service;
                    reminderAirportTransportationService.ReminderComments = comment;
                    reminderAirportTransportationService.ReminderDate = reminderDate.Value;
                    _context.ReminderAirportTransportationServices.Add(reminderAirportTransportationService);
                    break;
                case (21):
                    ReminderHomeFinding reminderHomeFinding = new ReminderHomeFinding();
                    reminderHomeFinding.CreatedBy = user;
                    reminderHomeFinding.CreatedDate = DateTime.Now;
                    reminderHomeFinding.HomeFindingId = service;
                    reminderHomeFinding.ReminderComments = comment;
                    reminderHomeFinding.ReminderDate = reminderDate.Value;
                    _context.ReminderHomeFindings.Add(reminderHomeFinding);
                    break;
            }
            _context.SaveChanges();
            return new ObjectResult("Reminder Add Success");

        }

        public ActionResult SupplierPartnersActive(int? country, int? city, int? supplierPartner, DateTime? range1, DateTime? range2)
        {
            range1 = range1.HasValue ? range1.Value : DateTime.Today.AddDays(-90);
            range2 = range2.HasValue ? range2.Value : DateTime.Today.AddDays(90);
            var suppliers = _context.ProfileUsers.Where(x => x.Title == 1 &&
                (x.ImmigrationSupplierPartners.Any(s => s.ServiceRecord.InitialAutho >= range1 && s.ServiceRecord.InitialAutho <= range2)
                || x.RelocationSupplierPartners.Any(s => s.ServiceRecord.InitialAutho >= range1 && s.ServiceRecord.InitialAutho <= range2))
                ).Select(s => new
                {
                    s.Id,
                    supplierPartner = $"{s.Name} {s.LastName} {s.MotherLastName}",
                    countryId = s.Country,
                    country = s.CountryNavigation.Name,
                    cityId = s.City,
                    city = s.CityNavigation.City,
                    immigration = s.ImmigrationSupplierPartners.Count(),
                    relocation = s.RelocationSupplierPartners.Count(),
                    total = s.ImmigrationSupplierPartners.Count() + s.RelocationSupplierPartners.Count()
                }).ToList();
            if (country.HasValue)
                suppliers = suppliers.Where(x => x.countryId == country.Value).ToList();
            if (city.HasValue)
                suppliers = suppliers.Where(x => x.cityId == city.Value).ToList();
            if (supplierPartner.HasValue)
                suppliers = suppliers.Where(x => x.Id == supplierPartner.Value).ToList();
            return new ObjectResult(suppliers);
        }
        //public ActionResult GetActivity(int? office, int? status, int? serviceLine, DateTime? range1, DateTime? range2)
        //{
        //    var countries = _context.CatCountries.Select(s => new
        //    {
        //        s.Id,
        //        s.Name,
        //        servicesRecords = s.AssigneeInformationHomeCountries.Where(x => x.HomeCountryId == s.Id).Count() + s.AssigneeInformationHostCountryNavigations.Where(x => x.HostCountry == s.Id).Count(),
        //        office = s.AssigneeInformationHomeCountries.Select(o => o.ServiceRecord.Office)
        //            .Union(s.AssigneeInformationHostCountryNavigations.Select(_s => _s.ServiceRecord.Office)).ToList(),
        //        status = s.AssigneeInformationHomeCountries.Select(o => o.ServiceRecord.StatusId)
        //            .Union(s.AssigneeInformationHostCountryNavigations.Select(_s => _s.ServiceRecord.StatusId)).ToList(),
        //        immigration = s.AssigneeInformationHomeCountries.Where(i => i.ServiceRecord.ImmigrationCoodinators.Any()).ToList(),
        //        relocation = s.AssigneeInformationHomeCountries.Where(i => i.ServiceRecord.RelocationCoordinators.Any()).ToList(),
        //        maxDate = s.AssigneeInformationHomeCountries.Select(a => a.ServiceRecord.InitialAutho).OrderByDescending(t => t).FirstOrDefault(),
        //        minDate = s.AssigneeInformationHomeCountries.Select(a => a.ServiceRecord.InitialAutho).OrderBy(t => t).FirstOrDefault(),
        //    }).ToList().OrderByDescending(o => o.servicesRecords);

        //    if (office.HasValue)
        //        countries = countries.Where(x => x.office.Contains(office.Value)).ToList().OrderByDescending(o => o.servicesRecords);
        //    if (status.HasValue)
        //        countries = countries.Where(x => x.status.Contains(status.Value)).ToList().OrderByDescending(o => o.servicesRecords);
        //    if (serviceLine.HasValue)
        //    {
        //        if (serviceLine.Value == 1)
        //            countries = countries.Where(x => x.immigration.Any()).ToList().OrderByDescending(o => o.servicesRecords);
        //        else if (serviceLine.Value == 2)
        //            countries = countries.Where(x => x.relocation.Any()).ToList().OrderByDescending(o => o.servicesRecords);
        //    }
        //    if (range1.HasValue && range2.HasValue)
        //        countries = countries.Where(x => x.minDate >= range1.Value && x.maxDate <= range2.Value).ToList().OrderByDescending(o => o.servicesRecords);

        //    var pendingAssignment = new { id = 1, count = ServicePendingAssignment() };
        //    var active = new { id = 2, count = _context.ServiceRecords.Count(x => 
        //                                           x.StatusId == 2 && (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any())) 
        //                                       + _context.ServiceRecords.Count(x => 
        //                                           x.StatusId == 2 && (x.ImmigrationCoodinators.Any() || x.ImmigrationSupplierPartners.Any()))};
        //    var inProgress = new { id = 3, count = _context.ServiceRecords.Count(x => 
        //                                               x.StatusId == 3 && (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()))
        //                                           + _context.ServiceRecords.Count(x => 
        //                                               x.StatusId == 3 && (x.ImmigrationCoodinators.Any() || x.ImmigrationSupplierPartners.Any())) };
        //    var onHold = new { id = 4, count = _context.ServiceRecords.Count(x => 
        //                                           x.StatusId == 4 && (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()))
        //                                       + _context.ServiceRecords.Count(x => 
        //                                           x.StatusId == 4 && (x.ImmigrationCoodinators.Any() || x.ImmigrationSupplierPartners.Any())) };
        //    var invoiced = new { id = 5, count = _context.ServiceRecords.Count(x => 
        //                                             x.StatusId == 5 && (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()))
        //                                         + _context.ServiceRecords.Count(x => 
        //                                             x.StatusId == 5 && (x.ImmigrationCoodinators.Any() || x.ImmigrationSupplierPartners.Any())) };
        //    var canceled = new { id = 6, count = _context.ServiceRecords.Count(x => 
        //                                             x.StatusId == 6 && (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()))
        //                                         + _context.ServiceRecords.Count(x => 
        //                                             x.StatusId == 6 && (x.ImmigrationCoodinators.Any() || x.ImmigrationSupplierPartners.Any())) };
        //    var closed = new { id = 7, count = _context.ServiceRecords.Count(x => 
        //                                           x.StatusId == 7 && (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()))
        //                                       + _context.ServiceRecords.Count(x => 
        //                                           x.StatusId == 7 && (x.ImmigrationCoodinators.Any() || x.ImmigrationSupplierPartners.Any())) };
        //    var objectReturn = new
        //    {
        //        countries = countries.ToList().OrderByDescending(o => o.servicesRecords).Take(7),
        //        pendingAssignment,
        //        active,
        //        inProgress,
        //        onHold,
        //        invoiced,
        //        canceled,
        //        closed
        //    };
        //    return new ObjectResult(objectReturn);
        //}

        private int ServicePendingAssignment()
        {
            int[] status = new[] { 1, 2 };
            var pendingAssigmentImmigration = _context.ServiceRecords
                .Where(x => !x.ImmigrationSupplierPartners.Any() && status.Contains(x.StatusId.Value))
                .Select(s => new
                {
                    s.Id,
                    coordinatorStatus = s.ImmigrationCoodinators.FirstOrDefault().StatusId.Value,
                    consultantStatus = s.ImmigrationSupplierPartners.FirstOrDefault().StatusId.Value
                }).Where(x => x.coordinatorStatus == 2 || x.consultantStatus == 2).ToList();
            var pendingAssigmentRelocation = _context.ServiceRecords
                .Where(x => !x.RelocationSupplierPartners.Any() && status.Contains(x.StatusId.Value))
                .Select(s => new
                {
                    s.Id,
                    coordinatorStatus = s.RelocationCoordinators.FirstOrDefault().StatusId.Value,
                    consultantStatus = s.RelocationSupplierPartners.FirstOrDefault().StatusId.Value
                }).Where(x => x.coordinatorStatus == 2 || x.consultantStatus == 2).ToList();
            var pendingAssigment = pendingAssigmentImmigration.Union(pendingAssigmentRelocation).ToList();

            var pendingAcceptanceImmigration = _context.ServiceRecords
                .Where(x => status.Contains(x.StatusId.Value) && x.ImmigrationSupplierPartners.Any() && x.ImmigrationCoodinators.Any())
                .Select(s => new
                {
                    s.Id,
                    coordinatorStatus = s.ImmigrationCoodinators.FirstOrDefault().StatusId.Value,
                    consultantStatus = s.ImmigrationSupplierPartners.FirstOrDefault().StatusId.Value
                }).Where(x => x.coordinatorStatus == 2 || x.consultantStatus == 2).ToList();
            var pendingAcceptanceRelocation = _context.ServiceRecords
                .Where(x => status.Contains(x.StatusId.Value) && x.RelocationSupplierPartners.Any() && x.RelocationCoordinators.Any())
                .Select(s => new
                {
                    s.Id,
                    coordinatorStatus = s.RelocationCoordinators.FirstOrDefault().StatusId.Value,
                    consultantStatus = s.RelocationSupplierPartners.FirstOrDefault().StatusId.Value
                }).Where(x => x.coordinatorStatus == 2 || x.consultantStatus == 2).ToList();
            var pendingAcceptance = pendingAcceptanceImmigration.Union(pendingAcceptanceRelocation).ToList();
            return pendingAssigmentImmigration.Count() + pendingAssigmentRelocation.Count() + pendingAcceptanceImmigration.Count() + pendingAcceptanceRelocation.Count();
        }
        public ActionResult GetAllActiveServices(int? country)
        {
            var countries = _context.CatCountries.Select(s => new
            {
                s.Id,
                s.Name,
                immigration = _context.WorkOrders
                    .Count(x => (x.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId == s.Id ?
                                    x.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId == s.Id : x.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry == s.Id)
                                && x.ServiceLineId == 1),
                relocation = _context.WorkOrders
                    .Count(x => (x.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId == s.Id ?
                                    x.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId == s.Id : x.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry == s.Id)
                                && x.ServiceLineId == 2),
                //total = s.AssigneeInformationHomeCountries.Select(i => i.ServiceRecord.WorkOrders.Count()).Count()
            }).ToList().OrderByDescending(o => o.Id);
            if (country.HasValue)
                countries = countries.Where(x => x.Id == country.Value).ToList().OrderByDescending(o => o.Id);
            return new ObjectResult(countries);
        }
        public ActionResult GetReportByCountry(int country, int? partner, int? client, int? supplierPartner, DateTime? range1, DateTime? range2,
            int? status, int? serviceLine, int? serviceCategory, int? city)
        {
            var serviceRecordImmigration = _context.ServiceRecords
                .Where(x => (x.ImmigrationSupplierPartners.Any() || x.ImmigrationCoodinators.Any()) && (x.AssigneeInformations.FirstOrDefault().HomeCountryId == country || x.AssigneeInformations.FirstOrDefault().HostCountry == country))
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Immigration",
                    serviceLineId = 1,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityId = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCityId
                        : s.AssigneeInformations.FirstOrDefault().HostCityId,
                    City = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    coordinator = s.ImmigrationCoodinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();
            var serviceRecordRelocation = _context.ServiceRecords
                .Where(x => (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()) && (x.AssigneeInformations.FirstOrDefault().HomeCountryId == country || x.AssigneeInformations.FirstOrDefault().HostCountry == country))
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Relocation",
                    serviceLineId = 2,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityId = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCityId
                        : s.AssigneeInformations.FirstOrDefault().HostCityId,
                    City = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    coordinator = s.RelocationCoordinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.RelocationSupplierPartners.FirstOrDefault().Supplier.Name} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();

            var serviceRecords = serviceRecordImmigration.Union(serviceRecordRelocation).ToList();
            if (partner.HasValue)
                serviceRecords = serviceRecords.Where(x => x.PartnerId == partner.Value).ToList();
            if (client.HasValue)
                serviceRecords = serviceRecords.Where(x => x.clientId == client.Value).ToList();
            if (supplierPartner.HasValue)
                serviceRecords = serviceRecords.Where(x => x.supplierPartnerId == supplierPartner.Value).ToList();
            if (range1.HasValue && range2.HasValue)
                serviceRecords = serviceRecords.Where(x => x.InitialAutho >= range1.Value && x.InitialAutho <= range2.Value).ToList();
            if (status.HasValue)
                serviceRecords = serviceRecords.Where(x => x.StatusId == status.Value).ToList();
            if (city.HasValue)
                serviceRecords = serviceRecords.Where(x => x.cityId == city.Value).ToList();
            if (serviceLine.HasValue)
            {
                if (serviceLine.Value == 1)
                    serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
                else if (serviceLine.Value == 2)
                    serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            }
            if (serviceCategory.HasValue)
                serviceRecords = serviceRecords.Where(x => x.services.Select(s => s.CategoryId).Contains(city.Value)).ToList();

            var serviceRecordClient = _context.ClientPartnerProfiles
                .Where(x => (x.ServiceRecordPartners.Any(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country)
                    || x.ServiceRecordPartners.Any(y => y.AssigneeInformations.FirstOrDefault().HostCountry == country)) ||
                    (x.ServiceRecordClients.Any(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country)
                    || x.ServiceRecordClients.Any(y => y.AssigneeInformations.FirstOrDefault().HostCountry == country))
                    )
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.IdTypePartnerClientProfileNavigation.Type,
                    s.IdTypePartnerClientProfile,
                    services = s.IdTypePartnerClientProfile == 1
                        ? s.ServiceRecordPartners.Count(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country || y.AssigneeInformations.FirstOrDefault().HostCountry == country)
                        : s.ServiceRecordClients.Count(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country || y.AssigneeInformations.FirstOrDefault().HostCountry == country)
                }).ToList().OrderByDescending(o => o.services).Take(7);
            var countries = new
            {
                serviceRecords = serviceRecords.GroupBy(g => g.Id).Select(x => x.First()).ToList(),
                clientsPartners = serviceRecordClient
            };
            return new ObjectResult(countries);
        }

        public ActionResult GetReportByClientPartner(int country, int partner, int? client, int? supplierPartner, DateTime? range1, DateTime? range2, int? status, int? serviceLine, int? serviceCategory, int? city)
        {
            var serviceRecordImmigration = _context.ServiceRecords
                .Where(x => (x.ImmigrationSupplierPartners.Any() || x.ImmigrationCoodinators.Any())
                    && (x.PartnerId == partner || x.ClientId == partner) && (x.AssigneeInformations.FirstOrDefault().HomeCountryId == country || x.AssigneeInformations.FirstOrDefault().HostCountry == country))
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Immigration",
                    serviceLineId = 1,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityId = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCityId
                        : s.AssigneeInformations.FirstOrDefault().HostCityId,
                    City = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    coordinator = s.ImmigrationCoodinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();
            var serviceRecordRelocation = _context.ServiceRecords
                .Where(x => (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any())
                    && (x.PartnerId == partner || x.ClientId == partner) && (x.AssigneeInformations.FirstOrDefault().HomeCountryId == country || x.AssigneeInformations.FirstOrDefault().HostCountry == country))
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Relocation",
                    serviceLineId = 2,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityId = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCityId
                        : s.AssigneeInformations.FirstOrDefault().HostCityId,
                    City = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    coordinator = s.RelocationCoordinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.RelocationSupplierPartners.FirstOrDefault().Supplier.Name} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();

            var serviceRecords = serviceRecordImmigration.Union(serviceRecordRelocation).ToList();
            if (client.HasValue)
                serviceRecords = serviceRecords.Where(x => x.clientId == client.Value).ToList();
            if (supplierPartner.HasValue)
                serviceRecords = serviceRecords.Where(x => x.supplierPartnerId == supplierPartner.Value).ToList();
            if (range1.HasValue && range2.HasValue)
                serviceRecords = serviceRecords.Where(x => x.InitialAutho >= range1.Value && x.InitialAutho <= range2.Value).ToList();
            if (status.HasValue)
                serviceRecords = serviceRecords.Where(x => x.StatusId == status.Value).ToList();
            if (city.HasValue)
                serviceRecords = serviceRecords.Where(x => x.cityId == city.Value).ToList();
            if (serviceLine.HasValue)
            {
                if (serviceLine.Value == 1)
                    serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
                else if (serviceLine.Value == 2)
                    serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            }
            if (serviceCategory.HasValue)
                serviceRecords = serviceRecords.Where(x => x.services.Select(s => s.CategoryId).Contains(city.Value)).ToList();

            var serviceRecordClient = _context.ClientPartnerProfiles
                .Where(x => x.ClientPartnerProfileClientIdClientFromNavigations.Any(y => y.IdClientTo == partner)
                    && (x.ServiceRecordClients.Any(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country)
                    || x.ServiceRecordClients.Any(y => y.AssigneeInformations.FirstOrDefault().HostCountry == country)))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.IdTypePartnerClientProfileNavigation.Type,
                    s.IdTypePartnerClientProfile,
                    services = s.IdTypePartnerClientProfile == 1
                        ? s.ServiceRecordPartners.Count(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country || y.AssigneeInformations.FirstOrDefault().HostCountry == country)
                        : s.ServiceRecordClients.Count(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country || y.AssigneeInformations.FirstOrDefault().HostCountry == country)
                }).ToList().OrderByDescending(o => o.services).Take(7);
            var countries = new
            {
                serviceRecords = serviceRecords,
                clientsPartners = serviceRecordClient
            };
            return new ObjectResult(countries);
        }

        public ActionResult GetReportBySupplierPartner(int? country, int? partner, int? client, int supplierPartner, DateTime? range1, DateTime? range2, int? status,
            int? serviceLine, int? serviceCategory, int? city, int? consultant)
        {
            var serviceRecordImmigration = _context.ServiceRecords
                .Where(x => x.ImmigrationSupplierPartners.Where(y => y.SupplierId == supplierPartner).Any())
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Immigration",
                    serviceLineId = 1,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityHostId = s.AssigneeInformations.FirstOrDefault().HostCityId,
                    cityHost = s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    countryHostId = s.AssigneeInformations.FirstOrDefault().HostCountry,
                    countryHost = s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                    cityHomeId = s.AssigneeInformations.FirstOrDefault().HomeCityId,
                    cityHome = s.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                    countryHomeId = s.AssigneeInformations.FirstOrDefault().HomeCountryId,
                    countryHome = s.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                    cityId = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCityId
                        : s.AssigneeInformations.FirstOrDefault().HostCityId,
                    City = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    coordinator = s.ImmigrationCoodinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();
            var serviceRecordRelocation = _context.ServiceRecords
                .Where(x => x.RelocationSupplierPartners.Where(y => y.SupplierId == supplierPartner).Any())
                .Select(s => new
                {
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Relocation",
                    serviceLineId = 2,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityHostId = s.AssigneeInformations.FirstOrDefault().HostCityId,
                    cityHost = s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    countryHostId = s.AssigneeInformations.FirstOrDefault().HostCountry,
                    countryHost = s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                    cityHomeId = s.AssigneeInformations.FirstOrDefault().HomeCityId,
                    cityHome = s.AssigneeInformations.FirstOrDefault().HomeCity.Name,
                    countryHomeId = s.AssigneeInformations.FirstOrDefault().HomeCountryId,
                    countryHome = s.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                    cityId = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCityId
                        : s.AssigneeInformations.FirstOrDefault().HostCityId,
                    City = s.AssigneeInformations.FirstOrDefault().HomeCountryId == country
                        ? s.AssigneeInformations.FirstOrDefault().HomeCity.Name
                        : s.AssigneeInformations.FirstOrDefault().HostCity.City,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    coordinator = s.RelocationCoordinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.RelocationSupplierPartners.FirstOrDefault().Supplier.Name} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();

            var serviceRecords = serviceRecordImmigration.Union(serviceRecordRelocation).ToList();
            if (client.HasValue)
                serviceRecords = serviceRecords.Where(x => x.clientId == client.Value).ToList();
            if (consultant.HasValue)
                serviceRecords = serviceRecords.Where(x => x.coordinator.Select(s => s.CoordinatorId).Contains(consultant.Value)).ToList();
            if (range1.HasValue && range2.HasValue)
                serviceRecords = serviceRecords.Where(x => x.InitialAutho >= range1.Value && x.InitialAutho <= range2.Value).ToList();
            if (status.HasValue)
                serviceRecords = serviceRecords.Where(x => x.StatusId == status.Value).ToList();
            if (country.HasValue)
                serviceRecords = serviceRecords.Where(x => x.countryHomeId == country.Value || x.countryHostId == country.Value).ToList();
            if (city.HasValue)
                serviceRecords = serviceRecords.Where(x => x.cityHomeId == city.Value || x.cityHostId == city.Value).ToList();
            if (serviceLine.HasValue)
            {
                if (serviceLine.Value == 1)
                    serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
                else if (serviceLine.Value == 2)
                    serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            }
            if (serviceCategory.HasValue)
                serviceRecords = serviceRecords.Where(x => x.services.Select(s => s.CategoryId).Contains(city.Value)).ToList();

            var serviceRecordClient = _context.ClientPartnerProfiles
                .Where(x => x.ClientPartnerProfileClientIdClientFromNavigations.Where(y => y.IdClientTo == partner).Any()
                    && x.ServiceRecordClients.Where(y => y.AssigneeInformations.FirstOrDefault().HomeCountryId == country).Any()
                    || x.ServiceRecordClients.Where(y => y.AssigneeInformations.FirstOrDefault().HostCountry == country).Any())
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.IdTypePartnerClientProfileNavigation.Type,
                    s.IdTypePartnerClientProfile,
                    services = s.ServiceRecordClients.Count
                }).ToList().OrderByDescending(o => o.services).Take(7);
            var countries = new
            {
                serviceRecords = serviceRecords,
                clientsPartners = serviceRecordClient
            };
            return new ObjectResult(countries);
        }

        public ActionResult GetServiceRecordByStatus(int status, int? country, int? city, int? serviceLine, int? serviceRecord, int? partner, int? client, int? supplierPartner,
            DateTime? range1, DateTime? range2, int? serviceCategory)
        {
            var serviceRecordImmigration = _context.ServiceRecords
                .Where(x => (x.ImmigrationSupplierPartners.Any() || x.ImmigrationCoodinators.Any()) && x.StatusId == status)
                .Select(s => new
                {
                    urgent = s.Urgent,
                    pendingAssignment = s.ImmigrationSupplierPartners.Where(x => x.StatusId == 2).Any(),
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Immigration",
                    serviceLineId = 1,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityId = s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.City,
                    city = s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.CityNavigation.City,
                    countryId = s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Country,
                    country = s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.CountryNavigation.Name,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    arrivalDate = s.AssigneeInformations.FirstOrDefault().InitialArrival,
                    coordinator = s.ImmigrationCoodinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Name} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.ImmigrationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();
            var serviceRecordRelocation = _context.ServiceRecords
                .Where(x => (x.RelocationSupplierPartners.Any() || x.RelocationCoordinators.Any()) && x.StatusId == status)
                .Select(s => new
                {
                    urgent = s.Urgent,
                    pendingAssignment = s.RelocationSupplierPartners.Where(x => x.StatusId == 2).Any(),
                    s.Id,
                    s.NumberServiceRecord,
                    s.Vip,
                    serviceLine = "Relocation",
                    serviceLineId = 2,
                    s.Status.Status,
                    s.StatusId,
                    s.InitialAutho,
                    cityId = s.RelocationSupplierPartners.FirstOrDefault().Supplier.City,
                    city = s.RelocationSupplierPartners.FirstOrDefault().Supplier.CityNavigation.City,
                    countryId = s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.Country,
                    country = s.ImmigrationSupplierPartners.FirstOrDefault().Supplier.CountryNavigation.Name,
                    partner = s.Partner.Name,
                    s.PartnerId,
                    client = s.Client.Name,
                    clientId = s.ClientId,
                    assigneeName = $"{s.AssigneeInformations.FirstOrDefault().AssigneeName}",
                    arrivalDate = s.AssigneeInformations.FirstOrDefault().InitialArrival,
                    coordinator = s.RelocationCoordinators.Select(c => new { c.CoordinatorId, name = $"{c.Coordinator.Name} {c.Coordinator.LastName} {c.Coordinator.MotherLastName}" }),
                    supplierPartner = $"{s.RelocationSupplierPartners.FirstOrDefault().Supplier.Name} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.LastName} {s.RelocationSupplierPartners.FirstOrDefault().Supplier.MotherLastName}",
                    supplierPartnerId = s.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                    services = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkOrderServiceId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    }).Union(_context.BundledServices.Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == s.Id).Select(_s => new
                    {
                        id = _s.WorkServicesId,
                        _s.Service.Service,
                        _s.ServiceNumber,
                        _s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        _s.CategoryId,
                        _s.Autho
                    })).ToList()
                }).ToList();

            var serviceRecords = serviceRecordImmigration.Union(serviceRecordRelocation).ToList();
            if (country.HasValue)
                serviceRecords = serviceRecords.Where(x => x.countryId == country.Value).ToList();
            if (city.HasValue)
                serviceRecords = serviceRecords.Where(x => x.cityId == city.Value).ToList();
            if (serviceLine.HasValue)
                serviceRecords = serviceRecords.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            if (serviceRecord.HasValue)
                serviceRecords = serviceRecords.Where(x => x.Id == serviceRecord.Value).ToList();
            if (partner.HasValue)
                serviceRecords = serviceRecords.Where(x => x.PartnerId == partner.Value).ToList();
            if (client.HasValue)
                serviceRecords = serviceRecords.Where(x => x.clientId == client.Value).ToList();
            if (supplierPartner.HasValue)
                serviceRecords = serviceRecords.Where(x => x.supplierPartnerId == supplierPartner.Value).ToList();
            if (range1.HasValue && range2.HasValue)
                serviceRecords = serviceRecords.Where(x => x.InitialAutho > range1.Value && x.InitialAutho < range2.Value).ToList();
            if (serviceCategory.HasValue)
                serviceRecords = serviceRecords.Where(x => x.services.Select(s => s.CategoryId).Contains(serviceCategory.Value)).ToList();

            var serviceRecordClient = _context.ClientPartnerProfiles
                .Where(x => (x.ServiceRecordClients.Where(y => y.StatusId == status).Any()
                    || x.ServiceRecordClients.Where(y => y.StatusId == status).Any()) || (x.ServiceRecordPartners.Where(y => y.StatusId == status).Any()
                    || x.ServiceRecordPartners.Where(y => y.StatusId == status).Any()))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.IdTypePartnerClientProfileNavigation.Type,
                    s.IdTypePartnerClientProfile,
                    services = s.ServiceRecordClients.Count + s.ServiceRecordPartners.Count
                }).ToList().OrderByDescending(o => o.services).Take(7);

            var response = new
            {
                services = serviceRecords.OrderByDescending(o => o.InitialAutho).ToList(),
                partnerClient = serviceRecordClient
            };

            return new ObjectResult(response);

        }

        public ActionResult GetCountryByServiceRecord(int sr)
        {
            var consult = _context.ServiceRecords
                .Join(_context.AssigneeInformations, a => a.Id, e => e.ServiceRecordId
                , (a, e) => new
                {
                    a.Id,
                    CountryHomeName = _context.CatCountries.Any(x => x.Name.ToLower() == e.HomeCountry.Name.ToLower()) ? e.HomeCountry.Name : "",
                    idCountryHome = _context.CatCountries.Any(x => x.Name.ToLower() == e.HomeCountry.Name.ToLower()) ? _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == e.HomeCountry.Name.ToLower()).Id : 0,
                    CountryHostName = e.HostCountryNavigation.Name,
                    idCountryHost = e.HostCountry,
                }).Where(x => x.Id == sr).ToList();

            return new ObjectResult(consult);
        }

        public ActionResult GetServiceRecordByUserApp(int user)
        {
            var profile = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user);
            var consult = _context.ServiceRecords
                .Where(x => x.ImmigrationSupplierPartners.Any())
                .Join(_context.AssigneeInformations, a => a.Id, e => e.ServiceRecordId
                , (a, e) => new
                {
                    a.Id,
                    a.NumberServiceRecord,
                    e.AssigneeName,
                    consult_id = a.ImmigrationSupplierPartners.FirstOrDefault().SupplierId,
                    coordinator = a.ImmigrationCoodinators.Any()
                    ? a.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                    : a.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                })
                .Join(_context.ProfileUsers, r => r.consult_id, g => g.Id,
                (r, g) => new
                {
                    r.Id,
                    r.NumberServiceRecord,
                    r.AssigneeName,
                    r.consult_id,
                    g.UserId
                })
                .Where(x => x.UserId == user).ToList();
            var consultZero = _context.ServiceRecords
                .Where(x => x.RelocationSupplierPartners.Any())
                .Join(_context.AssigneeInformations, a => a.Id, e => e.ServiceRecordId
                    , (a, e) => new
                    {
                        a.Id,
                        a.NumberServiceRecord,
                        e.AssigneeName,
                        consult_id = a.RelocationSupplierPartners.FirstOrDefault().SupplierId,
                        coordinator = a.ImmigrationCoodinators.Any()
                            ? a.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                            : a.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                    })
                .Join(_context.ProfileUsers, r => r.consult_id, g => g.Id,
                    (r, g) => new
                    {
                        r.Id,
                        r.NumberServiceRecord,
                        r.AssigneeName,
                        r.consult_id,
                        g.UserId
                    })
                .Where(x => x.UserId == user).ToList();
            var consultTwo = _context.ServiceRecords
                .Join(_context.AssigneeInformations, a => a.Id, e => e.ServiceRecordId
                    , (a, e) => new
                    {
                        a.Id,
                        a.NumberServiceRecord,
                        e.AssigneeName,
                        consult_id = a.ImmigrationCoodinators.Any()
                            ? a.ImmigrationCoodinators.FirstOrDefault().CoordinatorId
                            : a.RelocationCoordinators.FirstOrDefault().CoordinatorId,
                    })
                .Join(_context.ProfileUsers, r => r.consult_id, g => g.Id,
                    (r, g) => new
                    {
                        r.Id,
                        r.NumberServiceRecord,
                        r.AssigneeName,
                        r.consult_id,
                        g.UserId
                    })
                .Where(x => x.UserId == user).ToList();
            var consultsMachUp = consult.Union(consultTwo).Union(consultZero).OrderBy(x => x.NumberServiceRecord).ToList();

            return new ObjectResult(consultsMachUp);
        }

        public ActionResult GetCategoryByCountry(int idcountry, int IdClientPartnerProfile, int IdserviceLine)
        {
            var consult = _context.Set<biz.premier.Entities.ServiceLocation>()
                .Join(_context.CatServices, a => a.IdService, e => e.Id
                , (a, e) => new
                {
                    a.Id,
                    a.IdServiceLine,
                    a.IdClientPartnerProfile,
                    e.CategoryId,
                    e.Category.Category
                })
                .Join(_context.ServiceLocationCountries, a => a.Id, e => e.IdServiceLocation
                , (a, e) => new
                {
                    e.IdCountry,
                    a.IdServiceLine,
                    a.IdClientPartnerProfile,
                    a.CategoryId,
                    a.Category,
                }).Where(x => x.IdCountry == idcountry && x.IdClientPartnerProfile == IdClientPartnerProfile && x.IdServiceLine == IdserviceLine).ToList();

            return new ObjectResult(consult);
        }

        public ActionResult GetDeliveredTo(int service, int category, int serviceType)
        {
            var deliveredTo = serviceType == 1
                ? _context.StandaloneServiceWorkOrders.Where(x => x.Id == service && x.CategoryId == category).Select(s => new
                {
                    id = s.Id,
                    name = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Name
                        : s.DeliveredToNavigation.AssigneeInformation.AssigneeName,
                    email = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Email
                        : s.DeliveredToNavigation.AssigneeInformation.Email,
                    phone = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Phone
                        : s.DeliveredToNavigation.AssigneeInformation.MobilePhone,
                    photo = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Photo
                        : s.DeliveredToNavigation.AssigneeInformation.Photo,
                    s.DeliveredToNavigation.RelationshipId
                }).FirstOrDefault()
                : _context.BundledServices.Where(x => x.Id == service && x.CategoryId == category).Select(s => new
                {
                    id = s.Id,
                    name = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Name
                        : s.DeliveredToNavigation.AssigneeInformation.AssigneeName,
                    email = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Email
                        : s.DeliveredToNavigation.AssigneeInformation.Email,
                    phone = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Phone
                        : s.DeliveredToNavigation.AssigneeInformation.MobilePhone,
                    photo = s.DeliveredToNavigation.RelationshipId != 7
                        ? s.DeliveredToNavigation.Photo
                        : s.DeliveredToNavigation.AssigneeInformation.Photo,
                    s.DeliveredToNavigation.RelationshipId
                }).FirstOrDefault();
            return new ObjectResult(deliveredTo);
        }

        public ActionResult CompleteReport(int sr, int serviceLine)
        {
            var summaryGeneral = _context.ServiceRecords
                .Where(x => x.Id == sr)
                .Select(s => new
                {
                    s.Id,
                    s.AssigneeInformations.FirstOrDefault().AssigneeName,
                    Partner = s.Partner.Name,
                    Client = s.Client.Name,
                    dependents = s.AssigneeInformations.FirstOrDefault().DependentInformations.Any() ? "Yes" : "No",
                    dependentsNo = s.AssigneeInformations.FirstOrDefault().DependentInformations.Any()
                        ? s.AssigneeInformations.FirstOrDefault().DependentInformations.Count
                        : 0,
                    coordinatorImmigration = s.ImmigrationCoodinators
                        .Where(x => x.Accepted.HasValue)
                        .Select(i => $"{i.Coordinator.Name} {i.Coordinator.LastName} {i.Coordinator.MotherLastName}")
                        .ToList(),
                    coordinatorRelocation = s.RelocationCoordinators
                        .Where(x => x.Accepted.HasValue)
                        .Select(i => $"{i.Coordinator.Name} {i.Coordinator.LastName} {i.Coordinator.MotherLastName}")
                        .ToList(),
                    homeCountry = s.AssigneeInformations.FirstOrDefault().HomeCountry.Name,
                    hostCountry = s.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                    supplierImmigration = s.ImmigrationSupplierPartners
                        .Select(i => $"{i.Supplier.Name} {i.Supplier.LastName} {i.Supplier.MotherLastName}")
                        .ToList(),
                    initialArrivalDate = s.AssigneeInformations.FirstOrDefault().InitialArrival,
                    finalArrivalDate = s.AssigneeInformations.FirstOrDefault().FinalMove
                });

            #region Immigration

            var immigrationBundled = serviceLine == 1 ? _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLine &&
                            x.BundledServiceOrder.WorkOrder.StatusId != 3)
                .Select(s => new
                {
                    s.Id,
                    services = s.Category.Category,
                    location = s.Location,
                    AuthorizedTime = Convert.ToInt32(s.BundledServiceOrder.TotalTime),
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                Status = ""
                            }),
                    authoDate = s.Autho,
                    closingDate = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                date = "N/A"
                            }),
                    documentObtained = s.CategoryId == 1 ? s.WorkServices.EntryVisas
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 8 ? s.WorkServices.Renewals
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 9 ? s.WorkServices.Notifications
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                document = 0
                            }),
                }).ToList() : null;
            var immigrationStandalone = serviceLine == 1 ? _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == serviceLine &&
                            x.WorkOrder.StatusId != 3)
                .Select(s => new
                {
                    s.Id,
                    services = s.Category.Category,
                    location = s.Location,
                    AuthorizedTime = s.AuthoTime.Value,
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                Status = ""
                            }),
                    authoDate = s.Autho,
                    closingDate = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                date = "N/A"
                            }),
                    documentObtained = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                document = 0
                            }),
                }).ToList() : null;
            var immigration = serviceLine == 1 ? immigrationBundled.Union(immigrationStandalone).ToList() : null;
            var serviceDetailImmi = serviceLine == 1 ? _context.ServiceReportDays
                .Include(i => i.ReportDay)
                .ThenInclude(i => i.WorkOrderNavigation).ThenInclude(i => i.StandaloneServiceWorkOrders)
                .Include(i => i.ReportDay)
                .ThenInclude(i => i.WorkOrderNavigation).ThenInclude(i => i.BundledServicesWorkOrders).ThenInclude(i => i.BundledServices)
                .Where(x => immigration.Select(s => s.Id).Contains(x.Service.Value))
                .Select(s => new
                {
                    s.Id,
                    service = ReturnCatagory(immigration.Select(r => new ServiceModel { service = r.services, id = r.Id }).ToList(),
                        s.Service.Value),
                    dates = ReturnDates(s.ReportDay.WorkOrderNavigation, s.Service.Value),
                    description = s.Comments,
                    date = s.CreatedDate,
                    conclusions = s.ConclusionServiceReportDays.ToList()
                }).ToList() : null;

            var mashupInfoImmigration = serviceDetailImmi != null ? serviceDetailImmi.Select(s => new
            {
                s.Id,
                s.service,
                s.dates,
                serviceDetails = serviceDetailImmi.Where(x => x.service == s.service).ToList(),
                conclusions = s.conclusions
            }).GroupBy(g => g.service).Select(s => s.FirstOrDefault()).ToList()
                : null;

            #endregion

            #region Relocation

            var relocationBundled = serviceLine == 2 ? _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLine &&
                            x.BundledServiceOrder.WorkOrder.StatusId != 3)
                .Select(s => new
                {
                    s.Id,
                    services = s.Category.Category,
                    location = s.Location,
                    AuthorizedTime = Convert.ToInt32(s.BundledServiceOrder.TotalTime),
                    status = s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                Status = ""
                            }),
                    authoDate = s.Autho,
                    closingDate = s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                date = "N/A"
                            }),
                    documentObtained = s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 16 ? s.WorkServices.Departures
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 19 ? s.WorkServices.Transportations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0

                            })
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0

                            })
                        : s.CategoryId == 26 ? s.WorkServices.Others
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 27 ? s.WorkServices.TenancyManagements
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.WorkServices.WorkPermits
                            .Select(r => new
                            {
                                document = 0
                            })
                }).ToList() : null;
            var relocationStandalone = serviceLine == 2 ? _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == serviceLine &&
                            x.WorkOrder.StatusId != 3)
                .Select(s => new
                {
                    s.Id,
                    services = s.Category.Category,
                    location = s.Location,
                    AuthorizedTime = s.AuthoTime.Value,
                    status = s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                r.Status.Status
                            })
                        : s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                Status = ""
                            }),
                    authoDate = s.Autho,
                    closingDate = s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"

                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                date = (r.StatusId == 3 || r.StatusId == 4 || r.StatusId == 5) ? r.UpdatedDate.Value.ToString() : "N/A"
                            })
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                date = "N/A"
                            }),
                    documentObtained = s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 26 ? s.WorkOrderService.Others
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.CategoryId == 27 ? s.WorkOrderService.TenancyManagements
                            .Select(r => new
                            {
                                document = (r.StatusId == 4 || r.StatusId == 5) ? 1 : 0
                            })
                        : s.WorkOrderService.WorkPermits
                            .Select(r => new
                            {
                                document = 0
                            })
                }).ToList() : null;
            var relocation = (relocationBundled != null && relocationStandalone != null)
                ? relocationBundled.Union(relocationStandalone).ToList()
                : null;
            var serviceDetailRelo = serviceLine == 2 && relocation != null ? _context.ServiceReportDays
                .Include(i => i.ReportDay)
                    .ThenInclude(i => i.WorkOrderNavigation).ThenInclude(i => i.StandaloneServiceWorkOrders)
                .Include(i => i.ReportDay)
                .ThenInclude(i => i.WorkOrderNavigation).ThenInclude(i => i.BundledServicesWorkOrders).ThenInclude(i => i.BundledServices)
                .Where(x => relocation.Select(s => s.Id).Contains(x.Service.Value))
                            .Select(s => new
                            {
                                s.Id,
                                service = ReturnCatagory(relocation.Select(r => new ServiceModel { service = r.services, id = r.Id }).ToList(),
                                    s.Service.Value),
                                dates = ReturnDates(s.ReportDay.WorkOrderNavigation, s.Service.Value),
                                description = s.Comments,
                                serviceStartTime = s.ReportDay.StartTime,
                                serviceEndTime = s.ReportDay.EndTime,
                                serviceStartDate = s.ReportDay.CreationDate,
                                serviceEndDate = s.ReportDay.UpdatedDate,
                                hoursUsed = s.Time,
                                conclusions = s.ConclusionServiceReportDays.ToList()
                            }).ToList() : null;
            var mashupInfo = serviceDetailRelo != null ? serviceDetailRelo.Select(s => new
            {
                s.Id,
                s.service,
                s.dates,
                serviceDetails = serviceDetailRelo.Where(x => x.service == s.service).ToList(),
                conclusions = s.conclusions
            }).GroupBy(g => g.service).Select(s => s.FirstOrDefault()).ToList()
                    : null;

            #endregion

            var completeReport = new
            {
                generalSummary = summaryGeneral,
                detailImmigration = serviceDetailImmi != null ? mashupInfoImmigration : null,
                immigrationSummary = immigration,
                relocationSummary = relocation,
                detailRelocation = serviceDetailRelo != null ? mashupInfo : null
            };
            return new ObjectResult(completeReport);
        }

        public ActionResult CompleteReportInfo(int sr)
        {
            var info = _context.ServiceRecords.Where(x => x.Id == sr).Select(s => new
            {
                relocation = s.WorkOrders.Any(a => a.ServiceLineId == 2 && a.StatusId != 3),
                immigration = s.WorkOrders.Any(a => a.ServiceLineId == 1 && a.StatusId != 3),
                immigrationServicesClosed = s.ImmigrationClosed,
                relocationServicesClosed = s.RelocationClosed,
                immigrtionReportComplete = s.ImmigrationCompleteReport,
                relocationReportComplete = s.RelocationCompleteReport,
                serviceRecordComplete = s.WorkOrders.All(a => a.StatusId == 3)
            }).FirstOrDefault();
            return new ObjectResult(info);
        }

        //public ActionResult GetCountriesSr(int sr)
        //{
        //    var country = _context.CatCountries
        //        .Include(i => i.CatCities)
        //        .Where(x =>
        //        x.AssigneeInformationHomeCountries.Select(s => s.ServiceRecordId).Contains(sr)
        //        || x.AssigneeInformationHostCountryNavigations.Select(s => s.ServiceRecordId).Contains(sr))
        //        .Select(s => new
        //        {
        //            s.Id,
        //            s.Name,
        //            cityId = s.CatCities.FirstOrDefault(x =>
        //                x.AssigneeInformationHomeCities.Select(w => w.ServiceRecordId).Contains(sr)
        //                || x.AssigneeInformationHostCities.Select(w => w.ServiceRecordId).Contains(sr)
        //                ).Id
        //        }).ToList();
        //    return new ObjectResult(country);
        //}

        public ActionResult GetCoordinatorsAndSupplierBySr(int sr)
        {
            var IC = _context.ImmigrationCoodinators.Where(x => x.ServiceRecordId == sr).Select(s => new
            {
                profileId = s.CoordinatorId,
                userId = s.Coordinator.UserId,
                name = $"{s.Coordinator.Name} {s.Coordinator.LastName} {s.Coordinator.MotherLastName}",
                type = s.Coordinator.User.Role.Role,
                phoneNumber = s.Coordinator.PhoneNumber,
                serviceLine = s.Coordinator.Immigration.Value ? 1 : 0,
                s.StatusId

            }).ToList();
            var RC = _context.RelocationCoordinators.Where(x => x.ServiceRecordId == sr).Select(s => new
            {
                profileId = s.CoordinatorId,
                userId = s.Coordinator.UserId,
                name = $"{s.Coordinator.Name} {s.Coordinator.LastName} {s.Coordinator.MotherLastName}",
                type = s.Coordinator.User.Role.Role,
                phoneNumber = s.Coordinator.PhoneNumber,
                serviceLine = s.Coordinator.Relocation.Value ? 2 : 0,
                s.StatusId
            }).ToList();
            var ISP = _context.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == sr).Select(s => new
            {
                profileId = s.SupplierId,
                userId = s.Supplier.UserId,
                name = $"{s.Supplier.Name} {s.Supplier.LastName} {s.Supplier.MotherLastName}",
                type = s.Supplier.User.Role.Role,
                phoneNumber = s.Supplier.PhoneNumber,
                serviceLine = s.Supplier.Immigration.Value ? 1 : 0,
                s.StatusId
            }).ToList();
            var RSP = _context.RelocationSupplierPartners.Where(x => x.ServiceRecordId == sr).Select(s => new
            {
                profileId = s.SupplierId,
                userId = s.Supplier.UserId,
                name = $"{s.Supplier.Name} {s.Supplier.LastName} {s.Supplier.MotherLastName}",
                type = s.Supplier.User.Role.Role,
                phoneNumber = s.Supplier.PhoneNumber,
                serviceLine = s.Supplier.Relocation.Value ? 2 : 0,
                s.StatusId
            }).ToList();
            var profiles = IC.Union(RC).ToList();
            profiles = profiles.Union(ISP).ToList();
            profiles = profiles.Union(RSP).ToList();
            return new ObjectResult(profiles.Where(x => x.StatusId != 4));
        }

        public ActionResult GetEmails(int sr)
        {
            var dependent = _context.DependentInformations.Where(x => x.AssigneeInformation.ServiceRecordId == sr && x.RelationshipId == 1)
                .Select(s => new
                {
                    s.Id,
                    name = $"{s.Relationship.Relationship} / {s.Name} / {s.Email}",
                    s.Email
                }).ToList();
            var assignee = _context.AssigneeInformations.Where(x => x.ServiceRecordId == sr).Select(s => new
            {
                s.Id,
                name = $"Assignee / {s.AssigneeName} / {s.Email}",
                s.Email
            }).ToList();
            return new ObjectResult(assignee.Union(dependent).OrderBy(x => x.name).ToList());
        }

        public bool IsNewServiceRecord(int sr)
        {
            bool isSuccess = false;
            var find = _context.ServiceRecords.FirstOrDefault(x => x.Id == sr && x.WorkOrders.Count() == 1);
            if (find != null)
            {
                isSuccess = true;
            }

            return isSuccess;
        }

        public int GetUserNotification(int idUserProfile)
        {
            int find = _context.ProfileUsers.Include(x => x.User).FirstOrDefault(x => x.Id == idUserProfile).User.Id;

            return find;
        }

        public class ServiceModel
        {
            public string service { get; set; }
            public int id { get; set; }
        }

        private static string ReturnCatagory(List<ServiceModel> model, int key)
        {
            string catagory = model.FirstOrDefault(f => f.id == key).service;
            return catagory;
        }

        private static Tuple<DateTime, string> ReturnDates(biz.premier.Entities.WorkOrder order, int service)
        {
            Tuple<DateTime, string> tuple = new Tuple<DateTime, string>(DateTime.Now, "");
            var @default = order.StandaloneServiceWorkOrders.FirstOrDefault(f => f.Id == service);
            var bundled = order.BundledServicesWorkOrders
                .FirstOrDefault(x => x.BundledServices.Select(s => s.Id).Contains(service))
                ?.BundledServices
                .FirstOrDefault(f => f.Id == service);
            if (@default != null)
            {
                tuple = new Tuple<DateTime, string>(@default.Autho.Value, @default.UpdatedDate.Value.ToString());
            }
            else if (bundled != null)
            {
                tuple = new Tuple<DateTime, string>(bundled.Autho.Value, bundled.UpdatedDate.Value.ToString());
            }

            return tuple;
        }

        public ActionResult GetSuppliersConsultantAndServiceBySr(int sr)
        {
            var suppliersImmigration = _context.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == sr)
                .Select(s => new
                {
                    s.SupplierId,
                    s.Supplier.Name,
                    Type = "Consultant"
                }).Distinct().ToList();
            var supplierRelocation = _context.RelocationSupplierPartners.Where(x => x.ServiceRecordId == sr)
                .Select(s => new
                {
                    s.SupplierId,
                    s.Supplier.Name,
                    Type = "Consultant"
                }).Distinct().ToList();
            var workOrderServices = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceLineId == sr).Select(s => s.WorkServicesId.Value)
                .ToArray();
            var workOrderServicesStandalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceLineId == sr).Select(s => s.WorkOrderServiceId.Value).ToArray();
            var wos = workOrderServices.Union(workOrderServicesStandalone).ToList();
            var rental = _context.RentalFurnitureCoordinations
                .Where(x => wos.Contains(x.WorkOrderServicesId)).Select(s => new
                {
                    SupplierId = s.SupplierPartner,
                    Name = s.SupplierPartnerNavigation.ComercialName,
                    Type = "Service"
                }).ToList();
            var temporary = _context.TemporaryHousingCoordinatons
                .Where(x => wos.Contains(x.WorkOrderServicesId)).Select(s => new
                {
                    SupplierId = s.SupplierPartner,
                    Name = s.IdAdministrativeContactNavigation.ContactName,
                    Type = "Service"
                }).ToList();
            // var transportations = _context.Transportations
            //     .Where(x => wos.Contains(x.WorkOrderServicesId)).Select(s => new
            //     {
            //         SupplierId = s.SupplierPartner,
            //         Name = s..ContactName
            //     }).ToList();
            var airport = _context.AirportTransportationServices
                .Where(x => wos.Contains(x.WorkOrderServicesId)).Select(s => new
                {
                    SupplierId = s.SupplierPartner,
                    Name = s.SupplierPartnerNavigation.ComercialName,
                    Type = "Service"
                }).ToList();
            var suppliers = suppliersImmigration.Union(supplierRelocation).ToList();
            suppliers = suppliers.Union(rental).ToList();
            suppliers = suppliers.Union(temporary).ToList();
            suppliers = suppliers.Union(airport).ToList();
            return new ObjectResult(suppliers);
        }

        public Tuple<bool, string> OnHoldServiceRecord(int sr, int serviceLine)
        {
            Tuple<bool, string> tuple = new Tuple<bool, string>(false, "");
            var serviceRecord = _context.ServiceRecords.Find(sr);
            var standaloneImmigration = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == 1)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkOrderServiceId,
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkOrderService.Others.FirstOrDefault().StatusId
                        : s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var bundledImmigration = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == 1)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkServicesId,
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkServices.Others.FirstOrDefault().StatusId
                        : s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var immigration = standaloneImmigration.Union(bundledImmigration).ToList();
            var standaloneRelocation = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == 2)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkOrderServiceId,
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkOrderService.Others.FirstOrDefault().StatusId
                        : s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var bundledRelocation = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == 2)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkServicesId,
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkServices.Others.FirstOrDefault().StatusId
                        : s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var relocation = standaloneRelocation.Union(bundledRelocation).ToList();
            if (immigration.Any(x => x.status != 3 || x.status != 4 || x.status != 5 || x.status != 32) && serviceLine == 1)
            {
                foreach (var i in immigration.Where(x => x.status != 3 || x.status != 4 || x.status != 5 || x.status != 33).ToList())
                {
                    switch (i.category)
                    {
                        case (1):
                            var entryVisa = _context.EntryVisas.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            entryVisa.StatusId = 33;
                            _context.EntryVisas.Update(entryVisa);
                            break;
                        case (2):
                            var workPermit = _context.WorkPermits.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            workPermit.StatusId = 33;
                            _context.WorkPermits.Update(workPermit);
                            break;
                        case (4):
                            var residencyPermit = _context.ResidencyPermits.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            residencyPermit.StatusId = 33;
                            _context.ResidencyPermits.Update(residencyPermit);
                            break;
                        case (3):
                            var visaDeregistration = _context.VisaDeregistrations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            visaDeregistration.StatusId = 33;
                            _context.VisaDeregistrations.Update(visaDeregistration);
                            break;
                        case (8):
                            var renewal = _context.Renewals.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            renewal.StatusId = 33;
                            _context.Renewals.Update(renewal);
                            break;
                        case (7):
                            var corporateAssistance = _context.CorporateAssistances.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            corporateAssistance.StatusId = 33;
                            _context.CorporateAssistances.Update(corporateAssistance);
                            break;
                        case (9):
                            var notification = _context.Notifications.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            notification.StatusId = 33;
                            _context.Notifications.Update(notification);
                            break;
                        case (10):
                            var legalReview = _context.LegalReviews.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            legalReview.StatusId = 33;
                            _context.LegalReviews.Update(legalReview);
                            break;
                        case (5):
                            var documentManagement = _context.DocumentManagements.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            documentManagement.StatusId = 33;
                            _context.DocumentManagements.Update(documentManagement);
                            break;
                        case (6):
                            var localDocumentation = _context.LocalDocumentations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            localDocumentation.StatusId = 33;
                            _context.LocalDocumentations.Update(localDocumentation);
                            break;
                        case (12):
                            var predecisionOrientation = _context.PredecisionOrientations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            predecisionOrientation.StatusId = 33;
                            _context.PredecisionOrientations.Update(predecisionOrientation);
                            break;
                        case (13):
                            var areaOrientation = _context.AreaOrientations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            areaOrientation.StatusId = 33;
                            _context.AreaOrientations.Update(areaOrientation);
                            break;
                        case (14):
                            var settlingIn = _context.SettlingIns.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            settlingIn.StatusId = 33;
                            _context.SettlingIns.Update(settlingIn);
                            break;
                        case (15):
                            var schoolingSearch = _context.SchoolingSearches.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            schoolingSearch.StatusId = 33;
                            _context.SchoolingSearches.Update(schoolingSearch);
                            break;
                        case (16):
                            var departure = _context.Departures.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            departure.StatusId = 33;
                            _context.Departures.Update(departure);
                            break;
                        case (17):
                            var temporaryHousingCoordinaton = _context.TemporaryHousingCoordinatons.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            temporaryHousingCoordinaton.StatusId = 33;
                            _context.TemporaryHousingCoordinatons.Update(temporaryHousingCoordinaton);
                            break;
                        case (18):
                            var rentalFurniture = _context.RentalFurnitureCoordinations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            rentalFurniture.StatusId = 33;
                            _context.RentalFurnitureCoordinations.Update(rentalFurniture);
                            break;
                        case (19):
                            var transportation = _context.Transportations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            transportation.StatusId = 33;
                            _context.Transportations.Update(transportation);
                            break;
                        case (20):
                            var airportTransportation = _context.AirportTransportationServices.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            airportTransportation.StatusId = 33;
                            _context.AirportTransportationServices.Update(airportTransportation);
                            break;
                        case (21):
                            var homeFinding = _context.HomeFindings.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            homeFinding.StatusId = 33;
                            _context.HomeFindings.Update(homeFinding);
                            break;
                        case (22):
                            var leaseRenewal = _context.LeaseRenewals.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            leaseRenewal.StatusId = 33;
                            _context.LeaseRenewals.Update(leaseRenewal);
                            break;
                        case (23):
                            var homeSale = _context.HomeSales.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            homeSale.StatusId = 33;
                            _context.HomeSales.Update(homeSale);
                            break;
                        case (24):
                            var homePurchase = _context.HomePurchases.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            homePurchase.StatusId = 33;
                            _context.HomePurchases.Update(homePurchase);
                            break;
                        case (25):
                            var propertyManagement = _context.PropertyManagements.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            propertyManagement.StatusId = 33;
                            _context.PropertyManagements.Update(propertyManagement);
                            break;
                        case (26):
                            var other = _context.Others.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            other.StatusId = 33;
                            _context.Others.Update(other);
                            break;
                        case (27):
                            var tenancyManagement = _context.TenancyManagements.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            tenancyManagement.StatusId = 33;
                            _context.TenancyManagements.Update(tenancyManagement);
                            break;
                        case (28):
                            var otherImmigration = _context.Others.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            otherImmigration.StatusId = 33;
                            _context.Others.Update(otherImmigration);
                            break;
                    }
                }

                serviceRecord.StatusId = serviceRecord.StatusId == 4
                    ? serviceRecord.StatusId
                    : (!relocation.Any() && immigration.Any(x =>
                        x.status != 3 || x.status != 4 || x.status != 5 || x.status != 33))
                        ? 4
                        : (relocation.Any(x => x.status == 3 || x.status == 4 || x.status == 5 || x.status == 33)
                           && immigration.Any(x => x.status != 3 || x.status != 4 || x.status != 5 || x.status == 33))
                            ? serviceRecord.StatusId
                            : 4;
                tuple = new Tuple<bool, string>(true, "Services On Hold.");
            }
            else if (relocation.Any(x => x.status != 3 || x.status != 4 || x.status != 5 || x.status != 33) && serviceLine == 2)
            {
                foreach (var i in relocation.Where(x => (x.status != 3 || x.status != 4 || x.status != 5 || x.status != 33)).ToList())
                {
                    switch (i.category)
                    {
                        case (1):
                            var entryVisa = _context.EntryVisas.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            entryVisa.StatusId = 33;
                            _context.EntryVisas.Update(entryVisa);
                            break;
                        case (2):
                            var workPermit = _context.WorkPermits.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            workPermit.StatusId = 33;
                            _context.WorkPermits.Update(workPermit);
                            break;
                        case (4):
                            var residencyPermit = _context.ResidencyPermits.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            residencyPermit.StatusId = 33;
                            _context.ResidencyPermits.Update(residencyPermit);
                            break;
                        case (3):
                            var visaDeregistration = _context.VisaDeregistrations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            visaDeregistration.StatusId = 33;
                            _context.VisaDeregistrations.Update(visaDeregistration);
                            break;
                        case (8):
                            var renewal = _context.Renewals.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            renewal.StatusId = 33;
                            _context.Renewals.Update(renewal);
                            break;
                        case (7):
                            var corporateAssistance = _context.CorporateAssistances.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            corporateAssistance.StatusId = 33;
                            _context.CorporateAssistances.Update(corporateAssistance);
                            break;
                        case (9):
                            var notification = _context.Notifications.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            notification.StatusId = 33;
                            _context.Notifications.Update(notification);
                            break;
                        case (10):
                            var legalReview = _context.LegalReviews.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            legalReview.StatusId = 33;
                            _context.LegalReviews.Update(legalReview);
                            break;
                        case (5):
                            var documentManagement = _context.DocumentManagements.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            documentManagement.StatusId = 33;
                            _context.DocumentManagements.Update(documentManagement);
                            break;
                        case (6):
                            var localDocumentation = _context.LocalDocumentations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            localDocumentation.StatusId = 33;
                            _context.LocalDocumentations.Update(localDocumentation);
                            break;
                        case (12):
                            var predecisionOrientation = _context.PredecisionOrientations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            predecisionOrientation.StatusId = 33;
                            _context.PredecisionOrientations.Update(predecisionOrientation);
                            break;
                        case (13):
                            var areaOrientation = _context.AreaOrientations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            areaOrientation.StatusId = 33;
                            _context.AreaOrientations.Update(areaOrientation);
                            break;
                        case (14):
                            var settlingIn = _context.SettlingIns.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            settlingIn.StatusId = 33;
                            _context.SettlingIns.Update(settlingIn);
                            break;
                        case (15):
                            var schoolingSearch = _context.SchoolingSearches.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            schoolingSearch.StatusId = 33;
                            _context.SchoolingSearches.Update(schoolingSearch);
                            break;
                        case (16):
                            var departure = _context.Departures.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            departure.StatusId = 33;
                            _context.Departures.Update(departure);
                            break;
                        case (17):
                            var temporaryHousingCoordinaton = _context.TemporaryHousingCoordinatons.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            temporaryHousingCoordinaton.StatusId = 33;
                            _context.TemporaryHousingCoordinatons.Update(temporaryHousingCoordinaton);
                            break;
                        case (18):
                            var rentalFurniture = _context.RentalFurnitureCoordinations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            rentalFurniture.StatusId = 33;
                            _context.RentalFurnitureCoordinations.Update(rentalFurniture);
                            break;
                        case (19):
                            var transportation = _context.Transportations.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            transportation.StatusId = 33;
                            _context.Transportations.Update(transportation);
                            break;
                        case (20):
                            var airportTransportation = _context.AirportTransportationServices.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            airportTransportation.StatusId = 33;
                            _context.AirportTransportationServices.Update(airportTransportation);
                            break;
                        case (21):
                            var homeFinding = _context.HomeFindings.SingleOrDefault(x => x.WorkOrderServicesId == i.workOrderService);
                            homeFinding.StatusId = 33;
                            _context.HomeFindings.Update(homeFinding);
                            break;
                        case (22):
                            var leaseRenewal = _context.LeaseRenewals.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            leaseRenewal.StatusId = 33;
                            _context.LeaseRenewals.Update(leaseRenewal);
                            break;
                        case (23):
                            var homeSale = _context.HomeSales.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            homeSale.StatusId = 33;
                            _context.HomeSales.Update(homeSale);
                            break;
                        case (24):
                            var homePurchase = _context.HomePurchases.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            homePurchase.StatusId = 33;
                            _context.HomePurchases.Update(homePurchase);
                            break;
                        case (25):
                            var propertyManagement = _context.PropertyManagements.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            propertyManagement.StatusId = 33;
                            _context.PropertyManagements.Update(propertyManagement);
                            break;
                        case (26):
                            var other = _context.Others.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            other.StatusId = 33;
                            _context.Others.Update(other);
                            break;
                        case (27):
                            var tenancyManagement = _context.TenancyManagements.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            tenancyManagement.StatusId = 33;
                            _context.TenancyManagements.Update(tenancyManagement);
                            break;
                        case (28):
                            var otherImmigration = _context.Others.SingleOrDefault(x => x.WorkOrderServices == i.workOrderService);
                            otherImmigration.StatusId = 33;
                            _context.Others.Update(otherImmigration);
                            break;
                    }
                }

                serviceRecord.StatusId = serviceRecord.StatusId == 4
                    ? serviceRecord.StatusId
                    : (!immigration.Any() && relocation.Any(x =>
                        x.status != 3 || x.status != 4 || x.status != 5 || x.status != 33))
                        ? 4
                        : (immigration.Any(x => x.status == 3 || x.status == 4 || x.status == 5 || x.status == 33)
                           && relocation.Any(x => x.status != 3 || x.status != 4 || x.status != 5 || x.status == 33))
                            ? serviceRecord.StatusId
                            : 4;

                tuple = new Tuple<bool, string>(true, "Services On Hold.");
            }
            else
            {
                tuple = new Tuple<bool, string>(false, "All services already On Hold.");
            }

            _context.ServiceRecords.Update(serviceRecord);
            _context.SaveChanges();
            return tuple;
        }

        public Tuple<int, string> SetStatusServiceRecord(int sr, int serviceLine)
        {
            Tuple<int, string> tuple = new Tuple<int, string>(0, "");
            var standaloneImmigration = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == 1)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkOrderServiceId,
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkOrderService.Others.FirstOrDefault().StatusId
                        : s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var bundledImmigration = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == 1)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkServicesId,
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkServices.Others.FirstOrDefault().StatusId
                        : s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var immigration = standaloneImmigration.Union(bundledImmigration).ToList();
            var standaloneRelocation = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == sr && x.WorkOrder.ServiceLineId == 2)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkOrderServiceId,
                    status = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkOrderService.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkOrderService.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkOrderService.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkOrderService.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkOrderService.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkOrderService.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkOrderService.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkOrderService.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkOrderService.Others.FirstOrDefault().StatusId
                        : s.WorkOrderService.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var bundledRelocation = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == sr && x.BundledServiceOrder.WorkOrder.ServiceLineId == 2)
                .Select(s => new
                {
                    category = s.CategoryId,
                    workOrderService = s.WorkServicesId,
                    status = s.CategoryId == 1 ? s.WorkServices.EntryVisas.FirstOrDefault().StatusId
                        : s.CategoryId == 2 ? s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations.FirstOrDefault().StatusId
                        : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits.FirstOrDefault().StatusId
                        : s.CategoryId == 5 ? s.WorkServices.DocumentManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations.FirstOrDefault().StatusId
                        : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances.FirstOrDefault().StatusId
                        : s.CategoryId == 8 ? s.WorkServices.Renewals.FirstOrDefault().StatusId
                        : s.CategoryId == 9 ? s.WorkServices.Notifications.FirstOrDefault().StatusId
                        : s.CategoryId == 10 ? s.WorkServices.LegalReviews.FirstOrDefault().StatusId
                        : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 13 ? s.WorkServices.AreaOrientations.FirstOrDefault().StatusId
                        : s.CategoryId == 14 ? s.WorkServices.SettlingIns.FirstOrDefault().StatusId
                        : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches.FirstOrDefault().StatusId
                        : s.CategoryId == 16 ? s.WorkServices.Departures.FirstOrDefault().StatusId
                        : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons.FirstOrDefault().StatusId
                        : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations.FirstOrDefault().StatusId
                        : s.CategoryId == 19 ? s.WorkServices.Transportations.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices.FirstOrDefault().StatusId
                        : s.CategoryId == 21 ? s.WorkServices.HomeFindings.FirstOrDefault().StatusId
                        : s.CategoryId == 22 ? s.WorkServices.LeaseRenewals.FirstOrDefault().StatusId
                        : s.CategoryId == 23 ? s.WorkServices.HomeSales.FirstOrDefault().StatusId
                        : s.CategoryId == 24 ? s.WorkServices.HomePurchases.FirstOrDefault().StatusId
                        : s.CategoryId == 25 ? s.WorkServices.PropertyManagements.FirstOrDefault().StatusId
                        : s.CategoryId == 26 ? s.WorkServices.Others.FirstOrDefault().StatusId
                        : s.WorkServices.WorkPermits.FirstOrDefault().StatusId
                }).ToList();
            var relocation = standaloneRelocation.Union(bundledRelocation).ToList();

            //var _orden = _context.CatStatusWorkOrders.OrderBy(x => x.Ordered).ToList();
            List<int> _list = new List<int>();

            for (var i = 0; i < relocation.Count(); i++)
            {
                if (relocation[i].status != null)
                {
                    _list.Add(_context.CatStatusWorkOrders.FirstOrDefault(x => x.Id == relocation[i].status).Ordered.Value);
                }
            }
    
            int __List = _list.OrderByDescending(x => x).Take(1).FirstOrDefault();

            int _idCatStatus = _context.CatStatusWorkOrders.Any(x => x.Ordered == __List) ? _context.CatStatusWorkOrders.OrderBy(x => x.Ordered).FirstOrDefault(x => x.Ordered == __List).Id
                : 0;

            tuple = new Tuple<int, string>(
                _context.CatStatuses.Any(x => x.CatStatusWorkorderId == _idCatStatus) ?
                _context.CatStatuses.FirstOrDefault(x => x.CatStatusWorkorderId == _idCatStatus).Id
                : 11, _context.CatStatuses.Any(x => x.CatStatusWorkorderId == _idCatStatus) ?
                _context.CatStatuses.FirstOrDefault(x => x.CatStatusWorkorderId == _idCatStatus).Status
                : "Pending Services");

            return tuple;
        }

        public bool DeleteCoordinator(int coordinator, int serviceLine)
        {
            bool isSuccess = false;
            if (serviceLine == 1)
            {
                try
                {
                    ImmigrationCoodinator immigrationCoordinator =
                        _context.ImmigrationCoodinators.FirstOrDefault(x => x.Id == coordinator);
                    immigrationCoordinator.StatusId = 4;
                    _context.ImmigrationCoodinators.Update(immigrationCoordinator);
                    _context.SaveChanges();


                    var totalCoordinatot = _context.ImmigrationCoodinators.Where(x => x.ServiceRecordId == immigrationCoordinator.ServiceRecordId && x.StatusId != 4).ToList();

                    if (totalCoordinatot.Count() == 1)
                    {
                        //ImmigrationCoodinator imm = new ImmigrationCoodinator();

                        var _imm = _context.ImmigrationCoodinators.FirstOrDefault(c => c.Id == totalCoordinatot.FirstOrDefault().Id);
                        _imm.CoordinatorTypeId = 1;
                        _context.ImmigrationCoodinators.Update(_imm);
                        _context.SaveChanges();
                    }

                    isSuccess = true;
                }
                catch (DbException ex)
                {
                    isSuccess = false;
                }
                catch (Exception e)
                {
                    isSuccess = false;
                }
            }
            else if (serviceLine == 2)
            {
                try
                {
                    RelocationCoordinator relocationCoordinator =
                        _context.RelocationCoordinators.FirstOrDefault(x => x.Id == coordinator);
                    relocationCoordinator.StatusId = 4;
                    _context.RelocationCoordinators.Update(relocationCoordinator);
                    _context.SaveChanges();

                    var totalCoordinatot = _context.RelocationCoordinators.Where(x => x.ServiceRecordId == relocationCoordinator.ServiceRecordId && x.StatusId != 4).ToList();

                    if (totalCoordinatot.Count() == 1)
                    {
                        //ImmigrationCoodinator imm = new ImmigrationCoodinator();

                        var _imm = _context.RelocationCoordinators.FirstOrDefault(c => c.Id == totalCoordinatot.FirstOrDefault().Id);
                        _imm.CoordinatorTypeId = 1;
                        _context.RelocationCoordinators.Update(_imm);
                        _context.SaveChanges();
                    }

                    isSuccess = true;
                }
                catch (DbException ex)
                {
                    isSuccess = false;
                }
                catch (Exception e)
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public Tuple<bool, int> DeleteSupplier(int supplier, int serviceLine)
        {
            bool isSuccess = false;
            Tuple<bool, int> tuple = new Tuple<bool, int>(false, 0);
            if (serviceLine == 1)
            {
                try
                {
                    ImmigrationSupplierPartner immigrationSupplierPartner =
                        _context.ImmigrationSupplierPartners.FirstOrDefault(x => x.Id == supplier);
                    immigrationSupplierPartner.StatusId = 4;
                    _context.ImmigrationSupplierPartners.Update(immigrationSupplierPartner);
                    _context.SaveChanges();
                    tuple = new Tuple<bool, int>(true, immigrationSupplierPartner.ServiceRecordId);
                }
                catch (DbException ex)
                {
                    tuple = new Tuple<bool, int>(false, 0);
                }
                catch (Exception e)
                {
                    tuple = new Tuple<bool, int>(false, 0);
                }
            }
            else if (serviceLine == 2)
            {
                try
                {
                    RelocationSupplierPartner relocationSupplierPartner =
                        _context.RelocationSupplierPartners.FirstOrDefault(x => x.Id == supplier);
                    relocationSupplierPartner.StatusId = 4;
                    _context.RelocationSupplierPartners.Update(relocationSupplierPartner);
                    _context.SaveChanges();
                    tuple = new Tuple<bool, int>(true, relocationSupplierPartner.ServiceRecordId.Value);
                }
                catch (DbException ex)
                {
                    tuple = new Tuple<bool, int>(false, 0);
                }
                catch (Exception e)
                {
                    tuple = new Tuple<bool, int>(false, 0);
                }
            }

            return tuple;
        }

        public bool DeleteSupplierChat(int serviceRecordId, int supplierId)
        {
            bool isSuccess = false;
            try
            {
                var chat = _context.Conversations.FirstOrDefault(x => x.ServiceRecordId == serviceRecordId).Id;
                int supplier = _context.RelocationSupplierPartners.Include(x => x.Supplier)
                    .ThenInclude(x => x.User).FirstOrDefault(x => x.Id == supplierId).Supplier.UserId.Value;
                UserGroup _userGroup = _context.UserGroups.FirstOrDefault(x => x.Conversation == chat && x.UserReciver == supplier);

                _context.UserGroups.Remove(_userGroup);
                _context.SaveChanges();
                isSuccess = true;
            }
            catch (DbException ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public int GetRole(int user)
        {
            return _context.Users.FirstOrDefault(x => x.Id == user).RoleId;
        }
        public int change_detail_status_byWos_id(int wos_id, int type_sr, int status_detail_id)
        {
            switch (type_sr)
            {
                case 17://Pre-Decision Orientation
                    var d = _context.PredecisionOrientations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d.StatusId = status_detail_id;
                    _context.PredecisionOrientations.Update(d);
                    _context.SaveChanges();
                    break;

                case 18://Area Orientation
                    var d1 = _context.AreaOrientations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d1.StatusId = status_detail_id;
                    _context.AreaOrientations.Update(d1);
                    _context.SaveChanges();
                    break;

                case 19://Settling In
                    var d2 = _context.SettlingIns.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d2.StatusId = status_detail_id;
                    _context.SettlingIns.Update(d2);
                    _context.SaveChanges();

                    break;

                case 20://Schooling Search
                    var d3 = _context.SchoolingSearches.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d3.StatusId = status_detail_id;
                    _context.SchoolingSearches.Update(d3);
                    _context.SaveChanges();
                    break;

                case 21://Departure
                    var d4 = _context.Departures.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d4.StatusId = status_detail_id;
                    _context.Departures.Update(d4);
                    _context.SaveChanges();
                    break;

                case 22://Temporary Housing Coordinaton
                    var d5 = _context.TemporaryHousingCoordinatons.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d5.StatusId = status_detail_id;
                    _context.TemporaryHousingCoordinatons.Update(d5);
                    _context.SaveChanges();
                    break;

                case 23://Rental Furniture Coordination
                    var d6 = _context.RentalFurnitureCoordinations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d6.StatusId = status_detail_id;
                    _context.RentalFurnitureCoordinations.Update(d6);
                    _context.SaveChanges();
                    break;

                case 24://Transportation
                    var d7 = _context.Transportations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d7.StatusId = status_detail_id;
                    _context.Transportations.Update(d7);
                    _context.SaveChanges();
                    break;

                case 25://Airport Transportation Services
                    var d8 = _context.AirportTransportationServices.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d8.StatusId = status_detail_id;
                    _context.AirportTransportationServices.Update(d8);
                    _context.SaveChanges();
                    break;

                case 26://Home Finding
                    var d9 = _context.HomeFindings.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d9.StatusId = status_detail_id;
                    _context.HomeFindings.Update(d9);
                    _context.SaveChanges();
                    break;

                case 27://Lease Renewal
                    var d11 = _context.LeaseRenewals.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d11.StatusId = status_detail_id;
                    _context.LeaseRenewals.Update(d11);
                    _context.SaveChanges();
                    break;

                case 28://Home Sale
                    var d22 = _context.HomeSales.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d22.StatusId = status_detail_id;
                    _context.HomeSales.Update(d22);
                    _context.SaveChanges();
                    break;

                case 29://Home Purchase
                    var d33 = _context.HomePurchases.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d33.StatusId = status_detail_id;
                    _context.HomePurchases.Update(d33);
                    _context.SaveChanges();
                    break;

                case 30://Property Management
                    var d44 = _context.PropertyManagements.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d44.StatusId = status_detail_id;
                    _context.PropertyManagements.Update(d44);
                    _context.SaveChanges();
                    break;

                case 32://Tenancy Management
                    var d55 = _context.TenancyManagements.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d55.StatusId = status_detail_id;
                    _context.TenancyManagements.Update(d55);
                    _context.SaveChanges();
                    break;

                case 31://Other
                    var d66 = _context.Others.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d66.StatusId = status_detail_id;
                    _context.Others.Update(d66);
                    _context.SaveChanges();
                    break;
            }

            return (status_detail_id);


        }
    }

}