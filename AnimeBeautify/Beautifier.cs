using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnimeBeautify
{
    public class Beautifier
    {
        public static async Task<string> beautifyString(string path)
        {
            string res = "";
            await Task.Run(() =>
            {


                try
                {
                    /// Undescore
                    path = path.Replace("_", " ");

                    string[] splitsource = Regex.Split(path, @"\[.*?\]");
                    string resWithoutSource = "";
                    foreach (var item in splitsource)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            resWithoutSource += item;
                        }
                    }


                    /// Caps
                    string resWithoutCaps = "";
                    string[] splitcaps = Regex.Split(resWithoutSource, @"\(.*?\)");

                    foreach (var item in splitcaps)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            resWithoutCaps += item;
                        }
                    }

                    res = resWithoutCaps;

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

                    /// Remove up to 4 spaces (this is bullshit sorry)
                    res = res.Replace("  ", " ");
                    res = res.Replace("   ", " ");
                    res = res.Replace("    ", " ");

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
