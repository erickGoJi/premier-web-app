using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class PhotoWhereEatRepository : GenericRepository<biz.premier.Entities.PhotoWhereEat>, IPhotoWhereEatRepository
    {
        public PhotoWhereEatRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
