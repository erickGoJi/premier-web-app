using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using biz.premier.Models;
using biz.premier.Models.PushNotification;

namespace biz.premier.Repository.NotificationSystem
{
    public interface INotificationSystemRepository : IGenericRepository<Entities.NotificationSystem>
    {
        ActionResult GetAllCustom(int user, DateTime? dateRange1, DateTime? dateRange2, int? notificationType, int? serviceRecord, bool? archive);
        ActionResult GetAllCustomUnRead(int user, DateTime? dateRange1, DateTime? dateRange2, int? notificationType, int? serviceRecord, bool? archive);
        ActionResult GetNotifications(int user);
        int GetUserId(int key);
        int GetUserAssignee(int key);
        Tuple<int, int> GetCoordinator(int workOrder);

        Tuple<int, int> GetCoordinatorbyWosId(int WosId);
        int GetCoordinatorByServiceRecord(int sr);
        int?[] GetExperienceTeam(int sr, int serviceLine);
        int?[] GetExperienceTeamByConsultorUserId(int sr, int user_id);

        int[] GetCountryLeader(int sr);
        Task<PushNotificationRequest> SendNotificationAsync(int to, int from, int action, string title, string text, int type);
        List<UsersEscalate> GetUsers(int wo, int serviceLine, int level);
        string GetServiceRecordNumber(int sr);
        bool IsUrgentSr(int sr);
        string GetServiceAssigned(int work_order_service_id);
        ActionResult GetAllCustomApp(int user);
        biz.premier.Entities.ServiceRecord GetCoordinatorSendNotificationTimeReminder(int workOrder, int serviceLine);
        string GetTextStatusProperty(int id);
        ActionResult DeleteNotificationById(int id);
    }
}
