using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.NotificationSystem;
using api.premier.Models.WorkOrder;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestInvoiceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IWorkOrderRepository _serviceOrderRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUserRepository _userRepository;
        public RequestInvoiceController(
            IMapper mapper,
            ILoggerManager logger,
            IWorkOrderRepository serviceOrderRepository,
            IServiceRecordRepository serviceRecordRepository,
            IUtiltyRepository utiltyRepository,
            INotificationSystemRepository notificationSystemRepository,
            IProfileUserRepository profileUserRepository,
            IUserRepository userRepository
            )
        {
            _mapper = mapper;
            _logger = logger;
            _serviceOrderRepository = serviceOrderRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _utiltyRepository = utiltyRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _profileUserRepository = profileUserRepository;
            _userRepository = userRepository;
        }

        [HttpGet("GetOrdersInvoice", Name = "GetOrdersInvoice")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetOrdersInvoice([FromQuery] int sr, [FromQuery] int? supplierPartner)
        {
            try
            {
                var service = _serviceRecordRepository.Find(c => c.Id == sr);
                if (service != null)
                {
                    var result = _serviceOrderRepository.GetOrders(sr, supplierPartner);
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

        [HttpPost("AddOrdersInvoice", Name = "AddOrdersInvoice")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<InvoiceDto>>> AddOrdersInvoice([FromBody] List<InvoiceDto> Dto)
        {
            var response = new ApiResponse<List<InvoiceDto>>();
            try
            {
                List<InvoiceDto> InvoiceDto = new List<InvoiceDto>();
                foreach(var i in Dto)
                {
                    var user = i.Consultant.HasValue ? _profileUserRepository.GetConsultant(i.Consultant.Value).Id : i.CreatedBy.Value;
                    var usersFinance = _userRepository.GetAllIncluding(i => i.ProfileUsers).Where(x => x.ProfileUsers.Select(s => s.Title).Contains(5));
                    foreach (var a in usersFinance)
                    {
                        NotificationSystemDto notification = new NotificationSystemDto();
                        notification.Archive = false;
                        notification.View = false;
                        notification.ServiceRecord = i.ServiceRecord;
                        notification.Time = DateTime.Now.TimeOfDay;
                        notification.UserFrom = user;
                        notification.UserTo = a.Id;
                        notification.NotificationType = 10;
                        notification.Description = "";
                        notification.Color = "#ff8100";
                        notification.CreatedDate = DateTime.Now;
                        _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                    }
                    foreach (var d in i.DocumentInvoices)
                    {
                        d.FilePath = _utiltyRepository.UploadImageBase64(d.FilePath, "Files/Invoice/", d.FileExtension);
                    }
                    InvoiceDto.Add(_mapper.Map<InvoiceDto>(_serviceOrderRepository.AddInvoice(_mapper.Map<Invoice>(i))));
                }

                response.Success = true;
                response.Result = InvoiceDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }

            return StatusCode(202, response);
        }

        [HttpPut("UpdateOrdersInvoice", Name = "UpdateOrdersInvoice")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<InvoiceDto>>> UpdateOrdersInvoice([FromBody] List<InvoiceDto> Dto)
        {
            var response = new ApiResponse<List<InvoiceDto>>();
            try
            {
                List<InvoiceDto> InvoiceDto = new List<InvoiceDto>();
                foreach (var i in Dto)
                {
                    if(i.Id == 0)
                    {
                        var user = _profileUserRepository.GetConsultant(i.Consultant.Value);
                        var usersFinance = _userRepository.GetAllIncluding(i => i.ProfileUsers).Where(x => x.ProfileUsers.Select(s => s.Title).Contains(5));
                        foreach(var a in usersFinance)
                        {
                            NotificationSystemDto notification = new NotificationSystemDto();
                            notification.Archive = false;
                            notification.View = false;
                            notification.ServiceRecord = i.ServiceRecord;
                            notification.Time = DateTime.Now.TimeOfDay;
                            notification.UserFrom = a.Id;
                            notification.UserTo = user.UserId;
                            notification.NotificationType = 10;
                            notification.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. Integer aliquam iaculis nisl et imperdiet.";
                            notification.Color = "#ff8100";
                            notification.CreatedDate = DateTime.Now;
                            _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));                   }
                    }
                    if (i.Id != 0)
                    {
                        var user = _profileUserRepository.GetConsultant(i.Consultant.Value);
                        var usersFinance = _userRepository.GetAllIncluding(i => i.ProfileUsers).Where(x => x.ProfileUsers.Select(s => s.Title).Contains(5));
                        foreach (var a in usersFinance)
                        {
                            NotificationSystemDto notification = new NotificationSystemDto();
                            notification.Archive = false;
                            notification.View = false;
                            notification.ServiceRecord = i.ServiceRecord;
                            notification.Time = DateTime.Now.TimeOfDay;
                            notification.UserFrom = a.Id;
                            notification.UserTo = user.UserId;
                            notification.NotificationType = 11;
                            notification.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. Integer aliquam iaculis nisl et imperdiet.";
                            notification.Color = "#ff8100";
                            notification.CreatedDate = DateTime.Now;
                            _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                        }
                    }
                    foreach (var d in i.DocumentInvoices)
                    {
                        if (d.FilePath.Length > 150)
                            d.FilePath = _utiltyRepository.UploadImageBase64(d.FilePath, "Files/Invoice/", d.FileExtension);
                    }
                    InvoiceDto.Add(_mapper.Map<InvoiceDto>(_serviceOrderRepository.UpdateInvoice(_mapper.Map<Invoice>(i))));
                }

                response.Success = true;
                response.Result = InvoiceDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }

            return StatusCode(202, response);
        }

        [HttpGet("GetInvoice", Name = "GetInvoice")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetInvoice([FromQuery] int[] so, [FromQuery] int? invoice, [FromQuery] int? supplierPartner)
        {
            try
            {
                var result = _serviceOrderRepository.GetrequestInvoice(so, invoice, supplierPartner);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetInvoiceList/{user}/", Name = "GetInvoiceList")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetInvoiceList(int user, DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            try
            {
                var result = _serviceOrderRepository.GetInvoiceList(user, renge1, range2, status, serviceLine, invoiceType, coordinator, partner, client, country);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetRequestCenter", Name = "GetRequestCenter")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetRequestCenter(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            try
            {
                var result = _serviceOrderRepository.GetRequestCenter(renge1, range2, status, serviceLine, invoiceType, coordinator, partner, client, country);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
  

        [HttpGet("GetSupplierPartnerInvoices/{sr}/", Name = "GetSupplierPartnerInvoices")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartnerInvoices(int sr)
        {
            try
            {
                var result = _serviceOrderRepository.GetSupplierPartnerInvoices(sr);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
    }
}
