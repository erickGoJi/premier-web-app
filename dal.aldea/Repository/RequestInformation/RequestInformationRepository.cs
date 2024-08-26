using biz.premier.Entities;
using biz.premier.Models.Email;
using biz.premier.Repository.RequestInformation;
using biz.premier.Servicies;
using dal.premier.DBContext;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.RequestInformation
{
    public class RequestInformationRepository : GenericRepository<biz.premier.Entities.RequestInformation>, IRequestInformationRepository
    {
        private IConfiguration _config;
        private IEmailService _emailservice;
        public RequestInformationRepository(Db_PremierContext context, IEmailService emailService, IConfiguration configuration) : base(context) 
        {
            _config = configuration;
            _emailservice = emailService;
        }

        public List<Tuple<int, string>> HousingAvailible(int sr)
        {
            List<Tuple<int, string>> tuples = new List<Tuple<int, string>>();
            var preDecisionStandalone = _context.PredecisionOrientations
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
            var preDecisionBundled = _context.PredecisionOrientations
                .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr).Union(preDecisionStandalone);
            if (preDecisionBundled.Any())
                tuples.Add(new Tuple<int, string>(1, "Pre Decision"));
            
            var homeFindingStandalone = _context.HomeFindings
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
            var homeFindingBundled = _context.HomeFindings
                .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr).Union(homeFindingStandalone);
            if (homeFindingBundled.Any())
                tuples.Add(new Tuple<int, string>(2, "Home Finding"));
            
            var areaOrientationStandalone = _context.AreaOrientations
                .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
            var areaOrientationBundled = _context.AreaOrientations
                .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr).Union(areaOrientationStandalone);
            if (areaOrientationBundled.Any())
                tuples.Add(new Tuple<int, string>(3, "Area Orientation"));
            
            return tuples;
        }

        public int GetWorkOrderService(int sr, int type)
        {
            int id = 0;
            switch (type)
            {
                case (1):
                    var preDecisionStandalone = _context.PredecisionOrientations
                        .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
                    var preDecisionBundled = _context.PredecisionOrientations
                        .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr)
                        .Union(preDecisionStandalone);
                    id = preDecisionBundled.First().WorkOrderServicesId;
                    break;
                case (2):
                    var homeFindingStandalone = _context.HomeFindings
                        .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
                    var homeFindingBundled = _context.HomeFindings
                        .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr)
                        .Union(homeFindingStandalone);
                    id = homeFindingBundled.First().WorkOrderServicesId;
                    break;
                case (3):
                    var areaOrientationStandalone = _context.AreaOrientations
                        .Where(x => x.WorkOrderServices.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId == sr);
                    var areaOrientationBundled = _context.AreaOrientations
                        .Where(x => x.WorkOrderServices.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId == sr)
                        .Union(areaOrientationStandalone);
                    id = areaOrientationBundled.First().WorkOrderServicesId;
                    break;
                default:
                    id = -1;
                    break;
            }

            return id;
        }

        public biz.premier.Entities.RequestInformation insert(biz.premier.Entities.RequestInformation request)
        {
            request.Due = request.Due;
            _context.Set<biz.premier.Entities.RequestInformation>().Add(request);
            _context.SaveChanges();
            return request;
        }

        public string SendMail(string emailTo, string body, string subject, List<RequestInformationDocument> documents)
        {
            EmailModelAttach email = new EmailModelAttach();
            email.To = emailTo;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;
            email.File = new List<Files>();
            Files files = new Files();  
            foreach(var i in documents)
            {
                //Uri uriAddress1 = new Uri(i.FileRequest);
                //Dim fullUrl As String;
                string lastPart = i.FileRequest;
                files.attach = lastPart;
                email.File.Add(files);
            }

            return _emailservice.SendEmailAttach(email);
        }
    }
}
