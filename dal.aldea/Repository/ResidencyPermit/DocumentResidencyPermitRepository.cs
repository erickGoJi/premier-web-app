using biz.premier.Entities;
using biz.premier.Repository.ResidencyPermit;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ResidencyPermit
{
    public class DocumentResidencyPermitRepository : GenericRepository<DocumentResidencyPermit>, IDocumentResidencyPermitRepository
    {
        public DocumentResidencyPermitRepository(Db_PremierContext context) : base(context) { }
    }
}
