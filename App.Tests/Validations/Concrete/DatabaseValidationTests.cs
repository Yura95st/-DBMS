namespace App.Tests.Validations.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using App.Exceptions;
    using App.Models;
    using App.Validations.Concrete;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseValidationTests
    {
        private DatabaseValidationSettings _dbValidationSettings;

        [Test]
        public void CheckTableScheme_TableSchemeHasAttributeWithInvalidName_ThrowsInvalidTableAttributesException()
        {
            // Arrange
            string tableName = "testTable";
            string[] attributeNames = { null, "", " ", new string(Path.GetInvalidFileNameChars()) };
            string attributeType = this._dbValidationSettings.DataTypes.Keys.First();

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (string attributeName in attributeNames)
            {
                Models.Attribute attribute = new Models.Attribute { Name = attributeName, Type = attributeType };

                TableScheme tableScheme = new TableScheme(tableName, new List<Models.Attribute> { attribute });

                InvalidTableAttributesException ex =
                    Assert.Throws<InvalidTableAttributesException>(() => target.CheckTableScheme(tableScheme));

                Assert.NotNull(ex.InnerException);
                Assert.AreSame(ex.InnerException.GetType(), typeof(InvalidAttributeException));
            }
        }

        [Test]
        public void CheckTableScheme_TableSchemeHasAttributeWithUnknownType_ThrowsInvalidAttributeException()
        {
            // Arrange
            string tableName = "testTable";
            string attributeName = "testAttribute";
            string[] attributeTypes = { null, "testType" };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (string attributeType in attributeTypes)
            {
                Models.Attribute attribute = new Models.Attribute { Name = attributeName, Type = attributeType };

                TableScheme tableScheme = new TableScheme(tableName, new List<Models.Attribute> { attribute });

                InvalidTableAttributesException ex =
                    Assert.Throws<InvalidTableAttributesException>(() => target.CheckTableScheme(tableScheme));

                Assert.NotNull(ex.InnerException);
                Assert.AreSame(ex.InnerException.GetType(), typeof(InvalidAttributeException));
            }
        }

        [Test]
        public void CheckTableScheme_TableSchemeHasInvalidName_ThrowsInvalidTableNameException()
        {
            // Arrange
            string[] tableNames = { "", " ", new string(Path.GetInvalidFileNameChars()) };
            Models.Attribute[] attributes =
            {
                new Models.Attribute { Name = "testAttribute", Type = this._dbValidationSettings.DataTypes.Keys.First() }
            };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (string tableName in tableNames)
            {
                TableScheme tableScheme = new TableScheme(tableName, attributes);

                Assert.Throws<InvalidTableNameException>(() => target.CheckTableScheme(tableScheme));
            }
        }

        [Test]
        public void CheckTableScheme_TableSchemeHasNoAttributes_ThrowsInvalidTableAttributesException()
        {
            // Arrange
            TableScheme tableScheme = new TableScheme("testTable", new List<Models.Attribute>());

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<InvalidTableAttributesException>(() => target.CheckTableScheme(tableScheme));
        }

        [Test]
        public void CheckTableScheme_TableSchemeIsNull_ThrowsArgumentNullException()
        {
            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.CheckTableScheme(null));
        }

        [Test]
        public void CheckTableScheme_TableSchemeIsValid_DoesNotThrowAnyException()
        {
            // Arrange
            TableScheme tableScheme = new TableScheme("testTable",
                new List<Models.Attribute>
                {
                    new Models.Attribute { Name = "testAttribute", Type = this._dbValidationSettings.DataTypes.Keys.First() }
                });

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.DoesNotThrow(() => target.CheckTableScheme(tableScheme));
        }

        [Test]
        public void DoesRowFitTable_ArgumentsAreNull_ThrowsArgumentNullException()
        {
            // Arrange
            Table table = new Table();
            Row row = new Row();

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => target.DoesRowFitTable(null, row));
            Assert.Throws<ArgumentNullException>(() => target.DoesRowFitTable(table, null));
        }

        [Test]
        public void DoesRowFitTable_RowDoesNotFitTableSize_ReturnsFalse()
        {
            // Arrange
            Table table = new Table
            {
                Name = "testTable",
                Attributes =
                    new List<Models.Attribute>
                    {
                        new Models.Attribute
                            { Name = "testAttribute", Type = this._dbValidationSettings.DataTypes.Keys.First() }
                    }
            };
            List<Row> rows = new List<Row>
                { new Row { Value = new List<string> { "1", "2" } }, new Row { Value = new List<string>() } };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (Row row in rows)
            {
                Assert.IsFalse(target.DoesRowFitTable(table, row));
            }
        }

        [Test]
        public void DoesRowFitTable_RowFitsTable_ReturnsTrue()
        {
            // Arrange
            Table table = new Table
            {
                Name = "testTable",
                Attributes =
                    new List<Models.Attribute>
                    {
                        new Models.Attribute
                            { Name = "testAttribute", Type = this._dbValidationSettings.DataTypes.Keys.First() }
                    }
            };
            Row row = new Row { Value = new List<string> { "1" } };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.IsTrue(target.DoesRowFitTable(table, row));
        }

        [Test]
        public void DoesRowFitTable_RowHasInvalidValue_ReturnsFalse()
        {
            // Arrange
            Table table = new Table
            {
                Name = "testTable",
                Attributes =
                    new List<Models.Attribute>
                    {
                        new Models.Attribute
                            { Name = "testAttribute", Type = this._dbValidationSettings.DataTypes.Keys.First() }
                    }
            };
            Row row = new Row { Value = new List<string> { "1234sometext" } };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.IsFalse(target.DoesRowFitTable(table, row));
        }

        [SetUp]
        public void Init()
        {
            this._dbValidationSettings =
                new DatabaseValidationSettings(dataTypes:
                    new Dictionary<string, Regex> { { "someType", new Regex(@"^-*[0-9]+$") } });
        }

        [Test]
        public void IsValidDatabaseName_DatabaseNameIsInvalid_ReturnsFalse()
        {
            // Arrange
            string[] dbNames = { null, "", " ", new string(Path.GetInvalidFileNameChars()) };

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            foreach (string dbName  in dbNames)
            {
                Assert.IsFalse(target.IsValidDatabaseName(dbName));
            }
        }

        [Test]
        public void IsValidDatabaseName_DatabaseNameIsValid_ReturnsTrue()
        {
            // Arrange
            string dbName = "testDatabase";

            // Arrange - create target
            DatabaseValidation target = new DatabaseValidation(this._dbValidationSettings);

            // Act and Assert
            Assert.IsTrue(target.IsValidDatabaseName(dbName));
        }
    }
}