using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	class InitialPointMove : StrategyMove
	{
		public override StrategyState State => StrategyState.InitialPoint;
		private MoveCommand command;

		public InitialPointMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var gatheringPoint = new Point2D(world.Width / 15.0, world.Height / 15.0);
			var myVehicles = VehicleRegistry.MyVehicles(me);
			if (command == null)
			{
				CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height), world.TickIndex);
				command = new MoveCommand(myVehicles, gatheringPoint);
				CommandManager.EnqueueCommand(command, world.TickIndex);
			}
			if (command != null && command.IsStarted() && command.IsFinished())
			{
				command = null;
				return StrategyState.Scale;
			}
			return StrategyState.InitialPoint;
		}
	}
}