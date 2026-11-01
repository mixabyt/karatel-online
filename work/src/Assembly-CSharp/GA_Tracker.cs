using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GA_Tracker : MonoBehaviour
{
	public enum GAEventType
	{
		BreadCrumb,
		Start,
		OnDestroy,
		OnLevelWasLoaded,
		OnTriggerEnter,
		OnCollisionEnter,
		OnControllerColliderHit
	}

	public static Dictionary<GAEventType, string> EventTooltips = new Dictionary<GAEventType, string>
	{
		{
			GAEventType.BreadCrumb,
			"Send an event every time interval. Good for generating heatmaps of player position/movement in your levels."
		},
		{
			GAEventType.Start,
			"Send an event when the Start method is run. Use this for tracking spawning of the object"
		},
		{
			GAEventType.OnDestroy,
			"Send an event when the OnDestroy method is run. Use this for tracking \"death\" of the object."
		},
		{
			GAEventType.OnLevelWasLoaded,
			"Send an event when the OnLevelWasLoaded method is run. Use this for tracking when a new level is loaded."
		},
		{
			GAEventType.OnTriggerEnter,
			"Send an event when the OnTriggerEnter method is run. Use this for tracking when something (f.x. the player) enters a trigger area."
		},
		{
			GAEventType.OnCollisionEnter,
			"Send an event when the OnCollisionEnter method is run. Use this for tracking when objects collide."
		},
		{
			GAEventType.OnControllerColliderHit,
			"Send an event when the OnControllerColliderHit method is run. Use this for tracking when a controller hits a collider while performing a Move."
		}
	};

	[SerializeField]
	public List<GAEventType> TrackedEvents = new List<GAEventType>();

	public bool TrackedEventsFoldOut;

	public bool TrackTarget;

	public bool ShowGizmo = true;

	public float BreadCrumbTrackInterval = 1f;

	private bool _trackTargetAlreadySet;

	private float _lastBreadCrumbTrackTime;

	private void Start()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isPlaying)
		{
			return;
		}
		if (TrackedEvents.Contains(GAEventType.Start))
		{
			GA.API.Design.NewEvent("Start:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
		if (TrackTarget)
		{
			GA.SettingsGA.TrackTarget = ((Component)this).transform;
			if (_trackTargetAlreadySet)
			{
				GA.LogWarning("You should only set the Track Target of GA_Tracker once per scene");
			}
			_trackTargetAlreadySet = true;
		}
	}

	private void Update()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && TrackedEvents.Contains(GAEventType.BreadCrumb) && Time.time > _lastBreadCrumbTrackTime + BreadCrumbTrackInterval)
		{
			_lastBreadCrumbTrackTime = Time.time;
			GA.API.Design.NewEvent("BreadCrumb:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
	}

	private void OnDestroy()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && TrackedEvents.Contains(GAEventType.OnDestroy))
		{
			GA.API.Design.NewEvent("OnDestroy:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
	}

	public void OnLevelWasLoaded()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && TrackedEvents.Contains(GAEventType.OnLevelWasLoaded))
		{
			GA.API.Design.NewEvent("OnLevelWasLoaded:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
	}

	public void OnTriggerEnter()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && TrackedEvents.Contains(GAEventType.OnTriggerEnter))
		{
			GA.API.Design.NewEvent("OnTriggerEnter:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
	}

	public void OnCollisionEnter()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && TrackedEvents.Contains(GAEventType.OnCollisionEnter))
		{
			GA.API.Design.NewEvent("OnCollisionEnter:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
	}

	public void OnControllerColliderHit()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && TrackedEvents.Contains(GAEventType.OnControllerColliderHit))
		{
			GA.API.Design.NewEvent("OnControllerColliderHit:" + ((Object)((Component)this).gameObject).name, ((Component)this).transform.position);
		}
	}

	public Array GetEventValues()
	{
		return Enum.GetValues(typeof(GAEventType));
	}

	private void OnDrawGizmos()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (ShowGizmo)
		{
			Gizmos.DrawIcon(((Component)this).transform.position, "gaLogo", true);
		}
	}
}
