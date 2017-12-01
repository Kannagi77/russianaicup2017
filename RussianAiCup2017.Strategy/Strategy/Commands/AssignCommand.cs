using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class AssignCommand : Command
	{
		public override int FormationId { get; }
		private const ActionType ActionType = Model.ActionType.Assign;
		private bool isStarted;
		private readonly int groupId;

		public AssignCommand(int formationId, int groupId)
		{
			FormationId = formationId;
			this.groupId = groupId;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.Group = groupId;

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