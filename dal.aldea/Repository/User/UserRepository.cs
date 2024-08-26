using biz.premier.Entities;
using biz.premier.Repository;
using dal.premier.DBContext;
using CryptoHelper;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using biz.premier.Models.Email;
using biz.premier.Servicies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;

        public UserRepository(Db_PremierContext context, IConfiguration config, IEmailService emailService) : base(context)
        {
            _config = config;
            _emailservice = emailService;
        }

        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public string BuildToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                // Issuer = ,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public bool VerifyPassword(string hash, string password)
        {
            return Crypto.VerifyHashedPassword(hash, password);
        }

        public override User Add(User user)
        {
            user.Password = HashPassword(user.Password);
            return base.Add(user);
        }

        public override User Update(User user, object id)
        {
            user.UpdatedDate = DateTime.Now;
            return base.Update(user, id);
        }

        public string SendMail(string emailTo, string body, string subject)
        {
            EmailModel email = new EmailModel();
            email.To = emailTo;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;

            return _emailservice.SendEmail(email);

        }

        public ActionResult get()
        {
            var item = (from a in _context.WorkOrders
                        select new
                        {
                            a.Id,
                            standalone = a.StandaloneServiceWorkOrders.Where(x => x.ServiceTypeId == 2).ToList(),
                            buncled = a.BundledServicesWorkOrders.ToList()
                        });
            return new ObjectResult(item);
        }

        public ActionResult userList(int key, int[] users, int department)
        {
            int roleDepartment = department == 1 ? 11 : department == 2 ? 1 : 0;
            int[] roles = new[] { 1, 2, 3 };
            var query = _context.Set<User>().Where(x =>
                x.Id != key
                && !users.Contains(x.Id))
                .Select(s => new
                {
                    s.Id,
                    name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                    immigration = s.ProfileUsers.FirstOrDefault().Immigration,
                    relocation = s.ProfileUsers.FirstOrDefault().Relocation,
                    role = s.RoleId
                }).ToList();
            if (department != 0)
            {
                query = query.Where(x => x.role == roleDepartment).ToList();
            }
            else if (department == 0)
            {
                query = query.Where(x => roles.Contains(x.role)).ToList();
            }
            return new ObjectResult(query);
        }

        public string VerifyEmail(string email)
        {
            var result = "";

            if (_context.Users.SingleOrDefault(x => x.Email.ToLower().Trim() == email.ToLower().Trim()) != null)
            {
                result = "Exist";
            }
            else
            {
                result = "No Exist";
            }

            return result;
        }

        public ActionResult GetCustom(int?[] role)
        {
            var usersList = _context.ProfileUsers.Where(x => x.User.Status == true || !x.User.Status.HasValue).Select(s => new UserData()
            {
                Id = s.Id,
                UserId = s.UserId,
                Name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                Office = s.ResponsablePremierOfficeNavigation.Office,
                Country = s.CountryNavigation.Name,
                Title = s.TitleNavigation.Title,
                Role = s.User.Role.Role,
                CreatedDate = s.CreatedDate,
                RoleId = s.User.RoleId,
                Email = s.User.Email
            }).ToList();

            var usersAssignee = _context.AssigneeInformations.Where(x => x.User.Status == true || !x.User.Status.HasValue && x.UserId.HasValue)
                .Select(s => new UserData()
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Name = s.AssigneeName,
                    Office = "-",
                    Country = s.HostCountryNavigation.Name,
                    Title = "-",
                    Role = s.User.Role.Role,
                    CreatedDate = s.CreatedDate,
                    RoleId = s.User.RoleId,
                    Email = s.User.Email
                }).ToList();
            var users = usersList.Union(usersAssignee).ToList();
            var usersReal = new List<UserData>();
            int[] rolesNotIn = new[] { 1, 2, 3, 4 };
            if (role.Any())
            {
                foreach (var i in role)
                {
                    if (i != null)
                        switch (i.Value)
                        {
                            case 1:
                                usersReal.AddRange(users.Where(x => x.RoleId == 1 || x.RoleId == 2).ToList());
                                break;
                            case 2:
                                usersReal.AddRange(users.Where(x => x.RoleId == 3).ToList());
                                break;
                            case 3:
                                usersReal.AddRange(users.Where(x => !rolesNotIn.Contains(x.RoleId.Value)).ToList());
                                break;
                            case 4:
                                usersReal.AddRange(users.Where(x => x.RoleId == 4).ToList());
                                break;
                            case 5:
                                usersReal.AddRange(users.Where(x => x.RoleId == 1).ToList());
                                break;
                            case 6:
                                usersReal.AddRange(users.Where(x => x.RoleId == 5).ToList());
                                break;
                            default:
                                usersReal.AddRange(users.ToList());
                                break;
                        }
                }
            }
            else
            {
                usersReal = users.ToList();
            }
            return new ObjectResult(usersReal);
        }

        public ActionResult GetCustomNew(int?[] role)
        {
            var usersList = _context.ProfileUsers.Where(x => x.User.Status == true || !x.User.Status.HasValue).Select(s => new UserData()
            {
                Id = s.Id,
                UserId = s.UserId,
                Name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                Office = s.ResponsablePremierOfficeNavigation.Office,
                Country = s.CountryNavigation.Name,
                Title = s.TitleNavigation.Title,
                Role = s.User.Role.Role,
                CreatedDate = s.CreatedDate,
                RoleId = s.User.RoleId,
                Email = s.User.Email
            }).ToList();

            var usersAssignee = _context.AssigneeInformations.Where(x => x.User.Status == true || !x.User.Status.HasValue && x.UserId.HasValue)
                .Select(s => new UserData()
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Name = s.AssigneeName,
                    Office = "-",
                    Country = s.HostCountryNavigation.Name,
                    Title = "-",
                    Role = s.User.Role.Role,
                    CreatedDate = s.CreatedDate,
                    RoleId = s.User.RoleId,
                    Email = s.User.Email
                }).ToList();
            var users = usersList.Union(usersAssignee).ToList();
            var usersReal = new List<UserData>();
             if(role.Count() > 0)
            {
                usersReal.AddRange(users.Where(x => role.Contains(x.RoleId)).ToList());
            }
           else
            {
                usersReal = users.ToList();
            }
                

            //int[] rolesNotIn = new[] { 1, 2, 3, 4 };
            //if (role.Any())
            //{
            //    foreach (var i in role)
            //    {
            //        if (i != null)
            //            switch (i.Value)
            //            {
            //                case 1:
            //                    usersReal.AddRange(users.Where(x => x.RoleId == 1 || x.RoleId == 2).ToList());
            //                    break;
            //                case 2:
            //                    usersReal.AddRange(users.Where(x => x.RoleId == 3).ToList());
            //                    break;
            //                case 3:
            //                    usersReal.AddRange(users.Where(x => !rolesNotIn.Contains(x.RoleId.Value)).ToList());
            //                    break;
            //                case 4:
            //                    usersReal.AddRange(users.Where(x => x.RoleId == 4).ToList());
            //                    break;
            //                case 5:
            //                    usersReal.AddRange(users.Where(x => x.RoleId == 1).ToList());
            //                    break;
            //                case 6:
            //                    usersReal.AddRange(users.Where(x => x.RoleId == 5).ToList());
            //                    break;
            //                default:
            //                    usersReal.AddRange(users.ToList());
            //                    break;
            //            }
            //    }
            //}
            //else
            //{
            //    usersReal = users.ToList();
            //}
            return new ObjectResult(usersReal);
        }

        private class UserData
        {
            public int Id { get; set; }
            public int? UserId { get; set; }
            public string Name { get; set; }
            public string Office { get; set; }
            public string Country { get; set; }
            public string Title { get; set; }
            public string Role { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int? RoleId { get; set; }

            public string Email { get; set; }
        }

        public ActionResult GetUsersInactive()
        {
            var usersList = _context.ProfileUsers.Where(x => x.User.Status == false).Select(s => new
            {
                s.Id,
                s.UserId,
                name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                office = s.ResponsablePremierOfficeNavigation.Office,
                country = s.CountryNavigation.Name,
                title = s.TitleNavigation.Title,
                s.User.Role.Role,
                s.CreatedDate,
                s.User.RoleId
            }).ToList();

            var usersAssignee = _context.AssigneeInformations.Where(x => x.User.Status == false).Select(s => new
            {
                s.Id,
                s.UserId,
                name = s.AssigneeName,
                office = "-",
                country = s.HostCountryNavigation.Name,
                title = "-",
                s.User.Role.Role,
                s.CreatedDate,
                s.User.RoleId
            }).ToList();
            var users = usersList.Union(usersAssignee).ToList();

            return new ObjectResult(users);
        }

        public ActionResult GetUserData(int key)
        {
            var usersList = _context.ProfileUsers.Select(s => new
            {
                s.Id,
                s.Photo,
                s.UserId,
                s.Name,
                s.LastName,
                s.MotherLastName,
                s.Country,
                s.City,
                office = s.ResponsablePremierOffice,
                s.Title,
                role = s.User.RoleId,
                phone = s.PhoneNumber,
                s.User.Email,
                s.CreatedDate
            }).SingleOrDefault(s => s.Id == key);

            var _userList = _context.AssigneeInformations.Select(s => new
            {
                s.Id,
                s.User.Avatar,
                UserId = s.UserId,
                s.User.Name,
                s.User.LastName,
                s.User.MotherLastName,
                Country = s.HomeCountryId,
                City = s.HomeCityId,
                office = "N/A",
                Title = "N/A",
                role = s.User.RoleId,
                phone = s.MobilePhone,
                s.User.Email,
                s.User.CreatedDate

            }).SingleOrDefault(s => s.Id == key);

            if (usersList == null)
            {
                return new ObjectResult(_userList);
            }
            else
            {
                return new ObjectResult(usersList);
            }
                       
        }

        public string GetClientName(int key)
        {
            var client = _context.ServiceRecords.Where(x => x.AssigneeInformations.FirstOrDefault().Id == key)
                .Select(s => s.Client.Name).FirstOrDefault();
            return client;
        }

        public CatRole GetRole(int key)
        {
            var role = _context.CatRoles
                .Include(x => x.Permissions)
                .SingleOrDefault(x => x.Id == key);
            return role;
        }

        public List<int> GetOperationalLeaders(int sr)
        {
            List<int> leaders;
            leaders = _context.OperationLeaders
                .Where(x => x.ConsultantNavigation.ImmigrationCoodinators.Any(a => a.ServiceRecordId == sr)
                            || x.ConsultantNavigation.RelocationCoordinators.Any(a => a.ServiceRecordId == sr))
                .Select(s => s.CreatedBy).ToList();
            return leaders;
        }

        public List<int> GetCountryLeaders(int countryHost, int countryHome)
        {
            var countryLeaders = _context.CountryLeaders
                .Where(x => x.Country == countryHome || x.Country == countryHost)
                .Select(s => s.Leader).ToList();
            return countryLeaders;
        }

        public List<biz.premier.Entities.ProfileUser> GetUserProfile(int user)
        {
            var _ProfileUsers = _context.ProfileUsers
                 .Include(x => x.CountryNavigation)
                 .Include(x => x.CityNavigation)
                 .Include(x => x.TitleNavigation)
                 .Where(x => x.UserId == user)
                 .ToList();

            return _ProfileUsers;
        }

        public List<biz.premier.Entities.AssigneeInformation> GetAssigneInfo(int user)
        {
            var _ProfileUsers = _context.AssigneeInformations
                 .Include(x => x.HostCountryNavigation)
                 .Include(x => x.HostCity)
                 .Where(x => x.UserId == user)
                 .ToList();

            return _ProfileUsers;
        }
        public string GetCountryToCountry(int idCountry, int idCity)
        {
            var nameCountry = _context.Countries.FirstOrDefault(x => x.Id == idCountry).Name;

            var nameCity = _context.Cities.FirstOrDefault(x => x.Id == idCity).Name;

            var name_Country = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == nameCountry.ToLower()) == null ? nameCountry.Substring(0,3).ToString()
                : _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == nameCountry.ToLower()).Sortname;

            var name_City = _context.CatCities.FirstOrDefault(x => x.City.ToLower() == nameCity.ToLower()) == null ? nameCity.Substring(0, 3).ToString()
                : _context.CatCities.FirstOrDefault(x => x.City.ToLower() == nameCity.ToLower()).Sortname;

            return name_Country + "/"+ name_City;
        }
    }
}
