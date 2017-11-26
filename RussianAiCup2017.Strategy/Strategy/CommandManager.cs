using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class CommandManager
	{
		private readonly Queue<Tuple<Command, int>> commandsQueue = new Queue<Tuple<Command, int>>();
		private const int CommandTimeout = 450;
		private bool forcePlayNextCommand;

		public void EnqueueCommand(Command command, int worldTick)
		{
			commandsQueue.Enqueue(new Tuple<Command, int>(command, worldTick));
		}

		public void ClearCommandsQueue()
		{
			commandsQueue.Clear();
		}

		public Command PeekCommand()
		{
			return commandsQueue.Peek().Item1;
		}

		public Command PeekLastCommand()
		{
			return commandsQueue.Last().Item1;
		}

		public List<Command> PeekCommands(int count)
		{
			return commandsQueue.Take(count).Select(t => t.Item1).ToList();
		}

		public bool PlayCommandIfPossible(VehicleRegistry registry, Player player, Move move, int worldTick)
		{
			if (commandsQueue.Count == 0)
				return false;
			var canPlayCommand = CanPlayCommand(player);
			if (!canPlayCommand)
				return false;
			var currentCommand = commandsQueue.Peek().Item1;
			var currentCommandStartTick = commandsQueue.Peek().Item2;
			if (!currentCommand.IsStarted())
				currentCommand.Commit(move, registry);
			if (forcePlayNextCommand)
			{
				forcePlayNextCommand = currentCommand.ForcePlayNextCommand;
				return true;
			}
			if (currentCommand.CanBeParallel() || currentCommand.IsFinished(worldTick, registry) ||
			    worldTick - currentCommandStartTick > CommandTimeout)
			{
				forcePlayNextCommand = currentCommand.ForcePlayNextCommand;
				commandsQueue.Dequeue();
			}
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