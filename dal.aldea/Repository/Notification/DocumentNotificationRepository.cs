using biz.premier.Entities;
using biz.premier.Repository.Notification;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Notification
{
    public class DocumentNotificationRepository : GenericRepository<DocumentNotification>, IDocumentNotificationRepository
    {
        public DocumentNotificationRepository(Db_PremierContext context) : base(context) { }
    }
}
