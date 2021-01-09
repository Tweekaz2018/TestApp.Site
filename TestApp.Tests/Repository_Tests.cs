using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Repo;
using TestApp.Tests.Helpers;

namespace TestApp.Tests
{
    [TestClass]
    public class Repository_Tests
    {
        IRepository<Category> repo;
        Context db;
        [TestInitialize]
        public void SetTests()
        {
            db = new Context(RepositoryHelper.TestDbContextOptions());
            RepositoryHelper.InitializeDbForTests(db);
            repo = new Repository<Category>(db);
        }

        [TestMethod]
        public async Task GetRepo_Test()
        {
            var entries = await repo.GetRepo().ToListAsync();

            Assert.IsNotNull(entries);
            Xunit.Assert.IsType<List<Category>>(entries);
        }

        [TestMethod]
        public async Task AddAsync_Test()
        {
            var count = await db.Set<Category>().CountAsync();
            var category = new Category()
            {
                name = "Test cat"
            };

            var id = repo.AddAsync(category);
            var newCount = await db.Set<Category>().CountAsync();

            Assert.AreNotEqual(count, newCount);                
        }

        [TestMethod]
        public async Task UpdateAsync_Test()
        {
            var cat = await db.Set<Category>().FirstAsync();
            string newName = "test";
            cat.name = newName;

            await repo.UpdateAsync(cat);

            var needToBeUpdated = await db.Set<Category>().SingleOrDefaultAsync(x => x.name == newName);
            Assert.IsNotNull(needToBeUpdated);
        }

        [TestMethod]
        public async Task RemoveAsync_Test()
        {
            var cat = await db.Set<Category>().FirstAsync();
            var setCount = await db.Set<Category>().CountAsync();

            await repo.RemoveAsync(cat);

            var afterRemove = await db.Set<Category>().CountAsync();

            Assert.AreNotEqual(setCount, afterRemove);
        }

        [TestMethod]
        public async Task AddTangeAsync_Test()
        {
            List<Category> cats = new List<Category>()
            {
                new Category()
                {
                    name = "Test"
                },
                new Category()
                {
                    name = "Tests"
                }
            };
            var catsCount = await db.Set<Category>().CountAsync();

            await repo.AddRangeAsync(cats);

            var afterAddCount = await db.Set<Category>().CountAsync();

            Assert.AreNotEqual(catsCount, afterAddCount);
        }

        [TestMethod]
        public async Task RemoveRangeAsync_Test()
        {
            var toDelete = db.Set<Category>().Where(x => x.Id == 1).ToList();
            var catsCount = await db.Set<Category>().CountAsync();

            await repo.RemoveRangeAsync(toDelete);

            var AfterDeleteCatsCount = await db.Set<Category>().CountAsync();

            Assert.AreNotEqual(catsCount, AfterDeleteCatsCount);


        }

        [TestCleanup]
        public void DownTests()
        {
            db.Dispose();
        }
    }
}
