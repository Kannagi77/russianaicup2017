﻿using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk.Model;

namespace Com.CodeGame.CodeWars2017.DevKit.CSharpCgdk
{
	/// <inheritdoc />
	/// <summary>
	///     A simple implementation of rewind client in C#.
	/// </summary>
	/// <remarks>
	///     Note: You have to reference System.Drawing in order to use the client.
	///     Usage:
	///     RewindClient.Instance.AreaDescription(2, 4, AreaType.Cloud);
	///     RewindClient.Instance.End();
	/// </remarks>
	/// <seealso cref="T:System.IDisposable" />
	// ReSharper disable once UnusedMember.Global
	// ReSharper disable once StyleCop.SA1201
	public class RewindClient : IDisposable
	{
		private const int BufferSizeBytes = 1 << 20;

		private const int DefaultLayer = 3;

		private static RewindClient instance;

		private readonly TcpClient client;

		private readonly BinaryWriter writer;

		private RewindClient(string host, int port)
		{
			client = new TcpClient(host, port)
			{
				SendBufferSize = BufferSizeBytes,
				ReceiveBufferSize = BufferSizeBytes,
				NoDelay = true
			};

			writer = new BinaryWriter(client.GetStream());

			client = new TcpClient(host, port);

			CultureInfo newCInfo = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			newCInfo.NumberFormat.NumberDecimalSeparator = ".";
			Thread.CurrentThread.CurrentCulture = newCInfo;
		}

		public static RewindClient Instance => instance ?? (instance = new RewindClient("127.0.0.1", 9111));

		public void Dispose()
		{
			client?.Dispose();
			writer?.Dispose();
			instance?.Dispose();
		}

		public void End()
		{
			SendCommand("{\"type\":\"end\"}");
		}

		public void Circle(double x, double y, double r, Color color, int layer = DefaultLayer)
		{
			SendCommand(
				$"{{\"type\": \"circle\", \"x\": {x}, \"y\": {y}, \"r\": {r}, \"color\": {color.ToArgb()}, \"layer\": {layer}}}");
		}

		public void Rectangle(double x1, double y1, double x2, double y2, Color color, int layer = DefaultLayer)
		{
			SendCommand(
				$"{{\"type\": \"rectangle\", \"x1\": {x1}, \"y1\": {y1}, \"x2\": {x2}, \"y2\": {y2}, \"color\": {color.ToArgb()}, \"layer\": {layer}}}");
		}

		public void Line(double x1, double y1, double x2, double y2, Color color, int layer = DefaultLayer)
		{
			SendCommand(
				$"{{\"type\": \"line\", \"x1\": {x1}, \"y1\": {y1}, \"x2\": {x2}, \"y2\": {y2}, \"color\": {color.ToArgb()}, \"layer\": {layer}}}");
		}

		public void LivingUnit(
			double x,
			double y,
			double r,
			int hp,
			int maxHp,
			Side side,
			VehicleType vehicleType,
			double course = 0,
			int remCooldown = 0,
			int maxCooldown = 0,
			bool selected = false)
		{
			SendCommand(
				$"{{\"type\": \"unit\", \"x\": {x}, \"y\": {y}, \"r\": {r}, \"hp\": {hp}, \"max_hp\": {maxHp}, \"enemy\": {(int) side}, \"unit_type\": {(int) vehicleType.ToUnitType()}, \"course\": {course},"
				+ $"\"rem_cooldown\":{remCooldown}, \"cooldown\":{maxCooldown}, \"selected\":{(selected ? 1 : 0)} }}");
		}

		public void AreaDescription(int cellX, int cellY, AreaType areaType)
		{
			SendCommand($"{{\"type\": \"area\", \"x\": {cellX}, \"y\": {cellY}, \"area_type\": {(int) areaType}}}");
		}

		public void Message(string message)
		{
			SendCommand($"{{\"type\": \"message\", \"message\": \"{message}\"}}");
		}

		private void SendCommand(string command)
		{
			WriteString(command);
			writer.Flush();
		}

		private void WriteInt(int value)
		{
			writer.Write(value);
		}

		private void WriteString(string value)
		{
			if (value == null)
			{
				WriteInt(-1);
				return;
			}

			var bytes = Encoding.UTF8.GetBytes(value);
			writer.Write(bytes, 0, bytes.Length);
		}
	}

	public enum Side
	{
		Our = -1,
		Neutral = 0,
		Enemy = 1
	}

	public enum UnitType
	{
		Unknown = 0,
		Tank = 1,
		Ifv = 2,
		Arrv = 3,
		Helicopter = 4,
		Fighter = 5,
	}

	public enum AreaType
	{
		Unknown = 0,
		Forest = 1,
		Swamp = 2,
		Rain = 3,
		Cloud = 4
	}

	public static class RewindClientHelper
	{
		public static UnitType ToUnitType(this VehicleType type)
		{
			switch (type)
			{
				case VehicleType.Arrv:
					return UnitType.Arrv;
				case VehicleType.Fighter:
					return UnitType.Fighter;
				case VehicleType.Helicopter:
					return UnitType.Helicopter;
				case VehicleType.Ifv:
					return UnitType.Ifv;
				case VehicleType.Tank:
					return UnitType.Tank;
				default:
					return UnitType.Unknown;
			}
		}
	}
}
