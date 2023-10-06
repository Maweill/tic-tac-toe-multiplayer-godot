using Godot;
using Godot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TicTacToeMultiplayer.scripts.models;
using GameStateController = TicTacToeMultiplayer.scenes.controller.game_state_controller.GameStateController;
using MultiplayerController = TicTacToeMultiplayer.scenes.controller.multiplayer_controller.MultiplayerController;

namespace TicTacToeMultiplayer.scenes.autoload.dependency_installer;

public partial class DependencyInstaller : Node, IServicesConfigurator
{
	[Export]
	private MultiplayerController _multiplayerController = null!;
	[Export]
	private GameStateController _gameStateController = null!;
	
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton<MultiplayerModel>();
		
		services.AddSingleton(_multiplayerController);
		services.AddSingleton(_gameStateController);
		GD.Print("Binding services succeeded");
	}
}
