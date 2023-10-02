using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.lobby;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.ENetConnection;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.autoload.multiplayer_controller;

public partial class MultiplayerController : Node, IHostAttemptHandler, IJoinAttemptHandler
{
	public static readonly List<PlayerModel> Players = new();
	
	private const CompressionMode COMPRESSION_MODE = CompressionMode.RangeCoder;
	
	private ENetMultiplayerPeer? _peer;
	
	public string? HostIp { get; private set; }
	public int HostPort { get; private set; }

	[Rpc(RpcMode.AnyPeer, CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	public void ChangePlayerStatus(int id, PlayerStatus status)
	{
		PlayerModel player = Players.First(player => player.Id == id);
		GD.Print($"Player status changed, old={player.Status}, new={status}");
		player.Status = status;
	}

	public void HandleHostAttempt(string ip, int port)
	{
		HostGame(ip, port);
		AddPlayer(1);
		
		HostIp = ip;
		HostPort = port;
	}

	public void HandleJoinAttempt(string ip, int port)
	{
		_peer = new ENetMultiplayerPeer();
		_peer.CreateClient(ip, port);

		_peer.Host.Compress(COMPRESSION_MODE);
		Multiplayer.MultiplayerPeer = _peer;
		
		HostIp = ip;
		HostPort = port;
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

	[Rpc(RpcMode.AnyPeer)]
	private void AddPlayer(int id)
	{
		PlayerModel playerModel = new() {
				Id = id,
				Status = PlayerStatus.LOBBY
		};
		
		if (!Players.Contains(playerModel)) {
			Players.Add(playerModel);
			GD.Print("Player added, id = " + playerModel.Id);
		}

		if (!Multiplayer.IsServer()) {
			return;
		}
		foreach (PlayerModel item in Players) {
			Rpc(nameof(AddPlayer), item.Id);
		}
	}

	public PlayerModel Player
	{
		get
		{
			return Players.First(player => player.Id == Multiplayer.GetUniqueId());
		}
	}
}
