using api.premier.Models;
using api.premier.Models.Catalogos;
using api.premier.Models.ClientPartnerProfile;
using api.premier.Models.SupplierPartnerProfileConsultant;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Catalogue;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly ICurrencyRepository _currencyRepository; 
        private readonly ILanguagesRepository _languagesRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILifeCircleRepository _lifeCircleRepository;
        private readonly IProfileUserRepository _profileUserRepository;
        public CatalogController(IMapper mapper, ILoggerManager loggerManager, 
            ILanguagesRepository languagesRepository, ICurrencyRepository currencyRepository,
            IOfficeRepository officeRepository, IUserRepository userRepository, IUtiltyRepository utiltyRepository,
            IRoleRepository roleRepository, ITitleRepository titleRepository, ILifeCircleRepository lifeCircleRepository,
            IProfileUserRepository profileUserRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _currencyRepository = currencyRepository;
            _languagesRepository = languagesRepository;
            _officeRepository = officeRepository;
            _userRepository = userRepository;
            _utiltyRepository = utiltyRepository;
            _roleRepository = roleRepository;
            _titleRepository = titleRepository;
            _lifeCircleRepository = lifeCircleRepository;
            _profileUserRepository = profileUserRepository;
        }
        #region Languages
        // GET: Cat AllLanguages
        [HttpGet]
        [Route("GetAllLanguages")]
        public ActionResult<ApiResponse<List<CatLanguagesDto>>> GetLanguages()
        {
            var response = new ApiResponse<List<CatLanguagesDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatLanguagesDto>>(_languagesRepository.GetAll());
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

        // GET: Cat Language by Id
        [HttpGet]
        [Route("GetLanguage/{id}")]
        public ActionResult<ApiResponse<CatLanguagesDto>> GetLanguage(int id)
        {
            var response = new ApiResponse<CatLanguagesDto>();
            try
            {
                response.Result = _mapper.Map<CatLanguagesDto>(_languagesRepository.Find(x => x.Id == id));
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

        // Add: Cat Languages
        [HttpPost]
        [Route("AddLanguage")]
        public ActionResult<ApiResponse<CatLanguagesDto>> PostLanguage([FromBody] CatLanguagesDto dto)
        {
            var response = new ApiResponse<CatLanguagesDto>();
            try
            {
                response.Result = _mapper.Map<CatLanguagesDto>(_languagesRepository.Add(_mapper.Map<CatLanguage>(dto)));
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

        // Update: Cat Languages
        [HttpPut]
        [Route("UpdateLanguage")]
        public ActionResult<ApiResponse<CatLanguagesDto>> PutLanguage([FromBody]CatLanguagesDto dto)
        {
            var response = new ApiResponse<CatLanguagesDto>();
            try
            {
                response.Result = _mapper.Map<CatLanguagesDto>(_languagesRepository.Update(_mapper.Map<CatLanguage>(dto), dto.Id));
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
        // DELETE: Cat Language by Id
        [HttpDelete]
        [Route("DeleteLanguage/{id}")]
        public ActionResult<ApiResponse<CatLanguagesDto>> DeleteLanguage(int id)
        {
            var response = new ApiResponse<CatLanguagesDto>();
            try
            {
                var record = _languagesRepository.Find(x => x.Id == id);
                _languagesRepository.Delete(record);
                response.Result = _mapper.Map<CatLanguagesDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        #endregion 
        #region Currency
        // GET: Cat AllCurrency
        [HttpGet]
        [Route("GetAllCurrency")]
        public ActionResult<ApiResponse<List<CatCurrencyDto>>> GetCurrencies()
        {
            var response = new ApiResponse<List<CatCurrencyDto>>();
            try
            {
                response.Result = _mapper.Map<List<CatCurrencyDto>>(_currencyRepository.GetAll());
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

        // GET: Cat Currency by Id
        [HttpGet]
        [Route("GetCurrency/{id}")]
        public ActionResult<ApiResponse<CatCurrencyDto>> GetCurrency(int id)
        {
            var response = new ApiResponse<CatCurrencyDto>();
            try
            {
                response.Result = _mapper.Map<CatCurrencyDto>(_currencyRepository.Find(x => x.Id == id));
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

        // Add: Cat Languages
        [HttpPost]
        [Route("AddCurrency")]
        public ActionResult<ApiResponse<CatCurrencyDto>> AddCurrency([FromBody] CatCurrencyDto dto)
        {
            var response = new ApiResponse<CatCurrencyDto>();
            try
            {
                response.Result = _mapper.Map<CatCurrencyDto>(_currencyRepository.Add(_mapper.Map<CatCurrency>(dto)));
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

        // Update: Cat Languages
        [HttpPut]
        [Route("UpdateCurrency")]
        public ActionResult<ApiResponse<CatCurrencyDto>> UpdateCurrency([FromBody] CatCurrencyDto dto)
        {
            var response = new ApiResponse<CatCurrencyDto>();
            try
            {
                response.Result = _mapper.Map<CatCurrencyDto>(_currencyRepository.Update(_mapper.Map<CatCurrency>(dto), dto.Id));
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
        // DELETE: Cat Currency by Id
        [HttpDelete]
        [Route("DeleteCurrency/{id}")]
        public ActionResult<ApiResponse<CatCurrencyDto>> DeleteCurrency(int id)
        {
            var response = new ApiResponse<CatCurrencyDto>();
            try
            {
                var record = _currencyRepository.Find(x => x.Id == id);
                _currencyRepository.Delete(record);
                response.Result = _mapper.Map<CatCurrencyDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        #endregion 
        #region Office
        // GET: Cat AllOffice
        [HttpGet]
        [Route("GetAllOffice")]
        public ActionResult GetOfficis()
        {
            try
            {
                var offices = _officeRepository.GetAllIncluding(x => x.CountryNavigation, y => y.CityNavigation);
                return StatusCode(202, new { Success = true, Result = offices.Select(s => new
                {
                    s.Id,
                    s.Office,
                    country = s.CountryNavigation.Name,
                    s.CityNavigation.City,
                    s.Phone
                }), Message = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: Cat Office by Id
        [HttpGet]
        [Route("GetOffice/{id}")]
        public ActionResult<ApiResponse<CatOfficeDto>> GetOffice(int id)
        {
            var response = new ApiResponse<CatOfficeDto>();
            try
            {
                response.Result = _mapper.Map<CatOfficeDto>(_officeRepository.Find(x => x.Id == id));
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

        // Add: Cat Office
        [HttpPost]
        [Route("AddOffice")]
        public ActionResult<ApiResponse<CatOfficeDto>> AddOffice([FromBody] CatOfficeDto dto)
        {
            var response = new ApiResponse<CatOfficeDto>();
            try
            {
                if (dto.Image.Length > 150)
                    dto.Image = _utiltyRepository.UploadImageBase64(dto.Image, "Files/Office/", dto.ImageExtension);
                response.Result = _mapper.Map<CatOfficeDto>(_officeRepository.Add(_mapper.Map<CatOffice>(dto)));
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

        // Update: Cat Office
        [HttpPut]
        [Route("UpdateOffice")]
        public ActionResult<ApiResponse<CatOfficeDto>> UpdateOffice([FromBody] CatOfficeDto dto)
        {
            var response = new ApiResponse<CatOfficeDto>();
            try
            {
                if (dto.Image.Length > 150)
                    dto.Image = _utiltyRepository.UploadImageBase64(dto.Image, "Files/Office/", dto.ImageExtension);
                response.Result = _mapper.Map<CatOfficeDto>(_officeRepository.Update(_mapper.Map<CatOffice>(dto), dto.Id));
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
        // DELETE: Cat Office by Id
        [HttpDelete]
        [Route("DeleteOffice/{id}")]
        public ActionResult<ApiResponse<CatOfficeDto>> DeleteOffice(int id)
        {
            var response = new ApiResponse<CatOfficeDto>();
            try
            {
                var record = _officeRepository.Find(x => x.Id == id);
                _officeRepository.Delete(record);
                response.Result = _mapper.Map<CatOfficeDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        #endregion 
        #region Title
        // GET: Cat All Title
        [HttpGet]
        [Route("GetAllTitle")]
        public ActionResult GetAllTitle()
        {
            try
            {
                var offices = _titleRepository.GetAllIncluding(x => x.OfficeNavigation);
                return StatusCode(202, new
                {
                    Success = true,
                    Result = offices.Select(s => new
                    {
                        s.Id,
                        s.OfficeNavigation.Office,
                        s.Section,
                        s.Title
                    }),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: Cat Title by Id
        [HttpGet]
        [Route("GetTitle/{id}")]
        public ActionResult<ApiResponse<CatTitleDto>> GetTitle(int id)
        {
            var response = new ApiResponse<CatTitleDto>();
            try
            {
                response.Result = _mapper.Map<CatTitleDto>(_titleRepository.Find(x => x.Id == id));
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

        // Add: Cat Office
        [HttpPost]
        [Route("AddTitle")]
        public ActionResult<ApiResponse<CatTitleDto>> AddRole([FromBody] CatTitleDto dto)
        {
            var response = new ApiResponse<CatTitleDto>();
            try
            {
                response.Result = _mapper.Map<CatTitleDto>(_titleRepository.Add(_mapper.Map<CatTitle>(dto)));
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

        // Update: Cat Office
        [HttpPut]
        [Route("UpdateTitle")]
        public ActionResult<ApiResponse<CatTitleDto>> UpdateRole([FromBody] CatTitleDto dto)
        {
            var response = new ApiResponse<CatTitleDto>();
            try
            {
                response.Result = _mapper.Map<CatTitleDto>(_titleRepository.Update(_mapper.Map<CatTitle>(dto), dto.Id));
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
        // DELETE: Cat Title by Id
        [HttpDelete]
        [Route("DeleteTitle/{id}")]
        public ActionResult<ApiResponse<CatTitleDto>> DeleteTitle(int id)
        {
            var response = new ApiResponse<CatTitleDto>();
            try
            {
                var record = _titleRepository.Find(x => x.Id == id);
                _titleRepository.Delete(record);
                response.Result = _mapper.Map<CatTitleDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        #endregion
        #region Role
        // GET: Cat All Role
        [HttpGet]
        [Route("GetAllRole")]
        public ActionResult GetAllRole()
        {
            try
            {
                var offices = _roleRepository.GetAll();
                return StatusCode(202, new
                {
                    Success = true,
                    Result = offices.Select(s => new
                    {
                        s.Id,
                        s.Role,
                        s.Description,
                        s.CreatedDate
                    }),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: Cat Office by Id
        [HttpGet]
        [Route("GetRole/{id}")]
        public ActionResult<ApiResponse<CatRoleDto>> GetRole(int id)
        {
            try
            {
                return Ok(new { Success = true, result = _roleRepository.GetCustom(id) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }

        }

        // Add: Cat Office
        [HttpPost]
        [Route("AddRole")]
        public ActionResult<ApiResponse<CatRoleDto>> AddRole([FromBody] CatRoleDto dto)
        {
            var response = new ApiResponse<CatRoleDto>();
            try
            {
                response.Result = _mapper.Map<CatRoleDto>(_roleRepository.Add(_mapper.Map<CatRole>(dto)));
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

        // Update: Cat Office
        [HttpPut]
        [Route("UpdateRole")]
        public ActionResult<ApiResponse<CatRoleDto>> UpdateRole([FromBody] CatRoleDto dto)
        {
            var response = new ApiResponse<CatRoleDto>();
            try
            {
                response.Result = _mapper.Map<CatRoleDto>(_roleRepository.UpdateCustom(_mapper.Map<CatRole>(dto), dto.Id));
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
        // DELETE: Cat Role by Id
        [HttpDelete]
        [Route("DeleteRole/{id}")]
        public ActionResult<ApiResponse<CatRoleDto>> DeleteRole(int id)
        {
            var response = new ApiResponse<CatRoleDto>();
            try
            {
                var record = _roleRepository.Find(x => x.Id == id);
                _roleRepository.Delete(record);
                response.Result = _mapper.Map<CatRoleDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        #endregion
        #region Life Circle
        // GET: Cat All Life Circle
        [HttpGet]
        [Route("GetAllLifeCricle")]
        public ActionResult GetAllLifeCricle()
        {
            try
            {
                var lifeCircle = _lifeCircleRepository.GetAll();
                return StatusCode(202, new
                {
                    Success = true,
                    Result = lifeCircle.Select(s => new
                    {
                        s.Id,
                        s.LifeCircle1
                    }),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: Cat Office by Id
        [HttpGet]
        [Route("GetLifeCircle/{id}")]
        public ActionResult<ApiResponse<LifeCircleDto>> GetLifeCircle(int id)
        {
            var response = new ApiResponse<LifeCircleDto>();
            try
            {
                response.Result = _mapper.Map<LifeCircleDto>(_lifeCircleRepository.Find(x => x.Id == id));
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

        // Add: Cat Life Circle
        [HttpPost]
        [Route("AddLifeCircle")]
        public ActionResult<ApiResponse<LifeCircleDto>> AddLifeCircle([FromBody] LifeCircleDto dto)
        {
            var response = new ApiResponse<LifeCircleDto>();
            try
            {
                response.Result = _mapper.Map<LifeCircleDto>(_lifeCircleRepository.Add(_mapper.Map<LifeCircle>(dto)));
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

        // Update: Life Circle
        [HttpPut]
        [Route("UpdateLifeCircle")]
        public ActionResult<ApiResponse<LifeCircleDto>> UpdateLifeCircle([FromBody] LifeCircleDto dto)
        {
            var response = new ApiResponse<LifeCircleDto>();
            try
            {
                response.Result = _mapper.Map<LifeCircleDto>(_lifeCircleRepository.Update(_mapper.Map<LifeCircle>(dto), dto.Id));
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
        // DELETE: Cat LifeCircle by Id
        [HttpDelete]
        [Route("DeleteLifeCircle/{id}")]
        public ActionResult<ApiResponse<LifeCircleDto>> DeleteLifeCircle(int id)
        {
            var response = new ApiResponse<LifeCircleDto>();
            try
            {
                var record = _lifeCircleRepository.Find(x => x.Id == id);
                _lifeCircleRepository.Delete(record);
                response.Result = _mapper.Map<LifeCircleDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        #endregion
        #region User
        public class UserAdmin
        {
            public int Id { get; set; }
            public string name { get; set; }
            public string lastName { get; set; }
            public string motherLastName { get; set; }
            public int country { get; set; }
            public int city { get; set; }
            public int office { get; set; }
            public int title { get; set; }
            public int role { get; set; }
            public string phone { get; set; }
            public string email { get; set; }
            public DateTime createdDate { get; set; }
        }

        public class UserRole
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        // GET: Users Role
        [HttpGet]
        [Route("GetRoles")]
        public ActionResult<ApiResponse<List<UserRole>>> GetRoles()
        {
            var response = new ApiResponse<List<UserRole>>();
            try
            {
                var roles = new List<UserRole>()
                {
                    new UserRole(){Id = 1,Name = "Employees"},
                    new UserRole(){Id = 2,Name = "Suppliers"},
                   // new UserRole(){Id = 3,Name = "Partners & Clients"},
                    new UserRole(){Id = 4,Name = "Assignees"},
                };
                
                response.Result = roles;
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

        // GET: Users Role
        [HttpGet]
        [Route("GetRolesNew")]
        public ActionResult<ApiResponse<List<UserRole>>> GetRolesNew()
        {
            var response = new ApiResponse<List<UserRole>>();
            try
            {
                var roles = new List<UserRole>();
              //  {
                //    new UserRole(){Id = 1,Name = "Employees"},
                  //  new UserRole(){Id = 2,Name = "Suppliers"},
                   // new UserRole(){Id = 3,Name = "Partners & Clients"},
                    //new UserRole(){Id = 4,Name = "Assignees"},
                //};

                var _roles = _roleRepository.GetAll();

                foreach(CatRole r in _roles)
                {
                    roles.Add(new UserRole() { Id = r.Id, Name = r.Role });
                }

                response.Result = roles;
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

        // GET: Cat All Users
        [HttpGet]
        [Route("GetAllUsers")]
        public ActionResult GetAllUsers([FromQuery] int?[] role)
        {
            try
            {

                var res = _userRepository.GetCustom(role);
                return StatusCode(202, new
                {
                    Success = true,
                    Result = res,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("GetAllUsersNew")]
        public ActionResult GetAllUsersNew([FromQuery] int?[] role)
        {
            try
            {

                var res = _userRepository.GetCustomNew(role);
                return StatusCode(202, new
                {
                    Success = true,
                    Result = res,
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }

        // GET: Cat User by Id
        [HttpGet]
        [Route("GetUser/{id}")]
        public ActionResult GetUser(int id)
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _userRepository.GetUserData(id),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        
        // Add: User
        [HttpPost]
        [Route("AddUser")]
        public ActionResult<ApiResponse<ProfileUserDto>> AddUser([FromBody] UserAdmin dto)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                if (_userRepository.VerifyEmail(dto.email) == "Exist")
                {
                    response.Result = null;
                    response.Success = false;
                    response.Message = "Email already Exist.";
                    return StatusCode(409, response);
                }
                ProfileUser profile = new ProfileUser();
                profile.Email = dto.email;
                profile.City = dto.city;
                profile.Country = dto.country;
                profile.Name = dto.name;
                profile.LastName = dto.lastName;
                profile.MotherLastName = dto.motherLastName;
                profile.CreatedDate = DateTime.Now;
                profile.PhoneNumber = dto.phone;
                profile.ResponsablePremierOffice = dto.office;
                profile.Title = dto.title;
                profile.User = new User();
                profile.User.Name = dto.name;
                profile.User.LastName = dto.lastName;
                profile.User.MotherLastName = dto.motherLastName;
                profile.User.RoleId = dto.role;
                profile.User.Email = dto.email;
                profile.User.UserTypeId = 2;
                profile.User.Reset = true;
                profile.User.Push = true;
                if (dto.office != 0)
                {
                    profile.Offices = new List<Office>();
                    profile.Offices.Add(new Office() { Consultant = 0, Office1 = dto.office });    
                }
                
                var guid = "Premier00$";//Guid.NewGuid().ToString().Substring(0, 7);
                profile.User.Password = _userRepository.HashPassword("$" + guid);

                response.Result = _mapper.Map<ProfileUserDto>(_profileUserRepository.AddConsultant(profile));

                //StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email.html"));
                //string body = string.Empty;
                //body = reader.ReadToEnd();
                //body = body.Replace("{user}", $"{dto.name} {dto.lastName} {dto.motherLastName}");
                //body = body.Replace("{username}", $"{profile.Email}");
                //body = body.Replace("{pass}", " $" + guid);

                //_userRepository.SendMail(profile.Email, body, "App Access");
                var url_images = _utiltyRepository.Get_url_email_images();
                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/welcome.html"));
                string body = string.Empty;
                body = reader.ReadToEnd();
                body = body.Replace("{user}", $"{dto.name} {dto.lastName} {dto.motherLastName}");
                body = body.Replace("{username}", $"{profile.Email}");
                body = body.Replace("{pass}", " $" + guid);
                body = body.Replace("{url_images}", url_images);

                _userRepository.SendMail(profile.Email, body, "App Access");
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
        
        // Update: User
        [HttpPut]
        [Route("UpdateUser")]
        public ActionResult<ApiResponse<ProfileUserDto>> UpdateUser([FromBody] UserAdmin dto)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                ProfileUser profile = _profileUserRepository.GetConsultant(dto.Id);
                profile.Email = dto.email;
                profile.City = dto.city;
                profile.Country = dto.country;

                profile.Name = dto.name;
                profile.LastName = dto.lastName;
                profile.MotherLastName = dto.motherLastName;
                profile.UpdatedDate = DateTime.Now;
                profile.PhoneNumber = dto.phone;
                profile.ResponsablePremierOffice = dto.office;
                profile.Title = dto.title;
                profile.User.Name = dto.name;
                profile.User.LastName = dto.lastName;
                profile.User.MotherLastName = dto.motherLastName;
                profile.User.RoleId = dto.role;
                profile.User.Email = dto.email;

                response.Result = _mapper.Map<ProfileUserDto>(_profileUserRepository.UpdateCustom(profile, profile.Id));

                var user = _userRepository.Find(c => c.Id == response.Result.UserId);
                user.Name = profile.Name;
                user.Email = profile.Email;
                user.MobilePhone = profile.PhoneNumber;
                _userRepository.Update(user, user.Id);
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
        
        // DELETE: User by Id
        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public ActionResult<ApiResponse<ProfileUserDto>> DeleteUser(int id)
        {
            var response = new ApiResponse<ProfileUserDto>();
            try
            {
                var record = _profileUserRepository.GetConsultant(id);
                _profileUserRepository.DeleteCustom(record);
                response.Result = _mapper.Map<ProfileUserDto>(record);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        
        // DELETE: User by Id
        [HttpDelete]
        [Route("User/{id}/Inactive")]
        public ActionResult<ApiResponse<UserDto>> User(int id)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                var user = _userRepository.Find(x => x.Id == id);
                user.Status = false;
                user.UpdatedDate = DateTime.Now;
                _userRepository.Update(user, id);
                response.Result = _mapper.Map<UserDto>(user);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        
        // Update: Activate User by Id
        [HttpPut]
        [Route("User/{id}/Active")]
        public ActionResult<ApiResponse<UserDto>> ActivateUser(int id)
        {
            var response = new ApiResponse<UserDto>();
            try
            {
                var user = _userRepository.Find(x => x.Id == id);
                user.Status = true;
                user.UpdatedDate = DateTime.Now;
                _userRepository.Update(user, id);
                response.Result = _mapper.Map<UserDto>(user);
            }
            catch (DbUpdateException ex)
            {
                var sqlException = ex.GetBaseException() as SqlException;

                if (sqlException != null)
                {
                    var number = sqlException.Number;

                    if (number == 547)
                    {
                        response.Result = null;
                        response.Success = false;
                        response.Message = "Operation Not Permited";
                        _logger.LogError("Operation Not Permited");
                        return StatusCode(409, response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(202, response);
        }
        
        // GET: Cat All Users
        [HttpGet]
        [Route("User/Inactive")]
        public ActionResult GetUsersInactive()
        {
            try
            {
                return StatusCode(202, new
                {
                    Success = true,
                    Result = _userRepository.GetUsersInactive(),
                    Message = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = 0, Message = $"Internal server error {ex.Message}" });
            }
        }
        
        #endregion
    }
}
