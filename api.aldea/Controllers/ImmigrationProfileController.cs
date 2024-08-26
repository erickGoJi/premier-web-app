using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.DependentImmigrationInfo;
using api.premier.Models.DependentInformations;
using api.premier.Models.ImmigrationProfile;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.ImmigrationProfile;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImmigrationProfileController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IImmigrationProfileRepository _immigrationProfileRepository;
        private readonly IDocumentDependentImmigration _documentDependentImmigration;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IUtiltyRepository _utiltyRepository;

        public ImmigrationProfileController(IImmigrationProfileRepository immigrationProfileRepository, IMapper mapper,
            ILoggerManager logger, IServiceRecordRepository serviceRecordRepository, IUtiltyRepository utiltyRepository,
            IDocumentDependentImmigration documentDependentImmigration)
        {
            _immigrationProfileRepository = immigrationProfileRepository;
            _documentDependentImmigration = documentDependentImmigration;
            _serviceRecordRepository = serviceRecordRepository;
            _logger = logger;
            _mapper = mapper;
            _utiltyRepository = utiltyRepository;
        }

        [HttpPost("CreateImmigrationProfile", Name = "CreateImmigrationProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationProfileDto>> CreateImmigrationProfile([FromBody] ImmigrationProfileInsertDto dto)
        {
            var response = new ApiResponse<ImmigrationProfileDto>();
            try
            {
                var request = _serviceRecordRepository.Find(c => c.Id == dto.ServiceRecordId);
                var immProfile = _immigrationProfileRepository.Find(c => c.Id == dto.Id);
                if (request != null)
                {
                    if (immProfile == null)
                    {
                        //if (dto.AssigmentInformation.DocumentType != null)
                        //{
                        //    dto.AssigmentInformation.DocumentType = _utiltyRepository.UploadImageBase64
                        //        (dto.AssigmentInformation.DocumentType, "Files/AssigmentInformation/", dto.AssigmentInformation.DocumentTypeExtension);
                        //}
                        //if (dto.AssigmentInformation.LicenseDriver != null)
                        //{
                        //    dto.AssigmentInformation.LicenseDriver = _utiltyRepository.UploadImageBase64
                        //        (dto.AssigmentInformation.LicenseDriver, "Files/AssigmentInformation/", dto.AssigmentInformation.LicenseDriverExtension);
                        //}
                        foreach (var i in dto.DependentImmigrationInfos)
                        {
                            foreach (var s in i.DocumentDependentImmigrationInfos)
                            {
                                if (s.FileRequest != null)
                                {
                                    s.FileRequest = _utiltyRepository.UploadImageBase64(s.FileRequest, "Files/DependentImmigrationInfos/", s.FileExtension);
                                }
                                _documentDependentImmigration.Add(_mapper.Map<DocumentDependentInformation>(s));
                            }

                        }
                        ImmigrationProfile immigration = _immigrationProfileRepository.Add(_mapper.Map<ImmigrationProfile>(dto));
                        response.Success = true;
                        response.Result = _mapper.Map<ImmigrationProfileDto>(immigration);
                    }
                    else
                    {
                        //if (dto.AssigmentInformation.DocumentType.Length > 150 && dto.AssigmentInformation.DocumentType != null)
                        //{
                        //    dto.AssigmentInformation.DocumentType = _utiltyRepository.UploadImageBase64
                        //    (dto.AssigmentInformation.DocumentType, "Files/AssigmentInformation/", dto.AssigmentInformation.DocumentTypeExtension);
                        //}
                        //else if (dto.AssigmentInformation.DocumentType.Length <= 100)
                        //{
                        //    _utiltyRepository.DeleteFile(immProfile.AssigmentInformation.DocumentType);
                        //    dto.AssigmentInformation.DocumentType = _utiltyRepository.UploadImageBase64
                        //        (dto.AssigmentInformation.DocumentType, "Files/AssigmentInformation/", dto.AssigmentInformation.DocumentTypeExtension);
                        //}

                        //if (dto.AssigmentInformation.LicenseDriver != null)
                        //{
                        //    dto.AssigmentInformation.LicenseDriver = _utiltyRepository.UploadImageBase64
                        //        (dto.AssigmentInformation.LicenseDriver, "Files/AssigmentInformation/", dto.AssigmentInformation.LicenseDriverExtension);
                        //}
                        //else if (dto.AssigmentInformation.LicenseDriver.Length <= 100)
                        //{
                        //    _utiltyRepository.DeleteFile(immProfile.AssigmentInformation.LicenseDriver);
                        //    dto.AssigmentInformation.LicenseDriver = _utiltyRepository.UploadImageBase64
                        //        (dto.AssigmentInformation.LicenseDriver, "Files/AssigmentInformation/", dto.AssigmentInformation.LicenseDriverExtension);
                        //}

                        foreach (var i in dto.DependentImmigrationInfos)
                        {
                            if (i.DocumentDependentImmigrationInfos != null) {
                                foreach (var s in i.DocumentDependentImmigrationInfos)
                                {
                                    if (s.FileRequest.Length > 150 && s.FileRequest != null)
                                    {
                                        s.FileRequest = _utiltyRepository.UploadImageBase64(s.FileRequest, "Files/DependentImmigrationInfos/", s.FileExtension);
                                    }
                                }
                            }
                        }
                        ImmigrationProfile immigration = _immigrationProfileRepository.UpdateCustom(_mapper.Map<ImmigrationProfile>(dto));
                        response.Success = true;
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Service Record Not Found";
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

        [HttpPut("UpdateImmigrationProfileProvisional", Name = "UpdateImmigrationProfileProvisional")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationProfileDto>> UpdateImmigrationProfileProvisional([FromBody] ImmigrationProfileInsertDto dto)
        {
            var response = new ApiResponse<ImmigrationProfileDto>();
            try
            {
                var request = _immigrationProfileRepository.Find(c => c.Id == dto.Id);
                if (request != null)
                {
                    ImmigrationProfile immigration = _immigrationProfileRepository.UpdateCustom(_mapper.Map<ImmigrationProfile>(dto));
                    response.Success = true;
                    response.Result = _mapper.Map<ImmigrationProfileDto>(immigration);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Immigration Profile Not Found";
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

        [HttpPut("UpdateImmigrationProfile", Name = "UpdateImmigrationProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationProfileDto>> UpdateImmigrationProfile([FromBody] ImmigrationProfileInsertDto dto)
        {
            var response = new ApiResponse<ImmigrationProfileDto>();
            try
            {
                var request = _immigrationProfileRepository.Find(c => c.Id == dto.Id);
                if (request != null)
                {
                    if (dto.AssigmentInformation.DocumentType != null)
                    {
                        dto.AssigmentInformation.DocumentType = _utiltyRepository.UploadImageBase64
                            (dto.AssigmentInformation.DocumentType, "Files/AssigmentInformation/", dto.AssigmentInformation.DocumentTypeExtension);
                    }
                    else if (dto.AssigmentInformation.DocumentType.Length <= 100)
                    {
                        _utiltyRepository.DeleteFile(request.AssigmentInformation.DocumentType);
                        dto.AssigmentInformation.DocumentType = _utiltyRepository.UploadImageBase64
                            (dto.AssigmentInformation.DocumentType, "Files/AssigmentInformation/", dto.AssigmentInformation.DocumentTypeExtension);
                    }

                    if (dto.AssigmentInformation.LicenseDriver != null)
                    {
                        dto.AssigmentInformation.LicenseDriver = _utiltyRepository.UploadImageBase64
                            (dto.AssigmentInformation.LicenseDriver, "Files/AssigmentInformation/", dto.AssigmentInformation.LicenseDriverExtension);
                    }
                    else if (dto.AssigmentInformation.LicenseDriver.Length <= 100)
                    {
                        _utiltyRepository.DeleteFile(request.AssigmentInformation.LicenseDriver);
                        dto.AssigmentInformation.LicenseDriver = _utiltyRepository.UploadImageBase64
                            (dto.AssigmentInformation.LicenseDriver, "Files/AssigmentInformation/", dto.AssigmentInformation.LicenseDriverExtension);
                    }

                    foreach (var i in dto.DependentImmigrationInfos)
                    {
                        foreach (var s in i.DocumentDependentImmigrationInfos)
                        {
                            if (s.FileRequest.Length > 150 && s.FileRequest != null)
                            {
                                s.FileRequest = _utiltyRepository.UploadImageBase64(s.FileRequest, "Files/DependentImmigrationInfos/", s.FileExtension);
                            }
                        }

                    }
                    ImmigrationProfile immigration = _immigrationProfileRepository.UpdateCustom(_mapper.Map<ImmigrationProfile>(dto));
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Immigration Profile Not Found";
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

        [HttpGet("GetImmigrationProfile", Name = "GetImmigrationProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationProfileDto>> GetImmigrationProfile([FromQuery] int sr)
        {
            try
            {
                var result = _immigrationProfileRepository.SelectCustom(sr);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAssigneFamily", Name = "GetAssigneFamily")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAssigneFamily([FromQuery] int sr)
        {
            try
            {
                var result = _immigrationProfileRepository.document_assigne(sr);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAssigneFamilyById", Name = "GetAssigneFamilyById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAssigneFamilyById([FromQuery] int id)
        {
            try
            {
                var result = _immigrationProfileRepository.document_assigneById(id);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        [HttpPost("CreateDocumentDependent", Name = "CreateDocumentDependent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentDependentInformationDto>> CreateDocumentDependent([FromBody] DocumentDependentInformationDto dto)
        {
            var response = new ApiResponse<DocumentDependentInformationDto>();
            try
            {
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/AssigmentInformation/", dto.FileExtension);

                DocumentDependentInformation doc = _documentDependentImmigration.Add(_mapper.Map<DocumentDependentInformation>(dto));
                response.Success = true;
                response.Result = _mapper.Map<DocumentDependentInformationDto>(doc);

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

        // Delete document
        [HttpGet("DeleteDocumentDependent", Name = "DeleteDocumentDependent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentTask([FromQuery] int id)
        {
            try
            {
                var document = _documentDependentImmigration.Find(c => c.Id == id);
                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    _documentDependentImmigration.Delete(_mapper.Map<DocumentDependentInformation>(document));
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
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetImmigrationLibrary", Name = "GetImmigrationLibrary")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetImmigrationLibrary([FromQuery] int id)
        {
            try
            {
                var result = _immigrationProfileRepository.document_assigneById(id);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        [HttpGet("GetHistoryImmigrationLibrary", Name = "GetHistoryImmigrationLibrary")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetHistoryImmigrationLibrary([FromQuery] int service_record_id, int service_line)
        {
            try
            {
                var result = _immigrationProfileRepository.immigration_library(service_record_id, service_line);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        [HttpGet("GetAllHistoryImmigrationLibrary", Name = "GetAllHistoryImmigrationLibrary")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllHistoryImmigrationLibrary([FromQuery] int service_record_id, int service_line)
        {
            try
            {
                var result = _immigrationProfileRepository.All_immigration_library(service_record_id, service_line);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        [HttpGet("GetDocumentLibraryById", Name = "GetDocumentLibraryById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDocumentLibraryById([FromQuery] int tipo, int id)
        {
            try
            {
                var result = _immigrationProfileRepository.immigration_libraryById(tipo, id);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

        //APP
        [HttpGet("GetDocumentDependent", Name = "GetDocumentDependent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ImmigrationProfileDto>> GetDocumentDependent([FromQuery] int assigneeId)
        {
            try
            {
                var result = _immigrationProfileRepository.SelectDocumentDependent(assigneeId);
                if (result != null)
                {
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Immigration Profile Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Internal Error", Message = ex.ToString() });
            }
        }

    }
}
