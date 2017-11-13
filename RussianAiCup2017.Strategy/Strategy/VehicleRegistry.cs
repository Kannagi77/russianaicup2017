using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class VehicleRegistry
	{
		private List<VehicleWrapper> vehicles = new List<VehicleWrapper>();
		public void Update(World world)
		{
			var updatedVehicles = new List<VehicleWrapper>();
			vehicles.AddRange(world.NewVehicles.Select(nv => new VehicleWrapper(nv)));
			foreach (var vehicleWrapper in vehicles)
			{
				var currentUpdate = world.VehicleUpdates.FirstOrDefault(v => v.Id == vehicleWrapper.Id);
				if (currentUpdate == null)
				{
					vehicleWrapper.IsIdle = true;
					updatedVehicles.Add(vehicleWrapper);
					continue;
				}
				if (currentUpdate.Durability == 0)
				{
					continue;
				}
				if (Math.Abs(currentUpdate.X - vehicleWrapper.X) < double.Epsilon &&
				    Math.Abs(currentUpdate.Y - vehicleWrapper.Y) < double.Epsilon)
				{
					vehicleWrapper.IsIdle = true;
				}
				else
				{
					vehicleWrapper.IsIdle = false;
				}
				vehicleWrapper.Vehicle = new Vehicle(vehicleWrapper.Vehicle, currentUpdate);
				updatedVehicles.Add(vehicleWrapper);
			}
			vehicles = updatedVehicles;
		}

		public List<VehicleWrapper> MyVehicles(Player me)
		{
			return vehicles.Where(v => v.PlayerId == me.Id).ToList();
		}

		public List<VehicleWrapper> EnemyVehicles(Player me)
		{
			return vehicles.Where(v => v.PlayerId != me.Id).ToList();
		}

		public List<VehicleWrapper> SelectedVehicles(Player me)
		{
			return MyVehicles(me).Where(v => v.IsSelected).ToList();
		}
	}
}