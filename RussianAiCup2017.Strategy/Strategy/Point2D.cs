using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public struct Point2D
	{
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Point2D d && Equals(d);
		}

		public Point2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }

		public double Y { get; }

		public static bool operator ==(Point2D p1, Point2D p2)
		{
			return Math.Abs(p1.X - p2.X) < MagicConstants.Eps && Math.Abs(p1.Y - p2.Y) < MagicConstants.Eps;
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

		public override int GetHashCode()
		{
			unchecked
			{
				return (X.GetHashCode() * 397) ^ Y.GetHashCode();
			}
		}

		public bool Equals(Point2D other)
		{
			return X.Equals(other.X) && Y.Equals(other.Y);
		}
	}
}