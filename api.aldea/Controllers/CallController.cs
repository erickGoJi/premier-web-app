using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.Call;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Call;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ICallRepository _callRepository;

        public CallController(
        IMapper mapper,
        ILoggerManager logger,
        ICallRepository callRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _callRepository = callRepository;
        }

        // Post Create a new Call
        [HttpPost("CreateCall", Name = "CreateCall")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CallDto>> PostCall([FromBody] CallDto dto)
        {
            var response = new ApiResponse<CallDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<CallDto>(_callRepository.Add(_mapper.Map<Call>(dto)));
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

        // Put Update a Call
        [HttpPut("UpdateCall", Name = "UpdateCall")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CallDto>> UpdateCall([FromBody] CallDto dto)
        {
            var response = new ApiResponse<CallDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<CallDto>(_callRepository.UpdateCustom(_mapper.Map<Call>(dto)));
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

        // Get List Call
        [HttpGet("GetCallByServiceRecord", Name = "GetCallByServiceRecord")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CallByServiceRecord>>> GetCallByServiceRecordId([FromQuery] int Service_record_id, int service_line_id)
        {
            var response = new ApiResponse<List<CallByServiceRecord>>();
            try
            {
                var Result = _mapper.Map<List<CallByServiceRecord>>(_callRepository.GetCallByServiceRecord(Service_record_id, service_line_id));
                response.Success = true;
                response.Result = Result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }

            return Ok(response);
        }

        // Get List Call By Id
        [HttpGet("GetCallById", Name = "GetCallById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CallDto>> GetCallById([FromQuery] int Id)
        {
            var response = new ApiResponse<CallDto>();
            try
            {
                var callInclude = _callRepository.GetAllIncluding(x => x.CallAssistants, 
                    s => s.Service, 
                    a => a.ServiceLine,
                    d => d.CalleNavigation,
                    q => q.CallerNavigation,
                    o => o.DurationNavigation
                    ).Where(x => x.Id == Id).FirstOrDefault();
                response.Success = true;
                response.Result = _mapper.Map<CallDto>(callInclude);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }

            return Ok(response);
        }

    }
}
