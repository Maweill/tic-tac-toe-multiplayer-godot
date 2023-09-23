using System;
using System.Collections.Generic;
using Godot;
using Tetramino.scripts.event_bus_system;

namespace TicTacToeMultiplayer.scripts.event_bus_system;

public static class EventBus
{
	private static readonly Dictionary<Type, SubscribersList<IGlobalSubscriber>> _subscribers = new();

	public static void Subscribe(IGlobalSubscriber subscriber)
	{
		List<Type> subscriberTypes = EventBusHelper.GetSubscriberTypes(subscriber);
		foreach (Type t in subscriberTypes) {
			_subscribers.TryAdd(t, new SubscribersList<IGlobalSubscriber>());
			_subscribers[t].Add(subscriber);
		}
	}

	public static void Unsubscribe(IGlobalSubscriber subscriber)
	{
		List<Type> subscriberTypes = EventBusHelper.GetSubscriberTypes(subscriber);
		foreach (Type t in subscriberTypes) {
			if (_subscribers.TryGetValue(t, out SubscribersList<IGlobalSubscriber>? existingSubscriber)) {
				existingSubscriber.Remove(subscriber);
			}
		}
	}

	public static void RaiseEvent<TSubscriber>(Action<TSubscriber?> action) where TSubscriber : class, IGlobalSubscriber
	{
		if (!_subscribers.TryGetValue(typeof(TSubscriber), out SubscribersList<IGlobalSubscriber>? subscribers)) {
			GD.Print($"No subscribers for {typeof(TSubscriber).Name} event");
			return;
		}

		subscribers.Executing = true;
		foreach (IGlobalSubscriber? subscriber in subscribers.List) {
			action.Invoke(subscriber as TSubscriber);
		}
		subscribers.Executing = false;
		subscribers.Cleanup();
	}
}
