using System;
using System.Collections.Generic;
using api.premier.Models;
using api.premier.Models.Catalogos;
using AutoMapper;
using biz.premier.Repository.Catalogue;
using biz.premier.Servicies;
using Microsoft.AspNetCore.Mvc;

namespace api.premier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IRoleRepository _roleRepository;

        public RoleController(IMapper mapper, ILoggerManager loggerManager, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _logger = loggerManager;
            _roleRepository = roleRepository;
        }
        
        [HttpGet("{permission}/{section}/")]
        public ActionResult GetPermission(int menu, int submenu, int role)
        {
            try
            {
                return StatusCode(201, new { Success = true, Result = _roleRepository.GetSection(menu,submenu,role) });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: { ex.ToString() }");
                return StatusCode(500, new { Success = false, Result = ex.Message });
            }
        }
    }
}