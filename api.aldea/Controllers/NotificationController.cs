using api.premier.ActionFilter;
using AutoMapper;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using biz.premier.Repository;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IUserRepository _userRepository;

        public NotificationController(IMapper mapper, ILoggerManager loggerManager, INotificationSystemRepository notificationSystemRepository, IUtiltyRepository utiltyRepository,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _notificationSystemRepository = notificationSystemRepository;
            _utiltyRepository = utiltyRepository;
            _userRepository = userRepository;
        }
        //Get Notification Center
        [HttpGet("GetNotificationCenter/{user}", Name = "GetNotificationCenter/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetNotificationCenter(int user, [FromQuery] DateTime? dateRange1, [FromQuery] DateTime? dateRange2, [FromQuery] int? notificationType, [FromQuery] int? serviceRecord, [FromQuery] bool? archive)
        {
            try
            {
                var conversation = _notificationSystemRepository.GetAllCustom(user ,dateRange1, dateRange2, notificationType, serviceRecord, archive);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Get Notification Center UnRead
        [HttpGet("GetNotificationCenterUnRead/{user}", Name = "GetNotificationCenterUnRead/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetNotificationCenterUnRead(int user, [FromQuery] DateTime? dateRange1, [FromQuery] DateTime? dateRange2, [FromQuery] int? notificationType, [FromQuery] int? serviceRecord, [FromQuery] bool? archive)
        {
            try
            {
                var conversation = _notificationSystemRepository.GetAllCustomUnRead(user, dateRange1, dateRange2, notificationType, serviceRecord, archive);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Get Notification Center App
        [HttpGet("GetNotificationCenterApp/{user}", Name = "GetNotificationCenterApp/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetNotificationCenterApp(int user)
        {
            try
            {
                var conversation = _notificationSystemRepository.GetAllCustomApp(user);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Put Notification Center
        [HttpPut("PutArchive/{notification}/{archive}", Name = "PutArchive/{notification}/{archive}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult PutArchive(int notification, bool archive)
        {
            try
            {
                var findNotification = _notificationSystemRepository.Find(x => x.Id == notification);
                findNotification.Archive = archive;
                var conversation = _notificationSystemRepository.Update(findNotification, notification);
                return Ok(new { Success = true, Message = "Notification Archived.", Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Put Notification Center
        [HttpPut("PutViewed/{notification}/{viewed}", Name = "PutViewed/{notification}/{viewed}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult PutViewed(int notification, bool viewed)
        {
            try
            {
                var findNotification = _notificationSystemRepository.Find(x => x.Id == notification);
                findNotification.View = viewed;
                var conversation = _notificationSystemRepository.Update(findNotification, notification);
                return Ok(new { Success = true, Message = "Notification Viewed.", Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Get User List To Conversation
        [HttpGet("GetNotifications/{user}", Name = "GetNotifications/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetNotifications(int user)
        {
            try
            {
                var conversation = _notificationSystemRepository.GetNotifications(user);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Delete Notification
        [HttpGet("DeleteNotifications/{id}", Name = "DeleteNotifications/{id}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteNotifications(int id)
        {
            try
            {
                var conversation = _notificationSystemRepository.DeleteNotificationById(id);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //PUT Activate Or deactivated Push Notifications
        [HttpGet("Push-Notifications/{user}/{activate}", Name = "Push-Notifications/{user}/{activate}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> GetNotifications(int user, bool activate)
        {
            try
            {
                var userData = _userRepository.Find(x => x.Id == user);
                userData.Push = activate;
                await _userRepository.UpdateAsyn(userData, userData.Id);
                return Ok(new { Success = true, Message = "Operation was Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

    }
}
