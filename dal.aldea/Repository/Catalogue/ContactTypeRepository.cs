using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class ContactTypeRepository : GenericRepository<CatContactType>, IContactTypeRepository
    {
        public ContactTypeRepository(Db_PremierContext context) : base(context) { }
        public List<OfficeContactType> GetOfficesContactTypes()
        {
            return _context.OfficeContactTypes.ToList();
        }
    }
}
