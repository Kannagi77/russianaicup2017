using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class ScaleCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Scale;
		private bool isStarted;
		private IList<long> vehicleIds;
		private readonly double x;
		private readonly double y;
		private readonly double factor;
		private readonly bool canBeParallel;
		private Func<int, bool> isFinished;

		public ScaleCommand(IList<long> vehicleIds, Point2D point, double factor, bool canBeParallel = false, Func<int, bool> isFinished = null)
			: this(vehicleIds, point.X, point.Y, factor, canBeParallel, isFinished)
		{
		}

		public ScaleCommand(IList<long> vehicleIds, double x, double y, double factor, bool canBeParallel = false, Func<int, bool> isFinished = null)
		{
			this.vehicleIds = vehicleIds;
			this.x = x;
			this.y = y;
			this.factor = factor;
			this.canBeParallel = canBeParallel;
			this.isFinished = isFinished;
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

		public override bool IsFinished(int worldTick, VehicleRegistry registry)
		{
			if (isFinished != null)
				return isFinished(worldTick);
			vehicleIds = registry.FilterDeadVehicles(vehicleIds);
			return vehicleIds.All(registry.IsVehicleIdle);
		}

		public override bool CanBeParallel()
		{
			return canBeParallel;
		}
	}
}