using api.premier.ActionFilter;
using api.premier.Models;
using AutoMapper;
using biz.premier.Repository.ClientPartner;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using System;
using api.premier.Models.ClientPartnerProfile;
using biz.premier.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using Microsoft.EntityFrameworkCore.Internal;
using Task = System.Threading.Tasks.Task;

namespace api.premier.Controllers
{
    public class Client_PartnerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IClient_PartnerRepository _client_PartnerRepository;
        private readonly IDocumentClientPartnerProfileRepository _documentClientPartnerProfileRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        public Client_PartnerController(IMapper mapper, ILoggerManager loggerManager, IClient_PartnerRepository client_PartnerRepository, 
            IUtiltyRepository utiltyRepository, IDocumentClientPartnerProfileRepository documentClientPartnerProfileRepository,
            INotificationSystemRepository notificationSystemRepository, ICatNotificationSystemTypeRepository catNotificationSystemTypeRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _client_PartnerRepository = client_PartnerRepository;
            _utiltyRepository = utiltyRepository;
            _documentClientPartnerProfileRepository = documentClientPartnerProfileRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = catNotificationSystemTypeRepository;
        }

        // Post Create new Profile 
        [HttpPost("AddClientPartnerProfile", Name = "AddClientPartnerProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<ClientPartnerProfileDto>>> PostClientPartnerProfile([FromBody] ClientPartnerProfileDto dto)
        {
            var response = new ApiResponse<ClientPartnerProfileDto>();
            try
            {
                if (dto.Photo != null && dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/ClientProfile/", dto.PhotoExtension);
                }

                foreach (var j in dto.DocumentClientPartnerProfiles)
                {
                    j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/ClientProfile/", j.FileExtension);
                }

                foreach (var j in dto.GeneralContractPricingInfos)
                {
                    foreach (var i in j.DocumentGeneralContractPricingInfos)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ClientProfile/", i.FileExtension);
                    }                    
                }

                foreach (var j in dto.OfficeInformations)
                {
                    foreach (var i in j.DocumentOfficeInformations)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ClientProfile/", i.FileExtension);
                    }
                }

                foreach (var j in dto.ServiceLocations)
                {
                    foreach (var i in j.ServiceLocationCountries)
                    {
                        foreach (var x in i.DocumentLocationCountries)
                        {
                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/ClientProfile/", x.FileExtension);
                        }
                    }
                }
                if(dto.BelongsToPartner != null)
                {
                    dto.ClientPartnerProfileClientIdClientToNavigations = new List<ClientPartnerProfileClientDto>();
                    ClientPartnerProfileClientDto client = new ClientPartnerProfileClientDto();
                    client.IdClientFrom = dto.BelongsToPartner.Value;
                    client.IdClientTo = 0;
                    dto.ClientPartnerProfileClientIdClientToNavigations.Add(client);
                }
                dto.IdStatus = 1;
                dto.CreatedDate = DateTime.Now;

                if (dto.ServiceLocations.Any())
                {
                    List<ServiceLocationDto> dtos = new List<ServiceLocationDto>();
                    ServiceLocationCountryDto countryDto = new ServiceLocationCountryDto();
                    ServiceLocationDto locationDto = new ServiceLocationDto();
                    foreach (var serviceLocation in dto.ServiceLocations.FirstOrDefault().Services)
                    {
                        locationDto = new ServiceLocationDto()
                        {
                            Id = 0,
                            IdService = serviceLocation,
                            NickName = dto.ServiceLocations.FirstOrDefault()?.NickName,
                            IdServiceLine = dto.ServiceLocations.FirstOrDefault().IdServiceLine,
                            IdClientPartnerProfile = dto.ServiceLocations.FirstOrDefault().IdClientPartnerProfile,
                            ServiceLocationCountries = new List<ServiceLocationCountryDto>()
                        };
                        foreach (var country in dto.ServiceLocations.FirstOrDefault().ServiceLocationCountries.FirstOrDefault().Countries)
                        {
                            locationDto.ServiceLocationCountries.Add(new ServiceLocationCountryDto()
                            {
                                Id = 0,
                                IdCountry = country,
                                ScopeDescription = dto.ServiceLocations.FirstOrDefault()?.ServiceLocationCountries.FirstOrDefault()?.ScopeDescription,
                                IdServiceLocation = 0,
                                StandarScopeDocuments = dto.ServiceLocations.FirstOrDefault().ServiceLocationCountries.FirstOrDefault().StandarScopeDocuments,
                                DocumentLocationCountries = dto.ServiceLocations.FirstOrDefault()?.ServiceLocationCountries.FirstOrDefault()?.DocumentLocationCountries
                            });
                        }

                        dtos.Add(locationDto);
                    }
                    dto.ServiceLocations.Clear();
                    dto.ServiceLocations = dtos;
                }

                ClientPartnerProfileDto profileUser = _mapper.Map<ClientPartnerProfileDto>(_client_PartnerRepository.Add(_mapper.Map<ClientPartnerProfile>(dto)));

                if (profileUser.ClientPartnerProfileExperienceTeams.Any())
                {
                    foreach (var experienceTeam in profileUser.ClientPartnerProfileExperienceTeams)
                    {
                        var type = (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 25)).Type;
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Id = 0,
                            Archive = false,
                            View = false,
                            ServiceRecord = (int?)null,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = (int?)null,
                            UserTo = experienceTeam.UserId,
                            NotificationType = 25,
                            Description = $"{profileUser.Name}, {type}",
                            Color = "#6a1b9a",
                            CreatedDate = DateTime.Now
                        });
                        await _notificationSystemRepository.SendNotificationAsync(
                            experienceTeam.UserId,
                            0,
                            0,
                            type,
                            $"{profileUser.Name}, {type}",
                            2
                        );
                    }
                    
                }
                response.Success = true;
                response.Message = "Success";
                response.Result = profileUser;
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
        [HttpPut("UpdateClientPartnerProfile", Name = "UpdateClientPartnerProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ClientPartnerProfileDto>> UpdateClientPartnerProfile([FromBody] ClientPartnerProfileDto dto)
        {
            var response = new ApiResponse<ClientPartnerProfileDto>();
            try
            {
                if (dto.Photo != null && dto.Photo.Length > 150)
                {
                    if (_utiltyRepository.IsBase64(dto.Photo))
                    {
                        dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/ClientProfile/", dto.PhotoExtension);
                    }
                }
                foreach (var j in dto.DocumentClientPartnerProfiles)
                {
                    if (j.Id != 0)
                    {
                        if (_utiltyRepository.IsBase64(j.FileRequest))
                        {
                            DocumentClientPartnerProfile document = _documentClientPartnerProfileRepository.Find(x => x.Id == j.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Profile/", j.FileExtension);
                        }
                    }                    
                }
                foreach (var j in dto.GeneralContractPricingInfos)
                {
                    foreach (var i in j.DocumentGeneralContractPricingInfos)
                    {
                        if (j.Id != 0)
                        {
                            if (_utiltyRepository.IsBase64(i.FileRequest))
                            {
                                i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ClientProfile/", i.FileExtension);
                            }
                        }
                    }
                }
                foreach (var j in dto.OfficeInformations)
                {
                    foreach (var i in j.DocumentOfficeInformations)
                    {
                        if (_utiltyRepository.IsBase64(i.FileRequest))
                        {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ClientProfile/", i.FileExtension);
                        }
                    }
                }
                foreach (var j in dto.ServiceLocations)
                {
                    foreach (var i in j.ServiceLocationCountries)
                    {
                        foreach (var x in i.DocumentLocationCountries)
                        {
                            if (_utiltyRepository.IsBase64(x.FileRequest))
                            {
                                x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/ClientProfile/", x.FileExtension);
                            }
                        }
                    }
                }
                if (dto.IdTypePartnerClientProfile == 2)
                    _client_PartnerRepository.UpdatePartner(dto.Id, dto.BelongsToPartner.Value);
                ClientPartnerProfileDto profileUser = _mapper.Map<ClientPartnerProfileDto>(_client_PartnerRepository.Update(_mapper.Map<ClientPartnerProfile>(dto), dto.Id));

                response.Success = true;
                response.Message = "Success";
                response.Result = profileUser;
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
        [Route("GetClientPartnerProfile")]
        public ActionResult GetClientPartnerProfile(DateTime? date_range_in, DateTime? date_range_fi, int? type, 
            int? company_type, int? country, int? city, int? status, int lead_or_client, int? state)
        {
            try
            {
                return Ok(new { Success = true, result = _client_PartnerRepository.GetClientPartner(date_range_in, date_range_fi, type, company_type, country, city, status, lead_or_client, state) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetClientPartnerProfileCatalog")]
        public ActionResult GetClientPartnerProfileCatalog()
        {
            try
            {
                return Ok(new { Success = true, result = _client_PartnerRepository.GetClientPartnerCatalog() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetExperienceTeamCatalog/{serviceLine}")]
        public ActionResult GetExperienceTeamCatalog(int serviceLine, int? client)
        {
            try
            {
                return Ok(new { Success = true, result = _client_PartnerRepository.GetExperienceTeamCatalog(serviceLine, client) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetClientPartnerProfileById")]
        public async Task<ActionResult> GetClientPartnerProfileById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _client_PartnerRepository.GetClientPartnerBytestId(id)  });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetServiceLocation")]
        public async Task<ActionResult> GetServiceLocation(int id, int serviceLine)
        {
            try
            {
                return Ok(new { Success = true, result = _client_PartnerRepository.GetServiceLocation(id, serviceLine) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
