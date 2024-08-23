#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{
public partial class NestEggHandler : MonoBehaviour
{
	// 아직은 에디터에서만 호출됨
	void SpawnNestEgg()
	{
		var instance = EggPool.Instance.GetEggInstance(EEggOwner.Nest);
		instance.transform.SetPositionAndRotation(SpawnPosition, Quaternion.identity);

		var delta = _nestEggCountToMainTain - EggPool.Instance.NestEggs.Count;

		if (delta > 0)
		{

		}
		else if (delta < 0)
		{
			for (var i = 0; i < Mathf.Abs(delta); ++i)
			{
				EggPool.Instance.NestEggs[i].GetComponent<EggDamageSourceBase>().ForceInflictLethalDamage();
			}
		}
	}
}

}

namespace MC.Editors
{

[CustomEditor(typeof(NestEggHandler))]
internal sealed class NestEggHandlerEditor : Editor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

// Spawn nest egg button

		GUI.enabled = EditorApplication.isPlaying;

		EditorGUILayout.Space();

		if (GUILayout.Button("Spawn egg in nest"))
		{
			var nest = target as NestEggHandler;
			nest.SendMessage("SpawnNestEgg");
		}

		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}

#endregion // UnityCallbacks

}

}

#endif