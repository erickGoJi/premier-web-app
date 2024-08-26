using biz.premier.Repository.NotificationSystem;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using biz.premier.Models;
using biz.premier.Models.PushNotification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace dal.premier.Repository.NotificationSystem
{
    public class NotificationSystemRepository : GenericRepository<biz.premier.Entities.NotificationSystem>, INotificationSystemRepository
    {
        public NotificationSystemRepository(Db_PremierContext context) : base(context) { }
        private static Uri _fireBasePushNotificationsUrl = new Uri("https://fcm.googleapis.com/fcm/send");
        private static string _serverKey = "AAAAYrj9ams:APA91bEoD8HOUBJG97Uk0u6A5YYqcgpBOjJYuw9w53ehzcXNRGETsqT7-yNthRO20B6ZIG5uZUIv5ItL9_mOVWKJN0wvZfYyMZndHB4S3qti1AEirxrPzfD4WQTKudIGn8SNuGfx2w-V";
        private INotificationSystemRepository _notificationSystemRepositoryImplementation;

        public ActionResult GetAllCustom(int user, DateTime? dateRange1, DateTime? dateRange2, int? notificationType, int? serviceRecord, bool? archive)
        {
            var profile = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user);
            var notifications = _context.NotificationSystems.Where(x => x.UserTo == user && x.View == false).Select(s => new
            {
                s.Id,
                s.NotificationTypeNavigation.Type,
                serviceRecord = s.ServiceRecord.HasValue ? s.ServiceRecordNavigation.NumberServiceRecord : "",
                fromId = s.UserFrom,
                fromProfile = s.UserFromNavigation.ProfileUsers.FirstOrDefault().Id,
                toId = s.UserTo,
                roleFromId = s.UserFromNavigation.RoleId,
                roleToId = s.UserToNavigation.RoleId,
                from = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Name + " " + s.UserFromNavigation.LastName + " " + s.UserFromNavigation.MotherLastName
                    : "N/A",
                fromAvatar = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Avatar
                    : "N/A",
                s.Description,
                s.Time,
                s.CreatedDate,
                s.Color,
                s.NotificationType,
                serviceRecordId = s.ServiceRecord,
                clientName = s.ServiceRecordNavigation.Client.Name,
                s.ServiceRecordNavigation.Vip,
                s.ServiceRecordNavigation.Urgent,
                s.ServiceRecordNavigation.ConfidentialMove,
                assignee = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().AssigneeName,
                serviceRecordCountry = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCountry.Name,
                serviceRecordCountryHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCountryNavigation.Name,
                serviceRecordCityHome = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCity.Name,
                serviceRecordCityHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCity.City,
                s.Archive,
                s.View,
                actionCall = new
                { 
                    accept = s.NotificationType == 1 && s.ServiceRecordNavigation.ImmigrationCoodinators.Any(a=>a.CoordinatorId == (profile == null ? 0 : profile.Id)) 
                            ? $"api/ServiceRecord/AcceptImmigrationCoordinator/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.ImmigrationCoodinators.FirstOrDefault().Id).FirstOrDefault()}/true"
                            :  s.NotificationType == 1 && s.ServiceRecordNavigation.RelocationCoordinators.Any(a=>a.CoordinatorId == (profile == null ? 0 : profile.Id))
                            ? $"api/ServiceRecord/AcceptRelocationCoordinator/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.RelocationCoordinators.FirstOrDefault().Id).FirstOrDefault()}/true"
                            //: s.NotificationType == 2 && s.ServiceRecordNavigation.ImmigrationSupplierPartners.Any(a=>a.SupplierId == profile.Id)
                            //? $"api/ServiceRecord/AcceptImmigrationSupplierPartner/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.ImmigrationSupplierPartners.FirstOrDefault().Id).FirstOrDefault()}/true"
                            //: s.NotificationType == 2 && s.ServiceRecordNavigation.RelocationSupplierPartners.Any(a=>a.SupplierId == profile.Id)
                            //? $"api/ServiceRecord/AcceptRelocationSupplierPartner/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.RelocationSupplierPartners.FirstOrDefault().Id).FirstOrDefault()}/true"
                            : null,
                    rejected = s.NotificationType == 1 && s.ServiceRecordNavigation.ImmigrationCoodinators.Any(a=>a.CoordinatorId == (profile == null ? 0 : profile.Id))
                            ? $"api/ServiceRecord/AcceptImmigrationCoordinator/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.ImmigrationCoodinators.FirstOrDefault().Id).FirstOrDefault()}/false"
                            : s.NotificationType == 1 && s.ServiceRecordNavigation.RelocationCoordinators.Any(a=>a.CoordinatorId == (profile == null ? 0 : profile.Id))
                            ? $"api/ServiceRecord/AcceptRelocationCoordinator/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.RelocationCoordinators.FirstOrDefault().Id).FirstOrDefault()}/false"
                            //: s.NotificationType == 2 && s.ServiceRecordNavigation.ImmigrationSupplierPartners.Any(a=>a.SupplierId == profile.Id)
                            //? $"api/ServiceRecord/AcceptImmigrationSupplierPartner/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.ImmigrationSupplierPartners.FirstOrDefault().Id).FirstOrDefault()}/false"
                            //: s.NotificationType == 2 && s.ServiceRecordNavigation.RelocationSupplierPartners.Any(a=>a.SupplierId == profile.Id)
                            //? $"api/ServiceRecord/AcceptRelocationSupplierPartner/{_context.ServiceRecords.Where(x => x.Id == s.ServiceRecord).Select(a => a.RelocationSupplierPartners.FirstOrDefault().Id).FirstOrDefault()}/false"
                            : null,
                }
            }).OrderByDescending(o => o.CreatedDate).ThenByDescending(o => o.Time).ToList();

            //Query por Role
            //int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;
            //switch (role)
            //{
            //    case 2: // Coordinator
            //        notifications = notifications.Where(x => x.toId == user || x.fromId == user).ToList();
            //        break;
            //    case 3: // Supplier
            //        notifications = notifications.Where(x => x.fromId == user).ToList();
            //        break;
            //    default:
            //        Console.WriteLine("Default case");
            //        break;
            //}

            if (dateRange1.HasValue && dateRange2.HasValue)
                notifications = notifications.Where(x => x.CreatedDate > dateRange1.Value && x.CreatedDate < dateRange2.Value).ToList();
            if (notificationType.HasValue)
                notifications = notifications.Where(x => x.NotificationType == notificationType.Value).ToList();
            if (serviceRecord.HasValue)
                notifications = notifications.Where(x => x.serviceRecordId == serviceRecord.Value).ToList();
            if (archive.HasValue)
                notifications = notifications.Where(x => x.Archive == archive.Value).ToList();
            else
                notifications = notifications.Where(x => x.View == false).ToList();
            return new ObjectResult(notifications);
        }

        public ActionResult GetAllCustomUnRead(int user, DateTime? dateRange1, DateTime? dateRange2, int? notificationType, int? serviceRecord, bool? archive)
        {
            var notifications = _context.NotificationSystems.Where(x => x.UserTo == user && x.View == true).Select(s => new
            {
                s.Id,
                s.NotificationTypeNavigation.Type,
                serviceRecord = s.ServiceRecord.HasValue ? s.ServiceRecordNavigation.NumberServiceRecord : "",
                fromId = s.UserFrom,
                fromProfile = s.UserFromNavigation.ProfileUsers.FirstOrDefault().Id,
                toId = s.UserTo,
                roleFromId = s.UserFromNavigation.RoleId,
                roleToId = s.UserToNavigation.RoleId,
                from = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Name + " " + s.UserFromNavigation.LastName + " " + s.UserFromNavigation.MotherLastName
                    : "N/A",
                fromAvatar = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Avatar
                    : "N/A",
                s.Description,
                s.Time,
                s.CreatedDate,
                s.Color,
                s.NotificationType,
                serviceRecordId = s.ServiceRecord,
                clientName = s.ServiceRecordNavigation.Client.Name,
                s.ServiceRecordNavigation.Vip,
                s.ServiceRecordNavigation.Urgent,
                s.ServiceRecordNavigation.ConfidentialMove,
                assignee = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().AssigneeName,
                serviceRecordCountry = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCountry.Name,
                serviceRecordCountryHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCountryNavigation.Name,
                serviceRecordCityHome = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCity.Name,
                serviceRecordCityHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCity.City,
                s.Archive,
                s.View
                
            }).OrderByDescending(o => o.CreatedDate).ThenByDescending(o => o.Time).Take(10).ToList();

            return new ObjectResult(notifications);
        }

        public ActionResult GetAllCustomApp(int user)
        {
            var profile = _context.ProfileUsers.FirstOrDefault(x => x.UserId == user);
            var notifications = _context.NotificationSystems.Where(x => x.UserTo == user && x.Archive == true).Select(s => new
            {
                s.Id,
                s.NotificationTypeNavigation.Type,
                serviceRecord = s.ServiceRecord.HasValue ? s.ServiceRecordNavigation.NumberServiceRecord : "",
                fromId = s.UserFrom,
                fromProfile = s.UserFromNavigation.ProfileUsers.FirstOrDefault().Id,
                toId = s.UserTo,
                roleFromId = s.UserFromNavigation.RoleId,
                roleToId = s.UserToNavigation.RoleId,
                from = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Name + " " + s.UserFromNavigation.LastName + " " + s.UserFromNavigation.MotherLastName
                    : "N/A",
                fromAvatar = s.UserFrom.HasValue
                    ? s.UserFromNavigation.Avatar
                    : "N/A",
                s.Description,
                s.Time,
                s.CreatedDate,
                s.Color,
                s.NotificationType,
                serviceRecordId = s.ServiceRecord,
                clientName = s.ServiceRecordNavigation.Client.Name,
                s.ServiceRecordNavigation.Vip,
                s.ServiceRecordNavigation.Urgent,
                s.ServiceRecordNavigation.ConfidentialMove,
                assignee = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().AssigneeName,
                serviceRecordCountry = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCountry.Name,
                serviceRecordCountryHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCountryNavigation.Name,
                serviceRecordCityHome = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HomeCity.Name,
                serviceRecordCityHost = s.ServiceRecordNavigation.AssigneeInformations.SingleOrDefault().HostCity.City,
                s.Archive,
                s.View
            }).OrderByDescending(o => o.CreatedDate).ThenByDescending(o => o.Time).Take(50).ToList();

            return new ObjectResult(notifications);
        }


        public ActionResult GetNotifications(int user)
        {
            if (user == 0)
                return null;
            var notification = _context.NotificationSystems.Where(x => x.UserTo == user)
                .Select(s => new
                {
                    s.Id,
                    s.NotificationTypeNavigation.Type,
                    serviceRecord = s.ServiceRecord.HasValue ? s.ServiceRecordNavigation.NumberServiceRecord : "",
                    s.Description,
                    s.Time,
                    s.CreatedDate,
                    s.Color,
                    s.NotificationType,
                    serviceRecordId = s.ServiceRecord,
                    s.Archive,
                    s.View
                }).Where(x => x.View == false).ToList();
            return new ObjectResult(notification);
        }

        public ActionResult DeleteNotificationById(int id)
        {
            if (id == 0)
                return null;
            var notification = _context.NotificationSystems.SingleOrDefault(x => x.Id == id);

            _context.Remove(notification);
            _context.SaveChanges();
            return new ObjectResult(notification);
        }

        public int GetUserId(int key)
        {
            int userId = _context.ProfileUsers.SingleOrDefault(x => x.Id == key).UserId.Value;
            return userId;
        }

        public int GetUserAssignee(int key)
        {
            int userId = _context.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == key).UserId.Value;
            return userId;
        }

        public Tuple<int, int> GetCoordinator(int workOrder)
        {
            var work = _context.WorkOrders.SingleOrDefault(x => x.Id == workOrder);
            if(work == null)
            {
                return new Tuple<int, int>(0, 0);
            }
            var coordinator = _context.ServiceRecords.Where(x => x.Id == work.ServiceRecordId).Select(s => new
            {
                coordinator = work.ServiceLineId.Value == 1
                    ? s.ImmigrationCoodinators.FirstOrDefault().Coordinator.UserId.Value
                    : s.RelocationCoordinators.FirstOrDefault().Coordinator.UserId.Value,
                serviceRecord = s.Id
            }).FirstOrDefault();
            return new Tuple<int, int>(coordinator.serviceRecord, coordinator.coordinator);
        }

        public Tuple<int, int> GetCoordinatorbyWosId(int WosId)
        {
            var sa = _context.StandaloneServiceWorkOrders.Where(s => s.WorkOrderServiceId == WosId).ToList();
            var woId = 0;
            if(sa.Count() > 0)
            {
                woId = sa[0].WorkOrderId.Value;
            }
            else
            {
                var b = _context.BundledServices.Where(s => s.WorkServicesId == WosId).ToList();
                if (b.Count() > 0)
                {
                    var bs = _context.BundledServicesWorkOrders.Where(s => s.Id == b[0].BundledServiceOrderId).ToList();
                    woId = bs[0].WorkOrderId.Value;
                }
            }

            var work = _context.WorkOrders.SingleOrDefault(x => x.Id == woId);
            var coordinator = _context.ServiceRecords.Where(x => x.Id == work.ServiceRecordId).Select(s => new
            {
                coordinator = work.ServiceLineId.Value == 1
                    ? s.ImmigrationCoodinators.FirstOrDefault().Coordinator.UserId.Value
                    : s.RelocationCoordinators.FirstOrDefault().Coordinator.UserId.Value,
                serviceRecord = s.Id
            }).FirstOrDefault();
            return new Tuple<int, int>(coordinator.serviceRecord, coordinator.coordinator);
        }

        public biz.premier.Entities.ServiceRecord GetCoordinatorSendNotificationTimeReminder(int workOrder, int serviceLine)
        {
            var serviceRecord = serviceLine == 2 ? _context.ServiceRecords
                .Include(x => x.RelocationCoordinators)
                .ThenInclude(x => x.Coordinator)
                .SingleOrDefault(x => x.WorkOrders.FirstOrDefault().Id == workOrder)
                : _context.ServiceRecords
                .Include(x => x.ImmigrationCoodinators)
                .ThenInclude(x => x.Coordinator)
                .SingleOrDefault(x => x.WorkOrders.FirstOrDefault().Id == workOrder);

            return serviceRecord;

        }

        public string GetTextStatusProperty(int id)
        {
            var text =  _context.CatStatusHousings.FirstOrDefault(x => x.Id == id).Status;

            return text;

        }

        public int GetCoordinatorByServiceRecord(int sr)
        {
            var coordinator = _context.ServiceRecords.Where(x => x.Id == sr).Select(s => new
            {
                coordinator = s.ImmigrationCoodinators.Any()
                    ? s.ImmigrationCoodinators.Select(q => q.Coordinator.UserId).FirstOrDefault()
                    : s.RelocationCoordinators.Select(q => q.Coordinator.UserId).FirstOrDefault(),
                serviceRecord = s.Id
            }).FirstOrDefault();
            return coordinator.coordinator.Value;
        }

        public int?[] GetExperienceTeam(int sr, int serviceLine)
        {
            var assignee = _context.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == sr)?.UserId;
            var coordinator = serviceLine == 1
                ? _context.ImmigrationCoodinators.Where(x => x.ServiceRecordId == sr).Select(s => s.Coordinator.UserId).FirstOrDefault()
                : _context.RelocationCoordinators.Where(x => x.ServiceRecordId == sr).Select(s => s.Coordinator.UserId).FirstOrDefault();
            var consultor = serviceLine == 1
                ? _context.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == sr).Select(s => s.Supplier.UserId).FirstOrDefault()
                : _context.RelocationSupplierPartners.Where(x => x.ServiceRecordId == sr).Select(s => s.Supplier.UserId).FirstOrDefault();
            int?[] users = new[] {assignee, consultor, coordinator};
            return users.Where(a=> a.HasValue.Equals(true)).ToArray();
        }

        public int?[] GetExperienceTeamByConsultorUserId(int sr, int user_id)
        {
            var assignee = _context.AssigneeInformations.FirstOrDefault(x => x.ServiceRecordId == sr)?.UserId;
            var consultor = _context.ImmigrationSupplierPartners.Where(x => x.Supplier.UserId == user_id).Select(s => s.Supplier.UserId).FirstOrDefault();
            int? coordinator = null;
            if (consultor > 0) // esto sifginifa que es un consultor de imm  y busca al coordinador de esa sl
            {
                coordinator = _context.ImmigrationCoodinators.Where(x => x.ServiceRecordId == sr).Select(s => s.Coordinator.UserId).FirstOrDefault().Value;
            }
            else //consultor de relo y busca al coordinador de esa sl 
            {
                coordinator = _context.RelocationCoordinators.Where(x => x.ServiceRecordId == sr).Select(s => s.Coordinator.UserId).FirstOrDefault().Value;
            }

            int?[] users = new[] { assignee, user_id, coordinator };
            return users.Where(a => a.HasValue.Equals(true)).ToArray();
        }


        public int[] GetCountryLeader(int sr)
        {
            int? hostCountry = _context.AssigneeInformations.FirstOrDefault(f => f.ServiceRecordId == sr)?.HostCountry;
            int[] leaders = _context.CountryLeaders.Where(x => x.Country == hostCountry.Value).Select(s => s.Leader).ToArray();
            return leaders;
        }

        public async Task<PushNotificationRequest> SendNotificationAsync(int to, int from, int action, string title, string text, int type)
        {
            Random rnd = new Random();
            PushNotificationRequest notificationRequest = new PushNotificationRequest();
            var userData = _context.Users.FirstOrDefault(x => x.Id == to);
            if(userData == null)
            {

                return notificationRequest;
            }
            if (userData.Push.HasValue)
            {
                if (userData.Push.Value)
                {
                    notificationRequest.to = _context.Users.FirstOrDefault(x => x.Id == to)?.FcmToken;
                    notificationRequest.priority = "high";
                    notificationRequest.notification.body = text;
                    notificationRequest.notification.title = title;
                    notificationRequest.data.body = text;
                    notificationRequest.data.title = title;
                    //notificationRequest.notification.click_action = "FCM_PLUGIN_ACTIVITY";
                    //notificationRequest.data.click_action = "FCM_PLUGIN_ACTIVITY";
                    notificationRequest.data.sound = "default";
                    notificationRequest.data.ConversationId = action;
                    notificationRequest.notification.ConversationId = action;
                    notificationRequest.data.id_notificacion = LongRandom(100000000000000000, 100000000000000050, new Random());
                    notificationRequest.type = type;
                    var request = new HttpRequestMessage(HttpMethod.Post, _fireBasePushNotificationsUrl);
                    request.Headers.TryAddWithoutValidation("Authorization", "key =" + _serverKey);
                    string jsonMessage = JsonConvert.SerializeObject(notificationRequest);
                    request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                    HttpResponseMessage result;
                    using (var client = new HttpClient())
                    {
                        result = await client.SendAsync(request);
                    }
                }
            }

            return notificationRequest;
        }

        public List<UsersEscalate> GetUsers(int wo, int serviceLine, int level)
        {
            List<UsersEscalate> escalates = new List<UsersEscalate>();
            switch (level)
            {
                case (1):
                    var leader = _context.WorkOrders.Where(x => x.Id == wo).Select(s => new 
                    {
                        User = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Country
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Country,
                        Role = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.User.RoleId
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.User.RoleId
                    }).Distinct().ToList();
                    var countryLeaders = _context.CountryLeaders
                        .Where(x => leader.Select(s => s.User).Contains(x.Country))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.Leader,
                            Role = s.LeaderNavigation.RoleId
                        }).Distinct().ToList();
                    escalates.AddRange(countryLeaders);
                    break;
                case (2):
                    var leaderCaseTwo = _context.WorkOrders.Where(x => x.Id == wo).Select(s => new 
                    {
                        User = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Country
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Country,
                        Coordinator = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Id
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Id,
                        Role = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.User.RoleId
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.User.RoleId
                    }).Distinct().ToList();
                    var countryLeadersCaseTwo = _context.CountryLeaders
                        .Where(x => leaderCaseTwo.Select(s => s.User).Contains(x.Country))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.Leader,
                            Role = s.LeaderNavigation.RoleId
                        }).Distinct().ToList();
                    var managerFromSr = _context.OperationLeaders
                        .Where(x => leaderCaseTwo.Select(s => s.Coordinator).Contains(x.Consultant))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var managerAndCountryLeaders = countryLeadersCaseTwo.Union(managerFromSr).Distinct().ToList();
                    escalates.AddRange(managerAndCountryLeaders);
                    break;
                case (3):
                    var leaderCaseThree = _context.WorkOrders.Where(x => x.Id == wo).Select(s => new 
                    {
                        User = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Country
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Country,
                        Coordinator = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Id
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Id,
                        Role = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.User.RoleId
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.User.RoleId
                    }).Distinct().ToList();
                    var countryLeadersCaseThree = _context.CountryLeaders
                        .Where(x => leaderCaseThree.Select(s => s.User).Contains(x.Country))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.Leader,
                            Role = s.LeaderNavigation.RoleId
                        }).Distinct().ToList();
                    var managerFromSrThree = _context.OperationLeaders
                        .Where(x => leaderCaseThree.Select(s => s.Coordinator).Contains(x.Consultant))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var operationLeader = _context.OperationLeaders
                        .Where(x => managerFromSrThree.Select(s => s.User).Contains(x.ConsultantNavigation.UserId.Value))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var managerAndCountryLeadersThree = countryLeadersCaseThree.Union(managerFromSrThree).Distinct().ToList();
                    managerAndCountryLeadersThree =
                        managerAndCountryLeadersThree.Union(operationLeader).Distinct().ToList();
                    escalates.AddRange(managerAndCountryLeadersThree);
                    break;
                case (4):
                    var leaderCaseFour = _context.WorkOrders.Where(x => x.Id == wo).Select(s => new 
                    {
                        User = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Country
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Country,
                        Coordinator = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Id
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Id,
                        Role = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.User.RoleId
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.User.RoleId
                    }).Distinct().ToList();
                    var countryLeadersCaseFour = _context.CountryLeaders
                        .Where(x => leaderCaseFour.Select(s => s.User).Contains(x.Country))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.Leader,
                            Role = s.LeaderNavigation.RoleId
                        }).Distinct().ToList();
                    var managerFromSrFour = _context.OperationLeaders
                        .Where(x => leaderCaseFour.Select(s => s.Coordinator).Contains(x.CreatedBy))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var operationLeaderFour = _context.OperationLeaders
                        .Where(x => managerFromSrFour.Select(s => s.User).Contains(x.ConsultantNavigation.UserId.Value))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var usersFour = _context.Users.Where(x => x.RoleId == 19).Select(s => new UsersEscalate
                    {
                        User = s.Id,
                        Role = s.RoleId
                    }).ToList();
                    var managerAndCountryLeadersFour = countryLeadersCaseFour.Union(managerFromSrFour).Distinct().ToList();
                    managerAndCountryLeadersFour =
                        managerAndCountryLeadersFour.Union(operationLeaderFour).Distinct().ToList();
                    escalates.AddRange(managerAndCountryLeadersFour);
                    break;
                case (5):
                    var leaderCaseFive = _context.WorkOrders.Where(x => x.Id == wo).Select(s => new 
                    {
                        User = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Country
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Country,
                        Coordinator = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.Id
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.Id,
                        Role = serviceLine == 1
                            ? s.ServiceRecord.ImmigrationCoodinators.FirstOrDefault().Coordinator.User.RoleId
                            : s.ServiceRecord.RelocationCoordinators.FirstOrDefault().Coordinator.User.RoleId
                    }).Distinct().ToList();
                    var countryLeadersCaseFive = _context.CountryLeaders
                        .Where(x => leaderCaseFive.Select(s => s.User).Contains(x.Country))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.Leader,
                            Role = s.LeaderNavigation.RoleId
                        }).Distinct().ToList();
                    var managerFromSrFive = _context.OperationLeaders
                        .Where(x => leaderCaseFive.Select(s => s.Coordinator).Contains(x.CreatedBy))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var operationLeaderFive = _context.OperationLeaders
                        .Where(x => managerFromSrFive.Select(s => s.User).Contains(x.ConsultantNavigation.UserId.Value))
                        .Select(s => new UsersEscalate()
                        {
                            User = s.CreatedByNavigation.UserId.Value,
                            Role = s.CreatedByNavigation.User.RoleId
                        }).Distinct().ToList();
                    var users = _context.Users.Where(x => x.RoleId == 19).Select(s => new UsersEscalate
                    {
                        User = s.Id,
                        Role = s.RoleId
                    }).ToList();
                    var managerAndCountryLeadersFive = countryLeadersCaseFive.Union(managerFromSrFive).Distinct().ToList();
                    managerAndCountryLeadersFive =
                        managerAndCountryLeadersFive.Union(operationLeaderFive).Distinct().ToList();
                    escalates.AddRange(managerAndCountryLeadersFive);
                    break;
            }
            return escalates;
        }

        public string GetServiceRecordNumber(int sr)
        {
            var _sr = _context.ServiceRecords.FirstOrDefault(f => f.Id == sr);

            return _sr?.NumberServiceRecord;
        }

        public string GetServiceAssigned(int work_order_service_id)
        {
            return _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == work_order_service_id)?.ServiceNumber == null
                || _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == work_order_service_id)?.ServiceNumber == ""
                ? _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == work_order_service_id)?.ServiceNumber
                : _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == work_order_service_id)?.ServiceNumber;
        }

     
        public bool IsUrgentSr(int sr)
        {
            bool? i = _context.ServiceRecords.FirstOrDefault(f => f.Id == sr)?.Urgent;
            return i.HasValue ? i.Value : false;
        }

        private static long LongRandom(long min, long max, Random rand) {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            
            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}
