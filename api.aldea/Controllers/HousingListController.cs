using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.HousingSpecification;
using api.premier.Models.PropertyReport;
using api.premier.Models.RentalFurnitureCoordination;
using api.premier.Models.WorkOrder;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.AssigneeName;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.Reports;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Repository.Utility;
using biz.premier.Repository.WorkOrder;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousingListController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IHousingListRepository _housingListRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        private readonly IWorkOrderRepository _serviceOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssigneeNameRepository _assigneeNameRepository;
        private readonly IReports _reports;

        //  private readonly IStandaloneServiceWorkOrderRepository _StandaloneServiceWorkOrderRepository;

        public HousingListController(IHousingListRepository housingListRepository, IMapper mapper, ILoggerManager logger, IUtiltyRepository utiltyRepository,
                                     INotificationSystemRepository notificationSystemRepository, ICatNotificationSystemTypeRepository notificationSystemTypeRepository,
                                     IWorkOrderRepository serviceOrderRepository, IUserRepository userRepository,
                                     IAssigneeNameRepository assigneeNameRepository, IReports reports //, IStandaloneServiceWorkOrderRepository standaloneServiceWorkOrderRepository
            )
        {
            _mapper = mapper;
            _logger = logger;
            _housingListRepository = housingListRepository;
            _utiltyRepository = utiltyRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
            _serviceOrderRepository = serviceOrderRepository;
            _userRepository = userRepository;
            _assigneeNameRepository = assigneeNameRepository;
            _reports = reports;
            // _StandaloneServiceWorkOrderRepository = standaloneServiceWorkOrderRepository;
        }

        //Post Create a new Housing
        [HttpPost("PostHousing", Name = "PostHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListDto>> PostHousing([FromBody] HousingListDto dto)
        {
            var response = new ApiResponse<HousingListDto>();
            try
            {
                //si se pone en estatus sent o mayor se setea el valor wassended
                if (dto.HousingStatus != 3 && dto.HousingStatus != 10)
                {
                    dto.Wassended = true;
                }


                dto.CreatedDate = DateTime.Now;
                int countHouse = _housingListRepository.FindAll(x => x.WorkOrder == dto.WorkOrder).Count();
                dto.PropertyNo = countHouse + 1;
                foreach (var i in dto.DocumentHousings)
                {
                    if (i.Id == 0)
                    {
                        i.FilePath = _utiltyRepository.UploadImageBase64(i.FilePath, "Files/Housing/", i.FileExtension);
                    }
                }

              //  var atributos_generales = _utiltyRepository.()


                ///////////////////////////////// agergar objetos del leasse sumary 

                ContractDetailDto contracts = new ContractDetailDto();
                contracts.CreatedDate = DateTime.Now;
                contracts.ContractDetailId = dto.Id;
                contracts.IdServiceDetail = dto.IdServiceDetail;
                contracts.WorkOrderServicesId = dto.WorkOrderServicesId;
                contracts.CatCategoryId = dto.CatCategoryId;
                contracts.CatCategoryText = dto.CatCategoryText;
                dto.ContractDetails.Add(contracts);

                RenewalDetailHomeDto renewal = new RenewalDetailHomeDto();
                renewal.CreatedDate = DateTime.Now;
                renewal.Id = dto.Id;
                renewal.IdServiceDetail = dto.IdServiceDetail;
                dto.RenewalDetailHomes.Add(renewal);

                LandlordHeaderDetailsHomeDto land = new LandlordHeaderDetailsHomeDto();
                land.CreatedDate = DateTime.Now;
                land.HousingListId = dto.Id;
                land.IdServiceDetail = dto.IdServiceDetail;
                dto.LandlordHeaderDetailsHomes.Add(land);

                DepartureDetailsHomeDto dep = new DepartureDetailsHomeDto();
                dep.CreatedDate = DateTime.Now;
                dep.Id = dto.Id;
                dep.IdServiceDetail = dto.IdServiceDetail;
                dto.DepartureDetailsHomes.Add(dep);

                GroupPaymentsHousingDto gp = new GroupPaymentsHousingDto();
                gp.CreatedDate = DateTime.Now;
                gp.IdServiceDetail = dto.IdServiceDetail;
                gp.HousingListId = dto.Id;
                dto.GroupPaymnetsHousings.Add(gp);

                GroupCostSavingDto gc = new GroupCostSavingDto();
                gc.CreatedDate = DateTime.Now;
                gc.IdServiceDetail = dto.IdServiceDetail;
                gc.HousingListId = dto.Id;
                dto.GroupCostSavings.Add(gc);


                // Move In 
                PropertyReportDto pr = new PropertyReportDto();
                pr.CreatedDate = DateTime.Now;
                pr.HousingList = dto.Id;
                pr.PropertyInspection = 1;
                pr.IdServiceDetail = dto.IdServiceDetail;
                pr.IdStatus = 1; // Pending
                // PropertyInspection == 1 Es Movin == 2 Mov Out
                //public virtual ICollection<Attendee> Attendees { get; set; }
                //public virtual ICollection<KeyInventory> KeyInventories { get; set; }
                //public virtual ICollection<PropertyReportSection> PropertyReportSections { get; set; }
                dto.PropertyReports.Add(pr);


                // Move Out
                PropertyReportDto pro = new PropertyReportDto();
                pro.CreatedDate = DateTime.Now;
                pro.HousingList = dto.Id;
                pro.PropertyInspection = 2;
                pr.IdStatus = 1; // Pending
                pro.IdServiceDetail = dto.IdServiceDetail;
                dto.PropertyReports.Add(pro);


                //Grp I&R

                GroupIrDto gir = new GroupIrDto();
                gir.CreatedDate = DateTime.Now;
                gir.IdServiceDetail = dto.IdServiceDetail;
                gir.HousingListId = dto.Id;
                gir.IdStatus = 1; // Pending
                gir.WorkOrderServicesId = dto.WorkOrderServicesId;
                gir.CatCategoryId = dto.CatCategoryId;
                gir.CatCategoryText = dto.CatCategoryText;
                dto.GroupIrs.Add(gir);


                HousingList hl = _housingListRepository.Add(_mapper.Map<HousingList>(dto));

                if (dto.HousingStatus == 7) // si es permanente la que se agrega , si hay alguna otra permanente le quita ese estatus
                {
                    var permanet_hms = _housingListRepository.FindAll(s => s.IdServiceDetail == dto.IdServiceDetail && s.HousingStatus == 7 && s.Id != hl.Id); //casas permamentes 
                    foreach (var i in permanet_hms)
                    {
                        i.HousingStatus = 5;
                        _housingListRepository.Update(i, i.Id);
                    }
                }

                var coordinator = _notificationSystemRepository.GetCoordinator(dto.WorkOrder.Value);

                if (coordinator.Item2 > 0)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = coordinator.Item1,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = dto.CreatedBy,
                        UserTo = coordinator.Item2,
                        NotificationType = 32,
                        Description = $"{_notificationSystemTypeRepository.Find(x => x.Id == 32).Type}: " + dto.CatCategoryText,
                        Color = "#f06689",
                        CreatedDate = DateTime.Now
                    });


                    _notificationSystemRepository.SendNotificationAsync(
                        coordinator.Item2,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 32).Type,
                        "In " + dto.CatCategoryText,
                        2
                    );
                }


                response.Result = _mapper.Map<HousingListDto>(hl);
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


        //Post SendNotificationFront
        [HttpPost("SendNotificationsFront", Name = "SendNotificationsFront")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<notificationfront>> SendNotificationsFront([FromBody] notificationfront dto)
        {
            var response = new ApiResponse<notificationfront>();
            try
            {

                var coordinator = _notificationSystemRepository.GetCoordinator(dto.WorkOrder);

                if (coordinator.Item2 > 0)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = coordinator.Item1,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = dto.CreatedBy,
                        UserTo = coordinator.Item2,
                        NotificationType = dto.NotificationType,
                        Description = $"{_notificationSystemTypeRepository.Find(x => x.Id == dto.NotificationType).Type}: " + dto.Detail  ,
                        Color = "#f06689",
                        CreatedDate = DateTime.Now
                    });


                    _notificationSystemRepository.SendNotificationAsync(
                        coordinator.Item2,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == dto.NotificationType).Type,
                        dto.Detail,
                        2
                    );
                }
                response.Result = _mapper.Map<notificationfront>(dto);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Error al enviar la notificación: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }



        public class notificationfront
        {
            public int WorkOrder { get; set; }
            public int CreatedBy { get; set; }
            public string Detail { get; set; }
            public int NotificationType { get; set; }
            public int To { get; set; } // 1: Coordiandor de la SL , 2: Coordinadores de ambas SL , 3: Coordiandores y Manager 
        }


//Get Create a new Housing
    [HttpGet("GetLeaseInspectionsVersions", Name = "GetLeaseInspectionsVersions")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListSelectDto>> GetHousing([FromQuery] int id_service_detail, int id_catCategoryId, int housing_list_id)
        {
            var response = new ApiResponse<LeaseInspectionsVersions>();
            try
            {
                var  respuesta = new LeaseInspectionsVersions();

                 respuesta.lease_versions = _housingListRepository.GetLeaseVersions( id_service_detail,  id_catCategoryId,  housing_list_id);
                 respuesta.ins_rep_versions = _housingListRepository.GetInspectionsVersions(id_service_detail, id_catCategoryId, housing_list_id);

                foreach(var v in respuesta.lease_versions ){
                    if (v.IdServiceDetail == id_service_detail)
                        v.CatCategoryText = v.CatCategoryText + " (Current)";
                }

                foreach (var v in respuesta.ins_rep_versions)
                {
                    if (v.IdServiceDetail == id_service_detail)
                        v.CatCategoryText = v.CatCategoryText + " (Current)";
                }

                var result_ = respuesta;
                response.Result = result_;
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

        class LeaseInspectionsVersions
        {
            public List<ContractDetail> lease_versions { get; set; }
            public List<GroupIr> ins_rep_versions { get; set; }
        }

        //Put Create a new Housing
        [HttpPut("PutHousing", Name = "PutHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListDto>> PutHousing([FromBody] HousingListDto dto)
        {
            var response = new ApiResponse<HousingListDto>();

            //si se pone en estatus sent o mayor se setea el valor wassended
            if(dto.HousingStatus != 3 && dto.HousingStatus != 10)
            {
                dto.Wassended = true;
            }

            try
            {
                dto.UpdatedDate = DateTime.Now;
                foreach (var i in dto.DocumentHousings)
                {
                    if (i.Id == 0)
                    {
                        i.FilePath = _utiltyRepository.UploadImageBase64(i.FilePath, "Files/Housing/", i.FileExtension);
                    }
                }
                var find = _housingListRepository.Find(x => x.Id == dto.Id);
                //if ((dto.HousingStatus.Value == 4 || dto.HousingStatus.Value == 5 || dto.HousingStatus.Value == 6) 
                //    && (find.HousingStatus.Value != 4 || find.HousingStatus.Value != 5 || find.HousingStatus.Value != 6))
                if (dto.HousingStatus.Value == 7  &&  find.HousingStatus.Value != 7)
                {
                    var coordinator = _notificationSystemRepository.GetCoordinator(dto.WorkOrder.Value);

                    if(coordinator.Item2 > 0)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Archive = false,
                            View = false,
                            ServiceRecord = coordinator.Item1,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = dto.CreatedBy,
                            UserTo = coordinator.Item2,
                            NotificationType = 30,
                            Description = "Property with Address " + dto.Address  + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(dto.HousingStatus.Value),
                            Color = "#67757c",
                            CreatedDate = DateTime.Now
                        });


                        _notificationSystemRepository.SendNotificationAsync(
                            coordinator.Item2,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 30).Type + " | SR-" + coordinator.Item1.ToString(),
                            "Property with Address " + dto.Address  + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(dto.HousingStatus.Value),
                            2
                        );
                    }
                }
                else if (dto.HousingStatus.Value != find.HousingStatus.Value  )
                    {
                    var coordinator = _notificationSystemRepository.GetCoordinator(dto.WorkOrder.Value);
                    

                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = coordinator.Item1,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = dto.CreatedBy,
                        UserTo = coordinator.Item2,
                        NotificationType = 33,
                        Description = "Property with Address " + dto.Address  + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(dto.HousingStatus.Value),
                        Color = "#67757c",
                        CreatedDate = DateTime.Now
                    });


                    _notificationSystemRepository.SendNotificationAsync(
                        coordinator.Item2,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 33).Type + " | SR-" + coordinator.Item1.ToString(),
                        "Property with Address " + dto.Address  + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(dto.HousingStatus.Value),
                        2
                    );

                }


                HousingList hl = _housingListRepository.UpdateCustom(_mapper.Map<HousingList>(dto), dto.Id);
                response.Result = _mapper.Map<HousingListDto>(hl);
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


        //Put Create a new Housing
        [HttpPut("VisitHousing", Name = "VisitHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListDto>> VisitHousing([FromBody] int id_housing)
        {
            var response = new ApiResponse<HousingListDto>();

            try
            {
                
                var find = _housingListRepository.Find(x => x.Id == id_housing);

                find.VisitDate = DateTime.Now;
                find.VisitTime = DateTime.Now.TimeOfDay.Hours.ToString()  + ":" + DateTime.Now.TimeOfDay.Minutes.ToString();

                if( find.HousingStatus != 7 )
                {
                    find.HousingStatus = 5;
                }

                if (find.HousingStatus.Value == 7 && find.HousingStatus.Value != 7)
                {
                    var coordinator = _notificationSystemRepository.GetCoordinator(find.WorkOrder.Value);

                    if (coordinator.Item2 > 0)
                    {
                        _notificationSystemRepository.Add(new NotificationSystem()
                        {
                            Archive = false,
                            View = false,
                            ServiceRecord = coordinator.Item1,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = find.CreatedBy,
                            UserTo = coordinator.Item2,
                            NotificationType = 33,
                            Description = "Property with Address " + find.Address + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(find.HousingStatus.Value),
                            Color = "#67757c",
                            CreatedDate = DateTime.Now
                        });


                        _notificationSystemRepository.SendNotificationAsync(
                            coordinator.Item2,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 30).Type + " | SR-" + coordinator.Item1.ToString(),
                            "Property with Address " + find.Address + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(find.HousingStatus.Value),
                            2
                        );
                    }
                }
                else if (find.HousingStatus.Value != find.HousingStatus.Value)
                {
                    var coordinator = _notificationSystemRepository.GetCoordinator(find.WorkOrder.Value);


                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = coordinator.Item1,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = find.CreatedBy,
                        UserTo = coordinator.Item2,
                        NotificationType = 33,
                        Description = "Property with Address " + find.Address + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(find.HousingStatus.Value),
                        Color = "#67757c",
                        CreatedDate = DateTime.Now
                    });


                    _notificationSystemRepository.SendNotificationAsync(
                        coordinator.Item2,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 33).Type + " | SR-" + coordinator.Item1.ToString(),
                        "Property with Address " + find.Address + ". change to: " + _notificationSystemRepository.GetTextStatusProperty(find.HousingStatus.Value),
                        2
                    );

                }


                HousingList hl = _housingListRepository.UpdateCustom(_mapper.Map<HousingList>(find), find.Id);
                response.Result = _mapper.Map<HousingListDto>(hl);
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

        //Post/Delete Housing
        [HttpPut("LogicDeleteHousing", Name = "LogicDeleteHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListDto>> LogicDeleteHousing([FromBody] int id)
        {
            var response = new ApiResponse<HousingListDto>();
            try
            {
                HousingList hl = new HousingList();

                // var hl = _housingListRepository.Find(x => x.Id == id);
                var d =  _housingListRepository.LogicDeleteHousing(id);

                response.Result = _mapper.Map<HousingListDto>(hl);
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

        //Get Create a new Housing
        [HttpGet("GetHousing", Name = "GetHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListSelectDto>> GetHousing([FromQuery] int key)
        {
            var response = new ApiResponse<HousingListSelectDto>();
            try
            {
                HousingList housingList = _housingListRepository.GetCustom(key);
                var result_ = _mapper.Map<HousingListSelectDto>(housingList);
                response.Result = result_;
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

        [HttpGet("GetHousingByService", Name = "GetHousingByService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<HousingListSelectDto>> GetHousingByService([FromQuery] int key , int servide_detail_id)
        {
            var response = new ApiResponse<HousingListSelectDto>();
            try
            {
             //   HousingList housingList = _housingListRepository.GetCustom(key);
                HousingList _hosuging = _housingListRepository.GetCustom_new(key, servide_detail_id);

                //////////////////////////
                bool con_Cambios = false;

                if (_hosuging.RenewalDetailHomes.Count < 1)
                {
                    RenewalDetailHome renewal = new RenewalDetailHome();
                    renewal.Id = _hosuging.Id;
                    renewal.IdServiceDetail = servide_detail_id;
                    RenewalDetailHome hl = _housingListRepository.AddRenewalDetailHome(renewal);
                    _hosuging.RenewalDetailHomes.Add(hl);
                    con_Cambios = true;
                }

                if (_hosuging.ContractDetails.Count < 1)
                {
                    ContractDetail contracts = new ContractDetail();
                    contracts.ContractDetailId = _hosuging.Id;
                    contracts.IdServiceDetail = servide_detail_id;
                    ContractDetail hl = _housingListRepository.AddContractDetail(contracts);
                    _hosuging.ContractDetails.Add(hl);
                    con_Cambios = true;
                }

                //if (_hosuging.LandlordDetailsHomes.Count < 1)
                //{
                //    LandlordDetailsHome land = new LandlordDetailsHome();
                //    land.Id = _hosuging.Id;
                //    land.IdServiceDetail = servide_detail_id;
                //    LandlordDetailsHome hl = _housingListRepository.AddLandlordDetailsHome(land);
                //    _hosuging.LandlordDetailsHomes.Add(hl);
                //    con_Cambios = true;
                //}

                if (_hosuging.DepartureDetailsHomes.Count < 1)
                {
                    DepartureDetailsHome dep = new DepartureDetailsHome();
                    dep.CreatedDate = DateTime.Now;
                    dep.Id = _hosuging.Id;
                    dep.IdServiceDetail = servide_detail_id;
                    //dto.RenewalDetailHomes.Add(renewal);
                    DepartureDetailsHome hl = _housingListRepository.AddDepartureDetailHome(dep);
                    _hosuging.DepartureDetailsHomes.Add(hl);
                    con_Cambios = true;
                }

                HousingList housingList_ = new HousingList();
                housingList_ = _hosuging;
                //if (con_Cambios)
                //{
                //     housingList_ = _housingListRepository.Update(_hosuging, _hosuging.Id);

                //}
                //else
                //{
                //     housingList_ = _hosuging;

                //}
                //////////////////////////


                response.Result = _mapper.Map<HousingListSelectDto>(housingList_);
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



        [HttpGet("GetHistoricHousingByService", Name = "GetHistoricHousingByService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult  GetHistoricHousingByService([FromQuery] int key, int servide_detail_id)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                // HousingList housingList = _housingListRepository.GetCustom(key);
                // HousingList _hosuging = _housingListRepository.GetCustom_historic(key, servide_detail_id);
                var result  = _housingListRepository.GetHistoricHousingByTypeAndId(key, servide_detail_id);
                //////////////////////////
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            
        }

        [HttpGet("UpdateStatusHousingList", Name = "UpdateStatusHousingList")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult UpdateStatusHousingList([FromQuery] int idHl, [FromQuery] int status)
        {
            try
            {

                var result = _housingListRepository.UpdateStatusHousingList(idHl, status);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                return Ok(new { Success = false });
            }

        }

        [HttpGet("GetOnlyHomeDetail", Name = "GetOnlyHomeDetail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOnlyHomeDetail([FromQuery] int key)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {

                var result = _housingListRepository.GetOnlyHomeDetail(key);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpGet("GetHistoricHousingByServiceType", Name = "GetHistoricHousingByServiceType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetHistoricHousingByServiceType([FromQuery] int key, int servide_detail_id, int type)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                // HousingList housingList = _housingListRepository.GetCustom(key);
                // HousingList _hosuging = _housingListRepository.GetCustom_historic(key, servide_detail_id);

                //var result = _housingListRepository.GetHistoricHousingByTypeAndId(key, servide_detail_id,type);
                var result = _housingListRepository.GetHistoricHousingByServiceType(key, servide_detail_id, type);
                //////////////////////////
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //Get All Status Hustory
        [HttpGet("GetAllStatusHistory", Name = "GetAllStatusHistory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<HousingStatusHistorySelectDto>>> GetAllStatusHistory([FromQuery] int key)
        {
            var response = new ApiResponse<List<HousingStatusHistorySelectDto>>();
            try
            {
                var list = _housingListRepository.GetAllHistory(key);
                response.Result = _mapper.Map<List<HousingStatusHistorySelectDto>>(list);
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

        //Get Create a new Housing
        [HttpGet("GetAllHousing", Name = "GetAllHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllHousing([FromQuery] int key)
        {
            try
            {
                var hl = _housingListRepository
                    .GetAllIncluding(i => i.PropertyTypeNavigation, 
                                     u => u.CurrencyNavigation, 
                                     s => s.HousingStatusNavigation, 
                                     c => c.SupplierNavigation, 
                                     s => s.SupplierPartnerNavigation)
                    .Where(x => x.WorkOrder == key)
                    .ToList();
                var custom = hl.Select(s => new
                {
                    s.Id,
                    supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                    supplierCompany = s.SupplierNavigation == null ? null :s.SupplierPartnerNavigation.ComercialName,
                    s.PropertyNo,
                    PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                    s.Address,
                    s.Price,
                    Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                    Status = s.HousingStatusNavigation == null ? null :s.HousingStatusNavigation.Status
                }).ToList();
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetSegmentedHousing", Name = "GetSegmentedHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSegmentedHousing([FromQuery] int wo_id, int id_service_detail, int  shared )
        {
            try
            {
                // var sr_id_ = _housingListRepository.Find(x => x.IdServiceDetail == id_service_detail).WorkOrderNavigation.ServiceRecordId.Value;
               // var wo_id_ = _serviceOrderRepository.Find(x => x.Id == wo_id).ServiceRecordId;
                //var sr_id = _serviceOrderRepository.Find(x => x.Id == wo_id).ServiceRecordId;

                var hl = _housingListRepository
                    .GetAllIncluding(i => i.PropertyTypeNavigation,
                                     u => u.CurrencyNavigation,
                                     s => s.HousingStatusNavigation,
                                     c => c.SupplierNavigation,
                                     s => s.SupplierPartnerNavigation)
                    .Where(x => x.WorkOrder == wo_id
                    && x.IdServiceDetail == (shared == 1 ? x.IdServiceDetail : id_service_detail)
                    && x.Shared == shared) //si no es servicio que comparta lista solo se trae las de ese servicio
                     .ToList();
                var custom = hl.Select(s => new
                {
                    s.Id,
                    supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                    supplierCompany = s.SupplierNavigation == null ? null : s.SupplierPartnerNavigation.ComercialName,
                    s.PropertyNo,
                    PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                    s.Address,
                    s.Price,
                    s.Neighborhood,
                    s.VisitTime,
                    Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                    Status = s.HousingStatusNavigation == null ? null : s.HousingStatusNavigation.Status
                }).ToList();
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("GetHomeFindingHousingList", Name = "GetHomeFindingHousingList")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetHomeFindingHousingList([FromQuery] int id_service_detail)
        {
            try
            {
            
                var custom = _housingListRepository.GetHomeFindingHousingList(id_service_detail);

                return Ok(new { Success = true, custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }


        [HttpGet("GetPermanentHousingList", Name = "GetPermanentHousingList")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetPermanentHousingList([FromQuery] int id_sr)
        {
            try
            {

                var hl = _housingListRepository
                    .GetAllIncluding(i => i.PropertyTypeNavigation,
                                     u => u.CurrencyNavigation,
                                     s => s.HousingStatusNavigation,
                                     c => c.SupplierNavigation,
                                     s => s.SupplierPartnerNavigation,
                                     c => c.RelAppointmentHousingLists)
                    .Where(x => x.WorkOrderNavigation.ServiceRecordId == id_sr && x.HousingStatus == 7)
                    .OrderByDescending(hl => hl.VisitDate).ThenBy(n => n.VisitTime)
                     .ToList();
                var custom = hl.Select(s => new
                {
                    s.Id,
                    supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                    supplierCompany = s.SupplierNavigation == null ? null : s.SupplierPartnerNavigation.ComercialName,
                    s.PropertyNo,
                    PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                    s.Address,
                    s.Price,
                    s.Neighborhood,
                    s.VisitTime,
                    Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                    Status = s.HousingStatusNavigation == null ? null : s.HousingStatusNavigation.Status,
                    s.Wassended,
                    s.Bedrooms,
                    s.Bathrooms,
                    s.ParkingSpaces,
                    Sise = 0,
                    AssigneComments = "",
                    RelAppointmentHousingLists = s.RelAppointmentHousingLists.Any()

                }).ToList();

                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }


        [HttpGet("GetHousingExcel", Name = "GetHousingExcel")]
        public ActionResult<ApiResponse<string>> GetHousingExcel(int id_service_detail)
        {
            var response = new ApiResponse<string>();

            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "Housing" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string xFull = Path.Combine(xPath, xNombre);
                _reports.createExcelHousing(id_service_detail, xFull);
                response.Result = "ok";

                Byte[] bytes = System.IO.File.ReadAllBytes(xFull);
                String base64 = Convert.ToBase64String(bytes);

                if (System.IO.File.Exists(xFull))
                    System.IO.File.Delete(xFull);

                response.Result = "ok";
                response.Message = base64;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = "Internal server error";
                _logger.LogError($"Something went wrong: {ex.ToString()}");
                return StatusCode(500, response);
            }

            return Ok(response);

        }


        [HttpGet("GetPermanentHousing", Name = "GetPermanentHousing")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetPermanentHousing([FromQuery] int wo_id, int id_service_detail, int shared)
        {
            try
            {

                // var sr_id = _StandaloneServiceWorkOrderRepository.Find(x => x.WorkOrderServiceId == wos_id).WorkOrder.ServiceRecordId;
                // var  = _StandaloneServiceWorkOrderRepository.Find(x => x.WorkOrderServiceId == wos_id).WorkOrderId.Value;
                var sr_id = _serviceOrderRepository.Find(x => x.Id == wo_id).ServiceRecordId;

                var hl = _housingListRepository
                    .GetAllIncluding(i => i.PropertyTypeNavigation,
                                     u => u.CurrencyNavigation,
                                     s => s.HousingStatusNavigation,
                                     c => c.SupplierNavigation,
                                     s => s.SupplierPartnerNavigation)
                    .Where(x => x.HousingStatus ==  7 
                    && x.WorkOrderNavigation.ServiceRecordId == sr_id
                    && x.IdServiceDetail == (shared == 1 ? x.IdServiceDetail : id_service_detail)
                    && x.Shared == shared) //si no es servicio que comparta lista solo se trae las de ese servicio
                     .ToList();
                var custom = hl.Select(s => new
                {
                    s.Id,
                    supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                    supplierCompany = s.SupplierNavigation == null ? null : s.SupplierPartnerNavigation.ComercialName,
                    s.PropertyNo,
                    PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                    s.Address,
                    s.Price,
                    Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                    Status = s.HousingStatusNavigation == null ? null : s.HousingStatusNavigation.Status
                }).ToList();
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        //Get Create a new Housing
        [HttpGet("GetAllHousing/{status}", Name = "GetAllHousing/Status")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAllHousing([FromQuery] int key, int? status)
        {
            try
            {
                var hl = _housingListRepository.GetAllIncluding(i => i.PropertyTypeNavigation, u => u.CurrencyNavigation, s => s.HousingStatusNavigation, c => c.SupplierNavigation, s => s.SupplierPartnerNavigation)
                    .Where(x => x.WorkOrder == key)
                    .ToList();
                var custom = hl.Select(s => new
                {
                    s.Id,
                    supplier = s.SupplierNavigation.ContactName,
                    supplierCompany = s.SupplierPartnerNavigation == null ? null: s.SupplierPartnerNavigation.ComercialName,
                    s.PropertyNo,
                    PropertyType = s.PropertyTypeNavigation?.PropertyType,
                    s.Address,
                    s.Price,
                    Currency = s.CurrencyNavigation?.Currency,
                    s.HousingStatusNavigation?.Status,
                    s.HousingStatus
                }).ToList();
                if (status.HasValue)
                {
                    custom = custom.Where(x => x.HousingStatus.Value == status.Value).ToList();
                }
                return Ok(new { Success = true, Message = custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        //Post Create a new Departure
        [HttpPost("PostDepartureDetails", Name = "PostDepartureDetails")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DepartureDetailsHomeDto>> PostDeparture([FromBody] DepartureDetailsHomeDto dto)
        {
            var response = new ApiResponse<DepartureDetailsHomeDto>();
            try
            {
                var housing =  _housingListRepository.GetAllIncluding(x => x.DepartureDetailsHomes).SingleOrDefault(x => x.Id == dto.Id);
                dto.CreatedDate = DateTime.Now;
                //if(housing.DepartureDetailsHomes.Id == 0 || housing.DepartureDetailsHomes.Id == dto.Id)
                //{
                //    response.Result = null;
                //    response.Success = false;
                //    response.Message = "This Departure Detail Already Exist.";
                //    return StatusCode(409, response);
                //}
                DepartureDetailsHome hl = _housingListRepository.AddDepartureDetailHome(_mapper.Map<DepartureDetailsHome>(dto));
                response.Result = _mapper.Map<DepartureDetailsHomeDto>(hl);
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

        //Put Edit Departure
        [HttpPut("PutDepartureDetails", Name = "PutDepartureDetails")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DepartureDetailsHomeDto>> PutDepartureDetails([FromBody] DepartureDetailsHomeDto dto)
        {
            var response = new ApiResponse<DepartureDetailsHomeDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                DepartureDetailsHome hl = _housingListRepository.UpdateDepartureDetailHome(_mapper.Map<DepartureDetailsHome>(dto));
                response.Result = _mapper.Map<DepartureDetailsHomeDto>(hl);
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

        //Post Create a new Renewal Detail
        [HttpPost("PostRenewalDetail", Name = "PostRenewalDetail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RenewalDetailHomeDto>> PostRenewalDetail([FromBody] RenewalDetailHomeDto dto)
        {
            var response = new ApiResponse<RenewalDetailHomeDto>();
            try
            {
                var housing = _housingListRepository.GetAllIncluding(x => x.RenewalDetailHomes).SingleOrDefault(x => x.Id == dto.Id);
                dto.CreatedDate = DateTime.Now;
                //if (housing.RenewalDetailHomes != null && housing.RenewalDetailHomes.Id == dto.Id)
                //{
                //    response.Result = null;
                //    response.Success = false;
                //    response.Message = "This Renewal Detail Already Exist.";
                //    return StatusCode(409, response);
                //}
                RenewalDetailHome hl = _housingListRepository.AddRenewalDetailHome(_mapper.Map<RenewalDetailHome>(dto));
                response.Result = _mapper.Map<RenewalDetailHomeDto>(hl);
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

        //Put Edit Renewal Detail
        [HttpPut("PutRenewalDetail", Name = "PutRenewalDetail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RenewalDetailHomeDto>> PutRenewalDetail([FromBody] RenewalDetailHomeDto dto)
        {
            var response = new ApiResponse<RenewalDetailHomeDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                RenewalDetailHome hl = _housingListRepository.UpdateRenewalDetailHome(_mapper.Map<RenewalDetailHome>(dto));
                response.Result = _mapper.Map<RenewalDetailHomeDto>(hl);
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

        //Post Create a new Cost Saving
        [HttpPost("PostCostSavingHome", Name = "PostCostSavingHome")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CostSavingHomeDto>> PostCostSaving([FromBody] CostSavingHomeDto dto)
        {
            var response = new ApiResponse<CostSavingHomeDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                CostSavingHome hl = _housingListRepository.AddCostSavingHome(_mapper.Map<CostSavingHome>(dto));
                response.Result = _mapper.Map<CostSavingHomeDto>(hl);
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

        //Put Edit Cost Saving
        [HttpPut("PutSavingHomeHome", Name = "PutSavingHomeHome")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CostSavingHomeDto>> PuttRenewalDetail([FromBody] CostSavingHomeDto dto)
        {
            var response = new ApiResponse<CostSavingHomeDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                CostSavingHome hl = _housingListRepository.UpdateCostSanvingHome(_mapper.Map<CostSavingHome>(dto));
                response.Result = _mapper.Map<CostSavingHomeDto>(hl);
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

        //Post Create a new Payment
        [HttpPost("PostPayment", Name = "PostPayment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentHousingDto>> PostPayment([FromBody] PaymentHousingDto dto)
        {
            var response = new ApiResponse<PaymentHousingDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                PaymentHousing hl = _housingListRepository.AddPaymentHousing(_mapper.Map<PaymentHousing>(dto));
                response.Result = _mapper.Map<PaymentHousingDto>(hl);
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


        //Post Create a new Payment Schooling
        [HttpPost("PostPaymentSchooling", Name = "PostPaymentSchooling")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<Models.WorkOrder.PaymentSchoolingDto>> PostPaymentSchooling([FromBody] Models.WorkOrder.PaymentSchoolingDto dto)
        {
            var response = new ApiResponse<Models.WorkOrder.PaymentSchoolingDto>();
            try
            {
                 
                PaymentSchooling hl = _housingListRepository.AddPaymentSchooling(_mapper.Map<PaymentSchooling>(dto));
                response.Result = _mapper.Map<Models.WorkOrder.PaymentSchoolingDto>(hl);
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

        //Post Create a new Payment Rental
        [HttpPost("PostPaymentRental", Name = "PostPaymentRental")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentRentalDto>> PostPaymentRental([FromBody] PaymentRentalDto dto)
        {
            var response = new ApiResponse<PaymentRentalDto>();
            try
            {

                PaymentRental hl = _housingListRepository.AddPaymentRental(_mapper.Map<PaymentRental>(dto));
                response.Result = _mapper.Map<PaymentRentalDto>(hl);
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

        //Put Edit payments  Saving
        [HttpPut("PutPayment", Name = "PutPayment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentHousingDto>> PutPayment([FromBody] PaymentHousingDto dto)
        {
            var response = new ApiResponse<PaymentHousingDto>();
            try
            {
                
                dto.CreatedDate = DateTime.Now;
                dto.PaymentTypeNavigation = null;
                PaymentHousing hl = _housingListRepository.UpdatePaymentHousing(_mapper.Map<PaymentHousing>(dto));
                response.Result = _mapper.Map<PaymentHousingDto>(hl);
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


        //Put Edit Cost PaymentSchooling
        [HttpPut("PutPaymentSchooling", Name = "PutPaymentSchooling")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<Models.WorkOrder.PaymentSchoolingDto>> PutPaymentSchooling([FromBody] Models.WorkOrder.PaymentSchoolingDto dto)
        {
            var response = new ApiResponse<Models.WorkOrder.PaymentSchoolingDto>();
            try
            {
                 
                PaymentSchooling hl = _housingListRepository.UpdatePaymentSchooling(_mapper.Map<PaymentSchooling>(dto));
                response.Result = _mapper.Map<Models.WorkOrder.PaymentSchoolingDto>(hl);
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


        //Put Edit Cost Payment Rental
        [HttpPut("PutPaymentRental", Name = "PutPaymentRental")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentRentalDto>> PutPaymentRental([FromBody] PaymentRentalDto dto)
        {
            var response = new ApiResponse<PaymentRentalDto>();
            try
            {

                PaymentRental hl = _housingListRepository.UpdatePaymentRental(_mapper.Map<PaymentRental>(dto));
                response.Result = _mapper.Map<PaymentRentalDto>(hl);
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

        //Post Create a new Payment
        [HttpPost("PostContractDetail", Name = "PostContractDetail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ContractDetailDto>> PostContractDetail([FromBody] ContractDetailDto dto)
        {
            var response = new ApiResponse<ContractDetailDto>();
            try
            {
                var housing = _housingListRepository.GetAllIncluding(x => x.ContractDetails).SingleOrDefault(x => x.Id == dto.ContractDetailId);
                dto.CreatedDate = DateTime.Now;
                //if (housing.RenewalDetailHome != null && housing.ContractDetail.ContractDetailId == dto.ContractDetailId)
                //{
                //    response.Result = null;
                //    response.Success = false;
                //    response.Message = "This Contract Detail Already Exist.";
                //    return StatusCode(409, response);
                //}
                ContractDetail hl = _housingListRepository.AddContractDetail(_mapper.Map<ContractDetail>(dto));
                response.Result = _mapper.Map<ContractDetailDto>(hl);
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

        //Put Edit Cost Saving
        [HttpPut("PutContractDetail", Name = "PutContractDetail")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ContractDetailDto>> PutContractDetail([FromBody] ContractDetailDto dto)
        {
            var response = new ApiResponse<ContractDetailDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                var cd= _mapper.Map<ContractDetail>(dto);
                ContractDetail hl = _housingListRepository.UpdateContractDetail(cd);
                response.Result = _mapper.Map<ContractDetailDto>(hl);
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

        //Post Create a new AddLandlordDetailsHome
        [HttpPost("PostLandlordDetailsHome", Name = "PostLandlordDetailsHome")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult PostLandlordDetailsHome([FromBody] LandlordDetailsHomeDto dto)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {
               // var housing = _housingListRepository.GetAllIncluding(x => x.LandlordDetailsHomes).SingleOrDefault(x => x.Id == dto.Id);
                dto.CreatedDate = DateTime.Now;
                //if (housing.RenewalDetailHomes != null && housing.LandlordDetailsHomes.Id == dto.Id)
                //{
                //    response.Result = null;
                //    response.Success = false;
                //    response.Message = "This Land Lord Already Exist.";
                //    return StatusCode(409, response);
                //}
                var hl = _housingListRepository.AddLandlordDetailsHome(_mapper.Map<LandlordDetailsHome>(dto));
                return Ok(new { Success = true, hl });

               
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            
        }

        //Put Edit LandlordDetailsHome
        [HttpPut("PutLandlordDetailsHome", Name = "PutLandlordDetailsHome")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<LandlordDetailsHomeDto>> PutLandlordDetailsHome([FromBody] LandlordDetailsHomeDto dto)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                LandlordDetailsHome hl = _housingListRepository.UpdateLandlordDetailsHome(_mapper.Map<LandlordDetailsHome>(dto));
                response.Result = _mapper.Map<LandlordDetailsHomeDto>(hl);
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

        //delete  banking detail 
        [HttpPut("DeleteBankingDetails", Name = "DeleteBankingDetails")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteBankingDetails([FromBody] int id_landlord)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {
                
                var result = _housingListRepository.DeleteBankingDetails(id_landlord);
                
                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }



        //delete expense 
        [HttpPut("DeleteExpense", Name = "DeleteExpense")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteExpense([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

               // var result = _housingListRepository.DeleteBankingDetails(id);
                var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        //delete paymnet lease sumary 
        [HttpPut("DeletePaymnetType", Name = "DeletePaymnetType")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeletePaymnetType([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                 var result = _housingListRepository.DeletePaymnetType(id);
               // var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpPut("deletePropertyReportSection", Name = "deletePropertyReportSection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult deletePropertyReportSection([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

               // var result = _housingListRepository.DeletePaymnetType(id);
                 var result = _housingListRepository.deletePropertyReportSection(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpPut("deleteSectionInventory", Name = "deleteSectionInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult deleteSectionInventory([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                 var result = _housingListRepository.deleteSectionInventory(id);
                //var result = _housingListRepository.deletePropertyReportSection(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpPut("AddSectionInventory", Name = "AddSectionInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddSectionInventory([FromBody] SectionInventory si)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                //var result = _housingListRepository.deleteSectionInventory(id);

                foreach (var i in si.PhotosInventories)
                {
                    if (i.Id != 0)
                    {
                        if (i.Photo.Length > 150)
                        {
                            PhotosInspec document = _housingListRepository.getPhotoInspecById(i.Id);
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/PhotosInventories/", "jpg");
                            _utiltyRepository.DeleteFile(document.Photo);
                        }
                    }
                    else
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/PhotosInventories/", "jpg");
                    }
                }

                var result = _housingListRepository.AddSectionInventory(si);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        [HttpPut("EditSectionInventory", Name = "EditSectionInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult EditSectionInventory([FromBody] SectionInventory si)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                //var result = _housingListRepository.deleteSectionInventory(id);
                foreach (var i in si.PhotosInventories)
                {
                    if (i.Id != 0)
                    {
                        if (i.Photo.Length > 150)
                        {
                            PhotosInspec document = _housingListRepository.getPhotoInspecById(i.Id);
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/PhotosInventories/", "jpg");
                            _utiltyRepository.DeleteFile(document.Photo);
                        }
                    }
                    else
                    {
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/PhotosInventories/", "jpg");
                    }
                }

               // var result = _housingListRepository.AddSectionInventory(si);
                var result = _housingListRepository.EditSectionInventory(si);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        // delet KeyInventory lease sumary 
        [HttpPut("deletKeyInventory", Name = "deletKeyInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult deletKeyInventory([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

               // var result = _housingListRepository.DeletePaymnetType(id);
                 var result = _housingListRepository.deletKeyInventory(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        // delet KeyInventory lease sumary 
        [HttpPut("deleteAttend", Name = "deleteAttend")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult deleteAttend([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                 var result = _housingListRepository.deleteAttend(id);
                //var result = _housingListRepository.deletKeyInventory(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        //delete cost savings lease 
        [HttpPut("deleteCostSavings", Name = "deleteCostSavings")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult deleteCostSavings([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                //var result = _housingListRepository.DeletePaymnetType(id);
                 var result = _housingListRepository.deleteCostSavings(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //add expense 
        [HttpPut("AddExpense", Name = "AddExpense")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddExpense([FromBody] PropertyExpense pe)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                 var result = _housingListRepository.AddExpense(pe);
                //var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //add expense 
        [HttpPut("EditExpense", Name = "EditExpense")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult EditExpense([FromBody] PropertyExpense pe)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

                var result = _housingListRepository.EditExpense(pe);
                //var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //add AddConsiderarion 
        [HttpPut("AddConsiderarion", Name = "AddConsiderarion")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddConsiderarion([FromBody] SpecialConsideration pe)
        {
            var response = new ApiResponse<SpecialConsideration>();
            try
            {

                var result = _housingListRepository.AddConsiderarion(pe);
                //var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //aEditConsideration 
        [HttpPut("EditConsideration", Name = "EditConsideration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult EditConsideration([FromBody] SpecialConsideration pe)
        {
            var response = new ApiResponse<SpecialConsideration>();
            try
            {

                var result = _housingListRepository.EditConsideration(pe);
                //var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //DeleteConsideration 
        [HttpPut("DeleteConsideration", Name = "DeleteConsideration")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeleteConsideration([FromBody] int id)
        {
            var response = new ApiResponse<SpecialConsideration>();
            try
            {

                var result = _housingListRepository.DeleteConsideration(id);
                //var result = _housingListRepository.DeleteExpense(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, response);
            }

        }

        [HttpPut("AddPropertyReportSection", Name = "AddPropertyReportSection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult AddPropertyReportSection([FromBody] PropertyReportSection prs)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {

               // var result = _housingListRepository.AddExpense(pe);
                var result = _housingListRepository.AddPropertyReportSection(prs);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //Put Edit LandlordHeaderDetailsHomeDto
        [HttpPut("LandlordHeaderDetailsHome", Name = "LandlordHeaderDetailsHome")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<LandlordHeaderDetailsHomeDto>> LandlordHeaderDetailsHome([FromBody] LandlordHeaderDetailsHome header)
        {
            var response = new ApiResponse<LandlordHeaderDetailsHomeDto>();
            try
            {
                header.UpdatedDate = DateTime.Now;
                LandlordHeaderDetailsHome hl = _housingListRepository.UpdateLandlordHeaderDetailsHome(_mapper.Map<LandlordHeaderDetailsHome>(header));
                response.Result = _mapper.Map<LandlordHeaderDetailsHomeDto>(hl);
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


        public class dtogetatt
        {
            public dtogetatt() { }
            public int idservicedetail { get; set; }
            public int propertyid { get; set; }
            public int srid { get; set; }
        }

        //Post Get All Attendees Titles 
        [HttpPost("GetAttendeesTitles", Name = "GetAttendeesTitles")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetAttendeesTitles([FromBody] dtogetatt dto)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
                var result = _housingListRepository.GetAttendeesTitles(dto.idservicedetail, dto.propertyid, dto.srid);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong in GetAttendeesTitles: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            
        }


        public class req_status_ir
        {
            public int status_id { get; set; }
            public int id { get; set; }
        }

        public class req_status_change
        {
            public int status_id { get; set; }
            public int id_permanent_home { get; set; }

            public int id_service_detail { get; set; }

            public int type { get; set; }
        }

        //Post edit status ir
        [HttpPost("save_ir_status", Name = "save_ir_status")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult save_ir_status([FromBody] req_status_ir dto)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
                
               // var result = _housingListRepository.GetAttendeesTitles(dto.idservicedetail, dto.propertyid, dto.srid);
                var result = _housingListRepository.save_ir_status(dto.status_id, dto.id);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong in GetAttendeesTitles: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        //Post edit status ir
        [HttpPost("save_ir_statusbyhousingid", Name = "save_ir_statusbyhousingid")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult save_ir_statusbyhousingid([FromBody] req_status_change dto)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {

                // var result = _housingListRepository.GetAttendeesTitles(dto.idservicedetail, dto.propertyid, dto.srid);
                //var result = _housingListRepository.save_ir_status(dto.status_id, dto.id);
                var result = _housingListRepository.save_ir_statusbyhousingid(dto.status_id, dto.type, dto.id_service_detail, dto.id_permanent_home);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong in GetAttendeesTitles: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        public class req_items
        {
            public req_items() { }

            public int section_id { get; set; }
        }


        //Post Get All Attendees Titles 
        [HttpPost("GetItemsInventoryBySectionId", Name = "GetItemsInventoryBySectionId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetItemsInventoryBySectionId([FromBody]  req_items req)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
                // var results = _housingListRepository.GetAttendeesTitles(dto.idservicedetail, dto.propertyid, dto.srid);
                var result = _housingListRepository.GetItemsSectionInventory(req.section_id);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong in GetAttendeesTitles: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        //Post Get All Attendees Titles 
        [HttpPost("GetItemsSectionInventory", Name = "GetItemsSectionInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetItemsSectionInventory([FromBody] int section)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
               // var results = _housingListRepository.GetAttendeesTitles(dto.idservicedetail, dto.propertyid, dto.srid);
                var result = _housingListRepository.GetItemsSectionInventory(section);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong in GetAttendeesTitles: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        //Post Create a new Attendee
        [HttpPost("PostAttendee", Name = "PostAttendee")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AttendeeDto>> PostAttendee([FromBody] AttendeeDto dto)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
                //var housing = _housingListRepository.GetAllIncluding(x => x.Inspections).SingleOrDefault(x => x.Id == dto.Id);
                dto.CreatedDate = DateTime.Now;
                //if (housing.RenewalDetailHome != null && housing.Inspections.Select(s => s.Id).Contains(dto.Id))
                //{
                //    response.Result = null;
                //    response.Success = false;
                //    response.Message = "This Contract Detail Already Exist.";
                //    return StatusCode(409, response);
                //}
                Attendee hl = _housingListRepository.AddAttendee(_mapper.Map<Attendee>(dto));
                response.Result = _mapper.Map<AttendeeDto>(hl);
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


        //Post Create a new Attendee
        [HttpPost("GetPaymentRepairResponsability", Name = "GetPaymentRepairResponsability")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetPaymentRepairResponsability([FromBody] int dto)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
                
                var hl = _housingListRepository.GetCatPaymentRepairResponsability();
               // response.Result = hl;
                return Ok(new { Success = true, hl });
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


        //Post Create a new Attendee
        [HttpPost("PostAttendeeInspecs", Name = "PostAttendeeInspecs")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AttendeeInspecDto>> PostAttendeeInspecs([FromBody] AttendeeInspecDto dto)
        {
            var response = new ApiResponse<AttendeeInspecDto>();
            try
            {
                
                dto.CreatedDate = DateTime.Now;
                AttendeeInspec hl = _housingListRepository.AddAttendeeInspec(_mapper.Map<AttendeeInspec>(dto));
                response.Result = _mapper.Map<AttendeeInspecDto>(hl);
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

        //Put Edit Attendee
        [HttpPut("PutAttendee", Name = "PutAttendee")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AttendeeDto>> PutAttendee([FromBody] AttendeeDto dto)
        {
            var response = new ApiResponse<AttendeeDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                Attendee hl = _housingListRepository.UpdateAttendee(_mapper.Map<Attendee>(dto));
                response.Result = _mapper.Map<AttendeeDto>(hl);
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

        //Put Edit Attendee
        [HttpPut("PutAttendeeInspec", Name = "PutAttendeeInspec")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AttendeeInspecDto>> PutAttendeeInspec([FromBody] AttendeeInspecDto dto)
        {
            var response = new ApiResponse<AttendeeInspecDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                AttendeeInspec hl = _housingListRepository.UpdateAttendeeInspec(_mapper.Map<AttendeeInspec>(dto));
                response.Result = _mapper.Map<AttendeeInspecDto>(hl);
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

        //Post Create a new KeyInventory
        [HttpPost("PostKeyInventory", Name = "PostKeyInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<KeyInventoryDto>> PostKeyInventory([FromBody] KeyInventoryDto dto)
        {
            var response = new ApiResponse<KeyInventoryDto>();
            try
            {
                //var housing = _housingListRepository.GetAllIncluding(x => x.Inspections).SingleOrDefault(x => x.Id == dto.Id);
                dto.CreatedDate = DateTime.Now;
                //if (housing.RenewalDetailHome != null && housing.Inspections.Select(s => s.Id).Contains(dto.Id))
                //{
                //    response.Result = null;
                //    response.Success = false;
                //    response.Message = "This Contract Detail Already Exist.";
                //    return StatusCode(409, response);
                //}
                KeyInventory hl = _housingListRepository.AddKeyInventory(_mapper.Map<KeyInventory>(dto));
                response.Result = _mapper.Map<KeyInventoryDto>(hl);
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

        //Put Edit KeyInventory
        [HttpPut("PutKeyInventory", Name = "PutKeyInventory")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<KeyInventoryDto>> PutKeyInventory([FromBody] KeyInventoryDto dto)
        {
            var response = new ApiResponse<KeyInventoryDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                KeyInventory hl = _housingListRepository.UpdateKeyInventory(_mapper.Map<KeyInventory>(dto));
                response.Result = _mapper.Map<KeyInventoryDto>(hl);
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

        //Post Create a new Repair
        [HttpPost("PostRepair", Name = "PostRepair")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<Repair>>> PostRepair([FromBody] RepairDto dto)
        {
            var response = new ApiResponse<List<Repair>>();
            try
            {
                //var housing = _housingListRepository.GetAllIncluding(x => x.Inspections).SingleOrDefault(x => x.Id == dto.Id);
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentRepairs)
                {
                    if (i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Housing/", i.FileExtension);
                    }
                }
                List<Repair> hl = _housingListRepository.AddRepair(_mapper.Map<Repair>(dto));
                response.Result = hl;
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

        //Put Edit Repair
        [HttpPut("PutRepair", Name = "PutRepair")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<Repair>>> PutRepair([FromBody] RepairDto dto)
        {
            var response = new ApiResponse<List<Repair>>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                foreach (var i in dto.DocumentRepairs)
                {
                    if (i.FileRequest.Length > 150)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/Housing/", i.FileExtension);
                    }
                }
                List<Repair> hl = _housingListRepository.UpdateRepair(_mapper.Map<Repair>(dto));
                response.Result = hl;
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

        //Put Edit Repair
        [HttpPut("DeleteRepair", Name = "DeleteRepair")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<Repair>>> DeleteRepair([FromBody] int id)
        {
            var response = new ApiResponse<List<Repair>>();
            try
            {
                
               // List<RepairDto> hl = _housingListRepository.UpdateRepair(_mapper.Map<Repair>(dto));
                List<Repair> hl = _housingListRepository.DeleteRepair(id);
                response.Result = hl;
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


        //Put Edit Repair
        [HttpPut("DeleteDocumentRepair", Name = "DeleteDocumentRepair")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<DocumentRepair>>> DeleteDocumentRepair([FromBody] int id)
        {
            var response = new ApiResponse<List<DocumentRepair>>();
            try
            {

                List<DocumentRepair> hl = _housingListRepository.DeleteDocumentRepair(id);
                response.Result = hl;
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

        //Post Create a new Repair
        [HttpPost("PostInspection", Name = "PostInspection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<Inspection>>> PostInspection([FromBody] InspectionDto dto)
        {
            var response = new ApiResponse<List<Inspection>>();
            try
            {

                foreach (var i in dto.PhotosInspecs)
                {
                    if (i.Id != 0)
                    {
                        if (i.Photo.Length > 150)
                        {
                            PhotosInspec document = _housingListRepository.getPhotoInspecById(i.Id);
                           // i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", "png");
                            _utiltyRepository.DeleteFile(document.Photo);
                        }
                    }
                    else
                    {
                        //i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", "png");
                    }
                }

                List<Inspection> hl = _housingListRepository.AddInspection(_mapper.Map<Inspection>(dto));
                response.Result = hl;
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

        //Put Edit Repair
        [HttpPut("PutInspection", Name = "PutInspection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<Inspection>>> PutInspection([FromBody] InspectionDto dto)
        {
            var response = new ApiResponse<List<Inspection>>();
            try
            {
                dto.UpdatedDate = DateTime.Now;

                foreach (var i in dto.PhotosInspecs)
                {
                    if (i.Id != 0)
                    {
                        if (i.Photo.Length > 150)
                        {
                            PhotosInspec document = _housingListRepository.getPhotoInspecById(i.Id);
                           // i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", "png");
                            _utiltyRepository.DeleteFile(document.Photo);
                        }
                    }
                    else
                    {
                        //i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
                        i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", "png");
                    }
                }


                List<Inspection> hl = _housingListRepository.UpdateInspection(_mapper.Map<Inspection>(dto));
                response.Result = hl;
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


        //Delete Inspection
        [HttpPut("DeleteInspection", Name = "DeleteInspection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<Inspection>>> PutInspection([FromBody] int id)
        {
            var response = new ApiResponse<List<Inspection>>();
            try
            {
                
              //  List<Inspection> hl = _housingListRepository.UpdateInspection(_mapper.Map<Inspection>(dto));
                List<Inspection> hl = _housingListRepository.DeleteInspection(id);

                response.Result = hl;
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

        //Delete Inspection
        [HttpPut("DeletePhotoInspection", Name = "DeletePhotoInspection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<PhotosInspec>>> DeletePhotoInspection([FromBody] int id)
        {
            var response = new ApiResponse<List<PhotosInspec>>();
            try
            {

                //  List<Inspection> hl = _housingListRepository.UpdateInspection(_mapper.Map<Inspection>(dto));
                List<PhotosInspec> hl = _housingListRepository.DeletePhotoInspection(id);

                response.Result = hl;
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

        //Post Create a new Departure
        [HttpPost("SendPropertys", Name = "SendPropertys")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult SendPropertys([FromBody] send_houses_obj list_obj)
        {
            var response = new ApiResponse<DepartureDetailsHomeDto>();
            try
            {

                var result = _housingListRepository.UpdateSendPropertys(list_obj.list);
                //List<string> _datos_asignado = new List<string>();
                //_datos_asignado.Add(list_obj.id_sr.ToString());
                
                var _datos_asignado = _assigneeNameRepository.Find(f => f.ServiceRecordId == list_obj.id_sr);

                //var listHousing = _housingListRepository.Find(c => c.Id)
                if(_datos_asignado != null)
                {
                   // StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Properties.html"));
                    StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/housing_list.html"));
                    var url_images = _utiltyRepository.Get_url_email_images("newmail2");
                    string body = string.Empty;
                    body = reader.ReadToEnd();
                    body = body.Replace("{user}", _datos_asignado.AssigneeName);
                    body = body.Replace("{noPropiedades}",  list_obj.list.Count.ToString());
                    //body = body.Replace("{Propiedades}", "$" + "<br>" + list_obj.list.Count);
                    body = body.Replace("{url_images}", url_images);
                    // _userRepository.SendMail("alfonso.franco@stpmex.com", body, "New Propertys To View");  list_obj.list[0]
                    _userRepository.SendMail(_datos_asignado.Email, body, "New Propertys To View");
                }

                var wo_id = _housingListRepository.Find(x => x.Id == list_obj.list[0]).WorkOrder;
                var coordinator = _notificationSystemRepository.GetCoordinator(wo_id.Value);

                if (coordinator.Item2 > 0)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Archive = false,
                        View = false,
                        ServiceRecord = coordinator.Item1,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = coordinator.Item2,
                        UserTo = coordinator.Item2,
                        NotificationType = 33,
                        Description = _notificationSystemTypeRepository.Find(x => x.Id == 33).Type + " | SR-" + coordinator.Item1.ToString(),
                        Color = "67757c",
                        CreatedDate = DateTime.Now
                    });


                    _notificationSystemRepository.SendNotificationAsync(
                        coordinator.Item2,
                        0,
                        0,
                        _notificationSystemTypeRepository.Find(x => x.Id == 33).Type ,
                        _notificationSystemTypeRepository.Find(x => x.Id == 33).Type + " | SR-" + coordinator.Item1.ToString(),
                        2
                    );
                }

                return Ok(new { Success = true, result });
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


        public class send_houses_obj
        {
            public send_houses_obj() { }

            public List<int> list { get; set; }
            public int id_sr  { get; set;}
        }


        ///////////////////////////////////// PERFORMANCE  HOUSING LIST /////////////////////////////////////////

        [HttpGet("GetOnlyPropertyDetails", Name = "GetOnlyPropertyDetails")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOnlyPropertyDetails([FromQuery] int key, int servide_detail_id, int type)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                
                var result = _housingListRepository.GetOnlyPropertyDetails(key, servide_detail_id, type);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        [HttpGet("GetOnlyPropertyLSF", Name = "GetOnlyPropertyLSF")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOnlyPropertyLSF([FromQuery] int key, int servide_detail_id, int type)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                var result = _housingListRepository.GetOnlyPropertyLSF(key, servide_detail_id, type);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpGet("GetOnlyPropertyInspRep", Name = "GetOnlyPropertyInspRep")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOnlyPropertyInspRep([FromQuery] int key, int servide_detail_id, int type)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                var result = _housingListRepository.GetOnlyPropertyInspRep(key, servide_detail_id, type);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() + " -------------------- "  + ex.StackTrace.ToString() + " -------------------- " + ex.InnerException.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpGet("GetLSFPropertyPrint", Name = "GetLSFPropertyPrint")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetLSFPropertyPrint([FromQuery] int key, int servide_detail_id, int type)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                var result = _housingListRepository.GetLSFPropertyPrint(key, servide_detail_id, type);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        [HttpGet("GetIRPropertyPrint", Name = "GetIRPropertyPrint")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetIRPropertyPrint([FromQuery] int key, int servide_detail_id, int type)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                //var result = _housingListRepository.GetLSFPropertyPrint(key, servide_detail_id, type);
                var result = _housingListRepository.GetIRPropertyPrint(key, servide_detail_id, type);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }

        [HttpGet("GetHomestovist", Name = "GetHomestovist")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetHomestovist([FromQuery] int wosid, DateTime? dateViste)
        {
            try
            {

                var custom = _housingListRepository.GetHomestovist(wosid , dateViste);

                return Ok(new { Success = true, custom });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }


        ////////////////////////// SERVICIOS PARA LOS CAMBIOS EN LA APP 

        // Section  (1: Contract, 2: GroupCostSavings, 3: GroupPaymnetsHousings, 4: DepartureDetailsHomes, 5: RenewalDetailHomes , 6: LandLord)

        [HttpGet("GetLSFBySection", Name = "GetLSFBySection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetLSFBySection([FromQuery] int key, int servide_detail_id, int section)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
              //  var result = _housingListRepository.GetOnlyPropertyLSF(key, servide_detail_id, type);
                var result = _housingListRepository.GetLSFBySection(key, servide_detail_id, section);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        //section ( 1: Move In , 2: ove out , 3: Inspections & Repairs )
        [HttpGet("GetInspRepBySection", Name = "GetInspRepBySection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetInspRepBySection([FromQuery] int key, int servide_detail_id, int section)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                var result = _housingListRepository.GetInspRepBySection(key, servide_detail_id, section);
               // var result = _housingListRepository.GetOnlyPropertyInspRep(key, servide_detail_id, type);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() + " -------------------- " + ex.StackTrace.ToString() + " -------------------- " + ex.InnerException.ToString() }");
                return StatusCode(500, response);
            }

        }


        //section ( 1: Move In , 2: ove out , 3: Inspections & Repairs )
        [HttpGet("GetInspRepBySectionPrint", Name = "GetInspRepBySectionPrint")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<string>> GetInspRepBySectionPrint([FromQuery] int key, int servide_detail_id, int section)
        {
            //var response = new ApiResponse<HousingListSelectLSFDto>();
            var response = new ApiResponse<string>();
            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "1_archivo.pdf";
                string xFull = Path.Combine(xPath, xNombre);

                if (System.IO.File.Exists(xFull))
                    System.IO.File.Delete(xFull);

                string result = _housingListRepository.GetInspRepBySectionPrint(key, servide_detail_id, section, xFull);
                response.Result = "ok";
                response.Success = true;
                response.Message = result;

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() + " -------------------- " + ex.StackTrace.ToString() + " -------------------- " + ex.InnerException.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);

        }


        [HttpGet("GetLSFPrint", Name = "GetLSFPrint")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<string>> GetLSFPrint([FromQuery] int key, int servide_detail_id, int type) // type hay que mandar 26, en los otros dos mandar 
        {
            //var response = new ApiResponse<HousingListSelectLSFDto>();
            var response = new ApiResponse<string>();
            try
            {
                string xPath = Path.GetTempPath();
                string xNombre = "1_archivo.pdf";
                string xFull = Path.Combine(xPath, xNombre);

                if (System.IO.File.Exists(xFull))
                    System.IO.File.Delete(xFull);

                string result = _housingListRepository.GetLSFPrint(key, servide_detail_id, type, xFull);
                response.Result = "ok";
                response.Success = true;
                response.Message = result;

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() + " -------------------- " + ex.StackTrace.ToString() + " -------------------- " + ex.InnerException.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);

        }


        //delete expense 
        [HttpPut("delete_photo_section", Name = "delete_photo_section")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult delete_photo_section([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {
                var result = _housingListRepository.delete_photo_section(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        //delete  
        [HttpPut("delete_photo_section_inventory", Name = "delete_pdelete_photo_section_inventoryhoto_section")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult delete_photo_section_inventory([FromBody] int id)
        {
            var response = new ApiResponse<LandlordDetailsHomeDto>();
            try
            {
                var result = _housingListRepository.delete_photo_section_inventory(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        [HttpGet("GetStatusIR", Name = "GetStatusIR")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetStatusIR([FromQuery]  int servide_detail_id , int key)
        {
            var response = new ApiResponse<HousingListSelectLSFDto>();
            try
            {
                var result = _housingListRepository.GetStatusIR( servide_detail_id, key);
                return Ok(new { Success = true, result });

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() + " -------------------- " + ex.StackTrace.ToString() + " -------------------- " + ex.InnerException.ToString() }");
                return StatusCode(500, response);
            }

        }

        //delete expense 
        [HttpPut("delete_photo_inspection", Name = "delete_photo_inspection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult delete_photo_inspection([FromBody] int id)
        {
            var response = new ApiResponse<PhotosInspec>();
            try
            {
               // var result = _housingListRepository.delete_photo_section_inventory(id);
                var result = _housingListRepository.delete_photo_inspection(id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


        public class req_accept
        {
            public int id_property { get; set; }

            public int status_id { get; set; }
        }

        //change status property  
        [HttpPut("accept_reject_property", Name = "accept_reject_property")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult accept_reject_property([FromBody] req_accept req)
        {
            var response = new ApiResponse<PhotosInspec>();
            try
            {
                // var result = _housingListRepository.delete_photo_section_inventory(id);
                var result = _housingListRepository.accept_reject_property(req.id_property, req.status_id);

                return Ok(new { Success = true, result });
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }

        }


    }
}
