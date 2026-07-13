public interface IAgent
{
    void StartTraining(TrainingData trainingData);
    void StopTraining();
    void CancelTraining();
}