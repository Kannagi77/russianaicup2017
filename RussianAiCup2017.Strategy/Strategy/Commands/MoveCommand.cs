using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class MoveCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Move;
		private bool isStarted;
		private readonly IList<VehicleWrapper> vehicles;
		private readonly double x;
		private readonly double y;
		private readonly double maxSpeed;
		private readonly bool canBeParallel;

		public MoveCommand(IList<VehicleWrapper> vehicles, Point2D point, bool canBeParallel = false)
			: this(vehicles, point.X, point.Y, canBeParallel)
		{
		}

		public MoveCommand(IList<VehicleWrapper> vehicles, double x, double y, bool canBeParallel = false)
		{
			this.vehicles = vehicles;
			this.x = x;
			this.y = y;
			this.canBeParallel = canBeParallel;
		}

		public MoveCommand(IList<VehicleWrapper> vehicles, double x, double y, double maxSpeed, bool canBeParallel = false)
		{
			this.vehicles = vehicles;
			this.x = x;
			this.y = y;
			this.maxSpeed = maxSpeed;
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
			move.MaxSpeed = maxSpeed;

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