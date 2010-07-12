using System;

namespace System.MacOS
{
	internal static class Casing
	{
		public static bool IsPascalCased(string @string)
		{
			int upperCaseCount = 0;
			
			for (int i = 0; i < @string.Length; i++)
			{
				char c = @string[i];
				
				if (c >= 'A' && c <= 'Z')
					if (++upperCaseCount > 3)
						return false;
					else
						continue;
				else if (i == 0 || (c < 'a' || c > 'z') && (c < '0' || c > '9'))
					return false;
				
				upperCaseCount = 0;
			}
			
			return true;
		}
		
		public static bool IsCamelCased(string @string)
		{
			int upperCaseCount = 0;
			
			for (int i = 0; i < @string.Length; i++)
			{
				char c = @string[i];
				
				if (c >= 'A' && c <= 'Z')
					if (i == 0 || ++upperCaseCount > 3)
						return false;
					else
						continue;
				else if (((c < 'a' || c > 'z') && (c < '0' || c > '9')) || i == 0)
					return false;
				
				upperCaseCount = 0;
			}
			
			return true;
		}
	}
}
