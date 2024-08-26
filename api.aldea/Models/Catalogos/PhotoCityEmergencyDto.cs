using System;

namespace api.premier.Models.Catalogue
{
    public class PhotoCityEmergencyDto
    {
        public int Id { get; set; }
        public int IdEmergencyAbout { get; set; }
        public string FileName { get; set; }
        public string FileExtencion { get; set; }
        public string FileRequest { get; set; }
        public DateTime CreateDate { get; set; }
        public int? TypeId { get; set; }
    }
}