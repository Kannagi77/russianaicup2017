using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers
{
	public static class FormationHelper
	{
		public static VehicleType GetUpperOrLeftGroupType(double tanksCoord, double arrvsCoord, double ifvsCoord)
		{
			var upperCoord = new List<double> { tanksCoord, arrvsCoord, ifvsCoord }.OrderBy(y => y).First();
			return Math.Abs(tanksCoord - upperCoord) < MagicConstants.Eps
				? VehicleType.Tank
				: Math.Abs(arrvsCoord - upperCoord) < MagicConstants.Eps
					? VehicleType.Arrv
					: VehicleType.Ifv;
		}

		public static VehicleType GetBottomOrRightGroupType(double tanksCoord, double arrvsCoord, double ifvsCoord)
		{
			var bottomCoord = new List<double> { tanksCoord, arrvsCoord, ifvsCoord }.OrderBy(y => y).Last();
			return Math.Abs(tanksCoord - bottomCoord) < MagicConstants.Eps
				? VehicleType.Tank
				: Math.Abs(arrvsCoord - bottomCoord) < MagicConstants.Eps
					? VehicleType.Arrv
					: VehicleType.Ifv;
		}

		public static void PreventNuke(this IVehicleFormation formation, VehiclesGroup myArmy, World world, Game game, IList<Command> commands)
		{
			var opponentPlayer = world.GetOpponentPlayer();
			myArmy
				.Select(formation.Id)
				.Stop()
				.SelectArea(
					opponentPlayer.NextNuclearStrikeX - game.TacticalNuclearStrikeRadius,
					opponentPlayer.NextNuclearStrikeY - game.TacticalNuclearStrikeRadius,
					opponentPlayer.NextNuclearStrikeX + game.TacticalNuclearStrikeRadius,
					opponentPlayer.NextNuclearStrikeY + game.TacticalNuclearStrikeRadius,
					true
				)
				.Scale(
					opponentPlayer.NextNuclearStrikeX,
					opponentPlayer.NextNuclearStrikeY,
					10.0,
					tick => tick - world.TickIndex > game.TacticalNuclearStrikeDelay)
				.Scale(opponentPlayer.NextNuclearStrikeX,
					opponentPlayer.NextNuclearStrikeY,
					0.1);
			commands.Add(formation.CommandManager.PeekLastCommand(formation.Id));
		}

		public static bool IsNukeAlert(Player opponentPlayer)
		{
			return opponentPlayer.NextNuclearStrikeX > 0 && opponentPlayer.NextNuclearStrikeY > 0;
		}

		public static bool TryNuke(this IVehicleFormation formation,
			VehiclesGroup myArmy,
			IReadOnlyCollection<Vehicle> myVehicles,
			IReadOnlyList<Vehicle> enemyVehicles,
			Player me,
			Game game,
			World world,
			IList<Command> commands)
		{
			if (me.RemainingNuclearStrikeCooldownTicks != 0)
			{
				return false;
			}
			if (!enemyVehicles.Any())
			{
				return false;
			}
			if (myArmy.Center.GetDistanceTo(enemyVehicles.GetCenterPoint()) > 4 * game.TacticalNuclearStrikeRadius)
			{
				return false;
			}
			var nukeTarget = GetNukeTarget(enemyVehicles, myVehicles, game);
			if (nukeTarget == null)
			{
				return false;
			}
			var nukeGunner = formation.GetNukeGunner(nukeTarget, myVehicles, world, game);
			if (nukeGunner == null)
			{
				return false;
			}
			myArmy
				.Nuke(nukeGunner.Id, nukeTarget);
			commands.Add(formation.CommandManager.PeekLastCommand(formation.Id));
			myArmy
				.Select(formation.Id)
				.Stop();
			return true;
		}

		private static Vehicle GetNukeTarget(IReadOnlyList<Vehicle> enemyVehicles, IReadOnlyCollection<Vehicle> myVehicles, Game game)
		{
			var bestTarget = enemyVehicles.First();
			var topDamage = 0.0;
			for (var i = 1; i < enemyVehicles.Count; i++)
			{
				var currentTarget = enemyVehicles[i];
				var currentDamage = 0.0;
				foreach (var enemyVehicle in enemyVehicles
					.Where(v => v.Id != currentTarget.Id))
				{
					currentDamage += GetDamage(currentTarget, enemyVehicle, game);
				}
				foreach (var myVehicle in myVehicles)
				{
					currentDamage -= GetDamage(currentTarget, myVehicle, game);
				}

				if (currentDamage > topDamage)
				{
					bestTarget = currentTarget;
					topDamage = currentDamage;
				}
			}
			return topDamage > 0
				? bestTarget
				: null;
		}

		private static double GetDamage(Unit currentTarget, Unit vehicle, Game game)
		{
			var distance = currentTarget.GetDistanceTo(vehicle);
			var nukeRadius = game.TacticalNuclearStrikeRadius;
			if (distance >= nukeRadius)
				return 0;
			return (nukeRadius - distance) / game.MaxTacticalNuclearStrikeDamage;
		}

		private static Vehicle GetNukeGunner(this IVehicleFormation formation, Unit target, IEnumerable<Vehicle> myVehicles, World world, Game game)
		{
			return myVehicles
				.Where(v => formation.VehicleRegistry.GetVision(v.Id, world, game) >= v.GetDistanceTo(target))
				.OrderByDescending(v => v.Durability)
				.FirstOrDefault();
		}
	}
}