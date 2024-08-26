using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.Extensions;
using api.premier.Models;
using api.premier.Models.Catalogos;
using api.premier.Models.Catalogs;
using api.premier.Models.Catalogue;
using api.premier.Models.DependentInformations;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Applicant;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.City;
using biz.premier.Repository.Client;
using biz.premier.Repository.Country;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Status;
using biz.premier.Repository.TypeService;
using biz.premier.Repository.VisaCategory;
using biz.premier.Repository.Call;
using biz.premier.Repository.ClientPartner;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.premier.Models.Task;
using biz.premier.Repository.Task;
using api.premier.Models.MapIt;
using biz.premier.Repository.MapIt;
using biz.premier.Repository.ImmigrationProfile;
using biz.premier.Repository.NotificationSystemType;
using api.premier.Models.Coordinator;
using api.premier.Models.ClientPartnerProfile;
using api.premier.Models.Countries;
using api.premier.Models.HomeSale;
using biz.premier.Repository.SupplierPartnerProfile;
using biz.premier.Repository.ProfileUser;
using api.premier.Models.RequestPayment;
using api.premier.Models.Report;
using api.premier.Models.Invoice;
using api.premier.Models.Permit;
using api.premier.Models.PropertyManagement;
using api.premier.Models.TenancyManagement;
using api.premier.Models.Training;
using biz.premier.Repository.Countries;
using biz.premier.Repository.RequestInformation;
using biz.premier.Repository.TenancyManagement;
using biz.premier.Repository.Training;
using Microsoft.EntityFrameworkCore.Internal;
using biz.premier.Repository.StatusPropertyReport;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IAssignedServiceRepository _assignedServiceRepository;
        private readonly IBreedRepository _breedRepository;
        private readonly ICoordinatorRepository _coordinatorRepository;
        private readonly ICoordinatorTypeRepository _coordinatorTypeRepository;
        private readonly ILanguagesRepository _languagesRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly IPetTypeRepository _petTypeRepository;
        private readonly IRelationshipRepository _relationshipRepository;
        private readonly IServiceLineRepository _serviceLineRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository;
        private readonly ISupplierTypeRepository _supplierTypeRepository;
        private readonly IWeightMeasuresRepository _weightMeasuresRepository;
        private readonly IAuthorizedByRepository _authorizedByRepository;
        private readonly IMaritalstatusRepository _maritalstatusRepository;
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly ISexRepository _sexRepository;
        private readonly IDocumentTypeRepsoitory _documentTypeRepsoitory;
        private readonly IUserRepository _userRepository;
        private readonly IApplicantRepository _applicantRepository;
        private readonly ITypeServiceRepository _typeServiceRepository;
        private readonly IVisaCategoryRepository _visaCategoryRepository;
        private readonly IContractTypeRepository _contractTypeRepository;
        private readonly IMetricRepository _metricRepository;
        private readonly IAmenitieRepository _amenitieRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeliviredRepository _deliviredRepository;
        private readonly IHighestLevelEducationRepository _highestLevelEducationRepository;
        private readonly IProficiencyRepository _proficiencyRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IDurationRepository _durationRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IPropertyTypeHousingRepository _propertyTypeHousingRepository;
        private readonly IGradeSchoolingRepository _gradeSchoolingRepository;
        private readonly IPaymentTypeRepository _paymentTypeRepository;
        private readonly IRepairTypeRepository _repairTypeRepository;
        private readonly IReservationTypeRepository _reservationTypeRepository;
        private readonly IAssitanceWithRepository _assitanceWithRepository;
        private readonly ICallRepository _callRepository;
        private readonly ITransportTypeRepository _transportTypeRepository;
        private readonly IRequestTypeRepository _requestTypeRepository;
        private readonly IPaymetMethodRepository _paymetMethodRepository;
        private readonly IBankAccountTypeRepository _bankAccountTypeRepository;
        private readonly INotificationTypeRepository _notificationTypeRepository;
        private readonly ISchoolStatusRepository _schoolStatusRepository;
        private readonly IPropertySectionRepository _propertySectionRepository;
        //private readonly IStatusPropertyReportRepository _statusPropertyReportRepository;
        private readonly IStatusPropertySectionRepository _statusPropertySectionRepository;
        private readonly IStatusHousingRepository _statusHousingRepository;
        private readonly ICreditCardsRepository _creditCardsRepository;
        private readonly ILeaseTemplateRepository _leaseTemplateRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IMapItRepository _mapItRepository;
        private readonly IPrivacyRepository _privacyRepository;
        private readonly IAreaCoverageTypeRepository _areaCoverageTypeRepository;
        private readonly IVehicleTypeRepository _vehicleTypeRepository;
        private readonly ISupplierPartnerProfileStatusRepository _supplierPartnerProfileStatusRepository;
        private readonly IContactTypeRepository _contactTypeRepository;
        private readonly IImmigrationProfileRepository _immigrationProfileRepository;
        private readonly IActionTypeRepository _actionTypeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IDayRepository _dayRepository;
        private readonly ITypeHousingRepository _typeHousingRepository;
        private readonly ICatNotificationSystemTypeRepository _catNotificationSystemTypeRepository;
        private readonly IPaymentTypeStatusRepository _paymentTypeStatusRepository;
        private readonly IResponsablePaymentRepository _responsablePaymentRepository;
        private readonly IPurchaseStatusRepository _purchaseStatusRepository;
        private readonly IClient_PartnerRepository _client_PartnerRepository;
        private readonly ICompanyTypeRepository _companyTypeRepository;
        private readonly IResponsiblePremierOfficeRespository _responsiblePremierOfficeRespository;
        private readonly ILifeCircleRepository _lifeCircleRepository;
        private readonly ISuccessProbabilityRepository _successProbabilityRepository;
        private readonly IReferralFeeRepository _referralFeeRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly IPricingScheduleRepository _pricingScheduleRepository;
        private readonly IPaymentRecurrenceRepository _paymentRecurrenceRepository;
        private readonly ISupplierPartnerProfileConsultantRepository _supplierPartnerProfileConsultantRepository;
        private readonly ICatBenefitReposiotry _catBenefitReposiotry;
        private readonly IDocumentStatusRepository _documentStatusRepository;
        private readonly ILeaseGuaranteeRepository _leaseGuaranteeRepository;
        private readonly IPriceTermRepository _priceTerm;
        private readonly ITypeOfficeRepository _typeOfficeRepository;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IClientPartnerProfileExperienceTeamRepository _clientPartnerProfileExperienceTeamRepository;
        private readonly ITimeZoneRepository _timeZoneRepository;
        private readonly IRequestPaymentStatusRepository _requestPaymentStatusRepository;
        private readonly IReportTypeRepository _reportTypeRepository;
        private readonly IColumnsReportRepository _columnsReportRepository;
        private readonly IFilterReportRepository _filterReportRepository;
        private readonly IStatusClientPartnerProfileRepository _statusClientPartnerProfileRepository;
        private readonly IStatusInvoiceRepository _statusInvoiceRepository;
        private readonly IInvoiceTypeRepository _invoiceTypeRepository;
        private readonly ICatReportRepository _catReportRepository;
        private readonly ICreditTermRepository _creditTermRepository;
        private readonly ISupplierPartnerProfileTypeRepository _supplierPartnerProfileTypeRepository;
        private readonly ITaxePercentageRepository _taxePercentageRepository;
        private readonly ISupplierPartnerTypeRepository _supplierPartnerTypeRepository;
        private readonly ICatTypeRepository _catTypeRepository;
        private readonly ICatPaymentRecurrenceRepository _catPaymentRecurrenceRepository;
        private readonly ICatElementRepository _catElementRepository;
        private readonly ICatTrainingTypeRepository _catTrainingTypeRepository;
        private readonly ICatContentTypeRepository _catContentTypeRepository;
        private readonly ICatTrainingGroupRepository _catTrainingGroupRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly ICatStatusSaleRepository _saleRepository;
        private readonly ICatStatusHomePurchaseRepository _homePurchaseRepository;
        private readonly ICatBillTypeDtoRepository _billTypeDtoRepository;
        private readonly ICatSeverityDtoRepository _severityDtoRepository;
        private readonly ICatStatusReportIssueRepository _statusReportIssueRepository;
        private readonly ICatLibreryRepository _libreryRepository;
        private readonly ICatStatusWorkOrderRepository _statusWorkOrderRepository;
        private readonly ICatIndustryRepository _catIndustryRepository;
        private readonly ICatStatusReportAnEventRepository _statusReportAnEventRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IRequestInformationRepository _requestInformationRepository;
        private readonly INationalityRepository _nationalityRepository;
        private readonly ICountryPhoneCodeRepository _countryPhoneCodeRepository;
        private readonly ICountryGenericRepository _countryGenericRepository;
        private readonly ICityGenericRepository _cityGenericRepository;
        private readonly IStateGenericRepository _stateGenericRepository;
        private readonly ICatStatusAppointmentRepository _catStatusAppointmentRepository;
        private readonly ICatPaymentProcessRepository _catPaymentProcessRepository;
        private readonly ICatSchoolExpensesPaymentRepository _catSchoolExpensesPaymentRepository;
        private readonly ICatStatusTransportPickupRepository _catStatusTransportPickupRepository;

        public CatalogueController(IMapper mapper, ILoggerManager logger, ICityRepository cityRepository, IClientRepository clientRepository,
                                    ICountryRepository countryRepository, IStatusRepository statusRepository,
                                    IAssignedServiceRepository assignedServiceRepository, IBreedRepository breedRepository, ICoordinatorRepository coordinatorRepository,
                                    ICoordinatorTypeRepository coordinatorTypeRepository, ILanguagesRepository languagesRepository, IPartnerRepository partnerRepository,
                                    IPetTypeRepository petTypeRepository, IRelationshipRepository relationshipRepository, IServiceLineRepository serviceLineRepository,
                                    ISizeRepository sizeRepository, ISupplierRepository supplierRepository, ISupplierCompanyRepository supplierCompanyRepository,
                                    ISupplierTypeRepository supplierTypeRepository, IWeightMeasuresRepository weightMeasuresRepository, IAuthorizedByRepository authorizedByRepository,
                                    IMaritalstatusRepository maritalstatusRepository, IPolicyTypeRepository policyTypeRepository, ISexRepository sexRepository,
                                    IDocumentTypeRepsoitory documentTypeRepsoitory, IUserRepository userRepository, IApplicantRepository applicantRepository,
                                    ITypeServiceRepository typeServiceRepository, IVisaCategoryRepository visaCategoryRepository, IServiceRepository serviceRepository,
                                    ICategoryRepository categoryRepository, IDeliviredRepository deliviredRepository,
                                    IContractTypeRepository contractTypeRepository,
                                    IMetricRepository metricRepository, IAmenitieRepository amenitieRepository, IHighestLevelEducationRepository highestLevelEducationRepository,
                                    IProficiencyRepository proficiencyRepository,
                                    IServiceRecordRepository serviceRecordRepository,
                                    IDurationRepository durationRepository,
                                    ICurrencyRepository currencyRepository,
                                    IPropertyTypeHousingRepository propertyTypeHousingRepository,
                                    IGradeSchoolingRepository gradeSchoolingRepository,
                                    IPaymentTypeRepository paymentTypeRepository,
                                    IRepairTypeRepository repairTypeRepository,
                                    IReservationTypeRepository reservationTypeRepository,
                                    IAssitanceWithRepository assitanceWithRepository,
                                    ICallRepository callRepository,
                                    ITransportTypeRepository transportTypeRepository,
                                    IRequestTypeRepository requestTypeRepository,
                                    IPaymetMethodRepository paymetMethodRepository,
                                    IBankAccountTypeRepository bankAccountTypeRepository,
                                    INotificationTypeRepository notificationTypeRepository,
                                    ISchoolStatusRepository schoolStatusRepository,
                                    IPropertySectionRepository propertySectionRepository,
                                    IStatusPropertySectionRepository statusPropertySectionRepository,
                                    IStatusHousingRepository statusHousingRepository,
                                    ICreditCardsRepository creditCardsRepository,
                                    ILeaseTemplateRepository leaseTemplateRepository,
                                    ITaskRepository taskRepository,
                                    IMapItRepository mapItRepository,
                                    IPrivacyRepository privacyRepository,
                                    IAreaCoverageTypeRepository areaCoverageTypeRepository,
                                    IVehicleTypeRepository vehicleTypeRepository,
                                    ISupplierPartnerProfileStatusRepository supplierPartnerProfileStatusRepository,
                                    IContactTypeRepository contactTypeRepository,
                                    IImmigrationProfileRepository immigrationProfileRepository,
                                    IDepartmentRepository departmentRepository,
                                    IActionTypeRepository actionTypeRepository,
                                    IOfficeRepository officeRepository,
                                    IDayRepository dayRepository,
                                    ITypeHousingRepository typeHousingRepository,
                                    ICatNotificationSystemTypeRepository catNotificationSystemTypeRepository,
                                    IPaymentTypeStatusRepository paymentTypeStatusRepository,
                                    IResponsablePaymentRepository responsablePaymentRepository,
                                    IPurchaseStatusRepository purchaseStatusRepository,
                                    IClient_PartnerRepository client_PartnerRepository,
                                    ICompanyTypeRepository companyTypeRepository,
                                    IResponsiblePremierOfficeRespository responsiblePremierOfficeRespository,
                                    ILifeCircleRepository lifeCircleRepository,
                                    ISuccessProbabilityRepository successProbabilityRepository,
                                    IReferralFeeRepository referralFeeRepository,
                                    ITitleRepository titleRepository,
                                    ISupplierPartnerProfileConsultantRepository supplierPartnerProfileConsultantRepository,
                                    IPricingScheduleRepository pricingScheduleRepository,
                                    IPaymentRecurrenceRepository paymentRecurrenceRepository,
                                    ICatBenefitReposiotry catBenefitReposiotry,
                                    IDocumentStatusRepository documentStatusRepository,
                                    ILeaseGuaranteeRepository leaseGuaranteeRepository,
                                    IPriceTermRepository priceTerm,
                                    ITypeOfficeRepository typeOfficeRepository,
                                    IProfileUserRepository profileUserRepository,
                                    IClientPartnerProfileExperienceTeamRepository clientPartnerProfileExperienceTeamRepository,
                                    ITimeZoneRepository timeZoneRepository,
                                    IRequestPaymentStatusRepository requestPaymentStatusRepository,
                                    IReportTypeRepository reportTypeRepository,
                                    IColumnsReportRepository columnsReportRepository,
                                    IFilterReportRepository filterReportRepository,
                                    IStatusClientPartnerProfileRepository statusClientPartnerProfileRepository,
                                    IStatusInvoiceRepository statusInvoiceRepository,
                                    IInvoiceTypeRepository invoiceTypeRepository,
                                    ICatReportRepository catReportRepository,
                                    ICreditTermRepository creditTermRepository,
                                    ISupplierPartnerProfileTypeRepository supplierPartnerProfileTypeRepository,
                                    ITaxePercentageRepository taxePercentageRepository,
                                    ISupplierPartnerTypeRepository supplierPartnerTypeRepository,
                                    ICatTypeRepository catTypeRepository,
                                    ICatPaymentRecurrenceRepository catPaymentRecurrenceRepository,
                                    ICatElementRepository catElementRepository,
                                    ICatTrainingTypeRepository catTrainingTypeRepository,
                                    ICatContentTypeRepository catContentTypeRepository,
                                    ICatTrainingGroupRepository catTrainingGroupRepository,
                                    IMenuRepository menuRepository,
                                    ICatStatusSaleRepository saleRepository,
                                    ICatStatusHomePurchaseRepository homePurchaseRepository,
                                    ICatBillTypeDtoRepository billTypeDtoRepository,
                                    ICatSeverityDtoRepository severityDtoRepository,
                                    ICatStatusReportIssueRepository statusReportIssueRepository,
                                    ICatLibreryRepository libreryRepository,
                                    ICatStatusWorkOrderRepository statusWorkOrderRepository,
                                    ICatIndustryRepository catIndustryRepository,
                                    ICatStatusReportAnEventRepository statusReportAnEventRepository,
                                    IEventRepository eventRepository,
                                    IRequestInformationRepository requestInformationRepository,
                                    INationalityRepository nationalityRepository,
                                    ICountryPhoneCodeRepository countryPhoneCodeRepository,
                                    ICountryGenericRepository countryGenericRepository,
                                    ICityGenericRepository cityGenericRepository,
                                    IStateGenericRepository stateGenericRepository,
                                    ICatStatusAppointmentRepository catStatusAppointmentRepository,
                                    ICatPaymentProcessRepository catPaymentProcessRepository,
                                    ICatSchoolExpensesPaymentRepository catSchoolExpensesPaymentRepository,
                                    ICatStatusTransportPickupRepository catStatusTransportPickupRepository
                                    //,IStatusPropertyReportRepository statusPropertyReportRepository
        )
        {
            _mapper = mapper;
            _logger = logger;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _statusRepository = statusRepository;
            _clientRepository = clientRepository;
            _assignedServiceRepository = assignedServiceRepository;
            _breedRepository = breedRepository;
            _coordinatorRepository = coordinatorRepository;
            _coordinatorTypeRepository = coordinatorTypeRepository;
            _languagesRepository = languagesRepository;
            _partnerRepository = partnerRepository;
            _petTypeRepository = petTypeRepository;
            _relationshipRepository = relationshipRepository;
            _serviceLineRepository = serviceLineRepository;
            _sizeRepository = sizeRepository;
            _supplierRepository = supplierRepository;
            _supplierTypeRepository = supplierTypeRepository;
            _supplierCompanyRepository = supplierCompanyRepository;
            _weightMeasuresRepository = weightMeasuresRepository;
            _authorizedByRepository = authorizedByRepository;
            _maritalstatusRepository = maritalstatusRepository;
            _policyTypeRepository = policyTypeRepository;
            _sexRepository = sexRepository;
            _documentTypeRepsoitory = documentTypeRepsoitory;
            _userRepository = userRepository;
            _applicantRepository = applicantRepository;
            _typeServiceRepository = typeServiceRepository;
            _visaCategoryRepository = visaCategoryRepository;
            _serviceRepository = serviceRepository;
            _categoryRepository = categoryRepository;
            _deliviredRepository = deliviredRepository;
            _contractTypeRepository = contractTypeRepository;
            _metricRepository = metricRepository;
            _amenitieRepository = amenitieRepository;
            _highestLevelEducationRepository = highestLevelEducationRepository;
            _proficiencyRepository = proficiencyRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _durationRepository = durationRepository;
            _currencyRepository = currencyRepository;
            _propertyTypeHousingRepository = propertyTypeHousingRepository;
            _gradeSchoolingRepository = gradeSchoolingRepository;
            _repairTypeRepository = repairTypeRepository;
            _paymentTypeRepository = paymentTypeRepository;
            _reservationTypeRepository = reservationTypeRepository;
            _assitanceWithRepository = assitanceWithRepository;
            _callRepository = callRepository;
            _transportTypeRepository = transportTypeRepository;
            _requestTypeRepository = requestTypeRepository;
            _paymetMethodRepository = paymetMethodRepository;
            _bankAccountTypeRepository = bankAccountTypeRepository;
            _notificationTypeRepository = notificationTypeRepository;
            _schoolStatusRepository = schoolStatusRepository;
            _propertySectionRepository = propertySectionRepository;
            _statusPropertySectionRepository = statusPropertySectionRepository;
            _statusHousingRepository = statusHousingRepository;
            _creditCardsRepository = creditCardsRepository;
            _leaseTemplateRepository = leaseTemplateRepository;
            _taskRepository = taskRepository;
            _mapItRepository = mapItRepository;
            _privacyRepository = privacyRepository;
            _areaCoverageTypeRepository = areaCoverageTypeRepository;
            _vehicleTypeRepository = vehicleTypeRepository;
            _supplierPartnerProfileStatusRepository = supplierPartnerProfileStatusRepository;
            _contactTypeRepository = contactTypeRepository;
            _immigrationProfileRepository = immigrationProfileRepository;
            _departmentRepository = departmentRepository;
            _actionTypeRepository = actionTypeRepository;
            _officeRepository = officeRepository;
            _dayRepository = dayRepository;
            _typeHousingRepository = typeHousingRepository;
            _catNotificationSystemTypeRepository = catNotificationSystemTypeRepository;
            _paymentTypeStatusRepository = paymentTypeStatusRepository;
            _responsablePaymentRepository = responsablePaymentRepository;
            _purchaseStatusRepository = purchaseStatusRepository;
            _companyTypeRepository = companyTypeRepository;
            _responsiblePremierOfficeRespository = responsiblePremierOfficeRespository;
            _lifeCircleRepository = lifeCircleRepository;
            _successProbabilityRepository = successProbabilityRepository;
            _referralFeeRepository = referralFeeRepository;
            _titleRepository = titleRepository;
            _supplierPartnerProfileConsultantRepository = supplierPartnerProfileConsultantRepository;
            _pricingScheduleRepository = pricingScheduleRepository;
            _paymentRecurrenceRepository = paymentRecurrenceRepository;
            _catBenefitReposiotry = catBenefitReposiotry;
            _documentStatusRepository = documentStatusRepository;
            _leaseGuaranteeRepository = leaseGuaranteeRepository;
            _priceTerm = priceTerm;
            _typeOfficeRepository = typeOfficeRepository;
            _profileUserRepository = profileUserRepository;
            _client_PartnerRepository = client_PartnerRepository;
            _clientPartnerProfileExperienceTeamRepository = clientPartnerProfileExperienceTeamRepository;
            _timeZoneRepository = timeZoneRepository;
            _requestPaymentStatusRepository = requestPaymentStatusRepository;
            _reportTypeRepository = reportTypeRepository;
            _columnsReportRepository = columnsReportRepository;
            _filterReportRepository = filterReportRepository;
            _statusClientPartnerProfileRepository = statusClientPartnerProfileRepository;
            _statusInvoiceRepository = statusInvoiceRepository;
            _invoiceTypeRepository = invoiceTypeRepository;
            _catReportRepository = catReportRepository;
            _creditTermRepository = creditTermRepository;
            _supplierPartnerProfileTypeRepository = supplierPartnerProfileTypeRepository;
            _taxePercentageRepository = taxePercentageRepository;
            _supplierPartnerTypeRepository = supplierPartnerTypeRepository;
            _catTypeRepository = catTypeRepository;
            _catPaymentRecurrenceRepository = catPaymentRecurrenceRepository;
            _catElementRepository = catElementRepository;
            _catTrainingTypeRepository = catTrainingTypeRepository;
            _catContentTypeRepository = catContentTypeRepository;
            _catTrainingGroupRepository = catTrainingGroupRepository;
            _menuRepository = menuRepository;
            _saleRepository = saleRepository;
            _homePurchaseRepository = homePurchaseRepository;
            _billTypeDtoRepository = billTypeDtoRepository;
            _severityDtoRepository = severityDtoRepository;
            _statusReportIssueRepository = statusReportIssueRepository;
            _libreryRepository = libreryRepository;
            _statusWorkOrderRepository = statusWorkOrderRepository;
            _catIndustryRepository = catIndustryRepository;
            _statusReportAnEventRepository = statusReportAnEventRepository;
            _eventRepository = eventRepository;
            _requestInformationRepository = requestInformationRepository;
            _nationalityRepository = nationalityRepository;
            _countryPhoneCodeRepository = countryPhoneCodeRepository;
            _countryGenericRepository = countryGenericRepository;
            _cityGenericRepository = cityGenericRepository;
            _stateGenericRepository = stateGenericRepository;
            _catStatusAppointmentRepository = catStatusAppointmentRepository;
            _catPaymentProcessRepository = catPaymentProcessRepository;
            _catSchoolExpensesPaymentRepository = catSchoolExpensesPaymentRepository;
            _catStatusTransportPickupRepository = catStatusTransportPickupRepository;
            //_statusPropertyReportRepository = statusPropertyReportRepository;
        }

        [HttpGet]
        [Route("Generic/States/{country}")]
        public async Task<ActionResult<ApiResponse<List<StatesGenericDto>>>> GetGenericStates(int country)
        {
            var response = new ApiResponse<List<StatesGenericDto>>();

            try
            {
                var states = await _stateGenericRepository.GetAllAsyn();
                response.Result = _mapper.Map<List<StatesGenericDto>>(states.Where(x => x.IdCountry == country));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        [HttpGet]
        [Route("Generic/CitiesById/{id}")]
        public async Task<ActionResult<ApiResponse<List<CitiesGenericDto>>>> GetGenericCityById(int id)
        {
            var response = new ApiResponse<List<CitiesGenericDto>>();

            try
            {
                var cities = await _cityGenericRepository.FindByAsyn(x => x.Id == id);
                response.Result = _mapper.Map<List<CitiesGenericDto>>(cities);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }


        [HttpGet]
        [Route("Generic/Cities/{state}")]
        public async Task<ActionResult<ApiResponse<List<CitiesGenericDto>>>> GetGenericCity(int state)
        {
            var response = new ApiResponse<List<CitiesGenericDto>>();

            try
            {
                var cities = await _cityGenericRepository.GetAllAsyn();
                response.Result = _mapper.Map<List<CitiesGenericDto>>(cities.Where(x => x.IdState == state));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("Generic/Countries")]
        public async Task<ActionResult<ApiResponse<List<CountriesGenericDto>>>> GetGenericCountry()
        {
            var response = new ApiResponse<List<CountriesGenericDto>>();

            try
            {
                List<CountriesGenericDto> genericDtos = _mapper.Map<List<CountriesGenericDto>>(await _countryGenericRepository.GetAllAsyn());
                foreach (var dto in genericDtos)
                {
                    dto.Name = dto.Name.CapitalizeFirst();
                }
                response.Result = genericDtos;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("PhoneCode")]
        public ActionResult<ApiResponse<List<CountryPhoneCodeDto>>> GetPhoneCode()
        {
            var response = new ApiResponse<List<CountryPhoneCodeDto>>();

            try
            {
                response.Result = _mapper.Map<List<CountryPhoneCodeDto>>(_countryPhoneCodeRepository.GetAll().OrderBy(x => x.CountriesName));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("Nationalities")]
        public ActionResult<ApiResponse<List<NationalityDto>>> GetNationalities()
        {
            var response = new ApiResponse<List<NationalityDto>>();

            try
            {
                response.Result = _mapper.Map<List<NationalityDto>>(_nationalityRepository.GetAll().OrderBy(x => x.Nationality1).Where(x => x.Nationality1 != "?"));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("School_Expenses_Payment")]
        public ActionResult<ApiResponse<List<CatSchoolExpensesPaymentDto>>> GetCatSchoolExpensesPayment()
        {
            var response = new ApiResponse<List<CatSchoolExpensesPaymentDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatSchoolExpensesPaymentDto>>(_catSchoolExpensesPaymentRepository.GetAll().OrderBy(x => x.SchoolExpensesPayment));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("RequestHouses/{sr}")]
        public ActionResult GetHouses(int sr)
        {
            try
            {
                var houses = _requestInformationRepository.HousingAvailible(sr).OrderBy(x => x.Item2).ToList();
                return StatusCode(202, new { Success = true, Result = houses, Message = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Something went wrong: { ex.Message }" });
            }
        }

        [HttpGet]
        [Route("GetEmails/{sr}")]
        public ActionResult GetEmails(int sr)
        {
            try
            {
                return StatusCode(202, new { Success = true, Result = _serviceRecordRepository.GetEmails(sr), Message = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Something went wrong: { ex.Message }" });
            }
        }

        [HttpGet]
        [Route("GetEvent")]
        public ActionResult<ApiResponse<List<CatEventDto>>> GetEvent()
        {
            var response = new ApiResponse<List<CatEventDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatEventDto>>(_eventRepository.GetAll().OrderBy(x => x.Event));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusReportAnEvent")]
        public ActionResult<ApiResponse<List<CatStatusReportAnEventDto>>> GetStatusReportAnEvent()
        {
            var response = new ApiResponse<List<CatStatusReportAnEventDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusReportAnEventDto>>(_statusReportAnEventRepository.GetAll().OrderBy(x => x.Status));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetIndustry")]
        public ActionResult<ApiResponse<List<CatIndustryDto>>> GetIndustry()
        {
            var response = new ApiResponse<List<CatIndustryDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatIndustryDto>>(_catIndustryRepository.GetAll().OrderBy(x => x.Industry));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusWorkOrder")]
        public ActionResult<ApiResponse<List<CatStatusWorkOrderDto>>> GetLibrary([FromQuery] int category)
        {
            var response = new ApiResponse<List<CatStatusWorkOrderDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusWorkOrderDto>>(_statusWorkOrderRepository.GetAll()
                    .Where(x => x.Category == category || x.Category == 0).OrderBy(x => x.Status)
                );
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusServicebyRol")]
        public ActionResult<ApiResponse<List<CatStatusWorkOrderDto>>> GetLibrary([FromQuery] int category, int rol)
        {
            // service_vat_id  ( 21 HF) 
            // rol 3 supplier/consultor 

            var response = new ApiResponse<List<CatStatusWorkOrderDto>>();

            try
            {
                if(rol == 3)
                {
                    response.Result = _mapper.Map<List<CatStatusWorkOrderDto>>(_statusWorkOrderRepository.GetAll()
                    .Where(x => ((x.Category == category) || (x.Category == 0))
                             && (x.Id != 1 && x.Id != 5 && x.Id != 38 && x.Id != 3 && x.Id != 4 && x.Id != 33 && x.Id != 39)
                             && (x.Id != (category == 22 ? 37 : 10000)))
                    .OrderBy(x => x.Status));
                }
                else
                {
                    response.Result = _mapper.Map<List<CatStatusWorkOrderDto>>(_statusWorkOrderRepository.GetAll()
                    .Where(x => ((x.Category == category) || (x.Category == 0))
                             && (x.Id != 1 && x.Id != 5 && x.Id != 38)
                             && (x.Id != (category == 22 ? 37 : 10000)))
                    .OrderBy(x => x.Status));
                }
                

                
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusSerByuser")]
        public ActionResult<ApiResponse<List<CatStatusWorkOrderDto>>> GetStatusSerByuser([FromQuery] int category)
        {
            var response = new ApiResponse<List<CatStatusWorkOrderDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusWorkOrderDto>>(_statusWorkOrderRepository.GetAll()
                    .Where(x => x.Category == category || x.Category == 0).OrderBy(x => x.Status)
                );
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetLibrary")]
        public ActionResult<ApiResponse<List<CatLibraryDto>>> GetLibrary()
        {
            var response = new ApiResponse<List<CatLibraryDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatLibraryDto>>(_libreryRepository.GetAll().OrderBy(x => x.Library));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusReportIssue")]
        public ActionResult<ApiResponse<List<CatStatusReportIssueDto>>> GetStatusReportIssue()
        {
            var response = new ApiResponse<List<CatStatusReportIssueDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusReportIssueDto>>(_statusReportIssueRepository.GetAll().OrderBy(x => x.Status));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetSeverity")]
        public ActionResult<ApiResponse<List<CatSeverityDto>>> GetBillTypGetSeveritye()
        {
            var response = new ApiResponse<List<CatSeverityDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatSeverityDto>>(_severityDtoRepository.GetAll().OrderBy(x => x.Severity));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetBillType")]
        public ActionResult<ApiResponse<List<CatBillTypeDto>>> GetBillType()
        {
            var response = new ApiResponse<List<CatBillTypeDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatBillTypeDto>>(_billTypeDtoRepository.GetAll().OrderBy(x => x.Type));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusHomePurchase")]
        public ActionResult<ApiResponse<List<CatStatusHomePurchaseDto>>> GetStatusHomePurchase()
        {
            var response = new ApiResponse<List<CatStatusHomePurchaseDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusHomePurchaseDto>>(_homePurchaseRepository.GetAll().OrderBy(x => x.Status));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetStatusHomeSale")]
        public ActionResult<ApiResponse<List<CatStatusSaleDto>>> GetStatusHomeSale()
        {
            var response = new ApiResponse<List<CatStatusSaleDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusSaleDto>>(_saleRepository.GetAll().OrderBy(x => x.Status));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetCountry")]
        public ActionResult<ApiResponse<List<CatCountryDto>>> GetAll()
        {
            var response = new ApiResponse<List<CatCountryDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatCountryDto>>(_countryRepository.GetAll().OrderBy(x => x.Name));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetCountryByName")]
        public ActionResult<ApiResponse<List<CatCountryDto>>> GetCountryByName(string name)
        {
            var response = new ApiResponse<List<CatCountryDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatCountryDto>>(_countryRepository.FindBy(x => x.Name.ToLower() == name.ToLower()));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Retorna lista de países filtrada por los ya agregados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Country-Service-Location/{id}")]
        public ActionResult<ApiResponse<List<CatCountryDto>>> GetAllCountries(int id)
        {
            var response = new ApiResponse<List<CatCountryDto>>();

            try
            {
                List<CatCountryDto> catCountryDtos =
                    _mapper.Map<List<CatCountryDto>>(_countryRepository.GetAll().OrderBy(x => x.Name));
                var countries = _countryRepository.GetServiceLocationCountries(id);
                catCountryDtos = catCountryDtos.Where(x => !countries.Contains(x.Id)).ToList();
                response.Result = catCountryDtos;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }
        /// <summary>
        /// Retorna países que no tienen Scope
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Country-Service-Location/All/{id}")]
        public ActionResult<ApiResponse<ActionResult>> GetAllCountriesAll(int id)
        {
            var response = new ApiResponse<ActionResult>();

            try
            {
                var countries = _countryRepository.GetServicesLocationsCountries(id);
                response.Result = countries;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        // GET: GetState
        [HttpGet]
        [Route("GetState")]
        public ActionResult<ApiResponse<List<CatCityDto>>> GetState([FromQuery] int? country)
        {
            var response = new ApiResponse<List<CatCityDto>>();

            try
            {
                if (country.HasValue)
                    response.Result = _mapper.Map<List<CatCityDto>>(_cityRepository.FindAll(c => c.IdCountry == country).OrderBy(x => x.City));
                else
                    response.Result = _mapper.Map<List<CatCityDto>>(_cityRepository.FindAll(c => c.IdCountry != 0).OrderBy(x => x.City));
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

        // GET: GetCity
        [HttpGet]
        [Route("GetCity")]
        public ActionResult<ApiResponse<List<CatCityDto>>> GetCity()
        {
            var response = new ApiResponse<List<CatCityDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCityDto>>(_cityRepository.GetAll().OrderBy(x => x.City));
                //KCvar consult = _context.CatCities.Where(x => x.IdState == state).ToList();
                //return Ok(new { success = true, message = "Success Get City", result = consult });
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

        // GET: GetStatus
        [HttpGet]
        [Route("GetStatus")]
        public ActionResult<ApiResponse<List<CatStatusDto>>> GetStatus()
        {
            var response = new ApiResponse<List<CatStatusDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatStatusDto>>(_statusRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: GetClient
        [HttpGet]
        [Route("GetClient/{partner}")]
        public ActionResult GetClient(int partner)
        {
            try
            {
                //var client = partner  == 0 ? _client_PartnerRepository
                //    .GetAllIncluding(x => x.ClientPartnerProfileClientIdClientToNavigations, y=> y.ClientPartnerProfileClientIdClientFromNavigations )
                //    .Where(x => x.IdTypePartnerClientProfile == 3)
                //    .ToList() 
                //    : _client_PartnerRepository
                //    .GetAllIncluding(x => x.ClientPartnerProfileClientIdClientToNavigations, y => y.ClientPartnerProfileClientIdClientFromNavigations)
                //    .Where(x => x.ClientPartnerProfileClientIdClientToNavigations.Select(s => s.IdClientFrom).Contains(partner))
                //    .ToList();
                //if (partner == 0)
                //    client = client.Where(x => !x.ClientPartnerProfileClientIdClientToNavigations.Any()).ToList();
                //else
                //    client = client
                //        .Where(x => x.ClientPartnerProfileClientIdClientToNavigations.Select(s => s.IdClientFrom).Contains(partner))
                //        .ToList();

                return StatusCode(202, new { Success = true, Result = _client_PartnerRepository.GetClientList(partner), Message = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Something went wrong: { ex.Message }" });
            }
        }

        // GET: assigned service
        [HttpGet]
        [Route("GetAssignedService")]
        public ActionResult<ApiResponse<List<CatAssignedServiceDto>>> GetAssignedService()
        {
            var response = new ApiResponse<List<CatAssignedServiceDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatAssignedServiceDto>>(_assignedServiceRepository.GetAll().OrderBy(x => x.AssignedService));
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

        // GET: Cat Breed
        [HttpGet]
        [Route("GetBreed")]
        public ActionResult<ApiResponse<List<CatBreedDto>>> GetBreed([FromQuery] int id)
        {
            var response = new ApiResponse<List<CatBreedDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatBreedDto>>(_breedRepository.FindAll(x => x.PetTypeId == id).OrderBy(x => x.Breed));
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

        // GET: Cat Coordinator
        [HttpGet]
        [Route("GetCoordinator/{client}")]
        public ActionResult GetCoordinator(int? client, int? servileLine)
        {
            try
            {
                return StatusCode(202, new { Success = true, Result = _profileUserRepository.GetProfilesByTitle((int?)null, client.Value != 0 ? client.Value : 0, servileLine) });
            }
            catch (Exception ex)
            {
                //response.Result = null;
                //response.Success = false;
                //response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Something went wrong: { ex.Message }" });
            }
        }

        // GET: Cat CoordinatorType
        [HttpGet]
        [Route("GetCoordinatorType")]
        public ActionResult<ApiResponse<List<CatCoordinatorTypeDto>>> GetCoordinatortype()
        {
            var response = new ApiResponse<List<CatCoordinatorTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCoordinatorTypeDto>>(_coordinatorTypeRepository.GetAll().OrderBy(x => x.CoordinatorType));
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

        // GET: Cat Languages
        [HttpGet]
        [Route("GetLanguages")]
        public ActionResult<ApiResponse<List<CatLanguagesDto>>> GetLanguages()
        {
            var response = new ApiResponse<List<CatLanguagesDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatLanguagesDto>>(_languagesRepository.GetAll().OrderBy(x => x.Name));
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

        // GET: Cat Partner
        [HttpGet]
        [Route("GetPartner")]
        public ActionResult GetPartner()
        {
            try
            {
                var partner = _client_PartnerRepository.GetAll().Where(x => x.IdTypePartnerClientProfile == 1).Select(s => new
                {
                    s.Id,
                    coordinator = s.Name,
                }).OrderBy(x => x.coordinator).ToList();
                partner.Add(new { Id = 0, coordinator = "No Selected Partner" });
                return Ok(new { Success = true, Result = partner });
            }
            catch (Exception ex)
            {
                //response.Result = null;
                //response.Success = false;
                //response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Something went wrong: { ex.Message }" });
            }
        }

        // GET: Cat Pet Type
        [HttpGet]
        [Route("GetPetType")]
        public ActionResult<ApiResponse<List<CatPetTypeDto>>> GetPetType()
        {
            var response = new ApiResponse<List<CatPetTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPetTypeDto>>(_petTypeRepository.GetAll().OrderBy(x => x.PetType));
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

        // GET: Cat Relationship
        [HttpGet]
        [Route("GetRelationship")]
        public ActionResult<ApiResponse<List<CatRelationshipDto>>> GetRelationship()
        {
            var response = new ApiResponse<List<CatRelationshipDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatRelationshipDto>>(_relationshipRepository.FindBy(x => x.Id != 7).OrderBy(x => x.Relationship));
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

        // GET: Cat Service Line
        [HttpGet]
        [Route("GetServiceLine")]
        public ActionResult<ApiResponse<List<CatServiceLineDto>>> GetServiceLine()
        {
            var response = new ApiResponse<List<CatServiceLineDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatServiceLineDto>>(_serviceLineRepository.GetAll().OrderBy(x => x.ServiceLine));
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

        // GET: Cat Size
        [HttpGet]
        [Route("GetSize")]
        public ActionResult<ApiResponse<List<CatSizeDto>>> GetSize()
        {
            var response = new ApiResponse<List<CatSizeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSizeDto>>(_sizeRepository.GetAll().OrderBy(x => x.Size));
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

        // GET: Cat Supplier
        [HttpGet]
        [Route("GetSupplier")]
        public ActionResult<ApiResponse<List<CatSupplierDto>>> GetSupplier()
        {
            var response = new ApiResponse<List<CatSupplierDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierDto>>(_supplierRepository.GetAll().OrderBy(x => x.Supplier));
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

        // GET: Cat Supplier
        [HttpGet]
        [Route("GetSupplierByRecordId")]
        public ActionResult GetSupplierByRecordId(int ServiceLineId, int SR)
        {
            try
            {
                return Ok(new { Success = true, Result = _supplierRepository.GetSupplierBySR(ServiceLineId, SR) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = true, Result = ex.ToString() });
            }
        }

        // GET: Cat Supplier By Supplier Type
        [HttpGet]
        [Route("GetSupplierBySupplierType")]
        public ActionResult<ApiResponse<List<CatSupplierDto>>> GetSupplierBySupplierType([FromQuery] int key)
        {
            var response = new ApiResponse<List<CatSupplierDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierDto>>(_supplierRepository.GetAll().Where(x => x.SupplierType == key).OrderBy(x => x.Supplier));
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

        // GET: Cat Supplier Type
        [HttpGet]
        [Route("GetSupplierType")]
        public ActionResult<ApiResponse<List<CatSupplierTypeDto>>> GetSupplierType()
        {
            var response = new ApiResponse<List<CatSupplierTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierTypeDto>>(_supplierTypeRepository.GetAll().OrderBy(x => x.SupplierType));
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

        // GET: Cat Supplier Type
        [HttpGet]
        [Route("GetSupplierType/{type}")]
        public ActionResult<ApiResponse<List<CatSupplierTypeDto>>> GetSupplierType(int type)
        {
            var response = new ApiResponse<List<CatSupplierTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierTypeDto>>(_supplierTypeRepository.GetAll().Where(x => x.Type == type).OrderBy(x => x.SupplierType));
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

        // GET: Cat Supplier Type Cataloggue
        [HttpGet]
        [Route("GetSupplierTypeCatalogue")]
        public ActionResult<ApiResponse<List<CatSupplierTypeDto>>> GetSupplierTypeCatalogue([FromQuery] int[] id)
        {
            var response = new ApiResponse<List<CatSupplierTypeDto>>();
            try
            {
                var query = _mapper.Map<List<CatSupplierTypeDto>>(_supplierTypeRepository.GetAll().OrderBy(x => x.SupplierType));
                query = query.Where(x => id.Contains(x.Id)).ToList();
                response.Result = _mapper.Map<List<CatSupplierTypeDto>>(query);
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

        // GET: Cat Supplier Company
        [HttpGet]
        [Route("GetSupplierCompany")]
        public ActionResult<ApiResponse<List<CatSupplierCompanyDto>>> GetSupplierCompany([FromQuery] int id)
        {
            var response = new ApiResponse<List<CatSupplierCompanyDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierCompanyDto>>(_supplierCompanyRepository.FindAll(x => x.SupplierTypeId == id).OrderBy(x => x.SupplierCompany));
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

        // GET: Cat WeightMeasure
        [HttpGet]
        [Route("GetWeightMeasure")]
        public ActionResult<ApiResponse<List<CatWeightMeasureDto>>> GetWeightMeasure()
        {
            var response = new ApiResponse<List<CatWeightMeasureDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatWeightMeasureDto>>(_weightMeasuresRepository.GetAll().OrderBy(x => x.Name));
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

        // GET: Cat Authorized By
        [HttpGet]
        [Route("GetAuthorizedBy/{client}/{country}/{city}")]
        public ActionResult GetAuthorizedBy(int client, int country, int city)
        {
            try
            {
                var authorizedBy = _client_PartnerRepository.GetAuthorizedby(client, country, city);
                return StatusCode(202, new { Success = true, Result = authorizedBy });
            }
            catch (Exception ex)
            {
                //response.Result = null;
                //response.Success = false;
                //response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Something went wrong: { ex.Message }" });
            }
        }

        // GET: Cat MaritalStatus
        [HttpGet]
        [Route("GetMaritalStatus")]
        public ActionResult<ApiResponse<List<CatMaritalstatusDto>>> GetMaritalStatus()
        {
            var response = new ApiResponse<List<CatMaritalstatusDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatMaritalstatusDto>>(_maritalstatusRepository.GetAll().OrderBy(x => x.Name));
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


        // GET: Cat WeightMeasure
        [HttpGet]
        [Route("GetPolicyType")]
        public ActionResult<ApiResponse<List<CatPolicyTypeDto>>> GetPolicyType()
        {
            var response = new ApiResponse<List<CatPolicyTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPolicyTypeDto>>(_policyTypeRepository.GetAll().OrderBy(x => x.PolicyType));
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

        // GET: Cat Sex
        [HttpGet]
        [Route("GetSex")]
        public ActionResult<ApiResponse<List<CatSexDto>>> GetSex()
        {
            var response = new ApiResponse<List<CatSexDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSexDto>>(_sexRepository.GetAll().OrderBy(x => x.Sex));
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

        // GET: Cat DocumentType
        [HttpGet]
        [Route("GetDocumentType/{type}")]
        public ActionResult<ApiResponse<List<CatDocumentTypeDto>>> GetDocumentType(int type)
        {
            var response = new ApiResponse<List<CatDocumentTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatDocumentTypeDto>>(_documentTypeRepsoitory.GetAll().Where(x => x.Type == type).OrderBy(x => x.DocumentType));
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

        // GET: Cat payment_process
        [HttpGet]
        [Route("CatPaymentProcess")]
        public ActionResult<ApiResponse<List<CatPaymentProcessDto>>> CatPaymentProcess()
        {
            var response = new ApiResponse<List<CatPaymentProcessDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPaymentProcessDto>>(_catPaymentProcessRepository.GetAll().OrderBy(x => x.Payment));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        // GET: User List
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">ID de usuario loggeado</param>
        /// <param name="users">Lista de Usuarios a excluir de la lista</param>
        /// <param name="department">Filtrar por departamento, 0 si no se necesita filtrar, 1 Finanzas, 2 Gestión</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserTo")]
        public ActionResult GetUserFrom([FromQuery] int user, [FromQuery] int[] users, [FromQuery] int department)
        {
            try
            {
                var res = _userRepository.userList(user, users, department);
                return Ok(new { success = true, message = "", result = res });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                //return StatusCode(500, response);
                return BadRequest(new { success = false, message = $"Internal server error {ex.Message}", result = 0 });
            }
        }

        // GET: Cat Type Service
        [HttpGet]
        [Route("GetTypeService")]
        public ActionResult<ApiResponse<List<CatTypeServiceDto>>> GetTypeService()
        {
            var response = new ApiResponse<List<CatTypeServiceDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatTypeServiceDto>>(_typeServiceRepository.GetAll().OrderBy(x => x.Service));
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

        // GET: Cat Applicant
        [HttpGet]
        [Route("GetApplicant")]
        public ActionResult<ApiResponse<List<CatApplicantDto>>> GetApplicant()
        {
            var response = new ApiResponse<List<CatApplicantDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatApplicantDto>>(_applicantRepository.GetAll().OrderBy(x => x.Applicant));
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

        // GET: Cat Visa Category
        [HttpGet]
        [Route("GetVisaCategory")]
        public ActionResult<ApiResponse<List<CatVisaCategoryDto>>> GetVisaCategory()
        {
            var response = new ApiResponse<List<CatVisaCategoryDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatVisaCategoryDto>>(_visaCategoryRepository.GetAll().OrderBy(x => x.VisaCategory));
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

        // GET: Cat Visa Category
        [HttpGet]
        [Route("GetService")]
        public ActionResult GetService([FromQuery] int country, [FromQuery] int client, [FromQuery] int serviceLine)
        {
            try
            {
                var services = _serviceRepository
                    .GetServiceWithNickname(country, client, serviceLine);
                return Ok(new { Success = true, Message = "Operation was Success", result = services });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        // GET: NickName Dashboard
        [HttpGet]
        [Route("GetServiceDashboard")]
        public ActionResult GetServiceDashboard([FromQuery] int serviceId, [FromQuery] int client, [FromQuery] int serviceLine)
        {
            try
            {
                var services = _serviceRepository
                    .GetServiceWithNicknameByDasboard(serviceId, client, serviceLine);
                return Ok(new { Success = true, Message = "Operation was Success", result = services });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.ToString() });
            }
        }

        // GET: Cat Service by Service Line
        [HttpGet]
        [Route("GetServiceByServiceLine")]
        public ActionResult GetServiceByServiceLine(int idServiceLine)
        {
            try
            {
                return Ok(new { Success = true, result = _serviceRepository.GetServiceByServiceLine(idServiceLine) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }

        }

        // GET: Cat Visa Category
        [HttpGet]
        [Route("GetServiceByCategoryId")]
        public ActionResult<ApiResponse<List<CatServiceDto>>> GetServiceByCategoryId(int serviceCategoryId)
        {
            var response = new ApiResponse<List<CatServiceDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatServiceDto>>(_serviceRepository.FindBy(x => x.CategoryId == serviceCategoryId).OrderBy(x => x.Service));
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

        // GET: Cat Visa Category
        [HttpGet]
        [Route("GetCataegory")]
        public ActionResult<ApiResponse<List<CatCategoryDto>>> GetCataegory()
        {
            var response = new ApiResponse<List<CatCategoryDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCategoryDto>>(_categoryRepository.GetAll().OrderBy(x => x.Category));
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

        // GET: Cat Visa Category
        [HttpGet]
        [Route("GetCataegoryByServiceLineId")]
        public ActionResult<ApiResponse<List<CatCategoryDto>>> GetCataegoryByServiceLineId(int serviceLineId)
        {
            var response = new ApiResponse<List<CatCategoryDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCategoryDto>>(_categoryRepository.FindBy(x => x.SericeLineId == serviceLineId).OrderBy(x => x.Category));
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

        // GET: Cat Visa Category
        [HttpGet]
        [Route("GetDelivired")]
        public ActionResult<ApiResponse<List<CatDeliviredDto>>> GetDelivired()
        {
            var response = new ApiResponse<List<CatDeliviredDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatDeliviredDto>>(_deliviredRepository.GetAll().OrderBy(x => x.ServiceType));
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

        // GET: Cat Contract Type
        [HttpGet]
        [Route("GetContractType")]
        public ActionResult<ApiResponse<List<CatContractTypeDto>>> GetContractType()
        {
            var response = new ApiResponse<List<CatContractTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatContractTypeDto>>(_contractTypeRepository.GetAll().OrderBy(x => x.ContractType));
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

        // GET: Cat Metric 
        [HttpGet]
        [Route("GetMetric")]
        public ActionResult<ApiResponse<List<CatMetricDto>>> GetMetric()
        {
            var response = new ApiResponse<List<CatMetricDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatMetricDto>>(_metricRepository.GetAll().OrderBy(x => x.Metric));
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

        // GET: Cat Amenity
        [HttpGet]
        [Route("GetAmenity")]
        public ActionResult<ApiResponse<List<CatAmenitieDto>>> GetAmenity()
        {
            var response = new ApiResponse<List<CatAmenitieDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatAmenitieDto>>(_amenitieRepository.GetAll().OrderBy(x => x.Amenitie));
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

        // GET: Cat HighLevelEducation
        [HttpGet]
        [Route("GetHighestLevelEducation")]
        public ActionResult<ApiResponse<List<CatHighestLevelEducationDto>>> GetHighestLevelEducation()
        {
            var response = new ApiResponse<List<CatHighestLevelEducationDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatHighestLevelEducationDto>>(_highestLevelEducationRepository.GetAll().OrderBy(x => x.HighestLevelEducation));
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

        // GET: Cat Proficiency
        [HttpGet]
        [Route("GetProficiency")]
        public ActionResult<ApiResponse<List<CatProficiencyDto>>> GetProficiency()
        {
            var response = new ApiResponse<List<CatProficiencyDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatProficiencyDto>>(_proficiencyRepository.GetAll().OrderBy(x => x.Proficiency));
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

        // GET: Cat Proficiency
        [HttpGet]
        [Route("GetDependents")]
        public ActionResult<ApiResponse<List<DependentInformationDto>>> GetDependents([FromQuery] int sr, [FromQuery] int? relationship)
        {
            var response = new ApiResponse<List<DependentInformationDto>>();
            try
            {
                var dependets = _serviceRecordRepository.GetDependets(sr);
                if (relationship.HasValue)
                {
                    dependets = dependets.Where(x => x.RelationshipId == relationship.Value).ToList();
                }
                response.Result = _mapper.Map<List<DependentInformationDto>>(dependets);
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

        // GET: Cat Proficiency
        [HttpGet]
        [Route("GetLeaseSignators")]
        public ActionResult<ApiResponse<List<LeaseSignator>>> GetLeaseSignators([FromQuery] int id_signator )
        {
            var response = new ApiResponse<List<LeaseSignator>>();
            try
            {
                var dependets = _serviceRecordRepository.GetSignatorsbyId(id_signator);
                 
                response.Result = _mapper.Map<List<LeaseSignator>>(dependets);
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


        // GET: Cat Duration
        [HttpGet]
        [Route("GetSecurityDeposit")]
        public ActionResult<ApiResponse<List<SecurityDeposit>>> GetSecurityDeposit(int id)
        {
            var response = new ApiResponse<List<SecurityDeposit>>();
            try
            {
                var res_ = _serviceRecordRepository.GetSecurityDeposit(id);

                response.Result = _mapper.Map<List<SecurityDeposit>>(res_);
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

        // GET: Cat Duration
        [HttpGet]
        [Route("GetInitialRentPayment")]
        public ActionResult<ApiResponse<List<InitialRentPayment>>> GetInitialRentPayment(int id)
        {
            var response = new ApiResponse<List<InitialRentPayment>>();
            try
            {
                var res_ = _serviceRecordRepository.GetInitialRentPayment(id);

                response.Result = _mapper.Map<List<InitialRentPayment>>(res_);
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

        // GET: Cat Duration
        [HttpGet]
        [Route("GetOngoingRentPayment")]
        public ActionResult<ApiResponse<List<OngoingRentPayment>>> GetOngoingRentPayment(int id)
        {
            var response = new ApiResponse<List<OngoingRentPayment>>();
            try
            {
                var res_ = _serviceRecordRepository.GetOngoingRentPayment(id);

                response.Result = _mapper.Map<List<OngoingRentPayment>>(res_);
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

        // GET: Cat Duration
        [HttpGet]
        [Route("GetRealtorCommission")]
        public ActionResult<ApiResponse<List<RealtorCommission>>> GetRealtorCommission(int id)
        {
            var response = new ApiResponse<List<RealtorCommission>>();
            try
            {
                var res_ = _serviceRecordRepository.GetRealtorCommission(id);
                response.Success = true;
                response.Result = _mapper.Map<List<RealtorCommission>>(res_);
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

        // GET: Cat Duration
        [HttpGet]
        [Route("GetDuration")]
        public ActionResult<ApiResponse<List<CatDurationDto>>> GetDuration()
        {
            var response = new ApiResponse<List<CatDurationDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatDurationDto>>(_durationRepository.GetAll().OrderBy(x => x.Duration));
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

        // GET: Cat Duration
        [HttpGet]
        [Route("GetDurationForServiceRecord")]
        public ActionResult<ApiResponse<List<CatDurationDto>>> GetDurationForServiceRecord()
        {
            var response = new ApiResponse<List<CatDurationDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatDurationDto>>(_durationRepository
                    .GetAll()
                    .Where(x => x.Id == 4 || x.Id == 5 || x.Id == 6 || x.Id == 7)
                    .OrderBy(x => x.Duration)
                );
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

        // GET: Cat Currency
        [HttpGet]
        [Route("GetCurrency")]
        public ActionResult<ApiResponse<List<CatCurrencyDto>>> GetCurrency()
        {
            var response = new ApiResponse<List<CatCurrencyDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCurrencyDto>>(_currencyRepository.GetAll().OrderBy(x => x.Currency));
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

        // GET: Cat Property Type Housing
        [HttpGet]
        [Route("GetPropertyTypeHousing")]
        public ActionResult<ApiResponse<List<CatPropertyTypeHousingDto>>> GetPropertyTypeHousing()
        {
            var response = new ApiResponse<List<CatPropertyTypeHousingDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPropertyTypeHousingDto>>(_propertyTypeHousingRepository.GetAll().OrderBy(x => x.PropertyType));
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

        // GET: Cat Grade Schooling
        [HttpGet]
        [Route("GetGradeSchooling")]
        public ActionResult<ApiResponse<List<CatGradeSchoolingDto>>> GetGradeSchooling()
        {
            var response = new ApiResponse<List<CatGradeSchoolingDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatGradeSchoolingDto>>(_gradeSchoolingRepository.GetAll().OrderBy(x => x.Id));
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

        // GET: Cat Payment Type
        [HttpGet]
        [Route("GetPaymentType")]
        public ActionResult<ApiResponse<List<CatPaymentTypeDto>>> GetPaymentType()
        {
            var response = new ApiResponse<List<CatPaymentTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPaymentTypeDto>>(_paymentTypeRepository.GetAll().OrderBy(x => x.PaymentType));
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

        // GET: Cat Repair Type
        [HttpGet]
        [Route("GetRepairType")]
        public ActionResult<ApiResponse<List<CatRepairTypeDto>>> GetRepairType()
        {
            var response = new ApiResponse<List<CatRepairTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatRepairTypeDto>>(_repairTypeRepository.GetAll().OrderBy(x => x.RepairType));
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

        // GET: Cat Reservation Type
        [HttpGet]
        [Route("GetReservationType")]
        public ActionResult<ApiResponse<List<CatReservationTypeDto>>> GetReservationType()
        {
            var response = new ApiResponse<List<CatReservationTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatReservationTypeDto>>(_reservationTypeRepository.GetAll().OrderBy(x => x.ReservationType));
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

        // GET: Cat Assistance With
        [HttpGet]
        [Route("GetAssistanceWith")]
        public ActionResult<ApiResponse<List<CatAssitanceWithDto>>> GetAssistanceWith()
        {
            var response = new ApiResponse<List<CatAssitanceWithDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatAssitanceWithDto>>(_assitanceWithRepository.GetAll().OrderBy(x => x.Assistance));
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

        // GET: Cat Work Order By Sr
        [HttpGet("GetworkOrderBySR", Name = "GetworkOrderBySR")]
        public ActionResult GetWorkOrderBySR(int service_record_Id, int service_line_id)
        {
            //var response = new ApiResponse<List<ServiceOrderService>>();
            try
            {
                return Ok(new { Success = true, Result = _callRepository.GetworkOrderBySR(service_record_Id, service_line_id) });
                //response.Result = 
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
            //return Ok(response);
        }

        // GET: Cat Work Order By Sr
        [HttpGet("GetworkOrder/{sr}", Name = "GetworkOrder/{sr}")]
        public ActionResult GetworkOrder(int sr)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceRecordRepository.GetworkOrderBySR(sr) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // GET: Cat Services by SO
        [HttpGet("GetServiceBySOId", Name = "GetServiceBySOId")]
        public ActionResult GetServiceBySOId(int service_record_Id, int service_line_id)
        {
            //var response = new ApiResponse<List<ServiceOrderService>>();
            try
            {
                return Ok(new { Success = true, Result = _callRepository.GetServicesBySO(service_record_Id, service_line_id) });
                //response.Result = 
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
            //return Ok(response);
        }

        // GET: Cat Transport Type
        [HttpGet]
        [Route("GetTransportType")]
        public ActionResult<ApiResponse<List<CatTransportTypeDto>>> GetTransportType()
        {
            var response = new ApiResponse<List<CatTransportTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatTransportTypeDto>>(_transportTypeRepository.GetAll().OrderBy(x => x.TransportType));
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

        // GET: Cat Transport Type
        [HttpGet]
        [Route("GetRequestType")]
        public ActionResult<ApiResponse<List<CatRequestTypeDto>>> GetRequestType()
        {
            var response = new ApiResponse<List<CatRequestTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatRequestTypeDto>>(_requestTypeRepository.GetAll().OrderBy(x => x.RequestType));
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

        // GET: Cat Bank account type
        [HttpGet]
        [Route("GetBankAccountType")]
        public ActionResult<ApiResponse<List<CatBankAccountTypeDto>>> GetBankAccountType()
        {
            var response = new ApiResponse<List<CatBankAccountTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatBankAccountTypeDto>>(_bankAccountTypeRepository.GetAll().OrderBy(x => x.AccountType));
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

        // GET: Cat Payment Method
        [HttpGet]
        [Route("GetPaymentMethod")]
        public ActionResult<ApiResponse<List<CatPaymetMethodDto>>> GetPaymentMethod()
        {
            var response = new ApiResponse<List<CatPaymetMethodDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPaymetMethodDto>>(_paymetMethodRepository.GetAll().OrderBy(x => x.PaymentMethods));
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

        // GET: Cat Notification Type
        [HttpGet]
        [Route("GetNotificationType")]
        public ActionResult<ApiResponse<List<CatNotificationTypeDto>>> GetNotificationType()
        {
            var response = new ApiResponse<List<CatNotificationTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatNotificationTypeDto>>(_notificationTypeRepository.GetAll().OrderBy(x => x.Notification));
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

        // GET: Cat Notification Type
        [HttpGet]
        [Route("GetNotificationSystemType")]
        public ActionResult<ApiResponse<List<CatNotificationSystemTypeDto>>> GetNotificationSystemType()
        {
            var response = new ApiResponse<List<CatNotificationSystemTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatNotificationSystemTypeDto>>(_catNotificationSystemTypeRepository.GetAll().OrderBy(x => x.Type));
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
        // GET: Cat Notification Type
        [HttpGet]
        [Route("GetCatStatusTransportPickup")]
        public ActionResult<ApiResponse<List<CatStatusTransportPickupDto>>> GetCatStatusTransportPickup()
        {
            var response = new ApiResponse<List<CatStatusTransportPickupDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatStatusTransportPickupDto>>(_catStatusTransportPickupRepository.GetAll().OrderBy(x => x.Status));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            return Ok(response);
        }
        // GET: Cat Notification Type
        [HttpGet]
        [Route("GetSchoolStatus")]
        public ActionResult<ApiResponse<List<CatSchoolStatusDto>>> GetSchoolStatus()
        {
            var response = new ApiResponse<List<CatSchoolStatusDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSchoolStatusDto>>(_schoolStatusRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: Cat Property Section
        [HttpGet]
        [Route("GetPropertySection")]
        public ActionResult<ApiResponse<List<CatPropertySectionDto>>> GetPropertySection()
        {
            var response = new ApiResponse<List<CatPropertySectionDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPropertySectionDto>>(_propertySectionRepository.GetAll().OrderBy(x => x.PropertySection));
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

        // GET: Cat Status Property Section
        [HttpGet]
        [Route("GetStatusPropertySection")]
        public ActionResult<ApiResponse<List<CatStatusPropertySectionDto>>> GetStatusPropertySection()
        {
            var response = new ApiResponse<List<CatStatusPropertySectionDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatStatusPropertySectionDto>>(_statusPropertySectionRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: Cat Status Housing Repository
        [HttpGet]
        [Route("GetStatusHousing")]
        public ActionResult<ApiResponse<List<CatStatusHousingDto>>> GetStatusHousing()
        {
            var response = new ApiResponse<List<CatStatusHousingDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatStatusHousingDto>>(_statusHousingRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: Cat Credit Card
        [HttpGet]
        [Route("GetCreditCard")]
        public ActionResult<ApiResponse<List<CatCreditCardDto>>> GetCreditCard()
        {
            var response = new ApiResponse<List<CatCreditCardDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCreditCardDto>>(_creditCardsRepository.GetAll().OrderBy(x => x.Id));
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


        //// GET: Cat Credit Card
        //[HttpGet]
        //[Route("get_status_report")]
        //public ActionResult<ApiResponse<List<StatusPropertyReport>>> get_status_report()
        //{
        //    var response = new ApiResponse<List<StatusPropertyReport>>();
        //    try
        //    {
        //        response.Result = _mapper.Map<List<StatusPropertyReport>>(_statusPropertyReportRepository.GetAll().OrderBy(x => x.Id));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Result = null;
        //        response.Success = false;
        //        response.Message = $"Internal server error {ex.Message}";
        //        _logger.LogError($"Something went wrong: { ex.ToString() }");
        //        return StatusCode(500, response);
        //    }
        //    return Ok(response);
        //}

        // GET: Cat Lease Template
        [HttpGet]
        [Route("GetLeaseTemplate")]
        public ActionResult<ApiResponse<List<CatLeaseTemplateDto>>> GetLeaseTemplate()
        {
            var response = new ApiResponse<List<CatLeaseTemplateDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatLeaseTemplateDto>>(_leaseTemplateRepository.GetAll().OrderBy(x => x.Template));
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

        // GET: Cat Estatus Task
        [HttpGet]
        [Route("GetEstatusTask")]
        public ActionResult GetEstatusTask()
        {
            try
            {
                return Ok(new { Success = true, Result = _taskRepository.StatusTask() });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Location Type
        [HttpGet]
        [Route("GetLocationType")]
        public ActionResult GetLocationType()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapItRepository.LocationType() });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Work Order By Service Line
        [HttpGet]
        [Route("GetWorkOrderByServiceLine")]
        public ActionResult GetWorkOrderByServiceLine([FromQuery] int sr, [FromQuery] int sl)
        {
            try
            {
                var map = _serviceRecordRepository.GetWorkOrdersByServiceLine(sr, sl).Where(x => x.StatusId != 3).Select(s => new
                {
                    s.Id,
                    s.NumberWorkOrder
                }).OrderBy(x => x.NumberWorkOrder);
                return Ok(new { Success = true, Result = map });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Work Order By Service Line
        [HttpGet]
        [Route("GetServiceByServiceLineReports")]
        public ActionResult GetServiceByServiceLineReports([FromQuery] int sr, int sl, int idUser)
        {
            try
            {
                var map = _serviceRecordRepository.GetServiceByServiceLine(sr, sl, idUser);
                return Ok(new { Success = true, Result = map });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }


        // GET: Cat Work Order By Service Line
        [HttpGet]
        [Route("GetServiceByServiceLineReportsApp")]
        public ActionResult GetServiceByServiceLineReportsApp([FromQuery] int sr, int sl, int idUser)
        {
            try
            {
                var map = _serviceRecordRepository.GetServiceByServiceLineApp(sr, sl, idUser);
                return Ok(new { Success = true, Result = map });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }


        // GET: Cat Service By Work Order
        [HttpGet]
        [Route("GetServiceByWorkOrder")]
        public ActionResult GetServiceByWorkOrder([FromQuery] int wo, int idUser)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceRecordRepository.GetServiceByWorkOrder(wo, idUser) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Privacy
        [HttpGet]
        [Route("GetPrivacy")]
        public ActionResult GetPrivacy()
        {
            try
            {
                return Ok(new { Success = true, Result = _privacyRepository.GetAll().OrderBy(x => x.Privacy) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Area Coverage Type
        [HttpGet]
        [Route("GetAreaCoverageType")]
        public ActionResult GetAreaCoverageType()
        {
            try
            {
                return Ok(new { Success = true, Result = _areaCoverageTypeRepository.GetAll().OrderBy(x => x.Type) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Vehicle Type
        [HttpGet]
        [Route("GetVehicleType")]
        public ActionResult GetVehicleType()
        {
            try
            {
                return Ok(new { Success = true, Result = _vehicleTypeRepository.GetAll().OrderBy(x => x.Type) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Supplier Partner Profile Status
        [HttpGet]
        [Route("GetSupplierPartnerProfileStatus")]
        public ActionResult GetSupplierPartnerProfileStatus()
        {
            try
            {
                return Ok(new { Success = true, Result = _supplierPartnerProfileStatusRepository.GetAll().OrderBy(x => x.Status) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Contact Type
        [HttpGet]
        [Route("GetContactType")]
        public ActionResult GetContactType()
        {
            try
            {
                return Ok(new { Success = true, Result = _contactTypeRepository.GetAll().OrderBy(x => x.Type) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Office Contact Type
        [HttpGet]
        [Route("GetOfficeContactType")]
        public ActionResult GetOfficeContactType()
        {
            try
            {
                return Ok(new { Success = true, Result = _contactTypeRepository.GetOfficesContactTypes() });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Service By Work Order
        [HttpGet]
        [Route("GetDependentImmigration")]
        public ActionResult GetDependentImmigration([FromQuery] int sr)
        {
            try
            {
                return Ok(new { Success = true, Result = _immigrationProfileRepository.GetDependentImmigration(sr) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Action Type
        [HttpGet]
        [Route("GetActionType")]
        public ActionResult GetActionType()
        {
            try
            {
                return Ok(new { Success = true, Result = _actionTypeRepository.GetAll().OrderBy(x => x.ActionType) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Department
        [HttpGet]
        [Route("GetDepartment")]
        public ActionResult GetDepartment()
        {
            try
            {
                return Ok(new { Success = true, Result = _departmentRepository.GetAll().OrderBy(x => x.Department) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetServiceRecord/{user}/")]
        public ActionResult GetServiceRecord(int user, int? serviceLine)
        {
            try
            {
                var sr = serviceLine.HasValue ?
                    serviceLine == 1 ? _serviceRecordRepository
                    .GetAllIncluding(
                        i => i.AssigneeInformations,
                        a => a.ImmigrationSupplierPartners,
                        b => b.RelocationSupplierPartners).Where(x => x.ImmigrationCoodinators.Any()).Select(s => new
                        {
                            s.Id,
                            s.NumberServiceRecord,
                            assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName
                        }) : _serviceRecordRepository
                        .GetAllIncluding(
                            i => i.AssigneeInformations,
                            a => a.ImmigrationSupplierPartners,
                            b => b.RelocationSupplierPartners).Where(x => x.RelocationCoordinators.Any()).Select(s => new
                            {
                                s.Id,
                                s.NumberServiceRecord,
                                assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName
                            })
                    : _serviceRecordRepository.GetAllIncluding(i => i.AssigneeInformations).Select(s => new
                    {
                        s.Id,
                        s.NumberServiceRecord,
                        assigneeName = s.AssigneeInformations.FirstOrDefault().AssigneeName
                    }).OrderBy(x => x.NumberServiceRecord);
                return Ok(new { Success = true, Result = sr });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        ///App
        [HttpGet]
        [Route("GetServiceRecordApp/{user}/")]
        public ActionResult GetServiceRecordApp(int user)
        {
            try
            {
                var sr = _serviceRecordRepository.GetServiceRecordByUserApp(user);

                return Ok(new { Success = true, Result = sr });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Office
        [HttpGet]
        [Route("GetOffice")]
        public ActionResult GetOffice()
        {
            try
            {
                return Ok(new { Success = true, Result = _officeRepository.GetAll().OrderBy(x => x.Office) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Day
        [HttpGet]
        [Route("GetDay")]
        public ActionResult GetDay()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<CatDayDto>>(_dayRepository.GetAll().OrderBy(x => x.Day)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Day
        [HttpGet]
        [Route("GetTypeHousing")]
        public ActionResult GetTypeHousing()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<CatTypeHousingDto>>(_typeHousingRepository.GetAll().OrderBy(x => x.TypeHousing)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Notification System Type
        [HttpGet]
        [Route("GetNotidficationSystemType")]
        public ActionResult GetNotidficationSystemType()
        {
            try
            {
                return Ok(new
                {
                    Success = true,
                    Result = _mapper.Map<List<CatNotificationSystemTypeDto>>(_catNotificationSystemTypeRepository.GetAll().OrderBy(x => x.Type))
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }


        // GET: Cat Responsable Payment
        [HttpGet]
        [Route("GetResponsablePayment")]
        public ActionResult GetResponsablePayment()
        {
            try
            {
                return Ok(new
                {
                    Success = true,
                    Result = _mapper.Map<List<CatResponsablePaymentDto>>(_responsablePaymentRepository.GetAll().OrderBy(x => x.Responsable))
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }


        // GET: Cat Responsable Payment
        //[HttpGet]
        //[Route("GetPaymentRepairResponsability")]
        //public ActionResult GetPaymentRepairResponsability()
        //{
        //    try
        //    {
        //        return Ok(new
        //        {
        //            Success = true,
        //            Result = _responsablePaymentRepository.GetAllResponsablesPamentRep()
        //        }); 
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Success = false, Result = ex.ToString() });
        //    }
        //}

        // GET: Cat Payment Type Status
        [HttpGet]
        [Route("GetPaymentTypeStatus")]
        public ActionResult GetPaymentTypeStatus()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<CatPaymentTypeStatusDto>>(_paymentTypeStatusRepository.GetAll().OrderBy(x => x.Type)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Purchase Status
        [HttpGet]
        [Route("GetPurchaseStatus")]
        public ActionResult GetPurchaseStatus()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<CatPurchaseStatusDto>>(_purchaseStatusRepository.GetAll().OrderBy(x => x.Status)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Company Type
        [HttpGet]
        [Route("GetCompanyType")]
        public ActionResult GetCompanyType()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<CompanyTypeDto>>(_companyTypeRepository.GetAll().OrderBy(x => x.CompanyType1)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Responsible Premier Office
        [HttpGet]
        [Route("GetResponsiblePremierOffice")]
        public ActionResult GetResponsiblePremierOffice()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<ResponsiblePremierOffice>>(_responsiblePremierOfficeRespository.GetAll().OrderBy(x => x.ResponsiblePremierOffice1)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Life Circle
        [HttpGet]
        [Route("GetLifeCircle")]
        public ActionResult GetLifeCircle()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<LifeCircleDto>>(_lifeCircleRepository.GetAll()) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Success Probability
        [HttpGet]
        [Route("GetSuccessProbabilityRepository")]
        public ActionResult GetSuccessProbabilityRepository()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<SuccessProbabilityDto>>(_successProbabilityRepository.GetAll().OrderBy(x => x.SuccessProbability1)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Referrel Fee
        [HttpGet]
        [Route("GetReferrelFee")]
        public ActionResult GetReferrelFee()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<ReferralFeeDto>>(_referralFeeRepository.GetAll().OrderBy(x => x.ReferralFee1)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Precing Schedule 
        [HttpGet]
        [Route("GetPrecingSchedule")]
        public ActionResult GetPrecingSchedule()
        {
            try
            {
                return Ok(new { Success = true, Result = _mapper.Map<List<PricingScheduleDto>>(_pricingScheduleRepository.GetAll().OrderBy(x => x.PricingSchedule1)) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Companies
        [HttpGet]
        [Route("GetCompany")]
        public ActionResult GetCompany()
        {
            try
            {
                return Ok(new { Success = true, Result = _supplierPartnerProfileConsultantRepository.GetCompany() });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Referrel Fee
        [HttpGet]
        [Route("GetPaymentRecurrence")]
        public ActionResult GetPaymentRecurrence()
        {
            try
            {
                return Ok(new
                {
                    Success = true,
                    Result = _mapper.Map<List<PaymentRecurrenceDto>>(_paymentRecurrenceRepository.GetAll().OrderBy(x => x.PaymentRecurrence1))
                });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Title
        [HttpGet]
        [Route("GetTitle")]
        public ActionResult GetTitle()
        {
            try
            {
                return Ok(new { Success = true, Result = _titleRepository.GetAll().OrderBy(x => x.Title) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Benefit
        [HttpGet]
        [Route("GetBenefit")]
        public ActionResult GetBenefit()
        {
            try
            {
                return Ok(new { Success = true, Result = _catBenefitReposiotry.GetAll().OrderBy(x => x.Benefit) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Pricing_Type
        [HttpGet]
        [Route("GetPricingType")]
        public ActionResult GetPricingType()
        {
            return Ok(new { Success = true, Result = _pricingScheduleRepository.GetPricingType()});
        }

        // GET: Document Status
        [HttpGet]
        [Route("GetDocumentStatus")]
        public ActionResult GetDocumentStatus()
        {
            try
            {
                return Ok(new { Success = true, Result = _documentStatusRepository.GetAll().OrderBy(x => x.Status) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Lease Guarantee
        [HttpGet]
        [Route("GetLeaseGuarantee")]
        public ActionResult GetLeaseGuarantee()
        {
            try
            {
                return Ok(new { Success = true, Result = _leaseGuaranteeRepository.GetAll().OrderBy(x => x.Guarantee) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Price Term
        [HttpGet]
        [Route("GetPriceTerm")]
        public ActionResult GetPriceTerm()
        {
            try
            {
                return Ok(new { Success = true, Result = _priceTerm.GetAll().OrderBy(x => x.Price) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Type Office
        [HttpGet]
        [Route("GetTypeOffice")]
        public ActionResult GetTypeOffice()
        {
            try
            {
                return Ok(new { Success = true, Result = _typeOfficeRepository.GetAll().OrderBy(x => x.Type) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Sections Role
        [HttpGet]
        [Route("GetTimeZone")]
        public ActionResult GetTimeZone()
        {
            try
            {
                return Ok(new { Success = true, Result = _timeZoneRepository.GetAll().OrderBy(x => x.TimeZone) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Result = ex.ToString() });
            }
        }

        // GET: Cat Request Payment Status
        [HttpGet]
        [Route("GetRequestPaymentStatus")]
        public ActionResult<ApiResponse<List<CatRequestPaymentStatusDto>>> GetRequestPaymentStatus()
        {
            var response = new ApiResponse<List<CatRequestPaymentStatusDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatRequestPaymentStatusDto>>(_requestPaymentStatusRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: Cat Report Type 
        [HttpGet]
        [Route("GetReportType")]
        public ActionResult<ApiResponse<List<CatReportTypeDto>>> GetReportType()
        {
            var response = new ApiResponse<List<CatReportTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatReportTypeDto>>(_reportTypeRepository.GetAll().OrderBy(x => x.Name));
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

        // GET: Cat Columns Report
        [HttpGet]
        [Route("GetColumnsReport/{report}")]
        public ActionResult<ApiResponse<List<CatColumnsReportDto>>> GetColumnsReport(int report)
        {
            var response = new ApiResponse<List<CatColumnsReportDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatColumnsReportDto>>(_columnsReportRepository.GetAll().Where(x => x.Type == report).OrderBy(x => x.Name));
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

        // GET: Cat Filter Report
        [HttpGet]
        [Route("GetFilterReport/{report}")]
        public ActionResult<ApiResponse<List<CatFilterReportDto>>> GetFilterReport(int report)
        {
            var response = new ApiResponse<List<CatFilterReportDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatFilterReportDto>>(_filterReportRepository.GetAll().Where(x => x.Type == report).OrderBy(x => x.Name));
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

        // GET: Cat Filter Report
        [HttpGet]
        [Route("GetStatusClienPartner")]
        public ActionResult<ApiResponse<List<StatusClientPartnerProfileDto>>> GetStatusClienPartner()
        {
            var response = new ApiResponse<List<StatusClientPartnerProfileDto>>();
            try
            {
                response.Result = _mapper.Map<List<StatusClientPartnerProfileDto>>(_statusClientPartnerProfileRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: Cat Status Appointment
        [HttpGet]
        [Route("GetStatusAppointment")]
        public ActionResult<ApiResponse<List<CatStatusAppointmentDto>>> GetStatusAppointment()
        {
            var response = new ApiResponse<List<CatStatusAppointmentDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatStatusAppointmentDto>>(_catStatusAppointmentRepository.GetAll().OrderBy(x => x.Status));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        // GET: Cat Invoice Type
        [HttpGet]
        [Route("GetInvoiceType")]
        public ActionResult<ApiResponse<List<CatInvoiceTypeDto>>> GetInvoiceType()
        {
            var response = new ApiResponse<List<CatInvoiceTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatInvoiceTypeDto>>(_invoiceTypeRepository.GetAll().OrderBy(x => x.Type));
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

        // GET: Cat Status Invoice
        [HttpGet]
        [Route("GetStatusInvoice")]
        public ActionResult<ApiResponse<List<CatStatusInvoiceDto>>> GetStatusInvoice()
        {
            var response = new ApiResponse<List<CatStatusInvoiceDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatStatusInvoiceDto>>(_statusInvoiceRepository.GetAll().OrderBy(x => x.Status));
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

        // GET: Cat Report
        [HttpGet]
        [Route("GetCatReport")]
        public ActionResult<ApiResponse<List<CatReportDto>>> GetCatReport()
        {
            var response = new ApiResponse<List<CatReportDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatReportDto>>(_catReportRepository.GetAll().OrderBy(x => x.Report));
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

        // GET: Cat TaxePercentage
        [HttpGet]
        [Route("GetTaxePercentage")]
        public ActionResult<ApiResponse<List<CatTaxePercentageDto>>> GetTaxePercentage()
        {
            var response = new ApiResponse<List<CatTaxePercentageDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatTaxePercentageDto>>(_taxePercentageRepository.GetAll().OrderBy(x => x.Taxe));
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

        // GET: Cat CatSupplierPartnerProfileTypeDto
        [HttpGet]
        [Route("GetCreditTerm")]
        public ActionResult<ApiResponse<List<CatCreditTermDto>>> GetCreditTerm()
        {
            var response = new ApiResponse<List<CatCreditTermDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCreditTermDto>>(_creditTermRepository.GetAll().OrderBy(x => x.CreditTerm));
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

        // GET: Cat CatSupplierPartnerProfileTypeDto
        [HttpGet]
        [Route("GetSupplierPartnerProfileType")]
        public ActionResult<ApiResponse<List<CatSupplierPartnerProfileTypeDto>>> GetSupplierPartnerProfileType()
        {
            var response = new ApiResponse<List<CatSupplierPartnerProfileTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierPartnerProfileTypeDto>>(_supplierPartnerProfileTypeRepository.GetAll().OrderBy(x => x.Type));
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

        // GET: Cat CatSupplierPartnerTypeDto
        [HttpGet]
        [Route("GetSupplierPartnerType")]
        public ActionResult<ApiResponse<List<CatSupplierPartnerTypeDto>>> GetSupplierPartnerType()
        {
            var response = new ApiResponse<List<CatSupplierPartnerTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatSupplierPartnerTypeDto>>(_supplierPartnerTypeRepository.GetAll().OrderBy(x => x.Type));
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

        // GET: Cat Type 
        [HttpGet]
        [Route("GetCatType")]
        public ActionResult<ApiResponse<List<CatTypeDto>>> GetCatType()
        {
            var response = new ApiResponse<List<CatTypeDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatTypeDto>>(_catTypeRepository.GetAll().OrderBy(x => x.Type));
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

        // GET: Cat Recurrence Payment
        [HttpGet]
        [Route("GetCatPaymentRecurrence")]
        public ActionResult<ApiResponse<List<CatPaymentRecurrenceDto>>> GetCatRecurrencePayment()
        {
            var response = new ApiResponse<List<CatPaymentRecurrenceDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatPaymentRecurrenceDto>>(_catPaymentRecurrenceRepository.GetAll().OrderBy(x => x.Name));
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

        // GET: Element
        [HttpGet]
        [Route("GetElement")]
        public ActionResult<ApiResponse<List<CatElementDto>>> GetElement([FromQuery] int content)
        {
            var response = new ApiResponse<List<CatElementDto>>();

            try
            {
                if (content == 1)
                    response.Result = _mapper.Map<List<CatElementDto>>(_catElementRepository.FindAll(c => c.Type == 1 || c.Type == 2).OrderBy(x => x.Element));
                else if (content == 2)
                    response.Result = _mapper.Map<List<CatElementDto>>(_catElementRepository.FindAll(c => c.Type == 1 || c.Type == 3).OrderBy(x => x.Element));

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

        // GET: Training Type
        [HttpGet]
        [Route("GetTrainingType")]
        public ActionResult<ApiResponse<List<CatTrainingTypeDto>>> GetTrainingType()
        {
            var response = new ApiResponse<List<CatTrainingTypeDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatTrainingTypeDto>>(_catTrainingTypeRepository.GetAll().OrderBy(c => c.Type));
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

        // GET: Content Type
        [HttpGet]
        [Route("GetContentType")]
        public ActionResult<ApiResponse<List<CatContentTypeDto>>> GetContentType()
        {
            var response = new ApiResponse<List<CatContentTypeDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatContentTypeDto>>(_catContentTypeRepository.GetAll().OrderBy(x => x.Type));

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

        // GET: Training Group
        [HttpGet]
        [Route("GetTrainingGroup")]
        public ActionResult<ApiResponse<List<CatTrainingGroupDto>>> GetTrainingGroup()
        {
            var response = new ApiResponse<List<CatTrainingGroupDto>>();

            try
            {
                response.Result = _mapper.Map<List<CatTrainingGroupDto>>(_catTrainingGroupRepository.GetAll().OrderBy(x => x.Name));
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

        // GET: Cat Menu
        [HttpGet]
        [Route("GetMenu")]
        public ActionResult<ApiResponse<List<CatMenuDto>>> GetMenu()
        {
            var response = new ApiResponse<List<CatMenuDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatMenuDto>>(_menuRepository.GetMenus().OrderBy(x => x.Name));
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
    }
}
