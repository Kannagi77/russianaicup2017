using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class SelectAllCommand : SelectCommand
	{
		public SelectAllCommand(int formationId, World world)
			: base(formationId, 0, 0, world.Width, world.Height)
		{
		}

		public SelectAllCommand(int formationId, World world, bool forcePlayNextCommand)
			: base(formationId, 0, 0, world.Width, world.Height, forcePlayNextCommand)
		{
		}

		public SelectAllCommand(int formationId, World world, VehicleType? type, bool forcePlayNextCommand = false)
			: base(formationId, 0, 0, world.Width, world.Height, type, forcePlayNextCommand)
		{
		}
	}
}