using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.LocalDocumentation
{
    public interface ILocalDocumentationRepository : IGenericRepository<Entities.LocalDocumentation>
    {
        Entities.LocalDocumentation UpdateCustom(Entities.LocalDocumentation localDocumentation, int key);
    }
}
