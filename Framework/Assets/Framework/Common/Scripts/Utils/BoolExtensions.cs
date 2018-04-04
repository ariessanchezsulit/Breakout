using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Common.Extensions
{
    public static class BoolExtension
    {
        public const string TRUE = "1";
        public const string FALSE = "0";

        public static string To01(this bool val)
        {
            return val ? TRUE : FALSE;
        }
    }
}
