using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using MC.Utility;

namespace MC
{

public partial class SceneDependencyData : ScriptableObject
{

	/// <summary>
	/// 어떤 씬과 인접한 씬을 정의
	/// </summary>
	/// <remarks>
	/// 무향 그래프라서 한 쪽에서만 관계를 정의해도 상관 없으나, 그래도 다 정의하는 것을 추천
	/// </remarks>
	[Serializable]
	struct Node
	{
		public SceneReference sceneRef;
		public SceneReference[] nearSceneRefs;
	}

	/// <summary> <see cref="Node"/> 에 설정된 인접 씬 정보들을 토대로 씬 이름으로 구성된 그래프를 저장 </summary>
	class Graph
	{
		public Graph(Node[] dependencies)
		{
			var allSceneNames = new HashSet<string>();

			foreach (var node in dependencies)
			{
				allSceneNames.Add(node.sceneRef.SceneName);
				foreach (var nearNode in node.nearSceneRefs)
				{
					allSceneNames.Add(nearNode.SceneName);
				}
			}

			var aList = BuildAdjacencyList(allSceneNames, dependencies);

			distances = BuildDistanceMatrix(allSceneNames, aList);
		}

		/// <summary> 연결 리스트 생성 </summary>
		/// <remarks> <c>aList["SC_Stage_21"]</c> 은 <c>SC_Stage_21</c> 와 인접한 씬의 이름들에 대한 리스트이다. </remarks>
		static Dictionary<string, HashSet<string>> BuildAdjacencyList(HashSet<string> allSceneNames, Node[] dependencies)
		{
			var aList = allSceneNames.ToDictionary(name => name, _ => new HashSet<string>());

			foreach (var dependency in dependencies)
			{
				var cSceneName = dependency.sceneRef.SceneName;
				foreach (var nSceneRef in dependency.nearSceneRefs)
				{
					var nSceneName = nSceneRef.SceneName;

					aList[cSceneName].Add(nSceneName);
					aList[nSceneName].Add(cSceneName);
				}
			}

			return aList;
		}

		/// <summary> 씬과 씬의 거리를 저장하는 딕셔너리 생성 </summary>
		/// <remarks> <c>distances["SC_Stage_21"]["SC_Stage_22"]</c> 는 <c>SC_Stage_21</c> 과 <c>SC_Stage_22</c> 의 거리를 나타낸다. </remarks>
		static Dictionary<string, Dictionary<string, int>> BuildDistanceMatrix(HashSet<string> allSceneNames, Dictionary<string, HashSet<string>> alist)
		{
			var distances = allSceneNames.ToDictionary(name => name, _ => allSceneNames.ToDictionary(inName => inName, distance => int.MaxValue));

			foreach (var name in allSceneNames)
			{
				var toVisit = new Queue<string>();
				var visited = allSceneNames.ToDictionary(name => name, _ => false);

				var initSceneName = name;

				toVisit.Enqueue(initSceneName);
				visited[initSceneName] = true;
				distances[initSceneName][initSceneName] = 0;

				while (toVisit.Count != 0)
				{
					var cSceneName = toVisit.Dequeue();
					foreach (var nSceneName in alist[cSceneName])
					{
						if (visited[nSceneName])
						{
							continue;
						}

						visited[nSceneName] = true;

						distances[initSceneName][nSceneName] = distances[initSceneName][cSceneName] + 1;
						toVisit.Enqueue(nSceneName);
					}
				}
			}

			return distances;
		}
		public readonly Dictionary<string, Dictionary<string, int>> distances;
	} // class Graph

}

}