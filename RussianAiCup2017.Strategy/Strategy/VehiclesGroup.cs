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
		public List<long> VehicleIds { get; private set; }
		public Point2D Center => registry.GetVehiclesByIds(VehicleIds).GetCenterPoint();
		public VehiclesGroup(List<long> vehicleIds, VehicleRegistry registry, CommandManager commandManager)
		{
			this.registry = registry;
			this.commandManager = commandManager;
			VehicleIds = vehicleIds;
		}

		public VehiclesGroup Select(World world, VehicleType? type = null)
		{
			commandManager.EnqueueCommand(new SelectVehiclesCommand(VehicleIds, type), world.TickIndex);
			return this;
		}

		public VehiclesGroup AddToSelection(List<Vehicle> vehicles, World world)
		{
			commandManager.EnqueueCommand(new AddToSelectionCommand(
				vehicles.Select(v => v.X).Min(),
				vehicles.Select(v => v.Y).Min(),
				vehicles.Select(v => v.X).Max(),
				vehicles.Select(v => v.Y).Max()),
				world.TickIndex);
			return this;
		}

		public VehiclesGroup MoveTo(Point2D destination, World world, double maxSpeed = 0, bool canBeParallel = false)
		{
			var direction = Center.To(destination);
			commandManager.EnqueueCommand(new MoveCommand(VehicleIds, direction.X, direction.Y, maxSpeed, canBeParallel), world.TickIndex);
			return this;
		}

		public VehiclesGroup MoveByVector(Vector2D direction, World world = null, double maxSpeed = 0, bool canBeParallel = false)
		{
			return MoveByVector(direction.X, direction.Y, world, maxSpeed, canBeParallel);
		}

		public VehiclesGroup MoveByVector(double x, double y, World world = null, double maxSpeed = 0, bool canBeParallel = false)
		{
			var tickIndex = world?.TickIndex ?? int.MaxValue;
			commandManager.EnqueueCommand(new MoveCommand(VehicleIds, x, y, maxSpeed, canBeParallel), tickIndex);
			return this;
		}

		public VehiclesGroup MergeWith(List<long> anotherVehicleIds)
		{
			return new VehiclesGroup(VehicleIds.Concat(anotherVehicleIds).ToList(), registry, commandManager);
		}

		public VehiclesGroup RotateTo(Point2D rotationDirection, World world)
		{
			//todo
			return RotateBy(Double.Epsilon, world);
		}

		public VehiclesGroup RotateBy(double angle, World world)
		{
			commandManager.EnqueueCommand(new RotateCommand(VehicleIds, Center, angle), world.TickIndex);
			return this;
		}
	}
}