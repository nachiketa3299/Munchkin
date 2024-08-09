using UnityEngine;

#if UNITY_EDITOR

using System.Linq;
using UnityEditor;

#endif

namespace MC
{

/// <summary>
/// Nest의 영역을 상하좌우로 둘러싸는 4개의 콜라이더를 지정된 설정에 맞게 런타임에 생성한다.
/// </summary>
[DisallowMultipleComponent]
public class NestVolumeGenerator : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		var left = gameObject.AddComponent<BoxCollider>();
		var leftBounds = LeftBounds();
		left.center = leftBounds.center;
		left.size = leftBounds.size;

		var up = gameObject.AddComponent<BoxCollider>();
		var upBounds = UpBounds();
		up.center = upBounds.center;
		up.size = upBounds.size;

		var right = gameObject.AddComponent<BoxCollider>();
		var rightBounds = RightBounds();
		right.center = rightBounds.center;
		right.size = rightBounds.size;

		var down = gameObject.AddComponent<BoxCollider>();
		var downBounds = DownBounds();
		down.center = downBounds.center;
		down.size = downBounds.size;

		left.isTrigger = up.isTrigger = right.isTrigger = down.isTrigger = true;

		_colliders = new BoxCollider[] { left, up, right, down }; // Cache
	}

#endregion // UnityCallbacks

	Vector3 WidthOffset()
	{
		return new Vector3
		(
			x: _seedColliderSize.x / 2.0f  + _seedColliderSize.z / 2.0f,
			y: 0.0f,
			z: 0.0f
		);
	}
	Vector3 HeightOffset()
	{
		return new Vector3
		(
			x: 0.0f,
			y: _seedColliderSize.y / 2.0f + _seedColliderSize.z / 2.0f,
			z: 0.0f
		);
	}

	Bounds LeftBounds()
	{
		return new Bounds
		{
			center = _seedColliderCenter - WidthOffset(),
			size = new Vector3(_seedColliderSize.z, _seedColliderSize.y, _seedColliderSize.z)
		};
	}

	Bounds RightBounds()
	{
		return new Bounds
		{
			center = _seedColliderCenter + WidthOffset(),
			size = new Vector3(_seedColliderSize.z, _seedColliderSize.y, _seedColliderSize.z)
		};
	}

	Bounds UpBounds()
	{
		return new Bounds
		{
			center = _seedColliderCenter + HeightOffset(),
			size = new Vector3(_seedColliderSize.x, _seedColliderSize.z, _seedColliderSize.z)
		};
	}

	Bounds DownBounds()
	{
		return new Bounds
		{
			center = _seedColliderCenter - HeightOffset(),
			size = new Vector3(_seedColliderSize.x, _seedColliderSize.z, _seedColliderSize.z)
		};
	}

	[SerializeField] Vector3 _seedColliderSize = new(15.0f, 12.0f, 1.0f);
	[SerializeField] Vector3 _seedColliderCenter = new(0.0f, 0.0f, 0.0f);
	[SerializeField][HideInInspector] BoxCollider[] _colliders;

#if UNITY_EDITOR

	[DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
	static void DrawGeneratedNestVolumeGizmos(NestVolumeGenerator target, GizmoType gizmoType)
	{
		var gizmoColor = Color.yellow;

		Gizmos.DrawWireCube(target.transform.position + target._seedColliderCenter, target._seedColliderSize);

		var bounds = new Bounds[] {target.LeftBounds(), target.UpBounds(), target.RightBounds(), target.DownBounds() };

		gizmoColor = Color.green;
		gizmoColor.a = 0.2f;
		Gizmos.color = gizmoColor;

		foreach(var bound in bounds)
		{
			Gizmos.DrawCube(target.transform.position + bound.center, bound.size);
		}
	}

#endif
}

}