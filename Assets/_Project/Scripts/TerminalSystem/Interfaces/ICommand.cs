namespace TerminalSystem.Interfaces
{
	public interface ICommand
	{
		public ReturnValue Execute(params string[] args);
	}
}
