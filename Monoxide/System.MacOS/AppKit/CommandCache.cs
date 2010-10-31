using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace System.MacOS.AppKit
{
	internal sealed class CommandCache
	{
		private delegate void CommandInvocationDelegate(object target, object sender);
		
		#region Static Members
		
		private static Dictionary<Type, CommandCache> cacheDictionary = new Dictionary<Type, CommandCache>();

		private static Dictionary<string, CommandInvocationDelegate> BuildMethodDictionary(Type type)
		{
			var dictionary = new Dictionary<string, CommandInvocationDelegate>();
			
			foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
			{
				var parameters = method.GetParameters();
				
				if (method.ReturnType != typeof(void) ||
					parameters.Length != 1 ||
					parameters[0].ParameterType != typeof(object) ||
					parameters[0].Name != "sender")
					continue;
				
				var attributes = method.GetCustomAttributes(typeof(CommandAttribute), true) as CommandAttribute[];
				
				var commandName = string.Intern(attributes.Length > 0 ? attributes[0].CommandName : method.Name);
				
				dictionary.Add(commandName, CreateInvokeDelegate(method));
			}
			
			return dictionary;
		}
		
		private static CommandInvocationDelegate CreateInvokeDelegate(MethodInfo targetMethod)
		{
			var method = new DynamicMethod("CommandCache.Invoke", typeof(void), new [] { typeof(object), typeof(object) }, true);

			method.DefineParameter(1, ParameterAttributes.None, "target");
			method.DefineParameter(2, ParameterAttributes.None, "sender");
			
			var ilGenerator = method.GetILGenerator();
			
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Isinst, targetMethod.DeclaringType);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Tailcall);
			ilGenerator.Emit(OpCodes.Callvirt, targetMethod); // callvirt will check for null references
			ilGenerator.Emit(OpCodes.Ret);
			
			return method.CreateDelegate(typeof(CommandInvocationDelegate)) as CommandInvocationDelegate;
		}
		
		public static CommandCache Get(Type type)
		{
			CommandCache cache;
			
			if (!cacheDictionary.TryGetValue(type, out cache))
				lock (cacheDictionary)
					if (!cacheDictionary.TryGetValue(type, out cache))
						cacheDictionary.Add(type, cache = new CommandCache(type));
			
			return cache;
		}
		
		public static bool IsSupported(Type type, string commandName) { return Get(type).IsSupported(commandName);  }
		private static CommandInvocationDelegate GetMethod(Type type, string commandName) { return Get(type).GetMethod(commandName); }
		
		public static void Invoke(string commandName, object target, object sender)
		{
			GetMethod(target.GetType(), commandName)(target, sender);
		}
		
		#endregion

		private Dictionary<string, CommandInvocationDelegate> methodDictionary;
		
		private CommandCache(Type type)
		{
			methodDictionary = BuildMethodDictionary(type);
		}
		
		public bool IsSupported(string commandName) { return methodDictionary.ContainsKey(commandName); }
		private CommandInvocationDelegate GetMethod(string commandName) { return methodDictionary[commandName]; }
	}
}
