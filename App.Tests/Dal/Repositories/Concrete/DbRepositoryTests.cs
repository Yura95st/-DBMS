namespace App.Tests.Dal.Repositories.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using App.Dal.Repositories.Abstract;
    using App.Dal.Repositories.Concrete;
    using App.Exceptions;
    using App.Models;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class DbRepositoryTests
    {
        private readonly string _tempDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DBMS_unit_tests_temp_folder/");

        private DbRepositorySettings _dbRepositorySettings;

        private Database _testDb;

        [Test]
        public void Create_DatabaseIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Create(null));
        }

        [Test]
        public void Create_DatabaseIsValid_CreatesNewDatabase()
        {
            // Arrange
            Row row = new Row { Id = 0, Value = { "someValue" } };

            Table table = new Table
            {
                Name = "someTable", NextRowId = row.Id + 1,
                Attributes = { new Models.Attribute { Name = "firstAttribute", Type = "someType" } },
                Rows = { { row.Id, row } }
            };

            Database database = new Database
                { Name = "testDatabase", Tables = new Dictionary<string, Table> { { table.Name, table } } };

            string dbFilePath = this.GetDbFilePath(database.Name);

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act
            target.Create(database);

            // Assert
            Assert.IsTrue(File.Exists(dbFilePath));

            string dbJson = File.ReadAllText(dbFilePath);
            Assert.AreEqual(JsonConvert.DeserializeObject<Database>(dbJson), database);
        }

        [Test]
        public void Create_DatabaseWithSuchNameAlreadyExists_ThrowsDbRepositoryException()
        {
            // Arrange
            Database database = new Database { Name = this._testDb.Name };

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.Throws<DbRepositoryException>(() => target.Create(database));
        }

        [Test]
        public void Delete_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Delete(null));
            Assert.Throws<ArgumentException>(() => target.Delete(""));
        }

        [Test]
        public void Delete_DatabaseNameIsValid_DeletesDatabase()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string dbFilePath = this.GetDbFilePath(dbName);

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act
            target.Delete(dbName);

            // Assert
            Assert.IsFalse(File.Exists(dbFilePath));
        }

        [Test]
        public void Delete_NonexistentDatabaseName_DoesNotThrowAnyException()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.DoesNotThrow(() => target.Delete(dbName));
        }

        [TearDown]
        public void Dispose()
        {
            Directory.Delete(this._tempDirectory, true);
        }

        [Test]
        public void GetAllNames_DatabasesDirectoryDoesNotExists_ReturnsEmptyList()
        {
            // Arrange
            Directory.Delete(this.GetDatabasesDirectoryPath(), true);

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act
            IEnumerable<string> dbNames = target.GetAllNames();

            // Assert
            Assert.IsFalse(dbNames.Any());
        }

        [Test]
        public void GetAllNames_ReturnsDatabasesNamesList()
        {
            // Arrange
            List<string> testDbNames = new List<string>();
            foreach (string fileName in Directory.EnumerateFiles(this.GetDatabasesDirectoryPath()))
            {
                testDbNames.Add(Path.GetFileNameWithoutExtension(fileName));
            }

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act
            IEnumerable<string> dbNames = target.GetAllNames();

            // Assert
            Assert.AreEqual(dbNames, testDbNames);
        }

        [Test]
        public void GetByName_DatabaseFileIsCorrupted_ThrowsDbRepositoryException()
        {
            // Arrange
            string dbName = "testDatabase";

            File.WriteAllText(this.GetDbFilePath(dbName), "some text");

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            DbRepositoryException ex = Assert.Throws<DbRepositoryException>(() => target.GetByName(dbName));

            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf(typeof(JsonException), ex.InnerException);
        }

        [Test]
        public void GetByName_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetByName(null));
            Assert.Throws<ArgumentException>(() => target.GetByName(""));
        }

        [Test]
        public void GetByName_DatabaseNameIsValid_ReturnsDatabase()
        {
            // Arrange
            Database testDatabase = this._testDb;

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act
            Database database = target.GetByName(testDatabase.Name);

            // Assert
            Assert.AreEqual(database, testDatabase);
        }

        [Test]
        public void GetByName_NonexistentDatabaseName_ReturnsNull()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.IsNull(target.GetByName(dbName));
        }

        [SetUp]
        public void Init()
        {
            this._dbRepositorySettings = new DbRepositorySettings(storagePath: Path.Combine(this._tempDirectory),
                dbFileNameFormat: "{0}.db", databasesDirectoryName: "databases");

            this.InitDatabase();
        }

        [Test]
        public void Update_DatabaseIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.Update(null));
        }

        [Test]
        public void Update_DatabaseIsValid_UpdatesDatabase()
        {
            // Arrange
            Database database = new Database { Name = this._testDb.Name };
            string dbFilePath = this.GetDbFilePath(database.Name);

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act
            target.Update(database);

            // Assert
            string dbJson = File.ReadAllText(dbFilePath);
            Assert.AreEqual(JsonConvert.DeserializeObject<Database>(dbJson), database);
        }

        [Test]
        public void Update_NonexistentDatabaseName_ThrowsDbRepositoryException()
        {
            // Arrange
            Database database = new Database { Name = "testDatabase" };

            // Arrange - create target
            IDbRepository target = new DbRepository(this._dbRepositorySettings);

            // Act and Assert
            Assert.Throws<DbRepositoryException>(() => target.Update(database));
        }

        private string GetDatabasesDirectoryPath()
        {
            return Path.Combine(this._dbRepositorySettings.StoragePath, this._dbRepositorySettings.DatabasesDirectoryName);
        }

        private string GetDbFilePath(string dbName)
        {
            return Path.Combine(this._dbRepositorySettings.StoragePath, this._dbRepositorySettings.DatabasesDirectoryName,
                string.Format(this._dbRepositorySettings.DbFileNameFormat, dbName));
        }

        private void InitDatabase()
        {
            Row row = new Row { Id = 1, Value = { "someValue" } };

            Table table = new Table
            {
                Name = "someTable", NextRowId = row.Id + 1,
                Attributes = { new Models.Attribute { Name = "firstAttribute", Type = "someType" } },
                Rows = { { row.Id, row } }
            };

            this._testDb = new Database
                { Name = "someDatabase", Tables = new Dictionary<string, Table> { { table.Name, table } } };

            Directory.CreateDirectory(this.GetDatabasesDirectoryPath());

            string dbFilePath = this.GetDbFilePath(this._testDb.Name);
            string json = JsonConvert.SerializeObject(this._testDb);

            File.WriteAllText(dbFilePath, json);
        }
    }
}