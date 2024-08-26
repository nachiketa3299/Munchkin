using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{

/// <summary>
/// Nest의 영역을 상하좌우로 둘러싸는 4개의 콜라이더를 지정된 설정에 맞게 런타임에 생성한다.
/// </summary>
[DisallowMultipleComponent]
public partial class NestVolumeGenerator : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		// make empty game object and attach it as child
		var childObject = new GameObject("NestEggDeadZones");
			childObject.transform.SetParent(transform);
			childObject.transform.localPosition = Vector3.zero;
			childObject.transform.localRotation = Quaternion.identity;

	  // I know this sucks but ...
		if (_left)
		{
			var left = childObject.AddComponent<BoxCollider>();
			var leftBounds = LeftBounds();
			left.center = leftBounds.center;
			left.size = leftBounds.size;
			_colliders.Add(left);
		}

		if (_up)
		{
			var up = childObject.AddComponent<BoxCollider>();
			var upBounds = UpBounds();
			up.center = upBounds.center;
			up.size = upBounds.size;
			_colliders.Add(up);
		}

		if (_right)
		{
			var right = childObject.AddComponent<BoxCollider>();
			var rightBounds = RightBounds();
			right.center = rightBounds.center;
			right.size = rightBounds.size;
			_colliders.Add(right);
		}

		if (_down)
		{
			var down = childObject.AddComponent<BoxCollider>();
			var downBounds = DownBounds();
			down.center = downBounds.center;
			down.size = downBounds.size;
			_colliders.Add(down);
		}

		_colliders.ForEach(collider => collider.isTrigger = true);

		childObject.layer = _nestEggDeadZoneLayer;
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

	public int NestEggDeadZoneLayer => _nestEggDeadZoneLayer;

	readonly int _nestEggDeadZoneLayer = 10;
	[SerializeField] Vector3 _seedColliderSize = new(15.0f, 12.0f, 1.0f);
	[SerializeField] Vector3 _seedColliderCenter = new(0.0f, 0.0f, 0.0f);
	[SerializeField][HideInInspector] List<Collider> _colliders = new();
	[SerializeField] bool _up = true;
	[SerializeField] bool _down = false;
	[SerializeField] bool _left = true;
	[SerializeField] bool _right = true;

}

}