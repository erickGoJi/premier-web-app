using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ClientPartnerProfile;
using api.premier.Models.Training;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.RequestAdditionalTime;
using biz.premier.Repository.Training;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IThemeRepository _themeRepository;
        private readonly ITrainingRepository _trainingRepository;
        private readonly IParticipantRepository _participantRepository;
        private readonly IParticipantContentRepository _participantContentRepository;
        private readonly ICatTrainingGroupRepository _catTrainingGroupRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        public TrainingController(
            IMapper mapper,
            ILoggerManager logger,
            IUtiltyRepository utiltyRepository,
            IContentRepository contentRepository,
            IEvaluationRepository evaluationRepository,
            IThemeRepository themeRepository,
            ITrainingRepository trainingRepository,
            IParticipantRepository participantRepository,
            IParticipantContentRepository participantContentRepository,
            ICatTrainingGroupRepository catTrainingGroupRepository,
            INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository notificationSystemTypeRepository
            )
        {
            _mapper = mapper;
            _logger = logger;
            _utiltyRepository = utiltyRepository;
            _contentRepository = contentRepository;
            _evaluationRepository = evaluationRepository;
            _themeRepository = themeRepository;
            _trainingRepository = trainingRepository;
            _participantRepository = participantRepository;
            _participantContentRepository = participantContentRepository;
            _catTrainingGroupRepository = catTrainingGroupRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }
        
        // Post: Add Training
        [HttpPost]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<trainingDto>> AddTraining(trainingDto key)
        {
            var response = new ApiResponse<trainingDto>();
            try
            {
                foreach (var i in key.Contents)
                {
                    foreach (var j in i.Themes)
                    {
                        foreach (var file in j.Files)
                        {
                            file.Path = _utiltyRepository.UploadImageBase64(file.Path, "Files/Training/", file.FileExtension);
                        }
                    }
                }
                key.Photo = _utiltyRepository.UploadImageBase64(key.Photo, "Files/Training/", key.PhotoExtension);
                
                response.Result =_mapper.Map<trainingDto>(_trainingRepository.Add(_mapper.Map<Training>(key)));
                response.Message = "Training was added successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Put: Update Training
        [HttpPut]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<trainingDto>> UpdateTraining(trainingDto key)
        {
            var response = new ApiResponse<trainingDto>();
            try
            {
                foreach (var i in key.Contents)
                {
                    foreach (var j in i.Themes)
                    {
                        foreach (var file in j.Files)
                        {
                            file.Path = file.Path.Length > 250 
                                ? _utiltyRepository.UploadImageBase64(file.Path, "Files/Training/", file.FileExtension)
                                : file.Path;
                        }
                    }
                }
                key.Photo = key.Photo.Length > 250 
                    ? _utiltyRepository.UploadImageBase64(key.Photo, "Files/Training/", key.PhotoExtension)
                    : key.Photo;
                
                response.Result =_mapper.Map<trainingDto>(_trainingRepository.UpdateCustom(_mapper.Map<Training>(key), key.Id));
                response.Message = "Training was updated successfully.";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Get: Training By Id
        [HttpGet]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<trainingDto>> GetTrainingById(int key)
        {
            var response = new ApiResponse<trainingDto>();
            try
            {
                
                response.Result =_mapper.Map<trainingDto>(_trainingRepository.GetCustom(key));
                response.Message = "";
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Get: Training List
        [HttpGet("GetTrainings",Name = "GetTrainings")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult GetTrainings(int? type, DateTime? startDate, DateTime? endDate, Boolean? groups)
        {
            try
            {
                var list = _trainingRepository.GetAllIncluding(
                    x => x.TypeNavigation,
                    y => y.TrainingGroupNavigation
                    ).Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Description,
                    s.CreationDate,
                    s.TypeNavigation.Type,
                    typeId = s.Type,
                    groupId = s.TrainingGroup.Value,
                    GroupName = s.TrainingGroupNavigation.Name,
                    s.Groups
                }).ToList();
                if (type.HasValue)
                    list = list.Where(x => x.typeId == type.Value).ToList();
                if (startDate.HasValue && endDate.HasValue)
                    list = list.Where(x => x.CreationDate.Value >= startDate.Value && x.CreationDate.Value <= endDate.Value).ToList();
                if (groups.HasValue)
                    list = list.Where(x => x.Groups == groups.Value).ToList();
                return StatusCode(201, new { Result = list, Success = true, Message = ""});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Result = 0, Success = false, Message = $"Internal server error {ex.Message}"});
            }

            
        }
        
        // Get: MyTrainings
        [HttpGet("{user}/MyTrainings",Name = "{user}/MyTrainings")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult MyTrainings(int user)
        {
            try
            {
                return StatusCode(201, new { Result = _participantRepository.GetMyTrainings(user), Success = true, Message = ""});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Result = 0, Success = false, Message = $"Internal server error {ex.Message}"});
            }
        }
        
        // Get: Training Detail
        [HttpGet("{participant}/MyTraining",Name = "{user}/MyTraining")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult MyTrainingDetail(int participant)
        {
            try
            {
                return StatusCode(201, new { Result = _participantRepository.GetTraining(participant), Success = true, Message = ""});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Result = 0, Success = false, Message = $"Internal server error {ex.Message}"});
            }
        }

        // Delete: Remove Training
        [HttpDelete]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<trainingDto>> DeleteTraining(int key)
        {
            var response = new ApiResponse<trainingDto>();
            try
            {
                var training = _trainingRepository.Find(x => x.Id == key);
                _trainingRepository.Delete(training);
                response.Result = _mapper.Map<trainingDto>(training);
                response.Message = "Training was deleted successfully.";
                response.Success = true;
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
                        response.Message = "Operation Not Permitted";
                        _logger.LogError("Operation Not Permitted");
                        return StatusCode(490, response);
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
            
            return StatusCode(201, response);
        }
        
        // Post: Add Participant
        [HttpPost("AddParticipants")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<List<ParticipantDto>>>> AddParticipants(int[] participants, int? training, int? groups, int createdBy)
        {
            var response = new ApiResponse<List<ParticipantDto>>();
            try
            {
                response.Result = null;
                response.Message = "Participants were added successfully.";
                response.Success = _trainingRepository.AddParticipants(participants, training, groups, createdBy);

                foreach (var participant in participants)
                {
                    _notificationSystemRepository.Add(new NotificationSystem()
                    {
                        Id = 0,
                        Archive = false,
                        View = false,
                        Time = DateTime.Now.TimeOfDay,
                        UserFrom = createdBy,
                        UserTo = participant,
                        NotificationType = 24,
                        Description = "You have a new training.",
                        Color = "#f06689",
                        CreatedDate = DateTime.Now
                    });
                    _notificationSystemRepository.Save();
                    await _notificationSystemRepository.SendNotificationAsync(
                        participant,
                        createdBy,
                        0,
                        (await _notificationSystemTypeRepository.FindAsync(x => x.Id == 24)).Type,
                        $"You have a new training.",
                        2
                    );    
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

            return StatusCode(201, response);
        }
        
        // Post: Complete 
        [HttpPost("{trainingId}/CompleteContent")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult<ApiResponse<ParticipantContentDto>> CompleteContent([FromBody] ParticipantContentDto contentDto, int trainingId)
        {
            var response = new ApiResponse<ParticipantContentDto>();
            try
            {
                var training = _trainingRepository.GetAllIncluding(x => x.Contents).FirstOrDefault(x => x.Id == trainingId);
                var participant = _participantRepository
                    .GetAllIncluding(x => x.ParticipantContents)
                    .FirstOrDefault(x => x.Id == contentDto.Participant);
                participant.Percentage = ((participant.ParticipantContents.Count() + 1) * 100) / training.Contents.Count();
                participant.Status = participant.Status == 1 ? 2 : participant.Percentage.Value == (decimal) 100.00 ? 3 : participant.Status;
                participant.CompletedDate = participant.Status == 3 ? DateTime.Now : (DateTime?) null;
                participant.UpdatedDate = DateTime.Now;
                int corrects = 0;
                int totalQuestions = 0;
                foreach (var evaluation in contentDto.ParticipantEvaluations)
                {
                    var answer = _evaluationRepository
                        .GetAllIncluding(x => x.Answers)
                        .Where(x => x.ContentNavigation.Training == trainingId);
                    totalQuestions = answer.Count();
                    evaluation.Correct = answer.Any(x =>
                        x.Id == evaluation.Question && x.Answers.Any(q => q.Id == evaluation.Answer && q.Correct))
                        ? true
                        : false;
                    corrects = evaluation.Correct.Value ? corrects + 1 : corrects;
                }

                contentDto.Score = contentDto.ParticipantEvaluations.Any() ? ((corrects) * 100) / totalQuestions : 0;
                response.Result =
                    _mapper.Map<ParticipantContentDto>(
                        _participantContentRepository.Add(_mapper.Map<ParticipantContent>(contentDto)));
                response.Message = "Participants Content were added successfully.";
                response.Success = true;
                participant.Score = contentDto.Score.HasValue ? contentDto.Score.Value : 0;
                _participantRepository.Update(participant, participant.Id);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = $"Internal server error {ex.Message}";
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, response);
            }

            return StatusCode(201, response);
        }
        
        // Get: Participant List
        [HttpGet("ParticipantList")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public ActionResult ParticipantList()
        {
            try
            {
                
                return StatusCode(201, new { Result = _participantRepository.GetParticipantList() , Success = false, Message = "" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new {Result = 0, Success = false, Message = $"Internal server error {ex.Message}" });
            }
        }
        
        // POST: Group
        [HttpPost]
        [Route("AddGroup")]
        public ActionResult<ApiResponse<CatTrainingGroupDto>> GetTrainingGroup(CatTrainingGroupDto groupDto)
        {
            var response = new ApiResponse<CatTrainingGroupDto>();

            try
            {
                response.Result = _mapper.Map<CatTrainingGroupDto>(_catTrainingGroupRepository.Add(_mapper.Map<CatTrainingGroup>(groupDto)));
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
        
    }
}