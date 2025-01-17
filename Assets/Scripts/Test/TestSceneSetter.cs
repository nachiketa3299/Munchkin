using UnityEngine;
using UnityEngine.ProBuilder;

using TMPro;

namespace MC.Test
{

/// <summary> 간단하게 테스트 씬의 메쉬 색상을 랜덤화하기 위한 스크립트 </summary>
public class TestSceneSetter : MonoBehaviour
{

#region UnityCallbacks

	void Start()
	{

		var pbMesh = GetComponentInChildren<ProBuilderMesh>();

		if (!pbMesh)
		{
			return;
		}

		var color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

		for (var i = 0; i < pbMesh.faceCount; ++i)
		{
			pbMesh.SetFaceColor(pbMesh.faces[i], color);
		}

		pbMesh.Refresh();
	}

	void OnValidate() => SetTextMeshTextWithSceneName();
	void Reset() => SetTextMeshTextWithSceneName();

#endregion // UnityCallbacks

	void SetTextMeshTextWithSceneName() => GetComponentInChildren<TextMeshPro>().text = gameObject.scene.name;

} // class

} // namespace