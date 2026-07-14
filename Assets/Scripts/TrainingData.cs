using UnityEngine;

public class TrainingData : MonoBehaviour
{
    public PlayerTrainingType playerTrainingType;
    public ManTrainingType manTrainingType;
    public GirlTrainingType girlTrainingType;
    public Transform cameraTarget;
    public Transform cameraPlace;
    public Transform trainingPos;
    public Transform exitPos;
    public float trainingDuration;
}

public enum PlayerTrainingType
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

public enum ManTrainingType
{
    None,
    ShowOff,
    Squats,
    Treadmill,
    Bike,
    JumpBox,
    Dips,
    BarbellStand,
    BenchPress,
    PecFly,
    Crossover,
    DumbbellsSit,
    DumbbellsStand,
    LatPull,
    CableRow,
    PullUps,
    BackBarbell,
    TBar
}

public enum GirlTrainingType
{
    None,
    ShowOff,
    Squats,
    Treadmill,
    Bike,
    JumpBox,
    PecFly,
    DumbbellsStand1,
    DumbbellsStand2,
    CableRow,
    Rower,
    BackExtension,
    PullUps,
    Step,
    Stretch,
    YogaBall
}
