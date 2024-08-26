using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.HousingSpecification;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.HousingSpecification;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousingSpecificationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IHousingSpecification _housingSpecification;
        private readonly IWorkOrderRepository _workOrderRepository;
        public HousingSpecificationController(
            IMapper mapper,
            ILoggerManager logger,
            IHousingSpecification housingSpecification,
            IWorkOrderRepository workOrderRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _housingSpecification = housingSpecification;
            _workOrderRepository = workOrderRepository;
        }

        [HttpPost("CreateHousingSpecification", Name = "CreateHousingSpecification")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<HousingSpecificationDto>>> PostCreateHousingSpecification([FromBody] List<HousingSpecificationDto> dto)
        {
            var response = new ApiResponse<List<HousingSpecificationDto>>();
            try
            {
                List<HousingSpecification> rel = new List<HousingSpecification>();
                int id = 0;
                foreach(var i in dto)
                {
                    id = _housingSpecification.GetWorkOrderServiceId(i.WorkOrderServices.Value, i.TypeService.Value);
                    i.WorkOrderServices = id;
                    rel.Add(_housingSpecification.AddRelhousingAmenitie(_mapper.Map<HousingSpecification>(i)));
                }

                response.Result = _mapper.Map<List<HousingSpecificationDto>>(rel);
                response.Success = true;
                response.Message = "Success";
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

        [HttpPost("AddHousingSpecification", Name = "AddHousingSpecification")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingSpecificationDto>> AddHousingSpecification([FromBody] HousingSpecificationDto dto)
        {
            var response = new ApiResponse<HousingSpecificationDto>();
            try
            {
                HousingSpecification rel = new HousingSpecification();
                rel = _housingSpecification.AddRelhousingAmenitie(_mapper.Map<HousingSpecification>(dto));

                response.Result = _mapper.Map<HousingSpecificationDto>(rel);
                response.Success = true;
                response.Message = "Success";
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

        [HttpPut("PutCreateHousingSpecification", Name = "PutCreateHousingSpecification")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingSpecificationDto>> PutCreateHousingSpecification([FromBody] HousingSpecificationDto dto)
        {
            var response = new ApiResponse<HousingSpecificationDto>();
            try
            {
                HousingSpecification rel = _housingSpecification.UpdateRelhousingAmenitie(_mapper.Map<HousingSpecification>(dto));

                response.Result = _mapper.Map<HousingSpecificationDto>(rel);
                response.Success = true;
                response.Message = "Success";
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sr">Es el WorkOrderServicesId</param>
        /// <param name="service">El tipo de servicio ya sea Area Orientation(1), Home Finding(2) o Area Orientation(3)</param>
        /// <returns></returns>
        [HttpGet("GetHousingSpecitifcationByServiceRecord/{sr}/{service}", Name = "GetHousingSpecitifcationByServiceRecord/{sr}/{service}")]
        public ActionResult<ApiResponse<HousingSpecificationSelectDto>> GetHousingSpecitifcationByServiceRecord(int sr, int service)
        {
            var response = new ApiResponse<HousingSpecificationSelectDto>();
            try
            {
                var m = _housingSpecification.GetAllIncluding(x => x.RelHousingAmenities, y => y.PropertyType, z => z.Metric, o => o.ContractType, p => p.Currency
                    , w => w.TypeServiceNavigation);
                HousingSpecificationSelectDto rel = _mapper.Map<HousingSpecificationSelectDto>(
                    m.FirstOrDefault(x => 
                        x.WorkOrderServices == sr )
                    //    && x.TypeService == service)
                    );

                if(rel != null)
                {
                    var data = _housingSpecification.GetWorkOrder(rel.WorkOrderServices.Value, rel.TypeService.Value);
                    rel.workOrder = data.Item1;
                    rel.serviceID = data.Item2;

                }

                response.Result = rel;
                response.Success = true;
                response.Message = "Success";
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

        [HttpGet("GetHousingSpecification/{sr}/", Name = "GetHousingSpecification/{sr}/")]
        public ActionResult GetHousingSpecification(int sr)
        {
            try
            {
                return StatusCode(200, new { Success = true, Result = _housingSpecification.GetHousingSpecification(sr) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "", Message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetHousingSpecificationBySR/{sr}/", Name = "GetHousingSpecificationBySR/{sr}/")]
        public ActionResult GetHousingSpecificationBySR(int sr)
        {
            try
            {
                var hs = _housingSpecification.GetHousingSpecificationBySR(sr);
                return StatusCode(200, new { Success = true, Result = hs });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "", Message = ex.Message.ToString() });
            }
        }

        [HttpGet("GetHousingSpecitifcationByWosId/{wos_id}", Name = "GetHousingSpecitifcationByWosId/{wos_id}")]
        public ActionResult<ApiResponse<HousingSpecificationSelectDto>> GetHousingSpecitifcationByWosId(int wos_id)
        {
            var response = new ApiResponse<HousingSpecificationSelectDto>();
            try
            {
                var m = _housingSpecification.GetAllIncluding(x => x.RelHousingAmenities, y => y.PropertyType, z => z.Metric, o => o.ContractType, p => p.Currency
                    , w => w.TypeServiceNavigation);
                HousingSpecificationSelectDto rel = _mapper.Map<HousingSpecificationSelectDto>(
                    m.FirstOrDefault(x =>
                        x.WorkOrderServices == wos_id)
                    );

                if (rel != null)
                {
                    var data = _housingSpecification.GetWorkOrder(rel.WorkOrderServices.Value, rel.TypeService.Value);
                    rel.workOrder = data.Item1;
                    rel.serviceID = data.Item2;

                }

                response.Result = rel;
                response.Success = true;
                response.Message = "Success";
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

