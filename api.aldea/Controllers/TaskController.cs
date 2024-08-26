using System;
using System.Collections.Generic;
using System.Linq;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.NotificationSystem;
using api.premier.Models.Task;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Task;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskDocumentRepository _taskDocumentRepository;
        private readonly ITaskReplyRepository _taskReplyRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        public TaskController(
        IMapper mapper,
        ILoggerManager logger,
        ITaskRepository taskRepository,
        IUtiltyRepository utiltyRepository,
        ITaskDocumentRepository taskDocumentRepository,
        ITaskReplyRepository taskReplyRepository,
        IServiceRecordRepository serviceRecordRepository,
        INotificationSystemRepository notificationSystemRepository, 
        ICatNotificationSystemTypeRepository notificationSystemTypeRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _taskRepository = taskRepository;
            _taskDocumentRepository = taskDocumentRepository;
            _taskReplyRepository = taskReplyRepository;
            _utiltyRepository = utiltyRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        // Post Create a new Task
        [HttpPost("CreateTask", Name = "CreateTask")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<TaskDto>>> PostTask([FromBody] TaskDto dto)
        {
            var response = new ApiResponse<List<TaskDto>>();
            try
            {
                foreach (var j in dto.TaskDocuments)
                {
                    j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Task/", j.FileExtension);
                }
                List<TaskDto> task = new List<TaskDto>();
                foreach (var i in dto.to)
                {
                    dto.TaskTo = i;
                    task.Add(_mapper.Map<TaskDto>(_taskRepository.Add(_mapper.Map<Task>(dto))));
                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = dto.ServiceRecordId;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = dto.TaskFrom;
                    notification.UserTo = dto.TaskTo;
                    notification.NotificationType = 14;
                    notification.Description = "";
                    notification.Color = "#ef6c00";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    _notificationSystemRepository.SendNotificationAsync(
                        dto.TaskTo,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 14).Type,
                        "",
                        2
                    );
                }
                response.Success = true;
                response.Message = "Success";
                response.Result = task;
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

        // Put Update Task
        [HttpPut("UpdateTask", Name = "UpdateTask")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TaskDto>> PutTask([FromBody] TaskDto dto)
        {
            var response = new ApiResponse<TaskDto>();
            try
            {
                foreach (var j in dto.TaskDocuments)
                {
                    j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Task/", j.FileExtension);
                }
                //_mapper.Map<TaskDocumentDto>(_taskRepository.Add(_mapper.Map<TaskDocument>(dto.TaskDocuments)));
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<TaskDto>(_taskRepository.UpdateCustom(_mapper.Map<Task>(dto)));
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

        // Post Create a new Task Document
        [HttpPost("CreateTaskDocument", Name = "CreateTaskDocument")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TaskDocumentDto>> PostTaskDocument([FromBody] TaskDocumentDto dto)
        {
            var response = new ApiResponse<TaskDocumentDto>();
            try
            {
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/Task/", dto.FileExtension);

                TaskDocument dwp = _taskDocumentRepository.Add(_mapper.Map<TaskDocument>(dto));
                response.Success = true;
                response.Message = "Success";

                response.Result = _mapper.Map<TaskDocumentDto>(dwp);
                //_mapper.Map<TaskDto>(_taskRepository.Add(_mapper.Map<Task>(dto)));
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

        // Post Create a new Task Reply
        [HttpPost("CreateTaskReply", Name = "CreateTaskReply")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TaskReplyDto>> PostTaskReply([FromBody] TaskReplyDto dto)
        {
            var response = new ApiResponse<TaskReplyDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<TaskReplyDto>(_taskReplyRepository.Add(_mapper.Map<TaskReply>(dto)));
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

        // Get List All Task
        [HttpGet("GetAllTask", Name = "GetAllTask")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TaskDto>> GetAllTask([FromQuery] int service_record_id, int service_line_id)
        {
            var response = new ApiResponse<TaskDto>();
            try
            {
                return Ok(new { Success = true, Result = _taskRepository.GetAllTask(service_record_id, service_line_id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        // Get List Task By Id
        [HttpGet("GetTaskById", Name = "GetTaskById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TaskSelectDto>> GetTaskById([FromQuery] int Id)
        {
            var response = new ApiResponse<TaskSelectDto>();
            try
            {
                var task = _taskRepository.GetCustom(Id);
                var dataResult = _mapper.Map<TaskSelectDto>(_taskRepository.GetCustom(Id));
                if (!task.Department.HasValue)
                {
                    var service = task.WorkOrder.BundledServicesWorkOrders.Any(x=>x.BundledServices.Select(d => d.Id).Contains(task.ServiceId.Value))
                        ? task.WorkOrder.BundledServicesWorkOrders
                            .FirstOrDefault(x => x.BundledServices.Select(d => d.Id).Contains(task.ServiceId.Value))
                            .BundledServices.Where(x => x.WorkServicesId == task.ServiceId.Value)
                            .Select(s => s.ServiceNumber)
                            .FirstOrDefault()
                        : task.WorkOrder.StandaloneServiceWorkOrders
                            .FirstOrDefault(x => x.WorkOrderServiceId == task.ServiceId)
                            ?.ServiceNumber;
                    dataResult.ServiceName = service;
                }

                response.Success = true;
                response.Message = "Success";
                response.Result = dataResult;
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

        // Delete document
        [HttpGet("DeleteTask", Name = "DeleteTask")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteTask([FromQuery] TaskDto dto)
        {
            try
            {
                var task = _taskDocumentRepository.Find(c => c.Id == dto.Id);
                if (task != null)
                {
                    _taskRepository.Delete(_mapper.Map<Task>(task));
                    return Ok(new { Success = true, Result = "Task delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Task Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        // Delete document
        [HttpGet("DeleteDocumentTask", Name = "DeleteDocumentTask")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentTask([FromQuery] int id)
        {
            try
            {
                var document = _taskDocumentRepository.Find(c => c.Id == id);
                if (document != null)
                {
                    _taskDocumentRepository.Delete(_mapper.Map<TaskDocument>(document));
                    return Ok(new { Success = true, Result = "Document delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Document Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetActionItems/{user}/", Name = "GetActionItems/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetActionItems(int user, [FromQuery] DateTime? rangeDate1, [FromQuery] DateTime? rangeDate2, [FromQuery] bool? asignedToMeOrByMe, [FromQuery] int? serviceLine)
        {
            try
            {
                return Ok(new { Success = true, Result = _taskRepository.ActionItems(user, rangeDate1, rangeDate2, asignedToMeOrByMe, serviceLine) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Something went wrong", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAllActions/{user}/", Name = "GetAllActions/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllActions(int user, [FromQuery] DateTime? rangeDate1, [FromQuery] DateTime? rangeDate2, [FromQuery] int? sr, [FromQuery] int? status, [FromQuery] int? serviceLine)
        {
            try
            {
                return Ok(new { Success = true, Result = _taskRepository.AllActions(user, sr, status, rangeDate1, rangeDate2, serviceLine) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Something went wrong", Message = ex.ToString() });
            }
        }
    }
}
