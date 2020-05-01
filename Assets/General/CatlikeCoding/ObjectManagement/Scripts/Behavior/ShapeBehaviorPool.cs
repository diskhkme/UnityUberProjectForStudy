using System.Collections.Generic;
using UnityEngine;

//이제 shapebehavior가 mono가 아니므로, 별도로 pool을 만들어서 관리
public static class ShapeBehaviorPool<T> where T : ShapeBehavior, new()
{
    //이번 pool은 stack으로 관리
    static Stack<T> stack = new Stack<T>();

    public static T Get()
    {
        if(stack.Count > 0)
        {
            T behavior = stack.Pop();
#if UNITY_EDITOR
            behavior.IsReclaimed = false;
#endif
            return behavior;
        }
#if UNITY_EDITOR
        return ScriptableObject.CreateInstance<T>(); //shapebehavior가 scriptable object가 되었으므로, createinstance를 사용함
#else
        return new T();
#endif
    }

    public static void Reclaim(T behavior)
    {
#if UNITY_EDITOR
        behavior.IsReclaimed = true;
#endif
        stack.Push(behavior);
    }
}
