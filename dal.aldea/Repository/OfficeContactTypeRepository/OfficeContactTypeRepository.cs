using biz.premier.Entities;
using biz.premier.Repository.OfficeContactTypeRepository;
using dal.premier.DBContext;

namespace dal.premier.Repository.OfficeContactTypeRepository
{
    public class OfficeContactTypeRepository : GenericRepository<OfficeContactType>, IOfficeContactTypeRepository
    {
        public OfficeContactTypeRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}