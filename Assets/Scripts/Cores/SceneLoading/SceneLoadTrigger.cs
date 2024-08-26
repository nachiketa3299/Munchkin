using System;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{

/// <summary>
/// 게임 오브젝트가 씬의 로딩 박스에 존재하게 되거나 존재하게 되지 않을 때,<br/>
/// 주변 씬의 로드/언로드 요청을 보낸다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))] // 리지드 바디가 있어야, 컬리전을 검출할 수 있다.
public partial class SceneLoadTrigger : MonoBehaviour
{

#region UnityCallbacks
	void OnDisable() => SceneLoadManager.Instance.Disabled(this);

#endregion // UnityCallbacks

#region UnityCollision

	void OnTriggerEnter(Collider collider)
	{
		// 씬 로딩 박스가 맞나?
		if (!IsSceneLoadingBoxLayer(collider.gameObject.layer))
		{
			return;
		}

		var enteringSceneName = collider.gameObject.scene.name;

		if (_inSceneName == enteringSceneName)
		{
			return;
		}

		// 현재 이 오브젝트가 존재하는 씬 이름이 달라진다
		_inSceneName = enteringSceneName;
		_sceneNamesToMaintain = new(SceneLoadManager.Instance.RetrieveNearSceneNames(_inSceneName, _depthToLoad));

		// 이름이 달라지면 당연히 로딩 요청을 새로 보내여야 함
		// 여기서 시작
		SceneLoadManager.Instance.Entered(entering: this, sceneName: _inSceneName, depthToLoad: _depthToLoad);
	}

#endregion // UnityCollision

	public bool IsSceneLoadingBoxLayer(in int layer) => layer == _sceneLoadingBoxLayer;

	string _inSceneName = string.Empty;
	[SerializeField] int _depthToLoad = 1;
	[SerializeField][HideInInspector] List<string> _sceneNamesToMaintain = new();
	readonly int _sceneLoadingBoxLayer = 6;
}

}