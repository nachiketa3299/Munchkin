#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(NestVolumeGenerator))]
internal sealed class NestVolumeGenerator : Editor
{
}

}

#endif