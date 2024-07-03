using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using MC.Utility;

namespace MC
{
	/// <summary> 씬들의 상호 인접 관계를 정의하는 데이터. </summary>
	/// <remarks> 인접 관계는 그래프로 표현되며, <see cref="Node"/> 와 <see cref="Graph"/> 를 참고한다. </remarks>
	[CreateAssetMenu(menuName = "MC/Scriptable Objects/Scene Dependency Data", fileName = "SceneDependencyData_Default")]
	public class SceneDepenencyData : ScriptableObject
	{
		[Serializable]
		/// <summary> 어떤 씬과 인접한 씬을 정의 </summary>
		/// <remarks> 무향 그래프라서 한 쪽에서만 관계를 정의해도 상관 없으나, 그래도 다 정의하는 것을 추천 </remarks>
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

				distances = BuildDistanceCache(allSceneNames, aList);
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
			static Dictionary<string, Dictionary<string, int>> BuildDistanceCache(HashSet<string> allSceneNames, Dictionary<string, HashSet<string>> alist)
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

			public override string ToString()
			{
				var str = string.Empty;
				foreach (var (cSceneName, cSceneDistancePairs) in distances)
				{
					str += $"{cSceneName}: ";

					foreach (var (nSceneName, distance) in cSceneDistancePairs)
					{
						var distString = (distance == int.MaxValue) ? "INF" : distance.ToString();
						str += $"({nSceneName}, {distString}) ";
					}

					str += '\n';
				}
				return str;
			}

			public readonly Dictionary<string, Dictionary<string, int>> distances;
		} // class Graph

		#region Unity Messages

		void Awake() => RefreshGraph();
		void OnValidate() => RefreshGraph();

		#endregion // Unity Message

		void RefreshGraph()
		{
			_graph = new(_dependencies);

#if UNITY_EDITOR
			if (_logBuildingProcess)
			{
				Debug.Log($"씬 그래프가 업데이트 되었습니다. 결과:\n{_graph}");
			}
#endif
		}

		/// <summary><paramref name="fromName"/> 이름의 씬과 <paramref name="toName"/> 이름의 씬의 거리를 그래프에서 찾아 반환한다. </summary>
		/// <remarks> 
		/// <para>이 메서드의 반환 값에 따라서 어떤 씬의 로드/언로드 여부를 판단한다.</para>
		/// <para>관계가 정의되지 않은 씬이라면 아무 것도 하지 않기 위해 정수의 최대값을 반환한다.</para>
		/// </remarks>
		public int GetDistance(string fromName, string toName)
		{
			if (_graph.distances.ContainsKey(fromName))
			{
				if (_graph.distances[fromName].ContainsKey(toName))
				{
					return _graph.distances[fromName][toName];
				}
			}

			return int.MaxValue;
		}

		/// <summary> <paramref name="name"/> 이름의 씬과 거리 <paramref name="depth"/> 이하만큼 떨어진 씬을 찾아, 그 이름들을 반환한다. </summary>
		/// <remarks> 자기 자신과의 거리는 0 이지만, 반환 컬렉션에 포함시키지 않는다. </remarks>
		public List<string> RetrieveNearSceneNames(string name, int depth = 1)
		{
			var nSceneList = new List<string>();
			foreach (var (nname, dist) in _graph.distances[name])
			{
				if (nname != name && dist <= depth)
				{
					nSceneList.Add(nname);
				}
			}

			return nSceneList;
		}

		public string InitialSceneName => _initialSceneRef.SceneName;
		public string PersistentSceneName => _persistentSceneRef.SceneName;
		public string MainMenuSceneName => _mainMenuSceneRef.SceneName;

		Graph _graph;

		[SerializeField] Node[] _dependencies;
		[SerializeField] SceneReference _initialSceneRef;
		[SerializeField] SceneReference _persistentSceneRef;
		[SerializeField] SceneReference _mainMenuSceneRef;

#if UNITY_EDITOR
		[SerializeField] bool _logBuildingProcess = true;
#endif
	}
}