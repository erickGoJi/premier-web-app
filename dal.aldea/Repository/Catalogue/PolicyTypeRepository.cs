using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class PolicyTypeRepository : GenericRepository<CatPolicyType>, IPolicyTypeRepository
    {
        public PolicyTypeRepository(Db_PremierContext context) : base(context) { }
    }
}
