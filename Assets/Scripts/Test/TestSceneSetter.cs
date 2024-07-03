using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace MC.Test
{
	/// <summary> 간단하게 테스트 씬의 메쉬 색상을 랜덤화하기 위한 스크립트 </summary>
	public class TestSceneSetter : MonoBehaviour
	{
		#region Unity Messages

		void Start()
		{

			var pbMesh = GetComponentInChildren<ProBuilderMesh>();

			var color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

			if (!pbMesh)
			{
				return;
			}

			for (var i = 0; i < pbMesh.faceCount; ++i)
			{
				pbMesh.SetFaceColor(pbMesh.faces[i], color);
			}

			pbMesh.Refresh();
		}

		void OnValidate() => SetTextMeshTextWithSceneName();
		void Reset() => SetTextMeshTextWithSceneName();

		#endregion // Unity Messages

		void SetTextMeshTextWithSceneName() => GetComponentInChildren<TextMeshPro>().text = gameObject.scene.name;
	} // class
} // namespace