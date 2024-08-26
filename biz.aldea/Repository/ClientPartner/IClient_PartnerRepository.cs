using biz.premier.Entities;
using biz.premier.Models.ClientPartnerProfile;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.ClientPartner
{
    public interface IClient_PartnerRepository : IGenericRepository<biz.premier.Entities.ClientPartnerProfile>
    {
        ActionResult GetClientPartner(DateTime? date_range_in, DateTime? date_range_fi, int? type, int? company_type, int? country, int? city, int? status, int lead_or_client, int? state);

        ActionResult GetClientPartnerById(int Id);
        List<ClientPartnerProfileDto> GetClientPartnerBytestId(int Id);

        ActionResult GetClientPartnerCatalog();

        ActionResult GetExperienceTeamCatalog(int serviceLine, int? client);
        ActionResult GetAuthorizedby(int client, int country, int city);
        ActionResult GetClientList(int partner);
        bool UpdatePartner(int client, int partner);
        List<int> GetIdServicesByPartner(int id, int serviceLine);
        List<ServiceLocationDto> GetServiceLocation(int id, int serviceLine);
    }
}
