﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ApplicationDbContext context, ILogger<BoardGamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<BoardGame[]>> Get(
            [FromQuery] RequestDTO<BoardGameDTO> input)
        {
            // this creates the IQueryable<T> expression tree
            var query = _context.BoardGames.AsQueryable();

            if (!string.IsNullOrEmpty(input.FilterQuery))
            {
                query = query.Where(b => b.Name.Contains(input.FilterQuery));
            }

            var recordCount = await query.CountAsync();

            query = query
                    .OrderBy($"{input.SortColumn} {input.SortOrder}")
                    .Skip(input.PageIndex * input.PageSize)
                    .Take(input.PageSize);

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = input.PageIndex,
                PageSize = input.PageSize,
                RecordCount = recordCount,
                Links = new List<LinkDTO>()
                {
                    new LinkDTO(
                        Url.Action(null, "BoardGames", new { input.PageIndex, input.PageSize }, Request.Scheme)!,
                        "self",
                        "GET"),
                }
            };
        }

        [HttpPost(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame>> Post(BoardGameDTO model)
        {
            var boardgame = await _context.BoardGames
                .Where(b => b.Id == model.Id)
                .FirstOrDefaultAsync();

            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                {
                    boardgame.Name = model.Name;
                }
                if (model.Year.HasValue && model.Year.Value > 0)
                {
                    boardgame.Year = model.Year.Value;
                }
                boardgame.LastModifiedDate = DateTime.Now;
                _context.BoardGames.Update(boardgame);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame>()
            {
                Data = boardgame!,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                        Url.Action(null, "BoardGames", model, Request.Scheme)!,
                        "self",
                        "POST"),
                }
            };
        }

        [HttpDelete(Name = "DeleteBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame>> Delete(int id)
        {
            var boardgame = await _context.BoardGames
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync();

            if (boardgame != null)
            {
                _context.BoardGames.Remove(boardgame);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame>()
            {
                Data = boardgame!,
                Links = new List<LinkDTO>
                {
                    new LinkDTO(
                        Url.Action(null, "BoardGames", id, Request.Scheme)!,
                        "self",
                        "DELETE"),
                }
            };
        }
    }
}
