using System;
using System.IO;
using System.Reflection;

namespace Swish
{
	public class TypeFunctions
	{
		public static bool TypeIsFormOf(Type type, Type requiredType)
		{
			bool flag;
			bool flag1 = !EqualFunctions.Equal(type, null);
			if (flag1)
			{
				flag1 = !EqualFunctions.Equal(requiredType, null);
				if (flag1)
				{
					while (true)
					{
						flag1 = !EqualFunctions.Equal(type, null);
						if (flag1)
						{
							flag1 = !EqualFunctions.Equal(type, requiredType);
							if (flag1)
							{
								flag1 = EqualFunctions.Equal(type.GetInterface(requiredType.FullName), null);
								if (flag1)
								{
									flag1 = !requiredType.IsAssignableFrom(type);
									if (flag1)
									{
										type = type.BaseType;
									} else
									{
										flag = true;
										break;
									}
								} else
								{
									flag = true;
									break;
								}
							} else
							{
								flag = true;
								break;
							}
						} else
						{
							flag = false;
							break;
						}
					}
					return flag;
				} else
				{
					throw new ArgumentNullException("requiredType");
				}
			} else
			{
				throw new ArgumentNullException("type");
			}
		}

		public static bool TypeHasDefaultConstructor(Type type)
		{
			bool flag;
			bool isPrimitive = !EqualFunctions.Equal(type, null);
			if (isPrimitive)
			{
				isPrimitive = !type.IsPrimitive;
				if (isPrimitive)
				{
					isPrimitive = !type.IsInterface;
					if (isPrimitive)
					{
						ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
						flag = constructor != null;
					} else
					{
						flag = false;
					}
				} else
				{
					flag = true;
				}
				return flag;
			} else
			{
				throw new ArgumentNullException("type");
			}
		}

	}
}