using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Common.Extensions
{
    using Math = System.Math;

    public static class FloatExtensions
    {
        public static float Round(this float f, int decimals)
        {
            float multiplier = Mathf.Pow(10, decimals);
            f = Mathf.Round(f * multiplier);
            return f / multiplier;
        }

        public static float Truncate(this float value, int digits)
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }
    }
}
