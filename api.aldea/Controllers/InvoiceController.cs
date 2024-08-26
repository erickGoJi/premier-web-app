using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.NotificationSystem;
using api.premier.Models.WorkOrder;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.Ínvoice;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        public InvoiceController(IInvoiceRepository invoiceRepository, IMapper mapper, ILoggerManager logger, IProfileUserRepository profileUserRepository,
            IUserRepository userRepository, INotificationSystemRepository notificationSystemRepository, IUtiltyRepository utiltyRepository)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _logger = logger;
            _profileUserRepository = profileUserRepository;
            _userRepository = userRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _utiltyRepository = utiltyRepository;
        }
        [HttpGet("GetSupplierInvoices", Name = "GetSupplierInvoices")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierPartnerInvoices(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            try
            {
                var result = _invoiceRepository.GetSupplierInvoices(renge1, range2, status, serviceLine, invoiceType, coordinator, partner, client, country);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("GetInvoiceById", Name = "GetInvoiceById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetInvoiceById(int key)
        {
            try
            {
                var result = _invoiceRepository.GetRequestInvoice(key);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Invoice Not Found", Message = ex.ToString() });
            }
        }

        [HttpPut("", Name = "")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<InvoiceDto>> UpdateOrdersInvoice([FromBody] InvoiceDto invoice)
        {
            var response = new ApiResponse<InvoiceDto>();
            try
            {
                    if (invoice.Id != 0)
                    {
                        var user = _profileUserRepository.GetConsultant(invoice.Consultant.Value);
                        var usersFinance = _userRepository.GetAllIncluding(i => i.ProfileUsers).Where(x => x.ProfileUsers.Select(s => s.Title).Contains(5));
                        foreach (var a in usersFinance)
                        {
                            NotificationSystemDto notification = new NotificationSystemDto();
                            notification.Archive = false;
                            notification.View = false;
                            notification.ServiceRecord = invoice.ServiceRecord;
                            notification.Time = DateTime.Now.TimeOfDay;
                            notification.UserFrom = a.Id;
                            notification.UserTo = user.UserId;
                            notification.NotificationType = 12;
                            notification.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. Integer aliquam iaculis nisl et imperdiet.";
                            notification.Color = "#0277bd";
                            notification.CreatedDate = DateTime.Now;
                            _notificationSystemRepository.Add(_mapper.Map<NotificationSystem>(notification));
                        }
                    }
                    foreach (var d in invoice.DocumentInvoices)
                    {
                        if (d.FilePath.Length > 150)
                            d.FilePath = _utiltyRepository.UploadImageBase64(d.FilePath, "Files/Invoice/", d.FileExtension);
                    }
                    InvoiceDto result = _mapper.Map<InvoiceDto>(_invoiceRepository.UpdateCustom(_mapper.Map<Invoice>(invoice)));

                response.Success = true;
                response.Result = result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }

            return StatusCode(202, response);
        }
        
        [HttpGet("Finance/ThirdPartyInvoice", Name = "Finance/ThirdPartyInvoice")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult ThirdPartyInvoice(int sr)
        {
            try
            {
                var result = _invoiceRepository.GetThirdPartyInvoice(sr);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Invoice Not Found", Message = ex.ToString() });
            }
        }
        
        [HttpGet("Finance/ServiceInvoices/{sr}", Name = "Finance/ServiceInvoices/{sr}")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult ServiceInvoices(int sr, int? serviceLine, int? status)
        {
            try
            {
                var result = _invoiceRepository.ServiceInvoices(sr, serviceLine, status);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Invoice Not Found", Message = ex.ToString() });
            }
        }

    }
}
