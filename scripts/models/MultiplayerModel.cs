using System.Collections.Generic;
using Godot;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.models.multiplayer_model;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scripts.models;

public class MultiplayerModel
{
	private List<PlayerModel> _players = new();
	private ENetMultiplayerPeer? _peer;
	private string? _hostIp;
	private int _hostPort;

	public void Reset()
	{
		Peer?.Close();
		Peer = null;
		HostIp = null;
		HostPort = 0;
		Players.Clear();
	}
	
	public List<PlayerModel> Players
	{
		get { return _players; }
		set
		{
			_players = value;
			EventBus.RaiseEvent<IMultiplayerModelChangedHandler>(h => h?.HandleMultiplayerModelChanged(this));
		}
	}

	public ENetMultiplayerPeer? Peer
	{
		get { return _peer; }
		set
		{
			_peer = value;
			EventBus.RaiseEvent<IMultiplayerModelChangedHandler>(h => h?.HandleMultiplayerModelChanged(this));
		}
	}
	
	public string? HostIp
	{
		get { return _hostIp; }
		set
		{
			_hostIp = value;
			EventBus.RaiseEvent<IMultiplayerModelChangedHandler>(h => h?.HandleMultiplayerModelChanged(this));
		}
	}

	public int HostPort
	{
		get { return _hostPort; }
		set
		{
			_hostPort = value;
			EventBus.RaiseEvent<IMultiplayerModelChangedHandler>(h => h?.HandleMultiplayerModelChanged(this));
		}
	}
}
