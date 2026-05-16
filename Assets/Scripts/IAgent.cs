public interface IAgent
{
    void StartTraining(TrainingSpot trainingSpot);
    void StopTraining(TrainingSpot trainingSpot);
    void ResetPath();
}