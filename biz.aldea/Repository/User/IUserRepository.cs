using System;
using System.Collections.Generic;
using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        string HashPassword(string password);
        bool VerifyPassword(string hash, string password);
        string BuildToken(User user);
        string SendMail(string emailTo, string body, string subject);
        ActionResult get();
        ActionResult userList(int key, int[] users, int department);
        string VerifyEmail(string email);
        ActionResult GetCustom(int?[] role);

        ActionResult GetCustomNew(int?[] role);
        ActionResult GetUsersInactive();
        ActionResult GetUserData(int key);
        string GetClientName(int key);
        CatRole GetRole(int key);
        List<int> GetOperationalLeaders(int sr);
        List<int> GetCountryLeaders(int countryHost, int countryHome);
        List<biz.premier.Entities.ProfileUser> GetUserProfile(int user);
        List<biz.premier.Entities.AssigneeInformation> GetAssigneInfo(int user);
        string GetCountryToCountry(int idCountry, int idCity);
    }
}
