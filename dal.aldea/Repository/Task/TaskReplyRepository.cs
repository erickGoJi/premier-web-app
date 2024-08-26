using dal.premier.DBContext;
using biz.premier.Repository.Task;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Task
{
    public class TaskReplyRepository : GenericRepository<biz.premier.Entities.TaskReply>, ITaskReplyRepository
    {
        public TaskReplyRepository(Db_PremierContext context) : base(context)
        {

        }
    }
}
