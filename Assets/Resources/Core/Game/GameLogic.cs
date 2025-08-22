using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameLogic
{
    private static List<IModule> Modules = new List<IModule>();
    /// <summary>
    /// 创建模块（各个模块唯一）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void CreateModule<T>() where T: class, IModule
    {
        if (Contains(typeof(T)))
            return;

        T module = Activator.CreateInstance<T>();
        module.Initial();
        Modules.Add(module);
    }
    /// <summary>
    /// 更新所有模块
    /// </summary>
    public static void Update()
    {
        foreach(IModule module in Modules)
        {
            module.Update();
        }
    }
    /// <summary>
    /// 获取模块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetModule<T>() where T: class, IModule
    {
        foreach (IModule module in Modules)
        {
            if (module.GetType() == typeof(T))
            {
                return module as T;
            }
        }
        return null;
    }
    /// <summary>
    /// 查询游戏模块存在性
    /// </summary>
    /// <param name="moduleType"></param>
    /// <returns></returns>
    public static bool Contains(System.Type moduleType)
    {
        foreach (IModule module in Modules)
        {
            if (module.GetType() == moduleType)
            {
                return true;
            }
        }
        return false;
    }
}
