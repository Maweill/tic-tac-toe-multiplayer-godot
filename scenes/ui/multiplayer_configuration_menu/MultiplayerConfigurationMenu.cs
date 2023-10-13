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

public partial class MultiplayerConfigurationMenu : Control
{
	[Export]
	private LineEdit _hostAddressLineEdit = null!;
	[Export]
	private Button _hostButton = null!;
	[Export]
	private Button _joinButton = null!;
	[Export]
	private HBoxContainer _connectionButtonsContainer = null!;
	[Export]
	private Button _startGameButton = null!;
	[Export]
	private Label _statusLabel = null!;
	
	private MultiplayerModel _multiplayerModel = null!;
	
	[Inject] [UsedImplicitly]
	public void Construct(MultiplayerModel multiplayerModel)
	{
		_multiplayerModel = multiplayerModel;
	}

	public override void _Ready()
	{
		UpdateStateFromMultiplayerModel();
		
		_hostAddressLineEdit.TextChanged += OnHostAddressChanged;
		_hostButton.Pressed += OnHostButtonPressed;
		_joinButton.Pressed += OnJoinButtonPressed;
		_startGameButton.Pressed += OnStartGameButtonPressed;
		_multiplayerModel.Changed += UpdateStateFromMultiplayerModel;
		
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
	}

	private void OnHostButtonPressed()
	{
		_hostAddressLineEdit.Editable = false;
		_connectionButtonsContainer.Visible = false;
		
		string[] hostAddressParts = _hostAddressLineEdit.Text.Split(':');
		string hostAddress = hostAddressParts[0];
		if (!int.TryParse(hostAddressParts[1], out int parsedPort)) {
			GD.PrintErr($"Could not parse port from host address {_hostAddressLineEdit.Text}");
			return;
		}
		EventBus.RaiseEvent<IHostAttemptHandler>(h => h?.HandleHostAttempt(hostAddress, parsedPort));
	}

	private void OnJoinButtonPressed()
	{
		_hostAddressLineEdit.Editable = false;
		_connectionButtonsContainer.Visible = false;
		
		string[] hostAddressParts = _hostAddressLineEdit.Text.Split(':');
		string hostAddress = hostAddressParts[0];
		if (!int.TryParse(hostAddressParts[1], out int parsedPort)) {
			GD.PrintErr($"Could not parse port from host address {_hostAddressLineEdit.Text}");
			return;
		}
		EventBus.RaiseEvent<IJoinAttemptHandler>(h => h?.HandleJoinAttempt(hostAddress, parsedPort));
	}

	public override void _Process(double delta)
	{
		if (_multiplayerModel.Peer != null && !Multiplayer.IsServer()) {
			return;
		}
		List<PlayerModel> players = _multiplayerModel.Players;
		_startGameButton.Disabled = players.Count < 2 || players.Any(player => player.Status != PlayerStatus.LOBBY);
	}

	public override void _ExitTree()
	{
		_multiplayerModel.Changed -= UpdateStateFromMultiplayerModel;
		Multiplayer.PeerConnected -= OnPeerConnected;
		Multiplayer.PeerDisconnected -= OnPeerDisconnected;
	}

	private void UpdateStateFromMultiplayerModel()
	{
		// Check if the server was closed
		if (_multiplayerModel.Peer == null) {
			_hostAddressLineEdit.Clear();
			_hostAddressLineEdit.Editable = true;
			_connectionButtonsContainer.Visible = true;
			_startGameButton.Visible = false;
			_statusLabel.Visible = false;
			return;
		}
		
		_hostAddressLineEdit.Text = $"{_multiplayerModel.HostIp}:{_multiplayerModel.HostPort}";
		_hostAddressLineEdit.Editable = false;
		_startGameButton.Visible = Multiplayer.IsServer();
		_startGameButton.Disabled = true;
		_connectionButtonsContainer.Visible = _multiplayerModel.HostIp == null;

		if (!Multiplayer.IsServer()) {
			ShowStatus("Connected!\nWait for the host to start the game", new Color("#00c100"));
		}
	}

	private void OnStartGameButtonPressed()
	{
		EventBus.RaiseEvent<IGameStartAttemptHandler>(h => h?.HandleGameStartAttempt());
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
		
		ShowStatus("Connected!\nWait for the host to start the game", new Color("#00c100"));
	}

	private void OnConnectionFailed()
	{
		Multiplayer.ConnectionFailed -= OnConnectionFailed;
		
		_hostAddressLineEdit.Editable = true;
		_connectionButtonsContainer.Visible = true;
		ShowStatus("Connection failed", new Color("#db0031"));
	}

	private void OnHostAddressChanged(string newtext)
	{
		_hostButton.Disabled = newtext.Length == 0;
		_joinButton.Disabled = newtext.Length == 0;
	}

	private void ShowStatus(string text, Color color)
	{
		_statusLabel.Text = text;
		_statusLabel.AddThemeColorOverride("font_color", color);
		_statusLabel.Visible = true;
	}

	private string GetLocalAddress()
	{
		string hostName = Dns.GetHostName();
		IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
		// Print address list
		GD.Print(string.Join(", ", hostEntry.AddressList.Select(ip => ip.ToString())));
		List<string> localAddresses = hostEntry.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
		                                       .Select(ip => ip.ToString())
		                                       .ToList();
		return localAddresses.First();
	}

	private int GetAvailablePort()
	{
		using Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Bind(new IPEndPoint(IPAddress.Any, 0));
		if (socket.LocalEndPoint is not IPEndPoint ipEndPoint) {
			throw new System.Exception("Could not get local IP address");
		}
		return ipEndPoint.Port;
	}
}
