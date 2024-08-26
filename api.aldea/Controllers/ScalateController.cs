using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.NotificationSystem;
using api.premier.Models.Scalate;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.Scalate;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScalateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IScalateServiceRepository _scalateServiceRepository;
        private readonly IScalateCommentsRepository _scalateCommentsRepository;
        private readonly IScalateDocumentRepository _scalateDocumentRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;

        public ScalateController(IScalateServiceRepository scalateServiceRepository, IScalateCommentsRepository scalateCommentsRepository, IMapper mapper, ILoggerManager logger,
            IServiceRecordRepository serviceRecordRepository, IScalateDocumentRepository scalateDocumentRepository, IUtiltyRepository utiltyRepository,
            INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository)
        {
            _scalateServiceRepository = scalateServiceRepository;
            _scalateCommentsRepository = scalateCommentsRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _scalateDocumentRepository = scalateDocumentRepository;
            _utiltyRepository = utiltyRepository;
            _logger = logger;
            _mapper = mapper;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        [HttpPost("PostScalate", Name = "PostScalate")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ScalateServiceDto>> PostScalate([FromBody] ScalateServiceDto dto)
        {
            var response = new ApiResponse<ScalateServiceDto>();
            try
            {
                var sr = _serviceRecordRepository.Find(c => c.Id == dto.ServiceRecordId);
                if (sr != null)
                {
                    if (dto.ScalateDocuments.Any())
                    {
                        foreach(var i in dto.ScalateDocuments)
                        {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Scalate/", i.FileExtension);
                        }
                    }
                    ScalateService ss = _scalateServiceRepository.Add(_mapper.Map<ScalateService>(dto));
                    response.Result = _mapper.Map<ScalateServiceDto>(ss);
                    var data = _serviceRecordRepository.SelectCustom(sr.Id, sr.CreatedBy.Value);
                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = dto.ServiceRecordId;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = dto.UserFromId;
                    notification.UserTo = dto.UserToId;
                    notification.NotificationType = 8;
                    notification.Description = "";
                    notification.Color = "#0c7bf5";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));

                    if (dto.UserToId != null)
                        _notificationSystemRepository.SendNotificationAsync(
                            dto.UserToId.Value,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 8).Type,
                            "",
                            2
                        );
                    var users = _notificationSystemRepository
                        .GetUsers(dto.WorkOrderId.Value, dto.ServiceLineId.Value, dto.EscalationLevel.Value).ToList();
                    foreach (var user in users)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = dto.ServiceRecordId,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = dto.UserFromId,
                            UserTo = user.User,
                            NotificationType = 8,
                            Description = "",
                            Color = "#0c7bf5",
                            CreatedDate = DateTime.Now
                        });
                        _notificationSystemRepository.SendNotificationAsync(
                            user.User,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 8).Type,
                            "",
                            2
                        );
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "service record not found";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpPut("PutScalate", Name = "PutScalate")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ScalateServiceDto>> PutScalate([FromBody] ScalateServiceDto dto)
        {
            var response = new ApiResponse<ScalateServiceDto>();
            try
            {
                var sr = _scalateServiceRepository.Find(c => c.Id == dto.Id);
                if (sr != null)
                {
                    if (dto.ScalateDocuments.Any())
                    {
                        foreach (var i in dto.ScalateDocuments)
                        {
                            if(i.FileRequest.Length > 150)
                            {
                                i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Scalate/", i.FileExtension);
                            }
                        }
                    }
                    ScalateService ss = _scalateServiceRepository.Update(_mapper.Map<ScalateService>(dto), sr.Id);
                    if (dto.ScalateDocuments.Any())
                    {
                        foreach(var i in dto.ScalateDocuments)
                        {
                            if(i.Id == 0)
                            {
                                _scalateDocumentRepository.Add(_mapper.Map<ScalateDocument>(i));
                            }
                            else
                            {
                                _scalateDocumentRepository.Update(_mapper.Map<ScalateDocument>(i), i.Id);
                            }
                        }
                    }
                    if (dto.ScalateDocuments.Any())
                    {
                        foreach (var i in dto.ScalateComments)
                        {
                            if (i.Id == 0)
                            {
                                _scalateCommentsRepository.Add(_mapper.Map<ScalateComment>(i));
                            }
                            else
                            {
                                _scalateCommentsRepository.Update(_mapper.Map<ScalateComment>(i), i.Id);
                            }
                        }
                    }
                    response.Result = _mapper.Map<ScalateServiceDto>(ss);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Scalate Service not found";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpGet("GetScalate", Name = "GetScalate")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ScalateServiceDto>> GetScalate([FromQuery] int srId, [FromQuery] int user)
        {
            var response = new ApiResponse<ScalateServiceDto>();
            try
            {
                var sr = _serviceRecordRepository.Find(c => c.Id == srId);
                if (sr != null)
                {
                    var scalateService = _scalateServiceRepository.selectCustom(user, srId);
                    response.Result = _mapper.Map<ScalateServiceDto>(scalateService);
                }
                else
                {
                    response.Success = false;
                    response.Message = "service record not found";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpGet("GetScalateById", Name = "GetScalateById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ScalateServiceDto>> GetScalateById([FromQuery] int ss)
        {
            var response = new ApiResponse<ScalateServiceDto>();
            try
            {
                var scalateService = _scalateServiceRepository.Find(x => x.Id == ss);
                if (scalateService != null)
                {
                    scalateService.ScalateComments = _scalateCommentsRepository.GetAllIncluding(x => x.UserTo, a => a.UserFrom ).Where(x => x.ScalateServiceId == ss).ToList();
                    scalateService.ScalateDocuments = _scalateDocumentRepository.FindAll(x => x.ScalateServiceId == ss);
                    response.Result = _mapper.Map<ScalateServiceDto>(scalateService);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Scalate Service not found";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpPost("PostComment", Name = "PostComment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ScalateCommentDto>> PostComment([FromBody] ScalateCommentInsertDto dto)
        {
            var response = new ApiResponse<ScalateCommentDto>();
            try
            {
                var ss = _scalateServiceRepository.Find(c => c.Id == dto.ScalateServiceId);
                if (ss != null)
                {
                    ScalateComment sc = _scalateCommentsRepository.Add(_mapper.Map<ScalateComment>(dto));
                    response.Result = _mapper.Map<ScalateCommentDto>(sc);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Scalate Service not found";
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpDelete("DeleteDocument", Name = "DeleteDocument")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ScalateDocumentDto>> DeleteDocument([FromQuery] int docId)
        {
            var response = new ApiResponse<ScalateDocumentDto>();
            try
            {
                var ss = _scalateDocumentRepository.Find(c => c.Id == docId);
                if (ss != null)
                {
                    _utiltyRepository.DeleteFile(ss.FileRequest);
                    _scalateDocumentRepository.Delete(ss);
                    response.Result = null;
                    response.Message = "Scalate Document was delete.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Scalate Service not found";
                    return StatusCode(404, response);
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(200, response);
        }

        [HttpGet("GetEscalationCommunication", Name = "GetEscalationCommunication")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetEscalationCommunication([FromQuery] int ServiceLineId, [FromQuery] int sr)
        {
            try
            {
                return Ok(new { Success = true, Result = _scalateServiceRepository.GetEscalationCommunication(ServiceLineId ,sr) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetEscalationCommunicationById", Name = "GetEscalationCommunicationById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetEscalationCommunicationById([FromQuery] int id)
        {
            try
            {
                return Ok(new { Success = true, Result = _scalateServiceRepository.GetEscalationCommunicationById(id) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
