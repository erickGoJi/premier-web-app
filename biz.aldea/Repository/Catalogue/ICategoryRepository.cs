using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Catalogue
{
    public interface ICategoryRepository : IGenericRepository<CatCategory>
    {
        ActionResult GetCataegoryByServiceLineIdWhitNickName(int serviceLine);
    }
}
