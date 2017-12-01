using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class GappedVehicleFormation : VehicleFormationBase
	{
		private MoveCommand command;

		public GappedVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			if (command == null)
			{
				DoWork(me);
			}

			if (command != null && command.IsStarted() && command.IsFinished(world.TickIndex, VehicleRegistry))
			{
				return new VehicleFormationResult(new InitialGroundVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}
			return new VehicleFormationResult(this);
		}

		private void DoWork(Player me)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);
			var tanks = myVehicles.Where(v => v.Type == VehicleType.Tank).ToList();
			var centerOfTanks = tanks.GetCenterPoint();
#if DEBUG
			RewindClient.Instance.Message($"{nameof(centerOfTanks)} = {centerOfTanks}; ");
#endif

			var arrvs = myVehicles.Where(v => v.Type == VehicleType.Arrv).ToList();
			var centerOfArrvs = arrvs.GetCenterPoint();

#if DEBUG
			RewindClient.Instance.Message($"{nameof(centerOfArrvs)} = {centerOfArrvs}; ");
#endif

			var ifvs = myVehicles.Where(v => v.Type == VehicleType.Ifv).ToList();
			var centerOfIfvs = ifvs.GetCenterPoint();

#if DEBUG
			RewindClient.Instance.Message($"{nameof(centerOfIfvs)} = {centerOfIfvs}; ");
#endif

			var xTanksArrvs = Math.Abs(centerOfTanks.X - centerOfArrvs.X) < MagicConstants.Eps;
			var xArrvsIfvs = Math.Abs(centerOfArrvs.X - centerOfIfvs.X) < MagicConstants.Eps;
			var xTanksIfvs = Math.Abs(centerOfTanks.X - centerOfIfvs.X) < MagicConstants.Eps;
			var yTanksArrvs = Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) < MagicConstants.Eps;
			var yArrvsIfvs = Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) < MagicConstants.Eps;
			var yTanksIfvs = Math.Abs(centerOfTanks.Y - centerOfIfvs.Y) < MagicConstants.Eps;

#if DEBUG
			RewindClient.Instance.Message($"{nameof(xTanksArrvs)} = {xTanksArrvs}; ");
			RewindClient.Instance.Message($"{nameof(xArrvsIfvs)} = {xArrvsIfvs}; ");
			RewindClient.Instance.Message($"{nameof(xTanksIfvs)} = {xTanksIfvs}; ");
			RewindClient.Instance.Message($"{nameof(yTanksArrvs)} = {yTanksArrvs}; ");
			RewindClient.Instance.Message($"{nameof(yArrvsIfvs)} = {yArrvsIfvs}; ");
			RewindClient.Instance.Message($"{nameof(yTanksIfvs)} = {yTanksIfvs}; ");
#endif

			var xTanksArrvsGap = yTanksArrvs && Math.Abs(centerOfTanks.X - centerOfArrvs.X) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var xArrvsIfvsGap = yArrvsIfvs && Math.Abs(centerOfArrvs.X - centerOfIfvs.X) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var xTanksIfvsGap = yTanksIfvs && Math.Abs(centerOfTanks.X - centerOfIfvs.X) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var yTanksArrvsGap = xTanksArrvs && Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var yArrvsIfvsGap = xArrvsIfvs && Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var yTanksIfvsGap = xTanksIfvs && Math.Abs(centerOfTanks.Y - centerOfIfvs.Y) > MagicConstants.InitialGapSize + MagicConstants.Eps;

#if DEBUG
			RewindClient.Instance.Message($"{nameof(xTanksArrvsGap)} = {xTanksArrvsGap}; ");
			RewindClient.Instance.Message($"{nameof(xArrvsIfvsGap)} = {xArrvsIfvsGap}; ");
			RewindClient.Instance.Message($"{nameof(xTanksIfvsGap)} = {xTanksIfvsGap}; ");
			RewindClient.Instance.Message($"{nameof(yTanksArrvsGap)} = {yTanksArrvsGap}; ");
			RewindClient.Instance.Message($"{nameof(yArrvsIfvsGap)} = {yArrvsIfvsGap}; ");
			RewindClient.Instance.Message($"{nameof(yTanksIfvsGap)} = {yTanksIfvsGap}; ");
#endif

			var arrvsGroup = new VehiclesGroup(Id, arrvs.Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);
			var ifvsGroup = new VehiclesGroup(Id, ifvs.Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);
			if (xTanksArrvsGap)
			{
				MoveCloserX(arrvsGroup, centerOfTanks.X);
			}
			else if (xArrvsIfvsGap)
			{
				MoveCloserX(arrvsGroup, centerOfIfvs.X);
			}
			else if (xTanksIfvsGap)
			{
				MoveCloserX(ifvsGroup, centerOfTanks.X);
			}
			else if (yTanksArrvsGap)
			{
				MoveCloserY(arrvsGroup, centerOfTanks.Y);
			}
			else if (yArrvsIfvsGap)
			{
				MoveCloserY(arrvsGroup, centerOfIfvs.Y);
			}
			else
			{
				MoveCloserY(ifvsGroup, centerOfTanks.Y);
			}
			command = CommandManager.PeekLastCommand(Id) as MoveCommand;
		}

		private static void MoveCloserX(VehiclesGroup group, double secondGroupX)
		{
			group
				.SelectVehicles()
				.MoveTo(group.Center.X > secondGroupX
					? new Point2D(secondGroupX + MagicConstants.InitialGapSize, group.Center.Y)
					: new Point2D(group.Center.X + MagicConstants.InitialGapSize, group.Center.Y));
		}

		private static void MoveCloserY(VehiclesGroup group, double secondGroupY)
		{
			group
				.SelectVehicles()
				.MoveTo(group.Center.Y > secondGroupY
					? new Point2D(group.Center.X, secondGroupY + MagicConstants.InitialGapSize)
					: new Point2D(group.Center.X, group.Center.Y + MagicConstants.InitialGapSize));
		}
	}
}