using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	class InitialPointMove : StrategyMove
	{
		public override StrategyState State => StrategyState.InitialPoint;
		private Point2D previousCenterPoint;

		public InitialPointMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me)
		{
			var gatheringPoint = new Point2D(world.Width / 15.0, world.Height / 15.0);
			if (world.TickIndex == 0)
			{
				CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height));
				CommandManager.EnqueueCommand(new MoveCommand(gatheringPoint));
			}
			var currentCenterPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			var result = currentCenterPoint == previousCenterPoint
				? StrategyState.Scale
				: StrategyState.InitialPoint;
			previousCenterPoint = currentCenterPoint;
			return result;
		}
	}
}