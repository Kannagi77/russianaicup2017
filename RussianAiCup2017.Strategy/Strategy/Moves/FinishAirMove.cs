using System;
using System.Collections.Generic;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Helpers;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Moves
{
	public class FinishAirMove : StrategyMove
	{
		public override StrategyState State => StrategyState.FinishAirMove;
		private readonly List<MoveCommand> commands = new List<MoveCommand>();

		public FinishAirMove(CommandManager commandManager, VehicleRegistry vehicleRegistry)
			: base(commandManager, vehicleRegistry)
		{
		}

		public override StrategyState Perform(World world, Player me, Game game)
		{
			if (!commands.Any())
			{
				DoWork(me, world);
			}

			if (commands.Any() && commands.All(c => c.IsStarted()) && commands.All(c => c.IsFinished(VehicleRegistry)))
			{
				commands.Clear();
				return StrategyState.FinishFormation;
			}
			return StrategyState.FinishAirMove;
		}

		private void DoWork(Player me, World world)
		{
			var myVehicles = VehicleRegistry.MyVehicles(me);
			var tanks = myVehicles.Where(v => v.Type == VehicleType.Tank).ToList();
			var centerOfTanks = tanks.GetCenterPoint();

			var arrvs = myVehicles.Where(v => v.Type == VehicleType.Arrv).ToList();
			var centerOfArrvs = arrvs.GetCenterPoint();

			var ifvs = myVehicles.Where(v => v.Type == VehicleType.Ifv).ToList();
			var centerOfIfvs = ifvs.GetCenterPoint();

			VehicleType upperOrLeftGroupType;
			VehicleType bottomOrRightGroupType;
			if (IsHorizontalFormation(centerOfTanks, centerOfArrvs, centerOfIfvs))
			{
#if DEBUG
				RewindClient.Instance.Message("HORIZONTAL; ");
#endif
				upperOrLeftGroupType = FormationHelper.GetUpperOrLeftGroupType(centerOfTanks.X, centerOfArrvs.X, centerOfIfvs.X);
				bottomOrRightGroupType = FormationHelper.GetBottomOrRightGroupType(centerOfTanks.X, centerOfArrvs.X, centerOfIfvs.X);
			}
			else
			{
#if DEBUG
				RewindClient.Instance.Message("VERTICAL; ");
#endif
				upperOrLeftGroupType = FormationHelper.GetUpperOrLeftGroupType(centerOfTanks.Y, centerOfArrvs.Y, centerOfIfvs.Y);
				bottomOrRightGroupType = FormationHelper.GetBottomOrRightGroupType(centerOfTanks.Y, centerOfArrvs.Y, centerOfIfvs.Y);
			}
#if DEBUG
			RewindClient.Instance.Message($"{nameof(upperOrLeftGroupType)} = {upperOrLeftGroupType}; ");
			RewindClient.Instance.Message($"{nameof(bottomOrRightGroupType)} = {bottomOrRightGroupType}; ");
#endif

			var upperOrLeftGroupCenter = myVehicles.Where(v => v.Type == upperOrLeftGroupType).GetCenterPoint();
			var bottomOrRightGroupCenter = myVehicles.Where(v => v.Type == bottomOrRightGroupType).GetCenterPoint();

			var fightersGroup = new VehiclesGroup(myVehicles.Where(v => v.Type == VehicleType.Fighter).Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);
			var helicoptersGroup = new VehiclesGroup(myVehicles.Where(v => v.Type == VehicleType.Helicopter).Select(v => v.Id).ToList(), VehicleRegistry, CommandManager);

			var willIntersect = GeometryHelper.IsLinePartsIntersected(fightersGroup.Center, bottomOrRightGroupCenter,
				helicoptersGroup.Center, upperOrLeftGroupCenter);
#if DEBUG
			RewindClient.Instance.Message($"{nameof(fightersGroup.Center)} = {fightersGroup.Center}; ");
			RewindClient.Instance.Message($"{nameof(bottomOrRightGroupCenter)} = {bottomOrRightGroupCenter}; ");
			RewindClient.Instance.Message($"{nameof(helicoptersGroup.Center)} = {helicoptersGroup.Center}; ");
			RewindClient.Instance.Message($"{nameof(upperOrLeftGroupCenter)} = {upperOrLeftGroupCenter}; ");
			RewindClient.Instance.Message($"{nameof(willIntersect)} = {willIntersect}; ");
#endif
			if (!willIntersect)
			{
				fightersGroup
					.Select(world, VehicleType.Fighter)
					.MoveTo(bottomOrRightGroupCenter, world);
				commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

				helicoptersGroup
					.Select(world, VehicleType.Helicopter)
					.MoveTo(upperOrLeftGroupCenter, world);
				commands.Add(CommandManager.PeekLastCommand() as MoveCommand);
			}
			else
			{
				fightersGroup
					.Select(world, VehicleType.Fighter)
					.MoveTo(upperOrLeftGroupCenter, world);
				commands.Add(CommandManager.PeekLastCommand() as MoveCommand);

				helicoptersGroup
					.Select(world, VehicleType.Helicopter)
					.MoveTo(bottomOrRightGroupCenter, world);
				commands.Add(CommandManager.PeekLastCommand() as MoveCommand);
			}
		}

		private bool IsHorizontalFormation(Point2D centerOfTanks, Point2D centerOfArrvs, Point2D centerOfIfvs)
		{
			return Math.Abs(centerOfTanks.Y - centerOfArrvs.Y) < MagicConstants.Eps
				&& Math.Abs(centerOfArrvs.Y - centerOfIfvs.Y) < MagicConstants.Eps;
		}
	}
}