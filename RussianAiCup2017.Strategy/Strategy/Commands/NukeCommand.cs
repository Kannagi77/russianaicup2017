using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class NukeCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.TacticalNuclearStrike;
		private bool isStarted;
		private readonly long vehicleId;
		private readonly double x;
		private readonly double y;

		public NukeCommand(long vehicleId, double x, double y)
		{
			this.vehicleId = vehicleId;
			this.x = x;
			this.y = y;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.VehicleId = vehicleId;
			move.X = x;
			move.Y = y;

			isStarted = true;
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(VehicleRegistry registry)
		{
			return true;
		}

		public override bool CanBeParallel()
		{
			return true;
		}
	}
}