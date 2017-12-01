using System.Collections.Generic;
using System.Linq;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation
{
	public class VehicleFormationResult
	{
		public List<IVehicleFormation> NewFormations { get; private set; }

		public VehicleFormationResult(params IVehicleFormation[] newFormations)
		{
			NewFormations = newFormations.ToList();
		}
	}
}