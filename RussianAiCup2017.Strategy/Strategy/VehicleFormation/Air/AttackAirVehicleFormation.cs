using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Air
{
	public class AttackAirVehicleFormation : VehicleFormationBase
	{
		private readonly List<Command> commands = new List<Command>();
		private const int CacheTtl = 35;
		private const double DbscanRadius = 15;
		private const int DbscanMinimumClusterSize = 3;
		private List<long> cachedTargetGroup;
		private int lastClusteringTick;

		public AttackAirVehicleFormation(int id,
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
				return new VehicleFormationResult(new ShrinkAirVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}

			if (commands.Any(c => !c.IsFinished(world.TickIndex, VehicleRegistry)))
				return new VehicleFormationResult(this);

			Vector2D direction;
			if (myVehicles.Count < 100)
			{
				var myGroudForcesCenter = VehicleRegistry
					.MyVehicles(me)
					.Where(v => v.Type == VehicleType.Tank || v.Type == VehicleType.Arrv || v.Type == VehicleType.Ifv)
					.ToList()
					.GetCenterPoint();
				if (myArmy.Center.GetDistanceTo(myGroudForcesCenter) < 50)
				{
					myArmy
						.Select(MagicConstants.AirFormationGroupId)
						.Assign(MagicConstants.GroundFormationGroupId);
					return new VehicleFormationResult();
				}
				direction = myArmy.Center.To(myGroudForcesCenter);
			}
			else
			{
				var nextTargetGroup = NextTargetGroup(myArmy, world, me);
				var nextTargetGroupCenter = VehicleRegistry.GetVehiclesByIds(nextTargetGroup).GetCenterPoint();

				direction = myVehicles.GetMinimumDistanceTo(nextTargetGroupCenter) > 0.8 * game.HelicopterVisionRange
					  || nextTargetGroup.Count < VehicleIds.Count / 2 - 1
						? myArmy.Center.To(nextTargetGroupCenter)
						: nextTargetGroupCenter.To(myArmy.Center);
			}

			myArmy
				.Select(MagicConstants.AirFormationGroupId)
				.MoveByVector(direction.Length() > 5
						? direction.Mul(0.1)
						: direction,
					game.HelicopterSpeed * game.RainWeatherSpeedFactor);
			commands.Add(CommandManager.PeekLastCommand(Id));
			return new VehicleFormationResult(this);
		}

		private List<long> NextTargetGroup(VehiclesGroup myArmy, World world, Player me)
		{
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			if (world.TickIndex - lastClusteringTick > CacheTtl)
			{
				var nextEnemyGroup = NextEnemyGroup(myArmy.Center, enemyVehicles)?.ToList();
				cachedTargetGroup = nextEnemyGroup?
					                    .Select(v => v.Id)
					                    .ToList()
				                    ?? new List<long>
				                    {
					                    enemyVehicles
						                    .OrderBy(v => v.GetDistanceTo(myArmy.Center))
						                    .First()
						                    .Id
				                    };
				lastClusteringTick = world.TickIndex;
				return cachedTargetGroup;
			}
			return cachedTargetGroup;
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
				RewindClient.Instance.Rectangle(x1, y1, x2, y2, Color.Aqua);
			}
#endif
			return clusters.OrderBy(c => c.GetCenterPoint().GetDistanceTo(myArmyCenter)).FirstOrDefault();
		}
	}
}