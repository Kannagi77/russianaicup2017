using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

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
	}
}