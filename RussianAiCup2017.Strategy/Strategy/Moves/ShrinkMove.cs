using System.Collections.Generic;
using System.Linq;
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
			var vehicles = VehicleRegistry.MyVehicles(me);
			var currentCenterPoint = vehicles.GetCenterPoint();
			if (!started)
			{
				ShrinkVehicles(vehicles, world, currentCenterPoint, VehicleType.Tank);
				ShrinkVehicles(vehicles, world, currentCenterPoint, VehicleType.Fighter);
				ShrinkVehicles(vehicles, world, currentCenterPoint, VehicleType.Arrv);
				ShrinkVehicles(vehicles, world, currentCenterPoint, VehicleType.Helicopter);
				ShrinkVehicles(vehicles, world, currentCenterPoint, VehicleType.Ifv);
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

		private void ShrinkVehicles(IEnumerable<Vehicle> vehicles, World world, Point2D currentCenterPoint, VehicleType type)
		{
			var selectedVehicles = vehicles.Where(v => v.Type == type).ToList();
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, type), world.TickIndex);
			CommandManager.EnqueueCommand(new ScaleCommand(selectedVehicles, currentCenterPoint, 0.1, true), world.TickIndex);
		}
	}
}