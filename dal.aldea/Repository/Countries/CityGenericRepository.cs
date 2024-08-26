using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using biz.premier.Repository.Countries;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Countries
{
    public class CityGenericRepository: GenericRepository<biz.premier.Entities.City>, ICityGenericRepository
    {
        public CityGenericRepository(Db_PremierContext context) : base(context)
        {
            
        }

        public async Task<List<biz.premier.Entities.City>> GetAllCitiesWithState(int id)
        {
            var all = await _context.Cities.Where(x => x.IdStateNavigation.IdCountry == id).Select(s =>
                new biz.premier.Entities.City
                {
                    Id = s.Id,
                    IdState = s.IdState,
                    Name = $"{s.Name}, {s.IdStateNavigation.Name}"
                }).ToListAsync();
            return all;
        }

        public List<biz.premier.Entities.City> GetCtyByCountry(string countryName)
        {
            var country = _context.Cities.Where(x => x.IdStateNavigation.IdCountryNavigation.Name == countryName)
                .Select(s =>
                new biz.premier.Entities.City
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
            return country;
        }

        public List<biz.premier.Entities.City> GetCityByCountryCityNames(string countryName, string cityName)
        {
            var country = _context.Cities.Where(x => x.IdStateNavigation.IdCountryNavigation.Name == countryName
                                                     && x.Name.ToUpper().Contains(cityName.ToUpper()))
              .Select(s =>
              new biz.premier.Entities.City
              {
                  Id = s.Id,
                  Name = s.Name
              }).ToList();
            return country;
        }

        public List<biz.premier.Entities.City> GetCtyByCountryId(int countryId)
        {
            var country = _context.Cities.Where(x => x.IdStateNavigation.IdCountryNavigation.Id == countryId)
                .Select(s =>
                new biz.premier.Entities.City
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
            return country;
        }

       
    }
}