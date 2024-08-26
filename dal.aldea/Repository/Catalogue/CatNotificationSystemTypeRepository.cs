using biz.premier.Repository.NotificationSystemType;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class CatNotificationSystemTypeRepository : GenericRepository<biz.premier.Entities.CatNotificationSystemType>, ICatNotificationSystemTypeRepository
    {
        public CatNotificationSystemTypeRepository(Db_PremierContext context) : base(context) { }
    }
}
