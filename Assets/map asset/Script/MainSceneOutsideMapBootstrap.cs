using System.Collections;
using UnityEngine;

public class MainSceneOutsideMapBootstrap : MonoBehaviour
{
    [SerializeField] private SketchOutsideTransition outsideTransition;

    private IEnumerator Start()
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            yield break;
        }

        if (outsideTransition == null)
        {
            outsideTransition = GetComponent<SketchOutsideTransition>();
        }

        if (outsideTransition != null)
        {
            outsideTransition.BeginMainScene(player);
        }
    }
}
