using System.Collections.Generic;
using System.Threading.Tasks;

namespace biz.premier.Repository.Countries
{
    public interface ICityGenericRepository : IGenericRepository<Entities.City>
    {
        Task<List<Entities.City>> GetAllCitiesWithState(int id);
        List<biz.premier.Entities.City> GetCtyByCountry(string countryName);

        List<biz.premier.Entities.City> GetCityByCountryCityNames(string countryName , string cityName);
        List<biz.premier.Entities.City> GetCtyByCountryId(int countryId);
    }
}