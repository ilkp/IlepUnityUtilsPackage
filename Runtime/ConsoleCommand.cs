using UnityEngine;

namespace Ilep.Console
{
	public interface IConsoleCommand
	{
		public abstract string CmdName { get; }
		public abstract string Description { get; }
		public abstract string Help { get; }
		public abstract void Execute(string[] args);
	}

	public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
	{
		public abstract string CmdName { get; }

		public abstract string Description { get; }

		public abstract string Help { get; }

		public abstract void Execute(string[] args);
	}
}