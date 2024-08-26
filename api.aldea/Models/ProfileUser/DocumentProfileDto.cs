using api.premier.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ProfileUser
{
    public class DocumentProfileDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        public int? DocumentType { get; set; }
        public DateTime? UploadedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Location { get; set; }
        public int? Status { get; set; }
        public bool? Privacy { get; set; }
    }
    public class DocumentProfileSelectDto
    {
        public int Id { get; set; }
        public int? Profile { get; set; }
        public string FileRequest { get; set; }
        public int? DocumentType { get; set; }
        public DateTime? UploadedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Location { get; set; }
        public int? Status { get; set; }
        public bool? Privacy { get; set; }
        public virtual CatDocumentTypeDto DocumentTypeNavigation { get; set; }
        public virtual PersonalInformationDto ProfileNavigation { get; set; }
        public virtual CatDocumentStatusDto StatusNavigation { get; set; }
    }
}
