using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class VehiclesGroup
	{
		private readonly VehicleRegistry registry;
		private readonly CommandManager commandManager;
		public int FormationId { get; }
		public List<long> VehicleIds { get; }
		public Point2D Center => registry.GetVehiclesByIds(VehicleIds).GetCenterPoint();
		public VehiclesGroup(int formationId, List<long> vehicleIds, VehicleRegistry registry, CommandManager commandManager)
		{
			this.registry = registry;
			this.commandManager = commandManager;
			FormationId = formationId;
			VehicleIds = vehicleIds;
		}

		public VehiclesGroup SelectVehicles(VehicleType? type = null)
		{
			commandManager.EnqueueCommand(new SelectVehiclesCommand(FormationId, VehicleIds, type));
			return this;
		}

		public VehiclesGroup AddToSelectionVehicles(VehicleType? type = null)
		{
			commandManager.EnqueueCommand(new AddVehiclesToSelectionCommand(FormationId, VehicleIds, type));
			return this;
		}

		public VehiclesGroup Select(int groupId)
		{
			commandManager.EnqueueCommand(new SelectGroupCommand(FormationId, groupId));
			return this;
		}

		public VehiclesGroup SelectArea(double x1, double y1, double x2, double y2, bool forcePlayNextCommand = false)
		{
			commandManager.EnqueueCommand(new SelectCommand(FormationId, x1, y1, x2, y2, forcePlayNextCommand));
			return this;
		}

		public VehiclesGroup AddToSelection(List<Vehicle> vehicles)
		{
			commandManager.EnqueueCommand(new AddToSelectionCommand(
				FormationId,
				vehicles.Select(v => v.X).Min(),
				vehicles.Select(v => v.Y).Min(),
				vehicles.Select(v => v.X).Max(),
				vehicles.Select(v => v.Y).Max()));
			return this;
		}

		public VehiclesGroup MoveTo(Point2D destination, double maxSpeed = 0, bool canBeParallel = false)
		{
			var direction = Center.To(destination);
			commandManager.EnqueueCommand(new MoveCommand(FormationId, VehicleIds, direction.X, direction.Y, maxSpeed, canBeParallel));
			return this;
		}

		public VehiclesGroup MoveByVector(Vector2D direction, double maxSpeed = 0, bool canBeParallel = false)
		{
			return MoveByVector(direction.X, direction.Y, maxSpeed, canBeParallel);
		}

		public VehiclesGroup MoveByVector(double x, double y, double maxSpeed = 0, bool canBeParallel = false)
		{
			commandManager.EnqueueCommand(new MoveCommand(FormationId, VehicleIds, x, y, maxSpeed, canBeParallel));
			return this;
		}

		public VehiclesGroup ResetIdleness()
		{
			commandManager.EnqueueCommand(new ResetIdlenessCommand(FormationId, VehicleIds));
			return this;
		}

		public VehiclesGroup MergeWith(IEnumerable<long> anotherVehicleIds)
		{
			return new VehiclesGroup(FormationId, VehicleIds.Concat(anotherVehicleIds).ToList(), registry, commandManager);
		}

		public VehiclesGroup RotateBy(double angle)
		{
			commandManager.EnqueueCommand(new RotateCommand(FormationId, VehicleIds, Center, angle));
			return this;
		}

		public VehiclesGroup Nuke(long gunnerId, Vehicle target)
		{
			commandManager.EnqueueCommand(new NukeCommand(FormationId, gunnerId, target.X, target.Y));
			return this;
		}

		public VehiclesGroup Scale(double factor)
		{
			commandManager.EnqueueCommand(new ScaleCommand(FormationId, VehicleIds, Center, factor));
			return this;
		}

		public VehiclesGroup Scale(double x, double y, double factor, Func<int, bool> isFinished = null)
		{
			commandManager.EnqueueCommand(new ScaleCommand(FormationId, VehicleIds, x, y, factor, isFinished: isFinished));
			return this;
		}

		public VehiclesGroup Stop()
		{
			commandManager.EnqueueCommand(new StopCommand(FormationId, VehicleIds));
			return this;
		}

		public VehiclesGroup Assign(int groupId)
		{
			commandManager.EnqueueCommand(new AssignCommand(FormationId, groupId));
			return this;
		}
	}
}