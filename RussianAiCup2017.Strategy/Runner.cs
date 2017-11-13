using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	public sealed class Runner
	{
		private readonly RemoteProcessClient remoteProcessClient;
		private readonly string token;

		public static void Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
			if (args.Length == 3)
			{
				new Runner(args).Run();
			}
			else
			{
				var localRunnerProcessStartInfo = new ProcessStartInfo
				{
					WorkingDirectory = @"D:\Source\russianaicup\russianaicup2017\local-runner",
					FileName = @"D:\Source\russianaicup\russianaicup2017\local-runner\local-runner.bat",
					CreateNoWindow = false,
					UseShellExecute = false
				};
				Process.Start(localRunnerProcessStartInfo);
				Thread.Sleep(2000);
#if DEBUG
				Debug.connect("127.0.0.1", 13579);
#endif
				new Runner(new[] { "127.0.0.1", "31001", "0000000000000000" }).Run();
#if DEBUG
				Debug.disconnect();
#endif
			}
		}

		private Runner(IReadOnlyList<string> args)
		{
			remoteProcessClient = new RemoteProcessClient(args[0], int.Parse(args[1]));
			token = args[2];
		}

		public void Run()
		{
			try
			{
				remoteProcessClient.WriteTokenMessage(token);
				remoteProcessClient.WriteProtocolVersionMessage();
				remoteProcessClient.ReadTeamSizeMessage();
				Game game = remoteProcessClient.ReadGameContextMessage();

				IStrategy strategy = new MyStrategy();

				PlayerContext playerContext;

				while ((playerContext = remoteProcessClient.ReadPlayerContextMessage()) != null)
				{
					Player player = playerContext.Player;
					if (player == null)
					{
						break;
					}

					Move move = new Move();
					strategy.Move(player, playerContext.World, game, move);

					remoteProcessClient.WriteMoveMessage(move);
				}
			}
			finally
			{
				remoteProcessClient.Close();
			}
		}
	}
}