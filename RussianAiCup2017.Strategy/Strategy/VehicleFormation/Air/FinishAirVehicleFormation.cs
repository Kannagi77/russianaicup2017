using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Air
{
	public class FinishAirVehicleFormation : VehicleFormationBase
	{
		private readonly List<MoveCommand> commands = new List<MoveCommand>();
		private const double ScaleFactor = 1.5;

		public FinishAirVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			if (!commands.Any())
			{
				DoWork(me);
			}

			if (commands.Any() && commands.All(c => c.IsStarted()) && commands.All(c => c.IsFinished(world.TickIndex, VehicleRegistry)))
			{
				return new VehicleFormationResult(new RotateAirVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}
			return new VehicleFormationResult(this);
		}

		private void DoWork(Player me)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);

			var fightersGroup = new VehiclesGroup(Id,
				myVehicles
					.Where(v => v.Type == VehicleType.Fighter)
					.Select(v => v.Id)
					.ToList(),
				VehicleRegistry,
				CommandManager);
			var helicoptersGroup = new VehiclesGroup(Id,
				myVehicles
					.Where(v => v.Type == VehicleType.Helicopter)
					.Select(v => v.Id)
					.ToList(),
				VehicleRegistry,
				CommandManager);
			var fightersToTheRight = fightersGroup.Center.X > helicoptersGroup.Center.X;

			fightersGroup
				.SelectVehicles(VehicleType.Fighter)
				.Scale(ScaleFactor)
				.MoveByVector(0, 6);

			helicoptersGroup
				.SelectVehicles(VehicleType.Helicopter)
				.MoveByVector(0, 1)
				.Scale(ScaleFactor);

			fightersGroup
				.SelectVehicles(VehicleType.Fighter)
				.MoveByVector(fightersToTheRight ? -MagicConstants.InitialGapSize : MagicConstants.InitialGapSize, 0, canBeParallel: true);

			helicoptersGroup
				.SelectVehicles(VehicleType.Helicopter)
				.MoveByVector(fightersToTheRight ? MagicConstants.InitialGapSize : -MagicConstants.InitialGapSize, 0);
			commands.Add(CommandManager.PeekLastCommand(Id) as MoveCommand);
		}
	}
}