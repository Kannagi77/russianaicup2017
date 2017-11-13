using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class ScaleMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Scale;
		private bool started;
		private readonly List<ScaleCommand> commands = new List<ScaleCommand>();

		public ScaleMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var factor = 1.3;
			if(!started)
			{
				var vehicles = VehicleRegistry.MyVehicles(me);
				ScaleVehicles(vehicles, VehicleType.Tank, factor, world);
				ScaleVehicles(vehicles, VehicleType.Fighter, factor, world);
				ScaleVehicles(vehicles, VehicleType.Helicopter, factor, world);
				ScaleVehicles(vehicles, VehicleType.Ifv, factor, world);
				ScaleVehicles(vehicles, VehicleType.Arrv, factor, world);
				started = true;
			}
			if (commands.Any() && commands.All(c => c.IsStarted() && c.IsFinished()))
			{
				commands.Clear();
				started = false;
				return StrategyState.Gather;
			}
			return StrategyState.Scale;
		}

		private void ScaleVehicles(IEnumerable<VehicleWrapper> allVehicles, VehicleType type, double factor, World world)
		{
			var vehicles = allVehicles.Where(v => v.Type == type).ToList();
			var centerPoint = vehicles.GetCenterPoint();
			var selectCommand = new SelectCommand(0, 0, world.Width, world.Height, type);
			var scaleCommand = new ScaleCommand(vehicles, centerPoint, factor, true);
			CommandManager.EnqueueCommand(selectCommand);
			CommandManager.EnqueueCommand(scaleCommand);
			commands.Add(scaleCommand);
		}
	}
}