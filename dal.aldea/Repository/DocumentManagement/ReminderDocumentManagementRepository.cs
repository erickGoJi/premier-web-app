using biz.premier.Entities;
using biz.premier.Repository.DocumentManagement;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.DocumentManagement
{
    public class ReminderDocumentManagementRepository : GenericRepository<ReminderDocumentManagement>, IReminderDocumentManagementRepository
    {
        public ReminderDocumentManagementRepository(Db_PremierContext context) : base(context) { }
    }
}
