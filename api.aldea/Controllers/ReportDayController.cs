using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ReportDay;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.ReportDay;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDayController : ControllerBase
    {
        public class ReportSummary
        {
            public string Service { get; set; }
            public string ServiceId { get; set; }
            public int TimeRemining { get; set; }
            public string Saved { get; set; }
        }
        public class Summary
        {
            public List<string> services { get; set; }
            public List<string> serviceIds { get; set; }
            public string TimeService { get; set; }
            public List<ReportSummary> reportSummaries { get; set; }
        }
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IReportDayRepository _reportDayRepository;
        private readonly IServiceReportDayRepository _serviceReportDayRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        private readonly IUtiltyRepository _utilityRepository;
        
        public ReportDayController(IMapper mapper, ILoggerManager loggerManager, 
            IReportDayRepository reportDayRepository,
            IServiceReportDayRepository serviceReportDayRepository,
            INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository,
            IUtiltyRepository utilityRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _reportDayRepository = reportDayRepository;
            _serviceReportDayRepository = serviceReportDayRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
            _utilityRepository = utilityRepository;
        }

        [HttpGet("createExcelReportDay", Name = "createExcelReportDay")]
        public ActionResult<ApiResponse<string>> createExcelReportDay(int sr)
        {
            var response = new ApiResponse<string>();

            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "Activity_Reports" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string xFull = Path.Combine(xPath, xNombre);
                _reportDayRepository.createExcelReportDay(sr, xFull);
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
                response.Message = "Internal server error";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);

        }

        [HttpGet("GetTimeRemaindingPublic", Name = "GetTimeRemaindingPublic")]
        public ActionResult GetTimeRemaindingPublic(int service)
        {
            try
            {
                return Ok(new { Success = true, Result = _reportDayRepository.GetTimeRemaindingPublic(service) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        [HttpGet("GetReportNo", Name = "GetReportNo")]
        public ActionResult GetReportNo(int sr)
        {
            try
            {
                var reportNo = _reportDayRepository.CountByServiceRecord(sr);
                reportNo = reportNo + 1;
                return Ok(new { Success = true, Result = reportNo });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        [HttpPost("PostReportDay", Name = "PostReportDay")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<Summary>> PostReportDay([FromBody] ReportDayDto dto)
        {
            var response = new ApiResponse<Summary>();
            try
            {
                //var _service = _reportDayRepository.FindBy(x => x.ServiceReportDays.FirstOrDefault().Service == dto.ServiceReportDays.FirstOrDefault().Service);
               
                //if(_service.Count() > 0)
                //{
                //    dto.ServiceReportDays.FirstOrDefault().TimeReminder = Convert.ToInt32(dto.TotalTime) - Convert.ToInt32(dto.ServiceReportDays.LastOrDefault().TimeReminder);
                //}
                //else
                //{
                //    dto.ServiceReportDays.FirstOrDefault().TimeReminder = Convert.ToInt32(dto.TotalTime) - Convert.ToInt32(dto.ServiceReportDays.FirstOrDefault().Time);
                //}
                
                ReportDay rd = _reportDayRepository.Add(_mapper.Map<ReportDay>(dto));
                var data = _notificationSystemRepository.GetCoordinator(dto.WorkOrder.Value);
                var data1 = _notificationSystemRepository.GetCoordinatorSendNotificationTimeReminder(dto.WorkOrder.Value, dto.ServiceLine.Value);

                Summary summary = new Summary();
                summary.serviceIds = new List<string>();
                summary.services = new List<string>();
                summary.TimeService = "";
                summary.reportSummaries = new List<ReportSummary>();
               
                foreach(var i in dto.ServiceReportDays)
                {
                    if(i.TimeReminder <= 4)
                    {
                        if(dto.ServiceLine == 2)
                        {
                            foreach(var j in data1.RelocationCoordinators)
                            {
                                _notificationSystemRepository.Add(new NotificationSystem()
                                {
                                    Id = 0,
                                    Archive = false,
                                    View = false,
                                    ServiceRecord = data1.Id,
                                    Time = DateTime.Now.TimeOfDay,
                                    UserFrom = dto.CreatedBy,
                                    UserTo = j.Coordinator.UserId,
                                    NotificationType = 15,
                                    Description = i.ServiceName + " service within the " + "SR-" + j.Id + " may need extra time",
                                    Color = "#f06689",
                                    CreatedDate = DateTime.Now
                                });
                                _notificationSystemRepository.SendNotificationAsync(
                                    data.Item2,
                                    0,
                                    0,
                                    i.ServiceName + " service within the " + "SR-" + j.Id + " may need extra time",
                                    "",
                                    15
                                );
                            }
                        }
                    }
                }

                foreach(var i in rd.ServiceReportDays)
                {
                    ReportSummary reportSummary = new ReportSummary();
                    reportSummary = _mapper.Map<ReportSummary>(_reportDayRepository.GetTimeRemaining(i.Service.Value, dto.WorkOrder.Value));
                    var reportDays = _reportDayRepository
                        .GetAllIncluding(x => x.ServiceReportDays)
                        .Where(x => x.WorkOrder == dto.WorkOrder && x.ServiceReportDays.Select(s => s.Service.Value).Contains(i.Service.Value))
                        .Select(s => s.ServiceReportDays.Select(s => Convert.ToInt32(s.Time)).Sum() ).ToList();
                    var totalTime = reportDays.Sum(s => s);
                    reportSummary.TimeRemining = Math.Abs(reportSummary.TimeRemining - totalTime);
                    if(reportSummary.TimeRemining < 0)
                    {
                        //summary.TimeService += $"The Time of the service {reportSummary.Service} with the Service ID { reportSummary.ServiceId } exceeds the Autho Time.";
                        summary.services.Add(reportSummary.Service);
                        summary.serviceIds.Add(reportSummary.ServiceId);
                    }
                    summary.reportSummaries.Add(reportSummary);

                }

                var message = _notificationSystemTypeRepository.Find(x => x.Id == 16).Type;
                
                if (data.Item1 != 0 && data.Item2 != 0)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Id = 0,
                        Archive = false,
                        View = false,
                        ServiceRecord = data.Item1,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = dto.CreatedBy,
                        UserTo = data.Item2,
                        NotificationType = 16,
                        Description = "The Supplier assigned to the SR has created a report, please follow up",
                        Color = "#f06689",
                        CreatedDate = DateTime.Now
                    });
                    _notificationSystemRepository.SendNotificationAsync(
                        data.Item2,
                        0,
                        0,
                        "The Supplier assigned to the SR has created a report, please follow up",
                        "",
                        16
                    );
                }
                
                response.Result = _mapper.Map<Summary>(summary);
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

        [HttpPost("Add/Conclusion", Name = "Conclusion")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ConclusionServiceReportDayDto>> AddConclusion([FromBody] ConclusionServiceReportDayDto dto)
        {
            var response = new ApiResponse<ConclusionServiceReportDayDto>();
            try
            {
                dto.FilePath =
                    _utilityRepository.UploadImageBase64(dto.FilePath, "Files/Conclusion/", dto.FileExtension);
                ConclusionServiceReportDay day =
                    _reportDayRepository.AddConclusion(_mapper.Map<ConclusionServiceReportDay>(dto));
                response.Result = _mapper.Map<ConclusionServiceReportDayDto>(day);
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
        
        [HttpDelete("Delete/Conclusion", Name = "DeleteConclusion")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteConclusion([FromQuery] int key)
        {
            try
            {
                var report = _reportDayRepository.DeleteConclusion(key);
                if (report)
                {
                    return Ok(new { Success = true, Result = "Conclusion was delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Report Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpPut("PutReportDay", Name = "PutReportDay")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReportDayDto>> PutReportDay([FromBody] ReportDayDto dto)
        {
            var response = new ApiResponse<ReportDayDto>();
            try
            {
                ReportDay rd = _reportDayRepository.UpdateCustom(_mapper.Map<ReportDay>(dto), dto.Id);

                response.Result = _mapper.Map<ReportDayDto>(rd);
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

        [HttpPut("PutServiceReportDay", Name = "PutServiceReportDay")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ServiceReportDayDto>> PutServiceReportDay([FromBody] ServiceReportDayDto dto)
        {
            var response = new ApiResponse<ServiceReportDayDto>();
            try
            {
                ReportDay rd = _reportDayRepository.Update(_mapper.Map<ReportDay>(dto), dto.Id);

                response.Result = _mapper.Map<ServiceReportDayDto>(rd);
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

        // Delete Report Day
        [HttpGet("DeleteReportDay", Name = "DeleteReportDay")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteReportDay([FromQuery] ReportDay dto)
        {
            try
            {
                var report = _reportDayRepository.Find(c => c.Id == dto.Id);
                if (report != null)
                {
                    _reportDayRepository.Delete(_mapper.Map<ReportDay>(report));
                    return Ok(new { Success = true, Result = "Report delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Report Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetReportDayById", Name = "GetReportDayById")]
        public ActionResult<ApiResponse<ReportDay>> GetreportDayById([FromQuery] int id)
        {
            var response = new ApiResponse<ReportDay>();
            try
            {
                //ReportDayByIdDto _reportDayByIdDto = new ReportDayByIdDto();
                var consult = _reportDayRepository.GetAllIncluding(
                    x => x.ServiceReportDays, 
                    y => y.ReportByNavigation
                ).FirstOrDefault(x => x.Id == id);

                if(consult.ServiceReportDays != null)
                {
                    consult.ServiceReportDays = consult?.ServiceReportDays.Select(x => new ServiceReportDay()
                    {
                        Id = x.Id,
                        ReportDayId = x.ReportDayId,
                        Service = x.Service,
                        Time = x?.Time?.ToString(),
                        TimeReminder = x?.TimeReminder,
                        Comments = x.Comments,
                        CreatedBy = x.CreatedBy,
                        CreatedDate = x.CreatedDate,
                        UpdateBy = Convert.ToInt32(x?.Time),
                        UpdatedDate = x?.UpdatedDate
                    }).ToList();
                }

                //_reportDayByIdDto.Id = Convert.ToInt32(consult.ServiceReportDays.FirstOrDefault().Time);
                //var map = consult.FirstOrDefault(x => x.Id == id);
                response.Result = consult;
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

        [HttpGet("GetReportDayByIdApp", Name = "GetReportDayByIdApp")]
        public ActionResult<ApiResponse<ReportDayDto>> GetReportDayByIdApp([FromQuery] int id)
        {
            var response = new ApiResponse<ReportDayDto>();
            try
            {
                var consult = _reportDayRepository.FindBy(x => x.Id == id);

                //if (consult.ServiceReportDays != null)
                //{
                //    consult.ServiceReportDays = consult?.ServiceReportDays.Select(x => new ServiceReportDay()
                //    {
                //        Id = x.Id,
                //        ReportDayId = x.ReportDayId,
                //        Service = x.Service,
                //        Time = x?.Time?.ToString(),
                //        TimeReminder = x?.TimeReminder,
                //        Comments = x.Comments,
                //        CreatedBy = x.CreatedBy,
                //        CreatedDate = x.CreatedDate,
                //        UpdateBy = Convert.ToInt32(x?.Time),
                //        UpdatedDate = x?.UpdatedDate
                //    }).ToList();
                //}

                response.Result = _mapper.Map<ReportDayDto>(consult); ;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet("GetActivityReports", Name = "GetActivityReports")]
        public ActionResult GetServiceRecord(int sr, int? serviceLine, int? program, DateTime? initialReportDate, DateTime? finalReportDate, int? totalTimeAuthorized,
            int? timeRemaining)
        {
            try
            {
                //var map = _reportDayRepository.GetAllIncluding(x => x.ServiceReportDays, y => y.WorkOrderNavigation, z => z.ServiceLineNavigation, w => w.ReportByNavigation,
                //    o => o.WorkOrderNavigation);
                var map = _reportDayRepository.GetReportDay(sr, serviceLine, program, initialReportDate, finalReportDate, totalTimeAuthorized);
                var view = map.Select(s => new
                {
                    s.Id,
                    reprortedBy = s.ReportByNavigation.Name + " " + s.ReportByNavigation.LastName + " " + s.ReportByNavigation.MotherLastName,
                    s.CreationDate,
                    s.ReportDate,
                    s.ServiceLineNavigation.ServiceLine,
                    s.WorkOrderNavigation.NumberWorkOrder,
                    type = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any() ? "Stand Alone" : "Bundled",
                    s.WorkOrderNavigation.ServiceRecord.PartnerId,
                    services = s.ServiceReportDays.Select(c => new
                    {
                        c.Id,
                        Service = _reportDayRepository.GetServiceNameByWorOrder(c.Service.Value, s.WorkOrderNavigation.ServiceRecord.ClientId),
                        c.TimeReminder,
                        c.Time,
                        tipo = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any(d => d.Id == c.Service.Value) ? "standalone" : "bundle",
                        authoTime = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any(d => d.Id == c.Service.Value) 
                        ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.FirstOrDefault(d => d.Id == c.Service.Value).AuthoTime.Value
                        : Convert.ToInt32(s.WorkOrderNavigation.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault(d => d.Id == c.Service.Value).BundledServiceOrder.TotalTime)
                    }).ToList(),
                    s.TotalTime,
                    country = s.ReportByNavigation.ProfileUsers.FirstOrDefault(x => x.UserId == s.ReportBy).CountryNavigation.Name,
                    Remaining = Math.Abs(Convert.ToInt32(s.ServiceReportDays.Where(x => x.ReportDayId == s.Id).Count() > 1 ?
                    Convert.ToInt32(s.ServiceReportDays.Where(x => x.ReportDayId == s.Id).Sum(x => x.TimeReminder)) 
                    : Convert.ToInt32(s.ServiceReportDays.LastOrDefault(x => x.ReportDayId == s.Id).TimeReminder)))
                    }).OrderByDescending(x => x.CreationDate).ToList();
                if (timeRemaining.HasValue)
                   view = view.Where(x => Convert.ToInt32(x.Remaining) >=  timeRemaining.Value ).ToList();
                return Ok(new { Success = true, view });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetActivityReportsApp", Name = "GetActivityReportsApp")]
        public ActionResult GetActivityReportsApp(int sr)
        {
            try
            {
                var map = _reportDayRepository.GetReportDay(sr, null, null, null, null, null);
                var view = map.Select(s => new
                {
                    s.Id,
                    s.ReportByNavigation.ProfileUsers.FirstOrDefault().Photo,
                    reprortedBy = s.ReportByNavigation.Name + " " + s.ReportByNavigation.LastName + " " + s.ReportByNavigation.MotherLastName,
                    s.CreationDate,
                    ServiceNumber = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any() ? s.WorkOrderNavigation.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber :
                    s.WorkOrderNavigation.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().ServiceNumber,
                    s.ReportDate,
                    s.ServiceLineNavigation.ServiceLine,
                    s.WorkOrderNavigation.NumberWorkOrder,
                    type = s.WorkOrderNavigation.StandaloneServiceWorkOrders.Any() ? "Stand Alone" : "Bundled",
                    s.WorkOrderNavigation.ServiceRecord.PartnerId,
                    services = s.ServiceReportDays.Count(),
                    s.TotalTime,
                    time = Convert.ToInt32(s.ServiceReportDays.FirstOrDefault(x => x.ReportDayId == s.Id).Time),
                    country = s.ReportByNavigation.ProfileUsers.FirstOrDefault(x => x.UserId == s.ReportBy).CountryNavigation.Name,
                    city = s.ReportByNavigation.ProfileUsers.FirstOrDefault(x => x.UserId == s.ReportBy)?.CityNavigation?.City,
                    Remaining = Math.Abs(Convert.ToInt32(s.ServiceReportDays.Where(x => x.ReportDayId == s.Id).Count() > 1 ?
                    Convert.ToInt32(s.ServiceReportDays.Where(x => x.ReportDayId == s.Id).Sum(x => x.TimeReminder))
                    : Convert.ToInt32(s.ServiceReportDays.LastOrDefault(x => x.ReportDayId == s.Id).TimeReminder)))
                }).OrderByDescending(x => x.CreationDate).ToList();

                return Ok(new { Success = true, view });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetTotalesActivityReports", Name = "GetTotalesActivityReports")]
        public ActionResult GetTotalesActivityReports(int sr)
        {
            try
            {
                var view = _reportDayRepository.GetTotalesActivityReports(sr);
                return Ok(new { Success = true, view });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetActivityReportsByService", Name = "GetActivityReportsByService")]
        public ActionResult GetActivityReportsByService(int service, int workorder)
        {
            try
            {
                var view = _reportDayRepository.GetActivityReportsByService(service, workorder);
                return Ok(new { Success = true, view });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        private string TimeRemaining(int? workOrder, int[] services, int totalTime, int sr)
        {
            int total = 0;
            List<ReportSummary> summaries = new List<ReportSummary>();
            foreach (var i in services)
            {
                var summary = _reportDayRepository.GetTimeRemaining(i, workOrder.Value);
                summaries.Add(new ReportSummary()
                {
                    Saved = summary.Saved,
                    Service = summary.Service,
                    ServiceId = summary.ServiceId,
                    TimeRemining = summary.TimeRemining
                });
            }
            var res = _reportDayRepository.GetTimeRemaining(services[0], workOrder.Value);
            var data = _reportDayRepository.GetReportDay(sr, null, null, null, null,
                null);
            total = summaries.Sum(s=>s.TimeRemining);
            int timeUsed = data.Where(x => x.WorkOrder == workOrder).Sum(s => Convert.ToInt32(s.TotalTime));
            return Math.Abs(total).ToString();
        } 

        [HttpGet("GetSupplierPartnersRecord", Name = "GetSupplierPartnersRecord")]
        public ActionResult GetSupplierPartnersRecord(int sr)
        {
            try
            {
                var map = _reportDayRepository.GetAllIncluding(x => x.ServiceReportDays, y => y.WorkOrderNavigation, z => z.ServiceLineNavigation, w => w.ReportByNavigation,
                    o => o.WorkOrderNavigation);
                var view = map.Where(x => x.WorkOrderNavigation.ServiceRecordId == sr).Select(s => new
                {
                    s.Id,
                    s.CreationDate,
                    s.ServiceLineNavigation.ServiceLine,
                    s.WorkOrderNavigation.NumberWorkOrder,
                    services = s.ServiceReportDays.Count,
                    s.TotalTime
                }).ToList();
                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("ReportsHistory", Name = "ReportsHistory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ReportDayDto>>> GetReportsHistory(int sr, int? serviceLine, int? program, DateTime? initialReportDate, DateTime? finalReportDate
            , int? totalTimeAuthorized, int? service)
        {
            var response = new ApiResponse<List<ReportDayDto>>();
            try
            {
                string serviceLineName = "";
                var map = _mapper.Map<List<ReportDayDto>>(_reportDayRepository.GetReportDay(sr, serviceLine, program, initialReportDate, finalReportDate, totalTimeAuthorized));
                foreach (var reportDay in map)
                {
                    foreach (var day in reportDay.ServiceReportDays)
                    {
                        var tuple = _reportDayRepository.GetNumber(reportDay.WorkOrder.Value, day.Service.Value);
                        day.ServiceName = tuple.Item2;
                        serviceLineName = tuple.Item1;
                        day.Category = _reportDayRepository.GetCategory(reportDay.WorkOrder.Value, day.Service.Value);
                    }

                    reportDay.ServiceLineName = serviceLineName;
                    if (service.HasValue)
                    {
                        reportDay.ServiceReportDays = reportDay.ServiceReportDays.Where(x => x.Category == service.Value).ToList();
                    }
                }
                
                if (service.HasValue)
                {
                    map = map.Where(x => x.ServiceReportDays.Any()).ToList();
                }
                
                response.Result = _mapper.Map<List<ReportDayDto>>(map);
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

    }
}
