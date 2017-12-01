using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class CornerVehicleFormation : VehicleFormationBase
	{
		private readonly List<MoveCommand> commands = new List<MoveCommand>();

		public CornerVehicleFormation(int id,
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
				return new VehicleFormationResult(new TwoOnOneLineVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
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
					var oneSectorLength = Math.Abs(centerOfTanks.Y - centerOfArrvs.Y);
					if (Math.Abs(centerOfIfvs.Y - Math.Max(centerOfTanks.Y, centerOfArrvs.Y)) < MagicConstants.Eps)
					{
						var moveCommand = new MoveCommand(Id, ifvs.Select(v => v.Id).ToList(), 0, oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
					else
					{
						var moveCommand = new MoveCommand(Id, ifvs.Select(v => v.Id).ToList(), 0, 2 * oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
				}
				else if (xArrvsIfvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Tank));
					var oneSectorLength = Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y);
					if (Math.Abs(centerOfTanks.Y - Math.Max(centerOfArrvs.Y, centerOfIfvs.Y)) < MagicConstants.Eps)
					{
						var moveCommand = new MoveCommand(Id, tanks.Select(v => v.Id).ToList(), 0, oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);

					}
					else
					{
						var moveCommand = new MoveCommand(Id, tanks.Select(v => v.Id).ToList(), 0, 2 * oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
				}
				else
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Arrv));
					var oneSectorLength = Math.Abs(centerOfTanks.Y - centerOfIfvs.Y);
					if (Math.Abs(centerOfArrvs.Y - Math.Max(centerOfTanks.Y, centerOfIfvs.Y)) < MagicConstants.Eps)
					{
						var moveCommand = new MoveCommand(Id, arrvs.Select(v => v.Id).ToList(), 0, oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
					else
					{
						var moveCommand = new MoveCommand(Id, arrvs.Select(v => v.Id).ToList(), 0, 2 * oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
				}
			}
			else
			{
				if (yTanksArrvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Ifv));
					var oneSectorLength = Math.Abs(centerOfTanks.X - centerOfArrvs.X);
					if (Math.Abs(centerOfIfvs.X - Math.Max(centerOfTanks.X, centerOfArrvs.X)) < MagicConstants.Eps)
					{
						var moveCommand = new MoveCommand(Id, ifvs.Select(v => v.Id).ToList(), 0, oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
					else
					{
						var moveCommand = new MoveCommand(Id, ifvs.Select(v => v.Id).ToList(), 0, 2 * oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
				}
				else if (yArrvsIfvs)
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Tank));
					var oneSectorLength = Math.Abs(centerOfArrvs.X - centerOfIfvs.X);
					if (Math.Abs(centerOfTanks.X - Math.Max(centerOfArrvs.X, centerOfIfvs.X)) < MagicConstants.Eps)
					{
						var moveCommand = new MoveCommand(Id, tanks.Select(v => v.Id).ToList(), 0, oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);

					}
					else
					{
						var moveCommand = new MoveCommand(Id, tanks.Select(v => v.Id).ToList(), 0, 2 * oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
				}
				else
				{
					CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, VehicleType.Arrv));
					var oneSectorLength = Math.Abs(centerOfTanks.X - centerOfIfvs.X);
					if (Math.Abs(centerOfArrvs.X - Math.Max(centerOfTanks.X, centerOfIfvs.X)) < MagicConstants.Eps)
					{
						var moveCommand = new MoveCommand(Id, arrvs.Select(v => v.Id).ToList(), 0, oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
					else
					{
						var moveCommand = new MoveCommand(Id, arrvs.Select(v => v.Id).ToList(), 0, 2 * oneSectorLength);
						CommandManager.EnqueueCommand(moveCommand);
						commands.Add(moveCommand);
					}
				}
			}
		}
	}
}