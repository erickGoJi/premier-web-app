using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.WorkOrder;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.AssigneeName;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.SchoolingSearch;
using biz.premier.Repository.Reports;
using biz.premier.Repository.WorkOrder;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using biz.premier.Repository.Utility;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolsListController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ISchoolListRepository _schoolListRepository;
        private readonly ISchoolingSearchRepository _schoolingSearchRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssigneeNameRepository _assigneeNameRepository;
        private readonly IReports _reports;
        private readonly IUtiltyRepository _utiltyRepository;

        public SchoolsListController(ISchoolListRepository schoolListRepository, IMapper mapper, ILoggerManager logger,
            INotificationSystemRepository notificationSystemRepository, 
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository,
            IUserRepository userRepository,
            IAssigneeNameRepository assigneeNameRepository, 
            IReports reports,
            IUtiltyRepository utiltyRepository, ISchoolingSearchRepository schoolingSearchRepository
            )
        {
            _mapper = mapper;
            _logger = logger;
            _schoolListRepository = schoolListRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
            _userRepository = userRepository;
            _assigneeNameRepository = assigneeNameRepository;
            _schoolingSearchRepository = schoolingSearchRepository;
            _reports = reports;
            _utiltyRepository = utiltyRepository;
        }

        //Post Create a new Housing
        [HttpPost("PostSchools", Name = "PostSchools")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SchoolsListDto>> PostSchools([FromBody] SchoolsListDto dto)
        {
            var response = new ApiResponse<SchoolsListDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                int countSchool = _schoolListRepository.FindAll(x => x.WorkOrder == dto.WorkOrder).Count();
                dto.SchoolNo = countSchool + 1;
                SchoolsList hl = _schoolListRepository.Add(_mapper.Map<SchoolsList>(dto));
                response.Result = _mapper.Map<SchoolsListDto>(hl);
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

        //Put Create a new Housing
        [HttpPut("PutSchools", Name = "PutSchools")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SchoolsListDto>> PutSchools([FromBody] SchoolsListDto dto)
        {
            var response = new ApiResponse<SchoolsListDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                var find = _schoolListRepository.Find(x => x.Id == dto.Id);

                if (dto.SchoolingStatus != null)
                {
                    if (dto.SchoolingStatus.Value == 4 || dto.SchoolingStatus.Value == 5 || dto.SchoolingStatus.Value == 6)
                    {
                        var coordinator = _notificationSystemRepository.GetCoordinator(dto.WorkOrder.Value);
                        _notificationSystemRepository.SendNotificationAsync(
                            coordinator.Item1,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 18).Type,
                            "",
                            2
                        );
                    }
                }
                
                SchoolsList hl = _schoolListRepository.Update(_mapper.Map<SchoolsList>(dto), dto.Id);
                response.Result = _mapper.Map<SchoolsListDto>(hl);
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

        //Get Create a new Housing
        [HttpGet("GetSchool", Name = "GetSchool")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SchoolsListDto>> GetSchool([FromQuery] int key)
        {
            var response = new ApiResponse<SchoolsListDto>();
            try
            {
                SchoolsList hl = _schoolListRepository.GetAll().Where(x => x.Id == key).FirstOrDefault();
                response.Result = _mapper.Map<SchoolsListDto>(hl);
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

        //Get Create a new Housing
        [HttpGet("GetAllSchool", Name = "GetAllSchool")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllSchool([FromQuery] int sr)
        {
            try
            {
                var hl = _schoolListRepository.GetAllIncluding(i => i.DependentNavigation, u => u.CurrencyNavigation, s => s.SchoolingStatusNavigation, sr => sr.WorkOrderNavigation)
                    .Where(x => x.WorkOrderNavigation.ServiceRecordId.Value == sr)
                    .ToList();
                var custom = hl.Select(s => new
                {
                    s.Id,
                    s.SchoolNo,
                    s.SchoolName,
                    Name =  s.DependentNavigation == null ? "" : s.DependentNavigation.Name,
                    s.Address,
                    s.PerMonth,
                    Currency = s.CurrencyNavigation == null ? "": s.CurrencyNavigation.Currency,
                    Status = s.SchoolingStatusNavigation == null ? "" : s.SchoolingStatusNavigation.Status
                }).ToList();
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetAllSchoolByWoId", Name = "GetAllSchoolByWoId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllSchoolByWoId([FromQuery] int wo_id, int? school_search_id, DateTime? dateViste, int? status)
        {
            try
            {
                var hl = _schoolListRepository.GetAllIncluding(i => i.DependentNavigation, 
                    u => u.CurrencyNavigation, 
                    s => s.SchoolingStatusNavigation, 
                    sr => sr.WorkOrderNavigation,
                    c => c.GradeNavigation,
                    i => i.DependentNavigation,
                    i => i.RelAppointmentSchoolingLists)
                    .Where(x => x.WorkOrderNavigation.Id == wo_id)
                    .ToList();

                if(school_search_id != null)
                {
                    hl = hl.Where(s => s.SchoolingSearchId == school_search_id).ToList();
                }

                if (dateViste != null)
                {
                    hl = hl.Where(s => (s.VisitDate != null ? s.VisitDate.Value.ToShortDateString() : Convert.ToDateTime("01/01/1900").ToShortDateString()) == dateViste.Value.ToShortDateString()).ToList();
                }

                if (status != null)
                {
                    hl = hl.Where(s => s.SchoolingStatus == status).ToList();
                }

                var custom = hl
                    .Select(s => new
                {
                    s.Id,
                    s.SchoolNo,
                    s.SchoolName,
                    s.VisitDate,
                    Grade = s.GradeNavigation == null ? "" : s.GradeNavigation.Grade,
                    s.Admision,
                    Name = s.DependentNavigation == null ? "" : s.DependentNavigation.Name,
                    s.Address,
                    s.PerMonth,
                    Currency = s.CurrencyNavigation == null ? "" : s.CurrencyNavigation.Currency,
                    Status = s.SchoolingStatusNavigation == null ? "" : s.SchoolingStatusNavigation.Status,
                    RelAppointmentSchoolingLists = s.RelAppointmentSchoolingLists.Any(),
                    SendSchool = s.SendSchool == null ? false : s.SendSchool
                    }).ToList();
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetAllSchoolByserviceid", Name = "GetAllSchoolByserviceid")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllSchoolByserviceid([FromQuery] int wo_id, int? service_id, DateTime? dateViste, int? status)
        {
            try
            {
                var hl = _schoolListRepository.GetAllIncluding(i => i.DependentNavigation,
                    u => u.CurrencyNavigation,
                    s => s.SchoolingStatusNavigation,
                    sr => sr.WorkOrderNavigation,
                    c => c.GradeNavigation,
                    i => i.DependentNavigation,
                    i => i.RelAppointmentSchoolingLists)
                    .Where(x => x.WorkOrderNavigation.Id == wo_id)
                    .ToList();

                if (service_id != null)
                {
                    hl = hl.Where(s => s.SchoolingSearchId == service_id).ToList();
                }

                if (dateViste != null)
                {
                    hl = hl.Where(s => (s.VisitDate != null ? s.VisitDate.Value.ToShortDateString() : Convert.ToDateTime("01/01/1900").ToShortDateString()) == dateViste.Value.ToShortDateString()).ToList();
                }

                if (status != null)
                {
                    hl = hl.Where(s => s.SchoolingStatus == status).ToList();
                }

                var custom = hl
                    .Select(s => new
                    {
                        s.Id,
                        s.SchoolNo,
                        s.SchoolName,
                        s.VisitDate,
                        Grade = s.GradeNavigation == null ? "" : s.GradeNavigation.Grade,
                        s.Admision,
                        Name = s.DependentNavigation == null ? "" : s.DependentNavigation.Name,
                        s.Address,
                        s.PerMonth,
                        Currency = s.CurrencyNavigation == null ? "" : s.CurrencyNavigation.Currency,
                        Status = s.SchoolingStatusNavigation == null ? "" : s.SchoolingStatusNavigation.Status,
                        RelAppointmentSchoolingLists = s.RelAppointmentSchoolingLists.Any(),
                        SendSchool = s.SendSchool == null ? false : s.SendSchool
                    }).ToList();
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetAllSchoolExcel", Name = "GetAllSchoolExcel")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllSchoolExcel([FromQuery] int wo_id)
        {
            var response = new ApiResponse<string>();

            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "School" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string xFull = Path.Combine(xPath, xNombre);
                _reports.createExcelSchooling(wo_id, xFull);
                response.Result = "ok";

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
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        //Post Send School List
        [HttpPost("SendSchoolList", Name = "SendSchoolList")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult SendSchoolList([FromBody] send_schooling_obj list_obj)
        {
            var response = new ApiResponse<DepartureDetailsHomeDto>();
            try
            {

                var result = _schoolingSearchRepository.UpdateSendSchools(list_obj.list);
                var _datos_asignado = _assigneeNameRepository.Find(f => f.ServiceRecordId == list_obj.id_sr);

                if (_datos_asignado != null)
                {
                    //StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Properties.html"));
                    //string body = string.Empty;
                    //body = reader.ReadToEnd();
                    //body = body.Replace("{user}", _datos_asignado.AssigneeName);
                    //body = body.Replace("{noPropiedades}", "Number of shool to review: " + list_obj.list.Count);
                    //_userRepository.SendMail(_datos_asignado.Email, body, "New School To View");

                    StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/schools_list.html"));
                    string body = string.Empty;
                    body = reader.ReadToEnd();
                    body = body.Replace("{user}", _datos_asignado.AssigneeName);
                    var url_images = _utiltyRepository.Get_url_email_images("newmail2");
                    body = body.Replace("{url_images}", url_images);
                    body = body.Replace("{noPropiedades}", list_obj.list.Count.ToString());

                    //_userRepository.SendMail("ingarmandofranco@gmail.com", body, "New School To View");
                    _userRepository.SendMail(_datos_asignado.Email, body, "New School To View");




                }

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, response);
            }
        }

        // Delete: Schools List
        [HttpPost("DeleteSchools", Name = "DeleteSchools")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteSchools(int key)
        {
            try
            {
                var result = _schoolingSearchRepository.deleteSchool(key);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        public class send_schooling_obj
        {
            public send_schooling_obj() { }

            public List<int> list { get; set; }
            public int id_sr { get; set; }
        }
    }
}
