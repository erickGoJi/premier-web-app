using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.Follow;
using api.premier.Models.NotificationSystem;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Follow;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IFollowRepository _followRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;

        public FollowController(IMapper mapper,
            ILoggerManager logger,
            IFollowRepository followRepository,
            IServiceRecordRepository serviceRecordRepository,
            INotificationSystemRepository notificationSystemRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _followRepository = followRepository;
            _serviceRecordRepository = serviceRecordRepository; 
            _notificationSystemRepository = notificationSystemRepository;
        }

        [HttpPost("CreateFollow", Name = "CreateFollow")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<FollowDto>> CreateFollow([FromBody] FollowDto dto)
        {
            var response = new ApiResponse<FollowDto>();
            try
            {
                var service = _serviceRecordRepository.Find(c => c.Id == dto.ServiceRecordId);
                if (service != null)
                {
                    Follow follow = _followRepository.Add(_mapper.Map<Follow>(dto));
                    response.Result = _mapper.Map<FollowDto>(follow);

                    // NotificationSystemDto notification = new NotificationSystemDto();
                    // notification.Archive = false;
                    // notification.View = false;
                    // notification.ServiceRecord = dto.ServiceRecordId;
                    // notification.Time = DateTime.Now.TimeOfDay;
                    // notification.UserFrom = dto.UserId;
                    // notification.UserTo = dto.UserId;
                    // notification.NotificationType = 5;
                    // notification.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. Integer aliquam iaculis nisl et imperdiet.";
                    // notification.Color = "#0c7bf5";
                    // notification.CreatedDate = DateTime.Now;
                    // _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
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

        [HttpDelete("DeleteFollow", Name = "DeleteFollow")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteFollow([FromQuery] int id)
        {
            try
            {
                var follow = _followRepository.Find(c => c.Id == id);
                if (follow != null)
                {
                    _followRepository.Delete(follow);
                    // NotificationSystemDto notification = new NotificationSystemDto();
                    // notification.Archive = false;
                    // notification.View = false;
                    // notification.ServiceRecord = follow.ServiceRecordId;
                    // notification.Time = DateTime.Now.TimeOfDay;
                    // notification.UserFrom = follow.UserId;
                    // notification.UserTo = follow.UserId;
                    // notification.NotificationType = 6;
                    // notification.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. Integer aliquam iaculis nisl et imperdiet.";
                    // notification.Color = "#0c7bf5";
                    // notification.CreatedDate = DateTime.Now;
                    // _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    return Ok(new { Success = true, Result = "Follow record Delete Success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Follow Record Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Follow Record Not Found", Message = ex.ToString() });
            }
        }
    }
}
