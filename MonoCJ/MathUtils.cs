using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoCJ
{
    public static class MathUtils
    {
        /// <summary>
        /// Returns a percentage of a number given. 
        /// Percentage should be between 0f and 1f.
        /// If out of range, returns -1f;
        /// </summary>
        /// <param name="percentage"> between 0.0f - 1.0f </param>
        /// <param name="outOf"></param>
        /// <returns>Returns -1 if percentage is out of range</returns>
        public static float Percentage(float percentage, float outOf)
        {

            if (percentage < 0f || percentage > 1f) return -1f;
            return outOf * percentage;
        }
    }
}
