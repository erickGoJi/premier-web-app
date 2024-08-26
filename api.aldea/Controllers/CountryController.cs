using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.Country;
using AutoMapper;
using biz.premier.Repository.CountryGallery;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ICountryRepository _countryRepository;
        private readonly ICountryGalleryRepository _countryGalleryRepository;
        public CountryController(IMapper mapper, ILoggerManager loggerManager, ICountryRepository countryRepository, ICountryGalleryRepository countryGalleryRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _countryRepository = countryRepository;
            _countryGalleryRepository = countryGalleryRepository;
        }

        [HttpGet("GetCountryById", Name = "GetCountrybyId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CountryDto>> GetCountrybyId([FromQuery] int id)
        {
            var response = new ApiResponse<CountryDto>();
            try
            {
                var Result = _countryRepository.Find(x => x.Id == id);
                if(Result != null)
                {
                    Result.CountryGalleries = _countryGalleryRepository.FindAll(x => x.CountryId == Result.Id);
                    response.Result = _mapper.Map<CountryDto>(Result);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Country Not Found";
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

    }
}
