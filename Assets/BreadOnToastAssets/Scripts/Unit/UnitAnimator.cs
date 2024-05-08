using UnityEngine;
using System;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationVFXManager _vFXManager;

    private void Awake()
    {
        MoveAction[] moveActions = GetComponents<MoveAction>();

        if (moveActions.Length > 0)
        {
            foreach (MoveAction moveAction in moveActions)
            {
                moveAction.OnStartMoving += MoveAction_OnStartMoving;
                moveAction.OnStopMoving += MoveAction_OnStopMoving;
            }
        }

        if (TryGetComponent<RangeAction>(out RangeAction rangeAction))
        {
            rangeAction.OnShoot += RangeAction_OnShoot;
        }

    }

    //private void OnActionStarted(string actionName)
    //{
    //    _animator.Play($"{actionName}_Anim");
    //}

    private void RangeAction_OnShoot(object sender, OnShootEventArgs shootActionEventArgs)
    {
        _animator.Play($"{shootActionEventArgs.ActionName}_Anim");
        _vFXManager.SetShootVFXData(shootActionEventArgs);
    }
    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", true);
    }
    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", false);
    }

}