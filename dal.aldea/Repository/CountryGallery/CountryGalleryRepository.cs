using biz.premier.Entities;
using biz.premier.Repository.CountryGallery;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.CountryGallery
{
    public class CountryGalleryRepository : GenericRepository<biz.premier.Entities.CountryGallery>, ICountryGalleryRepository
    {
        public CountryGalleryRepository(Db_PremierContext context) : base(context) { }
    }
}
