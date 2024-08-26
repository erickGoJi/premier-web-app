using biz.premier.Entities;
using biz.premier.Repository.WorkPermit;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.WorkPermit
{
    public class DocumentWorkPermitRepository : GenericRepository<DocumentWorkPermit>, IDocumentWorkPermitRepository
    {
        public DocumentWorkPermitRepository(Db_PremierContext context): base(context) { }
    }
}
