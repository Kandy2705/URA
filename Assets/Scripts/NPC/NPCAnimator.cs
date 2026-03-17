using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetWalk(float speed)
    {
        _animator.SetBool("isMoving", speed != 0);
    }
}
