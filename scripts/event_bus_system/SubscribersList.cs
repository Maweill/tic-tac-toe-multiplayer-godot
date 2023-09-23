using System.Collections.Generic;

namespace Tetramino.scripts.event_bus_system;

internal class SubscribersList<TSubscriber> where TSubscriber : class
{
	public readonly List<TSubscriber?> List = new();
	public bool Executing;

	private bool _needsCleanUp;

	public void Add(TSubscriber subscriber)
	{
		List.Add(subscriber);
	}

	public void Remove(TSubscriber subscriber)
	{
		if (Executing) {
			int i = List.IndexOf(subscriber);
			if (i >= 0) {
				_needsCleanUp = true;
				List[i] = null;
			}
		} else {
			List.Remove(subscriber);
		}
	}

	public void Cleanup()
	{
		if (!_needsCleanUp) {
			return;
		}

		List.RemoveAll(s => s == null);
		_needsCleanUp = false;
	}
}
