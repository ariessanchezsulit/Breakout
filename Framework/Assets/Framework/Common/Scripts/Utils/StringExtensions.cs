using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Common.Extensions
{
    public static class StringExtension
    {
        public static Texture2D ToTexture(this string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);

                // automatically resize the texture by its dimensions.
                tex.LoadImage(fileData);
            }

            return tex;
        }
        
        public static int ToInt(this string str)
        {
            int result = -1;

            int.TryParse(str, out result);

            return result;
        }

        public static float ToFloat(this string str)
        {
            float result = -1f;

            float.TryParse(str, out result);

            return result;
        }

        public static double ToDouble(this string str)
        {
            double result = -1.0f;

            double.TryParse(str, out result);

            return result;
        }

        public static long ToLong(this string str)
        {
            long result = -1;

            long.TryParse(str, out result);

            return result;
        }
    }
}
