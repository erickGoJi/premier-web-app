using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using biz.premier.Repository.Appointment;
using AutoMapper;
using api.premier.Models.Appointment;
using biz.premier.Entities;
using api.premier.Models;
using api.premier.ActionFilter;
using api.premier.Models.NotificationSystem;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Servicies;
using biz.premier.Repository.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using biz.premier.Repository.Reports;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using biz.premier.Repository;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        private readonly IReports _reports;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUserRepository _userRepository;

        public class serviceRelated
        {
            public List<int> serviceList { get; set; }
        }
        public AppointmentController(IMapper mapper, ILoggerManager logger, IAppointmentRepository appointmentRepository, IUtiltyRepository utiltyRepository,
            INotificationSystemRepository notificationSystemRepository, ICatNotificationSystemTypeRepository notificationSystemTypeRepository, IReports reports, IHostingEnvironment hostingEnvironment, IUserRepository userRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _appointmentRepository = appointmentRepository;
            _utiltyRepository = utiltyRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
            _reports = reports;
            _hostingEnvironment = hostingEnvironment;
            _userRepository = userRepository;
        }

        // GET: Return all services by service record ID
        [HttpGet("All/Services/{sr}", Name = "AllServices")]
        public async Task<ActionResult<ApiResponse<ActionResult>>> AllServices(int sr)
        {
            ApiResponse<ActionResult> response = new ApiResponse<ActionResult>();
            try
            {
                response.Result = await _appointmentRepository.GetServicesByServiceRecord(sr);
                response.Message = "Operation was successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }

        // GET: Return all services by service record ID
        [HttpGet("All/AllServicesServiceLine/", Name = "AllServicesServiceLine")]
        public async Task<ActionResult<ApiResponse<ActionResult>>> AllServicesServiceLine(int sr, int sl)
        {
            ApiResponse<ActionResult> response = new ApiResponse<ActionResult>();
            try
            {
                var _result = await _appointmentRepository.GetServicesByServiceRecordAndServiceLine(sr, sl);
                response.Result = _result;
                response.Message = "Operation was successfully.";
                response.Success = true;
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

        // Get: Get by Id
        [HttpGet("GetAppointmentById", Name = "GetAppointmentById")]
        public ActionResult GetAppointmentById([FromQuery] int id)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.SelectCustom(id) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() }); ;
            }

        }

        // Get: Get by Id
        [HttpGet("GetAppointmentByIdApp", Name = "GetAppointmentByIdApp")]
        public ActionResult GetAppointmentByIdApp([FromQuery] int appointmentId)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAppointmentByIdApp(appointmentId) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() }); ;
            }

        }

        // Post Appointment housing
        [HttpPost("AddAppointmentHousing", Name = "AddAppointmentHousing")]
        public ActionResult AddAppointmentHousing(int appointmentId, int housingId, bool action)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.AddAppointmentHousing(appointmentId, housingId, action) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        // Post Appointment schooling
        [HttpPost("AddAppointmentSchooling", Name = "AddAppointmentSchooling")]
        public ActionResult AddAppointmentSchooling(int appointmentId, int schoolingId, bool action)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.AddAppointmentSchooling(appointmentId, schoolingId, action) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        // Get: Get 
        [HttpGet("GetAppointmentByServiceRecordId", Name = "GetAppointmentByServiceRecordId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAppointmentByServiceRecordId(int id)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAppointmentByServiceRecordId(id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAppointmentsExcel", Name = "GetAppointmentsExcel")]
        public ActionResult<ApiResponse<string>> GetAppointmentsExcel(int id)
        {
            var response = new ApiResponse<string>();

            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "Appointment" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string xFull = Path.Combine(xPath, xNombre);

                _reports.createExcelAppointment(id, xFull);

                Byte[] bytes = System.IO.File.ReadAllBytes(xFull);
                String base64 = Convert.ToBase64String(bytes);

                if (System.IO.File.Exists(xFull))
                    System.IO.File.Delete(xFull);

                response.Result = "ok";
                response.Message = base64;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = "Internal server error";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);

        }

        [HttpGet("GetPDF", Name = "GetPDF")]
        public ActionResult<ApiResponse<string>> GetPDF(string xURL, int miliseconds)
        {
            var response = new ApiResponse<string>();

            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "archivo" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                string xFull = Path.Combine(xPath, xNombre);

                _reports.createPDF(xURL, xFull, miliseconds);

                Byte[] bytes = System.IO.File.ReadAllBytes(xFull);
                String base64 = Convert.ToBase64String(bytes);

                if (System.IO.File.Exists(xFull))
                    System.IO.File.Delete(xFull);

                response.Result = "ok";
                response.Message = base64;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = "Internal server error";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);

        }

        [HttpGet("pdfExample", Name = "pdfExample")]
        public ActionResult<ApiResponse<string>> pdfExample(string xURL, int miliseconds)
        {
            var response = new ApiResponse<string>();

            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "archivo" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                string xFull = Path.Combine(xPath, xNombre);

                _reports.pdfExample();

                Byte[] bytes = System.IO.File.ReadAllBytes(xFull);
                String base64 = Convert.ToBase64String(bytes);

                if (System.IO.File.Exists(xFull))
                    System.IO.File.Delete(xFull);

                response.Result = "ok";
                response.Message = base64;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = "Internal server error";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);

        }

        [HttpGet("GetAllAppointment", Name = "GetAllAppointment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllAppointment(int id)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAllAppointment(id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }


        //public class add_edit_appointment
        //{
        //    List<AppointmentDto> lista_appointmenst { get; set; }
        //    List<int> lista_escuelas 
        //}

        // Post Create a new Appointment 
        [HttpPost("CreateAppointment", Name = "CreateAppointment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<AppointmentDto>>> PostAppointment([FromBody] List<AppointmentDto> dto)
        {
            var response = new ApiResponse<List<AppointmentDto>>();
            try
            {
                List<AppointmentDto> appointment = new List<AppointmentDto>();
                foreach (var i in dto)
                {
                    var isAvailable = _appointmentRepository.IsAvailable(i.To.Value, i.Date, i.StartTime.Value, i.EndTime.Value);
                    if (isAvailable.Item2)
                    {
                        i.CreatedDate = DateTime.Now;
                        i.Status = 1;
                        foreach (var j in i.DocumentAppointments)
                        {
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Appointment/", j.FileExtension);
                        }

                        appointment.Add(_mapper.Map<AppointmentDto>(_appointmentRepository.Add(_mapper.Map<Appointment>(i))));
                    }
                    else
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = $"{isAvailable.Item1} is not available.";
                        return StatusCode(400, response);
                    }
                }

                for(int x = 0; x < dto.Count(); x++)
                {
                    if (dto.Count() > 1)
                    {
                        var userTo =
                        _notificationSystemRepository.GetUserAssignee(dto[x].ServiceRecordId.Value);
                        _notificationSystemRepository.SendNotificationAsync(
                            userTo,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 3).Type,
                            "",
                            2
                        );

                        var _userAssignee = _userRepository.Find(x => x.Id == userTo);
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/appointment.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();

                        var url_images = _utiltyRepository.Get_url_email_images("newmail2");
                        body = body.Replace("{url_images}", url_images);
                        body = body.Replace("{user}", _userAssignee.Name);
                        body = body.Replace("{username}", $"{_userAssignee.Email}");
                        body = body.Replace("{pass}", "New Appointment");
                        body = body.Replace("{day}", dto[x].StartTime?.ToString("MM/dd/yyyy hh:mm"));

                        _userRepository.SendMail(_userAssignee.Email, body, "New Appointment | " + dto[x].StartTime?.ToString("MM/dd/yyyy hh:mm"));
                        var experienceInts = _notificationSystemRepository.GetExperienceTeamByConsultorUserId(dto[x].ServiceRecordId.Value, dto[x].To.Value);
                        if (experienceInts.Any())
                        {
                            foreach (var experienceInt in experienceInts)
                            {
                                if (dto[x].CreatedBy != experienceInt)
                                {
                                    _notificationSystemRepository.Add(new NotificationSystem()
                                    {
                                        Id = 0,
                                        Archive = false,
                                        View = false,
                                        ServiceRecord = dto[x].ServiceRecordId,
                                        Time = DateTime.Now.TimeOfDay,
                                        UserFrom = dto[x].CreatedBy,
                                        UserTo = experienceInt.Value,
                                        NotificationType = 3,
                                        Description = "Message: " + $"{(dto[x].Description.Length > 46 ? dto[x].Description.Substring(0, 46) + "..." : dto[x].Description)}",
                                        Color = "#c62828",
                                        CreatedDate = DateTime.Now
                                    });
                                    _notificationSystemRepository.SendNotificationAsync(
                                        experienceInt.Value,
                                        0,
                                        0,
                                        _notificationSystemTypeRepository.Find(x => x.Id == 3).Type,
                                        "Message: " + $"{(dto[x].Description.Length > 46 ? dto[x].Description.Substring(0, 46) + "..." : dto[x].Description)}",
                                        2
                                    );
                                }
                            }
                        }
                        break;
                    }
                    else
                    {
                        var userTo =
                        _notificationSystemRepository.GetUserAssignee(dto[x].ServiceRecordId.Value);
                        _notificationSystemRepository.SendNotificationAsync(
                            userTo,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 3).Type,
                            "",
                            2
                        );

                        var _userAssignee = _userRepository.Find(x => x.Id == userTo);
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/appointment.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{user}", _userAssignee.Name);
                        body = body.Replace("{username}", $"{_userAssignee.Email}");

                        var url_images = _utiltyRepository.Get_url_email_images("newmail2");
                        body = body.Replace("{url_images}", url_images);
                        body = body.Replace("{pass}", "New Appointment");
                        body = body.Replace("{day}", dto[x].StartTime?.ToString("MM/dd/yyyy h:mm tt"));

                       // _userRepository.SendMail("ingarmandofranco@gmail.com", body, "New Appointment | " + dto[x].StartTime?.ToString("MM/dd/yyyy h:mm tt"));
                        _userRepository.SendMail(_userAssignee.Email, body, "New Appointment | " + dto[x].StartTime?.ToString("MM/dd/yyyy h:mm tt"));

                        var experienceInts = _notificationSystemRepository.GetExperienceTeamByConsultorUserId(dto[x].ServiceRecordId.Value, dto[x].To.Value);
                        if (experienceInts.Any())
                        {
                            foreach (var experienceInt in experienceInts)
                            {
                                if (dto[x].CreatedBy != experienceInt)
                                {
                                    _notificationSystemRepository.Add(new NotificationSystem()
                                    {
                                        Id = 0,
                                        Archive = false,
                                        View = false,
                                        ServiceRecord = dto[x].ServiceRecordId,
                                        Time = DateTime.Now.TimeOfDay,
                                        UserFrom = dto[x].CreatedBy,
                                        UserTo = experienceInt.Value,
                                        NotificationType = 3,
                                        Description = "Message: " + $"{(dto[x].Description.Length > 46 ? dto[x].Description.Substring(0, 46) + "..." : dto[x].Description)}",
                                        Color = "#c62828",
                                        CreatedDate = DateTime.Now
                                    });
                                    _notificationSystemRepository.SendNotificationAsync(
                                        experienceInt.Value,
                                        0,
                                        0,
                                        _notificationSystemTypeRepository.Find(x => x.Id == 3).Type,
                                        "Message: " + $"{(dto[x].Description.Length > 46 ? dto[x].Description.Substring(0, 46) + "..." : dto[x].Description)}",
                                        2
                                    );
                                }
                            }
                        }
                    }
                }

                response.Success = true;
                response.Message = "Success";
                response.Result = appointment;
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

        // Put Update a Appointment 
        [HttpPost("UpdateAppointment", Name = "UpdateAppointment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<AppointmentDto>>> UpdateAppointment([FromBody] List<AppointmentDto> dto)
        {
            var response = new ApiResponse<List<AppointmentDto>>();
            try
            {
                foreach (var i in dto)
                {
                    i.CreatedDate = DateTime.Now;
                    foreach (var j in i.DocumentAppointments)
                    {
                        if(j.FileRequest.Length > 100)
                        {
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Appointment/", j.FileExtension);
                        }
                    }
                    _appointmentRepository.UpdateCustom(_mapper.Map<Appointment>(i));

                    var experienceTeamImmigration =
                        _notificationSystemRepository.GetExperienceTeam(i.ServiceRecordId.Value, 1);
                    var experienceTeamRelocation =
                        _notificationSystemRepository.GetExperienceTeam(i.ServiceRecordId.Value, 2);
                    var experienceInts = experienceTeamImmigration.Union(experienceTeamRelocation).ToArray();
                    if (experienceInts.Any())
                    {
                        foreach (var experienceInt in experienceInts)
                        {
                            if (i.UpdateBy != experienceInt)
                            {
                                int status = i.Status.Value.Equals(true) ? 3 : 4;
                                _notificationSystemRepository.Add(new NotificationSystem()
                                {
                                    Id = 0,
                                    Archive = false,
                                    View = false,
                                    ServiceRecord = i.ServiceRecordId,
                                    Time = DateTime.Now.TimeOfDay,
                                    UserFrom = i.CreatedBy,
                                    UserTo = experienceInt.Value,
                                    NotificationType = status,
                                    Description = $"{i.Description}",
                                    Color = "#c62828",
                                    CreatedDate = DateTime.Now
                                });
                                _notificationSystemRepository.SendNotificationAsync(
                                    experienceInt.Value,
                                    0,
                                    0,
                                    _notificationSystemTypeRepository.Find(x => x.Id == status).Type,
                                    $"{i.Description}",
                                    2
                                );
                            }
                        }
                    }

                    var userTo =
                       _notificationSystemRepository.GetUserAssignee(i.ServiceRecordId.Value);
                    _notificationSystemRepository.SendNotificationAsync(
                        userTo,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 3).Type,
                        "",
                        2
                    );

                    var _userAssignee = _userRepository.Find(x => x.Id == userTo);
                    StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/appup.html"));
                    string body = string.Empty;
                    body = reader.ReadToEnd();
                    body = body.Replace("{user}", _userAssignee.Name);
                    body = body.Replace("{username}", $"{_userAssignee.Email}");

                    var url_images = _utiltyRepository.Get_url_email_images("newmail2");
                    body = body.Replace("{url_images}", url_images);
                    var evento = "Updated";
                    if (i.Status == 3)
                        evento = "Canceled";

                    body = body.Replace("{day}", i.StartTime?.ToString("MM/dd/yyyy h:mm tt"));

                   //  _userRepository.SendMail("ingarmandofranco@gmail.com", body, evento+" Appointment | " + i.StartTime?.ToString("MM/dd/yyyy h:mm tt"));
                    _userRepository.SendMail(_userAssignee.Email, body, evento + " Appointment | " + i.StartTime?.ToString("MM/dd/yyyy h:mm tt"));


                }
                response.Success = true;
                response.Message = "Success";
                response.Result = dto;
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
        
        // Delete Update a Appointment 
        [HttpDelete]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AppointmentDto>> DeleteAppointment(int key)
        {
            var response = new ApiResponse<AppointmentDto>();
            try
            {
                Appointment appointment = _appointmentRepository.Find(f => f.Id == key);

                appointment.Status = 3;
                _appointmentRepository.Update(appointment, key);
                
                var userTo =
                    _notificationSystemRepository.GetUserAssignee(appointment.ServiceRecordId.Value);
                _notificationSystemRepository.SendNotificationAsync(
                    userTo,
                    0,
                    0,
                    _notificationSystemTypeRepository.Find(x => x.Id == 4).Type,
                    "",
                    2
                );

                response.Success = true;
                response.Message = "Delete was success";
                response.Result = _mapper.Map<AppointmentDto>(appointment);
                
                var experienceTeamImmigration =
                    _notificationSystemRepository.GetExperienceTeam(appointment.ServiceRecordId.Value, 1);
                var experienceTeamRelocation =
                    _notificationSystemRepository.GetExperienceTeam(appointment.ServiceRecordId.Value, 2);
                var experienceInts = experienceTeamImmigration.Union(experienceTeamRelocation).ToArray();
                if (experienceInts.Any())
                {
                    foreach (var experienceInt in experienceInts)
                    {
                        if (appointment.CreatedBy != experienceInt)
                        {
                            _notificationSystemRepository.Add(new NotificationSystem()
                            {
                                Id = 0,
                                Archive = false,
                                View = false,
                                ServiceRecord = appointment.ServiceRecordId,
                                Time = DateTime.Now.TimeOfDay,
                                UserFrom = appointment.CreatedBy,
                                UserTo = experienceInt.Value,
                                NotificationType = 4,
                                Description = $"{appointment.Description}",
                                Color = "#c62828",
                                CreatedDate = DateTime.Now
                            });
                            _notificationSystemRepository.SendNotificationAsync(
                                experienceInt.Value,
                                0,
                                0,
                                _notificationSystemTypeRepository.Find(x => x.Id == 4).Type,
                                $"{appointment.Description}",
                                2
                            );
                        }
                    }
                }
            }
            catch(DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    var number = sqlException.Number;
                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
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
            return StatusCode(202, response);
        }

        [HttpGet("GetAppointmentByUser", Name = "GetAppointmentByUser")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAppointmentByUser(int UserId, int? serviceRecordId, int? status, DateTime? dateRange1, DateTime? dateRange2)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAppointmentByUser(UserId, serviceRecordId, status, dateRange1, dateRange2) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAppointmentByAssigneeApp", Name = "GetAppointmentByAssigneeApp")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAppointmentByAssigneeApp(int UserId, int? status, DateTime? dateRange1, DateTime? dateRange2)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAppointmentByAssignee(UserId, status, dateRange1, dateRange2) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAppointmentByUserApp", Name = "GetAppointmentByUserApp")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAppointmentByUserApp(int UserId, int? serviceRecordId, int? status, DateTime? dateRange1, DateTime? dateRange2)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAppointmentByUser(UserId, serviceRecordId, status, dateRange1, dateRange2) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }


        //Servicios APP
        // GetAppointmentByAssigneeId
        [HttpGet("GetAppointmentByAssignee", Name = "GetAppointmentByAssignee")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAppointmentByAssignee(int UserId)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetAppointmentByAssigneeId(UserId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        // GetCountHousingSchoolAppointment
        [HttpPost("GetCountHousingSchoolAppointment", Name = "GetCountHousingSchoolAppointment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCountHousingSchoolAppointment([FromBody] List<int> servicesRelated, DateTime appointmentDate)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.GetCountHousingSchoolAppointment(servicesRelated, appointmentDate) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpPost("App/Start/{appointment}/{user}",Name = "App/Start/{appointment}/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddStart(int appointment, int user)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.AddReport(appointment, user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
        
        [HttpPut("App/End/{report}/{appointment}/{user}",Name = "App/End/{report}/{appointment}/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult UpdateEnd(int appointment, int report, int user)
        {
            try
            {
                return Ok(new { Success = true, Result = _appointmentRepository.UpdateReport(report, appointment, user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
    }
}
