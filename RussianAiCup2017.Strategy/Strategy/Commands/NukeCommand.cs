﻿using System.Drawing;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class NukeCommand : Command
	{
		public override int FormationId { get; }
		private const ActionType ActionType = Model.ActionType.TacticalNuclearStrike;
		private bool isStarted;
		private readonly long vehicleId;
		private readonly double x;
		private readonly double y;

		public NukeCommand(int formationId, long vehicleId, double x, double y)
		{
			FormationId = formationId;
			this.vehicleId = vehicleId;
			this.x = x;
			this.y = y;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.VehicleId = vehicleId;
			move.X = x;
			move.Y = y;

			isStarted = true;
#if DEBUG
			RewindClient.Instance.Circle(x, y, 50, Color.LawnGreen);
#endif
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(int worldTick, VehicleRegistry registry)
		{
			return true;
		}

		public override bool CanBeParallel()
		{
			return true;
		}
	}
}