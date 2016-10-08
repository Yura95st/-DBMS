namespace App.Tests.Services.Concrete
{
    using System;
    using System.IO;

    using App.Exceptions;
    using App.Models;
    using App.Services.Abstract;
    using App.Services.Concrete;
    using App.Validations.Abstract;

    using Moq;

    using Newtonsoft.Json;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseServiceTests
    {
        private readonly string _tempDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DBMS_unit_tests_temp_folder");

        private DatabaseServiceSettings _dbServiceSettings;

        private Mock<IDatabaseValidation> _dbValidationMock;

        [Test]
        public void CreateDatabase_DatabaseNameIsInvalid_ThrowsInvalidNameFormatException()
        {
            // Arrange
            string dbName = "testDatabase";
            string dbPath = Path.Combine(this._dbServiceSettings.StoragePath, dbName);

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.IsValidDatabaseName(dbName))
                .Returns(false);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidNameFormatException>(() => target.CreateDatabase(dbName));

            Assert.IsFalse(Directory.Exists(dbPath));

            this._dbValidationMock.Verify(v => v.IsValidDatabaseName(dbName), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateDatabase(null));
        }

        [Test]
        public void CreateDatabase_DatabaseNameIsValid_CreatesNewDatabaseAndReturnsDatabaseObject()
        {
            // Arrange
            string dbName = "testDatabase";
            string dbPath = Path.Combine(this._dbServiceSettings.StoragePath, dbName);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            target.CreateDatabase(dbName);

            // Assert
            Assert.IsTrue(Directory.Exists(dbPath));

            this._dbValidationMock.Verify(v => v.IsValidDatabaseName(dbName), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseWithSuchNameAlreadyExists_ThrowsDatabaseAlreadyExistsException()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.IsValidDatabaseName(dbName))
                .Returns(true);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            target.CreateDatabase(dbName);

            // Act and Assert
            Assert.Throws<DatabaseAlreadyExistsException>(() => target.CreateDatabase(dbName));
        }

        [Test]
        public void CreateTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.CreateTable(dbName, new Table()));
        }

        [Test]
        public void CreateTable_TableIsInvalid_ThrowsInvalidTableException()
        {
            // Arrange
            string dbName = "testDatabase";
            Table table = new Table { Name = "testTable" };

            string tablePath = Path.Combine(this._dbServiceSettings.StoragePath, dbName,
                this._dbServiceSettings.TablesDirectoryName,
                string.Format(this._dbServiceSettings.TableFileNameFormat, table.Name));

            Exception innerException = new Exception();

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.CheckTable(table))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            target.CreateDatabase(dbName);

            // Act and Assert
            InvalidTableException exeption = Assert.Throws<InvalidTableException>(() => target.CreateTable(dbName, table));
            Assert.AreSame(exeption.InnerException, innerException);

            Assert.IsFalse(File.Exists(tablePath));

            this._dbValidationMock.Verify(v => v.CheckTable(table), Times.Once);
        }

        [Test]
        public void CreateTable_TableIsValid_CreatesNewTable()
        {
            // Arrange
            string dbName = "testDatabase";
            Table table = new Table
                { Name = "testTable", Attributes = { new Models.Attribute { Name = "firstAttribute", Type = "someType" } } };

            string tablePath = Path.Combine(this._dbServiceSettings.StoragePath, dbName,
                this._dbServiceSettings.TablesDirectoryName,
                string.Format(this._dbServiceSettings.TableFileNameFormat, table.Name));

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            target.CreateDatabase(dbName);

            // Act
            target.CreateTable(dbName, table);

            // Assert
            string tableJson = File.ReadAllText(tablePath);
            Assert.AreEqual(JsonConvert.DeserializeObject<Table>(tableJson), table);

            this._dbValidationMock.Verify(v => v.CheckTable(table), Times.Once);
        }

        [Test]
        public void CreateTable_TableWithSuchNameAlreadyExists_ThrowsTableAlreadyExistsException()
        {
            // Arrange
            string dbName = "testDatabase";
            Table table = new Table { Name = "testTable" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            target.CreateDatabase(dbName);
            target.CreateTable(dbName, table);

            // Act and Assert
            Assert.Throws<TableAlreadyExistsException>(() => target.CreateTable(dbName, table));
        }

        [TearDown]
        public void Dispose()
        {
            Directory.Delete(this._tempDirectory, true);
        }

        [Test]
        public void DropDatabase_DatabaseNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DropDatabase(null));
        }

        [Test]
        public void DropDatabase_DatabaseNameIsValid_DropsDatabase()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            target.CreateDatabase(dbName);

            // Act
            target.DropDatabase(dbName);

            // Assert
            Assert.IsNull(target.GetDatabase(dbName));
        }

        [Test]
        public void DropDatabase_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.DropDatabase(dbName));
        }

        [Test]
        public void GetDatabase_DatabaseNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetDatabase(null));
        }

        [Test]
        public void GetDatabase_DatabaseNameIsValid_ReturnsDatabase()
        {
            // Arrange
            Database testDb = new Database { Name = "testDatabase", TableNames = new[] { "firstTable", "secondTable" } };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            target.CreateDatabase(testDb.Name);

            foreach (string tableName in testDb.TableNames)
            {
                target.CreateTable(testDb.Name, new Table { Name = tableName });
            }

            // Act
            Database db = target.GetDatabase(testDb.Name);

            // Assert
            Assert.AreEqual(db, testDb);
        }

        [Test]
        public void GetDatabase_NonexistentDatabaseName_ReturnsNull()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            Database db = target.GetDatabase(dbName);

            // Assert
            Assert.IsNull(db);
        }

        [SetUp]
        public void Init()
        {
            this.MockNameValidation();

            this._dbServiceSettings = new DatabaseServiceSettings
            {
                StoragePath = Path.Combine(this._tempDirectory, @"databases\"), TableFileNameFormat = "{0}.json",
                TablesDirectoryName = "tables"
            };

            Directory.CreateDirectory(this._dbServiceSettings.StoragePath);
        }

        private void MockNameValidation()
        {
            this._dbValidationMock = new Mock<IDatabaseValidation>();

            this._dbValidationMock.Setup(v => v.IsValidDatabaseName(It.IsAny<string>()))
                .Returns(true);
        }
    }
}