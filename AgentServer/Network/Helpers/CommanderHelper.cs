﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder
using LocalCommons.World;

namespace LocalCommons.Network.Helpers
{
	public static class CommanderHelper
	{
		/// <summary>
		/// Serializes an object that implements ICommander and places it into the packet.
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="commander"></param>
		public static void AddCommander(this Packet packet, ICommander commander)
		{
			packet.PutInt(commander.Handle);
			packet.PutInt(0);
			packet.AddAppearancePc(commander);
			packet.PutFloat(commander.Position.X);
			packet.PutFloat(commander.Position.Y);
			packet.PutFloat(commander.Position.Z);
			packet.PutInt(0);
			packet.PutInt(commander.Exp);
			packet.PutInt(0);
			packet.PutInt(commander.MaxExp);
			packet.PutInt(0);
			packet.PutInt(commander.TotalExp);
			packet.PutInt(0);

			packet.PutLong(commander.Id);

			// TODO: Assign new IDs to characters such that the social ID does not conflict.
			packet.PutLong(commander.Id + 1);   // SocialInfoId

			packet.PutInt(commander.Hp);
			packet.PutInt(commander.MaxHp);
			packet.PutShort(commander.Sp);
			packet.PutShort(commander.MaxSp);
			packet.PutInt(commander.Stamina);
			packet.PutInt(commander.MaxStamina);
			packet.PutShort(0); // Shield
			packet.PutShort(0); // MaxShield
		}
	}

	public interface ICommander : IAppearancePc
	{
		int Handle { get; }
		Position Position { get; }
		int Exp { get; }
		int MaxExp { get; }
		int TotalExp { get; }
		long Id { get; }
		int Hp { get; }
		int MaxHp { get; }
		int Sp { get; }
		int MaxSp { get; }
		int Stamina { get; }
		int MaxStamina { get; }
	}
}
