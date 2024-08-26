using biz.premier.Entities;
using biz.premier.Repository.Training;
using dal.premier.DBContext;

namespace dal.premier.Repository.Training
{
    public class CatTrainingTypeRepository : GenericRepository<CatTrainingType>, ICatTrainingTypeRepository
    {
        public CatTrainingTypeRepository(Db_PremierContext context):base(context){}
    }
}