﻿using System;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class Vector2D
	{
		public double X;
		public double Y;

		public Vector2D()
		{
			X = 0;
			Y = 0;
		}

		public Vector2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public Vector2D(Vector2D v)
		{
			X = v.X;
			Y = v.Y;
		}

		public Vector2D(double angle)
		{
			X = Math.Cos(angle);
			Y = Math.Sin(angle);
		}

		public Vector2D Copy()
		{
			return new Vector2D(this);
		}

		public Vector2D Add(Vector2D v)
		{
			X += v.X;
			Y += v.Y;
			return this;
		}

		public Vector2D Sub(Vector2D v)
		{
			X -= v.X;
			Y -= v.Y;
			return this;
		}

		public Vector2D Add(double dx, double dy)
		{
			X += dx;
			Y += dy;
			return this;
		}

		public Vector2D Sub(double dx, double dy)
		{
			X -= dx;
			Y -= dy;
			return this;
		}

		public Vector2D Mul(double f)
		{
			X *= f;
			Y *= f;
			return this;
		}

		public double Length()
		{
			return Math.Sqrt(X * X + Y * Y);
		}

		public double Distance(Vector2D v)
		{
			return Math.Sqrt(SquareDistance(v));
		}

		public double SquareDistance(Vector2D v)
		{
			double tx = X - v.X;
			double ty = Y - v.Y;
			return tx * tx + ty * ty;
		}

		public double SquareDistance(double x, double y)
		{
			double tx = X - x;
			double ty = Y - y;
			return tx * tx + ty * ty;
		}

		public double SquareLength()
		{
			return X * X + Y * Y;
		}

		public Vector2D Reverse()
		{
			X = -X;
			Y = -Y;
			return this;
		}

		public Vector2D Normalize()
		{
			var length = Length();
			if (Math.Abs(length) < double.Epsilon)
			{
				throw new Exception("Can\'t set angle of zero-width vector.");
			}
			X /= length;
			Y /= length;
			return this;
		}

		public Vector2D Length(double length)
		{
			var currentLength = Length();
			if (Math.Abs(currentLength) < double.Epsilon)
			{
				throw new Exception("Can\'t resize zero-width vector.");
			}
			return Mul(length / currentLength);
		}

		public Vector2D Perpendicular()
		{
			var a = Y;
			Y = -X;
			X = a;
			return this;
		}

		public double DotProduct(Vector2D vector)
		{
			return X * vector.X + Y * vector.Y;
		}

		public double Angle()
		{
			return Math.Atan2(Y, X);
		}

		public double AngleTo(Vector2D v)
		{
			return Math.Atan2(v.Y - Y, v.X - X);
		}

		public bool NearlyEqual(Vector2D potentialIntersectionPoint, double epsilon)
		{
			return Math.Abs(X - potentialIntersectionPoint.X) < epsilon && Math.Abs(Y - potentialIntersectionPoint.Y) < epsilon;
		}

		public Vector2D Rotate(Point2D angle)
		{
			return Rotate(new Vector2D(angle.X, angle.Y));
		}

		public Vector2D Rotate(Vector2D angle)
		{
			var newX = angle.X * X - angle.Y * Y;
			var newY = angle.Y * X + angle.X * Y;
			X = newX;
			Y = newY;
			return this;
		}

		public Vector2D RotateBack(Vector2D angle)
		{
			var newX = angle.X * X + angle.Y * Y;
			var newY = angle.X * Y - angle.Y * X;
			X = newX;
			Y = newY;
			return this;
		}

		public override string ToString()
		{
			return $"Vector ({X};{Y})";
		}

		public Vector2D Div(double f)
		{
			X /= f;
			Y /= f;
			return this;
		}

		public Vector2D CopyFrom(Vector2D position)
		{
			X = position.X;
			Y = position.Y;
			return this;
		}
	}
}