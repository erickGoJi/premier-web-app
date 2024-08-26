using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Training;
using dal.premier.DBContext;

namespace dal.premier.Repository.Training
{
    public class ThemeRepository : GenericRepository<Theme>, IThemeRepository
    {
        public ThemeRepository(Db_PremierContext context):base(context){}
    }
}