public interface IAgent
{
    void StartTraining(TrainingData trainingData);
    void StopTraining(TrainingData trainingData);
    void CancelTraining();
}