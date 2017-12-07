using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class NewVehiclesFormation : VehicleFormationBase
	{
		private readonly List<Command> commands = new List<Command>();
		private bool binded;
		public NewVehiclesFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			var army = new VehiclesGroup(Id, VehicleIds, VehicleRegistry, CommandManager);
			if (!binded)
			{
				Bind(army);
				return new VehicleFormationResult(this);
			}

			commands.RemoveAll(c => c.IsStarted() && c.IsFinished(world.TickIndex, VehicleRegistry));


			if (FormationHelper.IsNukeAlert(world.GetOpponentPlayer()))
			{
				commands.Clear();
				CommandManager.ClearCommandsQueue(Id);
				this.PreventNuke(army, world, game, commands);
				return new VehicleFormationResult(this);
			}

			if (commands.Any())
				return new VehicleFormationResult(this);

			var nextTarget = NextTarget(army, world, me, game);
			var direction = army.Center.To(nextTarget);
			army
				.Select(Id)
				.MoveByVector(direction.Length() > 20
						? direction.Mul(0.1)
						: direction,
					game.TankSpeed * game.ForestTerrainSpeedFactor);
			commands.Add(CommandManager.PeekLastCommand(Id));
			return new VehicleFormationResult(this);
		}

		private Point2D NextTarget(VehiclesGroup army, World world, Player me, Game game)
		{
			var closestUncapturedFacility = VehicleRegistry.GetUncapturedFacilities(world, me)
				.OrderBy(f => army.Center.GetDistanceTo(f.ToPoint(game)))
				.FirstOrDefault();
			var myGroudForcesCenter = VehicleRegistry.GetVehiclesByIds(
					VehicleRegistry.GetVehicleIdsByFormationId(MagicConstants.GroundFormationGroupId))
				.ToList()
				.GetCenterPoint();
			return closestUncapturedFacility?.ToPoint(game) ?? myGroudForcesCenter;
		}

		private void Bind(VehiclesGroup myArmy)
		{
			myArmy
				.SelectVehicles()
				.Assign(Id);
			VehicleRegistry.RegisterNewFormation(Id, VehicleIds);
			binded = true;
		}
	}
}