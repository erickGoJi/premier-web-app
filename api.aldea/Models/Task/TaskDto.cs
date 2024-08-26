using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Task
{
    public class TaskDto
    {
        public TaskDto()
        {
            TaskDocuments = new HashSet<TaskDocumentDto>();
        }

        public int Id { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? WorkOrderId { get; set; }
        public int? TaskFrom { get; set; }
        public int TaskTo { get; set; }
        public int[] to { get; set; }
        public int StatusId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? ServiceId { get; set; }
        public int? ServiceLineId { get; set; }
        public string Comments { get; set; }
        public string ActionTitle { get; set; }
        public string TaskDescription { get; set; }
        public bool? OverdueTask { get; set; }
        public int? Department { get; set; }
        public int? ActionType { get; set; }
        public bool? Urgent { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<ColaboratorMemberDto> ColaboratorMembers { get; set; }
        public virtual ICollection<TaskDocumentDto> TaskDocuments { get; set; }
        public virtual ICollection<TaskReplyDto> TaskReplies { get; set; }
    }
    public class TaskSelectDto
    {
        public int Id { get; set; }
        public int? ServiceRecordId { get; set; }
        public int? WorkOrderId { get; set; }
        public int? TaskFrom { get; set; }
        public int TaskTo { get; set; }
        public int StatusId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int? ServiceLineId { get; set; }
        public string Comments { get; set; }
        public string ActionTitle { get; set; }
        public string TaskDescription { get; set; }
        public bool? OverdueTask { get; set; }
        public int? Department { get; set; }
        public int? ActionType { get; set; }
        public bool? Urgent { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual CatActionTypeDto ActionTypeNavigation { get; set; }
        public virtual CatDepartmentDto DepartmentNavigation { get; set; }
        public virtual ServiceRecordTaskDto ServiceRecord { get; set; }
        public virtual CatServiceLineDto ServiceLine { get; set; }
        public virtual TaskStatusDto Status { get; set; }
        public virtual UserDto TaskFromNavigation { get; set; }
        public virtual UserDto TaskToNavigation { get; set; }
        public virtual WorkOrderTaskDto WorkOrder { get; set; }
        public virtual ICollection<ColaboratorMemberDto> ColaboratorMembers { get; set; }
        public virtual ICollection<TaskDocumentDto> TaskDocuments { get; set; }
        public virtual ICollection<TaskReplySelectDto> TaskReplies { get; set; }
    }
    public class WorkOrderTaskDto
    {
        public int Id { get; set; }
        public string NumberWorkOrder { get; set; }
    }
    public class ServiceRecordTaskDto
    {
        public int Id { get; set; }
        public string NumberServiceRecord { get; set; }
    }
}
