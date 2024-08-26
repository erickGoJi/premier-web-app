using biz.premier.Entities;
using biz.premier.Repository.DocumentManagement;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Repository.DocumentConsultantContactsService;

namespace dal.premier.Repository.DocumentConsultantContactsService
{
    public class DocumentConsultantContactsServiceRepository: GenericRepository<biz.premier.Entities.DocumentConsultantContactsService>,IDocumentConsultantContactsServiceRepository
    {
        public DocumentConsultantContactsServiceRepository(Db_PremierContext context) : base(context) { }
    }
}
