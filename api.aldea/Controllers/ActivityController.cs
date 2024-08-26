using AutoMapper;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        public ActivityController(IMapper mapper, ILoggerManager loggerManager, IUtiltyRepository utiltyRepository, IServiceRecordRepository serviceRecordRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _serviceRecordRepository = serviceRecordRepository;
        }

        // GET: SupplierPartnersActive
        [HttpGet]
        [Route("GetSupplierPartnersActive")]
        public ActionResult GetAllServices([FromQuery]int? country, [FromQuery] int? city, [FromQuery] int? supplierPartner, [FromQuery] DateTime? range1, [FromQuery] DateTime? range2)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _serviceRecordRepository.SupplierPartnersActive(country, city, supplierPartner, range1, range2),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        // GET: Activity
        [HttpGet]
        [Route("GetActivity")]
        public ActionResult GetActivity([FromQuery] int? office, [FromQuery] int? status, [FromQuery] int? serviceLine, [FromQuery] DateTime? range1, [FromQuery] DateTime? range2)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = "",//_serviceRecordRepository.GetActivity(office, status, serviceLine, range1, range2),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        // GET: ReportByCountry
        [HttpGet]
        [Route("GetReportByCountry/{country}")]
        public ActionResult GetReportByCountry(int country, int? partner, int? client, int? supplierPartner, DateTime? range1, DateTime? range2,
            int? status, int? serviceLine, int? serviceCategory, int? city)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _serviceRecordRepository.GetReportByCountry(country, partner, client, supplierPartner, range1, range2, status, serviceLine, serviceCategory, city),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        // GET: GetReportByPartner
        [HttpGet]
        [Route("GetReportByPartner/{country}/{partner}")]
        public ActionResult GetReportByCountry(int country, int partner, int? client, int? supplierPartner, DateTime? range1, DateTime? range2,
            int? status, int? serviceLine, int? serviceCategory, int? city)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _serviceRecordRepository.GetReportByClientPartner(country, partner, client, supplierPartner, range1, range2, status, serviceLine, serviceCategory, city),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        // GET: ReportByServiceRecordStatus
        [HttpGet]
        [Route("GetServiceRecordByStatus/{status}")]
        public ActionResult GetServiceRecordByStatus(int status, int? country, int? city, int? serviceLine, int? serviceRecord, int? partner, int? client, int? supplierPartner,
            DateTime? range1, DateTime? range2, int? serviceCategory)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _serviceRecordRepository.GetServiceRecordByStatus(status, country, city, serviceLine, serviceRecord, partner, client, supplierPartner,
                            range1,  range2, serviceCategory),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: GetReportBySupplierPartner
        [HttpGet]
        [Route("GetReportByPartner/{supplierPartner}")]
        public ActionResult GetReportBySupplierPartner(int? country, int? partner, int? client, int supplierPartner, DateTime? range1, DateTime? range2,
            int? status, int? serviceLine, int? serviceCategory, int? city, int? consultant)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _serviceRecordRepository.GetReportBySupplierPartner(country, partner, client, supplierPartner, range1, range2, status, serviceLine, serviceCategory, city, consultant),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: GetAllActiveServices
        [HttpGet]
        [Route("GetAllActiveServices")]
        public ActionResult GetAllActiveServices(int? country)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _serviceRecordRepository.GetAllActiveServices(country),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

    }
}
