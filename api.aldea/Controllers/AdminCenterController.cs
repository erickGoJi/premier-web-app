using api.premier.Models;
using api.premier.Models.Service;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Servicies;
using biz.premier.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models.Catalogos;
using api.premier.Models.Catalogs;
using api.premier.Models.Catalogue;
using api.premier.Models.ClientPartnerProfile;
using api.premier.Models.Country;
using api.premier.Models.TenancyManagement;
using biz.premier.Repository.Catalogs;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.ClientPartner;
using biz.premier.Repository.OfficeContactTypeRepository;
using biz.premier.Repository.TypeService;
using biz.premier.Repository.Utility;
using biz.premier.Repository.VisaCategory;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminCenterController : ControllerBase
    {
        private readonly biz.premier.Repository.AdminCenter.IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly ICompanyTypeRepository _companyTypeRepository;
        private readonly IPolicyTypeRepository _policyTypeRepository;
     //   private readonly IPartnerRepository _partnerRepository;
        private readonly ISupplierPartnerProfileStatusRepository _supplierPartnerProfileStatusRepository;
        private readonly IDocumentTypeRepsoitory _documentTypeRepsoitory;
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IVisaCategoryRepository _visaCategoryRepository;
        private readonly ISexRepository _sexRepository;
        private readonly IPetTypeRepository _petTypeRepository;
        private readonly IBreedRepository _breedRepository;
        private readonly IVehicleTypeRepository _vehicleTypeRepository;
        private readonly IProficiencyRepository _proficiencyRepository;
        private readonly IHighestLevelEducationRepository _highestLevelEducationRepository;
        private readonly ITaxePercentageRepository _taxePercentageRepository;
        private readonly IGradeSchoolingRepository _gradeSchoolingRepository;
        private readonly IResponsablePaymentRepository _responsablePaymentRepository;
        private readonly IPrivacyRepository _privacyRepository;
        private readonly IAreaCoverageTypeRepository _areaCoverageTypeRepository;
        private readonly ITypeServiceRepository _typeServiceRepository;
        private readonly ITransportTypeRepository _transportTypeRepository;
        private readonly ICoordinatorTypeRepository _coordinatorTypeRepository;
        private readonly INotificationTypeRepository _notificationTypeRepository;
        private readonly IContactTypeRepository _contactTypeRepository;
        private readonly ISupplierTypeRepository _supplierTypeRepository;
        private readonly ICatalogTypeRepository _catalogTypeRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IUpcomingEventRepository _upcomingEventRepository;
        private readonly ISlidePhraseRepository _slidePhraseRepository;
        private readonly ICatIndustryRepository _industryRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IReferralFeeRepository _referralFeeRepository;
        private readonly IResponsiblePremierOfficeRespository _responsiblePremierOfficeRespository;
        private readonly IContractTypeRepository _contractTypeRepository;
        private readonly IPropertyTypeHousingRepository _propertyTypeHousingRepository;
        private readonly IRelationshipContactRepository _relationshipContactRepository;
        private readonly IPricingScheduleRepository _pricingScheduleRepository;
        private readonly IBankAccountTypeRepository _bankAccountTypeRepository;
        private readonly ITimeZoneRepository _timeZoneRepository;
        private readonly IOfficeContactTypeRepository _officeContactTypeRepository;
        private readonly ITypeOfficeRepository _typeOfficeRepository;
        private readonly IClient_PartnerRepository _client_PartnerRepository;
        public AdminCenterController(biz.premier.Repository.AdminCenter.IServiceRepository serviceRepository, IMapper mapper, ILoggerManager loggerManager,
            IUtiltyRepository utiltyRepository,
            ICompanyTypeRepository companyTypeRepository,
            IPolicyTypeRepository policyTypeRepository,
            ISupplierPartnerProfileStatusRepository supplierPartnerProfileStatusRepository,
            IDocumentTypeRepsoitory documentTypeRepsoitory,
            IRelationshipRepository relationshipRepository,
            IVisaCategoryRepository visaCategoryRepository,
            ISexRepository sexRepository,
            IPetTypeRepository petTypeRepository,
            IBreedRepository breedRepository,
            IVehicleTypeRepository vehicleTypeRepository,
            IProficiencyRepository proficiencyRepository,
            IHighestLevelEducationRepository highestLevelEducationRepository,
            ITaxePercentageRepository taxePercentageRepository,
            IGradeSchoolingRepository gradeSchoolingRepository,
            IResponsablePaymentRepository responsablePaymentRepository,
            IPrivacyRepository privacyRepository,
            IAreaCoverageTypeRepository areaCoverageTypeRepository,
            ITypeServiceRepository typeServiceRepository,
            ITransportTypeRepository transportTypeRepository,
            ICoordinatorTypeRepository coordinatorTypeRepository,
            INotificationTypeRepository notificationTypeRepository,
            IContactTypeRepository contactTypeRepository,
            ISupplierTypeRepository supplierTypeRepository,
            ICatalogTypeRepository catalogTypeRepository,
            ICatalogRepository catalogRepository,
            ISlidePhraseRepository slidePhraseRepository,
            IUpcomingEventRepository upcomingEventRepository,
            ICatIndustryRepository industryRepository,
            IEventRepository eventRepository,
            IResponsiblePremierOfficeRespository responsiblePremierOfficeRespository,
            IReferralFeeRepository referralFeeRepository,
            IContractTypeRepository contractTypeRepository,
            IPropertyTypeHousingRepository propertyTypeHousingRepository,
            IRelationshipContactRepository relationshipContactRepository,
            IPricingScheduleRepository pricingScheduleRepository,
            IBankAccountTypeRepository bankAccountTypeRepository,
            ITimeZoneRepository timeZoneRepository,
            IOfficeContactTypeRepository officeContactTypeRepository,
            ITypeOfficeRepository typeOfficeRepository,
            IClient_PartnerRepository client_PartnerRepository
            )
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _companyTypeRepository = companyTypeRepository;
            _policyTypeRepository = policyTypeRepository;
            _supplierPartnerProfileStatusRepository = supplierPartnerProfileStatusRepository;
            _documentTypeRepsoitory = documentTypeRepsoitory;
            _relationshipRepository = relationshipRepository;
            _visaCategoryRepository = visaCategoryRepository;
            _sexRepository = sexRepository;
            _petTypeRepository = petTypeRepository;
            _breedRepository = breedRepository;
            _vehicleTypeRepository = vehicleTypeRepository;
            _proficiencyRepository = proficiencyRepository;
            _highestLevelEducationRepository = highestLevelEducationRepository;
            _taxePercentageRepository = taxePercentageRepository;
            _gradeSchoolingRepository = gradeSchoolingRepository;
            _responsablePaymentRepository = responsablePaymentRepository;
            _privacyRepository = privacyRepository;
            _areaCoverageTypeRepository = areaCoverageTypeRepository;
            _typeServiceRepository = typeServiceRepository;
            _transportTypeRepository = transportTypeRepository;
            _coordinatorTypeRepository = coordinatorTypeRepository;
            _notificationTypeRepository = notificationTypeRepository;
            _contactTypeRepository = contactTypeRepository;
            _supplierTypeRepository = supplierTypeRepository;
            _catalogTypeRepository = catalogTypeRepository;
            _catalogRepository = catalogRepository;
            _slidePhraseRepository = slidePhraseRepository;
            _upcomingEventRepository = upcomingEventRepository;
            _industryRepository = industryRepository;
            _eventRepository = eventRepository;
            _responsiblePremierOfficeRespository = responsiblePremierOfficeRespository;
            _referralFeeRepository = referralFeeRepository;
            _contractTypeRepository = contractTypeRepository;
            _propertyTypeHousingRepository = propertyTypeHousingRepository;
            _relationshipContactRepository = relationshipContactRepository;
            _pricingScheduleRepository = pricingScheduleRepository;
            _bankAccountTypeRepository = bankAccountTypeRepository;
            _timeZoneRepository = timeZoneRepository;
            _officeContactTypeRepository = officeContactTypeRepository;
            _typeOfficeRepository = typeOfficeRepository;
            _client_PartnerRepository = client_PartnerRepository;
        }
        #region Service 
        // GET: Cat All Services
        [HttpGet]
        [Route("GetAllServices/{serviceLine}")]
        public ActionResult GetAllServices(int serviceLine)
        {
            try
            {
                var lifeCircle = _serviceRepository.GetAllIncluding(x => x.Service1Navigation, y => y.ServiceCountries).Where(x => x.ServiceLine == serviceLine);
                return StatusCode(202, new
                {
                    Success = true,
                    Result = lifeCircle.Select(s => new
                    {
                        s.Id,
                        s.Service1Navigation.Service,
                        countries = s.ServiceCountries.Count,
                        countriesName = s.ServiceCountries.Select(s => s.CountryNavigation.Name).ToList()
                    }),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        // GET: Cat Service by Id
        [HttpGet]
        [Route("GetService/{id}")]
        public ActionResult<ApiResponse<ServiceSelectDto>> GetService(int id)
        {
            var response = new ApiResponse<ServiceSelectDto>();
            try
            {
                response.Result = _mapper.Map<ServiceSelectDto>(_serviceRepository.GetCustom(id));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        
        // Add: Cat Service
        [HttpPost]
        [Route("AddService")]
        public ActionResult<ApiResponse<ServiceDto>> AddService([FromBody] ServiceDto[] dto)
        {
            var response = new ApiResponse<ServiceDto>();
            try
            {
                for(int i = 0; i < dto.Count(); i++)
                {
                    if (!_serviceRepository.IsExistService(dto[i].Service1.Value))
                    {
                        foreach (var elemet in dto[i].ServiceCountries)
                        {
                            foreach (var d in elemet.DocumentServiceCountries)
                            {
                                d.FilePath = _utiltyRepository.UploadImageBase64(d.FilePath, "Files/Service/", d.FileExtension);
                            }
                        }
                        if(dto[i].Service1 != 0){
                            _serviceRepository.Add(_mapper.Map<Service>(dto[i]));
                        }
                    }
                    else {
                        var _service = _serviceRepository.searchServiceId(dto[i].Service1.Value);
                        response.Result = _mapper.Map<ServiceDto>(_serviceRepository.UpdateCustom(_mapper.Map<Service>(dto[i]), _service));
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        
        // Update: Service
        [HttpPut("UpdateService",Name = "UpdateService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ServiceDto>> UpdateService([FromBody] ServiceDto dto)
        {
            var response = new ApiResponse<ServiceDto>();
            try
            {
                foreach (var i in dto.ServiceCountries)
                {
                    foreach (var d in i.DocumentServiceCountries)
                    {
                        if (d.FilePath.Length > 150)
                            d.FilePath = _utiltyRepository.UploadImageBase64(d.FilePath, "Files/Service/", d.FileExtension);
                    }
                }
                
                // int[]? countries = dto.ServiceCountries.Any(x => x.Id == 0) ? dto.ServiceCountries.FirstOrDefault(x => x.Id == 0)?.Countries : null;
                // if (countries.Any())
                // {
                //     foreach (var country in countries)
                //     {
                //         ServiceCountryDto countryDto = new ServiceCountryDto();
                //         countryDto.Country = country;
                //         countryDto = dto.ServiceCountries.FirstOrDefault(x => x.Id == 0);
                //         dto.ServiceCountries.Add(countryDto);
                //     }
                // }
                response.Result = _mapper.Map<ServiceDto>(_serviceRepository.UpdateCustom(_mapper.Map<Service>(dto), dto.Id));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        
        // GET: Cat Service 
        [HttpGet]
        [Route("Services/ClientPartner/{serviceLine}")]
        public ActionResult<ApiResponse<List<ServiceDto>>> ServiceClientPartner(int serviceLine, int? idPartner)
        {
            var response = new ApiResponse<List<ServiceDto>>();
            try
            {
                if(idPartner != null)
                {
                    
                    var service_partner = _client_PartnerRepository.GetIdServicesByPartner(idPartner.Value, serviceLine);
                   

                    var services = _serviceRepository
                   .GetAllIncluding(x => x.ServiceCountries, a => a.Service1Navigation)
                   .Where(x => x.ServiceCountries.Any() && service_partner.Contains(x.Service1.Value))
                   .Select(s => new
                   {
                       s.Service1Navigation.Service,
                       s.Id,
                       s.Service1,
                       s.ServiceLine
                   }).ToList();

                    return StatusCode(202, new
                    {
                        Success = true,
                        Result = services,
                        Message = ""
                    });
                }
                else
                {
                    var services = _serviceRepository
                    .GetAllIncluding(x => x.ServiceCountries, a => a.Service1Navigation)
                    .Where(x => x.ServiceCountries.Any())
                    .Select(s => new
                    {
                        s.Service1Navigation.Service,
                        s.Id,
                        s.Service1,
                        s.ServiceLine
                    }).ToList();
                    return StatusCode(202, new
                    {
                        Success = true,
                        Result = services.Where(x => x.ServiceLine == serviceLine).ToList(),
                        Message = ""
                    });
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        
        // GET: Cat Service by Id
        [HttpGet]
        [Route("GetScopeDocument/{service}/{country}")]
        public ActionResult<ApiResponse<ServiceCountryDto>> GetScopeDocument(int service, int country)
        {
            var response = new ApiResponse<ServiceCountryDto>();
            try
            {
                var documents = _serviceRepository.GetCustom(service).ServiceCountries
                    .FirstOrDefault(x => x.Country == country);
                response.Result = _mapper.Map<ServiceCountryDto>(documents);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        
        // GET SCOPE DOCUMENTS TO SERVICE
        [HttpGet("ScopeDocuments/Service", Name = "ScopeDocuments/Service")]
        public ActionResult GetScopeDocumentsForService(int service, int client)
        {
            try
            {
                var documentsScope = _serviceRepository.GetScopeDocuments(service, client);
                return StatusCode(202, new
                {
                    Success = true,
                    Result = documentsScope,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // DELETE SCOPE DOCUMENT
        [HttpDelete("ScopeDocument/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDocument(int id)
        {
            ApiResponse<bool> response = new ApiResponse<bool>();
            try
            {
                await _serviceRepository.DeleteDocumentAsync(id);
                response.Result = true;
                response.Success = true;
                response.Message = "Document was removed successfully";
            }
            catch (Exception e)
            {
                response.Result = false;
                response.Success = false;
                response.Message = $"Something was wrong: {e.Message}";
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion

        #region Company Type
        // Get: Company Type 
        [HttpGet("GetCompanyType", Name = "GetCompanyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CompanyTypeDto>>> GetCompanyType()
        {
            var response = new ApiResponse<List<CompanyTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CompanyTypeDto>>(_companyTypeRepository.GetAll());
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
        
        // Get: Company Type By Id
        [HttpGet("GetCompanyTypeById", Name = "GetCompanyTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CompanyTypeDto>> GetCompanyTypeById(int key)
        {
            var response = new ApiResponse<CompanyTypeDto>();
            try
            {
                response.Result =_mapper.Map<CompanyTypeDto>(_companyTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Company Type 
        [HttpPost("AddCompanyType", Name = "AddCompanyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CompanyTypeDto>> AddCompanyType(CompanyTypeDto key)
        {
            var response = new ApiResponse<CompanyTypeDto>();
            try
            {
                response.Result =_mapper.Map<CompanyTypeDto>(_companyTypeRepository.Add(_mapper.Map<CompanyType>(key)));
                response.Message = "Company Type was added successfully.";
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
        
        // Put: Update Company Type 
        [HttpPut("UpdateCompanyType", Name = "UpdateCompanyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CompanyTypeDto>> UpdateCompanyType(CompanyTypeDto key)
        {
            var response = new ApiResponse<CompanyTypeDto>();
            try
            {
                response.Result =_mapper.Map<CompanyTypeDto>(_companyTypeRepository.Update(_mapper.Map<CompanyType>(key), key.Id));
                response.Message = "Company Type was updated successfully.";
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
        
        // Delete: Delete Company Type by Id
        [HttpDelete("CompanyType/{key}", Name = "CompanyType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CompanyTypeDto>> DeleteCompanyType(int key)
        {
            var response = new ApiResponse<CompanyTypeDto>();
            try
            {
                var company = _companyTypeRepository.Find(x => x.Id == key);
                _companyTypeRepository.Delete(company);
                response.Result = null;
                response.Message = "Company Type was deleted successfully.";
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
        #endregion
        #region Policy Type
        // Get: Policy Type 
        [HttpGet("GetPolicyType", Name = "GetPolicyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatPolicyTypeDto>>> GetPolicyType()
        {
            var response = new ApiResponse<List<CatPolicyTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatPolicyTypeDto>>(_policyTypeRepository.GetAll());
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
        
        // Get: Policy Type By Id
        [HttpGet("GetPolicyTypeById", Name = "GetPolicyTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPolicyTypeDto>> GetPolicyTypeById(int key)
        {
            var response = new ApiResponse<CatPolicyTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatPolicyTypeDto>(_policyTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Policy Type 
        [HttpPost("AddPolicyType", Name = "AddPolicyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPolicyTypeDto>> AddPolicyType(CatPolicyTypeDto key)
        {
            var response = new ApiResponse<CatPolicyTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatPolicyTypeDto>(_policyTypeRepository.Add(_mapper.Map<CatPolicyType>(key)));
                response.Message = "Policy Type was added successfully.";
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
        
        // Put: Update Policy Type 
        [HttpPut("UpdatePolicyType", Name = "UpdatePolicyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPolicyTypeDto>> UpdatePolicyType(CatPolicyTypeDto key)
        {
            var response = new ApiResponse<CatPolicyTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatPolicyTypeDto>(_policyTypeRepository.Update(_mapper.Map<CatPolicyType>(key), key.Id));
                response.Message = "Policy Type was updated successfully.";
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
        
        // Delete: Delete Policy Type by Id
        [HttpDelete("PolicyType/{key}", Name = "PolicyType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPolicyTypeDto>> DeletePolicyType(int key)
        {
            var response = new ApiResponse<CatPolicyTypeDto>();
            try
            {
                var company = _policyTypeRepository.Find(x => x.Id == key);
                _policyTypeRepository.Delete(company);
                response.Result = null;
                response.Message = "Policy Type was deleted successfully.";
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
        #endregion
        #region Partner Status
        // Get: Partner Status
        [HttpGet("GetPartnerStatus", Name = "GetPartnerStatus")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatSupplierPartnerProfileStatusDto>>> GetPartnerStatus()
        {
            var response = new ApiResponse<List<CatSupplierPartnerProfileStatusDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatSupplierPartnerProfileStatusDto>>(_supplierPartnerProfileStatusRepository.GetAll());
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
        
        // Get: Partner Status By Id
        [HttpGet("GetPartnerStatusById", Name = "GetPartnerStatusById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierPartnerProfileStatusDto>> GetPartnerStatusById(int key)
        {
            var response = new ApiResponse<CatSupplierPartnerProfileStatusDto>();
            try
            {
                response.Result =_mapper.Map<CatSupplierPartnerProfileStatusDto>(_supplierPartnerProfileStatusRepository.Find(x => x.Id == key));
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
        
        // Post: Add Partner Status 
        [HttpPost("AddPartnerStatus", Name = "AddPartnerStatus")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierPartnerProfileStatusDto>> AddPartnerStatus(CatSupplierPartnerProfileStatusDto key)
        {
            var response = new ApiResponse<CatSupplierPartnerProfileStatusDto>();
            try
            {
                response.Result =_mapper.Map<CatSupplierPartnerProfileStatusDto>(_supplierPartnerProfileStatusRepository.Add(_mapper.Map<CatSupplierPartnerProfileStatus>(key)));
                response.Message = "Policy Type was added successfully.";
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
        
        // Put: Update Partner Status
        [HttpPut("UpdatePartnerStatus", Name = "UpdatePartnerStatus")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierPartnerProfileStatusDto>> UpdatePolicyType(CatSupplierPartnerProfileStatusDto key)
        {
            var response = new ApiResponse<CatSupplierPartnerProfileStatusDto>();
            try
            {
                response.Result =_mapper.Map<CatSupplierPartnerProfileStatusDto>(_supplierPartnerProfileStatusRepository.Update(_mapper.Map<CatSupplierPartnerProfileStatus>(key), key.Id));
                response.Message = "Policy Type was updated successfully.";
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
        
        // Delete: Delete Partner Status by Id
        [HttpDelete("PartnerStatus/{key}", Name = "PartnerStatus/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierPartnerProfileStatusDto>> DeletePartnerStatus(int key)
        {
            var response = new ApiResponse<CatSupplierPartnerProfileStatusDto>();
            try
            {
                var company = _supplierPartnerProfileStatusRepository.Find(x => x.Id == key);
                _supplierPartnerProfileStatusRepository.Delete(company);
                response.Result = null;
                response.Message = "Policy Type was deleted successfully.";
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
        #endregion

        #region Lead Status
        /*
         * Not Use
         */
        #endregion

        #region Document Type

        public class TypeDocument
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        // Get: Document Type
        [HttpGet("GetTypeDocuments", Name = "GetTypeDocuments")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<TypeDocument>>> GetTypeDocuments()
        {
            var response = new ApiResponse<List<TypeDocument>>();
            try
            {
                var types = new List<TypeDocument>()
                {
                    new TypeDocument(){Id = 1,Name = "Relocation"},
                    new TypeDocument(){Id = 2,Name = "Immigration"},
                    new TypeDocument(){Id = 3,Name = "Profiles"}
                };
                
                response.Result = types;
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
        
        // Get: Document Type
        [HttpGet("GetDocumentType", Name = "GetDocumentType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatDocumentTypeDto>>> GetDocumentType()
        {
            var response = new ApiResponse<List<CatDocumentTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatDocumentTypeDto>>(_documentTypeRepsoitory.GetAll());
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
        
        // Get: Partner Status By Id
        [HttpGet("GetDocumentTypeById", Name = "GetDocumentTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatDocumentTypeDto>> GetDocumentTypeById(int key)
        {
            var response = new ApiResponse<CatDocumentTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatDocumentTypeDto>(_documentTypeRepsoitory.Find(x => x.Id == key));
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
        
        // Post: Add Partner Status 
        [HttpPost("AddDocumentType", Name = "AddDocumentType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatDocumentTypeDto>> AddDocumentType(CatDocumentTypeDto key)
        {
            var response = new ApiResponse<CatDocumentTypeDto>();
            try
            {
                var data = _documentTypeRepsoitory.Add(_mapper.Map<CatDocumentType>(key));
                response.Result =_mapper.Map<CatDocumentTypeDto>(data);
                response.Message = "Document Type was added successfully.";
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
        
        // Put: Update Partner Status
        [HttpPut("UpdateDocumentType", Name = "UpdateDocumentType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatDocumentTypeDto>> UpdateDocumentType(CatDocumentTypeDto key)
        {
            var response = new ApiResponse<CatDocumentTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatDocumentTypeDto>(_documentTypeRepsoitory.Update(_mapper.Map<CatDocumentType>(key), key.Id));
                response.Message = "Document Type was updated successfully.";
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
        
        // Delete: Delete Document Type by Id
        [HttpDelete("DocumentType/{key}", Name = "DocumentType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatDocumentTypeDto>> DeleteDocumentType(int key)
        {
            var response = new ApiResponse<CatDocumentTypeDto>();
            try
            {
                var documentType = _documentTypeRepsoitory.Find(x => x.Id == key);
                _documentTypeRepsoitory.Delete(documentType);
                response.Result = null;
                response.Message = "Document Type was deleted successfully.";
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

        #endregion
        
        #region Relationship

        // Get: Relationship
        [HttpGet("GetRelationship", Name = "GetRelationship")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatRelationshipDto>>> GetRelationship()
        {
            var response = new ApiResponse<List<CatRelationshipDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatRelationshipDto>>(_relationshipRepository.GetAll());
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
        
        // Get: Partner Status By Id
        [HttpGet("GetRelationshipById", Name = "GetRelationshipById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipDto>> GetRelationshipById(int key)
        {
            var response = new ApiResponse<CatRelationshipDto>();
            try
            {
                response.Result =_mapper.Map<CatRelationshipDto>(_relationshipRepository.Find(x => x.Id == key));
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
        
        // Post: Add Partner Status 
        [HttpPost("AddRelationship", Name = "AddRelationship")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipDto>> AddRelationship(CatRelationshipDto key)
        {
            var response = new ApiResponse<CatRelationshipDto>();
            try
            {
                response.Result =_mapper.Map<CatRelationshipDto>(_relationshipRepository.Add(_mapper.Map<CatRelationship>(key)));
                response.Message = "Relationship was added successfully.";
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
        
        // Put: Update Partner Status
        [HttpPut("UpdateRelationship", Name = "UpdateRelationship")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipDto>> UpdateRelationship(CatRelationshipDto key)
        {
            var response = new ApiResponse<CatRelationshipDto>();
            try
            {
                response.Result =_mapper.Map<CatRelationshipDto>(_relationshipRepository.Update(_mapper.Map<CatRelationship>(key), key.Id));
                response.Message = "Relationship was updated successfully.";
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
        
        // Delete: Delete Relationship by Id
        [HttpDelete("Relationship/{key}", Name = "Relationship/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipDto>> DeleteRelationship(int key)
        {
            var response = new ApiResponse<CatRelationshipDto>();
            try
            {
                var relationship = _relationshipRepository.Find(x => x.Id == key);
                _relationshipRepository.Delete(relationship);
                response.Result = null;
                response.Message = "Relationship was deleted successfully.";
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

        #endregion
        
        #region Visa Category

        // Get: Visa Category
        [HttpGet("GetVisaCategory", Name = "GetVisaCategory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatVisaCategoryDto>>> GetVisaCategory()
        {
            var response = new ApiResponse<List<CatVisaCategoryDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatVisaCategoryDto>>(_visaCategoryRepository.GetAll());
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
        
        // Get: Visa Category By Id
        [HttpGet("GetVisaCategoryById", Name = "GetVisaCategoryById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVisaCategoryDto>> GetVisaCategoryById(int key)
        {
            var response = new ApiResponse<CatVisaCategoryDto>();
            try
            {
                response.Result =_mapper.Map<CatVisaCategoryDto>(_visaCategoryRepository.Find(x => x.Id == key));
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
        
        // Post: Add Visa Category 
        [HttpPost("AddVisaCategory", Name = "AddVisaCategory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVisaCategoryDto>> AddVisaCategory(CatVisaCategoryDto key)
        {
            var response = new ApiResponse<CatVisaCategoryDto>();
            try
            {
                response.Result =_mapper.Map<CatVisaCategoryDto>(_visaCategoryRepository.Add(_mapper.Map<CatVisaCategory>(key)));
                response.Message = "Visa Category was added successfully.";
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
        
        // Put: Update Visa Category
        [HttpPut("UpdateVisaCatogory", Name = "UpdateVisaCatogory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVisaCategoryDto>> UpdateVisaCatogory(CatVisaCategoryDto key)
        {
            var response = new ApiResponse<CatVisaCategoryDto>();
            try
            {
                response.Result =_mapper.Map<CatVisaCategoryDto>(_visaCategoryRepository.Update(_mapper.Map<CatVisaCategory>(key), key.Id));
                response.Message = "Visa Category was updated successfully.";
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
        
        // Delete: Delete Visa Category by Id
        [HttpDelete("VisaCategory/{key}", Name = "VisaCategory/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVisaCategoryDto>> DeleteVisaCategory(int key)
        {
            var response = new ApiResponse<CatVisaCategoryDto>();
            try
            {
                var visaCategory = _visaCategoryRepository.Find(x => x.Id == key);
                _visaCategoryRepository.Delete(visaCategory);
                response.Result = null;
                response.Message = "Visa Category was deleted successfully.";
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

        #endregion
        
        #region Sex

        // Get: Sex
        [HttpGet("GetSex", Name = "GetSex")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatSexDto>>> GetSex()
        {
            var response = new ApiResponse<List<CatSexDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatSexDto>>(_sexRepository.GetAll());
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
        
        // Get: Sex By Id
        [HttpGet("GetSexById", Name = "GetSexById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSexDto>> GetSexById(int key)
        {
            var response = new ApiResponse<CatSexDto>();
            try
            {
                response.Result =_mapper.Map<CatSexDto>(_sexRepository.Find(x => x.Id == key));
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
        
        // Post: Add Visa Category 
        [HttpPost("AddSex", Name = "AddSex")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSexDto>> AddSex(CatSexDto key)
        {
            var response = new ApiResponse<CatSexDto>();
            try
            {
                response.Result =_mapper.Map<CatSexDto>(_sexRepository.Add(_mapper.Map<CatSex>(key)));
                response.Message = "Sex was added successfully.";
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
        
        // Put: Sex Category
        [HttpPut("UpdateSex", Name = "UpdateSex")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSexDto>> UpdateSex(CatSexDto key)
        {
            var response = new ApiResponse<CatSexDto>();
            try
            {
                response.Result =_mapper.Map<CatSexDto>(_sexRepository.Update(_mapper.Map<CatSex>(key), key.Id));
                response.Message = "Sex was updated successfully.";
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
        
        // Delete: Delete Sex by Id
        [HttpDelete("Sex/{key}", Name = "Sex/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSexDto>> DeleteSex(int key)
        {
            var response = new ApiResponse<CatSexDto>();
            try
            {
                var sex = _sexRepository.Find(x => x.Id == key);
                _sexRepository.Delete(sex);
                response.Result = null;
                response.Message = "Sex was deleted successfully.";
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

        #endregion
        
        #region Pet Type

        // Get: Pet Type
        [HttpGet("GetPetType", Name = "GetPetType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatPetTypeDto>>> GetPetType()
        {
            var response = new ApiResponse<List<CatPetTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatPetTypeDto>>(_petTypeRepository.GetAll());
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
        
        // Get: Pet Type By Id
        [HttpGet("GetPetTypeId", Name = "GetPetTypeId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPetTypeDto>> GetPetTypeId(int key)
        {
            var response = new ApiResponse<CatPetTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatPetTypeDto>(_petTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Pet Type
        [HttpPost("AddPetType", Name = "AddPetType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPetTypeDto>> AddPetType(CatPetTypeDto key)
        {
            var response = new ApiResponse<CatPetTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatPetTypeDto>(_petTypeRepository.Add(_mapper.Map<CatPetType>(key)));
                response.Message = "Pet Type was added successfully.";
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
        
        // Put: Sex Category
        [HttpPut("UpdatePetType", Name = "UpdatePetType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPetTypeDto>> UpdatePetType(CatPetTypeDto key)
        {
            var response = new ApiResponse<CatPetTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatPetTypeDto>(_petTypeRepository.Update(_mapper.Map<CatPetType>(key), key.Id));
                response.Message = "Pet Type was updated successfully.";
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
        
        // Delete: Delete Pet Type by Id
        [HttpDelete("PetType/{key}", Name = "PetType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPetTypeDto>> DeletePetType(int key)
        {
            var response = new ApiResponse<CatPetTypeDto>();
            try
            {
                var petType = _petTypeRepository.Find(x => x.Id == key);
                _petTypeRepository.Delete(petType);
                response.Result = null;
                response.Message = "Pet Type was deleted successfully.";
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

        #endregion
        
        #region Breed

        // Get: Breed
        [HttpGet("GetBreed", Name = "GetBreed")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatBreedDto>>> GetBreed()
        {
            var response = new ApiResponse<List<CatBreedDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatBreedDto>>(_breedRepository.GetAllIncluding(x => x.PetType));
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
        
        // Get: Breed By Id
        [HttpGet("GetBreedId", Name = "GetBreedId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBreedDto>> GetBreedId(int key)
        {
            var response = new ApiResponse<CatBreedDto>();
            try
            {
                response.Result =_mapper.Map<CatBreedDto>(_breedRepository.Find(x => x.Id == key));
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
        
        // Post: Add Pet Type
        [HttpPost("AddBreed", Name = "AddBreed")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBreedDto>> AddBreed(CatBreedDto key)
        {
            var response = new ApiResponse<CatBreedDto>();
            try
            {
                response.Result =_mapper.Map<CatBreedDto>(_breedRepository.Add(_mapper.Map<CatBreed>(key)));
                response.Message = "Breed was added successfully.";
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
        
        // Put: Sex Category
        [HttpPut("UpdateBreed", Name = "UpdateBreed")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBreedDto>> UpdateBreed(CatBreedDto key)
        {
            var response = new ApiResponse<CatBreedDto>();
            try
            {
                response.Result =_mapper.Map<CatBreedDto>(_breedRepository.Update(_mapper.Map<CatBreed>(key), key.Id));
                response.Message = "Breed was updated successfully.";
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
        
        // Delete: Delete Breed by Id
        [HttpDelete("Breed/{key}", Name = "Breed/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBreedDto>> DeleteBreed(int key)
        {
            var response = new ApiResponse<CatBreedDto>();
            try
            {
                var breed = _breedRepository.Find(x => x.Id == key);
                _breedRepository.Delete(breed);
                response.Result = null;
                response.Message = "Breed was deleted successfully.";
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

        #endregion

        #region Vehicle Type

        // Get: Vehicle Type
        [HttpGet("GetVehicleType", Name = "GetVehicleType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatVehicleTypeDto>>> GetVehicleType()
        {
            var response = new ApiResponse<List<CatVehicleTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatVehicleTypeDto>>(_vehicleTypeRepository.GetAll());
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
        
        // Get: Vehicle Type By Id
        [HttpGet("GetVehicleTypeById", Name = "GetVehicleTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVehicleTypeDto>> GetVehicleTypeById(int key)
        {
            var response = new ApiResponse<CatVehicleTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatVehicleTypeDto>(_vehicleTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Vehicle Type
        [HttpPost("AddVehicleType", Name = "AddVehicleType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVehicleTypeDto>> AddVehicleType(CatVehicleTypeDto key)
        {
            var response = new ApiResponse<CatVehicleTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatVehicleTypeDto>(_vehicleTypeRepository.Add(_mapper.Map<CatVehicleType>(key)));
                response.Message = "Vehicle Type was added successfully.";
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
        
        // Put: Vehicle Type
        [HttpPut("UpdateVehicleType", Name = "UpdateVehicleType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVehicleTypeDto>> UpdateVehicleType(CatVehicleTypeDto key)
        {
            var response = new ApiResponse<CatVehicleTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatVehicleTypeDto>(_vehicleTypeRepository.Update(_mapper.Map<CatVehicleType>(key), key.Id));
                response.Message = "Vehicle Type was updated successfully.";
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
        
        // Delete: Delete Vehicle Type by Id
        [HttpDelete("VehicleType/{key}", Name = "VehicleType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatVehicleTypeDto>> DeleteVehicleType(int key)
        {
            var response = new ApiResponse<CatVehicleTypeDto>();
            try
            {
                var vehicleType = _vehicleTypeRepository.Find(x => x.Id == key);
                _vehicleTypeRepository.Delete(vehicleType);
                response.Result = null;
                response.Message = "Vehicle Type was deleted successfully.";
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

        #endregion
        
        #region Proficiency

        // Get: Proficiency
        [HttpGet("GetProficiency", Name = "GetProficiency")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatProficiencyDto>>> GetProficiency()
        {
            var response = new ApiResponse<List<CatProficiencyDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatProficiencyDto>>(_proficiencyRepository.GetAll());
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
        
        // Get: Proficiency By Id
        [HttpGet("GetProficiencyById", Name = "GetProficiencyById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatProficiencyDto>> GetProficiencyById(int key)
        {
            var response = new ApiResponse<CatProficiencyDto>();
            try
            {
                response.Result =_mapper.Map<CatProficiencyDto>(_proficiencyRepository.Find(x => x.Id == key));
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
        
        // Post: Add Proficiency
        [HttpPost("AddProficiency", Name = "AddProficiency")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatProficiencyDto>> AddProficiency(CatProficiencyDto key)
        {
            var response = new ApiResponse<CatProficiencyDto>();
            try
            {
                response.Result =_mapper.Map<CatProficiencyDto>(_proficiencyRepository.Add(_mapper.Map<CatProficiency>(key)));
                response.Message = "Proficiency was added successfully.";
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
        
        // Put: Proficiency
        [HttpPut("UpdateProficiency", Name = "UpdateProficiency")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatProficiencyDto>> UpdateProficiency(CatProficiencyDto key)
        {
            var response = new ApiResponse<CatProficiencyDto>();
            try
            {
                response.Result =_mapper.Map<CatProficiencyDto>(_proficiencyRepository.Update(_mapper.Map<CatProficiency>(key), key.Id));
                response.Message = "Proficiency was updated successfully.";
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
        
        // Delete: Delete Proficiency by Id
        [HttpDelete("Proficiency/{key}", Name = "Proficiency/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatProficiencyDto>> DeleteProficiency(int key)
        {
            var response = new ApiResponse<CatProficiencyDto>();
            try
            {
                var proficiency = _proficiencyRepository.Find(x => x.Id == key);
                _proficiencyRepository.Delete(proficiency);
                response.Result = null;
                response.Message = "Proficiency was deleted successfully.";
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

        #endregion
        
        #region Education Level

        // Get: Education Level
        [HttpGet("GetEducationLevel", Name = "GetEducationLevel")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatHighestLevelEducationDto>>> GetEducationLevel()
        {
            var response = new ApiResponse<List<CatHighestLevelEducationDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatHighestLevelEducationDto>>(_highestLevelEducationRepository.GetAll());
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
        
        // Get: Education Level By Id
        [HttpGet("GetEducationLevelById", Name = "GetEducationLevelById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatHighestLevelEducationDto>> GetEducationLevelById(int key)
        {
            var response = new ApiResponse<CatHighestLevelEducationDto>();
            try
            {
                response.Result =_mapper.Map<CatHighestLevelEducationDto>(_highestLevelEducationRepository.Find(x => x.Id == key));
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
        
        // Post: Add Education Level
        [HttpPost("AddEducationLevel", Name = "AddEducationLevel")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatHighestLevelEducationDto>> AddEducationLevel(CatHighestLevelEducationDto key)
        {
            var response = new ApiResponse<CatHighestLevelEducationDto>();
            try
            {
                response.Result =_mapper.Map<CatHighestLevelEducationDto>(_highestLevelEducationRepository.Add(_mapper.Map<CatHighestLevelEducation>(key)));
                response.Message = "Education Level was added successfully.";
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
        
        // Put: Education Level
        [HttpPut("UpdateEducationLevel", Name = "UpdateEducationLevel")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatHighestLevelEducationDto>> UpdateEducationLevel(CatHighestLevelEducationDto key)
        {
            var response = new ApiResponse<CatHighestLevelEducationDto>();
            try
            {
                response.Result =_mapper.Map<CatHighestLevelEducationDto>(_highestLevelEducationRepository.Update(_mapper.Map<CatHighestLevelEducation>(key), key.Id));
                response.Message = "Education Level was updated successfully.";
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
        
        // Delete: Delete Education Level by Id
        [HttpDelete("EducationLevel/{key}", Name = "EducationLevel/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatHighestLevelEducationDto>> DeleteEducationLevel(int key)
        {
            var response = new ApiResponse<CatHighestLevelEducationDto>();
            try
            {
                var highestLevelEducation = _highestLevelEducationRepository.Find(x => x.Id == key);
                _highestLevelEducationRepository.Delete(highestLevelEducation);
                response.Result = null;
                response.Message = "Education Level was deleted successfully.";
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

        #endregion
        
        #region Tax Percentage

        // Get: Taxes Percentage
        [HttpGet("GetTaxesPercentage", Name = "GetTaxesPercentage")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatTaxePercentageDto>>> GetTaxesPercentage()
        {
            var response = new ApiResponse<List<CatTaxePercentageDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatTaxePercentageDto>>(_taxePercentageRepository.GetAll());
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
        
        // Get: Taxes Percentage By Id
        [HttpGet("GetTaxesPercentageById", Name = "GetTaxesPercentageById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTaxePercentageDto>> GetTaxesPercentageById(int key)
        {
            var response = new ApiResponse<CatTaxePercentageDto>();
            try
            {
                response.Result =_mapper.Map<CatTaxePercentageDto>(_taxePercentageRepository.Find(x => x.Id == key));
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
        
        // Post: Add Taxes Percentage
        [HttpPost("AddTaxesPercentage", Name = "AddTaxesPercentage")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTaxePercentageDto>> AddTaxesPercentage(CatTaxePercentageDto key)
        {
            var response = new ApiResponse<CatTaxePercentageDto>();
            try
            {
                response.Result =_mapper.Map<CatTaxePercentageDto>(_taxePercentageRepository.Add(_mapper.Map<CatTaxePercentage>(key)));
                response.Message = "Taxes Percentage was added successfully.";
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
        
        // Put: Taxes Percentage
        [HttpPut("UpdateTaxesPercentage", Name = "UpdateTaxesPercentage")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTaxePercentageDto>> UpdateTaxesPercentage(CatTaxePercentageDto key)
        {
            var response = new ApiResponse<CatTaxePercentageDto>();
            try
            {
                response.Result =_mapper.Map<CatTaxePercentageDto>(_taxePercentageRepository.Update(_mapper.Map<CatTaxePercentage>(key), key.Id));
                response.Message = "Taxes Percentage was updated successfully.";
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
        
        // Delete: Delete Taxes Percentage by Id
        [HttpDelete("TaxesPercentage/{key}", Name = "TaxesPercentage/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTaxePercentageDto>> DeleteTaxesPercentage(int key)
        {
            var response = new ApiResponse<CatTaxePercentageDto>();
            try
            {
                var taxePercentage = _taxePercentageRepository.Find(x => x.Id == key);
                _taxePercentageRepository.Delete(taxePercentage);
                response.Result = null;
                response.Message = "Taxes Percentage was deleted successfully.";
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

        #endregion
        
        #region Grade Schooling

        // Get: Grade Schooling
        [HttpGet("GetGradeSchooling", Name = "GetGradeSchooling")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatGradeSchoolingDto>>> GetGradeSchooling()
        {
            var response = new ApiResponse<List<CatGradeSchoolingDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatGradeSchoolingDto>>(_gradeSchoolingRepository.GetAll());
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
        
        // Get: Grade Schooling By Id
        [HttpGet("GetGradeSchoolingById", Name = "GetGradeSchoolingById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatGradeSchoolingDto>> GetGradeSchoolingById(int key)
        {
            var response = new ApiResponse<CatGradeSchoolingDto>();
            try
            {
                response.Result =_mapper.Map<CatGradeSchoolingDto>(_gradeSchoolingRepository.Find(x => x.Id == key));
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
        
        // Post: Add Grade Schooling
        [HttpPost("AddGradeSchooling", Name = "AddGradeSchooling")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatGradeSchoolingDto>> AddGradeSchooling(CatGradeSchoolingDto key)
        {
            var response = new ApiResponse<CatGradeSchoolingDto>();
            try
            {
                response.Result =_mapper.Map<CatGradeSchoolingDto>(_gradeSchoolingRepository.Add(_mapper.Map<CatGradeSchooling>(key)));
                response.Message = "Grade Schooling was added successfully.";
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
        
        // Put: Grade Schooling
        [HttpPut("UpdateGradeSchooling", Name = "UpdateGradeSchooling")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatGradeSchoolingDto>> UpdateGradeSchooling(CatGradeSchoolingDto key)
        {
            var response = new ApiResponse<CatGradeSchoolingDto>();
            try
            {
                response.Result =_mapper.Map<CatGradeSchoolingDto>(_gradeSchoolingRepository.Update(_mapper.Map<CatGradeSchooling>(key), key.Id));
                response.Message = "Grade Schooling was updated successfully.";
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
        
        // Delete: Delete Grade Schooling by Id
        [HttpDelete("GradeSchooling/{key}", Name = "GradeSchooling/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatGradeSchoolingDto>> DeleteGradeSchooling(int key)
        {
            var response = new ApiResponse<CatGradeSchoolingDto>();
            try
            {
                var gradeSchooling = _gradeSchoolingRepository.Find(x => x.Id == key);
                _gradeSchoolingRepository.Delete(gradeSchooling);
                response.Result = null;
                response.Message = "Grade Schooling was deleted successfully.";
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

        #endregion
        
        #region Payment Responsibility

        // Get: Payment Responsibility
        [HttpGet("GetPaymentResponsibility", Name = "GetPaymentResponsibility")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatResponsablePaymentDto>>> GetPaymentResponsibility()
        {
            var response = new ApiResponse<List<CatResponsablePaymentDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatResponsablePaymentDto>>(_responsablePaymentRepository.GetAll());
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
        
        // Get: Payment Responsibility By Id
        [HttpGet("GetPaymentResponsibilityById", Name = "GetPaymentResponsibilityById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatResponsablePaymentDto>> GetPaymentResponsibilityById(int key)
        {
            var response = new ApiResponse<CatResponsablePaymentDto>();
            try
            {
                response.Result =_mapper.Map<CatResponsablePaymentDto>(_responsablePaymentRepository.Find(x => x.Id == key));
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
        
        // Post: Add Payment Responsibility
        [HttpPost("AddPaymentResponsibility", Name = "AddPaymentResponsibility")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatResponsablePaymentDto>> AddGradeSchooling(CatResponsablePaymentDto key)
        {
            var response = new ApiResponse<CatResponsablePaymentDto>();
            try
            {
                response.Result =_mapper.Map<CatResponsablePaymentDto>(_responsablePaymentRepository.Add(_mapper.Map<CatResponsablePayment>(key)));
                response.Message = "Payment Responsibility was added successfully.";
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
        
        // Put: Payment Responsibility
        [HttpPut("UpdatePaymentResponsibility", Name = "UpdatePaymentResponsibility")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatResponsablePaymentDto>> UpdatePaymentResponsibility(CatResponsablePaymentDto key)
        {
            var response = new ApiResponse<CatResponsablePaymentDto>();
            try
            {
                response.Result =_mapper.Map<CatResponsablePaymentDto>(_responsablePaymentRepository.Update(_mapper.Map<CatResponsablePayment>(key), key.Id));
                response.Message = "Payment Responsibility was updated successfully.";
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
        
        // Delete: Delete Payment Responsibility by Id
        [HttpDelete("PaymentResponsibility/{key}", Name = "PaymentResponsibility/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatResponsablePaymentDto>> DeletePaymentResponsibility(int key)
        {
            var response = new ApiResponse<CatResponsablePaymentDto>();
            try
            {
                var responsablePayment = _responsablePaymentRepository.Find(x => x.Id == key);
                _responsablePaymentRepository.Delete(responsablePayment);
                response.Result = null;
                response.Message = "Payment Responsibility was deleted successfully.";
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

        #endregion
        
        #region Privacy

        // Get: Privacy
        [HttpGet("GetPrivacy", Name = "GetPrivacy")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatPrivacyDto>>> GetPrivacy()
        {
            var response = new ApiResponse<List<CatPrivacyDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatPrivacyDto>>(_privacyRepository.GetAll());
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
        
        // Get: Privacy By Id
        [HttpGet("GetPrivacyById", Name = "GetPrivacyById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPrivacyDto>> GetPrivacyById(int key)
        {
            var response = new ApiResponse<CatPrivacyDto>();
            try
            {
                response.Result =_mapper.Map<CatPrivacyDto>(_privacyRepository.Find(x => x.Id == key));
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
        
        // Post: Add Privacy
        [HttpPost("AddPrivacy", Name = "AddPrivacy")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPrivacyDto>> AddPrivacy(CatPrivacyDto key)
        {
            var response = new ApiResponse<CatPrivacyDto>();
            try
            {
                response.Result =_mapper.Map<CatPrivacyDto>(_privacyRepository.Add(_mapper.Map<CatPrivacy>(key)));
                response.Message = "Privacy was added successfully.";
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
        
        // Put: Privacy
        [HttpPut("UpdatePrivacy", Name = "UpdatePrivacy")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPrivacyDto>> UpdatePrivacy(CatPrivacyDto key)
        {
            var response = new ApiResponse<CatPrivacyDto>();
            try
            {
                response.Result =_mapper.Map<CatPrivacyDto>(_privacyRepository.Update(_mapper.Map<CatPrivacy>(key), key.Id));
                response.Message = "Privacy was updated successfully.";
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
        
        // Delete: Delete Privacy by Id
        [HttpDelete("Privacy/{key}", Name = "Privacy/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPrivacyDto>> DeletePrivacy(int key)
        {
            var response = new ApiResponse<CatPrivacyDto>();
            try
            {
                var privacy = _privacyRepository.Find(x => x.Id == key);
                _privacyRepository.Delete(privacy);
                response.Result = null;
                response.Message = "Privacy was deleted successfully.";
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

        #endregion
        
        #region Coverage Type

        // Get: Coverage Type
        [HttpGet("GetCoverageType", Name = "GetCoverageType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatAreaCoverageTypeDto>>> GetCoverageType()
        {
            var response = new ApiResponse<List<CatAreaCoverageTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatAreaCoverageTypeDto>>(_areaCoverageTypeRepository.GetAll());
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
        
        // Get: Coverage Type By Id
        [HttpGet("GetCoverageTypeById", Name = "GetCoverageTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatAreaCoverageTypeDto>> GetCoverageTypeById(int key)
        {
            var response = new ApiResponse<CatAreaCoverageTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatAreaCoverageTypeDto>(_areaCoverageTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Coverage Type
        [HttpPost("AddCoverageType", Name = "AddCoverageType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatAreaCoverageTypeDto>> AddCoverageType(CatAreaCoverageTypeDto key)
        {
            var response = new ApiResponse<CatAreaCoverageTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatAreaCoverageTypeDto>(_areaCoverageTypeRepository.Add(_mapper.Map<CatAreaCoverageType>(key)));
                response.Message = "Coverage Type was added successfully.";
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
        
        // Put: Coverage Type
        [HttpPut("UpdateCoverageType", Name = "UpdateCoverageType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatAreaCoverageTypeDto>> UpdateCoverageType(CatAreaCoverageTypeDto key)
        {
            var response = new ApiResponse<CatAreaCoverageTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatAreaCoverageTypeDto>(_areaCoverageTypeRepository.Update(_mapper.Map<CatAreaCoverageType>(key), key.Id));
                response.Message = "Coverage Type was updated successfully.";
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
        
        // Delete: Delete Coverage Type by Id
        [HttpDelete("CoverageType/{key}", Name = "CoverageType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatAreaCoverageTypeDto>> DeleteCoverageType(int key)
        {
            var response = new ApiResponse<CatAreaCoverageTypeDto>();
            try
            {
                var coverageType = _areaCoverageTypeRepository.Find(x => x.Id == key);
                _areaCoverageTypeRepository.Delete(coverageType);
                response.Result = null;
                response.Message = "Coverage Type was deleted successfully.";
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

        #endregion
        
        #region Service Type

        // Get: Service Type
        [HttpGet("GetServiceType", Name = "GetServiceType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatTypeServiceDto>>> GetServiceType()
        {
            var response = new ApiResponse<List<CatTypeServiceDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatTypeServiceDto>>(_typeServiceRepository.GetAll());
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
        
        // Get: Service Type By Id
        [HttpGet("GetServiceTypeById", Name = "GetServiceTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTypeServiceDto>> GetServiceTypeById(int key)
        {
            var response = new ApiResponse<CatTypeServiceDto>();
            try
            {
                response.Result =_mapper.Map<CatTypeServiceDto>(_typeServiceRepository.Find(x => x.Id == key));
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
        
        // Post: Add Service Type
        [HttpPost("AddServiceType", Name = "AddServiceType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTypeServiceDto>> AddServiceType(CatTypeServiceDto key)
        {
            var response = new ApiResponse<CatTypeServiceDto>();
            try
            {
                response.Result =_mapper.Map<CatTypeServiceDto>(_typeServiceRepository.Add(_mapper.Map<CatTypeService>(key)));
                response.Message = "Service Type was added successfully.";
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
        
        // Put: Coverage Type
        [HttpPut("UpdateServiceType", Name = "UpdateServiceType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTypeServiceDto>> UpdateServiceType(CatTypeServiceDto key)
        {
            var response = new ApiResponse<CatTypeServiceDto>();
            try
            {
                response.Result =_mapper.Map<CatTypeServiceDto>(_typeServiceRepository.Update(_mapper.Map<CatTypeService>(key), key.Id));
                response.Message = "Service Type was updated successfully.";
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
        
        // Delete: Delete Service Type by Id
        [HttpDelete("ServiceType/{key}", Name = "ServiceType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTypeServiceDto>> DeleteServiceType(int key)
        {
            var response = new ApiResponse<CatTypeServiceDto>();
            try
            {
                var typeService = _typeServiceRepository.Find(x => x.Id == key);
                _typeServiceRepository.Delete(typeService);
                response.Result = null;
                response.Message = "Service Type was deleted successfully.";
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

        #endregion
        
        #region Transport Type

        // Get: Transport Type
        [HttpGet("GetTransportType", Name = "GetTransportType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatTransportTypeDto>>> GetTransportType()
        {
            var response = new ApiResponse<List<CatTransportTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatTransportTypeDto>>(_transportTypeRepository.GetAll());
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
        
        // Get: Transport Type By Id
        [HttpGet("GetTransportTypeById", Name = "GetTransportTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTransportTypeDto>> GetTransportTypeById(int key)
        {
            var response = new ApiResponse<CatTransportTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatTransportTypeDto>(_transportTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Service Type
        [HttpPost("AddTransportType", Name = "AddTransportType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTransportTypeDto>> AddTransportType(CatTransportTypeDto key)
        {
            var response = new ApiResponse<CatTransportTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatTransportTypeDto>(_transportTypeRepository.Add(_mapper.Map<CatTransportType>(key)));
                response.Message = "Transport Type was added successfully.";
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
        
        // Put: Transport Type
        [HttpPut("UpdateTransportType", Name = "UpdateTransportType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTransportTypeDto>> UpdateTransportType(CatTransportTypeDto key)
        {
            var response = new ApiResponse<CatTransportTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatTransportTypeDto>(_transportTypeRepository.Update(_mapper.Map<CatTransportType>(key), key.Id));
                response.Message = "Transport Type was updated successfully.";
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
        
        // Delete: Delete Transport Type by Id
        [HttpDelete("TransportType/{key}", Name = "TransportType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTransportTypeDto>> DeleteTransportType(int key)
        {
            var response = new ApiResponse<CatTransportTypeDto>();
            try
            {
                var transportType = _transportTypeRepository.Find(x => x.Id == key);
                _transportTypeRepository.Delete(transportType);
                response.Result = null;
                response.Message = "Transport Type was deleted successfully.";
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

        #endregion
        
        #region Coordinator Type

        // Get: Coordinator Type
        [HttpGet("GetCoordinatorType", Name = "GetCoordinatorType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatCoordinatorTypeDto>>> GetCoordinatorType()
        {
            var response = new ApiResponse<List<CatCoordinatorTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatCoordinatorTypeDto>>(_coordinatorTypeRepository.GetAll());
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
        
        // Get: Coordinator Type By Id
        [HttpGet("GetCoordinatorTypeById", Name = "GetCoordinatorTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCoordinatorTypeDto>> GetCoordinatorTypeById(int key)
        {
            var response = new ApiResponse<CatCoordinatorTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatCoordinatorTypeDto>(_coordinatorTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Coordinator Type
        [HttpPost("AddCoordinatorType", Name = "AddCoordinatorType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCoordinatorTypeDto>> AddCoordinatorType(CatCoordinatorTypeDto key)
        {
            var response = new ApiResponse<CatCoordinatorTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatCoordinatorTypeDto>(_coordinatorTypeRepository.Add(_mapper.Map<CatCoordinatorType>(key)));
                response.Message = "Coordinator Type was added successfully.";
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
        
        // Put: Coordinator Type
        [HttpPut("UpdateCoordinatorType", Name = "UpdateCoordinatorType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCoordinatorTypeDto>> UpdateCoordinatorType(CatCoordinatorTypeDto key)
        {
            var response = new ApiResponse<CatCoordinatorTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatCoordinatorTypeDto>(_coordinatorTypeRepository.Update(_mapper.Map<CatCoordinatorType>(key), key.Id));
                response.Message = "Coordinator Type was updated successfully.";
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
        
        // Delete: Delete Coordinator Type by Id
        [HttpDelete("CoordinatorType/{key}", Name = "CoordinatorType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCoordinatorTypeDto>> DeleteCoordinatorType(int key)
        {
            var response = new ApiResponse<CatCoordinatorTypeDto>();
            try
            {
                var coordinatorType = _coordinatorTypeRepository.Find(x => x.Id == key);
                _coordinatorTypeRepository.Delete(coordinatorType);
                response.Result = null;
                response.Message = "Coordinator Type was deleted successfully.";
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

        #endregion
        
        #region Notification Type

        // Get: Notification Type
        [HttpGet("GetNotificationType", Name = "GetNotificationType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatNotificationTypeDto>>> GetNotificationType()
        {
            var response = new ApiResponse<List<CatNotificationTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatNotificationTypeDto>>(_notificationTypeRepository.GetAll());
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
        
        // Get: Notification Type By Id
        [HttpGet("GetNotificationTypeById", Name = "GetNotificationTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatNotificationTypeDto>> GetNotificationTypeById(int key)
        {
            var response = new ApiResponse<CatNotificationTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatNotificationTypeDto>(_notificationTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Notification Type
        [HttpPost("AddNotificationType", Name = "AddNotificationType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatNotificationTypeDto>> AddNotificationType(CatNotificationTypeDto key)
        {
            var response = new ApiResponse<CatNotificationTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatNotificationTypeDto>(_notificationTypeRepository.Add(_mapper.Map<CatNotificationType>(key)));
                response.Message = "Notification Type was added successfully.";
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
        
        // Put: Notification Type
        [HttpPut("UpdateNotificationType", Name = "UpdateNotificationType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatNotificationTypeDto>> UpdateNotificationType(CatNotificationTypeDto key)
        {
            var response = new ApiResponse<CatNotificationTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatNotificationTypeDto>(_notificationTypeRepository.Update(_mapper.Map<CatNotificationType>(key), key.Id));
                response.Message = "Notification Type was updated successfully.";
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
        
        // Delete: Delete Notification Type by Id
        [HttpDelete("NotificationType/{key}", Name = "NotificationType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatNotificationTypeDto>> DeleteNotificationType(int key)
        {
            var response = new ApiResponse<CatNotificationTypeDto>();
            try
            {
                var notificationType = _notificationTypeRepository.Find(x => x.Id == key);
                _notificationTypeRepository.Delete(notificationType);
                response.Result = null;
                response.Message = "Notification Type was deleted successfully.";
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

        #endregion
        
        #region Contact Type

        // Get: Contact Type
        [HttpGet("GetContactType", Name = "GetContactType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatContactTypeDto>>> GetContactType()
        {
            var response = new ApiResponse<List<CatContactTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatContactTypeDto>>(_contactTypeRepository.GetAll());
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
        
        // Get: Contact Type By Id
        [HttpGet("GetContactTypeById", Name = "GetContactTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContactTypeDto>> GetContactTypeById(int key)
        {
            var response = new ApiResponse<CatContactTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatContactTypeDto>(_contactTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Contact Type
        [HttpPost("AddContactType", Name = "AddContactType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContactTypeDto>> AddContactType(CatContactTypeDto key)
        {
            var response = new ApiResponse<CatContactTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatContactTypeDto>(_contactTypeRepository.Add(_mapper.Map<CatContactType>(key)));
                response.Message = "Notification Type was added successfully.";
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
        
        // Put: Contact Type
        [HttpPut("UpdateContactType", Name = "UpdateContactType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContactTypeDto>> UpdateContactType(CatContactTypeDto key)
        {
            var response = new ApiResponse<CatContactTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatContactTypeDto>(_contactTypeRepository.Update(_mapper.Map<CatContactType>(key), key.Id));
                response.Message = "Notification Type was updated successfully.";
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
        
        // Delete: Delete Contact Type by Id
        [HttpDelete("ContactType/{key}", Name = "ContactType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContactTypeDto>> DeleteContactType(int key)
        {
            var response = new ApiResponse<CatContactTypeDto>();
            try
            {
                var contactType = _contactTypeRepository.Find(x => x.Id == key);
                _contactTypeRepository.Delete(contactType);
                response.Result = null;
                response.Message = "Contact Type was deleted successfully.";
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

        #endregion
        
        #region Supplier Type

        // Get: Supplier Type
        [HttpGet("GetSupplierType", Name = "GetSupplierType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatSupplierTypeDto>>> GetSupplierType()
        {
            var response = new ApiResponse<List<CatSupplierTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatSupplierTypeDto>>(_supplierTypeRepository.GetAll());
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
        
        // Get: Supplier Type By Id
        [HttpGet("GetSupplierTypeById", Name = "GetSupplierTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierTypeDto>> GetSupplierTypeById(int key)
        {
            var response = new ApiResponse<CatSupplierTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatSupplierTypeDto>(_supplierTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Supplier Type
        [HttpPost("AddSupplierType", Name = "AddSupplierType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierTypeDto>> AddSupplierType(CatSupplierTypeDto key)
        {
            var response = new ApiResponse<CatSupplierTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatSupplierTypeDto>(_supplierTypeRepository.Add(_mapper.Map<CatSupplierType>(key)));
                response.Message = "Supplier Type was added successfully.";
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
        
        // Put: Supplier Type
        [HttpPut("UpdateSupplierType", Name = "UpdateSupplierType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierTypeDto>> UpdateSupplierType(CatSupplierTypeDto key)
        {
            var response = new ApiResponse<CatSupplierTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatSupplierTypeDto>(_supplierTypeRepository.Update(_mapper.Map<CatSupplierType>(key), key.Id));
                response.Message = "Supplier Type was updated successfully.";
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
        
        // Delete: Delete Supplier Type by Id
        [HttpDelete("SupplierType/{key}", Name = "SupplierType/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatSupplierTypeDto>> DeleteSupplierType(int key)
        {
            var response = new ApiResponse<CatSupplierTypeDto>();
            try
            {
                var supplierType = _supplierTypeRepository.Find(x => x.Id == key);
                _supplierTypeRepository.Delete(supplierType);
                response.Result = null;
                response.Message = "Supplier Type was deleted successfully.";
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

        #endregion
        
        #region Catalogs

        // Get: Catalogs Type
        [HttpGet("GetCatalogsType", Name = "GetCatalogsType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatTypeCatalogDto>>> GetCatalogsType()
        {
            var response = new ApiResponse<List<CatTypeCatalogDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatTypeCatalogDto>>(_catalogTypeRepository.GetAll());
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
        
        // Get: Catalogs
        [HttpGet("GetCatalogs", Name = "GetCatalogs")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCatalogs()
        {
            try
            {
                return StatusCode(201, _catalogRepository.GetAllIncluding(x => x.TypeNavigation).Select(s => new
                {
                    s.Id,
                    s.Catalog,
                    s.Description,
                    s.TypeNavigation.Type
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Message = $"Internal server error {ex.Message}", Result = 0, Success = false });
            }
        }
        
        // Get: Catalog By Id
        [HttpGet("GetCatalogById", Name = "GetCatalogById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCatalogDto>> GetCatalogById(int key)
        {
            var response = new ApiResponse<CatCatalogDto>();
            try
            {
                response.Result =_mapper.Map<CatCatalogDto>(_catalogRepository.Find(x => x.Id == key));
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
        
        // Put: Catalogs
        [HttpPut("UpdateCatalogs", Name = "UpdateCatalogs")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCatalogDto>> UpdateCatalogs(CatCatalogDto key)
        {
            var response = new ApiResponse<CatCatalogDto>();
            try
            {
                response.Result =_mapper.Map<CatCatalogDto>(_catalogRepository.Update(_mapper.Map<CatCatalog>(key), key.Id));
                response.Message = "Catalog was updated successfully.";
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

        #endregion

        #region Slider Phrase

        // Get: Slider Phrase
        [HttpGet("GetSliderPhrase", Name = "GetSliderPhrase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<SlidePhraseDto>>> GetSliderPhrase()
        {
            var response = new ApiResponse<List<SlidePhraseDto>>();
            try
            {
                response.Result =_mapper.Map<List<SlidePhraseDto>>(_slidePhraseRepository.GetAll());
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
        
        // Get: Slider Phrase By Id
        [HttpGet("GetSliderPhraseById", Name = "GetSliderPhraseById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SlidePhraseDto>> GetSliderPhraseById(int key)
        {
            var response = new ApiResponse<SlidePhraseDto>();
            try
            {
                response.Result =_mapper.Map<SlidePhraseDto>(_slidePhraseRepository.Find(x => x.Id == key));
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
        
        // POST: Slider Phrase
        [HttpPost("AddSliderPhrase", Name = "AddSliderPhrase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SlidePhraseDto>> AddSliderPhrase(SlidePhraseDto key)
        {
            var response = new ApiResponse<SlidePhraseDto>();
            try
            {
                key.Image = _utiltyRepository.UploadImageBase64(key.Image, "/Files/Slider/", key.Extension);
                response.Result =_mapper.Map<SlidePhraseDto>(_slidePhraseRepository.Add(_mapper.Map<SlidePhrase>(key)));
                response.Message = "Slider Phrase was added successfully.";
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
        
        // Put: Slider Phrase
        [HttpPut("UpdateSliderPhrase", Name = "UpdateSliderPhrase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SlidePhraseDto>> UpdateSliderPhrase(SlidePhraseDto key)
        {
            var response = new ApiResponse<SlidePhraseDto>();
            try
            {
                if (_utiltyRepository.IsBase64(key.Image))
                {
                    key.Image = _utiltyRepository.UploadImageBase64(key.Image, "/Files/Slider/", key.Extension);
                }
                response.Result =_mapper.Map<SlidePhraseDto>(_slidePhraseRepository.Update(_mapper.Map<SlidePhrase>(key), key.Id));
                response.Message = "Slider Phrase was updated successfully.";
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
        
        // DELETE: Remove slider phrase 
        [HttpDelete("Remove/SliderPhrase/{key}", Name = "Remove/SliderPhrase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SlidePhraseDto>> RemoveSliderPhrase(int key)
        {
            var response = new ApiResponse<SlidePhraseDto>();
            try
            {
                SlidePhrase slidePhrase = _slidePhraseRepository.Find(x => x.Id == key);
                _slidePhraseRepository.Delete(slidePhrase);
                
                response.Message = "Slider Phrase was remove successfully.";
                response.Success = true;
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    var number = sqlException.Number;
                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permitted";
                        _logger.LogError("Operation Not Permitted");
                        return StatusCode(409, response);
                    }
                }
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

        #endregion

        #region Upcoming Events

         // Get: Upcoming Events
        [HttpGet("GetUpcomingEvents", Name = "GetUpcomingEvents")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<UpcomingEventDto>>> GetUpcomingEvents()
        {
            var response = new ApiResponse<List<UpcomingEventDto>>();
            try
            {
                response.Result =_mapper.Map<List<UpcomingEventDto>>(_upcomingEventRepository.GetAllIncluding(x => x.CountryNavigation, q => q.CityNavigation));
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
        
        // Get: Upcoming Events By Id
        [HttpGet("GetUpcomingEventsById", Name = "GetUpcomingEventsById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<UpcomingEventDto>> GetUpcomingEventsById(int key)
        {
            var response = new ApiResponse<UpcomingEventDto>();
            try
            {
                response.Result =_mapper.Map<UpcomingEventDto>(_upcomingEventRepository.Find(x => x.Id == key));
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
        
        // POST: Upcoming Event
        [HttpPost("AddUpcomingEvent", Name = "AddUpcomingEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<UpcomingEventDto>> AddUpcomingEvent(UpcomingEventDto key)
        {
            var response = new ApiResponse<UpcomingEventDto>();
            try
            {
                response.Result =_mapper.Map<UpcomingEventDto>(_upcomingEventRepository.Add(_mapper.Map<UpcomingEvent>(key)));
                response.Message = "Upcoming Event was added successfully.";
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
        
        // Put: Upcoming Event
        [HttpPut("UpdateUpcomingEvent", Name = "UpdateUpcomingEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<UpcomingEventDto>> UpdateUpcomingEvent(UpcomingEventDto key)
        {
            var response = new ApiResponse<UpcomingEventDto>();
            try
            {
                response.Result =_mapper.Map<UpcomingEventDto>(_upcomingEventRepository.Update(_mapper.Map<UpcomingEvent>(key), key.Id));
                response.Message = "Upcoming Event was updated successfully.";
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

        #endregion

        #region Industry

         // Get: All Industries
        [HttpGet("GetIndustries", Name = "GetIndustries")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatIndustryDto>>> GetIndustries()
        {
            var response = new ApiResponse<List<CatIndustryDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatIndustryDto>>(_industryRepository.GetAll());
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
        
        // Get: Industry By Id
        [HttpGet("GetIndustryById", Name = "GetIndustryById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatIndustryDto>> GetIndustryById(int key)
        {
            var response = new ApiResponse<CatIndustryDto>();
            try
            {
                response.Result =_mapper.Map<CatIndustryDto>(_industryRepository.Find(x => x.Id == key));
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
        
        // POST: Upcoming Event
        [HttpPost("AddIndustry", Name = "AddIndustry")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatIndustryDto>> AddIndustry(CatIndustryDto key)
        {
            var response = new ApiResponse<CatIndustryDto>();
            try
            {
                response.Result =_mapper.Map<CatIndustryDto>(_industryRepository.Add(_mapper.Map<CatIndustry>(key)));
                response.Message = "Industry was added successfully.";
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
        
        // Put: Industry
        [HttpPut("UpdateIndustry", Name = "UpdateIndustry")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatIndustryDto>> UpdateIndustry(CatIndustryDto key)
        {
            var response = new ApiResponse<CatIndustryDto>();
            try
            {
                response.Result =_mapper.Map<CatIndustryDto>(_industryRepository.Update(_mapper.Map<CatIndustry>(key), key.Id));
                response.Message = "Industry was updated successfully.";
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
        
        // DELETE: Industry
        [HttpDelete("DeleteIndustry", Name = "DeleteIndustry")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatIndustryDto>> DeleteIndustry(CatIndustryDto key)
        {
            var response = new ApiResponse<CatIndustryDto>();
            try
            {
                _industryRepository.Delete(_mapper.Map<CatIndustry>(key));
                response.Result = null;
                response.Message = "Industry was deleted successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion
        
        #region Event

         // Get: All Events
        [HttpGet("Events", Name = "Events")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatEventDto>>> GetEvents()
        {
            var response = new ApiResponse<List<CatEventDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatEventDto>>(_eventRepository.GetAll());
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
        
        // Get: Industry By Id
        [HttpGet("Event/{key}", Name = "GetEventById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatEventDto>> GetEventById(int key)
        {
            var response = new ApiResponse<CatEventDto>();
            try
            {
                response.Result =_mapper.Map<CatEventDto>(_eventRepository.Find(x => x.Id == key));
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
        
        // POST: Upcoming Event
        [HttpPost("Add/Event", Name = "AddEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatEventDto>> AddEvent(CatEventDto key)
        {
            var response = new ApiResponse<CatEventDto>();
            try
            {
                response.Result =_mapper.Map<CatEventDto>(_eventRepository.Add(_mapper.Map<CatEvent>(key)));
                response.Message = "Event was added successfully.";
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
        
        // Put: Industry
        [HttpPut("Edit/Event", Name = "EditEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatEventDto>> EditEvent(CatEventDto key)
        {
            var response = new ApiResponse<CatEventDto>();
            try
            {
                response.Result =_mapper.Map<CatEventDto>(_eventRepository.Update(_mapper.Map<CatEvent>(key), key.Id));
                response.Message = "Event was updated successfully.";
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
        
        // DELETE: Industry
        [HttpDelete("Delete/Event/{key}", Name = "DeleteEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatEventDto>> DeleteEvent(int key)
        {
            var response = new ApiResponse<CatEventDto>();
            try
            {
                var find = _eventRepository.Find(f => f.Id == key);
                _eventRepository.Delete(_mapper.Map<CatEvent>(find));
                response.Result = null;
                response.Message = "Event was deleted successfully.";
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion

        #region Responsible

         // Get: All Responsible
        [HttpGet("Responsible/All", Name = "Responsible/All")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ResponsiblePremierOfficeDto>>> GertResponsibleAll()
        {
            var response = new ApiResponse<List<ResponsiblePremierOfficeDto>>();
            try
            {
                response.Result =_mapper.Map<List<ResponsiblePremierOfficeDto>>(_responsiblePremierOfficeRespository.GetAll());
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
        
        // Get: Responsible By Id
        [HttpGet("Responsible/{key}", Name = "ResponsibleById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ResponsiblePremierOfficeDto>> GetResponsibleById(int key)
        {
            var response = new ApiResponse<ResponsiblePremierOfficeDto>();
            try
            {
                response.Result =_mapper.Map<ResponsiblePremierOfficeDto>(_responsiblePremierOfficeRespository.Find(x => x.Id == key));
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
        
        // POST: Upcoming Responsible
        [HttpPost("Add/Responsible", Name = "AddResponsible")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ResponsiblePremierOfficeDto>> AddResponsible(ResponsiblePremierOfficeDto key)
        {
            var response = new ApiResponse<ResponsiblePremierOfficeDto>();
            try
            {
                response.Result =_mapper.Map<ResponsiblePremierOfficeDto>(_responsiblePremierOfficeRespository.Add(_mapper.Map<ResponsiblePremierOffice>(key)));
                response.Message = "Responsible was added successfully.";
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
        
        // Put: Responsible
        [HttpPut("Edit/Responsible", Name = "EditResponsible")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ResponsiblePremierOfficeDto>> EditResponsible(ResponsiblePremierOfficeDto key)
        {
            var response = new ApiResponse<ResponsiblePremierOfficeDto>();
            try
            {
                response.Result =_mapper.Map<ResponsiblePremierOfficeDto>(_responsiblePremierOfficeRespository.Update(_mapper.Map<ResponsiblePremierOffice>(key), key.Id));
                response.Message = "Responsible was updated successfully.";
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
        
        // DELETE: Industry
        [HttpDelete("Delete/Responsible/{key}", Name = "DeleteResponsible")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ResponsiblePremierOfficeDto>> DeleteResponsible(int key)
        {
            var response = new ApiResponse<ResponsiblePremierOfficeDto>();
            try
            {
                var find = _responsiblePremierOfficeRespository.Find(f => f.Id == key);
                _responsiblePremierOfficeRespository.Delete(_mapper.Map<ResponsiblePremierOffice>(find));
                response.Result = null;
                response.Message = "Responsible was deleted successfully.";
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion
        
        #region Referrral Fee

         // Get: All Referral Fee
        [HttpGet("ReferralFee/All", Name = "ReferralFee/All")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<ReferralFeeDto>>> GertReferralFeeAll()
        {
            var response = new ApiResponse<List<ReferralFeeDto>>();
            try
            {
                response.Result =_mapper.Map<List<ReferralFeeDto>>(_referralFeeRepository.GetAll());
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
        
        // Get: Responsible By Id
        [HttpGet("ReferralFee/{key}", Name = "ReferralFeeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReferralFeeDto>> GetReferralFeeById(int key)
        {
            var response = new ApiResponse<ReferralFeeDto>();
            try
            {
                response.Result =_mapper.Map<ReferralFeeDto>(_referralFeeRepository.Find(x => x.Id == key));
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
        
        // POST: Upcoming Responsible
        [HttpPost("Add/ReferralFee", Name = "AddReferralFee")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReferralFeeDto>> AddReferralFee(ReferralFeeDto key)
        {
            var response = new ApiResponse<ReferralFeeDto>();
            try
            {
                response.Result =_mapper.Map<ReferralFeeDto>(_referralFeeRepository.Add(_mapper.Map<ReferralFee>(key)));
                response.Message = "Referral Fee was added successfully.";
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
        /// <summary>
        /// Put: Update Referral Fee
        /// </summary>
        /// <param name="key">Object to update the entity</param>
        /// <returns></returns>
        [HttpPut("Edit/ReferralFee", Name = "EditReferralFee")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReferralFeeDto>> EditReferralFee(ReferralFeeDto key)
        {
            var response = new ApiResponse<ReferralFeeDto>();
            try
            {
                response.Result =_mapper.Map<ReferralFeeDto>(_referralFeeRepository.Update(_mapper.Map<ReferralFee>(key), key.Id));
                response.Message = "Referral Fee was updated successfully.";
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
        /// <summary>
        ///  DELETE: Remove Referral Fee
        /// </summary>
        /// <param name="key">Identifier of the object to remove record</param>
        /// <returns></returns>
        [HttpDelete("Delete/ReferralFee/{key}", Name = "DeleteReferralFee")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReferralFeeDto>> DeleteReferralFee(int key)
        {
            var response = new ApiResponse<ReferralFeeDto>();
            try
            {
                var find = _referralFeeRepository.Find(f => f.Id == key);
                _referralFeeRepository.Delete(_mapper.Map<ReferralFee>(find));
                response.Result = null;
                response.Message = "Referral Fee was deleted successfully.";
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion

        #region Contract Type

        // Get: All Contracts Type
        [HttpGet("ContractType/All", Name = "ContractType/All")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatContractTypeDto>>> GertContractTypeAll()
        {
            var response = new ApiResponse<List<CatContractTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatContractTypeDto>>(_contractTypeRepository.GetAll());
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
        
        // Get: Contracts Type By Id
        [HttpGet("ContractType/{key}", Name = "ContractTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContractTypeDto>> GetContractTypeById(int key)
        {
            var response = new ApiResponse<CatContractTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatContractTypeDto>(_contractTypeRepository.Find(x => x.Id == key));
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
        
        // POST: Add Contracts Type
        [HttpPost("Add/ContractType", Name = "AddContractType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContractTypeDto>> AddContractType(CatContractTypeDto key)
        {
            var response = new ApiResponse<CatContractTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatContractTypeDto>(_contractTypeRepository.Add(_mapper.Map<CatContractType>(key)));
                response.Message = "Contract Type was added successfully.";
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
        /// <summary>
        /// Put: Update Contracts Type
        /// </summary>
        /// <param name="key">Object to update the entity</param>
        /// <returns></returns>
        [HttpPut("Edit/ContractType", Name = "EditContractType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContractTypeDto>> EditContractType(CatContractTypeDto key)
        {
            var response = new ApiResponse<CatContractTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatContractTypeDto>(_contractTypeRepository.Update(_mapper.Map<CatContractType>(key), key.Id));
                response.Message = "Contract Type was updated successfully.";
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
        /// <summary>
        ///  DELETE: Remove Referral Fee
        /// </summary>
        /// <param name="key">Identifier of the object to remove record</param>
        /// <returns></returns>
        [HttpDelete("Delete/ContractType/{key}", Name = "DeleteContractType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatContractTypeDto>> DeleteContractType(int key)
        {
            var response = new ApiResponse<CatContractTypeDto>();
            try
            {
                var find = _contractTypeRepository.Find(f => f.Id == key);
                _contractTypeRepository.Delete(_mapper.Map<CatContractType>(find));
                response.Result = null;
                response.Message = "Contract Type was deleted successfully.";
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion

        #region Property Type

        // Get: All Property Type
        [HttpGet("PropertyType/All", Name = "PropertyType/All")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatPropertyTypeHousingDto>>> GertPropertyTypeAll()
        {
            var response = new ApiResponse<List<CatPropertyTypeHousingDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatPropertyTypeHousingDto>>(_propertyTypeHousingRepository.GetAll());
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
        
        // Get: Property Type By Id
        [HttpGet("PropertyType/{key}", Name = "PropertyTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPropertyTypeHousingDto>> GetPropertyTypeById(int key)
        {
            var response = new ApiResponse<CatPropertyTypeHousingDto>();
            try
            {
                response.Result =_mapper.Map<CatPropertyTypeHousingDto>(_propertyTypeHousingRepository.Find(x => x.Id == key));
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
        
        // POST: Add Property Type
        [HttpPost("Add/PropertyType", Name = "AddPropertyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPropertyTypeHousingDto>> AddPropertyType(CatPropertyTypeHousingDto key)
        {
            var response = new ApiResponse<CatPropertyTypeHousingDto>();
            try
            {
                response.Result =_mapper.Map<CatPropertyTypeHousingDto>(_propertyTypeHousingRepository.Add(_mapper.Map<CatPropertyTypeHousing>(key)));
                response.Message = "Property Type was added successfully.";
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
        /// <summary>
        /// Put: Update Property Type
        /// </summary>
        /// <param name="key">Object to update the entity</param>
        /// <returns></returns>
        [HttpPut("Edit/PropertyType", Name = "EditPropertyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPropertyTypeHousingDto>> EditPropertyType(CatPropertyTypeHousingDto key)
        {
            var response = new ApiResponse<CatPropertyTypeHousingDto>();
            try
            {
                response.Result =_mapper.Map<CatPropertyTypeHousingDto>(_propertyTypeHousingRepository.Update(_mapper.Map<CatPropertyTypeHousing>(key), key.Id));
                response.Message = "Property Type was updated successfully.";
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
        /// <summary>
        ///  DELETE: Remove Property
        /// </summary>
        /// <param name="key">Identifier of the object to remove record</param>
        /// <returns></returns>
        [HttpDelete("Delete/PropertyType/{key}", Name = "DeletePropertyType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatPropertyTypeHousingDto>> DeletePropertyType(int key)
        {
            var response = new ApiResponse<CatPropertyTypeHousingDto>();
            try
            {
                var find = _propertyTypeHousingRepository.Find(f => f.Id == key);
                _propertyTypeHousingRepository.Delete(_mapper.Map<CatPropertyTypeHousing>(find));
                response.Result = null;
                response.Message = "Property Type was deleted successfully.";
                response.Success = true;
            }
            catch (DbException ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion
        
        #region Relationship Contact

        // Get: Relationship
        [HttpGet("GetRelationshipContact", Name = "GetRelationshipContact")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatRelationshipContactDto>>> GetRelationshipContact()
        {
            var response = new ApiResponse<List<CatRelationshipContactDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatRelationshipContactDto>>(_relationshipContactRepository.GetAll());
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
        
        // Get: Partner Status By Id
        [HttpGet("GetRelationshipContactById", Name = "GetRelationshipContactById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipContactDto>> GetRelationshipContactById(int key)
        {
            var response = new ApiResponse<CatRelationshipContactDto>();
            try
            {
                response.Result =_mapper.Map<CatRelationshipContactDto>(_relationshipContactRepository.Find(x => x.Id == key));
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
        
        // Post: Add Partner Status 
        [HttpPost("AddRelationshipContact", Name = "AddRelationshipContact")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipContactDto>> AddRelationshipContact(CatRelationshipContactDto key)
        {
            var response = new ApiResponse<CatRelationshipContactDto>();
            try
            {
                response.Result =_mapper.Map<CatRelationshipContactDto>(_relationshipContactRepository.Add(_mapper.Map<CatRelationshipContact>(key)));
                response.Message = "Relationship was added successfully.";
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
        
        // Put: Update Partner Status
        [HttpPut("UpdateRelationshipContact", Name = "UpdateRelationshipContact")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipContactDto>> UpdateRelationship(CatRelationshipContactDto key)
        {
            var response = new ApiResponse<CatRelationshipContactDto>();
            try
            {
                response.Result =_mapper.Map<CatRelationshipContactDto>(_relationshipContactRepository.Update(_mapper.Map<CatRelationshipContact>(key), key.Id));
                response.Message = "Relationship was updated successfully.";
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
        
        // Delete: Delete Relationship by Id
        [HttpDelete("RelationshipContact/{key}", Name = "RelationshipContact/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatRelationshipContactDto>> DeleteRelationshipContact(int key)
        {
            var response = new ApiResponse<CatRelationshipContactDto>();
            try
            {
                var relationship = _relationshipContactRepository.Find(x => x.Id == key);
                _relationshipContactRepository.Delete(relationship);
                response.Result = null;
                response.Message = "Relationship was deleted successfully.";
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

        #endregion
        
        #region PricingSchedule

        // Get: Relationship
        [HttpGet("PricingSchedule", Name = "PricingSchedule")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<PricingScheduleDto>>> GetPricingSchedule()
        {
            var response = new ApiResponse<List<PricingScheduleDto>>();
            try
            {
                response.Result =_mapper.Map<List<PricingScheduleDto>>(_pricingScheduleRepository.GetAll());
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
        
        // Get: Partner Status By Id
        [HttpGet("PricingSchedule/{key}", Name = "GetPricingScheduleById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PricingScheduleDto>> GetPricingScheduleById(int key)
        {
            var response = new ApiResponse<PricingScheduleDto>();
            try
            {
                response.Result =_mapper.Map<PricingScheduleDto>(_pricingScheduleRepository.Find(x => x.Id == key));
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
        
        // Post: Add Partner Status 
        [HttpPost("PricingSchedule", Name = "AddPricingSchedule")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PricingScheduleDto>> AddPricingSchedule(PricingScheduleDto key)
        {
            var response = new ApiResponse<PricingScheduleDto>();
            try
            {
                response.Result =_mapper.Map<PricingScheduleDto>(_pricingScheduleRepository.Add(_mapper.Map<PricingSchedule>(key)));
                response.Message = "Pricing Schedule was added successfully.";
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
        
        // Put: Update Partner Status
        [HttpPut("PricingSchedule", Name = "UpdatePricingSchedule")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PricingScheduleDto>> UpdatePricingSchedule(PricingScheduleDto key)
        {
            var response = new ApiResponse<PricingScheduleDto>();
            try
            {
                response.Result =_mapper.Map<PricingScheduleDto>(_pricingScheduleRepository.Update(_mapper.Map<PricingSchedule>(key), key.Id));
                response.Message = "Pricing Schedule was updated successfully.";
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
        
        // Delete: Delete Relationship by Id
        [HttpDelete("PricingSchedule/{key}", Name = "PricingSchedule/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PricingScheduleDto>> DeletePricingSchedule(int key)
        {
            var response = new ApiResponse<PricingScheduleDto>();
            try
            {
                var pricingSchedule = _pricingScheduleRepository.Find(x => x.Id == key);
                _pricingScheduleRepository.Delete(pricingSchedule);
                response.Result = null;
                response.Message = "Pricing Schedule was deleted successfully.";
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

        #endregion

        #region Bank Account Type

        // Get: Bank Account Type
        [HttpGet("Bank-Account-Type", Name = "BankAccountType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatBankAccountTypeDto>>> GetBankAccountType()
        {
            var response = new ApiResponse<List<CatBankAccountTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatBankAccountTypeDto>>(_bankAccountTypeRepository.GetAll());
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
        
        // Get: Bank Account Type By Id
        [HttpGet("Bank-Account-Type/{key}", Name = "GetBankAccountTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBankAccountTypeDto>> GetBankAccountTypeById(int key)
        {
            var response = new ApiResponse<CatBankAccountTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatBankAccountTypeDto>(_bankAccountTypeRepository.Find(x => x.Id == key));
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
        
        // Post: Add Bank Account Type 
        [HttpPost("Bank-Account-Type", Name = "AddBankAccountType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBankAccountTypeDto>> AddBankAccountType(CatBankAccountTypeDto key)
        {
            var response = new ApiResponse<CatBankAccountTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatBankAccountTypeDto>(_bankAccountTypeRepository.Add(_mapper.Map<CatBankAccountType>(key)));
                response.Message = "Bank Account Type was added successfully.";
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
        
        // Put: Update Bank Account Type
        [HttpPut("Bank-Account-Type", Name = "UpdateBank-Account-Type")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBankAccountTypeDto>> UpdateBankAccountType(CatBankAccountTypeDto key)
        {
            var response = new ApiResponse<CatBankAccountTypeDto>();
            try
            {
                response.Result =_mapper.Map<CatBankAccountTypeDto>(_bankAccountTypeRepository.Update(_mapper.Map<CatBankAccountType>(key), key.Id));
                response.Message = "Bank Account Type was updated successfully.";
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
        
        // Delete: Delete Bank Account Type by Id
        [HttpDelete("Bank-Account-Type/{key}", Name = "Bank-Account-Type/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatBankAccountTypeDto>> DeleteBankAccountType(int key)
        {
            var response = new ApiResponse<CatBankAccountTypeDto>();
            try
            {
                var pricingSchedule = _bankAccountTypeRepository.Find(x => x.Id == key);
                _bankAccountTypeRepository.Delete(pricingSchedule);
                response.Result = null;
                response.Message = "Bank Account Type was deleted successfully.";
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

        #endregion

        #region Time Zone

        // Get: Time Zone
        [HttpGet("Time-Zone", Name = "TimeZone")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<CatTimeZoneDto>>> GetTimeZones()
        {
            var response = new ApiResponse<List<CatTimeZoneDto>>();
            try
            {
                response.Result =_mapper.Map<List<CatTimeZoneDto>>(_timeZoneRepository.GetAll());
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
        
        // Get: Time-Zone By Id
        [HttpGet("Time-Zone/{key}", Name = "GetTimeZoneById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTimeZoneDto>> GetTimeZoneById(int key)
        {
            var response = new ApiResponse<CatTimeZoneDto>();
            try
            {
                response.Result =_mapper.Map<CatTimeZoneDto>(_timeZoneRepository.Find(x => x.Id == key));
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
        
        // Post: Add Time-Zone
        [HttpPost("Time-Zone", Name = "AddTimeZone")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<CatTimeZoneDto>>> AddTimeZone(CatTimeZoneDto key)
        {
            var response = new ApiResponse<CatTimeZoneDto>();
            try
            {
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

                foreach (TimeZoneInfo timeZone in timeZones)
                {
                    // store whatever you need to store to a SQL Server table
                    Console.WriteLine(timeZone);
                    Console.WriteLine(timeZone.DaylightName);
                    // Console.WriteLine(String.Format(new EnglishFormatter(), "{0}", timeZone.DisplayName));
                    await _timeZoneRepository.AddAsyn(new CatTimeZone()
                    {
                        Id = 0,
                        TimeZone = timeZone.DisplayName
                    });
                }
                // response.Result = _mapper.Map<CatTimeZoneDto>(_timeZoneRepository.Add(_mapper.Map<CatTimeZone>(key)));
                response.Message = "Time Zone was added successfully.";
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
        
        // Put: Update Time-Zone
        [HttpPut("Time-Zone", Name = "UpdateTimeZone")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTimeZoneDto>> UpdateTimeZone(CatTimeZoneDto key)
        {
            var response = new ApiResponse<CatTimeZoneDto>();
            try
            {
                response.Result =_mapper.Map<CatTimeZoneDto>(_timeZoneRepository.Update(_mapper.Map<CatTimeZone>(key), key.Id));
                response.Message = "Time Zone was updated successfully.";
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
        
        // Delete: Delete Time-Zone by Id
        [HttpDelete("Time-Zone/{key}", Name = "Time-Zone/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatTimeZoneDto>> DeleteTimeZone(int key)
        {
            var response = new ApiResponse<CatTimeZoneDto>();
            try
            {
                var pricingSchedule = _timeZoneRepository.Find(x => x.Id == key);
                _timeZoneRepository.Delete(pricingSchedule);
                response.Result = null;
                response.Message = "Time Zone was deleted successfully.";
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

        #endregion

        #region Office Contact Type

        // Get: Office Contact Type
        [HttpGet("Office-Contact-Type", Name = "Office-Contact-Type")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<OfficeContactTypeDto>>> GetOfficeContactType()
        {
            var response = new ApiResponse<List<OfficeContactTypeDto>>();
            try
            {
                response.Result =_mapper.Map<List<OfficeContactTypeDto>>(_officeContactTypeRepository.GetAll());
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Get: Office Contact Type By Id
        [HttpGet("Office-Contact-Type/{key}", Name = "GetOfficeContactTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeContactTypeDto>> GetOfficeContactTypeById(int key)
        {
            var response = new ApiResponse<OfficeContactTypeDto>();
            try
            {
                response.Result =_mapper.Map<OfficeContactTypeDto>(_officeContactTypeRepository.Find(x => x.Id == key));
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Post: Add Office Contact Type
        [HttpPost("Office-Contact-Type", Name = "AddOfficeContactType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<OfficeContactTypeDto>>> AddOfficeContactType(OfficeContactTypeDto key)
        {
            var response = new ApiResponse<OfficeContactTypeDto>();
            try
            {
                response.Result = _mapper.Map<OfficeContactTypeDto>(_officeContactTypeRepository.Add(_mapper.Map<OfficeContactType>(key)));
                response.Message = "Office Contact Type was added successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Put: Update Office Contact Type
        [HttpPut("Office-Contact-Type", Name = "UpdateOffice-Contact-Type")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeContactTypeDto>> UpdateOfficeContactType(OfficeContactTypeDto key)
        {
            var response = new ApiResponse<OfficeContactTypeDto>();
            try
            {
                response.Result =_mapper.Map<OfficeContactTypeDto>(_officeContactTypeRepository.Update(_mapper.Map<OfficeContactType>(key), key.Id));
                response.Message = "Office Contact Type was updated successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Delete: Delete Office Contact Type by Id
        [HttpDelete("Office-Contact-Type/{key}", Name = "Office-Contact-Type/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OfficeContactTypeDto>> DeleteOfficeContactType(int key)
        {
            var response = new ApiResponse<OfficeContactTypeDto>();
            try
            {
                var officeContact = _officeContactTypeRepository.Find(x => x.Id == key);
                _officeContactTypeRepository.Delete(officeContact);
                response.Result = null;
                response.Message = "Office Contact Type was deleted successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion

        #region Office Type

        // Get: Office Type
        [HttpGet("Office-Type", Name = "Office-Type")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<TypeOfficeDto>>> GetOfficeType()
        {
            var response = new ApiResponse<List<TypeOfficeDto>>();
            try
            {
                response.Result =_mapper.Map<List<TypeOfficeDto>>(_typeOfficeRepository.GetAll());
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Get: Office Type By Id
        [HttpGet("Office-Type/{key}", Name = "GetOfficeTypeById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TypeOfficeDto>> GetOfficeTypeById(int key)
        {
            var response = new ApiResponse<TypeOfficeDto>();
            try
            {
                response.Result =_mapper.Map<TypeOfficeDto>(_typeOfficeRepository.Find(x => x.Id == key));
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Post: Add Office Type
        [HttpPost("Office-Type", Name = "AddOfficeType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<TypeOfficeDto>>> AddOfficeType(TypeOfficeDto key)
        {
            var response = new ApiResponse<TypeOfficeDto>();
            try
            {
                response.Result = _mapper.Map<TypeOfficeDto>(_typeOfficeRepository.Add(_mapper.Map<TypeOffice>(key)));
                response.Message = "Office Type was added successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Put: Update Office Type
        [HttpPut("Office-Type", Name = "UpdateOffice-Type")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TypeOfficeDto>> UpdateOfficeType(TypeOfficeDto key)
        {
            var response = new ApiResponse<TypeOfficeDto>();
            try
            {
                response.Result =_mapper.Map<TypeOfficeDto>(_typeOfficeRepository.Update(_mapper.Map<TypeOffice>(key), key.Id));
                response.Message = "Office Type was updated successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Delete: Delete Office Type by Id
        [HttpDelete("Office-Type/{key}", Name = "Office-Type/{key}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TypeOfficeDto>> DeleteOfficeType(int key)
        {
            var response = new ApiResponse<TypeOfficeDto>();
            try
            {
                var officeContact = _typeOfficeRepository.Find(x => x.Id == key);
                _typeOfficeRepository.Delete(officeContact);
                response.Result = null;
                response.Message = "Office Type was deleted successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }

        #endregion
        
    }
}
