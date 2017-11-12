using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class MoveCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Move;
		private readonly double x;
		private readonly double y;
		private readonly double maxSpeed;

		public MoveCommand(Point2D point)
			: this(point.X, point.Y)
		{
		}

		public MoveCommand(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public MoveCommand(double x, double y, double maxSpeed)
		{
			this.x = x;
			this.y = y;
			this.maxSpeed = maxSpeed;
		}

		public override void Commit(Move move)
		{
			move.Action = ActionType;
			move.X = x;
			move.Y = y;
			move.MaxSpeed = maxSpeed;
		}
	}
}