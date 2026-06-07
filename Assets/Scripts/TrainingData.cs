using UnityEngine;

public class TrainingData : MonoBehaviour
{
    public TrainingProgressType progressType;
    public string userAnimatorBool;
    public Vector3 cameraTargetLocalPosition;
    public Vector3 cameraPlaceLocalPosition;
    public Transform trainingPos;
    public Transform exitPos;
    public float trainingDuration;
    public string selfAnimatorBool;
}

public enum TrainingProgressType
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
