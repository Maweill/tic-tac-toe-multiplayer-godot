using Godot;
using Godot.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TicTacToeMultiplayer.scenes.autoload.game_state_controller;
using TicTacToeMultiplayer.scenes.autoload.multiplayer_controller;
using TicTacToeMultiplayer.scripts.models;

namespace TicTacToeMultiplayer.scenes.game_object.dependency_installer;

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
