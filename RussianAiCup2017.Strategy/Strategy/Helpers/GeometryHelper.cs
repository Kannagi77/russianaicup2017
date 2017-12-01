using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers
{
	public static class GeometryHelper
	{
		public static double GetDistanceTo(this Vehicle u, Point2D point)
		{
			return u.GetDistanceTo(point.X, point.Y);
		}

		public static double GetDistanceTo(this Point2D p1, Point2D p2)
		{
			var xRange = p1.X - p2.X;
			var yRange = p1.Y - p2.Y;
			return Math.Sqrt(xRange * xRange + yRange * yRange);
		}

		public static Point2D ToPoint(this Vehicle unit)
		{
			return new Point2D(unit.X, unit.Y);
		}

		public static Point2D ToPoint(this Facility facility, Game game)
		{
			return new Point2D(facility.Left + game.FacilityWidth / 2, facility.Top + game.FacilityHeight / 2);
		}

		public static Point2D GetCenterPoint(this IEnumerable<Vehicle> units)
		{
			var list = units.ToList();
			var x = list.Select(u => u.X).Sum() / list.Count;
			var y = list.Select(u => u.Y).Sum() / list.Count;
			return new Point2D(x, y);
		}

		public static double GetMinimumDistanceTo(this IEnumerable<Vehicle> units, Point2D point)
		{
			return units.Select(u => u.GetDistanceTo(point)).OrderBy(d => d).First();
		}

		public static Vehicle GetClosest(this IEnumerable<Vehicle> units, Point2D point)
		{
			return GetUnitsByDistance(units, point).OrderBy(p => p.Key).First().Value;
		}

		public static Vehicle GetClosestAtMinimumRange(this IEnumerable<Vehicle> units, Vehicle unit, double range)
		{
			return GetUnitsByDistance(units, unit.ToPoint()).OrderBy(p => p.Key).FirstOrDefault(p => p.Key > range).Value;
		}

		private static Dictionary<double, Vehicle> GetUnitsByDistance(IEnumerable<Vehicle> units, Point2D point)
		{
			var unitsByDistance = new Dictionary<double, Vehicle>();
			foreach (var unit in units.ToList())
			{
				var distance = unit.GetDistanceTo(point);
				if (unitsByDistance.ContainsKey(distance))
					continue;
				unitsByDistance.Add(distance, unit);
			}
			return unitsByDistance;
		}

		public static bool IsLinePartsIntersected(Point2D a, Point2D b, Point2D c, Point2D d)
		{
			var common = (b.X - a.X) * (d.Y - c.Y) - (b.Y - a.Y) * (d.X - c.X);

			if (Math.Abs(common) < MagicConstants.Eps) return false;

			var rH = (a.Y - c.Y) * (d.X - c.X) - (a.X - c.X) * (d.Y - c.Y);
			var sH = (a.Y - c.Y) * (b.X - a.X) - (a.X - c.X) * (b.Y - a.Y);

			var r = rH / common;
			var s = sH / common;

			return r >= 0 && r <= 1 && s >= 0 && s <= 1;
		}
	}
}