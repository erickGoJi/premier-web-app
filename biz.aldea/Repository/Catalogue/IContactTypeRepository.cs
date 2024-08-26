using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Catalogue
{
    public interface IContactTypeRepository : IGenericRepository<CatContactType>
    {
        List<OfficeContactType> GetOfficesContactTypes();
    }
}
