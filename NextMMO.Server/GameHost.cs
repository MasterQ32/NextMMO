﻿using Lidgren.Network;
using NextMMO.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NextMMO.Server
{
	public class GameHost
	{
		NetServer server;
		MessageDispatcher dispatcher;

		PlayerCollection players;

		public GameHost(int port)
		{
			var config = new NetPeerConfiguration("mq32.de.NextMMO");
			config.Port = port;
			this.server = new NetServer(config);

			this.players = new PlayerCollection(this);

			this.dispatcher = new MessageDispatcher();
			this.dispatcher[MessageType.UpdatePlayerPosition] = this.UpdatePlayer;
		}

		private void UpdatePlayer(MessageType type, NetIncomingMessage msg)
		{
			var player = this.players[msg.SenderConnection];

			float x = msg.ReadFloat();
			float y = msg.ReadFloat();
			byte animation = msg.ReadByte();

			int playerID = player.ID;

			var updateMsg = this.CreateMessag(MessageType.UpdatePlayerPosition);
			updateMsg.Write(playerID);
			updateMsg.Write(x);
			updateMsg.Write(y);
			updateMsg.Write(animation);

			this.players.BroadcastMessage(updateMsg, NetDeliveryMethod.Unreliable, player);
		}

		public void Start()
		{
			this.server.Start();
			while (this.server.Status != NetPeerStatus.NotRunning)
			{
				NetIncomingMessage msg;
				while((msg = this.server.ReadMessage()) != null)
				{
					switch(msg.MessageType)
					{
						case NetIncomingMessageType.Data:
							this.dispatcher.Dispatch(msg);
							break;
						default:
							Console.WriteLine("Unhandled message type: {0}", msg.MessageType);
							break;
					}
				}
				Thread.Sleep(10);
			}
		}

		public NetOutgoingMessage CreateMessag(MessageType type)
		{
			var msg = this.server.CreateMessage();
			msg.Write((byte)type);
			return msg;
		}
	}
}