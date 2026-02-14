using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GameHUDController : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text myStateText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text answersText;
    [SerializeField] private TMP_Text choicesText;
    [SerializeField] private TMP_Text setupHelpText;

    private NetworkGameManager gameManager;
    private PlayerNetworkState localPlayer;

    private void Start()
    {
        gameManager = NetworkGameManager.Instance;
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<NetworkGameManager>();
        }
    }

    private void Update()
    {
        if (gameManager == null)
        {
            SetText(headerText, "Game Manager: not found");
            return;
        }

        localPlayer = FindLocalPlayer();
        PlayerNetworkState thief = FindPlayerByRole(PlayerRole.Thief);
        PlayerNetworkState defender = FindPlayerByRole(PlayerRole.Defender);

        UpdateHeader();
        UpdateMyState();
        UpdateQuestionAndAnswers();
        UpdateChoices(thief, defender);
        UpdateSetupHelp();
    }

    private void UpdateHeader()
    {
        string phase = gameManager.CurrentPhase.ToString();
        string setName = GetSetName(gameManager.SelectedQuestionSetIndex);
        SetText(headerText, $"Phase: {phase} | Question Set: {setName}");
    }

    private void UpdateMyState()
    {
        if (localPlayer == null)
        {
            SetText(myStateText, "My State: waiting for local player...");
            return;
        }

        string role = localPlayer.Role.ToString();
        int roomShown = Mathf.Clamp(localPlayer.CurrentRoomIndex + 1, 1, 7);
        SetText(myStateText, $"Role: {role}\nScore: {localPlayer.Score}\nProgress Room: {roomShown}/7");
    }

    private void UpdateQuestionAndAnswers()
    {
        if (gameManager.SelectedQuestionSetIndex < 0)
        {
            SetText(questionText, "Question: Defender has not selected a question set yet.");
            SetText(answersText, string.Empty);
            return;
        }

        if (localPlayer == null)
        {
            SetText(questionText, "Question: waiting for local player...");
            SetText(answersText, string.Empty);
            return;
        }

        if (localPlayer.CurrentRoomIndex >= 6)
        {
            SetText(questionText, "Question: All rooms completed. Go to Vault.");
            SetText(answersText, string.Empty);
            return;
        }

        List<QuestionData> set = QuestionBank.GetQuestionSet(gameManager.SelectedQuestionSetIndex);
        int roomIndex = Mathf.Clamp(localPlayer.CurrentRoomIndex, 0, 5);
        QuestionData q = set[roomIndex];

        SetText(questionText, $"Room {roomIndex + 1} Question:\n{q.questionText}");

        StringBuilder sb = new();
        for (int i = 0; i < q.answers.Length; i++)
        {
            sb.AppendLine($"Door {i}: {q.answers[i]}");
        }
        SetText(answersText, sb.ToString());
    }

    private void UpdateChoices(PlayerNetworkState thief, PlayerNetworkState defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Thief last door: {FormatLastChoice(thief)}");
        sb.AppendLine($"Defender last door: {FormatLastChoice(defender)}");
        SetText(choicesText, sb.ToString());
    }

    private void UpdateSetupHelp()
    {
        if (gameManager.CurrentPhase != GamePhase.Setup)
        {
            SetText(setupHelpText, string.Empty);
            return;
        }

        SetText(
            setupHelpText,
            "Setup Keys\n" +
            "Thief: 1..6 select trap rooms, R ready\n" +
            "Defender: Z/X/C select set A/B/C, R ready"
        );
    }

    private PlayerNetworkState FindLocalPlayer()
    {
        PlayerNetworkState[] all = FindObjectsByType<PlayerNetworkState>(FindObjectsSortMode.None);
        foreach (PlayerNetworkState p in all)
        {
            if (p != null && p.IsOwner) return p;
        }
        return null;
    }

    private PlayerNetworkState FindPlayerByRole(PlayerRole role)
    {
        PlayerNetworkState[] all = FindObjectsByType<PlayerNetworkState>(FindObjectsSortMode.None);
        foreach (PlayerNetworkState p in all)
        {
            if (p != null && p.Role == role) return p;
        }
        return null;
    }

    private static string FormatLastChoice(PlayerNetworkState player)
    {
        if (player == null) return "N/A";
        if (player.LastChosenRoomIndex < 0 || player.LastChosenAnswerIndex < 0) return "none yet";
        return $"Room {player.LastChosenRoomIndex + 1}, Door {player.LastChosenAnswerIndex}";
    }

    private static string GetSetName(int setIndex)
    {
        return setIndex switch
        {
            0 => "A",
            1 => "B",
            2 => "C",
            _ => "Not Selected"
        };
    }

    private static void SetText(TMP_Text target, string value)
    {
        if (target == null) return;
        target.text = value;
    }
}
