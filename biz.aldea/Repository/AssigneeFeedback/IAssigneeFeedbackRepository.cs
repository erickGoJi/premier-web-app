using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.AssigneeFeedback
{
    public interface IAssigneeFeedbackRepository : IGenericRepository<Entities.AssigneeFeedback>
    {
        IActionResult GetAllCustom(int pageNumber, int pageSize);
        int TotalFeed();
    }
}
