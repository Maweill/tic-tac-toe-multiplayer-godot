using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Godot;
using TicTacToeMultiplayer.scripts.event_bus_system;
using TicTacToeMultiplayer.scripts.events;

namespace TicTacToeMultiplayer.scenes.ui.multiplayer_configuration_menu;

public partial class MultiplayerConfigurationMenu : Control, IServerCreatedHandler
{
	[Export]
	private LineEdit _hostAddressLineEdit = null!;
	[Export]
	private Button _connectButton = null!;
	[Export]
	private Button _startGameButton = null!;
	[Export]
	private Label _statusLabel = null!;

	public void HandleServerCreated(string hostIp, int port)
	{
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
		
		_hostAddressLineEdit.Text = $"{hostIp}:{port}";
		_hostAddressLineEdit.Editable = false;
		_startGameButton.Visible = true;
		_startGameButton.Disabled = true;
	}

	public override void _Ready()
	{
		_hostAddressLineEdit.TextChanged += OnHostAddressChanged;
		_connectButton.Pressed += OnConnectButtonPressed;
		_startGameButton.Pressed += OnStartGameButtonPressed;
		EventBus.Subscribe(this);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe(this);
	}

	private void OnStartGameButtonPressed()
	{
		EventBus.RaiseEvent<IGameStartHandler>(h => h?.HandleGameStart());
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
		
		ShowStatus("Connected!\nWait for the host to start the game", new Color(0, 1, 0));
	}

	private void OnConnectionFailed()
	{
		Multiplayer.ConnectionFailed -= OnConnectionFailed;
		
		_hostAddressLineEdit.Editable = true;
		_connectButton.Disabled = false;
		ShowStatus("Connection failed", new Color(1, 0, 0));
	}

	private void OnHostAddressChanged(string newtext)
	{
		_connectButton.Text = newtext.Length > 0 ? "Join" : "Host";
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
		List<string> localAddresses = hostEntry.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
		                                       .Select(ip => ip.ToString())
		                                       .ToList();
		return localAddresses.First();
	}
	
	public static int GetAvailablePort()
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
