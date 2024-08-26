using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.AirportTransportationService;
using api.premier.Models.AreaOrientation;
using api.premier.Models.CorporateAssistance;
using api.premier.Models.Departure;
using api.premier.Models.DocumentManagement;
using api.premier.Models.EntryVisa;
using api.premier.Models.HomeFinding;
using api.premier.Models.HomePurchase;
using api.premier.Models.HomeSale;
using api.premier.Models.HousingSpecification;
using api.premier.Models.LeaseRenewal;
using api.premier.Models.LegalReview;
using api.premier.Models.LocalDocumentation;
using api.premier.Models.Notification;
using api.premier.Models.NotificationSystem;
using api.premier.Models.Other;
using api.premier.Models.PredicisionOrientation;
using api.premier.Models.PropertyManagement;
using api.premier.Models.Renewal;
using api.premier.Models.RentalFurnitureCoordination;
using api.premier.Models.ResidencyPermit;
using api.premier.Models.SchoolingSearch;
using api.premier.Models.ServiceOrder;
using api.premier.Models.ServiceRecord;
using api.premier.Models.SettlingIn;
using api.premier.Models.TemporaryHousingCoordinaton;
using api.premier.Models.TenancyManagement;
using api.premier.Models.Transportation;
using api.premier.Models.VisaDeregistration;
using api.premier.Models.WorkPermit;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.AirportTransportationServices;
using biz.premier.Repository.AreaOrientation;
using biz.premier.Repository.City;
using biz.premier.Repository.CorporateAssistance;
using biz.premier.Repository.Departure;
using biz.premier.Repository.DocumentManagement;
using biz.premier.Repository.HomeFinding;
using biz.premier.Repository.HomePurchase;
using biz.premier.Repository.HomeSale;
using biz.premier.Repository.HousingSpecification;
using biz.premier.Repository.Immigration;
using biz.premier.Repository.LeaseRenewal;
using biz.premier.Repository.LegalReview;
using biz.premier.Repository.LocalDocumentation;
using biz.premier.Repository.Notification;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.Other;
using biz.premier.Repository.PredicisionOrientation;
using biz.premier.Repository.PropertyManagement;
using biz.premier.Repository.Renewal;
using biz.premier.Repository.RentalFurnitureCoordination;
using biz.premier.Repository.RequestAdditionalTime;
using biz.premier.Repository.ResidencyPermit;
using biz.premier.Repository.SchoolingSearch;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.SettlingIn;
using biz.premier.Repository.TemporaryHousingCoordinaton;
using biz.premier.Repository.TenancyManagement;
using biz.premier.Repository.Transportation;
using biz.premier.Repository.VisaDeregistration;
using biz.premier.Repository.WorkPermit;
using biz.premier.Servicies;
using dal.premier.Repository.Immigration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceOrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IWorkOrderRepository _serviceOrderRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IWorkOrderServicesRepository _serviceOrderServicesRepository;
        private readonly IHousingSpecification _housingSpecification;
        private readonly IImmigrationEntryVisaRepository _immigrationEntryVisaRepository;
        private readonly IWorkPermitRepository _workPermitRepository;
        private readonly IResidencyPermitRepository _residencyPermitRepository;
        private readonly IVisaDeregistrationRepository _visaDeregistrationRepository;
        private readonly IRenewalRepository _renewalRepository;
        private readonly ICorporateAssistanceRepository _corporateAssistanceRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILegalReviewRepository _legalReviewRepository;
        private readonly IDocumentManagementRepository _documentManagementRepository;
        private readonly ILocalDocumentationRepository _localDocumentationRepository;
        private readonly IPredicisionOrientationRepository _predicisionOrientationRepository;
        private readonly IAreaOrientationRepository _areaOrientationRepository;
        private readonly ISettlingInRepository _settlingInRepository;
        private readonly ISchoolingSearchRepository _schoolingSearchRepository;
        private readonly IDepartureRepository _departureRepository;
        private readonly ITemporaryHousingCoordinatonRepository _temporaryHousingCoordinatonRepository;
        private readonly IRentalFurnitureCoordinationRepository _rentalFurnitureCoordinationRepository;
        private readonly ITransportationRepository _transportationRepository;
        private readonly IAirportTransportationServicesRepository _airportTransportationServicesRepository;
        private readonly IHomeFindingRepository _homeFindingRepository;
        private readonly ILeaseRenewalRepository _leaseRenewalRepository;
        private readonly IRequestAdditionalTimeRepository _requestAdditionalTimeRepository;
        private readonly IHomeSaleRepository _homeSaleRepository;
        private readonly IHomePurchaseRepository _homePurchaseRepository;
        private readonly IPropertyManagementRepository _propertyManagementRepository;
        private readonly IOtherRepository _otherRepository;
        private readonly ITenancyManagementRepository _tenancyManagementRepository;
        private readonly ICityRepository _cityRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        
        public ServiceOrderController(IMapper mapper,
            ILoggerManager logger,
            IWorkOrderRepository serviceOrderRepository,
            IServiceRecordRepository serviceRecordRepository,
            IHousingSpecification housingSpecification,
            IImmigrationEntryVisaRepository immigrationEntryVisaRepository,
            IWorkOrderServicesRepository serviceOrderServicesRepository,
            IResidencyPermitRepository residencyPermitRepository,
            IWorkPermitRepository workPermitRepository,
            IVisaDeregistrationRepository visaDeregistrationRepository,
            IRenewalRepository renewalRepository,
            ICorporateAssistanceRepository corporateAssistanceRepository,
            INotificationRepository notificationRepository,
            ILegalReviewRepository legalReviewRepository,
            IDocumentManagementRepository documentManagementRepository,
            ILocalDocumentationRepository localDocumentationRepository,
            IPredicisionOrientationRepository predicisionOrientationRepository,
            IAreaOrientationRepository areaOrientationRepository,
            ISettlingInRepository settlingInRepository,
            ISchoolingSearchRepository schoolingSearchRepository,
            IDepartureRepository departureRepository,
            ITemporaryHousingCoordinatonRepository temporaryHousingCoordinatonRepository,
            IRentalFurnitureCoordinationRepository rentalFurnitureCoordinationRepository,
            ITransportationRepository transportationRepository,
            IAirportTransportationServicesRepository airportTransportationServicesRepository,
            IHomeFindingRepository homeFindingRepository,
            ILeaseRenewalRepository leaseRenewalRepository,
            IRequestAdditionalTimeRepository requestAdditionalTimeRepository,
            IHomeSaleRepository homeSaleRepository,
            IHomePurchaseRepository homePurchaseRepository,
            IPropertyManagementRepository propertyManagementRepository,
            IOtherRepository otherRepository,
            ITenancyManagementRepository tenancyManagementRepository,
            ICityRepository cityRepository,
            INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository
        )
        {
            _mapper = mapper;
            _logger = logger;
            _serviceOrderRepository = serviceOrderRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _housingSpecification = housingSpecification;
            _immigrationEntryVisaRepository = immigrationEntryVisaRepository;
            _serviceOrderServicesRepository = serviceOrderServicesRepository;
            _residencyPermitRepository = residencyPermitRepository;
            _workPermitRepository = workPermitRepository;
            _visaDeregistrationRepository = visaDeregistrationRepository;
            _renewalRepository = renewalRepository;
            _corporateAssistanceRepository = corporateAssistanceRepository;
            _notificationRepository = notificationRepository;
            _legalReviewRepository = legalReviewRepository;
            _localDocumentationRepository = localDocumentationRepository;
            _documentManagementRepository = documentManagementRepository;
            _predicisionOrientationRepository = predicisionOrientationRepository;
            _areaOrientationRepository = areaOrientationRepository;
            _settlingInRepository = settlingInRepository;
            _schoolingSearchRepository = schoolingSearchRepository;
            _departureRepository = departureRepository;
            _temporaryHousingCoordinatonRepository = temporaryHousingCoordinatonRepository;
            _rentalFurnitureCoordinationRepository = rentalFurnitureCoordinationRepository;
            _transportationRepository = transportationRepository;
            _airportTransportationServicesRepository = airportTransportationServicesRepository;
            _homeFindingRepository = homeFindingRepository;
            _leaseRenewalRepository = leaseRenewalRepository;
            _requestAdditionalTimeRepository = requestAdditionalTimeRepository;
            _homeSaleRepository = homeSaleRepository;
            _homePurchaseRepository = homePurchaseRepository;
            _propertyManagementRepository = propertyManagementRepository;
            _otherRepository = otherRepository;
            _tenancyManagementRepository = tenancyManagementRepository;
            _cityRepository = cityRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        [HttpPost("CreateOrder", Name = "CreateOrder")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<WorkOrderDto>> PostCreate([FromBody] WorkOrderInsertDto dto)
        {
            var response = new ApiResponse<WorkOrderDto>();
            try
            {
                _serviceOrderRepository.BeginTransaction();
                ServiceRecordSelectDto service = _mapper.Map<ServiceRecordSelectDto>(_serviceRecordRepository.SelectCustom(dto.ServiceRecordId.Value, 0));
                
                if (service != null)
                {
                    dto.StatusId = 2;
                    if(service.StatusId == 11)
                    {
                        if (service.ImmigrationSupplierPartners.Any() || service.RelocationSupplierPartners.Any())
                        {
                            service.StatusId = 18;
                            if(service.PartnerId == 0)
                            {
                                service.PartnerId = null;
                            }
                            _serviceRecordRepository.Update(_mapper.Map<ServiceRecord>(service), service.Id);
                        }
                        else {

                            service.StatusId = 1;
                            if (service.PartnerId == 0)
                            {
                                service.PartnerId = null;
                            }
                            _serviceRecordRepository.Update(_mapper.Map<ServiceRecord>(service), service.Id);
                        }
                    }

                    dto.NumberWorkOrder = "WO-" + _serviceOrderRepository.last_id();
                    int lastIdServiceOrder = _serviceOrderRepository.lastIdServiceOrderServices();
                    int lastIDBundle = _serviceOrderRepository.lastIdBundleService();
                    
                    foreach(var s in dto.StandaloneServiceWorkOrders)
                    {
                        s.Acceptance = null;
                        s.ServiceNumber = $"SN-{ lastIdServiceOrder }";
                        lastIdServiceOrder++;
                    }
                    
                    foreach (var sos in dto.BundledServicesWorkOrders)
                    {
                        foreach (var b in sos.BundledServices)
                        {
                            b.Acceptance = null;
                            b.ServiceNumber = $"SN-{ lastIDBundle }";
                            lastIDBundle++;
                        }
                    }

                    WorkOrder order = _serviceOrderRepository.Add(_mapper.Map<WorkOrder>(dto));

                    foreach (var i in order.StandaloneServiceWorkOrders)
                    {
                        int _DeliveringIn = _serviceOrderRepository.GetCountryToCountry(i.DeliveringIn.Value);
                        i.DeliveringIn = _DeliveringIn;

                        if (i.Acceptance == null)
                        {
                            i.Acceptance = DateTime.Now;
                        }

                        switch (i.CategoryId)
                        {
                            case (1):
                                EntryVisaDto entryVisa = new EntryVisaDto();
                                entryVisa.ApplicantId = i.DeliveredTo.Value;
                                entryVisa.AuthoDate = dto.CreatedDate.Value;
                                entryVisa.AuthoAcceptanceDate = i.Acceptance.Value;
                                entryVisa.StatusId = 1;
                                entryVisa.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                entryVisa.CreatedBy = i.CreatedBy;
                                entryVisa.CreatedDate = DateTime.Now;
                                entryVisa.CountryId = i.DeliveringIn;
                                entryVisa.CityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                _immigrationEntryVisaRepository.Add(_mapper.Map<EntryVisa>(entryVisa));
                                break;
                            case (2):
                                WorkPermitDto workPermit = new WorkPermitDto();
                                workPermit.ApplicantId = i.DeliveredTo.Value;
                                workPermit.AuthoDate = dto.CreatedDate.Value;
                                workPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                workPermit.StatusId = 1;
                                workPermit.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                workPermit.CreatedBy = i.CreatedBy;
                                workPermit.CreatedDate = DateTime.Now;
                                workPermit.HostCountryId = i.DeliveringIn;
                                workPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                _workPermitRepository.Add(_mapper.Map<WorkPermit>(workPermit));
                                break;
                            case (4):
                                ResidencyPermitDto residencyPermit = new ResidencyPermitDto();
                                residencyPermit.ApplicantId = i.DeliveredTo.Value;
                                residencyPermit.AuthoDate = dto.CreatedDate.Value;
                                residencyPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                residencyPermit.StatusId = 1;
                                residencyPermit.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                residencyPermit.CreatedBy = i.CreatedBy;
                                residencyPermit.CreatedDate = DateTime.Now;
                                residencyPermit.HostCityId = i.DeliveringIn;
                                residencyPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                _residencyPermitRepository.Add(_mapper.Map<ResidencyPermit>(residencyPermit));
                                break;
                            case (3):
                                VisaDeregistrationDto visaDeregistration = new VisaDeregistrationDto();
                                visaDeregistration.ApplicantId = i.DeliveredTo.Value;
                                visaDeregistration.AuthoDate = dto.CreatedDate.Value;
                                visaDeregistration.AuthoAcceptanceDate = i.Acceptance.Value;
                                visaDeregistration.StatusId = 1;
                                visaDeregistration.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                visaDeregistration.CreatedBy = i.CreatedBy;
                                visaDeregistration.CreatedDate = DateTime.Now;
                                visaDeregistration.HostCityId = i.DeliveringIn;
                                visaDeregistration.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                _visaDeregistrationRepository.Add(_mapper.Map<VisaDeregistration>(visaDeregistration));
                                break;
                            case (8):
                                RenewalDto renewal = new RenewalDto();
                                renewal.ApplicantId = i.DeliveredTo.Value;
                                renewal.AuthoDate = dto.CreatedDate.Value;
                                renewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                renewal.StatusId = 1;
                                renewal.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                renewal.CreatedBy = i.CreatedBy;
                                renewal.CreatedDate = DateTime.Now;
                                _renewalRepository.Add(_mapper.Map<Renewal>(renewal));
                                break;
                            case (7):
                                CorporateAssistanceDto corporateAssistance = new CorporateAssistanceDto();
                                corporateAssistance.ApplicantId = i.DeliveredTo.Value;
                                corporateAssistance.AuthoDate = dto.CreatedDate.Value;
                                corporateAssistance.AuthoAcceptanceDate = i.Acceptance.Value;
                                corporateAssistance.StatusId = 1;
                                corporateAssistance.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                corporateAssistance.CreatedBy = i.CreatedBy;
                                corporateAssistance.CreatedDate = DateTime.Now;
                                corporateAssistance.ServiceTypeId = i.ServiceTypeId;
                                _corporateAssistanceRepository.Add(_mapper.Map<CorporateAssistance>(corporateAssistance));
                                break;
                            case (9):
                                NotificationDto notification = new NotificationDto();
                                notification.ApplicantId = i.DeliveredTo.Value;
                                notification.AuthoDate = dto.CreatedDate.Value;
                                notification.AuthoAcceptanceDate = i.Acceptance.Value;
                                notification.StatusId = 1;
                                notification.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                notification.CreatedBy = i.CreatedBy;
                                notification.CreatedDate = DateTime.Now;
                                _notificationRepository.Add(_mapper.Map<Notification>(notification));
                                break;
                            case (10):
                                LegalReviewDto legalReview = new LegalReviewDto();
                                legalReview.ApplicantId = i.DeliveredTo.Value;
                                legalReview.AuthoDate = dto.CreatedDate.Value;
                                legalReview.AuthoAcceptanceDate = i.Acceptance.Value;
                                legalReview.StatusId = 1;
                                legalReview.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                legalReview.CreatedBy = i.CreatedBy;
                                legalReview.CreatedDate = DateTime.Now;
                                _legalReviewRepository.Add(_mapper.Map<LegalReview>(legalReview));
                                break;
                            case (5):
                                DocumentManagementDto documentManagement = new DocumentManagementDto();
                                documentManagement.ApplicantId = i.DeliveredTo.Value;
                                documentManagement.AuthoDate = dto.CreatedDate.Value;
                                documentManagement.AuthoAcceptanceDate = i.Acceptance.Value;
                                documentManagement.StatusId = 1;
                                documentManagement.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                documentManagement.CreatedBy = i.CreatedBy;
                                documentManagement.CreatedDate = DateTime.Now;
                                documentManagement.ServiceId = i.ServiceId;
                                documentManagement.ProjectFee = i.ProjectedFee;
                                _documentManagementRepository.Add(_mapper.Map<DocumentManagement>(documentManagement));
                                break;
                            case (6):
                                LocalDocumentationDto localDocumentation = new LocalDocumentationDto();
                                localDocumentation.ApplicantId = i.DeliveredTo.Value;
                                localDocumentation.AuthoDate = dto.CreatedDate.Value;
                                localDocumentation.AuthoAcceptanceDate = i.Acceptance.Value;
                                localDocumentation.StatusId = 1;
                                localDocumentation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                localDocumentation.CreatedBy = i.CreatedBy;
                                localDocumentation.CreatedDate = DateTime.Now;
                                localDocumentation.ServiceId = i.ServiceId;
                                localDocumentation.ProjectFee = i.ProjectedFee;
                                _localDocumentationRepository.Add(_mapper.Map<LocalDocumentation>(localDocumentation));
                                break;
                            case (12):
                                PredecisionOrientationDto predicisionOrientation = new PredecisionOrientationDto();
                                predicisionOrientation.AuthoDate = dto.CreatedDate.Value;
                                predicisionOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                predicisionOrientation.StatusId = 1;
                                predicisionOrientation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                predicisionOrientation.CreatedBy = i.CreatedBy;
                                predicisionOrientation.CreatedDate = DateTime.Now;
                                predicisionOrientation.Schoolings = new List<SchoolingDto>();
                                foreach (var d in service.AssigneeInformations.FirstOrDefault().DependentInformations)
                                {
                                    if (d.RelationshipId == 2)
                                    {
                                        d.NationalityId = _DeliveringIn;
                                        SchoolingDto schooling = new SchoolingDto();
                                        schooling.RelationshipId = d.Id;
                                        schooling.Avatar = d.Photo;
                                        schooling.Sex = d.Sex;
                                        schooling.Age = d.Age;
                                        schooling.Birth = d.Birth;
                                        schooling.Comments = d.AditionalComments;
                                        schooling.CurrentGrade = d.CurrentGrade;
                                        schooling.LanguagesSpoken = d.LanguagesId;
                                        schooling.Name = d.Name;
                                        schooling.Nationality = d.NationalityId;
                                        schooling.Active = true;
                                        predicisionOrientation.Schoolings.Add(schooling);
                                    }
                                }
                                _predicisionOrientationRepository.Add(_mapper.Map<PredecisionOrientation>(predicisionOrientation));
                                break;
                            case (13):
                                AreaOrientationDto areaOrientation = new AreaOrientationDto();
                                areaOrientation.AuthoDate = dto.CreatedDate.Value;
                                areaOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                areaOrientation.StatusId = 1;
                                areaOrientation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                areaOrientation.CreatedBy = i.CreatedBy;
                                areaOrientation.CreatedDate = DateTime.Now;
                                areaOrientation.SchoolingAreaOrientations = new List<SchoolingAreaOrientationDto>();
                                foreach (var d in service.AssigneeInformations.FirstOrDefault().DependentInformations)
                                {
                                    if (d.RelationshipId == 2)
                                    {
                                        d.NationalityId = _DeliveringIn;
                                        SchoolingAreaOrientationDto schoolingAreaOrientation = new SchoolingAreaOrientationDto();
                                        schoolingAreaOrientation.RelationshipId = d.Id;
                                        schoolingAreaOrientation.Age = d.Age;
                                        schoolingAreaOrientation.Sex = d.Sex;
                                        schoolingAreaOrientation.Avatar = d.Photo;
                                        schoolingAreaOrientation.Birth = d.Birth;
                                        schoolingAreaOrientation.Comments = d.AditionalComments;
                                        schoolingAreaOrientation.CurrentGrade = d.CurrentGrade;
                                        schoolingAreaOrientation.LanguagesSpokenSchoolingAreaOrientations = new List<LanguagesSpokenSchoolingAreaOrientationDto>();
                                        foreach (var o in d.LanguageDependentInformations)
                                        {
                                            schoolingAreaOrientation.LanguagesSpokenSchoolingAreaOrientations.Add(new LanguagesSpokenSchoolingAreaOrientationDto()
                                            {
                                                Schooling = 0,
                                                LanguagesSpoken = o.Language,
                                            });
                                        }
                                        schoolingAreaOrientation.Name = d.Name;
                                        schoolingAreaOrientation.Nationality = d.NationalityId;
                                        schoolingAreaOrientation.Sex = d.Sex;
                                        schoolingAreaOrientation.Active = true;
                                        areaOrientation.SchoolingAreaOrientations.Add(schoolingAreaOrientation);
                                    }
                                }
                                _areaOrientationRepository.Add(_mapper.Map<AreaOrientation>(areaOrientation));
                                break;
                            case (14):
                                SettlingInDto settlingIn = new SettlingInDto();
                                settlingIn.AuthoDate = dto.CreatedDate.Value;
                                settlingIn.AuthoAcceptanceDate = i.Acceptance.Value;
                                settlingIn.StatusId = 1;
                                settlingIn.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                settlingIn.CreatedBy = i.CreatedBy;
                                settlingIn.CreatedDate = DateTime.Now;
                                _settlingInRepository.Add(_mapper.Map<SettlingIn>(settlingIn));
                                break;
                            case (15):
                                SchoolingSearchDto schoolingSearch = new SchoolingSearchDto();
                                schoolingSearch.AuthoDate = dto.CreatedDate.Value;
                                schoolingSearch.AuthoAcceptanceDate = i.Acceptance.Value;
                                schoolingSearch.StatusId = 1;
                                schoolingSearch.Coordination = i.Coordination;
                                schoolingSearch.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                schoolingSearch.CreatedBy = i.CreatedBy;
                                schoolingSearch.CreatedDate = DateTime.Now;
                                schoolingSearch.SchoolingInformations = new List<SchoolingInformationDto>();
                                foreach (var s in service.AssigneeInformations.FirstOrDefault().DependentInformations)
                                {
                                    if (s.RelationshipId == 2)
                                    {
                                        s.NationalityId = s.NationalityId;
                                        SchoolingInformationDto schoolingInformation = new SchoolingInformationDto();
                                        schoolingInformation.RelationshipId = s.Id;
                                        schoolingInformation.Age = s.Age;
                                        schoolingInformation.Birth = s.Birth;
                                        schoolingInformation.Avatar = s.Photo;
                                        schoolingInformation.Sex = s.Sex;
                                        schoolingInformation.Comments = s.AditionalComments;
                                        schoolingInformation.CurrentGrade = s.CurrentGrade;
                                        LanguangeSpokenSchoolingInformationDto language = new LanguangeSpokenSchoolingInformationDto();
                                        List<LanguangeSpokenSchoolingInformationDto> languages = new List<LanguangeSpokenSchoolingInformationDto>();
                                        foreach (var o in s.LanguageDependentInformations)
                                        {
                                            language.LanguageSpoken = o.Language;
                                            languages.Add(new LanguangeSpokenSchoolingInformationDto()
                                            {
                                                LanguageSpoken = o.Language,
                                                SchoolingInformation = 0
                                            });
                                        }
                                        schoolingInformation.LanguangeSpokenSchoolingInformations = languages;
                                        schoolingInformation.Sex = s.Sex;
                                        schoolingInformation.Name = s.Name;
                                        schoolingInformation.Nationality = s.NationalityId;
                                        schoolingInformation.Active = true;
                                        schoolingSearch.SchoolingInformations.Add(schoolingInformation);
                                    }
                                }
                                _schoolingSearchRepository.Add(_mapper.Map<SchoolingSearch>(schoolingSearch));
                                break;
                            case (16):
                                DepartureDto departure = new DepartureDto();
                                departure.AuthoDate = dto.CreatedDate.Value;
                                departure.AuthoAcceptanceDate = i.Acceptance.Value;
                                departure.StatusId = 1;
                                departure.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                //departure.CreatedBy = i.CreatedBy;
                                //departure.CreatedDate = DateTime.Now;
                                _departureRepository.Add(_mapper.Map<Departure>(departure));
                                break;
                            case (17):
                                TemporaryHousingCoordinatonDto temporaryHousingCoordinaton = new TemporaryHousingCoordinatonDto();
                                temporaryHousingCoordinaton.AuthoDate = dto.CreatedDate.Value;
                                temporaryHousingCoordinaton.AuthoAcceptanceDate = i.Acceptance.Value;
                                temporaryHousingCoordinaton.StatusId = 1;
                                temporaryHousingCoordinaton.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                temporaryHousingCoordinaton.CreatedBy = i.CreatedBy;
                                temporaryHousingCoordinaton.CreatedDate = DateTime.Now;
                                _temporaryHousingCoordinatonRepository.Add(_mapper.Map<TemporaryHousingCoordinaton>(temporaryHousingCoordinaton));
                                break;
                            case (18):
                                RentalFurnitureCoordinationDto rentalFurnitureCoordination = new RentalFurnitureCoordinationDto();
                                rentalFurnitureCoordination.AuthoDate = dto.CreatedDate.Value;
                                rentalFurnitureCoordination.AuthoAcceptanceDate = i.Acceptance.Value;
                                rentalFurnitureCoordination.StatusId = 1;
                                rentalFurnitureCoordination.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                rentalFurnitureCoordination.CreatedBy = i.CreatedBy;
                                rentalFurnitureCoordination.CreatedDate = DateTime.Now;
                                _rentalFurnitureCoordinationRepository.Add(_mapper.Map<RentalFurnitureCoordination>(rentalFurnitureCoordination));
                                break;
                            case (19):
                                TransportationDto transportation = new TransportationDto();
                                transportation.AuthoDate = dto.CreatedDate.Value;
                                transportation.AuthoAcceptanceDate = i.Acceptance.Value;
                                transportation.ApplicantId = i.DeliveredTo.Value;
                                transportation.StatusId = 1;
                                transportation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                transportation.ProjectFee = i.ProjectedFee;
                                //transportation.CreatedBy = i.CreatedBy;
                                //transportation.CreatedDate = DateTime.Now;
                                _transportationRepository.Add(_mapper.Map<Transportation>(transportation));
                                break;
                            case (20):
                                AirportTransportationServiceDto airportTransportationService = new AirportTransportationServiceDto();
                                airportTransportationService.AuthoDate = dto.CreatedDate.Value;
                                airportTransportationService.AuthoAcceptanceDate = i.Acceptance.Value;
                                airportTransportationService.StatusId = 1;
                                airportTransportationService.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                airportTransportationService.CreatedBy = i.CreatedBy;
                                airportTransportationService.CreatedDate = DateTime.Now;
                                airportTransportationService.ApplicantId = i.DeliveredTo.Value;
                                airportTransportationService.ProjectFee = i.ProjectedFee;
                                _airportTransportationServicesRepository.Add(_mapper.Map<AirportTransportationService>(airportTransportationService));
                                break;
                            case (21):
                                HomeFindingDto homeFinding = new HomeFindingDto();
                                homeFinding.AuthoDate = dto.CreatedDate.Value;
                                homeFinding.AuthoAcceptanceDate = i.Acceptance.Value;
                                homeFinding.StatusId = 1;
                                homeFinding.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                homeFinding.CreatedBy = i.CreatedBy;
                                homeFinding.CreatedDate = DateTime.Now;
                                _homeFindingRepository.Add(_mapper.Map<HomeFinding>(homeFinding));
                                break;
                            case (22):
                                LeaseRenewalDto leaseRenewal = new LeaseRenewalDto();
                                leaseRenewal.AuthoDate = dto.CreatedDate.Value;
                                leaseRenewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                leaseRenewal.StatusId = 1;
                                leaseRenewal.WorkOrderServices = i.WorkOrderServiceId.Value;
                                leaseRenewal.CreationBy = i.CreatedBy;
                                leaseRenewal.CreatedDate = DateTime.Now;
                                _leaseRenewalRepository.Add(_mapper.Map<LeaseRenewal>(leaseRenewal));
                                break;
                            case (23):
                                HomeSaleDto homeSaleDto = new HomeSaleDto();
                                homeSaleDto.AuthoDate = dto.CreatedDate.Value;
                                homeSaleDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                homeSaleDto.StatusId = 1;
                                homeSaleDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                homeSaleDto.CreatedBy = i.CreatedBy;
                                homeSaleDto.CreatedDate = DateTime.Now;
                                _homeSaleRepository.Add(_mapper.Map<HomeSale>(homeSaleDto));
                                break;
                            case (24):
                                HomePurchaseDto homePurchaseDto = new HomePurchaseDto();
                                homePurchaseDto.AuthoDate = dto.CreatedDate.Value;
                                homePurchaseDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                homePurchaseDto.StatusId = 1;
                                homePurchaseDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                // homePurchaseDto.Cre = i.CreatedBy;
                                // homePurchaseDto.CreatedDate = DateTime.Now;
                                _homePurchaseRepository.Add(_mapper.Map<HomePurchase>(homePurchaseDto));
                                break;
                            case (25):
                                PropertyManagementDto propertyManagementDto = new PropertyManagementDto();
                                propertyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                propertyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                propertyManagementDto.StatusId = 1;
                                propertyManagementDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                // homePurchaseDto.Cre = i.CreatedBy;
                                // homePurchaseDto.CreatedDate = DateTime.Now;
                                _propertyManagementRepository.Add(_mapper.Map<PropertyManagement>(propertyManagementDto));
                                break;
                            case (26):
                                OtherDto otherDto = new OtherDto();
                                otherDto.AuthoDate = dto.CreatedDate.Value;
                                otherDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                otherDto.StatusId = 1;
                                otherDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                otherDto.CreatedBy = i.CreatedBy;
                                otherDto.CreatedDate = DateTime.Now;
                                otherDto.Coordination = i.Coordination.Value;
                                _otherRepository.Add(_mapper.Map<Other>(otherDto));
                                break;
                            case (27):
                                TenancyManagementDto tenancyManagementDto = new TenancyManagementDto();
                                tenancyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                tenancyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                tenancyManagementDto.StatusId = 1;
                                tenancyManagementDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                tenancyManagementDto.CreatedBy = i.CreatedBy;
                                tenancyManagementDto.CreatedDate = DateTime.Now;
                                tenancyManagementDto.Coordination = i.Coordination.Value;
                                _tenancyManagementRepository.Add(_mapper.Map<TenancyManagement>(tenancyManagementDto));
                                break;
                            case (28):
                                OtherDto otherImmigration = new OtherDto();
                                otherImmigration.AuthoDate = dto.CreatedDate.Value;
                                otherImmigration.AuthoAcceptanceDate = i.Acceptance.Value;
                                otherImmigration.StatusId = 1;
                                otherImmigration.WorkOrderServices = i.WorkOrderServiceId.Value;
                                otherImmigration.CreatedBy = i.CreatedBy;
                                otherImmigration.CreatedDate = DateTime.Now;
                                otherImmigration.Coordination = i.Coordination.Value;
                                _otherRepository.Add(_mapper.Map<Other>(otherImmigration));
                                break;
                        } 
                    }

                    foreach(var a in order.BundledServicesWorkOrders)
                    {
                        foreach(var i in a.BundledServices)
                        {
                            int _DeliveringIn = _serviceOrderRepository.GetCountryToCountry(i.DeliveringIn.Value);
                            i.DeliveringIn = _DeliveringIn;

                            if (i.Acceptance == null)
                            {
                                i.Acceptance = DateTime.Now;
                            }

                            switch (i.CategoryId)
                            {
                                case (1):
                                    EntryVisaDto entryVisa = new EntryVisaDto();
                                    entryVisa.ApplicantId = i.DeliveredTo.Value;
                                    entryVisa.AuthoDate = dto.CreatedDate.Value;
                                    entryVisa.AuthoAcceptanceDate = i.Acceptance.Value;
                                    entryVisa.StatusId = 1;
                                    entryVisa.WorkOrderServicesId = i.WorkServicesId.Value;
                                    entryVisa.CreatedBy = i.CreatedBy;
                                    entryVisa.CreatedDate = DateTime.Now;
                                    entryVisa.CountryId = i.DeliveringIn;
                                    entryVisa.CityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _immigrationEntryVisaRepository.Add(_mapper.Map<EntryVisa>(entryVisa));
                                    break;
                                case (2):
                                    WorkPermitDto workPermit = new WorkPermitDto();
                                    workPermit.ApplicantId = i.DeliveredTo.Value;
                                    workPermit.AuthoDate = dto.CreatedDate.Value;
                                    workPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                    workPermit.StatusId = 1;
                                    workPermit.WorkOrderServicesId = i.WorkServicesId.Value;
                                    workPermit.CreatedBy = i.CreatedBy;
                                    workPermit.CreatedDate = DateTime.Now;
                                    workPermit.HostCountryId = i.DeliveringIn;
                                    workPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _workPermitRepository.Add(_mapper.Map<WorkPermit>(workPermit));
                                    break;
                                case (4):
                                    ResidencyPermitDto residencyPermit = new ResidencyPermitDto();
                                    residencyPermit.ApplicantId = i.DeliveredTo.Value;
                                    residencyPermit.AuthoDate = dto.CreatedDate.Value;
                                    residencyPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                    residencyPermit.StatusId = 1;
                                    residencyPermit.WorkOrderServicesId = i.WorkServicesId.Value;
                                    residencyPermit.CreatedBy = i.CreatedBy;
                                    residencyPermit.CreatedDate = DateTime.Now;
                                    residencyPermit.HostCountryId = i.DeliveringIn;
                                    residencyPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _residencyPermitRepository.Add(_mapper.Map<ResidencyPermit>(residencyPermit));
                                    break;
                                case (3):
                                    VisaDeregistrationDto visaDeregistration = new VisaDeregistrationDto();
                                    visaDeregistration.ApplicantId = i.DeliveredTo.Value;
                                    visaDeregistration.AuthoDate = dto.CreatedDate.Value;
                                    visaDeregistration.AuthoAcceptanceDate = i.Acceptance.Value;
                                    visaDeregistration.StatusId = 1;
                                    visaDeregistration.WorkOrderServicesId = i.WorkServicesId.Value;
                                    visaDeregistration.CreatedBy = i.CreatedBy;
                                    visaDeregistration.CreatedDate = DateTime.Now;
                                    visaDeregistration.HostCountryId = i.DeliveringIn;
                                    visaDeregistration.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _visaDeregistrationRepository.Add(_mapper.Map<VisaDeregistration>(visaDeregistration));
                                    break;
                                case (8):
                                    RenewalDto renewal = new RenewalDto();
                                    renewal.ApplicantId = i.DeliveredTo.Value;
                                    renewal.AuthoDate = dto.CreatedDate.Value;
                                    renewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                    renewal.StatusId = 1;
                                    renewal.WorkOrderServicesId = i.WorkServicesId.Value;
                                    renewal.CreatedBy = i.CreatedBy;
                                    renewal.CreatedDate = DateTime.Now;
                                    _renewalRepository.Add(_mapper.Map<Renewal>(renewal));
                                    break;
                                case (7):
                                    CorporateAssistanceDto corporateAssistance = new CorporateAssistanceDto();
                                    corporateAssistance.ApplicantId = i.DeliveredTo.Value;
                                    corporateAssistance.AuthoDate = dto.CreatedDate.Value;
                                    corporateAssistance.AuthoAcceptanceDate = i.Acceptance.Value;
                                    corporateAssistance.StatusId = 1;
                                    corporateAssistance.WorkOrderServicesId = i.WorkServicesId.Value;
                                    corporateAssistance.CreatedBy = i.CreatedBy;
                                    corporateAssistance.CreatedDate = DateTime.Now;
                                    corporateAssistance.ServiceTypeId = i.ServiceTypeId;
                                    _corporateAssistanceRepository.Add(_mapper.Map<CorporateAssistance>(corporateAssistance));
                                    break;
                                case (9):
                                    NotificationDto notification = new NotificationDto();
                                    notification.ApplicantId = i.DeliveredTo.Value;
                                    notification.AuthoDate = dto.CreatedDate.Value;
                                    notification.AuthoAcceptanceDate = i.Acceptance.Value;
                                    notification.StatusId = 1;
                                    notification.WorkOrderServicesId = i.WorkServicesId.Value;
                                    notification.CreatedBy = i.CreatedBy;
                                    notification.CreatedDate = DateTime.Now;
                                    _notificationRepository.Add(_mapper.Map<Notification>(notification));
                                    break;
                                case (10):
                                    LegalReviewDto legalReview = new LegalReviewDto();
                                    legalReview.ApplicantId = i.DeliveredTo.Value;
                                    legalReview.AuthoDate = dto.CreatedDate.Value;
                                    legalReview.AuthoAcceptanceDate = i.Acceptance.Value;
                                    legalReview.StatusId = 1;
                                    legalReview.WorkOrderServicesId = i.WorkServicesId.Value;
                                    legalReview.CreatedBy = i.CreatedBy;
                                    legalReview.CreatedDate = DateTime.Now;
                                    _legalReviewRepository.Add(_mapper.Map<LegalReview>(legalReview));
                                    break;
                                case (5):
                                    DocumentManagementDto documentManagement = new DocumentManagementDto();
                                    documentManagement.ApplicantId = i.DeliveredTo.Value;
                                    documentManagement.AuthoDate = dto.CreatedDate.Value;
                                    documentManagement.AuthoAcceptanceDate = i.Acceptance.Value;
                                    documentManagement.StatusId = 1;
                                    documentManagement.WorkOrderServicesId = i.WorkServicesId.Value;
                                    documentManagement.CreatedBy = i.CreatedBy;
                                    documentManagement.CreatedDate = DateTime.Now;
                                    documentManagement.ServiceId = i.ServiceId;
                                    documentManagement.ProjectFee = a.ProjectedFee;
                                    _documentManagementRepository.Add(_mapper.Map<DocumentManagement>(documentManagement));
                                    break;
                                case (6):
                                    LocalDocumentationDto localDocumentation = new LocalDocumentationDto();
                                    localDocumentation.ApplicantId = i.DeliveredTo.Value;
                                    localDocumentation.AuthoDate = dto.CreatedDate.Value;
                                    localDocumentation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    localDocumentation.StatusId = 1;
                                    localDocumentation.WorkOrderServicesId = i.WorkServicesId.Value;
                                    localDocumentation.CreatedBy = i.CreatedBy;
                                    localDocumentation.CreatedDate = DateTime.Now;
                                    localDocumentation.ServiceId = i.ServiceId;
                                    localDocumentation.ProjectFee = a.ProjectedFee;
                                    _localDocumentationRepository.Add(_mapper.Map<LocalDocumentation>(localDocumentation));
                                    break;
                                case (12):
                                    PredecisionOrientationDto predicisionOrientation = new PredecisionOrientationDto();
                                    predicisionOrientation.AuthoDate = dto.CreatedDate.Value;
                                    predicisionOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    predicisionOrientation.StatusId = 1;
                                    predicisionOrientation.WorkOrderServicesId = i.WorkServicesId.Value;
                                    predicisionOrientation.CreatedBy = i.CreatedBy;
                                    predicisionOrientation.CreatedDate = DateTime.Now;
                                    predicisionOrientation.Schoolings = new List<SchoolingDto>();
                                    foreach (var d in service.AssigneeInformations.FirstOrDefault().DependentInformations)
                                    {
                                        if (d.RelationshipId == 2)
                                        {
                                            d.NationalityId = _DeliveringIn;
                                            SchoolingDto schooling = new SchoolingDto();
                                            schooling.RelationshipId = d.Id;
                                            schooling.Age = d.Age;
                                            schooling.Birth = d.Birth;
                                            schooling.Comments = d.AditionalComments;
                                            schooling.CurrentGrade = d.CurrentGrade;
                                            schooling.LanguagesSpoken = d.LanguagesId;
                                            schooling.Name = d.Name;
                                            schooling.Nationality = d.NationalityId;
                                            schooling.Active = true;
                                            predicisionOrientation.Schoolings.Add(schooling);
                                        }
                                    }
                                    _predicisionOrientationRepository.Add(_mapper.Map<PredecisionOrientation>(predicisionOrientation));
                                    break;
                                case (13):
                                    AreaOrientationDto areaOrientation = new AreaOrientationDto();
                                    areaOrientation.AuthoDate = dto.CreatedDate.Value;
                                    areaOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    areaOrientation.StatusId = 1;
                                    areaOrientation.WorkOrderServicesId = i.WorkServicesId.Value;
                                    areaOrientation.CreatedBy = i.CreatedBy;
                                    areaOrientation.CreatedDate = DateTime.Now;
                                    areaOrientation.SchoolingAreaOrientations = new List<SchoolingAreaOrientationDto>();
                                    foreach (var d in service.AssigneeInformations.FirstOrDefault().DependentInformations)
                                    {
                                        if (d.RelationshipId == 2)
                                        {
                                            d.NationalityId = _DeliveringIn;
                                            SchoolingAreaOrientationDto schoolingAreaOrientation = new SchoolingAreaOrientationDto();
                                            schoolingAreaOrientation.RelationshipId = d.Id;
                                            schoolingAreaOrientation.Age = d.Age;
                                            schoolingAreaOrientation.Birth = d.Birth;
                                            schoolingAreaOrientation.Comments = d.AditionalComments;
                                            schoolingAreaOrientation.CurrentGrade = d.CurrentGrade;
                                            List<LanguagesSpokenSchoolingAreaOrientationDto> languages = new List<LanguagesSpokenSchoolingAreaOrientationDto>();
                                            foreach (var o in d.LanguageDependentInformations)
                                            {
                                                languages.Add(new LanguagesSpokenSchoolingAreaOrientationDto()
                                                {
                                                    Schooling = 0,
                                                    LanguagesSpoken = o.Language
                                                });
                                            }
                                            schoolingAreaOrientation.LanguagesSpokenSchoolingAreaOrientations = languages;
                                            schoolingAreaOrientation.Name = d.Name;
                                            schoolingAreaOrientation.Nationality = d.NationalityId;
                                            schoolingAreaOrientation.Sex = d.Sex;
                                            schoolingAreaOrientation.Active = true;
                                            areaOrientation.SchoolingAreaOrientations.Add(schoolingAreaOrientation);
                                        }
                                    }
                                    _areaOrientationRepository.Add(_mapper.Map<AreaOrientation>(areaOrientation));
                                    break;
                                case (14):
                                    SettlingInDto settlingIn = new SettlingInDto();
                                    settlingIn.AuthoDate = dto.CreatedDate.Value;
                                    settlingIn.AuthoAcceptanceDate = i.Acceptance.Value;
                                    settlingIn.StatusId = 1;
                                    settlingIn.WorkOrderServicesId = i.WorkServicesId.Value;
                                    settlingIn.CreatedBy = i.CreatedBy;
                                    settlingIn.CreatedDate = DateTime.Now;
                                    _settlingInRepository.Add(_mapper.Map<SettlingIn>(settlingIn));
                                    break;
                                case (15):
                                    SchoolingSearchDto schoolingSearch = new SchoolingSearchDto();
                                    schoolingSearch.AuthoDate = dto.CreatedDate.Value;
                                    schoolingSearch.AuthoAcceptanceDate = i.Acceptance.Value;
                                    schoolingSearch.StatusId = 1;
                                    schoolingSearch.WorkOrderServicesId = i.WorkServicesId.Value;
                                    schoolingSearch.CreatedBy = i.CreatedBy;
                                    schoolingSearch.CreatedDate = DateTime.Now;
                                    schoolingSearch.SchoolingInformations = new List<SchoolingInformationDto>();
                                    foreach (var s in service.AssigneeInformations.FirstOrDefault().DependentInformations)
                                    {
                                        if (s.RelationshipId == 2)
                                        {
                                            s.NationalityId = s.NationalityId;
                                            SchoolingInformationDto schoolingInformation = new SchoolingInformationDto();
                                            schoolingInformation.RelationshipId = s.Id;
                                            schoolingInformation.Age = s.Age;
                                            schoolingInformation.Birth = s.Birth;
                                            schoolingInformation.Comments = s.AditionalComments;
                                            schoolingInformation.CurrentGrade = s.CurrentGrade;
                                            List<LanguangeSpokenSchoolingInformationDto> languages = new List<LanguangeSpokenSchoolingInformationDto>();
                                            foreach (var o in s.LanguageDependentInformations)
                                            {
                                                languages.Add(new LanguangeSpokenSchoolingInformationDto()
                                                {
                                                    LanguageSpoken = o.Language,
                                                    SchoolingInformation = 0
                                                });
                                            }
                                            schoolingInformation.LanguangeSpokenSchoolingInformations = languages;
                                            schoolingInformation.Name = s.Name;
                                            schoolingInformation.Nationality = s.NationalityId;
                                            schoolingInformation.Active = true;
                                            schoolingSearch.SchoolingInformations.Add(schoolingInformation);
                                        }
                                    }
                                    _schoolingSearchRepository.Add(_mapper.Map<SchoolingSearch>(schoolingSearch));
                                    break;
                                case (16):
                                    DepartureDto departure = new DepartureDto();
                                    departure.AuthoDate = dto.CreatedDate.Value;
                                    departure.AuthoAcceptanceDate = i.Acceptance.Value;
                                    departure.StatusId = 1;
                                    departure.WorkOrderServicesId = i.WorkServicesId.Value;
                                    //departure.CreatedBy = i.CreatedBy;
                                    //departure.CreatedDate = DateTime.Now;
                                    _departureRepository.Add(_mapper.Map<Departure>(departure));
                                    break;
                                case (17):
                                    TemporaryHousingCoordinatonDto temporaryHousingCoordinaton = new TemporaryHousingCoordinatonDto();
                                    temporaryHousingCoordinaton.AuthoDate = dto.CreatedDate.Value;
                                    temporaryHousingCoordinaton.AuthoAcceptanceDate = i.Acceptance.Value;
                                    temporaryHousingCoordinaton.StatusId = 1;
                                    temporaryHousingCoordinaton.WorkOrderServicesId = i.WorkServicesId.Value;
                                    temporaryHousingCoordinaton.CreatedBy = i.CreatedBy;
                                    temporaryHousingCoordinaton.CreatedDate = DateTime.Now;
                                    _temporaryHousingCoordinatonRepository.Add(_mapper.Map<TemporaryHousingCoordinaton>(temporaryHousingCoordinaton));
                                    break;
                                case (18):
                                    RentalFurnitureCoordinationDto rentalFurnitureCoordination = new RentalFurnitureCoordinationDto();
                                    rentalFurnitureCoordination.AuthoDate = dto.CreatedDate.Value;
                                    rentalFurnitureCoordination.AuthoAcceptanceDate = i.Acceptance.Value;
                                    rentalFurnitureCoordination.StatusId = 1;
                                    rentalFurnitureCoordination.WorkOrderServicesId = i.WorkServicesId.Value;
                                    rentalFurnitureCoordination.CreatedBy = i.CreatedBy;
                                    rentalFurnitureCoordination.CreatedDate = DateTime.Now;
                                    _rentalFurnitureCoordinationRepository.Add(_mapper.Map<RentalFurnitureCoordination>(rentalFurnitureCoordination));
                                    break;
                                case (19):
                                    TransportationDto transportation = new TransportationDto();
                                    transportation.AuthoDate = dto.CreatedDate.Value;
                                    transportation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    transportation.ApplicantId = i.DeliveredTo;
                                    transportation.StatusId = 1;
                                    transportation.WorkOrderServicesId = i.WorkServicesId.Value;
                                    transportation.ProjectFee = a.ProjectedFee;
                                    //transportation.CreatedBy = i.CreatedBy;
                                    //transportation.CreatedDate = DateTime.Now;
                                    _transportationRepository.Add(_mapper.Map<Transportation>(transportation));
                                    break;
                                case (20):
                                    AirportTransportationServiceDto airportTransportationService = new AirportTransportationServiceDto();
                                    airportTransportationService.AuthoDate = dto.CreatedDate.Value;
                                    airportTransportationService.AuthoAcceptanceDate = i.Acceptance.Value;
                                    airportTransportationService.StatusId = 1;
                                    airportTransportationService.WorkOrderServicesId = i.WorkServicesId.Value;
                                    airportTransportationService.CreatedBy = i.CreatedBy;
                                    airportTransportationService.CreatedDate = DateTime.Now;
                                    airportTransportationService.ProjectFee = a.ProjectedFee;
                                    airportTransportationService.ApplicantId = i.DeliveredTo.Value;
                                    _airportTransportationServicesRepository.Add(_mapper.Map<AirportTransportationService>(airportTransportationService));
                                    break;
                                case (21):
                                    HomeFindingDto homeFinding = new HomeFindingDto();
                                    homeFinding.AuthoDate = dto.CreatedDate.Value;
                                    homeFinding.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeFinding.StatusId = 1;
                                    homeFinding.WorkOrderServicesId = i.WorkServicesId.Value;
                                    homeFinding.CreatedBy = i.CreatedBy;
                                    homeFinding.CreatedDate = DateTime.Now;
                                    _homeFindingRepository.Add(_mapper.Map<HomeFinding>(homeFinding));
                                    break;
                                case (22):
                                    LeaseRenewalDto leaseRenewal = new LeaseRenewalDto();
                                    leaseRenewal.AuthoDate = dto.CreatedDate.Value;
                                    leaseRenewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                    leaseRenewal.StatusId = 1;
                                    leaseRenewal.WorkOrderServices = i.WorkServicesId.Value;
                                    leaseRenewal.CreationBy = i.CreatedBy;
                                    leaseRenewal.CreatedDate = DateTime.Now;
                                    _leaseRenewalRepository.Add(_mapper.Map<LeaseRenewal>(leaseRenewal));
                                    break;
                                case (23):
                                    HomeSaleDto homeSaleDto = new HomeSaleDto();
                                    homeSaleDto.AuthoDate = dto.CreatedDate.Value;
                                    homeSaleDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeSaleDto.StatusId = 1;
                                    homeSaleDto.WorkOrderServices = i.WorkServicesId.Value;
                                    homeSaleDto.CreatedBy = i.CreatedBy;
                                    homeSaleDto.CreatedDate = DateTime.Now;
                                    _homeSaleRepository.Add(_mapper.Map<HomeSale>(homeSaleDto));
                                    break;
                                case (24):
                                    HomePurchaseDto homePurchaseDto = new HomePurchaseDto();
                                    homePurchaseDto.AuthoDate = dto.CreatedDate.Value;
                                    homePurchaseDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homePurchaseDto.StatusId = 1;
                                    homePurchaseDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.Cre = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _homePurchaseRepository.Add(_mapper.Map<HomePurchase>(homePurchaseDto));
                                    break;
                                case (25):
                                    PropertyManagementDto propertyManagementDto = new PropertyManagementDto();
                                    propertyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    propertyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    propertyManagementDto.StatusId = 1;
                                    propertyManagementDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.Cre = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _propertyManagementRepository.Add(_mapper.Map<PropertyManagement>(propertyManagementDto));
                                    break;
                                case (26):
                                    OtherDto otherDto = new OtherDto();
                                    otherDto.AuthoDate = dto.CreatedDate.Value;
                                    otherDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherDto.StatusId = 1;
                                    otherDto.WorkOrderServices = i.WorkServicesId.Value;
                                    otherDto.Coordination = i.BundledServiceOrder.Package.Value;
                                    otherDto.CreatedBy = i.CreatedBy;
                                    otherDto.CreatedDate = DateTime.Now;
                                    _otherRepository.Add(_mapper.Map<Other>(otherDto));
                                    break;
                                case (27):
                                    TenancyManagementDto tenancyManagementDto = new TenancyManagementDto();
                                    tenancyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    tenancyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    tenancyManagementDto.StatusId = 1;
                                    tenancyManagementDto.WorkOrderServices = i.WorkServicesId.Value;
                                    tenancyManagementDto.CreatedBy = i.CreatedBy;
                                    tenancyManagementDto.CreatedDate = DateTime.Now;
                                    tenancyManagementDto.Coordination = i.BundledServiceOrder.Package.Value;
                                    _tenancyManagementRepository.Add(_mapper.Map<TenancyManagement>(tenancyManagementDto));
                                    break;
                                case (28):
                                    OtherDto otherImmigration = new OtherDto();
                                    otherImmigration.AuthoDate = dto.CreatedDate.Value;
                                    otherImmigration.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherImmigration.StatusId = 1;
                                    otherImmigration.WorkOrderServices = i.WorkServicesId.Value;
                                    otherImmigration.Coordination = i.BundledServiceOrder.Package.Value;
                                    otherImmigration.CreatedBy = i.CreatedBy;
                                    otherImmigration.CreatedDate = DateTime.Now;
                                    _otherRepository.Add(_mapper.Map<Other>(otherImmigration));
                                    break;
                            }
                        }
                    }
                    response.Result = _mapper.Map<WorkOrderDto>(order);
                    
                    // NOTIFICATIONS COORDINATORS
                    int type = _serviceRecordRepository.IsNewServiceRecord(service.Id) ? 1 : 22;
                    if (service.ImmigrationCoodinators.Any())
                    {
                        
                        var userTo =
                            _notificationSystemRepository.GetUserId(service.ImmigrationCoodinators.FirstOrDefault()
                                .CoordinatorId.Value);
                        var typeText = _notificationSystemTypeRepository.Find(x => x.Id == type).Type;
                        NotificationSystemDto notification = new NotificationSystemDto();
                        notification.Archive = false;
                        notification.View = false;
                        notification.ServiceRecord = service.Id;
                        notification.Time = DateTime.Now.TimeOfDay;
                        notification.UserFrom = dto.CreatedBy;
                        notification.UserTo = _notificationSystemRepository.GetUserId(service.ImmigrationCoodinators.FirstOrDefault().CoordinatorId.Value);
                        notification.NotificationType = type;
                        notification.Description = "Please accept the notification to continue with the SR";
                        notification.Color = "#f9a825";
                        notification.CreatedDate = DateTime.Now;
                        _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                        _notificationSystemRepository.SendNotificationAsync(
                            userTo,
                            order.CreatedBy.Value,
                            0,
                            typeText,
                            $"{typeText}, {order.NumberWorkOrder}",
                            type
                        );
                        if (service.Urgent.HasValue && type == 1)
                        {
                            if (service.Urgent.Value.Equals(true))
                            {
                                _notificationSystemRepository.Add(new NotificationSystem()
                                {
                                    Archive = false,
                                    View = false,
                                    ServiceRecord = service.Id,
                                    Time = DateTime.Now.TimeOfDay,
                                    UserFrom = service.CreatedBy,
                                    UserTo = userTo,
                                    NotificationType = 27,
                                    Description = "The SR is urgent, please follow up immediately",
                                    Color = "#f9a825",
                                    CreatedDate = DateTime.Now
                                });
                                _notificationSystemRepository.SendNotificationAsync(
                                    userTo,
                                    0,
                                    0,
                                    _notificationSystemTypeRepository.Find(x => x.Id == 27).Type,
                                    $"{service.NumberServiceRecord} is urgent",
                                    27
                                );
                            }
                        }
                    }
                    else if(service.RelocationCoordinators.Any())
                    {
                        var userTo =
                            _notificationSystemRepository.GetUserId(service.RelocationCoordinators.FirstOrDefault()
                                .CoordinatorId.Value);
                        var typeText02 = _notificationSystemTypeRepository.Find(x => x.Id == type).Type;
                        NotificationSystemDto notification = new NotificationSystemDto();
                        notification.Archive = false;
                        notification.View = false;
                        notification.ServiceRecord = service.Id;
                        notification.Time = DateTime.Now.TimeOfDay;
                        notification.UserFrom = dto.CreatedBy;
                        notification.UserTo = _notificationSystemRepository.GetUserId(service.RelocationCoordinators.FirstOrDefault().CoordinatorId.Value);
                        notification.NotificationType = type;
                        notification.Description = "Please accept the notification to continue with the SR";
                        notification.Color = "#f9a825";
                        notification.CreatedDate = DateTime.Now;
                        _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                        _notificationSystemRepository.SendNotificationAsync(
                            userTo,
                            order.CreatedBy.Value,
                            0,
                            typeText02,
                            $"{typeText02}, {order.NumberWorkOrder}",
                            type
                        );
                        if (service.Urgent.HasValue && type == 1)
                        {
                            if (service.Urgent.Value.Equals(true))
                            {
                                _notificationSystemRepository.Add(new NotificationSystem()
                                {
                                    Archive = false,
                                    View = false,
                                    ServiceRecord = service.Id,
                                    Time = DateTime.Now.TimeOfDay,
                                    UserFrom = service.CreatedBy,
                                    UserTo = userTo,
                                    NotificationType = 27,
                                    Description = "The SR is urgent, please follow up immediately",
                                    Color = "f9a825",
                                    CreatedDate = DateTime.Now
                                });
                                _notificationSystemRepository.SendNotificationAsync(
                                    userTo,
                                    0,
                                    0,
                                    _notificationSystemTypeRepository.Find(x => x.Id == 27).Type,
                                    $"{service.NumberServiceRecord} is urgent",
                                    27
                                );
                            }
                        }
                    }
                    // NOTIFICATION COUNTRY Leaders HOST COUNTRY
                    var leaders = _notificationSystemRepository.GetCountryLeader(service.Id);
                    if (leaders.Any())
                    {
                        var typeText03 = _notificationSystemTypeRepository.Find(x => x.Id == 22).Type;
                        foreach (var leader in leaders)
                        {
                            NotificationSystemDto notification = new NotificationSystemDto();
                            notification.Archive = false;
                            notification.View = false;
                            notification.ServiceRecord = service.Id;
                            notification.Time = DateTime.Now.TimeOfDay;
                            notification.UserFrom = dto.CreatedBy;
                            notification.UserTo = leader;
                            notification.NotificationType = 22;
                            notification.Description = "Country leader, please follow up on the Work Order";
                            notification.Color = "#f9a825";
                            notification.CreatedDate = DateTime.Now;
                            _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                            _notificationSystemRepository.SendNotificationAsync(
                                leader,
                                order.CreatedBy.Value,
                                0,
                                typeText03,
                                $"{typeText03}, {order.NumberWorkOrder}",
                                22
                            );
                        }
                    }
                    // END REGION
                    _serviceRecordRepository.EndTransaction();
                }
                else
                {
                    response.Success = false;
                    response.Message = "Service Record Not Found";
                }
            }
            catch (Exception ex)
            {
                _serviceOrderRepository.RollBack();
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpPut("UpdateOrder", Name = "UpdateOrder")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<WorkOrderDto>> PostUpdate([FromBody] WorkOrderInsertDto dto)
        {
            var response = new ApiResponse<WorkOrderDto>();
            try
            {
                _serviceOrderRepository.BeginTransaction();
                var order = _serviceOrderRepository.Find(c => c.Id == dto.Id);
                WorkOrderInsertDto orders = _mapper.Map<WorkOrderInsertDto>(_serviceOrderRepository.GetWorkOrder(order.Id));
                if (order != null)
                {
                    int lastIdServiceOrder = _serviceOrderRepository.lastIdServiceOrderServices();
                    int lastIDBundle = _serviceOrderRepository.lastIdBundleService();

                    foreach (var s in dto.StandaloneServiceWorkOrders)
                    {
                        if (s.Id == 0)
                        {
                            s.ServiceNumber = $"SN-{ lastIdServiceOrder }";
                            lastIdServiceOrder++;
                        }
                    }

                    foreach (var sos in dto.BundledServicesWorkOrders)
                    {
                        foreach (var b in sos.BundledServices)
                        {
                            if(b.Id == 0)
                            {
                                b.ServiceNumber = $"SN-{ lastIDBundle }";
                                lastIDBundle++;
                            }
                        }
                    }

                    

                    WorkOrder service = _serviceOrderRepository.UpdateCustom(_mapper.Map<WorkOrder>(dto), order.Id);

                    ServiceRecordSelectDto custom = _mapper.Map<ServiceRecordSelectDto>(_serviceRecordRepository.SelectCustom(dto.ServiceRecordId.Value, 0));

                    foreach (var i in service.StandaloneServiceWorkOrders)
                    {
                        int _DeliveringIn = _serviceOrderRepository.GetCountryToCountry(i.DeliveringIn.Value);
                        i.DeliveringIn = _DeliveringIn;

                        if (i.Acceptance == null)
                        {
                            i.Acceptance = DateTime.Now;
                        }

                        switch (i.CategoryId)
                        {
                            case (1):
                                var _exist = _immigrationEntryVisaRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                EntryVisaDto entryVisa = new EntryVisaDto();
                                if (_exist == null)
                                {
                                    entryVisa.ApplicantId = i.DeliveredTo.Value;
                                    entryVisa.AuthoDate = dto.CreatedDate.Value;
                                    entryVisa.AuthoAcceptanceDate = i.Acceptance.Value;
                                    entryVisa.StatusId = i.StatusId.Value;
                                    entryVisa.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    entryVisa.CreatedBy = i.CreatedBy;
                                    entryVisa.CreatedDate = DateTime.Now;
                                    entryVisa.CountryId = i.DeliveringIn;
                                    entryVisa.CityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _immigrationEntryVisaRepository.Add(_mapper.Map<EntryVisa>(entryVisa));
                                }
                                else
                                {
                                    entryVisa.Id = _exist.Id;
                                    entryVisa.ApplicantId = i.DeliveredTo.Value;
                                    entryVisa.AuthoDate = dto.CreatedDate.Value;
                                    entryVisa.AuthoAcceptanceDate = i.Acceptance.Value;
                                    entryVisa.StatusId = _exist.StatusId;
                                    entryVisa.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    entryVisa.UpdateBy = i.CreatedBy;
                                    entryVisa.UpdatedDate = DateTime.Now;
                                    entryVisa.CreatedBy = _exist.CreatedBy;
                                    entryVisa.CreatedDate = _exist.CreatedDate;
                                    entryVisa.CountryId = i.DeliveringIn;
                                    entryVisa.CityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _immigrationEntryVisaRepository.Update(_mapper.Map<EntryVisa>(entryVisa), _exist.Id);
                                }
                                break;
                            case (2):
                                var _existWP = _workPermitRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                WorkPermitDto workPermit = new WorkPermitDto();
                                if (_existWP == null)
                                {
                                    workPermit.ApplicantId = i.DeliveredTo.Value;
                                    workPermit.AuthoDate = dto.CreatedDate.Value;
                                    workPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                    workPermit.StatusId = i.StatusId.Value;
                                    workPermit.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    workPermit.CreatedBy = i.CreatedBy;
                                    workPermit.CreatedDate = DateTime.Now;
                                    workPermit.HostCountryId = i.DeliveringIn;
                                    workPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _workPermitRepository.Add(_mapper.Map<WorkPermit>(workPermit));
                                }
                                else
                                {
                                    workPermit.Id = _existWP.Id;
                                    workPermit.ApplicantId = i.DeliveredTo.Value;
                                    workPermit.AuthoDate = dto.CreatedDate.Value;
                                    workPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                    workPermit.StatusId = _existWP.StatusId.Value;
                                    workPermit.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    workPermit.UpdatedBy = i.CreatedBy;
                                    workPermit.UpdatedDate = DateTime.Now;
                                    workPermit.CreatedBy = _existWP.CreatedBy;
                                    workPermit.UpdatedDate = _existWP.UpdatedDate;
                                    workPermit.HostCountryId = i.DeliveringIn;
                                    workPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _workPermitRepository.Update(_mapper.Map<WorkPermit>(workPermit), _existWP.Id);
                                }
                                break;
                            case (4):
                                var _existRP = _residencyPermitRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                ResidencyPermitDto residencyPermit = new ResidencyPermitDto();
                                if(_existRP == null)
                                {
                                    residencyPermit.ApplicantId = i.DeliveredTo.Value;
                                    residencyPermit.AuthoDate = dto.CreatedDate.Value;
                                    residencyPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                    residencyPermit.StatusId = i.StatusId.Value;
                                    residencyPermit.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    residencyPermit.CreatedBy = i.CreatedBy;
                                    residencyPermit.CreatedDate = DateTime.Now;
                                    residencyPermit.HostCountryId = i.DeliveringIn;
                                    residencyPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _residencyPermitRepository.Add(_mapper.Map<ResidencyPermit>(residencyPermit));
                                }
                                else
                                {
                                    residencyPermit.Id = _existRP.Id;
                                    residencyPermit.ApplicantId = i.DeliveredTo.Value;
                                    residencyPermit.AuthoDate = dto.CreatedDate.Value;
                                    residencyPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                    residencyPermit.StatusId = _existRP.StatusId.Value;
                                    residencyPermit.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    residencyPermit.CreatedBy = _existRP.UpdatedBy;
                                    residencyPermit.CreatedDate = _existRP.CreatedDate;
                                    residencyPermit.UpdatedBy = i.CreatedBy;
                                    residencyPermit.UpdatedDate = DateTime.Now;
                                    residencyPermit.HostCountryId = i.DeliveringIn;
                                    residencyPermit.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _residencyPermitRepository.Update(_mapper.Map<ResidencyPermit>(residencyPermit), _existRP.Id);
                                }
                                break;
                            case (3):
                                var _existVDR = _visaDeregistrationRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                VisaDeregistrationDto visaDeregistration = new VisaDeregistrationDto();
                                if(_existVDR == null)
                                {
                                    visaDeregistration.ApplicantId = i.DeliveredTo.Value;
                                    visaDeregistration.AuthoDate = dto.CreatedDate.Value;
                                    visaDeregistration.AuthoAcceptanceDate = i.Acceptance.Value;
                                    visaDeregistration.StatusId = i.StatusId.Value;
                                    visaDeregistration.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    visaDeregistration.CreatedBy = i.CreatedBy;
                                    visaDeregistration.CreatedDate = DateTime.Now;
                                    visaDeregistration.HostCountryId = i.DeliveringIn;
                                    visaDeregistration.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _visaDeregistrationRepository.Add(_mapper.Map<VisaDeregistration>(visaDeregistration));
                                }
                                else
                                {
                                    visaDeregistration.Id = _existVDR.Id;
                                    visaDeregistration.ApplicantId = i.DeliveredTo.Value;
                                    visaDeregistration.AuthoDate = dto.CreatedDate.Value;
                                    visaDeregistration.AuthoAcceptanceDate = i.Acceptance.Value;
                                    visaDeregistration.StatusId = _existVDR.StatusId.Value;
                                    visaDeregistration.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    visaDeregistration.CreatedBy = _existVDR.CreatedBy;
                                    visaDeregistration.CreatedDate = _existVDR.CreatedDate;
                                    visaDeregistration.UpdatedBy = i.CreatedBy;
                                    visaDeregistration.UpdatedDate = DateTime.Now;
                                    visaDeregistration.HostCountryId = i.DeliveringIn;
                                    visaDeregistration.HostCityId = _cityRepository.Find(x => x.City == i.Location).Id;
                                    _visaDeregistrationRepository.Update(_mapper.Map<VisaDeregistration>(visaDeregistration), _existVDR.Id);
                                }
                                break;
                            case (8):
                                var _existR = _renewalRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                RenewalDto renewal = new RenewalDto();
                                if(_existR == null)
                                {
                                    renewal.ApplicantId = i.DeliveredTo.Value;
                                    renewal.AuthoDate = dto.CreatedDate.Value;
                                    renewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                    renewal.StatusId = i.StatusId.Value;
                                    renewal.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    renewal.CreatedBy = i.CreatedBy;
                                    renewal.CreatedDate = DateTime.Now;
                                    _renewalRepository.Add(_mapper.Map<Renewal>(renewal));
                                }
                                else
                                {
                                    renewal.Id = _existR.Id;
                                    renewal.ApplicantId = i.DeliveredTo.Value;
                                    renewal.AuthoDate = dto.CreatedDate.Value;
                                    renewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                    renewal.StatusId = _existR.StatusId.Value;
                                    renewal.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    renewal.CreatedBy = _existR.CreatedBy;
                                    renewal.CreatedDate = _existR.CreatedDate;
                                    renewal.UpdatedBy = i.CreatedBy;
                                    renewal.UpdatedDate = DateTime.Now;
                                    _renewalRepository.Update(_mapper.Map<Renewal>(renewal), _existR.Id);
                                }
                                break;
                            case (7):
                                var _existCA = _corporateAssistanceRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                CorporateAssistanceDto corporateAssistance = new CorporateAssistanceDto();
                                if(_existCA == null)
                                {
                                    corporateAssistance.ApplicantId = i.DeliveredTo.Value;
                                    corporateAssistance.AuthoDate = dto.CreatedDate.Value;
                                    corporateAssistance.AuthoAcceptanceDate = i.Acceptance.Value;
                                    corporateAssistance.StatusId = i.StatusId.Value;
                                    corporateAssistance.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    corporateAssistance.CreatedBy = i.CreatedBy;
                                    corporateAssistance.CreatedDate = DateTime.Now;
                                    corporateAssistance.ServiceTypeId = i.ServiceTypeId;
                                    _corporateAssistanceRepository.Add(_mapper.Map<CorporateAssistance>(corporateAssistance));
                                }
                                else
                                {
                                    corporateAssistance.Id = _existCA.Id;
                                    corporateAssistance.ApplicantId = i.DeliveredTo.Value;
                                    corporateAssistance.AuthoDate = dto.CreatedDate.Value;
                                    corporateAssistance.AuthoAcceptanceDate = i.Acceptance.Value;
                                    corporateAssistance.StatusId = _existCA.StatusId;
                                    corporateAssistance.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    corporateAssistance.CreatedBy = _existCA.CreatedBy;
                                    corporateAssistance.CreatedDate = _existCA.CreatedDate;
                                    corporateAssistance.UpdatedBy = i.CreatedBy;
                                    corporateAssistance.UpdatedDate = DateTime.Now;
                                    corporateAssistance.ServiceTypeId = i.ServiceTypeId;
                                    _corporateAssistanceRepository.Update(_mapper.Map<CorporateAssistance>(corporateAssistance), _existCA.Id);
                                }
                                break;
                            case (9):
                                var _existN = _notificationRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                NotificationDto notification = new NotificationDto();
                                if (_existN == null)
                                {
                                    notification.ApplicantId = i.DeliveredTo.Value;
                                    notification.AuthoDate = dto.CreatedDate.Value;
                                    notification.AuthoAcceptanceDate = i.Acceptance.Value;
                                    notification.StatusId = i.StatusId.Value;
                                    notification.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    notification.CreatedBy = i.CreatedBy;
                                    notification.CreatedDate = DateTime.Now;
                                    _notificationRepository.Add(_mapper.Map<Notification>(notification));
                                }
                                else
                                {
                                    notification.Id = _existN.Id;
                                    notification.ApplicantId = i.DeliveredTo.Value;
                                    notification.AuthoDate = dto.CreatedDate.Value;
                                    notification.AuthoAcceptanceDate = i.Acceptance.Value;
                                    notification.StatusId = _existN.StatusId;
                                    notification.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    notification.CreatedBy = _existN.CreatedBy;
                                    notification.CreatedDate = _existN.CreatedDate;
                                    notification.UpdatedBy = i.CreatedBy;
                                    notification.UpdatedDate = DateTime.Now;
                                    _notificationRepository.Update(_mapper.Map<Notification>(notification), _existN.Id);
                                }
                                break;
                            case (10):
                                var _existLR = _legalReviewRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                LegalReviewDto legalReview = new LegalReviewDto();
                                if(_existLR == null)
                                {
                                    legalReview.ApplicantId = i.DeliveredTo.Value;
                                    legalReview.AuthoDate = dto.CreatedDate.Value;
                                    legalReview.AuthoAcceptanceDate = i.Acceptance.Value;
                                    legalReview.StatusId = i.StatusId.Value;
                                    legalReview.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    legalReview.CreatedBy = i.CreatedBy;
                                    legalReview.CreatedDate = DateTime.Now;
                                    _legalReviewRepository.Add(_mapper.Map<LegalReview>(legalReview));
                                }
                                else
                                {
                                    legalReview.Id = _existLR.Id;
                                    legalReview.ApplicantId = i.DeliveredTo.Value;
                                    legalReview.AuthoDate = dto.CreatedDate.Value;
                                    legalReview.AuthoAcceptanceDate = i.Acceptance.Value;
                                    legalReview.StatusId = _existLR.StatusId;
                                    legalReview.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    legalReview.CreatedBy = _existLR.CreatedBy;
                                    legalReview.CreatedDate = _existLR.CreatedDate;
                                    legalReview.UpdatedBy = i.CreatedBy;
                                    legalReview.UpdatedDate = DateTime.Now;
                                    _legalReviewRepository.Update(_mapper.Map<LegalReview>(legalReview), _existLR.Id);
                                }
                                break;
                            case (5):
                                var _existDM = _documentManagementRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                DocumentManagementDto documentManagement = new DocumentManagementDto();
                                if(_existDM == null)
                                {
                                    documentManagement.ApplicantId = i.DeliveredTo.Value;
                                    documentManagement.AuthoDate = dto.CreatedDate.Value;
                                    documentManagement.AuthoAcceptanceDate = i.Acceptance.Value;
                                    documentManagement.StatusId = i.StatusId.Value;
                                    documentManagement.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    documentManagement.CreatedBy = i.CreatedBy;
                                    documentManagement.CreatedDate = DateTime.Now;
                                    documentManagement.ServiceId = i.ServiceId;
                                    documentManagement.ProjectFee = i.ProjectedFee;
                                    _documentManagementRepository.Add(_mapper.Map<DocumentManagement>(documentManagement));
                                }
                                else
                                {
                                    documentManagement.Id = _existDM.Id;
                                    documentManagement.ApplicantId = i.DeliveredTo.Value;
                                    documentManagement.AuthoDate = dto.CreatedDate.Value;
                                    documentManagement.AuthoAcceptanceDate = i.Acceptance.Value;
                                    documentManagement.StatusId = _existDM.StatusId;
                                    documentManagement.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    documentManagement.CreatedBy = _existDM.CreatedBy;
                                    documentManagement.CreatedDate = _existDM.CreatedDate;
                                    documentManagement.UpdateBy = i.CreatedBy;
                                    documentManagement.UpdatedDate = DateTime.Now;
                                    documentManagement.ServiceId = i.ServiceId;
                                    documentManagement.ProjectFee = i.ProjectedFee;
                                    _documentManagementRepository.Update(_mapper.Map<DocumentManagement>(documentManagement), _existDM.Id);
                                }
                                break;
                            case (6):
                                var _existLD = _localDocumentationRepository.FindAll(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value).FirstOrDefault();
                                LocalDocumentationDto localDocumentation = new LocalDocumentationDto();
                                if(_existLD == null)
                                {
                                    localDocumentation.ApplicantId = i.DeliveredTo.Value;
                                    localDocumentation.AuthoDate = dto.CreatedDate.Value;
                                    localDocumentation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    localDocumentation.StatusId = i.StatusId.Value;
                                    localDocumentation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    localDocumentation.CreatedBy = i.CreatedBy;
                                    localDocumentation.CreatedDate = DateTime.Now;
                                    localDocumentation.ServiceId = i.ServiceId;
                                    localDocumentation.ProjectFee = i.ProjectedFee;
                                    _localDocumentationRepository.Add(_mapper.Map<LocalDocumentation>(localDocumentation));
                                }
                                else
                                {
                                    localDocumentation.Id = _existLD.Id;
                                    localDocumentation.ApplicantId = i.DeliveredTo.Value;
                                    localDocumentation.AuthoDate = dto.CreatedDate.Value;
                                    localDocumentation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    localDocumentation.StatusId = _existLD.StatusId;
                                    localDocumentation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    localDocumentation.CreatedBy = _existLD.CreatedBy;
                                    localDocumentation.CreatedDate = _existLD.CreatedDate;
                                    localDocumentation.ServiceId = i.ServiceId;
                                    localDocumentation.ProjectFee = i.ProjectedFee;
                                    localDocumentation.UpdateBy = i.CreatedBy;
                                    localDocumentation.UpdatedDate = DateTime.Now;
                                    _localDocumentationRepository.Update(_mapper.Map<LocalDocumentation>(localDocumentation), _existLD.Id);
                                }
                                break;
                            case (12):
                                var _existPO = _predicisionOrientationRepository.FindAll(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value).FirstOrDefault();
                                PredecisionOrientationDto predicisionOrientation = new PredecisionOrientationDto();
                                if(_existPO == null)
                                {
                                    predicisionOrientation.AuthoDate = dto.CreatedDate.Value;
                                    predicisionOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    predicisionOrientation.StatusId = i.StatusId.Value;
                                    predicisionOrientation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    predicisionOrientation.CreatedBy = i.CreatedBy;
                                    predicisionOrientation.CreatedDate = DateTime.Now;
                                    predicisionOrientation.Schoolings = new List<SchoolingDto>();
                                    foreach (var d in custom.AssigneeInformations.FirstOrDefault().DependentInformations)
                                    {
                                        d.NationalityId = _DeliveringIn;
                                        if (d.RelationshipId == 2)
                                        {
                                            SchoolingDto schooling = new SchoolingDto();
                                            schooling.RelationshipId = d.Id;
                                            schooling.Age = d.Age;
                                            schooling.Birth = d.Birth;
                                            schooling.Comments = d.AditionalComments;
                                            schooling.CurrentGrade = d.CurrentGrade;
                                            schooling.LanguagesSpoken = d.LanguagesId;
                                            schooling.Name = d.Name;
                                            schooling.Nationality = d.NationalityId;
                                            schooling.Active = true;
                                            predicisionOrientation.Schoolings.Add(schooling);
                                        }
                                    }
                                    _predicisionOrientationRepository.Add(_mapper.Map<PredecisionOrientation>(predicisionOrientation));
                                }
                                else
                                {
                                    predicisionOrientation.Id = _existPO.Id;
                                    predicisionOrientation.AuthoDate = dto.CreatedDate.Value;
                                    predicisionOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    predicisionOrientation.StatusId = _existPO.StatusId;
                                    predicisionOrientation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    predicisionOrientation.CreatedBy = _existPO.CreatedBy;
                                    predicisionOrientation.CreatedDate = _existPO.CreatedDate;
                                    predicisionOrientation.UpdateBy = i.CreatedBy;
                                    predicisionOrientation.UpdatedDate = DateTime.Now;
                                    _predicisionOrientationRepository.Update(_mapper.Map<PredecisionOrientation>(predicisionOrientation), _existPO.Id);
                                }
                                break;
                            case (13):
                                var _existAO = _areaOrientationRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                AreaOrientationDto areaOrientation = new AreaOrientationDto();
                                if(_existAO == null)
                                {
                                    areaOrientation.AuthoDate = dto.CreatedDate.Value;
                                    areaOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    areaOrientation.StatusId = i.StatusId.Value;
                                    areaOrientation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    areaOrientation.CreatedBy = i.CreatedBy;
                                    areaOrientation.CreatedDate = DateTime.Now;
                                    areaOrientation.SchoolingAreaOrientations = new List<SchoolingAreaOrientationDto>();
                                    foreach (var d in custom.AssigneeInformations.FirstOrDefault().DependentInformations)
                                    {
                                        if (d.RelationshipId == 2)
                                        {
                                            d.NationalityId = _DeliveringIn;
                                            SchoolingAreaOrientationDto schoolingAreaOrientation = new SchoolingAreaOrientationDto();
                                            schoolingAreaOrientation.RelationshipId = d.Id;
                                            schoolingAreaOrientation.Age = d.Age;
                                            schoolingAreaOrientation.Birth = d.Birth;
                                            schoolingAreaOrientation.Comments = d.AditionalComments;
                                            schoolingAreaOrientation.CurrentGrade = d.CurrentGrade;
                                            List<LanguagesSpokenSchoolingAreaOrientationDto> languages = new List<LanguagesSpokenSchoolingAreaOrientationDto>();
                                            foreach (var o in d.LanguageDependentInformations)
                                            {
                                                languages.Add(new LanguagesSpokenSchoolingAreaOrientationDto()
                                                {
                                                    Schooling = 0,
                                                    LanguagesSpoken = o.Language
                                                });
                                            }
                                            schoolingAreaOrientation.LanguagesSpokenSchoolingAreaOrientations = languages;
                                            schoolingAreaOrientation.Name = d.Name;
                                            schoolingAreaOrientation.Nationality = d.NationalityId;
                                            schoolingAreaOrientation.Sex = d.Sex;
                                            schoolingAreaOrientation.Active = true;
                                            areaOrientation.SchoolingAreaOrientations.Add(schoolingAreaOrientation);
                                        }
                                    }
                                    _areaOrientationRepository.Add(_mapper.Map<AreaOrientation>(areaOrientation));
                                }
                                else
                                {
                                    areaOrientation.Id = _existAO.Id;
                                    areaOrientation.AuthoDate = dto.CreatedDate.Value;
                                    areaOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    areaOrientation.StatusId = _existAO.StatusId;
                                    areaOrientation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    areaOrientation.CreatedBy = _existAO.CreatedBy;
                                    areaOrientation.CreatedDate = _existAO.CreatedDate;
                                    areaOrientation.UpdateBy = i.CreatedBy;
                                    areaOrientation.UpdatedDate = DateTime.Now;
                                    _areaOrientationRepository.Update(_mapper.Map<AreaOrientation>(areaOrientation), _existAO.Id);
                                }
                                break;
                            case (14):
                                var _existS = _settlingInRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                SettlingInDto settlingIn = new SettlingInDto();
                                if(_existS == null)
                                {
                                    settlingIn.AuthoDate = dto.CreatedDate.Value;
                                    settlingIn.AuthoAcceptanceDate = i.Acceptance.Value;
                                    settlingIn.StatusId = i.StatusId.Value;
                                    settlingIn.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    settlingIn.CreatedBy = i.CreatedBy;
                                    settlingIn.CreatedDate = DateTime.Now;
                                    _settlingInRepository.Add(_mapper.Map<SettlingIn>(settlingIn));
                                }
                                else
                                {
                                    settlingIn.Id = _existS.Id;
                                    settlingIn.AuthoDate = dto.CreatedDate.Value;
                                    settlingIn.AuthoAcceptanceDate = i.Acceptance.Value;
                                    settlingIn.StatusId = _existS.StatusId;
                                    settlingIn.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    settlingIn.CreatedBy = _existS.CreatedBy;
                                    settlingIn.CreatedDate = _existS.CreatedDate;
                                    settlingIn.UpdateBy = i.CreatedBy;
                                    settlingIn.UpdatedDate = DateTime.Now;
                                    _settlingInRepository.Update(_mapper.Map<SettlingIn>(settlingIn), _existS.Id);
                                }
                                break;
                            case (15):
                                var _existSS = _schoolingSearchRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                SchoolingSearchDto schoolingSearch = new SchoolingSearchDto();
                                if (_existSS == null)
                                {
                                    schoolingSearch.AuthoDate = dto.CreatedDate.Value;
                                    schoolingSearch.AuthoAcceptanceDate = i.Acceptance.Value;
                                    schoolingSearch.StatusId = i.StatusId.Value;
                                    schoolingSearch.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    schoolingSearch.CreatedBy = i.CreatedBy;
                                    schoolingSearch.CreatedDate = DateTime.Now;
                                    schoolingSearch.SchoolingInformations = new List<SchoolingInformationDto>();
                                    foreach (var s in custom.AssigneeInformations.FirstOrDefault().DependentInformations)
                                    {
                                        if (s.RelationshipId == 2)
                                        {
                                            s.NationalityId = s.NationalityId;
                                            SchoolingInformationDto schoolingInformation = new SchoolingInformationDto();
                                            schoolingInformation.RelationshipId = s.Id;
                                            schoolingInformation.Age = s.Age;
                                            schoolingInformation.Birth = s.Birth;
                                            schoolingInformation.Comments = s.AditionalComments;
                                            schoolingInformation.CurrentGrade = s.CurrentGrade;
                                            LanguangeSpokenSchoolingInformationDto language = new LanguangeSpokenSchoolingInformationDto();
                                            List<LanguangeSpokenSchoolingInformationDto> languages = new List<LanguangeSpokenSchoolingInformationDto>();
                                            foreach (var o in s.LanguageDependentInformations)
                                            {
                                                languages.Add(new LanguangeSpokenSchoolingInformationDto()
                                                {
                                                    LanguageSpoken = o.Language,
                                                    SchoolingInformation = 0
                                                });
                                            }
                                            schoolingInformation.LanguangeSpokenSchoolingInformations = languages;
                                            schoolingInformation.Sex = s.Sex;
                                            schoolingInformation.Name = s.Name;
                                            schoolingInformation.Nationality = s.NationalityId;
                                            schoolingInformation.Active = true;
                                            schoolingSearch.SchoolingInformations.Add(schoolingInformation);
                                        }
                                    }
                                    _schoolingSearchRepository.Add(_mapper.Map<SchoolingSearch>(schoolingSearch));
                                }
                                else
                                {
                                    schoolingSearch.Id = _existSS.Id;
                                    schoolingSearch.AuthoDate = dto.CreatedDate.Value;
                                    schoolingSearch.AuthoAcceptanceDate = i.Acceptance.Value;
                                    schoolingSearch.StatusId = _existSS.StatusId;
                                    schoolingSearch.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    schoolingSearch.CreatedBy = _existSS.CreatedBy;
                                    schoolingSearch.CreatedDate = _existSS.CreatedDate;
                                    schoolingSearch.UpdateBy = i.CreatedBy;
                                    schoolingSearch.UpdatedDate = DateTime.Now;
                                    _schoolingSearchRepository.Update(_mapper.Map<SchoolingSearch>(schoolingSearch), _existSS.Id);
                                }
                                break;
                            case (16):
                                var _existD = _departureRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                DepartureDto departure = new DepartureDto();
                                if(_existD == null)
                                {
                                    departure.AuthoDate = dto.CreatedDate.Value;
                                    departure.AuthoAcceptanceDate = i.Acceptance.Value;
                                    departure.StatusId = i.StatusId.Value;
                                    departure.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    //departure.CreatedBy = i.CreatedBy;
                                    //departure.CreatedDate = DateTime.Now;
                                    _departureRepository.Add(_mapper.Map<Departure>(departure));
                                }
                                else
                                {
                                    departure.Id = _existD.Id;
                                    departure.AuthoDate = dto.CreatedDate.Value;
                                    departure.AuthoAcceptanceDate = i.Acceptance.Value;
                                    departure.StatusId = _existD.StatusId;
                                    departure.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    _departureRepository.Update(_mapper.Map<Departure>(departure), _existD.Id);
                                }
                                break;
                            case (17):
                                var _existTHC = _temporaryHousingCoordinatonRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                TemporaryHousingCoordinatonDto temporaryHousingCoordinaton = new TemporaryHousingCoordinatonDto();
                                if(_existTHC == null)
                                {
                                    temporaryHousingCoordinaton.AuthoDate = dto.CreatedDate.Value;
                                    temporaryHousingCoordinaton.AuthoAcceptanceDate = i.Acceptance.Value;
                                    temporaryHousingCoordinaton.StatusId = i.StatusId.Value;
                                    temporaryHousingCoordinaton.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    temporaryHousingCoordinaton.CreatedBy = i.CreatedBy;
                                    temporaryHousingCoordinaton.CreatedDate = DateTime.Now;
                                    _temporaryHousingCoordinatonRepository.Add(_mapper.Map<TemporaryHousingCoordinaton>(temporaryHousingCoordinaton));
                                }
                                else
                                {
                                    temporaryHousingCoordinaton.Id = _existTHC.Id;
                                    temporaryHousingCoordinaton.AuthoDate = dto.CreatedDate.Value;
                                    temporaryHousingCoordinaton.AuthoAcceptanceDate = i.Acceptance.Value;
                                    temporaryHousingCoordinaton.StatusId = _existTHC.StatusId;
                                    temporaryHousingCoordinaton.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    temporaryHousingCoordinaton.CreatedBy = _existTHC.CreatedBy;
                                    temporaryHousingCoordinaton.CreatedDate = _existTHC.CreatedDate;
                                    temporaryHousingCoordinaton.UpdateBy = i.CreatedBy;
                                    temporaryHousingCoordinaton.UpdatedDate = DateTime.Now;
                                    _temporaryHousingCoordinatonRepository.Update(_mapper.Map<TemporaryHousingCoordinaton>(temporaryHousingCoordinaton), _existTHC.Id);
                                }
                                break;
                            case (18):
                                var _existRFC = _rentalFurnitureCoordinationRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                RentalFurnitureCoordinationDto rentalFurnitureCoordination = new RentalFurnitureCoordinationDto();
                                if(_existRFC == null)
                                {
                                    rentalFurnitureCoordination.AuthoDate = dto.CreatedDate.Value;
                                    rentalFurnitureCoordination.AuthoAcceptanceDate = i.Acceptance.Value;
                                    rentalFurnitureCoordination.StatusId = i.StatusId.Value;
                                    rentalFurnitureCoordination.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    rentalFurnitureCoordination.CreatedBy = i.CreatedBy;
                                    rentalFurnitureCoordination.CreatedDate = DateTime.Now;
                                    _rentalFurnitureCoordinationRepository.Add(_mapper.Map<RentalFurnitureCoordination>(rentalFurnitureCoordination));
                                }
                                else
                                {
                                    rentalFurnitureCoordination.Id = _existRFC.Id;
                                    rentalFurnitureCoordination.AuthoDate = dto.CreatedDate.Value;
                                    rentalFurnitureCoordination.AuthoAcceptanceDate = i.Acceptance.Value;
                                    rentalFurnitureCoordination.StatusId = _existRFC.StatusId;
                                    rentalFurnitureCoordination.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    rentalFurnitureCoordination.CreatedBy = _existRFC.CreatedBy;
                                    rentalFurnitureCoordination.CreatedDate = _existRFC.CreatedDate;
                                    rentalFurnitureCoordination.UpdateBy = i.CreatedBy;
                                    rentalFurnitureCoordination.UpdatedDate = DateTime.Now;
                                    _rentalFurnitureCoordinationRepository.Update(_mapper.Map<RentalFurnitureCoordination>(rentalFurnitureCoordination), _existRFC.Id);
                                }
                                break;
                            case (19):
                                var _existT = _transportationRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                TransportationDto transportation = new TransportationDto();
                                if(_existT == null)
                                {
                                    transportation.AuthoDate = dto.CreatedDate.Value;
                                    transportation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    transportation.StatusId = i.StatusId.Value;
                                    transportation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    transportation.ApplicantId = i.DeliveredTo.Value;
                                    transportation.ProjectFee = i.ProjectedFee;
                                    //transportation.CreatedBy = i.CreatedBy;
                                    //transportation.CreatedDate = DateTime.Now;
                                    _transportationRepository.Add(_mapper.Map<Transportation>(transportation));
                                }
                                else
                                {
                                    transportation.Id = _existT.Id;
                                    transportation.AuthoDate = dto.CreatedDate.Value;
                                    transportation.AuthoAcceptanceDate = i.Acceptance.Value;
                                    transportation.StatusId = _existT.StatusId;
                                    transportation.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    _transportationRepository.Update(_mapper.Map<Transportation>(transportation), _existT.Id);
                                }
                                break;
                            case (20):
                                var _existATS = _airportTransportationServicesRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                AirportTransportationServiceDto airportTransportationService = new AirportTransportationServiceDto();
                                if(_existATS == null)
                                {
                                    airportTransportationService.AuthoDate = dto.CreatedDate.Value;
                                    airportTransportationService.AuthoAcceptanceDate = i.Acceptance.Value;
                                    airportTransportationService.StatusId = i.StatusId.Value;
                                    airportTransportationService.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    airportTransportationService.CreatedBy = i.CreatedBy;
                                    airportTransportationService.CreatedDate = DateTime.Now;
                                    airportTransportationService.ApplicantId = i.DeliveredTo.Value;
                                    airportTransportationService.ProjectFee = i.ProjectedFee;
                                    _airportTransportationServicesRepository.Add(_mapper.Map<AirportTransportationService>(airportTransportationService));
                                }
                                else
                                {
                                    airportTransportationService.Id = _existATS.Id;
                                    airportTransportationService.AuthoDate = dto.CreatedDate.Value;
                                    airportTransportationService.AuthoAcceptanceDate = i.Acceptance.Value;
                                    airportTransportationService.StatusId = _existATS.StatusId;
                                    airportTransportationService.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    airportTransportationService.CreatedBy = _existATS.CreatedBy;
                                    airportTransportationService.CreatedDate = _existATS.CreatedDate;
                                    airportTransportationService.UpdateBy = i.CreatedBy;
                                    airportTransportationService.UpdatedDate = DateTime.Now;
                                    _airportTransportationServicesRepository.Update(_mapper.Map<AirportTransportationService>(airportTransportationService), _existATS.Id);
                                }
                                break;
                            case (21):
                                var _existHF = _homeFindingRepository.Find(x => x.WorkOrderServicesId == i.WorkOrderServiceId.Value);
                                HomeFindingDto homeFinding = new HomeFindingDto();
                                if(_existHF == null)
                                {
                                    homeFinding.AuthoDate = dto.CreatedDate.Value;
                                    homeFinding.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeFinding.StatusId = i.StatusId.Value;
                                    homeFinding.WorkOrderServicesId = i.WorkOrderServiceId.Value;
                                    homeFinding.CreatedBy = i.CreatedBy;
                                    homeFinding.CreatedDate = DateTime.Now;
                                    _homeFindingRepository.Add(_mapper.Map<HomeFinding>(homeFinding));
                                }
                                else
                                {
                                    _existHF.AuthoDate = dto.CreatedDate.Value;
                                    _existHF.AuthoAcceptanceDate = i.Acceptance.Value;
                                    _existHF.UpdateBy = i.CreatedBy;
                                    _existHF.UpdatedDate = DateTime.Now;
                                    _homeFindingRepository.Update(_existHF, _existHF.Id);
                                }
                                break;
                            case (22):
                                var find = _leaseRenewalRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                LeaseRenewalDto leaseRenewal = new LeaseRenewalDto();
                                if(find == null)
                                {
                                    leaseRenewal.AuthoDate = dto.CreatedDate.Value;
                                    leaseRenewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                    leaseRenewal.StatusId = i.StatusId.Value;
                                    leaseRenewal.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    leaseRenewal.CreationBy = i.CreatedBy;
                                    leaseRenewal.CreatedDate = DateTime.Now;
                                    _leaseRenewalRepository.Add(_mapper.Map<LeaseRenewal>(leaseRenewal));
                                }
                                else
                                {
                                    leaseRenewal.Id = find.Id;
                                    leaseRenewal.AuthoDate = dto.CreatedDate.Value;
                                    leaseRenewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                    leaseRenewal.StatusId = find.StatusId.Value;
                                    leaseRenewal.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    leaseRenewal.CreationBy = find.CreationBy;
                                    leaseRenewal.CreatedDate = find.CreatedDate;
                                    leaseRenewal.UpdatedBy = i.CreatedBy;
                                    leaseRenewal.UpdatedDate = DateTime.Now;
                                    _leaseRenewalRepository.Update(_mapper.Map<LeaseRenewal>(leaseRenewal), find.Id);
                                }
                                break;
                            case (23):
                                var _homeSale = _homeSaleRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                HomeSaleDto homeSaleDto = new HomeSaleDto();
                                if(_homeSale == null)
                                {
                                    homeSaleDto.AuthoDate = dto.CreatedDate.Value;
                                    homeSaleDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeSaleDto.StatusId = i.StatusId.Value;
                                    homeSaleDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    homeSaleDto.CreatedBy = i.CreatedBy;
                                    homeSaleDto.CreatedDate = DateTime.Now;
                                    _homeSaleRepository.Add(_mapper.Map<HomeSale>(homeSaleDto));
                                }
                                else
                                {
                                    homeSaleDto.Id = _homeSale.Id;
                                    homeSaleDto.AuthoDate = dto.CreatedDate.Value;
                                    homeSaleDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeSaleDto.StatusId = _homeSale.StatusId;
                                    homeSaleDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    homeSaleDto.CreatedBy = _homeSale.CreatedBy;
                                    homeSaleDto.CreatedDate = _homeSale.CreatedDate;
                                    homeSaleDto.UpdateBy = i.CreatedBy;
                                    homeSaleDto.UpdatedDate = DateTime.Now;
                                    _homeSaleRepository.Update(_mapper.Map<HomeSale>(homeSaleDto), _homeSale.Id);
                                }
                                break;
                            case (24):
                                var _homePurchase = _homePurchaseRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                HomePurchaseDto homePurchaseDto = new HomePurchaseDto();
                                if(_homePurchase == null)
                                {
                                    homePurchaseDto.AuthoDate = dto.CreatedDate.Value;
                                    homePurchaseDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homePurchaseDto.StatusId = i.StatusId.Value;
                                    homePurchaseDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    // homePurchaseDto.CreationBy = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _homePurchaseRepository.Add(_mapper.Map<HomePurchase>(homePurchaseDto));
                                }
                                else
                                {
                                    homePurchaseDto.Id = _homePurchase.Id;
                                    homePurchaseDto.AuthoDate = dto.CreatedDate.Value;
                                    homePurchaseDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homePurchaseDto.StatusId = _homePurchase.StatusId;
                                    homePurchaseDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    // homePurchaseDto.CreationBy = find.CreationBy;
                                    // homePurchaseDto.CreatedDate = find.CreatedDate;
                                    // homePurchaseDto.UpdatedBy = i.CreatedBy;
                                    // homePurchaseDto.UpdatedDate = DateTime.Now;
                                    _homePurchaseRepository.Update(_mapper.Map<HomePurchase>(homePurchaseDto), _homePurchase.Id);
                                }
                                break;
                            case (25):
                                var _propertyManagement = _propertyManagementRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                PropertyManagementDto propertyManagementDto = new PropertyManagementDto();
                                if(_propertyManagement == null)
                                {
                                    propertyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    propertyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    propertyManagementDto.StatusId = i.StatusId.Value;
                                    propertyManagementDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    // homePurchaseDto.CreationBy = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _propertyManagementRepository.Add(_mapper.Map<PropertyManagement>(propertyManagementDto));
                                }
                                else
                                {
                                    propertyManagementDto.Id = _propertyManagement.Id;
                                    propertyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    propertyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    propertyManagementDto.StatusId = _propertyManagement.StatusId.Value;
                                    propertyManagementDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    // homePurchaseDto.CreationBy = find.CreationBy;
                                    // homePurchaseDto.CreatedDate = find.CreatedDate;
                                    // homePurchaseDto.UpdatedBy = i.CreatedBy;
                                    // homePurchaseDto.UpdatedDate = DateTime.Now;
                                    _propertyManagementRepository.Update(_mapper.Map<PropertyManagement>(propertyManagementDto), _propertyManagement.Id);
                                }
                                break;
                            case (26):
                                var _other = _otherRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                OtherDto otherDto = new OtherDto();
                                if(_other == null)
                                {
                                    otherDto.AuthoDate = dto.CreatedDate.Value;
                                    otherDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherDto.StatusId = i.StatusId.Value;
                                    otherDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    otherDto.CreatedBy = i.CreatedBy;
                                    otherDto.CreatedDate = DateTime.Now;
                                    _otherRepository.Add(_mapper.Map<Other>(otherDto));
                                }
                                else
                                {
                                    otherDto.Id = _other.Id;
                                    otherDto.AuthoDate = dto.CreatedDate.Value;
                                    otherDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherDto.StatusId = _other.StatusId;
                                    otherDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    otherDto.CreatedBy = _other.CreatedBy;
                                    otherDto.CreatedDate = _other.CreatedDate;
                                    otherDto.UpdatedBy = i.CreatedBy;
                                    otherDto.UpdatedDate = DateTime.Now;
                                    _otherRepository.Update(_mapper.Map<Other>(otherDto), _other.Id);
                                }
                                break;
                            case (27):
                                    var _tenancyManagement = _tenancyManagementRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                    TenancyManagementDto tenancyManagementDto = new TenancyManagementDto();
                                    if(_tenancyManagement == null)
                                    {
                                        tenancyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                        tenancyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                        tenancyManagementDto.StatusId = i.StatusId.Value;
                                        tenancyManagementDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                        tenancyManagementDto.CreatedBy = i.CreatedBy;
                                        tenancyManagementDto.CreatedDate = DateTime.Now;
                                        _tenancyManagementRepository.Add(_mapper.Map<TenancyManagement>(tenancyManagementDto));
                                    }
                                    else
                                    {
                                        tenancyManagementDto.Id = _tenancyManagement.Id;
                                        tenancyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                        tenancyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                        tenancyManagementDto.StatusId = _tenancyManagement.StatusId;
                                        tenancyManagementDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                        tenancyManagementDto.CreatedBy = _tenancyManagement.CreatedBy;
                                        tenancyManagementDto.CreatedDate = _tenancyManagement.CreatedDate;
                                        tenancyManagementDto.UpdateBy = i.CreatedBy;
                                        tenancyManagementDto.UpdatedDate = DateTime.Now;
                                        _tenancyManagementRepository.Update(_mapper.Map<TenancyManagement>(tenancyManagementDto), _tenancyManagement.Id);
                                    }
                                    break;
                            case (28):
                                var _otherImmigration = _otherRepository.Find(x => x.WorkOrderServices == i.WorkOrderServiceId.Value);
                                OtherDto otherImmigrationDto = new OtherDto();
                                if(_otherImmigration == null)
                                {
                                    otherImmigrationDto.AuthoDate = dto.CreatedDate.Value;
                                    otherImmigrationDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherImmigrationDto.StatusId = i.StatusId.Value;
                                    otherImmigrationDto.WorkOrderServices = i.WorkOrderServiceId.Value;
                                    otherImmigrationDto.CreatedBy = i.CreatedBy;
                                    otherImmigrationDto.CreatedDate = DateTime.Now;
                                    _otherRepository.Add(_mapper.Map<Other>(otherImmigrationDto));
                                }
                                else
                                {
                                    _otherImmigration.Id = _otherImmigration.Id;
                                    _otherImmigration.CreatedBy = _otherImmigration.CreatedBy;
                                    _otherImmigration.CreatedDate = _otherImmigration.CreatedDate;
                                    _otherImmigration.UpdatedBy = i.CreatedBy;
                                    _otherImmigration.UpdatedDate = DateTime.Now;
                                    _otherRepository.Update(_mapper.Map<Other>(_otherImmigration), _otherImmigration.Id);
                                }
                                break;
                        }

                        var existServiceWorkOrder = orders.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == i.Id);
                        if (existServiceWorkOrder != null)
                        {
                            int difference = existServiceWorkOrder.AuthoTime.Value - i.AuthoTime.Value;
                            if (difference != 0)
                            {
                                var request =  _requestAdditionalTimeRepository
                                    .GetAll()
                                    .Where(x =>
                                        x.WorkOrder == i.WorkOrderId && x.Service == i.WorkOrderServiceId)
                                    .OrderByDescending(o=>o.CreatedDate)
                                    .FirstOrDefault();
                                if (request != null) 
                                {
                                    _requestAdditionalTimeRepository.AddExtension(
                                        i.WorkOrderServiceId.Value,
                                        request.CreatedDate.Value, 
                                        i.CategoryId.Value, 
                                        i.CreatedBy.Value,
                                        Math.Abs(difference),
                                        request.CreatedBy.Value);
                                }
                            }
                        }
                        
                        

                    }

                    foreach (var a in service.BundledServicesWorkOrders)
                    {

                        foreach (var i in a.BundledServices)
                        {
                            int _DeliveringIn = _serviceOrderRepository.GetCountryToCountry(i.DeliveringIn.Value);
                            i.DeliveringIn = _DeliveringIn;

                            if (i.Acceptance == null)
                            {
                                i.Acceptance = DateTime.Now;
                            }

                            switch (i.CategoryId)
                            {
                                case (1):
                                    var _exist = _immigrationEntryVisaRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    EntryVisaDto entryVisa = new EntryVisaDto();
                                    if (_exist == null)
                                    {
                                        entryVisa.ApplicantId = i.DeliveredTo.Value;
                                        entryVisa.AuthoDate = dto.CreatedDate.Value;
                                        entryVisa.AuthoAcceptanceDate = i.Acceptance.Value;
                                        entryVisa.StatusId = i.StatusId.Value;
                                        entryVisa.WorkOrderServicesId = i.WorkServicesId.Value;
                                        entryVisa.CreatedBy = i.CreatedBy;
                                        entryVisa.CreatedDate = DateTime.Now;
                                        _immigrationEntryVisaRepository.Add(_mapper.Map<EntryVisa>(entryVisa));
                                    }
                                    else
                                    {
                                        entryVisa.Id = _exist.Id;
                                        entryVisa.ApplicantId = i.DeliveredTo.Value;
                                        entryVisa.AuthoDate = dto.CreatedDate.Value;
                                        entryVisa.AuthoAcceptanceDate = i.Acceptance.Value;
                                        entryVisa.StatusId = _exist.StatusId;
                                        entryVisa.WorkOrderServicesId = i.WorkServicesId.Value;
                                        entryVisa.UpdateBy = i.CreatedBy;
                                        entryVisa.UpdatedDate = DateTime.Now;
                                        entryVisa.CreatedBy = _exist.CreatedBy;
                                        entryVisa.CreatedDate = _exist.CreatedDate;
                                        _immigrationEntryVisaRepository.Update(_mapper.Map<EntryVisa>(entryVisa), _exist.Id);
                                    }
                                    break;
                                case (2):
                                    var _existWP = _workPermitRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    WorkPermitDto workPermit = new WorkPermitDto();
                                    if (_existWP == null)
                                    {
                                        workPermit.ApplicantId = i.DeliveredTo.Value;
                                        workPermit.AuthoDate = dto.CreatedDate.Value;
                                        workPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                        workPermit.StatusId = i.StatusId.Value;
                                        workPermit.WorkOrderServicesId = i.WorkServicesId.Value;
                                        workPermit.CreatedBy = i.CreatedBy;
                                        workPermit.CreatedDate = DateTime.Now;
                                        _workPermitRepository.Add(_mapper.Map<WorkPermit>(workPermit));
                                    }
                                    else
                                    {
                                        workPermit.Id = _existWP.Id;
                                        workPermit.ApplicantId = i.DeliveredTo.Value;
                                        workPermit.AuthoDate = dto.CreatedDate.Value;
                                        workPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                        workPermit.StatusId = _existWP.StatusId;
                                        workPermit.WorkOrderServicesId = i.WorkServicesId.Value;
                                        workPermit.UpdatedBy = i.CreatedBy;
                                        workPermit.UpdatedDate = DateTime.Now;
                                        workPermit.CreatedBy = _existWP.CreatedBy;
                                        workPermit.UpdatedDate = _existWP.UpdatedDate;
                                        _workPermitRepository.Update(_mapper.Map<WorkPermit>(workPermit), _existWP.Id);
                                    }
                                    break;
                                case (4):
                                    var _existRP = _residencyPermitRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    ResidencyPermitDto residencyPermit = new ResidencyPermitDto();
                                    if (_existRP == null)
                                    {
                                        residencyPermit.ApplicantId = i.DeliveredTo.Value;
                                        residencyPermit.AuthoDate = dto.CreatedDate.Value;
                                        residencyPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                        residencyPermit.StatusId = i.StatusId.Value;
                                        residencyPermit.WorkOrderServicesId = i.WorkServicesId.Value;
                                        residencyPermit.CreatedBy = i.CreatedBy;
                                        residencyPermit.CreatedDate = DateTime.Now;
                                        _residencyPermitRepository.Add(_mapper.Map<ResidencyPermit>(residencyPermit));
                                    }
                                    else
                                    {
                                        residencyPermit.Id = _existRP.Id;
                                        residencyPermit.ApplicantId = i.DeliveredTo.Value;
                                        residencyPermit.AuthoDate = dto.CreatedDate.Value;
                                        residencyPermit.AuthoAcceptanceDate = i.Acceptance.Value;
                                        residencyPermit.StatusId = _existRP.StatusId;
                                        residencyPermit.WorkOrderServicesId = i.WorkServicesId.Value;
                                        residencyPermit.CreatedBy = _existRP.UpdatedBy;
                                        residencyPermit.CreatedDate = _existRP.CreatedDate;
                                        residencyPermit.UpdatedBy = i.CreatedBy;
                                        residencyPermit.UpdatedDate = DateTime.Now;
                                        _residencyPermitRepository.Update(_mapper.Map<ResidencyPermit>(residencyPermit), _existRP.Id);
                                    }
                                    break;
                                case (3):
                                    var _existVDR = _visaDeregistrationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    VisaDeregistrationDto visaDeregistration = new VisaDeregistrationDto();
                                    if (_existVDR == null)
                                    {
                                        visaDeregistration.ApplicantId = i.DeliveredTo.Value;
                                        visaDeregistration.AuthoDate = dto.CreatedDate.Value;
                                        visaDeregistration.AuthoAcceptanceDate = i.Acceptance.Value;
                                        visaDeregistration.StatusId = i.StatusId.Value;
                                        visaDeregistration.WorkOrderServicesId = i.WorkServicesId.Value;
                                        visaDeregistration.CreatedBy = i.CreatedBy;
                                        visaDeregistration.CreatedDate = DateTime.Now;
                                        _visaDeregistrationRepository.Add(_mapper.Map<VisaDeregistration>(visaDeregistration));
                                    }
                                    else
                                    {
                                        visaDeregistration.Id = _existVDR.Id;
                                        visaDeregistration.ApplicantId = i.DeliveredTo.Value;
                                        visaDeregistration.AuthoDate = dto.CreatedDate.Value;
                                        visaDeregistration.AuthoAcceptanceDate = i.Acceptance.Value;
                                        visaDeregistration.StatusId = _existVDR.StatusId;
                                        visaDeregistration.WorkOrderServicesId = i.WorkServicesId.Value;
                                        visaDeregistration.CreatedBy = _existVDR.CreatedBy;
                                        visaDeregistration.CreatedDate = _existVDR.CreatedDate;
                                        visaDeregistration.UpdatedBy = i.CreatedBy;
                                        visaDeregistration.UpdatedDate = DateTime.Now;
                                        _visaDeregistrationRepository.Update(_mapper.Map<VisaDeregistration>(visaDeregistration), _existVDR.Id);
                                    }
                                    break;
                                case (8):
                                    var _existR = _renewalRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    RenewalDto renewal = new RenewalDto();
                                    if (_existR == null)
                                    {
                                        renewal.ApplicantId = i.DeliveredTo.Value;
                                        renewal.AuthoDate = dto.CreatedDate.Value;
                                        renewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                        renewal.StatusId = i.StatusId.Value;
                                        renewal.WorkOrderServicesId = i.WorkServicesId.Value;
                                        renewal.CreatedBy = i.CreatedBy;
                                        renewal.CreatedDate = DateTime.Now;
                                        _renewalRepository.Add(_mapper.Map<Renewal>(renewal));
                                    }
                                    else
                                    {
                                        renewal.Id = _existR.Id;
                                        renewal.ApplicantId = i.DeliveredTo.Value;
                                        renewal.AuthoDate = dto.CreatedDate.Value;
                                        renewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                        renewal.StatusId = _existR.StatusId;
                                        renewal.WorkOrderServicesId = i.WorkServicesId.Value;
                                        renewal.CreatedBy = _existR.CreatedBy;
                                        renewal.CreatedDate = _existR.CreatedDate;
                                        renewal.UpdatedBy = i.CreatedBy;
                                        renewal.UpdatedDate = DateTime.Now;
                                        _renewalRepository.Update(_mapper.Map<Renewal>(renewal), _existR.Id);
                                    }
                                    break;
                                case (7):
                                    var _existCA = _corporateAssistanceRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    CorporateAssistanceDto corporateAssistance = new CorporateAssistanceDto();
                                    if (_existCA == null)
                                    {
                                        corporateAssistance.ApplicantId = i.DeliveredTo.Value;
                                        corporateAssistance.AuthoDate = dto.CreatedDate.Value;
                                        corporateAssistance.AuthoAcceptanceDate = i.Acceptance.Value;
                                        corporateAssistance.StatusId = i.StatusId.Value;
                                        corporateAssistance.WorkOrderServicesId = i.WorkServicesId.Value;
                                        corporateAssistance.CreatedBy = i.CreatedBy;
                                        corporateAssistance.CreatedDate = DateTime.Now;
                                        corporateAssistance.ServiceTypeId = i.ServiceTypeId;
                                        _corporateAssistanceRepository.Add(_mapper.Map<CorporateAssistance>(corporateAssistance));
                                    }
                                    else
                                    {
                                        corporateAssistance.Id = _existCA.Id;
                                        corporateAssistance.ApplicantId = i.DeliveredTo.Value;
                                        corporateAssistance.AuthoDate = dto.CreatedDate.Value;
                                        corporateAssistance.AuthoAcceptanceDate = i.Acceptance.Value;
                                        corporateAssistance.StatusId = _existCA.StatusId;
                                        corporateAssistance.WorkOrderServicesId = i.WorkServicesId.Value;
                                        corporateAssistance.CreatedBy = _existCA.CreatedBy;
                                        corporateAssistance.CreatedDate = _existCA.CreatedDate;
                                        corporateAssistance.UpdatedBy = i.CreatedBy;
                                        corporateAssistance.UpdatedDate = DateTime.Now;
                                        corporateAssistance.ServiceTypeId = i.ServiceTypeId;
                                        _corporateAssistanceRepository.Update(_mapper.Map<CorporateAssistance>(corporateAssistance), _existCA.Id);
                                    }
                                    break;
                                case (9):
                                    var _existN = _notificationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    NotificationDto notification = new NotificationDto();
                                    if (_existN == null)
                                    {
                                        notification.ApplicantId = i.DeliveredTo.Value;
                                        notification.AuthoDate = dto.CreatedDate.Value;
                                        notification.AuthoAcceptanceDate = i.Acceptance.Value;
                                        notification.StatusId = i.StatusId.Value;
                                        notification.WorkOrderServicesId = i.WorkServicesId.Value;
                                        notification.CreatedBy = i.CreatedBy;
                                        notification.CreatedDate = DateTime.Now;
                                        _notificationRepository.Add(_mapper.Map<Notification>(notification));
                                    }
                                    else
                                    {
                                        notification.Id = _existN.Id;
                                        notification.ApplicantId = i.DeliveredTo.Value;
                                        notification.AuthoDate = dto.CreatedDate.Value;
                                        notification.AuthoAcceptanceDate = i.Acceptance.Value;
                                        notification.StatusId = _existN.StatusId;
                                        notification.WorkOrderServicesId = i.WorkServicesId.Value;
                                        notification.CreatedBy = _existN.CreatedBy;
                                        notification.CreatedDate = _existN.CreatedDate;
                                        notification.UpdatedBy = i.CreatedBy;
                                        notification.UpdatedDate = DateTime.Now;
                                        _notificationRepository.Update(_mapper.Map<Notification>(notification), _existN.Id);
                                    }
                                    break;
                                case (10):
                                    var _existLR = _legalReviewRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    LegalReviewDto legalReview = new LegalReviewDto();
                                    if (_existLR == null)
                                    {
                                        legalReview.ApplicantId = i.DeliveredTo.Value;
                                        legalReview.AuthoDate = dto.CreatedDate.Value;
                                        legalReview.AuthoAcceptanceDate = i.Acceptance.Value;
                                        legalReview.StatusId = i.StatusId.Value;
                                        legalReview.WorkOrderServicesId = i.WorkServicesId.Value;
                                        legalReview.CreatedBy = i.CreatedBy;
                                        legalReview.CreatedDate = DateTime.Now;
                                        _legalReviewRepository.Add(_mapper.Map<LegalReview>(legalReview));
                                    }
                                    else
                                    {
                                        legalReview.Id = _existLR.Id;
                                        legalReview.ApplicantId = i.DeliveredTo.Value;
                                        legalReview.AuthoDate = dto.CreatedDate.Value;
                                        legalReview.AuthoAcceptanceDate = i.Acceptance.Value;
                                        legalReview.StatusId = _existLR.StatusId;
                                        legalReview.WorkOrderServicesId = i.WorkServicesId.Value;
                                        legalReview.CreatedBy = _existLR.CreatedBy;
                                        legalReview.CreatedDate = _existLR.CreatedDate;
                                        legalReview.UpdatedBy = i.CreatedBy;
                                        legalReview.UpdatedDate = DateTime.Now;
                                        _legalReviewRepository.Update(_mapper.Map<LegalReview>(legalReview), _existLR.Id);
                                    }
                                    break;
                                case (5):
                                    var _existDM = _documentManagementRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    DocumentManagementDto documentManagement = new DocumentManagementDto();
                                    if (_existDM == null)
                                    {
                                        documentManagement.ApplicantId = i.DeliveredTo.Value;
                                        documentManagement.AuthoDate = dto.CreatedDate.Value;
                                        documentManagement.AuthoAcceptanceDate = i.Acceptance.Value;
                                        documentManagement.StatusId = i.StatusId.Value;
                                        documentManagement.WorkOrderServicesId = i.WorkServicesId.Value;
                                        documentManagement.CreatedBy = i.CreatedBy;
                                        documentManagement.CreatedDate = DateTime.Now;
                                        documentManagement.ServiceId = i.ServiceId;
                                        documentManagement.ProjectFee = i.BundledServiceOrder.ProjectedFee;
                                        _documentManagementRepository.Add(_mapper.Map<DocumentManagement>(documentManagement));
                                    }
                                    else
                                    {
                                        documentManagement.Id = _existDM.Id;
                                        documentManagement.ApplicantId = i.DeliveredTo.Value;
                                        documentManagement.AuthoDate = dto.CreatedDate.Value;
                                        documentManagement.AuthoAcceptanceDate = i.Acceptance.Value;
                                        documentManagement.StatusId = _existDM.StatusId;
                                        documentManagement.WorkOrderServicesId = i.WorkServicesId.Value;
                                        documentManagement.CreatedBy = _existDM.CreatedBy;
                                        documentManagement.CreatedDate = _existDM.CreatedDate;
                                        documentManagement.UpdateBy = i.CreatedBy;
                                        documentManagement.UpdatedDate = DateTime.Now;
                                        documentManagement.ServiceId = i.ServiceId;
                                        documentManagement.ProjectFee = i.BundledServiceOrder.ProjectedFee;
                                        _documentManagementRepository.Update(_mapper.Map<DocumentManagement>(documentManagement), _existDM.Id);
                                    }
                                    break;
                                case (6):
                                    var _existLD = _localDocumentationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    LocalDocumentationDto localDocumentation = new LocalDocumentationDto();
                                    if (_existLD == null)
                                    {
                                        localDocumentation.ApplicantId = i.DeliveredTo.Value;
                                        localDocumentation.AuthoDate = dto.CreatedDate.Value;
                                        localDocumentation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        localDocumentation.StatusId = i.StatusId.Value;
                                        localDocumentation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        localDocumentation.CreatedBy = i.CreatedBy;
                                        localDocumentation.CreatedDate = DateTime.Now;
                                        localDocumentation.ServiceId = i.ServiceId;
                                        localDocumentation.ProjectFee = i.BundledServiceOrder.ProjectedFee;
                                        _localDocumentationRepository.Add(_mapper.Map<LocalDocumentation>(localDocumentation));
                                    }
                                    else
                                    {
                                        localDocumentation.Id = _existLD.Id;
                                        localDocumentation.ApplicantId = i.DeliveredTo.Value;
                                        localDocumentation.AuthoDate = dto.CreatedDate.Value;
                                        localDocumentation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        localDocumentation.StatusId = _existLD.StatusId;
                                        localDocumentation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        localDocumentation.CreatedBy = _existLD.CreatedBy;
                                        localDocumentation.CreatedDate = _existLD.CreatedDate;
                                        localDocumentation.ServiceId = i.ServiceId;
                                        localDocumentation.ProjectFee = i.BundledServiceOrder.ProjectedFee;
                                        localDocumentation.UpdateBy = i.CreatedBy;
                                        localDocumentation.UpdatedDate = DateTime.Now;
                                        _localDocumentationRepository.Update(_mapper.Map<LocalDocumentation>(localDocumentation), _existLD.Id);
                                    }
                                    break;
                                case (12):
                                    var _existPO = _localDocumentationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    PredecisionOrientationDto predicisionOrientation = new PredecisionOrientationDto();
                                    if (_existPO == null)
                                    {
                                        predicisionOrientation.AuthoDate = dto.CreatedDate.Value;
                                        predicisionOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        predicisionOrientation.StatusId = i.StatusId.Value;
                                        predicisionOrientation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        predicisionOrientation.CreatedBy = i.CreatedBy;
                                        predicisionOrientation.CreatedDate = DateTime.Now;
                                        predicisionOrientation.Schoolings = new List<SchoolingDto>();
                                        foreach (var d in custom.AssigneeInformations.FirstOrDefault().DependentInformations)
                                        {
                                            if (d.RelationshipId == 2)
                                            {
                                                d.NationalityId = d.NationalityId;
                                                SchoolingDto schooling = new SchoolingDto();
                                                schooling.RelationshipId = d.Id;
                                                schooling.Age = d.Age;
                                                schooling.Birth = d.Birth;
                                                schooling.Comments = d.AditionalComments;
                                                schooling.CurrentGrade = d.CurrentGrade;
                                                schooling.LanguagesSpoken = d.LanguagesId;
                                                schooling.Name = d.Name;
                                                schooling.Nationality = d.NationalityId;
                                                schooling.Active = true;
                                                predicisionOrientation.Schoolings.Add(schooling);
                                            }
                                        }
                                        _predicisionOrientationRepository.Add(_mapper.Map<PredecisionOrientation>(predicisionOrientation));
                                    }
                                    else
                                    {
                                        predicisionOrientation.Id = _existPO.Id;
                                        predicisionOrientation.AuthoDate = dto.CreatedDate.Value;
                                        predicisionOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        predicisionOrientation.StatusId = _existPO.StatusId;
                                        predicisionOrientation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        predicisionOrientation.CreatedBy = _existPO.CreatedBy;
                                        predicisionOrientation.CreatedDate = _existPO.CreatedDate;
                                        predicisionOrientation.UpdateBy = i.CreatedBy;
                                        predicisionOrientation.UpdatedDate = DateTime.Now;
                                        _predicisionOrientationRepository.Update(_mapper.Map<PredecisionOrientation>(predicisionOrientation), _existPO.Id);
                                    }
                                    break;
                                case (13):
                                    var _existAO = _areaOrientationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    AreaOrientationDto areaOrientation = new AreaOrientationDto();
                                    if (_existAO == null)
                                    {
                                        areaOrientation.AuthoDate = dto.CreatedDate.Value;
                                        areaOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        areaOrientation.StatusId = i.StatusId.Value;
                                        areaOrientation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        areaOrientation.CreatedBy = i.CreatedBy;
                                        areaOrientation.CreatedDate = DateTime.Now;
                                        areaOrientation.SchoolingAreaOrientations = new List<SchoolingAreaOrientationDto>();
                                        foreach (var d in custom.AssigneeInformations.FirstOrDefault().DependentInformations)
                                        {
                                            if (d.RelationshipId == 2)
                                            {
                                                d.NationalityId = d.NationalityId;
                                                SchoolingAreaOrientationDto schoolingAreaOrientation = new SchoolingAreaOrientationDto();
                                                schoolingAreaOrientation.RelationshipId = d.Id;
                                                schoolingAreaOrientation.Age = d.Age;
                                                schoolingAreaOrientation.Birth = d.Birth;
                                                schoolingAreaOrientation.Comments = d.AditionalComments;
                                                schoolingAreaOrientation.CurrentGrade = d.CurrentGrade;
                                                List<LanguagesSpokenSchoolingAreaOrientationDto> languages = new List<LanguagesSpokenSchoolingAreaOrientationDto>();
                                                foreach (var o in d.LanguageDependentInformations)
                                                {
                                                    languages.Add(new LanguagesSpokenSchoolingAreaOrientationDto()
                                                    {
                                                        Schooling = 0,
                                                        LanguagesSpoken = o.Language
                                                    });
                                                }
                                                schoolingAreaOrientation.LanguagesSpokenSchoolingAreaOrientations = languages;
                                                schoolingAreaOrientation.Name = d.Name;
                                                schoolingAreaOrientation.Nationality = d.NationalityId;
                                                schoolingAreaOrientation.Sex = d.Sex;
                                                schoolingAreaOrientation.Active = true;
                                                areaOrientation.SchoolingAreaOrientations.Add(schoolingAreaOrientation);
                                            }
                                        }
                                        _areaOrientationRepository.Add(_mapper.Map<AreaOrientation>(areaOrientation));
                                    }
                                    else
                                    {
                                        areaOrientation.Id = _existAO.Id;
                                        areaOrientation.AuthoDate = dto.CreatedDate.Value;
                                        areaOrientation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        areaOrientation.StatusId = _existAO.StatusId;
                                        areaOrientation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        areaOrientation.CreatedBy = _existAO.CreatedBy;
                                        areaOrientation.CreatedDate = _existAO.CreatedDate;
                                        areaOrientation.UpdateBy = i.CreatedBy;
                                        areaOrientation.UpdatedDate = DateTime.Now;
                                        _areaOrientationRepository.Update(_mapper.Map<AreaOrientation>(areaOrientation), _existAO.Id);
                                    }
                                    break;
                                case (14):
                                    var _existS = _settlingInRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    SettlingInDto settlingIn = new SettlingInDto();
                                    if (_existS == null)
                                    {
                                        settlingIn.AuthoDate = dto.CreatedDate.Value;
                                        settlingIn.AuthoAcceptanceDate = i.Acceptance.Value;
                                        settlingIn.StatusId = i.StatusId.Value;
                                        settlingIn.WorkOrderServicesId = i.WorkServicesId.Value;
                                        settlingIn.CreatedBy = i.CreatedBy;
                                        settlingIn.CreatedDate = DateTime.Now;
                                        _settlingInRepository.Add(_mapper.Map<SettlingIn>(settlingIn));
                                    }
                                    else
                                    {
                                        settlingIn.Id = _existS.Id;
                                        settlingIn.AuthoDate = dto.CreatedDate.Value;
                                        settlingIn.AuthoAcceptanceDate = i.Acceptance.Value;
                                        settlingIn.StatusId = _existS.StatusId;
                                        settlingIn.WorkOrderServicesId = i.WorkServicesId.Value;
                                        settlingIn.CreatedBy = _existS.CreatedBy;
                                        settlingIn.CreatedDate = _existS.CreatedDate;
                                        settlingIn.UpdateBy = i.CreatedBy;
                                        settlingIn.UpdatedDate = DateTime.Now;
                                        _settlingInRepository.Update(_mapper.Map<SettlingIn>(settlingIn), _existS.Id);
                                    }
                                    break;
                                case (15):
                                    var _existSS = _schoolingSearchRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    SchoolingSearchDto schoolingSearch = new SchoolingSearchDto();
                                    if (_existSS == null)
                                    {
                                        schoolingSearch.AuthoDate = dto.CreatedDate.Value;
                                        schoolingSearch.AuthoAcceptanceDate = i.Acceptance.Value;
                                        schoolingSearch.StatusId = i.StatusId.Value;
                                        schoolingSearch.WorkOrderServicesId = i.WorkServicesId.Value;
                                        schoolingSearch.CreatedBy = i.CreatedBy;
                                        schoolingSearch.CreatedDate = DateTime.Now;
                                        schoolingSearch.SchoolingInformations = new List<SchoolingInformationDto>();
                                        foreach (var s in custom.AssigneeInformations.FirstOrDefault().DependentInformations)
                                        {
                                            if (s.RelationshipId == 2)
                                            {
                                                s.NationalityId = s.NationalityId;
                                                SchoolingInformationDto schoolingInformation = new SchoolingInformationDto();
                                                schoolingInformation.RelationshipId = s.Id;
                                                schoolingInformation.Age = s.Age;
                                                schoolingInformation.Birth = s.Birth;
                                                schoolingInformation.Comments = s.AditionalComments;
                                                schoolingInformation.CurrentGrade = s.CurrentGrade;
                                                LanguangeSpokenSchoolingInformationDto language = new LanguangeSpokenSchoolingInformationDto();
                                                List<LanguangeSpokenSchoolingInformationDto> languages = new List<LanguangeSpokenSchoolingInformationDto>();
                                                foreach (var o in s.LanguageDependentInformations)
                                                {
                                                    languages.Add(new LanguangeSpokenSchoolingInformationDto()
                                                    {
                                                        LanguageSpoken = o.Language,
                                                        SchoolingInformation = 0
                                                    });
                                                }
                                                schoolingInformation.LanguangeSpokenSchoolingInformations = languages;
                                                schoolingInformation.Sex = s.Sex;
                                                schoolingInformation.Name = s.Name;
                                                schoolingInformation.Nationality = s.NationalityId;
                                                schoolingInformation.Active = true;
                                                schoolingSearch.SchoolingInformations.Add(schoolingInformation);
                                            }
                                        }
                                        _schoolingSearchRepository.Add(_mapper.Map<SchoolingSearch>(schoolingSearch));
                                    }
                                    else
                                    {
                                        schoolingSearch.Id = _existSS.Id;
                                        schoolingSearch.AuthoDate = dto.CreatedDate.Value;
                                        schoolingSearch.AuthoAcceptanceDate = i.Acceptance.Value;
                                        schoolingSearch.StatusId = _existSS.StatusId;
                                        schoolingSearch.WorkOrderServicesId = i.WorkServicesId.Value;
                                        schoolingSearch.CreatedBy = _existSS.CreatedBy;
                                        schoolingSearch.CreatedDate = _existSS.CreatedDate;
                                        schoolingSearch.UpdateBy = i.CreatedBy;
                                        schoolingSearch.UpdatedDate = DateTime.Now;
                                        _schoolingSearchRepository.Update(_mapper.Map<SchoolingSearch>(schoolingSearch), _existSS.Id);
                                    }
                                    break;
                                case (16):
                                    var _existD = _departureRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    DepartureDto departure = new DepartureDto();
                                    if (_existD == null)
                                    {
                                        departure.AuthoDate = dto.CreatedDate.Value;
                                        departure.AuthoAcceptanceDate = i.Acceptance.Value;
                                        departure.StatusId = i.StatusId.Value;
                                        departure.WorkOrderServicesId = i.WorkServicesId.Value;
                                        //departure.CreatedBy = i.CreatedBy;
                                        //departure.CreatedDate = DateTime.Now;
                                        _departureRepository.Add(_mapper.Map<Departure>(departure));
                                    }
                                    else
                                    {
                                        departure.Id = _existD.Id;
                                        departure.AuthoDate = dto.CreatedDate.Value;
                                        departure.AuthoAcceptanceDate = i.Acceptance.Value;
                                        departure.StatusId = _existD.StatusId;
                                        departure.WorkOrderServicesId = i.WorkServicesId.Value;
                                        _departureRepository.Update(_mapper.Map<Departure>(departure), _existD.Id);
                                    }
                                    break;
                                case (17):
                                    var _existTHC = _temporaryHousingCoordinatonRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    TemporaryHousingCoordinatonDto temporaryHousingCoordinaton = new TemporaryHousingCoordinatonDto();
                                    if (_existTHC == null)
                                    {
                                        temporaryHousingCoordinaton.AuthoDate = dto.CreatedDate.Value;
                                        temporaryHousingCoordinaton.AuthoAcceptanceDate = i.Acceptance.Value;
                                        temporaryHousingCoordinaton.StatusId = i.StatusId.Value;
                                        temporaryHousingCoordinaton.WorkOrderServicesId = i.WorkServicesId.Value;
                                        temporaryHousingCoordinaton.CreatedBy = i.CreatedBy;
                                        temporaryHousingCoordinaton.CreatedDate = DateTime.Now;
                                        _temporaryHousingCoordinatonRepository.Add(_mapper.Map<TemporaryHousingCoordinaton>(temporaryHousingCoordinaton));
                                    }
                                    else
                                    {
                                        temporaryHousingCoordinaton.Id = _existTHC.Id;
                                        temporaryHousingCoordinaton.AuthoDate = dto.CreatedDate.Value;
                                        temporaryHousingCoordinaton.AuthoAcceptanceDate = i.Acceptance.Value;
                                        temporaryHousingCoordinaton.StatusId = _existTHC.StatusId;
                                        temporaryHousingCoordinaton.WorkOrderServicesId = i.WorkServicesId.Value;
                                        temporaryHousingCoordinaton.CreatedBy = _existTHC.CreatedBy;
                                        temporaryHousingCoordinaton.CreatedDate = _existTHC.CreatedDate;
                                        temporaryHousingCoordinaton.UpdateBy = i.CreatedBy;
                                        temporaryHousingCoordinaton.UpdatedDate = DateTime.Now;
                                        _temporaryHousingCoordinatonRepository.Update(_mapper.Map<TemporaryHousingCoordinaton>(temporaryHousingCoordinaton), _existTHC.Id);
                                    }
                                    break;
                                case (18):
                                    var _existRFC = _rentalFurnitureCoordinationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    RentalFurnitureCoordinationDto rentalFurnitureCoordination = new RentalFurnitureCoordinationDto();
                                    if (_existRFC == null)
                                    {
                                        rentalFurnitureCoordination.AuthoDate = dto.CreatedDate.Value;
                                        rentalFurnitureCoordination.AuthoAcceptanceDate = i.Acceptance.Value;
                                        rentalFurnitureCoordination.StatusId = i.StatusId.Value;
                                        rentalFurnitureCoordination.WorkOrderServicesId = i.WorkServicesId.Value;
                                        rentalFurnitureCoordination.CreatedBy = i.CreatedBy;
                                        rentalFurnitureCoordination.CreatedDate = DateTime.Now;
                                        _rentalFurnitureCoordinationRepository.Add(_mapper.Map<RentalFurnitureCoordination>(rentalFurnitureCoordination));
                                    }
                                    else
                                    {
                                        rentalFurnitureCoordination.Id = _existRFC.Id;
                                        rentalFurnitureCoordination.AuthoDate = dto.CreatedDate.Value;
                                        rentalFurnitureCoordination.AuthoAcceptanceDate = i.Acceptance.Value;
                                        rentalFurnitureCoordination.StatusId = _existRFC.StatusId;
                                        rentalFurnitureCoordination.WorkOrderServicesId = i.WorkServicesId.Value;
                                        rentalFurnitureCoordination.CreatedBy = _existRFC.CreatedBy;
                                        rentalFurnitureCoordination.CreatedDate = _existRFC.CreatedDate;
                                        rentalFurnitureCoordination.UpdateBy = i.CreatedBy;
                                        rentalFurnitureCoordination.UpdatedDate = DateTime.Now;
                                        _rentalFurnitureCoordinationRepository.Update(_mapper.Map<RentalFurnitureCoordination>(rentalFurnitureCoordination), _existRFC.Id);
                                    }
                                    break;
                                case (19):
                                    var _existT = _transportationRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    TransportationDto transportation = new TransportationDto();
                                    if (_existT == null)
                                    {
                                        transportation.AuthoDate = dto.CreatedDate.Value;
                                        transportation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        transportation.StatusId = i.StatusId.Value;
                                        transportation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        //transportation.CreatedBy = i.CreatedBy;
                                        //transportation.CreatedDate = DateTime.Now;
                                        transportation.ProjectFee = i.BundledServiceOrder.ProjectedFee;
                                        transportation.ApplicantId = i.DeliveredTo.Value;
                                        _transportationRepository.Add(_mapper.Map<Transportation>(transportation));
                                    }
                                    else
                                    {
                                        transportation.Id = _existT.Id;
                                        transportation.AuthoDate = dto.CreatedDate.Value;
                                        transportation.AuthoAcceptanceDate = i.Acceptance.Value;
                                        transportation.StatusId = _existT.StatusId;
                                        transportation.WorkOrderServicesId = i.WorkServicesId.Value;
                                        _transportationRepository.Update(_mapper.Map<Transportation>(transportation), _existT.Id);
                                    }
                                    break;
                                case (20):
                                    var _existATS = _airportTransportationServicesRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    AirportTransportationServiceDto airportTransportationService = new AirportTransportationServiceDto();
                                    if (_existATS == null)
                                    {
                                        airportTransportationService.AuthoDate = dto.CreatedDate.Value;
                                        airportTransportationService.AuthoAcceptanceDate = i.Acceptance.Value;
                                        airportTransportationService.StatusId = i.StatusId.Value;
                                        airportTransportationService.WorkOrderServicesId = i.WorkServicesId.Value;
                                        airportTransportationService.CreatedBy = i.CreatedBy;
                                        airportTransportationService.CreatedDate = DateTime.Now;
                                        airportTransportationService.ProjectFee = i.BundledServiceOrder.ProjectedFee;
                                        airportTransportationService.ApplicantId = i.DeliveredTo.Value;
                                        _airportTransportationServicesRepository.Add(_mapper.Map<AirportTransportationService>(airportTransportationService));
                                    }
                                    else
                                    {
                                        airportTransportationService.Id = _existATS.Id;
                                        airportTransportationService.AuthoDate = dto.CreatedDate.Value;
                                        airportTransportationService.AuthoAcceptanceDate = i.Acceptance.Value;
                                        airportTransportationService.StatusId = _existATS.StatusId;
                                        airportTransportationService.WorkOrderServicesId = i.WorkServicesId.Value;
                                        airportTransportationService.CreatedBy = _existATS.CreatedBy;
                                        airportTransportationService.CreatedDate = _existATS.CreatedDate;
                                        airportTransportationService.UpdateBy = i.CreatedBy;
                                        airportTransportationService.UpdatedDate = DateTime.Now;
                                        _airportTransportationServicesRepository.Update(_mapper.Map<AirportTransportationService>(airportTransportationService), _existATS.Id);
                                    }
                                    break;
                                case (21):
                                    var _existHF = _homeFindingRepository.Find(x => x.WorkOrderServicesId == i.WorkServicesId.Value);
                                    HomeFindingDto homeFinding = new HomeFindingDto();
                                    if (_existHF == null)
                                    {
                                        homeFinding.AuthoDate = dto.CreatedDate.Value;
                                        homeFinding.AuthoAcceptanceDate = i.Acceptance.Value;
                                        homeFinding.StatusId = i.StatusId.Value;
                                        homeFinding.WorkOrderServicesId = i.WorkServicesId.Value;
                                        homeFinding.CreatedBy = i.CreatedBy;
                                        homeFinding.CreatedDate = DateTime.Now;
                                        _homeFindingRepository.Add(_mapper.Map<HomeFinding>(homeFinding));
                                    }
                                    else
                                    {
                                        _existHF.AuthoAcceptanceDate = i.Acceptance.Value;
                                        _existHF.UpdateBy = i.CreatedBy;
                                        _existHF.UpdatedDate = DateTime.Now;
                                        _homeFindingRepository.Update(_existHF, _existHF.Id);
                                    }
                                    break;
                                case (22):
                                    var find = _leaseRenewalRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                    LeaseRenewalDto leaseRenewal = new LeaseRenewalDto();
                                    if(find == null)
                                    {
                                        leaseRenewal.AuthoDate = dto.CreatedDate.Value;
                                        leaseRenewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                        leaseRenewal.StatusId = i.StatusId.Value;
                                        leaseRenewal.WorkOrderServices = i.WorkServicesId.Value;
                                        leaseRenewal.CreationBy = i.CreatedBy;
                                        leaseRenewal.CreatedDate = DateTime.Now;
                                        _leaseRenewalRepository.Add(_mapper.Map<LeaseRenewal>(leaseRenewal));
                                    }
                                    else
                                    {
                                        leaseRenewal.Id = find.Id;
                                        leaseRenewal.AuthoDate = dto.CreatedDate.Value;
                                        leaseRenewal.AuthoAcceptanceDate = i.Acceptance.Value;
                                        leaseRenewal.StatusId = find.StatusId;
                                        leaseRenewal.WorkOrderServices = i.WorkServicesId.Value;
                                        leaseRenewal.CreationBy = find.CreationBy;
                                        leaseRenewal.CreatedDate = find.CreatedDate;
                                        leaseRenewal.UpdatedBy = i.CreatedBy;
                                        leaseRenewal.UpdatedDate = DateTime.Now;
                                        _leaseRenewalRepository.Update(_mapper.Map<LeaseRenewal >(leaseRenewal), find.Id);
                                    }
                                    break;
                                case (23):
                                var _homeSale = _homeSaleRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                HomeSaleDto homeSaleDto = new HomeSaleDto();
                                if(_homeSale == null)
                                {
                                    homeSaleDto.AuthoDate = dto.CreatedDate.Value;
                                    homeSaleDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeSaleDto.StatusId = i.StatusId.Value;
                                    homeSaleDto.WorkOrderServices = i.WorkServicesId.Value;
                                    homeSaleDto.CreatedBy = i.CreatedBy;
                                    homeSaleDto.CreatedDate = DateTime.Now;
                                    _homeSaleRepository.Add(_mapper.Map<HomeSale>(homeSaleDto));
                                }
                                else
                                {
                                    homeSaleDto.Id = _homeSale.Id;
                                    homeSaleDto.AuthoDate = dto.CreatedDate.Value;
                                    homeSaleDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homeSaleDto.StatusId = _homeSale.StatusId;
                                    homeSaleDto.WorkOrderServices = i.WorkServicesId.Value;
                                    homeSaleDto.CreatedBy = _homeSale.CreatedBy;
                                    homeSaleDto.CreatedDate = _homeSale.CreatedDate;
                                    homeSaleDto.UpdateBy = i.CreatedBy;
                                    homeSaleDto.UpdatedDate = DateTime.Now;
                                    _homeSaleRepository.Update(_mapper.Map<HomeSale>(homeSaleDto), _homeSale.Id);
                                }
                                break;
                            case (24):
                                var _homePurchase = _homePurchaseRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                HomePurchaseDto homePurchaseDto = new HomePurchaseDto();
                                if(_homePurchase == null)
                                {
                                    homePurchaseDto.AuthoDate = dto.CreatedDate.Value;
                                    homePurchaseDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homePurchaseDto.StatusId = i.StatusId.Value;
                                    homePurchaseDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.CreationBy = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _homePurchaseRepository.Add(_mapper.Map<HomePurchase>(homePurchaseDto));
                                }
                                else
                                {
                                    homePurchaseDto.Id = _homePurchase.Id;
                                    homePurchaseDto.AuthoDate = dto.CreatedDate.Value;
                                    homePurchaseDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    homePurchaseDto.StatusId = _homePurchase.StatusId;
                                    homePurchaseDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.CreationBy = find.CreationBy;
                                    // homePurchaseDto.CreatedDate = find.CreatedDate;
                                    // homePurchaseDto.UpdatedBy = i.CreatedBy;
                                    // homePurchaseDto.UpdatedDate = DateTime.Now;
                                    _homePurchaseRepository.Update(_mapper.Map<HomePurchase>(homePurchaseDto), _homePurchase.Id);
                                }
                                break;
                            case (25):
                                var _propertyManagement = _propertyManagementRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                PropertyManagementDto propertyManagementDto = new PropertyManagementDto();
                                if(_propertyManagement == null)
                                {
                                    propertyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    propertyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    propertyManagementDto.StatusId = i.StatusId.Value;
                                    propertyManagementDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.CreationBy = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _propertyManagementRepository.Add(_mapper.Map<PropertyManagement>(propertyManagementDto));
                                }
                                else
                                {
                                    propertyManagementDto.Id = _propertyManagement.Id;
                                    propertyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    propertyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    propertyManagementDto.StatusId = _propertyManagement.StatusId;
                                    propertyManagementDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.CreationBy = find.CreationBy;
                                    // homePurchaseDto.CreatedDate = find.CreatedDate;
                                    // homePurchaseDto.UpdatedBy = i.CreatedBy;
                                    // homePurchaseDto.UpdatedDate = DateTime.Now;
                                    _propertyManagementRepository.Update(_mapper.Map<PropertyManagement>(propertyManagementDto), _propertyManagement.Id);
                                }
                                break;
                            case (26):
                                var _other = _otherRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                OtherDto otherDto = new OtherDto();
                                if(_other == null)
                                {
                                    otherDto.AuthoDate = dto.CreatedDate.Value;
                                    otherDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherDto.StatusId = i.StatusId.Value;
                                    otherDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.CreationBy = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _otherRepository.Add(_mapper.Map<Other>(otherDto));
                                }
                                else
                                {
                                    otherDto.Id = _other.Id;
                                    otherDto.AuthoDate = dto.CreatedDate.Value;
                                    otherDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherDto.StatusId = _other.StatusId;
                                    otherDto.WorkOrderServices = i.WorkServicesId.Value;
                                    otherDto.CreatedBy = _other.CreatedBy;
                                    otherDto.CreatedDate = _other.CreatedDate;
                                    otherDto.UpdatedBy = i.CreatedBy;
                                    otherDto.UpdatedDate = DateTime.Now;
                                    _otherRepository.Update(_mapper.Map<Other>(otherDto), _other.Id);
                                }
                                break;
                            case (27):
                                var _tenancyManagement = _tenancyManagementRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                TenancyManagementDto tenancyManagementDto = new TenancyManagementDto();
                                if(_tenancyManagement == null)
                                {
                                    tenancyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    tenancyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    tenancyManagementDto.StatusId = i.StatusId.Value;
                                    tenancyManagementDto.WorkOrderServices = i.WorkServicesId.Value;
                                    tenancyManagementDto.CreatedBy = i.CreatedBy;
                                    tenancyManagementDto.CreatedDate = DateTime.Now;
                                    _tenancyManagementRepository.Add(_mapper.Map<TenancyManagement>(tenancyManagementDto));
                                }
                                else
                                {
                                    tenancyManagementDto.Id = _tenancyManagement.Id;
                                    tenancyManagementDto.AuthoDate = dto.CreatedDate.Value;
                                    tenancyManagementDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    tenancyManagementDto.StatusId = _tenancyManagement.StatusId;
                                    tenancyManagementDto.WorkOrderServices = i.WorkServicesId.Value;
                                    tenancyManagementDto.CreatedBy = _tenancyManagement.CreatedBy;
                                    tenancyManagementDto.CreatedDate = _tenancyManagement.CreatedDate;
                                    tenancyManagementDto.UpdateBy = i.CreatedBy;
                                    tenancyManagementDto.UpdatedDate = DateTime.Now;
                                    _tenancyManagementRepository.Update(_mapper.Map<TenancyManagement>(tenancyManagementDto), _tenancyManagement.Id);
                                }
                                break;
                            case (28):
                                var _otherImmigration = _otherRepository.Find(x => x.WorkOrderServices == i.WorkServicesId.Value);
                                OtherDto otherimmigrationDto = new OtherDto();
                                if(_otherImmigration == null)
                                {
                                    otherimmigrationDto.AuthoDate = dto.CreatedDate.Value;
                                    otherimmigrationDto.AuthoAcceptanceDate = i.Acceptance.Value;
                                    otherimmigrationDto.StatusId = i.StatusId.Value;
                                    otherimmigrationDto.WorkOrderServices = i.WorkServicesId.Value;
                                    // homePurchaseDto.CreationBy = i.CreatedBy;
                                    // homePurchaseDto.CreatedDate = DateTime.Now;
                                    _otherRepository.Add(_mapper.Map<Other>(otherimmigrationDto));
                                }
                                else
                                {
                                    _otherImmigration.UpdatedBy = i.CreatedBy;
                                    _otherImmigration.UpdatedDate = DateTime.Now;
                                    _otherRepository.Update(_mapper.Map<Other>(_otherImmigration), _otherImmigration.Id);
                                }
                                break;
                            }
                            
                            var existServiceWorkOrder = order.BundledServicesWorkOrders.FirstOrDefault(x => x.Id == i.Id);
                            if (existServiceWorkOrder != null)
                            {
                                int difference = Int32.Parse(existServiceWorkOrder.TotalTime) - Int32.Parse(a.TotalTime);
                                if (difference != 0)
                                {
                                    var request =  _requestAdditionalTimeRepository
                                        .GetAll()
                                        .Where(x =>
                                            x.WorkOrder == a.WorkOrderId && x.Service == i.WorkServicesId)
                                        .OrderByDescending(o=>o.CreatedDate)
                                        .FirstOrDefault();
                                    if (request != null)
                                    {
                                        _requestAdditionalTimeRepository.AddExtension(
                                            i.WorkServicesId.Value, 
                                            request.CreatedDate.Value,
                                            i.CategoryId.Value, 
                                            i.CreatedBy.Value, 
                                            difference, 
                                            request.CreatedBy.Value
                                            );
                                    }
                                }
                            }
                        }
                    }
                    response.Result = _mapper.Map<WorkOrderDto>(service);
                    _serviceOrderRepository.EndTransaction();
                }
                else
                {
                    response.Success = false;
                    response.Message = "Service Record Not Found";
                }
            }
            catch (Exception ex)
            {
                _serviceOrderRepository.RollBack();
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpDelete("DeleteOrder", Name = "DeleteOrder")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<WorkOrderDto>> DeleteOrder([FromQuery] int id)
        {
            int statusCode = 201;
            var response = new ApiResponse<WorkOrderDto>();
            try
            {
                var order = _serviceOrderRepository.Find(c => c.Id == id);
                if (order != null)
                {
                    var res = _serviceOrderRepository.Delete(_mapper.Map<WorkOrder>(order));
                    if (res)
                    {
                        statusCode = 201;
                        response.Success = true;
                        response.Message = "Work Order delete success";    
                    }
                    else
                    {
                        statusCode = 409;
                        response.Success = false;
                        response.Message = $"Some services in {order.NumberWorkOrder} already started.";
                    }
                    
                }
                else
                {
                    response.Success = false;
                    response.Message = "Work Order Not Found";
                }
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
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(statusCode, response);
        }

        [HttpDelete("DeleteStandaloneServiceWorkOrder", Name = "DeleteStandaloneServiceWorkOrder")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteStandaloneServiceWorkOrder([FromQuery] int id)
        {
            try
            {
                Boolean result = _serviceOrderRepository.DeleteStandStandaloneServiceWorkOrder(id);
                if (result)
                {
                    return Ok(new { Success = true, Result = "Standalone Service Order delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Standalone Service Order Not Found", });
                }
                //response.Result = _mapper.Map<ServiceOrderDto>(service);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Standalone Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpDelete("DeleteBundledService", Name = "DeleteBundledService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteBundledService([FromQuery] int id)
        {
            try
            {
                Boolean result = _serviceOrderRepository.DeleteBundledService(id);
                if (result)
                {
                    return Ok(new { Success = true, Result = "Bundled Service Order delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Bundled Service Order Not Found", });
                }
                //response.Result = _mapper.Map<ServiceOrderDto>(service);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Bundled Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetOrders", Name = "GetOrders")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOrders([FromQuery] int sr)
        {
            try
            {
                var service = _serviceRecordRepository.Find(c => c.Id == sr);
                if (service != null)
                {
                    var result = _serviceOrderRepository.GetOrders(sr, null);
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Service Record Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetOrderById", Name = "GetOrderById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOrder([FromQuery] int so)
        {
            try
            {
                var order = _serviceOrderRepository.Find(c => c.Id == so);
                if (order != null)
                {
                    var result = _serviceOrderRepository.GetOrderByWo(so);
                    return Ok(new { Success = true, Result = result });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Service Order Not Found" });//BadRequest(new { Success = false, Result = "Service Order Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetServiceStandalone", Name = "GetServiceStandalone")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetServiceStandalone(int SO)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceOrderRepository.GetServiceStandalone(SO) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetServicePackage", Name = "GetServicePackage")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetServicePackage(int SO)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceOrderRepository.GetServicePackage(SO) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetServiceAllService", Name = "GetServiceAllService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetServiceAllService(int serviceLineId, int serviceRecordId, int? service_type_id, int? status_id, DateTime? date_in, DateTime? date_last)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceOrderRepository.GetServiceAllService(serviceLineId, serviceRecordId, service_type_id, status_id, date_in, date_last) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetBundledService", Name = "GetBundledService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetBundledService([FromQuery]int wo)
        {
            try
            {
                return Ok(new { Success = true, Result = _serviceOrderRepository.GetPackage(wo) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }


        [HttpGet("GetBundledServiceByBundleId", Name = "GetBundledServiceByBundleId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetBundledServiceByBundleId([FromQuery] int bundle_swo_id)
        {
            try
            {
                var result = _serviceOrderRepository.GetBundledServiceByBundleId(bundle_swo_id);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpPost("PostAuthorizedTime/{wo}/{service}/{time}/{userRequest}/{userAccpet}", Name = "PostAuthorizedTime/{wo}/{service}/{time}/{userRequest}/{userAccpet}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult PostAuthorizedTime(int wo, int service, int time, int userAccpet, int userRequest)
        {
            try
            {
                var res = _serviceOrderRepository.AddAdditionalTime(wo, service, time, userAccpet, userRequest);
                return Ok(new { Success = res.Item1, Result = res.Item2 });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Something wnet wrong", Message = ex.ToString() });
            }
        }

        [HttpPost("GetCommentsHostory/{sr}/", Name = "GetCommentsHostory/{sr}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetCommentsHostory(int sr)
        {
            try
            {
                var res = _serviceOrderRepository.GetCommentsHistory(sr);
                return Ok(new { Success = true, Result = res });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Something wnet wrong", Message = ex.ToString() });
            }
        }

        [HttpGet("GetDeliverTo", Name = "GetDeliverTo")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetDeliverTo([FromQuery] int wos)
        {
            try
            {
                var res = _serviceOrderRepository.GetDeliverTo(wos);
                return Ok(new { Success = true, Result = res });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Something wnet wrong", Message = ex.ToString() });
            }
        }

    }
}
