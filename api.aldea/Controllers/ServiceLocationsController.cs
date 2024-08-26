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

namespace api.premier.Controllers
{
    public class ServiceLocationsController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IServiceLocationsRepository _serviceLocationsRepository;
        public ServiceLocationsController(IMapper mapper, ILoggerManager loggerManager, IServiceLocationsRepository serviceLocationsRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _serviceLocationsRepository = serviceLocationsRepository;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddServiceLocation", Name = "AddServiceLocation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ServiceLocationDto>>> AddServiceLocation([FromBody] ServiceLocationDto dto)
        {
            var response = new ApiResponse<List<ServiceLocationDto>>();
            try
            {
                List<ServiceLocationDto> dtos = new List<ServiceLocationDto>();
                ServiceLocationCountryDto countryDto = new ServiceLocationCountryDto();
                ServiceLocationDto locationDto = new ServiceLocationDto();

                foreach (var j in dto.ServiceLocationCountries)
                {
                    if (j.StandarScopeDocuments == 0)
                    {
                        foreach (var i in j.DocumentLocationCountries) {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ClientProfile/", i.FileExtension);
                        }
                    }
                }

                foreach (var i in dto.Services)
                {
                    locationDto = new ServiceLocationDto()
                    {
                        Id = 0,
                        IdService = i,
                        NickName = dto.NickName,
                        IdServiceLine = dto.IdServiceLine,
                        IdClientPartnerProfile = dto.IdClientPartnerProfile,
                        ServiceLocationCountries = new List<ServiceLocationCountryDto>()
                    };
                    foreach (var country in dto.ServiceLocationCountries.FirstOrDefault().Countries)
                    {
                        locationDto.ServiceLocationCountries.Add(new ServiceLocationCountryDto()
                        {
                            Id = 0,
                            IdCountry = country,
                            ScopeDescription = dto.ServiceLocationCountries.FirstOrDefault().ScopeDescription,
                            IdServiceLocation = 0,
                            StandarScopeDocuments = dto.ServiceLocationCountries.FirstOrDefault().StandarScopeDocuments,
                            DocumentLocationCountries = dto.ServiceLocationCountries.FirstOrDefault().DocumentLocationCountries
                        });
                    }
                    
                    dtos.Add(locationDto);
                }

                foreach (var serviceLocationDto in dtos)
                {
                    _mapper.Map<ServiceLocationDto>(_serviceLocationsRepository.Add(_mapper.Map<ServiceLocation>(serviceLocationDto)));
                }

                // ServiceLocationDto service = _mapper.Map<ServiceLocationDto>(_serviceLocationsRepository.Add(_mapper.Map<ServiceLocation>(dto)));

                response.Success = true;
                response.Message = "Success";
                response.Result = dtos;
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
        [HttpPut("UpdateServiceLocation", Name = "UpdateServiceLocation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeContactDto>> UpdateServiceLocation([FromBody] ServiceLocationDto dto)
        {
            var response = new ApiResponse<ServiceLocationDto>();
            try
            {
                foreach (var j in dto.ServiceLocationCountries)
                {
                    if (j.StandarScopeDocuments == 0)
                    {
                        foreach (var i in j.DocumentLocationCountries)
                        {
                            if (i.Id != 0)
                            {
                                if (_utiltyRepository.IsBase64(i.FileRequest))
                                {
                                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Profile/", i.FileExtension);
                                }
                            }
                            else {
                                if (_utiltyRepository.IsBase64(i.FileRequest))
                                {
                                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Profile/", i.FileExtension);
                                }
                            }
                        }
                    }
                }

                ServiceLocationDto service = _mapper.Map<ServiceLocationDto>(_serviceLocationsRepository.UpdateCustom(_mapper.Map<ServiceLocation>(dto), dto.Id));

                response.Success = true;
                response.Message = "Success";
                response.Result = service;
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
        [Route("GetServiceLocationById")]
        public ActionResult GetOfficeContactById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _serviceLocationsRepository.GetServiceLocationById(id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
