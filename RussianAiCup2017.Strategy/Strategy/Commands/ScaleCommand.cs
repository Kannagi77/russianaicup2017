using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class ScaleCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Scale;
		private bool isStarted;
		private readonly IList<VehicleWrapper> vehicles;
		private readonly double x;
		private readonly double y;
		private readonly double factor;
		private readonly bool canBeParallel;

		public ScaleCommand(IList<VehicleWrapper> vehicles, Point2D point, double factor, bool canBeParallel = false)
			: this(vehicles, point.X, point.Y, factor, canBeParallel)
		{
		}

		public ScaleCommand(IList<VehicleWrapper> vehicles, double x, double y, double factor, bool canBeParallel = false)
		{
			this.vehicles = vehicles;
			this.x = x;
			this.y = y;
			this.factor = factor;
			this.canBeParallel = canBeParallel;
		}

		public override void Commit(Move move)
		{
			foreach (var vehicle in vehicles)
			{
				vehicle.IsIdle = false;
			}
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

		public override bool IsFinished()
		{
			return vehicles.All(v => v.IsIdle);
		}

		public override bool CanBeParallel()
		{
			return canBeParallel;
		}
	}
}