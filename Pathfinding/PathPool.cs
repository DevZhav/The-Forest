﻿using System;
using System.Collections.Generic;

namespace Pathfinding
{
	
	public static class PathPool
	{
		
		public static void Pool(Path path)
		{
			object obj = PathPool.pool;
			lock (obj)
			{
				if (path.pooled)
				{
					throw new ArgumentException("The path is already pooled.");
				}
				Stack<Path> stack;
				if (!PathPool.pool.TryGetValue(path.GetType(), out stack))
				{
					stack = new Stack<Path>();
					PathPool.pool[path.GetType()] = stack;
				}
				path.pooled = true;
				path.OnEnterPool();
				stack.Push(path);
			}
		}

		
		public static int GetTotalCreated(Type type)
		{
			int result;
			if (PathPool.totalCreated.TryGetValue(type, out result))
			{
				return result;
			}
			return 0;
		}

		
		public static int GetSize(Type type)
		{
			Stack<Path> stack;
			if (PathPool.pool.TryGetValue(type, out stack))
			{
				return stack.Count;
			}
			return 0;
		}

		
		public static T GetPath<T>() where T : Path, new()
		{
			object obj = PathPool.pool;
			T result;
			lock (obj)
			{
				Stack<Path> stack;
				T t;
				if (PathPool.pool.TryGetValue(typeof(T), out stack) && stack.Count > 0)
				{
					t = (stack.Pop() as T);
				}
				else
				{
					t = Activator.CreateInstance<T>();
					if (!PathPool.totalCreated.ContainsKey(typeof(T)))
					{
						PathPool.totalCreated[typeof(T)] = 0;
					}
					Dictionary<Type, int> dictionary;
					(dictionary = PathPool.totalCreated)[typeof(T)] = dictionary[typeof(T)] + 1;
				}
				t.pooled = false;
				t.Reset();
				result = t;
			}
			return result;
		}

		
		private static readonly Dictionary<Type, Stack<Path>> pool = new Dictionary<Type, Stack<Path>>();

		
		private static readonly Dictionary<Type, int> totalCreated = new Dictionary<Type, int>();
	}
}
