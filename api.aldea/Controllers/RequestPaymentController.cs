using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.RequestPayment;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.RequestPayment;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestPaymentController : ControllerBase
    {
        private readonly IRequestPaymentRepository _requestPaymentRepository; 
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IUserRepository _userRepository;
        public RequestPaymentController(IRequestPaymentRepository requestPaymentRepository, IMapper mapper, ILoggerManager loggerManager, IUtiltyRepository utiltyRepository,
            IUserRepository userRepository)
        {
            _requestPaymentRepository = requestPaymentRepository;
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _userRepository = userRepository;
        }

        //Post Create a new PreDicision Orientation
        [HttpPost("PostRequestPayment", Name = "PostRequestPayment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RequestPaymentDto>> PostRequestPayment([FromBody] RequestPaymentDto dto)
        {
            var response = new ApiResponse<RequestPaymentDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var i in dto.DocumentRequestPayments)
                {
                    i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/RequestPayment/", i.FileExtension);
                }
                foreach (var i in dto.Payments)
                {
                    foreach (var _i in i.DocumentPaymentConcepts)
                    {
                        if (_i.FileRequest.Length > 150)
                        {
                            _i.FileRequest = _utiltyRepository.UploadImageBase64(_i.FileRequest, "Files/PaymentConcept/", _i.FileExtension);
                        }
                    }
                }
                List<PaymentDto> payments = new List<PaymentDto>();
                foreach(var o in dto.Payments)
                {
                    PaymentDto paymentDto = new PaymentDto();
                    paymentDto = o;
                    if (o.Recurrence.Value)
                    {
                        DateTime ifNoDate = o.RecurrencePaymentConcepts.FirstOrDefault().StartDate.Value.AddMonths(24);
                        int period = 0;
                        switch (o.RecurrencePaymentConcepts.FirstOrDefault().Period.Value)
                        {
                            case (4):
                                int weeks = GetWeekDifference
                                    (o.RecurrencePaymentConcepts.FirstOrDefault().StartDate.Value,
                                    (o.RecurrencePaymentConcepts.FirstOrDefault().Date.Value
                                        ? o.RecurrencePaymentConcepts.FirstOrDefault().EndDate.Value
                                        : ifNoDate));
                                period = (weeks / o.RecurrencePaymentConcepts.FirstOrDefault().RepeatEvery.Value);
                                for (int i = 1; i < period; i++)
                                {
                                    int addWeeks = (i * (weeks)) / period;
                                    paymentDto.DueDate.Value.AddDays((addWeeks * 7));
                                    dto.Payments.Add(paymentDto);
                                }
                                break;
                            case (5):
                                int months = GetMonthDifference
                                    (o.RecurrencePaymentConcepts.FirstOrDefault().StartDate.Value, 
                                    (o.RecurrencePaymentConcepts.FirstOrDefault().Date.Value 
                                        ? o.RecurrencePaymentConcepts.FirstOrDefault().EndDate.Value
                                        : ifNoDate));
                                period = (months / o.RecurrencePaymentConcepts.FirstOrDefault().RepeatEvery.Value);
                                for (int i = 1; i < period; i++)
                                {
                                    int addMonths = (i * (months))/ period;
                                    DateTime newDue = o.DueDate.Value.AddMonths(addMonths);
                                    payments.Add(new PaymentDto
                                    {
                                        Id = 0,
                                        AdvenceFee = o.AdvenceFee,
                                        CommentPaymentConcepts = o.CommentPaymentConcepts,
                                        Desciption = o.Desciption,
                                        CreatedBy = o.CreatedBy,
                                        CreatedDate = o.CreatedDate,
                                        CurrencyAdvanceFee = o.CurrencyAdvanceFee,
                                        CurrencyManagementFee = o.CurrencyManagementFee,
                                        CurrencyPaymentAmount = o.CurrencyPaymentAmount,
                                        CurrencyWireFee = o.CurrencyWireFee,
                                        DocumentPaymentConcepts = o.DocumentPaymentConcepts,
                                        DueDate = newDue,
                                        FiscalInvoice = o.FiscalInvoice,
                                        IfSupplierPartner = o.IfSupplierPartner,
                                        InvoiceDate = o.InvoiceDate,
                                        InvoiceNo = o.InvoiceNo,
                                        ManagementFee = o.ManagementFee,
                                        PaymentAmount = o.PaymentAmount,
                                        PaymentDate = o.PaymentDate,
                                        PaymentMethod = o.PaymentMethod,
                                        Recurrence = o.Recurrence,
                                        RecurrencePaymentConcepts = o.RecurrencePaymentConcepts,
                                        RequestPayment = o.RequestPayment,
                                        SameWithoutFee = o.SameWithoutFee,
                                        Service = o.Service,
                                        ServiceRecord = o.ServiceRecord,
                                        Supplier = o.Supplier,
                                        SupplierInvoiceDate = o.SupplierInvoiceDate,
                                        SupplierInvoiceNo = o.SupplierInvoiceNo,
                                        SupplierName = o.SupplierName,
                                        SupplierPartner = o.SupplierPartner,
                                        UpdatedBy= o.UpdatedBy,
                                        Urgent = o.Urgent,
                                        UpdatedDate= o.UpdatedDate,
                                        WireFee = o.WireFee,
                                        WireTransferPaymentConcepts = o.WireTransferPaymentConcepts,
                                        WireTransferServicePaymentConcepts = o.WireTransferServicePaymentConcepts,
                                        WorkOrder = o.WorkOrder,
                                        WorkOrderServices = o.WorkOrderServices
                                    });
                                }
                                break;
                        }
                    }
                }

                foreach(var q in payments)
                {
                    dto.Payments.Add(q);
                }

                RequestPayment rp = _requestPaymentRepository.Add(_mapper.Map<RequestPayment>(dto));
                response.Result = _mapper.Map<RequestPaymentDto>(rp);
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

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
        public static int GetWeekDifference(DateTime startDate, DateTime endDate)
        {
            int weeks = (int)((startDate - endDate).TotalDays / 7);
            return Math.Abs(weeks);
        }
        public class Months
        {
            public int Position { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        public static List<Months> GetMonthDifference(DateTime startDate, DateTime endDate, int recurrency)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            monthsApart = Math.Abs(monthsApart);
            List<Months> m = new List<Months>();
            int period = monthsApart / recurrency;
            for(int o = 0; o < period; o++)
            {
                Months months = new Months();
                months.Position = o;
                months.StartDate = o != 0 ? startDate.AddMonths(((o) * (monthsApart)) / period) : startDate;
                months.EndDate = o == 0 
                    ? startDate.AddMonths(((o + 1) * (monthsApart)) / period) 
                    : months.StartDate.AddMonths(((1) * (monthsApart)) / period);
                m.Add(months);
            }
            return m;
        }

        //Put Update a PreDicision Orientation
        [HttpPut("PutRequestPayment", Name = "PutRequestPayment")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<RequestPaymentDto>> PutRequestPayment([FromBody] RequestPaymentDto dto)
        {
            var response = new ApiResponse<RequestPaymentDto>();
            try
            {
                RequestPaymentDto requestPayment = new RequestPaymentDto();
                requestPayment = dto;
                var preview = _requestPaymentRepository.Find(x => x.Id == dto.Id);
                if (dto.Status == 2 && preview.Status == 1 && dto.Recurrence.Value)
                {
                    DateTime ifNoDate = requestPayment.RecurrenceRequestPayments.FirstOrDefault().StartDate.Value.AddMonths(24);
                    switch (requestPayment.RecurrenceRequestPayments.FirstOrDefault().Period.Value)
                    {
                        //case (4):
                        //    int weeks = GetWeekDifference
                        //        (requestPayment.RecurrenceRequestPayments.FirstOrDefault().StartDate.Value,
                        //        (requestPayment.RecurrenceRequestPayments.FirstOrDefault().Date.Value
                        //            ? requestPayment.RecurrenceRequestPayments.FirstOrDefault().EndDate.Value
                        //            : ifNoDate));
                        //    period = (weeks / requestPayment.RecurrenceRequestPayments.FirstOrDefault().RepeatEvery.Value);
                        //    for (int i = 0; i < period; i++)
                        //    {
                        //        int addWeeks = (i * (weeks)) / period;
                        //        paymentDto.DueDate.Value.AddDays((addWeeks * 7));
                        //        dto.Payments.Add(paymentDto);
                        //    }
                        //    break;
                        case (5):
                            var months = GetMonthDifference(requestPayment.RecurrenceRequestPayments.FirstOrDefault().StartDate.Value,
                                (requestPayment.RecurrenceRequestPayments.FirstOrDefault().Date.Value
                                    ? requestPayment.RecurrenceRequestPayments.FirstOrDefault().EndDate.Value
                                    : ifNoDate)
                                , requestPayment.RecurrenceRequestPayments.FirstOrDefault().RepeatEvery.Value);
                            int position = 0;
                            int requestId = dto.Id;
                            months.Reverse();
                            foreach(var n in months)
                            {
                                RequestPaymentDto request = new RequestPaymentDto();
                                request = dto;
                                request.Id = position == 0 ? requestId : 0;
                                //foreach(var k in dto.Payments)
                                //{
                                var _request = _requestPaymentRepository
                                    .GetAllIncluding(i => i.Payments).Where(x => x.Id == requestId).FirstOrDefault();
                                var _payments = _request.Payments.Where(x => x.DueDate >= n.StartDate && x.DueDate < n.EndDate).ToList();
                                request.Payments = new List<PaymentDto>();
                                foreach(var r in _payments)
                                {
                                    request.Payments.Add(_mapper.Map<PaymentDto>(r));
                                }
                                //}
                                //request.Payments = dto.Payments.Where(x => x.DueDate >= n.StartDate && x.DueDate < n.EndDate).ToList();
                                foreach(var o in request.Payments)
                                {
                                    o.Id = 0;
                                    o.RequestPayment = 0;
                                    foreach (var i in o.CommentPaymentConcepts)
                                    {
                                        i.Id = 0;
                                        i.PaymentConcept = 0;
                                    }
                                    foreach (var i in o.DocumentPaymentConcepts)
                                    {
                                        i.Id = 0;
                                        i.PaymentConcept = 0;
                                    }
                                    foreach (var i in o.RecurrencePaymentConcepts)
                                    {
                                        i.Id = 0;
                                        i.PaymentConcept = 0;
                                    }
                                }
                                //if(position == 0)
                                //{
                                //    _requestPaymentRepository.UpdateCustom(_mapper.Map<RequestPayment>(request), requestId);
                                //}
                                //else
                                //{
                                if (request.CommentRequestPayments.Any())
                                {
                                    foreach(var i in request.CommentRequestPayments)
                                    {
                                        i.Id = 0;
                                        i.RequestPaymentId = 0;
                                    }
                                    foreach (var i in request.DocumentRequestPayments)
                                    {
                                        i.Id = 0;
                                        i.RequestPaymentId = 0;
                                    }
                                    foreach (var i in request.RecurrenceRequestPayments)
                                    {
                                        i.Id = 0;
                                        i.RequestPayment = 0;
                                    }
                                }
                                request.Id = 0;
                                _requestPaymentRepository.Add(_mapper.Map<RequestPayment>(request));
                                //}
                                position++;
                            }
                            var _requestToDelete = _requestPaymentRepository.Find(x => x.Id == requestId);
                            _requestPaymentRepository.Delete(_requestToDelete);
                            break;
                    }

                }
                else
                {
                    dto.UpdatedDate = DateTime.Now;
                    foreach (var i in dto.DocumentRequestPayments)
                    {
                        if (i.Id != 0)
                        {
                            if (i.FileRequest.Length > 150)
                            {
                                List<DocumentRequestPaymentDto> document = _mapper.Map<List<DocumentRequestPaymentDto>>(_requestPaymentRepository.GetAllIncluding(x => x.Id == i.RequestPaymentId)
                                    .Select(s => s.DocumentRequestPayments).ToList());
                                DocumentRequestPaymentDto documentId = document.Where(x => x.Id == i.Id).FirstOrDefault();
                                i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/RequestPayment/", i.FileExtension);
                                _utiltyRepository.DeleteFile(documentId.FileRequest);
                            }
                        }
                        else {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/RequestPayment/", i.FileExtension);
                        }
                    }
                    foreach (var i in dto.Payments)
                    {
                        foreach(var _i in i.DocumentPaymentConcepts)
                        {
                            if (_i.FileRequest.Length > 150)
                            {
                                _i.FileRequest = _utiltyRepository.UploadImageBase64(_i.FileRequest, "Files/PaymentConcept/", _i.FileExtension);
                            }
                        }
                    }
                    RequestPayment rp = _requestPaymentRepository.UpdateCustom(_mapper.Map<RequestPayment>(dto), dto.Id);
                    response.Result = _mapper.Map<RequestPaymentDto>(rp);
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
            return StatusCode(201, response);
        }

        [HttpGet("GetRequestPaymentById", Name = "GetRequestPaymentById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetRequestPaymentById([FromQuery] int requestPaymentId)
        {
            try
            {
                return StatusCode(200, new { Success = true, Message = "", Result = _requestPaymentRepository.GetRequestPaymentById(requestPaymentId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = true, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetRequestPayments", Name = "GetRequestPayments")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<RequestPaymentDto>>> GetRequestPayments([FromQuery] int WorkOrderServicesId)
        {
            var response = new ApiResponse<List<RequestPaymentDto>>();
            try
            {
                var Result = _requestPaymentRepository.GetAllIncluding(x => x.CommentRequestPayments, u => u.DocumentRequestPayments);
                response.Result = _mapper.Map<List<RequestPaymentDto>>(Result.Where(x => x.WorkOrderServicesId == WorkOrderServicesId));
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(200, response);
        }

        [HttpPost("AddPaymentConceptFromService", Name = "AddPaymentConceptFromService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<List<PaymentDto>>> AddPaymentConceptFromService([FromBody] List<PaymentDto> paymentConceptDto)
        {
            var response = new ApiResponse<List<PaymentDto>>();
            try
            {
                foreach(var concept in paymentConceptDto)
                {
                    foreach (var i in concept.DocumentPaymentConcepts)
                    {
                        if (i.FileRequest.Length > 150)
                        {
                            i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/PaymentConcept/", i.FileExtension);
                        }
                    }
                }
                var result = _requestPaymentRepository.AddPaymentConcept(_mapper.Map<List<Payment>>(paymentConceptDto));
                response.Result = _mapper.Map<List<PaymentDto>>(result);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(200, response);
        }

        [HttpDelete("DeletePaymentConcept/{paymentConceptId}/{allPaymentConcept}/", Name = "DeletePaymentConcept/{paymentConceptId}/{allPaymentConcept}/")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeletePaymentConcept( int paymentConceptId, bool allPaymentConcept)
        {
            try
            {
                var res = _requestPaymentRepository.DeletePaymentConcept(paymentConceptId, allPaymentConcept);
                if (res)
                {
                    return StatusCode(200, new { Success = true, Message = "Delete was success.", Result = 0 });
                }
                else
                {
                    return StatusCode(404, new { Success = false, Message = "Payment Concept Not Found.", Result = 0 });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = true, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpPut("UpdatePaymentConcept", Name = "UpdatePaymentConcept")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentDto>> UpdatePaymentConcept([FromBody] PaymentDto paymentConceptDto)
        {
            var response = new ApiResponse<PaymentDto>();
            try
            {
                foreach (var i in paymentConceptDto.DocumentPaymentConcepts)
                {
                    if (i.FileRequest.Length > 150 && i.Id == 0)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/PaymentConcept/", i.FileExtension);
                    }
                }
                var result = _requestPaymentRepository.UpdatePaymentConceptCustom(_mapper.Map<Payment>(paymentConceptDto));
                response.Result = _mapper.Map<PaymentDto>(result);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(200, response);
        }

        [HttpGet("GetRequestedPayments", Name = "GetRequestedPayments")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetRequestedPayments([FromQuery] int WorkOrderServicesId)
        {
            try
            {
                var Result = _requestPaymentRepository.GetAllIncluding(x => x.CommentRequestPayments, u => u.DocumentRequestPayments);
                return StatusCode(200, new { Success = true, Message = "", Result = _requestPaymentRepository.GetRequestedPayment(WorkOrderServicesId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = true, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetPaymentConceptById", Name = "GetPaymentConceptById")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentSelectDto>> GetPaymentConceptById([FromQuery] int paymentConcept)
        {
            var response = new ApiResponse<PaymentSelectDto>();
            try
            {
                var result = _requestPaymentRepository.GetPaymentConceptById(paymentConcept);
                response.Result = _mapper.Map<PaymentSelectDto>(result);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(200, response);
        }

        [HttpGet("GetPaymentsRelated", Name = "GetPaymentsRelated")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetPaymentsRelated([FromQuery] int requestPaymentId)
        {
            try
            {
                return StatusCode(200, new { Success = true, Message = "", Result = _requestPaymentRepository.GetPaymentsRelated(requestPaymentId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = true, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetFees", Name ="GetFees")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetFees([FromQuery] int requestPaymentId)
        {
            try
            {
                return StatusCode(200, new { Success = true, Message = "", Result = _requestPaymentRepository.GetFee(requestPaymentId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = true, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }

        [HttpGet("GetFinance", Name = "GetFinance")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetFinance([FromQuery] int sr)
        {
            try
            {
                return StatusCode(200, new { Success = true, Message = "", Result = _requestPaymentRepository.GetFinance(sr) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = true, Message = $"Something went wrong: { ex.Message.ToString() }", Result = 0 });
            }
        }
        [HttpGet("Finance/GetSupplierInvoices", Name = "Finance/GetSupplierInvoices")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetSupplierInvoices(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            try
            {
                var result = _requestPaymentRepository.GetSupplierInvoices(renge1, range2, status, serviceLine, invoiceType, coordinator, partner, client, country);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }

        [HttpGet("Finance/GetThirdPartyExpenses", Name = "Finance/GetThirdPartyExpenses")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetThirdPartyExpenses(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            try
            {
                var result = _requestPaymentRepository.GetThirdPartyExpenses(renge1, range2, status, serviceLine, invoiceType, coordinator, partner, client, country);
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Result = "Service Order Not Found", Message = ex.ToString() });
            }
        }
        
        [HttpGet("Finance/GetInvoicesService", Name = "Finance/GetInvoicesService")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetInvoicesService(DateTime? renge1, DateTime? range2, int? status, int? serviceLine, int? invoiceType, int? coordinator, int? partner, int? client, int? country)
        {
            try
            {
                var result = _requestPaymentRepository.GetInvoicesService(renge1, range2, status, serviceLine, invoiceType, coordinator, partner, client, country);
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
