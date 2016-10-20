
/***************************************************************
 * BR_Timer.cs
 * 
 * Description: BR_Timer is an extension for delaying methods.
 * 	           Supports arguments, delegates, repetition with
 *             intervals, infinite repetition, pausing and
 *             canceling.
 * ************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

// Debuging Purpose
#if (UNITY_EDITOR && DEBUG)
using System.Diagnostics;
#endif


public class BR_Timer : MonoBehaviour {

	private static GameObject m_MainObject = null;

	private static List<Event> m_Active = new List<Event> ();
	private static List<Event> m_Pool = new List<Event>();

	private static Event m_NewEvent = null;
	private static int m_EventCount = 0;

	private static int m_EventBatch = 0;
	private static int m_EventIterator = 0;

	/// <summary>
	/// Maxium amount of callbacks for the timer system
	/// </summary>
	public static int MaxEventsPerFrame = 500;

	public delegate void Callback();

	public delegate void ArgCallback(object args);

	public struct Stats
	{
		public int Created;
		public int Inactive;
		public int Active;
	}

	public bool WasAddedCorrectly
	{
		get
		{
			if (!Application.isPlaying)
				return false;
			if(gameObject != m_MainObject)
				return false;
			return true;
		}
	}

	/// <summary>
	/// Destroy if it is not added Correctly
	/// </summary>
	private void Awake()
	{
		if (!WasAddedCorrectly) 
		{
			Destroy(this);
			return;
		}
	}

	/// <summary>
	/// The active list is looped every frame,
	/// executing the timer events for which time is up
	/// </summary>
	private void Update()
	{
		m_EventBatch = 0;

		while ((BR_Timer.m_Active.Count > 0) && m_EventBatch < MaxEventsPerFrame) 
		{
			if(m_EventIterator < 0)
			{
				m_EventIterator = BR_Timer.m_Active.Count - 1;
				break;
			}

			if(m_EventIterator > BR_Timer.m_Active.Count - 1)
				m_EventIterator = BR_Timer.m_Active.Count - 1;

			if(Time.time >= BR_Timer.m_Active[m_EventIterator].DueTime ||
			   BR_Timer.m_Active[m_EventIterator].Id == 0)
				BR_Timer.m_Active[m_EventIterator].Execute();
			else
			{
				if(BR_Timer.m_Active[m_EventIterator].Paused)
					BR_Timer.m_Active[m_EventIterator].DueTime += Time.deltaTime;
				else
					BR_Timer.m_Active[m_EventIterator].LifeTime += Time.deltaTime;
			}

			m_EventIterator--;
			m_EventBatch++;
		}

	}

	//////////////////////////////////////////
	/// 
	/// This in methods are use for scheduling
	/// 
	//////////////////////////////////////////

	public static void In(float delay, Callback callback, Handle timerHandle = null)
	{
		Schedule (delay, callback, null, null, timerHandle, 1, -1.0f);
	}

	public static void In(float delay, Callback callback,int iterations, Handle timerHandle = null)
	{
		Schedule (delay, callback, null, null, timerHandle, iterations, -1.0f);
	}

	public static void In(float delay, Callback callback, int iterations, float interval, Handle timerHandle = null)
	{
		Schedule (delay, callback, null, null, timerHandle, iterations, interval);
	}

	public static void In(float delay, ArgCallback callback, object arguments, Handle timerHandle = null)
	{
		Schedule (delay, null, callback, arguments, timerHandle, 1, -1.0f);
	}

	public static void In(float delay, ArgCallback callback, object arguments, int iterations, Handle timerHandle = null)
	{
		Schedule (delay, null, callback, arguments, timerHandle, iterations, -1.0f);
	}

	public static void In(float delay, ArgCallback callback, object arguments, int iterations, float interval, Handle timerHandle = null)
	{
		Schedule (delay, null, callback, arguments, timerHandle, iterations, interval);
	}

	public static void Start(Handle timerHandle)
	{
		/* TEN MOTHERFUCKING YEARS CAUSE WHY NOT!*/
		Schedule (315360000.0f, delegate() {}, null, null, timerHandle, 1, -1.0f);
	}

	/// <summary>
	/// Schedule the specified time, func, argFunc, arfs, timerHandle, iterations and interval.
	/// purpose is for measuring time
	/// it takes a mandatory timer to handle as only input argument
	/// </summary>
	private static void Schedule(float time, Callback func, ArgCallback argFunc, object args, Handle timerHandle, int iterations, float interval)
	{
		if (func == null && argFunc == null) {
			UnityEngine.Debug.LogError ("Error: (BR_Timer) Aborted because functions are null");
			return;
		}

		if (m_MainObject == null) {
			m_MainObject = new GameObject ("Timers");
			m_MainObject.AddComponent<BR_Timer> ();
			UnityEngine.Object.DontDestroyOnLoad (m_MainObject);

#if (UNITY_EDITOR && DEBUG)
			m_MainObject.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
		}

		time = Mathf.Max (0.0f, time);
		iterations = Mathf.Max (0, iterations);
		interval = (interval == 1.0f) ? time : Math.Max (0.0f, interval);

		m_NewEvent = null;
		if (m_Pool.Count > 0) {
			m_NewEvent = m_Pool [0];
			m_Pool.Remove (m_NewEvent);
		} else 
			m_NewEvent = new Event ();

		BR_Timer.m_EventCount++;
		m_NewEvent.Id = BR_Timer.m_EventCount;

		if (func != null)
			m_NewEvent.Function = func;
		else if (argFunc != null) {
			m_NewEvent.ArgFunction = argFunc;
			m_NewEvent.Arguments = args;
		}
		m_NewEvent.StartTime = Time.time;
		m_NewEvent.DueTime = Time.time + time;
		m_NewEvent.Iterations = iterations;
		m_NewEvent.Interval = interval;
		m_NewEvent.LifeTime = 0.0f;
		m_NewEvent.Paused = false;

		BR_Timer.m_Active.Add (m_NewEvent);

		if (timerHandle != null) {
			if (timerHandle.Active)
				timerHandle.Cancel ();
			timerHandle.Id = m_NewEvent.Id;
		}

#if(UNITY_EDITOR && DEBUG)
		m_NewEvent.StoreCallingMethod ();
		EditorRefresh ();
#endif	
	}

	private static void Cancel(BR_Timer.Handle handle)
	{
		if (handle == null)
			return;

		if (handle.Active) 
		{
			handle.Id = 0;
			return;
		}
	}

	public static void CancelAll()
	{
		for (int i = BR_Timer.m_Active.Count - 1; i > -1; i--) 
		{
			BR_Timer.m_Active[i].Id = 0;
		}
	}

	public static void DestroyAll()
	{
		BR_Timer.m_Active.Clear ();
		BR_Timer.m_Pool.Clear ();

#if (UNITY_EDITOR && DEBUG)
		EditorRefresh();
#endif
	}

	private void OnLevelWasLoaded()
	{
		for (int i = BR_Timer.m_Active.Count - 1; i > -1; i--) 
		{
			if(BR_Timer.m_Active[i].CancelOnLoad)
				BR_Timer.m_Active[i].Id = 0;
		}
	}

	public static Stats EditorGetStats()
	{
		Stats stats;
		stats.Created = m_Active.Count + m_Pool.Count;
		stats.Inactive = m_Pool.Count;
		stats.Active = m_Active.Count;

		return stats;
	}

	public static int EditorGetMethodId(int eventIndex)
	{
		if (eventIndex < 0 || eventIndex > m_Active.Count - 1)
			return 0;
		return m_Active [eventIndex].Id;
	}

#if (DEBUG && UNITY_EDITOR)
	private static void EditorRefresh()
	{
		m_MainObject.name = "Timer (" + m_Active.Count + " / " + (m_Pool.Count + m_Active.Count).ToString () + ")";
	}
#endif

/***************************************************************
 * BR_Timer.Event
 * 
 * Description: this class will represent the event handles
 * ************************************************************/
	private class Event
	{
		public int Id;

		public Callback Function = null;
		public ArgCallback ArgFunction = null;
		public object Arguments = null;

		public int Iterations = 1;
		public float Interval = -1.0f;
		public float DueTime = 0.0f;
		public float StartTime = 0.0f;
		public float LifeTime = 0.0f;
		public bool Paused = false;
		public bool CancelOnLoad = true;

#if (DEBUG && UNITY_EDITOR)
		private string m_CallingMethod = "";
#endif
		public void Execute()
		{
			if(Id == 0 || DueTime == 0.0f)
			{
				Recycle();
				return;
			}

			if (Function != null)
				Function ();
			else if (ArgFunction != null)
				ArgFunction (Arguments);
			else 
			{
				Error("Aborted Event");
				Recycle();
				return;
			}

			if (Iterations < 0) 
			{
				Iterations--;
				if(Iterations < 1)
				{
					Recycle();
					return;
				}
			}

			DueTime = Time.time + Interval;
		}

		private void Recycle()
		{
			Id = 0;
			DueTime = 0.0f;
			StartTime = 0.0f;
			CancelOnLoad = true;

			Function = null;
			ArgFunction = null;
			Arguments = null;

			if (BR_Timer.m_Active.Remove (this))
				m_Pool.Add (this);

#if (UNITY_EDITOR && DEBUG)
			EditorRefresh();
#endif

		}

		private void Destroy()
		{
			BR_Timer.m_Active.Remove (this);
			BR_Timer.m_Pool.Remove (this);

		}

#if (UNITY_EDITOR && DEBUG)
		public void StoreCallingMethod()
		{
			StackTrace stackTrace = new StackTrace ();

			string result = "";
			string declaringType = "";

			for (int v = 3; v < stackTrace.FrameCount; v++)
			{
				StackFrame stackFrame = stackTrace.GetFrame (v);
				declaringType = stackFrame.GetMethod().DeclaringType.ToString();
				result += " <- " + declaringType + ":" + stackFrame.GetMethod().Name.ToString ();
			}
			m_CallingMethod = result;
		}
#endif
		private void Error(string message)
		{
			string msg = "Error: " + this + " " + message;
#if (UNITY_EDITOR && DEBUG)
			msg += MethodInfo;
#endif
			UnityEngine.Debug.LogError (msg);

		}

		public string MethodName
		{
			get
			{
				if(Function != null)
				{
					if(Function.Method != null)
					{
						if(Function.Method.Name[0] == '<')
							return "delegate";
						else return Function.Method.Name;
					}
				}
				else if(ArgFunction != null)
				{
					if(ArgFunction.Method != null)
					{
						if(ArgFunction.Method.Name[0] != '<')
							return "delegate";
						else return ArgFunction.Method.Name;
					}
				}
				return null;
			}
		}

		public string MethodInfo
		{
			get
			{
				string s = MethodName;
				if(!string.IsNullOrEmpty(s))
				{
					s += "(";
					if(Arguments != null)
					{
						if(Arguments.GetType().IsArray)
						{
							object[] array = (object[])Arguments;
							foreach(object o in array)
							{
								s += o.ToString();
								if(Array.IndexOf(array, o) < array.Length - 1)
									s += ", ";
							}
						}
						else
							s += Arguments;
					}
					s+=")";
				}
				else 
					s = "(Function = null)";
#if (UNITY_EDITOR && DEBUG)
				s += m_CallingMethod;
#endif
				return s;
			}
		}
	}

/***************************************************************
 * BR_Timer.Handle
 * 
 * Description: this class will represent the handles
 * ************************************************************/

	public class Handle
	{
		private BR_Timer.Event m_Event = null;
		private int m_Id = 0;
		private int m_StartIterations = 1;
		private float m_FirstDueTime = 0.0f;

		public bool Paused
		{
			get
			{
				return Active && m_Event.Paused;
			}
			set 
			{
				if(Active)
					m_Event.Paused = value;
			}
		}

		public float TimeOfInitiation
		{
			get
			{
				if(Active)
					return m_Event.StartTime;
				else return 0.0f;
			}
		}


		public float TimeOfFirstIteration
		{
			get
			{
				if(Active)
					return m_FirstDueTime;
				return 0.0f;
			}
		}

		public float TimeOfNextIteration
		{
			get
			{
				if(Active)
					return m_Event.DueTime;
				return 0.0f;
			}
		}

		public float TimeOfLastIteration
		{
			get
			{
				if(Active)
					return Time.time + DurationLeft;
				return 0.0f;
			}
		}

		public float Delay
		{
			get
			{
				return(Mathf.Round ((m_FirstDueTime - TimeOfInitiation) * 1000.0f) / 1000.0f);
			}
		}

		public float Interval
		{
			get
			{
				if(Active)
					return m_Event.Interval;
				return 0.0f;
			}
		}

		public float TimeUntilNextIteration
		{
			get
			{
				if(Active)
					return m_Event.DueTime - Time.time;
				return 0.0f;
			}
		}

		public float DurationLeft
		{
			get
			{
				if(Active)
					return TimeUntilNextIteration + ((m_Event.Iterations - 1) * m_Event.Interval);
				return 0.0f;
			}
		}

		public float DurationTotal
		{
			get
			{
				if(Active)
				{
					return Delay + ((m_StartIterations) * ((m_StartIterations > 1) ? Interval : 0.0f));
				}
				return 0.0f;
			}
		}

		public float Duration
		{
			get
			{
				if(Active)
					return m_Event.LifeTime;
				return 0.0f;
			}
		}

		public int IterationsTotal
		{
			get
			{
				return m_StartIterations;
			}
		}

		public int IterationsLeft
		{
			get
			{
				if(Active)
					return m_Event.Iterations;
				return 0;
			}
		}

		public int Id
		{
			get
			{
				return m_Id;
			}
			set
			{
				m_Id = value;

				if(m_Id == 0)
				{
					m_Event.DueTime = 0.0f;
					return;
				}
				m_Event = null;
				for(int t = BR_Timer.m_Active.Count - 1; t > -1; t--)
				{
					if(BR_Timer.m_Active[t].Id == m_Id)
					{
						m_Event = BR_Timer.m_Active[t];
						break;
					}
				}
				if(m_Event == null)
					UnityEngine.Debug.LogError("Error: " + this + " Failed to assign ID: " + m_Id);

				m_StartIterations = m_Event.Iterations;
				m_FirstDueTime = m_Event.DueTime;
			}
		}

		public bool Active
		{
			get
			{
				if(m_Event == null || Id == 0 || m_Event.Id == 0)
					return false;
				return m_Event.Id == Id;
			}
		}

		public string MethodName
		{
			get
			{
				if(Active)
					return m_Event.MethodName;
				return "";
			}
		}

		public string MethodInfo
		{
			get
			{
				if(Active)
					return m_Event.MethodInfo;
				return "";
			}
		}

		public bool CancelOnLoad
		{
			get
			{
				if(Active)
					return m_Event.CancelOnLoad;
				return true;
			}
			set
			{
				if(Active)
				{
					m_Event.CancelOnLoad = value;
					return;
				}
				UnityEngine.Debug.LogWarning ("Warning: " + this + " Tried to cancel but failed");
			}
		}

		public void Cancel()
		{
			BR_Timer.Cancel (this);
		}

		public void Execute()
		{
			m_Event.DueTime = Time.time;
		}
	}
}
