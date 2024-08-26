using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace biz.premier.Repository.SchoolingSearch
{
    public interface ISchoolingSearchRepository : IGenericRepository<Entities.SchoolingSearch>
    {
         Entities.SchoolingSearch GetCustom(int key); 

        ActionResult GetCustomList(int key);
        Entities.SchoolingSearch GetSchoolsList(int key);
        Entities.SchoolingSearch UpdateCustom(Entities.SchoolingSearch schoolingSearch);

        Entities.SchoolsList GetSchoolsListsingle(int key);

        List<Entities.SchoolingInformation> get_si(int Key);

        bool deleteSchool(int key);
        List<int> UpdateSendSchools(List<int> list);
        List<biz.premier.Entities.DependentInformation> GetDependent(int key);
    }
}
