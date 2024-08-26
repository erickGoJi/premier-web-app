using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogue
{
    public class CountryPhoneCodeRepository : GenericRepository<CountryPhoneCode>, ICountryPhoneCodeRepository
    {
        public CountryPhoneCodeRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}