using UnityEngine;

namespace Ilep.Console
{
	[CreateAssetMenu(fileName = "HelpCommand", menuName = "ScriptableObjects/Ilep/ConsoleCommands/New HelpCommand")]
	public class HelpCommand : ConsoleCommand
	{
		public override string CmdName => "help";

		public override string Description => "Print command information";

		public override string Help => "help [command_name]";

		public override void Execute(string[] args)
		{
			if (args.Length <= 0)
				foreach (ConsoleCommand cmd in ConsoleManager.Instance.Commands)
					ConsoleManager.Instance.PrintLine(cmd.CmdName);
			else
			{
				foreach (ConsoleCommand cmd in ConsoleManager.Instance.Commands)
				{
					if (cmd.CmdName.Equals(args[0]))
					{
						ConsoleManager.Instance.PrintLine(cmd.Help);
						ConsoleManager.Instance.PrintLine(cmd.Description);
						break;
					}
				}
			}
		}
	}
}