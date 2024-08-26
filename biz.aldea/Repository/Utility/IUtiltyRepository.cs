using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace biz.premier.Repository.Utility
{
    public interface IUtiltyRepository : IGenericRepository<Entities.ServiceRecord>
    {
        string CompressString(string text);
        string DecompressString(string compressedText);
        string UploadImageBase64(string image, string ruta, string extension);
        Stream DeleteFile(string ruta);
        bool IsBase64(string base64String);
        ActionResult atributos_generales(int id_service, int type_sr);
        ActionResult change_sr_status_byservice_id(int id_service, int type_sr);
        ActionResult get_docs_scope_byservice_id(int id_service);
        int change_detail_status_byWos_id(int wos_id, int type_sr, int status_detail_id, int sr);

        string Get_url_email_images();

        string Get_url_email_images(string carpeta);
    }
}
