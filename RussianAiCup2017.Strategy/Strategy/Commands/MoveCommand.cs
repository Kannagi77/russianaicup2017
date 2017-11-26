using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class MoveCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Move;
		private bool isStarted;
		private IList<long> vehicleIds;
		private readonly double x;
		private readonly double y;
		private readonly double maxSpeed;
		private readonly bool canBeParallel;

		public MoveCommand(IList<long> vehicleIds, Point2D point, bool canBeParallel = false)
			: this(vehicleIds, point.X, point.Y, canBeParallel)
		{
		}

		public MoveCommand(IList<long> vehicleIds, double x, double y, bool canBeParallel = false)
		{
			this.vehicleIds = vehicleIds;
			this.x = x;
			this.y = y;
			this.canBeParallel = canBeParallel;
		}

		public MoveCommand(IList<long> vehicleIds, double x, double y, double maxSpeed, bool canBeParallel = false)
		{
			this.vehicleIds = vehicleIds;
			this.x = x;
			this.y = y;
			this.maxSpeed = maxSpeed;
			this.canBeParallel = canBeParallel;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.X = x;
			move.Y = y;
			move.MaxSpeed = maxSpeed;

			isStarted = true;
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(int worldTick, VehicleRegistry registry)
		{
			vehicleIds = registry.FilterDeadVehicles(vehicleIds);
			return vehicleIds.All(registry.IsVehicleIdle);
		}

		public override bool CanBeParallel()
		{
			return canBeParallel;
		}
	}
}