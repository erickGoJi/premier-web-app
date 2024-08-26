using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class DocumentClientPartnerProfileRepository : GenericRepository<biz.premier.Entities.DocumentClientPartnerProfile>, IDocumentClientPartnerProfileRepository
    {
        public DocumentClientPartnerProfileRepository(Db_PremierContext context) : base(context) { }
    }
}
