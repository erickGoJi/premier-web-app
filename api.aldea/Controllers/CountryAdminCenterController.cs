using api.premier.ActionFilter;
using api.premier.Models;
using AutoMapper;
using biz.premier.Repository.ClientPartner;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using api.premier.Models.ClientPartnerProfile;
using biz.premier.Entities;
using api.premier.Models.Catalogue;
using biz.premier.Repository.Country;
using api.premier.Models.Country;
using api.premier.Models.Catalogos;
using biz.premier.Repository.City;
using biz.premier.Repository.Countries;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryAdminCenterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityGenericRepository _cityGenericRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICountryDocumentRepository _countryDocumentRepository;
        private readonly ICityAboutRepository _cityAboutRepository;
        private readonly ICityAttractionsRepository _cityAttractionsRepository;
        private readonly ICityEmergencyRepository _cityEmergencyRepository;
        private readonly ICityWhatToDoRepository _cityWhatToDoRepository;
        private readonly ICityWhereEatRepository _cityWhereEatRepository;
        private readonly IPhotoAboutRepository _photoAboutRepository;
        private readonly IPhotoAttractionsRepository _photoAttractionsRepository;
        private readonly IPhotoEmergencyRepository _photoEmergencyRepository;
        private readonly IPhotoWhatToDoRepository _photoWhatToDoRepository;
        private readonly IPhotoWhereEatRepository _photoWhereEatRepository;
        private readonly IUtiltyRepository _utiltyRepository;

        public CountryAdminCenterController(IMapper mapper, ILoggerManager loggerManager, 
            ICountryRepository countryRepository,
            ICityGenericRepository cityGenericRepository,
            ICityRepository cityRepository,
            ICountryDocumentRepository countryDocumentRepository,
            ICityGenericRepository countryGenericRepository,
            ICityAboutRepository cityAboutRepository,
            ICityAttractionsRepository cityAttractionsRepository,
            ICityEmergencyRepository cityEmergencyRepository,
            ICityWhatToDoRepository cityWhatToDoRepository,
            ICityWhereEatRepository cityWhereEatRepository,
            IPhotoAboutRepository photoAboutRepository,
            IPhotoAttractionsRepository photoAttractionsRepository,
            IPhotoEmergencyRepository photoEmergencyRepository,
            IPhotoWhatToDoRepository photoWhatToDoRepository,
            IPhotoWhereEatRepository photoWhereEatRepository,
            IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _cityGenericRepository = cityGenericRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _utiltyRepository = utiltyRepository;
            _countryDocumentRepository = countryDocumentRepository;
            _cityAboutRepository = cityAboutRepository;
            _cityEmergencyRepository = cityEmergencyRepository;
            _cityAttractionsRepository = cityAttractionsRepository;
            _cityWhatToDoRepository = cityWhatToDoRepository;
            _cityWhereEatRepository = cityWhereEatRepository;
            _photoAboutRepository = photoAboutRepository;
            _photoAttractionsRepository = photoAttractionsRepository;
            _photoEmergencyRepository = photoEmergencyRepository;
            _photoWhatToDoRepository = photoWhatToDoRepository;
            _photoWhereEatRepository = photoWhereEatRepository;
        }

        // Post Create new Cat Country 
        [HttpPost("AddCountry", Name = "AddCountry")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CatCountryDto>> AddCountry([FromBody] CatCountryDto dto)
        {
            var response = new ApiResponse<CatCountryDto>();
            try
            {
                if (_countryRepository.Exists(f=>f.Name == dto.Name))
                {
                    response.Result = null;
                    response.Success = false;
                    response.Message = $"Country {dto.Name} already Exists";
                    return StatusCode(409, response);
                }

                if (dto.CatCities.Any())
                {
                    foreach (var city in dto.CatCities)
                    {
                        if (_cityRepository.Exists(e=>e.City==city.City))
                        {
                            response.Result = null;
                            response.Success = false;
                            response.Message = $"City {city.City} already Exists";
                            return StatusCode(409, response);
                        }
                    }
                }
                
                dto.CreatedDate = DateTime.Now;
                if (dto.CountryDocuments != null)
                {
                    foreach (var j in dto.CountryDocuments)
                    {
                        if (_utiltyRepository.IsBase64(j.FileRequest))
                        {
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileExtencion);
                        }
                    }
                }

                
                if (dto.CatCities != null) {

                    foreach (var j in dto.CatCities)
                    {
                        if (_utiltyRepository.IsBase64(j.FileRequest))
                        {
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileName.Split(".")[1]);
                        }

                        if (_utiltyRepository.IsBase64(j.ResorucesGuide))
                        {
                            j.FileRequest = _utiltyRepository.UploadImageBase64(j.ResorucesGuideRequest, "Files/Country/", j.ResorucesGuide.Split(".")[1]);
                        }
                    }

                    foreach (var j in dto.CatCities)
                    {
                        j.CreateDate = DateTime.Now;
                        if (j.CityAbouts != null)
                        {
                            foreach (var i in j.CityAbouts)
                            {
                                if (i.PhotoCityAbouts != null)
                                {
                                    foreach (var x in i.PhotoCityAbouts)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var j in dto.CatCities)
                    {
                        if (j.CityAttractions != null)
                        {
                            foreach (var i in j.CityAttractions)
                            {
                                if (i.PhotoCityAttractions != null)
                                {
                                    foreach (var x in i.PhotoCityAttractions)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var j in dto.CatCities)
                    {
                        if (j.CityEmergencies != null)
                        {
                            foreach (var i in j.CityEmergencies)
                            {
                                if (i.PhotoCityEmergencies != null)
                                {
                                    foreach (var x in i.PhotoCityEmergencies)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var j in dto.CatCities)
                    {
                        if (j.CityWhatToDos != null)
                        {
                            foreach (var i in j.CityWhatToDos)
                            {
                                if (i.PhotoWhatToDos != null)
                                {
                                    foreach (var x in i.PhotoWhatToDos)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var j in dto.CatCities)
                    {
                        if (j.CityWhereEats != null)
                        {
                            foreach (var i in j.CityWhereEats)
                            {
                                if (i.PhotoWhereEats != null)
                                {
                                    foreach (var x in i.PhotoWhereEats)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                CatCountryDto country = _mapper.Map<CatCountryDto>(_countryRepository.Add(_mapper.Map<CatCountry>(dto)));

                response.Success = true;
                response.Message = "Success";
                response.Result = country;
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

        // Put Cat Country
        //[HttpPut("UpdateCountry", Name = "UpdateCountry")]
        //[ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        //public ActionResult<ApiResponse<CatCountryDto>> UpdateCountry([FromBody] CatCountryDto dto)
        //{
        //    var response = new ApiResponse<CatCountryDto>();
        //    try
        //    {
        //        dto.UpdatedDate = DateTime.Now;
        //        if (dto.CountryDocuments != null) {
        //            foreach (var j in dto.CountryDocuments)
        //            {
        //                if (j.Id != 0)
        //                {
        //                    if (_utiltyRepository.IsBase64(j.FileRequest))
        //                    {
        //                        j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileExtencion);
        //                    }
        //                }
        //                else {
        //                    if (_utiltyRepository.IsBase64(j.FileRequest))
        //                    {
        //                        j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileExtencion);
        //                    }
        //                }
        //            }
        //        }
                
        //        foreach (var j in dto.CatCities)
        //        {
        //            if (j.Id != 0)
        //            {
        //                if (j.CityAbouts != null)
        //                {
        //                    foreach (var i in j.CityAbouts)
        //                    {
        //                        if (i.PhotoCityAbouts != null)
        //                        {
        //                            foreach (var x in i.PhotoCityAbouts)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (j.CityAbouts != null)
        //                {
        //                    foreach (var i in j.CityAbouts)
        //                    {
        //                        if (i.PhotoCityAbouts != null)
        //                        {
        //                            foreach (var x in i.PhotoCityAbouts)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var j in dto.CatCities)
        //        {
        //            if (j.Id != 0)
        //            {
        //                if (j.CityAttractions != null)
        //                {
        //                    foreach (var i in j.CityAttractions)
        //                    {
        //                        if (i.PhotoCityAttractions != null)
        //                        {
        //                            foreach (var x in i.PhotoCityAttractions)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else {
        //                if (j.CityAttractions != null)
        //                {
        //                    foreach (var i in j.CityAttractions)
        //                    {
        //                        if (i.PhotoCityAttractions != null)
        //                        {
        //                            foreach (var x in i.PhotoCityAttractions)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var j in dto.CatCities)
        //        {
        //            if (j.Id != 0)
        //            {
        //                if (j.CityEmergencies != null)
        //                {
        //                    foreach (var i in j.CityEmergencies)
        //                    {
        //                        if (i.PhoneCityEmergencies != null)
        //                        {
        //                            foreach (var x in i.PhoneCityEmergencies)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else {
        //                if (j.CityEmergencies != null)
        //                {
        //                    foreach (var i in j.CityEmergencies)
        //                    {
        //                        if (i.PhoneCityEmergencies != null)
        //                        {
        //                            foreach (var x in i.PhoneCityEmergencies)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var j in dto.CatCities)
        //        {
        //            if (j.Id != 0)
        //            {
        //                if (j.CityWhatToDos != null)
        //                {
        //                    foreach (var i in j.CityWhatToDos)
        //                    {
        //                        if (i.PhotoWhatToDos != null)
        //                        {
        //                            foreach (var x in i.PhotoWhatToDos)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else {
        //                if (j.CityWhatToDos != null)
        //                {
        //                    foreach (var i in j.CityWhatToDos)
        //                    {
        //                        if (i.PhotoWhatToDos != null)
        //                        {
        //                            foreach (var x in i.PhotoWhatToDos)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var j in dto.CatCities)
        //        {
        //            if (j.Id != 0)
        //            {
        //                if (j.CityWhereEats != null)
        //                {
        //                    foreach (var i in j.CityWhereEats)
        //                    {
        //                        if (i.PhotoWhereEats != null)
        //                        {
        //                            foreach (var x in i.PhotoWhereEats)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else {
        //                if (j.CityWhereEats != null)
        //                {
        //                    foreach (var i in j.CityWhereEats)
        //                    {
        //                        if (i.PhotoWhereEats != null)
        //                        {
        //                            foreach (var x in i.PhotoWhereEats)
        //                            {
        //                                if (_utiltyRepository.IsBase64(x.FileRequest))
        //                                {
        //                                    x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        CatCountryDto country = _mapper.Map<CatCountryDto>(_countryRepository.Update(_mapper.Map<CatCountry>(dto), dto.Id));

        //        response.Success = true;
        //        response.Message = "Success";
        //        response.Result = country;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Result = null;
        //        response.Success = false;
        //        response.Message = ex.ToString();
        //        _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
        //        return StatusCode(500, response);
        //    }
        //    return StatusCode(201, response);
        //}

        [HttpPut]
        [Route("UpdateCountry")]
        public ActionResult<ApiResponse<CatCountryDto>> UpdateCountry([FromBody] CatCountryDto dto)
        {
            var response = new ApiResponse<CatCountryDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                if (dto.CountryDocuments != null)
                {
                    foreach (var j in dto.CountryDocuments)
                    {
                        if (j.Id != 0)
                        {
                            if (_utiltyRepository.IsBase64(j.FileRequest))
                            {
                                j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileExtencion);
                            }
                        }
                        else
                        {
                            if (_utiltyRepository.IsBase64(j.FileRequest))
                            {
                                j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileExtencion);
                            }
                        }
                    }
                }

                foreach (var j in dto.CatCities)
                {
                    if (_utiltyRepository.IsBase64(j.FileRequest))
                    {
                        j.FileRequest = _utiltyRepository.UploadImageBase64(j.FileRequest, "Files/Country/", j.FileName.Split(".")[1]);
                    }

                    if (_utiltyRepository.IsBase64(j.ResorucesGuideRequest))
                    {
                        j.ResorucesGuideRequest = _utiltyRepository.UploadImageBase64(j.ResorucesGuideRequest, "Files/Country/", j.ResorucesGuide.Split(".")[1]);
                    }
                }

                foreach (var j in dto.CatCities)
                {
                    if (j.Id != 0)
                    {
                        if (j.CityAbouts != null)
                        {
                            foreach (var i in j.CityAbouts)
                            {
                                if (i.PhotoCityAbouts != null)
                                {
                                    foreach (var x in i.PhotoCityAbouts)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j.CityAbouts != null)
                        {
                            foreach (var i in j.CityAbouts)
                            {
                                if (i.PhotoCityAbouts != null)
                                {
                                    foreach (var x in i.PhotoCityAbouts)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var j in dto.CatCities)
                {
                    if (j.Id != 0)
                    {
                        if (j.CityAttractions != null)
                        {
                            foreach (var i in j.CityAttractions)
                            {
                                if (i.PhotoCityAttractions != null)
                                {
                                    foreach (var x in i.PhotoCityAttractions)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j.CityAttractions != null)
                        {
                            foreach (var i in j.CityAttractions)
                            {
                                if (i.PhotoCityAttractions != null)
                                {
                                    foreach (var x in i.PhotoCityAttractions)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var j in dto.CatCities)
                {
                    if (j.Id != 0)
                    {
                        if (j.CityEmergencies != null)
                        {
                            foreach (var i in j.CityEmergencies)
                            {
                                if (i.PhotoCityEmergencies != null)
                                {
                                    foreach (var x in i.PhotoCityEmergencies)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j.CityEmergencies != null)
                        {
                            foreach (var i in j.CityEmergencies)
                            {
                                if (i.PhotoCityEmergencies != null)
                                {
                                    foreach (var x in i.PhotoCityEmergencies)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var j in dto.CatCities)
                {
                    if (j.Id != 0)
                    {
                        if (j.CityWhatToDos != null)
                        {
                            foreach (var i in j.CityWhatToDos)
                            {
                                if (i.PhotoWhatToDos != null)
                                {
                                    foreach (var x in i.PhotoWhatToDos)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j.CityWhatToDos != null)
                        {
                            foreach (var i in j.CityWhatToDos)
                            {
                                if (i.PhotoWhatToDos != null)
                                {
                                    foreach (var x in i.PhotoWhatToDos)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var j in dto.CatCities)
                {
                    if (j.Id != 0)
                    {
                        if (j.CityWhereEats != null)
                        {
                            foreach (var i in j.CityWhereEats)
                            {
                                if (i.PhotoWhereEats != null)
                                {
                                    foreach (var x in i.PhotoWhereEats)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (j.CityWhereEats != null)
                        {
                            foreach (var i in j.CityWhereEats)
                            {
                                if (i.PhotoWhereEats != null)
                                {
                                    foreach (var x in i.PhotoWhereEats)
                                    {
                                        if (_utiltyRepository.IsBase64(x.FileRequest))
                                        {
                                            x.FileRequest = _utiltyRepository.UploadImageBase64(x.FileRequest, "Files/Country/", x.FileExtencion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                response.Success = true;
                response.Message = "Success";
                response.Result = _mapper.Map<CatCountryDto>(_countryRepository.UpdateCustom(_mapper.Map<CatCountry>(dto)));
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

        [HttpGet]
        [Route("GetCountry")]
        public ActionResult GetCountry()
        {
            try
            {
                return Ok(new { Success = true, result = _countryRepository.GetCountry() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }



        [HttpGet]
        [Route("GetCountryById")]
        public ActionResult GetCountryById(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _countryRepository.GetCountryById(id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetCityByCountryName")]
        public ActionResult GetCityByCountryName(string countryName)
        {
            try
            {
                return Ok(new { Success = true, result = _cityGenericRepository.GetCtyByCountry(countryName) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetCityByCountryCityNames")]
        public ActionResult GetCityByCountryCityNames(string countryName, string cityname)
        {
            try
            {
                //return Ok(new { Success = true, result = _cityGenericRepository.GetCtyByCountry(countryName) });
                return Ok(new { Success = true, result = _cityGenericRepository.GetCityByCountryCityNames(countryName, cityname) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet]
        [Route("GetCityByCountryId")]
        public ActionResult GetCityByCountryId(int countryId)
        {
            try
            {
                return Ok(new { Success = true, result = _cityGenericRepository.GetCtyByCountryId(countryId) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpGet("{user}/Country-City-Info")]
        public ActionResult GetCountryCityInfo(int user)
        {
            try
            {
                return Ok(new { Success = true, result = _countryRepository.GetCountryCityInfo(user) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }
        
        [HttpGet("Leader-List")]
        public ActionResult GetLeaderList()
        {
            try
            {
                return Ok(new { Success = true, result = _countryRepository.GetUserList() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpDelete]
        [Route("DeleteCountry")]
        public ActionResult DeleteCountryById(int id)
        {
            try
            {
                var country = _countryRepository.Find(c => c.Id == id);
                _countryRepository.Delete(country);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
                //return Ok(new { Success = false, Message = ex.ToString() });
            }
        }

        [HttpDelete]
        [Route("DeleteCity")]
        public ActionResult DeleteCity(int id)
        {
            try
            {
                var city = _cityRepository.Find(c => c.Id == id);
                _cityRepository.Delete(city);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeleteCountryDocument")]
        public ActionResult DeleteCountryDocument(int id)
        {
            try
            {
                var document = _countryDocumentRepository.Find(c => c.Id == id);
                CountryDocument _document = _countryDocumentRepository.Find(x => x.Id == id);
                _utiltyRepository.DeleteFile(_document.FileRequest);
                _countryDocumentRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeleteCityAbout")]
        public ActionResult DeleteCityAbout(int id)
        {
            try
            {
                var document = _cityAboutRepository.Find(c => c.Id == id);
                _cityAboutRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeleteCityAttractions")]
        public ActionResult DeleteCityAttractions(int id)
        {
            try
            {
                var document = _cityAttractionsRepository.Find(c => c.Id == id);
                _cityAttractionsRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeleteCityEmergency")]
        public ActionResult DeleteCityEmergency(int id)
        {
            try
            {
                var document = _cityEmergencyRepository.Find(c => c.Id == id);
                _cityEmergencyRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeleteCityWhatToDo")]
        public ActionResult DeleteCityWhatToDo(int id)
        {
            try
            {
                var document = _cityWhatToDoRepository.Find(c => c.Id == id);
                _cityWhatToDoRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeleteCityWhereEat")]
        public ActionResult DeleteCityWhereEat(int id)
        {
            try
            {
                var document = _cityWhereEatRepository.Find(c => c.Id == id);
                _cityWhereEatRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeletePhotoAbout")]
        public ActionResult DeletePhotoAbout(int id)
        {
            try
            {
                var document = _photoAboutRepository.Find(c => c.Id == id);
                PhotoCityAbout _document = _photoAboutRepository.Find(x => x.Id == id);
                _utiltyRepository.DeleteFile(_document.FileRequest);
                _photoAboutRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeletePhotoAttraction")]
        public ActionResult DeletePhotoAttraction(int id)
        {
            try
            {
                var document = _photoAttractionsRepository.Find(c => c.Id == id);
                PhotoCityAttraction _document = _photoAttractionsRepository.Find(x => x.Id == id);
                _utiltyRepository.DeleteFile(_document.FileRequest);
                _photoAttractionsRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeletePhotoEmergency")]
        public ActionResult DeletePhotoEmergency(int id)
        {
            try
            {
                var document = _photoEmergencyRepository.Find(c => c.Id == id);
                PhotoCityEmergency _document = _photoEmergencyRepository.Find(x => x.Id == id);
                _utiltyRepository.DeleteFile(_document.FileRequest);
                _photoEmergencyRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeletePhotoWhatToDo")]
        public ActionResult DeletePhotoWhatToDo(int id)
        {
            try
            {
                var document = _photoWhatToDoRepository.Find(c => c.Id == id);
                PhotoWhatToDo _document = _photoWhatToDoRepository.Find(x => x.Id == id);
                _utiltyRepository.DeleteFile(_document.FileRequest);
                _photoWhatToDoRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpDelete]
        [Route("DeletePhotoWhereEat")]
        public ActionResult DeletePhotoWhereEat(int id)
        {
            try
            {
                var document = _photoWhereEatRepository.Find(c => c.Id == id);
                PhotoWhereEat _document = _photoWhereEatRepository.Find(x => x.Id == id);
                _utiltyRepository.DeleteFile(_document.FileRequest);
                _photoWhereEatRepository.Delete(document);
                return Ok(new { Success = true, Message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }
        
        [HttpPost("Country-Document-Group",Name = "Country-Document-Group")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CountryDocumentGroupDto>> AddCountryDocumentGroup(CountryDocumentGroupDto groupDto)
        {
            var response = new ApiResponse<CountryDocumentGroupDto>();
            try
            {
                response.Result = _mapper.Map<CountryDocumentGroupDto>(_countryRepository.AddGroup(_mapper.Map<CountryDocumentGroup>(groupDto)));
                response.Success = true;
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
        
        [HttpPut("Country-Document-Group",Name = "Country-Document-Group")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<CountryDocumentGroupDto>> UpdateCountryDocumentGroup(CountryDocumentGroupDto groupDto)
        {
            var response = new ApiResponse<CountryDocumentGroupDto>();
            try
            {
                response.Result = _mapper.Map<CountryDocumentGroupDto>(_countryRepository.UpdateGroup(_mapper.Map<CountryDocumentGroup>(groupDto)));
                response.Success = true;
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
        
        [HttpGet("Country-Document-Group", Name = "Country-Document-Group")]
        public ActionResult GetCountryDocumentGroupList()
        {
            try
            {
                return Ok(new { Success = true, Message = "Success", Result = _countryRepository.GetGroups() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }

        [HttpGet("GetTypeResources", Name = "GetTypeResources")]
        public ActionResult GetTypeResources()
        {
            try
            {
                return Ok(new { Success = true, Message = "Success", Result = _countryRepository.GetTypeResources() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message.ToString()}");
                return StatusCode(409, Ok(new { Success = false, Message = ex.ToString() }));
            }
        }
    }
}
