using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Hubs;
using api.premier.Models;
using api.premier.Models.Appointment;
using api.premier.Models.AssigneeInformation;
using api.premier.Models.Catalogos;
using api.premier.Models.Conversation;
using api.premier.Models.DependentInformations;
using api.premier.Models.ImmigrationCoodinator;
using api.premier.Models.NotificationSystem;
using api.premier.Models.ServiceRecord;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Paged;
using biz.premier.Repository;
using biz.premier.Repository.Conversation;
using biz.premier.Repository.Follow;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using static api.premier.Controllers.ChatController;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRecordController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        private readonly IFollowRepository _followRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IHubContext<MessageHub> _hubContext;

        public ServiceRecordController(IMapper mapper, ILoggerManager logger, IServiceRecordRepository serviceRecordRepository, IUtiltyRepository utiltyRepository, IUserRepository userRepository,
             INotificationSystemRepository notificationSystemRepository, ICatNotificationSystemTypeRepository notificationSystemTypeRepository,
             IFollowRepository followRepository,
             IConversationRepository conversationRepository,
            IHubContext<MessageHub> hubContext)
        {
            _mapper = mapper;
            _logger = logger;
            _serviceRecordRepository = serviceRecordRepository;
            _utiltyRepository = utiltyRepository;
            _userRepository = userRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
            _followRepository = followRepository;
            _conversationRepository = conversationRepository;
            _hubContext = hubContext;
        }

        // GET: GetServiceRecord
        [HttpGet("GetServiceRecord/{user}", Name = "GetServiceRecord")]
        public ActionResult GetServiceRecord([FromQuery] bool? vip, [FromQuery] int? client,
            [FromQuery] int? partner, [FromQuery] int? country, [FromQuery] int? city, [FromQuery] int? supplier,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string sr, [FromQuery] int? status,
            [FromQuery] int? coordinator, [FromQuery] string? serviceLine, int user)
        {
            try
            {
                var map = _serviceRecordRepository.GetServiceRecord(vip, client, partner, country, city, supplier,
                    startDate, endDate, sr, status, coordinator, serviceLine, user);

                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // GET: GetServiceRecordListApp
        [HttpGet("GetServiceRecordListApp/{user}", Name = "GetServiceRecordListApp")]
        public ActionResult GetServiceRecordListApp(
            [FromQuery] int? partner, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? status, bool? pending_acceptance_services, bool? pending_activity_reports, int user)
        {
            try
            {
                var map = _serviceRecordRepository.GetServiceRecordListApp(partner, startDate, endDate, status, pending_acceptance_services, pending_activity_reports, user);

                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("Get_Test_Record", Name = "Get_Test_Record")]
        public ActionResult Get_TestAsync(int pageNumber, int pageSize, [FromQuery] bool? vip, [FromQuery] int? client,
            [FromQuery] int? partner, [FromQuery] int? country, [FromQuery] int? city, [FromQuery] int? supplier,
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string sr, [FromQuery] int? status,
            [FromQuery] int? coordinator, [FromQuery] string serviceLine, int user)
        {

            try
            {
                var Result = _serviceRecordRepository.GetServiceRecord(vip, client, partner, country, city, supplier, startDate, endDate, sr, status, coordinator, serviceLine, user);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Profile
        [HttpGet("GetProfile/{pageNumber}/{pageSize}", Name = "GetProfile")]
        public ActionResult GetProfile(int pageNumber, int pageSize)
        {
            try
            {
                var map = _serviceRecordRepository.GetProfile(pageNumber, pageSize);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
        // Get: Get Service Record
        [HttpGet("GetServiceRecordById", Name = "GetServiceRecordById")]
        public ActionResult<ApiResponse<ServiceRecordSelectDto>> GetServiceRecordById([FromQuery] int id, [FromQuery] int user)
        {
            var response = new ApiResponse<ServiceRecordSelectDto>();
            try
            {
                var map = _mapper.Map<ServiceRecordSelectDto>(_serviceRecordRepository.SelectCustom(id, user));
                //map.AssigneeInformations.FirstOrDefault().HomeCountryId = _serviceRecordRepository.getHomeCountry(map.AssigneeInformations.FirstOrDefault().HomeCountryId.Value);
                map.Follows = map.Follows.Where(x => x.UserId == user).ToList();
                map.AssigneeInformations.FirstOrDefault().DependentInformations =
                    map.AssigneeInformations.FirstOrDefault().DependentInformations.Where(x => x.RelationshipId != 7).ToList();
                map.ImmigrationCoodinators = map.ImmigrationCoodinators.Where(x => x.StatusId != 4).ToList();
                map.RelocationCoordinators = map.RelocationCoordinators.Where(x => x.StatusId != 4).ToList();
                map.ImmigrationSupplierPartners = map.ImmigrationSupplierPartners.Where(x => x.StatusId != 4).ToList();
                map.RelocationSupplierPartners = map.RelocationSupplierPartners.Where(x => x.StatusId != 4).ToList();
                map.Client = _serviceRecordRepository.returnClient(id);
                map.Partner = _serviceRecordRepository.returnClient(id);

                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        // Get: Get Service Record
        [HttpGet("GetServiceRecordByIdApp", Name = "GetServiceRecordByIdApp")]
        public ActionResult GetServiceRecordByIdApp([FromQuery] int serviceRecordId)
        {
            try
            {
                var map = _serviceRecordRepository.GetServiceRecordByIdApp(serviceRecordId);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetServicesApp", Name = "GetServicesApp")]
        public ActionResult GetServicesApp([FromQuery] int service_record_id, [FromQuery] int? status, [FromQuery] int? serviceLine, [FromQuery] int? user)
        {
            try
            {
                var map = _serviceRecordRepository.GetServicesApp(service_record_id, status, serviceLine, user);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetServicesAppAssignee", Name = "GetServicesAppAssignee")]
        public ActionResult GetServicesAppAssignee([FromQuery] int? status, [FromQuery] int? serviceLine, [FromQuery] int? user)
        {
            try
            {
                var map = _serviceRecordRepository.GetServicesAppAssignee(status, serviceLine, user);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetAssigneeInformationApp", Name = "GetAssigneeInformationApp")]
        public ActionResult GetAssigneeInformationApp([FromQuery] int service_record_id)
        {
            try
            {
                var map = _serviceRecordRepository.GetAssigneeInformationApp(service_record_id);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetHomeCountryById", Name = "GetHomeCountryById")]
        public string GetHomeCountryById([FromQuery] int id)
        {
            return _serviceRecordRepository.returnHomeCountry(id);
        }

        [HttpGet("GetHomeCityById", Name = "GetHomeCityById")]
        public string GetHomeCityById([FromQuery] int id)
        {
            return _serviceRecordRepository.returnHomeCity(id);
        }

        // Post Create a new service record 
        [HttpPost("Create", Name = "Create")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ServiceRecordDto>> PostCreate([FromBody] ServiceRecordInsertDto dto)
        {
            var response = new ApiResponse<ServiceRecordDto>();
            try
            {
                UserCreateDto _userdto = new UserCreateDto();

                if (_userRepository.Exists(c => c.Email == dto.AssigneeInformations.FirstOrDefault().Email))
                {
                    response.Success = false;
                    response.Message = $"Email: {dto.AssigneeInformations.FirstOrDefault().Email} Already Exists";
                    return BadRequest(response);
                }

                //var _dependent = dto.AssigneeInformations.FirstOrDefault().DependentInformations.ToList();

                //for (int i = 0; i < _dependent.Count(); i++)
                //{
                //    if (_serviceRecordRepository.ValidateEmailDependent(_dependent[i].Email))
                //    {
                //        response.Success = false;
                //        response.Message = $"Email: {_dependent[i].Email} Already Exists";
                //        return BadRequest(response);
                //    }
                //}

                _userdto.Email = dto.AssigneeInformations.FirstOrDefault().Email;
                _userdto.Password = "$" + "Premier00$";
                _userdto.Name = dto.AssigneeInformations.FirstOrDefault().AssigneeName;
                _userdto.LastName = "N/A";
                _userdto.MotherLastName = "N/A";
                _userdto.MobilePhone = "";
                _userdto.RoleId = 4;
                _userdto.UserTypeId = 5;
                _userdto.ServiceLineId = 1;

                if (dto.AssigneeInformations.FirstOrDefault().Photo != null)
                {
                    _userdto.Avatar = _utiltyRepository.UploadImageBase64(dto.AssigneeInformations.FirstOrDefault().Photo,
                        "Files/Assignee/", dto.AssigneeInformations.FirstOrDefault().PhotoExtension);
                }
                _userdto.CreatedBy = 0;
                _userdto.CreatedDate = DateTime.Now;
                _userdto.UpdateBy = 0;
                _userdto.UpdatedDate = DateTime.Now;

                User user = _userRepository.Add(_mapper.Map<User>(_userdto));


                if (dto.AssigneeInformations.Count > 0)
                {

                    if (dto.AssigneeInformations.FirstOrDefault().Photo != null)
                    {
                        dto.AssigneeInformations.FirstOrDefault().Photo = _utiltyRepository.UploadImageBase64(dto.AssigneeInformations.FirstOrDefault().Photo,
                            "Files/Assignee/", dto.AssigneeInformations.FirstOrDefault().PhotoExtension);
                    }

                    foreach (var i in dto.AssigneeInformations.FirstOrDefault().PetsNavigation)
                    {
                        if (i.Photo != null)
                        {
                            i.Photo = _serviceRecordRepository.UploadImageBase64(i.Photo);
                        }
                    }

                    foreach (var i in dto.AssigneeInformations.FirstOrDefault().DependentInformations)
                    {
                        if (i.Photo != null && i.Photo != "")
                        {
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/Dependent/", i.PhotoExtension);
                        }
                    }
                }

                dto.AssigneeInformations.FirstOrDefault().UserId = user.Id;
                dto.StatusId = 11;
                if (dto.PartnerId == 0)
                {
                    dto.PartnerId = null;
                }
                DependentInformationDto dependent = new DependentInformationDto();

                dependent.AditionalComments = "No Comments";
                dependent.Birth = dto.AssigneeInformations.FirstOrDefault().Birth.HasValue
                    ? dto.AssigneeInformations.FirstOrDefault().Birth.Value
                    : (DateTime?)null;
                dependent.RelationshipId = 7;
                dependent.Name = dto.AssigneeInformations.FirstOrDefault().AssigneeName;
                dependent.NationalityId = dto.AssigneeInformations.FirstOrDefault().NationalityId.HasValue
                    ? dto.AssigneeInformations.FirstOrDefault().NationalityId
                    : null;
                dependent.Age = dto.AssigneeInformations.FirstOrDefault().Age.HasValue
                    ? dto.AssigneeInformations.FirstOrDefault().Age
                    : null;


                dto.AssigneeInformations.FirstOrDefault().DependentInformations.Add(dependent);
                ServiceRecord record = _serviceRecordRepository.AddNewService(_mapper.Map<ServiceRecord>(dto));
                response.Result = _mapper.Map<ServiceRecordDto>(record);

                ///Create Chat grupal
                ConversationDto conversation = new ConversationDto();
                MessageDto message = new MessageDto();
                UserGroupDto userReciver = new UserGroupDto();
                DocumentMessageDto document = new DocumentMessageDto();
                List<ConversationDto> ConversationDto = new List<ConversationDto>();

                conversation = new ConversationDto();
                conversation.UserTo = dto.CreatedBy;
                conversation.UserReciver = null;
                conversation.CreatedDate = DateTime.Now;
                conversation.serviceRecordId = record.Id;
                conversation.Status = false;
                conversation.Groups = true;
                conversation.GroupName = "Chat SR - " + record.NumberServiceRecord + " " + (record.ImmigrationSupplierPartners.Any() ? "Immigration" : "Relocation");
                conversation.UserGroups = new List<UserGroupDto>();

                List<int> _userList = new List<int>();
                //_userList.Add(record.ImmigrationSupplierPartners.Any() ? record.ImmigrationSupplierPartners.FirstOrDefault().Supplier.UserId.Value
                //    : record.RelocationSupplierPartners.FirstOrDefault().Supplier.UserId.Value);
                //_userList.Add(_notificationSystemRepository.GetUserId(record.ImmigrationSupplierPartners.FirstOrDefault()
                //            .SupplierId.Value));
                _userList.Add(_notificationSystemRepository.GetUserId(record.RelocationCoordinators.FirstOrDefault()
                            .CoordinatorId.Value));
                _userList.Add(dto.AssigneeInformations.FirstOrDefault().UserId.Value);


                foreach (var i in _userList)
                {
                    userReciver = new UserGroupDto();
                    userReciver.UserReciver = i;
                    conversation.UserGroups.Add(userReciver);
                }
                conversation.Messages = new List<MessageDto>();

                // Insercion Mensaje
                message = new MessageDto();
                message.UserId = dto.CreatedBy.Value;
                message.Time = DateTime.Now;
                message.Status = false;
                message.Message1 = "Hello " + dto.AssigneeInformations.FirstOrDefault().AssigneeName + ", welcome to the premier experience, in this chat we will solve all your doubts";
                message.DocumentMessages = new List<DocumentMessageDto>();

                conversation.Messages.Add(message);

                var res = _conversationRepository.Add(_mapper.Map<Conversation>(conversation));

                ConversationDto.Add(_mapper.Map<ConversationDto>(res));
                conversation = _mapper.Map<ConversationDto>(res);

                foreach (var i in conversation.UserGroups)
                {
                    RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversation.Id, conversation.UserTo.Value, i.UserReciver);
                    _hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);
                }

                //foreach (var @group in conversation.UserGroups)
                //{
                //    if (@group.UserReciver != conversation.UserTo)
                //    {
                //        _notificationSystemRepository.SendNotificationAsync(
                //            @group.UserReciver,
                //            0,
                //            conversation.Id,
                //            _notificationSystemTypeRepository.Find(x => x.Id == 20).Type,
                //            item.message,
                //            1
                //        );
                //    }
                //}

                ///end Create Chat grupal

                //ServiceRecord service = _serviceRecordRepository.Add(_mapper.Map<ServiceRecord>(dto));
                //response.Result = _mapper.Map<ServiceRecordDto>(service);
                // NOTIFICATIONS COORDINATORS
                // if (record.ImmigrationCoodinators.Any())
                // {
                //     var userTo =
                //         _notificationSystemRepository.GetUserId(record.ImmigrationCoodinators.FirstOrDefault()
                //             .CoordinatorId.Value);
                //     NotificationSystemDto notification = new NotificationSystemDto();
                //     notification.Archive = false;
                //     notification.View = false;
                //     notification.ServiceRecord = record.Id;
                //     notification.Time = DateTime.Now.TimeOfDay;
                //     notification.UserFrom = record.CreatedBy;
                //     notification.UserTo = _notificationSystemRepository.GetUserId(record.ImmigrationCoodinators.FirstOrDefault().CoordinatorId.Value);
                //     notification.NotificationType = 1;
                //     notification.Description = "";
                //     notification.Color = "#f06689";
                //     notification.CreatedDate = DateTime.Now;
                //     _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                //     _notificationSystemRepository.SendNotificationAsync(
                //         userTo,
                //         0,
                //         0,
                //         _notificationSystemTypeRepository.Find(x => x.Id == 1).Type,
                //         ""
                //     );
                // }
                // else if(record.RelocationCoordinators.Any())
                // {
                //     var userTo =
                //         _notificationSystemRepository.GetUserId(record.RelocationCoordinators.FirstOrDefault()
                //             .CoordinatorId.Value);
                //     NotificationSystemDto notification = new NotificationSystemDto();
                //     notification.Archive = false;
                //     notification.View = false;
                //     notification.ServiceRecord = record.Id;
                //     notification.Time = DateTime.Now.TimeOfDay;
                //     notification.UserFrom = record.CreatedBy;
                //     notification.UserTo = _notificationSystemRepository.GetUserId(record.RelocationCoordinators.FirstOrDefault().CoordinatorId.Value);
                //     notification.NotificationType = 1;
                //     notification.Description = "";
                //     notification.Color = "#f06689";
                //     notification.CreatedDate = DateTime.Now;
                //     _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                //     _notificationSystemRepository.SendNotificationAsync(
                //         userTo,
                //         0,
                //         0,
                //         _notificationSystemTypeRepository.Find(x => x.Id == 1).Type,
                //         ""
                //     );
                // }
                // NOTIFICATION SUPPLIER PARTNERS
                if (record.ImmigrationSupplierPartners.Any())
                {
                    var userTo =
                        _notificationSystemRepository.GetUserId(record.ImmigrationSupplierPartners.FirstOrDefault()
                            .SupplierId.Value);
                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = record.Id;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom  = record.CreatedBy;
                    notification.UserTo = _notificationSystemRepository.GetUserId(record.ImmigrationSupplierPartners.FirstOrDefault().SupplierId.Value);
                    notification.NotificationType = 25;
                    notification.Description = "New SR assigned, go to your dashboard for more information";
                    notification.Color = "#1565c0";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                        userTo,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 2).Type,
                        "",
                        25
                    );
                    //if (record.Urgent.HasValue)
                    //{
                    //    if (record.Urgent.Value.Equals(true))
                    //    {
                    //        _notificationSystemRepository.Add(new NotificationSystem()
                    //        {
                    //            Archive = false,
                    //            View = false,
                    //            ServiceRecord = record.Id,
                    //            Time = DateTime.Now.TimeOfDay,
                    //            UserFrom = record.CreatedBy,
                    //            UserTo = userTo,
                    //            NotificationType = 27,
                    //            Description = $"{record.NumberServiceRecord} is urgent",
                    //            Color = "ff8f00",
                    //            CreatedDate = DateTime.Now
                    //        });
                    //        _notificationSystemRepository.SendNotificationAsync(
                    //            userTo,
                    //            0,
                    //            0,
                    //            _notificationSystemTypeRepository.Find(x => x.Id == 27).Type,
                    //            $"{record.NumberServiceRecord} is urgent",
                    //            27
                    //        );
                    //    }
                    //}
                }
                else if (record.RelocationSupplierPartners.Any())
                {
                    var userTo =
                        _notificationSystemRepository.GetUserId(record.RelocationSupplierPartners.FirstOrDefault()
                            .SupplierId.Value);
                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = record.Id;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = record.CreatedBy;
                    notification.UserTo = _notificationSystemRepository.GetUserId(record.RelocationSupplierPartners.FirstOrDefault().SupplierId.Value);
                    notification.NotificationType = 25;
                    notification.Description = "New SR assigned, go to your dashboard for more information";
                    notification.Color = "#1565c0";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                        userTo,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 2).Type,
                        "",
                        25
                    );
                    //if (record.Urgent.HasValue)
                    //{
                    //    if (record.Urgent.Value.Equals(true))
                    //    {
                    //        _notificationSystemRepository.Add(new NotificationSystem()
                    //        {
                    //            Archive = false,
                    //            View = false,
                    //            ServiceRecord = record.Id,
                    //            Time = DateTime.Now.TimeOfDay,
                    //            UserFrom = record.CreatedBy,
                    //            UserTo = userTo,
                    //            NotificationType = 27,
                    //            Description = $"{record.NumberServiceRecord} is urgent",
                    //            Color = "ff8f00",
                    //            CreatedDate = DateTime.Now
                    //        });
                    //        _notificationSystemRepository.SendNotificationAsync(
                    //            userTo,
                    //            0,
                    //            0,
                    //            _notificationSystemTypeRepository.Find(x => x.Id == 27).Type,
                    //            $"{record.NumberServiceRecord} is urgent",
                    //            27
                    //        );
                    //    }
                    //}
                }
                
                if (dto.AssigneeInformations.Any()) 
                {
                        int host = dto.AssigneeInformations.FirstOrDefault().HostCountry.HasValue
                            ? dto.AssigneeInformations.FirstOrDefault().HostCountry.Value
                            : 0;
                        int home = dto.AssigneeInformations.FirstOrDefault().HomeCountryId.HasValue
                            ? dto.AssigneeInformations.FirstOrDefault().HomeCountryId.Value
                            : 0;
                        List<int> countryLeaders = _userRepository.GetCountryLeaders(host, home);
                        foreach (var i in countryLeaders)
                        {
                            if (dto.CreatedBy.Value != i)
                            {
                                _notificationSystemRepository.Add(new NotificationSystem()
                                {
                                    Id = 0,
                                    Archive = false,
                                    View = false,
                                    ServiceRecord = record.Id,
                                    Time = DateTime.Now.TimeOfDay,
                                    UserFrom = record.CreatedBy,
                                    UserTo = i,
                                    NotificationType = 26,
                                    Description = $"Service Record {record.NumberServiceRecord} was added.",
                                    Color = "#f06689",
                                    CreatedDate = DateTime.Now
                                });
                                _notificationSystemRepository.Save();
                                _notificationSystemRepository.SendNotificationAsync(
                                    i,
                                    0,
                                    0,
                                    (_notificationSystemTypeRepository.Find(x => x.Id == 26)).Type,
                                    $"Service Record {record.NumberServiceRecord} was added.",
                                    26
                                );    
                            }
                        }
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

        // Put Update a new service record 
        [HttpPut("Update", Name = "Update")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<ServiceRecordDto>>> PostUpdate([FromBody] ServiceRecordInsertDto dto)
        {
            bool isChildNew = false;
            var response = new ApiResponse<ServiceRecordDto>();
            try
            {
                var serviceNewRecord = await _serviceRecordRepository.FindAsync(c => c.Id == dto.Id);

                if (serviceNewRecord != null)
                {
                    if (dto.AssigneeInformations.Count > 0)
                    {
                        if (dto.AssigneeInformations.FirstOrDefault().Photo != null && dto.AssigneeInformations.FirstOrDefault().Photo.Length > 250)
                        {
                            dto.AssigneeInformations.FirstOrDefault().Photo = _utiltyRepository.UploadImageBase64(dto.AssigneeInformations.FirstOrDefault().Photo,
                                "Files/Assignee/", dto.AssigneeInformations.FirstOrDefault().PhotoExtension);
                        }

                        foreach (var i in dto.AssigneeInformations.FirstOrDefault().PetsNavigation)
                        {
                            if (i.Photo != null && i.Photo.Length > 250)
                            {
                                i.Photo = _serviceRecordRepository.UploadImageBase64(i.Photo);
                            }
                        }

                        foreach (var i in dto.AssigneeInformations.FirstOrDefault().DependentInformations)
                        {
                            if (i.Photo != null && i.Photo.Length > 250)
                            {
                                i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/Dependent/", i.PhotoExtension);
                            }
                        }
                    }

                    isChildNew = dto.AssigneeInformations.FirstOrDefault().DependentInformations
                        .Any(x => x.Id == 0 && x.RelationshipId == 2);

                    var _dependent = dto.AssigneeInformations.FirstOrDefault().DependentInformations.ToList();

                    ServiceRecord service = _serviceRecordRepository.UpdateCustom(_mapper.Map<ServiceRecord>(dto), dto.Id);
                    response.Result = _mapper.Map<ServiceRecordDto>(service);

                    var userData = await _userRepository.FindAsync(x => x.Id == dto.AssigneeInformations.FirstOrDefault().UserId);
                    if (userData != null)
                    {
                        userData.Email = dto.AssigneeInformations.FirstOrDefault().Email;
                        await _userRepository.UpdateAsyn(userData, userData.Id);
                    }

                    foreach (var imm in dto.ImmigrationCoodinators)
                    {
                        if(imm.Id == 0)
                        {
                            _notificationSystemRepository.Add(new NotificationSystem()
                            {
                                Id = 0,
                                Archive = false,
                                View = false,
                                ServiceRecord = service.Id,
                                Time = DateTime.Now.TimeOfDay,
                                UserFrom = service.UpdateBy,
                                UserTo = _serviceRecordRepository.GetUserNotification(imm.CoordinatorId.Value),
                                NotificationType = 1,
                                Description = "Please accept the notification to continue with the SR",
                                Color = "#f06689",
                                CreatedDate = DateTime.Now
                            });
                            _notificationSystemRepository.Save();
                            await _notificationSystemRepository.SendNotificationAsync(
                                _serviceRecordRepository.GetUserNotification(imm.CoordinatorId.Value),
                                0,
                                0,
                                (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 21)).Type,
                                 "Please accept the notification to continue with the SR",
                                1
                            );
                        }
                    }
                    //for(int i=0; i< dto.ImmigrationCoodinators.Count(); i++)
                    //{
                    //    dto.ImmigrationCoodinators[0].Id
                    //    //if (dto.ImmigrationCoodinators[i].Id == 0) { 

                    //    //}
                    //}

                    var users = _followRepository.GetAll().Where(x => x.ServiceRecordId == service.Id);
                    if (users.Any())
                    {
                        foreach (var user in users.ToList())
                        {
                            if (dto.UpdateBy.Value != user.UserId)
                            {
                                _notificationSystemRepository.Add(new NotificationSystem()
                                {
                                    Id = 0,
                                    Archive = false,
                                    View = false,
                                    ServiceRecord = service.Id,
                                    Time = DateTime.Now.TimeOfDay,
                                    UserFrom = service.UpdateBy,
                                    UserTo = user.UserId,
                                    NotificationType = 21,
                                    Description = "The SR was modified, enter it for more information",
                                    Color = "#f06689",
                                    CreatedDate = DateTime.Now
                                });
                                _notificationSystemRepository.Save();
                                await _notificationSystemRepository.SendNotificationAsync(
                                    user.UserId,
                                    0,
                                    0,
                                    (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 21)).Type,
                                    "The SR was modified, enter it for more information",
                                    21
                                );    
                            }
                        }
                    }

                    if (dto.Vip != null && dto.Vip.Value)
                    {
                        var operationalLeaders = _userRepository.GetOperationalLeaders(dto.Id);
                        foreach (var leader in operationalLeaders)
                        {
                            _notificationSystemRepository.Add(new NotificationSystem()
                            {
                                Id = 0,
                                Archive = false,
                                View = false,
                                ServiceRecord = service.Id,
                                Time = DateTime.Now.TimeOfDay,
                                UserFrom = service.UpdateBy,
                                UserTo = leader,
                                NotificationType = 21,
                                Description = "The SR was modified, enter it for more information",
                                Color = "#f06689",
                                CreatedDate = DateTime.Now
                            });
                            _notificationSystemRepository.Save();
                           await _notificationSystemRepository.SendNotificationAsync(
                                leader,
                                0,
                                0,
                                (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 21)).Type,
                                "The SR was modified, enter it for more information",
                                21
                            );
                        }
                    }

                    if (isChildNew)
                    {
                        // TO ADD NEW CHILD TO SERVICES WITH-OUT CHILD YET
                    }
                }
                else
                {
                    response.Message = $"Service record SR-{ serviceNewRecord.Id } Not Found";
                    return NotFound(response);
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

        // Send notification Time Reminder 
        [HttpPut("SendNotificationTimeReminder", Name = "SendNotificationTimeReminder")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<ServiceRecordDto>>> SendNotificationTimeReminder(int serviceRecordId)
        {
            var response = new ApiResponse<ServiceRecordDto>();
            try
            {
                var serviceNewRecord = await _serviceRecordRepository.FindAsync(c => c.Id == serviceRecordId);

                foreach (var imm in serviceNewRecord.ImmigrationCoodinators)
                {
                    if (imm.Id == 0)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = serviceRecordId,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = serviceNewRecord.UpdateBy,
                            UserTo = _serviceRecordRepository.GetUserNotification(imm.CoordinatorId.Value),
                            NotificationType = 1,
                            Description = "Please accept the notification to continue with the SR",
                            Color = "#f06689",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.Save();
                        await _notificationSystemRepository.SendNotificationAsync(
                            _serviceRecordRepository.GetUserNotification(imm.CoordinatorId.Value),
                            0,
                            0,
                            (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 21)).Type,
                             "Please accept the notification to continue with the SR",
                            1
                        );
                    }
                }

                foreach (var imm in serviceNewRecord.RelocationCoordinators)
                {
                    if (imm.Id == 0)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = serviceRecordId,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = serviceNewRecord.UpdateBy,
                            UserTo = _serviceRecordRepository.GetUserNotification(imm.CoordinatorId.Value),
                            NotificationType = 1,
                            Description = "Please accept the notification to continue with the SR",
                            Color = "#f06689",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.Save();
                        await _notificationSystemRepository.SendNotificationAsync(
                            _serviceRecordRepository.GetUserNotification(imm.CoordinatorId.Value),
                            0,
                            0,
                            (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 21)).Type,
                             "Please accept the notification to continue with the SR",
                            1
                        );
                    }
                }

                response.Result = _mapper.Map<ServiceRecordDto>(serviceNewRecord);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        // PUT activate service record
        [HttpPut("Activate/{sr}", Name = "Activate")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<ServiceRecordDto>>> PutActivate(int sr)
        {
            ApiResponse<ServiceRecordDto> response = new ApiResponse<ServiceRecordDto>();
            try
            {
                ServiceRecord @record = await _serviceRecordRepository.FindAsync(f => f.Id == sr);
                if (@record != null)
                {
                    @record.StatusId = 2;
                    await _serviceRecordRepository.UpdateAsyn(@record, sr);
                    response.Message = $"Service Record {@record.NumberServiceRecord} was updated.";
                    response.Success = true;
                    response.Result = _mapper.Map<ServiceRecordDto>(@record);
                }
                else
                {
                    response.Message = $"Service Record with ID {sr} was NOT FOUND";
                    response.Success = true;
                    response.Result = _mapper.Map<ServiceRecordDto>(@record);
                    return StatusCode(404, response);
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Something went wrong: { ex.Message }";
                _logger.LogError($"Something went wrong: { ex.Message }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        // Get: Get Services
        [HttpGet("GetServices/{service_record_id}", Name = "GetServices")]
        public ActionResult GetServices(int service_record_id, int type, int? status, int? deliverTo, int? serviceType, int? program, int? userId)
        {
            try
            {
                var map = _serviceRecordRepository.GetServices(service_record_id, type, status, deliverTo, serviceType, program, userId);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: GetInboxHome
        [HttpGet("GetInboxHome", Name = "GetInboxHome")]
        public ActionResult GetInboxHome(int userId)
        {
            try
            {
                var map = _serviceRecordRepository.GetInboxHome(userId);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: GetMessageHome
        [HttpGet("GetMessageHome/{user_id}", Name = "GetMessageHome")]
        public ActionResult GetMessageHome(int userId)
        {
            try
            {
                var map = _serviceRecordRepository.GetMessageHome(userId);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: GetMessageHome
        [HttpGet("GetMessageNotificatiosApp", Name = "GetMessageNotificatiosApp")]
        public ActionResult GetMessageNotificatiosApp(int userId)
        {
            try
            {
                var map = _serviceRecordRepository.GetMessageNotificatiosApp(userId);
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }


        // Get: ValidateEmail
        [HttpGet("ValidateEmail", Name = "ValidateEmail")]
        public ActionResult ValidateEmail(string email)
        {
            try
            {
                var result = _serviceRecordRepository.ValidateEmailDependent(email);
                return Ok(new { Success = true, Result = result, Message = $"Email: {email} Already Exists" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Services
        [HttpGet("GetApplicant/{sr}", Name = "GetApplicant")]
        public ActionResult GetServices(int sr)
        {
            try
            {
                var applicant = _serviceRecordRepository.GetApplicant(sr);
                return Ok(new { Success = true, applicant });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Services
        [HttpGet("GetChildrenBySrId/{sr}", Name = "GetChildrenBySrId")]
        public ActionResult GetChildrenBySrId(int sr)
        {
            try
            {
                var applicant = _serviceRecordRepository.GetChildrenBySrId(sr);
                return Ok(new { Success = true, applicant });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpPut("CompleteServiceRecord", Name = "CompleteServiceRecord")]
        public ActionResult CompleteServiceRecord([FromQuery] int sr, [FromQuery] int serviceLine)
        {
            try
            {
                var applicant = _serviceRecordRepository.CompleteService(sr, serviceLine);
                if (applicant.Item1)
                {
                    _notificationSystemRepository.SendNotificationAsync(
                        _notificationSystemRepository.GetCoordinatorByServiceRecord(sr),
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 17).Type,
                        "",
                        2
                    );
                    return Ok(new { Success = applicant.Item1, Message = applicant.Item2 });
                }
                else
                {
                    return BadRequest(new { Success = applicant.Item1, Message = applicant.Item2 });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpPut("AcceptImmigrationCoordinator/{coordinator}/{accepted}/", Name = "AcceptImmigrationCoordinator/{coordinator}/{accepted}/")]
        public ActionResult AccpetImmigrationCoordinator(int coordinator, bool accepted, int sr)
        {
            try
            {
                var applicant = _serviceRecordRepository.AcceptOrRejectCoordinator(coordinator, accepted, sr);
                if (applicant)
                {
                    return Ok(new { Success = applicant, Message = "Coordinator Accepted." });
                }
                else
                {
                    return BadRequest(new { Success = applicant, Message = "Coordinator Rejected." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }
        [HttpPut("AcceptRelocationCoordinator/{coordinator}/{accepted}/", Name = "AcceptRelocationCoordinator/{coordinator}/{accepted}/")]
        public ActionResult AccpetRelocationCoordinator(int coordinator, bool accepted, int sr)
        {
            try
            {
                var applicant = _serviceRecordRepository.AccpetOrRejectRelocationCoordinator(coordinator, accepted, sr);
                if (applicant)
                {
                    return Ok(new { Success = applicant, Message = "Coordinator Accepted." });
                }
                else
                {
                    return BadRequest(new { Success = applicant, Message = "Coordinator Rejected." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }
        [HttpPut("AcceptImmigrationSupplierPartner/{supplier}/{accepted}/", Name = "AcceptImmigrationSupplierPartner/{supplier}/{accepted}/")]
        public ActionResult AccpetImmigrationSupplierPartner(int supplier, bool accepted, int sr)
        {
            try
            {
                var applicant = _serviceRecordRepository.AccpetImmigrationSupplierPartner(supplier, accepted, sr);
                var from = supplier;//_serviceRecordRepository.GetUserBySupplier(supplier);
                var to = _serviceRecordRepository.GetUserByCoordinatorImm(sr);
                
                if (applicant)
                {
                    for (int i = 0; i < to.Count(); i++)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = sr,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = from,
                            UserTo = to[i],
                            NotificationType = 28,
                            Description = "The supplier accepted the SR",
                            Color = "#F1C40F",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.Save();
                    }
                    return Ok(new { Success = applicant, Message = "Coordinator Accepted." });
                }
                else
                {
                    return BadRequest(new { Success = applicant, Message = "Coordinator Rejected." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }
        [HttpPut("AcceptRelocationSupplierPartner/{supplier}/{accepted}/", Name = "AcceptRelocationSupplierPartner/{supplier}/{accepted}/")]
        public ActionResult AccpetRelocationSupplierPartner(int supplier, bool accepted, int sr)
        {
            try
            {
                var applicant = _serviceRecordRepository.AccpetRelocationSupplierPartner(supplier, accepted, sr);
                var from = supplier;//_serviceRecordRepository.GetUserBySupplier(supplier);
                var to = _serviceRecordRepository.GetUserByCoordinatorRelo(sr);

                if (applicant)
                {
                    for (int i = 0; i < to.Count(); i++)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = sr,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = from,
                            UserTo = to[i],
                            NotificationType = 28,
                            Description = "The supplier accepted the SR",
                            Color = "#F1C40F",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.Save();
                    }
                    return Ok(new { Success = applicant, Message = "Supplier Accepted." });
                }
                else
                {
                    return BadRequest(new { Success = applicant, Message = "Supplier Rejected." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: {ex.ToString()}" });
            }
        }

        [HttpPut("AccpetImmigrationSupplierPartnerIndividual/{supplier}/{accepted}/{ServiceOrderServicesId}/", Name = "AccpetImmigrationSupplierPartnerIndividual/{supplier}/{accepted}/{ServiceOrderServicesId}/")]
        public ActionResult AccpetImmigrationSupplierPartnerIndividual(int supplier, bool accepted, int sr, int ServiceOrderServicesId)
        {
            try
            {
                var applicant = _serviceRecordRepository.AccpetImmigrationSupplierPartnerIndividual(supplier, accepted, sr, ServiceOrderServicesId);
                var from = supplier;//_serviceRecordRepository.GetUserBySupplier(supplier);
                var to = _serviceRecordRepository.GetUserByCoordinatorImm(sr);

                if (applicant)
                {
                    for (int i = 0; i < to.Count(); i++)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = sr,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = from,
                            UserTo = to[i],
                            NotificationType = 28,
                            Description = "The supplier accepted the SR",
                            Color = "#F1C40F",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.Save();
                    }
                    return Ok(new { Success = applicant, Message = "Coordinator Accepted." });
                }
                else
                {
                    return BadRequest(new { Success = applicant, Message = "Coordinator Rejected." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }
        [HttpPut("AccpetRelocationSupplierPartnerIndividual/{supplier}/{accepted}/{ServiceOrderServicesId}/", Name = "AccpetRelocationSupplierPartnerIndividual/{supplier}/{accepted}/{ServiceOrderServicesId}/")]
        public ActionResult AccpetRelocationSupplierPartnerIndividual(int supplier, bool accepted, int sr, int ServiceOrderServicesId)
        {
            try
            {
                var applicant = _serviceRecordRepository.AccpetRelocationSupplierPartnerIndividual(supplier, accepted, sr, ServiceOrderServicesId);
                var from = supplier;// _serviceRecordRepository.GetUserBySupplier(supplier);
                var to = _serviceRecordRepository.GetUserByCoordinatorRelo(sr);

                if (applicant)
                {
                    for (int i = 0; i < to.Count(); i++)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = sr,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = from,
                            UserTo = to[i],
                            NotificationType = 28,
                            Description = "The supplier accepted the SR",
                            Color = "#F1C40F",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.Save();
                    }
                    return Ok(new { Success = applicant, Message = "Coordinator Accepted." });
                }
                else
                {
                    return BadRequest(new { Success = applicant, Message = "Coordinator Rejected." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: {ex.ToString()}" });
            }
        }

        [HttpGet("GetPendingAuthorizations/{user}/", Name = "GetPendingAuthorizations/{user}/")]
        public ActionResult GetPendingAuthorizations(int user, [FromQuery] int? country, [FromQuery] int? city, [FromQuery] int? service_line, [FromQuery] int? sr)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceRecordRepository.GetPendingAuthorizations(user, country, city, service_line, sr) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: {ex.ToString()}" });
            }
        }

        // Get: Country Work Order
        [HttpGet("GetCountryByServiceRecord/{sr}", Name = "GetCountryByServiceRecord")]
        public ActionResult GetCountryByServiceRecord(int sr)
        {
            try
            {
                var Country = _serviceRecordRepository.GetCountryByServiceRecord(sr);
                return Ok(new { Success = true, Country });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Category Work Order
        [HttpGet("GetCategoryByCountry/{idcountry}/{IdClientPartnerProfile}", Name = "GetCategoryByCountry")]
        public ActionResult GetCategoryByCountry(int idcountry, int IdClientPartnerProfile, int IdserviceLine)
        {
            try
            {
                var Country = _serviceRecordRepository.GetCategoryByCountry(idcountry, IdClientPartnerProfile, IdserviceLine);
                return Ok(new { Success = true, Country });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Delivered To
        [HttpGet("GetDeliveredTo", Name = "GetDeliveredTo")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDeliveredTo(int service, int category, int serviceType)
        {
            try
            {
                var deliveredTo = _serviceRecordRepository.GetDeliveredTo(service, category, serviceType);
                return Ok(new { Success = true, Result = deliveredTo, Message = "Success"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }
        
        // GET: Completion Report
        [HttpGet("CompleteReport", Name = "CompleteReport")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult CompleteReport(int sr, int serviceLine)
        {
            try
            {
                var completeReport = _serviceRecordRepository.CompleteReport(sr, serviceLine);
                return Ok(new { Success = true, Result = completeReport, Message = "Success"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }
        
        // POST: Completion Report Save
        [HttpPost("CompleteReport/Save", Name = "CompleteReport/Save")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult CompleteReportSave(int sr, int serviceLine)
        {
            try
            {
                var find = _serviceRecordRepository.Find(f => f.Id == sr);
                if (serviceLine == 1)
                {
                    find.ImmigrationCompleteReport = true;
                }
                else
                {
                    find.RelocationCompleteReport = true;
                }

                _serviceRecordRepository.Update(find, find.Id);
                return Ok(new { Success = true, Result = 0, Message = "Complete Report was added Success"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        // GET: Completion Report Save
        [HttpGet("See/CompleteReport", Name = "SeeCompleteReport")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult CompleteReportSave(int sr)
        {
            try
            {
                var find = _serviceRecordRepository.CompleteReportInfo(sr);

                // _serviceRecordRepository.Update(find, find.Id);
                return Ok(new { Success = true, Result = find, Message = "Complete Report was added Success"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }
        
        // GET: List Countries in Sr
        [HttpGet("Countries/{sr}", Name = "Countries")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCountriesInSr(int sr)
        {
            try
            {
                //var countries = _serviceRecordRepository.GetCountriesSr(sr);
                
                return Ok(new { Success = true, Result = "", Message = "Operation was successfully."});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }
        
        // GET: Experience Team By Sr
        [HttpGet("ExperienceTeam/{sr}", Name = "ExperienceTeam")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult ExperienceTeam(int sr)
        {
            try
            {
                var find = _serviceRecordRepository.GetCoordinatorsAndSupplierBySr(sr);
                return Ok(new { Success = true, Result = find, Message = "Operation was successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }
        
        // GET: Experience Team By Sr
        [HttpGet("SupplierServiceConsultant/{sr}", Name = "SupplierServiceConsultant")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierServiceConsultant(int sr)
        {
            try
            {
                var find = _serviceRecordRepository.GetSuppliersConsultantAndServiceBySr(sr);
                return Ok(new { Success = true, Result = find, Message = "Operation was successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        // GET: GetReportDayApp
        [HttpGet("GetReportDayApp/{report_day_id}", Name = "GetReportDayApp")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetReportDayApp(int report_day_id)
        {
            try
            {
                var find = _serviceRecordRepository.GetReportDayApp(report_day_id);
                return Ok(new { Success = true, Result = find, Message = "Operation was successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        // GET: Experience Team By Sr
        [HttpPut("OnHold/{sr}/{serviceLine}", Name = "OnHold")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult OnHoldServiceRecord(int sr, int serviceLine)
        {
            try
            {
                var find = _serviceRecordRepository.OnHoldServiceRecord(sr,serviceLine);
                return Ok(new { Success = true, Result = find, Message = "Operation was successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        [HttpDelete("Coordinator/{coordinator}/{serviceLine}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteCoordinator(int coordinator, int serviceLine)
        {
            try
            {
                var find = _serviceRecordRepository.DeleteCoordinator(coordinator, serviceLine);
                if (find)
                {
                    return Ok(new {Success = true, Result = find, Message = "Operation was successfully."});
                }
                else
                {
                    return BadRequest(new {Success = false, Result = 0, Message = "Operation was not successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }
        
        [HttpDelete("Supplier/{supplier}/{serviceLine}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteSupplier(int supplier, int serviceLine)
        {
            try
            {
                var find = _serviceRecordRepository.DeleteSupplier(supplier, serviceLine);
                var chat = _serviceRecordRepository.DeleteSupplierChat(find.Item2, supplier);                

                if (find.Item1)
                {
                    return Ok(new {Success = true, Result = find, Message = "Operation was successfully."});
                }
                else
                {
                    return BadRequest(new {Success = false, Result = 0, Message = "Operation was not successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        // GET: Service Record Status
        [HttpGet("SetStatusServiceRecord/{sr}/{serviceLine}", Name = "SetStatusServiceRecord")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<Tuple<int, string>>> SetStatusServiceRecord(int sr, int serviceLine)
        {
            var response = new ApiResponse<Tuple<int, string>>();
            try
            {
                response.Result = _serviceRecordRepository.SetStatusServiceRecord(sr, serviceLine);
                response.Success = true;
                response.Message = "Operation was successfully";
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        // GET: GetChangeNotificationsToRead
        [HttpGet("GetChangeNotificationsToRead/{user}", Name = "GetChangeNotificationsToRead")]
        public ActionResult GetChangeNotificationsToRead(int user)
        {
            try
            {
                var map = _serviceRecordRepository.GetChangeNotificationsToRead(user);

                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// SERVICIOS APP ENERO  2023 
        /// 


        //// Get: Get Service Record
        [HttpGet("GetAssigneeInformationsBySRId", Name = "GetAssigneeInformationsBySRId")]
        public ActionResult GetAssigneeInformationsBySRId([FromQuery] int service_record_id)
        {
            try
            {
                var mao = _serviceRecordRepository.GetAssigneeInformationAppAll(service_record_id);
                //var map = _serviceRecordRepository.GetAssigneeInformationApp(service_record_id);

                mao.DependentInformations = mao.DependentInformations.Where(d => d.RelationshipId != 7).ToList();
                return Ok(new { Success = true, mao });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }


        //// Get: Get Service Record
        [HttpGet("GetBasicServiceData", Name = "GetBasicServiceData")]
        public ActionResult GetBasicServiceData([FromQuery] int id_service, int type_sr)
        {
            try
            {
                //var mao = _serviceRecordRepository.GetAssigneeInformationAppAll(service_record_id);
                var atributos_generales = _utiltyRepository.atributos_generales(id_service, type_sr);
                //var map = _serviceRecordRepository.GetAssigneeInformationApp(service_record_id);

               // mao.DependentInformations = mao.DependentInformations.Where(d => d.RelationshipId != 7).ToList();
                return Ok(new { Success = true, atributos_generales });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

    }
}
