using System.Collections.Generic;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class StopCommand : MoveCommand
	{
		public StopCommand(int formationId, IList<long> vehicleIds)
			: base(formationId, vehicleIds, 0, 0)
		{
		}
	}
}