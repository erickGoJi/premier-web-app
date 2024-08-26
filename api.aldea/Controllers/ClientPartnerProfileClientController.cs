using api.premier.ActionFilter;
using api.premier.Models;
using api.premier.Models.ClientPartnerProfile;
using AutoMapper;
using biz.premier.Entities;
using biz.premier.Repository.ClientPartner;
using biz.premier.Repository.ProfileUser;
using biz.premier.Repository.Utility;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Controllers
{
    public class ClientPartnerProfileClientController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IProfileUserRepository _profileUserRepository;
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly IClientPartnerProfileClientRepository _clientPartnerProfileClientRepository;
        public ClientPartnerProfileClientController(IMapper mapper, ILoggerManager loggerManager, IClientPartnerProfileClientRepository clientPartnerProfileClientRepository, IUtiltyRepository utiltyRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _clientPartnerProfileClientRepository = clientPartnerProfileClientRepository;
            _utiltyRepository = utiltyRepository;
        }

        // Post a Profile 
        [HttpPut("AddClientPartnerProfileClient", Name = "AddClientPartnerProfileClient")]
        [ServiceFilterAttribute(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult<ApiResponse<ClientPartnerProfileClientDto>>> AddClientPartnerProfileClient([FromBody] ClientPartnerProfileClientDto dto)
        {
            var response = new ApiResponse<ClientPartnerProfileClientDto>();
            try
            {
                ClientPartnerProfileClientDto Client = _mapper.Map<ClientPartnerProfileClientDto>(_clientPartnerProfileClientRepository.Add(_mapper.Map<ClientPartnerProfileClient>(dto)));


                await _clientPartnerProfileClientRepository.AssociatePartnerInformation(Client.IdClientFrom,
                    Client.IdClientTo);
                response.Success = true;
                response.Message = "Success";
                response.Result = Client;
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Success = false;
                response.Message = ex.ToString();
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return StatusCode(500, response);
            }
            return StatusCode(201, response);
        }

        [HttpDelete]
        [Route("DeleteClientPartnerProfileClient")]
        public async Task<ActionResult> DeleteClientPartnerProfileClient(int id)
        {
            try
            {
                var client = _clientPartnerProfileClientRepository.Find(c => c.Id == id);
                if (client != null)
                {
                    _clientPartnerProfileClientRepository.Delete(_mapper.Map<ClientPartnerProfileClient>(client));
                    await _clientPartnerProfileClientRepository.DetachPartnerInformation(client.IdClientFrom,
                        client.IdClientTo);
                    return Ok(new { Success = true, Result = "Client delete success" });
                }
                else
                {
                    return BadRequest(new { Success = false, Result = "Client Not Found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.Message.ToString() }");
                return BadRequest(new { Success = false, Message = ex.ToString() });
            }
        }
    }
}
