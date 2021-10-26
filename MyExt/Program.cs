using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyExt.Providers;

namespace MyExt
{
    class Program
    {
        const int ErrorCode = -1;

        static IConfiguration Configuration;

        static IProvider Provider;

        static async Task Main(string[] args)
        {
            Console.WriteLine();

            Configuration = new ConfigurationBuilder().AddJsonFile($"{nameof(MyExt)}.json", true).AddCommandLine(args).Build();

            string input = Configuration.GetValue<string>(nameof(input));
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"请指定需要修复的文件");
                Environment.Exit(ErrorCode);
                return;
            }

            input = Path.GetFullPath(input);
            Console.WriteLine($"输入文件：{input}");
            if (!File.Exists(input))
            {
                Console.WriteLine($"文件不存在");
                Environment.Exit(ErrorCode);
                return;
            }

            ProviderType provider = Configuration.GetValue(nameof(provider), ProviderType.SQLite);
            Console.WriteLine($"诊断器：{provider}");

            switch (provider)
            {
                //case ProviderType.MySQL: { Provider = new MySQL(Configuration.GetSection(nameof(MySQL))); break; }
                case ProviderType.SQLite: { Provider = new SQLite(Configuration.GetSection(nameof(SQLite))); break; }
                //case ProviderType.MSSQL: { Provider = new MSSQL(Configuration.GetSection(nameof(MSSQL))); break; }                
                case ProviderType.API:
                default:
                    {
                        Console.WriteLine($"不被支持的诊断器：{provider}");
                        Environment.Exit(ErrorCode);
                        return;
                    }
            }

            List<HeadMap> maps = await Provider.GetHeadMapList();
            if (maps.Count == 0)
            {
                Console.WriteLine($"诊断器无有效数据");
                Environment.Exit(ErrorCode);
                return;
            }

            HeadMap map = GetMap(maps, input);
            Console.WriteLine();
            if (map == null)
            {
                Console.WriteLine($"未知文件");
                Environment.Exit(ErrorCode);
                return;
            }

            Console.WriteLine($"命中特征：{map.Description}({map.Type})");
            if (string.IsNullOrEmpty(map.Type))
            {
                Environment.Exit(0);
                return;
            }
            string ext = Path.GetExtension(input);
            if (ext.StartsWith('.'))
            {
                ext = ext[1..];
            }
            if (string.Equals(ext, map.Type, StringComparison.OrdinalIgnoreCase))
            {
                Environment.Exit(0);
                return;
            }
            if (map.Alias?.Any(x => string.Equals(ext, x, StringComparison.OrdinalIgnoreCase)) == true)
            {
                Console.WriteLine($"可接受文件类型：{ext}({string.Join(';', map.Alias)})");
                Environment.Exit(0);
                return;
            }
            string output = $"{input}.{map.Type}";
            try
            {
                File.Move(input, output);
                Console.WriteLine($"重命名文件成功：{output}");
                Environment.Exit(0);
            }
            catch
            {
                Console.WriteLine($"重命名文件失败");
                Environment.Exit(-2);
            }
        }

        static HeadMap GetMap(List<HeadMap> maps, string input)
        {
            using FileStream fs = new(input, FileMode.Open, FileAccess.Read);
            for(int i=0;i<maps.Count;++i)            
            {
                HeadMap map = maps[i];
                Console.Write($"{'\r'}扫描特征：{i+1}/{maps.Count}");

                if (fs.Length < map.Offset + map.Head.Length)
                {
                    continue;
                }
                byte[] data = new byte[map.Head.Length];
                fs.Seek(map.Offset, SeekOrigin.Begin);
                if (fs.Read(data, 0, data.Length) == 0)
                {
                    Console.WriteLine($"无法读取文件");
                    Environment.Exit(ErrorCode);
                    return null;
                }

                if (Equals(map.Head, data))
                {
                    return map;
                }
            }
            return null;
        }

        static bool Equals(string[] head, byte[] data)
        {
            for (int i = 0; i < head.Length; ++i)
            {
                string x = head[i];
                if ("xx;??;**".Split(';').Any(z => string.Equals(x, z, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                string y = $"{data[i]:X2}";
                if (!string.Equals(x, y, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
