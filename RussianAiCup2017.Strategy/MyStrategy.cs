using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Air;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public sealed class MyStrategy : IStrategy
	{
		private static readonly CommandManager CommandManager = new CommandManager();
		private static readonly VehicleRegistry VehicleRegistry = new VehicleRegistry();
		private static List<IVehicleFormation> formations = new List<IVehicleFormation>();
#if DEBUG
		private bool isGridDrawn;
#endif

		public void Move(Player me, World world, Game game, Move move)
		{
			VehicleRegistry.Update(world, me, game);
			if (world.TickIndex == 0)
			{
				InitFormations(me);
			}
#if DEBUG
			RewindClient.Instance.Message($"Commands queue size: {CommandManager.GetCurrentQueueSize()}; ");
			RewindClient.Instance.Message($"My points: {world.GetMyPlayer().Score}; ");
			RewindClient.Instance.Message($"Opponent points: {world.GetOpponentPlayer().Score}; ");
			if (!isGridDrawn)
			{
				DrawGrid(world);
				isGridDrawn = true;
			}
#endif
			if (CommandManager.PlayCommandIfPossible(VehicleRegistry, me, move, world.TickIndex))
				return;
			var unusedFacilities = VehicleRegistry.GetUnusedFacilities(world, me).ToList();
			if (unusedFacilities.Any())
			{
				var facility = unusedFacilities.First();
				if (!AnyVehiclesNearFacility(facility, VehicleRegistry.MyVehicles(me), game))
				{
					CommandManager.EnqueueCommand(new SetProductionCommand(facility.Id, VehicleType.Tank));
					return;
				}
			}
			var nextTickFormations = new List<IVehicleFormation>();
			foreach (var formation in formations)
			{
				var result = formation.PerformAction(world, me, game);
				nextTickFormations.AddRange(result.NewFormations);
			}
			formations = nextTickFormations;
			TryIntroduceNewFormations();
		}

		private bool AnyVehiclesNearFacility(Facility facility, IEnumerable<Vehicle> vehicles, Game game)
		{
			return vehicles
				.Any(v => v.X > facility.Left - game.FacilityWidth / 2
			                           && v.X < facility.Left + game.FacilityWidth / 2
			                           && v.Y > facility.Top - game.FacilityHeight / 2
			                           && v.Y < facility.Top + game.FacilityHeight / 2);
		}

		private static void InitFormations(Player me)
		{
			formations.Add(new InitialGroundVehicleFormation(MagicConstants.GroundFormationGroupId,
				VehicleRegistry
					.MyVehicles(me)
					.Where(v => v.Type == VehicleType.Arrv || v.Type == VehicleType.Ifv || v.Type == VehicleType.Tank)
					.Select(v => v.Id)
					.ToList(),
				CommandManager, VehicleRegistry));
			formations.Add(new InitialAirVehicleFormation(MagicConstants.AirFormationGroupId,
				VehicleRegistry
					.MyVehicles(me)
					.Where(v => v.Type == VehicleType.Fighter || v.Type == VehicleType.Helicopter)
					.Select(v => v.Id)
					.ToList(),
				CommandManager, VehicleRegistry));
			VehicleRegistry.RemoveFromNewVehicles(VehicleRegistry.MyVehicleIds(me));
		}

		private static void TryIntroduceNewFormations()
		{
			var newVehicleIds = VehicleRegistry
				.GetNewCreatedVehicleIds()
				.ToList();
			if (!newVehicleIds.Any())
				return;
			var newVehicles = VehicleRegistry.GetVehiclesByIds(newVehicleIds);
			var clusters = Dbscan.Cluster(newVehicles, 80, 33);
			if (!clusters.Any())
				return;
			foreach (var cluster in clusters)
			{
				var ids = cluster.Select(v => v.Id).ToList();
				formations.Add(new NewVehiclesFormation(MagicConstants.NextFormationId++, ids, CommandManager, VehicleRegistry));
				VehicleRegistry.RemoveFromNewVehicles(ids);
			}
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