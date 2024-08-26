using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Immigration
{
    public interface IImmigrationSupplierPartenerRepository : IGenericRepository<ImmigrationSupplierPartner>
    {
        ActionResult GetSuppplierPartnerList(int id);
        ActionResult GetAssignedServices(int id);
        int GetCountryById(int id);
    }
}
