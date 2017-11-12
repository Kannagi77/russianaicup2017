using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers
{
	public static class GeometryHelper
	{
		public static double GetDistanceTo(this Unit u, Point2D point)
		{
			return u.GetDistanceTo(point.X, point.Y);
		}

		public static Point2D GetCenterPoint(this IEnumerable<Unit> units)
		{
			var list = units.ToList();
			var x = list.Select(u => u.X).Sum() / list.Count;
			var y = list.Select(u => u.Y).Sum() / list.Count;
			return new Point2D(x, y);
		}

		public static double GetMinimumDistanceTo(this IEnumerable<Unit> units, Point2D point)
		{
			return units.Select(u => u.GetDistanceTo(point)).OrderBy(d => d).First();
		}

		public static Unit GetClosest(this IEnumerable<Unit> units, Point2D point)
		{
			return units.ToDictionary(u => u.GetDistanceTo(point), u => u).OrderBy(p => p.Key).First().Value;
		}

		public static Unit GetClosestAtMinimumRange(this IEnumerable<Unit> units, Unit point, double range)
		{
			return units.ToDictionary(u => u.GetDistanceTo(point), u => u).OrderBy(p => p.Key).FirstOrDefault(p => p.Key > range).Value;
		}
	}
}