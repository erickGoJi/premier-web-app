using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.EmailSend;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Email;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailSendController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IEmailRepository _emailRepository;
        private readonly IUserRepository _userRepository;
        public EmailSendController(
        IMapper mapper,
        ILoggerManager logger,
        IEmailRepository emailRepository,
        IUserRepository userRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _emailRepository = emailRepository;
            _userRepository = userRepository;
        }

        // Post Create a new Email Send
        [HttpPost("CreateEmailSend", Name = "CreateEmailSend")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EmailSendDto>> PostEmailSend([FromBody] List<EmailSendDto> dto)
        {
            var response = new ApiResponse<EmailSendDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<EmailSendDto>(_emailRepository.Add(_mapper.Map<EmailSend>(dto))); ;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        // Post a new Email Send
        [HttpPost("EmailSend", Name = "EmailSend")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EmailSendDto>> EmailSend([FromBody] EmailSendDto dto, string email, string username)
        {
            var response = new ApiResponse<EmailSendDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<EmailSendDto>(_emailRepository.Add(_mapper.Map<EmailSend>(dto))); 

                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/EmailSend.html"));
                string body = string.Empty;
                body = reader.ReadToEnd();
                body = body.Replace("{username}", "Hola," + " " + username);

                _userRepository.SendMail(email, body, "Recuperar contraseña");
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        // Get List Call
        [HttpGet("GetEmail", Name = "GetEmail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllEmailSend([FromQuery] int service_line_id, int service_record_id)
        {
            try
            {
                return Ok(new { Success = true, Result = _emailRepository.GetEmailSendByServiceRecord(service_record_id, service_line_id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
    }
}
