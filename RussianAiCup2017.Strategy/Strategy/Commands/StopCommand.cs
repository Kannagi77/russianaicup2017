using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class StopCommand : MoveCommand
	{
		public StopCommand(IList<long> vehicleIds)
			: base(vehicleIds, 0, 0)
		{
		}
	}
}