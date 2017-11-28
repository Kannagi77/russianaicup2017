using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			var myVehicleIds = VehicleRegistry.MyVehicleIds(me);

			if (IsNukeAlert(world.GetOpponentPlayer()))
			{
				PreventNuke(myVehicleIds, world, game);
				return StrategyState.Attack;
			}

			if (commands.Any(c => !c.IsFinished(world.TickIndex, VehicleRegistry)))
				return StrategyState.Attack;

			commands.Clear();
			CommandManager.ClearCommandsQueue();

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

		private static bool IsNukeAlert(Player opponentPlayer)
		{
			return opponentPlayer.NextNuclearStrikeX > 0 && opponentPlayer.NextNuclearStrikeY > 0;
		}

		private void PreventNuke(IList<long> myVehicleIds, World world, Game game)
		{
			var opponentPlayer = world.GetOpponentPlayer();
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, true), world.TickIndex);
			CommandManager.EnqueueCommand(new MoveCommand(myVehicleIds, 0, 0), world.TickIndex);
			CommandManager.EnqueueCommand(new SelectCommand(opponentPlayer.NextNuclearStrikeX - game.TacticalNuclearStrikeRadius,
				opponentPlayer.NextNuclearStrikeY - game.TacticalNuclearStrikeRadius,
				opponentPlayer.NextNuclearStrikeX + game.TacticalNuclearStrikeRadius,
				opponentPlayer.NextNuclearStrikeY + game.TacticalNuclearStrikeRadius,
				true), world.TickIndex);
			var scaleCommand = new ScaleCommand(myVehicleIds,
				opponentPlayer.NextNuclearStrikeX,
				opponentPlayer.NextNuclearStrikeY,
				10.0,
				isFinished: tick => tick - world.TickIndex > game.TacticalNuclearStrikeDelay);
			CommandManager.EnqueueCommand(scaleCommand, world.TickIndex);
			var unscaleCommand = new ScaleCommand(myVehicleIds, opponentPlayer.NextNuclearStrikeX, opponentPlayer.NextNuclearStrikeY, 0.1);
			CommandManager.EnqueueCommand(unscaleCommand, world.TickIndex);
			commands.Add(scaleCommand);
			commands.Add(unscaleCommand);
		}

		private static IEnumerable<Vehicle> NextEnemyGroup(Point2D myArmyCenter, List<Vehicle> enemyVehicles)
		{
			const double radius = 15;
			const int minimumClusterSize = 3;

#if DEBUG
			var stopwatch = new Stopwatch();
			stopwatch.Start();
#endif
			var clusters = Dbscan.Cluster(enemyVehicles, radius, minimumClusterSize);
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