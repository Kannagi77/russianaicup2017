﻿using System;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

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
			var currentCenterPoint = VehicleRegistry.MyVehicles(me).GetCenterPoint();
			if (!started && CommandManager.GetCurrentQueueSize() < 10)
			{
				RotateVehicles(world, game, currentCenterPoint, VehicleType.Tank);
				RotateVehicles(world, game, currentCenterPoint, VehicleType.Fighter);
				RotateVehicles(world, game, currentCenterPoint, VehicleType.Arrv);
				RotateVehicles(world, game, currentCenterPoint, VehicleType.Helicopter);
				RotateVehicles(world, game, currentCenterPoint, VehicleType.Ifv);
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

		private void RotateVehicles(World world, Game game, Point2D currentCenterPoint, VehicleType type)
		{
			CommandManager.EnqueueCommand(new SelectCommand(0, 0, world.Width, world.Height, type));
			CommandManager.EnqueueCommand(new RotateCommand(currentCenterPoint, GetRotationAngle(game)));
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