using biz.premier.Repository.Notification;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Notification
{
    public class NotificationRepository: GenericRepository<biz.premier.Entities.Notification>, INotificationRepository
    {
        public NotificationRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.Notification GetNotificaitionById(int id)
        {
            biz.premier.Entities.Notification review = _context.Notifications
                .Include(x => x.ReminderNotifications)
                .Include(x => x.DocumentNotifications)
                .Include(x => x.CommentNotifications)
                    .ThenInclude(x => x.User).ThenInclude(i => i.Role)
                .Where(x => x.Id == id)
                .FirstOrDefault();
            return review;  
        }

        public biz.premier.Entities.Notification UpdateCustom(biz.premier.Entities.Notification notification, int key)
        {
            if (notification == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.Notification>()
                .Include(i => i.CommentNotifications)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(notification);
                foreach (var i in notification.CommentNotifications)
                {
                    var comments = exist.CommentNotifications.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comments == null)
                    {
                        i.User = null;
                        exist.CommentNotifications.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comments).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId.Value,exist.StatusId);
            return exist;
        }
    }
}
