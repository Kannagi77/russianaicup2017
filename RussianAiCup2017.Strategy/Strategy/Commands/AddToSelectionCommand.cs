using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class AddToSelectionCommand : Command
	{
		public override int FormationId { get; }
		private const ActionType ActionType = Model.ActionType.AddToSelection;
		private readonly double x1;
		private readonly double y1;
		private readonly double x2;
		private readonly double y2;
		private readonly VehicleType? type;
		private readonly bool forcePlayNextCommand;
		private bool isStarted;

		public AddToSelectionCommand(int formationId, double x1, double y1, double x2, double y2)
			: this(formationId, x1, y1, x2, y2, null)
		{
		}

		public AddToSelectionCommand(int formationId,
			double x1,
			double y1,
			double x2,
			double y2,
			VehicleType? type,
			bool forcePlayNextCommand = false)
		{
			FormationId = formationId;
			this.forcePlayNextCommand = forcePlayNextCommand;
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

		public override bool ForcePlayNextCommand => forcePlayNextCommand;

#if DEBUG
		public override string ToString()
		{
			return $"{ActionType}: " +
			       $"{nameof(x1)}={x1}, " +
			       $"{nameof(y1)}={y1}, " +
				   $"{nameof(x2)}={x2}, "+
				   $"{nameof(y2)}={y2}, "+
				   $"{nameof(type)}={type}";
		}
#endif
	}
}