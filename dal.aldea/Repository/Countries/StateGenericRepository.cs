using biz.premier.Entities;
using biz.premier.Repository.Countries;
using dal.premier.DBContext;

namespace dal.premier.Repository.Countries
{
    public class StateGenericRepository: GenericRepository<State>, IStateGenericRepository
    {
        public StateGenericRepository(Db_PremierContext context):base(context){}
    }
}