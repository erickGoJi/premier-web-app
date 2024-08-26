using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Training;
using dal.premier.DBContext;

namespace dal.premier.Repository.Training
{
    public class EvaluationRepository : GenericRepository<Evaluation>, IEvaluationRepository
    {
        public EvaluationRepository(Db_PremierContext context) : base(context){}
    }
}