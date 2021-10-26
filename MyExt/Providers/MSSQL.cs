using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace MyExt.Providers
{
    public class MSSQL: DatabaseProvider
    {
        protected string Sql_CreateHeadTable => throw new NotImplementedException();

        protected override string Sql_GetHeadMapList => throw new NotImplementedException();

        public MSSQL(IConfiguration configuration) : base(configuration)
        {
            string db = Configuration.GetValue<string>(nameof(db));
            if (string.IsNullOrEmpty(db))
            {
                throw new ApplicationException(nameof(db));
            }
            CreateConnection = () => new SqlConnection(db);
            Initialize();
        }

        private void Initialize()
        {
            using IDbConnection db = CreateConnection();
            db.Execute(Sql_CreateHeadTable);
        }
    }
}
