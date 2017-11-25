using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class VehicleRegistry
	{
		private readonly Dictionary<long, Vehicle> vehiclesByIds = new Dictionary<long, Vehicle>();
		private readonly HashSet<long> idleVehicleIds = new HashSet<long>();

		public void Update(World world, Player me, Game game)
		{
			foreach (var vehicle in world.NewVehicles)
			{
				vehiclesByIds.Add(vehicle.Id, vehicle);
			}
			var updates = world.VehicleUpdates;
			foreach (var update in updates)
			{
				var vehicle = vehiclesByIds[update.Id];
				vehiclesByIds.Remove(vehicle.Id);
				if (update.Durability == 0)
				{
					continue;
				}
				if (Math.Abs(update.X - vehicle.X) < MagicConstants.Eps && Math.Abs(update.Y - vehicle.Y) < MagicConstants.Eps)
					idleVehicleIds.Add(vehicle.Id);
				else if (idleVehicleIds.Contains(vehicle.Id))
					idleVehicleIds.Remove(vehicle.Id);
				vehiclesByIds.Add(vehicle.Id, new Vehicle(vehicle, update));
			}
			foreach (var vehicle in vehiclesByIds.Where(v => updates.All(vu => vu.Id != v.Key)))
			{
				idleVehicleIds.Add(vehicle.Key);
			}
#if DEBUG
			foreach (var vehicle in vehiclesByIds.Values)
			{
				RewindClient.Instance.LivingUnit(vehicle.X,
					vehicle.Y,
					game.VehicleRadius,
					vehicle.Durability,
					game.ArrvDurability,
					vehicle.PlayerId == me.Id ? Side.Our : Side.Enemy,
					vehicle.Type);
				if (vehicle.IsSelected)
				{
					RewindClient.Instance.Rectangle(vehicle.X - game.VehicleRadius,
						vehicle.Y - game.VehicleRadius,
						vehicle.X + game.VehicleRadius,
						vehicle.Y + game.VehicleRadius,
						Color.Crimson);
				}
			}
			RewindClient.Instance.End();
#endif
		}

		public Vehicle GetVehicleById(long id) => vehiclesByIds.ContainsKey(id) ? vehiclesByIds[id] : null;

		public List<Vehicle> GetVehiclesByIds(IEnumerable<long> ids) => ids.Select(GetVehicleById).Where(v => v != null).ToList();

		public List<Vehicle> MyVehicles(Player me)
		{
			return vehiclesByIds.Values.Where(v => v.PlayerId == me.Id).ToList();
		}

		public List<long> MyVehicleIds(Player me) => vehiclesByIds.Where(p => p.Value.PlayerId == me.Id).Select(p => p.Key).ToList();

		public List<Vehicle> EnemyVehicles(Player me)
		{
			return vehiclesByIds.Values.Where(v => v.PlayerId != me.Id).ToList();
		}

		public List<Vehicle> SelectedVehicles(Player me)
		{
			return MyVehicles(me).Where(v => v.IsSelected).ToList();
		}

		public bool IsVehicleIdle(Vehicle vehicle) => idleVehicleIds.Contains(vehicle.Id);
		public bool IsVehicleIdle(long vehicleId) => idleVehicleIds.Contains(vehicleId);
		public void ForceUnidleVehicle(Vehicle vehicle) => idleVehicleIds.Remove(vehicle.Id);

		public IList<long> FilterDeadVehicles(IEnumerable<long> vehicleIds)
		{
			return vehicleIds.Where(id => vehiclesByIds.ContainsKey(id)).ToList();
		}
	}
}