using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class GatherMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Gather;
		private Point2D previousCenterPoint;
		private bool started;

		public GatherMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var gatheringPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			if(!started)
			{
				MoveToCenter(world, me, gatheringPoint, VehicleType.Tank);
				MoveToCenter(world, me, gatheringPoint, VehicleType.Fighter);
				MoveToCenter(world, me, gatheringPoint, VehicleType.Arrv);
				MoveToCenter(world, me, gatheringPoint, VehicleType.Helicopter);
				MoveToCenter(world, me, gatheringPoint, VehicleType.Ifv);
				started = true;
			}
			var currentCenterPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			StrategyState result;
			if (currentCenterPoint != previousCenterPoint)
			{
				result = StrategyState.Gather;
			}
			else
			{
				result = StrategyState.Shrink;
				started = false;
			}
			previousCenterPoint = currentCenterPoint;
			return result;
		}

		private void MoveToCenter(World world, Player me, Point2D centerPoint, VehicleType type)
		{
			var vehicles = VehicleRegistry.MyVehicles(me).Where(v => v.Type == type).ToList();
			var groupCenterPoint = vehicles.GetCenterPoint();
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, type), world.TickIndex);
			CommandManager.EnqueueCommand(new MoveCommand(vehicles.Select(v => v.Id).ToList(), centerPoint.X - groupCenterPoint.X,
				centerPoint.Y - groupCenterPoint.Y, true), world.TickIndex);
		}
	}
}