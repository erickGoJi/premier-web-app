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
    public class GeneralContractPricingController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IGeneralContractPricingInfoRepository _generalContractPricingInfoRepository;
        public GeneralContractPricingController(IMapper mapper, ILoggerManager loggerManager, IGeneralContractPricingInfoRepository generalContractPricingInfoRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _generalContractPricingInfoRepository = generalContractPricingInfoRepository;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddGeneralContractPricingInfo", Name = "AddGeneralContractPricingInfo")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<GeneralContractPricingInfoDto>> AddGeneralContractPricingInfo([FromBody] GeneralContractPricingInfoDto dto)
        {
            var response = new ApiResponse<GeneralContractPricingInfoDto>();
            try
            {
                foreach (var j in dto.DocumentGeneralContractPricingInfos)
                {
                    j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/ClientProfile/", j.FileExtension);
                }

                GeneralContractPricingInfoDto GeneralContract = _mapper.Map<GeneralContractPricingInfoDto>(_generalContractPricingInfoRepository.Add(_mapper.Map<GeneralContractPricingInfo>(dto)));

                response.Success = true;
                response.Message = "Success";
                response.Result = GeneralContract;
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
        [HttpPut("UpdateGeneralContractPricingInfo", Name = "UpdateGeneralContractPricingInfo")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<GeneralContractPricingInfoDto>> UpdateGeneralContractPricingInfo([FromBody] GeneralContractPricingInfoDto dto)
        {
            var response = new ApiResponse<GeneralContractPricingInfoDto>();
            try
            {
                foreach (var j in dto.DocumentGeneralContractPricingInfos)
                {
                    if (j.Id != 0)
                    {
                        if (_utiltyRepository.IsBase64(j.FileRequest))
                        {
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Profile/", j.FileExtension);
                        }
                    }
                }
                GeneralContractPricingInfoDto GeneralContract = _mapper.Map<GeneralContractPricingInfoDto>(_generalContractPricingInfoRepository.Update(_mapper.Map<GeneralContractPricingInfo>(dto), dto.Id));

                response.Success = true;
                response.Message = "Success";
                response.Result = GeneralContract;
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
        [Route("GetGeneralContractPricingInfoById")]
        public ActionResult GetGeneralContractPricingInfoById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _generalContractPricingInfoRepository.Find(x => x.Id == id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
