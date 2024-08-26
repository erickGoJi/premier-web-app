using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.premier.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.ClientPartner
{
    public class ClientPartnerProfileClientRepository : GenericRepository<biz.premier.Entities.ClientPartnerProfileClient>, IClientPartnerProfileClientRepository
    {
        public ClientPartnerProfileClientRepository(Db_PremierContext context) : base(context) { }
        public async Task<bool> AssociatePartnerInformation(int idClientFrom, int idClientTo)
        {
            var serviceLocationsPartner =
                _context.ServiceLocations.Include(i=>i.ServiceLocationCountries).Where(x => x.IdClientPartnerProfile == idClientFrom);
            var documentsPartner =
                _context.DocumentClientPartnerProfiles.Where(x => x.IdClientPartnerProfile == idClientFrom);
            var serviceScoreAwardsPartner = _context.ServiceScoreAwards.Where(x => x.IdClientPartnerProfile == idClientFrom);
            var experienceTeamPartner =
                _context.ClientPartnerProfileExperienceTeams.Where(x => x.IdClientPartnerProfile == idClientFrom);
            List<ServiceLocation> locations = new List<ServiceLocation>(); 
            foreach (var serviceLocation in serviceLocationsPartner)
            {
                locations.Add(new ServiceLocation()
                {
                    IdService = serviceLocation.IdService,
                    NickName = serviceLocation.NickName,
                    IdServiceLine = serviceLocation.IdServiceLine,
                    IdClientPartnerProfile = idClientTo,
                    ServiceLocationCountries = serviceLocation.ServiceLocationCountries
                });
            }
            
            List<DocumentClientPartnerProfile> documentClientPartnerProfiles = new List<DocumentClientPartnerProfile>();
            foreach (var documentClientPartnerProfile in documentsPartner)
            {
                documentClientPartnerProfiles.Add(new DocumentClientPartnerProfile()
                {
                    Comments = documentClientPartnerProfile.Comments,
                    Description = documentClientPartnerProfile.Description,
                    Privacy = documentClientPartnerProfile.Privacy,
                    IdClientPartnerProfile = idClientTo,
                    FileName = documentClientPartnerProfile.FileName,
                    FileRequest = documentClientPartnerProfile.FileRequest,
                    IdDocumentType = documentClientPartnerProfile.IdDocumentType,
                    UploadDate = documentClientPartnerProfile.UploadDate,
                });
            }
            
            List<ServiceScoreAward> awards = new List<ServiceScoreAward>();
            foreach (var scoreAward in serviceScoreAwardsPartner)
            {
                awards.Add(new ServiceScoreAward()
                {
                    Comment = scoreAward.Comment,
                    Description = scoreAward.Description,
                    Year = scoreAward.Year,
                    IdClientPartnerProfile = idClientTo,
                    IdType = scoreAward.IdType,
                    IdServiceLine = scoreAward.IdServiceLine
                });
            }

            List<ClientPartnerProfileExperienceTeam> experienceTeams = new List<ClientPartnerProfileExperienceTeam>();
            foreach (var experienceTeam in experienceTeamPartner)
            {
                experienceTeams.Add(new ClientPartnerProfileExperienceTeam()
                {
                    UserId = experienceTeam.UserId,
                    IdServiceLine = experienceTeam.IdServiceLine,
                    IdClientPartnerProfile = idClientTo
                });
            }

            _context.ClientPartnerProfileExperienceTeams.AddRange(experienceTeams);
            _context.ServiceScoreAwards.AddRange(awards);
            _context.DocumentClientPartnerProfiles.AddRange(documentClientPartnerProfiles);
            _context.ServiceLocations.AddRange(locations);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DetachPartnerInformation(int idClientFrom, int idClientTo)
        {
            var serviceLocationsPartner =
                _context.ServiceLocations.Include(i=>i.ServiceLocationCountries).Where(x => x.IdClientPartnerProfile == idClientFrom).AsNoTracking();
            var serviceLocationsClient =
                _context.ServiceLocations.Include(i=>i.ServiceLocationCountries).Where(x => x.IdClientPartnerProfile == idClientTo).AsNoTracking();
            List<ServiceLocation> locations = new List<ServiceLocation>();
            foreach (var serviceLocation in serviceLocationsPartner)
            {
                var client = serviceLocationsClient.FirstOrDefault(x =>
                    x.IdServiceLine == serviceLocation.IdServiceLine
                    && x.NickName == serviceLocation.NickName
                    && x.IdService == serviceLocation.IdService);
                    if(client!= null) 
                        locations.Add(new ServiceLocation()
                        {
                            Id = client.Id,
                            IdService = client.IdService,
                            NickName = client.NickName,
                            IdServiceLine = client.IdServiceLine,
                            IdClientPartnerProfile = client.IdClientPartnerProfile
                        });
            }
            
            var documentsPartner =
                _context.DocumentClientPartnerProfiles.Where(x => x.IdClientPartnerProfile == idClientFrom).AsNoTracking();
            var documentsClient =
                _context.DocumentClientPartnerProfiles.Where(x => x.IdClientPartnerProfile == idClientTo).AsNoTracking();
            List<DocumentClientPartnerProfile> documentClientPartnerProfiles = new List<DocumentClientPartnerProfile>();
            foreach (var documentClientPartnerProfile in documentsPartner)
            {
                var document = documentsClient.FirstOrDefault(x =>
                    x.Comments == documentClientPartnerProfile.Comments
                    && x.Description == documentClientPartnerProfile.Description
                    && x.Privacy == documentClientPartnerProfile.Privacy
                    && x.IdClientPartnerProfile == documentClientPartnerProfile.IdClientPartnerProfile
                    && x.FileName == documentClientPartnerProfile.FileName
                    && x.FileRequest == documentClientPartnerProfile.FileRequest
                    && x.IdDocumentType == documentClientPartnerProfile.IdDocumentType
                    && x.UploadDate == documentClientPartnerProfile.UploadDate);
                if(document != null) 
                    documentClientPartnerProfiles.Add(new DocumentClientPartnerProfile()
                    {
                        Comments = document.Comments,
                        Description = document.Description,
                        Id = document.Id,
                        Privacy = document.Privacy,
                        FileName = document.FileName,
                        FileRequest = document.FileRequest,
                        UploadDate = document.UploadDate,
                        IdDocumentType = document.IdDocumentType,
                        IdClientPartnerProfile = document.IdClientPartnerProfile
                    });
            }

            var serviceScoreAwardsPartner = _context.ServiceScoreAwards.Where(x => x.IdClientPartnerProfile == idClientFrom).AsNoTracking();
            var serviceScoreAwardsClient = _context.ServiceScoreAwards.Where(x => x.IdClientPartnerProfile == idClientTo).AsNoTracking();
            List<ServiceScoreAward> awards = new List<ServiceScoreAward>();
            foreach (var award in serviceScoreAwardsPartner)
            {
                var scoreAward = serviceScoreAwardsClient.FirstOrDefault(x =>
                    x.Comment == award.Comment
                    && x.Description == award.Description
                    && x.Year == award.Year
                    && x.IdType == award.IdType
                    && x.IdServiceLine == award.IdServiceLine);
                if(scoreAward != null) 
                    awards.Add(new ServiceScoreAward()
                    {
                        Id = scoreAward.Id,
                        Comment = scoreAward.Comment,
                        Description = scoreAward.Description,
                        Year = scoreAward.Year,
                        IdType = scoreAward.IdType,
                        IdServiceLine = scoreAward.IdServiceLine,
                        IdClientPartnerProfile = scoreAward.IdClientPartnerProfile
                    });
            }

            var experienceTeamPartner =
                _context.ClientPartnerProfileExperienceTeams.Where(x => x.IdClientPartnerProfile == idClientFrom).AsNoTracking();
            var experienceTeamClient =
                _context.ClientPartnerProfileExperienceTeams.Where(x => x.IdClientPartnerProfile == idClientTo).AsNoTracking();
            List<ClientPartnerProfileExperienceTeam> experienceTeams = new List<ClientPartnerProfileExperienceTeam>();
            foreach (var experienceTeam in experienceTeamPartner)
            {
                var profileExperienceTeam = experienceTeamClient.FirstOrDefault(x =>
                    x.UserId == experienceTeam.UserId && x.IdServiceLine == experienceTeam.IdServiceLine);
                if(profileExperienceTeam != null) 
                    experienceTeams.Add(new ClientPartnerProfileExperienceTeam()
                    {
                        Id = profileExperienceTeam.Id,
                        UserId = profileExperienceTeam.UserId,
                        IdClientPartnerProfile = profileExperienceTeam.IdClientPartnerProfile,
                        IdServiceLine = profileExperienceTeam.IdServiceLine
                    });
            }

            _context.ChangeTracker.DetectChanges();
            _context.ClientPartnerProfileExperienceTeams.RemoveRange(experienceTeams.Distinct());
            _context.ServiceScoreAwards.RemoveRange(awards.Distinct());
            _context.DocumentClientPartnerProfiles.RemoveRange(documentClientPartnerProfiles.Distinct());
            _context.ServiceLocations.RemoveRange(locations.Distinct());
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
