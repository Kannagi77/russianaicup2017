using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class ShrinkMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Shrink;
		private Point2D previousCenterPoint;
		private bool started;

		public ShrinkMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me)
		{
			var currentCenterPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			if (!started)
			{
				CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height));
				CommandManager.EnqueueCommand(new ScaleCommand(currentCenterPoint, 0.1));
				started = true;
			}
			StrategyState result;
			if (currentCenterPoint != previousCenterPoint)
			{
				result = StrategyState.Shrink;
			}
			else
			{
				result = StrategyState.Attack;
				started = false;
			}
			previousCenterPoint = currentCenterPoint;
			return result;
		}
	}
}