using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.AirportTransportationService;
using api.premier.Models.AreaOrientation;
using api.premier.Models.Departure;
using api.premier.Models.HomeFinding;
using api.premier.Models.HomePurchase;
using api.premier.Models.HomeSale;
using api.premier.Models.LeaseRenewal;
using api.premier.Models.Other;
using api.premier.Models.PredicisionOrientation;
using api.premier.Models.PropertyManagement;
using api.premier.Models.RentalFurnitureCoordination;
using api.premier.Models.SchoolingSearch;
using api.premier.Models.SettlingIn;
using api.premier.Models.TemporaryHousingCoordinaton;
using api.premier.Models.TenancyManagement;
using api.premier.Models.Transportation;
using api.premier.Models.WorkOrder;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.AirportTransportationServices;
using biz.premier.Repository.AreaOrientation;
using biz.premier.Repository.Departure;
using biz.premier.Repository.HomeFinding;
using biz.premier.Repository.HomePurchase;
using biz.premier.Repository.HomeSale;
using biz.premier.Repository.LeaseRenewal;
using biz.premier.Repository.Other;
using biz.premier.Repository.PredicisionOrientation;
using biz.premier.Repository.PropertyManagement;
using biz.premier.Repository.RentalFurnitureCoordination;
using biz.premier.Repository.SchoolingSearch;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Repository.SettlingIn;
using biz.premier.Repository.TemporaryHousingCoordinaton;
using biz.premier.Repository.TenancyManagement;
using biz.premier.Repository.Transportation;
using biz.premier.Repository.Utility;
using biz.premier.Repository.WorkOrder;
using biz.premier.Servicies;
using dal.premier.Repository.Departure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelocationServicesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IWorkOrderRepository _serviceOrderRepository;
        private readonly IWorkOrderServicesRepository _serviceOrderServicesRepository;

        private readonly IPredicisionOrientationRepository _predicisionOrientationRepository;
        private readonly IDocumentPredicisionOrientationRepository _documentPredicisionOrientationRepository;
        private readonly IReminderPredicisionOrientationRepository _reminderPredicisionOrientationRepository;
        //private readonly IHousingPredicisionOrientationRepository _housingPredicisionOrientationRepository;
        private readonly ISchoolingRepository _schoolingRepository;

        private readonly IAreaOrientationRepository _areaOrientationRepository;
        private readonly IDocumentAreaOrientationRepository _documentAreaOrientationRepository;
        private readonly IReminderAreaOrientationRepository _reminderAreaOrientationRepository;
        private readonly ISchoolingAreaOrientationRepository _schoolingAreaOrientationRepository;

        private readonly ISettlingInRepository _settlingInRepository;
        private readonly IReminderSettlingInRepository _reminderSettlingInRepository;
        private readonly IDocumentSettlingInRepository _documentSettlingInRepository;

        private readonly ISchoolingSearchRepository _schoolingSearchRepository;
        private readonly IReminderSchoolingSearchRepository _reminderSchoolingSearchRepository;
        private readonly IDocumentSchoolingSearchRepository _documentSchoolingSearchRepository;
        private readonly ISchoolingInformationSchoolingSearchRepository _schoolingInformationSchoolingSearchRepository;

        private readonly IAssistanceWithDepartureRepository _assistanceWithDepartureRepository;
        private readonly IDepartureRepository _departureRepository;
        private readonly IDocumentDepartureRepository _documentDepartureRepository;
        private readonly IDocumentRepairDepartureRepository _documentRepairDepartureRepository;
        private readonly IPaymentsDepartureRepository _paymentsDepartureRepository;
        private readonly IReminderDepartureRepository _reminderDepartureRepository;
        private readonly IRepairDepartureRepository _repairDepartureRepository;

        private readonly IExtensionTemporaryHousingCoordinatonRepository _extensionTemporaryHousingCoordinatonRepository;
        private readonly IDocumentTemporaryHousingCoordinatonRepository _documentTemporaryHousingCoordinatonRepository;
        private readonly IReminderTemporaryHousingCoordinatonRepository _reminderTemporaryHousingCoordinatonRepository;
        private readonly ITemporaryHousingCoordinatonRepository _temporaryHousingCoordinatonRepository;

        private readonly IRentalFurnitureCoordinationRepository _rentalFurnitureCoordinationRepository;
        private readonly IPaymentsRentalFurnitureCoordinationRepository _paymentsRentalFurnitureCoordinationRepository;
        private readonly IExtensionRentalFurnitureCoordinationRepository _extensionRentalFurnitureCoordinationRepository;
        private readonly IDocumentRentalFurnitureCoordinationRepository _documentRentalFurnitureCoordinationRepository;
        private readonly IReminderRentalFurnitureCoordinationRepository _reminderRentalFurnitureCoordinationRepository;

        private readonly ITransportationRepository _transportationRepository;
        private readonly IPaymentTransportationRepository _paymentTransportationRepository;
        private readonly IDocumentTransportationRepository _documentTransportationRepository;
        private readonly IReminderTransportationRepository _reminderTransportationRepository;
        private readonly ITransportationPickupRepository _transportationPickupRepository; 
        private readonly IReminderAirportTransportationServicesRepository _reminderAirportTransportationServicesRepository;
        private readonly IPaymentAirportTransportationServicesRepository _paymentAirportTransportationServicesRepository;
        private readonly IDocumentAirportTransportationServicesRepository _documentAirportTransportationServicesRepository;
        private readonly IAirportTransportationServicesRepository _airportTransportationServicesRepository;
        private readonly IAirportTransportPickupRepository _airportTransportPickupRepository;

        private readonly ILeaseRenewalRepository _leaseRenewalRepository;
        
        private readonly IHousingListRepository _housingListRepository;

        private readonly IHomeSaleRepository _homeSaleRepository;
        private readonly IHomePurchaseRepository _homePurchaseRepository;
        private readonly IPropertyManagementRepository _propertyManagementRepository;
        private readonly IOtherRepository _otherRepository;

        private readonly IReportAnEventRepository _reportAnEventRepository;
        private readonly ITenancyManagementRepository _tenancyManagementRepository;

        private readonly IHomeFindingRepository _homeFindingRepository;
      //  private readonly IStandaloneServiceWorkOrderRepository _StandaloneServiceWorkOrderRepository;
       // private readonly IBundledServicesWorkOrderRepository _bundledServicesWorkOrderRepository;

        public RelocationServicesController(IMapper mapper, ILoggerManager logger, IUtiltyRepository utiltyRepository, IWorkOrderRepository serviceOrderRepository,
            IWorkOrderServicesRepository serviceOrderServicesRepository, IPredicisionOrientationRepository predicisionOrientationRepository, IDocumentPredicisionOrientationRepository documentPredicisionOrientationRepository,
            IReminderPredicisionOrientationRepository reminderPredicisionOrientationRepository, ISchoolingRepository schoolingRepository,
            IAreaOrientationRepository areaOrientationRepository, IDocumentAreaOrientationRepository documentAreaOrientationRepository, IReminderAreaOrientationRepository reminderAreaOrientationRepository,
            ISchoolingAreaOrientationRepository schoolingAreaOrientationRepository,
            ISettlingInRepository settlingInRepository, IReminderSettlingInRepository reminderSettlingInRepository, IDocumentSettlingInRepository documentSettlingInRepository,
            ISchoolingSearchRepository schoolingSearchRepository, IReminderSchoolingSearchRepository reminderSchoolingSearchRepository, IDocumentSchoolingSearchRepository documentSchoolingSearchRepository,
            ISchoolingInformationSchoolingSearchRepository schoolingInformationSchoolingSearchRepository, IAssistanceWithDepartureRepository assistanceWithDepartureRepository,
            IDepartureRepository departureRepository, IDocumentDepartureRepository documentDepartureRepository, IDocumentRepairDepartureRepository documentRepairDepartureRepository,
            IPaymentsDepartureRepository paymentsDepartureRepository, IReminderDepartureRepository reminderDepartureRepository, IRepairDepartureRepository repairDepartureRepository,
            IExtensionTemporaryHousingCoordinatonRepository extensionTemporaryHousingCoordinatonRepository, IDocumentTemporaryHousingCoordinatonRepository documentTemporaryHousingCoordinatonRepository,
            IReminderTemporaryHousingCoordinatonRepository reminderTemporaryHousingCoordinatonRepository, ITemporaryHousingCoordinatonRepository temporaryHousingCoordinatonRepository,
            IRentalFurnitureCoordinationRepository rentalFurnitureCoordinationRepository, IPaymentsRentalFurnitureCoordinationRepository paymentsRentalFurnitureCoordinationRepository,
            IExtensionRentalFurnitureCoordinationRepository extensionRentalFurnitureCoordinationRepository, IDocumentRentalFurnitureCoordinationRepository documentRentalFurnitureCoordinationRepository,
            IReminderRentalFurnitureCoordinationRepository reminderRentalFurnitureCoordinationRepository, ITransportationRepository transportationRepository, IPaymentTransportationRepository paymentTransportationRepository,
            IDocumentTransportationRepository documentTransportationRepository, IReminderTransportationRepository reminderTransportationRepository, ITransportationPickupRepository transportationPickupRepository,
            IReminderAirportTransportationServicesRepository reminderAirportTransportationServicesRepository,
            IPaymentAirportTransportationServicesRepository paymentAirportTransportationServicesRepository, IDocumentAirportTransportationServicesRepository documentAirportTransportationServicesRepository,
            IAirportTransportationServicesRepository airportTransportationServicesRepository,
            IHomeFindingRepository homeFindingRepository,
           // IStandaloneServiceWorkOrderRepository standaloneServiceWorkOrderRepository,
           // IBundledServicesWorkOrderRepository bundledServicesWorkOrderRepository,
            ILeaseRenewalRepository leaseRenewalRepository,
            IHousingListRepository housingListRepository,
            IHomeSaleRepository homeSaleRepository,
            IHomePurchaseRepository homePurchaseRepository,
            IPropertyManagementRepository propertyManagementRepository,
            IOtherRepository otherRepository,
            IReportAnEventRepository reportAnEventRepository,
            ITenancyManagementRepository tenancyManagementRepository,
            IAirportTransportPickupRepository airportTransportPickupRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _utiltyRepository = utiltyRepository;
            _serviceOrderRepository = serviceOrderRepository;
            _serviceOrderServicesRepository = serviceOrderServicesRepository;
           
            _predicisionOrientationRepository = predicisionOrientationRepository;
            _documentPredicisionOrientationRepository = documentPredicisionOrientationRepository;
            _reminderPredicisionOrientationRepository = reminderPredicisionOrientationRepository;
            //_housingPredicisionOrientationRepository = housingPredicisionOrientationRepository;
            _schoolingRepository = schoolingRepository;

            _areaOrientationRepository = areaOrientationRepository;
            _documentAreaOrientationRepository = documentAreaOrientationRepository;
            _reminderAreaOrientationRepository = reminderAreaOrientationRepository;
            _schoolingAreaOrientationRepository = schoolingAreaOrientationRepository;

            _settlingInRepository = settlingInRepository;
            _reminderSettlingInRepository = reminderSettlingInRepository;
            _documentSettlingInRepository = documentSettlingInRepository;

            _schoolingSearchRepository = schoolingSearchRepository;
            _reminderSchoolingSearchRepository = reminderSchoolingSearchRepository;
            _documentSchoolingSearchRepository = documentSchoolingSearchRepository;
            _schoolingInformationSchoolingSearchRepository = schoolingInformationSchoolingSearchRepository;

            _assistanceWithDepartureRepository = assistanceWithDepartureRepository;
            _departureRepository = departureRepository;
            _documentDepartureRepository = documentDepartureRepository;
            _documentRepairDepartureRepository = documentRepairDepartureRepository;
            _paymentsDepartureRepository = paymentsDepartureRepository;
            _reminderDepartureRepository = reminderDepartureRepository;
            _repairDepartureRepository = repairDepartureRepository;

            _extensionTemporaryHousingCoordinatonRepository = extensionTemporaryHousingCoordinatonRepository;
            _documentTemporaryHousingCoordinatonRepository = documentTemporaryHousingCoordinatonRepository;
            _reminderTemporaryHousingCoordinatonRepository = reminderTemporaryHousingCoordinatonRepository;
            _temporaryHousingCoordinatonRepository = temporaryHousingCoordinatonRepository;

            _rentalFurnitureCoordinationRepository = rentalFurnitureCoordinationRepository;
            _paymentsRentalFurnitureCoordinationRepository = paymentsRentalFurnitureCoordinationRepository;
            _extensionRentalFurnitureCoordinationRepository = extensionRentalFurnitureCoordinationRepository;
            _documentRentalFurnitureCoordinationRepository = documentRentalFurnitureCoordinationRepository;
            _reminderRentalFurnitureCoordinationRepository = reminderRentalFurnitureCoordinationRepository;

            _transportationRepository = transportationRepository;
            _paymentTransportationRepository = paymentTransportationRepository;
            _documentTransportationRepository = documentTransportationRepository;
            _reminderTransportationRepository = reminderTransportationRepository;
            _transportationPickupRepository = transportationPickupRepository;

            _reminderAirportTransportationServicesRepository = reminderAirportTransportationServicesRepository;
            _paymentAirportTransportationServicesRepository = paymentAirportTransportationServicesRepository;
            _documentAirportTransportationServicesRepository = documentAirportTransportationServicesRepository;
            _airportTransportationServicesRepository = airportTransportationServicesRepository;

            _homeFindingRepository = homeFindingRepository;
          //  _StandaloneServiceWorkOrderRepository = standaloneServiceWorkOrderRepository;
          //  _bundledServicesWorkOrderRepository = bundledServicesWorkOrderRepository;

            _leaseRenewalRepository = leaseRenewalRepository;
            _housingListRepository = housingListRepository;
            _homeSaleRepository = homeSaleRepository;
            _homePurchaseRepository = homePurchaseRepository;
            _propertyManagementRepository = propertyManagementRepository;
            _otherRepository = otherRepository;

            _reportAnEventRepository = reportAnEventRepository;
            _tenancyManagementRepository = tenancyManagementRepository;
            _airportTransportPickupRepository = airportTransportPickupRepository;
        }

        #region PreDecision Orientation
        //Post Create a new PreDecision Orientation
        [HttpPost("PostPreDecisionOrientation", Name = "PostPreDecisionOrientation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PredecisionOrientationDto>> PostPreDecisionOrientation([FromBody] PredecisionOrientationDto dto)
        {
            var response = new ApiResponse<PredecisionOrientationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentPredecisionOrientations)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/PreDicisionOrientation/", i.FileExtension);
                }
                PredecisionOrientation immi = _predicisionOrientationRepository.Add(_mapper.Map<PredecisionOrientation>(dto));
                response.Result = _mapper.Map<PredecisionOrientationDto>(immi);
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

        //Put Update a PreDicision Orientation
        [HttpPut("PutPreDecisionOrientation", Name = "PutPreDecisionOrientation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PredecisionOrientationDto>> PutPreDecisionOrientation([FromBody] PredecisionOrientationDto dto)
        {
            var response = new ApiResponse<PredecisionOrientationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                PredecisionOrientation immi = _predicisionOrientationRepository.UpdateCustom(_mapper.Map<PredecisionOrientation>(dto), dto.Id);
                foreach (var i in dto.ReminderPredecisionOrientations)
                {
                    if (i.Id != 0)
                    {
                        _reminderPredicisionOrientationRepository.Update(_mapper.Map<ReminderPredecisionOrientation>(i), i.Id);
                    }
                    else
                    {
                        _reminderPredicisionOrientationRepository.Add(_mapper.Map<ReminderPredecisionOrientation>(i));
                    }
                }

                foreach (var i in dto.DocumentPredecisionOrientations)
                {
                    if (i.Id != 0)
                    {
                        if(i.FileRequest.Length > 150)
                        {
                            DocumentPredecisionOrientation document = _documentPredicisionOrientationRepository.Find(x => x.Id == i.Id);
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/PreDicisionOrientation/", i.FileExtension);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            _documentPredicisionOrientationRepository.Update(_mapper.Map<DocumentPredecisionOrientation>(i), i.Id);
                        }
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/PreDicisionOrientation/", i.FileExtension);
                        _documentPredicisionOrientationRepository.Add(_mapper.Map<DocumentPredecisionOrientation>(i));
                    }
                }

                foreach(var i in dto.Schoolings)
                {
                    if (i.Id != 0)
                    {
                        _schoolingRepository.Update(_mapper.Map<Schooling>(i), i.Id);
                    }
                    else
                    {
                        _schoolingRepository.Add(_mapper.Map<Schooling>(i));
                    }
                }

                response.Result = _mapper.Map<PredecisionOrientationDto>(immi);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

           // HomeFinding hf = _homeFindingRepository.UpdateCustom(_mapper.Map<HomeFinding>(dto), dto.Id);
          

            //change sr status by service in progress 
           // if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
           // {
                var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 17); //es el id de pre desici  en el catalogo de servicios 
           // }

            return StatusCode(201, response);
        }

        [HttpGet("GetPredecisionOrientation", Name = "GetPredecisionOrientation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetPredicisionOrientation([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _predicisionOrientationRepository.Find(x => x.WorkOrderServicesId   == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetPredecisionOrientationById", Name = "GetPredecisionOrientationById")]
        public ActionResult<ApiResponse<PredecisionOrientationSelectDto>> GetPredecisionOrientationById([FromQuery] int id)
        {
            var response = new ApiResponse<PredecisionOrientationSelectDto>();
            try
            {
                var map = _mapper.Map<PredecisionOrientationSelectDto>(_predicisionOrientationRepository.GetCustom(id));
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

        [HttpDelete("DeleteReminderPDO", Name = "DeleteReminderPDO")]
        public ActionResult DeleteReminderPDO([FromQuery] int id)
        {
            try
            {
                var find = _reminderPredicisionOrientationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderPredicisionOrientationRepository.Delete(_mapper.Map<ReminderPredecisionOrientation>(find));
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

        [HttpDelete("DeleteDocumentPDO", Name = "DeleteDocumentPDO")]
        public ActionResult DeleteDocumentPDO([FromQuery] int id)
        {
            try
            {
                var find = _documentPredicisionOrientationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentPredicisionOrientationRepository.Delete(_mapper.Map<DocumentPredecisionOrientation>(find));
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
        #endregion
        #region Area Orientation
        //Post Create a new Are Orientation
        [HttpPost("PostAreaOrientation", Name = "PostAreaOrientation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AreaOrientationDto>> PostAreaOrientation([FromBody] AreaOrientationDto dto)
        {
            var response = new ApiResponse<AreaOrientationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentAreaOrientations)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/AreaOrientation/", i.FileExtension);
                }
                AreaOrientation immi = _areaOrientationRepository.Add(_mapper.Map<AreaOrientation>(dto));
                response.Result = _mapper.Map<AreaOrientationDto>(immi);
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

        //Put Update a Area Orientation
        [HttpPut("PutAreaOrientation", Name = "PutAreaOrientation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AreaOrientationDto>> PutAreaOrientation([FromBody] AreaOrientationDto dto)
        {
            var response = new ApiResponse<AreaOrientationDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                AreaOrientation immi = _areaOrientationRepository.UpdateCustom(_mapper.Map<AreaOrientation>(dto), dto.Id);
                foreach (var i in dto.ReminderAreaOrientations)
                {
                    if (i.Id != 0)
                    {
                        _reminderAreaOrientationRepository.Update(_mapper.Map<ReminderAreaOrientation>(i), i.Id);
                    }
                    else
                    {
                        _reminderAreaOrientationRepository.Add(_mapper.Map<ReminderAreaOrientation>(i));
                    }
                }

                foreach (var i in dto.DocumentAreaOrientations)
                {
                    if (i.Id != 0)
                    {
                        DocumentAreaOrientation document = _documentAreaOrientationRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/AreaOrientation/", i.FileExtension);
                        _documentAreaOrientationRepository.Update(_mapper.Map<DocumentAreaOrientation>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/AreaOrientation/", i.FileExtension);
                        _documentAreaOrientationRepository.Add(_mapper.Map<DocumentAreaOrientation>(i));
                    }
                }

                foreach (var i in dto.SchoolingAreaOrientations)
                {
                    if (i.Id != 0)
                    {
                        _schoolingAreaOrientationRepository.Update(_mapper.Map<SchoolingAreaOrientation>(i), i.Id);
                    }
                    else
                    {
                        _schoolingAreaOrientationRepository.Add(_mapper.Map<SchoolingAreaOrientation>(i));
                    }
                }

                //change sr status by service in progress 
             //   if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
             //   {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 18); //es el id de temp hou  en el catalogo de servicios 
               // }

                response.Result = _mapper.Map<AreaOrientationDto>(immi);
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

        [HttpGet("GetAreaOrientation", Name = "GetAreaOrientation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAreaOrientation([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _areaOrientationRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetAreaOrientationById", Name = "GetAreaOrientationById")]
        public ActionResult<ApiResponse<AreaOrientationSelectDto>> GetAreaOrientationById([FromQuery] int id)
        {
            var response = new ApiResponse<AreaOrientationSelectDto>();
            try
            {
                var map = _mapper.Map<AreaOrientationSelectDto>(_areaOrientationRepository.GetCustom(id));
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

        [HttpDelete("DeleteReminderAO", Name = "DeleteReminderAO")]
        public ActionResult DeleteReminderAO([FromQuery] int id)
        {
            try
            {
                var find = _reminderAreaOrientationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderAreaOrientationRepository.Delete(_mapper.Map<ReminderAreaOrientation>(find));
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

        [HttpDelete("DeleteDocumentAO", Name = "DeleteDocumentAO")]
        public ActionResult DeleteDocumentAO([FromQuery] int id)
        {
            try
            {
                var find = _documentAreaOrientationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentAreaOrientationRepository.Delete(_mapper.Map<DocumentAreaOrientation>(find));
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
        #endregion
        #region Settling In
        //Post Create a new Settling In
        [HttpPost("PostSettlingIn", Name = "PostSettlingIn")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SettlingInDto>> PostSettlingIn([FromBody] SettlingInDto dto)
        {
            var response = new ApiResponse<SettlingInDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentSettlingIns)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/SettlingIn/", i.FileExtension);
                }
                SettlingIn immi = _settlingInRepository.Add(_mapper.Map<SettlingIn>(dto));
                response.Result = _mapper.Map<SettlingInDto>(immi);
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

        //Put Update a Settling In
        [HttpPut("PutSettlingIn", Name = "PutSettlingIn")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SettlingInDto>> PutSettlingIn([FromBody] SettlingInDto dto)
        {
            var response = new ApiResponse<SettlingInDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                SettlingIn immi = _settlingInRepository.UpdateCustom(_mapper.Map<SettlingIn>(dto), dto.Id);
                foreach (var i in dto.ReminderSettlingIns)
                {
                    if (i.Id != 0)
                    {
                        _reminderSettlingInRepository.Update(_mapper.Map<ReminderSettlingIn>(i), i.Id);
                    }
                    else
                    {
                        _reminderSettlingInRepository.Add(_mapper.Map<ReminderSettlingIn>(i));
                    }
                }

                foreach (var i in dto.DocumentSettlingIns)
                {
                    if (i.Id != 0)
                    {
                        DocumentSettlingIn document = _documentSettlingInRepository.Find(x => x.Id == i.Id);
                        _utiltyRepository.DeleteFile(document.FileRequest);
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/SettlingIn/", i.FileExtension);
                        _documentSettlingInRepository.Update(_mapper.Map<DocumentSettlingIn>(i), i.Id);
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/SettlingIn/", i.FileExtension);
                        _documentSettlingInRepository.Add(_mapper.Map<DocumentSettlingIn>(i));
                    }
                }


                //change sr status by service in progress 
               // if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
             //   {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 19); //es el id de setting  en el catalogo de servicios 
              //  }


                response.Result = _mapper.Map<SettlingInDto>(immi);
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

        [HttpGet("GetSettlingIn", Name = "GetSettlingIn")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSettlingIn([FromQuery] int service_order_services_Id)
        {
            try
            {
                var Result = _settlingInRepository.Find(x => x.WorkOrderServicesId == service_order_services_Id);
                return Ok(new { Success = true, Result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetSettlingInById", Name = "GetSettlingInById")]
        public ActionResult<ApiResponse<SettlingInSelectDto>> GetSettlingInById([FromQuery] int id)
        {
            var response = new ApiResponse<SettlingInSelectDto>();
            try
            {
                var map = _settlingInRepository.GetCustom(id);
                response.Result = _mapper.Map<SettlingInSelectDto>(map);
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

        [HttpDelete("DeleteReminderSI", Name = "DeleteReminderSI")]
        public ActionResult DeleteReminderSI([FromQuery] int id)
        {
            try
            {
                var find = _reminderSettlingInRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderSettlingInRepository.Delete(_mapper.Map<ReminderSettlingIn>(find));
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

        [HttpDelete("DeleteDocumentSI", Name = "DeleteDocumentSI")]
        public ActionResult DeleteDocumentSI([FromQuery] int id)
        {
            try
            {
                var find = _documentSettlingInRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentSettlingInRepository.Delete(_mapper.Map<DocumentSettlingIn>(find));
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
        #endregion
        #region Schooling Search
        //Post Create a new Schooling Search
        [HttpPost("PostSchoolingSearch", Name = "PostSchoolingSearch")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SchoolingSearchDto>> PostSchoolingSearch([FromBody] SchoolingSearchDto dto)
        {
            var response = new ApiResponse<SchoolingSearchDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentSchoolingSearches)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/SchoolingSearch/", i.FileExtension);
                }
                SchoolingSearch immi = _schoolingSearchRepository.Add(_mapper.Map<SchoolingSearch>(dto));
                response.Result = _mapper.Map<SchoolingSearchDto>(immi);
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

        //Put Update a Schooling Search
        [HttpPut("PutSchoolingSearch", Name = "PutSchoolingSearch")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<SchoolingSearchDto>> PutSchoolingSearch([FromBody] SchoolingSearchDto dto)
        {
            var response = new ApiResponse<SchoolingSearchDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                SchoolingSearch immi = _schoolingSearchRepository.UpdateCustom(_mapper.Map<SchoolingSearch>(dto));
                foreach (var i in dto.ReminderSchoolingSearches)
                {
                    if (i.Id != 0)
                    {
                        _reminderSchoolingSearchRepository.Update(_mapper.Map<ReminderSchoolingSearch>(i), i.Id);
                    }
                    else
                    {
                        _reminderSchoolingSearchRepository.Add(_mapper.Map<ReminderSchoolingSearch>(i));
                    }
                }

                foreach (var i in dto.DocumentSchoolingSearches)
                {
                    if (i.Id != 0)
                    {
                        if(i.FileRequest.Length > 150)
                        {
                            DocumentSchoolingSearch document = _documentSchoolingSearchRepository.Find(x => x.Id == i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/SchoolingSearch/", i.FileExtension);
                            _documentSchoolingSearchRepository.Update(_mapper.Map<DocumentSchoolingSearch>(i), i.Id);
                        }
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/SchoolingSearch/", i.FileExtension);
                        _documentSchoolingSearchRepository.Add(_mapper.Map<DocumentSchoolingSearch>(i));
                    }
                }

                foreach (var i in dto.SchoolingInformations)
                {
                    if (i.Id != 0)
                    {
                        _schoolingInformationSchoolingSearchRepository.Update(_mapper.Map<SchoolingInformation>(i), i.Id);
                    }
                    else
                    {
                        _schoolingInformationSchoolingSearchRepository.Add(_mapper.Map<SchoolingInformation>(i));
                    }
                }

                //change sr status by service in progress 
             //   if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
               // {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 20); //es el id de schooling  en el catalogo de servicios 
              // }

                response.Result = _mapper.Map<SchoolingSearchDto>(immi);
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

        // Get: Get Service Record
        [HttpGet("GetSchoolingSearchById", Name = "GetSchoolingSearchById")]
        //public ActionResult<ApiResponse<SchoolingSearchSelectDto>> ([FromQuery] int id)
        public ActionResult<ApiResponse<SchoolingSearch>> GetSchoolingSearchById([FromQuery] int id)
        {
            var response = new ApiResponse<SchoolingSearch>();
            try
            {
                //var map = _mapper.Map<SchoolingSearchSelectDto>(_schoolingSearchRepository.GetCustom(id));
                var map = _schoolingSearchRepository.GetCustom(id);

               // var map1= _schoolingSearchRepository.GetCustomList(id);

                response.Result =    map;
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


        // Get: Get Dependent Child
        [HttpGet("GetDependentChild", Name = "GetDependentChild")]
        //public ActionResult<ApiResponse<SchoolingSearchSelectDto>> ([FromQuery] int id)
        public ActionResult<ApiResponse<List<DependentInformation>>> GetDependentChild([FromQuery] int id)
        {
            var response = new ApiResponse<List<DependentInformation>>();
            try
            {
                var map = _schoolingSearchRepository.GetDependent(id);

                // var map1= _schoolingSearchRepository.GetCustomList(id);

                response.Result = map;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);
        }

        // Get: Get Service Record
        [HttpGet("GetSchoolingSearchById_2022", Name = "GetSchoolingSearchById_2022")]
        //public ActionResult<ApiResponse<SchoolingSearchSelectDto>> GetSchoolingSearchById([FromQuery] int id)
        public ActionResult<ApiResponse<SchoolingSearch>> GetSchoolingSearchById_2022([FromQuery] int id)
        {
            var response = new ApiResponse<SchoolingSearch>();
            try
            {
                //var map = _mapper.Map<SchoolingSearchSelectDto>(_schoolingSearchRepository.GetCustom(id));
                var map = _schoolingSearchRepository.GetCustom(id);

                var map1 = _schoolingSearchRepository.GetCustomList(id);

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

        [HttpGet("GetSchoolinginformation", Name = "GetSchoolinginformation")]
        public ActionResult<ApiResponse<SchoolingSearch>> GetSchoolinginformation([FromQuery] int id)
        {
            var response = new ApiResponse<SchoolingSearch>();
            try
            {
                //var map = _mapper.Map<SchoolingSearchSelectDto>(_schoolingSearchRepository.GetCustom(id));
                var map = _schoolingSearchRepository.GetSchoolsList(id);
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

        [HttpDelete("DeleteReminderSS", Name = "DeleteReminderSS")]
        public ActionResult DeleteReminderSS([FromQuery] int id)
        {
            try
            {
                var find = _reminderSchoolingSearchRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderSchoolingSearchRepository.Delete(_mapper.Map<ReminderSchoolingSearch>(find));
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

        [HttpDelete("DeleteDocumentSS", Name = "DeleteDocumentSS")]
        public ActionResult DeleteDocumentSS([FromQuery] int id)
        {
            try
            {
                var find = _documentSchoolingSearchRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentSchoolingSearchRepository.Delete(_mapper.Map<DocumentSchoolingSearch>(find));
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
        #endregion
        #region Departure
        //Post Create a new Departure
        [HttpPost("PostDeparture", Name = "PostDeparture")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DepartureDto>> PostDeparture([FromBody] DepartureDto dto)
        {
            var response = new ApiResponse<DepartureDto>();
            try
            {
                foreach (var i in dto.DocumentDepartures)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Departure/", i.FileExtension);
                }
                Departure d = _departureRepository.Add(_mapper.Map<Departure>(dto));
                response.Result = _mapper.Map<DepartureDto>(d);
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

        //Put Update a Departure
        [HttpPut("PutDeparture", Name = "PutDeparture")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DepartureDto>> PutDeparture([FromBody] DepartureDto dto)
        {
            var response = new ApiResponse<DepartureDto>();
            try
            {
                Departure immi = _departureRepository.UpdateCustom(_mapper.Map<Departure>(dto), dto.Id);
                foreach (var i in dto.ReminderDepartures)
                {
                    if (i.Id != 0)
                    {
                        _reminderDepartureRepository.Update(_mapper.Map<ReminderDeparture>(i), i.Id);
                    }
                    else
                    {
                        _reminderDepartureRepository.Add(_mapper.Map<ReminderDeparture>(i));
                    }
                }

                foreach (var i in dto.DocumentDepartures)
                {
                    if (i.Id != 0)
                    {
                        if (i.FileRequest.Length > 150)
                        {
                            DocumentDeparture document = _documentDepartureRepository.Find(x => x.Id == i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Departure/", i.FileExtension);
                            _documentDepartureRepository.Update(_mapper.Map<DocumentDeparture>(i), i.Id);
                        }
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Departure/", i.FileExtension);
                        _documentDepartureRepository.Add(_mapper.Map<DocumentDeparture>(i));
                    }
                }

                foreach (var i in dto.DepartureAssistanceWiths)
                {
                    if (i.Id != 0)
                    {
                        _assistanceWithDepartureRepository.Update(_mapper.Map<DepartureAssistanceWith>(i), i.Id);
                    }
                    else
                    {
                        _assistanceWithDepartureRepository.Add(_mapper.Map<DepartureAssistanceWith>(i));
                    }
                }

                //change sr status by service in progress 
             //   if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
             //   {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 21); //es el id de departure en el catalogo de servicios 
             //   }


                response.Result = _mapper.Map<DepartureDto>(immi);
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

        // Get: Get Service Record
        [HttpGet("GetDepartureById", Name = "GetDepartureById")]
        public ActionResult<ApiResponse<DepartureSelectDto>> GetDepartureById([FromQuery] int id)
        {
            var response = new ApiResponse<DepartureSelectDto>();
            try
            {
                var map = _mapper.Map<DepartureSelectDto>(_departureRepository.GetCustom(id));
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

        [HttpDelete("DeleteReminderD", Name = "DeleteReminderD")]
        public ActionResult DeleteReminderD([FromQuery] int id)
        {
            try
            {
                var find = _reminderDepartureRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderDepartureRepository.Delete(_mapper.Map<ReminderDeparture>(find));
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

        [HttpDelete("DeleteDocumentD", Name = "DeleteDocumentD")]
        public ActionResult DeleteDocumentD([FromQuery] int id)
        {
            try
            {
                var find = _documentDepartureRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentDepartureRepository.Delete(_mapper.Map<DocumentDeparture>(find));
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
        #endregion
        #region Temporary Housing Coordinaton
        //Post Create a new temporary Housing Coordinaton
        [HttpPost("PostTemporaryHousingCoordinaton", Name = "PostTemporaryHousingCoordinaton")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TemporaryHousingCoordinatonDto>> PostTemporaryHousingCoordinaton([FromBody] TemporaryHousingCoordinatonDto dto)
        {
            var response = new ApiResponse<TemporaryHousingCoordinatonDto>();
            try
            {
                foreach (var i in dto.DocumentTemporaryHousingCoordinatons)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/TemporaryHousingCoordinaton/", i.FileExtension);
                }
                TemporaryHousingCoordinaton d = _temporaryHousingCoordinatonRepository.Add(_mapper.Map<TemporaryHousingCoordinaton>(dto));
                response.Result = _mapper.Map<TemporaryHousingCoordinatonDto>(d);
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

        //Put Update a Departure
        [HttpPut("PutTemporaryHousingCoordinaton", Name = "PutTemporaryHousingCoordinaton")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TemporaryHousingCoordinatonDto>> PutTemporaryHousingCoordinaton([FromBody] TemporaryHousingCoordinatonDto dto)
        {
            var response = new ApiResponse<TemporaryHousingCoordinatonDto>();
            try
            {
                TemporaryHousingCoordinaton immi = _temporaryHousingCoordinatonRepository.UpdateCustom(_mapper.Map<TemporaryHousingCoordinaton>(dto), dto.Id);
                foreach (var i in dto.ReminderTemporaryHousingCoordinatons)
                {
                    if (i.Id != 0)
                    {
                        _reminderTemporaryHousingCoordinatonRepository.Update(_mapper.Map<ReminderTemporaryHousingCoordinaton>(i), i.Id);
                    }
                    else
                    {
                        _reminderTemporaryHousingCoordinatonRepository.Add(_mapper.Map<ReminderTemporaryHousingCoordinaton>(i));
                    }
                }

                foreach (var i in dto.DocumentTemporaryHousingCoordinatons)
                {
                    if (i.Id != 0)
                    {
                        if (i.FileRequest.Length > 150)
                        {
                            DocumentTemporaryHousingCoordinaton document = _documentTemporaryHousingCoordinatonRepository.Find(x => x.Id == i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/TemporaryHousingCoordinaton/", i.FileExtension);
                            _documentTemporaryHousingCoordinatonRepository.Update(_mapper.Map<DocumentTemporaryHousingCoordinaton>(i), i.Id);
                        }
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/TemporaryHousingCoordinaton/", i.FileExtension);
                        _documentTemporaryHousingCoordinatonRepository.Add(_mapper.Map<DocumentTemporaryHousingCoordinaton>(i));
                    }
                }

                foreach (var i in dto.StayExtensionTemporaryHousings)
                {
                    if (i.Id != 0)
                    {
                        _extensionTemporaryHousingCoordinatonRepository.Update(_mapper.Map<StayExtensionTemporaryHousing>(i), i.Id);
                    }
                    else
                    {
                        var days = _extensionTemporaryHousingCoordinatonRepository.GetAll().Where(x => x.TemporaryHousingCoordinationId == i.TemporaryHousingCoordinationId).Sum(s => s.TotalDays);
                        i.TotalDays = days.HasValue ? days.Value + dto.TotalDays.Value : dto.TotalDays.Value; 
                        _extensionTemporaryHousingCoordinatonRepository.Add(_mapper.Map<StayExtensionTemporaryHousing>(i));
                    }
                }
                //change sr status by service in progress 
              //  if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
              //  {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 22); //es el id de temp hou  en el catalogo de servicios 
             //   }

                response.Result = _mapper.Map<TemporaryHousingCoordinatonDto>(immi);
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

        // Get: Get Service Record
        [HttpGet("GetTemporaryHousingCoordinatonById", Name = "GetTemporaryHousingCoordinatonById")]
        public ActionResult<ApiResponse<TemporaryHousingCoordinatonSelectDto>> GetTemporaryHousingCoordinatonById([FromQuery] int id)
        {
            var response = new ApiResponse<TemporaryHousingCoordinatonSelectDto>();
            try
            {
                var map = _mapper.Map<TemporaryHousingCoordinatonSelectDto>(_temporaryHousingCoordinatonRepository.GetCustom(id));
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

        [HttpDelete("DeleteReminderTHC", Name = "DeleteReminderTHC")]
        public ActionResult DeleteReminderTHC([FromQuery] int id)
        {
            try
            {
                var find = _reminderTemporaryHousingCoordinatonRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderTemporaryHousingCoordinatonRepository.Delete(_mapper.Map<ReminderTemporaryHousingCoordinaton>(find));
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

        [HttpDelete("DeleteDocumentTHC", Name = "DeleteDocumentTHC")]
        public ActionResult DeleteDocumentTHC([FromQuery] int id)
        {
            try
            {
                var find = _documentTemporaryHousingCoordinatonRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentTemporaryHousingCoordinatonRepository.Delete(_mapper.Map<DocumentTemporaryHousingCoordinaton>(find));
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
        #endregion
        #region Rental Furniture Coordinaton

        //Put Update a Rental Furniture Coordinaton
        [HttpPut("PutRentalFurnitureCoordinaton", Name = "PutRentalFurnitureCoordinaton")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RentalFurnitureCoordinationDto>> PutRentalFurnitureCoordinaton([FromBody] RentalFurnitureCoordinationDto dto)
        {
            var response = new ApiResponse<RentalFurnitureCoordinationDto>();
            try
            {
                RentalFurnitureCoordination immi = _rentalFurnitureCoordinationRepository.UpdateCustom(_mapper.Map<RentalFurnitureCoordination>(dto), dto.Id);
                foreach (var i in dto.ReminderRentalFurnitureCoordinations)
                {
                    if (i.Id != 0)
                    {
                        _reminderRentalFurnitureCoordinationRepository.Update(_mapper.Map<ReminderRentalFurnitureCoordination>(i), i.Id);
                    }
                    else
                    {
                        _reminderRentalFurnitureCoordinationRepository.Add(_mapper.Map<ReminderRentalFurnitureCoordination>(i));
                    }
                }

                foreach (var i in dto.DocumentRentalFurnitureCoordinations)
                {
                    if (i.Id != 0)
                    {
                        if (i.FileRequest.Length > 150)
                        {
                            DocumentRentalFurnitureCoordination document = _documentRentalFurnitureCoordinationRepository.Find(x => x.Id == i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/RentalFurnitureCoordinaton/", i.FileExtension);
                            _documentRentalFurnitureCoordinationRepository.Update(_mapper.Map<DocumentRentalFurnitureCoordination>(i), i.Id);
                        }
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/RentalFurnitureCoordinaton/", i.FileExtension);
                        _documentRentalFurnitureCoordinationRepository.Add(_mapper.Map<DocumentRentalFurnitureCoordination>(i));
                    }
                }

                foreach (var i in dto.StayExtensionRentalFurnitureCoordinations)
                {
                    if (i.Id != 0)
                    {
                        _extensionRentalFurnitureCoordinationRepository.Update(_mapper.Map<StayExtensionRentalFurnitureCoordination>(i), i.Id);
                    }
                    else
                    {
                        _extensionRentalFurnitureCoordinationRepository.Add(_mapper.Map<StayExtensionRentalFurnitureCoordination>(i));
                    }
                }

                foreach (var i in dto.PaymentsRentalFurnitureCoordinations)
                {
                    if (i.Id != 0)
                    {
                        _paymentsRentalFurnitureCoordinationRepository.Update(_mapper.Map<PaymentsRentalFurnitureCoordination>(i), i.Id);
                    }
                    else
                    {
                        _paymentsRentalFurnitureCoordinationRepository.Add(_mapper.Map<PaymentsRentalFurnitureCoordination>(i));
                    }
                }

                //change sr status by service in progress 
             //   if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
             //   {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 23); //es el id de rental  en el catalogo de servicios 
             //   }
                response.Result = _mapper.Map<RentalFurnitureCoordinationDto>(immi);
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

        // Get: Get Service Record
        [HttpGet("GetRentalFurnitureCoordinationById", Name = "GetRentalFurnitureCoordinationById")]
        public ActionResult<ApiResponse<RentalFurnitureCoordinationSelectDto>> GetRentalFurnitureCoordinationById([FromQuery] int id)
        {
            var response = new ApiResponse<RentalFurnitureCoordinationSelectDto>();
            try
            {
                var map = _mapper.Map<RentalFurnitureCoordinationSelectDto>(_rentalFurnitureCoordinationRepository.GetCustom(id));
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

        [HttpDelete("DeleteReminderRFC", Name = "DeleteReminderRFC")]
        public ActionResult DeleteReminderRFC([FromQuery] int id)
        {
            try
            {
                var find = _reminderRentalFurnitureCoordinationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderRentalFurnitureCoordinationRepository.Delete(_mapper.Map<ReminderRentalFurnitureCoordination>(find));
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

        [HttpDelete("DeleteDocumentRFC", Name = "DeleteDocumentRFC")]
        public ActionResult DeleteDocumentRFC([FromQuery] int id)
        {
            try
            {
                var find = _documentRentalFurnitureCoordinationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentRentalFurnitureCoordinationRepository.Delete(_mapper.Map<DocumentRentalFurnitureCoordination>(find));
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
        #endregion
        #region Transportation

        //Put Update a Transportation
        [HttpPut("PutTransportation", Name = "PutTransportation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CreateTransportationDto>> PutTransportation([FromBody] CreateTransportationDto dto)
        {
            var response = new ApiResponse<CreateTransportationDto>();
            try
            {
                CreateTransportationDto transportationDtos = new CreateTransportationDto();
                Transportation immi = _transportationRepository.UpdateCustom(_mapper.Map<Transportation>(dto), dto.Id);
                foreach (var _i in dto.ReminderTransportations)
                {
                    if (_i.Id != 0)
                    {
                        _reminderTransportationRepository.Update(_mapper.Map<ReminderTransportation>(_i), _i.Id);
                    }
                    else
                    {
                        _reminderTransportationRepository.Add(_mapper.Map<ReminderTransportation>(_i));
                    }
                }

                foreach (var _i in dto.DocumentTransportations)
                {
                    if (_i.Id != 0)
                    {
                        if (_i.FileRequest.Length > 150)
                        {
                            DocumentTransportation document = _documentTransportationRepository.Find(x => x.Id == _i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            _i.FileRequest = _utiltyRepository.UploadImageBase64(_i.FileRequest, "Files/Transportation/", _i.FileExtension);
                            _documentTransportationRepository.Update(_mapper.Map<DocumentTransportation>(_i), _i.Id);
                        }
                    }
                    else
                    {
                        _i.FileRequest = _utiltyRepository.UploadImageBase64(_i.FileRequest, "Files/Transportation/", _i.FileExtension);
                        _documentTransportationRepository.Add(_mapper.Map<DocumentTransportation>(_i));
                    }
                }

                foreach (var _i in dto.PaymentTransportations)
                {
                    if (_i.Id != 0)
                    {
                        _paymentTransportationRepository.Update(_mapper.Map<PaymentTransportation>(_i), _i.Id);
                    }
                    else
                    {
                        _paymentTransportationRepository.Add(_mapper.Map<PaymentTransportation>(_i));
                    }
                }

                foreach (var _i in dto.TransportPickups)
                {
                    if (_i.Id != 0)
                    {
                        _i.TransportationId = dto.Id;
                        _transportationPickupRepository.Update(_mapper.Map<TransportPickup>(_i), _i.Id);
                    }
                    else
                    {
                        _i.TransportationId = dto.Id;
                        _transportationPickupRepository.Add(_mapper.Map<TransportPickup>(_i));
                    }
                }
                //transportationDtos.Add(_mapper.Map<CreateTransportationDto>(immi));
                //change sr status by service in progress 
                // if (dto[0].StatusId != 1 && dto[0].StatusId != 38) // In progres 
                //  {
                var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 24); //es el id de transp mgt en el catalogo de servicios 
              //  }

                response.Result = dto;
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

        // Get: Get Service Record
        [HttpGet("GetTransportationById", Name = "GetTransportationById")]
        public ActionResult GetTransportationById([FromQuery] int applicatId, [FromQuery] int service_order_id, [FromQuery] int type_service)
        {
            try
            {
                return StatusCode(200, new { Success = true, Result = _transportationRepository.GetTransportations(applicatId, service_order_id, type_service), Message = "" });
                //response.Result = map;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "", Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        [HttpGet("GetSingleTransportationByIdS", Name = "GetSingleTransportationByIdS")]
        public ActionResult GetSingleTransportationByIdS([FromQuery] int service_id, int country_id)
        {
            try
            {
                return StatusCode(200, new { Success = true, Result = _transportationRepository.GetSingleTransportationById(service_id, country_id), Message = "" });
                //response.Result = map;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "", Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        [HttpGet("GetTransportation", Name = "GetTransportation")]
        public ActionResult<ApiResponse<TransportationSelectDto>> GetTransportationById([FromQuery] int id)
        {
            var response = new ApiResponse<TransportationSelectDto>();
            try
            {
                var map = _mapper.Map<TransportationSelectDto>(_transportationRepository.GetCustom(id));
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

        [HttpDelete("DeleteReminderT", Name = "DeleteReminderT")]
        public ActionResult DeleteReminderT([FromQuery] int id)
        {
            try
            {
                var find = _reminderTransportationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderTransportationRepository.Delete(_mapper.Map<ReminderTransportation>(find));
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

        [HttpDelete("DeleteDocumentT", Name = "DeleteDocumentT")]
        public ActionResult DeleteDocumentT([FromQuery] int id)
        {
            try
            {
                var find = _documentTransportationRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentTransportationRepository.Delete(_mapper.Map<DocumentTransportation>(find));
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
        #endregion
        #region Airport Transportation 

        //Put Update a Airport Transportation
        [HttpPut("PutAirportTransportationServices", Name = "PutAirportTransportationServices")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        //PutTransportation([FromBody] CreateTransportationDto dto)
        public ActionResult<ApiResponse<CreateAirportTransportationDto>> PutAirportTransportationServices([FromBody] CreateAirportTransportationDto dto)
        {
            var response = new ApiResponse<CreateAirportTransportationDto>();
            try
            {
                CreateAirportTransportationDto transportationDtos = new CreateAirportTransportationDto();
                AirportTransportationService immi = _airportTransportationServicesRepository.UpdateCustom(_mapper.Map<AirportTransportationService>(dto), dto.Id);
                foreach (var _i in dto.ReminderAirportTransportationServices)
                {
                    if (_i.Id != 0)
                    {
                        _reminderAirportTransportationServicesRepository.Update(_mapper.Map<ReminderAirportTransportationService>(_i), _i.Id);
                    }
                    else
                    {
                        _reminderAirportTransportationServicesRepository.Add(_mapper.Map<ReminderAirportTransportationService>(_i));
                    }
                }

                foreach (var _i in dto.DocumentAirportTransportationServices)
                {
                    if (_i.Id != 0)
                    {
                        if (_i.FileRequest.Length > 150)
                        {
                            DocumentAirportTransportationService document = _documentAirportTransportationServicesRepository.Find(x => x.Id == _i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            _i.FileRequest = _utiltyRepository.UploadImageBase64(_i.FileRequest, "Files/Transportation/", _i.FileExtension);
                            _documentAirportTransportationServicesRepository.Update(_mapper.Map<DocumentAirportTransportationService>(_i), _i.Id);
                        }
                    }
                    else
                    {
                        _i.FileRequest = _utiltyRepository.UploadImageBase64(_i.FileRequest, "Files/Transportation/", _i.FileExtension);
                        _documentAirportTransportationServicesRepository.Add(_mapper.Map<DocumentAirportTransportationService>(_i));
                    }
                }

                foreach (var _i in dto.PaymentAirportTransportationServices)
                {
                    if (_i.Id != 0)
                    {
                        _paymentAirportTransportationServicesRepository.Update(_mapper.Map<PaymentAirportTransportationService>(_i), _i.Id);
                    }
                    else
                    {
                        _paymentAirportTransportationServicesRepository.Add(_mapper.Map<PaymentAirportTransportationService>(_i));
                    }
                }

                foreach (var _i in dto.AirportTransportPickup)
                {
                    if (_i.Id != 0)
                    {
                        _i.TransportationId = dto.Id;
                        _airportTransportPickupRepository.Update(_mapper.Map<AirportTransportPickup>(_i), _i.Id);
                    }
                    else
                    {
                        _i.TransportationId = dto.Id;
                        _airportTransportPickupRepository.Add(_mapper.Map<AirportTransportPickup>(_i));
                    }
                }
                //transportationDtos.Add(_mapper.Map<CreateTransportationDto>(immi));
                //change sr status by service in progress 
                // if (dto[0].StatusId != 1 && dto[0].StatusId != 38) // In progres 
                //  {
                var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 24); //es el id de transp mgt en el catalogo de servicios 
                                                                                              //  }

                response.Result = dto;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        // Get: Get Service Record
        [HttpGet("GetAirportTransportationServicesById", Name = "GetAirportTransportationServicesById")]
        public ActionResult GetAirportTransportationServicesById([FromQuery] int applicatId, [FromQuery] int service_order_id, [FromQuery] int type_service)
        {
            try
            {
                return StatusCode(200, new { Success = true, 
                    Result = _airportTransportationServicesRepository.GetAirportTransportation(applicatId, service_order_id, type_service), 
                    Message = "" }
                );
                //response.Result = map;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "", Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }

        // Get: Get Service Record
        [HttpGet("GetSingleAirportTransportationServicesById", Name = "GetSingleAirportTransportationServicesById")]
        public ActionResult GetSingleAirportTransportationServicesById([FromQuery] int service_id )
        {
            try
            {
                return StatusCode(200, new
                {
                    Success = true,
                    Result = _airportTransportationServicesRepository.GetSingleAirportTransportationServicesById(service_id),
                    Message = ""
                }
                );
                //response.Result = map;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "", Message = $"Something went wrong: { ex.Message.ToString() }" });
            }
        }


        [HttpDelete("DeleteReminderATS", Name = "DeleteReminderATS")]
        public ActionResult DeleteReminderATS([FromQuery] int id)
        {
            try
            {
                var find = _reminderAirportTransportationServicesRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _reminderAirportTransportationServicesRepository.Delete(_mapper.Map<ReminderAirportTransportationService>(find));
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

        [HttpDelete("DeleteDocumentATS", Name = "DeleteDocumentATS")]
        public ActionResult DeleteDocumentATS([FromQuery] int id)
        {
            try
            {
                var find = _documentAirportTransportationServicesRepository.Find(x => x.Id == id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _documentAirportTransportationServicesRepository.Delete(_mapper.Map<DocumentAirportTransportationService>(find));
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
        #endregion

        #region Home Finding

        //Put Update a Home Finding
        [HttpPut("PutHomeFinding", Name = "PutHomeFinding")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HomeFindingDto>> PutHomeFinding([FromBody] HomeFindingDto dto)
        {
            var response = new ApiResponse<HomeFindingDto>();
            try
            {
                foreach (var i in dto.DocumentHomeFindings)
                {
                    if (i.Id != 0)
                    {
                        if (i.FileRequest.Length > 150)
                        {
                            DocumentHomeFinding document = _homeFindingRepository.FindDocument(i.Id);
                            _utiltyRepository.DeleteFile(document.FileRequest);
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/HomeFinding/", i.FileExtension);
                        }
                    }
                    else
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/HomeFinding/", i.FileExtension);
                    }
                }

                HomeFinding hf = _homeFindingRepository.UpdateCustom(_mapper.Map<HomeFinding>(dto), dto.Id, dto.userId, dto.number_server);
                //change sr status by service in progress 
                // if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
                //  {
                var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 26); //es el id de home finding  en el catalogo de servicios 
              //  }

                response.Result = _mapper.Map<HomeFindingDto>(hf);
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

        // Get: Get Service Record
        [HttpGet("GetHomeFindingById", Name = "GetHomeFindingById")]
        public ActionResult<ApiResponse<HomeFindingSelectDto>> GetHomeFindingById([FromQuery] int id)
        {

            //var hl = _housingListRepository
            //        .GetAllIncluding(i => i.PropertyTypeNavigation)
            //        .Where(x => x.Service == id)
            //        .ToList(); 

            

            var response = new ApiResponse<HomeFindingSelectDto>();
            try
            {
                var map = _mapper.Map<HomeFindingSelectDto>(_homeFindingRepository.GetCustom(id));
                response.Result = map;

                //if (hl.Count < 1)
                //{
                //    var wos_id = _mapper.Map<StandaloneServiceWorkOrder>(_homeFindingRepository.GetAloneServiceWoId(map.WorkOrderServicesId));
                //    var wob_id = _mapper.Map<BundledServicesWorkOrder>(_homeFindingRepository.GetBundleServiceWoId(map.WorkOrderServicesId));
                //    var wo_id = 0;
                //    if (wos_id != null)
                //        wo_id = wos_id.WorkOrderId.Value;
                //    else
                //    {
                //        if (wob_id != null)
                //            wo_id = wob_id.WorkOrderId.Value;
                //    }

                //        HousingListDto dtoHL = new HousingListDto();
                //    dtoHL.HousingStatus = 7;
                //    dtoHL.Service = id;
                //    dtoHL.WorkOrder = wo_id;
                //    dtoHL.PropertyNo = 1;
                //    dtoHL.Currency = 2;
                //    dtoHL.CreatedBy = map.CreatedBy;
                //    dtoHL.UpdateBy = map.UpdateBy;
                //    dtoHL.PropertyType = 1;

                //  //  HousingList hlnew = _housingListRepository.Add(_mapper.Map<HousingList>(dtoHL));
                //}
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



        [HttpDelete("DeleteReminderHF", Name = "DeleteReminderHF")]
        public ActionResult DeleteReminderHF([FromQuery] int id)
        {
            try
            {
                var res = _homeFindingRepository.DeleteReminder(id);
                if (res)
                {
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

        [HttpDelete("DeleteDocumentHF", Name = "DeleteDocumentHF")]
        public ActionResult DeleteDocumentHF([FromQuery] int id)
        {
            try
            {
                var find = _homeFindingRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _homeFindingRepository.DeleteDocument(id);
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

        [HttpDelete("DeleteDocumentHousing", Name = "DeleteDocumentHousing")]
        public ActionResult DeleteDocumentHousing([FromQuery] int id)
        {
            try
            {
                _homeFindingRepository.DeleteDocumentHousing(id);
                return Ok(new { Success = true, Result = "Document was delete" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        #endregion

        #region Lease Renewal

        // Get: Get Lease Renewal By Id
        [HttpGet("GetLeaseRenewalById", Name = "GetLeaseRenewalById")]
        public ActionResult<ApiResponse<LeaseRenewalDto>> GetLeaseRenewalById([FromQuery] int id)
        {
            var response = new ApiResponse<LeaseRenewalDto>();
            try
            {
                var map = _mapper.Map<LeaseRenewalDto>(_leaseRenewalRepository.GetCustom(id));
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
        
        //Put Update Lease Renewal
        [HttpPut("PutLeaseRenewal", Name = "PutLeaseRenewal")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<LeaseRenewalDto>> PutLeaseRenewal([FromBody] LeaseRenewalDto dto)
        {
            var response = new ApiResponse<LeaseRenewalDto>();
            try
            {
                foreach (var i in dto.DocumentLeaseRenewals)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LeaseRenewal/", i.FileExtension);
                    }
                }

                LeaseRenewal hf = _leaseRenewalRepository.UpdateCustom(_mapper.Map<LeaseRenewal>(dto));

                //change sr status by service in progress 
              //  if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
              //  {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 27); //es el id de leae renewal en el catalogo de servicios 
              //  }

                response.Result = _mapper.Map<LeaseRenewalDto>(hf);
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
        
        [HttpDelete("DeleteReminderLeaseRenewal", Name = "DeleteReminderLeaseRenewal")]
        public ActionResult DeleteReminderLeaseRenewal([FromQuery] int id)
        {
            try
            {
                var res = _leaseRenewalRepository.DeleteReminder(id);
                if (res)
                {
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

        [HttpDelete("DeleteDocumentLeaseRenewal", Name = "DeleteDocumentLeaseRenewal")]
        public ActionResult DeleteDocumentLeaseRenewal([FromQuery] int id)
        {
            try
            {
                var find = _leaseRenewalRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _leaseRenewalRepository.DeleteDocument(id);
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
        
        // Get: Get Lease Renewal By Id
        [HttpGet("GetProperties", Name = "GetProperties")]
        public ActionResult GetProperties([FromQuery] int id)
        {
            try
            {
                var result = _housingListRepository.GetAll().Where(x => x.WorkOrderNavigation.ServiceRecordId == id)
                    .Select(s => new
                    {
                        s.Id,
                        s.Address,
                        s.PropertyNo
                    }).ToList();
                return StatusCode(202, new {Success = true, Result = result});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500,  new {Success = true, Result = 0, Message = ex.Message});
                
            }
        }
        
        // Get: Get Permanent Home
        [HttpGet("GetPermanentHome", Name = "GetPermanentHome")]
        public ActionResult<ApiResponse<HousingListDto>> GetPermanentHome([FromQuery] int id)
        {
            var response = new ApiResponse<HousingListDto>();
            try
            {
                var map = _mapper.Map<HousingListDto>(_housingListRepository.GetCustom(id));
                if (map.HousingStatus == 7 || map.HousingStatus == 8)
                {
                    response.Result = map;
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

            return Ok(response);
        }

        #endregion

        #region Home Sale

        // Get: Get Home Sale By Id
        [HttpGet("GetHomeSaleById", Name = "GetHomeSaleById")]
        public ActionResult<ApiResponse<HomeSaleDto>> GetHomeSaleById([FromQuery] int id)
        {
            var response = new ApiResponse<HomeSaleDto>();
            try
            {
                var map = _mapper.Map<HomeSaleDto>(_homeSaleRepository.GetCustom(id));
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
        
        //Put Update Home Sale
        [HttpPut("PutHomeSale", Name = "PutHomeSale")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HomeSaleDto>> PutHomeSale([FromBody] HomeSaleDto dto)
        {
            var response = new ApiResponse<HomeSaleDto>();
            try
            {
                foreach (var i in dto.DocumentHomeSales)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/LeaseRenewal/", i.FileExtension);
                    }
                }

                HomeSale hf = _homeSaleRepository.UpdateCustom(_mapper.Map<HomeSale>(dto), dto.Id);
                //change sr status by service in progress 
             //   if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
              //  {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 28); //es el id de home sale  en el catalogo de servicios 
              //  }

                response.Result = _mapper.Map<HomeSaleDto>(hf);
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
        
        [HttpDelete("DeleteReminderHomeSale", Name = "DeleteReminderHomeSale")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteHomeSale([FromQuery] int id)
        {
            try
            {
                var res = _homeSaleRepository.DeleteReminder(id);
                if (res)
                {
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
        
        [HttpDelete("DeleteDocumentHomeSale", Name = "DeleteDocumentHomeSale")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentHomeSale([FromQuery] int id)
        {
            try
            {
                var find = _leaseRenewalRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _homeSaleRepository.DeleteDocument(id);
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

        #endregion

        #region Home Purchase

        // Get: Get Home Purchase By Id
        [HttpGet("GetHomePurchaseById", Name = "GetHomePurchaseById")]
        public ActionResult<ApiResponse<HomePurchaseDto>> GetHomePurchaseById([FromQuery] int id)
        {
            var response = new ApiResponse<HomePurchaseDto>();
            try
            {
                var map = _mapper.Map<HomePurchaseDto>(_homePurchaseRepository.GetCustom(id));
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
        
        //Put Update Home Purchase
        [HttpPut("PutHomePurchase", Name = "PutHomePurchase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HomePurchaseDto>> PutHomePurchase([FromBody] HomePurchaseDto dto)
        {
            var response = new ApiResponse<HomePurchaseDto>();
            try
            {
                foreach (var i in dto.DocumentHomePurchases)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/HomePurchase/", i.FileExtension);
                    }
                }

                HomePurchase hf = _homePurchaseRepository.UpdateCustom(_mapper.Map<HomePurchase>(dto), dto.Id);
                //change sr status by service in progress 
               // if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
               // {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 29); //es el id de home purchase  en el catalogo de servicios 
               // }

                response.Result = _mapper.Map<HomePurchaseDto>(hf);
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
        
        [HttpDelete("DeleteReminderHomePurchase", Name = "DeleteReminderHomePurchase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteReminderHomePurchase([FromQuery] int id)
        {
            try
            {
                var res = _homePurchaseRepository.DeleteReminder(id);
                if (res)
                {
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
        
        [HttpDelete("DeleteDocumentHomePurchase", Name = "DeleteDocumentHomePurchase")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentHomePurchase([FromQuery] int id)
        {
            try
            {
                var find = _homePurchaseRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _homePurchaseRepository.DeleteDocument(id);
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

        #endregion

        #region Property Management

        // Get: Get Property Management By Id
        [HttpGet("GetPropertyManagementById", Name = "GetPropertyManagementById")]
        public ActionResult<ApiResponse<PropertyManagementDto>> GetPropertyManagementById([FromQuery] int id)
        {
            var response = new ApiResponse<PropertyManagementDto>();
            try
            {
                var map = _mapper.Map<PropertyManagementDto>(_propertyManagementRepository.GetCustom(id));
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
        
        //Put Update Home Purchase
        [HttpPut("PutPropertyManagement", Name = "PutPropertyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PropertyManagementDto>> PutPropertyManagement([FromBody] PropertyManagementDto dto)
        {
            var response = new ApiResponse<PropertyManagementDto>();
            try
            {
                foreach (var i in dto.DocumentPropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/PropertyManagement/", i.FileExtension);
                    }
                }
                
                foreach (var i in dto.DocumentReportIssuePropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.FilePath = _utiltyRepository.UploadImageBase64(i.FilePath, "Files/PropertyManagement/", i.FileExtension);
                    }
                }
                
                foreach (var i in dto.PhotoInspectionPropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyManagement/", i.PhotoExtension);
                    }
                }
                
                foreach (var i in dto.PhotoPropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyManagement/", i.PhotoExtension);
                    }
                }
                
                foreach (var i in dto.PhotoBillPropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyManagement/", i.PhotoExtension);
                    }
                }
                
                foreach (var i in dto.PhotoMailPropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyManagement/", i.PhotoExtension);
                    }
                }
                
                foreach (var i in dto.PhotoReportIssuePropertyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyManagement/", i.PhotoExtension);
                    }
                }

                PropertyManagement management = _propertyManagementRepository.UpdateCustom(_mapper.Map<PropertyManagement>(dto), dto.Id);
                //change sr status by service in progress 
             //   if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
             //   {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 30); //es el id de property mgt en el catalogo de servicios 
              //  }

                response.Result = _mapper.Map<PropertyManagementDto>(management);
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
        
        [HttpDelete("DeleteReminderPropertyManagement", Name = "DeleteReminderPropertyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteReminderPropertyManagement([FromQuery] int id)
        {
            try
            {
                var res = _propertyManagementRepository.DeleteReminder(id);
                if (res)
                {
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
        
        [HttpDelete("DeleteDocumentPropertyManagement", Name = "DeleteDocumentPropertyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentPropertyManagement([FromQuery] int id)
        {
            try
            {
                var find = _propertyManagementRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _propertyManagementRepository.DeleteDocument(find);
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
        
        [HttpDelete("DeletePhotoPropertyManagement", Name = "DeletePhotoPropertyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeletePhotoPropertyManagement([FromQuery] int id, [FromQuery] int type)
        {
            try
            {
                var find = _propertyManagementRepository.FindPhoto(id, type);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.Item2);
                    _propertyManagementRepository.DeletePhoto(find.Item1, type);
                    return Ok(new { Success = true, Result = "Photo was delete" });
                }
                else
                {
                    return NotFound(new { Success = false, Result = "Photo Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500);
            }
        }

        #endregion

        #region Other

        // Get: Get Other By Id
        [HttpGet("GetOtherById", Name = "GetOtherById")]
        public ActionResult<ApiResponse<OtherDto>> GetOtherById([FromQuery] int id)
        {
            var response = new ApiResponse<OtherDto>();
            try
            {
                var map = _mapper.Map<OtherDto>(_otherRepository.GetCustom(id));
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
        
        //Put Update Other
        [HttpPut("PutOther", Name = "PutOther")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<OtherDto>> PutOther([FromBody] OtherDto dto)
        {
            var response = new ApiResponse<OtherDto>();
            try
            {
                foreach (var i in dto.DocumentOthers)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Other/", i.FileExtension);
                    }
                }

                Other management = _otherRepository.UpdateCustom(_mapper.Map<Other>(dto), dto.Id);

                //change sr status by service in progress 
             //   if(dto.StatusId != 1 && dto.StatusId != 38) // Pendig Asign y pendig accept  
              //  {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 31); //es el id de other en el catalogo de servicios 
              //  }
                    

                response.Result = _mapper.Map<OtherDto>(management);
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
        
        [HttpDelete("DeleteOther", Name = "DeleteOther")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteOther([FromQuery] int id)
        {
            try
            {
                var res = _otherRepository.DeleteReminder(id);
                if (res)
                {
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
        
        [HttpDelete("DeleteDocumentOther", Name = "DeleteDocumentOther")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentOther([FromQuery] int id)
        {
            try
            {
                var find = _otherRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _otherRepository.DeleteDocument(find);
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

        #endregion

        #region Tenancy Management

        // Get: Get Tenancy Management By Id
        [HttpGet("TenancyManagementById", Name = "TenancyManagementById")]
        public ActionResult<ApiResponse<TenancyManagementDto>> TenancyManagementById([FromQuery] int id)
        {
            var response = new ApiResponse<TenancyManagementDto>();
            try
            {
                var map = _mapper.Map<TenancyManagementDto>(_tenancyManagementRepository.GetCustom(id));
                map.EventTables = _tenancyManagementRepository.GetReportAnEventTable(id);
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
        
        //Put Update TenancyManagement
        [HttpPut("TenancyManagement", Name = "TenancyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<TenancyManagementDto>> PutTenancyManagement([FromBody] TenancyManagementDto dto)
        {
            var response = new ApiResponse<TenancyManagementDto>();
            try
            {
                foreach (var i in dto.DocumentTenancyManagements)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/TenancyManagement/", i.FileExtension);
                    }
                }

                foreach (var reportAnEvent in dto.ReportAnEvents)
                {
                    foreach (var consultantPhoto in reportAnEvent.SupplierConsultantPhotos)
                    {
                        if (consultantPhoto.Id == 0)
                        {
                            consultantPhoto.PhotoPath = _utiltyRepository.UploadImageBase64(consultantPhoto.PhotoPath, "Files/TenancyManagement/", consultantPhoto.PhotoExtension);
                        }
                    }
                    foreach (var assignedPhoto in reportAnEvent.AssignedPhotos)
                    {
                        if (assignedPhoto.Id == 0)
                        {
                            assignedPhoto.PhotoPath = _utiltyRepository.UploadImageBase64(assignedPhoto.PhotoPath, "Files/TenancyManagement/", assignedPhoto.PhotoExtension);
                        }
                    }
                }

                TenancyManagement management = _tenancyManagementRepository.UpdateCustom(_mapper.Map<TenancyManagement>(dto), dto.Id);
                //change sr status by service in progress 
              //  if (dto.StatusId != 1 && dto.StatusId != 38) // In progres 
              //  {
                    var new_status = _utiltyRepository.change_sr_status_byservice_id(dto.Id, 32); //es el id de tenancy mgt en el catalogo de servicios 
               // }

                response.Result = _mapper.Map<TenancyManagementDto>(management);
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
        
        [HttpDelete("Reminder/TenancyManagement", Name = "Reminder/TenancyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteTenancyManagement([FromQuery] int id)
        {
            try
            {
                var res = _tenancyManagementRepository.DeleteReminder(id);
                if (res)
                {
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
        
        [HttpDelete("Document/TenancyManagement", Name = "DeleteDocumentTenancyManagement")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteDocumentTenancyManagement([FromQuery] int id)
        {
            try
            {
                var find = _tenancyManagementRepository.FindDocument(id);
                if (find != null)
                {
                    _utiltyRepository.DeleteFile(find.FileRequest);
                    _tenancyManagementRepository.DeleteDocument(find);
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
        
        [HttpPost("Add/ReportAnEvent", Name = "Add/ReportAnEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReportAnEventDto>> AddReportAnEvent([FromBody] ReportAnEventDto dto)
        {
            var response = new ApiResponse<ReportAnEventDto>();
            try
            {
                foreach (var consultantPhoto in dto.SupplierConsultantPhotos)
                {
                    if (consultantPhoto.Id == 0)
                    {
                        consultantPhoto.PhotoPath = _utiltyRepository.UploadImageBase64(consultantPhoto.PhotoPath, "Files/TenancyManagement/", consultantPhoto.PhotoExtension);
                    }
                }
                foreach (var assignedPhoto in dto.AssignedPhotos)
                {
                    if (assignedPhoto.Id == 0)
                    {
                        assignedPhoto.PhotoPath = _utiltyRepository.UploadImageBase64(assignedPhoto.PhotoPath, "Files/TenancyManagement/", assignedPhoto.PhotoExtension);
                    }
                }

                ReportAnEvent management = _reportAnEventRepository.Add(_mapper.Map<ReportAnEvent>(dto));

                response.Result = _mapper.Map<ReportAnEventDto>(management);
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
        
        [HttpPut("Edit/ReportAnEvent", Name = "Edit/ReportAnEvent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ReportAnEventDto>> EditReportAnEvent([FromBody] ReportAnEventDto dto)
        {
            var response = new ApiResponse<ReportAnEventDto>();
            try
            {
                foreach (var consultantPhoto in dto.SupplierConsultantPhotos)
                {
                    if (consultantPhoto.Id == 0)
                    {
                        consultantPhoto.PhotoPath = _utiltyRepository.UploadImageBase64(consultantPhoto.PhotoPath, "Files/TenancyManagement/", consultantPhoto.PhotoExtension);
                    }
                }
                foreach (var assignedPhoto in dto.AssignedPhotos)
                {
                    if (assignedPhoto.Id == 0)
                    {
                        assignedPhoto.PhotoPath = _utiltyRepository.UploadImageBase64(assignedPhoto.PhotoPath, "Files/TenancyManagement/", assignedPhoto.PhotoExtension);
                    }
                }

                ReportAnEvent management = _reportAnEventRepository.UpdateCustom(_mapper.Map<ReportAnEvent>(dto), dto.Id);

                response.Result = _mapper.Map<ReportAnEventDto>(management);
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
        
        [HttpDelete("Photo/Assigned", Name = "PhotoAssigned")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult PhotoAssigned([FromQuery] int id)
        {
            try
            {
                var res = _reportAnEventRepository.DeleteAssignedPhoto(id);
                if (res)
                {
                    return Ok(new { Success = true, Result = "Photo was delete" });
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
        
        [HttpDelete("Photo/SupplierConsultant", Name = "PhotoSupplierConsultant")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult PhotoSupplierConsultant([FromQuery] int id)
        {
            try
            {
                var res = _reportAnEventRepository.DeleteSupplierConsultantPhoto(id);
                if (res)
                {
                    return Ok(new { Success = true, Result = "Photo was delete" });
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

        #endregion
        
    }
}
