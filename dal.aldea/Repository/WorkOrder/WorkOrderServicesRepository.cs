using biz.premier.Entities;
using biz.premier.Repository.ServiceOrder;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.ServiceOrder
{
    public class WorkOrderServicesRepository : GenericRepository<WorkOrderService>, IWorkOrderServicesRepository
    {
        public WorkOrderServicesRepository(Db_PremierContext context): base(context) { }
    }
}
