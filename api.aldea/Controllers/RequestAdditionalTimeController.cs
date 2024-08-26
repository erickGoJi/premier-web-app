using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.NotificationSystem;
using api.premier.Models.RequestAdditionalTime;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.RequestAdditionalTime;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestAdditionalTimeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IRequestAdditionalTimeRepository _requestAdditionalTimeRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        public RequestAdditionalTimeController(IRequestAdditionalTimeRepository requestAdditionalTimeRepository, IMapper mapper, ILoggerManager loggerManager,
            INotificationSystemRepository notificationSystemRepository, 
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository) 
        {
            _requestAdditionalTimeRepository = requestAdditionalTimeRepository;
            _mapper = mapper;
            _logger = loggerManager;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        [HttpPost("PostRequestAdditionalTime", Name = "PostRequestAdditionalTime")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<RequestAdditionalTimeDto>>> PostRequestAdditionalTime([FromBody] List<RequestAdditionalTimeDto> dto)
        {
            var response = new ApiResponse<List<RequestAdditionalTimeDto>>();
            try
            {
                List<RequestAdditionalTime> request = new List<RequestAdditionalTime>();
                var isValidate = _requestAdditionalTimeRepository.ValidateAdditionalTime(_mapper.Map<List<RequestAdditionalTime>>(dto));
                if (isValidate.Item1)
                {
                    foreach (var i in dto)
                    {
                        request.Add(_requestAdditionalTimeRepository.Add(_mapper.Map<RequestAdditionalTime>(i)));
                        
                        var coordinator = _notificationSystemRepository.GetCoordinator(i.WorkOrder.Value);
                        if (coordinator.Item1 != 0) {
                            NotificationSystemDto notification = new NotificationSystemDto();
                            notification.Archive = false;
                            notification.View = false;
                            notification.ServiceRecord = coordinator.Item1;
                            notification.Time = DateTime.Now.TimeOfDay;
                            notification.UserFrom = i.CreatedBy;
                            notification.UserTo = coordinator.Item2;
                            notification.NotificationType = 15;
                            notification.Description = "";
                            notification.Color = "#00838f";
                            notification.CreatedDate = DateTime.Now;
                            _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                            _notificationSystemRepository.SendNotificationAsync(
                                coordinator.Item2,
                                0,
                                0,
                                _notificationSystemTypeRepository.Find(x => x.Id == 15).Type,
                                "",
                                2
                            );
                        }
                        
                    }
                    response.Result = _mapper.Map<List<RequestAdditionalTimeDto>>(request);
                }
                else
                {
                    response.Result = null;
                    response.Success = false;
                    response.Message = isValidate.Item2;
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

        [HttpPut("PutRequestAdditionalTime", Name = "PutRequestAdditionalTime")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RequestAdditionalTimeDto>> PutRequestAdditionalTime([FromBody] RequestAdditionalTimeDto dto)
        {
            var response = new ApiResponse<RequestAdditionalTimeDto>();
            try
            {
                RequestAdditionalTime rd = _requestAdditionalTimeRepository.Update(_mapper.Map<RequestAdditionalTime>(dto), dto.Id);

                response.Result = _mapper.Map<RequestAdditionalTimeDto>(rd);
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
