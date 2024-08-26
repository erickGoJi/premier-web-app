using biz.premier.Entities;
using biz.premier.Repository.Catalogs;
using dal.premier.DBContext;

namespace dal.premier.Repository.Catalogs
{
    public class RelationshipContactRepository : GenericRepository<CatRelationshipContact>, IRelationshipContactRepository
    {
        public RelationshipContactRepository(Db_PremierContext context) : base(context)
        {
            
        }
    }
}