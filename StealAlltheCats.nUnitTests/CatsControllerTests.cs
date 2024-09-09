using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StealAllTheCats.Data;

namespace StealAlltheCats.nUnitTests
{
    [TestFixture]
    public class Tests
    {
        // to have the same Configuration object as in Startup
        private IConfigurationRoot _configuration;

        // represents database's configuration
        private DbContextOptions<DataContext> _options;
        private readonly DataContext _context;

        public Tests(DataContext context)
        {
            _context = context;
        }

        [SetUp]
        public void Setup()
        { }



        [Test]
        public void CatsController_TestEndpoint_GetCatById()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();
            _options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(_configuration.GetConnectionString("DbConnection"))
                .Options;

            Assert.Pass();

            var controller = new StealAllTheCats.Controllers.CatsController(_context);
            var result = controller.GetSpecificCat(0) as IActionResult;
            Assert.AreEqual("The Id should be > 0", result.ToString());

        }
    }
}