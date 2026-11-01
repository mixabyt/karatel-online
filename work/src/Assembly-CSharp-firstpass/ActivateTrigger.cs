using UnityEngine;

public class ActivateTrigger : MonoBehaviour
{
	public enum Mode
	{
		Trigger,
		Replace,
		Activate,
		Enable,
		Animate,
		Deactivate
	}

	public Mode action = Mode.Activate;

	public Object target;

	public GameObject source;

	public int triggerCount = 1;

	public bool repeatTrigger;

	private void DoActivateTrigger()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		triggerCount--;
		if (triggerCount != 0 && !repeatTrigger)
		{
			return;
		}
		Object val = (Object)((!(target != (Object)null)) ? ((object)((Component)this).gameObject) : ((object)target));
		Behaviour val2 = (Behaviour)(object)((val is Behaviour) ? val : null);
		GameObject val3 = (GameObject)(object)((val is GameObject) ? val : null);
		if ((Object)(object)val2 != (Object)null)
		{
			val3 = ((Component)val2).gameObject;
		}
		switch (action)
		{
		case Mode.Trigger:
			val3.BroadcastMessage("DoActivateTrigger");
			break;
		case Mode.Replace:
			if ((Object)(object)source != (Object)null)
			{
				Object.Instantiate((Object)(object)source, val3.transform.position, val3.transform.rotation);
				Object.DestroyObject((Object)(object)val3);
			}
			break;
		case Mode.Activate:
			val3.SetActive(true);
			break;
		case Mode.Enable:
			if ((Object)(object)val2 != (Object)null)
			{
				val2.enabled = true;
			}
			break;
		case Mode.Animate:
			val3.animation.Play();
			break;
		case Mode.Deactivate:
			val3.SetActive(false);
			break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		DoActivateTrigger();
	}
}
