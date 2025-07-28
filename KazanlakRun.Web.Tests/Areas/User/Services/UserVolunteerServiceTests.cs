using AutoMapper;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.User.Models;
using KazanlakRun.Web.Areas.User.Services;
using KazanlakRun.Web.MappingProfiles;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Tests.Areas.User.Services
{
    [TestFixture]
    public class UserVolunteerServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options = null!;
        private ApplicationDbContext _db = null!;
        private IRepository<Volunteer> _volunteerRepo = null!;
        private VolunteerService _svc = null!;
        private IMapper _mapper = null!;

        private const string UserId = "user-abc";

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new ApplicationDbContext(_options);

            _volunteerRepo = new Repository<Volunteer>(_db);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VolunteerProfile>();
            });
            _mapper = config.CreateMapper();

            _svc = new VolunteerService(_volunteerRepo, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task ExistsAsync_ReturnsFalse_WhenNoVolunteer()
        {
            bool exists = await _svc.ExistsAsync(UserId);
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task CreateAsync_AddsVolunteerToDatabase()
        {
            var input = new VolunteerInputModel
            {
                Names = "Alice",
                Email = "alice@example.com",
                Phone = "555-1234"
            };

            await _svc.CreateAsync(UserId, input);

            var v = await _db.Volunteers.FirstOrDefaultAsync(x => x.UserId == UserId);
            Assert.IsNotNull(v);
            Assert.AreEqual("Alice", v!.Names);
            Assert.AreEqual("alice@example.com", v.Email);
            Assert.AreEqual("555-1234", v.Phone);
            Assert.AreEqual(1, v.AidStationId);
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsNull_IfNotFound()
        {
            var result = await _svc.GetByUserIdAsync("no-such");
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsInputModel_WhenFound()
        {
            await _db.Volunteers.AddAsync(new Volunteer
            {
                UserId = UserId,
                Names = "Bob",
                Email = "bob@example.com",
                Phone = "999-0000"
            });
            await _db.SaveChangesAsync();

            var result = await _svc.GetByUserIdAsync(UserId);
            Assert.IsNotNull(result);
            Assert.AreEqual("Bob", result!.Names);
            Assert.AreEqual("bob@example.com", result.Email);
            Assert.AreEqual("999-0000", result.Phone);
        }

        [Test]
        public void UpdateAsync_Throws_WhenNotFound()
        {
            var input = new VolunteerInputModel();
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _svc.UpdateAsync(UserId, input)
            );
        }

        [Test]
        public async Task UpdateAsync_ChangesProperties_WhenFound()
        {
            var v = new Volunteer
            {
                UserId = UserId,
                Names = "Old",
                Email = "o@e.com",
                Phone = "000"
            };
            await _db.Volunteers.AddAsync(v);
            await _db.SaveChangesAsync();

            var input = new VolunteerInputModel
            {
                Names = "New",
                Email = "n@e.com",
                Phone = "111"
            };

            await _svc.UpdateAsync(UserId, input);

            var updated = await _db.Volunteers.FirstAsync(x => x.UserId == UserId);
            Assert.AreEqual("New", updated.Names);
            Assert.AreEqual("n@e.com", updated.Email);
            Assert.AreEqual("111", updated.Phone);
        }

        [Test]
        public async Task DeleteAsync_DoesNothing_WhenNotFound()
        {
            await _svc.DeleteAsync("unknown");
            Assert.Pass();
        }
    }
}
