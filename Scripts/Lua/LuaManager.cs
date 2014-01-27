using LuaInterface;
using System.Reflection;
namespace LuaSystem
{
	public static class LuaManager
	{
		//Reference to the Lua virtual machine
		public static Lua lua
		{
			get
			{
				if (_lua == null)
				{
					Init();
				}
				return _lua;
			}
		}
		private static Lua _lua = null;
		private static bool didAllFiles = false;
		//Filename of the Lua file to load in the Streaming Assets folder
		public static void Init()
		{
			if (_lua != null)
			{
				return;
			}
			//Init instance of Lua virtual machine (Note: Can only init ONCE)
			_lua = new LuaInterface.Lua();

			System.Type t = typeof(LuaManager);
			MethodInfo print = t.GetMethod("LuaPrint");
			lua.RegisterFunction("print", null, print);
			lua.RegisterFunction("error", null, t.GetMethod("LuaError"));
			lua.RegisterFunction("assert", null, t.GetMethod("LuaAssert",
															BindingFlags.Static | BindingFlags.Public,
															System.Type.DefaultBinder,
															new[] { typeof(bool), typeof(string) },
															null));

			//Init LuaBinding class that demonstrates communication
			//Also tell Lua about the LuaBinding object to allow Lua to call C# functions
			new LuaBinding("main");
		}

		public static void DoAllFiles()
		{
			if (didAllFiles)
			{
				return;
			}
			didAllFiles = true;
			string[] files = ModSystem.ModLoader.GetScripts();
			foreach (string file in files)
			{
				DoFile(GlobalConsts.GetDataPath() + file);
			}
		}

		public static void LuaPrint(object message)
		{
			//Output message into the debug log
			Console.Log(new LocalizedString(GlobalConsts.SKIP_LOCALIZATION, message.ToString()), true);
		}

		public static void LuaError(object message)
		{
			Console.LogError(new LocalizedString(GlobalConsts.SKIP_LOCALIZATION, message.ToString()));
		}

		public static bool LuaAssert(bool value, object message)
		{
			if (value)
			{
				return true;
			}
			else
			{
				LuaError(message);
				return false;
			}
		}

		public static void DoFile(string dataPath)
		{
			string extension = System.IO.Path.GetExtension(dataPath);
			if (extension.ToLower() != ".lua")
			{
				Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_NOT_LUA_FILE, "Cannot load %s as it is not an Lua file.", new object[] { dataPath }));
			}
			try
			{
				lua.DoFile(dataPath);
				Console.Log(new LocalizedString(GlobalConsts.ID_EXECUTED_LUA, "Executed %s.", new object[] { dataPath }), true);
			}
			catch (System.Exception e)
			{
				if (e is KopiLua.Lua.LuaException)
				{
					Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_LUA_EXCEPTION, "LuaException in %s!\n%s", new object[] { dataPath, ((LuaException)e).InnerException }));
				}
				else
				{
					Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_LOADING_FAILED, "Could not load %s!\n%s", new object[] { dataPath, e }));
				}
			}
		}

		public static bool DoString(string luaString)
		{
			try
			{
				lua.DoString(luaString);
				Console.Log(new LocalizedString(GlobalConsts.ID_EXECUTED_LUA, "Executed %s.", new object[] { luaString }), true);
				return true;
			}
			catch (System.Exception e)
			{
				Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_LOADING_FAILED, "Could not load %s!\n%s", new object[] { luaString, e }));
				return false;
			}
		}
	}
}