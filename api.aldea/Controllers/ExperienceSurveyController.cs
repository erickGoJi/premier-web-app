using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ExperienceSurvey;
using api.premier.Models.NotificationSystem;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.ExperienceSurvey;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceSurveyController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IExperienceSurveyRepository _experienceSurveyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationTypeRepository;

        public ExperienceSurveyController(IMapper mapper, ILoggerManager loggerManager, IExperienceSurveyRepository experienceSurveyRepository,
            INotificationSystemRepository notificationSystemRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _experienceSurveyRepository = experienceSurveyRepository;
            _notificationSystemRepository = notificationSystemRepository;
        }
        
        //Post Create a new Experience Survey
        [HttpPost("CreateExperienceSurvey", Name = "CreateExperienceSurvey")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ExperienceSurveyDto>> CreateExperienceSurvey([FromBody] ExperienceSurveyDto dto)
        {
            var response = new ApiResponse<ExperienceSurveyDto>();
            try
            {
                ExperienceSurvey experienceSurvey = _experienceSurveyRepository.Add(_mapper.Map<ExperienceSurvey>(dto));
                response.Result = _mapper.Map<ExperienceSurveyDto>(experienceSurvey);
                if (!_experienceSurveyRepository.CompleteSrByServiceLine(experienceSurvey.ServiceRecord.Value,
                    experienceSurvey.CompleteServiceRecord.Value, experienceSurvey.ServiceLine.Value))
                {
                    NotificationSystemDto notification = new NotificationSystemDto();
                    notification.Archive = false;
                    notification.View = false;
                    notification.ServiceRecord = experienceSurvey.ServiceRecord;
                    notification.Time = DateTime.Now.TimeOfDay;
                    notification.UserFrom = experienceSurvey.CreatedBy.Value;
                    notification.UserTo = _experienceSurveyRepository.ReturnCoordinator(experienceSurvey.ServiceRecord.Value, experienceSurvey.ServiceLine.Value);
                    notification.NotificationType = 17;
                    notification.Description = _experienceSurveyRepository.ReturnService(experienceSurvey.WorkOrderServicesId.Value, experienceSurvey.ServiceRecord.Value);
                    notification.Color = "#4527a0";
                    notification.CreatedDate = DateTime.Now;
                    _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
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

        //Put Experience Survey
        [HttpPut("UpdateExperienceSurvey", Name = "UpdateExperienceSurvey")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ExperienceSurveyDto>> UpdateExperienceSurvey([FromBody] ExperienceSurveyDto dto)
        {
            var response = new ApiResponse<ExperienceSurveyDto>();
            try
            {
                ExperienceSurvey immi = _experienceSurveyRepository.UpdateCustom(_mapper.Map<ExperienceSurvey>(dto), dto.Id);
                response.Result = _mapper.Map<ExperienceSurveyDto>(immi);
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

        [HttpGet("GetExperienceSurveyByServiceRecord", Name = "GetExperienceSurveyByServiceRecord")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetExperienceSurveyByServiceRecord([FromQuery] int sr)
        {
            try
            {
                var Result = _experienceSurveyRepository.FindAll(x => x.ServiceRecord == sr);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get Experience Survey
        [HttpGet("GetExperienceSurveyById", Name = "GetExperienceSurveyById")]
        public ActionResult<ApiResponse<ExperienceSurveySelectDto>> GetExperienceSurveyById([FromQuery] int id)
        {
            var response = new ApiResponse<ExperienceSurveySelectDto>();
            try
            {
                var map = _mapper.Map<ExperienceSurveySelectDto>(_experienceSurveyRepository.SelectCustom(id));
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

    }
}
