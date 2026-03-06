using Npgsql;

namespace Stratsphere.DataImport.Importers;

public interface IImporter
{
    string TableName { get; }
    Task<int> ImportAsync(NpgsqlConnection conn, string sourceDir);
}
