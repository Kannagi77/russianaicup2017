using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class AttackMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Attack;
		private bool started;

		public AttackMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);
			var myArmyCenter = myVehicles.GetCenterPoint();
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			if (!started)
			{
				var enemiesCenterPoint = enemyVehicles.GetCenterPoint();
				CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height), world.TickIndex);
				CommandManager.EnqueueCommand(new MoveCommand(myVehicles,
					enemiesCenterPoint.X - myArmyCenter.X, enemiesCenterPoint.Y - myArmyCenter.Y, 0.2), world.TickIndex);
				started = true;
			}
			var isMyArmyStretched = IsMyArmyStretched(myVehicles);
			var minimumDistanceToEnemy = enemyVehicles.GetMinimumDistanceTo(myArmyCenter);
			if (!isMyArmyStretched && (minimumDistanceToEnemy > 200 || minimumDistanceToEnemy < 100))
			{
				var closestToEnemy = myVehicles.GetClosest(enemyVehicles.GetCenterPoint());
				var nukeTarget = enemyVehicles.GetClosestAtMinimumRange(closestToEnemy, game.TacticalNuclearStrikeRadius);
				if (nukeTarget != null)
				{
					CommandManager.EnqueueCommand(new NukeCommand(closestToEnemy.Id, nukeTarget.X, nukeTarget.Y), world.TickIndex);
				}
				return StrategyState.Attack;
			}
			CommandManager.ClearCommandsQueue();
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height), world.TickIndex);
			CommandManager.EnqueueCommand(new MoveCommand(myVehicles, 0, 0), world.TickIndex);
			started = false;
			return StrategyState.Shrink;
		}

		private static bool IsMyArmyStretched(IReadOnlyCollection<VehicleWrapper> myVehicles)
		{
			var minX = myVehicles.Min(v => v.X);
			var maxX = myVehicles.Max(v => v.X);
			var minY = myVehicles.Min(v => v.Y);
			var maxY = myVehicles.Max(v => v.Y);
			const int threshold = 150;
			return maxX - minX > threshold || maxY - minY > threshold;
		}
	}
}