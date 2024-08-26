using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.AssignedServiceSuplier
{
    public class AssignedServiceSuplierDto
    {
        public int Id { get; set; }
        public int? ImmigrationSupplierPartnerId { get; set; }
        public int? RelocationSupplierPartnerId { get; set; }
        public int? ServiceOrderServicesId { get; set; }
        public string? service { get; set; }
    }

    public class AssignedServiceSuplierSrDto
    {
        public List<AssignedServiceSuplierDto> lista { get; set; }
        public int srid { get; set; }
    }

    public class assignedservices
    {
        public bool assignee { get; set; }
        public DateTime creationDate { get; set; }
        public int id { get; set; }
        public int idsuplier { get; set; }
        public string service { get; set; }

        public string serviceNumber { get; set; }

        public int? statusId { get; set; }

        public int supplierId { get; set; }

        public int value { get; set; }
    }

    public class ServiceSuplierSrDto
    {
        public List<assignedservices> lista { get; set; }
        public int srid { get; set; }
    }



}
