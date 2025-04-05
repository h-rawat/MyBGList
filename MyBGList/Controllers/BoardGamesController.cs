using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBGList.DTO;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ILogger<BoardGamesController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public RestDTO<BoardGame[]> Get()
        {
            return new RestDTO<BoardGame[]>()
            {
                Data = new BoardGame[]
                {
                    new BoardGame()
                    {
                        Id = 1,
                        Name = "name 1",
                        Year = 2000,
                    },
                    new BoardGame()
                    {
                        Id = 2,
                        Name = "name 2",
                        Year = 2000,
                    },
                    new BoardGame()
                    {
                        Id = 3,
                        Name = "name 3",
                        Year = 2000,
                    },
                },
                Links = new List<LinkDTO>
                {
                    new LinkDTO(Url.Action(null, "BoardGames", null, Request.Scheme)!, "self", "GET"),
                }
            };
        }
    }
}
