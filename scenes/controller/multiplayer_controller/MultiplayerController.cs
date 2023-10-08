using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.DependencyInjection.Attributes;
using JetBrains.Annotations;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.lobby;
using TicTacToeMultiplayer.scripts.models;
using TicTacToeMultiplayer.scripts.multiplayer;
using static Godot.ENetConnection;
using static Godot.MultiplayerApi;
using static Godot.MultiplayerPeer;

namespace TicTacToeMultiplayer.scenes.controller.multiplayer_controller;

public partial class MultiplayerController : Node, IHostAttemptHandler, IJoinAttemptHandler
{
	private const CompressionMode COMPRESSION_MODE = CompressionMode.RangeCoder;

	private MultiplayerModel _multiplayerModel = null!;
	
	[Rpc(RpcMode.AnyPeer, CallLocal = true, TransferMode = TransferModeEnum.Reliable)]
	public void ChangePlayerStatus(int id, PlayerStatus status)
	{
		PlayerModel player = _multiplayerModel.Players.First(player => player.Id == id);
		GD.Print($"Player status changed, old={player.Status}, new={status}");
		player.Status = status;
	}

	public void HandleHostAttempt(string ip, int port)
	{
		HostGame(port);
		AddPlayer(1);
		
		_multiplayerModel.HostIp = ip;
		_multiplayerModel.HostPort = port;
	}

	public void HandleJoinAttempt(string ip, int port)
	{
		ENetMultiplayerPeer peer = new();
		peer.CreateClient(ip, port);
		peer.Host.Compress(COMPRESSION_MODE);
		Multiplayer.MultiplayerPeer = peer;
		_multiplayerModel.Peer = peer;
		
		_multiplayerModel.HostIp = ip;
		_multiplayerModel.HostPort = port;
	}
	
	[Inject] [UsedImplicitly]
	public void Construct(MultiplayerModel multiplayerModel)
	{
		_multiplayerModel = multiplayerModel;
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
		if (id == 1) {
			_multiplayerModel.Reset();
			return;
		}
		List<PlayerModel> players = _multiplayerModel.Players;
		players.Remove(players.First(i => i.Id == id));
	}

	private void OnPeerConnected(long id)
	{
		GD.Print("Player Connected! " + id);
	}

	private void HostGame(int port)
	{
		ENetMultiplayerPeer peer = new();
		Error error = peer.CreateServer(port, 2);
		if (error != Error.Ok) {
			GD.Print("error cannot host! :" + error);
			return;
		}
		peer.Host.Compress(COMPRESSION_MODE);
		Multiplayer.MultiplayerPeer = peer;
		_multiplayerModel.Peer = peer;
		
		GD.Print("Waiting For Players!");
	}

	[Rpc(RpcMode.AnyPeer)]
	private void AddPlayer(int id)
	{
		PlayerModel playerModel = new() {
				Id = id,
				Status = PlayerStatus.LOBBY
		};

		List<PlayerModel> players = _multiplayerModel.Players;
		if (!players.Contains(playerModel)) {
			players.Add(playerModel);
			GD.Print("Player added, id = " + playerModel.Id);
		}

		if (!Multiplayer.IsServer()) {
			return;
		}
		foreach (PlayerModel item in players) {
			Rpc(nameof(AddPlayer), item.Id);
		}
	}

	public PlayerModel Player
	{
		get => _multiplayerModel.Players.First(player => player.Id == Multiplayer.GetUniqueId());
	}
}
