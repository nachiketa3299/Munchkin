using UnityEngine;

namespace MC.Utility
{

public static class LayerUtility
{
	public static void SetLayerRecursive(GameObject target, int newLayer)
	{
		target.layer = newLayer;

		foreach(Transform childTransform in target.transform)
		{
			SetLayerRecursive(childTransform.gameObject, newLayer);
		}
	}
}

}