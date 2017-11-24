using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class TwoOnOneLineFormationMove : StrategyMove
	{
		public override StrategyState State => StrategyState.TwoOnOneLineFormation;
		private readonly List<MoveCommand> commands = new List<MoveCommand>();
		private const double Eps = 0.1;

		public TwoOnOneLineFormationMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			if (!commands.Any())
			{
				DoWork(me, world);
			}

			if (commands.Any() && commands.All(c => c.IsStarted()) && commands.All(c => c.IsFinished(VehicleRegistry)))
			{
				commands.Clear();
				return StrategyState.FinishAirMove;
			}
			return StrategyState.TwoOnOneLineFormation;
		}

		private void DoWork(Player me, World world)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);
			var tanks = myVehicles.Where(v => v.Type == VehicleType.Tank).ToList();
			var centerOfTanks = tanks.GetCenterPoint();

			var arrvs = myVehicles.Where(v => v.Type == VehicleType.Arrv).ToList();
			var centerOfArrvs = arrvs.GetCenterPoint();

			var ifvs = myVehicles.Where(v => v.Type == VehicleType.Ifv).ToList();
			var centerOfIfvs = ifvs.GetCenterPoint();

			var xTanksArrvs = Math.Abs(centerOfTanks.X - centerOfArrvs.X) < Eps;
			var xArrvsIfvs = Math.Abs(centerOfArrvs.X - centerOfIfvs.X) < Eps;
			var xTanksIfvs = Math.Abs(centerOfTanks.X - centerOfIfvs.X) < Eps;
			var yTanksArrvs = Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) < Eps;
			var yArrvsIfvs = Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) < Eps;

			if (xTanksArrvs || xArrvsIfvs || xTanksIfvs)
			{
				if (xTanksArrvs)
				{
					CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, VehicleType.Ifv),
						world.TickIndex);
					var moveCommand = new MoveCommand(ifvs.Select(v => v.Id).ToList(), centerOfTanks.X - centerOfIfvs.X, 0);
					CommandManager.EnqueueCommand(moveCommand, world.TickIndex);
					commands.Add(moveCommand);
				}
				else if (xArrvsIfvs)
				{
					CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, VehicleType.Tank),
						world.TickIndex);
					var moveCommand = new MoveCommand(tanks.Select(v => v.Id).ToList(), centerOfArrvs.X - centerOfTanks.X, 0);
					CommandManager.EnqueueCommand(moveCommand, world.TickIndex);
					commands.Add(moveCommand);

				}
				else
				{
					CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, VehicleType.Arrv),
						world.TickIndex);
					var moveCommand = new MoveCommand(arrvs.Select(v => v.Id).ToList(), centerOfTanks.X - centerOfArrvs.X, 0);
					CommandManager.EnqueueCommand(moveCommand, world.TickIndex);
					commands.Add(moveCommand);
				}
			}
			else
			{
				if (yTanksArrvs)
				{
					CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, VehicleType.Ifv),
						world.TickIndex);
					var moveCommand = new MoveCommand(ifvs.Select(v => v.Id).ToList(), 0, centerOfTanks.Y - centerOfIfvs.Y);
					CommandManager.EnqueueCommand(moveCommand, world.TickIndex);
					commands.Add(moveCommand);
				}
				else if (yArrvsIfvs)
				{
					CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, VehicleType.Tank),
						world.TickIndex);
					var moveCommand = new MoveCommand(tanks.Select(v => v.Id).ToList(), 0, centerOfArrvs.Y - centerOfTanks.Y);
					CommandManager.EnqueueCommand(moveCommand, world.TickIndex);
					commands.Add(moveCommand);
				}
				else
				{
					CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, VehicleType.Arrv),
						world.TickIndex);
					var moveCommand = new MoveCommand(arrvs.Select(v => v.Id).ToList(), 0, centerOfTanks.Y - centerOfArrvs.Y);
					CommandManager.EnqueueCommand(moveCommand, world.TickIndex);
					commands.Add(moveCommand);
				}
			}
		}
	}
}