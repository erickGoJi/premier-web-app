using biz.premier.Entities;
using biz.premier.Repository.Country;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.Country
{
    public class CountryRepository : GenericRepository<CatCountry>, ICountryRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;

        public CountryRepository(Db_PremierContext context, IConfiguration config, IEmailService emailService) : base(context)
        {
            _config = config;
            _emailservice = emailService;
        }

        public ActionResult GetCountry()
        {
            var country = _context.CatCountries
                .Include(x => x.IdCurrencyNavigation)
                .Include(x => x.IdLenguageNavigation)
                .Include(x => x.CatCities)
                .Select(x => new
                {
                    x.Id,
                    x.IdCurrencyNavigation.Currency,
                    lenguage = x.IdLenguageNavigation.Name,
                    country = x.Name,
                    cities = x.CatCities.Count(),
                    citiesData = x.CatCities,
                    x.CreatedDate

                }).ToList();
            return new ObjectResult(country);
        }

        public ActionResult GetCountryById(int id)
        {
            var country = _context.CatCountries
                .Include(x => x.CatCities)
                .Include(x => x.CountryDocuments)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityAbouts).ThenInclude(i => i.PhotoCityAbouts)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityAttractions).ThenInclude(i => i.PhotoCityAttractions)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityEmergencies).ThenInclude(i => i.PhotoCityEmergencies)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityWhatToDos).ThenInclude(i => i.PhotoWhatToDos)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityWhereEats).ThenInclude(i => i.PhotoWhereEats)
                .Where(x => x.Id == id)
                .Select(x => new {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.IdCurrency,
                    x.IdLenguage,
                    x.Sortname,
                    x.Phonecode,
                    x.Flag,
                    x.CreatedBy,
                    x.CreatedDate,
                    x.UpdateBy,
                    x.UpdatedDate,
                    x.CountryDocuments,
                    CountryLeaders = x.CountryLeaders.Select(q => new
                    {
                        Profile = q.LeaderNavigation.ProfileUsers.FirstOrDefault().TitleNavigation.Title,
                        Name = q.LeaderNavigation.Name,
                        leader = q.Leader,
                        q.Country
                    }),
                    CatCities = x.CatCities.Select(d => new {
                        d.Id,
                        d.City,
                        d.IdCountry,
                        d.IdTimeZone,
                        d.Title,
                        d.Subtitle,
                        d.FileName,
                        d.FileRequest,
                        d.TypeId,
                        d.Latitude,
                        d.Length,
                        d.ResorucesGuide,
                        d.ResorucesGuideRequest,
                        d.CityAbouts,
                        d.CityAttractions,
                        d.CityEmergencies,
                        d.CityWhatToDos,
                        d.CityWhereEats
                    }).ToList()
                }).ToList();
            return new ObjectResult(country);
        }

        public ActionResult GetCountryCityInfo(int user)
        {
            var userCountry = _context.Users
                .Include(i => i.ProfileUsers)
                .Include(i => i.AssigneeInformations)
                .SingleOrDefault(s => s.Id == user);
            int[] countryId = userCountry.ProfileUsers.Any()
                ? new int[] {userCountry.ProfileUsers.FirstOrDefault().Country.Value }
                : new []{ userCountry.AssigneeInformations.FirstOrDefault().HostCountry.Value, userCountry.AssigneeInformations.FirstOrDefault().HomeCountryId.Value };
            int[] citiesId = userCountry.ProfileUsers.Any()
                ? new int[] {userCountry.ProfileUsers.FirstOrDefault().City.Value }
                : new []{ userCountry.AssigneeInformations.FirstOrDefault().HostCityId.Value, userCountry.AssigneeInformations.FirstOrDefault().HomeCityId.Value };
            var country = _context.CatCountries
                .Include(x => x.CatCities)
                .Include(x => x.CountryDocuments)
                .Include(x => x.CountryLeaders)
                    .ThenInclude(i => i.LeaderNavigation)
                        .ThenInclude(i => i.ProfileUsers)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityAbouts).ThenInclude(i => i.PhotoCityAbouts)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityAttractions).ThenInclude(i => i.PhotoCityAttractions)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityEmergencies).ThenInclude(i => i.PhotoCityEmergencies)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityWhatToDos).ThenInclude(i => i.PhotoWhatToDos)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityWhereEats).ThenInclude(i => i.PhotoWhereEats)
                .Where(x => countryId.Contains(x.Id))
                .Select(x => new {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.IdCurrency,
                    x.IdLenguage,
                    x.Sortname,
                    x.Phonecode,
                    x.Flag,
                    x.CreatedBy,
                    x.CreatedDate,
                    x.UpdateBy,
                    x.UpdatedDate,
                    CountryDocuments = x.CountryDocuments,
                    Leader = x.CountryLeaders.Select(q => new
                    {
                        Profile = q.LeaderNavigation.ProfileUsers.FirstOrDefault().TitleNavigation.Title,
                        Name = q.LeaderNavigation.Name,
                        userId = q.LeaderNavigation.Id
                    }),
                    CatCities = x.CatCities.Where(w=>citiesId.Contains(w.Id)).Select(d => new {
                        d.Id,
                        d.City,
                        d.IdCountry,
                        d.IdTimeZone,
                        d.CityAbouts,
                        d.CityAttractions,
                        d.CityEmergencies,
                        d.CityWhatToDos,
                        d.CityWhereEats
                    }).ToList(),
                    cities = x.CatCities.Where(w=>citiesId.Contains(w.Id)).Select(c => new {
                        c.City,
                        c.IdTimeZoneNavigation.TimeZone,
                        c.CreateDate,
                        resource_guide = (c.CityAbouts.Count > 0 ? 1 : 0) + (c.CityAttractions.Count > 0 ? 1 : 0) + (c.CityEmergencies.Count > 0 ? 1 : 0) + (c.CityWhatToDos.Count > 0 ? 1 : 0) + (c.CityWhereEats.Count > 0 ? 1 : 0)
                    }).ToList()
                }).ToList();
            // foreach (var x in country)
            // {
            //     x.CatCities = x.CatCities.SkipWhile(s => !citiesId.Contains(s.Id)).ToList();
            // }
            return new ObjectResult(country);
        }

        public CountryDocumentGroup UpdateGroup(CountryDocumentGroup @group)
        {
            _context.CountryDocumentGroups.Update(@group);
            _context.SaveChanges();
            return @group;
        }

        public List<CountryDocumentGroup> GetGroups()
        {
            var groups = _context.CountryDocumentGroups.ToList();
            return groups;
        }

        public biz.premier.Entities.CatCountry UpdateCustom(biz.premier.Entities.CatCountry country)
        {
            if (country == null)
                return null;
            var exist = _context.CatCountries
                .Include(x => x.CatCities)
                .Include(x => x.CountryDocuments)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityAbouts).ThenInclude(i => i.PhotoCityAbouts)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityAttractions).ThenInclude(i => i.PhotoCityAttractions)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityEmergencies).ThenInclude(i => i.PhotoCityEmergencies)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityWhatToDos).ThenInclude(i => i.PhotoWhatToDos)
                .Include(x => x.CatCities)
                    .ThenInclude(x => x.CityWhereEats).ThenInclude(i => i.PhotoWhereEats)
                .Include(i => i.CountryLeaders)
                .SingleOrDefault(s => s.Id == country.Id);

            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(country);
                foreach (var document in country.CountryDocuments)
                {
                    var existingDocument = exist.CountryDocuments.Where(p => p.Id == document.Id).FirstOrDefault();
                    if (existingDocument == null)
                    {
                        exist.CountryDocuments.Add(document);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingDocument).CurrentValues.SetValues(document);
                    }
                }
                foreach (var city in country.CatCities)
                {
                    var existingCities = exist.CatCities.Where(p => p.Id == city.Id).FirstOrDefault();
                    if (existingCities == null)
                    {
                        exist.CatCities.Add(city);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingCities).CurrentValues.SetValues(city);
                        foreach (var about in city.CityAbouts)
                        {
                            var existingAbout = existingCities.CityAbouts.Where(p => p.Id == about.Id).FirstOrDefault();
                            if (existingAbout == null)
                            {
                                existingCities.CityAbouts.Add(about);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(existingAbout).CurrentValues.SetValues(about);
                                foreach(var photo in about.PhotoCityAbouts) 
                                {
                                    var existingPhoto = existingAbout.PhotoCityAbouts.Where(p => p.Id == photo.Id).FirstOrDefault();
                                    if (existingPhoto == null)
                                    {
                                        existingAbout.PhotoCityAbouts.Add(photo);
                                        _context.SaveChanges();
                                    }
                                    else 
                                    {
                                        _context.Entry(existingPhoto).CurrentValues.SetValues(photo);
                                    }
                                }
                            }
                        }
                        foreach (var attraction in city.CityAttractions)
                        {
                            var existingAttraction = existingCities.CityAttractions.Where(p => p.Id == attraction.Id).FirstOrDefault();
                            if (existingAttraction == null)
                            {
                                existingCities.CityAttractions.Add(attraction);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(existingAttraction).CurrentValues.SetValues(attraction);
                                foreach (var photo in attraction.PhotoCityAttractions)
                                {
                                    var existingPhoto = existingAttraction.PhotoCityAttractions.Where(p => p.Id == photo.Id).FirstOrDefault();
                                    if (existingPhoto == null)
                                    {
                                        existingAttraction.PhotoCityAttractions.Add(photo);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(existingPhoto).CurrentValues.SetValues(photo);
                                    }
                                }
                            }
                        }
                        foreach (var emergencies in city.CityEmergencies)
                        {
                            var existingEmergencies = existingCities.CityEmergencies.Where(p => p.Id == emergencies.Id).FirstOrDefault();
                            if (existingEmergencies == null)
                            {
                                existingCities.CityEmergencies.Add(emergencies);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(existingEmergencies).CurrentValues.SetValues(emergencies);
                                foreach (var photo in emergencies.PhotoCityEmergencies)
                                {
                                    var existingPhoto = existingEmergencies.PhotoCityEmergencies.Where(p => p.Id == photo.Id).FirstOrDefault();
                                    if (existingPhoto == null)
                                    {
                                        existingEmergencies.PhotoCityEmergencies.Add(photo);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(existingPhoto).CurrentValues.SetValues(photo);
                                    }
                                }
                            }
                        }
                        foreach (var whatToDos in city.CityWhatToDos)
                        {
                            var existingWhatToDos = existingCities.CityWhatToDos.Where(p => p.Id == whatToDos.Id).FirstOrDefault();
                            if (existingWhatToDos == null)
                            {
                                existingCities.CityWhatToDos.Add(whatToDos);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(existingWhatToDos).CurrentValues.SetValues(whatToDos);
                                foreach (var photo in whatToDos.PhotoWhatToDos)
                                {
                                    var existingPhoto = existingWhatToDos.PhotoWhatToDos.Where(p => p.Id == photo.Id).FirstOrDefault();
                                    if (existingPhoto == null)
                                    {
                                        existingWhatToDos.PhotoWhatToDos.Add(photo);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(existingPhoto).CurrentValues.SetValues(photo);
                                    }
                                }
                            }
                        }
                        foreach (var whereEats in city.CityWhereEats)
                        {
                            var existingWhereEats = existingCities.CityWhereEats.Where(p => p.Id == whereEats.Id).FirstOrDefault();
                            if (existingWhereEats == null)
                            {
                                existingCities.CityWhereEats.Add(whereEats);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(existingWhereEats).CurrentValues.SetValues(whereEats);
                                foreach (var photo in whereEats.PhotoWhereEats)
                                {
                                    var existingPhoto = existingWhereEats.PhotoWhereEats.Where(p => p.Id == photo.Id).FirstOrDefault();
                                    if (existingPhoto == null)
                                    {
                                        existingWhereEats.PhotoWhereEats.Add(photo);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(existingPhoto).CurrentValues.SetValues(photo);
                                    }
                                }
                            }
                        }
                    }
                }

                //foreach (var o in country.CountryDocuments)
                //{
                //    exist.CountryDocuments.Add(o);
                //}
                //foreach (var o in country.CatCities)
                //{
                //    exist.CatCities.Add(o);
                //}
                exist.CountryLeaders.Clear();
                _context.SaveChanges();
                foreach (var leader in country.CountryLeaders)
                {
                    exist.CountryLeaders.Add(leader);
                }
                _context.SaveChanges();
            }
            return exist;
        }

        public int[] GetServiceLocationCountries(int id)
        {
            int[] countries = _context.ServiceLocationCountries.Where(x => x.IdServiceLocation == id )
                .Select(s => s.IdCountry).ToArray();
            return countries;
        }

        public ActionResult GetServicesLocationsCountries(int id)
        {
            var countries = _context.ServiceLocationCountries
                .Where(x => x.IdServiceLocation == id && x.StandarScopeDocuments.Equals(0))
                .Select(s => new
                {
                    s.Id,
                    s.IdCountry,
                    s.IdCountryNavigation.Name
                }).ToList();
            return new ObjectResult(countries);
        }

        public ActionResult GetUserList()
        {
            var users = _context.ProfileUsers.Where(x => x.Title == 2 || x.Title == 3).Select(s => new
            {
                id = s.UserId,
                Profile = s.TitleNavigation.Title,
                Name = $"{s.Name} {s.LastName} {s.MotherLastName}"
            }).ToList();
            return new ObjectResult(users);
        }

        public CountryDocumentGroup AddGroup(CountryDocumentGroup @group)
        {
            _context.CountryDocumentGroups.Add(@group);
            _context.SaveChanges();
            return @group;
        }

        public List<CatTypeResource> GetTypeResources()
        {
            return _context.CatTypeResources.ToList();
        }
    }
}
