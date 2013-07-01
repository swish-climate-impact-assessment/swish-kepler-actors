using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Swish
{
	public class TypeFunctions
	{
		private static List<Assembly> _assemblies;

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
			SortedList<string, InterfaceType> types = new SortedList<string, InterfaceType>();
			foreach (Assembly assembly in Assemblies)
			{
				SortedList<string, InterfaceType> subTypes = FindTypesOfBaseType<InterfaceType>(assembly);
				foreach (var type in subTypes)
				{
					if (types.ContainsKey(type.Key))
					{
						continue;
					}

					types.Add(type.Key, type.Value);
				}
			}
			return new List<InterfaceType>(types.Values);
		}

		public static SortedList<string, BaseType> FindTypesOfBaseType<BaseType>(Assembly assembly)
		{
			SortedList<string, BaseType> results = new SortedList<string, BaseType>();
			Type[] types;
			try
			{
				types = assembly.GetExportedTypes();
			} catch (Exception error)
			{
				SwishFunctions.MessageTextBox(ExceptionFunctions.Write(error, false) + Environment.NewLine + "could not get types of \"" + assembly.FullName + "\"", false);

				return results;
			}
			foreach (Type type in types)
			{
				if (!TypeIsFormOf(type, typeof(BaseType)) || type == typeof(BaseType) || results.ContainsKey(type.AssemblyQualifiedName) || !TypeHasDefaultConstructor(type))
				{
					continue;
				}

				BaseType item = (BaseType)Activator.CreateInstance(type);
				results.Add(type.AssemblyQualifiedName, item);
			}

			return results;
		}

		public static List<Assembly> Assemblies
		{
			get
			{
				if (_assemblies == null)
				{
					_assemblies = new List<Assembly>();
					Add(AppDomain.CurrentDomain.GetAssemblies());
					Add(DirectoryAssemblies(Application.StartupPath));
				}
				return _assemblies;
			}
		}

		private static void Add(IList<Assembly> assemblies)
		{
			for (int assemblyIndex = 0; assemblyIndex < assemblies.Count; assemblyIndex++)
			{
				Add(assemblies[assemblyIndex]);
			}
		}

		private static void Add(Assembly assembly)
		{
			if (!AssemblyUsable(assembly))
			{
				//continue;
			}

			if (!string.IsNullOrEmpty(assembly.Location) && Path.GetDirectoryName(assembly.Location) == Application.StartupPath)
			{
				int index = SimilarAssembly(assembly);
				if (index >= 0)
				{
					Assemblies.RemoveAt(index);
				}
			}

			ListFunctions.TryAddSorted(assembly, Assemblies, Compare);
		}

		private static int SimilarAssembly(Assembly assembly)
		{
			for (int index = 0; index < Assemblies.Count; index++)
			{
				if (Similar(assembly, Assemblies[index]))
				{
					return index;
				}
			}
			return -1;
		}

		private static List<Assembly> DirectoryAssemblies(string directory)
		{
			if (directory == null || !Directory.Exists(directory))
			{
				throw new ArgumentException("directory");
			}
			List<string> librarieFileNames = new List<string>();
			librarieFileNames.AddRange(Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly));
			librarieFileNames.AddRange(Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly));

			List<Assembly> assemblies = new List<Assembly>();
			foreach (string fileName in librarieFileNames)
			{
				Assembly assembly;
				if (!TryGetAssembly(fileName, out assembly))
				{
					continue;
				}
				assemblies.Add(assembly);
			}
			return assemblies;
		}

		public static bool AssemblyUsable(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			//Hack
			Type type = assembly.GetType();
			if (EqualFunctions.Equal(type, typeof(System.Reflection.Emit.AssemblyBuilder)))
			{
				return false;
			}

			return true;
		}

		private static List<string> AssemblyDirectories()
		{
			List<Assembly> assemblies = new List<Assembly>(){
				Assembly.GetAssembly(typeof(TypeFunctions)),
				Assembly.GetEntryAssembly(),
				Assembly.GetExecutingAssembly(),
				Assembly.GetCallingAssembly(),
			};

			List<string> directories = new List<string>();
			directories.Add(Path.GetFullPath(Application.StartupPath));
			for (int index = 0; index < assemblies.Count; index++)
			{
				Assembly assembly = assemblies[index];
				if (assembly == null || !AssemblyUsable(assembly))
				{
					continue;
				}
				string directory;
				try
				{
					string path = assembly.Location;
					directory = Path.GetFullPath(Path.GetDirectoryName(path));
				} catch { continue; }

				if (directories.Contains(directory))
				{
					continue;
				}

				directories.Add(directory);
			}

			return directories;
		}

		public static bool TryGetAssembly(string assemblyName, out Assembly assembly)
		{
			if (assemblyName == null || assemblyName == string.Empty)
			{
				throw new ArgumentNullException("assemblyName");
			}

			List<string> directories = AssemblyDirectories();

			List<string> paths = new List<string>();
			paths.Add(assemblyName);
			paths.Add(Path.GetFullPath(assemblyName));

			paths.Add(assemblyName + ".dll");
			paths.Add(Path.GetFullPath(assemblyName) + ".dll");
			paths.Add(assemblyName + ".exe");
			paths.Add(Path.GetFullPath(assemblyName) + ".exe");

			for (int directoryIndex = 0; directoryIndex < directories.Count; directoryIndex++)
			{
				string directory = directories[directoryIndex];
				paths.Add(Path.Combine(directory, assemblyName));
				paths.Add(Path.Combine(directory, assemblyName) + ".dll");
				paths.Add(Path.Combine(directory, assemblyName) + ".exe");
			}

			for (int index = 0; index < paths.Count; index++)
			{
				string fileName = paths[index];

				if (TryLoadAssembly(fileName, out assembly))
				{
					Add(assembly);
					return true;
				}
			}

			assembly = null;
			return false;
		}

		public static bool TryLoadAssembly(string fileName, out Assembly assembly)
		{
			if (fileName == null || fileName == string.Empty)
			{
				throw new ArgumentNullException("fileName");
			}

			if (File.Exists(fileName))
			{
				// if the file exists try file load first
				try
				{
					assembly = Assembly.LoadFile(fileName);
					return true;
				} catch { }
				try
				{
#pragma warning disable 618
					assembly = Assembly.LoadWithPartialName(fileName);
#pragma warning restore 618
					return true;
				} catch { }
			} else
			{
				// otherwise try loading try name load first
				try
				{
#pragma warning disable 618
					assembly = Assembly.LoadWithPartialName(fileName);
#pragma warning restore 618
					return true;
				} catch { }
				try
				{
					assembly = Assembly.LoadFile(fileName);
					return true;
				} catch { }
			}

			// try the other loads
			try
			{
				assembly = Assembly.Load(fileName);
				return true;
			} catch { }
			try
			{
				assembly = Assembly.LoadFrom(fileName);
				return true;
			} catch { }
#if !MONO
			try
			{
				assembly = Assembly.UnsafeLoadFrom(fileName);
				return true;
			} catch { }
#endif
			try
			{
				assembly = Assembly.ReflectionOnlyLoad(fileName);
				return true;
			} catch { }
			try
			{
				assembly = Assembly.ReflectionOnlyLoadFrom(fileName);
				return true;
			} catch { }

			assembly = null;
			return false;
		}

		public static int Compare(Assembly left, Assembly right)
		{
			// "System.Reflection.Emit.InternalAssemblyBuilder"
			Type leftType = left.GetType();
			Type rightType = right.GetType();

			if (!EqualFunctions.Equal(leftType, rightType))
			{
				return string.Compare(leftType.AssemblyQualifiedName, rightType.AssemblyQualifiedName);
			}

			int result;
			result = string.Compare(left.Location, right.Location);
			if (result != 0)
			{
				return result;
			}

			result = string.Compare(left.FullName, right.FullName);
			if (result != 0)
			{
				return result;
			}

			result = string.Compare(left.CodeBase, right.CodeBase);
			if (result != 0)
			{
				return result;
			}

			result = string.Compare(left.EscapedCodeBase, right.EscapedCodeBase);
			if (result != 0)
			{
				return result;
			}

			return 0;
		}

		public static bool Similar(Assembly left, Assembly right)
		{
			// "System.Reflection.Emit.InternalAssemblyBuilder"
			Type leftType = left.GetType();
			Type rightType = right.GetType();

			if (!EqualFunctions.Equal(leftType, rightType))
			{
				return false;
			}

			return false
				|| (!string.IsNullOrEmpty(left.Location) && left.Location == right.Location)
				|| (!string.IsNullOrEmpty(left.FullName) && left.FullName == right.FullName)
				|| (!string.IsNullOrEmpty(left.CodeBase) && left.CodeBase == right.CodeBase)
				|| (!string.IsNullOrEmpty(left.EscapedCodeBase) && left.EscapedCodeBase == right.EscapedCodeBase)
			;
		}



	}
}
