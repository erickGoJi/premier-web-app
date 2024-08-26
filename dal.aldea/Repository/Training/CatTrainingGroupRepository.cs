using biz.premier.Entities;
using biz.premier.Repository.Training;
using dal.premier.DBContext;

namespace dal.premier.Repository.Training
{
    public class CatTrainingGroupRepository : GenericRepository<CatTrainingGroup>, ICatTrainingGroupRepository
    {
        public CatTrainingGroupRepository(Db_PremierContext context): base(context){}
    }
}