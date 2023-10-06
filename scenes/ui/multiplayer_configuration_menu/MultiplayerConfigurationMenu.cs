using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Godot;
using Godot.DependencyInjection.Attributes;
using JetBrains.Annotations;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events.game_state;
using TicTacToeMultiplayer.scripts.events.lobby;
using TicTacToeMultiplayer.scripts.models;
using TicTacToeMultiplayer.scripts.multiplayer;

namespace TicTacToeMultiplayer.scenes.ui.multiplayer_configuration_menu;

public partial class MultiplayerConfigurationMenu : Control, IServerCreatedHandler, IServerClosedHandler
{
	[Export]
	private LineEdit _hostAddressLineEdit = null!;
	[Export]
	private Button _connectButton = null!;
	[Export]
	private Button _startGameButton = null!;
	[Export]
	private Label _statusLabel = null!;
	
	private MultiplayerModel _multiplayerModel = null!;
	private bool _isServerActive;

	public void HandleServerCreated(string hostIp, int port)
	{
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		
		_hostAddressLineEdit.Text = $"{hostIp}:{port}";
		_hostAddressLineEdit.Editable = false;
		_startGameButton.Visible = true;
		_startGameButton.Disabled = true;
		
		_isServerActive = true;
	}

	public void HandleServerClosed()
	{
		_hostAddressLineEdit.Clear();
		_hostAddressLineEdit.Editable = true;
		_connectButton.Disabled = false;
		_startGameButton.Visible = false;
		_statusLabel.Visible = false;
		
		_isServerActive = false;
	}
	
	[Inject] [UsedImplicitly]
	public void Construct(MultiplayerModel multiplayerModel)
	{
		_multiplayerModel = multiplayerModel;
	}

	public override void _Ready()
	{
		RefreshStateFromController();
		
		_hostAddressLineEdit.TextChanged += OnHostAddressChanged;
		_connectButton.Pressed += OnConnectButtonPressed;
		_startGameButton.Pressed += OnStartGameButtonPressed;
		EventBus.Subscribe(this);
	}

	public override void _Process(double delta)
	{
		if (_isServerActive && !Multiplayer.IsServer()) {
			return;
		}
		List<PlayerModel> players = _multiplayerModel.Players;
		_startGameButton.Disabled = players.Count < 2 || players.Any(player => player.Status != PlayerStatus.LOBBY);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}

	private void OnStartGameButtonPressed()
	{
		EventBus.RaiseEvent<IGameStartAttemptHandler>(h => h?.HandleGameStartAttempt());
	}

	private void OnConnectButtonPressed()
	{
		_hostAddressLineEdit.Editable = false;
		_connectButton.Disabled = true;
		
		if (_hostAddressLineEdit.Text.Length > 0) {
			Multiplayer.ConnectedToServer += OnConnectedToServer;
			Multiplayer.ConnectionFailed += OnConnectionFailed;
			
			string[] hostAddressParts = _hostAddressLineEdit.Text.Split(':');
			string hostAddress = hostAddressParts[0];
			int port = int.Parse(hostAddressParts[1]);
			EventBus.RaiseEvent<IJoinAttemptHandler>(h => h?.HandleJoinAttempt(hostAddress, port));
		} else {
			string localAddress = GetLocalAddress();
			EventBus.RaiseEvent<IHostAttemptHandler>(h => h?.HandleHostAttempt(localAddress, GetAvailablePort()));
		}
	}

	private void OnPeerConnected(long id)
	{
		_startGameButton.Disabled = false;
	}

	private void OnPeerDisconnected(long id)
	{
		if (Multiplayer.GetPeers().Length > 1) {
			return;
		}
		_startGameButton.Disabled = true;
	}

	private void OnConnectedToServer()
	{
		Multiplayer.ConnectedToServer -= OnConnectedToServer;
		
		ShowJoinStatus("Connected!\nWait for the host to start the game", new Color(0, 1, 0));
	}

	private void OnConnectionFailed()
	{
		Multiplayer.ConnectionFailed -= OnConnectionFailed;
		
		_hostAddressLineEdit.Editable = true;
		_connectButton.Disabled = false;
		ShowJoinStatus("Connection failed", new Color(1, 0, 0));
	}

	private void OnHostAddressChanged(string newtext)
	{
		_connectButton.Text = newtext.Length > 0 ? "Join" : "Host";
	}

	private void RefreshStateFromController()
	{
		if (_multiplayerModel.Players.Count == 0) {
			return;
		}
		
		if (Multiplayer.IsServer()) {
			_hostAddressLineEdit.Text = $"{_multiplayerModel.HostIp}:{_multiplayerModel.HostPort}";
			_hostAddressLineEdit.Editable = false;
			
			_connectButton.Disabled = true;
			_startGameButton.Visible = true;
			return;
		}
		
		_hostAddressLineEdit.Text = $"{_multiplayerModel.HostIp}:{_multiplayerModel.HostPort}";
		_hostAddressLineEdit.Editable = false;
		_connectButton.Disabled = true;
		_startGameButton.Visible = false;
		ShowJoinStatus("Connected!\nWait for the host to start the game", new Color(0, 1, 0));
	}

	private void ShowJoinStatus(string text, Color color)
	{
		_statusLabel.Text = text;
		_statusLabel.AddThemeColorOverride("font_color", color);
		_statusLabel.Visible = true;
	}

	private string GetLocalAddress()
	{
		string hostName = Dns.GetHostName();
		IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
		List<string> localAddresses = hostEntry.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
		                                       .Select(ip => ip.ToString())
		                                       .ToList();
		return localAddresses.First();
	}

	private int GetAvailablePort()
	{
		using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
		{
			socket.Bind(new IPEndPoint(IPAddress.Any, 0));
			IPEndPoint? ipEndPoint = socket.LocalEndPoint as IPEndPoint;
			if (ipEndPoint == null) {
				throw new System.Exception("Could not get local IP address");
			}
			return ipEndPoint.Port;
		}
	}
}
