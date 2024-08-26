using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.SchoolingSearch;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.SchoolingSearch;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentSchoolingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IPaymentSchoolingRepository _paymentSchoolingRepository;

        public PaymentSchoolingController(IPaymentSchoolingRepository paymentSchoolingRepository, 
            IMapper mapper, 
            ILoggerManager logger)
        {
            _mapper = mapper;
            _logger = logger;
            _paymentSchoolingRepository = paymentSchoolingRepository;
        }


        //Post Create a new Housing
        [HttpPost("PostPaymentSchool", Name = "PostPaymentSchool")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PaymentSchoolingInformationDto>> PostPaymentSchools([FromBody] PaymentSchoolingInformationDto dto)
        {
            var response = new ApiResponse<PaymentSchoolingInformationDto>();
            try
            {
                PaymentSchoolingInformation hl = _paymentSchoolingRepository.Add(_mapper.Map<PaymentSchoolingInformation>(dto));
                response.Result = _mapper.Map<PaymentSchoolingInformationDto>(hl);
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

        //Put Create a new Housing
        [HttpPut("PutPaymentSchools", Name = "PutPaymentSchools")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<Models.SchoolingSearch.PaymentSchoolingInformationDto>> PutPaymentSchools([FromBody] PaymentSchoolingInformationDto dto)
        {
            var response = new ApiResponse<Models.SchoolingSearch.PaymentSchoolingInformationDto>();
            try
            {
                var find = _paymentSchoolingRepository.Find(x => x.Id == dto.Id);

                PaymentSchoolingInformation hl = _paymentSchoolingRepository.Update(_mapper.Map<PaymentSchoolingInformation>(dto), dto.Id);
                response.Result = _mapper.Map<Models.SchoolingSearch.PaymentSchoolingInformationDto>(hl);
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

        // Delete: Payment List
        [HttpPost("DeletePaymenteSchools", Name = "DeletePaymenteSchools")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult DeletePaymenteSchools(int key)
        {
            try
            {
                var find = _paymentSchoolingRepository.Find(f => f.Id == key);
                _paymentSchoolingRepository.Delete(find);

                return Ok(new { Success = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
    }
}
