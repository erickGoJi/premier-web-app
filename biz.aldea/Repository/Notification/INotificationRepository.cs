using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Notification
{
    public interface INotificationRepository : IGenericRepository<Entities.Notification>
    {
        Entities.Notification UpdateCustom(Entities.Notification notification, int key);
        Entities.Notification GetNotificaitionById(int id);
    }
}
