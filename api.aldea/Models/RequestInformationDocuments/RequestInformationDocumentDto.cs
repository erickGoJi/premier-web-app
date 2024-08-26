using api.premier.Models.RequestInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.RequestInformationDocuments
{
    public class RequestInformationDocumentDto
    {
        public int Id { get; set; }
        public int? DocumentType { get; set; }
        public int? RelationshipId { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int? RequestInformationId { get; set; }
        public virtual RequestInformationDto RequestInformation { get; set; }

    }
}
