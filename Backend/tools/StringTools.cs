using System.Collections.Generic;
using System.Linq;

namespace Backend.tools
{
    public static class StringTools
    {
        public static string FormatList<T>(List<T> list)
        {
            var ret = list.Aggregate("[", (current, item) => current + $"{item}, ");

            if (ret.Length > 1)
            {
                ret = ret[..^2];
            }

            return ret + " ]";
        }
    }
}