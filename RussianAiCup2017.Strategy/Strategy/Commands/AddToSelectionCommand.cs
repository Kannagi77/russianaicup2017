using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class AddToSelectionCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.AddToSelection;
		private readonly double x1;
		private readonly double y1;
		private readonly double x2;
		private readonly double y2;
		private readonly VehicleType? type;
		private bool isStarted;

		public AddToSelectionCommand(double x1, double y1, double x2, double y2)
			: this(x1, y1, x2, y2, null)
		{
		}

		public AddToSelectionCommand(double x1, double y1, double x2, double y2, VehicleType? type)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			this.type = type;
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

		public override void Commit(Move move, VehicleRegistry registry)
		{
			move.Action = ActionType;
			move.Left = x1;
			move.Top = y1;
			move.Right = x2;
			move.Bottom = y2;
			if (type.HasValue)
			{
				move.VehicleType = type.Value;
			}

			isStarted = true;
		}
	}
}