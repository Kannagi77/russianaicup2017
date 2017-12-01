using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.VehicleFormation
{
	public interface IVehicleFormation
	{
		int Id { get; }
		CommandManager CommandManager { get; }
		VehicleRegistry VehicleRegistry { get; }
		VehicleFormationResult PerformAction(World world, Player me, Game game);
	}
}