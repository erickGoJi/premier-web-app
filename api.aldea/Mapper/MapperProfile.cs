using AutoMapper;
using api.premier.Models;
using biz.premier.Entities;
using api.premier.Models.Catalogue;
using api.premier.Models.Catalogos;
using api.premier.Models.ServiceRecord;
using biz.premier.Paged;
using api.premier.Models.AssigneeInformation;
using api.premier.Models.ServiceOrder;
using api.premier.Models.PackageServices;
using api.premier.Models.StandaloneServices;
using api.premier.Models.DependentInformations;
using api.premier.Models.Pet;
using api.premier.Models.ImmigrationCoodinator;
using api.premier.Models.ImmigrationSupplierPartner;
using api.premier.Models.RelocationCoordinator;
using api.premier.Models.RelocationSupplierPartner;
using api.premier.Models.RequestInformation;
using api.premier.Models.RequestInformationDocuments;
using api.premier.Models.HousingSpecification;
using api.premier.Models.AssigmentInformation;
using api.premier.Models.DependentImmigrationInfo;
using api.premier.Models.EducationalBackground;
using api.premier.Models.ImmigrationProfile;
using api.premier.Models.LenguageProficiency;
using api.premier.Models.PassportInformation;
using api.premier.Models.PreviousHostCountry;
using api.premier.Models.AssignedServiceRelocation;
using api.premier.Models.AssignedServiceImmigration;
using api.premier.Models.DependentInformationLanguage;
using api.premier.Models.Follow;
using api.premier.Models.Scalate;
using api.premier.Models.AssignedServiceSuplier;
using api.premier.Models.ServiceOrderService;
using api.premier.Models.EntryVisa;
using api.premier.Models.WorkPermit;
using api.premier.Models.ResidencyPermit;
using api.premier.Models.VisaDeregistration;
using api.premier.Models.Appointment;
using api.premier.Models.CorporateAssistance;
using api.premier.Models.Renewal;
using api.premier.Models.Notification;
using api.premier.Models.LegalReview;
using api.premier.Models.DocumentManagement;
using api.premier.Models.LocalDocumentation;
using api.premier.Models.PredicisionOrientation;
using api.premier.Models.AreaOrientation;
using api.premier.Models.SettlingIn;
using api.premier.Models.SchoolingSearch;
using api.premier.Models.Departure;
using api.premier.Models.TemporaryHousingCoordinaton;
using api.premier.Models.ChatImmigrationRelocation;
using api.premier.Models.RentalFurnitureCoordination;
using api.premier.Models.Transportation;
using api.premier.Models.Call;
using api.premier.Models.EmailSend;
using api.premier.Models.AirportTransportationService;
using api.premier.Models.HomeFinding;
using biz.premier.Models.Call;
using api.premier.Models.Task;
using api.premier.Models.MapIt;
using api.premier.Models.AssigneeFeedback;
using api.premier.Models.Country;
using api.premier.Models.PropertyReport;
using api.premier.Models.WorkOrder;
using api.premier.Models.RequestPayment;
using System.Collections.Generic;
using api.premier.Models.Catalogs;
using api.premier.Models.ExperienceSurvey;
using biz.premier.Models;
using api.premier.Models.ReportDay;
using api.premier.Models.RequestAdditionalTime;
using api.premier.Models.SupplierPartnerProfileService;
using api.premier.Models.SupplierPartnerProfileConsultant;
using api.premier.Models.ProfileUser;
using api.premier.Models.Conversation;
using api.premier.Models.NotificationSystem;
using api.premier.Models.Coordinator;
using api.premier.Models.ClientPartnerProfile;
using api.premier.Models.Countries;
using api.premier.Models.Email;
using api.premier.Models.HomePurchase;
using api.premier.Models.HomeSale;
using api.premier.Models.Permit;
using api.premier.Models.Service;
using api.premier.Models.Report;
using api.premier.Models.Invoice;
using api.premier.Models.LeaseRenewal;
using api.premier.Models.Other;
using api.premier.Models.PostIt;
using api.premier.Models.PropertyManagement;
using api.premier.Models.TenancyManagement;
using api.premier.Models.Training;

namespace api.premier.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region User
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserCreateDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            #endregion
            
            #region CatCity
            CreateMap<CatCity, CatCityDto>().ReverseMap();
            CreateMap<CityAbout, CityAboutDto>().ReverseMap();
            CreateMap<PhotoCityAbout, PhotoCityAboutDto>().ReverseMap();
            CreateMap<CityAttraction, CityAttractionDto>().ReverseMap();
            CreateMap<PhotoCityAttraction, PhotoCityAttractionDto>().ReverseMap();
            CreateMap<CityEmergency, CityEmergencyDto>().ReverseMap();
            CreateMap<PhotoCityEmergency, PhotoCityEmergencyDto>().ReverseMap();
            CreateMap<CityWhatToDo, CityWhatToDoDto>().ReverseMap();
            CreateMap<PhotoWhatToDo, PhotoWhatToDoDto>().ReverseMap();
            CreateMap<CityWhereEat, CityWhereEatDto>().ReverseMap();
            CreateMap<PhotoWhereEat, PhotoWhereEatDto>().ReverseMap();
            #endregion

            #region CatCountry
            CreateMap<CatCountry, CatCountryDto>().ReverseMap();
            CreateMap<CountryDocumentGroup, CountryDocumentGroupDto>().ReverseMap();
            CreateMap<CountryLeader, CountryLeaderDto>().ReverseMap();
            CreateMap<CountryDocument, CountryDocumentDto>().ReverseMap();
            #endregion
            
            #region CatStatus
            CreateMap<CatStatus, CatStatusDto>().ReverseMap();
            #endregion
            
            #region CatClient
            CreateMap<CatClient, CatClientDto>().ReverseMap();
            #endregion
            
            #region ServiceRecord
            CreateMap<ServiceRecord, ServiceRecordDto>().ReverseMap();
            CreateMap<ServiceRecord, ServiceRecordTaskDto>().ReverseMap();
            CreateMap<PagedList<ServiceRecord>, PagedList<ServiceRecordDto>>().ReverseMap();
            CreateMap<PagedList<ServiceRecord>, PagedList<ServiceRecordSelectDto>>().ReverseMap();
            CreateMap<ServiceRecord, ServiceRecordInsertDto>().ReverseMap();
            CreateMap<ServiceRecord, ServiceRecordSelectDto>().ReverseMap();
            CreateMap<biz.premier.Models.DashboardDto, api.premier.Models.Dashboard.DashboardDto>().ReverseMap();
            #endregion

            #region AssigneeInformation
            CreateMap<AssigneeInformation, AssigneeInformationDto>().ReverseMap();
            CreateMap<AssigneeInformation, AssigneeInformationAppDto>().ReverseMap();
            CreateMap<AssigneeInformation, AssigneeInformationInsertDto>().ReverseMap();
            CreateMap<AssigneeInformation, AssigneeInformationUserEditDto>().ReverseMap();
            CreateMap<CatAssignedService, CatAssignedServiceDto>().ReverseMap();
            CreateMap<LanguagesSpoken, LanguagesSpokenDto>().ReverseMap();
            CreateMap<HistoryHostHome, HistoryHostHomeDto>().ReverseMap();
            CreateMap<NationalityAssigneeInformation, NationalityAssigneeInformationDto>().ReverseMap();
            #endregion
            
            #region CatBreed
            CreateMap<CatBreed, CatBreedDto>().ReverseMap();
            #endregion
            
            #region CatCoordinator
            CreateMap<CatCoordinator, CatCoordinatorDto>().ReverseMap();
            #endregion
            
            #region CatCoordinatorType
            CreateMap<CatCoordinatorType, CatCoordinatorTypeDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatLanguage, CatLanguagesDto>().ReverseMap();
            #endregion

            #region 
            CreateMap<CatPartner, CatPartnerDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatPetType, CatPetTypeDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatRelationship, CatRelationshipDto>().ReverseMap();
            CreateMap<CatRelationshipContact, CatRelationshipContactDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatServiceLine, CatServiceLineDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatSize, CatSizeDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatSupplier, CatSupplierDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatSupplierCompany, CatSupplierCompanyDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatSupplierType, CatSupplierTypeDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatWeightMeasure, CatWeightMeasureDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatAuthorizedBy, CatAuthorizedByDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatMaritalstatus, CatMaritalstatusDto>().ReverseMap();
            #endregion

            CreateMap<CatDocumentType, CatDocumentTypeDto>().ReverseMap();

            CreateMap<WorkOrder, WorkOrderDto>().ReverseMap();
            CreateMap<WorkOrder, WorkOrderInsertDto>().ReverseMap();
            CreateMap<WorkOrder, WorkOrderTaskDto>().ReverseMap();
            
            CreateMap<DependentInformation, DependentInformationDto>().ReverseMap();
            CreateMap<LanguageDependentInformation, LanguageDependentInformationDto>().ReverseMap();
            CreateMap<Pet, PetDto>().ReverseMap();
            CreateMap<ImmigrationCoodinator, ImmigrationCoodinatorDto>().ReverseMap();
            CreateMap<ImmigrationCoodinator, ImmigrationCoodinatorSelectDto>().ReverseMap();
            CreateMap<ImmigrationSupplierPartner, ImmigrationSupplierPartnerDto>().ReverseMap();
            CreateMap<RelocationCoordinator, RelocationCoordinatorDto>().ReverseMap();
            CreateMap<RelocationCoordinator, RelocationCoordinatorSelectDto>().ReverseMap();
            CreateMap<RelocationSupplierPartner, RelocationSupplierPartnerDto>().ReverseMap();
            CreateMap<CatStatusSupplierCoordinator, CatStatusSupplierCoordinatorDto>().ReverseMap();
            CreateMap<CatActionType, CatActionTypeDto>().ReverseMap();
            CreateMap<CatDepartment, CatDepartmentDto>().ReverseMap();
            CreateMap<CatTypeHousing, CatTypeHousingDto>().ReverseMap();
            #region Request Information
            CreateMap<RequestInformation, RequestInformationDto>().ReverseMap();
            CreateMap<RequestInformation, RequestInformationSelectDto>().ReverseMap();
            CreateMap<RequestInformationDocument, RequestInformationDocumentDto>().ReverseMap();
            #endregion

            #region
            CreateMap<CatPolicyType, CatPolicyTypeDto>().ReverseMap();
            #endregion

            #region Immigration Profile
            CreateMap<ImmigrationProfile, ImmigrationProfileInsertDto>().ReverseMap();
            CreateMap<ImmigrationProfile, ImmigrationProfileDto>().ReverseMap();
            #endregion
            #region Assigment Information
            CreateMap<AssigmentInformation, AssigmentInformationDto>().ReverseMap();
            #endregion
            #region Dependent Immigration Info
            CreateMap<DependentImmigrationInfo, DependentImmigrationInfoDto>().ReverseMap();
            CreateMap<DocumentDependentInformation, DocumentDependentInformationDto>().ReverseMap();
            #endregion
            #region Educational Background
            CreateMap<EducationalBackground, EducationalBackgroundDto>().ReverseMap();
            #endregion
            #region Lenguage Proficiency
            CreateMap<LenguageProficiency, LenguageProficiencyDto>().ReverseMap();
            #endregion
            #region Passport Information
            CreateMap<PassportInformation, PassportInformationDto>().ReverseMap();
            #endregion
            #region Previous Host Country
            CreateMap<PreviousHostCountry, PreviousHostCountryDto>().ReverseMap();
            #endregion

            #region Cuestionarios
            CreateMap<HousingSpecification, HousingSpecificationDto>().ReverseMap();
            CreateMap<RelHousingAmenitie, RelHousingAmenitieDto>().ReverseMap();
            CreateMap<HousingSpecification, HousingSpecificationSelectDto>().ReverseMap();
            #endregion

            #region Catalogue Sex
            CreateMap<CatSex, CatSexDto>().ReverseMap();
            #endregion
            #region Relocation AssignedService
            #endregion
            #region Immigration AssignedService
            #endregion
            #region Dependent Information
            //CreateMap<DependentInformationLanguage, DependentInformationLanguageDto>().ReverseMap();
            #endregion
            #region Follow
            CreateMap<Follow, FollowDto>().ReverseMap();
            #endregion
            #region Scalation
            CreateMap<ScalateService, ScalateServiceDto>().ReverseMap();
            CreateMap<ScalateComment, ScalateCommentDto>().ReverseMap();
            CreateMap<ScalateComment, ScalateCommentInsertDto>().ReverseMap();
            CreateMap<ScalateDocument, ScalateDocumentDto>().ReverseMap();
            #endregion
            #region Asssigned Service Suplier
            CreateMap<AssignedServiceSuplier, AssignedServiceSuplierDto>().ReverseMap();
            #endregion
            #region Service Order Service
            CreateMap<WorkOrderService, WorkOrderServiceDto>().ReverseMap();
            #endregion

            #region Entry Visa
            CreateMap<EntryVisa, EntryVisaDto>().ReverseMap();
            CreateMap<EntryVisa, EntryVisaSelectDto>().ReverseMap();
            CreateMap<DocumentEntryVisa, DocumentEntryVisaDto>().ReverseMap();
            CreateMap<DocumentEntryVisa, DocumentEntryVisaSelectDto>().ReverseMap();
            CreateMap<ReminderEntryVisa, ReminderEntryVisaDto>().ReverseMap();
            CreateMap<CommentsEntryVisa, CommentsEntryVisaDto>().ReverseMap();
            #endregion

            #region Work Permit
            CreateMap<WorkPermit, WorkPermitDto>().ReverseMap();
            CreateMap<WorkPermit, WorkPermitSelectDto>().ReverseMap();
            CreateMap<DocumentWorkPermit, DocumentWorkPermitDto>().ReverseMap();
            CreateMap<DocumentWorkPermit, DocumentWorkPermitSelectDto>().ReverseMap();
            CreateMap<ReminderWorkPermit, ReminderWorkPermitDto>().ReverseMap();
            CreateMap<CommentsWorkPermit, CommentsWorkPermitDto>().ReverseMap();
            #endregion

            #region Catalogue
            CreateMap<CatApplicant, CatApplicantDto>().ReverseMap();
            CreateMap<CatTypeService, CatTypeServiceDto>().ReverseMap();
            CreateMap<CatVisaCategory, CatVisaCategoryDto>().ReverseMap();
            CreateMap<CatService, CatServiceDto>().ReverseMap();
            CreateMap<CatDeliviredIn, CatDeliviredDto>().ReverseMap();
            CreateMap<CatCategory, CatCategoryDto>().ReverseMap();
            CreateMap<CatAssitanceWith, CatAssitanceWithDto>().ReverseMap();
            CreateMap<CatTransportType, CatTransportTypeDto>().ReverseMap();
            CreateMap<CatBankAccountType, CatBankAccountTypeDto>().ReverseMap();
            CreateMap<CatPaymetMethod, CatPaymetMethodDto>().ReverseMap();
            CreateMap<CatNotificationType, CatNotificationTypeDto>().ReverseMap();
            CreateMap<CatSchoolStatus, CatSchoolStatusDto>().ReverseMap();
            CreateMap<CatUserType, CatUserTypeDto>().ReverseMap();

            CreateMap<CatSchoolExpensesPayment, CatSchoolExpensesPaymentDto>();
            CreateMap<CatRole, CatRoleDto>().ReverseMap();
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<CatMenu, CatMenuDto>().ReverseMap();
            CreateMap<CatSubMenu, CatSubMenuDto>().ReverseMap();
            CreateMap<CatSection, CatSectionDto>().ReverseMap();
            
            CreateMap<CatOffice, CatOfficeDto>().ReverseMap();
            CreateMap<CatDay, CatDayDto>().ReverseMap();
            CreateMap<CatPurchaseStatus, CatPurchaseStatusDto>().ReverseMap();
            CreateMap<CatTitle, CatTitleDto>().ReverseMap();
            
            CreateMap<Nationality, NationalityDto>().ReverseMap();
            CreateMap<CatStatusTransportPickup, CatStatusTransportPickupDto>().ReverseMap();
            #endregion

            #region Residency Permit
            CreateMap<ResidencyPermit, ResidencyPermitDto>().ReverseMap();
            CreateMap<ResidencyPermit, ResidencyPermitSelectDto>().ReverseMap();
            CreateMap<DocumentResidencyPermit, DocumentResidencyPermitDto>().ReverseMap();
            CreateMap<DocumentResidencyPermit, DocumentResidencyPermitSelectDto>().ReverseMap();
            CreateMap<ReminderResidencyPermit, ReminderResidencyPermitDto>().ReverseMap();
            CreateMap<CommentResidencyPermit, CommentResidencyPermitDto>().ReverseMap();
            #endregion

            #region Visa Deregistration
            CreateMap<VisaDeregistration, VisaDeregistrationDto>().ReverseMap();
            CreateMap<VisaDeregistration, VisaDeregistrationSelcetDto>().ReverseMap();
            CreateMap<DocumentVisaDeregistration, DocumentVisaDeregistrationDto>().ReverseMap();
            CreateMap<DocumentVisaDeregistration, DocumentVisaDeregistrationSelectDto>().ReverseMap();
            CreateMap<ReminderVisaDeregistration, ReminderVisaDeregistrationDto>().ReverseMap();
            CreateMap<CommentVisaDeregistration, CommentVisaDeregistrationDto>().ReverseMap();
            #endregion
            #region Delivired In
            CreateMap<CatDeliviredIn, CatDeliveredInDto>().ReverseMap();
            #endregion
            #region Catalogues
            CreateMap<CatContractType, CatContractTypeDto>().ReverseMap();
            CreateMap<CatMetric, CatMetricDto>().ReverseMap();
            CreateMap<CatAmenitie, CatAmenitieDto>().ReverseMap();
            CreateMap<CatCurrency, CatCurrencyDto>().ReverseMap();
            CreateMap<CatPropertyTypeHousing, CatPropertyTypeHousingDto>().ReverseMap();
            CreateMap<CatHighestLevelEducation, CatHighestLevelEducationDto>().ReverseMap();
            CreateMap<CatProficiency, CatProficiencyDto>().ReverseMap();
            CreateMap<CatDuration, CatDurationDto>().ReverseMap();
            CreateMap<CatGradeSchooling, CatGradeSchoolingDto>().ReverseMap();
            CreateMap<CatPropertyType, CatPropertyTypeDto>().ReverseMap();
            CreateMap<CatPaymentType, CatPaymentTypeDto>().ReverseMap();
            //CreateMap<CatPropertyType, CatPropertyTypeDto>().ReverseMap();
            CreateMap<CatReservationType, CatReservationTypeDto>().ReverseMap();
            CreateMap<CatRepairType, CatRepairTypeDto>().ReverseMap();
            CreateMap<CatRequestType, CatRequestTypeDto>().ReverseMap();
            CreateMap<CatStatusWorkOrder, CatStatusWorkOrderDto>().ReverseMap();
            CreateMap<CatPropertySection, CatPropertySectionDto>().ReverseMap();
            CreateMap<CatStatusPropertySection, CatStatusPropertySectionDto>().ReverseMap();
            CreateMap<CatStatusHousing, CatStatusHousingDto>().ReverseMap();
            CreateMap<CatCreditCard, CatCreditCardDto>().ReverseMap();
            CreateMap<CatLeaseTemplate, CatLeaseTemplateDto>().ReverseMap();
            CreateMap<CatPaymentTypeStatus, CatPaymentTypeStatusDto>().ReverseMap();
            CreateMap<CatResponsablePayment, CatResponsablePaymentDto>().ReverseMap();
            CreateMap<CatBenefit, CatBenefitDto>().ReverseMap();
            CreateMap<CatLeaseGuarantee, CatLeaseGuaranteeDto>().ReverseMap();
            CreateMap<CatTaxePercentage, CatTaxePercentageDto>().ReverseMap();
            CreateMap<CatSupplierPartnerProfileType, CatSupplierPartnerProfileTypeDto>().ReverseMap();
            CreateMap<CatCreditTerm, CatCreditTermDto>().ReverseMap();
            CreateMap<CatSupplierPartnerType, CatSupplierPartnerTypeDto>().ReverseMap();
            CreateMap<CatType, CatTypeDto>().ReverseMap();
            CreateMap<CatPaymentRecurrence, CatPaymentRecurrenceDto>().ReverseMap();
            CreateMap<CatTypeCatalog, CatTypeCatalogDto>().ReverseMap();
            CreateMap<CatCatalog, CatCatalogDto>().ReverseMap();
            #endregion
            #region Corporate Assistance
            CreateMap<CorporateAssistance, CorporateAssistanceDto>().ReverseMap();
            CreateMap<CorporateAssistance, CorporateAssistanceSelectDto>().ReverseMap();
            CreateMap<DocumentCorporateAssistance, DocumentCorporateAssistanceDto>().ReverseMap();
            CreateMap<DocumentCorporateAssistance, DocumentCorporateAssistanceSelectDto>().ReverseMap();
            CreateMap<RemiderCorporateAssistance, RemiderCorporateAssistanceDto>().ReverseMap();
            CreateMap<CommentCorporateAssistance, CommentCorporateAssistanceDto>().ReverseMap();
            CreateMap<CommentCorporateAssistance, CommentCorporateAssistanceSelectDto>().ReverseMap();
            #endregion
            #region Renewal
            CreateMap<Renewal, RenewalDto>().ReverseMap();
            CreateMap<Renewal, RenewalSelectDto>().ReverseMap();
            CreateMap<DocumentRenewal, DocumentRenewalDto>().ReverseMap();
            CreateMap<DocumentRenewal, DocumentRenewalSelectDto>().ReverseMap();
            CreateMap<ReminderRenewal, ReminderRenewalDto>().ReverseMap();
            CreateMap<CommentRenewal, CommentRenewalDto>().ReverseMap();
            #endregion
            #region Notification
            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<Notification, NotificationSelectDto>().ReverseMap();
            CreateMap<DocumentNotification, DocumentNotificationDto>().ReverseMap();
            CreateMap<DocumentNotification, DocumentNotificationSelectDto>().ReverseMap();
            CreateMap<ReminderNotification, ReminderNotificationDto>().ReverseMap();
            CreateMap<CommentNotification, CommentNotificationDto>().ReverseMap();
            #endregion
            #region Legal; Review / Consultation
            CreateMap<LegalReview, LegalReviewDto>().ReverseMap();
            CreateMap<LegalReview, LegalReviewSelectDto>().ReverseMap();
            CreateMap<DocumentLegalReview, DocumentLegalReviewDto>().ReverseMap();
            CreateMap<DocumentLegalReview, DocumentLegalReviewSelectDto>().ReverseMap();
            CreateMap<ReminderLegalReview, ReminderLegalReviewDto>().ReverseMap();
            CreateMap<CommentLegalReview, CommentLegalReviewDto>().ReverseMap();
            #endregion
            #region Document Management
            CreateMap<DocumentManagement, DocumentManagementDto>().ReverseMap();
            CreateMap<List<DocumentManagement>, List<DocumentManagementDto>>().ReverseMap();
            CreateMap<DocumentManagement, DocumentManagementSelectDto>().ReverseMap();
            CreateMap<DocumentDocumentManagement, DocumentDocumentManagementDto>().ReverseMap();
            CreateMap<DocumentDocumentManagement, DocumentDocumentManagementSelectDto>().ReverseMap();
            CreateMap<ReminderDocumentManagement, ReminderDocumentManagementDto>().ReverseMap();
            CreateMap<CommentDocumentManagement, CommentDocumentManagementDto>().ReverseMap();
            #endregion
            #region Local Documentation
            CreateMap<LocalDocumentation, LocalDocumentationDto>().ReverseMap();
            CreateMap<List<LocalDocumentation>, List<LocalDocumentationDto>>().ReverseMap();
            CreateMap<LocalDocumentation, LocalDocumentationSelectDto>().ReverseMap();
            CreateMap<DocumentLocalDocumentation, DocumentLocalDocumentationDto>().ReverseMap();
            CreateMap<DocumentLocalDocumentation, DocumentLocalDocumentationSelectDto>().ReverseMap();
            CreateMap<ReminderLocalDocumentation, ReminderLocalDocumentationDto>().ReverseMap();
            CreateMap<CommentLocalDocumentation, CommentLocalDocumentationDto>().ReverseMap();
            #endregion
            #region Appointment
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<DocumentAppointment, DocumentAppointmentDto>().ReverseMap();
            CreateMap<AppointmentWorkOrderService, AppointmentWorkOrderServiceDto>().ReverseMap();
            #endregion
            #region Predecision Orientation
            CreateMap<PredecisionOrientation, PredecisionOrientationDto>().ReverseMap();
            CreateMap<PredecisionOrientation, PredecisionOrientationSelectDto>().ReverseMap();
            CreateMap<CommentPredecisionOrientation, CommentPredecisionOrientationDto>().ReverseMap();
            CreateMap<CommentPredecisionOrientation, CommentPredecisionOrientationSelectDto>().ReverseMap();
            CreateMap<ReminderPredecisionOrientation, ReminderPredecisionOrientationDto>().ReverseMap();
            CreateMap<DocumentPredecisionOrientation, DocumentPredecisionOrientationDto>().ReverseMap();
            CreateMap<DocumentPredecisionOrientation, DocumentPredecisionOrientationSelectDto>().ReverseMap();
            //CreateMap<HousingPredecisionOrientation, HousingPredecisionOrientationDto>().ReverseMap();
            CreateMap<Schooling, SchoolingDto>().ReverseMap();
            CreateMap<SchoolingListLanguangeDto, SchoolingListLanguangeDto>().ReverseMap();
            //CreateMap<RelHousing, RelHousingDto>().ReverseMap();
            //CreateMap<SchoolPredecisionOrientation, SchoolPredecisionOrientationDto>().ReverseMap();
            CreateMap<ExtensionPredecisionOrientation, ExtensionPredecisionOrientationDto>().ReverseMap();
            #endregion
            #region Area Orientation
            CreateMap<AreaOrientation, AreaOrientationDto>().ReverseMap();
            CreateMap<AreaOrientation, AreaOrientationSelectDto>().ReverseMap();
            CreateMap<ReminderAreaOrientation, ReminderAreaOrientationDto>().ReverseMap();
            CreateMap<DocumentAreaOrientation, DocumentAreaOrientationDto>().ReverseMap();
            CreateMap<DocumentAreaOrientation, DocumentAreaOrientationSelectDto>().ReverseMap();
            //CreateMap<HousingAreaOrientation, HousingAreaOrientationDto>().ReverseMap();
            CreateMap<SchoolingAreaOrientation, SchoolingAreaOrientationDto>().ReverseMap();
            CreateMap<RelHousingAreaOrientation, RelHousingAreaOrientationDto>().ReverseMap();
            CreateMap<CommentAreaOrientation, CommentAreaOrientationDto>().ReverseMap();
            CreateMap<CommentAreaOrientation, CommentAreaOrientationSelectDto>().ReverseMap();
            CreateMap<ExtensionAreaOrientation, ExtensionAreaOrientationDto>().ReverseMap();
            //CreateMap<SchoolAreaOrientation, SchoolAreaOrientationDto>().ReverseMap();
            CreateMap<LanguagesSpokenSchoolingAreaOrientation, LanguagesSpokenSchoolingAreaOrientationDto>().ReverseMap();
            #endregion
            #region Settling In
            CreateMap<DocumentSettlingIn, DocumentSettlingInDto>().ReverseMap();
            CreateMap<DocumentSettlingIn, DocumentSettlingInSelectDto>().ReverseMap();
            CreateMap<ReminderSettlingIn, ReminderSettlingInDto>().ReverseMap();
            CreateMap<SettlingIn, SettlingInDto>().ReverseMap();
            CreateMap<SettlingIn, SettlingInSelectDto>().ReverseMap();
            CreateMap<CommentSettlingIn, CommentSettlingInDto>().ReverseMap();
            CreateMap<CommentSettlingIn, CommentSettlingInSelectDto>().ReverseMap();
            CreateMap<ExtensionSettlingIn, ExtensionSettlingInDto>().ReverseMap();
            #endregion
            #region Schooling Search
            CreateMap<SchoolingSearch, SchoolingSearchDto>().ReverseMap();
            CreateMap<SchoolingSearch, SchoolingSearchSelectDto>().ReverseMap();
            CreateMap<PaymentSchooling, Models.WorkOrder.PaymentSchoolingDto>().ReverseMap();
            CreateMap<DocumentSchoolingSearch, DocumentSchoolingSearchDto>().ReverseMap();
            CreateMap<DocumentSchoolingSearch, DocumentSchoolingSearchSelectDto>().ReverseMap();
            CreateMap<ReminderSchoolingSearch, ReminderSchoolingSearchDto>().ReverseMap();
            CreateMap<SchoolingInformation, SchoolingInformationDto>().ReverseMap();
            CreateMap<PaymentSchoolingInformation, PaymentSchoolingInformationDto>().ReverseMap();
            CreateMap<CommentSchoolingSearch, CommentSchoolingSearchDto>().ReverseMap();
            CreateMap<CommentSchoolingSearch, CommentSchoolingSearchSelectDto>().ReverseMap();
            CreateMap<ExtensionSchoolingSearch, ExtensionSchoolingSearchDto>().ReverseMap();
            CreateMap<LanguangeSpokenSchoolingInformation, LanguangeSpokenSchoolingInformationDto>().ReverseMap();
            #endregion
            #region Departure
            CreateMap<Departure, DepartureDto>().ReverseMap();
            CreateMap<Departure, DepartureSelectDto>().ReverseMap();
            CreateMap<DeparturePayment, DeparturePaymentDto>().ReverseMap();
            CreateMap<DepartureAssistanceWith, DepartureAssistanceWithDto>().ReverseMap();
            CreateMap<DocumentDeparture, DocumentDepartureDto>().ReverseMap();
            CreateMap<DocumentDeparture, DocumentDepartureSelectDto>().ReverseMap();
            CreateMap<DocumentRepair, DocumentRepairDto>().ReverseMap();
            CreateMap<ReminderDeparture, ReminderDepartureDto>().ReverseMap();
            CreateMap<ExtensionDeparture, ExtensionDepartureDto>().ReverseMap();
            CreateMap<CommentDeparture, CommentDepartureDto>().ReverseMap();
            CreateMap<CommentDeparture, CommentDepartureSelectDto>().ReverseMap();
            CreateMap<DepartureCostSaving, DepartureCostSavingDto>().ReverseMap();
            #endregion
            #region Temporary
            CreateMap<TemporaryHousingCoordinaton, TemporaryHousingCoordinatonDto>().ReverseMap();
            CreateMap<TemporaryHousingCoordinaton, TemporaryHousingCoordinatonSelectDto>().ReverseMap();
            CreateMap<DocumentTemporaryHousingCoordinaton, DocumentTemporaryHousingCoordinatonDto>().ReverseMap();
            CreateMap<ExtensionTemporaryHousingCoordinaton, ExtensionTemporaryHousingCoordinatonDto>().ReverseMap();
            CreateMap<ReminderTemporaryHousingCoordinaton, ReminderTemporaryHousingCoordinatonDto>().ReverseMap();
            CreateMap<StayExtensionTemporaryHousing, StayExtensionTemporaryHousingDto>().ReverseMap();
            CreateMap<CommentTemporaryHosuing, CommentTemporaryHosuingDto>().ReverseMap();
            CreateMap<CommentTemporaryHosuing, CommentTemporaryHosuingSelectDto>().ReverseMap();
            #endregion

            #region Chat
            CreateMap<ChatConversationImmigrationRelocation, ChatConversationImmigrationRelocationDto>().ReverseMap();
            CreateMap<ChatImmigrationRelocation, ChatImmigrationRelocationDto>().ReverseMap();
            CreateMap<ChatDocumentImmigrationRelocation, ChatDocumentImmigrationRelocationDto>().ReverseMap();
            #endregion
            #region Rental Furniture
            CreateMap<RentalFurnitureCoordination, RentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<PaymentRental, PaymentRentalDto>().ReverseMap();
            CreateMap<RentalFurnitureCoordination, RentalFurnitureCoordinationSelectDto>().ReverseMap();
            CreateMap<DocumentRentalFurnitureCoordination, DocumentRentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<ExtensionRentalFurnitureCoordination, ExtensionRentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<PaymentsRentalFurnitureCoordination, PaymentsRentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<ReminderRentalFurnitureCoordination, ReminderRentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<StayExtensionRentalFurnitureCoordination, StayExtensionRentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<CommentRentalFurnitureCoordination, CommentRentalFurnitureCoordinationDto>().ReverseMap();
            CreateMap<CommentRentalFurnitureCoordination, CommentRentalFurnitureCoordinationSelectDto>().ReverseMap();
            #endregion

            #region Transportation
            CreateMap<Transportation, TransportationDto>().ReverseMap();
            CreateMap<Transportation, TransportationSelectDto>().ReverseMap();
            CreateMap<Transportation, CreateTransportationDto>().ReverseMap();
            CreateMap<TransportPickup, TransportPickupDto>().ReverseMap();
            CreateMap<FamilyMemberTransportation, FamilyMemberTransportationSelectDto>().ReverseMap();
            CreateMap<FamilyMemberTransportation, FamilyMemberTransportationDto>().ReverseMap();
            CreateMap<CommentTransportation, CommentTransportationDto>().ReverseMap();
            CreateMap<CommentTransportation, CommentTransportationSelectDto>().ReverseMap();
            CreateMap<DocumentTransportation, DocumentTransportationDto>().ReverseMap();
            CreateMap<PaymentTransportation, PaymentTransportationDto>().ReverseMap();
            CreateMap<ReminderTransportation, ReminderTransportationDto>().ReverseMap();
            CreateMap<ExtensionTransportation, ExtensionTransportationDto>().ReverseMap();
            #endregion

            CreateMap<Call, CallDto>().ReverseMap();
            CreateMap<CallAssistant, CallAssistantDto>().ReverseMap();
            CreateMap<api.premier.Models.Call.CallByServiceRecord, biz.premier.Models.Call.CallByServiceRecord>().ReverseMap();
            CreateMap<EmailSend, EmailSendDto>().ReverseMap();

            #region Airport Transportation Service
            CreateMap<AirportTransportationService, AirportTransportationServiceDto>().ReverseMap();
            CreateMap<AirportTransportationService, AirportTransportationServiceSelectDto>().ReverseMap();
            CreateMap<AirportTransportationService, CreateAirportTransportationDto>().ReverseMap();
            CreateMap<DocumentAirportTransportationService, DocumentAirportTransportationServiceDto>().ReverseMap();
            CreateMap<PaymentAirportTransportationService, PaymentAirportTransportationServiceDto>().ReverseMap();
            CreateMap<ReminderAirportTransportationService, ReminderAirportTransportationServiceDto>().ReverseMap();
            //CreateMap<TransportService, TransportServiceDto>().ReverseMap();
            CreateMap<FamilyMemberTransportService, FamilyMemberTransportServiceDto>().ReverseMap();
            CreateMap<ExtensionAirportTransportationService, ExtensionAirportTransportationServiceDto>().ReverseMap();
            CreateMap<CommentAirportTransportationService, CommentAirportTransportationServiceDto>().ReverseMap();
            CreateMap<CommentAirportTransportationService, CommentAirportTransportationServiceSelectDto>().ReverseMap();
            CreateMap<AirportTransportPickup, AirportTransportPickupDto>().ReverseMap();
            #endregion
            #region Home Finding
            CreateMap<HomeFinding, HomeFindingDto>().ReverseMap();
            CreateMap<HomeFinding, HomeFindingSelectDto>().ReverseMap();
            CreateMap<CostSaving, CostSavingDto>().ReverseMap();
            CreateMap<CostSavingDetail, CostSavingDetailDto>().ReverseMap();
            CreateMap<DepartureDetail, DepartureDetailDto>().ReverseMap();
            //CreateMap<HousingHomeFinding, HousingHomeFindingDto>().ReverseMap();
            CreateMap<IncludedRent, IncludedRentDto>().ReverseMap();
            CreateMap<LandLordBankDetail, LandLordBankDetailDto>().ReverseMap();
            CreateMap<LandLordBank, LandLordBankDto>().ReverseMap();
            CreateMap<LandlordDetail, LandlordDetailDto>().ReverseMap();
            CreateMap<PaymentHomeFinding, PaymentHomeFindingDto>().ReverseMap();
            CreateMap<PermanentHome, PermanentHomeDto>().ReverseMap();
            //CreateMap<RelHousingHomeFinding, RelHousingHomeFindingDto>().ReverseMap();
            CreateMap<ReminderHomeFinding, ReminderHomeFindingDto>().ReverseMap();
            CreateMap<RenewalDetail, RenewalDetailDto>().ReverseMap();
            CreateMap<DocumentHomeFinding, DocumentHomeFindingDto>().ReverseMap();
            CreateMap<DocumentHomeFinding, DocumentHomeFindingSelectDto>().ReverseMap();
            CreateMap<CommentHomeFinding, CommentHomeFindingDto>().ReverseMap();
            CreateMap<CommentHomeFinding, CommentHomeFindingSelectDto>().ReverseMap();
            CreateMap<ExtensionHomeFinding, ExtensionHomeFindingDto>().ReverseMap();
            CreateMap<DocumentRepairHomeFinding, DocumentRepairHomeFindingDto>().ReverseMap();
            CreateMap<InspectionHomeFinding, InspectionHomeFindingDto>().ReverseMap();
            CreateMap<HomeFindingRepair, HomeFindingRepairDto>().ReverseMap();
            #endregion

            #region Task
            CreateMap<Task, TaskDto>().ReverseMap();
            CreateMap<Task, TaskSelectDto>().ReverseMap();
            CreateMap<TaskDocument, TaskDocumentDto>().ReverseMap();
            CreateMap<TaskReply, TaskReplyDto>().ReverseMap();
            CreateMap<TaskReply, TaskReplySelectDto>().ReverseMap();
            CreateMap<TaskStatus, TaskStatusDto>().ReverseMap();
            CreateMap<ColaboratorMember, ColaboratorMemberDto>().ReverseMap();
            #endregion
            #region MapIt
            CreateMap<MapIt, MapItDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
            #endregion
            #region Assignee FeedBack
            CreateMap<AssigneeFeedback, AssigneeFeedbackDto>().ReverseMap();
            #endregion
            #region Country
            CreateMap<Country1, Models.Country.CountryDto>().ReverseMap();
            CreateMap<CountryGallery, CountryGalleryDto>().ReverseMap();
            #endregion
            #region Property report
            CreateMap<PropertyReport, PropertyReportDto>().ReverseMap();
            CreateMap<PropertyReport, PropertyReportSelectDto>().ReverseMap();
            CreateMap<PropertyReportSection, PropertyReportSectionDto>().ReverseMap();
            CreateMap<PhotosPropertyReportSection, PhotosPropertyReportSectionDto>().ReverseMap();
            CreateMap<SectionInventory, SectionInventoryDto>().ReverseMap();
            CreateMap<PhotosInventory, PhotosInventoryDto>().ReverseMap();
            CreateMap<Attendee, AttendeeDto>().ReverseMap();
            CreateMap<KeyInventory, KeyInventoryDto>().ReverseMap();
            CreateMap<Repair, RepairDto>().ReverseMap();
            CreateMap<Inspection, InspectionDto>().ReverseMap();
            CreateMap<GroupIr, GroupIrDto> ().ReverseMap();
            CreateMap<AttendeeInspec, AttendeeInspecDto>().ReverseMap();
            CreateMap<PhotosInspec, PhotosInspecDto>().ReverseMap();
            #endregion

            #region Work Order
            CreateMap<StandaloneServiceWorkOrder, StandaloneServiceWorkOrderDto>().ReverseMap();
            CreateMap<BundledService, BundledServiceDto>().ReverseMap();
            CreateMap<BundledServicesWorkOrder, BundledServicesWorkOrderDto>().ReverseMap();
            CreateMap<api.premier.Models.WorkOrder.InvoiceDto, biz.premier.Models.InvoiceDto>().ReverseMap();
            #endregion

            #region Request Payment
            CreateMap<RequestPayment, RequestPaymentDto>().ReverseMap();
            CreateMap<RequestPayment, RequestPaymentSelectDto>().ReverseMap();
            CreateMap<CommentRequestPayment, CommentRequestPaymentDto>().ReverseMap();
            CreateMap<CommentRequestPayment, CommentRequestPaymentSelectDto>().ReverseMap();
            CreateMap<DocumentRequestPayment, DocumentRequestPaymentDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Payment, PaymentSelectDto>().ReverseMap();
            CreateMap<Concept, ConceptDto>().ReverseMap();
            CreateMap<RecurrenceRequestPayment, RecurrenceRequestPaymentDto>().ReverseMap();
            CreateMap<RepeatTheRecurrenceRequestPayment, RepeatTheRecurrenceRequestPaymentDto>().ReverseMap();
            CreateMap<CommentPaymentConcept, CommentPaymentConceptDto>().ReverseMap();
            CreateMap<CommentPaymentConcept, CommentPaymentConceptSelectDto>().ReverseMap();
            CreateMap<DocumentPaymentConcept, DocumentPaymentConceptDto>().ReverseMap();
            CreateMap<RecurrencePaymentConcept, RecurrencePaymentConceptDto>().ReverseMap();
            CreateMap<RepeatThePaymentConcept, RepeatThePaymentConceptDto>().ReverseMap();
            CreateMap<WireTransferPaymentConcept, WireTransferPaymentConceptDto>().ReverseMap();
            CreateMap<WireTransferServicePaymentConcept, WireTransferServicePaymentConceptDto>().ReverseMap();
            CreateMap<WireTransferServicePaymentConcept, WireTransferServicePaymentConceptSelectDto>().ReverseMap();
            CreateMap<CatRequestPaymentStatus, CatRequestPaymentStatusDto>().ReverseMap();
            #endregion

            #region Hosuing List Schools List
            CreateMap<HousingList, HousingListDto>().ReverseMap();
            CreateMap<HousingList, HousingListSelectDto>().ReverseMap();
            CreateMap<CommentHousing, CommentHousingDto>().ReverseMap();
            CreateMap<CommentHousing, CommentHousingSelectDto>().ReverseMap();
            CreateMap<DocumentHousing, DocumentHousingDto>().ReverseMap();
            CreateMap<HousingStatusHistory, HousingStatusHistorySelectDto>().ReverseMap();
            CreateMap<HousingStatusHistory, HousingStatusHistoryDto>().ReverseMap();
            CreateMap<AmenitiesHousingList, AmenitiesHousingListDto>().ReverseMap();
            CreateMap<SchoolsList, SchoolsListDto>().ReverseMap();
            CreateMap<RenewalDetailHome, RenewalDetailHomeDto>().ReverseMap();
            CreateMap<ContractDetail, ContractDetailDto>().ReverseMap();
            CreateMap<GroupPaymnetsHousing, GroupPaymentsHousingDto>().ReverseMap();
            CreateMap<GroupCostSaving, GroupCostSavingDto>().ReverseMap();
            CreateMap<PropertyExpense, PropertyExpenseDto>().ReverseMap();
            CreateMap<RenewalDetailHome, RenewalDetailHomeDto>().ReverseMap();
            CreateMap<DepartureDetailsHome, DepartureDetailsHomeDto>().ReverseMap();
            CreateMap<LandlordDetailsHome, LandlordDetailsHomeDto>().ReverseMap();
            CreateMap<LandlordHeaderDetailsHome, LandlordHeaderDetailsHomeDto>().ReverseMap();
            CreateMap<CreditCardLandLordDetail, CreditCardLandLordDetailDto>().ReverseMap();
            CreateMap<CostSavingHome, CostSavingHomeDto>().ReverseMap();
            CreateMap<HousingReport, HousingReportDto>().ReverseMap();
            CreateMap<PaymentHousing, PaymentHousingDto>().ReverseMap();
            #endregion

            #region Survey
            CreateMap<ExperienceSurvey, ExperienceSurveyDto>().ReverseMap();
            CreateMap<ExperienceSurvey, ExperienceSurveySelectDto>().ReverseMap();
            CreateMap<ExperienceSurveySupplier, ExperienceSurveySupplierDto>().ReverseMap();
            CreateMap<ExperienceSurveySupplier, ExperienceSurveySupplierSelectDto>().ReverseMap();
            #endregion
            #region Report of the day
            CreateMap<ReportDay, ReportDayDto>().ReverseMap();
            CreateMap<ServiceReportDay, ServiceReportDayDto>().ReverseMap();
            CreateMap<Summary, Controllers.ReportDayController.Summary>().ReverseMap();
            CreateMap<ReportSummary, Controllers.ReportDayController.ReportSummary>().ReverseMap();
            CreateMap<ConclusionServiceReportDay, ConclusionServiceReportDayDto>().ReverseMap();
            #endregion
            #region Request additional time
            CreateMap<RequestAdditionalTime, RequestAdditionalTimeDto>().ReverseMap();
            #endregion
            #region Supplier Partner Profile Service
            CreateMap<SupplierPartnerProfileService, SupplierPartnerProfileServiceDto>().ReverseMap();
            CreateMap<SupplierPartnerDetail, SupplierPartnerDetailDto>().ReverseMap();
            CreateMap<TypeVehiclesSupplierPartnerDetail, TypeVehiclesSupplierPartnerDetailDto>().ReverseMap();
            CreateMap<AreasCoverageService, AreasCoverageServiceDto>().ReverseMap();
            CreateMap<Campus, CampusDto>().ReverseMap();
            CreateMap<DocumentAreasCoverageService, DocumentAreasCoverageServiceDto>().ReverseMap();
            CreateMap<CityAreasCoverageService, CityAreasCoverageServiceDto>().ReverseMap();
            CreateMap<PaymentInformationService, PaymentInformationServiceDto>().ReverseMap();
            CreateMap<CreditCardPaymentInformationService, CreditCardPaymentInformationServiceDto>().ReverseMap();
            CreateMap<WireTransferService, WireTransferServiceDto>().ReverseMap();
            CreateMap<AdministrativeContactsService, AdministrativeContactsServiceDto>().ReverseMap();
            CreateMap<DocumentAdministrativeContactsService, DocumentAdministrativeContactsServiceDto>().ReverseMap();
            CreateMap<ConsultantContactsService, ConsultantContactsServiceDto>().ReverseMap();
            CreateMap<DocumentConsultantContactsService, DocumentConsultantContactsServiceDto>().ReverseMap();
            CreateMap<LanguagesConsultantContactsService, LanguagesConsultantContactsServiceDto>().ReverseMap();
            CreateMap<VehicleService, VehicleServiceDto>().ReverseMap();
            CreateMap<PhotosVehicleService, PhotosVehicleServiceDto>().ReverseMap();
            CreateMap<DocumentVehicleService, DocumentVehicleServiceDto>().ReverseMap();


            CreateMap<CatPrivacy, CatPrivacyDto>().ReverseMap();
            CreateMap<CatVehicleType, CatVehicleTypeDto>().ReverseMap();
            CreateMap<CatAreaCoverageType, CatAreaCoverageTypeDto>().ReverseMap();
            CreateMap<CatSupplierPartnerProfileStatus, CatSupplierPartnerProfileStatusDto>().ReverseMap();
            CreateMap<CatContactType, CatContactTypeDto>().ReverseMap();
            #endregion

            #region Supplier Partner Profile Consultant
            CreateMap<SupplierPartnerProfileConsultant, SupplierPartnerProfileConsultantDto>().ReverseMap();
            CreateMap<AreasCoverageConsultant, AreasCoverageConsultantDto>().ReverseMap();
            CreateMap<DocumentAreasCoverageConsultant, DocumentAreasCoverageConsultantDto>().ReverseMap();
            CreateMap<CityAreasCoverageConsultant, CityAreasCoverageConsultantDto>().ReverseMap();
            CreateMap<PaymentInformationConsultant, PaymentInformationConsultantDto>().ReverseMap();
            CreateMap<CreditCardPaymentInformationConsultant, CreditCardPaymentInformationConsultantDto>().ReverseMap();
            CreateMap<WireTransferConsultant, WireTransferConsultantDto>().ReverseMap();
            CreateMap<AdministrativeContactsConsultant, AdministrativeContactsConsultantDto>().ReverseMap();
            CreateMap<DocumentAdministrativeContactsConsultant, DocumentAdministrativeContactsConsultantDto>().ReverseMap();
            CreateMap<ProfileUser, ProfileUserDto>().ReverseMap();
            CreateMap<DocumentConsultantContactsConsultant, DocumentConsultantContactsConsultantDto>().ReverseMap();
            CreateMap<LanguagesConsultantContactsConsultant, LanguagesConsultantContactsConsultantDto>().ReverseMap();
            CreateMap<VehicleConsultant, VehicleConsultantDto>().ReverseMap();
            CreateMap<PhotosVehicleConsultant, PhotosVehicleConsultantDto>().ReverseMap();
            CreateMap<DocumentVehicleConsultant, DocumentVehicleConsultantDto>().ReverseMap();

            CreateMap<CalendarConsultantContactsConsultant, CalendarConsultantContactsConsultantDto>().ReverseMap();
            #endregion

            #region Profile
            CreateMap<PersonalInformation, PersonalInformationDto>().ReverseMap();
            CreateMap<PersonalInformation, PersonalInformationSelectDto>().ReverseMap();
            CreateMap<EmergencyContact, EmergencyContactDto>().ReverseMap();
            CreateMap<EmergencyContact, EmergencyContactSelectDto>().ReverseMap();
            CreateMap<CompesationBenefit, CompesationBenefitDto>().ReverseMap(); 
            CreateMap<CompesationBenefit, CompesationBenefitSelectDto>().ReverseMap();
            CreateMap<PaymentInformationProfile, PaymentInformationProfileDto>().ReverseMap();
            //CreateMap<DocumentProfile, DocumentProfileDto>().ReverseMap();
            CreateMap<CatDocumentStatus, CatDocumentStatusDto>().ReverseMap();
            CreateMap<AssignedTeam, AssignedTeamDto>().ReverseMap();
            #endregion
            #region Chat
            CreateMap<Conversation, ConversationDto>().ReverseMap();
            CreateMap<Message, MessageDto>().ReverseMap();
            CreateMap<UserGroup, UserGroupDto>().ReverseMap();
            CreateMap<DocumentMessage, DocumentMessageDto>().ReverseMap();
            #endregion
            #region Notification System
            CreateMap<NotificationSystem, NotificationSystemDto>().ReverseMap();
            CreateMap<NotificationSystem, NotificationSystemSelectDto>().ReverseMap();
            CreateMap<CatNotificationSystemType, CatNotificationSystemTypeDto>().ReverseMap();
            #endregion
            #region Coordinator
            CreateMap<Coordinator, CoordinatorDto>().ReverseMap();
            #endregion

            #region Client & Partner Profile
            CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();
            CreateMap<ClientPartnerProfileClient, ClientPartnerProfileClientDto>().ReverseMap();
            CreateMap<ClientPartnerProfile, ClientPartnerProfileDto>().ReverseMap();
            CreateMap<ClientPartnerProfileExperienceTeam, ClientPartnerProfileExperienceTeamDto>().ReverseMap();
            CreateMap<CompanyType, CompanyTypeDto>().ReverseMap();
            CreateMap<DocumentClientPartnerProfile, DocumentClientPartnerProfileDto>().ReverseMap();
            CreateMap<DocumentGeneralContractPricingInfo, DocumentGeneralContractPricingInfoDto>().ReverseMap();
            CreateMap<DocumentOfficeInformation, DocumentOfficeInformationDto>().ReverseMap();
            CreateMap<GeneralContractPricingInfo, GeneralContractPricingInfoDto>().ReverseMap();
            CreateMap<LifeCircle, LifeCircleDto>().ReverseMap();
            CreateMap<OfficeContact, OfficeContactDto>().ReverseMap();
            CreateMap<OfficeContactType, OfficeContactTypeDto>().ReverseMap();
            CreateMap<OfficeInformation, OfficeInformationDto>().ReverseMap();
            CreateMap<PaymentInformationOffice, PaymentInformationOfficeDto>().ReverseMap();
            CreateMap<PaymentRecurrence, PaymentRecurrenceDto>().ReverseMap();
            CreateMap<PricingSchedule, PricingScheduleDto>().ReverseMap();
            CreateMap<PricingType, PricingTypeDto>().ReverseMap();
            CreateMap<ReferralFee, ReferralFeeDto>().ReverseMap();
            CreateMap<ResponsiblePremierOffice, ResponsiblePremierOfficeDto>().ReverseMap();
            CreateMap<ServiceLocation, ServiceLocationDto>().ReverseMap();
            CreateMap<ServiceLocationCountry, ServiceLocationCountryDto>().ReverseMap();
            CreateMap<DocumentLocationCountry, DocumentLocationCountryDto>().ReverseMap();
            CreateMap<ServiceScoreAward, ServiceScoreAwardDto>().ReverseMap();
            CreateMap<SuccessProbability, SuccessProbabilityDto>().ReverseMap();
            CreateMap<TermsDeal, TermsDealDto>().ReverseMap();
            CreateMap<TypeOffice, TypeOfficeDto>().ReverseMap();
            CreateMap<TypePartnerClientProfile, TypePartnerClientProfileDto>().ReverseMap();
            CreateMap<WireTransferPaymentInformationOffice, WireTransferPaymentInformationOfficeDto>().ReverseMap();
            CreateMap<OperationLeader, OperationLeaderDto>().ReverseMap();
            CreateMap<Office, OfficeDto>().ReverseMap();
            CreateMap<CountryService, CountryServiceDto>().ReverseMap();
            CreateMap<StatusClientPartnerProfile, StatusClientPartnerProfileDto>().ReverseMap();
            #endregion
            #region Admin Center Service
            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<ServiceCountry, ServiceCountryDto>().ReverseMap();
            CreateMap<DocumentServiceCountry, DocumentServiceCountryDto>().ReverseMap();
            CreateMap<Service, ServiceSelectDto>().ReverseMap();
            CreateMap<ServiceCountry, ServiceCountrySelectDto>().ReverseMap();
            CreateMap<DocumentServiceCountry, DocumentServiceCountrySelectDto>().ReverseMap();
            #endregion
            #region Report
            CreateMap<Report, ReportDto>().ReverseMap();
            CreateMap<Report, ReportInsertDto>().ReverseMap();
            CreateMap<Filter, FilterDto>().ReverseMap();
            CreateMap<Filter, FilterSelectDto>().ReverseMap();
            CreateMap<Column, ColumnDto>().ReverseMap();
            CreateMap<Column, ColumnSelectDto>().ReverseMap();
            CreateMap<CatFilterReport, CatFilterReportDto>().ReverseMap();
            CreateMap<CatColumnsReport, CatColumnsReportDto>().ReverseMap();
            CreateMap<CatReportType, CatReportTypeDto>().ReverseMap();
            CreateMap<CatReport, CatReportDto>().ReverseMap();
            #endregion
            #region Invoice
            CreateMap<CatStatusInvoice, CatStatusInvoiceDto>().ReverseMap();
            CreateMap<CatInvoiceType, CatInvoiceTypeDto>().ReverseMap();
            CreateMap<Invoice, Models.WorkOrder.InvoiceDto>().ReverseMap();
            CreateMap<ServiceInvoice, ServiceInvoiceDto>().ReverseMap();
            CreateMap<CommentInvoice, CommentInvoiceDto>().ReverseMap();
            CreateMap<DocumentInvoice, DocumentInvoiceDto>().ReverseMap();
            CreateMap<AdditionalExpense, AdditionalExpenseDto>().ReverseMap();
            #endregion

            #region Training

            CreateMap<Training, trainingDto>().ReverseMap();
            CreateMap<CatTrainingGroup, CatTrainingGroupDto>().ReverseMap();
            CreateMap<CatTrainingType, CatTrainingTypeDto>().ReverseMap();
            CreateMap<Content, ContentDto>().ReverseMap();
            CreateMap<CatContentType, CatContentTypeDto>().ReverseMap();
            CreateMap<Evaluation, EvaluationDto>().ReverseMap();
            CreateMap<Answer, AnswerDto>().ReverseMap();
            CreateMap<CatElement, CatElementDto>().ReverseMap();
            CreateMap<Theme, ThemeDto>().ReverseMap();
            CreateMap<biz.premier.Entities.File, FileDto>().ReverseMap();
            CreateMap<Participant, ParticipantDto>().ReverseMap();
            CreateMap<ParticipantContent, ParticipantContentDto>().ReverseMap();
            CreateMap<ParticipantEvaluation, ParticipantEvaluationDto>().ReverseMap();

            #endregion

            #region Lease Renewal

            CreateMap<LeaseRenewal, LeaseRenewalDto>().ReverseMap();
            CreateMap<CommentLeaseRenewal, CommentLeaseRenewalDto>().ReverseMap();
            CreateMap<DocumentLeaseRenewal, DocumentLeaseRenewalDto>().ReverseMap();
            CreateMap<ReminderLeaseRenewal, ReminderLeaseRenewalDto>().ReverseMap();

            #endregion

            #region Slide Phrase & Upcoming Events

            CreateMap<UpcomingEvent, UpcomingEventDto>().ReverseMap();
            CreateMap<SlidePhrase, SlidePhraseDto>().ReverseMap();

            #endregion

            #region Email

            CreateMap<Email, EmailDto>().ReverseMap();
            CreateMap<EmailServiceRecord, EmailServiceRecordDto>().ReverseMap();

            #endregion

            #region Home Sale

            CreateMap<HomeSale, HomeSaleDto>().ReverseMap();
            CreateMap<VisitHomeSale, VisitHomeSaleDto>().ReverseMap();
            CreateMap<ReminderHomeSale, ReminderHomeSaleDto>().ReverseMap();
            CreateMap<DocumentHomeSale, DocumentHomeSaleDto>().ReverseMap();
            CreateMap<CommentHomeSale, CommentHomeSaleDto>().ReverseMap();

            #endregion

            #region Home Purchase

            CreateMap<HomePurchase, HomePurchaseDto>().ReverseMap();
            CreateMap<CommentHomePurchase, CommentHomePurchaseDto>().ReverseMap();
            CreateMap<DocumentHomePurchase, DocumentHomePurchaseDto>().ReverseMap();
            CreateMap<ReminderHomePurchase, ReminderHomePurchaseDto>().ReverseMap();

            #endregion

            #region Property Management
            
            CreateMap<PropertyManagement, PropertyManagementDto>().ReverseMap();
            CreateMap<VisitReportPropertyManagement, VisitReportPropertyManagementDto>().ReverseMap();
            CreateMap<DocumentPropertyManagement, DocumentPropertyManagementDto>().ReverseMap();
            CreateMap<PhotoPropertyManagement, PhotoPropertyManagementDto>().ReverseMap();
            CreateMap<PhotoInspectionPropertyManagement, PhotoInspectionPropertyManagementDto>().ReverseMap();
            CreateMap<PhotoReportIssuePropertyManagement, PhotoReportIssuePropertyManagementDto>().ReverseMap();
            CreateMap<DocumentReportIssuePropertyManagement, DocumentReportIssuePropertyManagementDto>().ReverseMap();
            CreateMap<CommentPropertyManagement, CommentPropertyManagementDto>().ReverseMap();
            CreateMap<PhotoBillPropertyManagement, PhotoBillPropertyManagementDto>().ReverseMap();
            CreateMap<PhotoMailPropertyManagement, PhotoMailPropertyManagementDto>().ReverseMap();
            CreateMap<ReminderPropertyManagement, ReminderPropertyManagementDto>().ReverseMap();

            #endregion

            #region Other

            CreateMap<Other, OtherDto>().ReverseMap();
            CreateMap<CommentOther, CommentOtherDto>().ReverseMap();
            CreateMap<ReminderOther, ReminderOtherDto>().ReverseMap();
            CreateMap<DocumentOther, DocumentOtherDto>().ReverseMap(); 

            #endregion

            #region Catalogs Home Sale/Purchase, Porperty Management
            
            CreateMap<CatStatusSale, CatStatusSaleDto>().ReverseMap();
            CreateMap<CatStatusHomePurchase, CatStatusHomePurchaseDto>().ReverseMap();
            CreateMap<CatStatusReportIssue, CatStatusReportIssueDto>().ReverseMap();
            CreateMap<CatSeverity, CatSeverityDto>().ReverseMap();
            CreateMap<CatBillType, CatBillTypeDto>().ReverseMap();
            CreateMap<CatLibrary, CatLibraryDto>().ReverseMap();

            #endregion

            #region Industry

            CreateMap<CatIndustry, CatIndustryDto>().ReverseMap();

            #endregion

            #region Post It

            CreateMap<PostIt, PostItDto>().ReverseMap();

            #endregion

            #region Tenancy Management

            CreateMap<TenancyManagement, TenancyManagementDto>().ReverseMap();
            CreateMap<ReminderTenancyManagement, ReminderTenancyManagementDto>().ReverseMap();
            CreateMap<DocumentTenancyManagement, DocumentTenancyManagementDto>().ReverseMap();
            CreateMap<CommentTenancyManagement, CommentTenancyManagementDto>().ReverseMap();
            CreateMap<ReportAnEvent, ReportAnEventDto>().ReverseMap();
            CreateMap<AssignedPhoto, AssignedPhotoDto>().ReverseMap();
            CreateMap<SupplierConsultantPhoto, SupplierConsultantPhotoDto>().ReverseMap();
            CreateMap<CommentReportAnEvent, CommentReportAnEventDto>().ReverseMap();
            CreateMap<CatStatusReportAnEvent, CatStatusReportAnEventDto>().ReverseMap();
            CreateMap<CatEvent, CatEventDto>().ReverseMap();

            #endregion

            CreateMap<CountryPhoneCode, CountryPhoneCodeDto>().ReverseMap();

            CreateMap<Country, CountriesGenericDto>().ReverseMap();
            CreateMap<Country, CountriesDto>().ReverseMap();
            CreateMap<State, StatesGenericDto>().ReverseMap();
            CreateMap<City, CitiesGenericDto>().ReverseMap();
            CreateMap<CatTimeZone, CatTimeZoneDto>().ReverseMap();
            CreateMap<CatStatusAppointment, CatStatusAppointmentDto>().ReverseMap();
            CreateMap<CatPaymentProcess, CatPaymentProcessDto>().ReverseMap();

        }
    }
}
