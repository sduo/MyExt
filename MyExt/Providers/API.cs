using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExt.Providers
{
    public class API : IProvider
    {
        public Task<List<HeadMap>> GetHeadMapList()
        {
            throw new NotImplementedException();
        }
    }
}
