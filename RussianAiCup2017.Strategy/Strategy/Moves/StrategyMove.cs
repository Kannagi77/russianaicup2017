using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public abstract class StrategyMove
	{
		public abstract StrategyState State { get; }
		protected readonly CommandManager CommandManager;
		protected readonly VehicleRegistry VehicleRegistry;

		protected StrategyMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
		{
			CommandManager = commandManager;
			VehicleRegistry = vehicleRegistry;
		}

		public abstract StrategyState Perform(World world, Player me);
	}
}