using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.TemporaryHousingCoordinaton
{
    public class CommentTemporaryHosuingDto
    {
        public int Id { get; set; }
        public int? TemporaryHousingCoordinationId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TemporaryHousingCoordinatonDto TemporaryHousingCoordination { get; set; }
    }
    public class CommentTemporaryHosuingSelectDto
    {
        public int Id { get; set; }
        public int? TemporaryHousingCoordinationId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual TemporaryHousingCoordinatonDto TemporaryHousingCoordination { get; set; }
        public virtual UserDto User { get; set; }
    }
}
