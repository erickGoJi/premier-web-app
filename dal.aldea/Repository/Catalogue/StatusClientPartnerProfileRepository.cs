using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class StatusClientPartnerProfileRepository: GenericRepository<biz.premier.Entities.StatusClientPartnerProfile>, IStatusClientPartnerProfileRepository
    {
        public StatusClientPartnerProfileRepository(Db_PremierContext context) : base(context) { }
    }
}
