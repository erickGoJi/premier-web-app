using System;

namespace api.premier.Models.Catalogos
{
    public class CountryDocumentDto
    {
        public string FileExtencion { get; set; }
        public int Id { get; set; }
        public int IdCountry { get; set; }
        public int IdDocumentType { get; set; }
        public string FileRequest { get; set; }
        public string FileName { get; set; }
        public bool? Group { get; set; }
        public int? IdGroup { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Location { get; set; }
        public int IdStatus { get; set; }
        public int IdPrivacy { get; set; }

        public virtual CountryDocumentGroupDto IdGroupNavigation { get; set; }
    }
    public class CountryDocumentGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}