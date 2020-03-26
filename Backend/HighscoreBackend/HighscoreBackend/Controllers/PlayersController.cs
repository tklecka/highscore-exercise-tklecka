using HighscoreBackend.Data;
using HighscoreBackend.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HighscoreBackend.Controllers
{
    [Route("api")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly HighscoreDBContext _context;
        private static readonly HttpClient client = new HttpClient();

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
        public async Task<ActionResult<Highscore>> PostHighscore(HighscoreRecaptcha scoreR)
        {
            var values = new Dictionary<string, string>
            {
                { "secret", "6LcNOeQUAAAAAG_tEuADEyMdI0pEdb6KpFdYhz2m" },
                { "response", scoreR.Token.ToString() }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);

            if (responseDict["success"].ToString().ToLower().Equals("True".ToLower()))
            {
                Highscore score = new Highscore { Initials = scoreR.Initials, Score = scoreR.Score };
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
            else
            {
                return Unauthorized("reCAPTCHA failed" + responseString);
            }

        }
    }
}
