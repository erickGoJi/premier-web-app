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
    public class OfficeInformationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IOfficeInformationRepository _officeInformationRepository;
        public OfficeInformationController(IMapper mapper, ILoggerManager loggerManager, IOfficeInformationRepository officeInformationRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _officeInformationRepository = officeInformationRepository;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddOfficeInformation", Name = "AddOfficeInformation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeInformationDto>> AddOfficeInformation([FromBody] OfficeInformationDto dto)
        {
            var response = new ApiResponse<OfficeInformationDto>();
            try
            {
                OfficeInformationDto Office = _mapper.Map<OfficeInformationDto>(_officeInformationRepository.Add(_mapper.Map<OfficeInformation>(dto)));

                response.Success = true;
                response.Message = "Success";
                response.Result = Office;
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
        [HttpPut("UpdateOfficeInformation", Name = "UpdateOfficeInformation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeInformationDto>> UpdateOfficeInformation([FromBody] OfficeInformationDto dto)
        {
            var response = new ApiResponse<OfficeInformationDto>();
            try
            {
                OfficeInformationDto Office = _mapper.Map<OfficeInformationDto>(_officeInformationRepository.Update(_mapper.Map<OfficeInformation>(dto), dto.Id));

                response.Success = true;
                response.Message = "Success";
                response.Result = Office;
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
        [Route("GetOfficeInformationById")]
        public ActionResult GetGeneralContractPricingInfoById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _officeInformationRepository.Find(x => x.Id == id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
        
        [HttpGet]
        [Route("GetOfficeInformation/{client}")]
        public ActionResult GetOfficeInformation(int client)
        {
            try
            {
                return Ok(new { Success = true, result = _officeInformationRepository.FindAll(x => x.IdClientPartnerProfile == client) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
        
    }
}
