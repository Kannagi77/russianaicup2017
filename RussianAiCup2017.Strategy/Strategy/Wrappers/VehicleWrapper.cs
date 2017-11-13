using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers
{
	public class VehicleWrapper
	{
		public Vehicle Vehicle { get; set; }
		public long Id => Vehicle.Id;
		public long PlayerId => Vehicle.PlayerId;
		public bool IsSelected => Vehicle.IsSelected;
		public double X => Vehicle.X;
		public double Y => Vehicle.Y;
		public VehicleType Type => Vehicle.Type;
		public bool IsIdle { get; set; }

		public VehicleWrapper(Vehicle vehicle)
		{
			Vehicle = vehicle;
		}

		public double GetDistanceTo(double x, double y)
		{
			return Vehicle.GetDistanceTo(x, y);
		}
	}
}