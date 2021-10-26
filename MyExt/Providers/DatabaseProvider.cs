using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace MyExt.Providers
{
    

    public abstract class DatabaseProvider : IProvider
    {
        protected Func<IDbConnection> CreateConnection { get; set; }

        protected IConfiguration Configuration { get; set; }

        protected abstract string Sql_GetHeadMapList { get; }

        public DatabaseProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        
        public async Task<List<HeadMap>> GetHeadMapList()
        {
            List<HeadMap> list = new();
            try
            {
                using IDbConnection connection = CreateConnection();
                list.AddRange((await connection.QueryAsync<Map>(Sql_GetHeadMapList)).Select(x=>new HeadMap(x)));
            }
            catch
            {
                list.Clear();
            }
            return list;
        }
    }
}
