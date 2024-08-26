using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class BreedRepository : GenericRepository<CatBreed>, IBreedRepository
    {
        public BreedRepository(Db_PremierContext context) : base(context)
        {

        }
    }
}
