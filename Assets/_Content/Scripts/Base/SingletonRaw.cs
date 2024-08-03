using System;

namespace Base
{
	public abstract class SingletonRaw<T>
	{
		private static T _instance;

		private static readonly object _lock = new object();

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = (T) Activator.CreateInstance(typeof(T));
						}
					}
				}

				return _instance;
			}
		}
	}
}