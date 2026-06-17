using UnityEngine;

public class TrainingData : MonoBehaviour
{
    public TrainingType trainingType;
    public string agentAnimatorBool;
    public Vector3 cameraTargetLocalPosition;
    public Vector3 cameraPlaceLocalPosition;
    public Transform trainingPos;
    public Transform exitPos;
    public float trainingDuration;
    public string selfAnimatorBool;
}

public enum TrainingType
{
    None,
    Treadmill,
    Bike,
    JumpBox,
    BenchPress,
    PecFly,
    Crossover,
    Dips,
    LatPull,
    CableRow,
    Rower,
    BackExtension,
    TBar,
    PullUps
}
