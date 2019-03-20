using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticHwChecker
{
    public static class Utils
    {
        public static string Repeat(this string @this, int times)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < times; i++)
                str.Append(@this);
            return str.ToString();
        }
        
        public static IEnumerable<string> GetAllInnerFiles(this string dirPath) => 
            Directory.EnumerateFiles(dirPath).Concat(Directory.EnumerateDirectories(dirPath).SelectMany(GetAllInnerFiles));

        public static bool RecreateDirectory(this string path)
        {
            bool didDelete = false;

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                didDelete = true;
            }
                
            Directory.CreateDirectory(path);
            return didDelete;
        }

        public static string TryRunFor(int ms, Func<bool> func)
        {
            try
            {
                var cancellation = new CancellationToken();
                var task = new Task<bool>(func, cancellation);
                task.Start();
                task.Wait(ms);
                if (task.Status != TaskStatus.RanToCompletion)
                    return "Didn't Finish";
                return task.Result.AsString();
            }
            catch (Exception e)
            {
                return "Exception";
            }
        }

        public static string AsString(this bool @this) => @this ? "v" : "x";

        public static string PadZeroesLeft(this string @this, int amount)
        {
            if (@this.Length >= amount)
                return @this;
            return string.Concat("0".Repeat(amount - @this.Length), @this);
        }

        public static string CsvLine(IEnumerable<string> data, params string[] rest)
        {
            return string.Join(",", data.Concat(rest).Select(str => "\"" + str.Replace("\"", "\"\"") + "\""));
        }
    }
}
