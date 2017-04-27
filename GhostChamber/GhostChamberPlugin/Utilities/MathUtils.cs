using System;

namespace GhostChamberPlugin.Utilities
{
	public static class MathUtils
	{
        /**
         * @static
         * Templated method to Clamp a value to a minimum and maximum.
         * @param val the Value to clamp.
         * @param min the minimum.
         * @param max the maximum.
         * @return 
         */
		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0)
			{
				return min;
			}
			else if (val.CompareTo(max) > 0)
			{
				return max;
			}
			else
			{
				return val;
			}
		}
	}
}