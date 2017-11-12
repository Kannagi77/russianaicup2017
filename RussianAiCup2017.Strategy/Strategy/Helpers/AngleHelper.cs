using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers
{
	public static class AngleHelper
	{
		public static double ToDegrees(this double radians)
		{
			return radians * 180.0 / Math.PI;
		}

		public static double ToDegrees(this int radians)
		{
			return radians * 180.0 / Math.PI;
		}

		public static double ToRadians(this double degrees)
		{
			return degrees * Math.PI / 180.0;
		}

		public static double ToRadians(this int degrees)
		{
			return degrees * Math.PI / 180.0;
		}
	}
}