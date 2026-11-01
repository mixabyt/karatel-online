using UnityEngine;

public class Player_Script : MonoBehaviour
{
	public Transform World_Prefab;

	private Rect m_ShiftDetectionRectangle;

	private World m_World { get; set; }

	private void Start()
	{
		MovePlayerToCenterOfMap();
	}

	private void Update()
	{
	}

	private void CenterWorldAroundPlayer()
	{
	}

	private void MoveMapShiftDetectionRectangle(Vector3 amountToMove)
	{
		ref Rect shiftDetectionRectangle = ref m_ShiftDetectionRectangle;
		shiftDetectionRectangle.x += amountToMove.x;
		ref Rect shiftDetectionRectangle2 = ref m_ShiftDetectionRectangle;
		shiftDetectionRectangle2.y += amountToMove.y;
	}

	private void MovePlayerToCenterOfMap()
	{
	}
}
