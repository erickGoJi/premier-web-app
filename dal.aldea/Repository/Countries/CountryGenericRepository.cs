using biz.premier.Repository.Countries;
using dal.premier.DBContext;

namespace dal.premier.Repository.Countries
{
    public class CountryGenericRepository : GenericRepository<biz.premier.Entities.Country>, ICountryGenericRepository
    {
        public CountryGenericRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}