using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.SupplierPartnerProfileConsultant;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.SupplierPartnerProfile;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ICalendarConsultantContactsConsultantRepository _calendarConsultantContactsConsultantRepository;
        private readonly ISupplierPartnerProfileConsultantRepository _supplierPartnerProfileConsultantRepository;
        private readonly IUserRepository _userRepository;
        public CalendarController(IMapper mapper, ILoggerManager loggerManager, ICalendarConsultantContactsConsultantRepository calendarConsultantContactsConsultantRepository,
            ISupplierPartnerProfileConsultantRepository supplierPartnerProfileConsultantRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _calendarConsultantContactsConsultantRepository = calendarConsultantContactsConsultantRepository;
            _supplierPartnerProfileConsultantRepository = supplierPartnerProfileConsultantRepository;
            _userRepository = userRepository;
        }

        [HttpPost("AddAvailability", Name = "AddAvailability")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CalendarConsultantContactsConsultantDto>> AddAvailability([FromBody] CalendarConsultantContactsConsultantDto dto)
        {
            var response = new ApiResponse<CalendarConsultantContactsConsultantDto>();
            try
            {
                var service = _userRepository
                    .GetAllIncluding(x => x.ProfileUsers)
                    .FirstOrDefault(x => x.Id== dto.ConsultantContactsConsultant);
                if (service.ProfileUsers.Any())
                {
                    dto.ConsultantContactsConsultant = service.ProfileUsers.FirstOrDefault().Id;
                    CalendarConsultantContactsConsultant calendar = _calendarConsultantContactsConsultantRepository.Add(_mapper.Map<CalendarConsultantContactsConsultant>(dto));
                    response.Result = _mapper.Map<CalendarConsultantContactsConsultantDto>(calendar);
                }
                else
                {
                    response.Success = false;
                    response.Message = "User Not Found";
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

        [HttpPost("UpdateAvailability", Name = "UpdateAvailability")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CalendarConsultantContactsConsultantDto>> UpdateAvailability([FromBody] CalendarConsultantContactsConsultantDto dto)
        {
            var response = new ApiResponse<CalendarConsultantContactsConsultantDto>();
            try
            {
                var service = _userRepository
                    .GetAllIncluding(x => x.ProfileUsers)
                    .FirstOrDefault(x => x.Id == dto.ConsultantContactsConsultant);
                if (service.ProfileUsers.Any())
                {
                    dto.ConsultantContactsConsultant = service.ProfileUsers.FirstOrDefault().Id;
                    CalendarConsultantContactsConsultant calendar = _calendarConsultantContactsConsultantRepository.Update(_mapper.Map<CalendarConsultantContactsConsultant>(dto), dto.Id);
                    response.Result = _mapper.Map<CalendarConsultantContactsConsultantDto>(calendar);
                }
                else
                {
                    response.Success = false;
                    response.Message = "User Not Found";
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

        [HttpPost("AddNotAvailable", Name = "AddNotAvailable")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CalendarConsultantContactsConsultantDto>> AddNotAvailable([FromBody] CalendarConsultantContactsConsultantDto dto)
        {
            var response = new ApiResponse<CalendarConsultantContactsConsultantDto>();
            try
            {
                var service = _userRepository
                   .GetAllIncluding(x => x.ProfileUsers)
                   .FirstOrDefault(x => x.Id == dto.ConsultantContactsConsultant);
                if (service.ProfileUsers.Any())
                {
                    var available = _calendarConsultantContactsConsultantRepository.FindAll(x => x.ConsultantContactsConsultant == service.ProfileUsers.FirstOrDefault().Id && x.Day == dto.Day);
                    foreach (var i in available)
                    {
                        _calendarConsultantContactsConsultantRepository.Delete(i);
                    }
                    dto.ConsultantContactsConsultant = service.ProfileUsers.FirstOrDefault().Id;
                    CalendarConsultantContactsConsultant calendar = _calendarConsultantContactsConsultantRepository.Add(_mapper.Map<CalendarConsultantContactsConsultant>(dto));
                    response.Result = _mapper.Map<CalendarConsultantContactsConsultantDto>(calendar);
                }
                else
                {
                    response.Success = false;
                    response.Message = "User Not Found";
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

        [HttpDelete("DeleteAvailability/{id}/", Name = "DeleteAvailability/{id}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CalendarConsultantContactsConsultantDto>> DeleteAvailability(int user, int id)
        {
            var response = new ApiResponse<CalendarConsultantContactsConsultantDto>();
            try
            {

                CalendarConsultantContactsConsultant calendar = _calendarConsultantContactsConsultantRepository.Get(id);
                _calendarConsultantContactsConsultantRepository.Delete(calendar);
                response.Message = "Delete Success";
                response.Success = true;
                response.Result = _mapper.Map<CalendarConsultantContactsConsultantDto>(calendar);
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

        [HttpGet("GetAvailability/{user}/", Name = "GetAvailability/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CalendarConsultantContactsConsultantDto>>> GetAvailability(int user)
        {
            var response = new ApiResponse<List<CalendarConsultantContactsConsultantDto>>();
            try
            {
                var service = _userRepository
                   .GetAllIncluding(x => x.ProfileUsers)
                   .FirstOrDefault(x => x.Id == user);
                if (service.ProfileUsers.Any())
                {
                    var calendar = _calendarConsultantContactsConsultantRepository.GetAll()
                        .Where(x => x.ConsultantContactsConsultant == service.ProfileUsers.FirstOrDefault().Id).ToList();
                    response.Success = true;
                    response.Result = _mapper.Map<List<CalendarConsultantContactsConsultantDto>>(calendar);
                }
                else
                {
                    response.Success = false;
                    response.Message = "User Not Found";
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

        [HttpGet("Availability/{key}", Name = "AvailabilityById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CalendarConsultantContactsConsultantDto>> AvailabilityById(int key)
        {
            var response = new ApiResponse<CalendarConsultantContactsConsultantDto>();
            try
            {
                var calendar = _calendarConsultantContactsConsultantRepository.Find(f=>f.Id == key);
                response.Success = true;
                response.Result = _mapper.Map<CalendarConsultantContactsConsultantDto>(calendar);
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
