using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class RotateMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Rotate;
		private RotateCommand command;

		public RotateMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			if (command == null)
			{
				DoWork(me, world);
			}

			if (command != null && command.IsStarted() && command.IsFinished(VehicleRegistry))
			{
				command = null;
				return StrategyState.Attack;
			}
			return StrategyState.Rotate;


		}

		private void DoWork(Player me, World world)
		{
			var myVehicleIds = VehicleRegistry.MyVehicleIds(me);
			var vehicles = VehicleRegistry.GetVehiclesByIds(myVehicleIds);
			var width = vehicles.Select(v => v.X).Max() - vehicles.Select(v => v.X).Min();
			var height = vehicles.Select(v => v.Y).Max() - vehicles.Select(v => v.Y).Min();
			var rotationAngle = width > height
				? -45.ToRadians()
				: 45.ToRadians();

			var army = new VehiclesGroup(myVehicleIds, VehicleRegistry, CommandManager);
			army
				.Select(world)
				.RotateBy(rotationAngle, world);
			command = CommandManager.PeekLastCommand() as RotateCommand;
		}
	}
}