namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public static class MagicConstants
	{
		public static Point2D GroundGatheringPoint = new Point2D(192, 192);
		public static Point2D FightersGatheringPoint = new Point2D(32 * 6, 32 * 9);
		public static Point2D HelicoptersGatheringPoint = new Point2D(32 * 9, 32 * 6);
		public static Point2D AirGatheringPoint = new Point2D(32 * 9, 32 * 9);

		public const double Eps = 0.01;
		public const double InitialGroupSize = 54.0;
		public const double InitialGapSize = InitialGroupSize + 20.0;

		public static Point2D Point11 = new Point2D(45, 45);
		public static Point2D Point12 = new Point2D(45, 119);
		public static Point2D Point13 = new Point2D(45, 193);

		public static Point2D Point21 = new Point2D(119, 45);
		public static Point2D Point22 = new Point2D(119, 119);
		public static Point2D Point23 = new Point2D(119, 193);

		public static Point2D Point31 = new Point2D(193, 45);
		public static Point2D Point32 = new Point2D(193, 119);
		public static Point2D Point33 = new Point2D(193, 193);
	}
}