using biz.premier.Repository.ImmigrationProfile;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.ImmigrationProfile
{
    public class DocumentDependentImmigration : GenericRepository<biz.premier.Entities.DocumentDependentInformation>, IDocumentDependentImmigration
    {
        public DocumentDependentImmigration(Db_PremierContext context) : base(context) { }
    }
}
