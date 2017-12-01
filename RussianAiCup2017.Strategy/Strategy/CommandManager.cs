using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy
{
	public class CommandManager
	{
		private readonly Dictionary<int, Queue<Command>> commandQueues = new Dictionary<int, Queue<Command>>();
		private bool forcePlayNextCommand;
		private int lockedFormationId;

		public void EnqueueCommand(Command command)
		{
			var formationId = command.FormationId;
			if (!commandQueues.ContainsKey(formationId))
				commandQueues.Add(formationId, new Queue<Command>());
			commandQueues[formationId].Enqueue(command);
		}

		public void ClearCommandsQueue(int formationId)
		{
			if (commandQueues.ContainsKey(formationId))
				commandQueues[formationId].Clear();
		}

		public Command PeekCommand(int formationId)
		{
			return !commandQueues.ContainsKey(formationId)
				? null
				: commandQueues[formationId].Peek();
		}

		public Command PeekLastCommand(int formationId)
		{
			return !commandQueues.ContainsKey(formationId)
				? null
				: commandQueues[formationId].Last();
		}

		public bool PlayCommandIfPossible(VehicleRegistry registry, Player player, Move move, int worldTick)
		{
			return lockedFormationId != 0
				? PlayFormationCommandIfPossible(commandQueues[lockedFormationId], registry, player, move, worldTick)
				: commandQueues.Any(p => PlayFormationCommandIfPossible(p.Value, registry, player, move, worldTick));
		}

		private bool PlayFormationCommandIfPossible(Queue<Command> formationQueue,
			VehicleRegistry registry,
			Player player,
			Move move,
			int worldTick)
		{
			if (formationQueue.Count == 0)
				return false;
			var canPlayCommand = CanPlayCommand(player);
			if (!canPlayCommand)
				return false;
			var currentCommand = formationQueue.Peek();
#if DEBUG
			RewindClient.Instance.Message($"Current command {currentCommand}");
#endif
			if (!currentCommand.IsStarted())
			{
				currentCommand.Commit(move, registry);
				if (IsSelectCommand(currentCommand))
					lockedFormationId = currentCommand.FormationId;
				if (formationQueue.Count == 1 || IsSelectCommand(formationQueue.ElementAt(1)))
					lockedFormationId = 0;
#if DEBUG
				RewindClient.Instance.Message($"Commiting command {currentCommand}");
#endif
			}
			if (forcePlayNextCommand)
			{
				forcePlayNextCommand = currentCommand.ForcePlayNextCommand;
				return true;
			}
			if (currentCommand.CanBeParallel() || currentCommand.IsFinished(worldTick, registry))
			{
#if DEBUG
				RewindClient.Instance.Message($"{currentCommand} {nameof(currentCommand.CanBeParallel)}:{currentCommand.CanBeParallel()}");
				RewindClient.Instance.Message($"{currentCommand} {nameof(currentCommand.IsFinished)}:{currentCommand.IsFinished(worldTick, registry)}");
#endif
				forcePlayNextCommand = currentCommand.ForcePlayNextCommand;
				formationQueue.Dequeue();
				return true;
			}
			return false;
		}

		private bool IsSelectCommand(Command command)
		{
			return command is SelectCommand || command is SelectGroupCommand || command is SelectVehiclesCommand;
		}

		private static bool CanPlayCommand(Player player)
		{
			return player.RemainingActionCooldownTicks == 0;
		}

		public int GetCurrentQueueSize()
		{
			return commandQueues.SelectMany(p => p.Value).Count();
		}
	}
}