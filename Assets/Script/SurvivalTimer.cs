
using UnityEngine;

public class SurvivalTimer : MonoBehaviour
{
    public float goal = 60f;
    public Animator gateAnimator;

    private float time;
    private bool won;

    void Update()
    {
        if (won) return;

        time += Time.deltaTime;

        if (time >= goal)
        {
            won = true;
            Debug.Log("YOU WIN!");

            if (gateAnimator != null)
            {
                gateAnimator.SetBool("Open", true);
            }
        }
    }
}
