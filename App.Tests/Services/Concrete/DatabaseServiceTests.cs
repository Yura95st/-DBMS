namespace App.Tests.Services.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DBMS_unit_tests_temp_folder/");

        private DatabaseServiceSettings _dbServiceSettings;

        private Mock<IDatabaseValidation> _dbValidationMock;

        private Database _testDb;

        private Table _testTable;

        [Test]
        public void AddRow_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.AddRow(null, tableName, row));
            Assert.Throws<ArgumentNullException>(() => target.AddRow(dbName, null, row));
            Assert.Throws<ArgumentNullException>(() => target.AddRow(dbName, tableName, null));

            Assert.Throws<ArgumentException>(() => target.AddRow("", tableName, row));
            Assert.Throws<ArgumentException>(() => target.AddRow(dbName, "", row));
        }

        [Test]
        public void AddRow_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.AddRow(dbName, tableName, row));
        }

        [Test]
        public void AddRow_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.AddRow(dbName, tableName, row));
        }

        [Test]
        public void AddRow_RowIsInvalid_ThrowsIvalidRowException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row();

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.DoesRowFitTable(table, row))
                .Returns(false);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<InvalidRowException>(() => target.AddRow(dbName, table.Name, row));

            // Table was not changed.
            Assert.AreEqual(target.GetTable(dbName, table.Name), this._testTable);

            this._dbValidationMock.Verify(v => v.DoesRowFitTable(table, row), Times.Once);
        }

        [Test]
        public void AddRow_RowIsValid_AddsNewRowAndUpdatesTableNextRowIdCounter()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row { Value = { "valueOne", "valueTwo" } };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            target.AddRow(dbName, table.Name, row);

            // Assert
            row.Id = table.NextRowId;
            Table updatedTable = target.GetTable(dbName, table.Name);

            Assert.AreEqual(updatedTable.Rows[row.Id], row);
            Assert.AreEqual(updatedTable.NextRowId, row.Id + 1);
        }

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
        public void CreateDatabase_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateDatabase(null));
            Assert.Throws<ArgumentException>(() => target.CreateDatabase(""));
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
        }

        [Test]
        public void CreateDatabase_DatabaseWithSuchNameAlreadyExists_ThrowsDatabaseAlreadyExistsException()
        {
            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseAlreadyExistsException>(() => target.CreateDatabase(this._testDb.Name));
        }

        [Test]
        public void CreateTable_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CreateTable(null, tableScheme));
            Assert.Throws<ArgumentNullException>(() => target.CreateTable(dbName, null));

            Assert.Throws<ArgumentException>(() => target.CreateTable("", tableScheme));
        }

        [Test]
        public void CreateTable_ArgumentsAreValid_CreatesNewTable()
        {
            // Arrange
            string dbName = this._testDb.Name;
            TableScheme tableScheme = new TableScheme("testTable",
                new List<Models.Attribute> { new Models.Attribute { Name = "firstAttribute", Type = "someType" } });

            string tablePath = Path.Combine(this._dbServiceSettings.StoragePath, dbName,
                this._dbServiceSettings.TablesDirectoryName,
                string.Format(this._dbServiceSettings.TableFileNameFormat, tableScheme.Name));

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            target.CreateTable(dbName, tableScheme);

            // Assert
            Database db = target.GetDatabase(dbName);
            Assert.IsTrue(db.TableNames.Contains(tableScheme.Name));

            Table table = new Table { Name = tableScheme.Name, Attributes = tableScheme.Attributes };
            string tableJson = File.ReadAllText(tablePath);

            Assert.AreEqual(JsonConvert.DeserializeObject<Table>(tableJson), table);
        }

        [Test]
        public void CreateTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.CreateTable(dbName, tableScheme));
        }

        [Test]
        public void CreateTable_TableSchemeIsInvalid_ThrowsInvalidTableSchemeException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            string tablePath = Path.Combine(this._dbServiceSettings.StoragePath, dbName,
                this._dbServiceSettings.TablesDirectoryName,
                string.Format(this._dbServiceSettings.TableFileNameFormat, tableScheme.Name));

            Exception innerException = new Exception();

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.CheckTableScheme(tableScheme))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            InvalidTableSchemeException exeption =
                Assert.Throws<InvalidTableSchemeException>(() => target.CreateTable(dbName, tableScheme));
            Assert.AreSame(exeption.InnerException, innerException);

            Assert.IsFalse(File.Exists(tablePath));

            this._dbValidationMock.Verify(v => v.CheckTableScheme(tableScheme), Times.Once);
        }

        [Test]
        public void CreateTable_TableWithSuchNameAlreadyExists_ThrowsTableAlreadyExistsException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            TableScheme tableScheme = new TableScheme(this._testTable.Name, new List<Models.Attribute>());

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableAlreadyExistsException>(() => target.CreateTable(dbName, tableScheme));
        }

        [Test]
        public void DeleteRow_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DeleteRow(null, tableName, rowId));
            Assert.Throws<ArgumentNullException>(() => target.DeleteRow(dbName, null, rowId));

            Assert.Throws<ArgumentException>(() => target.DeleteRow("", tableName, rowId));
            Assert.Throws<ArgumentException>(() => target.DeleteRow(dbName, "", rowId));
            Assert.Throws<ArgumentException>(() => target.DeleteRow(dbName, "", -1));
        }

        [Test]
        public void DeleteRow_ArgumentsAreValid_DeletesRow()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = this._testTable.Name;
            int rowId = this._testTable.Rows.First()
                .Key;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            target.DeleteRow(dbName, tableName, rowId);

            // Assert
            Table updatedTable = target.GetTable(dbName, tableName);

            Assert.IsFalse(updatedTable.Rows.ContainsKey(rowId));
        }

        [Test]
        public void DeleteRow_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.DeleteRow(dbName, tableName, rowId));
        }

        [Test]
        public void DeleteRow_NonexistentRowId_ThrowsRowNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = this._testTable.Name;
            int rowId = this._testTable.NextRowId;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<RowNotFoundException>(() => target.DeleteRow(dbName, tableName, rowId));
        }

        [Test]
        public void DeleteRow_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            int rowId = 1;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.DeleteRow(dbName, tableName, rowId));
        }

        [TearDown]
        public void Dispose()
        {
            Directory.Delete(this._tempDirectory, true);
        }

        [Test]
        public void DropDatabase_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DropDatabase(null));
            Assert.Throws<ArgumentException>(() => target.DropDatabase(""));
        }

        [Test]
        public void DropDatabase_DatabaseNameIsValid_DropsDatabase()
        {
            // Arrange
            string dbName = this._testDb.Name;

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

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
        public void DropTable_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "tableName";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DropTable(null, tableName));
            Assert.Throws<ArgumentNullException>(() => target.DropTable(dbName, null));

            Assert.Throws<ArgumentException>(() => target.DropTable("", tableName));
            Assert.Throws<ArgumentException>(() => target.DropTable(dbName, ""));
        }

        [Test]
        public void DropTable_ArgumentsAreValid_DropsTable()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = new Table { Name = this._testTable.Name };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            target.DropTable(dbName, table.Name);

            // Assert
            Assert.IsNull(target.GetTable(dbName, table.Name));
        }

        [Test]
        public void DropTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.DropTable(dbName, tableName));
        }

        [Test]
        public void DropTable_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.DropTable(dbName, tableName));
        }

        [Test]
        public void GetDatabase_DatabaseNameIsNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetDatabase(null));
            Assert.Throws<ArgumentException>(() => target.GetDatabase(""));
        }

        [Test]
        public void GetDatabase_DatabaseNameIsValid_ReturnsDatabase()
        {
            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            Database db = target.GetDatabase(this._testDb.Name);

            // Assert
            Assert.AreEqual(db, this._testDb);
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

        [Test]
        public void GetDatabaseNames_ReturnsListOfDatabaseNames()
        {
            // Arrange
            string[] testDbNames = { this._testDb.Name };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            IEnumerable<string> dbNames = target.GetDatabaseNames();

            // Assert
            Assert.IsTrue(dbNames.SequenceEqual(testDbNames));
        }

        [Test]
        public void GetTable_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "tableName";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetTable(null, tableName));
            Assert.Throws<ArgumentNullException>(() => target.GetTable(dbName, null));

            Assert.Throws<ArgumentException>(() => target.GetTable("", tableName));
            Assert.Throws<ArgumentException>(() => target.GetTable(dbName, ""));
        }

        [Test]
        public void GetTable_ArgumetnsAreValid_ReturnsTable()
        {
            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTable(this._testDb.Name, this._testTable.Name);

            // Assert
            Assert.AreEqual(table, this._testTable);
        }

        [Test]
        public void GetTable_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.GetTable(dbName, tableName));
        }

        [Test]
        public void GetTable_NonexistentTableName_ReturnsNull()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTable(dbName, tableName);

            // Assert
            Assert.IsNull(table);
        }

        [Test]
        public void GetTableProjection_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "someAttribute" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.GetTableProjection(null, tableName, attributesNames));
            Assert.Throws<ArgumentNullException>(() => target.GetTableProjection(dbName, null, attributesNames));
            Assert.Throws<ArgumentNullException>(() => target.GetTableProjection(dbName, tableName, null));

            Assert.Throws<ArgumentException>(() => target.GetTableProjection("", tableName, attributesNames));
            Assert.Throws<ArgumentException>(() => target.GetTableProjection(dbName, "", attributesNames));
            Assert.Throws<ArgumentException>(() => target.GetTableProjection(dbName, tableName, Enumerable.Empty<string>()));
        }

        [Test]
        public void GetTableProjection_ArgumetnsAreValid_ReturnsTable()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table testTable = this._testTable;
            string[] attributesNames =
            {
                testTable.Attributes.First()
                    .Name
            };

            for (int i = 1; i < testTable.Attributes.Count; i++)
            {
                testTable.Attributes.RemoveAt(i);

                foreach (Row row in testTable.Rows.Values)
                {
                    row.Value.RemoveAt(i);
                }
            }

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTableProjection(dbName, testTable.Name, attributesNames);

            // Assert
            Assert.AreEqual(table, testTable);
        }

        [Test]
        public void GetTableProjection_NonexistentAttributeName_ThrowsNonexistentAttributeException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = this._testTable.Name;
            string[] attributesNames =
            {
                this._testTable.Attributes.First()
                    .Name,
                "testAttribute"
            };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<NonexistentAttributeException>(() => target.GetTableProjection(dbName, tableName, attributesNames));
        }

        [Test]
        public void GetTableProjection_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            string[] attributesNames = { "testAttribute" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.GetTableProjection(dbName, tableName, attributesNames));
        }

        [Test]
        public void GetTableProjection_NonexistentTableName_ReturnsNull()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            string[] attributesNames = { "testAttribute" };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            Table table = target.GetTableProjection(dbName, tableName, attributesNames);

            // Assert
            Assert.IsNull(table);
        }

        [SetUp]
        public void Init()
        {
            this._dbServiceSettings =
                new DatabaseServiceSettings(storagePath: Path.Combine(this._tempDirectory, "databases/"),
                    tableFileNameFormat: "{0}.json", tablesDirectoryName: "tables");

            this.MockValidation();

            this.InitDatabase();
        }

        [Test]
        public void UpdateRow_ArgumentsAreNullOrEmpty_ThrowsArgumentExceptions()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.UpdateRow(null, tableName, row));
            Assert.Throws<ArgumentNullException>(() => target.UpdateRow(dbName, null, row));
            Assert.Throws<ArgumentNullException>(() => target.UpdateRow(dbName, tableName, null));

            Assert.Throws<ArgumentException>(() => target.UpdateRow("", tableName, row));
            Assert.Throws<ArgumentException>(() => target.UpdateRow(dbName, "", row));
        }

        [Test]
        public void UpdateRow_NonexistentDatabaseName_ThrowsDatabaseNotFoundException()
        {
            // Arrange
            string dbName = "testDatabase";
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<DatabaseNotFoundException>(() => target.UpdateRow(dbName, tableName, row));
        }

        [Test]
        public void UpdateRow_NonexistentRowId_ThrowsRowNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row { Id = table.NextRowId };

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<RowNotFoundException>(() => target.UpdateRow(dbName, table.Name, row));
        }

        [Test]
        public void UpdateRow_NonexistentTableName_ThrowsTableNotFoundException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            string tableName = "testTable";
            Row row = new Row();

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            Assert.Throws<TableNotFoundException>(() => target.UpdateRow(dbName, tableName, row));
        }

        [Test]
        public void UpdateRow_RowIsInvalid_ThrowsIvalidRowException()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;
            Row row = new Row();

            Exception innerException = new Exception();

            // Arrange - mock dbValidation
            this._dbValidationMock.Setup(v => v.DoesRowFitTable(table, row))
                .Throws(innerException);

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act and Assert
            InvalidRowException exeption = Assert.Throws<InvalidRowException>(() => target.UpdateRow(dbName, table.Name, row));
            Assert.AreSame(exeption.InnerException, innerException);

            // Table was not changed.
            Assert.AreEqual(target.GetTable(dbName, table.Name), this._testTable);

            this._dbValidationMock.Verify(v => v.DoesRowFitTable(table, row), Times.Once);
        }

        [Test]
        public void UpdateRow_RowIsValid_UpdatesRow()
        {
            // Arrange
            string dbName = this._testDb.Name;
            Table table = this._testTable;

            Row row = table.Rows.Values.First();
            row.Value[0] = "newValue";

            // Arrange - create target
            IDatabaseService target = new DatabaseService(this._dbServiceSettings, this._dbValidationMock.Object);

            // Act
            target.UpdateRow(dbName, table.Name, row);

            // Assert
            Table updatedTable = target.GetTable(dbName, table.Name);

            Assert.AreEqual(updatedTable.Rows[row.Id], row);
        }

        private void InitDatabase()
        {
            Row row = new Row { Id = 0, Value = { "someValue", "anotherValue" } };

            this._testTable = new Table
            {
                Name = "someTable", NextRowId = row.Id + 1,
                Attributes =
                {
                    new Models.Attribute { Name = "firstAttribute", Type = "someType" },
                    new Models.Attribute { Name = "secondAttribute", Type = "someType" }
                },
                Rows = { { row.Id, row } }
            };

            this._testDb = new Database { Name = "someDatabase", TableNames = new List<string> { this._testTable.Name } };

            Directory.CreateDirectory(this._dbServiceSettings.StoragePath);

            string dbPath = Path.Combine(this._dbServiceSettings.StoragePath, this._testDb.Name);
            Directory.CreateDirectory(dbPath);

            string tablesFolderPath = Path.Combine(dbPath, this._dbServiceSettings.TablesDirectoryName);
            Directory.CreateDirectory(tablesFolderPath);

            string tablePath = Path.Combine(tablesFolderPath,
                string.Format(this._dbServiceSettings.TableFileNameFormat, this._testTable.Name));
            File.WriteAllText(tablePath, JsonConvert.SerializeObject(this._testTable));
        }

        private void MockValidation()
        {
            this._dbValidationMock = new Mock<IDatabaseValidation>();

            this._dbValidationMock.Setup(v => v.IsValidDatabaseName(It.IsAny<string>()))
                .Returns(true);
            this._dbValidationMock.Setup(v => v.DoesRowFitTable(It.IsAny<Table>(), It.IsAny<Row>()))
                .Returns(true);
        }
    }
}