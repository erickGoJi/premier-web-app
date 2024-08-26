using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.AssignedServiceRelocation;
using api.premier.Models.AssignedServiceSuplier;
using api.premier.Models.Conversation;
using api.premier.Models.NotificationSystem;
using api.premier.Models.RelocationCoordinator;
using api.premier.Models.RelocationSupplierPartner;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.AssignedService;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.Conversation;
using biz.premier.Repository.Immigration;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.Relocation;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelocationController : ControllerBase
    {
        private readonly IRelocationCoordinatorRepository _relocationCoordinatorRepository;
        private readonly IRelocationSupplierPartenerRepository _relocationSupplierPartenerRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IAssignedServiceSupplierRepository _assignedServiceRelocationRepository;
        private readonly IUtiltyRepository _utiltyRepository;

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationTypeRepository;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IImmigrationSupplierPartenerRepository _immigrationSupplierPartenerRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IConversationRepository _conversationRepository;

        public RelocationController(IRelocationCoordinatorRepository relocationCoordinatorRepository, IRelocationSupplierPartenerRepository relocationSupplierPartenerRepository,
            IMapper mapper, ILoggerManager logger, IServiceRecordRepository serviceRecordRepository, IAssignedServiceSupplierRepository assignedServiceRelocationRepository,
            INotificationSystemRepository notificationSystemRepository, ICatNotificationSystemTypeRepository notificationTypeRepository,
            IProfileUserRepository profileUserRepository, IUtiltyRepository utiltyRepository,
            IImmigrationSupplierPartenerRepository immigrationSupplierPartenerRepository,
            IUserGroupRepository userGroupRepository,
            IConversationRepository conversationRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _relocationCoordinatorRepository = relocationCoordinatorRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _relocationSupplierPartenerRepository = relocationSupplierPartenerRepository;
            _assignedServiceRelocationRepository = assignedServiceRelocationRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationTypeRepository = notificationTypeRepository;
            _profileUserRepository = profileUserRepository;
            _utiltyRepository = utiltyRepository;
            _immigrationSupplierPartenerRepository = immigrationSupplierPartenerRepository;
            _userGroupRepository = userGroupRepository;
            _conversationRepository = conversationRepository;
        }

        [HttpPost("CreateCoordinator", Name = "CreateCoordinator")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RelocationCoordinatorDto>> CreateRelocationCoordinator([FromBody] RelocationCoordinatorDto dto)
        {
            var response = new ApiResponse<RelocationCoordinatorDto>();
            try
            {
                var find = _serviceRecordRepository.Find(x => x.Id == dto.ServiceRecordId);
                if(find != null)
                {
                    dto.CreatedDate = DateTime.Now;
                    RelocationCoordinator coordinator = _relocationCoordinatorRepository.AddCoordinator(_mapper.Map<RelocationCoordinator>(dto));
                    response.Success = true;
                    response.Result = _mapper.Map<RelocationCoordinatorDto>(coordinator);
                    
                    var user = _notificationSystemRepository.GetUserId(coordinator.CoordinatorId.Value);
                    var numberSr = _notificationSystemRepository.GetServiceRecordNumber(coordinator.ServiceRecordId);
                    var text = _notificationTypeRepository.Find(x => x.Id == 25).Type;
                    var isUrgent = _notificationSystemRepository.IsUrgentSr(coordinator.ServiceRecordId);

                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = dto.ServiceRecordId;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = dto.CreatedBy;
                    notification.UserTo = user;
                    //immi.Supplier.UserId;
                    notification.NotificationType = 25;
                    notification.Description = $"{numberSr}, {text}";
                    notification.Color = "#2e7d32";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                        user, 
                        coordinator.CreatedBy.Value, 
                        0, 
                        text,
                        $"{numberSr}, {text}",
                        2
                    );
                    if (isUrgent)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Archive = false,
                            View = false,
                            ServiceRecord = coordinator.ServiceRecordId,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = dto.CreatedBy,
                            UserTo = user,
                            NotificationType = 27,
                            Description = $"{numberSr} is urgent",
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
                else
                {
                    response.Success = false;
                    response.Message = "Service Record Not Found";
                }
            }
            catch(Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpPost("CreateSupplierPartner", Name = "CreateSupplierPartner")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RelocationSupplierPartnerDto>> CreateSupplierPartner([FromBody] RelocationSupplierPartnerDto dto)
        {
            var response = new ApiResponse<RelocationSupplierPartnerDto>();
            try
            {
                var find = _serviceRecordRepository.Find(x => x.Id == dto.ServiceRecordId);

                if (find != null)
                {
                    int _countryId = 0;
                    dto.CreatedDate = DateTime.Now;
                    _countryId = _relocationSupplierPartenerRepository.GetCountryById(dto.SupplierTypeId.Value);
                    dto.SupplierTypeId = _countryId;
                    RelocationSupplierPartner coordinator = _relocationSupplierPartenerRepository.Add(_mapper.Map<RelocationSupplierPartner>(dto));
                    response.Success = true;
                    response.Result = _mapper.Map<RelocationSupplierPartnerDto>(coordinator);

                    var numberSr = _notificationSystemRepository.GetServiceRecordNumber(coordinator.ServiceRecordId.Value);
                    var text = _notificationTypeRepository.Find(x => x.Id == 25).Type;
                    var isUrgent = _notificationSystemRepository.IsUrgentSr(coordinator.ServiceRecordId.Value);
                    var userTo = _relocationSupplierPartenerRepository.GetAllIncluding(x => x.Supplier)
                        .SingleOrDefault(x => x.Id == coordinator.Id)?.Supplier.UserId;

                    var _convertation = _conversationRepository.Find(x => x.ServiceRecordId == dto.ServiceRecordId);

                    if(_convertation != null)
                    {
                        UserGroup userReciver = new UserGroup();
                        userReciver.UserReciver = userTo.Value;
                        userReciver.Conversation = _convertation.Id;
                        _userGroupRepository.AddUser(userReciver);
                    }

                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = dto.ServiceRecordId;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = dto.CreatedBy;
                    notification.UserTo = userTo.Value;
                    //immi.Supplier.UserId;
                    notification.NotificationType = 25;
                    notification.Description = "New SR assigned, go to your dashboard for more information";
                    notification.Color = "#1565c0";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                           userTo.Value,
                           0,
                           0,
                           _notificationTypeRepository.Find(x => x.Id == 25).Type,
                            "You have been assigned to a "+ numberSr,
                           25
                       );

                    if (isUrgent)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Archive = false,
                            View = false,
                            ServiceRecord = coordinator.ServiceRecordId,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = dto.CreatedBy,
                            UserTo = userTo.Value,
                            NotificationType = 27,
                            Description = $"{numberSr} is urgent",
                            Color = "#f9a825",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.SendNotificationAsync(
                            userTo.Value,
                            0,
                            0,
                            _notificationTypeRepository.Find(x => x.Id == 27).Type,
                            $"{numberSr} is urgent",
                            27
                        );
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Service Record Not Found";
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

        [HttpPost("AddAssignedRelocation", Name = "AddAssignedRelocation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<AssignedServiceSuplierDto>>> AddAssignedRelocation([FromBody] List<AssignedServiceSuplierDto> dto)
        {
            var response = new ApiResponse<List<AssignedServiceSuplierDto>>();
            try
            {
                List<AssignedServiceSuplierDto> assignedServiceSupplierDtos = new List<AssignedServiceSuplierDto>();
                foreach(var i in dto)
                {
                    var find = _relocationSupplierPartenerRepository.Find(x => x.Id == i.RelocationSupplierPartnerId.Value);
                    var userProfile = _profileUserRepository.Find(x => x.Id == find.SupplierId);
                    var numberSr = _notificationSystemRepository.GetServiceRecordNumber(find.ServiceRecordId.Value);
                    var text = _notificationTypeRepository.Find(x => x.Id == 2).Type;
                    string service_number = _notificationSystemRepository.GetServiceAssigned(i.ServiceOrderServicesId.Value);
                    //var SRData = _serviceRecordRepository.Find(x => x.Id == find.ServiceRecordId.Value);

                    if (find != null)
                    {
                        AssignedServiceSuplier asr = _assignedServiceRelocationRepository.Add(_mapper.Map<AssignedServiceSuplier>(i));
                        assignedServiceSupplierDtos.Add(_mapper.Map<AssignedServiceSuplierDto>(asr));

                        //if (SRData != null)
                        //{
                        //    if(SRData.StatusId == 1)
                        //    {
                        //        SRData.StatusId = 18;
                        //        _serviceRecordRepository.Update(SRData, find.ServiceRecordId);
                        //        _serviceRecordRepository.Save();
                        //    }
                           
                        //}

                        var workOrder = _serviceRecordRepository.GetWorkOrderByServiceRecord(find.ServiceRecordId.Value);
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
                                        _utiltyRepository.change_detail_status_byWos_id(standAlone[x].WorkOrderServiceId.Value, standAlone[x].ServiceId.Value, 38, find.ServiceRecordId.Value);
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
                                            _utiltyRepository.change_detail_status_byWos_id(bundle[y].BundledServices.FirstOrDefault().WorkServicesId.Value, standAlone[y].ServiceId.Value, 38, find.ServiceRecordId.Value);
                                        }
                                    }
                                }
                            }
                        }

                        NotificationSystemDto notification = new NotificationSystemDto();
                        notification.Archive = false;
                        notification.View = false;
                        notification.ServiceRecord = find.ServiceRecordId;
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
                           $"{numberSr}",
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
                response.Result = assignedServiceSupplierDtos;
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


        [HttpPost("AddMultiAssignedRelocation", Name = "AddMultiAssignedRelocation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddMultiAssignedRelocation([FromBody] ServiceSuplierSrDto req )
        {

            try
            {

                var lista  = req.lista;

                var asignados_actuales = _relocationSupplierPartenerRepository.GetAssignedServiceSuplierById(lista[0].supplierId, req.srid);
                var nuevas_asignaciones = new List<AssignedServiceSuplier>();
                var eliminar_asignaciones = new List<AssignedServiceSuplier>();

                foreach (var A in lista)
                {

                    if (A.assignee) // no estaba asignado previemante
                    {
                        if (asignados_actuales.Where(a => a.ServiceOrderServicesId == A.id).Count() < 1) // no estaba asignado previemante
                        {
                            AssignedServiceSuplier a = new AssignedServiceSuplier();
                            a.RelocationSupplierPartnerId = A.idsuplier;
                            a.ServiceOrderServicesId = A.id;
                            nuevas_asignaciones.Add(a);
                        }
                    }
                    else
                    {
                        if (asignados_actuales.Where(a => a.ServiceOrderServicesId == A.id).Count() > 0) // no estaba asignado previemante
                        {
                            AssignedServiceSuplier a = new AssignedServiceSuplier();
                            a.RelocationSupplierPartnerId = A.idsuplier;
                            a.ServiceOrderServicesId = A.id;
                            eliminar_asignaciones.Add(a);
                        }
                    }

                }

                List<AssignedServiceSuplierDto> assignedServiceSupplierDtos = new List<AssignedServiceSuplierDto>();

                if (nuevas_asignaciones.Count > 0)
                {
                    

                    var find = _relocationSupplierPartenerRepository.Find(x => x.Id == nuevas_asignaciones[0].RelocationSupplierPartnerId.Value);
                    var userProfile = _profileUserRepository.Find(x => x.Id == find.SupplierId);
                    var numberSr = _notificationSystemRepository.GetServiceRecordNumber(find.ServiceRecordId.Value);

                    foreach (var i in nuevas_asignaciones)
                    {
                        var text = _notificationTypeRepository.Find(x => x.Id == 2).Type;
                        string service_number = _notificationSystemRepository.GetServiceAssigned(i.ServiceOrderServicesId.Value);

                        if (find != null)
                        {
                            AssignedServiceSuplier asr = _assignedServiceRelocationRepository.Add(_mapper.Map<AssignedServiceSuplier>(i));
                            assignedServiceSupplierDtos.Add(_mapper.Map<AssignedServiceSuplierDto>(asr));


                            var workOrder = _serviceRecordRepository.GetWorkOrderByServiceRecord(find.ServiceRecordId.Value);

                            for (int a = 0; a < workOrder.Count(); a++)
                            {
                                var standAlone = workOrder[a].StandaloneServiceWorkOrders
                                    .Where(x => x.WorkOrderServiceId == asr.Id).ToList();

                                var bundle = workOrder[a].BundledServicesWorkOrders
                                    .Where(x => x.BundledServices.FirstOrDefault().WorkServicesId == asr.Id).ToList();

                                if (standAlone != null)
                                {
                                    for (int x = 0; x < standAlone.Count(); x++)
                                    {
                                        if (_serviceRecordRepository.IsAssigned(standAlone[x].WorkOrderServiceId.Value))
                                        {
                                            standAlone[x].StatusId = 38;
                                            _utiltyRepository.change_detail_status_byWos_id(standAlone[x].WorkOrderServiceId.Value, standAlone[x].ServiceId.Value, 38, find.ServiceRecordId.Value);
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
                                                _utiltyRepository.change_detail_status_byWos_id(bundle[y].BundledServices.FirstOrDefault().WorkServicesId.Value, standAlone[y].ServiceId.Value, 38, find.ServiceRecordId.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {

                            return Ok(new { Success = false, Message = "Error al actualizar" });
                        }
                    }

                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = find.ServiceRecordId;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = find.CreatedBy;
                    notification.UserTo = userProfile.UserId;
                    //immi.Supplier.UserId;
                    notification.NotificationType = 2;
                    notification.Description = "Services assigned in the service record: " + numberSr;
                    notification.Color = "#2e7d32";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                       userProfile.UserId.Value,
                       0,
                       0,
                       _notificationTypeRepository.Find(x => x.Id == 2).Type,
                       $"{numberSr}",
                       2
                   );
                }

                if(eliminar_asignaciones.Count() > 0)
                {
                    
                    foreach (var i in eliminar_asignaciones)
                    {
                        var find = _assignedServiceRelocationRepository.Find(x => x.ServiceOrderServicesId == i.ServiceOrderServicesId);

                        

                        var userProfile = _profileUserRepository.Find(x => x.Id == lista[0].supplierId);
                        var numberSr = _notificationSystemRepository.GetServiceRecordNumber(req.srid);

                        if (find != null)
                        {
                            _assignedServiceRelocationRepository.Delete(find);

                            var workOrder = _serviceRecordRepository.GetWorkOrderByServiceRecord(req.srid);

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
                                            _utiltyRepository.change_detail_status_byWos_id(standAlone[x].WorkOrderServiceId.Value, standAlone[x].ServiceId.Value, 1, req.srid);
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
                                                _utiltyRepository.change_detail_status_byWos_id(bundle[y].BundledServices.FirstOrDefault().WorkServicesId.Value, standAlone[y].ServiceId.Value, 1, req.srid);
                                            }
                                        }
                                    }
                                }
                            }


                        }
                    }

                    var _find = _relocationSupplierPartenerRepository.Find(x => x.Id == eliminar_asignaciones[0].RelocationSupplierPartnerId.Value);
                    var _userProfile = _profileUserRepository.Find(x => x.Id == _find.SupplierId);
                    var _numberSr = _notificationSystemRepository.GetServiceRecordNumber(_find.ServiceRecordId.Value);
                    var text = _notificationTypeRepository.Find(x => x.Id == 29).Type;


                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = _find.ServiceRecordId;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = _find.CreatedBy;
                    notification.UserTo = _userProfile.UserId;
                    notification.NotificationType = 29;
                    notification.Description = $"{_numberSr}, {text}";
                    notification.Color = "#CD5C5C";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                       _userProfile.UserId.Value,
                       0,
                       0,
                       _notificationTypeRepository.Find(x => x.Id == 29).Type,
                       $"{_numberSr}",
                       29
                   );
                }

               
            }
            catch (Exception ex)
            {
                
                return Ok(new { Success = false, Message = ex.ToString() });
            }


            var Result = _relocationSupplierPartenerRepository.GetSuppplierPartnerList(req.srid, null);
            return Ok(new { Success = true, Result });
        }


        [HttpGet("GetSupplierPartnerRelocation", Name = "GetSupplierPartnerRelocation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartner([FromQuery] int serviceRecord, int? countryId)
        {
            try
            {
                var Result = _relocationSupplierPartenerRepository.GetSuppplierPartnerList(serviceRecord, countryId);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetAssignedServiceRelocation", Name = "GetAssignedServiceRelocation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAssignedService([FromQuery] int serviceRecord)
        {
            try
            {
                var Result = _relocationSupplierPartenerRepository.GetAssignedServices(serviceRecord);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
