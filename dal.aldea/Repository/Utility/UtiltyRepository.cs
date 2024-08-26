using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Utility
{
    public class UtiltyRepository : GenericRepository<biz.premier.Entities.ServiceRecord>, IUtiltyRepository
    {
        public UtiltyRepository(Db_PremierContext context) :
            base(context)
        { }


        public string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public string UploadImageBase64(string image, string ruta, string _extension)
        {
            //string ruta;

            var filePath = Environment.CurrentDirectory;
            string[] extensions = new[] { "msword", "document", "template", "12", "ms-excel", "sheet", "ms-powerpoint", "presentation", "slideshow" };
            if (extensions.Contains(_extension))
            {
                switch (_extension)
                {
                    case ("msword"):
                        _extension = "doc";
                        break;
                    case ("document"):
                        _extension = "docx";
                        break;
                    case ("template"):
                        _extension = "potx";
                        break;
                    case ("12"):
                        _extension = "docm";
                        break;
                    case ("ms-excel"):
                        _extension = "xls";
                        break;
                    case ("sheet"):
                        _extension = "xlsx";
                        break;
                    case ("ms-powerpoint"):
                        _extension = "ppt";
                        break;
                    case ("presentation"):
                        _extension = "pptx";
                        break;
                    case ("slideshow"):
                        _extension = "ppsx";
                        break;
                    default:
                        _extension = _extension;
                        break;
                }
            }
            var extension = _extension;
            var _guid = Guid.NewGuid();
            var path = ruta + _guid + "." + extension;

            var bytes = Convert.FromBase64String(image);
            using (var imageFile = new FileStream(filePath + "/" + path, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            //ruta = selRuta.Url + _guid + "." + extension;
            return path;
        }

        public Stream DeleteFile(string ruta)
        {
            var filePath = Environment.CurrentDirectory;
            Stream retVal = null;

            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                retVal = new MemoryStream(fileData);
                File.Delete(filePath);
            }
            return retVal;
        }

        public bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
               || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception exception)
            {
                // Handle the exception
            }
            return false;
        }

        public string Get_url_email_images()
        {
            //var url = "http://34.237.214.147/back/api_premier_qa/Files/newmail/";
            var url = "https://my.premierds.com/api-test-premier/Files/newmail/";
            return url;
        }

        public string Get_url_email_images(string carpeta)
        {
            // var url = "http://34.237.214.147/back/api_premier_qa/Files/" + carpeta + "/";
            var url = "https://my.premierds.com/api-test-premier/Files/" + carpeta + "/";
            return url;
        }

        public ActionResult atributos_generales(int id_service, int type_sr)
        {
            var wos_id = 0;


            switch (type_sr)
            {
                case 17://Pre-Decision Orientation
                    wos_id = _context.PredecisionOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 18://Area Orientation
                    wos_id = _context.AreaOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 19://Settling In
                    wos_id = _context.SettlingIns.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 20://Schooling Search
                    wos_id = _context.SchoolingSearches.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 21://Departure
                    wos_id = _context.Departures.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 22://Temporary Housing Coordinaton
                    wos_id = _context.TemporaryHousingCoordinatons.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 23://Rental Furniture Coordination
                    wos_id = _context.RentalFurnitureCoordinations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 24://Transportation
                    wos_id = _context.Transportations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 25://Airport Transportation Services
                    wos_id = _context.AirportTransportationServices.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 26://Home Finding
                    wos_id = _context.HomeFindings.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 27://Lease Renewal
                    wos_id = _context.LeaseRenewals.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 28://Home Sale
                    wos_id = _context.HomeSales.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 29://Home Purchase
                    wos_id = _context.HomePurchases.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 30://Property Management
                    wos_id = _context.PropertyManagements.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 32://Tenancy Management
                    wos_id = _context.TenancyManagements.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 31://Other
                    wos_id = _context.Others.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;
            }

            if (wos_id != 0)
            {
                var standalone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderServiceId == wos_id)
                    .Select(s => new
                    {
                        country = s.DeliveringIn,
                        country_name = s.DeliveringInNavigation.Name,
                        location = s.Location,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.WorkOrderId,
                        wo_number = s.WorkOrder.NumberWorkOrder,
                        service_number = s.ServiceNumber,
                        partner_id = s.WorkOrder.ServiceRecord.PartnerId,
                        sr_id = s.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(a => a.Id == s.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = type_sr //cat_service table 
                        ,cat_service_id = s.ServiceId
                        ,cat_category_id = s.CategoryId
                    }).ToList();

                if (standalone.Count > 0)
                {
                    return new ObjectResult(standalone);
                }
                else
                {
                    var bundled = _context.BundledServices
                    .Where(x => x.WorkServicesId == wos_id)
                    .Select(s => new
                    {
                        country = s.DeliveringIn,
                        country_name = s.DeliveringInNavigation.Name,
                        partner_id = s.BundledServiceOrder.WorkOrder.ServiceRecord.PartnerId,
                        type = s.ServiceId, //stand alone 
                        wos_id = wos_id,
                        location = s.Location,
                        service_number = s.ServiceNumber,
                        wo_id = s.BundledServiceOrder.WorkOrder.Id,
                        wo_number = s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(j => j.Id == s.BundledServiceOrder.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = type_sr //cat_service table 
                        ,cat_service_id = s.ServiceId
                        ,cat_category_id = s.CategoryId
                    }).ToList();

                    if (bundled.Count > 0)
                    {
                        return new ObjectResult(bundled);
                    }
                }
                return new ObjectResult(null);
            }
            else
            {
                return new ObjectResult(null);
            }


        }

        public ActionResult atributos_generales_by_wosid(int wos_id)
        {
          

            if (wos_id != 0)
            {
                var standalone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderServiceId == wos_id)
                    .Select(s => new
                    {
                        country = s.DeliveringIn,
                        country_name = s.DeliveringInNavigation.Name,
                        location = s.Location,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.WorkOrderId,
                        wo_number = s.WorkOrder.NumberWorkOrder,
                        service_number = s.ServiceNumber,
                        partner_id = s.WorkOrder.ServiceRecord.PartnerId,
                        sr_id = s.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(a => a.Id == s.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = s.ServiceId //cat_service table 
                        , cat_service_id = s.ServiceId
                        , cat_category_id = s.CategoryId
                    }).ToList();

                if (standalone.Count > 0)
                {
                    return new ObjectResult(standalone);
                }
                else
                {
                    var bundled = _context.BundledServices
                    .Where(x => x.WorkServicesId == wos_id)
                    .Select(s => new
                    {
                        country = s.DeliveringIn,
                        country_name = s.DeliveringInNavigation.Name,
                        partner_id = s.BundledServiceOrder.WorkOrder.ServiceRecord.PartnerId,
                        type = s.ServiceId, //stand alone 
                        wos_id = wos_id,
                        location = s.Location,
                        service_number = s.ServiceNumber,
                        wo_id = s.BundledServiceOrder.WorkOrder.Id,
                        wo_number = s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                        sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(j => j.Id == s.BundledServiceOrder.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = s.ServiceId //cat_service table 
                        , cat_service_id = s.ServiceId
                        , cat_category_id = s.CategoryId
                    }).ToList();

                    if (bundled.Count > 0)
                    {
                        return new ObjectResult(bundled);
                    }
                }
                return new ObjectResult(null);
            }
            else
            {
                return new ObjectResult(null);
            }


        }

        public ActionResult change_sr_status_byservice_id(int id_service, int type_sr)
        {
            var wos_id = 0;
            var status_detail_id = 0; 
            switch (type_sr)
            {
                case 17://Pre-Decision Orientation
                    wos_id = _context.PredecisionOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.PredecisionOrientations.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 18://Area Orientation
                    wos_id = _context.AreaOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.AreaOrientations.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 19://Settling In
                    wos_id = _context.SettlingIns.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.SettlingIns.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 20://Schooling Search
                    wos_id = _context.SchoolingSearches.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.SchoolingSearches.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 21://Departure
                    wos_id = _context.Departures.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.Departures.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 22://Temporary Housing Coordinaton
                    wos_id = _context.TemporaryHousingCoordinatons.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.TemporaryHousingCoordinatons.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 23://Rental Furniture Coordination
                    wos_id = _context.RentalFurnitureCoordinations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.RentalFurnitureCoordinations.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 24://Transportation
                    wos_id = _context.Transportations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.Transportations.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 25://Airport Transportation Services
                    wos_id = _context.AirportTransportationServices.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = _context.AirportTransportationServices.FirstOrDefault(s => s.Id == id_service).StatusId;
                    break;

                case 26://Home Finding
                    wos_id = _context.HomeFindings.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    status_detail_id = (int)_context.HomeFindings.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;

                case 27://Lease Renewal
                    wos_id = _context.LeaseRenewals.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    status_detail_id = _context.LeaseRenewals.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;

                case 28://Home Sale
                    wos_id = _context.HomeSales.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    status_detail_id = _context.HomeSales.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;

                case 29://Home Purchase
                    wos_id = _context.HomePurchases.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    status_detail_id = _context.HomePurchases.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;

                case 30://Property Management
                    wos_id = _context.PropertyManagements.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    status_detail_id = _context.PropertyManagements.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;

                case 32://Tenancy Management
                    wos_id = _context.TenancyManagements.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    status_detail_id = _context.TenancyManagements.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;

                case 31://Other
                    wos_id = _context.Others.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    status_detail_id = _context.Others.FirstOrDefault(s => s.Id == id_service).StatusId.Value;
                    break;
            }

            if (wos_id != 0)
            {
                int country = 0;
                int service_type = 0;
                int assa_id = 0;
                int wo_id = 0;
                int sr_id = 0;
                var standalone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderServiceId == wos_id)
                    .Select(s => new
                    {
                        country = s.DeliveringIn,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.WorkOrderId,
                        sr_id = s.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id
                    }).ToList();

                if (standalone.Count > 0)
                {
                    country = standalone.FirstOrDefault().country.Value;
                    service_type = standalone.FirstOrDefault().type.Value;
                    assa_id = standalone.FirstOrDefault().ass_id;
                    wo_id = standalone.FirstOrDefault().wo_id == null ? 0 : standalone.FirstOrDefault().wo_id.Value;
                    sr_id = (int)(standalone.FirstOrDefault().sr_id);

                    var stand = _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.WorkOrderServiceId == wos_id);
                    stand.StatusId = status_detail_id;
                    _context.StandaloneServiceWorkOrders.Update(stand);
                    _context.SaveChanges();
                }
                else
                {
                    var bundled = _context.BundledServices
                    .Where(x => x.WorkServicesId == wos_id)
                    .Select(s => new
                    {
                        country = s.DeliveringIn,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.BundledServiceOrderId,
                        sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id

                    }).ToList();

                    if (bundled.Count > 0)
                    {
                        country = bundled.FirstOrDefault().country.Value;
                        service_type = bundled.FirstOrDefault().type.Value;
                        assa_id = bundled.FirstOrDefault().ass_id;
                        wo_id = bundled.FirstOrDefault().wo_id == null ? 0 : bundled.FirstOrDefault().wo_id.Value;
                        sr_id = (int)(bundled.FirstOrDefault().sr_id);
                    }

                    var bund = _context.BundledServices.FirstOrDefault(x => x.WorkServicesId == wos_id);
                    bund.StatusId = status_detail_id;
                    _context.BundledServices.Update(bund);
                    _context.SaveChanges();

                }

                var sr_ = _context.ServiceRecords.FirstOrDefault(s => s.Id == sr_id);
                if (sr_.StatusId < 3)
                {
                    if (status_detail_id != 1 && status_detail_id != 38) // Si el estatus seteado no es pending accpet o asign 
                    {
                        sr_.StatusId = 3;
                        var new_status = _context.ServiceRecords.Update(sr_);
                        _context.SaveChanges();
                    }

                }

                return new ObjectResult(sr_.StatusId);
            }
            else
            {
                return new ObjectResult(null);
            }


        }

        public int change_detail_status_byWos_id(int wos_id, int type_sr, int status_detail_id, int sr)
        {
            switch (type_sr)
            {
                case 17://Pre-Decision Orientation
                    var d = _context.PredecisionOrientations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d.StatusId = status_detail_id;
                    _context.PredecisionOrientations.Update(d);
                    _context.SaveChanges();
                    break;

                case 18://Area Orientation
                    var d1 = _context.AreaOrientations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d1.StatusId = status_detail_id;
                    _context.AreaOrientations.Update(d1);
                    _context.SaveChanges();
                    break;

                case 19://Settling In
                    var d2 = _context.SettlingIns.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d2.StatusId = status_detail_id;
                    _context.SettlingIns.Update(d2);
                    _context.SaveChanges();

                    break;

                case 20://Schooling Search
                    var d3 = _context.SchoolingSearches.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d3.StatusId = status_detail_id;
                    _context.SchoolingSearches.Update(d3);
                    _context.SaveChanges();
                    break;

                case 21://Departure
                    var d4 = _context.Departures.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d4.StatusId = status_detail_id;
                    _context.Departures.Update(d4);
                    _context.SaveChanges();
                    break;

                case 22://Temporary Housing Coordinaton
                    var d5 = _context.TemporaryHousingCoordinatons.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d5.StatusId = status_detail_id;
                    _context.TemporaryHousingCoordinatons.Update(d5);
                    _context.SaveChanges();
                    break;

                case 23://Rental Furniture Coordination
                    var d6 = _context.RentalFurnitureCoordinations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d6.StatusId = status_detail_id;
                    _context.RentalFurnitureCoordinations.Update(d6);
                    _context.SaveChanges();
                    break;

                case 24://Transportation
                    var d7 = _context.Transportations.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d7.StatusId = status_detail_id;
                    _context.Transportations.Update(d7);
                    _context.SaveChanges();
                    break;

                case 25://Airport Transportation Services
                    var d8 = _context.AirportTransportationServices.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d8.StatusId = status_detail_id;
                    _context.AirportTransportationServices.Update(d8);
                    _context.SaveChanges();
                    break;

                case 26://Home Finding
                    var d9 = _context.HomeFindings.FirstOrDefault(s => s.WorkOrderServicesId == wos_id);
                    d9.StatusId = status_detail_id;
                    _context.HomeFindings.Update(d9);
                    _context.SaveChanges();
                    break;

                case 27://Lease Renewal
                    var d11 = _context.LeaseRenewals.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d11.StatusId = status_detail_id;
                    _context.LeaseRenewals.Update(d11);
                    _context.SaveChanges();
                    break;

                case 28://Home Sale
                    var d22 = _context.HomeSales.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d22.StatusId = status_detail_id;
                    _context.HomeSales.Update(d22);
                    _context.SaveChanges();
                    break;

                case 29://Home Purchase
                    var d33 = _context.HomePurchases.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d33.StatusId = status_detail_id;
                    _context.HomePurchases.Update(d33);
                    _context.SaveChanges();
                    break;

                case 30://Property Management
                    var d44 = _context.PropertyManagements.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d44.StatusId = status_detail_id;
                    _context.PropertyManagements.Update(d44);
                    _context.SaveChanges();
                    break;

                case 32://Tenancy Management
                    var d55 = _context.TenancyManagements.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d55.StatusId = status_detail_id;
                    _context.TenancyManagements.Update(d55);
                    _context.SaveChanges();
                    break;

                case 31://Other
                    var d66 = _context.Others.FirstOrDefault(s => s.WorkOrderServices == wos_id);
                    d66.StatusId = status_detail_id;
                    _context.Others.Update(d66);
                    _context.SaveChanges();
                    break;
            }

            var workOrder = _context.WorkOrders
                       .Where(x => x.ServiceRecordId == sr).ToList();


            var SRData = _context.ServiceRecords.SingleOrDefault(x => x.Id == sr);

            bool isStatus = false;

            for (int a = 0; a < workOrder.Count(); a++)
            {
                var standAlone = workOrder[a].StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderId == workOrder[a].Id).ToList();

                var bundle = workOrder[a].BundledServicesWorkOrders
                    .Where(x => x.WorkOrderId == workOrder[a].Id).ToList();

                if (standAlone != null)
                {
                    for (int x = 0; x < standAlone.Count(); x++)
                    {
                        if (standAlone[x].StatusId == status_detail_id)
                        {
                            isStatus = true;
                        }
                    }
                }

                if (bundle != null)
                {

                    for (int y = 0; y < bundle.Count(); y++)
                    {
                        for (int z = 0; z < bundle[y].BundledServices.Count(); z++)
                        {
                            if (bundle[y].BundledServices.FirstOrDefault().StatusId == status_detail_id)
                            {
                                isStatus = true;
                            }
                        }
                    }
                }

                if (isStatus)
                {
                    switch (status_detail_id)
                    {
                        case 38: //Pending To Accept

                            if (SRData != null)
                            {
                                if(SRData.StatusId == 1)
                                {
                                    SRData.StatusId = 18;
                                    _context.ServiceRecords.Update(SRData);
                                }
                            }
                            break;

                        case 39: //Active
                            if (SRData != null)
                            {
                                SRData.StatusId = 2;
                                _context.ServiceRecords.Update(SRData);
                            }
                            break;
                    }
                }
                else
                {
                    if (SRData != null)
                    {
                        SRData.StatusId = 1;
                        _context.ServiceRecords.Update(SRData);
                    }
                }


                //if (wos_id != 0)
                //{
                //    int country = 0;
                //    int service_type = 0;
                //    int assa_id = 0;
                //    int wo_id = 0;
                //    int sr_id = 0;
                //    var standalone = _context.StandaloneServiceWorkOrders
                //        .Where(x => x.WorkOrderServiceId == wos_id)
                //        .Select(s => new
                //        {
                //            country = s.DeliveringIn,
                //            type = s.ServiceId,
                //            wos_id = wos_id,
                //            wo_id = s.WorkOrderId,
                //            sr_id = s.WorkOrder.ServiceRecordId,
                //            ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id
                //        }).ToList();

                //    if (standalone.Count > 0)
                //    {
                //        country = standalone.FirstOrDefault().country.Value;
                //        service_type = standalone.FirstOrDefault().type.Value;
                //        assa_id = standalone.FirstOrDefault().ass_id;
                //        wo_id = standalone.FirstOrDefault().wo_id == null ? 0 : standalone.FirstOrDefault().wo_id.Value;
                //        sr_id = (int)(standalone.FirstOrDefault().sr_id);

                //    }
                //    else
                //    {
                //        var bundled = _context.BundledServices
                //        .Where(x => x.WorkServicesId == wos_id)
                //        .Select(s => new
                //        {
                //            country = s.DeliveringIn,
                //            type = s.ServiceId,
                //            wos_id = wos_id,
                //            wo_id = s.BundledServiceOrderId,
                //            sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                //            ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id

                //        }).ToList();

                //        if (bundled.Count > 0)
                //        {
                //            country = bundled.FirstOrDefault().country.Value;
                //            service_type = bundled.FirstOrDefault().type.Value;
                //            assa_id = bundled.FirstOrDefault().ass_id;
                //            wo_id = bundled.FirstOrDefault().wo_id == null ? 0 : bundled.FirstOrDefault().wo_id.Value;
                //            sr_id = (int)(bundled.FirstOrDefault().sr_id);
                //        }


                //    }

                //    /////////////// MIJOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                //    /// ACA PUEDES CAMBIAR EL STATUS DE LAS SR SI LO NECESITAS 


                //    var sr_ = _context.ServiceRecords.FirstOrDefault(s => s.Id == sr_id);

                //    //if (sr_.StatusId < 3)
                //    //{
                //    //    if (status_detail_id != 1 && status_detail_id != 38) // Si el estatus seteado no es pending accpet o asign 
                //    //    {
                //    //        sr_.StatusId = 3;
                //    //        var new_status = _context.ServiceRecords.Update(sr_);
                //    //        _context.SaveChanges();
                //    //    }

                //    //}

                //    return new ObjectResult(sr_.StatusId);
                //}
                //else
                //{
                //    return new ObjectResult(null);
                //}


            }
            return (status_detail_id);
        }

        public ActionResult get_docs_scope_byservice_id(int id_service)
        {
            var wos_id = _context.SchoolingSearches.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
            int country = 0;
            int service_type = 0;
            int assa_id = 0;
            int wo_id = 0;
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId,
                    wos_id = wos_id,
                    wo_id = s.WorkOrderId,
                    sr_id = s.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id
                }).ToList();

            if (standalone.Count > 0)
            {
                country = standalone.FirstOrDefault().country.Value;
                service_type = standalone.FirstOrDefault().type.Value;
                assa_id = standalone.FirstOrDefault().ass_id;
                wo_id = standalone.FirstOrDefault().wo_id == null ? 0 : standalone.FirstOrDefault().wo_id.Value;
                return new ObjectResult(standalone);
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId,
                    wos_id = wos_id,
                    wo_id = s.BundledServiceOrderId,
                    sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id
                }).ToList();

                if (bundled.Count > 0)
                {
                    return new ObjectResult(country);
                }
            }
            return new ObjectResult(null);
        }

    }

      
}
