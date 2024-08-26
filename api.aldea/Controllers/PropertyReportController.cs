using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.PropertyReport;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.PropertyReport;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IPropertyReportRepository _propertyReportRepository;
        private readonly IPhotoPropertyReportRepository _photoPropertyReportRepository;
        public PropertyReportController(IMapper mapper, ILoggerManager loggerManager, IUtiltyRepository utiltyRepository,
            IPropertyReportRepository propertyReportRepository, IPhotoPropertyReportRepository photoPropertyReportRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _propertyReportRepository = propertyReportRepository;
            _photoPropertyReportRepository = photoPropertyReportRepository;
        }

        [HttpPost("PostPropertyReport", Name = "PostPropertyReport")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PropertyReportDto>> PostPropertyReport([FromBody] PropertyReportDto dto)
        {
            var response = new ApiResponse<PropertyReportDto>();
            try
            {
                dto.CreatedDate = DateTime.Now;
                foreach (var p in dto.PropertyReportSections)
                {
                    foreach (var i in p.PhotosPropertyReportSections)
                    {
                        if (i.Id != 0)
                        {
                            if (i.Photo.Length > 150)
                            {
                                PhotosPropertyReportSection document = _photoPropertyReportRepository.Find(x => x.Id == i.Id);
                                //i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
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
                    foreach (var l in p.SectionInventories)
                    {
                        foreach (var s in l.PhotosInventories)
                        {
                            if (s.Id != 0)
                            {
                                if (s.Photo.Length > 150)
                                {
                                    PhotosInventory document = _propertyReportRepository.FindphotoInventory(s.Id);
                                    //s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", s.PhotoExtension);
                                    s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", "png");
                                    _utiltyRepository.DeleteFile(document.Photo);
                                }
                            }
                            else
                            {
                                //s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", s.PhotoExtension);
                                s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", "png");
                            }
                        }
                    }
                }
                PropertyReport immi = _propertyReportRepository.Add(_mapper.Map<PropertyReport>(dto));
                response.Result = _mapper.Map<PropertyReportDto>(immi);
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

        [HttpPut("PutPropertyReport", Name = "PutPropertyReport")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PropertyReportDto>> PutPropertyReport([FromBody] PropertyReportDto dto)
        {
            var response = new ApiResponse<PropertyReportDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;
                foreach (var p in dto.PropertyReportSections)
                {
                    foreach(var i in p.PhotosPropertyReportSections)
                    {
                        if (i.Id != 0)
                        {
                            if (i.Photo.Length > 150)
                            {
                                PhotosPropertyReportSection document = _photoPropertyReportRepository.Find(x => x.Id == i.Id);
                                //i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
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
                    foreach(var l in p.SectionInventories)
                    {
                        foreach(var s in l.PhotosInventories)
                        {
                            if (s.Id != 0)
                            {
                                if (s.Photo.Length > 150)
                                {
                                    PhotosInventory document = _propertyReportRepository.FindphotoInventory(s.Id);
                                    //s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", s.PhotoExtension);
                                    s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", "png");
                                    _utiltyRepository.DeleteFile(document.Photo);
                                }
                            }
                            else
                            {
                                //s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", s.PhotoExtension);
                                s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", "png");
                            }
                        }
                    }
                }
                PropertyReport immi = _propertyReportRepository.UpdateCustom(_mapper.Map<PropertyReport>(dto), dto.Id);
                response.Result = _mapper.Map<PropertyReportDto>(immi);
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

        [HttpPut("PutPropertyReportSection", Name = "PutPropertyReportSection")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<PropertyReportSectionDto>> PutPropertyReportSection([FromBody] PropertyReportSectionDto dto)
        {
            var response = new ApiResponse<PropertyReportSectionDto>();
            try
            {
                dto.UpdatedDate = DateTime.Now;

                    foreach (var i in dto.PhotosPropertyReportSections)
                    {
                        if (i.Id != 0)
                        {
                            if (i.Photo.Length > 150)
                            {
                                PhotosPropertyReportSection document = _photoPropertyReportRepository.Find(x => x.Id == i.Id);
                            //i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/PropertyReport/", i.PhotoExtension);
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
                    foreach (var l in dto.SectionInventories)
                    {
                        foreach (var s in l.PhotosInventories)
                        {
                            if (s.Id != 0)
                            {
                                if (s.Photo.Length > 150)
                                {
                                    PhotosInventory document = _propertyReportRepository.FindphotoInventory(s.Id);
                                //s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", s.PhotoExtension);
                                s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", "png");
                                _utiltyRepository.DeleteFile(document.Photo);
                                }
                            }
                            else
                            {
                            //s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", s.PhotoExtension);
                            s.Photo = _utiltyRepository.UploadImageBase64(s.Photo, "Files/PropertyReport/PhotosInventories/", "png");
                        }
                        }
                    }

                PropertyReportSection immi = _propertyReportRepository.UpdateCustomSection(_mapper.Map<PropertyReportSection>(dto), dto.Id);
                response.Result = _mapper.Map<PropertyReportSectionDto>(immi);
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


        [HttpGet("GetPropertyReportById", Name = "GetPropertyReportById")]
        public ActionResult<ApiResponse<PropertyReportSelectDto>> GetPropertyReportById([FromQuery] int id)
        {
            var response = new ApiResponse<PropertyReportSelectDto>();
            try
            {
                var map = _mapper.Map<PropertyReportSelectDto>(_propertyReportRepository.GetCustom(id));
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

        
        [HttpGet]
        [Route("get_status_report")]
        public ActionResult<ApiResponse<List<StatusPropertyReport>>> get_status_report()
        {
            var response = new ApiResponse<List<StatusPropertyReport>>();
            try
            {
                response.Result = _propertyReportRepository.GetAllStatusPropertyReport();
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        [HttpGet("GetPropertyReportByHosuing", Name = "GetPropertyReportByHousing")]
        public ActionResult GetPropertyReportByHousing([FromQuery] int id)
        {
            try
            {
                var map = _propertyReportRepository.GetAllIncluding(x => x.CreatedByNavigation, u => u.PropertyReportSections);
                var result = map.Where(x => x.HousingList == id).Select(s => new
                {
                    s.Id,
                    s.CreatedDate,
                    s.ReportDate,
                    reportedBy = s.CreatedByNavigation.Name + " " + s.CreatedByNavigation.LastName + " " + s.CreatedByNavigation.MotherLastName,
                    s.PropertyAddress,
                    s.ZipCode,
                    s.PropertyReportSections.Count
                }).FirstOrDefault();
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, new { Success = false, Message = ex.Message});
            }
        }

        [HttpGet("GetSupplierPartner/{sr}", Name = "GetSupplierPartner")]
        public ActionResult GetSupplierPartner([FromQuery] int? supplier_type, int? supplier, int sr)
        {
            try
            {
                return Ok(new { Success = true, Result = _propertyReportRepository.GetsupplierPartner(supplier_type, supplier, sr) });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = true, Message = ex.ToString() });
            }

        }

        //[HttpGet("GetSupplierPartner/{section}", Name = "GetSupplierPartner")]
        //public ActionResult GetSupplierPartner([FromQuery] int section)
        //{
        //    try
        //    {
        //        re

        //        return Ok(new { Success = true, Result = _propertyReportRepository.GetsupplierPartner(supplier_type, supplier, sr) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { Success = true, Message = ex.ToString() });
        //    }

        //}
    }
}
