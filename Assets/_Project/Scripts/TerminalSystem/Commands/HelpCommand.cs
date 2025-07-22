using UnityEngine;

namespace TerminalSystem.Commands
{
	[CreateAssetMenu(menuName = "TerminalSystem/Commands/Help")]
	public class HelpCommand : BaseCommand
	{
		public override ReturnValue Execute(params string[] args)
		{
			if (Parent.RegisteredCommands.Count <= 0)
			{
				Parent.AppendLine("No commands have been registered.");
			}
			else
			{
				Parent.AppendLine("Registered Commands:");
		            
				foreach (var keyValuePair in Parent.RegisteredCommands)
				{
					Parent.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value.description}");
				}
		            
				Parent.AppendLine();
			}

			return base.Execute(args);
		}
	}
}
