using System.Collections.Generic;
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
		private int nukePreventionTick;
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
				return new VehicleFormationResult(new ShrinkGroundVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}

			if (commands.Any(c => !c.IsFinished(world.TickIndex, VehicleRegistry)))
				return new VehicleFormationResult(this);

			var nextTarget = NextTarget(myArmy, world, me, game);
			var direction = myArmy.Center.To(nextTarget);
			myArmy
				.Select(Id)
				.MoveByVector(direction.Length() > 20
						? direction.Mul(0.1)
						: direction,
					game.TankSpeed * game.ForestTerrainSpeedFactor);
#if DEBUG
			RewindClient.Instance.Line(myArmy.Center.X, myArmy.Center.Y, nextTarget.X, nextTarget.Y, Color.Fuchsia);
#endif
			commands.Add(CommandManager.PeekLastCommand(Id));
			return new VehicleFormationResult(this);
		}

		private Point2D NextTarget(VehiclesGroup myArmy, World world, Player me, Game game)
		{
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			if (world.TickIndex - lastClusteringTick <= CacheTtl)
			{
				return cachedTarget;
			}
			var closestUncapturedFacility = VehicleRegistry.GetUncapturedFacilities(world, me, Id)
				.OrderBy(f => myArmy.Center.GetDistanceTo(f.ToPoint(game)))
				.FirstOrDefault();

			if (closestUncapturedFacility != null)
			{
				VehicleRegistry.SetCaptureTarget(closestUncapturedFacility.Id, Id);
				cachedTarget = closestUncapturedFacility.ToPoint(game);
				return cachedTarget;
			}

			var nextEnemyGroup = NextEnemyGroup(myArmy.Center, enemyVehicles, world.TickIndex)?.ToList();
			if (nextEnemyGroup != null)
			{
				cachedTargetGroup = nextEnemyGroup.Select(v => v.Id).ToList();
				cachedTarget = nextEnemyGroup.GetCenterPoint();
				lastClusteringTick = world.TickIndex;
				return cachedTarget;
			}

			return new Point2D(world.Height, world.Width);
		}

		private static IEnumerable<Vehicle> NextEnemyGroup(Point2D myArmyCenter, List<Vehicle> enemyVehicles, int tick)
		{
			var clusters = Dbscan.GetEnemiesClusters(enemyVehicles, DbscanRadius, DbscanMinimumClusterSize, tick);
			return clusters.OrderBy(c => c.GetCenterPoint().GetDistanceTo(myArmyCenter)).FirstOrDefault();
		}
	}
}