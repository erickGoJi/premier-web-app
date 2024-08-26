using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class NationalityRepository : GenericRepository<Nationality>, INationalityRepository
    {
        public NationalityRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}