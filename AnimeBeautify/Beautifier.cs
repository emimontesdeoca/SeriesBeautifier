using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeBeautify
{
    public class Beautifier
    {
        public static async Task<string> beautifyString(string path)
        {
            string res = path;
            await Task.Run(() =>
            {


                try
                {
                    /// Undescore
                    res = res.Replace("_", " ");

                    ///Source
                    res = res.Split(']')[1];
                    res = res.Split('[')[0];

                    /// Caps
                    res = res.Split('(')[0];

                    /// First is space

                    if (res[0].ToString() == " ")
                    {
                        res = res.Remove(0, 1);
                    }

                    /// Last is space
                    if (res[res.Count() - 1].ToString() == " ")
                    {
                        res = res.Remove(res.Count() - 1, 1);
                    }

                }
                catch (Exception)
                {

                }

                if (res == "")
                {
                    res = path;
                }


            });
            return res;
        }
    }
}
