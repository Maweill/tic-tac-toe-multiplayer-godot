using System.Collections.Generic;
using System.Linq;
using System.Net;
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

	public void HandleServerCreated(string hostIp)
	{
		_hostAddressLineEdit.Text = hostIp;
		_hostAddressLineEdit.Editable = false;
		_startGameButton.Visible = true;
	}

	public override void _Ready()
	{
		_startGameButton.Visible = false;
		
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
			EventBus.RaiseEvent<IJoinAttemptHandler>(h => h?.HandleJoinAttempt(_hostAddressLineEdit.Text));
		} else {
			string localAddress = GetLocalAddress();
			EventBus.RaiseEvent<IHostAttemptHandler>(h => h?.HandleHostAttempt(localAddress));
		}
	}

	private void OnConnectedToServer()
	{
		Multiplayer.ConnectedToServer -= OnConnectedToServer;
		
		_hostAddressLineEdit.Editable = false;
		//TODO Change status to "Connected! Wait for the host to start the game."
	}

	private void OnHostAddressChanged(string newtext)
	{
		_connectButton.Text = newtext.Length > 0 ? "Join" : "Host";
	}

	private string GetLocalAddress()
	{
		string hostName = Dns.GetHostName();
		IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
		List<string> localAddresses = hostEntry.AddressList.Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
		                                       .Select(ip => ip.ToString())
		                                       .ToList();
		return localAddresses.First();
	}
}
