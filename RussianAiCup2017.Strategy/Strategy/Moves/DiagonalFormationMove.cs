using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class DiagonalFormationMove : StrategyMove
	{
		public override StrategyState State => StrategyState.DiagonalFormation;
		private readonly List<MoveCommand> commands = new List<MoveCommand>();
		private const double Eps = 0.1;

		public DiagonalFormationMove(CommandManager commandManager, VehicleRegistry vehicleRegistry) : base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			if (!commands.Any())
			{
				DoWork(me, world);
			}

			if (commands.Any() && commands.All(c => c.IsStarted()) && commands.All(c => c.IsFinished(world.TickIndex, VehicleRegistry)))
			{
				commands.Clear();
				return StrategyState.InitFormation;
			}
			return StrategyState.DiagonalFormation;
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
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, leftGroupType), world.TickIndex);
			var command = new MoveCommand(leftGroup.Select(v => v.Id).ToList(), MagicConstants.InitialGapSize, 0);
			CommandManager.EnqueueCommand(command, world.TickIndex);
			commands.Add(command);
		}

		private static VehicleType GetLeftGroupType(double tanksCoord, double arrvsCoord, double ifvsCoord)
		{
			var upperCoord = new List<double> { tanksCoord, arrvsCoord, ifvsCoord }.OrderBy(y => y).First();
			return Math.Abs(tanksCoord - upperCoord) < Eps
				? VehicleType.Tank
				: Math.Abs(arrvsCoord - upperCoord) < Eps
					? VehicleType.Arrv
					: VehicleType.Ifv;
		}
	}
}