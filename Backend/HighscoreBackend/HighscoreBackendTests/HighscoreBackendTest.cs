using HighscoreBackend.Controllers;
using HighscoreBackend.Data;
using HighscoreBackend.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HighscoreBackendTests
{
    public class HighscoreBackendTest
    {
        [Fact]
        public async Task TestAddOneHighscoreAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            await controller.PostHighscore(new Highscore { Initials = "AAA", Score = 100 });
            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Single(list);
        }

        [Fact]
        public async Task TestAddFiveScoresAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            for (int i = 0; i < 5; i++)
            {
                await controller.PostHighscore(new Highscore { Initials = "AAA", Score = 100 });
            }
            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Equal(5, list.Count);
        }

        [Fact]
        public async Task TestAddTwelveScoresAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            for (int i = 0; i < 12; i++)
            {
                await controller.PostHighscore(new Highscore { Initials = "AAA", Score = 100 });
            }
            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Equal(10, list.Count);
        }

        [Fact]
        public async Task TestAddThreeScoresAndCheckOrderAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            for (int i = 0; i < 3; i++)
            {
                await controller.PostHighscore(new Highscore { Initials = "AA" + i, Score = 100 + i });
            }
            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Equal(3, list.Count);

            Assert.Equal(102, list[0].Score);
            Assert.Equal("AA2", list[0].Initials);

            Assert.Equal(101, list[1].Score);
            Assert.Equal("AA1", list[1].Initials);

            Assert.Equal(100, list[2].Score);
            Assert.Equal("AA0", list[2].Initials);
        }

        [Fact]
        public async Task TestAddElevenScoresAndCheckValuesAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            for (int i = 0; i < 10; i++)
            {
                await controller.PostHighscore(new Highscore { Initials = "AA" + i, Score = 100 + i });
            }
            await controller.PostHighscore(new Highscore { Initials = "AAA", Score = 200 });

            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Equal(10, list.Count);

            Assert.Equal(200, list[0].Score);
            Assert.Equal("AAA", list[0].Initials);

            Assert.Equal(102, list[8].Score);
            Assert.Equal("AA2", list[8].Initials);

            Assert.Equal(101, list[9].Score);
            Assert.Equal("AA1", list[9].Initials);
        }

        [Fact]
        public async Task TestAddElevenScoresAndCheckValuesAsync2()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            for (int i = 0; i < 10; i++)
            {
                await controller.PostHighscore(new Highscore { Initials = "AA" + i, Score = 100 + i });
            }
            await controller.PostHighscore(new Highscore { Initials = "AAA", Score = 50 });

            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Equal(10, list.Count);

            Assert.Equal(109, list[0].Score);
            Assert.Equal("AA9", list[0].Initials);

            Assert.Equal(101, list[8].Score);
            Assert.Equal("AA1", list[8].Initials);

            Assert.Equal(100, list[9].Score);
            Assert.Equal("AA0", list[9].Initials);
        }

        [Fact]
        public async Task TestAddFalseInitialsAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            await controller.PostHighscore(new Highscore { Initials = "TEST", Score = 100 });
            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Empty(list);
        }

        [Fact]
        public async Task TestAddFalseScoreAsync()
        {
            DbContextOptions<HighscoreDBContext> options = new DbContextOptionsBuilder<HighscoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            PlayersController controller = new PlayersController(new HighscoreDBContext(options));

            await controller.PostHighscore(new Highscore { Initials = "AAA", Score = -1 });
            var resGet = await controller.GetHighscore();

            List<Highscore> list = resGet.Value.ToList();
            Assert.Empty(list);
        }
    }
}
