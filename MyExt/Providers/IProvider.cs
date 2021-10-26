using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExt.Providers
{
    public enum ProviderType : byte
    {
        Unknow = 0,
        SQLite = 1,
        MySQL = 2,
        MSSQL = 3,
        API = 255
    }

    public record Map
    {
        public string Head { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public string Alias { get; set; }

        public long Offset { get; set; }
    }

    public record HeadMap
    {
        public string Type { get; set; }

        public string[] Head { get; set; }

        public string Description { get; set; }

        public string[] Alias { get; set; }

        public long Offset { get; set; }

        public HeadMap(Map map)
        {
            Head = map.Head.Split(' ');
            Description = map.Description;           
            Type = map.Type;
            Alias = map.Alias?.Split(';');
            Offset = map.Offset;
        }
    }

    public interface IProvider
    {
        Task<List<HeadMap>> GetHeadMapList();
    }
}
