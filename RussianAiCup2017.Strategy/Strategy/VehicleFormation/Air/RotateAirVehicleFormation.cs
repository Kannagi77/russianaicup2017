using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Air
{
	public class RotateAirVehicleFormation : VehicleFormationBase
	{
		private readonly bool r2;
		private RotateCommand command;
		private bool binded;

		public RotateAirVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry,
			bool r2 = false)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
			this.r2 = r2;
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			if (command == null)
			{
				DoWork();
			}

			if (command != null && command.IsStarted() && command.IsFinished(world.TickIndex, VehicleRegistry))
			{
				return new VehicleFormationResult(new ShrinkAirVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}
			return new VehicleFormationResult(this);
		}

		private void DoWork()
		{
			var army = new VehiclesGroup(Id, VehicleIds, VehicleRegistry, CommandManager);
			if (!binded)
			{
				Bind(army);
				return;
			}
			var vehicles = VehicleRegistry.GetVehiclesByIds(VehicleIds);
			var width = vehicles.Select(v => v.X).Max() - vehicles.Select(v => v.X).Min();
			var height = vehicles.Select(v => v.Y).Max() - vehicles.Select(v => v.Y).Min();
			var rotationAngle = width > height
				? -45.ToRadians()
				: 45.ToRadians();

			army
				.Select(Id)
				.RotateBy(rotationAngle);
			command = CommandManager.PeekLastCommand(Id) as RotateCommand;
		}

		private void Bind(VehiclesGroup myArmy)
		{
			if (r2)
			{
				myArmy
					.SelectVehicles()
					.Assign(Id);
			}
			else
			{
				myArmy
					.SelectVehicles(VehicleType.Fighter)
					.AddToSelectionVehicles(VehicleType.Helicopter)
					.Assign(MagicConstants.AirFormationGroupId);
			}

			VehicleRegistry.RegisterNewFormation(Id, VehicleIds);
			binded = true;
		}
	}
}