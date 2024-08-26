using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ChatImmigrationRelocation;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Chat_Immigration_Relocation;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using api.premier.Hubs;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatImmigrationRelocationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IChatImmigrationRelocationRepository _chatImmigrationRelocationRepository;
        private readonly IChatCommentImmigrationRelocationRepository _chatCommentImmigrationRelocationRepository;
        private readonly IUtiltyRepository _utiltyRepository; 
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;


        public ChatImmigrationRelocationController(
            IMapper mapper,
            ILoggerManager logger,
            IChatImmigrationRelocationRepository chatImmigrationRelocationRepository,
            IChatCommentImmigrationRelocationRepository chatCommentImmigrationRelocationRepository,
            IUtiltyRepository utiltyRepository,
            IHubContext<MessageHub> hubContext,
            INotificationSystemRepository notificationSystemRepository, 
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _chatImmigrationRelocationRepository = chatImmigrationRelocationRepository;
            _chatCommentImmigrationRelocationRepository = chatCommentImmigrationRelocationRepository;
            _utiltyRepository = utiltyRepository;
            _hubContext = hubContext;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        // Post Create a new Coversation
        [HttpPost("CreateConversation", Name = "CreateConversation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ChatConversationImmigrationRelocationDto>>> PostConvertarion([FromBody] List<ChatConversationImmigrationRelocationDto> dto)
        {
            var response = new ApiResponse<List<ChatConversationImmigrationRelocationDto>>();
            try
            {
                List<ChatConversationImmigrationRelocationDto> convertation = new List<ChatConversationImmigrationRelocationDto>();
                foreach (var i in dto)
                {
                    i.CreatedDate = DateTime.Now;
                    foreach (var j in i.ChatImmigrationRelocations)
                    {
                        j.DateComment = DateTime.Now;
                        foreach (var x in j.ChatDocumentImmigrationRelocations)
                        {
                            x.DateComment = DateTime.Now;
                            x.FilePath = _utiltyRepository.UploadImageBase64(x.FilePath, "Files/Chat_Immigration_Relocation/", x.FileExtension);
                        }
                    }
                    _hubContext.Clients.All.SendAsync("ReceiveOne", i);
                    convertation.Add(_mapper.Map<ChatConversationImmigrationRelocationDto>(_chatImmigrationRelocationRepository.Add(_mapper.Map<ChatConversationImmigrationRelocation>(i))));
                    // Send Push Notification
                    if (i.ServiceRecordId != null)
                    {
                        int?[] users =
                            _notificationSystemRepository.GetExperienceTeam(i.ServiceRecordId.Value, i.ServiceLineId);
                        foreach (var user in users)
                        {
                            if (user.HasValue)
                            {
                                if (user.Value != i.CreatedBy)
                                {
                                    _notificationSystemRepository.Add(new NotificationSystem()
                                    {
                                        Id = 0,
                                        Archive = false,
                                        View = false,
                                        ServiceRecord = i.ServiceRecordId,
                                        Time = DateTime.Now.TimeOfDay,
                                        UserFrom = i.CreatedBy,
                                        UserTo = i.ChatImmigrationRelocations.FirstOrDefault().UserId,
                                        NotificationType = 20,
                                        Description = "New chat, go to the community tab on the SR-" + i.ServiceRecordId,
                                        Color = "#f06689",
                                        CreatedDate = DateTime.Now
                                    });
                                    _notificationSystemRepository.Save();
                                    _notificationSystemRepository.SendNotificationAsync(
                                        user.Value,
                                        0,
                                        convertation.FirstOrDefault().Id,
                                        _notificationSystemTypeRepository.Find(x => x.Id == 20).Type,
                                        convertation.FirstOrDefault()?.ChatImmigrationRelocations.FirstOrDefault()?.Comment,
                                        20
                                    );
                                }
                            }
                        }
                    }
                }

                response.Success = true;
                response.Message = "Success";
                response.Result = convertation;
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

        // Post Create a new Comment
        [HttpPost("CreateComment", Name = "CreateComment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ChatImmigrationRelocationDto>>> PostComment([FromBody] List<ChatImmigrationRelocationDto> dto)
        {
            var response = new ApiResponse<List<ChatImmigrationRelocationDto>>();
            try
            {
                List<ChatImmigrationRelocationDto> comment = new List<ChatImmigrationRelocationDto>();
                foreach (var i in dto)
                {
                    i.DateComment = DateTime.Now;
                    foreach (var x in i.ChatDocumentImmigrationRelocations)
                    {
                        x.DateComment = DateTime.Now;
                        x.FilePath = _utiltyRepository.UploadImageBase64(x.FilePath, "Files/Chat_Immigration_Relocation/", x.FileExtension);
                    }

                    _hubContext.Clients.All.SendAsync("ReceiveOne", i);
                    comment.Add(_mapper.Map<ChatImmigrationRelocationDto>(_chatCommentImmigrationRelocationRepository.Add(_mapper.Map<ChatImmigrationRelocation>(i))));
                    // Send Push Notification
                    var chat = _chatImmigrationRelocationRepository.Find(x => x.Id == i.ChatCoversationId);
                    if (chat.ServiceRecordId != null)
                    {
                        int?[] users =
                            _notificationSystemRepository.GetExperienceTeam(chat.ServiceRecordId.Value, chat.ServiceLineId);
                        foreach (var user in users)
                        {
                            if(user.HasValue)
                                if (user.Value != i.UserId)
                                {
                                    _notificationSystemRepository.SendNotificationAsync(
                                        user.Value,
                                        0,
                                        comment.FirstOrDefault().Id,
                                        _notificationSystemTypeRepository.Find(x => x.Id == 20).Type,
                                        comment.FirstOrDefault()?.Comment,
                                        1
                                    );
                                }
                        }
                    }
                }

                response.Success = true;
                response.Message = "Success";
                response.Result = comment;
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

        // Get List Conversation
        [HttpGet("GetConversation", Name = "GetConversation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ChatImmigrationRelocationDto>>> GetConversation([FromQuery] int Service_record_id, int type, int? user)
        {
            try
            {
                return Ok(new { Success = true, Result = _chatImmigrationRelocationRepository.GetConversation(Service_record_id, type, user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
        
        // Get List Conversation By User
        [HttpGet("GetListConversationByUser", Name = "GetListConversationByUser")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetListConversationByUser(int user)
        {
            try
            {
                return StatusCode(202,
                    new
                    {
                        Success = true, Result = _chatImmigrationRelocationRepository.GetConversationsByUser(user),
                        Message = ""
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(404, new { Success = false, Result = 0, Message = ex.Message.ToString() });
            }
        }
        // Get List User Team
        [HttpGet("GetUsersTeam", Name = "GetUsersTeam")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetUsersTeam(int conversation)
        {
            try
            {
                return StatusCode(202,
                    new
                    {
                        Success = true, Result = _chatImmigrationRelocationRepository.GetUsersTeam(conversation),
                        Message = ""
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(404, new { Success = false, Result = 0, Message = ex.Message.ToString() });
            }
        }
    }
}
