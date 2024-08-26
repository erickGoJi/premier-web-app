using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.MapIt;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.MapIt;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapItController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IMapItRepository _mapItRepository;
        public MapItController(IMapper mapper, ILoggerManager loggerManager, IMapItRepository mapItRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _mapItRepository = mapItRepository;
        }


        [HttpPost("PostMapIt", Name = "PostMapIt")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<MapItDto>> PostMapIt([FromBody] MapItDto dto)
        {
            var response = new ApiResponse<MapItDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                MapIt mapIt = _mapItRepository.Add(_mapper.Map<MapIt>(dto));
                response.Result = _mapper.Map<MapItDto>(mapIt);
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

        [HttpPut("PutMapIt", Name = "PutMapIt")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<MapItDto>> PutMapIt([FromBody] MapItDto dto)
        {
            var response = new ApiResponse<MapItDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                MapIt mapIt = _mapItRepository.UpdateCustom(_mapper.Map<MapIt>(dto), dto.Id);
                response.Result = _mapper.Map<MapItDto>(mapIt);
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

        [HttpGet("GetMapItById", Name = "GetMapItById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetMapItById([FromQuery] int id)
        {
            try
            {
                return Ok(new { Success = true, Result = _mapItRepository.SelectCustom(id) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("GetMapIt", Name = "GetMapIt")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetMapIt([FromQuery] int ServiceLineId, int service_record_id)
        {
            try
            {
                return Ok(new { Success = true, Result = _mapItRepository.GetMapIt(ServiceLineId, service_record_id) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
