using biz.premier.Repository.Country;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Country
{
    public class CityAboutRepository : GenericRepository<biz.premier.Entities.CityAbout>, ICityAboutRepository
    {
        public CityAboutRepository(Db_PremierContext context) : base(context)
        {
        }
    }
}
