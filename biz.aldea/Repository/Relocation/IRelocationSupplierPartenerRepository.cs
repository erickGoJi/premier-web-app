using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Relocation
{
    public interface IRelocationSupplierPartenerRepository : IGenericRepository<RelocationSupplierPartner>
    {
        ActionResult GetSuppplierPartnerList(int id, int? countryId);
        List<AssignedServiceSuplier> GetAssignedServiceSuplierById(int id, int sr_id);
        ActionResult GetAssignedServices(int id);
        int GetCountryById(int id);
    }
}
