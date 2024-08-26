using biz.premier.Entities;
using biz.premier.Repository.Scalate;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Scalate
{
    public class ScalateDocumentRepository : GenericRepository<ScalateDocument>, IScalateDocumentRepository
    {
        public ScalateDocumentRepository(Db_PremierContext context) : base(context) { }
    }
}
