using UnityEngine;

namespace RedOwl.Core
{
	public static class Ensure
	{
		private static readonly EnsureThat Instance = new EnsureThat();

		public static bool IsActive { get; set; }

		public static void Off() => IsActive = false;

		public static void On() => IsActive = true;

		public static EnsureThat That(string paramName)
		{
			Instance.ParamName = paramName;
			return Instance;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnRuntimeMethodLoad()
		{
			IsActive = Application.isEditor || Debug.isDebugBuild;
		}
	}
}