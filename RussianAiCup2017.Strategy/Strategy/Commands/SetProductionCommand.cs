using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class SetProductionCommand : Command
	{
		public override int FormationId { get; }
		private const ActionType ActionType = Model.ActionType.SetupVehicleProduction;
		private readonly long facilityId;
		private readonly VehicleType vehicleType;
		private bool isStarted;

		public SetProductionCommand(long facilityId, VehicleType vehicleType)
		{
			this.facilityId = facilityId;
			this.vehicleType = vehicleType;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.FacilityId = facilityId;
			move.VehicleType = vehicleType;

			isStarted = true;
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(int worldTick, VehicleRegistry registry)
		{
			return true;
		}

		public override bool CanBeParallel()
		{
			return false;
		}
	}
}