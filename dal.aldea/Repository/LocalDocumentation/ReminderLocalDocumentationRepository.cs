using biz.premier.Entities;
using biz.premier.Repository.LocalDocumentation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.LocalDocumentation
{
    public class ReminderLocalDocumentationRepository : GenericRepository<ReminderLocalDocumentation>, IReminderLocalDocumentationRepository
    {
        public ReminderLocalDocumentationRepository(Db_PremierContext context) : base(context) { }
    }
}
