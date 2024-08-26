using biz.premier.Repository.Task;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Task
{
    public class TaskDocumentRepository : GenericRepository<biz.premier.Entities.TaskDocument>, ITaskDocumentRepository
    {
        public TaskDocumentRepository(Db_PremierContext context) : base(context)
        {

        }
    }
}
