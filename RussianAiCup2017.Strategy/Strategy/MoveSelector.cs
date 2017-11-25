using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class MoveSelector
	{
		private readonly StrategyMove[] moves;

		public MoveSelector(CommandManager commandManager, VehicleRegistry vehicleRegistry)
		{
			moves = new StrategyMove[]
			{
				new InitFormationMove(commandManager, vehicleRegistry),
				new DiagonalFormationMove(commandManager, vehicleRegistry),
				new CornerFormationMove(commandManager, vehicleRegistry),
				new GappedFormationMove(commandManager, vehicleRegistry), 
				new TwoOnOneLineFormationMove(commandManager, vehicleRegistry),
				new FinishAirMove(commandManager, vehicleRegistry), 
				new FinishFormationMove(commandManager, vehicleRegistry),
				new ShrinkMove(commandManager, vehicleRegistry),
				new RotateMove(commandManager, vehicleRegistry),
				new AttackMove(commandManager, vehicleRegistry)
			};
		}

		public StrategyState MakeNextMove(StrategyState currentState, World world, Player player, Game game)
		{
			var strategyMove = moves.First(m => m.State == currentState);
#if DEBUG
			RewindClient.Instance.Message($"Move: {strategyMove.State}");
			RewindClient.Instance.End();
#endif
			return strategyMove.Perform(world, player, game);
		}
	}
}