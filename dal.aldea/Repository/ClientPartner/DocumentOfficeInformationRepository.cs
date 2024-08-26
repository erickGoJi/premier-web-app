using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ClientPartner
{
    public class DocumentOfficeInformationRepository : GenericRepository<biz.premier.Entities.DocumentOfficeInformation>, IDocumentOfficeInformationRepository
    {
        public DocumentOfficeInformationRepository(Db_PremierContext context) : base(context) { }
    }
}
