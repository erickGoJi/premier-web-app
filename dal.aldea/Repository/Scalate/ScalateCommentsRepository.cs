using biz.premier.Entities;
using biz.premier.Repository.Scalate;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Scalate
{
    public class ScalateCommentsRepository : GenericRepository<ScalateComment>, IScalateCommentsRepository
    {
        public ScalateCommentsRepository(Db_PremierContext context) : base(context) { }
    }
}
