using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Country
{
    public interface ICountryRepository : IGenericRepository<CatCountry>
    {
        ActionResult GetCountry();
        ActionResult GetCountryById(int id);
        ActionResult GetCountryCityInfo(int user);
        ActionResult GetUserList();
        CountryDocumentGroup AddGroup(CountryDocumentGroup @group);
        CountryDocumentGroup UpdateGroup(CountryDocumentGroup @group);
        List<CountryDocumentGroup> GetGroups();
        biz.premier.Entities.CatCountry UpdateCustom(biz.premier.Entities.CatCountry country);
        int[] GetServiceLocationCountries(int id);
        ActionResult GetServicesLocationsCountries(int id);
        List<CatTypeResource> GetTypeResources();
    }
}
