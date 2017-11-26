using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class ShrinkMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Shrink;
		private ScaleCommand command;

		public ShrinkMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			if (command == null)
			{
				DoWork(me, world);
			}

			if (command != null && command.IsStarted() && command.IsFinished(world.TickIndex, VehicleRegistry))
			{
				command = null;
				return StrategyState.Attack;
			}
			return StrategyState.Shrink;
		}

		private void DoWork(Player me, World world)
		{
			var vehicles = VehicleRegistry.MyVehicleIds(me);
			var army = new VehiclesGroup(vehicles, VehicleRegistry, CommandManager);
			army
				.Select(world)
				.Scale(0.1, world);
			command = CommandManager.PeekLastCommand() as ScaleCommand;
		}
	}
}