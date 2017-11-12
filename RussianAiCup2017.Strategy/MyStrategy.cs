using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public sealed class MyStrategy : IStrategy
	{
		private static readonly CommandManager CommandManager = new CommandManager();
		private static readonly VehicleRegistry VehicleRegistry = new VehicleRegistry();
		private readonly MoveSelector moveSelector = new MoveSelector(CommandManager, VehicleRegistry);
		private StrategyState currentState = StrategyState.InitialPoint;

		public void Move(Player me, World world, Game game, Move move)
		{
			VehicleRegistry.Update(world);
			if (CommandManager.PlayCommandIfPossible(me, move))
				return;
			currentState = moveSelector.MakeNextMove(currentState, world, me);
		}
	}
}