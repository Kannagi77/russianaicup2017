using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class ScaleCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Scale;
		private readonly double x;
		private readonly double y;
		private readonly double factor;

		public ScaleCommand(Point2D point, double factor)
			: this(point.X, point.Y, factor)
		{
		}

		public ScaleCommand(double x, double y, double factor)
		{
			this.x = x;
			this.y = y;
			this.factor = factor;
		}

		public override void Commit(Move move)
		{
			move.Action = ActionType;
			move.X = x;
			move.Y = y;
			move.Factor = factor;
		}
	}
}