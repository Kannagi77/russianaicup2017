using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy;
using NUnit.Framework;

namespace RussianAiCup2017.Tests
{
	public class BenchmarkTests
	{
		private static readonly Random Rnd = new Random();
		[Test]
		[TestCase(500, 15, 3, 10)]
		[TestCase(500, 50, 3, 10)]
		[TestCase(500, 100, 10, 10)]
		[TestCase(1000, 15, 3, 10)]
		[TestCase(1000, 150, 3, 10)]
		[TestCase(1000, 150, 30, 10)]
		[TestCase(1000, 400, 50, 10)]
		public void TestDbscan(int unitsCount, double radius, int minimumClusterSize, int runsCount)
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
			Console.WriteLine($"Avarage: {TimeSpan.FromMilliseconds((double) stopwatch.ElapsedMilliseconds / runsCount)}");
		}

		private List<Vehicle> GenerateVehicles(int count)
		{
			return Enumerable
				.Range(0, count)
				.Select(i => new Vehicle(i, GenerateCoord(), GenerateCoord(), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, VehicleType.Tank, true, true, new int[0]))
				.ToList();
		}

		private double GenerateCoord()
		{
			return Rnd.Next(102401) / 100.0;
		}
	}
}
