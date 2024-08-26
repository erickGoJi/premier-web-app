using biz.premier.Entities;
using biz.premier.Repository.VisaDeregistration;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.VisaDeregistration
{
    public class DocumentVisaDeregistrationRepository : GenericRepository<DocumentVisaDeregistration>, IDocumentVisaDeregistrationRepository
    {
        public DocumentVisaDeregistrationRepository(Db_PremierContext context) : base(context) { }
    }
}
