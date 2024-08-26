using api.premier.Models.Catalogue;
using api.premier.Models.ServiceRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.NotificationSystem
{
    public class NotificationSystemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? NotificationType { get; set; }
        public int? ServiceRecord { get; set; }
        public TimeSpan? Time { get; set; }
        public int? UserFrom { get; set; }
        public bool? Accept { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Color { get; set; }
        public int? UserTo { get; set; }
        public bool? Archive { get; set; }
        public bool? View { get; set; }

    }
    public class NotificationSystemSelectDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? NotificationType { get; set; }
        public int? ServiceRecord { get; set; }
        public TimeSpan? Time { get; set; }
        public int? UserFrom { get; set; }
        public bool? Accept { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Color { get; set; }
        public int? UserTo { get; set; }
        public bool? Archive { get; set; }
        public bool? View { get; set; }
        public virtual CatNotificationSystemTypeDto NotificationTypeNavigation { get; set; }
        public virtual ServiceRecordDto ServiceRecordNavigation { get; set; }
        public virtual UserDto UserFromNavigation { get; set; }
    }
}
