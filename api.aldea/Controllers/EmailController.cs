using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.Email;
using api.premier.Models.EmailSend;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.EmailServiceRecord;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailServiceRecordRepository _emailServiceRecordRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;

        public EmailController(IEmailRepository emailRepository,
            IEmailServiceRecordRepository emailServiceRecordRepository,
            IMapper mapper,
            ILoggerManager loggerManager,
            IUserRepository userRepository)
        {
            _emailRepository = emailRepository;
            _emailServiceRecordRepository = emailServiceRecordRepository;
            _mapper = mapper;
            _logger = loggerManager;
            _userRepository = userRepository;
        }
        
        // POST: Add new email
        [HttpPost]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EmailDto>> AddNewEmail([FromBody] EmailDto dto)
        {
            var response = new ApiResponse<EmailDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<EmailDto>(_emailRepository.Add(_mapper.Map<Email>(dto))); ;
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
        
        // PUT: edit email
        [HttpPut]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EmailDto>> UpdateEmail([FromBody] EmailDto dto)
        {
            var response = new ApiResponse<EmailDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<EmailDto>(_emailRepository.Update(_mapper.Map<Email>(dto), dto.Id)); ;
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
        
        // Get: return email by id
        [HttpGet]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EmailDto>> GetEmailById([FromQuery] int key)
        {
            var response = new ApiResponse<EmailDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<EmailDto>(_emailRepository.Get(key)); ;
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
        
        // GET: Return Emails list
        [HttpGet("GetEmails", Name = "GetEmails")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetEmails()
        {
            try
            {
                return Ok(new { Success = true, Result = _emailRepository.GetAllIncluding(x => x.Country, service => service.Service).Select(s => new
                {
                    s.Id,
                    s.NickName,
                    s.Subject,
                    s.Country.Name,
                    s.Service.Category,
                    s.CreationDate
                }).OrderByDescending(o => o.CreationDate).ToList() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
        
        // GET: Return Emails list From SERVICE RECORD
        [HttpGet("ServiceRecord", Name = "ServiceRecord")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetEmails(int user, int sr)
        {
            try
            {
                return Ok(new { Success = true, Result = _emailRepository.GetEmailForServiceRecord(user, sr) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
        
        // POST: Send Email
        [HttpPost("SendEmail", Name = "SendEmail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EmailServiceRecordDto>> SendEmail([FromBody] EmailServiceRecordDto dto)
        {
            var response = new ApiResponse<EmailServiceRecordDto>();
            try
            {
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<EmailServiceRecordDto>(_emailServiceRecordRepository.Add(_mapper.Map<EmailServiceRecord>(dto))); ;
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
        
        [HttpPut("SendAppAccess", Name = "SendAppAccess")]
        public async Task<ActionResult<ApiResponse<UserDto>>> SendAppAccess(int sr)
        {
            var response = new ApiResponse<UserDto>();

            try
            {
                var _user = _mapper.Map<User>(await _userRepository.FindAsync(c => c.AssigneeInformations.FirstOrDefault().ServiceRecordId == sr));

                if (_user != null)
                {

                    var guid = Guid.NewGuid().ToString().Substring(0, 7);
                    var password = _userRepository.HashPassword("$" + guid);

                    _user.Password = password;
                    _user.Reset = true;
                    _user.UpdateBy = _user.Id;
                    _user.UpdatedDate = DateTime.Now;
                    
                    _userRepository.Update(_mapper.Map<User>(_user), _user.Id);

                    StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email.html"));
                    string body = string.Empty;
                    body = reader.ReadToEnd();
                    body = body.Replace("{user}",  _user.Name + " " + _user.LastName + " " + _user.MotherLastName );
                    body = body.Replace("{username}",  $"{_user.Email}");
                    body = body.Replace("{pass}", " $" + guid);

                    _userRepository.SendMail(_user.Email, body, "App Access");

                    response.Result = _mapper.Map<UserDto>(_user);
                    response.Success = true;
                    response.Message = "success";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Hubo un error";

                }

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        
    }
}