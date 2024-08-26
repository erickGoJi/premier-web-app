using api.premier.Models.Catalogos;
using System;

namespace api.premier.Models.ClientPartnerProfile
{
    public class DocumentClientPartnerProfileDto
    {
        public int Id { get; set; }
        public int IdClientPartnerProfile { get; set; }
        public int IdDocumentType { get; set; }
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public bool Privacy { get; set; }
        public string FileName { get; set; }
        public string FileRequest { get; set; }
        public string FileExtension { get; set; }
        
        public virtual ClientPartnerProfileDto IdClientPartnerProfileNavigation { get; set; }
        public virtual CatDocumentTypeDto IdDocumentTypeNavigation { get; set; }
    }
}