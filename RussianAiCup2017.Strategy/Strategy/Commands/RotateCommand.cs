using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class RotateCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.Rotate;
		private readonly double x;
		private readonly double y;
		private readonly double angle;

		public RotateCommand(double x, double y, double angle)
		{
			this.x = x;
			this.y = y;
			this.angle = angle;
		}

		public override void Commit(Move move)
		{
			move.Action = ActionType;
			move.X = x;
			move.Y = y;
			move.Angle = angle;
		}
	}
}