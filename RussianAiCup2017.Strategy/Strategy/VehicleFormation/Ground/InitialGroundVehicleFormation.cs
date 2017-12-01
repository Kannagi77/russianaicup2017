using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation.Ground
{
	public class InitialGroundVehicleFormation : VehicleFormationBase
	{
		public InitialGroundVehicleFormation(int id,
			IEnumerable<long> vehicleIds,
			CommandManager commandManager,
			VehicleRegistry vehicleRegistry)
			: base(id, vehicleIds, commandManager, vehicleRegistry)
		{
		}

		public override VehicleFormationResult PerformAction(World world, Player me, Game game)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);

			var tanks = myVehicles.Where(v => v.Type == VehicleType.Tank).ToList();
			var centerOfTanks = tanks.GetCenterPoint();

			var arrvs = myVehicles.Where(v => v.Type == VehicleType.Arrv).ToList();
			var centerOfArrvs = arrvs.GetCenterPoint();

			var ifvs = myVehicles.Where(v => v.Type == VehicleType.Ifv).ToList();
			var centerOfIfvs = ifvs.GetCenterPoint();

			var xTanksArrvs = Math.Abs(centerOfTanks.X - centerOfArrvs.X) < MagicConstants.Eps;
			var xArrvsIfvs = Math.Abs(centerOfArrvs.X - centerOfIfvs.X) < MagicConstants.Eps;
			var xTanksIfvs = Math.Abs(centerOfTanks.X - centerOfIfvs.X) < MagicConstants.Eps;
			var yTanksArrvs = Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) < MagicConstants.Eps;
			var yArrvsIfvs = Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) < MagicConstants.Eps;
			var yTanksIfvs = Math.Abs(centerOfTanks.Y - centerOfIfvs.Y) < MagicConstants.Eps;

			var xTanksArrvsGap = yTanksArrvs && Math.Abs(centerOfTanks.X - centerOfArrvs.X) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var xArrvsIfvsGap = yArrvsIfvs && Math.Abs(centerOfArrvs.X - centerOfIfvs.X) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var xTanksIfvsGap = yTanksIfvs && Math.Abs(centerOfTanks.X - centerOfIfvs.X) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var yTanksArrvsGap = xTanksArrvs && Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var yArrvsIfvsGap = xArrvsIfvs && Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) > MagicConstants.InitialGapSize + MagicConstants.Eps;
			var yTanksIfvsGap = xTanksIfvs && Math.Abs(centerOfTanks.Y - centerOfIfvs.Y) > MagicConstants.InitialGapSize + MagicConstants.Eps;

			if (xTanksArrvs && xArrvsIfvs || yTanksArrvs && yArrvsIfvs)
			{
				return new VehicleFormationResult(new FinishGroundVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			} 

			if (xTanksArrvs && !yTanksArrvsGap && !yTanksIfvs && !yArrvsIfvs
				|| xArrvsIfvs && !yArrvsIfvsGap && !yTanksArrvs && !yTanksIfvs
				|| xTanksIfvs && !yTanksIfvsGap && !yArrvsIfvs && !yTanksArrvs
				|| yTanksArrvs && !xTanksArrvsGap && !xTanksIfvs && !xArrvsIfvs
				|| yArrvsIfvs && !xArrvsIfvsGap && !xTanksArrvs && !xTanksIfvs
				|| yTanksIfvs && !xTanksIfvsGap && !xTanksArrvs && !xArrvsIfvs)
			{
				return new VehicleFormationResult(new TwoOnOneLineVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}

			if (xTanksArrvs && !yTanksArrvsGap && (yTanksIfvs && !xTanksIfvsGap || yArrvsIfvs && !xArrvsIfvsGap)
				|| xArrvsIfvs && !yArrvsIfvsGap && (yTanksArrvs && !xTanksArrvsGap || yTanksIfvs && !xTanksIfvsGap)
				|| xTanksIfvs && !yTanksIfvsGap && (yTanksArrvs && !xTanksArrvsGap || yArrvsIfvs && !xArrvsIfvsGap))
			{
				return new VehicleFormationResult(new CornerVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}

			if (xTanksArrvsGap || xTanksIfvsGap || xArrvsIfvsGap
				|| yTanksArrvsGap || yTanksIfvsGap || yArrvsIfvsGap)
			{
				return new VehicleFormationResult(new GappedVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
			}
			return new VehicleFormationResult(new DiagonalVehicleFormation(Id, VehicleIds, CommandManager, VehicleRegistry));
		}
	}
}