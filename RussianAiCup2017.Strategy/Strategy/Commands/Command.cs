using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public abstract class Command
	{
		public abstract int FormationId { get; }
		public abstract void Commit(Move move, VehicleRegistry registry);
		public abstract bool IsStarted();
		public abstract bool IsFinished(int worldTick, VehicleRegistry registry);
		public abstract bool CanBeParallel();

		public virtual bool ForcePlayNextCommand => false;
	}
}