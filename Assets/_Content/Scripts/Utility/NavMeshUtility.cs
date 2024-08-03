	using UnityEngine;
	using UnityEngine.AI;

	public static class NavMeshUtility
	{
		public static bool CanBeReachedOnNavmesh(Vector3 startPos, Vector3 endPos)
		{
			var closestNavmeshPosition = endPos;
			if (NavMesh.SamplePosition(endPos, out var navMeshHit, 0.05f, NavMesh.AllAreas))
			{
				closestNavmeshPosition = navMeshHit.position;
			}

			NavMeshPath path = new NavMeshPath();
			var pathFound = NavMesh.CalculatePath(startPos, closestNavmeshPosition, 
				NavMesh.AllAreas, path);
			return pathFound;
		}
	}
