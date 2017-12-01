using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class SelectAllCommand : SelectCommand
	{
		public SelectAllCommand(World world)
			: base(0, 0, world.Width, world.Height)
		{
		}

		public SelectAllCommand(World world, bool forcePlayNextCommand)
			: base(0, 0, world.Width, world.Height, forcePlayNextCommand)
		{
		}

		public SelectAllCommand(World world, VehicleType? type, bool forcePlayNextCommand = false)
			: base(0, 0, world.Width, world.Height, type, forcePlayNextCommand)
		{
		}
	}
}