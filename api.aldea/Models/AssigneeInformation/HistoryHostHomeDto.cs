using System;

namespace api.premier.Models.AssigneeInformation
{
    public class HistoryHostHomeDto
    {
        public int Id { get; set; }
        public int AssigneeInformationId { get; set; }
        public int CityHomeId { get; set; }
        public int CountryHomeId { get; set; }
        public int CityHostId { get; set; }
        public int CountryHostId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AssigneeInformationDto AssigneeInformation { get; set; }
    }
}