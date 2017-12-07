using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class VehicleRegistry
	{
		private const int CellSize = 32;
		private readonly Dictionary<long, Vehicle> vehiclesByIds = new Dictionary<long, Vehicle>();
		private readonly HashSet<long> idleVehicleIds = new HashSet<long>();
		private readonly HashSet<long> newCreatedVehicles = new HashSet<long>();
		private readonly Dictionary<int, List<long>> vehicleIdsByFormationId = new Dictionary<int, List<long>>();

		public void Update(World world, Player me, Game game)
		{
			foreach (var vehicle in world.NewVehicles)
			{
				vehiclesByIds.Add(vehicle.Id, vehicle);
				newCreatedVehicles.Add(vehicle.Id);
			}
			var updates = world.VehicleUpdates;
			foreach (var update in updates)
			{
				var vehicle = vehiclesByIds[update.Id];
				vehiclesByIds.Remove(vehicle.Id);
				if (update.Durability == 0)
				{
					continue;
				}
				if (Math.Abs(update.X - vehicle.X) < MagicConstants.Eps && Math.Abs(update.Y - vehicle.Y) < MagicConstants.Eps)
					idleVehicleIds.Add(vehicle.Id);
				else if (idleVehicleIds.Contains(vehicle.Id))
					idleVehicleIds.Remove(vehicle.Id);
				vehiclesByIds.Add(vehicle.Id, new Vehicle(vehicle, update));
			}
			foreach (var vehicle in vehiclesByIds.Where(v => updates.All(vu => vu.Id != v.Key)))
			{
				idleVehicleIds.Add(vehicle.Key);
			}
#if DEBUG
			foreach (var vehicle in vehiclesByIds.Values)
			{
				RewindClient.Instance.LivingUnit(vehicle.X,
					vehicle.Y,
					game.VehicleRadius,
					vehicle.Durability,
					game.ArrvDurability,
					vehicle.PlayerId == me.Id ? Side.Our : Side.Enemy,
					vehicle.Type);
				if (vehicle.IsSelected)
				{
					RewindClient.Instance.Rectangle(vehicle.X - game.VehicleRadius,
						vehicle.Y - game.VehicleRadius,
						vehicle.X + game.VehicleRadius,
						vehicle.Y + game.VehicleRadius,
						Color.Crimson);
				}
			}
			RewindClient.Instance.End();
#endif
		}

		public Vehicle GetVehicleById(long id) => vehiclesByIds.ContainsKey(id) ? vehiclesByIds[id] : null;

		public List<Vehicle> GetVehiclesByIds(IEnumerable<long> ids) => ids.Select(GetVehicleById).Where(v => v != null).ToList();

		public List<Vehicle> MyVehicles(Player me)
		{
			return vehiclesByIds.Values.Where(v => v.PlayerId == me.Id).ToList();
		}

		public List<long> MyVehicleIds(Player me) => vehiclesByIds.Where(p => p.Value.PlayerId == me.Id).Select(p => p.Key).ToList();

		public List<Vehicle> EnemyVehicles(Player me)
		{
			return vehiclesByIds.Values.Where(v => v.PlayerId != me.Id).ToList();
		}

		public List<Vehicle> SelectedVehicles(Player me)
		{
			return MyVehicles(me).Where(v => v.IsSelected).ToList();
		}

		public bool IsVehicleIdle(Vehicle vehicle) => idleVehicleIds.Contains(vehicle.Id);
		public bool IsVehicleIdle(long vehicleId) => idleVehicleIds.Contains(vehicleId);
		public void ForceUnidleVehicle(long vehicleId) => idleVehicleIds.Remove(vehicleId);

		public IList<long> FilterDeadVehicles(IEnumerable<long> vehicleIds)
		{
			return vehicleIds.Where(id => vehiclesByIds.ContainsKey(id)).ToList();
		}

		public double GetVision(long id, World world, Game game)
		{
			var vehicle = GetVehicleById(id);
			return vehicle.VisionRange * GetVisionCoefficient(vehicle, world, game);
		}

		public static IEnumerable<Facility> GetUncapturedFacilities(World world, Player me)
		{
			return world.Facilities.Where(f => f.OwnerPlayerId != me.Id).ToList();
		}

		public static IEnumerable<Facility> GetUnusedFacilities(World world, Player me)
		{
			return world.Facilities
				.Where(f => f.OwnerPlayerId == me.Id && f.Type == FacilityType.VehicleFactory && f.VehicleType == null)
				.ToList();
		}

		public IEnumerable<long> GetNewCreatedVehicleIds()
		{
			return newCreatedVehicles;
		}

		public void RemoveFromNewVehicles(IEnumerable<long> ids)
		{
			foreach (var id in ids)
			{
				if (newCreatedVehicles.Contains(id))
					newCreatedVehicles.Remove(id);
			}
		}

		public void RegisterNewFormation(int formationId, List<long> vehicleIds)
		{
			vehicleIdsByFormationId.Add(formationId, vehicleIds);
		}

		public IEnumerable<long> GetVehicleIdsByFormationId(int formationId)
		{
			return vehicleIdsByFormationId.Where(p => p.Key == formationId).SelectMany(p => p.Value);
		}

		private double GetVisionCoefficient(Vehicle vehicle, World world, Game game)
		{
			var x = (int) vehicle.X / CellSize;
			var y = (int) vehicle.Y / CellSize;
			var weatherType = world.WeatherByCellXY[x][y];
			var terrainType = world.TerrainByCellXY[x][y];
			switch (vehicle.Type)
			{
				case VehicleType.Arrv:
					return GetVisionCoefficient(terrainType, game);
				case VehicleType.Fighter:
					return GetVisionCoefficient(weatherType, game);
				case VehicleType.Helicopter:
					return GetVisionCoefficient(weatherType, game);
				case VehicleType.Ifv:
					return GetVisionCoefficient(terrainType, game);
				case VehicleType.Tank:
					return GetVisionCoefficient(terrainType, game);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private double GetVisionCoefficient(WeatherType weatherType, Game game)
		{
			switch (weatherType)
			{
				case WeatherType.Clear:
					return game.ClearWeatherVisionFactor;
				case WeatherType.Cloud:
					return game.CloudWeatherVisionFactor;
				case WeatherType.Rain:
					return game.RainWeatherVisionFactor;
				default:
					throw new ArgumentOutOfRangeException(nameof(weatherType), weatherType, null);
			}
		}

		private double GetVisionCoefficient(TerrainType terrainType, Game game)
		{
			switch (terrainType)
			{
				case TerrainType.Plain:
					return game.PlainTerrainVisionFactor;
				case TerrainType.Swamp:
					return game.SwampTerrainVisionFactor;
				case TerrainType.Forest:
					return game.ForestTerrainVisionFactor;
				default:
					throw new ArgumentOutOfRangeException(nameof(terrainType), terrainType, null);
			}
		}
	}
}