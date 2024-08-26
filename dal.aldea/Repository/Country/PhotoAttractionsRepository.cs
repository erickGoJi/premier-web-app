using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class PhotoAttractionsRepository : GenericRepository<biz.premier.Entities.PhotoCityAttraction>, IPhotoAttractionsRepository
    {
        public PhotoAttractionsRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
