using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ClientPartnerProfile;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.ClientPartner;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Controllers
{
    public class ServiceScoreAwardsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IServiceScoreAwardsRepository _serviceScoreAwardsRepository;
        public ServiceScoreAwardsController(IMapper mapper, ILoggerManager loggerManager, IServiceScoreAwardsRepository serviceScoreAwardsRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _serviceScoreAwardsRepository = serviceScoreAwardsRepository; ;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddServiceScoreAward", Name = "AddServiceScoreAward")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ServiceScoreAwardDto>> AddServiceScoreAward([FromBody] ServiceScoreAwardDto dto)
        {
            var response = new ApiResponse<ServiceScoreAwardDto>();
            try
            {
                ServiceScoreAwardDto service = _mapper.Map<ServiceScoreAwardDto>(_serviceScoreAwardsRepository.Add(_mapper.Map<ServiceScoreAward>(dto)));

                response.Success = true;
                response.Message = "Success";
                response.Result = service;
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

        // Put Update a Profile 
        [HttpPut("UpdateServiceScoreAward", Name = "UpdateServiceScoreAward")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ServiceScoreAwardDto>> UpdateServiceScoreAward([FromBody] ServiceScoreAwardDto dto)
        {
            var response = new ApiResponse<ServiceScoreAwardDto>();
            try
            {
                ServiceScoreAwardDto service = _mapper.Map<ServiceScoreAwardDto>(_serviceScoreAwardsRepository.Update(_mapper.Map<ServiceScoreAward>(dto), dto.Id));

                response.Success = true;
                response.Message = "Success";
                response.Result = service;
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

        [HttpGet]
        [Route("GetServiceScoreAwardById")]
        public ActionResult GetServiceScoreAwardById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _serviceScoreAwardsRepository.Find(x => x.Id == id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
