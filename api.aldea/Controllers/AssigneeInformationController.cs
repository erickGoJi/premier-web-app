using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.AssigneeInformation;
using api.premier.Models.DependentInformations;
using api.premier.Models.Pet;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.AssigneeName;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssigneeInformationController : ControllerBase
    {
        private readonly IAssigneeNameRepository _assigneeNameRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IUserRepository _userRepository;
        public AssigneeInformationController(IAssigneeNameRepository assigneeNameRepository, IMapper mapper, ILoggerManager logger, 
            IUtiltyRepository utiltyRepository, IUserRepository userRepository)
        {
            _assigneeNameRepository = assigneeNameRepository;
            _logger = logger;
            _mapper = mapper;
            _utiltyRepository = utiltyRepository;
            _userRepository = userRepository;
        }

        // GET: Get AssigneeInformation
        [HttpGet]
        [Route("GetAssigneeInformation")]
        public ActionResult<ApiResponse<AssigneeInformationInsertDto>> GetAssigneeInformation([FromQuery] int id)
        {
            var response = new ApiResponse<AssigneeInformationInsertDto>();
            try
            {
                var mao = _assigneeNameRepository.Find(f => f.ServiceRecordId == id);
                mao.DependentInformations = mao.DependentInformations.Where(d => d.RelationshipId != 7).ToList();
                response.Result = _mapper.Map<AssigneeInformationInsertDto>(mao);

               // response.Result = _mapper.Map<AssigneeInformationInsertDto>(_assigneeNameRepository.Find(f => f.ServiceRecordId == id));
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



        [HttpGet("GetAssigneeInfoByWOSId", Name = "GetAssigneeInfoByWOSId")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AssigneeInformationInsertDto>> GetAssigneeInfoByWOSId([FromQuery] int wos_id)
        {
            var response = new ApiResponse<AssigneeInformationInsertDto>();
            try
            {
                var assa_info = _assigneeNameRepository.GetAssigneeInfoByWOSId(wos_id);
                response.Result = _mapper.Map<AssigneeInformationInsertDto>(assa_info);
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


        [HttpPost("EditAssigneeInformartionUser", Name = "EditAssigneeInformartionUser")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AssigneeInformationUserEditDto>> EditAssigneeInformartionUser([FromBody] AssigneeInformationUserEditDto dto)
        {
            var response = new ApiResponse<AssigneeInformationUserEditDto>();
            try
            {
                var assigneeInformation = _assigneeNameRepository.Find(c => c.Id == dto.Id);
                assigneeInformation.Email = dto.Email;
                assigneeInformation.MobilePhone = dto.MobilePhone;
                assigneeInformation.AssigneeName = dto.AssigneeName;

                if (assigneeInformation != null)
                {
                    var user = _userRepository.Find(c => c.Id == assigneeInformation.UserId);
                    user.Name = dto.AssigneeName;
                    user.Email = dto.Email;
                    _userRepository.Update(user, user.Id);

                    AssigneeInformation info = _assigneeNameRepository.Update(assigneeInformation, dto.Id);
                    response.Result = _mapper.Map<AssigneeInformationUserEditDto>(info);
                    response.Success = true;
                    response.Message = "Assignee User Updated";


                }
                else
                {
                    response.Success = false;
                    response.Message = "Assignee Information Not Found";
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

        [HttpPost("CreateAssigneeInformartion", Name = "CreateAssigneeInformartion")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AssigneeInformationInsertDto>> CreateAssigneeInformartion([FromBody] AssigneeInformationInsertDto dto)
        {
            var response = new ApiResponse<AssigneeInformationInsertDto>();
            try
            {
                var assigneeInformation = _assigneeNameRepository.Find(c => c.Id == dto.Id);
                if (assigneeInformation != null)
                {
                    if (dto.Photo != null && dto.Photo.Length > 150)
                    {
                        dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/Assignee/", "png");
                    }
                    foreach (var i in dto.PetsNavigation)
                    {
                        if(i.Photo != null && i.Photo.Length > 150)
                        {
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/Pets/", "png");
                        }
                    }
                    foreach (var i in dto.DependentInformations)
                    {
                        if (i.Photo != null && i.Photo.Length > 150)
                        {
                            i.Photo = _utiltyRepository.UploadImageBase64(i.Photo, "Files/Dependent/", "png");
                        }
                    }
                    AssigneeInformation info = _assigneeNameRepository.updateCustom(_mapper.Map<AssigneeInformation>(dto));
                    response.Result = _mapper.Map<AssigneeInformationInsertDto>(info); 
                }
                else
                {
                    response.Success = false;
                    response.Message = "Assignee Information Not Found";
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

        // GET: Get AssigneeInformation
        [HttpGet]
        [Route("GetAssigneeInformationApp")]
        public ActionResult GetAssigneeInformationApp([FromQuery] int id)
        {
            var response = new ApiResponse<AssigneeInformation>();
            try
            {
                var mao = _assigneeNameRepository.GetAssigneeInfoBySRId(id);
                response.Result = mao;// _mapper.Map<AssigneeInformationInsertDto>(mao);
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


        // GET: Get AssigneeInformation
        [HttpGet]
        [Route("GetDependentInformationId")]
        public ActionResult GetDependentInformationId([FromQuery] int id)
        {
            var response = new ApiResponse<DependentInformation>();
            try
            {
                var mao = _assigneeNameRepository.GetDependentInformationId(id);
                response.Result = mao;// _mapper.Map<AssigneeInformationInsertDto>(mao);
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


        [HttpPost("UpdateDependentInformation", Name = "UpdateDependentInformation")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DependentInformationDto>> UpdateDependentInformation([FromBody] DependentInformationDto dto)
        {
            var response = new ApiResponse<DependentInformationDto>();
            try
            {
                if (dto.Id > 0)
                {
                    var DependentInformations = _assigneeNameRepository.GetDependentInformationId(dto.Id);
                    if (DependentInformations != null)
                    {

                        if (dto.Photo != null && dto.Photo.Length > 150)
                        {
                            dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/Dependent/", "png");
                        }

                        DependentInformation info = _assigneeNameRepository.updateDepCustom(_mapper.Map<DependentInformation>(dto));
                        response.Result = _mapper.Map<DependentInformationDto>(info);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Dependent Information Information Not Found";
                    }
                }
                else
                {
                    var DependentInformations = dto;
                    if (DependentInformations != null)
                    {

                        if (dto.Photo != null && dto.Photo.Length > 150)
                        {
                            dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/Dependent/", dto.PhotoExtension);
                        }

                        DependentInformation info = _assigneeNameRepository.AddDepCustom(_mapper.Map<DependentInformation>(dto));
                        response.Result = _mapper.Map<DependentInformationDto>(info);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Dependent Information Information Not Found";
                    }
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


        // GET: Get AssigneeInformation
        [HttpGet]
        [Route("DeleteDependent")]
        public ActionResult DeleteDependent([FromQuery] int id)
        {
            var response = new ApiResponse<int>();
            try
            {
                var mao = _assigneeNameRepository.DeleteDependent(id);
                response.Result = mao;// _mapper.Map<AssigneeInformationInsertDto>(mao);
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        // GET: Get GetPetId
        [HttpGet]
        [Route("GetPetId")]
        public ActionResult GetPetId([FromQuery] int id)
        {
            var response = new ApiResponse<Pet>();
            try
            {
                var mao = _assigneeNameRepository.GetPetId(id);
                response.Result = mao;// _mapper.Map<AssigneeInformationInsertDto>(mao);
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


        [HttpPost("UpdatePet", Name = "UpdatePet")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<DependentInformationDto>> UpdatePet([FromBody] PetDto dto)
        {
            var response = new ApiResponse<PetDto>();
            try
            {
                if (dto.Id > 0)
                {
                    var Pet = _assigneeNameRepository.GetPetId(dto.Id);
                    if (Pet != null)
                    {
                        if (dto.Photo != null && dto.Photo.Length > 150)
                        {
                            dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/Dependent/", "png");
                        }

                        Pet info = _assigneeNameRepository.updatePetCustom(_mapper.Map<Pet>(dto));
                        response.Result = _mapper.Map<PetDto>(info);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Dependent Information Information Not Found";
                    }
                }
                else
                {
                    var Pet = dto;
                    if (Pet != null)
                    {

                        if (dto.Photo != null && dto.Photo.Length > 150)
                        {
                            dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/Dependent/", "png");
                        }

                        Pet info = _assigneeNameRepository.AddPetCustom(_mapper.Map<Pet>(dto));
                        response.Result = _mapper.Map<PetDto>(info);
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Pet Information Information Not Found";
                    }
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

        // GET: Get GetPetId
        [HttpGet]
        [Route("DeletePet")]
        public ActionResult DeletePet([FromQuery] int id)
        {
            var response = new ApiResponse<int>();
            try
            {
                var mao = _assigneeNameRepository.DeletePet(id);
                response.Result = mao;// _mapper.Map<AssigneeInformationInsertDto>(mao);
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }
            return Ok(response);
        }

        public class onboarding
        {
            public int MobilePhoneCodeId { get; set; }
            public string? MobilePhone { get; set; }
            public DateTime? InitialArrival { get; set; }
            public string? Photo { get; set; }
            public int Id { get; set; }
            public DateTime? FinalMove { get; set; }

            public bool? ShowOnboard { get; set; }

        } 

        [HttpPost("UpdateOnboarding", Name = "UpdateOnboarding")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<AssigneeInformationUserEditDto>> UpdateOnboarding([FromBody] onboarding dto)
        {
            
            var response = new ApiResponse<AssigneeInformationUserEditDto>();
            try
            {
                if (dto.Id > 0)
                {

                    var assigneeInformation = _assigneeNameRepository.Find(c => c.Id == dto.Id);

                    if (assigneeInformation != null)
                    {

                        if (dto.Photo != null && dto.Photo.Length > 150)
                        {
                            dto.Photo = _utiltyRepository.UploadImageBase64(dto.Photo, "Files/Assignee/", "png");
                            assigneeInformation.Photo = dto.Photo;

                            var user = _userRepository.Find(c => c.Id == assigneeInformation.UserId);
                            user.Avatar = dto.Photo;
                            _userRepository.Update(user, user.Id);

                        }

                        if (dto.MobilePhone != null && dto.MobilePhone.Length > 5 ) //photo 
                        {
                            assigneeInformation.MobilePhone = dto.MobilePhone;
                        }

                        if (dto.InitialArrival != null ) 
                        {
                            assigneeInformation.InitialArrival = dto.InitialArrival;

                        }

                        if(dto.FinalMove != null){
                            assigneeInformation.FinalMove = dto.FinalMove;
                        }

                        if(dto.ShowOnboard != null)
                        {
                            assigneeInformation.ShowOnboard = dto.ShowOnboard;
                        }

                        if (dto.MobilePhoneCodeId != 0)
                        {
                            assigneeInformation.MobilePhoneCodeId = dto.MobilePhoneCodeId;
                        }

                        AssigneeInformation info = _assigneeNameRepository.Update(assigneeInformation, dto.Id);
                        response.Result = _mapper.Map<AssigneeInformationUserEditDto>(info);
                        response.Success = true;
                        response.Message = "Assignee User Updated";
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Dependent Information Information Not Found";
                    }
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
