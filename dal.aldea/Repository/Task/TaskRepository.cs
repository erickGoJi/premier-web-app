using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using biz.premier.Repository.Task;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Task
{
    public class TaskRepository : GenericRepository<biz.premier.Entities.Task>, ITaskRepository
    {
        public TaskRepository(Db_PremierContext context) : base(context)
        {

        }

        public ActionResult ActionItems(int? user, DateTime? rangeDate1, DateTime? rangeDate2, bool? asignedToMeOrByMe, int? serviceLine)
        {
            var range1 = rangeDate1.HasValue ? rangeDate1.Value : DateTime.Today.AddDays(-20);
            var range2 = rangeDate2.HasValue ? rangeDate2.Value : DateTime.Today.AddDays(20);
            var pending = _context.Tasks.Where(x => (x.DueDate > range1 && x.DueDate < range2) && x.StatusId == 1).Select(s => new
            {
                s.Id,
                s.TaskDescription,
                s.ActionTypeNavigation.ActionType,
                actionTypeId = s.ActionType,
                fromId = s.TaskFrom,
                from = s.TaskFromNavigation.Name + " " + s.TaskFromNavigation.LastName + " " + s.TaskFromNavigation.MotherLastName,
                fromAvatar = s.TaskFromNavigation.Avatar,
                toId = s.TaskTo,
                to = s.TaskToNavigation.Name + " " + s.TaskToNavigation.LastName + " " + s.TaskToNavigation.MotherLastName,
                toAvatar = s.TaskToNavigation.Avatar,
                status = s.Status.Status,
                s.StatusId,
                s.CreatedDate,
                s.DueDate,
                s.ServiceRecord.NumberServiceRecord,
                s.DepartmentNavigation.Department,
                group = s.ColaboratorMembers.Any() ? true : false,
                s.Urgent,
                color = s.DueDate <= DateTime.Today.AddDays(3) && s.DueDate >= DateTime.Today ? "#00954e" // GREEN
                    : s.DueDate >= DateTime.Today.AddDays(-3) && s.DueDate <= DateTime.Today ? "#fbb03b" // YELLOW
                    : s.DueDate == DateTime.Today || s.DueDate <= DateTime.Today ? "#ff4141" // RED
                    : s.StatusId == 3 ? "#3f5be6" // BLUE
                    : "#ffffff",
                serviceLine = s.ActionType == 1 ? s.WorkOrder.ServiceLine.ServiceLine: "",
                serviceLineId = s.ActionType == 1 ? s.WorkOrder.ServiceLineId : 0
            }).ToList();
            var inProgress = _context.Tasks.Where(x => (x.DueDate > range1 && x.DueDate < range2) && x.StatusId == 2).Select(s => new
            {
                s.Id,
                s.TaskDescription,
                s.ActionTypeNavigation.ActionType,
                actionTypeId = s.ActionType,
                fromId = s.TaskFrom,
                from = s.TaskFromNavigation.Name + " " + s.TaskFromNavigation.LastName + " " + s.TaskFromNavigation.MotherLastName,
                fromAvatar = s.TaskFromNavigation.Avatar,
                toId = s.TaskTo,
                to = s.TaskToNavigation.Name + " " + s.TaskToNavigation.LastName + " " + s.TaskToNavigation.MotherLastName,
                toAvatar = s.TaskToNavigation.Avatar,
                status = s.Status.Status,
                s.StatusId,
                s.CreatedDate,
                s.DueDate,
                s.ServiceRecord.NumberServiceRecord,
                s.DepartmentNavigation.Department,
                group = s.ColaboratorMembers.Any() ? true : false,
                s.Urgent,
                color = s.DueDate <= DateTime.Today.AddDays(3) && s.DueDate >= DateTime.Today ? "#00954e" // GREEN
                    : s.DueDate >= DateTime.Today.AddDays(-3) && s.DueDate <= DateTime.Today ? "#fbb03b" // YELLOW
                    : s.DueDate == DateTime.Today || s.DueDate <= DateTime.Today ? "#ff4141" // RED
                    : s.StatusId == 3 ? "#3f5be6" // BLUE
                    : "#ffffff",
                serviceLine = s.ActionType == 1 ? s.WorkOrder.ServiceLine.ServiceLine: "",
                serviceLineId = s.ActionType == 1 ? s.WorkOrder.ServiceLineId : 0
            }).ToList();
            var done = _context.Tasks.Where(x => (x.DueDate > range1 && x.DueDate < range2) && x.StatusId == 3).Select(s => new
            {
                s.Id,
                s.TaskDescription,
                s.ActionTypeNavigation.ActionType,
                actionTypeId = s.ActionType,
                fromId = s.TaskFrom,
                from = s.TaskFromNavigation.Name + " " + s.TaskFromNavigation.LastName + " " + s.TaskFromNavigation.MotherLastName,
                fromAvatar = s.TaskFromNavigation.Avatar,
                toId = s.TaskTo,
                to = s.TaskToNavigation.Name + " " + s.TaskToNavigation.LastName + " " + s.TaskToNavigation.MotherLastName,
                toAvatar = s.TaskToNavigation.Avatar,
                status = s.Status.Status,
                s.StatusId,
                s.CreatedDate,
                s.CompletedDate,
                s.DueDate,
                s.ServiceRecord.NumberServiceRecord,
                s.DepartmentNavigation.Department,
                group = s.ColaboratorMembers.Any() ? true : false,
                s.Urgent,
                color = s.DueDate <= DateTime.Today.AddDays(3) && s.DueDate >= DateTime.Today ? "#00954e" // GREEN
                    : s.DueDate >= DateTime.Today.AddDays(-3) && s.DueDate <= DateTime.Today ? "#fbb03b" // YELLOW
                    : s.DueDate == DateTime.Today || s.DueDate <= DateTime.Today ? "#ff4141" // RED
                    : s.StatusId == 3 ? "#3f5be6" // BLUE
                    : "#ffffff",
                serviceLine = s.ActionType == 1 ? s.WorkOrder.ServiceLine.ServiceLine: "",
                serviceLineId = s.ActionType == 1 ? s.WorkOrder.ServiceLineId : 0
            }).ToList();
            
            //Query por Role
            int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;
            switch (role)
            {
                case 2: // Coordinator
                    pending = pending.Where(x => x.toId == user || x.fromId == user).ToList();
                    inProgress = inProgress.Where(x => x.toId == user || x.fromId == user).ToList();
                    done = done.Where(x => x.toId == user || x.fromId == user).ToList();
                    break;
                case 3: // Supplier
                    pending = pending.Where(x => x.toId == user || x.fromId == user).ToList();
                    inProgress = inProgress.Where(x => x.toId == user || x.fromId == user).ToList();
                    done = done.Where(x => x.toId == user || x.fromId == user).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            
            if (serviceLine.HasValue)
            {
                pending = pending.Where(x => x.serviceLineId == serviceLine.Value).ToList();
                inProgress = inProgress.Where(x => x.serviceLineId == serviceLine.Value).ToList();
                done = done.Where(x => x.serviceLineId == serviceLine.Value).ToList();
            }
            
            if (asignedToMeOrByMe.HasValue)
            {
                if (asignedToMeOrByMe.Value)
                {
                    pending = pending.Where(x => x.toId == user.Value).ToList();
                    inProgress = inProgress.Where(x => x.toId == user.Value).ToList();
                    done = done.Where(x => x.toId == user.Value).ToList();
                }
                else
                {
                    pending = pending.Where(x => x.fromId == user.Value).ToList();
                    inProgress = inProgress.Where(x => x.fromId == user.Value).ToList();
                    done = done.Where(x => x.fromId == user.Value).ToList();
                }
            }
            var actionList = new
            {
                pending = pending,
                inProgress = inProgress,
                done = done
            };
            return new ObjectResult(actionList);
        }

        public ActionResult AllActions(int user, int? sr, int? status, DateTime? rangeDate1, DateTime? rangeDate2, int? serviceLine)
        {
            var allActions = _context.Tasks.Select(s => new
            {
                s.Id,
                serviceRecordId = s.ServiceRecordId.HasValue ? s.ServiceRecordId : 0,
                serviceRecord = s.ServiceRecordId.HasValue ? s.ServiceRecord.NumberServiceRecord : s.DepartmentNavigation.Department,
                from = s.TaskFromNavigation.Name + " " + s.TaskFromNavigation.LastName + " " + s.TaskFromNavigation.MotherLastName,
                fromId = s.TaskFrom,
                fromAvatar = s.TaskFromNavigation.Avatar,
                to = s.TaskToNavigation.Name + " " + s.TaskToNavigation.LastName + " " + s.TaskToNavigation.MotherLastName,
                toId = s.TaskTo,
                toAvatar = s.TaskToNavigation.Avatar,
                s.Status.Status,
                s.StatusId,
                s.AssignedDate,
                s.DueDate,
                completedDate = s.CompletedDate.HasValue ? s.CompletedDate.Value : (DateTime?)null,
                s.ActionTypeNavigation.ActionType,
                serviceLine = s.ActionType == 1 ? s.WorkOrder.ServiceLine.ServiceLine: "",
                serviceLineId = s.ActionType == 1 ? s.WorkOrder.ServiceLineId : 0,
                actionTypeId = s.ActionType,
            }).ToList();
            
            //Query por Role
            int role = _context.Users.SingleOrDefault(x => x.Id == user).RoleId;
            switch (role)
            {
                case 2: // Coordinator
                    allActions = allActions.Where(x => x.toId == user || x.fromId == user).ToList();
                    break;
                case 3: // Supplier
                    allActions = allActions.Where(x => x.toId == user || x.fromId == user).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            
            if (sr.HasValue)
                allActions = allActions.Where(x => x.serviceRecordId == sr.Value).ToList();
            if (status.HasValue)
                allActions = allActions.Where(x => x.StatusId == status.Value).ToList();
            if (rangeDate1.HasValue && rangeDate2.HasValue)
                allActions = allActions.Where(x => x.DueDate > rangeDate1.Value && x.DueDate < rangeDate2.Value).ToList();
            if (serviceLine.HasValue)
                allActions = allActions.Where(x => x.serviceLineId == serviceLine.Value).ToList();
                
            return new ObjectResult(allActions);
        }

        public ActionResult GetAllTask(int service_record_id, int service_line_id)
        {
            var categories = _context.CatCategories;
            var services = _context.Tasks
                .Include(x => x.TaskFromNavigation)
                .Include(x => x.TaskToNavigation)
                .Include(i => i.Service).ThenInclude(i => i.BundledServices).ThenInclude(i => i.Category)
                .Include(i => i.Service).ThenInclude(i => i.StandaloneServiceWorkOrders).ThenInclude(i => i.Category)
                .Where(e => e.ServiceRecordId == service_record_id && e.ServiceLineId == service_line_id);
            var service = _context.Tasks
                .Include(x => x.TaskFromNavigation)
                .Include(x => x.TaskToNavigation)
                .Include(i => i.WorkOrder).ThenInclude(i => i.BundledServicesWorkOrders).ThenInclude(i => i.BundledServices).ThenInclude(i => i.Category)
                .Include(i => i.WorkOrder).ThenInclude(i => i.StandaloneServiceWorkOrders).ThenInclude(i => i.Category)
                .Where(e => e.ServiceRecordId == service_record_id && e.ServiceLineId == service_line_id)
                .Select(k => new
                {
                    k.Id,
                    TaskFrom = (k.TaskFromNavigation.Name + " " + k.TaskFromNavigation.LastName) == null ? k.TaskFromNavigation.Name : k.TaskFromNavigation.Name + " " + k.TaskFromNavigation.LastName,
                    AvatarTaskFrom = k.TaskFromNavigation.ProfileUsers.FirstOrDefault(x => x.UserId == k.TaskFrom),
                    TaskTo = (k.TaskToNavigation.Name + " " + k.TaskToNavigation.LastName) == null ? k.TaskToNavigation.Name : k.TaskToNavigation.Name + " " + k.TaskToNavigation.LastName,
                    AvatarTaskTo = k.TaskToNavigation.ProfileUsers.FirstOrDefault(x => x.UserId == k.TaskTo),
                    k.Status.Status,
                    k.AssignedDate,
                    k.DueDate,
                    CompletedDate = k.CompletedDate == null ? "" : k.CompletedDate.ToString(),
                    k.TaskDescription,
                    Service = k.Service.BundledServices.Any() ? categories.FirstOrDefault(x => x.Id == k.WorkOrder.BundledServicesWorkOrders.FirstOrDefault().BundledServices.FirstOrDefault().CategoryId.Value).Category
                        : categories.FirstOrDefault(x => x.Id == k.WorkOrder.StandaloneServiceWorkOrders.FirstOrDefault().CategoryId.Value).Category,
                    //service = k.Service.ServiceNumber + "/" + k.Service.Category.Category,
                    k.ServiceLine.ServiceLine,
                    document = k.TaskDocuments,
                    k.OverdueTask,
                    k.Urgent
                }).ToList();

            return new ObjectResult(service);
        }

        public biz.premier.Entities.Task GetCustom(int id)
        {
            if (id == 0)
                return null;
            var actionItme = _context.Tasks
                .Include(i => i.TaskDocuments)
                .Include(i => i.TaskFromNavigation)
                .Include(i => i.TaskToNavigation)
                .Include(i => i.DepartmentNavigation)
                .Include(i => i.ActionTypeNavigation)
                .Include(i => i.ServiceLine)
                .Include(i => i.ServiceRecord)
                .Include(i => i.WorkOrder).ThenInclude(i => i.StandaloneServiceWorkOrders)
                .Include(i => i.WorkOrder).ThenInclude(i => i.BundledServicesWorkOrders).ThenInclude(i => i.BundledServices)
                .Include(i => i.Status)
                .Include(i => i.ColaboratorMembers)
                    .ThenInclude(i => i.ColaboratorNavigation)
                .Include(i => i.TaskReplies)
                    .ThenInclude(i => i.CreatedByNavigation).ThenInclude(i => i.UserType)
                .SingleOrDefault(s => s.Id == id);
            return actionItme;
        }

        public ActionResult GetTaskById(int id)
        {
            var service = _context.Tasks
                .Where(e => e.Id == id)
                .Select(k => new
                {
                    k.Id,
                    k.ServiceRecordId,
                    TaskFromName = k.TaskFromNavigation.Name + " " + k.TaskFromNavigation.LastName,
                    k.TaskFrom,
                    TaskToName = k.TaskToNavigation.Name + " " + k.TaskToNavigation.LastName,
                    k.TaskTo,
                    k.Status.Status,
                    k.StatusId,
                    k.AssignedDate,
                    k.DueDate,
                    CompletedDate = k.CompletedDate == null ? "" : k.CompletedDate.ToString(),
                    k.ServiceId,
                    standalone = k.Service.BundledServices.Any() ? k.Service.BundledServices.Where(x => x.WorkServicesId == k.ServiceId).FirstOrDefault() :
                        null,
                    bundled = k.Service.StandaloneServiceWorkOrders.Any() ? k.Service.StandaloneServiceWorkOrders.Where(x => x.WorkOrderServiceId == k.ServiceId).FirstOrDefault() :
                        null,
                    k.ServiceLineId,
                    k.ServiceLine.ServiceLine,
                    k.Comments,
                    k.TaskDescription,
                    user_name_comment = _context.Users.FirstOrDefault(f => f.Id == k.CreatedBy).Name + " " + _context.Users.FirstOrDefault(f => f.Id == k.CreatedBy).LastName,
                    tittle_comment = _context.Users.FirstOrDefault(f => f.Id == k.CreatedBy).UserType.Type,
                    Reply = _context.TaskReplies
                        .Where(x => x.TaskId == k.Id)
                        .Select(g => new
                        {
                            user_name = _context.Users.FirstOrDefault(f => f.Id == g.CreatedBy).Name + " " + _context.Users.FirstOrDefault(f => f.Id == g.CreatedBy).LastName,
                            tittle = _context.Users.FirstOrDefault(f => f.Id == g.CreatedBy).UserType.Type,
                            g.Id,
                            g.TaskId,
                            g.Reply,
                            g.CreatedDate
                        }).ToList(),
                    document = k.TaskDocuments,
                    k.OverdueTask
                }).ToList();

            return new ObjectResult(service);
        }

        public ActionResult StatusTask()
        {
            var service = _context.TaskStatuses
                .Select(k => new
                {
                    k.Id,
                    k.Status
                }).OrderBy(x => x.Status).ToList();

            return new ObjectResult(service);
        }

        public biz.premier.Entities.Task UpdateCustom(biz.premier.Entities.Task task)
        {
            if (task == null)
                return null;
            var exist = _context.Tasks
                .Include(i => i.TaskDocuments)
                .Include(i => i.TaskFromNavigation)
                .Include(i => i.TaskToNavigation)
                .Include(i => i.DepartmentNavigation)
                .Include(i => i.ActionTypeNavigation)
                .Include(i => i.ServiceLine)
                .Include(i => i.ServiceRecord)
                .Include(i => i.WorkOrder).ThenInclude(i => i.StandaloneServiceWorkOrders)
                .Include(i => i.WorkOrder).ThenInclude(i => i.BundledServicesWorkOrders).ThenInclude(i => i.BundledServices)
                .Include(i => i.Status)
                .Include(i => i.ColaboratorMembers)
                    .ThenInclude(i => i.ColaboratorNavigation)
                .Include(i => i.TaskReplies)
                    .ThenInclude(i => i.CreatedByNavigation).ThenInclude(i => i.UserType)
                .SingleOrDefault(s => s.Id == task.Id);
            if(exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(task);
                foreach (var reply in task.TaskReplies)
                {
                    var replies = exist.TaskReplies.Where(i => i.Id == reply.Id).FirstOrDefault();
                    if (replies == null)
                    {
                        exist.TaskReplies.Add(reply);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(replies).CurrentValues.SetValues(reply);
                    }
                }

                foreach (var document in task.TaskDocuments)
                {
                    var documents = exist.TaskDocuments.Where(i => i.Id == document.Id).FirstOrDefault();
                    if (documents == null)
                    {
                        exist.TaskDocuments.Add(document);
                    }
                    else
                    {
                        _context.Entry(documents).CurrentValues.SetValues(document);
                    }
                }
                _context.SaveChanges();
            }
            return exist;
        }
    }
}
