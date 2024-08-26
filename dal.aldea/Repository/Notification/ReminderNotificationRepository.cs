using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Notification;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Notification
{
    public class ReminderNotificationRepository : GenericRepository<ReminderNotification>, IReminderNotificationRepository
    {
        public ReminderNotificationRepository(Db_PremierContext context): base(context) { }
    }
}
