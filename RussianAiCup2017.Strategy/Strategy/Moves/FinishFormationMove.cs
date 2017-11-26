using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class FinishFormationMove : StrategyMove
	{
		public override StrategyState State => StrategyState.FinishFormation;
		private readonly List<MoveCommand> commands = new List<MoveCommand>();
		private const int Step = 15;
		private const int Adjustment = 6;

		public FinishFormationMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
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
				return StrategyState.Rotate;
			}
			return StrategyState.FinishFormation;
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

			var xTanskArrvs = Math.Abs(centerOfTanks.X - centerOfArrvs.X) < MagicConstants.Eps;
			var xArrvsIfvs = Math.Abs(centerOfArrvs.X - centerOfIfvs.X) < MagicConstants.Eps;

			var allUnits = new List<Vehicle>();
			allUnits.AddRange(tanks);
			allUnits.AddRange(arrvs);
			allUnits.AddRange(ifvs);

			var x1 = allUnits.Select(v => v.X).Min();
			var y1 = allUnits.Select(v => v.Y).Min();
			var x2 = allUnits.Select(v => v.X).Max();
			var y2 = allUnits.Select(v => v.Y).Max();

			if (xTanskArrvs && xArrvsIfvs)
			{
				VerticalFormation(myVehicles, world, x1, y1, x2, y2, allUnits, centerOfTanks, centerOfArrvs, centerOfIfvs);
			}
			else
			{
				HorizontalFormation(myVehicles, world, x1, y1, x2, y2, allUnits, centerOfTanks, centerOfArrvs, centerOfIfvs);
			}
		}

		private void VerticalFormation(IList<Vehicle> myVehicles,
			World world,
			double x1,
			double y1,
			double x2,
			double y2,
			IList<Vehicle> allUnits,
			Point2D centerOfTanks,
			Point2D centerOfArrvs,
			Point2D centerOfIfvs)
		{
			for (var i = 0; i < 10; i++)
			{
				var inc = 1 + i * Adjustment + i * Step;
				CommandManager.EnqueueCommand(new SelectCommand(x1 + inc, y1, x2 + inc, y2, true), int.MaxValue);
				var moveCommand = new MoveCommand(allUnits.Select(v => v.Id).ToList(), Step, 0);
				CommandManager.EnqueueCommand(moveCommand, int.MaxValue);
				commands.Add(moveCommand);
			}

			var upperGroupType = FormationHelper.GetUpperOrLeftGroupType(centerOfTanks.Y, centerOfArrvs.Y, centerOfIfvs.Y);
			var upperGroup = new VehiclesGroup(myVehicles.Where(v => v.Type == upperGroupType).Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);
			var bottomGroupType = FormationHelper.GetBottomOrRightGroupType(centerOfTanks.Y, centerOfArrvs.Y, centerOfIfvs.Y);
			var bottomGroup = new VehiclesGroup(myVehicles.Where(v => v.Type == bottomGroupType).Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);

			upperGroup
				.Select(world)
				.MoveByVector(1, 0)
				.MoveByVector(Adjustment, 0);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

			bottomGroup
				.Select(world)
				.MoveByVector(-1, 0)
				.MoveByVector(-Adjustment, 0);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

			upperGroup
				.Select(world)
				.MoveByVector(0, MagicConstants.InitialGapSize);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

			bottomGroup
				.Select(world)
				.MoveByVector(0, -MagicConstants.InitialGapSize);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);
		}

		private void HorizontalFormation(IList<Vehicle> myVehicles,
			World world,
			double x1,
			double y1,
			double x2,
			double y2,
			IList<Vehicle> allUnits,
			Point2D centerOfTanks,
			Point2D centerOfArrvs,
			Point2D centerOfIfvs)
		{
			for (var i = 0; i < 10; i++)
			{
				var inc = 1 + i * Adjustment + i * Step;
				CommandManager.EnqueueCommand(new SelectCommand(x1, y1 + inc, x2, y2 + inc, true), int.MaxValue);
				var moveCommand = new MoveCommand(allUnits.Select(v => v.Id).ToList(), 0, Step);
				CommandManager.EnqueueCommand(moveCommand, int.MaxValue);
				commands.Add(moveCommand);
			}

			var leftGroupType = FormationHelper.GetUpperOrLeftGroupType(centerOfTanks.X, centerOfArrvs.X, centerOfIfvs.X);
			var leftGroup = new VehiclesGroup(myVehicles.Where(v => v.Type == leftGroupType).Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);
			var rightGroupType = FormationHelper.GetBottomOrRightGroupType(centerOfTanks.X, centerOfArrvs.X, centerOfIfvs.X);
			var rightGroup = new VehiclesGroup(myVehicles.Where(v => v.Type == rightGroupType).Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);

			leftGroup
				.Select(world)
				.MoveByVector(0, 1)
				.MoveByVector(0, Adjustment);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

			rightGroup
				.Select(world)
				.MoveByVector(0, -1)
				.MoveByVector(0, -Adjustment);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

			leftGroup
				.Select(world)
				.MoveByVector(MagicConstants.InitialGapSize, 0);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

			rightGroup
				.Select(world)
				.MoveByVector(-MagicConstants.InitialGapSize, 0);
			commands.Add(CommandManager.PeekLastCommand() as MoveCommand);
		}
	}
}