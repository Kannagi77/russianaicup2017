using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public abstract class Command
	{
		public abstract void Commit(Move move);
		public abstract bool IsStarted();
		public abstract bool IsFinished();
		public abstract bool CanBeParallel();
	}
}