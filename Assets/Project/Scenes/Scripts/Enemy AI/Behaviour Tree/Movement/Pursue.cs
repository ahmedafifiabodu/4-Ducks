using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Pursue : NavMeshMovement
{
    [UnityEngine.Serialization.FormerlySerializedAs("targetDistPrediction")]
    public SharedFloat m_TargetDistPrediction = 20;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Multiplier for predicting the look ahead distance")]
    [UnityEngine.Serialization.FormerlySerializedAs("targetDistPredictionMult")]
    public SharedFloat m_TargetDistPredictionMult = 20;

    [UnityEngine.Serialization.FormerlySerializedAs("target")]
    public SharedGameObject m_Target;

    private Vector3 targetPosition;

    public override void OnStart()
    {
        base.OnStart();

        targetPosition = m_Target.Value.transform.position;
        SetDestination(Target());
    }

    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
            return TaskStatus.Success;

        SetDestination(Target());

        return TaskStatus.Running;
    }

    private Vector3 Target()
    {
        var distance = (m_Target.Value.transform.position - transform.position).magnitude;
        var speed = Velocity().magnitude;

        float futurePrediction;
        if (speed <= distance / m_TargetDistPrediction.Value)
            futurePrediction = m_TargetDistPrediction.Value;
        else
            futurePrediction = (distance / speed) * m_TargetDistPredictionMult.Value;

        // Predict the future by taking the velocity of the target and multiply it by the future prediction
        var prevTargetPosition = targetPosition;
        targetPosition = m_Target.Value.transform.position;
        return targetPosition + (targetPosition - prevTargetPosition) * futurePrediction;
    }

    public override void OnReset()
    {
        base.OnReset();

        m_TargetDistPrediction = 20;
        m_TargetDistPredictionMult = 20;
        m_Target = null;
    }
}