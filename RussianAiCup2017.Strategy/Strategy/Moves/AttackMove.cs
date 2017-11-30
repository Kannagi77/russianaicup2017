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
		private const int CacheTtl = 35;
		private const double DbscanRadius = 15;
		private const int DbscanMinimumClusterSize = 3;
		private Point2D cachedTarget;
		private List<long> cachedTargetGroup;
		private int lastClusteringTick;

		public AttackMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var myVehicleIds = VehicleRegistry.MyVehicleIds(me);
			var myArmy = new VehiclesGroup(myVehicleIds, VehicleRegistry, CommandManager);
			var myVehicles = VehicleRegistry.GetVehiclesByIds(myVehicleIds).ToList();
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);

			if (IsNukeAlert(world.GetOpponentPlayer()))
			{
				PreventNuke(myVehicleIds, world, game);
				return StrategyState.Attack;
			}

			if (TryNuke(myArmy, myVehicles, me, game, world))
			{
				return StrategyState.Shrink;
			}

			if (commands.Any(c => !c.IsFinished(world.TickIndex, VehicleRegistry)))
				return StrategyState.Attack;

			commands.Clear();
			CommandManager.ClearCommandsQueue();

			var nextTarget = NextTarget(world, myArmy, enemyVehicles);
			var direction = myArmy.Center.To(nextTarget);
			myArmy
				.Select(world)
				.MoveByVector(direction.Length() > 20
						? direction.Mul(0.1)
						: direction,
					world,
					game.TankSpeed * game.ForestTerrainSpeedFactor);
			commands.Add(CommandManager.PeekLastCommand());
			return StrategyState.Attack;
		}

		private bool TryNuke(VehiclesGroup myArmy,
			IReadOnlyCollection<Vehicle> myVehicles,
			Player me,
			Game game,
			World world)
		{
			if (me.RemainingNuclearStrikeCooldownTicks != 0)
			{
				return false;
			}
			if (cachedTargetGroup == null)
			{
				return false;
			}
			var enemyVehicles = VehicleRegistry.GetVehiclesByIds(cachedTargetGroup);
			if (myArmy.Center.GetDistanceTo(enemyVehicles.GetCenterPoint()) > 2 * game.TacticalNuclearStrikeRadius)
			{
				return false;
			}
			var nukeTarget = GetNukeTarget(enemyVehicles, myVehicles, game);
			if (nukeTarget == null)
			{
				return false;
			}
			var nukeGunner = GetNukeGunner(nukeTarget, myVehicles, world, game);
			if (nukeGunner == null)
			{
				return false;
			}
			myArmy
				.Nuke(nukeGunner.Id, nukeTarget, world);
			commands.Add(CommandManager.PeekLastCommand());
			myArmy
				.Select(world)
				.MoveByVector(0, 0);
			return true;
		}

		private static Vehicle GetNukeTarget(IReadOnlyList<Vehicle> enemyVehicles, IReadOnlyCollection<Vehicle> myVehicles, Game game)
		{
			var bestTarget = enemyVehicles.First();
			var topDamage = 0.0;
			for (var i = 1; i < enemyVehicles.Count; i++)
			{
				var currentTarget = enemyVehicles[i];
				var currentDamage = 0.0;
				foreach (var enemyVehicle in enemyVehicles
					.Where(v => v.Id != currentTarget.Id))
				{
					currentDamage += GetDamage(currentTarget, enemyVehicle, game);
				}
				foreach (var myVehicle in myVehicles)
				{
					currentDamage -= GetDamage(currentTarget, myVehicle, game);
				}

				if (currentDamage > topDamage)
				{
					bestTarget = currentTarget;
					topDamage = currentDamage;
				}
			}
			return topDamage > 0
				? bestTarget
				: null;
		}

		private static double GetDamage(Unit currentTarget, Unit vehicle, Game game)
		{
			var distance = currentTarget.GetDistanceTo(vehicle);
			var nukeRadius = game.TacticalNuclearStrikeRadius;
			if (distance >= nukeRadius)
				return 0;
			return (nukeRadius - distance) / game.MaxTacticalNuclearStrikeDamage;
		}

		private Vehicle GetNukeGunner(Unit target, IEnumerable<Vehicle> myVehicles, World world, Game game)
		{
			return myVehicles
				.Where(v => VehicleRegistry.GetVision(v.Id, world, game) >= v.GetDistanceTo(target))
				.OrderBy(v => v.Durability)
				.FirstOrDefault();
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

		private Point2D NextTarget(World world, VehiclesGroup myArmy, List<Vehicle> enemyVehicles)
		{
			if (world.TickIndex - lastClusteringTick > CacheTtl)
			{
				var nextEnemyGroup = NextEnemyGroup(myArmy.Center, enemyVehicles)?.ToList();
				if (nextEnemyGroup != null)
				{
					cachedTargetGroup = nextEnemyGroup.Select(v => v.Id).ToList();
					cachedTarget = nextEnemyGroup.GetCenterPoint();
					return cachedTarget;
				}
				var nextTarget = enemyVehicles
					.OrderBy(v => v.GetDistanceTo(myArmy.Center))
					.First()
					.ToPoint();
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