using System;
using System.Collections.Generic;
using System.Linq;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.PostIt;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.PostIt;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostItController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IPostItRepository _postItRepository;

        public PostItController(IPostItRepository postItRepository, IMapper mapper, ILoggerManager loggerManager,
            IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _postItRepository = postItRepository;
        }
        
        // GET: SupplierPartnersActive
        [HttpGet("All/{user}", Name = "All/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<PostItDto>>> GetIndustries(int user)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _mapper.Map<List<PostItDto>>(_postItRepository.GetAll().Where(x => x.CreatedBy == user).OrderBy(o => o.CreatedDate)),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        
        // POST: Add Post It
        [HttpPost]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PostItDto>> AddIndustry(PostItDto key)
        {
            var response = new ApiResponse<PostItDto>();
            try
            {
                response.Result =_mapper.Map<PostItDto>(_postItRepository.Add(_mapper.Map<PostIt>(key)));
                response.Message = "Post It was added successfully.";
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
        
        // Put: Update Post It
        [HttpPut]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PostItDto>> UpdateIndustry(PostItDto key)
        {
            var response = new ApiResponse<PostItDto>();
            try
            {
                response.Result =_mapper.Map<PostItDto>(_postItRepository.Update(_mapper.Map<PostIt>(key), key.Id));
                response.Message = "Post It was updated successfully.";
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
        
        // Put: Update Post It
        [HttpDelete]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PostItDto>> DeletePostIt([FromQuery]int key)
        {
            var response = new ApiResponse<PostItDto>();
            try
            {
                var find = _postItRepository.Find(x => x.Id == key);
                _postItRepository.Delete(find);
                response.Result = null;
                response.Message = "Post It was deleted successfully.";
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