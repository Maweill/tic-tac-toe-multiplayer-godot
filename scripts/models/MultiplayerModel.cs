using System;
using System.Collections.Generic;
using Godot;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scripts.models;

public class MultiplayerModel
{
	public event Action? Changed;
	
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
			Changed?.Invoke();
		}
	}

	public ENetMultiplayerPeer? Peer
	{
		get { return _peer; }
		set
		{
			_peer = value;
			Changed?.Invoke();
		}
	}
	
	public string? HostIp
	{
		get { return _hostIp; }
		set
		{
			_hostIp = value;
			Changed?.Invoke();
		}
	}

	public int HostPort
	{
		get { return _hostPort; }
		set
		{
			_hostPort = value;
			Changed?.Invoke();
		}
	}
}
