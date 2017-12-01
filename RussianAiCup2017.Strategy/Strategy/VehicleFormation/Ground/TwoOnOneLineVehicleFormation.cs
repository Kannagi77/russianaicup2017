using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class TwoOnOneLineVehicleFormation : VehicleFormationBase
	{
		private readonly List<MoveCommand> commands = new List<MoveCommand>();

		public TwoOnOneLineVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			if (!commands.Any())
			{
				DoWork(me, world);
			}

			if (commands.Any() && commands.All(c => c.IsStarted()) && commands.All(c => c.IsFinished(world.TickIndex, VehicleRegistry)))
			{
				return new VehicleFormationResult(new FinishGroundVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}
			return new VehicleFormationResult(this);
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

			var xTanksArrvs = Math.Abs(centerOfTanks.X - centerOfArrvs.X) < MagicConstants.Eps;
			var xArrvsIfvs = Math.Abs(centerOfArrvs.X - centerOfIfvs.X) < MagicConstants.Eps;
			var xTanksIfvs = Math.Abs(centerOfTanks.X - centerOfIfvs.X) < MagicConstants.Eps;
			var yTanksArrvs = Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) < MagicConstants.Eps;
			var yArrvsIfvs = Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) < MagicConstants.Eps;

			if (xTanksArrvs || xArrvsIfvs || xTanksIfvs)
			{
				if (xTanksArrvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Ifv));
					var moveCommand = new MoveCommand(Id, ifvs.Select(v => v.Id).ToList(), centerOfTanks.X - centerOfIfvs.X, 0);
					CommandManager.EnqueueCommand(moveCommand);
					commands.Add(moveCommand);
				}
				else if (xArrvsIfvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Tank));
					var moveCommand = new MoveCommand(Id, tanks.Select(v => v.Id).ToList(), centerOfArrvs.X - centerOfTanks.X, 0);
					CommandManager.EnqueueCommand(moveCommand);
					commands.Add(moveCommand);

				}
				else
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Arrv));
					var moveCommand = new MoveCommand(Id, arrvs.Select(v => v.Id).ToList(), centerOfTanks.X - centerOfArrvs.X, 0);
					CommandManager.EnqueueCommand(moveCommand);
					commands.Add(moveCommand);
				}
			}
			else
			{
				if (yTanksArrvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Ifv));
					var moveCommand = new MoveCommand(Id, ifvs.Select(v => v.Id).ToList(), 0, centerOfTanks.Y - centerOfIfvs.Y);
					CommandManager.EnqueueCommand(moveCommand);
					commands.Add(moveCommand);
				}
				else if (yArrvsIfvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Tank));
					var moveCommand = new MoveCommand(Id, tanks.Select(v => v.Id).ToList(), 0, centerOfArrvs.Y - centerOfTanks.Y);
					CommandManager.EnqueueCommand(moveCommand);
					commands.Add(moveCommand);
				}
				else
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Arrv));
					var moveCommand = new MoveCommand(Id, arrvs.Select(v => v.Id).ToList(), 0, centerOfTanks.Y - centerOfArrvs.Y);
					CommandManager.EnqueueCommand(moveCommand);
					commands.Add(moveCommand);
				}
			}
		}
	}
}