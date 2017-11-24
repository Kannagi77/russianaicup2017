using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public struct Point2D
	{
		private const double Eps = 0.1;

		public Point2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public static bool operator ==(Point2D p1, Point2D p2)
		{
			return Math.Abs(p1.X - p2.X) < Eps && Math.Abs(p1.Y - p2.Y) < Eps;
		}

		public static bool operator !=(Point2D p1, Point2D p2)
		{
			return !(p1 == p2);
		}

		public Vector2D To(Point2D p)
		{
			return new Vector2D(p.X - X, p.Y - Y);
		}

		public bool IsNear(Point2D p, double r)
		{
			return this.GetDistanceTo(p) <= r;
		}

		public override string ToString()
		{
			return $"({nameof(X)} = {X}, {nameof(Y)} = {Y})";
		}
	}
}