using UnityEngine;

public class CheckAttackAnimation : StateMachineBehaviour
{
    private AttackRadius _attackRadius;
    private bool hasCalled = false;

    private void Awake() => _attackRadius = FindFirstObjectByType<AttackRadius>();

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName(GameConstant.EnemyAnimation.EnemyAnimationState.AttackState) && stateInfo.normalizedTime >= 0.5f && !hasCalled)
        {
            _attackRadius.OnAttackAnimationCompleted();
            hasCalled = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasCalled = false;
    }
}