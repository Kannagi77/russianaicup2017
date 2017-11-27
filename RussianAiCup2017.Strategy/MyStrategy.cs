using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public sealed class MyStrategy : IStrategy
	{
		private static readonly CommandManager CommandManager = new CommandManager();
		private static readonly VehicleRegistry VehicleRegistry = new VehicleRegistry();
		private readonly MoveSelector moveSelector = new MoveSelector(CommandManager, VehicleRegistry);
		private StrategyState currentState = StrategyState.InitFormation;
#if DEBUG
		private bool isGridDrawn;
#endif

		public void Move(Player me, World world, Game game, Move move)
		{
			VehicleRegistry.Update(world, me, game);
#if DEBUG
			RewindClient.Instance.Message($"Commands queue size: {CommandManager.GetCurrentQueueSize()}; ");
			if (!isGridDrawn)
			{
				DrawGrid(world);
				isGridDrawn = true;
			}
#endif
			if (CommandManager.PlayCommandIfPossible(VehicleRegistry, me, move, world.TickIndex))
				return;
			currentState = moveSelector.MakeNextMove(currentState, world, me, game);
		}

#if DEBUG
		private static void DrawGrid(World world)
		{
			var width = world.TerrainByCellXY.Length;
			var height = world.TerrainByCellXY[0].Length;
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var areaType = GetAreaType(world.TerrainByCellXY[x][y], world.WeatherByCellXY[x][y]);
					if (areaType != AreaType.Unknown)
						RewindClient.Instance.AreaDescription(x, y, areaType);
				}
			}
		}

		private static AreaType GetAreaType(TerrainType terrainType, WeatherType weatherType)
		{
			return terrainType == TerrainType.Forest
				? AreaType.Forest
				: terrainType == TerrainType.Swamp
					? AreaType.Swamp
					: weatherType == WeatherType.Cloud
						? AreaType.Cloud
						: weatherType == WeatherType.Rain
							? AreaType.Rain
							: AreaType.Unknown;
		}
#endif
	}
}