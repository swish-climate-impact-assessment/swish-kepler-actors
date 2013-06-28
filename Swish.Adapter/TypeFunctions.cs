using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Swish
{
	public class TypeFunctions
	{
		public static bool TypeIsFormOf(Type type, Type requiredType)
		{
			if (EqualFunctions.Equal(type, null))
			{
				throw new ArgumentNullException("type");
			}
			if (EqualFunctions.Equal(requiredType, null))
			{
				throw new ArgumentNullException("requiredType");
			}
			while (!EqualFunctions.Equal(type, null))
			{
				if (EqualFunctions.Equal(type, requiredType))
				{
					return true;
				}
				if (!EqualFunctions.Equal(type.GetInterface(requiredType.FullName), null))
				{
					return true;
				}
				if (requiredType.IsAssignableFrom(type))
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		public static bool TypeHasDefaultConstructor(Type type)
		{
			if (EqualFunctions.Equal(type, null))
			{
				throw new ArgumentNullException("type");
			}

			if (type.IsPrimitive)
			{
				return true;
			}
			if (type.IsInterface)
			{
				return false;
			}

			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			return constructor != null;
		}

		public static List<InterfaceType> Interfaces<InterfaceType>()
		{
			Type iAdapterType = typeof(InterfaceType);
			Assembly dll = iAdapterType.Assembly;
			Type[] exportedTypes = dll.GetExportedTypes();

			List<InterfaceType> adapters = new List<InterfaceType>();
			for (int typeIndex = 0; typeIndex < exportedTypes.Length; typeIndex++)
			{
				Type type = exportedTypes[typeIndex];

				if (TypeIsFormOf(type, iAdapterType) && type != iAdapterType && TypeHasDefaultConstructor(type))
				{
					InterfaceType adapter = (InterfaceType)Activator.CreateInstance(type);
					adapters.Add(adapter);
				}
			}
			return adapters;
		}

	}
}
