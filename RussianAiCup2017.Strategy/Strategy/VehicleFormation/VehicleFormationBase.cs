using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation
{
	public abstract class VehicleFormationBase : IVehicleFormation
	{
		public int Id { get; }
		public CommandManager CommandManager { get; }
		public VehicleRegistry VehicleRegistry { get; }
		protected readonly List<long> VehicleIds;

		protected VehicleFormationBase(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
		{
			Id = id;
			VehicleIds = vehicleRegistry.FilterDeadVehicles(vehicleIds).ToList();
			CommandManager = commandManager;
			VehicleRegistry = vehicleRegistry;
		}

		public abstract VehicleFormationResult PerformAction(World world, Player me, Game game);
	}
}