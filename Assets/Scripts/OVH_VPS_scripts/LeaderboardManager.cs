using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEditor;
using System;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private TMP_Text internetStatusText;
    [SerializeField] private GameObject pseudoManager;
    [SerializeField] private TMP_InputField pseudoInput;
    [SerializeField] private TMP_Text pseudoStatusText;
    [SerializeField] private Button pseudoSubmitButton;
    [SerializeField] private GameObject scoreManager;
    [SerializeField] private TMP_InputField scoreInput;
    [SerializeField] private TMP_Text scoreStatusText;
    [SerializeField] private GameObject rankManager;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text highestScoreText;
    [SerializeField] private TMP_Text lastScoreText;
    [SerializeField] private Image upAndDown;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button showLeaderboardButton;
    private string publicIP = "https://darwinsgym.eu/";
    private string url;
    private bool internetIsConnected;
    private bool userIsBeingProcessed;
    private bool userIsCreated;
    private bool scoreIsNotNull;
    private bool rankIsUpdated;
    private bool highestScoreIsUpdated;
    private string playerPseudo;
    private int localScore;
    private int lastLocalRank;

    void Awake()
    {
        DeleteLocalUser();

        if (PlayerPrefs.HasKey("PlayerPseudo"))
        {
            playerPseudo = PlayerPrefs.GetString("PlayerPseudo");
            userIsCreated = true;
        }

        if (PlayerPrefs.HasKey("LocalScore"))
        {
            localScore = PlayerPrefs.GetInt("LocalScore");
            lastScoreText.text = localScore.ToString();
            scoreIsNotNull = true;
        }
    }

    async void Start()
    {
        pseudoManager.SetActive(false);
        scoreManager.SetActive(false);
        scoreStatusText.text = "Tape ton score";
        await CheckInternet();
    }

    void Update()
    {
        UImanager();
    }

    private void UImanager()
    {
        if (internetIsConnected)
        {
            pseudoManager.SetActive(true);
            if (!userIsCreated)
            {
                scoreManager.SetActive(false);
                rankManager.SetActive(false);

                if (!userIsBeingProcessed)
                {
                    if (pseudoInput.text.Trim().Length < 2)
                    {
                        pseudoStatusText.text = "Minimum 2 lettres";
                        pseudoSubmitButton.interactable = false;
                    }
                    else
                    {
                        pseudoStatusText.text = "Entrée conforme";
                        pseudoSubmitButton.interactable = true;
                    }
                }
            }
            else
            {
                pseudoInput.interactable = false;
                pseudoSubmitButton.interactable = false;
                pseudoStatusText.text = $"Salut {playerPseudo}";
                scoreManager.SetActive(true);

                if (scoreIsNotNull)
                {
                    rankManager.SetActive(true);
                    if (!rankIsUpdated) UpdateRank();
                    if (!highestScoreIsUpdated) GetHighestScore();
                }
                else
                {
                    rankManager.SetActive(false);
                }
            }
        }
    }

    public void OnPseudoInputSelect()
    {
        userIsBeingProcessed = false;
    }

    private async Task CheckInternet()
    {
        internetStatusText.text = "Vérification de la connexion...";

        url = "www.google.com";

        using var www = UnityWebRequest.Get(url);
        www.timeout = 10;

        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            internetStatusText.text = "Connecté !";
            internetIsConnected = true;
        }
        else
        {
            internetStatusText.text = "Pas de connexion internet";
        }
    }

    public async void OnPseudoSubmit()
    {
        userIsBeingProcessed = true;
        await CreateUser(pseudoInput.text.Trim());
    }

    private void CreateLocalUser(string input)
    {
        PlayerPrefs.SetString("PlayerPseudo", input);
        PlayerPrefs.Save();
        playerPseudo = PlayerPrefs.GetString("PlayerPseudo");
        userIsCreated = true;
    }

    private void DeleteLocalUser()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    private async Task CreateUser(string input)
    {
        userIsBeingProcessed = true;
        url = $"{publicIP}create_user.php?pseudo={UnityWebRequest.EscapeURL(input)}";

        using var www = UnityWebRequest.Get(url);
        www.timeout = 5;

        await www.SendWebRequest();
        string reponse = www.downloadHandler.text.Trim();

        if (www.result == UnityWebRequest.Result.Success || www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (reponse == "CREATED")
            {
                pseudoStatusText.text = $"{input} est créé !";
                CreateLocalUser(input);
            }
            else if (reponse == "TAKEN")
            {
                Debug.Log($"TAKEN");
                pseudoStatusText.text = $"{input} n'est pas diponible";
            }
            else if (reponse == "INVALID")
            {
                pseudoStatusText.text = "Minimum 2 lettres";
            }
            else if (reponse == "ERROR")
            {
                pseudoStatusText.text = "Erreur inconnue";
            }
            else
            {
                pseudoStatusText.text = "Erreur serveur";
            }
        }
        else
        {
            pseudoStatusText.text = "Pas de connexion";
        }
    }

    public async void OnScoreSubmit()
    {
        int score = Convert.ToInt32(scoreInput.text.Trim());
        await UpdateScore(score);
        UpdateLocalScore(score);
        UpdateRank();
        highestScoreIsUpdated = false;
    }

    private async Task UpdateScore(int input)
    {
        url = $"{publicIP}update_score.php?pseudo={UnityWebRequest.EscapeURL(playerPseudo)}&score={input}";
        using var www = UnityWebRequest.Get(url);
        www.timeout = 5;
        await www.SendWebRequest();
        string reponse = www.downloadHandler.text.Trim();

        if (www.result == UnityWebRequest.Result.Success || www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (reponse == "INVALID")
            {
                scoreStatusText.text = $"Entrée non-valide !";
            }
            else if (reponse == "PSEUDONOTFOUND")
            {
                scoreStatusText.text = "Pseudo inconnu";
            }
            else if (reponse == "SCOREUPDATED")
            {
                scoreStatusText.text = "Score est mis à jour";
                scoreIsNotNull = true;
            }
            else if (reponse == "SCORELOWER")
            {
                scoreStatusText.text = "Ancien score est supérieur";
            }
            else if (reponse == "ERROR")
            {
                scoreStatusText.text = "Pas de connexion";
            }
            else
            {
                scoreStatusText.text = "Erreur inconnue";
            }
        }
        else
        {
            scoreStatusText.text = "Erreur serveur";
        }
    }

    public async void UpdateRank()
    {
        await GetRankFromServerTask();
        rankIsUpdated = true;
    }

    private async Task GetRankFromServerTask()
    {
        url = $"{publicIP}update_rank.php?pseudo={UnityWebRequest.EscapeURL(playerPseudo)}";
        using var www = UnityWebRequest.Get(url);
        www.timeout = 5;
        await www.SendWebRequest();
        string reponse = www.downloadHandler.text.Trim();
        if (www.result == UnityWebRequest.Result.Success || www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (reponse == "INVALID")
            {
                scoreStatusText.text = $"Entrée non-valide !";
            }
            else if (reponse == "PSEUDONOTFOUND")
            {
                scoreStatusText.text = "Pseudo inconnu";
            }
            else if (reponse == "ERROR")
            {
                scoreStatusText.text = "Pas de connexion";
            }
            else
            {
                rankText.text = reponse;
                int newRank = Convert.ToInt32(reponse);
                RankFluctuation(newRank);
                lastLocalRank = newRank;
            }
        }
        else
        {
            scoreStatusText.text = "Erreur serveur";
        }
    }

    private void RankFluctuation(int newRank)
    {
        if (lastLocalRank > 0)
        {
            upAndDown.gameObject.SetActive(true);
            if (newRank > lastLocalRank)
            {
                upAndDown.sprite =  down;
            }
            else if (newRank < lastLocalRank)
            {
                upAndDown.sprite =  up;
            }
            else
            {
                upAndDown.gameObject.SetActive(false);
            }
        }
        else
        {
            upAndDown.gameObject.SetActive(false);
        }
    }

    private async void GetHighestScore()
    {
        await GetHighestScoreTask();
    }

    private async Task GetHighestScoreTask()
    {
        url = $"{publicIP}get_score.php?pseudo={UnityWebRequest.EscapeURL(playerPseudo)}";
        using var www = UnityWebRequest.Get(url);
        www.timeout = 5;
        await www.SendWebRequest();
        string reponse = www.downloadHandler.text.Trim();
        if (www.result == UnityWebRequest.Result.Success || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Requête est passée");
            if (reponse == "INVALID")
            {
                scoreStatusText.text = $"Entrée non-valide !";
            }
            else if (reponse == "PSEUDONOTFOUND")
            {
                scoreStatusText.text = "Pseudo inconnu";
            }
            else if (reponse == "ERROR")
            {
                scoreStatusText.text = "Pas de connexion";
            }
            else
            {
                highestScoreText.text = reponse;
                highestScoreIsUpdated = true;
            }
        }
        else
        {
            scoreStatusText.text = "Erreur serveur";
        }
    }

    public void UpdateLocalScore(int input)
    {
        PlayerPrefs.SetInt("LocalScore", input);
        PlayerPrefs.Save();
        localScore = PlayerPrefs.GetInt("LocalScore");
        lastScoreText.text = localScore.ToString();
    }
}
