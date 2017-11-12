using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class VehicleRegistry
	{
		private readonly List<Vehicle> vehicles = new List<Vehicle>();
		public void Update(World world)
		{
			vehicles.AddRange(world.NewVehicles);
			foreach (var vehicleUpdate in world.VehicleUpdates)
			{
				var currentVehicle = vehicles.FirstOrDefault(v => v.Id == vehicleUpdate.Id);
				if (currentVehicle == null)
					continue;
				vehicles.Remove(currentVehicle);
				if (vehicleUpdate.Durability == 0)
				{
					continue;
				}
				vehicles.Add(new Vehicle(currentVehicle, vehicleUpdate));
			}
		}

		public List<Vehicle> MyVehicles(Player me)
		{
			return vehicles.Where(v => v.PlayerId == me.Id).ToList();
		}

		public List<Vehicle> EnemyVehicles(Player me)
		{
			return vehicles.Where(v => v.PlayerId != me.Id).ToList();
		}

		public List<Vehicle> SelectedVehicles(Player me)
		{
			return MyVehicles(me).Where(v => v.IsSelected).ToList();
		}
	}
}