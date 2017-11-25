using System.Collections.Generic;
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
			var enemiesCenter = enemyVehicles.GetCenterPoint();
			var direction = myArmy.Center.To(enemiesCenter);
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
	}
}