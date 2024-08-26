using System;
using System.Collections.Generic;
using System.Linq;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.AssignedServiceImmigration;
using api.premier.Models.AssignedServiceSuplier;
using api.premier.Models.ImmigrationCoodinator;
using api.premier.Models.ImmigrationSupplierPartner;
using api.premier.Models.NotificationSystem;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.AssignedService;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.Immigration;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.Relocation;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using biz.premier.Repository.Utility;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImmigrationController : ControllerBase
    {
        private readonly IIminigrationCoordinatorRepository _iminigrationCoordinatorRepository;
        private readonly IImmigrationSupplierPartenerRepository _immigrationSupplierPartenerRepository;
        private readonly IRelocationSupplierPartenerRepository _relocationSupplierPartenerRepository;
        private readonly IAssignedServiceSupplierRepository _assignedServiceImmigrationRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationTypeRepository;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IUtiltyRepository _utiltyRepository;

        public ImmigrationController(
            IMapper mapper,
            ILoggerManager logger,
            IIminigrationCoordinatorRepository iminigrationCoordinatorRepository,
            IImmigrationSupplierPartenerRepository immigrationSupplierPartenerRepository,
            IRelocationSupplierPartenerRepository relocationSupplierPartenerRepository,
            IAssignedServiceSupplierRepository assignedServiceImmigrationRepository,
            INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository notificationTypeRepository,
            IProfileUserRepository profileUserRepository,
            IServiceRecordRepository serviceRecordRepository,
            IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _iminigrationCoordinatorRepository = iminigrationCoordinatorRepository;
            _immigrationSupplierPartenerRepository = immigrationSupplierPartenerRepository;
            _relocationSupplierPartenerRepository = relocationSupplierPartenerRepository;
            _assignedServiceImmigrationRepository = assignedServiceImmigrationRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationTypeRepository = notificationTypeRepository;
            _profileUserRepository = profileUserRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _utiltyRepository = utiltyRepository;
        }

        //Post Create a new Immigration Cordinator
        [HttpPost("CreateCoordinatorImmigration", Name = "CreateCoordinatorImmigration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationCoodinatorDto>> PostCreateCoordinator([FromBody] ImmigrationCoodinatorDto dto)
        {
            var response = new ApiResponse<ImmigrationCoodinatorDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                ImmigrationCoodinator immi = _iminigrationCoordinatorRepository.AddCoordinator(_mapper.Map<ImmigrationCoodinator>(dto));
                response.Result = _mapper.Map<ImmigrationCoodinatorDto>(immi);

                var user = _notificationSystemRepository.GetUserId(immi.CoordinatorId.Value);
                var numberSr = _notificationSystemRepository.GetServiceRecordNumber(immi.ServiceRecordId);
                var text = _notificationTypeRepository.Find(x => x.Id == 25).Type;
                var isUrgent = _notificationSystemRepository.IsUrgentSr(immi.ServiceRecordId);

                NotificationSystemDto notification = new NotificationSystemDto();
                notification.Archive = false;
                notification.View = false;
                notification.ServiceRecord = dto.ServiceRecordId;
                notification.Time = DateTime.Now.TimeOfDay;
                notification.UserFrom = dto.CreatedBy;
                notification.UserTo = user;
                //immi.Supplier.UserId;
                notification.NotificationType = 25;
                notification.Description = "Go to your dashboard to accept the SR services";
                notification.Color = "#1565c0";
                notification.CreatedDate = DateTime.Now;
                _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                _notificationSystemRepository.SendNotificationAsync(
                    user,
                    immi.CreatedBy.Value,
                    0,
                    text,
                    $"{numberSr}, {text}",
                    25
                );

                if (isUrgent)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = immi.ServiceRecordId,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = dto.CreatedBy,
                        UserTo = user,
                        NotificationType = 27,
                        Description = "The SR is urgent, please follow up immediately",
                        Color = "#f9a825",
                        CreatedDate = DateTime.Now
                    });
                    _notificationSystemRepository.SendNotificationAsync(
                        user,
                        0,
                        0,
                        _notificationTypeRepository.Find(x => x.Id == 27).Type,
                        $"{numberSr} is urgent",
                        27
                    );
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpPost("CreateSupplierPartnerImmigration", Name = "CreateSupplierPartnerImmigration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationSupplierPartnerDto>> PostCreateSupplierPartner([FromBody] ImmigrationSupplierPartnerDto dto)
        {
            var response = new ApiResponse<ImmigrationSupplierPartnerDto>();
            try
            {

                int _countryId = 0;
                dto.CreatedDate = DateTime.Now;
                _countryId = _immigrationSupplierPartenerRepository.GetCountryById(dto.SupplierTypeId.Value);
                dto.SupplierTypeId = _countryId;
                ImmigrationSupplierPartner immi = _immigrationSupplierPartenerRepository.Add(_mapper.Map<ImmigrationSupplierPartner>(dto));
                response.Result = _mapper.Map<ImmigrationSupplierPartnerDto>(immi);

               

                var numberSr = _notificationSystemRepository.GetServiceRecordNumber(immi.ServiceRecordId);
                var text = _notificationTypeRepository.Find(x => x.Id == 25).Type;
                var isUrgent = _notificationSystemRepository.IsUrgentSr(immi.ServiceRecordId);

                NotificationSystemDto notification = new NotificationSystemDto();
                notification.Archive = false;
                notification.View = false;
                notification.ServiceRecord = dto.ServiceRecordId;
                notification.Time = DateTime.Now.TimeOfDay;
                notification.UserFrom = dto.CreatedBy;
                notification.UserTo = _immigrationSupplierPartenerRepository.GetAllIncluding(x => x.Supplier)
                    .SingleOrDefault(x => x.Id == immi.Id)
                    ?.Supplier.UserId;
                //immi.Supplier.UserId;
                notification.NotificationType = 25;
                notification.Description = "New SR assigned, go to your dashboard for more information";
                notification.Color = "#1565c0";
                notification.CreatedDate = DateTime.Now;
                _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                var userId = _immigrationSupplierPartenerRepository.GetAllIncluding(x => x.Supplier)
                    .SingleOrDefault(x => x.Id == immi.Id)
                    ?.Supplier.UserId;
                _notificationSystemRepository.SendNotificationAsync(
                    userId.Value,
                    0,
                    0,
                    _notificationTypeRepository.Find(x => x.Id == 2).Type,
                    $"{numberSr}, {text}",
                    25
                    );
                if (isUrgent)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = immi.ServiceRecordId,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = dto.CreatedBy,
                        UserTo = userId.Value,
                        NotificationType = 27,
                        Description = "The SR is urgent, please follow up immediately",
                        Color = "#f9a825",
                        CreatedDate = DateTime.Now
                    });
                    _notificationSystemRepository.SendNotificationAsync(
                        userId.Value,
                        0,
                        0,
                        _notificationTypeRepository.Find(x => x.Id == 27).Type,
                        $"{numberSr} is urgent",
                        27
                    );
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpPost("AddAssignedImmigration", Name = "AddAssignedImmigration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<AssignedServiceSuplierDto>>> AddAssignedImmigration([FromBody] List<AssignedServiceSuplierDto> dto)
        {
            var response = new ApiResponse<List<AssignedServiceSuplierDto>>();
            try
            {
                List<AssignedServiceSuplierDto> assignedServiceSuplierDtos = new List<AssignedServiceSuplierDto>();

                foreach (var i in dto)
                {
                    var find = _immigrationSupplierPartenerRepository.Find(x => x.Id == i.ImmigrationSupplierPartnerId.Value);
                    //var standAlone = _stan.Find(x => x.Id == i.ImmigrationSupplierPartnerId.Value);
                    var userProfile = _profileUserRepository.Find(x => x.Id == find.SupplierId);
                    var numberSr = _notificationSystemRepository.GetServiceRecordNumber(find.ServiceRecordId);
                    var text = _notificationTypeRepository.Find(x => x.Id == 2).Type;
                    string service_number = _notificationSystemRepository.GetServiceAssigned(i.ServiceOrderServicesId.Value);

                    if (find != null)
                    {
                        
                        AssignedServiceSuplier asr = _assignedServiceImmigrationRepository.Add(_mapper.Map<AssignedServiceSuplier>(i));
                        assignedServiceSuplierDtos.Add(_mapper.Map<AssignedServiceSuplierDto>(asr));

                        var workOrder = _serviceRecordRepository.GetWorkOrderByServiceRecord(find.ServiceRecordId);
                        //var workOrder = SRData.WorkOrders
                        //           .Where(x => x.ServiceRecordId == SRData.Id).ToList();

                        for (int a = 0; a < workOrder.Count(); a++)
                        {
                            var standAlone = workOrder[a].StandaloneServiceWorkOrders
                                .Where(x => x.WorkOrderId == workOrder[a].Id).ToList();

                            var bundle = workOrder[a].BundledServicesWorkOrders
                                .Where(x => x.WorkOrderId == workOrder[a].Id).ToList();

                            if (standAlone != null)
                            {
                                for (int x = 0; x < standAlone.Count(); x++)
                                {
                                    if (_serviceRecordRepository.IsAssigned(standAlone[x].WorkOrderServiceId.Value))
                                    {
                                        standAlone[x].StatusId = 38;
                                        _utiltyRepository.change_detail_status_byWos_id(standAlone[x].WorkOrderServiceId.Value, standAlone[x].ServiceId.Value, 38, find.ServiceRecordId);
                                    }
                                }
                            }

                            if (bundle != null)
                            {

                                for (int y = 0; y < bundle.Count(); y++)
                                {
                                    for (int z = 0; z < bundle[y].BundledServices.Count(); z++)
                                    {
                                        if (_serviceRecordRepository.IsAssigned(bundle[y].BundledServices.FirstOrDefault().BundledServiceOrderId.Value))
                                        {
                                            bundle[y].BundledServices.FirstOrDefault().StatusId = 38;
                                            _utiltyRepository.change_detail_status_byWos_id(bundle[y].BundledServices.FirstOrDefault().WorkServicesId.Value, standAlone[y].ServiceId.Value, 38, find.ServiceRecordId);
                                        }
                                    }
                                }
                            }
                        }

                        NotificationSystemDto notification = new NotificationSystemDto();
                        notification.Archive = false;
                        notification.View = false;
                        notification.ServiceRecord = find.ServiceRecordId ;
                        notification.Time = DateTime.Now.TimeOfDay;
                        notification.UserFrom = find.CreatedBy;
                        notification.UserTo = userProfile.UserId;
                        //immi.Supplier.UserId;
                        notification.NotificationType = 2;
                        notification.Description = "Assigned " + service_number + " service go to your dashboard for more information";
                        notification.Color = "#2e7d32";
                        notification.CreatedDate = DateTime.Now;
                        _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                        _notificationSystemRepository.SendNotificationAsync(
                           userProfile.UserId.Value,
                           0,
                           0,
                           _notificationTypeRepository.Find(x => x.Id == 2).Type,
                           $"{numberSr}" + " " + "Assigned service " + i.service,
                           2
                       );
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Relocation Supplier Partner Not Found";
                    }
                }
                response.Success = true;
                response.Result = assignedServiceSuplierDtos;

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }
        [HttpDelete("DeleteAssignedImmigration", Name = "DeleteAssignedImmigration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]

        public ActionResult<ApiResponse<AssignedServiceSuplierDto>> DeleteAssignedImmigration(int id)
        {
            var response = new ApiResponse<AssignedServiceSuplierDto>();
            try
            {
                var find = _assignedServiceImmigrationRepository.Find(x => x.ServiceOrderServicesId == id);

                var _find_supplier = _immigrationSupplierPartenerRepository.Find(x => x.Id == find.ImmigrationSupplierPartnerId) == null 
                    ? _relocationSupplierPartenerRepository.Find(x => x.Id == find.RelocationSupplierPartnerId).SupplierId
                    : _immigrationSupplierPartenerRepository.Find(x => x.Id == find.ImmigrationSupplierPartnerId).SupplierId;
                var _find_sr = _immigrationSupplierPartenerRepository.Find(x => x.Id == find.ImmigrationSupplierPartnerId) == null
                    ? _relocationSupplierPartenerRepository.Find(x => x.Id == find.RelocationSupplierPartnerId).ServiceRecordId
                    : _immigrationSupplierPartenerRepository.Find(x => x.Id == find.ImmigrationSupplierPartnerId).ServiceRecordId;
                var _find_create = _immigrationSupplierPartenerRepository.Find(x => x.Id == find.ImmigrationSupplierPartnerId) == null
                    ? _relocationSupplierPartenerRepository.Find(x => x.Id == find.RelocationSupplierPartnerId).CreatedBy
                    : _immigrationSupplierPartenerRepository.Find(x => x.Id == find.ImmigrationSupplierPartnerId).CreatedBy;

                var userProfile = _profileUserRepository.Find(x => x.Id == _find_supplier);
                var numberSr = _notificationSystemRepository.GetServiceRecordNumber(_find_sr.Value);
                var text = _notificationTypeRepository.Find(x => x.Id == 29).Type;
                if (find != null)
                {
                    _assignedServiceImmigrationRepository.Delete(find);

                    var workOrder = _serviceRecordRepository.GetWorkOrderByServiceRecord(_find_sr.Value);
                    //var workOrder = SRData.WorkOrders
                    //           .Where(x => x.ServiceRecordId == SRData.Id).ToList();

                    for (int a = 0; a < workOrder.Count(); a++)
                    {
                        var standAlone = workOrder[a].StandaloneServiceWorkOrders
                            .Where(x => x.WorkOrderId == workOrder[a].Id).ToList();

                        var bundle = workOrder[a].BundledServicesWorkOrders
                            .Where(x => x.WorkOrderId == workOrder[a].Id).ToList();

                        if (standAlone != null)
                        {
                            for (int x = 0; x < standAlone.Count(); x++)
                            {
                                if (!_serviceRecordRepository.IsAssigned(standAlone[x].WorkOrderServiceId.Value))
                                {
                                    standAlone[x].StatusId = 1;
                                    _utiltyRepository.change_detail_status_byWos_id(standAlone[x].WorkOrderServiceId.Value, standAlone[x].ServiceId.Value, 1, _find_sr.Value);
                                }
                            }
                        }

                        if (bundle != null)
                        {

                            for (int y = 0; y < bundle.Count(); y++)
                            {
                                for (int z = 0; z < bundle[y].BundledServices.Count(); z++)
                                {
                                    if (!_serviceRecordRepository.IsAssigned(bundle[y].BundledServices.FirstOrDefault().BundledServiceOrderId.Value))
                                    {
                                        bundle[y].BundledServices.FirstOrDefault().StatusId = 1;
                                        _utiltyRepository.change_detail_status_byWos_id(bundle[y].BundledServices.FirstOrDefault().WorkServicesId.Value, standAlone[y].ServiceId.Value, 1, _find_sr.Value);
                                    }
                                }
                            }
                        }
                    }

                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = _find_sr;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = _find_create;
                    notification.UserTo = userProfile.UserId;
                    //immi.Supplier.UserId;
                    notification.NotificationType = 29;
                    notification.Description = $"{numberSr}, {text}";
                    notification.Color = "#CD5C5C";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                       userProfile.UserId.Value,
                       0,
                       0,
                       _notificationTypeRepository.Find(x => x.Id == 29).Type,
                       $"{numberSr}",
                       29
                   );
                    response.Success = true;
                    //response.Result = "Success";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Record Not Found";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpGet("GetSupplierPartnerImmigration", Name = "GetSupplierPartnerImmigration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartner([FromQuery] int serviceRecord)
        {
            try
            {
                var Result = _immigrationSupplierPartenerRepository.GetSuppplierPartnerList(serviceRecord);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetAssignedServiceImmigration", Name = "GetAssignedServiceImmigration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAssignedService([FromQuery] int serviceRecord)
        {
            try
            {
                var Result = _immigrationSupplierPartenerRepository.GetAssignedServices(serviceRecord);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }


    }
}
