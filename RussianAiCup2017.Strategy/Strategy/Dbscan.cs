using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	//note: uses poorly implemented DBSCAN algorithm
	public static class Dbscan
	{
		private static int lastClusteringTick;
		public static List<List<Vehicle>> LastCacheResult = new List<List<Vehicle>>();

		public static List<List<Vehicle>> GetEnemiesClusters(List<Vehicle> vehicles, double radius, int minimumClusterSize, int tick)
		{
			if (tick - lastClusteringTick > MagicConstants.EnemiesClusteringRate)
			{
				LastCacheResult = Cluster(vehicles, radius, minimumClusterSize);
				lastClusteringTick = tick;
			}
			return LastCacheResult;
		}

		public static List<List<Vehicle>> Cluster(List<Vehicle> vehicles, double radius, int minimumClusterSize)
		{
#if DEBUG
			var stopwatch = new Stopwatch();
			stopwatch.Start();
#endif
			var unitsByX = new Dictionary<int, HashSet<Vehicle>>();
			var unitsByY = new Dictionary<int, HashSet<Vehicle>>();
			foreach (var vehicle in vehicles)
			{
				var x = (int) vehicle.X;
				var y = (int) vehicle.Y;
				if (!unitsByX.ContainsKey(x))
					unitsByX.Add(x, new HashSet<Vehicle>());
				if(!unitsByY.ContainsKey(y))
					unitsByY.Add(y, new HashSet<Vehicle>());
				unitsByX[x].Add(vehicle);
				unitsByY[y].Add(vehicle);
			}
			var orderedUnitsByX = unitsByX.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
			var orderedUnitsByY = unitsByY.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);

			var unvisitedVehicleIds = new HashSet<long>();
			foreach (var id in vehicles.Select(u => u.Id))
			{
				unvisitedVehicleIds.Add(id);
			}
			var clusters = new List<List<Vehicle>>();

			foreach (var currentVehicle in vehicles)
			{
				if (!unvisitedVehicleIds.Contains(currentVehicle.Id))
					continue;
				var currentCluster = (HashSet<Vehicle>) GetNearbyVehicles(currentVehicle, radius, orderedUnitsByX, orderedUnitsByY, unvisitedVehicleIds);
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
					var newVehicles = FindNearbyVehicles(currentCluster, orderedUnitsByX, orderedUnitsByY, unvisitedVehicleIds, radius, minimumClusterSize)
						.ToArray();
					if (newVehicles.Length == 0)
						break;
					foreach (var newVehicle in newVehicles)
					{
						if (unvisitedVehicleIds.Contains(newVehicle.Id))
							unvisitedVehicleIds.Remove(newVehicle.Id);
						if (!currentCluster.Contains(newVehicle))
							currentCluster.Add(newVehicle);
					}
				} while (true);

				clusters.Add(currentCluster.ToList());
			}
#if DEBUG
			stopwatch.Stop();
			RewindClient.Instance.Message($"Clustering time: {stopwatch.Elapsed}");
			foreach (var cluster in clusters)
			{
				var x1 = cluster.Select(v => v.X).Min();
				var y1 = cluster.Select(v => v.Y).Min();
				var x2 = cluster.Select(v => v.X).Max();
				var y2 = cluster.Select(v => v.Y).Max();
				RewindClient.Instance.Rectangle(x1, y1, x2, y2, Color.Yellow);
			}
#endif
			return clusters;
		}

		private static IEnumerable<Vehicle> FindNearbyVehicles(IEnumerable<Vehicle> currentCluster,
			Dictionary<int, HashSet<Vehicle>> vehiclesByX,
			Dictionary<int, HashSet<Vehicle>> vehiclesByY,
			ICollection<long> unvisitedVehicleIds,
			double radius,
			int minimumClusterSize)
		{
			return currentCluster
				.Select(vehicle => GetNearbyVehicles(vehicle, radius, vehiclesByX, vehiclesByY, unvisitedVehicleIds).ToList())
				.Where(newNearbyVehicles => newNearbyVehicles.Count >= minimumClusterSize)
				.SelectMany(newNearbyVehicles => newNearbyVehicles);
		}

		private static IEnumerable<Vehicle> GetNearbyVehicles(Unit vehicle,
			double radius,
			Dictionary<int, HashSet<Vehicle>> vehiclesByX,
			Dictionary<int, HashSet<Vehicle>> vehiclesByY,
			ICollection<long> unvisitedVehicleIds)
		{
			var result = new HashSet<Vehicle>();
			foreach (var v in vehiclesByX
				.SkipWhile(p => p.Key < vehicle.X - radius)
				.TakeWhile(p => p.Key <= vehicle.X + radius)
				.SelectMany(p => p.Value)
				.Concat(vehiclesByY
					.SkipWhile(p => p.Key < vehicle.Y - radius)
					.TakeWhile(p => p.Key <= vehicle.Y + radius)
					.SelectMany(p => p.Value))
				.Distinct()
				.Where(v => unvisitedVehicleIds.Contains(v.Id) && v.GetDistanceTo(vehicle) < radius))
			{
				result.Add(v);
			}
			return result;
		}
	}
}