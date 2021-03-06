﻿using System;
using System.IO;
using System.Net;

namespace LibClassicBot.Networking
{
	/// <summary>
	/// Static class to create all the packets that can be sent by a Minecraft Classic client.
	/// </summary>
	public static class Packets
	{
		/// <summary>Classic protocol version. Currently stands at seven.</summary>
		public const byte ProtocolVersion = 7;
		
		/// <summary>Creates a login packet from the given username and verification key / mppass.</summary>
		/// <param name="username">The username to use. This should ideally be 16 characters or less,
		/// as servers kick usernames with more than sixteen characters.</param>
		/// <param name="verificationKey">The verification key to use, can be simply used as String.Empty.</param>		
		public static byte[] CreateLoginPacket(string username, string verkey)
		{
			using (MemoryStream LogMemStream = new MemoryStream(131))
				using (BinaryWriter LoginWriter = new BinaryWriter(LogMemStream))
			{
				LoginWriter.Write((byte)ClientPackets.PlayerIdentification); //PacketID
				LoginWriter.Write((byte)ProtocolVersion); //Unused
				LoginWriter.Write(StringToBytes(username));
				LoginWriter.Write(StringToBytes(verkey));
				LoginWriter.Write((byte)0);
				return LogMemStream.ToArray();
			}			
		}
		
		public static byte[] StringToBytes(string s)
		{
			byte[] MessageBytes = System.Text.Encoding.ASCII.GetBytes(s);
			byte[] FullArray = new byte[64];
			for (int i = 0; i < FullArray.Length; i++)
			{
				FullArray[i] = 32; //' ' Vanilla doesn't have a problem if we use 0, but other servers do.
			}
			Buffer.BlockCopy(MessageBytes,0,FullArray,0,MessageBytes.Length);
			return FullArray;
		}
		
			
		/// <summary>Creates a setblock packet at the specified coordinates, with the specified mode and blocktype.</summary>
		/// <remarks>0x00 = Delete, 0x01 = Create for mode.</remarks> 
		public static byte[] CreateSetBlockPacket(short x, short y, short z, byte mode, byte type)
		{
			using (MemoryStream blockMemStream = new MemoryStream(9))
				using (BinaryWriter BlockWriter = new BinaryWriter(blockMemStream))
			{
				BlockWriter.Write((byte)ClientPackets.SetBlock); //PacketID
				BlockWriter.Write(IPAddress.HostToNetworkOrder(x));
				BlockWriter.Write(IPAddress.HostToNetworkOrder(z));
				BlockWriter.Write(IPAddress.HostToNetworkOrder(y));
				BlockWriter.Write(mode);
				BlockWriter.Write(type);
				return blockMemStream.ToArray();
			}
		}

		/// <summary>Creates a position update packet from the specified X,Y and Z coordinates. Also takes yaw and pitch as bytes.</summary>
		public static byte[] CreatePositionPacket(short x, short y, short z, byte yaw, byte pitch)
		{
			using (MemoryStream PosMemStream = new MemoryStream(10))
				using (BinaryWriter PosWriter = new BinaryWriter(PosMemStream))
			{
				PosWriter.Write((byte)ClientPackets.PositionUpdate); //PacketID
				PosWriter.Write((byte)255); //Unused
				PosWriter.Write(IPAddress.HostToNetworkOrder((short)(x * 32)));
				PosWriter.Write(IPAddress.HostToNetworkOrder((short)((z + 1.21f) * 32))); //1.21 = character height
				PosWriter.Write(IPAddress.HostToNetworkOrder((short)(y * 32)));
				PosWriter.Write(yaw);
				PosWriter.Write(pitch);
				return PosMemStream.ToArray();
			}
		}

		/// <summary>Creates a  message packet. A single chat line has a maximum length of 64 characters, 
		/// and if the string length is greater than this, it will be cut off.</summary>
		public static byte[] CreateMessagePacket(string message)
		{
			if(message.Length > 64) message.Substring(0, 64);
			using (MemoryStream ChatMemStream = new MemoryStream(66))
				using (BinaryWriter ChatWriter = new BinaryWriter(ChatMemStream))
			{
				ChatWriter.Write((byte)ServerPackets.Message);
				ChatWriter.Write((byte)255);//Unused.
				ChatWriter.Write(StringToBytes(message));
				return ChatMemStream.ToArray();
			}
		}
		
	}
}