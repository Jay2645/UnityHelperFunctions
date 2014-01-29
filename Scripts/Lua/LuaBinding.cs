using LuaInterface;
using ObserverSystem;
using System;
using System.Collections.Generic;
namespace LuaSystem
{
	public class LuaBinding : Observer
	{
		protected LuaBinding()
		{
			/* EMPTY */
		}
		public LuaBinding(string binding)
		{
			SetBinding(binding);
		}

		protected void SetBinding(string binding)
		{
			binding = binding.ToLower();
			binding = binding.Replace(" ", "");
			LuaManager.lua[binding] = this;
			name = binding;
		}

		protected Dictionary<string, LuaFunction> functionList = new Dictionary<string, LuaFunction>();
		protected string name = "";
		public void BindMessageFunction(LuaFunction func, string name)
		{
			//Binding
			if (functionList.ContainsKey(name))
			{
				functionList[name] = func;
			}
			else
			{
				functionList.Add(name, func);
			}
		}

		public object[] MessageToLua(LuaFunction luaFunction, object[] args)
		{
			try
			{
				if (args == null)
				{
					return luaFunction.Call();
				}
				else
				{
					return luaFunction.Call(args);
				}
			}
			catch (Exception ex)
			{
				if (ex is KopiLua.Lua.LuaException)
				{
					Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_LUA_EXCEPTION, "LuaException in %s!\n%s", new object[] { name, ex }));
				}
				else
				{
					Console.LogError(new LocalizedString(GlobalConsts.ID_ERROR_GENERIC_EXCEPTION, "Exception: %s", new object[] { ex }));
				}
				return null;
			}
		}

		public override void OnNotify(UnityEngine.MonoBehaviour entity, string eventType)
		{
			CallLuaFunction(eventType);
		}

		public object[] CallLuaFunction(string funcName)
		{
			return CallLuaFunction(funcName, null);
		}

		public object[] CallLuaFunction(string funcName, object[] args)
		{
			foreach (KeyValuePair<string, LuaFunction> kvp in functionList)
			{
				if (kvp.Key.ToLower() == funcName.ToLower())
				{
					return MessageToLua(kvp.Value, args);
				}
			}
			return null;
		}
	}
}