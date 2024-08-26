using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.RequestInformation;
using api.premier.Models.RequestInformationDocuments;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.Appointment;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.RequestInformation;
using biz.premier.Repository.ServiceOrder;
using biz.premier.Repository.ServiceRecord;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestInformationController : ControllerBase
    {
        private readonly IRequestInformationRepository _requestInformationRepository;
        private readonly IServiceRecordRepository _serviceRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _catNotificationSystemTypeRepository;
        public RequestInformationController(IRequestInformationRepository requestInformationRepository, IMapper mapper, ILoggerManager loggerManager,
            IServiceRecordRepository serviceRecordRepository, IUtiltyRepository utiltyRepository, INotificationSystemRepository notificationSystemRepository,
            ICatNotificationSystemTypeRepository catNotificationSystemTypeRepository, IAppointmentRepository appointmentRepository)
        {
            _requestInformationRepository = requestInformationRepository;
            _serviceRecordRepository = serviceRecordRepository;
            _logger = loggerManager;
            _mapper = mapper;
            _utiltyRepository = utiltyRepository;
            _catNotificationSystemTypeRepository = catNotificationSystemTypeRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _appointmentRepository = appointmentRepository;
        }
        [HttpPost("UploadFile", Name = "UploadFile")]
        public async Task<IActionResult> PostRequestInfo(List<IFormFile> file)
        {
            var result = new ApiResponse<string>();
            try
            {
                string ruta;
                long size = file.Sum(f => f.Length);
                var filePath = Environment.CurrentDirectory;
                var extencion = file[0].FileName.Split(".");
                var _guid = Guid.NewGuid();
                var path = "/Files/" + _guid + "." + extencion[1];
                foreach (var formFile in file)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(filePath + path, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                ruta = "http://localhost/api/Files/" + _guid + "." + extencion[1];
                result.Result = "Sucess";
                result.Success = true;
                result.Message = ruta;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                result.Result = "Error";
                result.Success = false;
                result.Message = ex.ToString();
            }
            return new ObjectResult(result);
        }

        [HttpPost("PostRequestInformation", Name = "PostRequestInformation")]
        public ActionResult<ApiResponse<RequestInformationDto>> PostRequestInformation([FromBody] RequestInformationDto dto)
        {
            var response = new ApiResponse<RequestInformationDto>();
            try
            {
                var service = _serviceRecordRepository.GetAllIncluding(i => i.AssigneeInformations).FirstOrDefault(c => c.Id == dto.ServiceRecordId);
                if (service != null)
                {

                    foreach (var i in dto.RequestInformationDocuments)
                    {
                        i.FileRequest = _utiltyRepository.UploadImageBase64(i.FileRequest, "Files/RequestInformation/", i.FileExtension);
                    }

                    RequestInformation order = _requestInformationRepository.insert(_mapper.Map<RequestInformation>(dto));
                    response.Result = _mapper.Map<RequestInformationDto>(order);

                    if (dto.SentsTo.Any())
                    {
                        if (dto.SentTo.Length > 0)
                        {
                            dto.SentsTo.Add(new string(dto.SentTo));
                        }
                    }
                    else
                    {
                        dto.SentsTo = new List<string> { new string(dto.SentTo) };
                    }

                    var services = _appointmentRepository.GetServicesByServiceRecord(service.Id);
                    //int _service = 0;
                    string _serviceName = "";
                    switch (dto.House)
                    {
                        case 1:
                            _serviceName = "Pre Decision";
                            break;
                        case 2:
                            _serviceName = "Home Finding";
                            break;
                        case 3:
                            _serviceName = "Area Orientation";
                            break;
                        default:
                            // code block
                            break;
                    }
                    foreach (var sentTo in dto.SentsTo)
                    {
                        var url_images = _utiltyRepository.Get_url_email_images();
                        //StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/EmailRequestInformation.html"));
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/request.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{paragraph}", $"Service Record : { service.NumberServiceRecord }, " +
                                                           $"Services : " +
                                                           $"{ _serviceName } <br/> https://my.premierds.com/ds/assigneeAndFamilyInfo/{ order.Id }");
                        body = body.Replace("{dueDate}", $"Due Date : {dto.Due}");


                        body = body.Replace("{ur_request}", $"{ _serviceName } https://my.premierds.com/ds/assigneeAndFamilyInfo/{ order.Id }");
                      
                        body = body.Replace("{url_images}", url_images);

                        List<RequestInformationDocumentDto> documentDtos = new List<RequestInformationDocumentDto>();

                        foreach (var i in dto.RequestInformationDocuments)
                        {
                            documentDtos.Add(i);
                        }

                        _requestInformationRepository.SendMail(sentTo, body, " Information Request :" + service.NumberServiceRecord + "/" + _serviceName,
                            _mapper.Map<List<RequestInformationDocument>>(documentDtos));
                    }

                    //_notificationSystemRepository.Add(new NotificationSystem()
                    //{
                    //    Id = 0,
                    //    Archive = false,
                    //    View = false,
                    //    ServiceRecord = service.Id,
                    //    Time = DateTime.Now.TimeOfDay,
                    //    UserFrom = dto.AuthorizedBy,
                    //    UserTo = service.AssigneeInformations.FirstOrDefault().UserId,
                    //    NotificationType = 23,
                    //    Description = $"{service.NumberServiceRecord} request Information",
                    //    Color = "#f06689",
                    //    CreatedDate = DateTime.Now
                    //});
                    //_notificationSystemRepository.Save();
                    //_notificationSystemRepository.SendNotificationAsync(
                    //    service.AssigneeInformations.FirstOrDefault().UserId.Value,
                    //    0,
                    //    0,
                    //    _catNotificationSystemTypeRepository.Find(x => x.Id == 23).Type,
                    //    $"{service.NumberServiceRecord} was modified.",
                    //    2
                    //);

                }
                else
                {
                    response.Success = false;
                    response.Message = "Service Record Not Found";
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

        [HttpGet("GetRequestInformation", Name = "GetRequestInformation")]
        public ActionResult<ApiResponse<RequestInformationSelectDto>> GetRequestInformation([FromQuery] int id)
        {
            var response = new ApiResponse<RequestInformationSelectDto>();
            try
            {
                var request = _mapper.Map<RequestInformationSelectDto>(_requestInformationRepository.Find(c => c.Id == id));
                if (request != null)
                {
                    var dateLimit = request.Due.Value;
                    dateLimit = dateLimit.AddDays(3);
                    if (dateLimit == DateTime.Now)
                    {
                        response.Success = false;
                        response.Message = $"Request information has been expired {dateLimit.ToString(" dd/MM/yyyy")}";
                    }
                    else
                    {
                        List<Tuple<int, string>> tuples = _requestInformationRepository.HousingAvailible(request.ServiceRecordId.Value);
                        tuples = tuples.Where(x => x.Item1 == request.House).ToList();
                        request.HousingAvailible = new List<TypeHousingAvailible>();
                        foreach (var i in tuples)
                        {
                            TypeHousingAvailible availible = new TypeHousingAvailible();
                            availible.availible = true;
                            availible.Id = i.Item1;
                            availible.TypeHousing = i.Item2;
                            availible.WorkOrderServices =
                                _requestInformationRepository.GetWorkOrderService(request.ServiceRecordId.Value,
                                    i.Item1);
                            request.HousingAvailible.Add(availible);
                        }
                        response.Result = _mapper.Map<RequestInformationSelectDto>(request);
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Request information Not Found";
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
