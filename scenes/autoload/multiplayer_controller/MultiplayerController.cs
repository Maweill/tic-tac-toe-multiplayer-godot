using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.ENetConnection;

namespace TicTacToeMultiplayer.scenes.autoload.multiplayer_controller;

public partial class MultiplayerController : Node, IHostAttemptHandler, IJoinAttemptHandler
{
	public static readonly List<PlayerInfo> Players = new();
	
	private const CompressionMode COMPRESSION_MODE = CompressionMode.RangeCoder;
	
	private ENetMultiplayerPeer? _peer;

	public void HandleHostAttempt(string ip, int port)
	{
		HostGame(ip, port);
		AddPlayer(1);
	}

	public void HandleJoinAttempt(string ip, int port)
	{
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(ip, port);

		_peer.Host.Compress(COMPRESSION_MODE);
		Multiplayer.MultiplayerPeer = _peer;
	}

	public override void _Ready()
	{
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		Multiplayer.ConnectedToServer += OnConnectedToServer;
		Multiplayer.ConnectionFailed += OnConnectionFailed;
		
		EventBus.Subscribe(this);
	}
	
	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}

	private void OnConnectionFailed()
	{
		GD.Print("Connection Failed");
	}

	private void OnConnectedToServer()
	{
		GD.Print("Connected To Server");
		RpcId(1, nameof(AddPlayer), Multiplayer.GetUniqueId());
	}

	private void OnPeerDisconnected(long id)
	{
		GD.Print("Player Disconnected: " + id);
		Players.Remove(Players.First(i => i.Id == id));
		Array<Node> players = GetTree().GetNodesInGroup("Player");

		foreach (Node item in players) {
			if (item.Name == id.ToString()) {
				item.QueueFree();
			}
		}
	}

	private void OnPeerConnected(long id)
	{
		GD.Print("Player Connected! " + id);
	}

	private void HostGame(string ip, int port)
	{
		_peer = new ENetMultiplayerPeer();
		Error error = _peer.CreateServer(port, 2);
		if (error != Error.Ok) {
			GD.Print("error cannot host! :" + error);
			return;
		}
		_peer.Host.Compress(COMPRESSION_MODE);

		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Waiting For Players!");
		EventBus.RaiseEvent<IServerCreatedHandler>(h => h?.HandleServerCreated(ip, port));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void AddPlayer(int id)
	{
		PlayerInfo playerInfo = new() {
				Id = id
		};
		
		if (!Players.Contains(playerInfo)) {
			Players.Add(playerInfo);
			GD.Print("Player added, id = " + playerInfo.Id);
		}

		if (!Multiplayer.IsServer()) {
			return;
		}
		foreach (PlayerInfo item in Players) {
			Rpc(nameof(AddPlayer), item.Id);
		}
	}
}
