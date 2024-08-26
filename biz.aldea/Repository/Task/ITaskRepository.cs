using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Task
{
    public interface ITaskRepository : IGenericRepository<biz.premier.Entities.Task>
    {
        ActionResult GetAllTask(int service_record_id, int service_line_id);
        ActionResult GetTaskById(int id);
        ActionResult StatusTask();
        ActionResult ActionItems(int? user, DateTime? rangeDate1, DateTime? rangeDate2, bool? asignedToMeOrByMe, int? serviceLine);
        ActionResult AllActions(int user, int? sr, int? status, DateTime? rangeDate1, DateTime? rangeDate2, int? serviceLine);
        Entities.Task GetCustom(int id);
        Entities.Task UpdateCustom(Entities.Task task);
    }
}
