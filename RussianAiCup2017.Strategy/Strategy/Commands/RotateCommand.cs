﻿using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class RotateCommand : Command
	{
		public override int FormationId { get; }
		private const ActionType ActionType = Model.ActionType.Rotate;
		private bool isStarted;
		private IList<long> vehicleIds;
		private readonly double x;
		private readonly double y;
		private readonly double angle;
		private bool canBeParallel;

		public RotateCommand(int formationId, IList<long> vehicleIds, Point2D p, double angle, bool canBeParallel = false)
			: this(formationId, vehicleIds, p.X, p.Y, angle, canBeParallel)
		{
		}

		public RotateCommand(int formationId, IList<long> vehicleIds, double x, double y, double angle, bool canBeParallel = false)
		{
			FormationId = formationId;
			this.vehicleIds = vehicleIds;
			this.x = x;
			this.y = y;
			this.angle = angle;
			this.canBeParallel = canBeParallel;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.X = x;
			move.Y = y;
			move.Angle = angle;

			isStarted = true;
			foreach (var vehicleId in vehicleIds)
			{
				registry.ForceUnidleVehicle(vehicleId);
			}
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