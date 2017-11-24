using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Strategy.Commands
{
	public class SelectVehiclesCommand : Command
	{
		private const ActionType ActionType = Model.ActionType.ClearAndSelect;
		private readonly IList<long> vehicleIds;
		private readonly VehicleType? type;
		private readonly bool forcePlayNextCommand;
		private bool isStarted;

		public SelectVehiclesCommand(IList<long> vehicleIds, VehicleType? type = null, bool forcePlayNextCommand = false)
		{
			this.vehicleIds = vehicleIds;
			this.type = type;
			this.forcePlayNextCommand = forcePlayNextCommand;
		}

		public override void Commit(Move move, VehicleRegistry registry)
		{
			var vehicles = registry.GetVehiclesByIds(vehicleIds);

			var x1 = vehicles.Select(v => v.X).Min() - 1;
			var y1 = vehicles.Select(v => v.Y).Min() - 1;
			var x2 = vehicles.Select(v => v.X).Max() + 1;
			var y2 = vehicles.Select(v => v.Y).Max() + 1;

			move.Action = ActionType;
			move.Left = x1;
			move.Top = y1;
			move.Right = x2;
			move.Bottom = y2;
			if (type.HasValue)
			{
				move.VehicleType = type.Value;
			}

			isStarted = true;

#if DEBUG
			RewindClient.Instance.Rectangle(x1, y1, x2, y2, Color.Crimson);
			RewindClient.Instance.End();
#endif
		}

		public override bool IsStarted()
		{
			return isStarted;
		}

		public override bool IsFinished(VehicleRegistry registry)
		{
			return true;
		}

		public override bool CanBeParallel()
		{
			return false;
		}
		public override bool ForcePlayNextCommand => forcePlayNextCommand;
	}
}