using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeBeautify
{
    public class Log
    {
        public static string getLog(string text)
        {

            string res = "";

            string datetime = "[" + DateTime.Now.ToShortTimeString() + "]";

            res = datetime + " - " + text + ".\n";
            return res;
        }

        public static async Task<string> getLogAsync(string text)
        {

            string res = "";

            await Task.Run(() =>
            {
                string datetime = "[" + DateTime.Now.ToShortTimeString() + "]";

                res = datetime + " - " + text + ".\n";
            });


            return res;
        }
    }
}
