using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class AttackGroundVehicleFormation : VehicleFormationBase
	{
		private readonly List<Command> commands = new List<Command>();
		private const int CacheTtl = 35;
		private const double DbscanRadius = 15;
		private const int DbscanMinimumClusterSize = 3;
		private Point2D cachedTarget;
		private List<long> cachedTargetGroup;
		private int lastClusteringTick;
		public AttackGroundVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			var myArmy = new VehiclesGroup(Id, VehicleIds, VehicleRegistry, CommandManager);
			commands.RemoveAll(c => c.IsStarted() && c.IsFinished(world.TickIndex, VehicleRegistry));
			if (commands.Any())
				return new VehicleFormationResult(this);

			var myVehicles = VehicleRegistry.GetVehiclesByIds(VehicleIds).ToList();

			if (FormationHelper.IsNukeAlert(world.GetOpponentPlayer()))
			{
				this.PreventNuke(myArmy, world, game, commands);
				return new VehicleFormationResult(this);
			}

			if (cachedTargetGroup != null
				&& this.TryNuke(myArmy, myVehicles, VehicleRegistry.GetVehiclesByIds(cachedTargetGroup), me, game, world, commands))
			{
				return new VehicleFormationResult(new ShrinkGroundVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}

			if (commands.Any(c => !c.IsFinished(world.TickIndex, VehicleRegistry)))
				return new VehicleFormationResult(this);

			var nextTarget = NextTarget(myArmy, world, me, game);
			var direction = myArmy.Center.To(nextTarget);
			myArmy
				.Select(MagicConstants.GroundFormationGroupId)
				.MoveByVector(direction.Length() > 20
						? direction.Mul(0.1)
						: direction,
					game.TankSpeed * game.ForestTerrainSpeedFactor);
			commands.Add(CommandManager.PeekLastCommand(Id));
			return new VehicleFormationResult(this);
		}

		private Point2D NextTarget(VehiclesGroup myArmy, World world, Player me, Game game)
		{
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			if (world.TickIndex - lastClusteringTick > CacheTtl)
			{
				var closestUncapturedFacility = VehicleRegistry.GetUncapturedFacilities(world, me)
					.OrderBy(f => myArmy.Center.GetDistanceTo(f.ToPoint(game)))
					.FirstOrDefault();
				var nextEnemyGroup = NextEnemyGroup(myArmy.Center, enemyVehicles)?.ToList();
				if (nextEnemyGroup != null)
				{
					cachedTargetGroup = nextEnemyGroup.Select(v => v.Id).ToList();
					cachedTarget = closestUncapturedFacility != null
						&& myArmy.Center.GetDistanceTo(closestUncapturedFacility.ToPoint(game)) < myArmy.Center.GetDistanceTo(nextEnemyGroup.GetCenterPoint())
						? closestUncapturedFacility.ToPoint(game)
						: nextEnemyGroup.GetCenterPoint();
					return cachedTarget;
				}
				var nextEnemyTarget = enemyVehicles
					.OrderBy(v => v.GetDistanceTo(myArmy.Center))
					.First()
					.ToPoint();
				var nextTarget = closestUncapturedFacility != null
				                 && myArmy.Center.GetDistanceTo(closestUncapturedFacility.ToPoint(game)) <
				                 myArmy.Center.GetDistanceTo(nextEnemyTarget)
					? closestUncapturedFacility.ToPoint(game)
					: nextEnemyTarget;
				cachedTarget = nextTarget;
				lastClusteringTick = world.TickIndex;
				return nextTarget;
			}
			else
			{
				return cachedTarget;
			}
		}

		private static IEnumerable<Vehicle> NextEnemyGroup(Point2D myArmyCenter, List<Vehicle> enemyVehicles)
		{
#if DEBUG
			var stopwatch = new Stopwatch();
			stopwatch.Start();
#endif
			var clusters = Dbscan.Cluster(enemyVehicles, DbscanRadius, DbscanMinimumClusterSize);
#if DEBUG
			stopwatch.Stop();
			RewindClient.Instance.Message($"Clustering time: {stopwatch.Elapsed}");
			Console.WriteLine($"Clustering time: {stopwatch.Elapsed}");
			foreach (var cluster in clusters)
			{
				var x1 = cluster.Select(v => v.X).Min();
				var y1 = cluster.Select(v => v.Y).Min();
				var x2 = cluster.Select(v => v.X).Max();
				var y2 = cluster.Select(v => v.Y).Max();
				RewindClient.Instance.Rectangle(x1, y1, x2, y2, Color.Yellow);
			}
#endif
			return clusters.OrderBy(c => c.GetCenterPoint().GetDistanceTo(myArmyCenter)).FirstOrDefault();
		}
	}
}