using api.premier.ActionFilter;
using api.premier.Hubs;
using api.premier.Models;
using api.premier.Models.Conversation;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Conversation;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;

        public ChatController(IMapper mapper, ILoggerManager loggerManager, IConversationRepository conversationRepository, IUtiltyRepository utiltyRepository,
            IHubContext<MessageHub> hubContext,
            INotificationSystemRepository notificationSystemRepository, 
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _conversationRepository = conversationRepository;
            _utiltyRepository = utiltyRepository;
            _hubContext = hubContext;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        public class RealTimeMessageResponse
        {
            public int conversationId { get; set; }
            public int user1Id { get; set; }
            public int user2Id { get; set; }
            public RealTimeMessageResponse(int conversationId, int user1Id, int user2Id)
            {
                this.conversationId = conversationId;
                this.user1Id = user1Id;
                this.user2Id = user2Id;
            }
        }

        public class NewMessageModel
        {
            public int userid { get; set; }
            public List<int> userList { get; set; }
            public string message { get; set; }
            public string file { get; set; }
            public string fileExtension { get; set; }
            public bool group { get; set; }
            public string GroupName { get; set; }
        }

        //Message sent to one or multiple users
        //If theres no conversations first create a conversation between those users
        [HttpPost("SentNewMessage", Name = "SentNewMessage")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ConversationDto>>> SentNewMessage([FromBody] NewMessageModel item)
        {
            var response = new ApiResponse<List<ConversationDto>>();
            try
            {
                ConversationDto conversation = new ConversationDto();
                MessageDto message = new MessageDto();
                UserGroupDto userReciver = new UserGroupDto();
                DocumentMessageDto document = new DocumentMessageDto();
                List<ConversationDto> ConversationDto = new List<ConversationDto>();
                if (item.group)
                {
                    // Creacion Conversacion
                    conversation = new ConversationDto();
                    conversation.UserTo = item.userid;
                    conversation.UserReciver = null;
                    conversation.CreatedDate = DateTime.Now;
                    conversation.Status = false;
                    conversation.Groups = true;
                    conversation.GroupName = item.GroupName;
                    conversation.UserGroups = new List<UserGroupDto>();
                    foreach(var i in item.userList)
                    {
                        userReciver = new UserGroupDto();
                        userReciver.UserReciver = i;
                        conversation.UserGroups.Add(userReciver);
                    }
                    conversation.Messages = new List<MessageDto>();

                    // Insercion Mensaje
                    message = new MessageDto();
                    message.UserId = item.userid;
                    message.Time = DateTime.Now;
                    message.Status = false;
                    message.Message1 = item.message;
                    message.DocumentMessages = new List<DocumentMessageDto>();

                    if(item.file.Length > 150)
                    {
                        document = new DocumentMessageDto();
                        document.Date = DateTime.Now;
                        document.FilePath = _utiltyRepository.UploadImageBase64(item.file, "Files/Chat/", item.fileExtension);
                        document.Status = false;
                        message.DocumentMessages.Add(document);
                    }

                    conversation.Messages.Add(message);

                    var res = _conversationRepository.Add(_mapper.Map<Conversation>(conversation));

                    ConversationDto.Add(_mapper.Map<ConversationDto>(res));
                    conversation = _mapper.Map<ConversationDto>(res);
                }
                else
                {
                    foreach (var i in item.userList)
                    {
                        bool alreadyMessage = _conversationRepository.GetAll().Count(
                           a => (a.UserTo == i && a.UserReciver == item.userid) ||
                                (a.UserTo == item.userid && a.UserReciver == i)) > 0;
                        if (!alreadyMessage)
                        {
                            // Creacion Conversacion
                            conversation = new ConversationDto();
                            conversation.UserTo = item.userid;
                            conversation.UserReciver = i;
                            conversation.CreatedDate = DateTime.Now;
                            conversation.Status = false;
                            conversation.Groups = false;
                            conversation.Messages = new List<MessageDto>();

                            // Insercion Mensaje
                            message = new MessageDto();
                            message.UserId = item.userid;
                            message.Time = DateTime.Now;
                            message.Status = false;
                            message.Message1 = item.message;
                            message.DocumentMessages = new List<DocumentMessageDto>();

                            if (item.file.Length > 150)
                            {
                                document = new DocumentMessageDto();
                                document.Date = DateTime.Now;
                                document.FilePath = _utiltyRepository.UploadImageBase64(item.file, "Files/Chat/", item.fileExtension);
                                document.Status = false;
                                message.DocumentMessages.Add(document);
                            }

                            conversation.Messages.Add(message);
                            ConversationDto.Add(_mapper.Map<ConversationDto>(_conversationRepository.Add(_mapper.Map<Conversation>(conversation))));
                        }
                        else
                        {
                            var conversationId = _conversationRepository.GetAll().FirstOrDefault(
                                a => (a.UserTo == i && a.UserReciver == item.userid) ||
                                (a.UserTo == item.userid && a.UserReciver == i));

                            //Message message = new Message();
                            message = new MessageDto();
                            message.UserId = item.userid;
                            message.Time = DateTime.Now;
                            message.Status = false;
                            conversation.Groups = false;
                            message.Conversation = conversationId.Id;
                            message.Message1 = item.message;

                            message.DocumentMessages = new List<DocumentMessageDto>();

                            if (item.file.Length > 150)
                            {
                                document = new DocumentMessageDto();
                                document.Date = DateTime.Now;
                                document.FilePath = _utiltyRepository.UploadImageBase64(item.file, "Files/Chat/", item.fileExtension);
                                document.Status = false;
                                message.DocumentMessages.Add(document);
                            }

                            _conversationRepository.AddMessage(_mapper.Map<Message>(message));

                            conversation = _mapper.Map<ConversationDto>(conversationId);
                            ConversationDto.Add(conversation);
                        } // End else (alreadyMessage)
                    }
                }

                response.Success = true;
                response.Message = "Success";
                response.Result = ConversationDto;
                if (conversation.Groups.Value)
                {
                    foreach (var i in conversation.UserGroups)
                    {
                        RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversation.Id, conversation.UserTo.Value, i.UserReciver);
                        _hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);
                    }
                }
                else
                {
                    RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversation.Id, conversation.UserTo.Value, conversation.UserReciver.Value);
                    _hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);   // Send is the name thaw we're going to use in the client to bind the event      
                }
                
                if (conversation.Groups.Value)
                {
                    foreach (var @group in conversation.UserGroups)
                    {
                        if (@group.UserReciver != conversation.UserTo)
                        {
                            _notificationSystemRepository.SendNotificationAsync(
                                @group.UserReciver,
                                0,
                                conversation.Id,
                                _notificationSystemTypeRepository.Find(x => x.Id == 20).Type,
                                item.message,
                                1
                            );
                        }
                    }
                }
                else
                {
                    if (conversation.UserReciver != null)
                        if (conversation.UserReciver != conversation.UserTo)
                        {
                            _notificationSystemRepository.SendNotificationAsync(
                                conversation.UserReciver != item.userid
                                    ? conversation.UserReciver.Value
                                    : conversation.UserTo.Value,
                                0,
                                conversation.Id,
                                _notificationSystemTypeRepository.Find(x => x.Id == 20).Type,
                                item.message,
                                1
                            );
                        }
                }
                
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

        // Sent Message to Some User
        // With a conversation initialized
        [HttpPost("SentMessage", Name = "SentMessage")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ConversationDto>> SentMessage([FromBody] MessageDto item)
        {
            var response = new ApiResponse<MessageDto>();
            try
            {
                foreach(var i in item.DocumentMessages)
                {
                    if (i.FilePath.Length > 150)
                        i.FilePath = _utiltyRepository.UploadImageBase64(i.FilePath, "Files/Chat/", i.FileExtension);
                }
                var messageAdded = _conversationRepository.AddMessage(_mapper.Map<Message>(item));
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<MessageDto>(messageAdded);
                var conversation = _conversationRepository.GetAllIncluding(x => x.UserGroups, c => c.UserReciverNavigation).FirstOrDefault(x => x.Id == item.Conversation);
                RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(item.Conversation.Value, conversation.UserTo.Value, conversation.UserReciver.HasValue 
                    ? conversation.UserReciver.Value : conversation.UserGroups.Select(s => s.UserReciver).FirstOrDefault());
                _hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);   // Send is the name thaw we're going to use in the client to bind the event      
                // Send Notification Push\
                if (conversation.Groups.Value)
                {
                    foreach (var @group in conversation.UserGroups)
                    {
                        if (@group.UserReciver != item.UserId)
                        {
                            _notificationSystemRepository.SendNotificationAsync(
                                @group.UserReciver,
                                0,
                                item.Conversation.Value,
                                conversation.GroupName,
                                item.Message1,
                                20
                            );
                        }
                    }
                }
                else
                {
                    _notificationSystemRepository.SendNotificationAsync(
                        conversation.UserTo.Value,
                        0,
                        item.Conversation.Value,
                        conversation.UserReciverNavigation.Name,
                        item.Message1,
                        20
                    );
                }
                
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

        // Get List Conversations
        // [Authorize] 
        [HttpGet("SeeChatsApp", Name = "SeeChatsApp")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversationApp([FromQuery] int user)
        {
            try
            {
                return Ok(new { Success = true, Result = _conversationRepository.SeeChatsApp(user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("SeeChats", Name = "SeeChats")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversation([FromQuery] int user)
        {
            try
            {
                return Ok(new { Success = true, Result = _conversationRepository.SeeChats(user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        // Get Conversation by Id
        // [Authorize] 
        [HttpGet("SeeChatsById", Name = "SeeChatsById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversationById([FromQuery] int user, int conversationId)
        {
            try
            {
                return Ok(new { Success = true, Result = _conversationRepository.SeeChatsById(user, conversationId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        //Conversation View in the Client
        [HttpGet("GetConversation/{conversationId}/{user}", Name = "GetConversation/{conversationId}/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversationUserConsierge(int conversationId, int user)
        {
            try
            {
                var conversation = _conversationRepository.GetConversation(conversationId, user);
                //RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversationId, user, user);
                //_hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Conversation View in the Client Push App
        [HttpGet("GetConversationAppPush/{conversationId}/{user}", Name = "GetConversationAppPush/{conversationId}/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversationAppPush(int conversationId, int user)
        {
            try
            {
                var conversation = _conversationRepository.GetConversationAppPush(conversationId, user);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Conversation View in the Client Complete
        [HttpGet("GetConversationComplete/{conversationId}/{user}", Name = "GetConversationComplete/{conversationId}/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversationComplete(int conversationId, int user)
        {
            try
            {
                var conversation = _conversationRepository.GetConversationComplete(conversationId, user);
                //RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversationId, user, user);
                //_hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Conversation View in the Client Complete
        [HttpGet("GetConversationCompleteApp/{conversationId}/{user}", Name = "GetConversationCompleteApp/{conversationId}/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConversationCompleteApp(int conversationId, int user)
        {
            try
            {
                var conversation = _conversationRepository.GetConversationCompleteApp(conversationId, user);
                //RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversationId, user, user);
                //_hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        [HttpDelete("DeleteConversation/{key}", Name = "DeleteConversation/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ConversationDto>> DeleteConversation(int key)
        {
            var response = new ApiResponse<ConversationDto>();
            try
            {
                var conversation = _conversationRepository.Find(x => x.Id == key);
                _conversationRepository.Delete(conversation);
                response.Result = null;
                response.Message = "Conversation delete";
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

        [HttpDelete("DeleteMessage/{key}", Name = "DeleteMessage/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteMessage(int key)
        {
            try
            {
                var message = _conversationRepository.DeleteMessage(key);
                return Ok(new { Success = true, Result = message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Get User List To Conversation
        [HttpGet("GetUserList/{user}/{country}", Name = "GetUserList/{user}/{country}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetUserList(int user, int country)
        {
            try
            {
                var conversation = _conversationRepository.GetUserList(user, country);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Get User List To Conversation
        [HttpGet("GetUserListConversation/{conversationId}", Name = "GetUserListConversation/{conversationId}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetUserListConversation(int conversationId)
        {
            try
            {
                var conversation = _conversationRepository.GetUserListConversation(conversationId);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Get User List To Conversation
        [HttpGet("GetChatNotification/{user}", Name = "GetChatNotification/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetChatNotification(int user)
        {
            try
            {
                var conversation = _conversationRepository.GetChatNotification(user);
                return Ok(new { Success = true, Result = conversation });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        //Get User List To Conversation 
        [HttpGet("GetChatNotificationCount/{user}", Name = "GetChatNotificationCount/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetChatNotificationCount(int user)
        {
            try
            {
                int countM = _conversationRepository.GetChatNotificationCount(user).Value;
                int countN = _conversationRepository.GetNotificationCount(user).Value;

                return Ok(new { Success = true, Result = countM.ToString() + "/" + countN.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Get UpdateToSeeNotification
        [HttpGet("UpdateToSeeNotification/{user}", Name = "UpdateToSeeNotification/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult UpdateToSeeNotification(int user)
        {
            try
            {
                var notification = _conversationRepository.UpdateToSeeNotification(user);
                return Ok(new { Success = true, Result = "File Update" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        //Get UpdateToSeeMessage
        [HttpGet("UpdateToSeeMessage/{user}", Name = "UpdateToSeeMessage/{user}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult UpdateToSeeMessage(int user, int converssationId)
        {
            try
            {
                var notification = _conversationRepository.UpdateToSeeMessage(user, converssationId);
                return Ok(new { Success = true, Result = "File Update" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return BadRequest(new { Success = false, Message = $"Something went wrong: {ex.Message.ToString()}" });
            }
        }

        [HttpPut("Check/{message}/{user}", Name = "Check")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<bool>> UpdateMessage(int message, int user, int conversationId, int userReciver)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                response.Result = _conversationRepository.CheckMessage(message, user);

                RealTimeMessageResponse rtmessage = new RealTimeMessageResponse(conversationId, user, userReciver);
                _hubContext.Clients.All.SendAsync("ReceiveOne", rtmessage);

                response.Success = true;
                response.Message = "Operation was success";
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

    }
}
