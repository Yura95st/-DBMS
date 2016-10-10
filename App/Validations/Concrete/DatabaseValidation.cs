namespace App.Validations.Concrete
{
    using System;
    using System.IO;

    using App.Exceptions;
    using App.Models;
    using App.Utils;
    using App.Validations.Abstract;

    public class DatabaseValidation : IDatabaseValidation
    {
        private readonly DatabaseValidationSettings _settings;

        public DatabaseValidation(DatabaseValidationSettings settings)
        {
            Guard.NotNull(settings, "settings");

            this._settings = settings;
        }

        #region IDatabaseValidation Members

        public void CheckRow(Table table, Row row)
        {
            throw new NotImplementedException();
        }

        public void CheckTableScheme(TableScheme tableScheme)
        {
            Guard.NotNull(tableScheme, "tableScheme");

            if (!DatabaseValidation.IsValidFileName(tableScheme.Name))
            {
                throw new InvalidTableSchemeException($"Table scheme has invalid name \"{tableScheme.Name}\".");
            }

            if (tableScheme.Attributes.Count == 0)
            {
                throw new InvalidTableSchemeException("Table scheme has no attributes.");
            }

            try
            {
                foreach (Models.Attribute attribute in tableScheme.Attributes)
                {
                    this.CheckAttribute(attribute);
                }
            }
            catch (InvalidAttributeException ex)
            {
                throw new InvalidTableSchemeException("Table scheme is invalid. See inner exception for details.", ex);
            }
        }

        public bool IsValidDatabaseName(string dbName)
        {
            return DatabaseValidation.IsValidFileName(dbName);
        }

        #endregion

        private void CheckAttribute(Models.Attribute attribute)
        {
            Guard.NotNull(attribute, "attribute");

            if (!DatabaseValidation.IsValidFileName(attribute.Name))
            {
                throw new InvalidAttributeException($"Attribute's name \"{attribute.Name}\" is invalid.");
            }

            if (attribute.Type == null || !this._settings.DataTypes.ContainsKey(attribute.Type))
            {
                throw new InvalidAttributeException($"Attribute's type \"{attribute.Type}\" is unknown.");
            }
        }

        private static bool IsValidFileName(string fileName)
        {
            return !string.IsNullOrWhiteSpace(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }
    }
}