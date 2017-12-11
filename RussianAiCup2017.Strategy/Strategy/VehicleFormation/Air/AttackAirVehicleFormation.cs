using System.Collections.Generic;
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
		private double maximumDurabilityTolerance = 0.75;
		private int nukePreventionTick;

		public AttackAirVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
#if DEBUG
			if (cachedTargetGroup != null)
			{
				foreach (var cachedTarget in VehicleRegistry.GetVehiclesByIds(cachedTargetGroup))
				{
					RewindClient.Instance.Rectangle(cachedTarget.X - game.VehicleRadius,
						cachedTarget.Y - game.VehicleRadius,
						cachedTarget.X + game.VehicleRadius,
						cachedTarget.Y + game.VehicleRadius,
						Color.Pink);
				}
			}
#endif
			var myArmy = new VehiclesGroup(Id, VehicleIds, VehicleRegistry, CommandManager);
			commands.RemoveAll(c => c.IsStarted() && c.IsFinished(world.TickIndex, VehicleRegistry));

			if (commands.Any())
				return new VehicleFormationResult(this);

			if (FormationHelper.IsNukeAlert(world.GetOpponentPlayer()) && world.TickIndex - nukePreventionTick > game.BaseTacticalNuclearStrikeCooldown / 2)
			{
				commands.Clear();
				CommandManager.ClearCommandsQueue(Id);
				this.PreventNuke(myArmy, world, game, commands);
				nukePreventionTick = world.TickIndex;
				return new VehicleFormationResult(this);
			}

			var myVehicles = VehicleRegistry.GetVehiclesByIds(VehicleIds).ToList();

			if (cachedTargetGroup != null
				&& this.TryNuke(myArmy, myVehicles, VehicleRegistry.GetVehiclesByIds(cachedTargetGroup), me, game, world, commands))
			{
				return new VehicleFormationResult(new ShrinkAirVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}

			if (commands.Any(c => !c.IsFinished(world.TickIndex, VehicleRegistry)))
				return new VehicleFormationResult(this);

			Vector2D direction;
			var groundFormationVehicles = VehicleRegistry.GetVehiclesByIds(
					VehicleRegistry.GetVehicleIdsByFormationId(MagicConstants.GroundFormationGroupId))
				.ToList();
			var myGroudForcesCenter = groundFormationVehicles.Any()
				? groundFormationVehicles.GetCenterPoint()
				: new Point2D(0, 0);
			if (TimeToRetreat(myVehicles))
			{
#if DEBUG
				RewindClient.Instance.Message("=== TIME TO RETREAT! ===");
#endif
				var ifvs = groundFormationVehicles.Where(v => v.Type == VehicleType.Ifv).ToList();
				if (ifvs.Any())
				{
					var ifvsCenter = ifvs.GetCenterPoint();
					if (myArmy.Center.GetDistanceTo(ifvsCenter) < 10)
					{
						return new VehicleFormationResult(this);
					}
					direction = myArmy.Center.To(ifvsCenter);
				}
				else
				{
					if (myArmy.Center.GetDistanceTo(myGroudForcesCenter) < 10)
					{
						myArmy
							.Select(MagicConstants.AirFormationGroupId)
							.Assign(MagicConstants.GroundFormationGroupId);
						return new VehicleFormationResult();
					}
					direction = myArmy.Center.To(myGroudForcesCenter);
				}
			}
			else
			{
				var nextTargetGroup = NextTargetGroup(myArmy, world, me);
				var closest = VehicleRegistry.GetVehiclesByIds(nextTargetGroup).GetClosest(myGroudForcesCenter);
				var nextTargetClosestPoint = nextTargetGroup.Any()
					? (closest?.ToPoint() ?? new Point2D(0, 0))
					: VehicleRegistry
						  .GetUncapturedFacilities(world, me)
						  .Select(f => f.ToPoint(game))
						  .FirstOrDefault();

				var minimumDistanceToNextTargetCenter = myVehicles.GetMinimumDistanceTo(nextTargetClosestPoint);
				var minimumDistanceToNextTargetCenterCondition = minimumDistanceToNextTargetCenter > 0.8 * game.HelicopterVisionRange;
				var nextTargetGroupCount = nextTargetGroup.Count;
				var myVehiclesCount = VehicleIds.Count;
				var countCondition = nextTargetGroupCount < myVehiclesCount / 2 - 1;
				var myForcesCenterToNextTargetCenterDistance = myGroudForcesCenter.GetDistanceTo(nextTargetClosestPoint);
				var myGroundForcesToMyArmyCenterCondition = myGroudForcesCenter.GetDistanceTo(myArmy.Center);
				var myGroundForcesCondition = myForcesCenterToNextTargetCenterDistance < myGroundForcesToMyArmyCenterCondition;

#if DEBUG
				RewindClient.Instance.Message($"=== minimumDistanceToNextTargetCenter = {minimumDistanceToNextTargetCenter} ===");
				RewindClient.Instance.Message($"=== minimumDistanceToNextTargetCenterCondition = {minimumDistanceToNextTargetCenterCondition} ===");
				RewindClient.Instance.Message($"=== nextTargetGroupCount = {nextTargetGroupCount} ===");
				RewindClient.Instance.Message($"=== myVehiclesCount = {myVehiclesCount} ===");
				RewindClient.Instance.Message($"=== countCondition = {countCondition} ===");
				RewindClient.Instance.Message($"=== myForcesCenterToNextTargetCenterDistance = {myForcesCenterToNextTargetCenterDistance} ===");
				RewindClient.Instance.Message($"=== myGroundForcesToMyArmyCenterCondition = {myGroundForcesToMyArmyCenterCondition} ===");
				RewindClient.Instance.Message($"=== myGroundForcesCondition = {myGroundForcesCondition} ===");
#endif

				direction = minimumDistanceToNextTargetCenterCondition
					  || countCondition
					  || myGroundForcesCondition
						? myArmy.Center.To(nextTargetClosestPoint)
						: nextTargetClosestPoint.To(myArmy.Center);
			}

			myArmy
				.Select(Id)
				.MoveByVector(direction.Length() > 5
						? direction.Mul(0.1)
						: direction,
					game.HelicopterSpeed * game.RainWeatherSpeedFactor);
#if DEBUG
			RewindClient.Instance.Line(myArmy.Center.X, myArmy.Center.Y, myArmy.Center.X + direction.X, myArmy.Center.Y + direction.Y, Color.Fuchsia);
#endif
			commands.Add(CommandManager.PeekLastCommand(Id));
			return new VehicleFormationResult(this);
		}

		private bool TimeToRetreat(IReadOnlyCollection<Vehicle> vehicles)
		{
			var totalDurability = vehicles.Sum(v => v.MaxDurability);
			var currentDurability = vehicles.Sum(v => v.Durability);
			return currentDurability / (double) totalDurability < maximumDurabilityTolerance;
		}

		private List<long> NextTargetGroup(VehiclesGroup myArmy, World world, Player me)
		{
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			if (!enemyVehicles.Any())
				return Enumerable
					.Empty<long>()
					.ToList();
			if (world.TickIndex - lastClusteringTick > CacheTtl)
			{
				var nextEnemyGroup = NextEnemyGroup(myArmy.Center, enemyVehicles, world.TickIndex)?.ToList();
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

		private static IEnumerable<Vehicle> NextEnemyGroup(Point2D myArmyCenter, List<Vehicle> enemyVehicles, int tick)
		{
			var clusters = Dbscan.GetEnemiesClusters(enemyVehicles, DbscanRadius, DbscanMinimumClusterSize, tick);
			return clusters.OrderBy(c => c.GetCenterPoint().GetDistanceTo(myArmyCenter)).FirstOrDefault();
		}
	}
}