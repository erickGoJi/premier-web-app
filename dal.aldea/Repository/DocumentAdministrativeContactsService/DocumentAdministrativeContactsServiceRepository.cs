using biz.premier.Entities;
using biz.premier.Repository.DocumentManagement;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Repository.DocumentAdministrativeContactsService;

namespace dal.premier.Repository.DocumentAdministrativeContactsService
{                
    public class DocumentAdministrativeContactsServiceRepository : GenericRepository<biz.premier.Entities.DocumentAdministrativeContactsService>, IDocumentAdministrativeContactsServiceRepository
    {
        public DocumentAdministrativeContactsServiceRepository(Db_PremierContext context) : base(context) { }
    }
   
}
