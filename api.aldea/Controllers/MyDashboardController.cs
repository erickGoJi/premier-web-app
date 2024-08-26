using api.premier.ActionFilter;
using api.premier.Models;
using AutoMapper;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using biz.premier.Repository;
using api.premier.Models.Dashboard;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyDashboardController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IUpcomingEventRepository _upcomingEventRepository;
        private readonly ISlidePhraseRepository _slidePhraseRepository;

        public MyDashboardController(IMapper mapper, ILoggerManager loggerManager,
            IServiceRecordRepository serviceRecordRepository,
            IUpcomingEventRepository upcomingEventRepository,
            ISlidePhraseRepository slidePhraseRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _serviceRecordRepository = serviceRecordRepository;
            _upcomingEventRepository = upcomingEventRepository;
            _slidePhraseRepository = slidePhraseRepository;
        }

        [HttpGet("GetDashboard/{user}/", Name = "GetDashboard/{user}/{serviceLine}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDashboard(int user, string serviceLine, [FromQuery] int? country, [FromQuery] int? city,
            [FromQuery] int? partner,
            [FromQuery] int? client, [FromQuery] int? coordinator, [FromQuery] int? supplier, [FromQuery] int? status, 
            [FromQuery] DateTime? rangeDate1, [FromQuery] DateTime? rangeDate2)
        {
            try
            {
                var map = _serviceRecordRepository.GetDashboard(user, serviceLine, country, city, partner, client,
                    coordinator, supplier, status, rangeDate1, rangeDate2);
               
                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetDashboardApp/{user}/", Name = "GetDashboardApp/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDashboardApp(int user, int? serviceLine)
        {
            try
            {
                ActionResult map = new EmptyResult();
                switch (_serviceRecordRepository.GetRole(user))
                {
                    case 3:
                        map = _serviceRecordRepository.GetDashboardApp(user);
                        break;
                    case 4:
                        map = _serviceRecordRepository.GetDashboardAppAssigne(user, serviceLine);
                        break;
                    default:
                        // code block
                        break;
                }
                //var map =   _serviceRecordRepository.GetDashboardApp(user);

                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetDashboardTest/{user}/", Name = "GetDashboardTest/{user}/{serviceLine}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<DashboardDto>>> GetDashboardTest(int user, string serviceLine, [FromQuery] int? country, [FromQuery] int? city,
            [FromQuery] int? partner,
            [FromQuery] int? client, [FromQuery] int? coordinator, [FromQuery] int? supplier, [FromQuery] int? status,
            [FromQuery] DateTime? rangeDate1, [FromQuery] DateTime? rangeDate2)
        {
            var response = new ApiResponse<List<DashboardDto>>();
            try
            {

                var map = _mapper.Map<List<DashboardDto>>(_serviceRecordRepository.GetDashboardTest(user, serviceLine, country, city, partner, client,
                    coordinator, supplier, status, rangeDate1, rangeDate2));

                foreach (var item in map)
                {
                    //item.status = _serviceRecordRepository.SetStatusServiceRecord(item.id, item.serviceLineId);
                }

                response.Result = map;
                response.Success = true;
                response.Message = "Success";
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

        [HttpGet("GetCountServices/{id}", Name = "GetCountServices/{id}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public int GetCountServices(int id)
        {
            try
            {
                int map = _serviceRecordRepository.GetServicesSupplierCount(id);

                return map;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return 0;
            }
        }

        [HttpGet("GetDashboardAdmin/{sr}", Name = "GetDashboardAdmin/{sr}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDashboardAdmin(int sr, int? userId)
        {
            try
            {
                var map = _serviceRecordRepository.getServicesBySupplierAdmin(sr, userId);

                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetDashboardAdminSupplier/{sr}/{idUser}", Name = "GetDashboardAdminSupplier/{sr}/{idUser}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDashboardAdminSupplier(int sr, int idUser)
        {
            try
            {
                var map = _serviceRecordRepository.getServicesBySupplier(sr, idUser);

                return Ok(new { Success = true, map });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetCalendar/{user}/", Name = "GetCalendar/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCalendar(int user, [FromQuery] int? country, [FromQuery] int? city,
            [FromQuery] int? partner,
            [FromQuery] int? client, [FromQuery] int? coordinator, [FromQuery] int? supplier,
            [FromQuery] int? serviceLine, [FromQuery] DateTime? rangeDate1, [FromQuery] DateTime? rangeDate2)
        {
            try
            {
                var map = _serviceRecordRepository.GetCalendar(user, serviceLine, country, city, partner, client,
                    coordinator, supplier, rangeDate1, rangeDate2);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetFollowing/{user}/", Name = "GetFollowing/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetFollowing(int user, [FromQuery] int? sr, [FromQuery] int? coordinator)
        {
            try
            {
                var map = _serviceRecordRepository.GetFollowing(user, sr, coordinator);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }
        
        [HttpGet("Following/Coordinators/{user}/", Name = "GetFollowing/Coordinators/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetFollowingCoordinators(int user)
        {
            try
            {
                var map = _serviceRecordRepository.GetFollowingCoordinators(user);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }
        
        [HttpGet("Following/SR/{user}/", Name = "GetFollowing/Sr/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetFollowingSr(int user)
        {
            try
            {
                var map = _serviceRecordRepository.GetFollowingsSr(user);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetCoordinators/{user}/", Name = "GetCoordinators/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCoordinators(int user, [FromQuery] int? country, [FromQuery] int? city,
            [FromQuery] int? serviceLine, [FromQuery] int? office)
        {
            try
            {
                var map = _serviceRecordRepository.GetCoordinators(user, country, city, serviceLine, office);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetEscalation/{user}/", Name = "GetEscalation/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetEscalation(int user, [FromQuery] bool? status, [FromQuery] DateTime? rangeDate1,
            [FromQuery] DateTime? rangeDate2,
            [FromQuery] int? level, [FromQuery] int? client, [FromQuery] int? supplierPartner, [FromQuery] int? city,
            [FromQuery] int? partner, [FromQuery] int? country, [FromQuery] int? serviceLine, [FromQuery] int? office)
        {
            try
            {
                var map = _serviceRecordRepository.GetEscalation(user, status, rangeDate1, rangeDate2, level, client,
                    supplierPartner, city, partner, country, serviceLine, office);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetUpcomingArrivals/{user}/", Name = "GetUpcomingArrivals/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetUpcomingArrivals(int user, [FromQuery] DateTime? rangeDate1,
            [FromQuery] DateTime? rangeDate2,
            [FromQuery] int? city, [FromQuery] int? partner, [FromQuery] int? client, [FromQuery] int? coordinator,
            [FromQuery] int? supplierPartner)
        {
            try
            {
                var map = _serviceRecordRepository.GetUpcomingArrivals(user, rangeDate1, rangeDate2, city, partner,
                    client, coordinator, supplierPartner);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetCalls/{user}/", Name = "GetCalls/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCalls(int user, [FromQuery] string caller, [FromQuery] int? sr, [FromQuery] int? wo,
            [FromQuery] int? service)
        {
            try
            {
                var map = _serviceRecordRepository.GetCalls(user, caller, sr, wo, service);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpGet("GetReminders/{user}/", Name = "GetReminders/{user}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetReminders(int user, [FromQuery] DateTime? rangeDate1, [FromQuery] DateTime? rangeDate2,
            [FromQuery] int? city, [FromQuery] int? sr, [FromQuery] int? sl, [FromQuery] int? wo)
        {
            try
            {
                var map = _serviceRecordRepository.GetReminders(user, rangeDate1, rangeDate2, city, sr, sl, wo);

                return Ok(new {Success = true, map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        [HttpPost("AddReminder", Name = "AddReminder")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddReminder([FromBody] AddReminder dto)
        {
            try
            {
                var map = _serviceRecordRepository.AddReminder(dto.user, dto.service, dto.reminderDate, dto.category,
                    dto.comment);

                return Ok(new {Success = true, Message = map});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new {Success = false, Message = ex.ToString()});
            }
        }

        // Get: Slider Phrase
        [HttpGet("GetSlidersPhrase", Name = "GetSlidersPhrase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<SlidePhraseDto>>> GetSliderPhrase()
        {
            var response = new ApiResponse<List<SlidePhraseDto>>();
            try
            {
                response.Result = _mapper.Map<List<SlidePhraseDto>>(_slidePhraseRepository.GetAll());
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Get: Upcoming Events
        [HttpGet("GetUpcomingEvents/{user}", Name = "GetUpcomingEvents/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<UpcomingEventDto>>> GetUpcomingEvents(int user)
        {
            var response = new ApiResponse<List<UpcomingEventDto>>();
            try
            {
                var region = _upcomingEventRepository.GetRegion(user);
                response.Result =_mapper.Map<List<UpcomingEventDto>>(_upcomingEventRepository.GetAllIncluding(
                    x => x.CountryNavigation, 
                    q => q.CityNavigation)
                    .Where(x => x.City == region.Item2 && x.Country == region.Item1 && x.EventDate >= DateTime.Now)
                );
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

    }
}
