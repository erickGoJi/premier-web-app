using biz.premier.Repository.CountryGallery;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.CountryGallery
{
    public class CountryRepository : GenericRepository<biz.premier.Entities.Country1>, ICountryRepository
    {
        public CountryRepository(Db_PremierContext context) : base(context) { }
    }
}
