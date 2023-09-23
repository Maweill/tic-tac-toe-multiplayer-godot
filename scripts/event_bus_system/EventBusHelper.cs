using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetramino.scripts.event_bus_system;

internal static class EventBusHelper
{
	private static readonly Dictionary<Type, List<Type>> _cashedSubscriberTypes = new();

	public static List<Type> GetSubscriberTypes(IGlobalSubscriber globalSubscriber)
	{
		Type type = globalSubscriber.GetType();
		if (_cashedSubscriberTypes.TryGetValue(type, out List<Type>? cashedSubscriberType)) {
			return cashedSubscriberType;
		}

		List<Type> subscriberTypes = type.GetInterfaces().Where(t => t.GetInterfaces().Contains(typeof(IGlobalSubscriber))).ToList();

		_cashedSubscriberTypes[type] = subscriberTypes;
		return subscriberTypes;
	}
}
