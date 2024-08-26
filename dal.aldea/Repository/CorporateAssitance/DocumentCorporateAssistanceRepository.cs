using biz.premier.Repository.CorporateAssistance;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace dal.premier.Repository.CorporateAssitance
{
    public class DocumentCorporateAssistanceRepository : GenericRepository<biz.premier.Entities.DocumentCorporateAssistance>, IDocumentCorporateAssistanceRepository
    {
        public DocumentCorporateAssistanceRepository(Db_PremierContext context) : base(context) { }
    }
}
