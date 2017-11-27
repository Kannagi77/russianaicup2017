using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	//note: uses poorly implemented DBSCAN algorithm
	public static class Dbscan
	{
		public static List<List<Vehicle>> Cluster(List<Vehicle> vehicles, double radius, int minimumClusterSize)
		{
			var unitsByX = new Dictionary<int, List<Vehicle>>();
			var unitsByY = new Dictionary<int, List<Vehicle>>();
			foreach (var vehicle in vehicles)
			{
				var x = (int) vehicle.X;
				var y = (int) vehicle.Y;
				if (!unitsByX.ContainsKey(x))
					unitsByX.Add(x, new List<Vehicle>());
				if(!unitsByY.ContainsKey(y))
					unitsByY.Add(y, new List<Vehicle>());
				unitsByX[x].Add(vehicle);
				unitsByY[y].Add(vehicle);
			}

			var unvisitedVehicleIds = vehicles.Select(u => u.Id).ToList();
			var clusters = new List<List<Vehicle>>();

			foreach (var currentVehicle in vehicles)
			{
				if (!unvisitedVehicleIds.Contains(currentVehicle.Id))
					continue;
				var currentCluster = GetNearbyVehicles(currentVehicle, radius, unitsByX, unitsByY)
					.Where(v => unvisitedVehicleIds.Contains(v.Id) && v.GetDistanceTo(currentVehicle) < radius)
					.ToList();
				if (currentCluster.Count < minimumClusterSize)
				{
					continue;
				}

				foreach (var id in currentCluster.Select(v => v.Id))
				{
					unvisitedVehicleIds.Remove(id);
				}

				do
				{
					var newVehicles = FindNearbyVehicles(currentCluster, unitsByX, unitsByY, unvisitedVehicleIds, radius, minimumClusterSize);
					if (newVehicles.Count == 0)
						break;
					foreach (var newVehicle in newVehicles)
					{
						if (unvisitedVehicleIds.Contains(newVehicle.Id))
							unvisitedVehicleIds.Remove(newVehicle.Id);
						if (!currentCluster.Contains(newVehicle))
							currentCluster.Add(newVehicle);
					}
				} while (true);

				clusters.Add(currentCluster);
			}
			return clusters;
		}

		private static List<Vehicle> FindNearbyVehicles(IEnumerable<Vehicle> currentCluster,
			Dictionary<int, List<Vehicle>> vehiclesByX,
			Dictionary<int, List<Vehicle>> vehiclesByY,
			ICollection<long> unvisitedVehicleIds,
			double radius,
			int minimumClusterSize)
		{
			var result = new List<Vehicle>();
			foreach (var vehicle in currentCluster)
			{
				var newNearbyVehicles = GetNearbyVehicles(vehicle, radius, vehiclesByX, vehiclesByY)
					.Where(v => unvisitedVehicleIds.Contains(v.Id) && v.GetDistanceTo(vehicle) < radius)
					.ToList();
				if (newNearbyVehicles.Count < minimumClusterSize)
				{
					continue;
				}
				result.AddRange(newNearbyVehicles);
			}
			return result;
		}

		private static IEnumerable<Vehicle> GetNearbyVehicles(Unit vehicle,
			double radius,
			Dictionary<int, List<Vehicle>> vehiclesByX,
			Dictionary<int, List<Vehicle>> vehiclesByY)
		{
			return vehiclesByX
				.Where(p => vehicle.X - radius <= p.Key && vehicle.X + radius >= p.Key)
				.SelectMany(p => p.Value)
				.Concat(vehiclesByY
					.Where(p => vehicle.Y - radius <= p.Key && vehicle.Y + radius >= p.Key)
					.SelectMany(p => p.Value))
				.ToList();
		}
	}
}