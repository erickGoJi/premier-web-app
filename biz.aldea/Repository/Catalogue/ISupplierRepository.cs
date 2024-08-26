using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Catalogue
{
    public interface ISupplierRepository : IGenericRepository<CatSupplier>
    {
        ActionResult GetSupplierBySR(int ServiceLineId, int SR);
    }
}
