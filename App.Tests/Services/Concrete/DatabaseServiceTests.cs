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

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseServiceTests
    {
        private readonly string _tempDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"DBMS_knu_lab_work");

        private DatabaseServiceSettings _dbServiceSettings;

        private Mock<INameValidation> _nameValidationMock;

        [Test]
        public void CreateDatabase_DatabaseNameIsInvalid_ThrowsInvalidNameFormatException()
        {
            // Arrange
            string dbName = "testDatabase";
            string dbPath = Path.Combine(this._dbServiceSettings.StoragePath, dbName);

            // Arrange - mock nameValidation
            this._nameValidationMock.Setup(v => v.IsValidDatabaseName(dbName))
                .Returns(false);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidNameFormatException>(() => target.CreateDatabase(dbName));

            Assert.IsFalse(Directory.Exists(dbPath));

            this._nameValidationMock.Verify(v => v.IsValidDatabaseName(dbName), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

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
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

            // Act
            target.CreateDatabase(dbName);

            // Assert
            Assert.IsTrue(Directory.Exists(dbPath));

            this._nameValidationMock.Verify(v => v.IsValidDatabaseName(dbName), Times.Once);
        }

        [Test]
        public void CreateDatabase_DatabaseWithSuchNameAlreadyExists_ThrowsDatabaseAlreadyExistsException()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - mock nameValidation
            this._nameValidationMock.Setup(v => v.IsValidDatabaseName(dbName))
                .Returns(true);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

            target.CreateDatabase(dbName);

            // Act and Assert
            Assert.Throws<DatabaseAlreadyExistsException>(() => target.CreateDatabase(dbName));
        }

        [TearDown]
        public void Dispose()
        {
            Directory.Delete(this._tempDirectory, true);
        }

        [Test]
        public void GetDatabase_DatabaseNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetDatabase(null));
        }

        [Test]
        public void GetDatabase_DatabaseNameIsValid_ReturnsDatabase()
        {
            // Arrange
            Database testDb = new Database { Name = "testDatabase", TableNames = new[] { "firstTable", "secondTable" } };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

            target.CreateDatabase(testDb.Name);

            //foreach (string tableName in testDb.TableNames)
            //{
            //    target.CreateTable(testDb.Name, new Table { Name = tableName });
            //}

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
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._nameValidationMock.Object);

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
                { StoragePath = Path.Combine(this._tempDirectory, @"databases\") };

            Directory.CreateDirectory(this._dbServiceSettings.StoragePath);
        }

        private void MockNameValidation()
        {
            this._nameValidationMock = new Mock<INameValidation>();

            this._nameValidationMock.Setup(v => v.IsValidDatabaseName(It.IsAny<string>()))
                .Returns(true);
        }
    }
}