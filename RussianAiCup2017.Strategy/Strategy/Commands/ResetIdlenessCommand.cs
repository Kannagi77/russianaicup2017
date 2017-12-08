using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class ResetIdlenessCommand : Command
	{
		public override int FormationId { get; }
		private const ActionType ActionType = Model.ActionType.Move;
		private bool isStarted;
		private IList<long> vehicleIds;

		public ResetIdlenessCommand(int formationId, IList<long> vehicleIds)
		{
			FormationId = formationId;
			this.vehicleIds = vehicleIds;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.X = 1;
			move.Y = 0;

			isStarted = true;
			foreach (var vehicleId in vehicleIds)
			{
				registry.ForceUnidleVehicle(vehicleId);
			}
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(int worldTick, VehicleRegistry registry)
		{
			vehicleIds = registry.FilterDeadVehicles(vehicleIds);
			return vehicleIds.All(registry.IsVehicleIdle);
		}

		public override bool CanBeParallel()
		{
			return false;
		}
	}
}