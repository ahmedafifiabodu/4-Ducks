using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedDetectionMode : SharedVariable<DetectionMode>
{
    public static implicit operator SharedDetectionMode(DetectionMode value) => new() { Value = value };
}