using biz.premier.Entities;
using biz.premier.Repository.AssignedService;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.AssignedService
{
    public class AssignedServiceSupplierRepository : GenericRepository<AssignedServiceSuplier>, IAssignedServiceSupplierRepository
    {
        public AssignedServiceSupplierRepository(Db_PremierContext context): base(context) { }
    }
}
