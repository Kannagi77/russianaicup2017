using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class AttackMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Attack;
		private readonly List<Command> commands = new List<Command>();

		public AttackMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			if (commands.Any(c => !c.IsFinished(VehicleRegistry)))
				return StrategyState.Attack;

			commands.Clear();
			CommandManager.ClearCommandsQueue();
			var myVehicleIds = VehicleRegistry.MyVehicleIds(me);
			var myArmy = new VehiclesGroup(myVehicleIds, VehicleRegistry, CommandManager);
			var myVehicles = VehicleRegistry.GetVehiclesByIds(myVehicleIds).ToList();
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			var nextEnemiesGroup = NextEnemyGroup(myArmy.Center, enemyVehicles);
			var direction = myArmy.Center.To(nextEnemiesGroup?.GetCenterPoint()
			                                 ?? enemyVehicles
				                                 .OrderBy(v => v.GetDistanceTo(myArmy.Center))
				                                 .First()
				                                 .ToPoint());
			myArmy
				.Select(world)
				.MoveByVector(direction.Mul(0.1), world, game.TankSpeed * game.ForestTerrainSpeedFactor);
			commands.Add(CommandManager.PeekLastCommand());

			var closestToEnemy = myVehicles.GetClosest(enemyVehicles.GetCenterPoint());
			var nukeTarget = enemyVehicles.GetClosestAtMinimumRange(closestToEnemy, game.TacticalNuclearStrikeRadius * 0.9);
			if (me.RemainingNuclearStrikeCooldownTicks == 0 && nukeTarget != null && closestToEnemy.GetDistanceTo(nukeTarget) <= game.TacticalNuclearStrikeRadius)
			{
				myArmy
					.Nuke(closestToEnemy.Id, nukeTarget, world);
				commands.Add(CommandManager.PeekLastCommand());
				myArmy
					.Select(world)
					.MoveByVector(0, 0);
				return StrategyState.Shrink;
			}
			return StrategyState.Attack;
		}

		//note: uses poorly implemented DBSCAN algorithm
		private static IEnumerable<Vehicle> NextEnemyGroup(Point2D myArmyCenter, List<Vehicle> enemyVehicles)
		{
			const double radius = 15;
			const int minimumClusterSize = 3;

			var unvisitedVehicles = enemyVehicles.Select(v => v.Id).ToList();
			var clusters = new List<List<Vehicle>>();

			foreach (var currentVehicle in enemyVehicles)
			{
				var currentCluster = enemyVehicles
					.Where(v => unvisitedVehicles.Contains(v.Id) && v.GetDistanceTo(currentVehicle) < radius)
					.ToList();
				if (currentCluster.Count < minimumClusterSize)
				{
					continue;
				}

				foreach (var id in currentCluster.Select(v => v.Id).Where(id => unvisitedVehicles.Contains(id)))
				{
					unvisitedVehicles.Remove(id);
				}

				do
				{
					var newVehicles = FindNearbyVehicles(currentCluster, enemyVehicles, radius, minimumClusterSize);
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
#if DEBUG
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

		private static List<Vehicle> FindNearbyVehicles(ICollection<Vehicle> currentCluster,
			IReadOnlyCollection<Vehicle> enemyVehicles,
			double radius,
			int minimumClusterSize)
		{
			var result = new List<Vehicle>();
			foreach (var vehicle in currentCluster)
			{
				var newNearbyVehicles = enemyVehicles
					.Where(v => !currentCluster.Contains(v) && v.GetDistanceTo(vehicle) < radius)
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