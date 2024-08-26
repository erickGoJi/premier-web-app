using System;
using System.Collections.Generic;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.CorporateAssistance;
using api.premier.Models.DocumentManagement;
using api.premier.Models.EntryVisa;
using api.premier.Models.LegalReview;
using api.premier.Models.LocalDocumentation;
using api.premier.Models.Notification;
using api.premier.Models.Renewal;
using api.premier.Models.ResidencyPermit;
using api.premier.Models.VisaDeregistration;
using api.premier.Models.WorkPermit;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.CorporateAssistance;
using biz.premier.Repository.DocumentManagement;
using biz.premier.Repository.Immigration;
using biz.premier.Repository.LegalReview;
using biz.premier.Repository.LocalDocumentation;
using biz.premier.Repository.Notification;
using biz.premier.Repository.Renewal;
using biz.premier.Repository.ResidencyPermit;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Repository.Utility;
using biz.premier.Repository.VisaDeregistration;
using biz.premier.Repository.WorkPermit;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImmigrationServicesController : ControllerBase
    {
        private readonly IWorkOrderRepository _serviceOrderRepository;
        private readonly IWorkOrderServicesRepository _serviceOrderServicesRepository;
        private readonly IImmigrationEntryVisaRepository _immigrationEntryVisaRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;

        private readonly IWorkPermitRepository _workPermitRepository;
        private readonly IDocumentWorkPermitRepository _documentWorkPermitRepository;
        private readonly IReminderWorkPermitRepository _reminderWorkPermitRepository;

        private readonly IResidencyPermitRepository _residencyPermitRepository;
        private readonly IDocumentResidencyPermitRepository _documentResidencyPermitRepository;
        private readonly IReminderResidencyPermitRepository _reminderResidencyPermitRepository;

        private readonly IVisaDeregistrationRepository _visaDeregistrationRepository;
        private readonly IDocumentVisaDeregistrationRepository _documentVisaDeregistrationRepository;
        private readonly IReminderVisaDeregistrationRepository _reminderVisaDeregistrationRepository;

        private readonly IRenewalRepository _renewalRepository;
        private readonly IDocumentRenawalRepository _documentRenawalRepository;
        private readonly IReminderRenawalRepository _reminderRenawalRepository;

        private readonly ICorporateAssistanceRepository _corporateAssistanceRepository;
        private readonly IDocumentCorporateAssistanceRepository _documentCorporateAssistanceRepository;
        private readonly IReminderCorporateAssistanceRepository _reminderCorporateAssistanceRepository;

        private readonly INotificationRepository _notificationRepository;
        private readonly IReminderNotificationRepository _reminderNotificationRepository;
        private readonly IDocumentNotificationRepository _documentNotificationRepository;

        private readonly ILegalReviewRepository _legalReviewRepository;
        private readonly IReminderLegalReviewRepository _reminderLegalReviewRepository;
        private readonly IDocumentLegalReviewRepository _documentLegalReviewRepository;

        private readonly IDocumentManagementRepository _documentManagementRepository;
        private readonly IDocumentDocumentManagementRepository _documentDocumentManagementRepository;
        private readonly IReminderDocumentManagementRepository _reminderDocumentManagementRepository;

        private readonly ILocalDocumentationRepository _localDocumentationRepository;
        private readonly IReminderLocalDocumentationRepository _reminderLocalDocumentationRepository;
        private readonly IDocumentLocalDocumentationRepository _documentLocalDocumentationRepository;
        public ImmigrationServicesController(
            IMapper mapper,
            ILoggerManager logger,
            
            IImmigrationEntryVisaRepository immigrationEntryVisaRepository,
            
            IUtiltyRepository utiltyRepository,
            
            IWorkPermitRepository workPermitRepository,
            IDocumentWorkPermitRepository documentWorkPermitRepository,
            IReminderWorkPermitRepository reminderWorkPermitRepository,
            
            IResidencyPermitRepository residencyPermitRepository,
            IDocumentResidencyPermitRepository documentResidencyPermitRepository,
            IReminderResidencyPermitRepository reminderResidencyPermitRepository,
            IVisaDeregistrationRepository visaDeregistrationRepository,
            IDocumentVisaDeregistrationRepository documentVisaDeregistrationRepository,
            IReminderVisaDeregistrationRepository reminderVisaDeregistrationRepository,
            IRenewalRepository renewalRepository, IDocumentRenawalRepository documentRenawalRepository, IReminderRenawalRepository reminderRenawalRepository,
            ICorporateAssistanceRepository corporateAssistanceRepository, IDocumentCorporateAssistanceRepository documentCorporateAssistanceRepository,
            IReminderCorporateAssistanceRepository reminderCorporateAssistanceRepository,
            INotificationRepository notificationRepository, IReminderNotificationRepository reminderNotificationRepository, IDocumentNotificationRepository documentNotificationRepository,
            ILegalReviewRepository legalReviewRepository, IReminderLegalReviewRepository reminderLegalReviewRepository, IDocumentLegalReviewRepository documentLegalReviewRepository,
            IDocumentManagementRepository documentManagementRepository, IDocumentDocumentManagementRepository documentDocumentManagementRepository, IReminderDocumentManagementRepository reminderDocumentManagementRepository,
            ILocalDocumentationRepository localDocumentationRepository, IReminderLocalDocumentationRepository reminderLocalDocumentationRepository, IDocumentLocalDocumentationRepository documentLocalDocumentationRepository,
            IWorkOrderServicesRepository serviceOrderServicesRepository,
            IWorkOrderRepository serviceOrderRepository)
        {
            _serviceOrderRepository = serviceOrderRepository;
            _mapper = mapper;
            _logger = logger;
            _immigrationEntryVisaRepository = immigrationEntryVisaRepository;
            _utiltyRepository = utiltyRepository;

            _workPermitRepository = workPermitRepository;
            _documentWorkPermitRepository = documentWorkPermitRepository;
            _reminderWorkPermitRepository = reminderWorkPermitRepository;

            _residencyPermitRepository = residencyPermitRepository;
            _documentResidencyPermitRepository = documentResidencyPermitRepository;
            _reminderResidencyPermitRepository = reminderResidencyPermitRepository;

            _visaDeregistrationRepository = visaDeregistrationRepository;
            _documentVisaDeregistrationRepository = documentVisaDeregistrationRepository;
            _reminderVisaDeregistrationRepository = reminderVisaDeregistrationRepository;
            
            _renewalRepository = renewalRepository;
            _documentRenawalRepository = documentRenawalRepository;
            _reminderRenawalRepository = reminderRenawalRepository;

            _corporateAssistanceRepository = corporateAssistanceRepository;
            _documentCorporateAssistanceRepository = documentCorporateAssistanceRepository;
            _reminderCorporateAssistanceRepository = reminderCorporateAssistanceRepository;

            _notificationRepository = notificationRepository;
            _reminderNotificationRepository = reminderNotificationRepository;
            _documentNotificationRepository = documentNotificationRepository;

            _legalReviewRepository = legalReviewRepository;
            _reminderLegalReviewRepository = reminderLegalReviewRepository;
            _documentLegalReviewRepository = documentLegalReviewRepository;

            _documentManagementRepository = documentManagementRepository;
            _documentDocumentManagementRepository = documentDocumentManagementRepository;
            _reminderDocumentManagementRepository = reminderDocumentManagementRepository;

            _localDocumentationRepository = localDocumentationRepository;
            _reminderLocalDocumentationRepository = reminderLocalDocumentationRepository;
            _documentLocalDocumentationRepository = documentLocalDocumentationRepository;
            _serviceOrderServicesRepository = serviceOrderServicesRepository;
        }

        #region Entry Visa
        //Post Create a new Immigration Entry Visa
        [HttpPost("CreateEntryVisa", Name = "CreateEntryVisa")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EntryVisaDto>> PostCreateEntryVisa([FromBody] EntryVisaDto dto)
        {
            var response = new ApiResponse<EntryVisaDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentEntryVisas)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/EntryVisa/", i.FileExtension);
                }
                EntryVisa immi = _immigrationEntryVisaRepository.Add(_mapper.Map<EntryVisa>(dto));
                response.Result = _mapper.Map<EntryVisaDto>(immi);
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

        //Post Create a new Immigration Entry Visa
        [HttpPut("UpdateEntryVisa", Name = "UpdateEntryVisa")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<EntryVisaDto>> PutUpdateEntryVisa([FromBody] EntryVisaDto dto)
        {
            var response = new ApiResponse<EntryVisaDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentEntryVisas)
                {
                    if (i.Id != 0)
                    {
                        DocumentWorkPermit document = _documentWorkPermitRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/EntryVisa/", i.FileExtension);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/EntryVisa/", i.FileExtension);
                    }
                }

                EntryVisa immi = _immigrationEntryVisaRepository.UpdateCustom(_mapper.Map<EntryVisa>(dto));
                response.Result = _mapper.Map<EntryVisaDto>(immi);
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

        [HttpGet("GetEntryVisa", Name = "GetEntryVisa")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetEntryVisa([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _immigrationEntryVisaRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetEntryVisaById", Name = "GetEntryVisaById")]
        public ActionResult<ApiResponse<EntryVisaSelectDto>> GetServiceRecordById([FromQuery] int id)
        {
            var response = new ApiResponse<EntryVisaSelectDto>();
            try
            {
                var map = _mapper.Map<EntryVisaSelectDto>(_immigrationEntryVisaRepository.GetEntryVisaCustom(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId.Value);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderEV", Name = "DeleteReminderEV")]
        public ActionResult DeleteReminderEV([FromQuery] int id)
        {
            try
            {
                var Success = _immigrationEntryVisaRepository.DeleteReminder(id);
                if (Success)
                {
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder was not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentEV", Name = "DeleteDocumentWorkPermitEV")]
        public ActionResult DeleteDocumentEV([FromQuery] int id)
        {
            try
            {
                var find = _immigrationEntryVisaRepository.DeleteDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                //    _documentWorkPermitRepository.Delete(_mapper.Map<DocumentWorkPermit>(find));
                    return Ok(new { Success = true, Result = "Document Entry Visa was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Document Entry Visa Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }
        #endregion
        #region Work Permit
        //Post Create a new Immigration Entry Visa
        [HttpPost("PostWorkPermit", Name = "PostWorkPermit")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<WorkPermitDto>> CreateWorkPermit([FromBody] WorkPermitDto dto)
        {
            var response = new ApiResponse<WorkPermitDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentWorkPermits)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/WorkPermit/", i.FileExtension);
                }
                WorkPermit immi = _workPermitRepository.Add(_mapper.Map<WorkPermit>(dto));
                response.Result = _mapper.Map<WorkPermitDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutWorkPermit", Name = "PutWorkPermit")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<WorkPermitDto>> PutWorkPermit([FromBody] WorkPermitDto dto)
        {
            var response = new ApiResponse<WorkPermitDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                WorkPermit immi = _workPermitRepository.UpdateCustom(_mapper.Map<WorkPermit>(dto));
                foreach(var i in dto.ReminderWorkPermits)
                {
                    if(i.Id != 0)
                    {
                        _reminderWorkPermitRepository.Update(_mapper.Map<ReminderWorkPermit>(i), i.Id);
                    }
                    else
                    {
                        _reminderWorkPermitRepository.Add(_mapper.Map<ReminderWorkPermit>(i));
                    }
                }

                foreach(var i in dto.DocumentWorkPermits)
                {
                    if (i.Id != 0)
                    {
                        DocumentWorkPermit document = _documentWorkPermitRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/WorkPermit/", i.FileExtension);
                        _documentWorkPermitRepository.Update(_mapper.Map<DocumentWorkPermit>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/WorkPermit/", i.FileExtension);
                        _documentWorkPermitRepository.Add(_mapper.Map<DocumentWorkPermit>(i));
                    }
                }

                response.Result = _mapper.Map<WorkPermitDto>(immi);
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

        [HttpGet("GetWorkPermit", Name = "GetWorkPermit")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetWorkPermit([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _workPermitRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetWorkPermitById", Name = "GetWorkPermitById")]
        public ActionResult<ApiResponse<WorkPermitSelectDto>> GetWorkPermitById([FromQuery] int id)
        {
            var response = new ApiResponse<WorkPermitSelectDto>();
            try
            {
                var map = _mapper.Map<WorkPermitSelectDto>(_workPermitRepository.GetCustomWorkPermit(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId.Value);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderWP", Name = "DeleteReminderWP")]
        public ActionResult DeleteReminder([FromQuery] int id)
        {
            try
            {
                var find = _reminderWorkPermitRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderWorkPermitRepository.Delete(_mapper.Map<ReminderWorkPermit>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentWP", Name = "DeleteDocumentWorkPermitWP")]
        public ActionResult DeleteDocument([FromQuery] int id)
        {
            try
            {
                var find = _documentWorkPermitRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentWorkPermitRepository.Delete(_mapper.Map<DocumentWorkPermit>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentWP", Name = "PutDocumentWP")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentWorkPermitDto>> CreateDocumentWorkPermit([FromBody] DocumentWorkPermitDto dto)
        {
            var response = new ApiResponse<DocumentWorkPermitDto>();
            try
            {
                DocumentWorkPermit document = _documentWorkPermitRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/WorkPermit/", dto.FileExtension);

                    DocumentWorkPermit dwp = _documentWorkPermitRepository.Update(_mapper.Map<DocumentWorkPermit>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentWorkPermitDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentWP", Name = "PostDocumentWP")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentWorkPermitDto>> PostDocumentWorkPermit([FromBody] DocumentWorkPermitDto dto)
        {
            var response = new ApiResponse<DocumentWorkPermitDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/WorkPermit/", dto.FileExtension);

                DocumentWorkPermit dwp = _documentWorkPermitRepository.Add(_mapper.Map<DocumentWorkPermit>(dto));
                response.Result = _mapper.Map<DocumentWorkPermitDto>(dwp);
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
        #endregion
        #region Residency Permit
        //Post Create a new Immigration Entry Visa
        [HttpPost("PostResidencyPermit", Name = "PostResidencyPermit")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ResidencyPermitDto>> CreateResidencyPermit([FromBody] ResidencyPermitDto dto)
        {
            var response = new ApiResponse<ResidencyPermitDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentResidencyPermits)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ResidencyPermit/", i.FileExtension);
                }
                ResidencyPermit immi = _residencyPermitRepository.Add(_mapper.Map<ResidencyPermit>(dto));
                response.Result = _mapper.Map<ResidencyPermitDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutResidencyPermit", Name = "PutResidencyPermit")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ResidencyPermitDto>> PutResidencyPermit([FromBody] ResidencyPermitDto dto)
        {
            var response = new ApiResponse<ResidencyPermitDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                ResidencyPermit immi = _residencyPermitRepository.UpdateCustom(_mapper.Map<ResidencyPermit>(dto), dto.Id);
                foreach (var i in dto.ReminderResidencyPermits)
                {
                    if (i.Id != 0)
                    {
                        _reminderResidencyPermitRepository.Update(_mapper.Map<ReminderResidencyPermit>(i), i.Id);
                    }
                    else
                    {
                        _reminderResidencyPermitRepository.Add(_mapper.Map<ReminderResidencyPermit>(i));
                    }
                }

                foreach (var i in dto.DocumentResidencyPermits)
                {
                    if (i.Id != 0)
                    {
                        DocumentResidencyPermit document = _documentResidencyPermitRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ResidencyPermit/", i.FileExtension);
                        _documentResidencyPermitRepository.Update(_mapper.Map<DocumentResidencyPermit>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/ResidencyPermit/", i.FileExtension);
                        _documentResidencyPermitRepository.Add(_mapper.Map<DocumentResidencyPermit>(i));
                    }
                }
                response.Result = _mapper.Map<ResidencyPermitDto>(immi);
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

        [HttpGet("GetResidencyPermit", Name = "GetResidencyPermit")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetResidencyPermit([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _residencyPermitRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetResidencyPermitById", Name = "GetResidencyPermitById")]
        public ActionResult<ApiResponse<ResidencyPermitSelectDto>> GetResidencyPermitById([FromQuery] int id)
        {
            var response = new ApiResponse<ResidencyPermitSelectDto>();
            try
            {
                var map = _mapper.Map<ResidencyPermitSelectDto>(_residencyPermitRepository.GetResidencyPermitCustom(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderRP", Name = "DeleteReminderRP")]
        public ActionResult DeleteReminderRP([FromQuery] int id)
        {
            try
            {
                var find = _reminderResidencyPermitRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderResidencyPermitRepository.Delete(_mapper.Map<ReminderResidencyPermit>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentRP", Name = "DeleteDocumentWorkPermitRP")]
        public ActionResult DeleteDocumentRp([FromQuery] int id)
        {
            try
            {
                var find = _documentResidencyPermitRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentResidencyPermitRepository.Delete(_mapper.Map<DocumentResidencyPermit>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentRP", Name = "PutDocumentRP")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentResidencyPermitDto>> PutDocumentRP([FromBody] DocumentResidencyPermitDto dto)
        {
            var response = new ApiResponse<DocumentResidencyPermitDto>();
            try
            {
                DocumentResidencyPermit document = _documentResidencyPermitRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/WorkPermit/", dto.FileExtension);

                    DocumentResidencyPermit dwp = _documentResidencyPermitRepository.Update(_mapper.Map<DocumentResidencyPermit>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentResidencyPermitDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentRP", Name = "PostDocumentRP")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentResidencyPermitDto>> PostDocumentRP([FromBody] DocumentResidencyPermitDto dto)
        {
            var response = new ApiResponse<DocumentResidencyPermitDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/WorkPermit/", dto.FileExtension);

                DocumentResidencyPermit dwp = _documentResidencyPermitRepository.Add(_mapper.Map<DocumentResidencyPermit>(dto));
                response.Result = _mapper.Map<DocumentResidencyPermitDto>(dwp);
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
        #endregion
        #region Visa Deregistration
        //Post Create a new Visa Deregistration
        [HttpPost("PostVisaDeregistration", Name = "PostVisaDeregistration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<VisaDeregistrationDto>> PostVisaDeregistration([FromBody] VisaDeregistrationDto dto)
        {
            var response = new ApiResponse<VisaDeregistrationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentVisaDeregistrations)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/VisaDeregistration/", i.FileExtension);
                }
                VisaDeregistration immi = _visaDeregistrationRepository.Add(_mapper.Map<VisaDeregistration>(dto));
                response.Result = _mapper.Map<VisaDeregistrationDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutVisaDeregistration", Name = "PutVisaDeregistration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<VisaDeregistrationDto>> PutVisaDeregistration([FromBody] VisaDeregistrationDto dto)
        {
            var response = new ApiResponse<VisaDeregistrationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                VisaDeregistration immi = _visaDeregistrationRepository.UpdateCustom(_mapper.Map<VisaDeregistration>(dto), dto.Id);
                foreach (var i in dto.ReminderVisaDeregistrations)
                {
                    if (i.Id != 0)
                    {
                        _reminderVisaDeregistrationRepository.Update(_mapper.Map<ReminderVisaDeregistration>(i), i.Id);
                    }
                    else
                    {
                        _reminderVisaDeregistrationRepository.Add(_mapper.Map<ReminderVisaDeregistration>(i));
                    }
                }

                foreach (var i in dto.DocumentVisaDeregistrations)
                {
                    if (i.Id != 0)
                    {
                        DocumentVisaDeregistration document = _documentVisaDeregistrationRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/VisaDeregistration/", i.FileExtension);
                        _documentVisaDeregistrationRepository.Update(_mapper.Map<DocumentVisaDeregistration>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/VisaDeregistration/", i.FileExtension);
                        _documentVisaDeregistrationRepository.Add(_mapper.Map<DocumentVisaDeregistration>(i));
                    }
                }
                response.Result = _mapper.Map<VisaDeregistrationDto>(immi);
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

        [HttpGet("GetVisaDeregistration", Name = "GetVisaDeregistration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetVisaDeregistration([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _visaDeregistrationRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetVisaDeregistrationById", Name = "GetVisaDeregistrationById")]
        public ActionResult<ApiResponse<VisaDeregistrationSelcetDto>> GetVisaDeregistrationById([FromQuery] int id)
        {
            var response = new ApiResponse<VisaDeregistrationSelcetDto>();
            try
            {
                var map = _mapper.Map<VisaDeregistrationSelcetDto>(_visaDeregistrationRepository.GetCustomVisaDeregistration(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderVD", Name = "DeleteReminderVD")]
        public ActionResult DeleteReminderVD([FromQuery] int id)
        {
            try
            {
                var find = _reminderVisaDeregistrationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderVisaDeregistrationRepository.Delete(_mapper.Map<ReminderVisaDeregistration>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentVD", Name = "DeleteDocumentVD")]
        public ActionResult DeleteDocumentVD([FromQuery] int id)
        {
            try
            {
                var find = _documentVisaDeregistrationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentVisaDeregistrationRepository.Delete(_mapper.Map<DocumentVisaDeregistration>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentVD", Name = "PutDocumentVD")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentVisaDeregistrationDto>> PutDocumentVD([FromBody] DocumentVisaDeregistrationDto dto)
        {
            var response = new ApiResponse<DocumentVisaDeregistrationDto>();
            try
            {
                DocumentVisaDeregistration document = _documentVisaDeregistrationRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/VisaDeregistration/", dto.FileExtension);

                    DocumentVisaDeregistration dwp = _documentVisaDeregistrationRepository.Update(_mapper.Map<DocumentVisaDeregistration>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentVisaDeregistrationDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentVD", Name = "PostDocumentVD")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentVisaDeregistrationDto>> PostDocumentVD([FromBody] DocumentVisaDeregistrationDto dto)
        {
            var response = new ApiResponse<DocumentVisaDeregistrationDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/VisaDeregistration/", dto.FileExtension);

                DocumentVisaDeregistration dwp = _documentVisaDeregistrationRepository.Add(_mapper.Map<DocumentVisaDeregistration>(dto));
                response.Result = _mapper.Map<DocumentVisaDeregistrationDto>(dwp);
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
        #endregion
        #region Renewal
        //Post Create a new Visa Deregistration
        [HttpPost("PostRenewal", Name = "PostRenewal")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RenewalDto>> PostRenewal([FromBody] RenewalDto dto)
        {
            var response = new ApiResponse<RenewalDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentRenewals)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Renewal/", i.FileExtension);
                }
                Renewal immi = _renewalRepository.Add(_mapper.Map<Renewal>(dto));
                response.Result = _mapper.Map<RenewalDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutRenewal", Name = "PutRenewal")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RenewalDto>> PutRenewal([FromBody] RenewalDto dto)
        {
            var response = new ApiResponse<RenewalDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                Renewal immi = _renewalRepository.UpdateCustom(_mapper.Map<Renewal>(dto), dto.Id);
                foreach (var i in dto.ReminderRenewals)
                {
                    if (i.Id != 0)
                    {
                        _reminderRenawalRepository.Update(_mapper.Map<ReminderRenewal>(i), i.Id);
                    }
                    else
                    {
                        _reminderRenawalRepository.Add(_mapper.Map<ReminderRenewal>(i));
                    }
                }

                foreach (var i in dto.DocumentRenewals)
                {
                    if (i.Id != 0)
                    {
                        DocumentRenewal document = _documentRenawalRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Renewal/", i.FileExtension);
                        _documentRenawalRepository.Update(_mapper.Map<DocumentRenewal>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Renewal/", i.FileExtension);
                        _documentRenawalRepository.Add(_mapper.Map<DocumentRenewal>(i));
                    }
                }
                response.Result = _mapper.Map<RenewalDto>(immi);
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

        [HttpGet("GetRenewal", Name = "GetRenewal")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetRenewal([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _renewalRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetRenewalById", Name = "GetRenewalById")]
        public ActionResult<ApiResponse<RenewalSelectDto>> GetRenewalById([FromQuery] int id)
        {
            var response = new ApiResponse<RenewalSelectDto>();
            try
            {
                var map = _mapper.Map<RenewalSelectDto>(_renewalRepository.GetRenewalById(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderR", Name = "DeleteReminderR")]
        public ActionResult DeleteReminderR([FromQuery] int id)
        {
            try
            {
                var find = _reminderRenawalRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderRenawalRepository.Delete(_mapper.Map<ReminderRenewal>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentR", Name = "DeleteDocumentR")]
        public ActionResult DeleteDocumentR([FromQuery] int id)
        {
            try
            {
                var find = _documentRenawalRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentRenawalRepository.Delete(_mapper.Map<DocumentRenewal>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentR", Name = "PutDocumentR")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentRenewalDto>> PutDocumentR([FromBody] DocumentRenewalDto dto)
        {
            var response = new ApiResponse<DocumentRenewalDto>();
            try
            {
                DocumentRenewal document = _documentRenawalRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/Renewal/", dto.FileExtension);

                    DocumentRenewal dwp = _documentRenawalRepository.Update(_mapper.Map<DocumentRenewal>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentRenewalDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentR", Name = "PostDocumentR")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentRenewalDto>> PostDocumentR([FromBody] DocumentRenewalDto dto)
        {
            var response = new ApiResponse<DocumentRenewalDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/Renewal/", dto.FileExtension);

                DocumentRenewal dwp = _documentRenawalRepository.Add(_mapper.Map<DocumentRenewal>(dto));
                response.Result = _mapper.Map<DocumentRenewalDto>(dwp);
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
        #endregion
        #region Corporate Assistance
        //Post Create a new Visa Deregistration
        [HttpPost("PostCorporateAssistance", Name = "PostCorporateAssistance")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CorporateAssistanceDto>> PostCorporateAssistance([FromBody] CorporateAssistanceDto dto)
        {
            var response = new ApiResponse<CorporateAssistanceDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentCorporateAssistances)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/CorporateAssistance/", i.FileExtension);
                }
                CorporateAssistance immi = _corporateAssistanceRepository.Add(_mapper.Map<CorporateAssistance>(dto));
                response.Result = _mapper.Map<CorporateAssistanceDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutCorporateAssistance", Name = "PutCorporateAssistance")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CorporateAssistanceDto>> PutCorporateAssistance([FromBody] CorporateAssistanceDto dto)
        {
            var response = new ApiResponse<CorporateAssistanceDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                CorporateAssistance immi = _corporateAssistanceRepository.UpdateCustom(_mapper.Map<CorporateAssistance>(dto), dto.Id);
                foreach (var i in dto.RemiderCorporateAssistances)
                {
                    if (i.Id != 0)
                    {
                        _reminderCorporateAssistanceRepository.Update(_mapper.Map<RemiderCorporateAssistance>(i), i.Id);
                    }
                    else
                    {
                        _reminderCorporateAssistanceRepository.Add(_mapper.Map<RemiderCorporateAssistance>(i));
                    }
                }

                foreach (var i in dto.DocumentCorporateAssistances)
                {
                    if (i.Id != 0)
                    {
                        DocumentCorporateAssistance document = _documentCorporateAssistanceRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/CorporateAssistance/", i.FileExtension);
                        _documentCorporateAssistanceRepository.Update(_mapper.Map<DocumentCorporateAssistance>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/CorporateAssistance/", i.FileExtension);
                        _documentCorporateAssistanceRepository.Add(_mapper.Map<DocumentCorporateAssistance>(i));
                    }
                }
                response.Result = _mapper.Map<CorporateAssistanceDto>(immi);
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

        [HttpGet("GetCorporateAssistance", Name = "GetCorporateAssistance")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCorporateAssistance([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _corporateAssistanceRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetCorporateAssistanceById", Name = "GetCorporateAssistanceById")]
        public ActionResult<ApiResponse<CorporateAssistanceSelectDto>> GetCorporateAssistanceById([FromQuery] int id)
        {
            var response = new ApiResponse<CorporateAssistanceSelectDto>();
            try
            {
                var map = _mapper.Map<CorporateAssistanceSelectDto>(_corporateAssistanceRepository.GetCorporateAssistanceById(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderCA", Name = "DeleteReminderCA")]
        public ActionResult DeleteReminderCA([FromQuery] int id)
        {
            try
            {
                var find = _reminderCorporateAssistanceRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderCorporateAssistanceRepository.Delete(_mapper.Map<RemiderCorporateAssistance>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentCA", Name = "DeleteDocumentCA")]
        public ActionResult DeleteDocumentCA([FromQuery] int id)
        {
            try
            {
                var find = _documentCorporateAssistanceRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentCorporateAssistanceRepository.Delete(_mapper.Map<DocumentCorporateAssistance>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentCA", Name = "PutDocumentCA")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentCorporateAssistanceDto>> PutDocumentCA([FromBody] DocumentCorporateAssistanceDto dto)
        {
            var response = new ApiResponse<DocumentCorporateAssistanceDto>();
            try
            {
                DocumentCorporateAssistance document = _documentCorporateAssistanceRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/CorporateAssistance/", dto.FileExtension);

                    DocumentCorporateAssistance dwp = _documentCorporateAssistanceRepository.Update(_mapper.Map<DocumentCorporateAssistance>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentCorporateAssistanceDto>(dwp);
                }   
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentCA", Name = "PostDocumentCA")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentCorporateAssistanceDto>> PostDocumentR([FromBody] DocumentCorporateAssistanceDto dto)
        {
            var response = new ApiResponse<DocumentCorporateAssistanceDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/CorporateAssistance/", dto.FileExtension);

                DocumentCorporateAssistance dwp = _documentCorporateAssistanceRepository.Add(_mapper.Map<DocumentCorporateAssistance>(dto));
                response.Result = _mapper.Map<DocumentCorporateAssistanceDto>(dwp);
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
        #endregion
        #region Notification
        //Post Create a new Visa Deregistration
        [HttpPost("PostNotification", Name = "PostNotification")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<NotificationDto>> PostNotification([FromBody] NotificationDto dto)
        {
            var response = new ApiResponse<NotificationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentNotifications)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Notification/", i.FileExtension);
                }
                Notification immi = _notificationRepository.Add(_mapper.Map<Notification>(dto));
                response.Result = _mapper.Map<NotificationDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutNotification", Name = "PutNotification")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<NotificationDto>> PutNotification([FromBody] NotificationDto dto)
        {
            var response = new ApiResponse<NotificationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                Notification immi = _notificationRepository.UpdateCustom(_mapper.Map<Notification>(dto), dto.Id);
                foreach (var i in dto.ReminderNotifications)
                {
                    if (i.Id != 0)
                    {
                        _reminderNotificationRepository.Update(_mapper.Map<ReminderNotification>(i), i.Id);
                    }
                    else
                    {
                        _reminderNotificationRepository.Add(_mapper.Map<ReminderNotification>(i));
                    }
                }

                foreach (var i in dto.DocumentNotifications)
                {
                    if (i.Id != 0)
                    {
                        DocumentNotification document = _documentNotificationRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Notification/", i.FileExtension);
                        _documentNotificationRepository.Update(_mapper.Map<DocumentNotification>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Notification/", i.FileExtension);
                        _documentNotificationRepository.Add(_mapper.Map<DocumentNotification>(i));
                    }
                }
                response.Result = _mapper.Map<NotificationDto>(immi);
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

        [HttpGet("GetNotification", Name = "GetNotification")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetNotification([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _notificationRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetNotificationById", Name = "GetNotificationById")]
        public ActionResult<ApiResponse<NotificationSelectDto>> GetNotificationById([FromQuery] int id)
        {
            var response = new ApiResponse<NotificationSelectDto>();
            try
            {
                var map = _mapper.Map<NotificationSelectDto>(_notificationRepository.GetNotificaitionById(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderN", Name = "DeleteReminderN")]
        public ActionResult DeleteReminderN([FromQuery] int id)
        {
            try
            {
                var find = _reminderNotificationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderNotificationRepository.Delete(_mapper.Map<ReminderNotification>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentN", Name = "DeleteDocumentN")]
        public ActionResult DeleteDocumentN([FromQuery] int id)
        {
            try
            {
                var find = _documentNotificationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentNotificationRepository.Delete(_mapper.Map<DocumentNotification>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentN", Name = "PutDocumentN")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentNotificationDto>> PutDocumentN([FromBody] DocumentNotificationDto dto)
        {
            var response = new ApiResponse<DocumentNotificationDto>();
            try
            {
                DocumentNotification document = _documentNotificationRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/Notification/", dto.FileExtension);

                    DocumentNotification dwp = _documentNotificationRepository.Update(_mapper.Map<DocumentNotification>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentNotificationDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentN", Name = "PostDocumentN")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentNotificationDto>> PostDocumentN([FromBody] DocumentNotificationDto dto)
        {
            var response = new ApiResponse<DocumentNotificationDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/Notification/", dto.FileExtension);

                DocumentNotification dwp = _documentNotificationRepository.Add(_mapper.Map<DocumentNotification>(dto));
                response.Result = _mapper.Map<DocumentNotificationDto>(dwp);
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
        #endregion
        #region Legal Review
        //Post Create a new Visa Deregistration
        [HttpPost("PostLegalReview", Name = "PostLegalReview")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<LegalReviewDto>> PostLegalReview([FromBody] LegalReviewDto dto)
        {
            var response = new ApiResponse<LegalReviewDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentLegalReviews)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LegalReview/", i.FileExtension);
                }
                LegalReview immi = _legalReviewRepository.Add(_mapper.Map<LegalReview>(dto));
                response.Result = _mapper.Map<LegalReviewDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutLegalReview", Name = "PutLegalReview")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<LegalReviewDto>> PutLegalReview([FromBody] LegalReviewDto dto)
        {
            var response = new ApiResponse<LegalReviewDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                LegalReview immi = _legalReviewRepository.UpdateCustom(_mapper.Map<LegalReview>(dto), dto.Id);
                foreach (var i in dto.ReminderLegalReviews)
                {
                    if (i.Id != 0)
                    {
                        _reminderLegalReviewRepository.Update(_mapper.Map<ReminderLegalReview>(i), i.Id);
                    }
                    else
                    {
                        _reminderLegalReviewRepository.Add(_mapper.Map<ReminderLegalReview>(i));
                    }
                }

                foreach (var i in dto.DocumentLegalReviews)
                {
                    if (i.Id != 0)
                    {
                        DocumentLegalReview document = _documentLegalReviewRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LegalReview/", i.FileExtension);
                        _documentLegalReviewRepository.Update(_mapper.Map<DocumentLegalReview>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LegalReview/", i.FileExtension);
                        _documentLegalReviewRepository.Add(_mapper.Map<DocumentLegalReview>(i));
                    }
                }
                response.Result = _mapper.Map<LegalReviewDto>(immi);
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

        [HttpGet("GetLegalReview", Name = "GetLegalReview")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetLegalReview([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _legalReviewRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetLegalReviewById", Name = "GetLegalReviewById")]
        public ActionResult<ApiResponse<LegalReviewSelectDto>> GetLegalReviewById([FromQuery] int id)
        {
            var response = new ApiResponse<LegalReviewSelectDto>();
            try
            {
                var map = _mapper.Map<LegalReviewSelectDto>(_legalReviewRepository.GetLegalReviewById(id));
                var data = _serviceOrderRepository.GetName(map.ApplicantId);
                map.ApplicantName = data.Item1;
                map.Relationship = data.Item2;
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderLR", Name = "DeleteReminderLR")]
        public ActionResult DeleteReminderLR([FromQuery] int id)
        {
            try
            {
                var find = _reminderLegalReviewRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderLegalReviewRepository.Delete(_mapper.Map<ReminderLegalReview>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentLR", Name = "DeleteDocumentLR")]
        public ActionResult DeleteDocumentLR([FromQuery] int id)
        {
            try
            {
                var find = _documentLegalReviewRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentLegalReviewRepository.Delete(_mapper.Map<DocumentLegalReview>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentLR", Name = "PutDocumentLR")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentLegalReviewDto>> PutDocumentLR([FromBody] DocumentLegalReviewDto dto)
        {
            var response = new ApiResponse<DocumentLegalReviewDto>();
            try
            {
                DocumentNotification document = _documentNotificationRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/LegalReview/", dto.FileExtension);

                    DocumentLegalReview dwp = _documentLegalReviewRepository.Update(_mapper.Map<DocumentLegalReview>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentLegalReviewDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentLR", Name = "PostDocumentLR")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentLegalReviewDto>> PostDocumentLR([FromBody] DocumentLegalReviewDto dto)
        {
            var response = new ApiResponse<DocumentLegalReviewDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/LegalReview/", dto.FileExtension);

                DocumentLegalReview dwp = _documentLegalReviewRepository.Add(_mapper.Map<DocumentLegalReview>(dto));
                response.Result = _mapper.Map<DocumentLegalReviewDto>(dwp);
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
        #endregion
        #region Document Management
        //Post Create a new Visa Deregistration
        [HttpPost("PostDocumentManagement", Name = "PostDocumentManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentManagementDto>> PostDocumentManagement([FromBody] DocumentManagementDto dto)
        {
            var response = new ApiResponse<DocumentManagementDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentDocumentManagements)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/DocumentManagement/", i.FileExtension);
                }
                DocumentManagement immi = _documentManagementRepository.Add(_mapper.Map<DocumentManagement>(dto));
                response.Result = _mapper.Map<DocumentManagementDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutDocumentManagement", Name = "PutDocumentManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<DocumentManagementDto>>> PutDocumentManagement([FromBody] List<DocumentManagementDto> dto)
        {
            var response = new ApiResponse<List<DocumentManagementDto>>();
            try
            {
                foreach(var documentManagement in dto)
                {
                    documentManagement.CreatedDate = DateTime.Now;
                    DocumentManagement immi = _documentManagementRepository.UpdateCustom(_mapper.Map<DocumentManagement>(documentManagement), documentManagement.Id);
                    foreach (var i in documentManagement.ReminderDocumentManagements)
                    {
                        if (i.Id != 0)
                        {
                            _reminderDocumentManagementRepository.Update(_mapper.Map<ReminderDocumentManagement>(i), i.Id);
                        }
                        else
                        {
                            _reminderDocumentManagementRepository.Add(_mapper.Map<ReminderDocumentManagement>(i));
                        }
                    }

                    foreach (var i in documentManagement.DocumentDocumentManagements)
                    {
                        if (i.Id != 0)
                        {
                            if (i.FileRequest.Length > 100)
                            {
                                DocumentDocumentManagement document = _documentDocumentManagementRepository.Find(x => x.Id == i.Id);
                                _utiltyRepository.DeleteFile(document.FileRequest);
                                i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/DocumentManagement/", i.FileExtension);
                                _documentDocumentManagementRepository.Update(_mapper.Map<DocumentDocumentManagement>(i), i.Id);
                            }
                        }
                        else
                        {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/DocumentManagement/", i.FileExtension);
                            _documentDocumentManagementRepository.Add(_mapper.Map<DocumentDocumentManagement>(i));
                        }
                    }
                }
                response.Result = _mapper.Map<List<DocumentManagementDto>>(dto);
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

        [HttpGet("GetDocumentManagement", Name = "GetDocumentManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDocumentManagement([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _documentManagementRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetDocumentManagementById", Name = "GetDocumentManagementById")]
        public ActionResult<ApiResponse<DocumentManagementSelectDto>> GetDocumentManagementById([FromQuery] int id)
        {
            var response = new ApiResponse<DocumentManagementSelectDto>();
            try
            {
                var map = _mapper.Map<DocumentManagementSelectDto>(_documentManagementRepository.Find(x => x.Id == id));
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderDM", Name = "DeleteReminderDM")]
        public ActionResult DeleteReminderDM([FromQuery] int id)
        {
            try
            {
                var find = _reminderDocumentManagementRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderDocumentManagementRepository.Delete(_mapper.Map<ReminderDocumentManagement>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentDM", Name = "DeleteDocumentDM")]
        public ActionResult DeleteDocumentDM([FromQuery] int id)
        {
            try
            {
                var find = _documentDocumentManagementRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentDocumentManagementRepository.Delete(_mapper.Map<DocumentDocumentManagement>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentDM", Name = "PutDocumentDM")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentDocumentManagementDto>> PutDocumentDM([FromBody] DocumentDocumentManagementDto dto)
        {
            var response = new ApiResponse<DocumentDocumentManagementDto>();
            try
            {
                DocumentDocumentManagement document = _documentDocumentManagementRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/DocumentManagement/", dto.FileExtension);

                    DocumentDocumentManagement dwp = _documentDocumentManagementRepository.Update(_mapper.Map<DocumentDocumentManagement>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentDocumentManagementDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentDM", Name = "PostDocumentDM")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentDocumentManagementDto>> PostDocumentDM([FromBody] DocumentDocumentManagementDto dto)
        {
            var response = new ApiResponse<DocumentDocumentManagementDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/LegalReview/", dto.FileExtension);

                DocumentDocumentManagement dwp = _documentDocumentManagementRepository.Add(_mapper.Map<DocumentDocumentManagement>(dto));
                response.Result = _mapper.Map<DocumentDocumentManagementDto>(dwp);
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
        #endregion
        #region Local Documentation
        //Post Create a new Visa Deregistration
        [HttpPost("PostLocalDocumentation", Name = "PostLocalDocumentation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<LocalDocumentationDto>> PostLocalDocumentation([FromBody] LocalDocumentationDto dto)
        {
            var response = new ApiResponse<LocalDocumentationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentLocalDocumentations)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LocalDocumentation/", i.FileExtension);
                }
                LocalDocumentation immi = _localDocumentationRepository.Add(_mapper.Map<LocalDocumentation>(dto));
                response.Result = _mapper.Map<LocalDocumentationDto>(immi);
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

        //Post Create a new Relocation Work Permit
        [HttpPut("PutLocalDocumentation", Name = "PutLocalDocumentation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<LocalDocumentationDto>>> PutLocalDocumentation([FromBody] List<LocalDocumentationDto> dto)
        {
            var response = new ApiResponse<List<LocalDocumentationDto>>();
            try
            {
                foreach(var localDocument in dto)
                {
                    localDocument.CreatedDate = DateTime.Now;
                    LocalDocumentation immi = _localDocumentationRepository.UpdateCustom(_mapper.Map<LocalDocumentation>(localDocument), localDocument.Id);
                    foreach (var i in localDocument.ReminderLocalDocumentations)
                    {
                        if (i.Id != 0)
                        {
                            _reminderLocalDocumentationRepository.Update(_mapper.Map<ReminderLocalDocumentation>(i), i.Id);
                        }
                        else
                        {
                            _reminderLocalDocumentationRepository.Add(_mapper.Map<ReminderLocalDocumentation>(i));
                        }
                    }

                    foreach (var i in localDocument.DocumentLocalDocumentations)
                    {
                        if (i.Id != 0)
                        {
                            if (i.FileRequest.Length > 100)
                            {
                                DocumentLocalDocumentation document = _documentLocalDocumentationRepository.Find(x => x.Id == i.Id);
                                _utiltyRepository.DeleteFile(document.FileRequest);
                                i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LocalDocumentation/", i.FileExtension);
                                _documentLocalDocumentationRepository.Update(_mapper.Map<DocumentLocalDocumentation>(i), i.Id);
                            }
                        }
                        else
                        {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LocalDocumentation/", i.FileExtension);
                            _documentLocalDocumentationRepository.Add(_mapper.Map<DocumentLocalDocumentation>(i));
                        }
                    }
                }
                response.Result = _mapper.Map<List<LocalDocumentationDto>>(dto);
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

        [HttpGet("GetLocalOrDocumentation", Name = "GetLocalOrDocumentation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetLocalDocumentation([FromQuery] int applicantId, [FromQuery] int category, int service_order_id, int type_service)
        {
            try
            {
                var Result = _serviceOrderRepository.GetDocumentation(category, applicantId, service_order_id, type_service);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetLocalDocumentationById", Name = "GetLocalDocumentationById")]
        public ActionResult<ApiResponse<LocalDocumentationSelectDto>> GetLocalDocumentationById([FromQuery] int id)
        {
            var response = new ApiResponse<LocalDocumentationSelectDto>();
            try
            {
                var map = _mapper.Map<LocalDocumentationSelectDto>(_localDocumentationRepository.Find(x => x.Id == id));
                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteReminderLD", Name = "DeleteReminderLD")]
        public ActionResult DeleteReminderLD([FromQuery] int id)
        {
            try
            {
                var find = _reminderLocalDocumentationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderLocalDocumentationRepository.Delete(_mapper.Map<ReminderLocalDocumentation>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteDocumentLD", Name = "DeleteDocumentLD")]
        public ActionResult DeleteDocumentLD([FromQuery] int id)
        {
            try
            {
                var find = _documentLocalDocumentationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentLocalDocumentationRepository.Delete(_mapper.Map<DocumentLocalDocumentation>(find));
                    return Ok(new { Success = true, Result = "Reminder was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Reminder Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        [HttpPut("PutDocumentLD", Name = "PutDocumentLD")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentLocalDocumentationDto>> PutDocumentLD([FromBody] DocumentLocalDocumentationDto dto)
        {
            var response = new ApiResponse<DocumentLocalDocumentationDto>();
            try
            {
                DocumentLocalDocumentation document = _documentLocalDocumentationRepository.Find(x => x.Id == dto.Id);

                if (document != null)
                {
                    _utiltyRepository.DeleteFile(document.FileRequest);
                    dto.CreatedDate = DateTime.Now;
                    dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/LocalDocumentation/", dto.FileExtension);

                    DocumentLocalDocumentation dwp = _documentLocalDocumentationRepository.Update(_mapper.Map<DocumentLocalDocumentation>(dto), dto.Id);
                    response.Result = _mapper.Map<DocumentLocalDocumentationDto>(dwp);
                }
                else
                {
                    response.Result = null;
                    response.Message = "Documento Not Found";
                    return StatusCode(404, response);
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

        [HttpPost("PostDocumentLD", Name = "PostDocumentLD")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DocumentLocalDocumentationDto>> PostDocumentLD([FromBody] DocumentLocalDocumentationDto dto)
        {
            var response = new ApiResponse<DocumentLocalDocumentationDto>();
            try
            {
                //_utiltyRepository.DeleteFile(document.FileRequest);
                dto.CreatedDate = DateTime.Now;
                dto.FileRequest = _utiltyRepository.UploadImageBase64(dto.FileRequest, "Files/LocalDocumentation/", dto.FileExtension);

                DocumentLocalDocumentation dwp = _documentLocalDocumentationRepository.Add(_mapper.Map<DocumentLocalDocumentation>(dto));
                response.Result = _mapper.Map<DocumentLocalDocumentationDto>(dwp);
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
        #endregion
        [HttpDelete("DeleteAllServiceById", Name = "DeleteAllServiceById")]
        public ActionResult DeleteAllServiceById(int id, int type)
        {
            try
            {
                var _entry_visa = _immigrationEntryVisaRepository.Find(x => x.Id == id);
                var _work_permit = _workPermitRepository.Find(x => x.Id == id);
                var _residency_permit = _residencyPermitRepository.Find(x => x.Id == id);
                var _visa_registration = _visaDeregistrationRepository.Find(x => x.Id == id);
                var _renewal = _renewalRepository.Find(x => x.Id == id);

                switch (type)
                {
                    case 1:
                        _immigrationEntryVisaRepository.Delete(_mapper.Map<EntryVisa>(_entry_visa));
                        return Ok(new { Success = true, Result = "Entry Visa was delete" });
                        //break;
                    case 2:
                        _workPermitRepository.Delete(_mapper.Map<WorkPermit>(_work_permit));
                        return Ok(new { Success = true, Result = "Work Permit Visa was delete" });
                        //break;
                    case 3:
                        _visaDeregistrationRepository.Delete(_mapper.Map<VisaDeregistration>(_visa_registration));
                        return Ok(new { Success = true, Result = "Visa was delete" });
                        //break;
                    case 4:
                        _residencyPermitRepository.Delete(_mapper.Map<ResidencyPermit>(_residency_permit));
                        return Ok(new { Success = true, Result = "Resicency Visa was delete" });
                        //break;
                    case 5:
                        _renewalRepository.Delete(_mapper.Map<Renewal>(_renewal));
                        return Ok(new { Success = true, Result = "Renewal was delete" });
                        //break;
                    default:
                       
                        break;
                }

                return Ok(new { Success = true, Result = "No delete" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }
    }
}
