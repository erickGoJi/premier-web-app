using biz.premier.Entities;
using biz.premier.Repository.DocumentManagement;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Repository.DocumentAdministrativeContactsConsultant;

namespace dal.premier.Repository.DocumentAdministrativeContactsConsultant
{
    public class DocumentAdministrativeContactsConsultantRepository : GenericRepository<biz.premier.Entities.DocumentAdministrativeContactsConsultant>, IDocumentAdministrativeContactsConsultantRepository
    {
        public DocumentAdministrativeContactsConsultantRepository(Db_PremierContext context) : base(context) { }
    }
   
}
