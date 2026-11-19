
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-90)]
public class EnemyLevelSetup : MonoBehaviour
{
    private void Awake()
    {
        // Only configure the first level.
        if (SceneManager.GetActiveScene().name != "Level1")
            return;

        // MidtermLevel creates the actual platforms first.
        GameObject navigationRoot =
            new GameObject("Runtime Platform NavMesh");

        int platformCount = 0;

        GameObject[] objects =
            FindObjectsByType<GameObject>(
                FindObjectsSortMode.None
            );

        foreach (GameObject obj in objects)
        {
            if (obj.name == "Floating Platform" &&
                obj.transform.parent == null)
            {
                obj.transform.SetParent(
                    navigationRoot.transform, true
                );

                platformCount++;
            }
        }

        if (platformCount == 0)
        {
            Debug.LogError(
                "EnemyLevelSetup: No platforms found!"
            );

            Destroy(navigationRoot);
            return;
        }

        // Build navigation from the real platforms.
        NavMeshSurface surface =
            navigationRoot.AddComponent<NavMeshSurface>();

        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry =
            NavMeshCollectGeometry.PhysicsColliders;

        surface.BuildNavMesh();

        // Platform heights come from BuildLevelOne().
        MoveEnemy(
            "PatrollerRoot",
            new Vector3(-1f, 0.25f, -5f)
        );

        MoveEnemy(
            "ShooterRoot",
            new Vector3(-1.6f, 0.55f, 1f)
        );

        MoveEnemy(
            "ChargerRoot",
            new Vector3(0.8f, 0.35f, 7f)
        );

        // Both patrol points must be on the same platform.
        MovePoint(
            "PatrolPoint_A",
            new Vector3(-1.4f, 0.25f, -5.7f)
        );

        MovePoint(
            "PatrolPoint_B",
            new Vector3(1f, 0.25f, -4.5f)
        );

        Debug.Log(
            "EnemyLevelSetup: Built NavMesh from " +
            platformCount + " real platforms."
        );
    }


    private void MoveEnemy(string name, Vector3 position)
    {
        GameObject enemy = GameObject.Find(name);

        if (enemy == null)
        {
            Debug.LogError(name + " not found!");
            return;
        }

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError(name + " has no NavMeshAgent!");
            return;
        }

        // Find a valid position on the new NavMesh.
        if (!NavMesh.SamplePosition(
            position,
            out NavMeshHit hit,
            1.5f,
            agent.areaMask))
        {
            Debug.LogError(name + ": NavMesh position not found!");
            return;
        }

        // Place the enemy BEFORE activating NavMeshAgent.
        agent.enabled = false;
        enemy.transform.position = hit.position;

        // NavMesh has already been created.
        agent.enabled = true;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(name + ": could not join NavMesh!");
            agent.enabled = false;
            return;
        }

        Debug.Log(name + " successfully placed on NavMesh!");
    }


    private void MovePoint(string name, Vector3 position)
    {
        GameObject point = GameObject.Find(name);

        if (point != null)
        {
            point.transform.position = position;
        }
        else
        {
            Debug.LogWarning(name + " not found!");
        }
    }
}
