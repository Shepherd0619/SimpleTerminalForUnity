using UnityEngine;

namespace TerminalSystem.Commands
{
	[CreateAssetMenu(menuName = "TerminalSystem/Commands/Clear")]
	public class ClearCommand : BaseCommand
	{
		public override ReturnValue Execute(params string[] args)
		{
			Parent.InitializeTerminal();
			return base.Execute(args);
		}
	}
}
