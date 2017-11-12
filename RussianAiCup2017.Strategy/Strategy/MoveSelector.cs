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
				new InitialPointMove(commandManager, vehicleRegistry), 
				new ScaleMove(commandManager, vehicleRegistry),
				new GatherMove(commandManager, vehicleRegistry),
				new ShrinkMove(commandManager, vehicleRegistry), 
				new AttackMove(commandManager, vehicleRegistry)
			};
		}

		public StrategyState MakeNextMove(StrategyState currentState, World world, Player player)
		{
			return moves.First(m => m.State == currentState).Perform(world, player);
		}
	}
}