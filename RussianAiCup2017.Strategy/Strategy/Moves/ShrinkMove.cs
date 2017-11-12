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

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var currentCenterPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			if (!started)
			{
				ShrinkVehicles(world, currentCenterPoint, VehicleType.Tank);
				ShrinkVehicles(world, currentCenterPoint, VehicleType.Fighter);
				ShrinkVehicles(world, currentCenterPoint, VehicleType.Arrv);
				ShrinkVehicles(world, currentCenterPoint, VehicleType.Helicopter);
				ShrinkVehicles(world, currentCenterPoint, VehicleType.Ifv);
				started = true;
			}
			StrategyState result;
			if (currentCenterPoint != previousCenterPoint)
			{
				result = StrategyState.Shrink;
			}
			else
			{
				result = StrategyState.Rotate;
				started = false;
			}
			previousCenterPoint = currentCenterPoint;
			return result;
		}

		private void ShrinkVehicles(World world, Point2D currentCenterPoint, VehicleType type)
		{
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, type));
			CommandManager.EnqueueCommand(new ScaleCommand(currentCenterPoint, 0.1));
		}
	}
}