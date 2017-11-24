using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class ScaleCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Scale;
		private bool isStarted;
		private readonly IList<Vehicle> vehicles;
		private readonly double x;
		private readonly double y;
		private readonly double factor;
		private readonly bool canBeParallel;

		public ScaleCommand(IList<Vehicle> vehicles, Point2D point, double factor, bool canBeParallel = false)
			: this(vehicles, point.X, point.Y, factor, canBeParallel)
		{
		}

		public ScaleCommand(IList<Vehicle> vehicles, double x, double y, double factor, bool canBeParallel = false)
		{
			this.vehicles = vehicles;
			this.x = x;
			this.y = y;
			this.factor = factor;
			this.canBeParallel = canBeParallel;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.X = x;
			move.Y = y;
			move.Factor = factor;

			isStarted = true;
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(VehicleRegistry registry)
		{
			return vehicles.All(registry.IsVehicleIdle);
		}

		public override bool CanBeParallel()
		{
			return canBeParallel;
		}
	}
}