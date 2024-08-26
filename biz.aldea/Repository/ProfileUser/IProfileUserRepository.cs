using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Models.PricingInfo;

namespace biz.premier.Repository.ProfileUser
{
    public interface IProfileUserRepository : IGenericRepository<biz.premier.Entities.ProfileUser>
    {
        Entities.ProfileUser AddConsultant(Entities.ProfileUser consultantContactsConsultant);
        Entities.ProfileUser UpdateCustom(Entities.ProfileUser consultantContactsConsultant, int key);
        Entities.ProfileUser GetConsultant(int key);
        ActionResult GetDirectory(int? title, int? country, int? city, int? company, int? office);
        ActionResult DashboardInicio(int userId);
        string GetSupplierPartner(int key);
        PricingInfo GetPricingInfo(int key);
        bool isVip(int key);
        ActionResult GetClients(int key);
        ActionResult GetCountryLeader(int key);
        ActionResult GetAssignedTeam(int? type);
        ActionResult GetProfilesByTitle(int? key, int? client, int? serviceLine);
        Entities.ProfileUser DeleteCustom(Entities.ProfileUser user);
    }
}
