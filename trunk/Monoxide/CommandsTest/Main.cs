namespace CommandsTest
{
	using System;
	using System.MacOS.AppKit;
	
	sealed class MainClass : CommandTarget
	{
		public static class Commands
		{
			public static readonly Command Toto = Command.Register("Toto");
			public static readonly Command Titi = Command.Register("Titi");
			public static readonly Command Tutu = Command.Register("Tutu");
		}
		
		public static void Main(string[] args)
		{
			var @this = new MainClass();
			
			@this.Execute(Commands.Toto, @this);
			@this.Execute(Commands.Titi, @this);
			@this.Execute(Commands.Tutu, @this);
		}
		
		public void Toto(object sender) { Console.WriteLine("Toto"); }
		
		public void Titi(object sender) { Console.WriteLine("Titi"); }
		
		[Command("Tutu")]
		public void Tata(object sender) { Console.WriteLine("Tata"); }
	}
}
