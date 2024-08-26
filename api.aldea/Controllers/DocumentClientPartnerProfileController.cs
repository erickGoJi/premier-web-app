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
    public class DocumentClientPartnerProfileController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IDocumentClientPartnerProfileRepository _documentClientPartnerProfileRepository;
        public DocumentClientPartnerProfileController(IMapper mapper, ILoggerManager loggerManager, IDocumentClientPartnerProfileRepository documentClientPartnerProfileRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _documentClientPartnerProfileRepository = documentClientPartnerProfileRepository;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddDocumentClientPartnerProfile", Name = "AddDocumentClientPartnerProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentClientPartnerProfileDto>> AddDocumentClientPartnerProfile([FromBody] DocumentClientPartnerProfileDto dto)
        {
            var response = new ApiResponse<DocumentClientPartnerProfileDto>();
            try
            {
                if (dto.FileRequest != null && dto.FileRequest.Length > 150)
                {
                    if (_utiltyRepository.IsBase64(dto.FileRequest))
                    {
                        dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/ClientProfile/", dto.FileExtension);
                    }
                }

                DocumentClientPartnerProfileDto service = _mapper.Map<DocumentClientPartnerProfileDto>(_documentClientPartnerProfileRepository.Add(_mapper.Map<DocumentClientPartnerProfile>(dto)));

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

        [HttpDelete]
        [Route("DeleteDocumentClientPartnerProfile")]
        public ActionResult DeleteDocumentClientPartnerProfile(int id)
        {
            try
            {
                var client = _documentClientPartnerProfileRepository.Find(c => c.Id == id);
                if (client != null)
                {
                    _documentClientPartnerProfileRepository.Delete(_mapper.Map<DocumentClientPartnerProfile>(client));
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
