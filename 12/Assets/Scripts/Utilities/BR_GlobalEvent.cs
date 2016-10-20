/**********************************************************************
 * BR_GLOBALEVENT.CS
 * 
 * Description: This class will allow to send generic events to and from
 *              any class with a generic listener. Event should have
 *              0-3 Arguments.
 * 
***********************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void BR_GlobalCallback();
public delegate void BR_GlobalCallback<T>(T arg1);
public delegate void BR_GlobalCallback<T, U>(T arg1, U arg2);
public delegate void BR_GlobalCallback<T, U, V>(T arg1, U arg2, V arg3);

public delegate R BR_GlobalCallbackReturn<R>();
public delegate R BR_GlobalCallbackReturn<T, R>(T arg1);
public delegate R BR_GlobalCallbackReturn<T, U, R>(T arg1, U arg2);
public delegate R BR_GlobalCallbackReturn<T, U, V, R>(T arg1, U arg2, V arg3);

public enum BR_GlobalEventMode
{
	DONT_REQUIRE_LISTENER,
	REQUIRE_LISTENER
}

static internal class BR_GlobalEventInternal
{
	public static Hashtable Callbacks = new Hashtable ();

	public static UnregisterException ShowUnregisterException(string name)
	{
		return new UnregisterException (string.Format ("Attempting to Unregister the Event {0}", name));
	}

	public static SendException ShowSendException(string name)
	{
		return new SendException(string.Format ("Attempting to send the Event {0}", name));
	}

	public class UnregisterException : Exception { public UnregisterException(string msg) : base(msg){}}
	public class SendException : Exception { public SendException(string msg) : base(msg){} }
}

/// <summary>
/// No Arguments
/// </summary>
public static class BR_GlobalEvent
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;

	public static void Register(string name, BR_GlobalCallback callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallback> callbacks = (List<BR_GlobalCallback>)m_Callbacks [name];
		if (callback == null) 
		{
			callbacks = new List<BR_GlobalCallback>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}

	public static void Unregister(string name, BR_GlobalCallback callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallback> callbacks = (List<BR_GlobalCallback>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}

	public static void Send(string name)
	{
		Send (name, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	public static void Send(string name, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");

		List<BR_GlobalCallback> callbacks = (List<BR_GlobalCallback>)m_Callbacks [name];
		if(callbacks != null)
			foreach(BR_GlobalCallback c in callbacks)
				c();
		else if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
			throw BR_GlobalEventInternal.ShowSendException(name);
	}
}

/// <summary>
/// One Argument
/// </summary>
public static class BR_GlobalEvent<T>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;

	public static void Register(string name, BR_GlobalCallback<T> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");

		List<BR_GlobalCallback<T>> callbacks = (List<BR_GlobalCallback<T>>)m_Callbacks [name];
		if (callback == null) 
		{
			callbacks = new List<BR_GlobalCallback<T>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}
	
	public static void Unregister(string name, BR_GlobalCallback<T> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallback<T>> callbacks = (List<BR_GlobalCallback<T>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}
	
	public static void Send(string name, T arg1)
	{
		Send (name, arg1, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}
	
	public static void Send(string name, T arg1, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");
		if (arg1 == null)
			throw new ArgumentNullException ("arg1");
		List<BR_GlobalCallback<T>> callbacks = (List<BR_GlobalCallback<T>>)m_Callbacks [name];
		if(callbacks != null)
			foreach(BR_GlobalCallback<T> c in callbacks)
				c(arg1);
		else if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
			throw BR_GlobalEventInternal.ShowSendException(name);
	}
}

/// <summary>
/// Two Arguments
/// </summary>
public static class BR_GlobalEvent<T, U>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;
	
	public static void Register(string name, BR_GlobalCallback<T, U> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if (callback == null)
			throw new ArgumentNullException ("callback");
		List<BR_GlobalCallback<T, U>> callbacks = (List<BR_GlobalCallback<T, U>>)m_Callbacks [name];
		if (callbacks == null) 
		{
			callbacks = new List<BR_GlobalCallback<T, U>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}

	public static void Unregister(string name, BR_GlobalCallback<T,U> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if (callback == null)
			throw new ArgumentNullException ("callback");

		List<BR_GlobalCallback<T,U>> callbacks = (List<BR_GlobalCallback<T,U>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else
			throw BR_GlobalEventInternal.ShowUnregisterException (name);
	}
	
	public static void Send(string name, T arg1, U arg2)
	{
		Send (name, arg1, arg2, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	public static void Send(string name, T arg1, U arg2, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if (arg1 == null)
			throw new ArgumentNullException ("arg1");
		if (arg2 == null)
			throw new ArgumentNullException ("arg2");

		List<BR_GlobalCallback<T,U>> callbacks = (List<BR_GlobalCallback<T,U>>)m_Callbacks [name];
		if (callbacks != null) {
			foreach (BR_GlobalCallback<T,U> c in callbacks)
				c (arg1, arg2);
		} else if (mode == BR_GlobalEventMode.REQUIRE_LISTENER)
			throw BR_GlobalEventInternal.ShowSendException (name);
	}
}


public static class BR_GlobalEvent<T, U, V>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;

	public static void Register(string name, BR_GlobalCallback<T, U, V> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallback<T,U,V>> callbacks = (List<BR_GlobalCallback<T,U,V>>)m_Callbacks [name];
		if (callback == null) 
		{
			callbacks = new List<BR_GlobalCallback<T,U,V>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}
	
	public static void Unregister(string name, BR_GlobalCallback<T, U, V> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallback<T,U,V>> callbacks = (List<BR_GlobalCallback<T,U,V>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}
	
	public static void Send(string name, T arg1, U arg2, V arg3)
	{
		Send (name, arg1, arg2, arg3, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}
	
	public static void Send(string name, T arg1, U arg2, V arg3, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");
		if (arg1 == null)
			throw new ArgumentNullException ("arg1");
		if (arg2 == null)
			throw new ArgumentNullException ("arg2");
		if (arg3 == null)
			throw new ArgumentNullException ("arg3");

		List<BR_GlobalCallback<T,U,V>> callbacks = (List<BR_GlobalCallback<T,U,V>>)m_Callbacks [name];
		if (callbacks != null)
			foreach (BR_GlobalCallback<T,U,V> c in callbacks)
				c (arg1, arg2, arg3);
		else if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
			throw BR_GlobalEventInternal.ShowSendException(name);
	}
}

public static class BR_GlobalEventReturn<R>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;

	public static void Register(string name, BR_GlobalCallbackReturn<R> callback)
	{
		if(string.IsNullOrEmpty(name))
		   throw new ArgumentNullException(@"name");
		if (callback == null)
			throw new ArgumentNullException ("callback");
		List<BR_GlobalCallbackReturn<R>> callbacks = (List<BR_GlobalCallbackReturn<R>>)m_Callbacks [name];
		if (callbacks == null) 
		{
			callbacks = new List<BR_GlobalCallbackReturn<R>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}

	public static void Unregister(string name, BR_GlobalCallbackReturn<R> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallbackReturn<R>> callbacks = (List<BR_GlobalCallbackReturn<R>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}
	
	public static R Send(string name)
	{
		return Send (name, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}
	
	public static R Send(string name, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");
		
		List<BR_GlobalCallbackReturn<R>> callbacks = (List<BR_GlobalCallbackReturn<R>>)m_Callbacks [name];
		if (callbacks != null) {
			R val = default(R);
			foreach (BR_GlobalCallbackReturn<R> c in callbacks)
				val = c ();
			return val;
		} 
		else 
		{
			if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
				throw BR_GlobalEventInternal.ShowSendException(name);
			return default(R);
		}
	}
}

public static class BR_GlobalEventReturn<T, R>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;
	
	public static void Register(string name, BR_GlobalCallbackReturn<T, R> callback)
	{
		if(string.IsNullOrEmpty(name))
			throw new ArgumentNullException(@"name");
		if (callback == null)
			throw new ArgumentNullException ("callback");
		List<BR_GlobalCallbackReturn<T, R>> callbacks = (List<BR_GlobalCallbackReturn<T, R>>)m_Callbacks [name];
		if (callbacks == null) 
		{
			callbacks = new List<BR_GlobalCallbackReturn<T, R>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}
	
	public static void Unregister(string name, BR_GlobalCallbackReturn<T, R> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallbackReturn<T, R>> callbacks = (List<BR_GlobalCallbackReturn<T, R>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}
	
	public static R Send(string name, T arg1)
	{
		return Send (name, arg1, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}
	
	public static R Send(string name, T arg1, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");
		if (arg1 == null)
			throw new ArgumentNullException ("arg1");

		List<BR_GlobalCallbackReturn<T, R>> callbacks = (List<BR_GlobalCallbackReturn<T, R>>)m_Callbacks [name];
		if (callbacks != null) {
			R val = default(R);
			foreach (BR_GlobalCallbackReturn<T, R> c in callbacks)
				val = c (arg1);
			return val;
		} 
		else 
		{
			if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
				throw BR_GlobalEventInternal.ShowSendException(name);
			return default(R);
		}
	}
}

public static class BR_GlobalEventReturn<T, U, R>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;
	
	public static void Register(string name, BR_GlobalCallbackReturn<T, U, R> callback)
	{
		if(string.IsNullOrEmpty(name))
			throw new ArgumentNullException(@"name");
		if (callback == null)
			throw new ArgumentNullException ("callback");
		List<BR_GlobalCallbackReturn<T, U, R>> callbacks = (List<BR_GlobalCallbackReturn<T, U, R>>)m_Callbacks [name];
		if (callbacks == null) 
		{
			callbacks = new List<BR_GlobalCallbackReturn<T, U, R>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}
	
	public static void Unregister(string name, BR_GlobalCallbackReturn<T, U, R> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallbackReturn<T, U, R>> callbacks = (List<BR_GlobalCallbackReturn<T, U, R>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}
	
	public static R Send(string name, T arg1, U arg2)
	{
		return Send (name, arg1, arg2, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}
	
	public static R Send(string name, T arg1, U arg2, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");
		if (arg1 == null)
			throw new ArgumentNullException ("arg1");
		if (arg2 == null)
			throw new ArgumentNullException ("arg2");

		List<BR_GlobalCallbackReturn<T, U, R>> callbacks = (List<BR_GlobalCallbackReturn<T, U, R>>)m_Callbacks [name];
		if (callbacks != null) {
			R val = default(R);
			foreach (BR_GlobalCallbackReturn<T, U, R> c in callbacks)
				val = c (arg1, arg2);
			return val;
		} 
		else 
		{
			if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
				throw BR_GlobalEventInternal.ShowSendException(name);
			return default(R);
		}
	}
}

public static class BR_GlobalEventReturn<T, U, V, R>
{
	private static Hashtable m_Callbacks = BR_GlobalEventInternal.Callbacks;
	
	public static void Register(string name, BR_GlobalCallbackReturn<T, U, V, R> callback)
	{
		if(string.IsNullOrEmpty(name))
			throw new ArgumentNullException(@"name");
		if (callback == null)
			throw new ArgumentNullException ("callback");
		List<BR_GlobalCallbackReturn<T, U, V, R>> callbacks = (List<BR_GlobalCallbackReturn<T, U, V, R>>)m_Callbacks [name];
		if (callbacks == null) 
		{
			callbacks = new List<BR_GlobalCallbackReturn<T, U, V, R>>();
			m_Callbacks.Add (name, callbacks);
		}
		callbacks.Add (callback);
	}
	
	public static void Unregister(string name, BR_GlobalCallbackReturn<T, U, V, R> callback)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException (@"name");
		if(callback == null)
			throw new ArgumentNullException("callback");
		List<BR_GlobalCallbackReturn<T, U, V, R>> callbacks = (List<BR_GlobalCallbackReturn<T, U, V, R>>)m_Callbacks [name];
		if (callbacks != null)
			callbacks.Remove (callback);
		else 
			throw BR_GlobalEventInternal.ShowUnregisterException(name);
	}
	
	public static R Send(string name, T arg1, U arg2, V arg3)
	{
		return Send (name, arg1, arg2, arg3, BR_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}
	
	public static R Send(string name, T arg1, U arg2, V arg3, BR_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty (name))
			throw new ArgumentNullException(@"name");
		if (arg1 == null)
			throw new ArgumentNullException ("arg1");
		if (arg2 == null)
			throw new ArgumentNullException ("arg2");
		if (arg3 == null)
			throw new ArgumentNullException ("arg3");
		
		List<BR_GlobalCallbackReturn<T, U, V, R>> callbacks = (List<BR_GlobalCallbackReturn<T, U, V, R>>)m_Callbacks [name];
		if (callbacks != null) {
			R val = default(R);
			foreach (BR_GlobalCallbackReturn<T, U, V, R> c in callbacks)
				val = c (arg1, arg2, arg3);
			return val;
		} 
		else 
		{
			if(mode == BR_GlobalEventMode.REQUIRE_LISTENER)
				throw BR_GlobalEventInternal.ShowSendException(name);
			return default(R);
		}
	}
}
