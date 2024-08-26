using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class OfficeInformationRepository : GenericRepository<biz.premier.Entities.OfficeInformation>, IOfficeInformationRepository
    {
        public OfficeInformationRepository(Db_PremierContext context) : base(context) { }
    }
}
