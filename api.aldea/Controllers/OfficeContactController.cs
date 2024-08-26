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
    public class OfficeContactController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IOfficeContactRepository _officeContactRepository;
        public OfficeContactController(IMapper mapper, ILoggerManager loggerManager, IOfficeContactRepository officeContactRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _officeContactRepository = officeContactRepository;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddOfficeContact", Name = "AddOfficeContact")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeContactDto>> AddOfficeContact([FromBody] OfficeContactDto dto)
        {
            var response = new ApiResponse<OfficeContactDto>();
            try
            {
                OfficeContactDto Office = _mapper.Map<OfficeContactDto>(_officeContactRepository.Add(_mapper.Map<OfficeContact>(dto)));

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
        [HttpPut("UpdateOfficeContact", Name = "UpdateOfficeContact")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeContactDto>> UpdateOfficeContact([FromBody] OfficeContactDto dto)
        {
            var response = new ApiResponse<OfficeContactDto>();
            try
            {
                OfficeContactDto Office = _mapper.Map<OfficeContactDto>(_officeContactRepository.Update(_mapper.Map<OfficeContact>(dto), dto.Id));

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
        [Route("GetOfficeContactById")]
        public ActionResult GetOfficeContactById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _officeContactRepository.Find(x => x.Id == id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
        
        [HttpGet]
        [Route("GetOfficeContact/{office}")]
        public ActionResult GetOfficeContact(int office)
        {
            try
            {
                return Ok(new { Success = true, result = _officeContactRepository.FindAll(x => x.IdOfficeInformation == office) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
