using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class TitleRepository : GenericRepository<biz.premier.Entities.CatTitle>, ITitleRepository
    {
        public TitleRepository(Db_PremierContext context) : base(context) { }
    }
}
