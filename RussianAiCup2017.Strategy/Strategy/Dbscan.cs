using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	//note: uses poorly implemented DBSCAN algorithm
	public static class Dbscan
	{
		public static List<List<Vehicle>> Cluster(List<Vehicle> units, double radius, int minimumClusterSize)
		{
			var unvisitedVehicles = units.Select(u => u.Id).ToList();
			var clusters = new List<List<Vehicle>>();

			foreach (var currentVehicle in units)
			{
				if (!unvisitedVehicles.Contains(currentVehicle.Id))
					continue;
				var currentCluster = units
					.Where(v => unvisitedVehicles.Contains(v.Id) && v.GetDistanceTo(currentVehicle) < radius)
					.ToList();
				if (currentCluster.Count < minimumClusterSize)
				{
					continue;
				}

				foreach (var id in currentCluster.Select(v => v.Id))
				{
					unvisitedVehicles.Remove(id);
				}

				do
				{
					var newVehicles = FindNearbyVehicles(currentCluster, units, unvisitedVehicles, radius, minimumClusterSize);
					if (newVehicles.Count == 0)
						break;
					foreach (var newVehicle in newVehicles)
					{
						if (unvisitedVehicles.Contains(newVehicle.Id))
							unvisitedVehicles.Remove(newVehicle.Id);
						if (!currentCluster.Contains(newVehicle))
							currentCluster.Add(newVehicle);
					}
				} while (true);

				clusters.Add(currentCluster);
			}
			return clusters;
		}

		private static List<Vehicle> FindNearbyVehicles(ICollection<Vehicle> currentCluster,
			IReadOnlyCollection<Vehicle> enemyVehicles,
			ICollection<long> unvisitedVehicles,
			double radius,
			int minimumClusterSize)
		{
			var result = new List<Vehicle>();
			foreach (var vehicle in currentCluster)
			{
				var newNearbyVehicles = enemyVehicles
					.Where(v => unvisitedVehicles.Contains(v.Id) && v.GetDistanceTo(vehicle) < radius)
					.ToList();
				if (newNearbyVehicles.Count < minimumClusterSize)
				{
					continue;
				}
				result.AddRange(newNearbyVehicles);
			}
			return result;
		}
	}
}