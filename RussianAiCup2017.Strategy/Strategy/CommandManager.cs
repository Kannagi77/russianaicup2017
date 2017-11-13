using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class CommandManager
	{
		private readonly Queue<Command> commandsQueue = new Queue<Command>();

		public void EnqueueCommand(Command command)
		{
			commandsQueue.Enqueue(command);
		}

		public void ClearCommandsQueue()
		{
			commandsQueue.Clear();
		}

		public Command PeekCommand()
		{
			return commandsQueue.Peek();
		}

		public List<Command> PeekCommands(int count)
		{
			return commandsQueue.Take(count).ToList();
		}

		public bool PlayCommandIfPossible(Player player, Move move)
		{
			if (commandsQueue.Count == 0)
				return false;
			var canPlayCommand = CanPlayCommand(player);
			if (!canPlayCommand)
				return false;
			var currentCommand = commandsQueue.Peek();
			if (!currentCommand.IsStarted())
				currentCommand.Commit(move);
			if (currentCommand.CanBeParallel() || currentCommand.IsFinished())
				commandsQueue.Dequeue();
			return true;
		}

		public bool CanPlayCommand(Player player)
		{
			return player.RemainingActionCooldownTicks == 0;
		}

		public int GetCurrentQueueSize()
		{
			return commandsQueue.Count;
		}
	}
}