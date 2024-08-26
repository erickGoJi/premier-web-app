using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class PhotoEmergencyRepository : GenericRepository<biz.premier.Entities.PhotoCityEmergency>, IPhotoEmergencyRepository
    {
        public PhotoEmergencyRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
