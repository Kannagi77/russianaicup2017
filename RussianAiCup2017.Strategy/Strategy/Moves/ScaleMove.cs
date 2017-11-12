using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class ScaleMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Scale;
		private Point2D previousCenterPoint;
		private bool started;

		public ScaleMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me)
		{
			var factor = 1.3;
			if(!started)
			{
				ScaleVehicles(me, VehicleType.Tank, factor);
				ScaleVehicles(me, VehicleType.Fighter, factor);
				ScaleVehicles(me, VehicleType.Helicopter, factor);
				ScaleVehicles(me, VehicleType.Ifv, factor);
				ScaleVehicles(me, VehicleType.Arrv, factor);
				started = true;
			}
			var currentCenterPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			var result = currentCenterPoint == previousCenterPoint
				? StrategyState.Gather
				: StrategyState.Scale;
			previousCenterPoint = currentCenterPoint;
			return result;
		}

		private void ScaleVehicles(Player me, VehicleType type, double factor)
		{
			var vehicles = VehicleRegistry.MyVehicles(me).Where(v => v.Type == type);
			var centerPoint = vehicles.GetCenterPoint();
			CommandManager.EnqueueCommand(new ScaleCommand(centerPoint, factor));
		}
	}
}