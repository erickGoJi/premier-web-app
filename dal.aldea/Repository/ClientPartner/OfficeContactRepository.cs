using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class OfficeContactRepository : GenericRepository<biz.premier.Entities.OfficeContact>, IOfficeContactRepository
    {
        public OfficeContactRepository(Db_PremierContext context) : base(context) { }
    }
}
