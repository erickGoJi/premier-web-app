using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class ServiceRepository : GenericRepository<CatService>, IServiceRepository
    {
        public ServiceRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetServiceByServiceLine(int key)
        {
            var service = _context.CatServices.Select(c => new
            {
                c.Id,
                c.Service,
                c.Category.SericeLineId,
                c.CategoryId
            }).Where(x => x.SericeLineId == key).OrderBy(x => x.Service).ToList();

            return new ObjectResult(service);
        }

        public ActionResult GetServiceWithNickname(int country, int client, int serviceLine)
        {
            var services = _context.ServiceLocations
                .Where(x => x.IdClientPartnerProfile == client &&
                            x.ServiceLocationCountries.Select(s => s.IdCountry).Contains(country)
                            && x.IdServiceLine == serviceLine)
                .Select(s => new
                {
                    id = s.IdServiceNavigation.Id,
                    service = s.NickName == "--" ? s.IdServiceNavigation.Service : s.NickName,
                    s.IdServiceNavigation.CategoryId
                }).OrderBy(x => x.service).ToList();
            return new ObjectResult(services);
        }

        public ActionResult GetServiceWithNicknameByDasboard(int serviceId, int client, int serviceLine)
        {
            var services = _context.ServiceLocations
                .Where(x => x.IdClientPartnerProfile == client &&
                            x.IdService == serviceId
                            && x.IdServiceLine == serviceLine)
                .Select(s => new
                {
                    id = s.IdServiceNavigation.Id,
                    service = s.NickName == "--" ? s.IdServiceNavigation.Service : s.NickName,
                    s.IdServiceNavigation.CategoryId
                }).OrderBy(x => x.service).ToList();
            return new ObjectResult(services);
        }
    }
}
