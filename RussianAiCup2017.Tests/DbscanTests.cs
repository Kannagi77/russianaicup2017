using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy;
using NUnit.Framework;

namespace RussianAiCup2017.Tests
{
	public class DbscanTests
	{
		[Test]
		public void ClusterTest()
		{
			var vehicles = new List<Vehicle>
			{
				CreateVehicle(1, 1, 1),
				CreateVehicle(2, 2, 1),
				CreateVehicle(3, 1, 2),
				CreateVehicle(4, 15, 1),

				CreateVehicle(5, 100, 100),
				CreateVehicle(6, 101, 110),
				CreateVehicle(7, 105, 110),
				CreateVehicle(8, 101, 110),

				CreateVehicle(9, 401, 410),
				CreateVehicle(10, 401, 410),
				CreateVehicle(11, 401, 410),

				CreateVehicle(12, 501, 510),
				CreateVehicle(13, 501, 510),
			};
			var clusters = Dbscan.Cluster(vehicles, 15, 3);
			Assert.AreEqual(3, clusters.Count);

			var firstCluster = clusters.ElementAt(0).Select(v => v.Id).ToList();
			Assert.AreEqual(4, firstCluster.Count);
			Assert.True(firstCluster.Contains(1));
			Assert.True(firstCluster.Contains(2));
			Assert.True(firstCluster.Contains(3));
			Assert.True(firstCluster.Contains(4));

			var secondCluster = clusters.ElementAt(1).Select(v => v.Id).ToList();
			Assert.AreEqual(4, secondCluster.Count);
			Assert.True(secondCluster.Contains(5));
			Assert.True(secondCluster.Contains(6));
			Assert.True(secondCluster.Contains(7));
			Assert.True(secondCluster.Contains(8));

			var thirdCluster = clusters.ElementAt(2).Select(v => v.Id).ToList();
			Assert.AreEqual(3, thirdCluster.Count);
			Assert.True(thirdCluster.Contains(9));
			Assert.True(thirdCluster.Contains(10));
			Assert.True(thirdCluster.Contains(11));
		}

		private static readonly Random Rnd = new Random();
		[Test]
		[TestCase( 500,  15,  3, 10,  35)]
		[TestCase( 500,  50,  3, 10,  55)]
		[TestCase( 500, 100, 10, 10,  80)]
		[TestCase(1000,  15,  3, 10,  65)]
		[TestCase(1000, 150,  3, 10, 833)]
		[TestCase(1000, 150, 30, 10, 855)]
		[TestCase(1000, 400, 50, 10, 725)]
		public void BenchmarkTest(int unitsCount, double radius, int minimumClusterSize, int runsCount, double threshold)
		{
			var stopwatch = new Stopwatch();
			for (var i = 0; i < runsCount; i++)
			{
				var vehicles = GenerateVehicles(unitsCount);
				stopwatch.Start();
				var clusters = Dbscan.Cluster(vehicles, radius, minimumClusterSize);
				stopwatch.Stop();
				Console.WriteLine($"Clusters found: {clusters.Count}, current time spent: {stopwatch.Elapsed}");
			}
			var average = TimeSpan.FromMilliseconds((double)stopwatch.ElapsedMilliseconds / runsCount);
			Console.WriteLine($"Avarage: {average}");
			Assert.Less(average, TimeSpan.FromMilliseconds(threshold));
		}

		private static List<Vehicle> GenerateVehicles(int count)
		{
			return Enumerable
				.Range(0, count)
				.Select(i => CreateVehicle(i, GenerateCoord(), GenerateCoord()))
				.ToList();
		}

		private static Vehicle CreateVehicle(long id, double x, double y)
		{
			return new Vehicle(id, x, y, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, VehicleType.Tank, true, true, new int[0]);
		}

		private static double GenerateCoord()
		{
			return Rnd.Next(102401) / 100.0;
		}
	}
}
