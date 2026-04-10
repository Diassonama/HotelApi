using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuRelatorioController : ControllerBase
    {
        private readonly GhotelDbContext _context;

        public MenuRelatorioController(GhotelDbContext context)
        {
            _context = context;
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu(string perfil)
        {
            var allMenuItems = await _context.MenuItem
                .Where(m => m.Perfil == perfil)
                .ToListAsync();

            var menuHierarchy = BuildMenuHierarchy(allMenuItems);

            return Ok(menuHierarchy);
        }
        private List<MenuItemDto> BuildMenuHierarchy(List<MenuItem> allItems, int? parentId = null)
        {
            return allItems
                .Where(m => m.ParentId == parentId)
                .Select(m => new MenuItemDto
                {
                    Id = m.Id,
                    Label = m.Label,
                    Action = m.Action,
                    SubmenuRef = m.SubmenuRef,
                    Children = BuildMenuHierarchy(allItems, m.Id)
                })
                .ToList();
        }
    }
}