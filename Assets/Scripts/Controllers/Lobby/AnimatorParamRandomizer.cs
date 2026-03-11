using System.Collections;
using UnityEngine;

public class AnimatorParamRandomizer : MonoBehaviour
{
    private static Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("RandomizeAnimatorParams", 0, 6);
    }

    private void OnDestroy()
    {
        CancelInvoke("RandomizeAnimatorParams");
    }

    private void OnApplicationQuit()
    {
        CancelInvoke("RandomizeAnimatorParams");
    }

    private void RandomizeAnimatorParams()
    {
        float randomFloat = Random.Range(0, 3);
        var randomSpeed = (Random.value - 0.5f) / 2 + 1;
        var randomBool = randomFloat == 0;
        animator.SetBool("Run", randomBool);
        animator.SetFloat("Speed", randomSpeed);
        StartCoroutine(ResetParams());
    }

    private IEnumerator ResetParams()
    {
        yield return new WaitForSeconds(1);
        animator.SetBool("Run", false);
    }
}