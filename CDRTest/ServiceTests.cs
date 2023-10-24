using CDRModel;
using CDRServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CDRTest
{
    public class ServiceTests
    {
        private const string DefaultCallerId = "abcd";
        private const string DefaultReference = "12345";

        private static async Task<CDRRepository> GetRepository()
        {
            var logger = new LoggerFactory().CreateLogger<CDRRepository>();

            var options = new DbContextOptionsBuilder<CDRContext>().UseInMemoryDatabase("CDR").Options;

            var context = new CDRContext(options);

            await context.Database.EnsureCreatedAsync();

            var repository = new CDRRepository(context, logger);

            return repository;
        }

        [Fact]
        public async Task CanSave()
        {
            var repository = await GetRepository();

            var saved = await repository.Save(new[] { new Call { CallDate = DateTime.UtcNow, CallerId = DefaultCallerId, Cost = 10.5M, Currency = "GBP", Duration = 10, EndTime = new TimeSpan(0, 10, 0), Recipient = "xxxxxx", Reference = DefaultReference, Type = CallType.Domestic } });

            Assert.Equal(1, saved);
        }
    }
}
