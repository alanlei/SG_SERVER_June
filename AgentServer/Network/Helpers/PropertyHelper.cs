﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence.txt in the main folder
using System.Collections.Generic;
using LocalCommons.World.ObjectProperties;

namespace LocalCommons.Network.Helpers
{
	public static class PropertyHelper
	{
		/// <summary>
		/// Adds properties to packet, with key and value. Does not write
		/// the property's size!
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="properties"></param>
		public static void AddProperties(this Packet packet, IEnumerable<IProperty> properties)
		{
			foreach (var property in properties)
			{
				packet.PutInt(property.Id);
				switch (property)
				{
					case FloatProperty floatProperty: packet.PutFloat(floatProperty.Value); break;
					case StringProperty stringProperty: packet.PutLpString(stringProperty.Value); break;
				}
			}
		}
	}
}
