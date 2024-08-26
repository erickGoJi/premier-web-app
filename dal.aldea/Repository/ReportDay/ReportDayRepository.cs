using biz.premier.Models;
using biz.premier.Repository.ReportDay;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using biz.premier.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Column = DocumentFormat.OpenXml.Spreadsheet.Column;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;

namespace dal.premier.Repository.ReportDay
{
    public class ReportDayRepository : GenericRepository<biz.premier.Entities.ReportDay>, IReportDayRepository
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ReportDayRepository(Db_PremierContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public int CountByServiceRecord(int key)
        { 
            if (key == 0)
                return 0;
            var consult = _context.ReportDays.Where(x => x.WorkOrderNavigation.ServiceRecordId == key).Count();
            return consult;
        }

        public List<biz.premier.Entities.ReportDay> GetReportDay(int sr, int? serviceLine, int? program, DateTime? initialReportDate, DateTime? finalReportDate, int? totalTimeAuthorized)
        {
            if (sr == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.ReportDay>()
                .Include(i => i.ServiceReportDays)
                .Include(i => i.WorkOrderNavigation)
                    .ThenInclude(i => i.BundledServicesWorkOrders)
                    .ThenInclude(i => i.BundledServices)
                    .ThenInclude(i => i.Service)
                .Include(i => i.WorkOrderNavigation)
                    .ThenInclude(i => i.StandaloneServiceWorkOrders)
                    .ThenInclude(i => i.Service)
                .Include(i => i.ServiceLineNavigation)
                .Include(i => i.ReportByNavigation)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.CountryNavigation)
                .Include(i => i.WorkOrderNavigation)
                    .ThenInclude(i => i.ServiceRecord)
                    .ThenInclude(i => i.Partner)
                .Where(s => s.WorkOrderNavigation.ServiceRecordId == sr).ToList();
            if (serviceLine.HasValue)
                exist = exist.Where(x => x.WorkOrderNavigation.ServiceLineId == serviceLine.Value).ToList();
            if (program.HasValue)
                exist = program.Value == 1 ? exist.Where(x => x.WorkOrderNavigation.BundledServicesWorkOrders.Any() && !x.WorkOrderNavigation.StandaloneServiceWorkOrders.Any()).ToList() 
                    : exist.Where(x => !x.WorkOrderNavigation.BundledServicesWorkOrders.Any() && x.WorkOrderNavigation.StandaloneServiceWorkOrders.Any()).ToList();
            if (totalTimeAuthorized.HasValue)
                exist = exist.Where(x => x.TotalTime == totalTimeAuthorized.Value.ToString()).ToList();
            if (initialReportDate.HasValue && finalReportDate.HasValue)
                exist = exist.Where(x => x.ReportDate >= initialReportDate.Value && x.ReportDate <= finalReportDate.Value).ToList();
            return exist;
        }

        public string GetServiceNameByWorOrder(int idService, int PartnerId)
        {
            var serviceRecord = _context.StandaloneServiceWorkOrders.Any(x => x.Id == idService)
                ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == idService).ServiceId
                : _context.BundledServices.FirstOrDefault(x => x.Id == idService).ServiceId;

            var nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == PartnerId
            && f.IdService == serviceRecord).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == PartnerId
            && f.IdService == serviceRecord).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == PartnerId
           && f.IdService == serviceRecord).NickName;

            return nickName;
        }

        public int GetActivityReportsByService(int service, int workorder)
        {
            var view = _context.ServiceReportDays.Any(x => x.ReportDay.WorkOrder == workorder && x.Service == service)
                 ? Convert.ToInt32(_context.ServiceReportDays.Select(c => new { 
                    c.Service,
                    c.ReportDay.WorkOrder,
                    c.Id,
                    c.TimeReminder,
                    c.Time
                 }).Where(x => x.WorkOrder == workorder && x.Service == service).Sum(c => Convert.ToInt32(c.Time)))
                 : GetTimeRemaining(service, workorder).TimeRemining;

            return view;
        }

        public int GetTimeRemaindingPublic(int service)
        {
            var view = _context.ServiceReportDays.Any(x => x.Service == service)
                 ? _context.ServiceReportDays.OrderByDescending(x => x.Id).FirstOrDefault(d => d.Service == service).TimeReminder.Value
                 : (_context.StandaloneServiceWorkOrders.Any(x => x.Id == service) 
                    ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == service).AuthoTime
                    : Convert.ToInt32(_context.BundledServices.FirstOrDefault(x => x.Id == service).BundledServiceOrder.TotalTime));

            return view.Value;
        }

        public ReportSummary GetTimeRemaining(int service, int workOrder)
        {
            ReportSummary reportSummary = new ReportSummary();
            var timeRemining = _context.WorkOrders.Where(x => x.Id == workOrder).Select(s => new
            {
                s.Id,
                service = _context.StandaloneServiceWorkOrders.Where(x => x.Id == service).Select(_s => _s.Service.Service).FirstOrDefault(),
                serviceName = _context.StandaloneServiceWorkOrders.Where(x => x.Id == service).Select(_s => _s.ServiceNumber).FirstOrDefault(),
                timeRemining = _context.StandaloneServiceWorkOrders.Where(x => x.Id == service).Select(_s => _s.AuthoTime).FirstOrDefault(),
                reportStatus = "Saved"
            }).FirstOrDefault();

            var timeReminingBundled = _context.WorkOrders.Where(x => x.Id == workOrder).Select(s => new
            {
                s.Id,
                service = _context.BundledServicesWorkOrders.Select(_s => _s.BundledServices.Where(x => x.Id == service).Select(q => q.Service.Service).FirstOrDefault()).FirstOrDefault(),
                serviceName = _context.BundledServicesWorkOrders.Where(x => x.BundledServices.Select(q=>q.Id).Contains(service)).Select(_s => _s.BundledServices.Where(x => x.Id == service).Select(q => q.ServiceNumber).FirstOrDefault()).FirstOrDefault(),
                timeRemining = _context.BundledServicesWorkOrders.Where(x => x.BundledServices.Select(q=>q.Id).Contains(service)).Select(_s => Convert.ToInt32(_s.TotalTime)).FirstOrDefault(),
                reportStatus = "Saved"
            }).FirstOrDefault();

            reportSummary.Saved = timeRemining.reportStatus;
            reportSummary.Service = timeRemining.timeRemining.HasValue ? timeRemining.service : timeReminingBundled.service;
            reportSummary.ServiceId = timeRemining.timeRemining.HasValue ? timeRemining.serviceName : timeReminingBundled.serviceName;
            reportSummary.TimeRemining = timeRemining.timeRemining.HasValue ? timeRemining.timeRemining.Value : timeReminingBundled.timeRemining;
            return reportSummary;
        }

        public Tuple<string, string> GetNumber(int workOrder, int service)
        {
            Tuple<string, string> tuple = new Tuple<string, string>("","");
            var bundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrderId == workOrder && x.Id == service)
                .Select(s => new Tuple<string, string>(
                        s.BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
                        $"{s.ServiceNumber} / {s.Category.Category}"
                    )
                ).FirstOrDefault();
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderId == workOrder && x.Id == service)
                .Select(s => new Tuple<string, string>(
                        s.WorkOrder.ServiceLine.ServiceLine,
                        $"{s.ServiceNumber} / {s.Category.Category}"
                    )
                ).FirstOrDefault();
            tuple = bundled != null ? bundled : standalone != null ? standalone : new Tuple<string, string>("","");
            return tuple;
        }
        
        public int GetCategory(int workOrder, int service)
        {
            int category = 0;
            var bundled = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrderId == workOrder && x.Id == service)
                .Select(s => new Tuple<string, int>(
                        s.BundledServiceOrder.WorkOrder.ServiceLine.ServiceLine,
                        s.CategoryId.Value
                    )
                ).FirstOrDefault();
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderId == workOrder && x.Id == service)
                .Select(s => new Tuple<string, int>(
                        s.WorkOrder.ServiceLine.ServiceLine,
                        s.CategoryId.Value
                    )
                ).FirstOrDefault();
            category = bundled != null ? bundled.Item2 : standalone != null ? standalone.Item2 : 0;
            return category;
        }

        public ConclusionServiceReportDay AddConclusion(ConclusionServiceReportDay day)
        {
            _context.ConclusionServiceReportDays.Add(day);
            _context.SaveChanges();
            return day;
        }

        public bool DeleteConclusion(int key)
        {
            bool isSuccess = false;
            var find = _context.ConclusionServiceReportDays.Find(key);
            if (find != null)
            {
                _context.ConclusionServiceReportDays.Remove(find);
                _context.SaveChanges();
                isSuccess = true;
            }

            return isSuccess;
        }

        public biz.premier.Entities.ReportDay UpdateCustom(biz.premier.Entities.ReportDay reportDay, int key)
        {
            if (reportDay == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.ReportDay>()
                .Include(i => i.ServiceReportDays)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(reportDay);
                foreach (var i in reportDay.ServiceReportDays)
                {
                    var service = exist.ServiceReportDays.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (service == null)
                    {
                        exist.ServiceReportDays.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(service).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            return exist;
        }

        public ActionResult GetTotalesActivityReports(int sr)
        {
            int standaloneRemaining = 0;
            var standalone = _context.ServiceReportDays
                //.Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                .Join(_context.ReportDays, da => da.ReportDayId, ser => ser.Id, 
                (da, ser) => new
                {
                    da.Id,
                    da.Service
                })
                .Join(_context.StandaloneServiceWorkOrders, rep => rep.Service, sta => sta.Id,
                (rep, sta) => new
                {
                    sta.WorkOrder.ServiceRecordId,
                    sta.WorkOrder.ServiceLineId,
                    sta.AuthoTime,
                    rep.Service
                })
                .Where(c => c.ServiceRecordId == sr && c.ServiceLineId == 2).ToList();

            var standaloneAuthoTime = 0;
            int _serviceSelect = 0;
            List<int> _service = new List<int>();

            for (int i = 0; i < standalone.Count(); i++)
            {
                if(i>0)
                {
                    _service.Add(standalone[i].Service.Value);
                    for (int j = 0; j < _service.Count(); j++)
                    {
                        if(_serviceSelect != _service[j])
                        {
                            standaloneAuthoTime += standalone[i].AuthoTime.Value;
                        }
                    }
                }
                else
                {
                    _serviceSelect = standalone[i].Service.Value;
                    standaloneAuthoTime += standalone[i].AuthoTime.Value; //standalone.Sum(x => x.AuthoTime);
                }

            }

            var _countService = _context.ServiceReportDays
                .Where(x => x.ReportDay.WorkOrderNavigation.ServiceRecordId == sr).ToList();

            var services = _countService.Select(x => x.Service).Distinct().ToList();

            for (int i = 0; services.Count() > i; i++)
            {
                standaloneRemaining += _countService.Where(x => x.Service == services[i].Value).OrderByDescending(x => x.Id).FirstOrDefault().TimeReminder.Value;
            }

            int bundleRemaining = 0;
            var bundle = _context.ServiceReportDays
                //.Join(_context.AssignedServiceSupliers, stand => stand.WorkOrderServiceId, assig => assig.ServiceOrderServicesId,
                .Join(_context.ReportDays, da => da.ReportDayId, ser => ser.Id,
                (da, ser) => new
                {
                    da.Id,
                    da.Service
                })
                .Join(_context.BundledServicesWorkOrders, rep => rep.Service, sta => sta.Id,
                (rep, sta) => new
                {
                    sta.WorkOrder.ServiceRecordId,
                    sta.WorkOrder.ServiceLineId,
                    sta.TotalTime
                })
                .Where(c => c.ServiceRecordId == sr && c.ServiceLineId == 2).ToList();
            //_context.BundledServicesWorkOrders.Where(c => c.WorkOrder.ServiceRecordId == sr && c.WorkOrder.ServiceLineId == 2).ToList();
            int bundleAuthoTime = bundle.Sum(x => Convert.ToInt32(x.TotalTime));

            //for (int i = 0; services.Count() > i; i++)
            //{
            //    bundleRemaining += _countService.Where(x => x.Service == services[i].Value).OrderByDescending(x => x.Id).FirstOrDefault().TimeReminder.Value;
            //    //bundleAuthoTime += Convert.ToInt32(bundle[i].TotalTime);
            //}

            return new ObjectResult(new { totalTime = standaloneAuthoTime + bundleAuthoTime, timeRemaining = standaloneRemaining });
        }

        private string getServicios(int idService,int PartnerId)
        {
            var serviceRecord = _context.StandaloneServiceWorkOrders.Any(x => x.Id == idService)
                ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == idService).ServiceId
                : _context.BundledServices.FirstOrDefault(x => x.Id == idService).ServiceId;

            var nickName = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == PartnerId
            && f.IdService == serviceRecord).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == PartnerId
            && f.IdService == serviceRecord).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == PartnerId
           && f.IdService == serviceRecord).NickName;
            return nickName;
        }

        public void createExcelReportDay(int sr, string path)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart wbp = document.AddWorkbookPart();
                WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
                Workbook wb = new Workbook();
                FileVersion fv = new FileVersion();
                fv.ApplicationName = "Microsoft Office Excel";
                Worksheet ws = new Worksheet();
                SheetData sd = new SheetData();

                WorkbookStylesPart wbsp = wbp.AddNewPart<WorkbookStylesPart>();
                wbsp.Stylesheet = GenerateStyleSheet();
                wbsp.Stylesheet.Save();

                #region Imagen
                string xImage = _hostingEnvironment.ContentRootPath;
                string sImagePath = Path.Combine(xImage, "premier.png");
                DrawingsPart dp = wsp.AddNewPart<DrawingsPart>();
                ImagePart imgp = dp.AddImagePart(ImagePartType.Png, wsp.GetIdOfPart(dp));
                using (FileStream fs = new FileStream(sImagePath, FileMode.Open))
                {
                    imgp.FeedData(fs);
                }

                NonVisualDrawingProperties nvdp = new NonVisualDrawingProperties();
                nvdp.Id = 1025;
                nvdp.Name = "Picture 1";
                nvdp.Description = "polymathlogo";
                DocumentFormat.OpenXml.Drawing.PictureLocks picLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks();
                picLocks.NoChangeAspect = true;
                picLocks.NoChangeArrowheads = true;
                NonVisualPictureDrawingProperties nvpdp = new NonVisualPictureDrawingProperties();
                nvpdp.PictureLocks = picLocks;
                NonVisualPictureProperties nvpp = new NonVisualPictureProperties();
                nvpp.NonVisualDrawingProperties = nvdp;
                nvpp.NonVisualPictureDrawingProperties = nvpdp;

                DocumentFormat.OpenXml.Drawing.Stretch stretch = new DocumentFormat.OpenXml.Drawing.Stretch();
                stretch.FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle();

                BlipFill blipFill = new BlipFill();
                DocumentFormat.OpenXml.Drawing.Blip blip = new DocumentFormat.OpenXml.Drawing.Blip();
                blip.Embed = dp.GetIdOfPart(imgp);
                blip.CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print;
                blipFill.Blip = blip;
                blipFill.SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle();
                blipFill.Append(stretch);

                DocumentFormat.OpenXml.Drawing.Transform2D t2d = new DocumentFormat.OpenXml.Drawing.Transform2D();
                DocumentFormat.OpenXml.Drawing.Offset offset = new DocumentFormat.OpenXml.Drawing.Offset();
                offset.X = 0;
                offset.Y = 0;
                t2d.Offset = offset;
                Bitmap bm = new Bitmap(sImagePath);
                DocumentFormat.OpenXml.Drawing.Extents extents = new DocumentFormat.OpenXml.Drawing.Extents();
                extents.Cx = (long)bm.Width * (long)((float)914400 / bm.HorizontalResolution);
                extents.Cy = (long)bm.Height * (long)((float)914400 / bm.VerticalResolution);
                bm.Dispose();
                t2d.Extents = extents;
                ShapeProperties sp = new ShapeProperties();
                sp.BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto;
                sp.Transform2D = t2d;
                DocumentFormat.OpenXml.Drawing.PresetGeometry prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry();
                prstGeom.Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle;
                prstGeom.AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList();
                sp.Append(prstGeom);
                sp.Append(new DocumentFormat.OpenXml.Drawing.NoFill());

                DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture();
                picture.NonVisualPictureProperties = nvpp;
                picture.BlipFill = blipFill;
                picture.ShapeProperties = sp;

                Position pos = new Position();
                pos.X = 0;
                pos.Y = 0;
                Extent ext = new Extent();
                ext.Cx = extents.Cx;
                ext.Cy = extents.Cy;
                AbsoluteAnchor anchor = new AbsoluteAnchor();
                anchor.Position = pos;
                anchor.Extent = ext;
                anchor.Append(picture);
                anchor.Append(new ClientData());
                WorksheetDrawing wsd = new WorksheetDrawing();
                wsd.Append(anchor);
                Drawing drawing = new Drawing();
                drawing.Id = dp.GetIdOfPart(imgp);

                wsd.Save(dp);
                #endregion

                DocumentFormat.OpenXml.Spreadsheet.Row headerRow00 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow01 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow02 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow03 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow04_1 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow04_2 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                DocumentFormat.OpenXml.Spreadsheet.Row headerRow04 = new DocumentFormat.OpenXml.Spreadsheet.Row();


                #region Headers
                creaCelda("A7","Date",14,headerRow04);
                creaCelda("B7", "Service Line", 14, headerRow04);
                creaCelda("C7", "Work Order", 14, headerRow04);
                creaCelda("D7", "Services", 14, headerRow04);
                creaCelda("E7", "Reporte By", 14, headerRow04);
                creaCelda("F7", "Time Used", 14, headerRow04);
                #endregion

                #region Datos
                var result = _context.Set<biz.premier.Entities.ReportDay>()
                    .Include(i => i.ServiceReportDays)
                    .Include(i => i.WorkOrderNavigation)
                        .ThenInclude(i => i.BundledServicesWorkOrders)
                        .ThenInclude(i => i.BundledServices)
                        .ThenInclude(i => i.Service)
                    .Include(i => i.WorkOrderNavigation)
                        .ThenInclude(i => i.StandaloneServiceWorkOrders)
                        .ThenInclude(i => i.Service)
                    .Include(i => i.ServiceLineNavigation)
                    .Include(i => i.ReportByNavigation)
                        .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.CountryNavigation)
                    .Include(i => i.WorkOrderNavigation)
                        .ThenInclude(i => i.ServiceRecord)
                        .ThenInclude(i => i.Partner)
                    .Where(s => s.WorkOrderNavigation.ServiceRecordId == sr).Select(s => new
                    {
                        s.Id,
                        reprortedBy = s.ReportByNavigation.Name + " " + s.ReportByNavigation.LastName + " " + s.ReportByNavigation.MotherLastName,
                        s.CreationDate,
                        s.ReportDate,
                        s.ServiceLineNavigation.ServiceLine,
                        s.WorkOrderNavigation.NumberWorkOrder,
                        s.WorkOrderNavigation.ServiceRecord.PartnerId,
                        services = s.ServiceReportDays.Select(c => new
                        {
                            serviceRecord = _context.StandaloneServiceWorkOrders.Any(x => x.Id == c.Service.Value)
                                            ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId
                                            : _context.BundledServices.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId,

                            Service = _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
            && f.IdService == (_context.StandaloneServiceWorkOrders.Any(x => x.Id == c.Service.Value) ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId : _context.BundledServices.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId)).NickName == "--" ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
            && f.IdService == (_context.StandaloneServiceWorkOrders.Any(x => x.Id == c.Service.Value) ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId : _context.BundledServices.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId)).IdServiceNavigation.Service : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == s.WorkOrderNavigation.ServiceRecord.ClientId
           && f.IdService == (_context.StandaloneServiceWorkOrders.Any(x => x.Id == c.Service.Value) ? _context.StandaloneServiceWorkOrders.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId : _context.BundledServices.FirstOrDefault(x => x.Id == c.Service.Value).ServiceId)).NickName,
                          
                        }).ToList(),
                        s.TotalTime,
                    }).OrderByDescending(x => x.CreationDate).ToList();
                    
                sd.Append(headerRow00);
                sd.Append(headerRow01);
                sd.Append(headerRow02);
                sd.Append(headerRow03);
                sd.Append(headerRow04_1);
                sd.Append(headerRow04_2);
                sd.Append(headerRow04);

                int contador = 8;
                foreach (var item in result)
                {
                    if (item.services.Count > 0)
                    {
                        foreach (var servicio in item.services)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row headerRow05 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            creaCelda($"A{contador}", item.CreationDate.HasValue == false ? "" : item.CreationDate.GetValueOrDefault().ToString("dd/MM/yyyy"), 14, headerRow05);
                            creaCelda($"B{contador}", item.ServiceLine, 14, headerRow05);
                            creaCelda($"C{contador}", item.NumberWorkOrder, 14, headerRow05);
                            creaCelda($"D{contador}", servicio.Service, 14, headerRow05);
                            creaCelda($"E{contador}", item.reprortedBy, 14, headerRow05);
                            creaCelda($"F{contador}", item.TotalTime, 14, headerRow05);
                            contador++;
                            sd.Append(headerRow05);
                        }
                    }
                    else
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow05 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        creaCelda($"A{contador}", item.CreationDate.HasValue == false ? "" : item.CreationDate.GetValueOrDefault().ToString("dd/MM/yyyy"), 14, headerRow05);
                        creaCelda($"B{contador}", item.ServiceLine, 14, headerRow05);
                        creaCelda($"C{contador}", item.NumberWorkOrder, 14, headerRow05);
                        creaCelda($"D{contador}", "", 14, headerRow05);
                        creaCelda($"E{contador}", item.reprortedBy, 14, headerRow05);
                        creaCelda($"F{contador}", item.TotalTime, 14, headerRow05);
                        contador++;
                        sd.Append(headerRow05);
                    }

                }

                #endregion              

                Columns columns = new Columns();
                Column column00;
                column00 = new Column();
                column00.Min = 4;
                column00.Max = 4;
                column00.Width = 20;
                column00.CustomWidth = true;
                columns.Append(column00);

                Column column01;
                column01 = new Column();
                column01.Min = 8;
                column01.Max = 8;
                column01.Width = 20;
                column01.CustomWidth = true;
                columns.Append(column01);

                Column column02;
                column02 = new Column();
                column02.Min = 13;
                column02.Max = 13;
                column02.Width = 20;
                column02.CustomWidth = true;
                columns.Append(column02);

                ws.Append(columns);
                ws.Append(sd);
                ws.Append(drawing);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();
                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Activity Reports";
                sheet.SheetId = 1;
                sheet.Id = wbp.GetIdOfPart(wsp);
                sheets.Append(sheet);
                wb.Append(fv);
                wb.Append(sheets);

                document.WorkbookPart.Workbook = wb;
                document.WorkbookPart.Workbook.Save();
                document.Close();

            }
        }

        private void creaCelda(string referencia, string valor, UInt32Value style, Row fila )
        {
            DocumentFormat.OpenXml.Spreadsheet.Cell cell04 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
            cell04.CellReference = referencia;
            cell04.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
            cell04.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(valor);
            cell04.StyleIndex = style;
            fila.Append(cell04);
        }

        private Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
            new Fonts(
            new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 0 - The default font.
            new FontSize() { Val = 16 },
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
            new FontName() { Val = "Calibri" },
            new Bold()),
            new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 1 - The Italic font.
            new Bold(),
            new FontSize() { Val = 14 },
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
            new FontName() { Val = "Calibri" }),
            new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 2 - The Times Roman font. with 16 size
            new FontSize() { Val = 11 },
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
            new FontName() { Val = "Arial" },
            new Bold()),
            new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 3 - Arial Black
            new FontSize() { Val = 10 },
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
            new FontName() { Val = "Arial Black" },
            new Bold()),
            new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 4 - The Times Roman font. with 16 size
            new FontSize() { Val = 11 },
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
            new FontName() { Val = "Arial" }
            )
            ),
            new Fills(
            new Fill( // Index 0 - The default fill.
            new PatternFill() { PatternType = PatternValues.None }),
            new Fill( // Index 1 - The default fill of gray 125 (required)
            new PatternFill() { PatternType = PatternValues.Gray125 }),
            new Fill( // Index 2 - AZUL.
            new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "00BFFF" } }) { PatternType = PatternValues.Solid }),
            new Fill( // Index 3 - BLACK.
            new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "000000" } }) { PatternType = PatternValues.Solid }),
            new Fill( // Index 4 - Yellow.
            new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFF76" } }) { PatternType = PatternValues.Solid }),
            new Fill( // Index 5 - Gris.
            new PatternFill(new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "BBBBBB" } }) { PatternType = PatternValues.Solid })
            ),
            new Borders(
            new Border( // Index 0 - The default border.
            new LeftBorder(),
            new RightBorder(),
            new TopBorder(),
            new BottomBorder(),
            new DiagonalBorder()),
            new Border( // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
            new LeftBorder(
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }
            )
            { Style = BorderStyleValues.Thin },
            new RightBorder(
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }
            )
            { Style = BorderStyleValues.Thin },
            new TopBorder(
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }
            )
            { Style = BorderStyleValues.Thin },
            new BottomBorder(
            new DocumentFormat.OpenXml.Spreadsheet.Color() { Auto = true }
            )
            { Style = BorderStyleValues.Thin })
            ),
            new CellFormats(
            new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 }, // Index 0 - The default cell style. If a cell does not have a style index applied it will use this style combination instead
            new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 1 - Bold
            new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 2 - Italic
            new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 3 - Times Roman
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true, ApplyFont = true }, // Index 4 - Azul Cabecera
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 1, FillId = 3, BorderId = 0, ApplyFill = true, ApplyFont = true }, // Index 5 - Negro Cabecera
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 0, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 6 - Azul Cabecera
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 4, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 7 - Amarillo Cabecera
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 3, FillId = 0, BorderId = 0, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 8 - Arial Black
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 3, FillId = 0, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 9 - Arial Black con border
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 0, BorderId = 0, ApplyFill = false, ApplyFont = true, ApplyBorder = false }, // Index 10 - Numeros
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 2, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 11 - Azul con Borders
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Justify, Vertical = VerticalAlignmentValues.Top }) { FontId = 2, FillId = 4, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 12 - Amarillo Cabecera Texto Top
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 13 - Solo Texto
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 0, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 14 - Texto, bordes y centro
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Left, Vertical = VerticalAlignmentValues.Center }) { FontId = 4, FillId = 0, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 15 - Texto, bordes y izquierdo
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Right, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 0, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 16 - Texto, bordes y derecho
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Justify, Vertical = VerticalAlignmentValues.Center }) { FontId = 4, FillId = 0, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 17 - Sin Bold, Texto, bordes y justificado
            new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }) { FontId = 2, FillId = 5, BorderId = 1, ApplyFill = true, ApplyFont = true, ApplyBorder = true }, // Index 18 - Texto, bordes y justificado y Gris
            new CellFormat() { FontId = 3, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 19 - Bold Negro
            new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true }, // Index 20 - Sin Bold Negro

            new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true })
            ); // return
        }
    }
}
