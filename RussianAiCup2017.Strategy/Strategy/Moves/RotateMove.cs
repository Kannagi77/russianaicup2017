using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Wrappers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class RotateMove : StrategyMove
	{
		public override StrategyState State => StrategyState.Rotate;
		private Point2D previousCenterPoint;
		private bool started;

		public RotateMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);
			var currentCenterPoint = myVehicles.GetCenterPoint();
			if (!started && CommandManager.GetCurrentQueueSize() < 10)
			{
				RotateVehicles(myVehicles, world, game, currentCenterPoint, VehicleType.Tank);
				RotateVehicles(myVehicles, world, game, currentCenterPoint, VehicleType.Fighter);
				RotateVehicles(myVehicles, world, game, currentCenterPoint, VehicleType.Arrv);
				RotateVehicles(myVehicles, world, game, currentCenterPoint, VehicleType.Helicopter);
				RotateVehicles(myVehicles, world, game, currentCenterPoint, VehicleType.Ifv);
				started = true;
			}
			StrategyState result;
			if (currentCenterPoint != previousCenterPoint)
			{
				result = StrategyState.Rotate;
			}
			else
			{
				result = StrategyState.Attack;
				started = false;
			}
			previousCenterPoint = currentCenterPoint;
			return result;
		}

		private void RotateVehicles(IEnumerable<VehicleWrapper> vehicles, World world, Game game, Point2D currentCenterPoint, VehicleType type)
		{
			var selectedVehicles = vehicles.Where(v => v.Type == type).ToList();
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, type), world.TickIndex);
			CommandManager.EnqueueCommand(new RotateCommand(selectedVehicles, currentCenterPoint, GetRotationAngle(game), true), world.TickIndex);
		}

		private static double GetRotationAngle(Game game)
		{
			unchecked
			{
				var randomSeed = (int) game.RandomSeed;
				var random = new Random(randomSeed);
				var angle = random.Next(10) - 5;
				var radians = angle.ToRadians();
				return Math.PI / 10 + radians;
			}
		}
	}
}