using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class RotateCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Rotate;
		private bool isStarted;
		private readonly IList<VehicleWrapper> vehicles;
		private readonly double x;
		private readonly double y;
		private readonly double angle;
		private bool canBeParallel;

		public RotateCommand(IList<VehicleWrapper> vehicles, Point2D p, double angle, bool canBeParallel = false)
			: this(vehicles, p.X, p.Y, angle, canBeParallel)
		{
		}

		public RotateCommand(IList<VehicleWrapper> vehicles, double x, double y, double angle, bool canBeParallel = false)
		{
			this.vehicles = vehicles;
			this.x = x;
			this.y = y;
			this.angle = angle;
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
			move.Angle = angle;

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