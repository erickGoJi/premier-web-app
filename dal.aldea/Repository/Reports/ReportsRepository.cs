using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using biz.premier.Repository.Reports;
using dal.premier.DBContext;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using biz.premier.Repository.WorkOrder;
using Microsoft.AspNetCore.Hosting;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;

namespace dal.premier.Repository.Reports
{
    public class ReportsRepository : GenericRepository<biz.premier.Entities.Appointment>, IReports
    {
        private readonly IHousingListRepository _housingListRepository;
        private readonly ISchoolListRepository _schoolListRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReportsRepository(Db_PremierContext context, IHousingListRepository housingListRepository, ISchoolListRepository schoolListRepository, IHostingEnvironment hostingEnvironment ) : base(context)
        {
            _housingListRepository = housingListRepository;
            _schoolListRepository = schoolListRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        public void createPDF(string xUrl, string filePath, int miliseconds)
        {
            string xKey = "Mgo+DSMBaFt/QHJqVVhkX1pFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9jQXxTd01mXH1ecH1WRQ==;Mgo+DSMBMAY9C3t2VVhiQlFaclxJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1hWX5WdXFXRmFcVUc=";

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(xKey);
            //Initialize HTML to PDF converter with Blink rendering engine.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            blinkConverterSettings.AdditionalDelay = miliseconds;
            blinkConverterSettings.EnableJavaScript = true;
            blinkConverterSettings.Margin.All = 50;
            blinkConverterSettings.TempPath = "C://tempoSYNC";
            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = blinkConverterSettings;

            PdfDocument document = htmlConverter.Convert(xUrl);



            FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
            //Save and close the PDF document.
            document.Save(fileStream);
            document.Close(true);
            fileStream.Dispose();
            document.Dispose();
        }
        public void createExcelHousing(int id_service_detail, string path)
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
                DocumentFormat.OpenXml.Spreadsheet.Cell cell04 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell04.CellReference = "A7";
                cell04.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell04.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Property  #");
                cell04.StyleIndex = 14;
                headerRow04.Append(cell04);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell05 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell05.CellReference = "B7";
                cell05.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell05.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Property Type");
                cell05.StyleIndex = 14;
                headerRow04.Append(cell05);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell06 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell06.CellReference = "C7";
                cell06.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell06.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Neighborhood");
                cell06.StyleIndex = 14;
                headerRow04.Append(cell06);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell07 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell07.CellReference = "D7";
                cell07.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell07.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Address");
                cell07.StyleIndex = 14;
                headerRow04.Append(cell07);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell08 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell08.CellReference = "E7";
                cell08.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell08.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("# of Bedrooms");
                cell08.StyleIndex = 14;
                headerRow04.Append(cell08);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell09 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell09.CellReference = "F7";
                cell09.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell09.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("# of Bathrooms");
                cell09.StyleIndex = 14;
                headerRow04.Append(cell09);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell10.CellReference = "G7";
                cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("# of Parking Spaces");
                cell10.StyleIndex = 14;
                headerRow04.Append(cell10);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell11 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell11.CellReference = "H7";
                cell11.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell11.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Size (M²)");
                cell11.StyleIndex = 14;
                headerRow04.Append(cell11);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell12 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell12.CellReference = "I7";
                cell12.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell12.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("List Rent");
                cell12.StyleIndex = 14;
                headerRow04.Append(cell12);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell13 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell13.CellReference = "J7";
                cell13.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell13.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Currency");
                cell13.StyleIndex = 14;
                headerRow04.Append(cell13);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell14 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell14.CellReference = "K7";
                cell14.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell14.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Status");
                cell14.StyleIndex = 14;
                headerRow04.Append(cell14);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell15 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell15.CellReference = "L7";
                cell15.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell15.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Assignee's comments");
                cell15.StyleIndex = 14;
                headerRow04.Append(cell15);
                #endregion

                #region Datos
                var result = _context.HousingLists
                    .Where(x => x.IdServiceDetail == id_service_detail)
                    .OrderByDescending(x => x.VisitDate).ThenBy(n => n.VisitTime)
                    .Select(s => new
                {
                    AssigneName = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    ClienteName = s.WorkOrderNavigation.ServiceRecord.Client.Name,
                    s.Id,
                    supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                    supplierCompany = s.SupplierNavigation == null ? null : s.SupplierPartnerNavigation.ComercialName,
                    s.PropertyNo,
                    PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                    s.Address,
                    s.Price,
                    s.Neighborhood,
                    s.VisitTime,
                    Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                    Status = s.HousingStatusNavigation == null ? null : s.HousingStatusNavigation.Status,
                    s.Wassended,
                    s.Bedrooms,
                    s.Bathrooms,
                    s.ParkingSpaces,
                    Sise = s.Size,
                    AssigneComments = "",
                    RelAppointmentHousingLists = s.RelAppointmentHousingLists.Any()
                }).ToList();

                #region Fila 2
                DocumentFormat.OpenXml.Spreadsheet.Cell cell00 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell00.CellReference = "D2";
                cell00.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell00.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Assignee:");
                cell00.StyleIndex = 19;
                headerRow01.Append(cell00);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell01 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell01.CellReference = "E2";
                cell01.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell01.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(result.Count == 0 ? "" : result.FirstOrDefault().AssigneName);
                cell01.StyleIndex = 20;
                headerRow01.Append(cell01);
                #endregion

                #region Fila 3
                DocumentFormat.OpenXml.Spreadsheet.Cell cell02 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell02.CellReference = "D3";
                cell02.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell02.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Client:");
                cell02.StyleIndex = 19;
                headerRow02.Append(cell02);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell03 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell03.CellReference = "E3";
                cell03.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell03.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(result.Count == 0 ? "" : result.FirstOrDefault().ClienteName);
                cell03.StyleIndex = 20;
                headerRow02.Append(cell03);
                #endregion


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
                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow05 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    DocumentFormat.OpenXml.Spreadsheet.Cell cellA = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellA.CellReference = "A" + contador;
                    cellA.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellA.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PropertyNo.GetValueOrDefault());
                    cellA.StyleIndex = 15;
                    headerRow05.Append(cellA);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellB = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellB.CellReference = "B" + contador;
                    cellB.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellB.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.PropertyType);
                    cellB.StyleIndex = 15;
                    headerRow05.Append(cellB);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellC = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellC.CellReference = "C" + contador;
                    cellC.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellC.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Neighborhood);
                    cellC.StyleIndex = 15;
                    headerRow05.Append(cellC);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellD = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellD.CellReference = "D" + contador;
                    cellD.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellD.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Address);
                    cellD.StyleIndex = 15;
                    headerRow05.Append(cellD);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellE = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellE.CellReference = "E" + contador;
                    cellE.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellE.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Bedrooms.GetValueOrDefault());
                    cellE.StyleIndex = 15;
                    headerRow05.Append(cellE);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellF = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellF.CellReference = "F" + contador;
                    cellF.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellF.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Bathrooms.GetValueOrDefault());
                    cellF.StyleIndex = 15;
                    headerRow05.Append(cellF);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellG = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellG.CellReference = "G" + contador;
                    cellG.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellG.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.ParkingSpaces.GetValueOrDefault());
                    cellG.StyleIndex = 15;
                    headerRow05.Append(cellG);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellH = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellH.CellReference = "H" + contador;
                    cellH.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellH.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Sise.GetValueOrDefault());
                    cellH.StyleIndex = 15;
                    headerRow05.Append(cellH);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellI = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellI.CellReference = "I" + contador;
                    cellI.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellI.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Price.GetValueOrDefault());
                    cellI.StyleIndex = 15;
                    headerRow05.Append(cellI);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellJ = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellJ.CellReference = "J" + contador;
                    cellJ.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellJ.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Currency);
                    cellJ.StyleIndex = 15;
                    headerRow05.Append(cellJ);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellK = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellK.CellReference = "K" + contador;
                    cellK.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellK.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("");
                    cellK.StyleIndex = 15;
                    headerRow05.Append(cellK);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellL = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellL.CellReference = "L" + contador;
                    cellL.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellL.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("");
                    cellL.StyleIndex = 15;
                    headerRow05.Append(cellL);

                    contador++;

                    sd.Append(headerRow05);

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
                sheet.Name = "Housing";
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
        public void createExcelAppointment(int service_record_id, string path)
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
                DocumentFormat.OpenXml.Spreadsheet.Cell cell04 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell04.CellReference = "A7";
                cell04.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell04.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Date");
                cell04.StyleIndex = 14;
                headerRow04.Append(cell04);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell05 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell05.CellReference = "B7";
                cell05.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell05.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Supplier");
                cell05.StyleIndex = 14;
                headerRow04.Append(cell05);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell06 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell06.CellReference = "C7";
                cell06.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell06.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Services");
                cell06.StyleIndex = 14;
                headerRow04.Append(cell06);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell07 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell07.CellReference = "D7";
                cell07.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell07.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Start Time");
                cell07.StyleIndex = 14;
                headerRow04.Append(cell07);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell08 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell08.CellReference = "E7";
                cell08.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell08.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("End Time");
                cell08.StyleIndex = 14;
                headerRow04.Append(cell08);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell09 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell09.CellReference = "F7";
                cell09.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell09.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Location");
                cell09.StyleIndex = 14;
                headerRow04.Append(cell09);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell10 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell10.CellReference = "G7";
                cell10.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell10.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Appointment Message");
                cell10.StyleIndex = 14;
                headerRow04.Append(cell10);
                #endregion

                #region Datos

                var service = _context.Appointments
                .Include(i => i.AppointmentWorkOrderServices)
                .ThenInclude(i => i.WorkOrderService)
                .Select(e => new
                {
                    AssigneName = e.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    ClienteName = e.ServiceRecord.Client.Name,
                    Message = e.Description,
                    e.Id,
                    e.ServiceRecordId,
                    e.Date,
                    supplier = _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Name,
                    e.Status,
                    StatusName = e.StatusNavigation.Status,
                    e.CommentCancel,
                    avatar_supplier =
                    _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo == null ||
                    _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo == "" ?
                    "images/users/avatar.png" : _context.ProfileUsers.FirstOrDefault(x => x.UserId == e.To).Photo,
                    assignee = e.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                    avatar_assignee =
                    e.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo == null ?
                    "images/users/avatar.png" : e.ServiceRecord.AssigneeInformations.FirstOrDefault().Photo,
                    servicio = e.AppointmentWorkOrderServices.Any() ? _context.AppointmentWorkOrderServices.Where(x => x.AppointmentId == e.Id)
                         .Select(k => new
                         {
                             id = k.WorkOrderService.StandaloneServiceWorkOrders.Any().Equals(false)
                                 ? k.WorkOrderService.BundledServices.SingleOrDefault().Id
                                 : k.WorkOrderService.StandaloneServiceWorkOrders.SingleOrDefault().Id,
                             serviceNumber = (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceNumber
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceNumber)
                             + "-" +
                             (_context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName == "--"
                             ? _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).IdServiceNavigation.Service
                             : _context.ServiceLocations.SingleOrDefault(f => f.IdClientPartnerProfile == e.ServiceRecord.ClientId
                               && f.IdService == (k.WorkOrderService.StandaloneServiceWorkOrders.Any()
                             ? k.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                             : k.WorkOrderService.BundledServices.FirstOrDefault().ServiceId)).NickName),
                         }).ToList() : null,
                    StartTime = e.StartTime,
                    StartTimeZone = _context.CatCities.FirstOrDefault(x => x.IdCountry == e.ToNavigation.ProfileUsers.FirstOrDefault().Country).IdTimeZoneNavigation.TimeZone,
                    EndTime = e.EndTime,
                    EndTimeZone = _context.CatCities.FirstOrDefault(x => x.IdCountry == e.ToNavigation.ProfileUsers.FirstOrDefault().Country).IdTimeZoneNavigation.TimeZone,
                    location = e.AppointmentWorkOrderServices.Select(s => s.WorkOrderService.BundledServices.Any(a => a.WorkServicesId == s.WorkOrderServiceId)
                            ? s.WorkOrderService.BundledServices.FirstOrDefault().Location
                            : s.WorkOrderService.StandaloneServiceWorkOrders.FirstOrDefault().Location).FirstOrDefault(),
                    document = _context.DocumentAppointments.Count(x => x.AppointmentId == e.Id),
                    documentAppointments = _context.DocumentAppointments.Where(x => x.AppointmentId == e.Id).ToList(),
                    start = e.Start.HasValue ? e.Start.Value : false,
                    ended = e.Ended.HasValue ? e.Ended.Value : false,
                    e.Report,
                    e.To,
                    e.From
                }).Where(y => y.ServiceRecordId == service_record_id && y.Date.Date >= DateTime.Now.Date).ToList();

                #region Fila 2
                DocumentFormat.OpenXml.Spreadsheet.Cell cell00 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell00.CellReference = "D2";
                cell00.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell00.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Assignee:");
                cell00.StyleIndex = 19;
                headerRow01.Append(cell00);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell01 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell01.CellReference = "E2";
                cell01.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell01.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(service.Count == 0 ? "" : service.FirstOrDefault().AssigneName);
                cell01.StyleIndex = 20;
                headerRow01.Append(cell01);
                #endregion

                #region Fila 3
                DocumentFormat.OpenXml.Spreadsheet.Cell cell02 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell02.CellReference = "D3";
                cell02.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell02.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Client:");
                cell02.StyleIndex = 19;
                headerRow02.Append(cell02);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell03 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell03.CellReference = "E3";
                cell03.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell03.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(service.Count == 0 ? "" : service.FirstOrDefault().ClienteName);
                cell03.StyleIndex = 20;
                headerRow02.Append(cell03);
                #endregion


                sd.Append(headerRow00);
                sd.Append(headerRow01);
                sd.Append(headerRow02);
                sd.Append(headerRow03);
                sd.Append(headerRow04_1);
                sd.Append(headerRow04_2);
                sd.Append(headerRow04);
                
                int contador = 8;
                foreach (var item in service)
                {
                    if (item.servicio.Count > 0)
                    {
                        foreach (var servicio in item.servicio)
                        {
                            DocumentFormat.OpenXml.Spreadsheet.Row headerRow05 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            DocumentFormat.OpenXml.Spreadsheet.Cell cellA = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellA.CellReference = "A" + contador;
                            cellA.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellA.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Date.ToString("dd/MM/yyyy"));
                            cellA.StyleIndex = 15;
                            headerRow05.Append(cellA);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cellB = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellB.CellReference = "B" + contador;
                            cellB.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellB.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.supplier);
                            cellB.StyleIndex = 15;
                            headerRow05.Append(cellB);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cellC = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellC.CellReference = "C" + contador;
                            cellC.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellC.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(servicio.serviceNumber);
                            cellC.StyleIndex = 15;
                            headerRow05.Append(cellC);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cellD = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellD.CellReference = "D" + contador;
                            cellD.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellD.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.StartTime.HasValue == false ? "" :  item.StartTime.GetValueOrDefault().ToString("dd/MM/yyyy"));
                            cellD.StyleIndex = 15;
                            headerRow05.Append(cellD);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cellE = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellE.CellReference = "E" + contador;
                            cellE.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellE.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.EndTime.HasValue == false ? "": item.EndTime.GetValueOrDefault().ToString("dd/MM/yyyy"));
                            cellE.StyleIndex = 15;
                            headerRow05.Append(cellE);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cellF = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellF.CellReference = "F" + contador;
                            cellF.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellF.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.location);
                            cellF.StyleIndex = 15;
                            headerRow05.Append(cellF);

                            DocumentFormat.OpenXml.Spreadsheet.Cell cellG = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            cellG.CellReference = "G" + contador;
                            cellG.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                            cellG.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Message);
                            cellG.StyleIndex = 15;
                            headerRow05.Append(cellG);

                            contador++;

                            sd.Append(headerRow05);
                        }
                    }
                    else
                    {
                        DocumentFormat.OpenXml.Spreadsheet.Row headerRow05 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        DocumentFormat.OpenXml.Spreadsheet.Cell cellA = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellA.CellReference = "A" + contador;
                        cellA.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellA.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Date.ToString("dd/MM/yyyy"));
                        cellA.StyleIndex = 15;
                        headerRow05.Append(cellA);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellB = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellB.CellReference = "B" + contador;
                        cellB.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellB.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.supplier);
                        cellB.StyleIndex = 15;
                        headerRow05.Append(cellB);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellC = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellC.CellReference = "C" + contador;
                        cellC.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellC.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("");
                        cellC.StyleIndex = 15;
                        headerRow05.Append(cellC);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellD = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellD.CellReference = "D" + contador;
                        cellD.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellD.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.StartTime.HasValue == false ? "" : item.StartTime.GetValueOrDefault().ToString("dd/MM/yyyy"));
                        cellD.StyleIndex = 15;
                        headerRow05.Append(cellD);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellE = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellE.CellReference = "E" + contador;
                        cellE.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellE.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.EndTime.HasValue == false ? "" : item.EndTime.GetValueOrDefault().ToString("dd/MM/yyyy"));
                        cellE.StyleIndex = 15;
                        headerRow05.Append(cellE);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellF = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellF.CellReference = "F" + contador;
                        cellF.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellF.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.location);
                        cellF.StyleIndex = 15;
                        headerRow05.Append(cellF);

                        DocumentFormat.OpenXml.Spreadsheet.Cell cellG = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        cellG.CellReference = "G" + contador;
                        cellG.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                        cellG.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Message);
                        cellG.StyleIndex = 15;
                        headerRow05.Append(cellG);

                        contador++;

                        sd.Append(headerRow05);
                    }
                    
                }

                #endregion              

                Columns columns = new Columns();
                Column column00;
                column00 = new Column();
                column00.Min = 7;
                column00.Max = 7;
                column00.Width = 20;
                column00.CustomWidth = true;
                columns.Append(column00);

                ws.Append(columns);
                ws.Append(sd);
                ws.Append(drawing);
                wsp.Worksheet = ws;
                wsp.Worksheet.Save();
                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Name = "Appointment";
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
        public void createExcelSchooling(int wo_id, string path)
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
                DocumentFormat.OpenXml.Spreadsheet.Cell cell04 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell04.CellReference = "A7";
                cell04.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell04.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Visit Date");
                cell04.StyleIndex = 14;
                headerRow04.Append(cell04);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell05 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell05.CellReference = "B7";
                cell05.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell05.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Schools Name");
                cell05.StyleIndex = 14;
                headerRow04.Append(cell05);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell06 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell06.CellReference = "C7";
                cell06.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell06.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Address");
                cell06.StyleIndex = 14;
                headerRow04.Append(cell06);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell07 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell07.CellReference = "D7";
                cell07.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell07.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Grade Levels");
                cell07.StyleIndex = 14;
                headerRow04.Append(cell07);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell08 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell08.CellReference = "E7";
                cell08.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell08.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Website");
                cell08.StyleIndex = 14;
                headerRow04.Append(cell08);
                #endregion

                #region Datos


                var hl = _context.SchoolsLists
                    .Include(x => x.GradeNavigation)
                    .Include(x => x.WorkOrderNavigation)
                    .ThenInclude(x => x.ServiceRecord)
                    .ThenInclude(x => x.AssigneeInformations)
                    .Include(x => x.WorkOrderNavigation)
                    .ThenInclude(x => x.ServiceRecord)
                    .ThenInclude(x => x.Client)
                    .Where(x => x.WorkOrderNavigation.Id == wo_id)
                    .ToList();
                var custom = hl
                    .Select(s => new
                    {
                        AssigneName = s.WorkOrderNavigation.ServiceRecord.AssigneeInformations.FirstOrDefault().AssigneeName,
                        ClienteName = s.WorkOrderNavigation.ServiceRecord.Client.Name,
                        s.WebSite,
                        s.Id,
                        s.SchoolNo,
                        s.SchoolName,
                        s.VisitDate,
                        Grade = s.GradeNavigation == null ? "" : s.GradeNavigation.Grade,
                        s.Admision,
                        Name = s.DependentNavigation == null ? "" : s.DependentNavigation.Name,
                        s.Address,
                        s.PerMonth,
                        Currency = s.CurrencyNavigation == null ? "" : s.CurrencyNavigation.Currency,
                        Status = s.SchoolingStatusNavigation == null ? "" : s.SchoolingStatusNavigation.Status
                    }).ToList();

                #region Fila 2
                DocumentFormat.OpenXml.Spreadsheet.Cell cell00 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell00.CellReference = "D2";
                cell00.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell00.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Assignee:");
                cell00.StyleIndex = 19;
                headerRow01.Append(cell00);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell01 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell01.CellReference = "E2";
                cell01.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell01.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(custom.Count == 0 ? "" : custom.FirstOrDefault().AssigneName);
                cell01.StyleIndex = 20;
                headerRow01.Append(cell01);
                #endregion

                #region Fila 3
                DocumentFormat.OpenXml.Spreadsheet.Cell cell02 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell02.CellReference = "D3";
                cell02.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell02.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue("Client:");
                cell02.StyleIndex = 19;
                headerRow02.Append(cell02);

                DocumentFormat.OpenXml.Spreadsheet.Cell cell03 = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                cell03.CellReference = "E3";
                cell03.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                cell03.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(custom.Count == 0 ? "" : custom.FirstOrDefault().ClienteName);
                cell03.StyleIndex = 20;
                headerRow02.Append(cell03);
                #endregion


                sd.Append(headerRow00);
                sd.Append(headerRow01);
                sd.Append(headerRow02);
                sd.Append(headerRow03);
                sd.Append(headerRow04_1);
                sd.Append(headerRow04_2);
                sd.Append(headerRow04);
                

                int contador = 8;
                foreach (var item in custom)
                {
                    DocumentFormat.OpenXml.Spreadsheet.Row headerRow05 = new DocumentFormat.OpenXml.Spreadsheet.Row();
                    DocumentFormat.OpenXml.Spreadsheet.Cell cellA = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellA.CellReference = "A" + contador;
                    cellA.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellA.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.VisitDate.HasValue == false ? "" : item.VisitDate.GetValueOrDefault().ToString("dd/MM/yyyy"));
                    cellA.StyleIndex = 15;
                    headerRow05.Append(cellA);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellB = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellB.CellReference = "B" + contador;
                    cellB.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellB.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.SchoolName);
                    cellB.StyleIndex = 15;
                    headerRow05.Append(cellB);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellC = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellC.CellReference = "C" + contador;
                    cellC.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellC.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Address);
                    cellC.StyleIndex = 15;
                    headerRow05.Append(cellC);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellD = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellD.CellReference = "D" + contador;
                    cellD.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellD.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.Grade);
                    cellD.StyleIndex = 15;
                    headerRow05.Append(cellD);

                    DocumentFormat.OpenXml.Spreadsheet.Cell cellE = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                    cellE.CellReference = "E" + contador;
                    cellE.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.String;
                    cellE.CellValue = new DocumentFormat.OpenXml.Spreadsheet.CellValue(item.WebSite);
                    cellE.StyleIndex = 15;
                    headerRow05.Append(cellE);

                    contador++;

                    sd.Append(headerRow05);

                }

                #endregion              

                Columns columns = new Columns();
                Column column00;
                column00 = new Column();
                column00.Min = 2;
                column00.Max = 2;
                column00.Width = 20;
                column00.CustomWidth = true;
                columns.Append(column00);

                Column column01;
                column01 = new Column();
                column01.Min = 3;
                column01.Max = 3;
                column01.Width = 20;
                column01.CustomWidth = true;
                columns.Append(column01);

                Column column02;
                column02 = new Column();
                column02.Min = 5;
                column02.Max = 5;
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
                sheet.Name = "Schooling";
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

        public void pdfExample()
        {
            string xKey = "Mgo+DSMBaFt/QHJqVVhkX1pFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9jQXxTd01mXH1ecH1WRQ==;Mgo+DSMBMAY9C3t2VVhiQlFaclxJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1hWX5WdXFXRmFcVUc=";

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(xKey);

            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            string htmlText = "";
            string baseUrl = Path.GetFullPath("./images/");

            PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);

            //Save and close the PDF document.
            //FileStream fileStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.ReadWrite);
            ////Save and close the PDF document.
            //document.Save(fileStream);
            //document.Close(true);
            //fileStream.Dispose();
            //document.Dispose();
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
