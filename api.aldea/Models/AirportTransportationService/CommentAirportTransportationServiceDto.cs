using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AirportTransportationService
{
    public class CommentAirportTransportationServiceDto
    {
        public int Id { get; set; }
        public int? AirportTransportationServicesId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AirportTransportationServiceDto AirportTransportationServices { get; set; }
        //public virtual User User { get; set; }
    }
    public class CommentAirportTransportationServiceSelectDto
    {
        public int Id { get; set; }
        public int? AirportTransportationServicesId { get; set; }
        public string Reply { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AirportTransportationServiceDto AirportTransportationServices { get; set; }
        public virtual UserDto User { get; set; }
    }
}
