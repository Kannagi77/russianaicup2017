using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public struct Point2D
	{
		public Point2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public static bool operator ==(Point2D p1, Point2D p2)
		{
			return Math.Abs(p1.X - p2.X) < Double.Epsilon && Math.Abs(p1.Y - p2.Y) < Double.Epsilon;
		}

		public static bool operator !=(Point2D p1, Point2D p2)
		{
			return !(p1 == p2);
		}
	}
}