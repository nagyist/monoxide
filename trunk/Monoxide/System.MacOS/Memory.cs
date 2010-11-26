using System;
using System.Reflection.Emit;

namespace System.MacOS
{
	sealed class Memory
	{
		unsafe delegate void CopyMemoryDelegate(void* destination, void* source, uint length);
		unsafe delegate void SetMemoryDelegate(void* destination, byte value, uint length);
	
		static DynamicMethod copyMemoryMethod = CreateCopyMemoryMethod();
		static DynamicMethod setMemoryMethod = CreateSetMemoryMethod();
	
		static CopyMemoryDelegate copyMemoryDelegate = (CopyMemoryDelegate)copyMemoryMethod.CreateDelegate(typeof(CopyMemoryDelegate));
		static SetMemoryDelegate setMemoryDelegate = (SetMemoryDelegate)setMemoryMethod.CreateDelegate(typeof(SetMemoryDelegate));
	
		static unsafe DynamicMethod CreateCopyMemoryMethod()
		{
			var copyMemoryMethod = new DynamicMethod("Copy", typeof(void), new Type[] { typeof(void*), typeof(void*), typeof(uint) }, typeof(Memory).Module, false);
			var ilGenerator = copyMemoryMethod.GetILGenerator();

			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Ldarg_2);
			ilGenerator.Emit(OpCodes.Cpblk);
			ilGenerator.Emit(OpCodes.Ret);
	
			return copyMemoryMethod;
		}
	
		static unsafe DynamicMethod CreateSetMemoryMethod()
		{
			var setMemoryMethod = new DynamicMethod("Set", typeof(void), new Type[] { typeof(void*), typeof(byte), typeof(uint) }, typeof(Memory).Module, false);
			var ilGenerator = setMemoryMethod.GetILGenerator();
	
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Ldarg_2);
			ilGenerator.Emit(OpCodes.Initblk);
			ilGenerator.Emit(OpCodes.Ret);
	
			return setMemoryMethod;
		}
	
		public static unsafe void Copy(void* destination, void* source, int length)
		{
			copyMemoryDelegate(destination, source, checked((uint)length));
		}

		public static unsafe void Set(void* destination, byte value, int length)
		{
			setMemoryDelegate(destination, value, checked((uint)length));
		}
	}
}
