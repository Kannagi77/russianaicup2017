using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Air
{
	public class InitialAirVehicleFormation : VehicleFormationBase
	{
		private readonly List<MoveCommand> commands = new List<MoveCommand>();

		public InitialAirVehicleFormation(int id,
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
				return new VehicleFormationResult(new FinishAirVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
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

			var leftPoint = new Point2D(MagicConstants.InitialGapSize * 1, MagicConstants.InitialGapSize * 3);
			var rightPoint = new Point2D(MagicConstants.InitialGapSize * 3, MagicConstants.InitialGapSize * 1);


			if (fightersGroup.Center.GetDistanceTo(leftPoint) < helicoptersGroup.Center.GetDistanceTo(leftPoint))
			{
				fightersGroup
					.SelectVehicles(VehicleType.Fighter)
					.MoveTo(leftPoint);

				helicoptersGroup
					.SelectVehicles(VehicleType.Helicopter)
					.MoveTo(rightPoint);

				fightersGroup
					.SelectVehicles(VehicleType.Fighter)
					.MoveByVector(0, -MagicConstants.InitialGapSize, canBeParallel: true);
				commands.Add(CommandManager.PeekLastCommand(Id) as MoveCommand);

				helicoptersGroup
					.SelectVehicles(VehicleType.Helicopter)
					.MoveByVector(0, MagicConstants.InitialGapSize);
				commands.Add(CommandManager.PeekLastCommand(Id) as MoveCommand);
			}
			else
			{
				fightersGroup
					.SelectVehicles(VehicleType.Fighter)
					.MoveTo(rightPoint);

				helicoptersGroup
					.SelectVehicles(VehicleType.Helicopter)
					.MoveTo(leftPoint);

				fightersGroup
					.SelectVehicles(VehicleType.Fighter)
					.MoveByVector(0, MagicConstants.InitialGapSize, canBeParallel: true);
				commands.Add(CommandManager.PeekLastCommand(Id) as MoveCommand);

				helicoptersGroup
					.SelectVehicles(VehicleType.Helicopter)
					.MoveByVector(0, -MagicConstants.InitialGapSize);
				commands.Add(CommandManager.PeekLastCommand(Id) as MoveCommand);
			}
		}
	}
}