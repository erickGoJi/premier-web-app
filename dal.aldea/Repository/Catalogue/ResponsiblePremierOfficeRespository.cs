using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class ResponsiblePremierOfficeRespository : GenericRepository<biz.premier.Entities.ResponsiblePremierOffice>, IResponsiblePremierOfficeRespository
    {
        public ResponsiblePremierOfficeRespository(Db_PremierContext context) : base(context) { }
    }
}
