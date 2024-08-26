using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using AutoMapper;
using biz.premier.Repository.AssigneeFeedback;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssigneeFeedbackController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IAssigneeFeedbackRepository _assigneeFeedbackRepository;

        public AssigneeFeedbackController(IMapper mapper, ILoggerManager loggerManager, IAssigneeFeedbackRepository assigneeFeedbackRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _assigneeFeedbackRepository = assigneeFeedbackRepository;
        }

        [HttpGet("GetAssigneeFeedback", Name = "GetAssigneeFeedback")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public IActionResult GetAssigneeFeedback([FromQuery] int pageNumber, int Pagesize)
        {
            try
            {
                var Result = _assigneeFeedbackRepository.GetAllCustom(pageNumber, Pagesize);
                return Ok(new { Success = true, Result, total_record = _assigneeFeedbackRepository.TotalFeed() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.ToString() });

            }
        }
    }
}
