using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
   public class PhotoAboutRepository : GenericRepository<biz.premier.Entities.PhotoCityAbout>, IPhotoAboutRepository
    {
        public PhotoAboutRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
