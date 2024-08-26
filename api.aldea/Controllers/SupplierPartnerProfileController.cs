using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.SupplierPartnerProfileConsultant;
using api.premier.Models.SupplierPartnerProfileService;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.SupplierPartnerProfile;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Task = System.Threading.Tasks.Task;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierPartnerProfileController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly ISupplierPartnerProfileServiceRepository _supplierPartnerProfileServiceRepository;
        private readonly ISupplierPartnerProfileConsultantRepository _supplierPartnerProfileConsultantRepository;
        private readonly IUserRepository _userRepository;

        public SupplierPartnerProfileController(IMapper mapper, ILoggerManager loggerManager, IUtiltyRepository utiltyRepository, 
            ISupplierPartnerProfileServiceRepository supplierPartnerProfileServiceRepository, ISupplierPartnerProfileConsultantRepository supplierPartnerProfileConsultantRepository,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _supplierPartnerProfileServiceRepository = supplierPartnerProfileServiceRepository;
            _supplierPartnerProfileConsultantRepository = supplierPartnerProfileConsultantRepository;
            _userRepository = userRepository;
        }

        [HttpPost("PostService", Name = "PostService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SupplierPartnerProfileServiceDto>> PostService([FromBody] SupplierPartnerProfileServiceDto dto)
        {
            var response = new ApiResponse<SupplierPartnerProfileServiceDto>();
            try
            {
                if(dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/SupplierPartnerProfileService/", dto.PhotoExtension);
                }
                foreach(var acs in dto.AreasCoverageServices)
                {
                    foreach(var document in acs. DocumentAreasCoverageServices)
                    {
                        if (document.FilePath.Length > 150)
                            document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAreasCoverageServices/", document.FileExtension);
                    }

                    foreach(var administrative in acs.AdministrativeContactsServices)
                    {
                        if(administrative.Photo.Length > 150)
                        {
                            administrative.Photo = _utiltyRepository.UploadImageBase64(administrative.Photo, "Files/AdministrativeContactsServices/", administrative.PhotoExtension);
                        }
                        foreach(var document in administrative.DocumentAdministrativeContactsServices)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAdministrativeContactsServices/", document.FileExtension);
                        }
                    }

                    foreach (var consultant in acs.ConsultantContactsServices)
                    {
                        if(consultant.Photo.Length > 150)
                        {
                            consultant.Photo = _utiltyRepository.UploadImageBase64(consultant.Photo, "Files/ConsultantContactsServices/", consultant.PhotoExtension);
                        }
                        foreach (var document in consultant.DocumentConsultantContactsServices)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentConsultantContactsServices/", document.FileExtension);
                        }
                        foreach(var vehicle in consultant.VehicleServices)
                        {
                            foreach (var document in vehicle.DocumentVehicleServices)
                            {
                                if (document.FilePath.Length > 150)
                                    document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentVehicleServices/", document.FileExtension);
                            }
                            foreach (var photo in vehicle.PhotosVehicleServices)
                            {
                                if (photo.Photo.Length > 150)
                                    photo.Photo = _utiltyRepository.UploadImageBase64(photo.Photo, "Files/PhotosVehicleServices/", photo.PhotoExtension);
                            }
                        }
                    }
                }

                    var add = _supplierPartnerProfileServiceRepository.Add(_mapper.Map<SupplierPartnerProfileService>(dto));
                    response.Result = _mapper.Map<SupplierPartnerProfileServiceDto>(add);
                    response.Success = true;
                    response.Message = "Supplier Partener Profile Service Added";

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

        [HttpPut("PutService", Name = "PutService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SupplierPartnerProfileServiceDto>> PutService([FromBody] SupplierPartnerProfileServiceDto dto)
        {
            var response = new ApiResponse<SupplierPartnerProfileServiceDto>();
            try
            {
                if (dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/SupplierPartnerProfileService/", dto.PhotoExtension);
                }
                foreach (var acs in dto.AreasCoverageServices)
                {
                    foreach (var document in acs.DocumentAreasCoverageServices)
                    {
                        if(document.FilePath.Length > 150)
                            document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAreasCoverageServices/", document.FileExtension);
                    }

                    foreach (var administrative in acs.AdministrativeContactsServices)
                    {
                        if (administrative.Photo.Length > 150)
                        {
                            administrative.Photo = _utiltyRepository.UploadImageBase64(administrative.Photo, "Files/AdministrativeContactsServices/", administrative.PhotoExtension);
                        }
                        foreach (var document in administrative.DocumentAdministrativeContactsServices)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAdministrativeContactsServices/", document.FileExtension);
                        }
                    }

                    foreach (var consultant in acs.ConsultantContactsServices)
                    {
                        if (consultant.Photo.Length > 150)
                        {
                            consultant.Photo = _utiltyRepository.UploadImageBase64(consultant.Photo, "Files/ConsultantContactsServices/", consultant.PhotoExtension);
                        }
                        foreach (var document in consultant.DocumentConsultantContactsServices)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentConsultantContactsServices/", document.FileExtension);
                        }
                        foreach (var vehicle in consultant.VehicleServices)
                        {
                            foreach (var document in vehicle.DocumentVehicleServices)
                            {
                                if (document.FilePath.Length > 150)
                                    document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentVehicleServices/", document.FileExtension);
                            }
                            foreach (var photo in vehicle.PhotosVehicleServices)
                            {
                                if (photo.Photo.Length > 150)
                                    photo.Photo = _utiltyRepository.UploadImageBase64(photo.Photo, "Files/PhotosVehicleServices/", photo.PhotoExtension);
                            }
                        }
                    }
                }
                var add = _supplierPartnerProfileServiceRepository.UpdatedCustom(_mapper.Map<SupplierPartnerProfileService>(dto), dto.Id);
                response.Result = _mapper.Map<SupplierPartnerProfileServiceDto>(add);
                response.Success = true;
                response.Message = "Supplier Partener Profile Service Updated";
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

        [HttpGet("GetService", Name = "GetService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SupplierPartnerProfileServiceDto>> GetService([FromQuery] int key)
        {
            var response = new ApiResponse<SupplierPartnerProfileServiceDto>();
            try
            {
                response.Result = _mapper.Map<SupplierPartnerProfileServiceDto>(_supplierPartnerProfileServiceRepository.GetCustom(key));
                response.Success = true;
                response.Message = "Supplier Partner Profile Service Updated";
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(202, response);
        }

        [HttpPost("PostConsultant", Name = "PostConsultant")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SupplierPartnerProfileConsultantDto>> PostConsultant([FromBody] SupplierPartnerProfileConsultantDto dto)
        {
            var response = new ApiResponse<SupplierPartnerProfileConsultantDto>();
            try
            {
                if (dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/SupplierPartnerProfileConsultant/", dto.PhotoExtension);
                }
                foreach (var acs in dto.AreasCoverageConsultants)
                {
                    foreach (var document in acs.DocumentAreasCoverageConsultants)
                    {
                        if (document.FilePath.Length > 150)
                            document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAreasCoverageConsultants/", document.FileExtension);
                    }

                    foreach (var administrative in acs.AdministrativeContactsConsultants)
                    {
                        if (administrative.Photo.Length > 150)
                        {
                            administrative.Photo = _utiltyRepository.UploadImageBase64(administrative.Photo, "Files/AdministrativeContactsConsultants/", administrative.PhotoExtension);
                        }
                        foreach (var document in administrative.DocumentAdministrativeContactsConsultants)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAdministrativeContactsConsultants/", document.FileExtension);
                        }
                    }

                    foreach (var consultant in acs.ProfileUsers)
                    {
                        if (_userRepository.Exists(c => c.Email == consultant.Email))
                        {
                            response.Success = false;
                            response.Message = $"Email: { consultant.Email } Already Exists";
                            return BadRequest(response);
                        }
                        UserCreateDto user = new UserCreateDto();
                        user.Email = consultant.Email;
                        user.UserTypeId = 3;//consultant.Title == 1 ? 4 : consultant.Title == 2 ? 3 : consultant.Title == 3 ? 1 : 1 ;
                        user.RoleId = 3; //consultant.Title == 1 ? 3 : consultant.Title == 2 ? 2 : consultant.Title == 3 ? 1 : 1;
                        user.ServiceLineId = 1;
                        user.Password = _supplierPartnerProfileServiceRepository.HashPassword("$" + Guid.NewGuid().ToString().Substring(0, 7));
                        user.CreatedDate = DateTime.Now;
                        user.UpdatedDate = DateTime.Now;
                        user.Reset = true;
                        consultant.User = user;
                        consultant.UserId = 0;
                        if (consultant.Photo.Length > 150)
                        {
                            consultant.Photo = _utiltyRepository.UploadImageBase64(consultant.Photo, "Files/ConsultantContactsConsultants/", consultant.PhotoExtension);
                            user.Avatar = consultant.Photo;
                        }
                        foreach (var document in consultant.DocumentConsultantContactsConsultants)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentConsultantContactsConsultants/", document.FileExtension);
                        }
                        foreach (var vehicle in consultant.VehicleConsultants)
                        {
                            foreach (var document in vehicle.DocumentVehicleConsultants)
                            {
                                if (document.FilePath.Length > 150)
                                    document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentVehicleConsultants/", document.FileExtension);
                            }
                            foreach (var photo in vehicle.PhotosVehicleConsultants)
                            {
                                if (photo.Photo.Length > 150)
                                    photo.Photo = _utiltyRepository.UploadImageBase64(photo.Photo, "Files/PhotosVehicleConsultants/", photo.PhotoExtension);
                            }
                        }
                    }
                }
                var add = _supplierPartnerProfileConsultantRepository.Add(_mapper.Map<SupplierPartnerProfileConsultant>(dto));
                response.Result = _mapper.Map<SupplierPartnerProfileConsultantDto>(add);
                response.Success = true;
                response.Message = "Supplier Partner Profile Consultant Added";
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

        [HttpPut("PutConsultant", Name = "PutConsultant")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<SupplierPartnerProfileConsultantDto>>> PutConsultant([FromBody] SupplierPartnerProfileConsultantDto dto)
        {
            var response = new ApiResponse<SupplierPartnerProfileConsultantDto>();
            try
            {
                if (dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/SupplierPartnerProfileConsultant/", dto.PhotoExtension);
                }
                foreach (var acs in dto.AreasCoverageConsultants)
                {
                    foreach (var document in acs.DocumentAreasCoverageConsultants)
                    {
                        if (document.FilePath.Length > 150)
                            document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAreasCoverageConsultants/", document.FileExtension);
                    }

                    foreach (var administrative in acs.AdministrativeContactsConsultants)
                    {
                        if (administrative.Photo.Length > 150)
                        {
                            administrative.Photo = _utiltyRepository.UploadImageBase64(administrative.Photo, "Files/AdministrativeContactsConsultants/", administrative.PhotoExtension);
                        }
                        foreach (var document in administrative.DocumentAdministrativeContactsConsultants)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentAdministrativeContactsConsultants/", document.FileExtension);
                        }
                    }

                    foreach (var consultant in acs.ProfileUsers)
                    {
                        if (consultant.Photo.Length > 150)
                        {
                            consultant.Photo = _utiltyRepository.UploadImageBase64(consultant.Photo, "Files/ConsultantContactsConsultants/", consultant.PhotoExtension);
                        }
                        foreach (var document in consultant.DocumentConsultantContactsConsultants)
                        {
                            if (document.FilePath.Length > 150)
                                document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentConsultantContactsConsultants/", document.FileExtension);
                        }
                        foreach (var vehicle in consultant.VehicleConsultants)
                        {
                            foreach (var document in vehicle.DocumentVehicleConsultants)
                            {
                                if (document.FilePath.Length > 150)
                                    document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentVehicleConsultants/", document.FileExtension);
                            }
                            foreach (var photo in vehicle.PhotosVehicleConsultants)
                            {
                                if (photo.Photo.Length > 150)
                                    photo.Photo = _utiltyRepository.UploadImageBase64(photo.Photo, "Files/PhotosVehicleConsultants/", photo.PhotoExtension);
                            }
                        }
                    }
                }
                var add = await Task.WhenAll(_supplierPartnerProfileConsultantRepository.UpdatedCustom(
                    _mapper.Map<SupplierPartnerProfileConsultant>(dto), dto.Id
                ));
                response.Result = _mapper.Map<SupplierPartnerProfileConsultantDto>(dto);
                response.Success = true;
                response.Message = "Supplier Partner Profile Consultant Updated";
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
        
        [HttpGet("GetConsultant", Name = "GetConsultant")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SupplierPartnerProfileConsultantDto>> GetConsultant([FromQuery] int key)
        {
            var response = new ApiResponse<SupplierPartnerProfileConsultantDto>();
            try
            {
                response.Result = _mapper.Map<SupplierPartnerProfileConsultantDto>(_supplierPartnerProfileConsultantRepository.GetCustom(key));
                response.Success = true;
                response.Message = "Supplier Partner Profile Consultant found it.";
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(202, response);
        }

        [HttpGet("GetSupplierPartners", Name = "GetSupplierPartners")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartners([FromQuery] int? supplierCategory, [FromQuery] int? partnerType, [FromQuery] int? country, [FromQuery] int? city, [FromQuery] int? status)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetSupplierPartners(supplierCategory, partnerType, country, city, status);
                return StatusCode(202, new { 
                    Success = true, 
                    Message = "", 
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0});
            }
        }
        
        [HttpGet("GetSupplierPartnerServiceByServices", Name = "GetSupplierPartnerServiceByServices")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartnerServiceByServices([FromQuery] int workOrderService, [FromQuery] int? supplierType, [FromQuery] int? serviceLine)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetSupplierPartnerServiceByServices(workOrderService, supplierType, serviceLine);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetServiceProviderByServiceId", Name = "GetServiceProviderByServiceId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetServiceProviderByServiceId([FromQuery] int workOrderService)
        {
            try
            {
                var serviceproviders = _supplierPartnerProfileServiceRepository.GetServiceProviderByServiceId(workOrderService);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = serviceproviders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }


        [HttpGet("GetServProvByServiceTypeCountry", Name = "GetServProvByServiceTypeCountry")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetServProvByServiceTypeCountry([FromQuery] int workOrderService, int type)
        {
            try
            {
                var serviceproviders = _supplierPartnerProfileServiceRepository.GetServProvByServiceTypeCountry(workOrderService, type);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = serviceproviders
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }


        [HttpGet("GetAdministrativeContactsServiceBySupplierPartner", Name = "GetAdministrativeContactsServiceBySupplierPartner")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAdministrativeContactsServiceBySupplierPartner([FromQuery] int workOrderService, [FromQuery] int supplierPartner)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetAdministrativeContactsServiceBySupplierPartner(workOrderService, supplierPartner);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetConsultantContactsService", Name = "GetConsultantContactsService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConsultantContactsService([FromQuery] int? supplierPartner, [FromQuery] int? supplierType)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetConsultantContactsService(supplierPartner, supplierType);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }


        [HttpGet("GetAdmintContactsServiceProv", Name = "GetAdmintContactsServiceProv")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAdmintContactsServiceProv([FromQuery] int supplierPartner, [FromQuery] int workOrderService)
        {
            try
            {
               // var supplierPartners = _supplierPartnerProfileServiceRepository.GetConsultantContactsService(supplierPartner, supplierType);
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetAdmintContactsServiceProv(supplierPartner, workOrderService);

                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetSupplierPartnerConsultant", Name = "GetSupplierPartnerConsultant")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartnerConsultant([FromQuery] int country, [FromQuery] int city, [FromQuery] int? serviceLine)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileConsultantRepository.GetSupplierPartnerConsultant(country, city, serviceLine);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetConsultantContactsConsultants", Name = "GetConsultantContactsConsultants")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetConsultantContactsConsultants([FromQuery] int? supplierPartner, [FromQuery] int country, [FromQuery] int city, [FromQuery] int? serviceLine)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileConsultantRepository.GetConsultantContactsConsultants(supplierPartner, country, city, serviceLine.Value);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetSupplierPartnerServiceInvoice", Name = "GetSupplierPartnerServiceInvoice")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartnerServiceInvoice([FromQuery] int sr)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetSupplierPartnerServiceInvoice(sr);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }
        
        [HttpGet("GetSupplierPartnersBySR/{sr}", Name = "GetSupplierPartnersBySR")]
        public ActionResult GetSupplierPartnersBySR(int sr)
        {
            try
            {
                var supplierPartners = _supplierPartnerProfileServiceRepository.GetSupplierPartnersBySR(sr);
                return StatusCode(202, new
                {
                    Success = true,
                    Message = "",
                    Result = supplierPartners
                });
            }
            catch(Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpDelete("Delete/Service/AdministrativeContact/{id}")]
        public ActionResult<ApiResponse<bool>> AdministrativeContact(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                _supplierPartnerProfileServiceRepository.DeleteAdministrativeContact(id);
                response.Result = true;
                response.Success = true;
                response.Message = "Administrative Contact was removed successfully.";
            }
            catch (DbException ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202,response);
        }
        
        [HttpDelete("Delete/Service/ConsultantContacts/{id}")]
        public ActionResult<ApiResponse<bool>> ConsultantContactsServices(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                _supplierPartnerProfileServiceRepository.DeleteConsultantContact(id);
            }
            catch (DbException ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202,response);
        }

        [HttpDelete("Delete/Service/Payment/{id}")]
        public ActionResult<ApiResponse<bool>> DeleteServicePayment(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                var res = _supplierPartnerProfileServiceRepository.DeletePaymentInformation(id);
                response.Message = "Payment was delete successfully.";
                response.Result = res;
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202,response);
        }
        
        [HttpDelete("Delete/Consultant/Payment/{id}")]
        public ActionResult<ApiResponse<bool>> DeleteConsultantPayment(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                var res = _supplierPartnerProfileConsultantRepository.DeletePaymentInformation(id);
                response.Message = "Payment was delete successfully.";
                response.Result = res;
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202,response);
        }
        
        [HttpDelete("Delete/Service/WireTransfer/{id}")]
        public ActionResult<ApiResponse<bool>> DeleteServiceWireTransfer(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                var res = _supplierPartnerProfileServiceRepository.DeleteWireTransfer(id);
                response.Message = "Wire Transfer was delete successfully.";
                response.Result = res;
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202,response);
        }
        
        [HttpDelete("Delete/Consultant/WireTransfer/{id}")]
        public ActionResult<ApiResponse<bool>> DeleteConsultantWireTransfer(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                var res = _supplierPartnerProfileConsultantRepository.DeleteWireTransfer(id);
                response.Message = "Wire Transfer was delete successfully.";
                response.Result = res;
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.Message.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202,response);
        }
        
    }
}
