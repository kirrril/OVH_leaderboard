using UnityEngine;

public class TrainingPadController : MonoBehaviour
{
    [SerializeField] private TrainingProgressType trainingType;
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private Material disabledMaterial;
    [SerializeField] private Renderer padRenderer;
    private bool hasInitialized;
    private bool isCompleted;

    void Update()
    {
        SetPadColor();
    }

    private void SetPadColor()
    {
        bool shouldBeCompleted = GameManager.Instance.IsTrainingCompleted(trainingType);
        if (hasInitialized && shouldBeCompleted == isCompleted) return;

        hasInitialized = true;
        isCompleted = shouldBeCompleted;
        padRenderer.sharedMaterial = isCompleted ? disabledMaterial : enabledMaterial;
    }
}
