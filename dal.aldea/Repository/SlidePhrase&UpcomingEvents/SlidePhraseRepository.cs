using biz.premier.Entities;
using biz.premier.Repository;
using dal.premier.DBContext;

namespace dal.premier.Repository
{
    public class SlidePhraseRepository : GenericRepository<SlidePhrase>, ISlidePhraseRepository
    {
        public SlidePhraseRepository(Db_PremierContext context):base(context){}
    }
}