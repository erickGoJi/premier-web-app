using api.premier.Models;
using api.premier.Models.Report;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Report;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IReportRepository _reportRepository;
        public ReportController(IMapper mapper, ILoggerManager loggerManager, IUtiltyRepository utiltyRepository, IReportRepository reportRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _utiltyRepository = utiltyRepository;
            _reportRepository = reportRepository;
        }
        // GET: Add Table
        [HttpPost]
        [Route("AddTable")]
        public ActionResult<ApiResponse<ReportInsertDto>> AddTable([FromBody] ReportInsertDto dto)
        {
            var response = new ApiResponse<ReportInsertDto>();
            try
            {
                response.Result = _mapper.Map<ReportInsertDto>(_reportRepository.Add(_mapper.Map<Report>(dto)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
            return StatusCode(202, response);
        }
        // GET: Edit Filters
        [HttpPost]
        [Route("EditFilters/{report}")]
        public ActionResult<ApiResponse<List<FilterDto>>> EditFilters([FromBody] List<FilterDto> dto, int report)
        {
            var response = new ApiResponse<List<FilterDto>>();
            try
            {
                response.Result = _mapper.Map<List<FilterDto>>(_reportRepository.AddOrEditFilters(_mapper.Map<List<Filter>>(dto), report));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
            return StatusCode(202, response);
        }
        // GET: Edit Columns
        [HttpPost]
        [Route("EditColumns/{report}")]
        public ActionResult<ApiResponse<List<ColumnDto>>> EditColumns([FromBody] List<ColumnDto> dto, int report)
        {
            var response = new ApiResponse<List<ColumnDto>>();
            try
            {
                response.Result = _mapper.Map<List<ColumnDto>>(_reportRepository.AddOrEditColumns(_mapper.Map<List<Column>>(dto), report));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
            return StatusCode(202, response);
        }

        // GET: Get Tables
        [HttpGet]
        [Route("GetTables/{user}/{report}")]
        public ActionResult GetTables(int user, int report)
        {
            try
            {
                return StatusCode(202, new { Success = true, Result = _reportRepository.GetCustom(user, report), Message = ""});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: Edit Filters
        [HttpPost]
        [Route("AddFiltersOpertionals/{report}/{reportType}")]
        public ActionResult<ApiResponse<List<FilterDto>>> AddFiltersOpertionals([FromBody] List<FilterDto> dto, int report, int reportType)
        {
            var response = new ApiResponse<List<FilterDto>>();
            try
            {
                response.Result = _mapper.Map<List<FilterDto>>(_reportRepository.AddFiltersOpertionals(_mapper.Map<List<Filter>>(dto), report, reportType));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
            return StatusCode(202, response);
        }

        // DELETE: Remove Report
        [HttpDelete]
        [Route("RemoveReport/{report}")]
        public ActionResult RemoveReport(int report)
        {
            try
            {
                var reportObj = _reportRepository.Find(x => x.Id == report);
                _reportRepository.Delete(reportObj);
                return StatusCode(202, new { Success = true, Result = 0, Message = "Delete was success." } );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        [HttpGet("GetReport/Test", Name = "GetReport/Test")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetReport()
        {
            try
            {
                return StatusCode(202, new { Success = true, Result = _reportRepository.ReturnReport(), Message = "Delete was success." } );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

    }
}
