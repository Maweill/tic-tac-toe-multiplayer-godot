using System;
using System.Collections.Generic;
using System.Linq;
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
		get => _players;
		set
		{
			_players = value;
			Changed?.Invoke();
		}
	}

	public ENetMultiplayerPeer? Peer
	{
		get => _peer;
		set
		{
			_peer = value;
			Changed?.Invoke();
		}
	}
	
	public string? HostIp
	{
		get => _hostIp;
		set
		{
			_hostIp = value;
			Changed?.Invoke();
		}
	}

	public int HostPort
	{
		get => _hostPort;
		set
		{
			_hostPort = value;
			Changed?.Invoke();
		}
	}
}
