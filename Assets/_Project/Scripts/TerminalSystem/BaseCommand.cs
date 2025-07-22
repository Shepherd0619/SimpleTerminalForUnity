using TerminalSystem.Interfaces;
using UnityEngine;

namespace TerminalSystem
{
	public class BaseCommand : ScriptableObject, ICommand
	{
		public string commandName;
		public string description;
		
		public SimpleTerminal Parent => _parent;
		private SimpleTerminal _parent;
		
		public virtual ReturnValue Execute(params string[] args)
		{
			var returnValue = new ReturnValue();
			returnValue.Success = true;
			
			return returnValue;
		}

		public virtual void Initialize(SimpleTerminal parent)
		{
			_parent = parent;
		}
	}
}
