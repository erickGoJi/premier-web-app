using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class ClientPartnerProfileExperienceTeamRepository : GenericRepository<biz.premier.Entities.ClientPartnerProfileExperienceTeam>, IClientPartnerProfileExperienceTeamRepository
    {
        public ClientPartnerProfileExperienceTeamRepository(Db_PremierContext context) : base(context) { }
    }
}
