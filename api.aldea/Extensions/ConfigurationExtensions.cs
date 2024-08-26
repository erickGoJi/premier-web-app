using Microsoft.Extensions.DependencyInjection;
using api.premier.ActionFilter;
using biz.premier.Repository;
using biz.premier.Servicies;
using dal.premier.Repository;
using biz.premier.Repository.City;
using dal.premier.Repository.City;
using biz.premier.Repository.Country;
using dal.premier.Repository.Country;
using biz.premier.Repository.Status;
using dal.premier.Repository.Status;
using biz.premier.Repository.Client;
using dal.premier.Repository.Client;
using biz.premier.Repository.ServiceRecord;
using dal.premier.Repository.ServiceRecord;
using biz.premier.Repository.Catalogue;
using dal.premier.Repository.Catalogue;
using biz.premier.Repository.ServiceOrder;
using dal.premier.Repository.ServiceOrder;
using biz.premier.Repository.AssigneeName;
using dal.premier.Repository.AssigneeName;
using biz.premier.Repository.RequestInformation;
using dal.premier.Repository.RequestInformation;
using biz.premier.Repository.ImmigrationProfile;
using dal.premier.Repository.ImmigrationProfile;
using biz.premier.Repository.HousingSpecification;
using dal.premier.Repository.HousingSpecification;
using biz.premier.Repository.Relocation;
using dal.premier.Repository.Relocation;
using biz.premier.Repository.Utility;
using dal.premier.Repository.Utility;
using biz.premier.Repository.Immigration;
using dal.premier.Repository.Immigration;
using biz.premier.Repository.Follow;
using dal.premier.Repository.Follow;
using biz.premier.Repository.Scalate;
using dal.premier.Repository.Scalate;
using biz.premier.Repository.AssignedService;
using dal.premier.Repository.AssignedService;
using biz.premier.Repository.WorkPermit;
using dal.premier.Repository.WorkPermit;
using biz.premier.Repository.TypeService;
using dal.premier.Repository.TypeService;
using Microsoft.AspNetCore.Builder;
using biz.premier.Repository.Applicant;
using dal.premier.Repository.Applicant;
using biz.premier.Repository.ResidencyPermit;
using dal.premier.Repository.ResidencyPermit;
using biz.premier.Repository.VisaCategory;
using dal.premier.Repository.VisaCategory;
using biz.premier.Repository.VisaDeregistration;
using dal.premier.Repository.VisaDeregistration;
using biz.premier.Repository.Appointment;
using dal.premier.Repository.Appointment;
using biz.premier.Repository.CorporateAssistance;
using dal.premier.Repository.CorporateAssitance;
using biz.premier.Repository.Renewal;
using dal.premier.Repository.Renewal;
using biz.premier.Repository.LegalReview;
using dal.premier.Repository.LegalReview;
using biz.premier.Repository.Notification;
using dal.premier.Repository.Notification;
using biz.premier.Repository.LocalDocumentation;
using dal.premier.Repository.LocalDocumentation;
using biz.premier.Repository.DocumentManagement;
using dal.premier.Repository.DocumentManagement;
using biz.premier.Repository.PredicisionOrientation;
using dal.premier.Repository.PredicisionOrientation;
using biz.premier.Repository.AreaOrientation;
using dal.premier.Repository.AreaOrientation;
using biz.premier.Repository.SettlingIn;
using dal.premier.Repository.SettlingIn;
using biz.premier.Repository.SchoolingSearch;
using dal.premier.Repository.SchoolingSearch;
using biz.premier.Entities;
using biz.premier.Repository.Departure;
using dal.premier.Repository.Departure;
using dal.premier.Repository.TemporaryHousingCoordinaton;
using biz.premier.Repository.TemporaryHousingCoordinaton;
using biz.premier.Repository.Chat_Immigration_Relocation;
using dal.premier.Repository.Chat_Immigration_Relocation;
using dal.premier.Repository.RentalFurnitureCoordination;
using biz.premier.Repository.RentalFurnitureCoordination;
using biz.premier.Repository.Transportation;
using dal.premier.Repository.Transportation;
using biz.premier.Repository.Call;
using dal.premier.Repository.Call;
using biz.premier.Repository.AirportTransportationServices;
using dal.premier.Repository.AirportTransportationServices;
using biz.premier.Repository.HomeFinding;
using dal.premier.Repository.HomeFinding;
using biz.premier.Repository.Task;
using biz.premier.Repository.MapIt;
using dal.premier.Repository.MapIt;
using dal.premier.Repository.Task;
using biz.premier.Repository.AssigneeFeedback;
using dal.premier.Repository.AssigneeFeedback;
using biz.premier.Repository.CountryGallery;
using dal.premier.Repository.CountryGallery;
using Microsoft.EntityFrameworkCore.Metadata;
using biz.premier.Repository.PropertyReport;
using dal.premier.Repository.PropertyReport;
using biz.premier.Repository.RequestPayment;
using api.premier.Models.RequestPayment;
using biz.premier.Repository.Catalogs;
using dal.premier.Repository.RequestPayment;
using biz.premier.Repository.WorkOrder;
using dal.premier.Repository.WorkOrder;
using biz.premier.Repository.ExperienceSurvey;
using dal.premier.Repository.ExperienceSurvey;
using biz.premier.Repository.ReportDay;
using dal.premier.Repository.ReportDay;
using biz.premier.Repository.RequestAdditionalTime;
using dal.premier.Repository.RequestAdditionalTime;
using biz.premier.Repository.SupplierPartnerProfile;
using dal.premier.Repository.SupplierPartnerProfile;
using biz.premier.Repository.ProfileUser;
using dal.premier.Repository.ProfileUser;
using biz.premier.Repository.Conversation;
using dal.premier.Repository.Conversation;
using biz.premier.Repository.NotificationSystem;
using dal.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.ClientPartner;
using biz.premier.Repository.Countries;
using biz.premier.Repository.EmailServiceRecord;
using biz.premier.Repository.HomePurchase;
using biz.premier.Repository.HomeSale;
using dal.premier.Repository.ClientPartner;
using dal.premier.Repository.Report;
using biz.premier.Repository.Report;
using biz.premier.Repository.Ínvoice;
using biz.premier.Repository.LeaseRenewal;
using biz.premier.Repository.OfficeContactTypeRepository;
using biz.premier.Repository.Other;
using biz.premier.Repository.PostIt;
using biz.premier.Repository.PropertyManagement;
using biz.premier.Repository.TenancyManagement;
using biz.premier.Repository.Training;
using dal.premier.Repository.Catalogs;
using dal.premier.Repository.Countries;
using dal.premier.Repository.EmailServiceRecord;
using dal.premier.Repository.HomePurchase;
using dal.premier.Repository.HomeSale;
using dal.premier.Repository.Invoice;
using dal.premier.Repository.LeaseRenewal;
using dal.premier.Repository.OfficeContactTypeRepository;
using dal.premier.Repository.Other;
using dal.premier.Repository.PostIt;
using dal.premier.Repository.PropertyManagement;
using dal.premier.Repository.TenancyManagement;
using dal.premier.Repository.Training;
using EmailRepository = dal.premier.Repository.Email.EmailRepository;
using IEmailRepository = biz.premier.Repository.Email.IEmailRepository;
using biz.premier.Repository.DocumentConsultantContactsService;
using dal.premier.Repository.DocumentConsultantContactsService;
using biz.premier.Repository.DocumentAdministrativeContactsConsultant;
using dal.premier.Repository.DocumentAdministrativeContactsConsultant;
using dal.premier.Repository.DocumentAdministrativeContactsService;
using biz.premier.Repository.DocumentAdministrativeContactsService;
using biz.premier.Repository.Reports;
using dal.premier.Repository.Reports;

namespace api.premier.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy",
            //        builder => builder
            //        //.WithOrigins("http://localhost:4201")
            //        //.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials());
            //});

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins("http://localhost")
                .WithOrigins("http://localhost:4200")
                .WithOrigins("http://localhost:8100")
                .WithOrigins("http://demo-minimalist.com")
                .WithOrigins("http://34.237.214.147")
                .WithOrigins("https://my.premierds.com/")
                .WithOrigins("Ionic://localhost")
                .WithOrigins("capacitor://localhost")
                .WithOrigins("http://localhost:63410")
                .AllowCredentials();
            }));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<biz.premier.Repository.Country.ICountryRepository, dal.premier.Repository.Country.CountryRepository>();
            services.AddTransient<IStatusRepository, StatusRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IServiceRecordRepository, ServiceRecordRepository>();
            services.AddTransient<IAssignedServiceRepository, AssignedServiceRepository>();
            services.AddTransient<IBreedRepository, BreedRepository>();
            services.AddTransient<ICoordinatorRepository, CoordinatorRepository>();
            services.AddTransient<ICoordinatorTypeRepository, CoordinatorTypeRepository>();
            services.AddTransient<ILanguagesRepository, LanguagesRepository>();
            services.AddTransient<IPartnerRepository, PartnerRepository>();
            services.AddTransient<IPetTypeRepository, PetTypeRepository>();
            services.AddTransient<IRelationshipRepository, RelationshipRepository>();
            services.AddTransient<IServiceLineRepository, ServiceLineRepository>();
            services.AddTransient<ISizeRepository, SizeRepository>();
            services.AddTransient<ISupplierCompanyRepository, SupplierCompanyRepository>();
            services.AddTransient<ISupplierRepository, SupplierRepository>();
            services.AddTransient<ISupplierTypeRepository, SupplierTyperRepository>();
            services.AddTransient<IWeightMeasuresRepository, WeightMeasuresRepository>();
            services.AddTransient<IAuthorizedByRepository, AuthorizedByRepository>();
            services.AddTransient<IWorkOrderRepository, WorkOrderRepository>();
            services.AddTransient<IMaritalstatusRepository, MaritalstatusRepository>();
            services.AddTransient<IAssigneeNameRepository, AssigneeNameRepository>();
            services.AddTransient<IRequestInformationRepository, RequestInformationRepository>();
            services.AddTransient<IPolicyTypeRepository, PolicyTypeRepository>();
            services.AddTransient<IHousingSpecification, dal.premier.Repository.HousingSpecification.HousingSpecification>();
            services.AddTransient<IImmigrationProfileRepository, ImmigrationProfileRepository>();
            services.AddTransient<IDocumentDependentImmigration, DocumentDependentImmigration>();
            services.AddTransient<ISexRepository, SexRepository>();
            services.AddTransient<IRelocationSupplierPartenerRepository, RelocationSupplierPartenerRepository>();
            services.AddTransient<IRelocationCoordinatorRepository, RelocationCoordinatorRepository>();
            services.AddTransient<IUtiltyRepository, UtiltyRepository>();
            services.AddTransient<IDocumentTypeRepsoitory, DocumentTypeRepsoitory>();
            services.AddTransient<IIminigrationCoordinatorRepository, ImmigrationCoordinatorRepository>();
            services.AddTransient<IImmigrationSupplierPartenerRepository, ImmigrationSupplierPartenerRepository>();
            services.AddTransient<IFollowRepository, FollowRepository>();
            services.AddTransient<IScalateCommentsRepository, ScalateCommentsRepository>();
            services.AddTransient<IScalateServiceRepository, ScalateServiceRepository>();
            services.AddTransient<IScalateDocumentRepository, ScalateDocumentRepository>();
            services.AddTransient<IAssignedServiceSupplierRepository, AssignedServiceSupplierRepository>();
            services.AddTransient<IImmigrationEntryVisaRepository, ImmigrationEntryVisaRepository>();
            services.AddTransient<IWorkPermitRepository, WorkPermitRepository>();
            services.AddTransient<IDocumentWorkPermitRepository, DocumentWorkPermitRepository>();
            services.AddTransient<IReminderWorkPermitRepository, ReminderWorkPermitRepository>();
            services.AddTransient<ITypeServiceRepository, TypeServiceRepository>();
            services.AddTransient<IApplicantRepository, ApplicantRepository>();
            services.AddTransient<IReminderResidencyPermitRepository, ReminderResidencyPermitRepository>();
            services.AddTransient<IDocumentResidencyPermitRepository, DocumentResidencyPermitRepository>();
            services.AddTransient<IResidencyPermitRepository, ResidencyPermitRepository>();
            services.AddTransient<IVisaCategoryRepository, VisaCategoryRepository>();
            services.AddTransient<IServiceRepository, ServiceRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IDeliviredRepository, DeliviredRepository>();
            services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<IVisaDeregistrationRepository, VisaDeregistrationRepository>();
            services.AddTransient<IReminderVisaDeregistrationRepository, ReminderVisaDeregistrationRepository>();
            services.AddTransient<IDocumentResidencyPermitRepository, DocumentResidencyPermitRepository>();
            services.AddTransient<IDeliveredInRepository, DeliveredInRepository>();
            services.AddTransient<IContractTypeRepository, ContractTypeRepository>();
            services.AddTransient<IMetricRepository, MetricRepository>();
            services.AddTransient<IAmenitieRepository, AmenitieRepository>();
            services.AddTransient<ICurrencyRepository, CurrencyRepository>();
            services.AddTransient<IPropertyTypeHousingRepository, PropertyTypeHousingRepository>();
            services.AddTransient<IHighestLevelEducationRepository, HighestLevelEducationRepository>();
            services.AddTransient<IProficiencyRepository, ProficiencyRepository>();
            services.AddTransient<IDocumentVisaDeregistrationRepository, DocumentVisaDeregistrationRepository>();
            services.AddTransient<ICorporateAssistanceRepository, CorporateAssistanceRepository>();
            services.AddTransient<IDocumentCorporateAssistanceRepository, DocumentCorporateAssistanceRepository>();
            services.AddTransient<IReminderCorporateAssistanceRepository, ReminderCorporateAssistanceRepository>();
            services.AddTransient<IRenewalRepository, RenewalRepository>();
            services.AddTransient<IDocumentRenawalRepository, DocumentRenawalRepository>();
            services.AddTransient<IReminderRenawalRepository, ReminderRenawalRepository>();
            services.AddTransient<IWorkOrderServicesRepository, WorkOrderServicesRepository>();
            services.AddTransient<ILegalReviewRepository, LegalReviewRepository>();
            services.AddTransient<IReminderLegalReviewRepository, ReminderLegalReviewRepository>();
            services.AddTransient<IDocumentLegalReviewRepository, DocumentLegalReviewRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IReminderNotificationRepository, ReminderNotificationRepository>();
            services.AddTransient<IDocumentNotificationRepository, DocumentNotificationRepository>();
            services.AddTransient<ILocalDocumentationRepository, LocalDocumentationRepository>();
            services.AddTransient<IReminderLocalDocumentationRepository, ReminderLocalDocumentationRepository>();
            services.AddTransient<IDocumentLocalDocumentationRepository, DocumentLocalDocumentationRepository>();
            services.AddTransient<IDocumentManagementRepository, DocumentManagementRepository>();
            services.AddTransient<IReminderDocumentManagementRepository, ReminderDocumentManagementRepository>();
            services.AddTransient<IDocumentDocumentManagementRepository, DocumentDocumentManagementRepository>();
            services.AddTransient<IDurationRepository, DurationRepository>();
            services.AddTransient<IPredicisionOrientationRepository, PredicisionOrientationRepository>();
            services.AddTransient<IReminderPredicisionOrientationRepository, ReminderPredicisionOrientationRepository>();
            services.AddTransient<IDocumentPredicisionOrientationRepository, DocumentPredicisionOrientationRepository>();
            //services.AddTransient<IHousingPredicisionOrientationRepository, HousingPredicisionOrientationRepository>();
            services.AddTransient<ISchoolingRepository, SchoolingRepository>();

            services.AddTransient<IAreaOrientationRepository, AreaOrientationRepository>();
            services.AddTransient<IReminderAreaOrientationRepository, ReminderAreaOrientationRepository>();
            services.AddTransient<IDocumentAreaOrientationRepository, DocumentAreaOrientationRepository>();
            services.AddTransient<ISchoolingAreaOrientationRepository, SchoolingAreaOrientationRepository>();

            services.AddTransient<IDocumentSettlingInRepository, DocumentSettlingInRepository>();
            services.AddTransient<IReminderSettlingInRepository, ReminderSettlingInRepository>();
            services.AddTransient<ISettlingInRepository, SettlingInRepository>();

            services.AddTransient<ISchoolingSearchRepository, SchoolingSearchRepository>();
            services.AddTransient<IDocumentSchoolingSearchRepository, DocumentSchoolingSearchRepository>();
            services.AddTransient<IReminderSchoolingSearchRepository, ReminderSchoolingSearchRepository>();
            services.AddTransient<ISchoolingInformationSchoolingSearchRepository, SchoolingInformationSchoolingSearchRepository>();
            services.AddTransient<IPaymentSchoolingRepository, PaymentSchoolingRepository>();

            services.AddTransient<IGradeSchoolingRepository, GradeSchoolingRepository>();

            services.AddTransient<IAssistanceWithDepartureRepository, AssistanceWithDepartureRepository>();
            services.AddTransient<IDepartureRepository, DepartureRepository>();
            services.AddTransient<IDocumentDepartureRepository, DocumentDepartureRepository>();
            services.AddTransient<IDocumentRepairDepartureRepository, DocumentRepairDepartureRepository>();
            services.AddTransient<IPaymentsDepartureRepository, PaymentsDepartureRepository>();
            services.AddTransient<IReminderDepartureRepository, ReminderDepartureRepository>();
            services.AddTransient<IRepairDepartureRepository, RepairDepartureRepository>();

            services.AddTransient<IRepairTypeRepository, RepairTypeRepository>();
            services.AddTransient<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddTransient<IReservationTypeRepository, ReservationTypeRepository>();

            services.AddTransient<IExtensionTemporaryHousingCoordinatonRepository, ExtensionTemporaryHousingCoordinatonRepository>();
            services.AddTransient<IDocumentTemporaryHousingCoordinatonRepository, DocumentTemporaryHousingCoordinatonRepository>();
            services.AddTransient<IReminderTemporaryHousingCoordinatonRepository, ReminderTemporaryHousingCoordinatonRepository>();
            services.AddTransient<ITemporaryHousingCoordinatonRepository, TemporaryHousingCoordinatonRepository>();

            //Chat
            services.AddTransient<IChatImmigrationRelocationRepository, ChatImmigrationRelocationRepository>();
            services.AddTransient<IChatCommentImmigrationRelocationRepository, ChatCommentImmigrationRelocationRepository>();

            services.AddTransient<IRentalFurnitureCoordinationRepository, RentalFurnitureCoordinationRepository>();
            services.AddTransient<IPaymentsRentalFurnitureCoordinationRepository, PaymentsRentalFurnitureCoordinationRepository>();
            services.AddTransient<IExtensionRentalFurnitureCoordinationRepository, ExtensionRentalFurnitureCoordinationRepository>();
            services.AddTransient<IDocumentRentalFurnitureCoordinationRepository, DocumentRentalFurnitureCoordinationRepository>();
            services.AddTransient<IReminderRentalFurnitureCoordinationRepository, ReminderRentalFurnitureCoordinationRepository>();

            services.AddTransient<IReminderTransportationRepository, ReminderTransportationRepository>();
            services.AddTransient<IDocumentTransportationRepository, DocumentTransportationRepository>();
            services.AddTransient<IPaymentTransportationRepository, PaymentTransportationRepository>();
            services.AddTransient<ITransportationRepository, TransportationRepository>();
            services.AddTransient<ITransportationPickupRepository, TransportationPickupRepository>();

            services.AddTransient<IAssitanceWithRepository, AssitanceWithRepository>();

            services.AddTransient<ICallRepository, CallRepository>();
            services.AddTransient<IEmailRepository, EmailRepository>();

            services.AddTransient<ITransportTypeRepository, TransportTypeRepository>();

            services.AddTransient<IReminderAirportTransportationServicesRepository, ReminderAirportTransportationServicesRepository>();
            services.AddTransient<IPaymentAirportTransportationServicesRepository, PaymentAirportTransportationServicesRepository>();
            services.AddTransient<IDocumentAirportTransportationServicesRepository, DocumentAirportTransportationServicesRepository>();
            services.AddTransient<IAirportTransportationServicesRepository, AirportTransportationServicesRepository>();

            services.AddTransient<IHomeFindingRepository, HomeFindingRepository>();

            services.AddTransient<ITaskRepository, TaskRepository>();
            services.AddTransient<ITaskDocumentRepository, TaskDocumentRepository>();
            services.AddTransient<ITaskReplyRepository, TaskReplyRepository>();
            services.AddTransient<IMapItRepository, MapItRepository>();

            services.AddTransient<IAssigneeFeedbackRepository, AssigneeFeedbackRepository>();

            services.AddTransient<biz.premier.Repository.CountryGallery.ICountryRepository, dal.premier.Repository.CountryGallery.CountryRepository>();
            services.AddTransient<ICountryGalleryRepository, CountryGalleryRepository>();

            services.AddTransient<IPropertyReportRepository, PropertyReportRepository>();
            services.AddTransient<IPhotoPropertyReportRepository, PhotoPropertyReportRepository>();

            services.AddTransient<IRequestPaymentRepository, RequestPaymentRepository>();

            services.AddTransient<IRequestTypeRepository, RequestTypeRepository>();

            services.AddTransient<IBankAccountTypeRepository, BankAccountTypeRepository>();
            services.AddTransient<IPaymetMethodRepository, PaymetMethodRepository>();
            services.AddTransient<INotificationTypeRepository, NotificationTypeRepository>();

            services.AddTransient<IHousingListRepository, HousingListRepository>();
            services.AddTransient<ISchoolListRepository, SchoolListRepository>();
            services.AddTransient<IStatusWorkOrderRepository, StatusWorkOrderRepository>();
            services.AddTransient<IExperienceSurveyRepository, ExperienceSurveyRepository>();
            services.AddTransient<ISchoolStatusRepository, SchoolStatusRepository>();

            services.AddTransient<IPropertySectionRepository, PropertySectionRepository>();
            services.AddTransient<IStatusPropertySectionRepository, StatusPropertySectionRepository>();
            services.AddTransient<IStatusHousingRepository, StatusHousingRepository>();
            services.AddTransient<ICreditCardsRepository, CreditCardsRepository>();
            services.AddTransient<ILeaseTemplateRepository, LeaseTemplateRepository>();

            services.AddTransient<IReportDayRepository, ReportDayRepository>();
            services.AddTransient<IRequestAdditionalTimeRepository, RequestAdditionalTimeRepository>();

            services.AddTransient<ISupplierPartnerProfileServiceRepository, SupplierPartnerProfileServiceRepository>();
            services.AddTransient<ISupplierPartnerProfileConsultantRepository, SupplierPartnerProfileConsultantRepository>();

            services.AddTransient<IPrivacyRepository, PrivacyRepository>();
            services.AddTransient<IAreaCoverageTypeRepository, AreaCoverageTypeRepository>();
            services.AddTransient<IVehicleTypeRepository, VehicleTypeRepository>();
            services.AddTransient<ISupplierPartnerProfileStatusRepository, SupplierPartnerProfileStatusRepository>();
            services.AddTransient<IContactTypeRepository, ContactTypeRepository>();
            services.AddTransient<IActionTypeRepository, ActionTypeRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IOfficeRepository, OfficeRepository>();
            services.AddTransient<ICalendarConsultantContactsConsultantRepository, CalendarConsultantContactsConsultantRepository>();
            services.AddTransient<IDayRepository, DayRepository>();
            services.AddTransient<ITypeHousingRepository, TypeHousingRepository>();
            services.AddTransient<IProfileUserRepository, ProfileUserRepository>();
            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddTransient<INotificationSystemRepository, NotificationSystemRepository>();
            services.AddTransient<ICatNotificationSystemTypeRepository, CatNotificationSystemTypeRepository>();
            services.AddTransient<IPaymentTypeStatusRepository, PaymentTypeStatusRepository>();
            services.AddTransient<IResponsablePaymentRepository, ResponsablePaymentRepository>();
            services.AddTransient<IPurchaseStatusRepository, PurchaseStatusRepository>();
            services.AddTransient<IClient_PartnerRepository, Client_PartnerRepository>();
            services.AddTransient<ICompanyTypeRepository, CompanyTypeRepository>();
            services.AddTransient<IResponsiblePremierOfficeRespository, ResponsiblePremierOfficeRespository>();
            services.AddTransient<ILifeCircleRepository, LifeCircleRepository>();
            services.AddTransient<ISuccessProbabilityRepository, SuccessProbabilityRepository>();
            services.AddTransient<IReferralFeeRepository, ReferralFeeRepository>();
            services.AddTransient<IPricingScheduleRepository, PricingScheduleRepository>();
            services.AddTransient<IPaymentRecurrenceRepository, PaymentRecurrenceRepository>();
            services.AddTransient<ITitleRepository, TitleRepository>();
            services.AddTransient<ICatBenefitReposiotry, CatBenefitReposiotry>();
            services.AddTransient<IDocumentStatusRepository, DocumentStatusRepository>();
            services.AddTransient<ILeaseGuaranteeRepository, LeaseGuaranteeRepository>();
            services.AddTransient<IPriceTermRepository, PriceTermRepository>();
            services.AddTransient<IGeneralContractPricingInfoRepository, GeneralContractPricingInfoRepository>();
            services.AddTransient<ITypeOfficeRepository, TypeOfficeRepository>();
            services.AddTransient<IDocumentClientPartnerProfileRepository, DocumentClientPartnerProfileRepository>();
            services.AddTransient<IServiceScoreAwardsRepository, ServiceScoreAwardsRepository>();
            services.AddTransient<IClientPartnerProfileExperienceTeamRepository, ClientPartnerProfileExperienceTeamRepository>();
            services.AddTransient<IClientPartnerProfileClientRepository, ClientPartnerProfileClientRepository>();
            services.AddTransient<IOfficeInformationRepository, OfficeInformationRepository>();
            services.AddTransient<IOfficeContactRepository, OfficeContactRepository>();
            services.AddTransient<IServiceLocationsRepository, ServiceLocationsRepository>();
            services.AddTransient<IDocumentOfficeInformationRepository, DocumentOfficeInformationRepository>();
            services.AddTransient<IDocumentLocationCountryRepository, DocumentLocationCountryRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<biz.premier.Repository.AdminCenter.IServiceRepository, dal.premier.Repository.AdminCenter.ServiceRepository>();
            services.AddTransient<ITimeZoneRepository, TimeZoneRepository>();
            services.AddTransient<ICountryDocumentRepository, CountryDocumentRepository>();
            services.AddTransient<ICityAboutRepository, CityAboutRepository>();
            services.AddTransient<ICityAttractionsRepository, CityAttractionsRepository>();
            services.AddTransient<ICityEmergencyRepository, CityEmergencyRepository>();
            services.AddTransient<ICityWhatToDoRepository, CityWhatToDoRepository>(); 
            services.AddTransient<ICityWhereEatRepository, CityWhereEatRepository>();
            services.AddTransient<IPhotoAboutRepository, PhotoAboutRepository>();
            services.AddTransient<IPhotoAttractionsRepository, PhotoAttractionsRepository>();
            services.AddTransient<IPhotoEmergencyRepository, PhotoEmergencyRepository>();
            services.AddTransient<IPhotoWhatToDoRepository, PhotoWhatToDoRepository>();
            services.AddTransient<IPhotoWhereEatRepository, PhotoWhereEatRepository>();
            services.AddTransient<IRequestPaymentStatusRepository, RequestPaymentStatusRepository>();
            services.AddTransient<IReportRepository, ReportRepository>();
            services.AddTransient<IReportTypeRepository, ReportTypeRepository>();
            services.AddTransient<IColumnsReportRepository, ColumnsReportRepository>();
            services.AddTransient<IFilterReportRepository, FilterReportRepository>();
            services.AddTransient<IStatusClientPartnerProfileRepository, StatusClientPartnerProfileRepository>();
            services.AddTransient<IStatusInvoiceRepository, StatusInvoiceRepository>();
            services.AddTransient<IInvoiceTypeRepository, InvoiceTypeRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<ICatReportRepository, CatReportRepository>();
            services.AddTransient<ICreditTermRepository, CreditTermRepository>();
            services.AddTransient<ISupplierPartnerProfileTypeRepository, SupplierPartnerProfileTypeRepository>();
            services.AddTransient<ITaxePercentageRepository, TaxePercentageRepository>();
            services.AddTransient<ISupplierPartnerTypeRepository, SupplierPartnerTypeRepository>();
            services.AddTransient<ICatTypeRepository, CatTypeRepository>();
            services.AddTransient<ICatPaymentRecurrenceRepository, CatPaymentRecurrenceRepository>();
            services.AddTransient<ICatalogTypeRepository, CatalogTypeRepository>();
            services.AddTransient<ICatalogRepository, CatalogRepository>();
            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IEvaluationRepository, EvaluationRepository>();
            services.AddTransient<IThemeRepository, ThemeRepository>();
            services.AddTransient<ITrainingRepository, TrainingRepository>();
            services.AddTransient<ICatElementRepository, CatElementRepository>();
            services.AddTransient<ICatTrainingTypeRepository, CatTrainingTypeRepository>();
            services.AddTransient<ICatContentTypeRepository, CatContentTypeRepository>();
            services.AddTransient<ICatTrainingGroupRepository, CatTrainingGroupRepository>();
            services.AddTransient<IParticipantRepository, ParticipantRepository>();
            services.AddTransient<IParticipantContentRepository, ParticipantContentRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<ILeaseRenewalRepository, LeaseRenewalRepository>();
            services.AddTransient<IUpcomingEventRepository, UpcomingEventRepository>();
            services.AddTransient<ISlidePhraseRepository, SlidePhraseRepository>();
            services.AddTransient<biz.premier.Repository.EmailServiceRecord.IEmailRepository, dal.premier.Repository.EmailServiceRecord.EmailRepository>();
            services.AddTransient<IEmailServiceRecordRepository, EmailServiceRecordRepository>();
            services.AddTransient<IHomeSaleRepository, HomeSaleRepository>();
            services.AddTransient<IHomePurchaseRepository, HomePurchaseRepository>();
            services.AddTransient<IPropertyManagementRepository, PropertyManagementRepository>();
            services.AddTransient<IOtherRepository, OtherRepository>();
            services.AddTransient<ICatStatusHomePurchaseRepository, CatStatusHomePurchaseRepository>();
            services.AddTransient<ICatStatusSaleRepository, CatStatusSaleRepository>();
            services.AddTransient<ICatBillTypeDtoRepository, CatBillTypeDtoRepository>();
            services.AddTransient<ICatSeverityDtoRepository, CatSeverityDtoRepository>();
            services.AddTransient<ICatStatusReportIssueRepository, CatStatusReportIssueRepository>();
            services.AddTransient<ICatLibreryRepository, CatLibreryRepository>();
            services.AddTransient<ICatStatusWorkOrderRepository, CatStatusWorkOrderRepository>();
            services.AddTransient<ICatIndustryRepository, CatIndustryRepository>();
            services.AddTransient<IPostItRepository, PostItRepository>();
            
            services.AddTransient<ICatStatusReportAnEventRepository, CatStatusReportAnEventRepository>();
            services.AddTransient<IReportAnEventRepository, ReportAnEventRepository>();
            services.AddTransient<ITenancyManagementRepository, TenancyManagementRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            
            services.AddTransient<INationalityRepository, NationalityRepository>();
            
            services.AddTransient<ICountryPhoneCodeRepository, CountryPhoneCodeRepository>();
            services.AddTransient<IRelationshipContactRepository, RelationshipContactRepository>();
            
            services.AddTransient<ICountryGenericRepository, CountryGenericRepository>();
            services.AddTransient<ICityGenericRepository, CityGenericRepository>();
            services.AddTransient<IStateGenericRepository, StateGenericRepository>();
            services.AddTransient<IOfficeContactTypeRepository, OfficeContactTypeRepository>();

            services.AddTransient<IDocumentConsultantContactsServiceRepository, DocumentConsultantContactsServiceRepository>();
            services.AddTransient<IDocumentAdministrativeContactsConsultantRepository, DocumentAdministrativeContactsConsultantRepository>();
                                   
            services.AddTransient<IDocumentAdministrativeContactsServiceRepository, DocumentAdministrativeContactsServiceRepository> ();
            services.AddTransient<IServiceReportDayRepository, ServiceReportDayRepository>();

            services.AddTransient<ICatStatusAppointmentRepository, CatStatusAppointmentRepository>();
            services.AddTransient<ICatPaymentProcessRepository,CatPaymentProcessRepository>();

            services.AddTransient<IReports, ReportsRepository>();

            services.AddTransient<ICatSchoolExpensesPaymentRepository, CatSchoolExpensesPaymentRepository>();
            services.AddTransient<ICatStatusTransportPickupRepository, CatStatusTransportPickupRepository>();

            services.AddTransient<IAirportTransportPickupRepository, AirportTransportPickupRepository>();
            services.AddTransient<IUserGroupRepository, UserGroupRepository>();

        }
        
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
        }
    }
}
