using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class DiagonalVehicleFormation : VehicleFormationBase
	{
		private readonly List<MoveCommand> commands = new List<MoveCommand>();

		public DiagonalVehicleFormation(int id,
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
				return new VehicleFormationResult(new InitialGroundVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
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

			var leftGroupType = GetLeftGroupType(centerOfTanks.X, centerOfArrvs.X, centerOfIfvs.X);
			var leftGroup = leftGroupType == VehicleType.Tank
				? tanks
				: leftGroupType == VehicleType.Arrv
					? arrvs
					: ifvs;
			CommandManager.EnqueueCommand(new SelectAllCommand(Id, world, leftGroupType));
			var command = new MoveCommand(Id, leftGroup.Select(v => v.Id).ToList(), MagicConstants.InitialGapSize, 0);
			CommandManager.EnqueueCommand(command);
			commands.Add(command);
		}

		private static VehicleType GetLeftGroupType(double tanksCoord, double arrvsCoord, double ifvsCoord)
		{
			var upperCoord = new List<double> { tanksCoord, arrvsCoord, ifvsCoord }.OrderBy(y => y).First();
			return Math.Abs(tanksCoord - upperCoord) < MagicConstants.Eps
				? VehicleType.Tank
				: Math.Abs(arrvsCoord - upperCoord) < MagicConstants.Eps
					? VehicleType.Arrv
					: VehicleType.Ifv;
		}
	}
}