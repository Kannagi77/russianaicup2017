using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

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

		public override StrategyState Perform(World world, Player me)
		{
			if (!started)
			{
				CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height));
				CommandManager.EnqueueCommand(new MoveCommand(world.Width, world.Height, 0.2));
				started = true;
			}
			var myArmyCenter = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			var enemyVehicles = VehicleRegistry.EnemyVehicles(me);
			var minimumDistanceToEnemy = enemyVehicles.GetMinimumDistanceTo(myArmyCenter);
			if (minimumDistanceToEnemy > 100)
			{
				return StrategyState.Attack;
			}
			started = false;
			return StrategyState.Gather;
		}
	}
}