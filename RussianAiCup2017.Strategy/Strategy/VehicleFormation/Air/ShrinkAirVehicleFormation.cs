using System.Collections.Generic;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Air
{
	public class ShrinkAirVehicleFormation : VehicleFormationBase
	{
		private ScaleCommand command;

		public ShrinkAirVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			if (command == null)
			{
				DoWork();
			}

			if (command != null && command.IsStarted() && command.IsFinished(world.TickIndex, VehicleRegistry))
			{
				return new VehicleFormationResult(new AttackAirVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}
			return new VehicleFormationResult(this);
		}

		private void DoWork()
		{
			var army = new VehiclesGroup(Id, VehicleIds, VehicleRegistry, CommandManager);
			army
				.Select(Id)
				.MoveByVector(0, 1)
				.Scale(0.1);
			command = CommandManager.PeekLastCommand(Id) as ScaleCommand;
		}
	}
}