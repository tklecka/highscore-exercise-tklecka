using HighscoreBackend.Data;
using HighscoreBackend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HighscoreBackend.Controllers
{
    [Route("api")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly HighscoreDBContext _context;

        public PlayersController(HighscoreDBContext context)
        {
            _context = context;
        }

        // GET: api/highscore
        [HttpGet]
        [Route("highscore")]
        public async Task<ActionResult<IEnumerable<Highscore>>> GetHighscore()
        {
            //Get all the Highscore-Entries and sort them compared to their scores
            List<Highscore> highscores = await _context.Highscore.ToListAsync();
            highscores.Sort((h1, h2) => h2.Score.CompareTo(h1.Score));

            //Return the 10 highest scores of the list ♣
            if (highscores.Count > 10)
            {
                return highscores.GetRange(0, 10);
            }
            return highscores;
        }

        //{"Initials":"MK", "Score":69}
        [HttpPost]
        [Route("addScore")]
        public async Task<ActionResult<Highscore>> PostHighscore(Highscore score)
        {
            if (score.Initials.Length != 3 || score.Score < 0)
            {
                return BadRequest("Wrong arguments");
            }

            //Add the posted highscore to the Highscore-DbSet
            _context.Highscore.Add(score);
            await _context.SaveChangesAsync();

            List<Highscore> highscores = await _context.Highscore.ToListAsync();
            if (highscores.Count > 10)
            {
                highscores.Sort((h1, h2) => h1.Score.CompareTo(h2.Score));
                _context.Highscore.Remove(highscores[0]);
                await _context.SaveChangesAsync();

                if (highscores[0].HighscoreID == score.HighscoreID)
                {
                    return BadRequest("Not enough points");
                }
            }

            //Return the newly created highscore with the id
            return Created("PostHighscore", score);
        }
    }
}
