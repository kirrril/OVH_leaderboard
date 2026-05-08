using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneHUD : MonoBehaviour
{
    [SerializeField] private Image water;
    [SerializeField] private Image healthPointsImage;
    [SerializeField] private Image thirstyDeathScreen;
    [SerializeField] private Image fallingDownScreen;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Sprite[] healthPointsSprites;
    private bool isVoidFalling;
    private bool isDyingOfThirst;


    void Update()
    {
        UpdateWaterLevel();
        VoidFallingScreen();
        UpdateHealthPoints();
    }

    private void UpdateHealthPoints()
    {
        switch (GameManager.Instance.health)
        {
            case 5:
                healthPointsImage.sprite = healthPointsSprites[0];
                break;
            case 4:
                healthPointsImage.sprite = healthPointsSprites[1];
                break;
            case 3:
                healthPointsImage.sprite = healthPointsSprites[2];
                break;
            case 2:
                healthPointsImage.sprite = healthPointsSprites[3];
                break;
            case 1:
                healthPointsImage.sprite = healthPointsSprites[4];
                break;
        }
    }

    private void VoidFallingScreen()
    {
        if (playerController.currentState == PlayerController.State.Falling && !isVoidFalling)
        {
            isVoidFalling = true;
            StartCoroutine(FallingDownScreenCoroutine());
        }
    }

    public IEnumerator FallingDownScreenCoroutine()
    {
        fallingDownScreen.gameObject.SetActive(true);

        float duration = 1f;
        float elapsedTime = 0f;

        Color color = fallingDownScreen.color;
        color.a = 0f;
        fallingDownScreen.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Clamp01(elapsedTime / duration);
            fallingDownScreen.color = color;

            yield return null;
        }
        color.a = 1f;
        fallingDownScreen.color = color;
        yield return new WaitForSeconds(2f);
        color.a = 0f;
        fallingDownScreen.color = color;

        fallingDownScreen.gameObject.SetActive(false);
        isVoidFalling = false;
    }

    private void UpdateWaterLevel()
    {
        water.fillAmount = GameManager.Instance.water;

        if (GameManager.Instance.water <= 0 && !isDyingOfThirst)
        {
            isDyingOfThirst = true;
            StartCoroutine(ThirstyDeathScreenCoroutine());
        }
    }

    public IEnumerator ThirstyDeathScreenCoroutine()
    {
        thirstyDeathScreen.gameObject.SetActive(true);

        float duration = 3f;
        float elapsedTime = 0f;

        Color color = thirstyDeathScreen.color;
        color.a = 0f;
        thirstyDeathScreen.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            color.a = Mathf.Clamp01(elapsedTime / duration);
            thirstyDeathScreen.color = color;

            yield return null;
        }
        color.a = 1f;
        thirstyDeathScreen.color = color;
        yield return new WaitForSeconds(1f);
        color.a = 0f;
        thirstyDeathScreen.color = color;

        thirstyDeathScreen.gameObject.SetActive(false);
        isDyingOfThirst = false;
    }
}
