using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace MyExt.Providers
{
    public class SQLite: DatabaseProvider
    {
        

        protected string Sql_CreateHeadTable => "CREATE TABLE IF NOT EXISTS \"head\" (\"offset\" INTEGER NOT NULL DEFAULT 0,\"head\" TEXT NOT NULL,\"type\" TEXT NOT NULL,\"description\" TEXT NOT NULL,\"alias\" TEXT,PRIMARY KEY(\"offset\",\"head\"));";

        protected override string Sql_GetHeadMapList => "SELECT \"offset\",\"head\",\"type\",\"alias\",\"description\" FROM \"head\" ORDER BY LENGTH(\"head\") DESC,\"offset\" DESC;";

        public SQLite(IConfiguration configuration) : base(configuration)
        {
            string db = Configuration.GetValue(nameof(db), $"{nameof(MyExt)}.sqlite3");
            if (!Path.IsPathFullyQualified(db))
            {
                db = Path.Combine(AppContext.BaseDirectory, db);
            }
            if (string.IsNullOrEmpty(db))
            {
                throw new ApplicationException(nameof(db));
            }
            Console.WriteLine(db);
            CreateConnection = () => new SqliteConnection($"Data Source={db};");
            Initialize();
        }

        private void Initialize()
        {
            using IDbConnection db = CreateConnection();
            db.Execute(Sql_CreateHeadTable);
        }
    }
}
