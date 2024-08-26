using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ProfileUser;
using api.premier.Models.SupplierPartnerProfileConsultant;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.DocumentConsultantContactsService;
using biz.premier.Repository.DocumentAdministrativeContactsConsultant;
using biz.premier.Repository.DocumentAdministrativeContactsService;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.SupplierPartnerProfile;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IDocumentConsultantContactsServiceRepository _documentConsultantContactsServiceRepository;
        private readonly IDocumentAdministrativeContactsConsultantRepository _documentAdministrativeContactsConsultantRepository;
        private readonly IDocumentAdministrativeContactsServiceRepository _documentAdministrativeContactsServiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISupplierPartnerProfileServiceRepository _supplierPartnerProfileServiceRepository;
        public ProfileController(IMapper mapper, ILoggerManager loggerManager, IProfileUserRepository profileUserRepository, IUtiltyRepository utiltyRepository,
            IUserRepository userRepository, ISupplierPartnerProfileServiceRepository supplierPartnerProfileServiceRepository
            , IDocumentConsultantContactsServiceRepository documentConsultantContactsServiceRepository
            , IDocumentAdministrativeContactsConsultantRepository documentAdministrativeContactsConsultantRepository
            , IDocumentAdministrativeContactsServiceRepository documentAdministrativeContactsServiceRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _profileUserRepository = profileUserRepository;
            _utiltyRepository = utiltyRepository;
            _userRepository = userRepository;
            _supplierPartnerProfileServiceRepository = supplierPartnerProfileServiceRepository;
            _documentConsultantContactsServiceRepository = documentConsultantContactsServiceRepository;
            _documentAdministrativeContactsConsultantRepository = documentAdministrativeContactsConsultantRepository;
            _documentAdministrativeContactsServiceRepository = documentAdministrativeContactsServiceRepository;
        }

        // Post Create new Profile 
        [HttpPost("AddProfile", Name = "AddProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ProfileUserDto>> PostAppointment([FromBody] ProfileUserDto dto)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                if (_userRepository.Exists(c => c.Email == dto.Email))
                {
                    response.Success = false;
                    response.Message = $"Email: { dto.Email } Already Exists";
                    return BadRequest(response);
                }
                if (dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/ConsultantContactsConsultants/", dto.PhotoExtension);
                }
                UserCreateDto user = new UserCreateDto();
                user.Email = dto.Email;
                user.Avatar = dto.Photo;
                user.Name = dto.Name;
                user.LastName = dto.LastName;
                user.MotherLastName = dto.MotherLastName;
                user.UserTypeId = dto.role_ID.Value; //dto.Title == 1 ? 4 : dto.Title == 2 ? 3 : dto.Title == 3 ? 1 : 1;
                user.RoleId = dto.role_ID.Value;//dto.Title == 1 ? 3 : dto.Title == 2 ? 2 : dto.Title == 3 ? 1 : 1;
                user.ServiceLineId = 1;
                user.Password = _supplierPartnerProfileServiceRepository.HashPassword("$" + Guid.NewGuid().ToString().Substring(0, 7));
                user.CreatedDate = DateTime.Now;
                user.UpdatedDate = DateTime.Now;
                user.Reset = true;
                user.Push = true;

                dto.User = user;
                dto.UserId = 0;

                if(dto.Title == 1)
                {
                    OfficeDto office = new OfficeDto();
                    dto.Offices = new List<OfficeDto>();
                    office.Consultant = 0;
                    office.Office1 = dto.ResponsablePremierOffice.Value;
                    dto.Offices.Add(office);
                }
                foreach (var document in dto.DocumentConsultantContactsConsultants)
                {
                    if (document.FilePath.Length > 150)
                        document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentConsultantContactsConsultants/", document.FileExtension);
                }
                foreach (var vehicle in dto.VehicleConsultants)
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
                ProfileUserDto profileUser = _mapper.Map<ProfileUserDto>(_profileUserRepository.AddConsultant(_mapper.Map<ProfileUser>(dto)));

                response.Success = true;
                response.Message = "Success";
                response.Result = profileUser;
                
                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email.html"));
                string body = string.Empty;
                body = reader.ReadToEnd();
                body = body.Replace("{user}", $"{user.Name} {user.LastName} {user.MotherLastName}");
                body = body.Replace("{username}", $"{user.Email}");
                body = body.Replace("{pass}", " $" + user.Password);

                _userRepository.SendMail(user.Email, body, "App Access");
                
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
        [HttpPut("UpdateProfile", Name = "UpdateProfile")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ProfileUserDto>> UpdateProfile([FromBody] ProfileUserDto dto)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                var user = _userRepository.Find(f => f.Id == dto.UserId);
                
                user.Email = dto.Email;
                user.Name = dto.Name;
                user.LastName = dto.LastName;
                user.MotherLastName = dto.MotherLastName;
               // user.UserTypeId = dto.role_ID.Value; //dto.Title == 1 ? 4 : dto.Title == 2 ? 3 : dto.Title == 3 ? 1 : 1;
               // user.RoleId = dto.role_ID.Value;//dto.Title == 1 ? 3 : dto.Title == 2 ? 2 : dto.Title == 3 ? 1 : 1;
                user.UpdatedDate = DateTime.Now;

                if (dto.Photo.Length > 150)
                {
                    dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/ConsultantContactsConsultants/", dto.PhotoExtension);
                    user.Avatar = dto.Photo;
                }
                foreach (var document in dto.DocumentConsultantContactsConsultants)
                {
                    if (document.FilePath.Length > 150)
                        document.FilePath = _utiltyRepository.UploadImageBase64(document.FilePath, "Files/DocumentConsultantContactsConsultants/", document.FileExtension);
                }
                foreach (var vehicle in dto.VehicleConsultants)
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
                ProfileUserDto profileUser = _mapper.Map<ProfileUserDto>(_profileUserRepository.UpdateCustom(_mapper.Map<ProfileUser>(dto), dto.Id));
                _userRepository.Update(user, user.Id);
                
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
        // DELETE 
        [HttpDelete("{id}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProfile(int id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                
                // ProfileUserDto profileUser = _mapper.Map<ProfileUserDto>(_profileUserRepository.GetConsultant(id));
                // _profileUserRepository.DeleteCustom(_mapper.Map<ProfileUser>(profileUser));
                var profile = await _profileUserRepository.FindAsync(f => f.Id == id);
                await _profileUserRepository.DeleteAsyn(profile);
                
                response.Success = true;
                response.Message = "Success";
                response.Result = true;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = false;
                        response.Success = false;
                        response.Message = "Operation Not Permitted";
                        _logger.LogError("Operation Not Permitted");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }


        /// delete DocumentConsultantContactsService 

        [HttpDelete("DeleteDocumentContactsService", Name = "DeleteDocumentContactsService")]
        public ActionResult DeleteDocumentContactsService([FromQuery] int id)
        {
            try
            {
                var find = _documentConsultantContactsServiceRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _documentConsultantContactsServiceRepository.Delete(_mapper.Map<DocumentConsultantContactsService>(find));
                    return Ok(new { Success = true, Result = "Document was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Document Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        /// delete DocumentConsultantContactsService 

        [HttpDelete("documentAdministrativeContacts", Name = "documentAdministrativeContacts")]
        public ActionResult documentAdministrativeContacts([FromQuery] int id)
        {
            try
            {
                var find = _documentAdministrativeContactsConsultantRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _documentAdministrativeContactsConsultantRepository.Delete(_mapper.Map<DocumentAdministrativeContactsConsultant>(find));
                    return Ok(new { Success = true, Result = "Document was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Document Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        /// delete Document_Administrative_Contacts_Service y Administrative_Contacts_Service 

        [HttpDelete("DocumentAdministrativeContactsService", Name = "DocumentAdministrativeContactsService")]
        public ActionResult DocumentAdministrativeContactsService([FromQuery] int id)
        {
            try
            {
                var find = _documentAdministrativeContactsServiceRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _documentAdministrativeContactsServiceRepository.Delete(_mapper.Map<DocumentAdministrativeContactsService>(find));
                    return Ok(new { Success = true, Result = "Document was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Document Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }


        // Get Profile By Id
        [HttpGet("GetProfile/{key}/", Name = "GetProfile/{key}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ProfileUserDto>> GetProfile(int key)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                ProfileUserDto profileUser = _mapper.Map<ProfileUserDto>(_profileUserRepository.GetConsultant(key));
                if(profileUser.Title == 1)
                {
                    profileUser.SupplierPartner = profileUser.AreasCoverage.HasValue ?
                        _profileUserRepository.GetSupplierPartner(profileUser.AreasCoverage.Value)
                        : "N/A";
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
        // Get Profile By Id From App
        [HttpGet("App/GetProfile/{key}/", Name = "App/GetProfile/{key}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ProfileUserDto>> GetAppProfile(int key)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                ProfileUserDto profileUser = _mapper.Map<ProfileUserDto>(_profileUserRepository.GetConsultant(key));
                if(profileUser.Title == 1)
                {
                    profileUser.SupplierPartner = profileUser.AreasCoverage.HasValue ?
                        _profileUserRepository.GetSupplierPartner(profileUser.AreasCoverage.Value)
                        : "N/A";
                    var data = _profileUserRepository.GetPricingInfo(profileUser.AreasCoverage.Value);
                    profileUser.AmountPerHour = data.AmountPerHour;
                    profileUser.Currency = data.Currency;
                    profileUser.CreditTerms = data.CreditTerms;
                    profileUser.TaxesPercentage = data.TaxesPercentage;
                    profileUser.Vip = _profileUserRepository.isVip(profileUser.AreasCoverage.Value) ? "Yes" : "No";
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
        // Get Directory
        [HttpGet("GetDirectory", Name = "GetDirectory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDirectory(int? title, int? country, int? city, int? company, int? office)
        {
            try
            {
                return Ok(new { Success = true, result = _profileUserRepository.GetDirectory(title, country, city, company, office) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
        // Get Dashboard Inicio
        [HttpGet("GetDashboardInicio", Name = "GetDashboardInicio")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDashboardInicio(int userId)
        {
            try
            {
                return Ok(new { Success = true, result = _profileUserRepository.DashboardInicio(userId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
        // Get Clients
        [HttpGet("GetClients", Name = "GetClients")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetClients(int user)
        {
            try
            {
                return Ok(new { Success = true, result = _profileUserRepository.GetClients(user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
        // Get CountriesLeader
        [HttpGet("GetCountriesLeader", Name = "GetCountriesLeader")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCountriesLeader(int country)
        {
            try
            {
                return Ok(new { Success = true, result = _profileUserRepository.GetCountryLeader(country) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
        // Get AssignedTeam
        [HttpGet("GetAssignedTeam", Name = "GetAssignedTeam")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAssignedTeam(int? type)
        {
            try
            {
                var profiles = _profileUserRepository.GetAssignedTeam(type);
                return Ok(new { Success = true, result = profiles });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

    }
}
