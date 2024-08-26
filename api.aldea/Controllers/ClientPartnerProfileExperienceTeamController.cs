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
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;

namespace api.premier.Controllers
{
    public class ClientPartnerProfileExperienceTeamController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IClientPartnerProfileExperienceTeamRepository _clientPartnerProfileExperienceTeamRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        public ClientPartnerProfileExperienceTeamController(
            IMapper mapper, 
            ILoggerManager loggerManager,
            IClientPartnerProfileExperienceTeamRepository clientPartnerProfileExperienceTeamRepository, 
            IUtiltyRepository utiltyRepository,
            INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository catNotificationSystemTypeRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _clientPartnerProfileExperienceTeamRepository = clientPartnerProfileExperienceTeamRepository;
            _utiltyRepository = utiltyRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = catNotificationSystemTypeRepository;
        }

        // Post a Profile 
        [HttpPut("AddClientPartnerProfile", Name = "AddClientPartnerProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<ClientPartnerProfileExperienceTeamDto>>> AddClientPartnerProfile([FromBody] ClientPartnerProfileExperienceTeamDto dto)
        {
            var response = new ApiResponse<ClientPartnerProfileExperienceTeamDto>();
            try
            {
                ClientPartnerProfileExperienceTeamDto service = 
                    _mapper.Map<ClientPartnerProfileExperienceTeamDto>(
                        _clientPartnerProfileExperienceTeamRepository.Add(_mapper.Map<ClientPartnerProfileExperienceTeam>(dto))
                        );

                response.Success = true;
                response.Message = "Success";
                response.Result = service;

                var client = _clientPartnerProfileExperienceTeamRepository
                    .GetAllIncluding(g => g.IdClientPartnerProfileNavigation)
                    .FirstOrDefault(f => f.IdClientPartnerProfile == service.IdClientPartnerProfile)?
                    .IdClientPartnerProfileNavigation.Name;
                
                var type = (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 25)).Type;
                _notificationSystemRepository.Add(new NotificationSystem()
                {
                    Id = 0,
                    Archive = false,
                    View = false,
                    ServiceRecord = (int?)null,
                    Time = DateTime.Now.TimeOfDay,
                    UserFrom = (int?)null,
                    UserTo = service.UserId,
                    NotificationType = 25,
                    Description = $"{client}, {type}",
                    Color = "#ad1457",
                    CreatedDate = DateTime.Now
                });
                await _notificationSystemRepository.SendNotificationAsync(
                    service.UserId,
                    0,
                    0,
                    type,
                    $"{client}, {type}",
                    2
                );
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

        [HttpDelete]
        [Route("DeleteClientPartnerProfile")]
        public ActionResult DeleteClientPartnerProfile(int id)
        {
            try
            {
                var client = _clientPartnerProfileExperienceTeamRepository.Find(c => c.Id == id);
                if (client != null)
                {
                    _clientPartnerProfileExperienceTeamRepository.Delete(_mapper.Map<ClientPartnerProfileExperienceTeam>(client));
                    return Ok(new { Success = true, Result = "Document delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Document Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
