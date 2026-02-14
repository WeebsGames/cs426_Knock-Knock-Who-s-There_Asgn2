using UnityEngine;
using UnityEngine.InputSystem;

public class SetupUIController : MonoBehaviour
{
    private NetworkGameManager gameManager;

    private void Start()
    {
        gameManager = NetworkGameManager.Instance;
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<NetworkGameManager>();
        }

        Debug.Log($"[SetupUI] Ready. Found GameManager={(gameManager != null)}");
    }

    private void Update()
    {
        if (gameManager == null) return;
        if (Keyboard.current == null) return;

        if (gameManager.CurrentPhase == GamePhase.Finished)
        {
            // Restart vote in finished phase.
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Debug.Log("[SetupUI] Key input -> RequestRestart()");
                gameManager.RequestRestart();
            }
            return;
        }

        if (gameManager.CurrentPhase != GamePhase.Setup) return;

        // Trap room hotkeys (toggle): 1..9/0 => rooms 0..9
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectTrapRoom(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectTrapRoom(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectTrapRoom(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectTrapRoom(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectTrapRoom(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectTrapRoom(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectTrapRoom(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SelectTrapRoom(7);
        if (Keyboard.current.digit9Key.wasPressedThisFrame) SelectTrapRoom(8);
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SelectTrapRoom(9);

        // Question set hotkeys: Z = A, X = B, C = C
        if (Keyboard.current.zKey.wasPressedThisFrame) SelectQuestionSet(0);
        if (Keyboard.current.xKey.wasPressedThisFrame) SelectQuestionSet(1);
        if (Keyboard.current.cKey.wasPressedThisFrame) SelectQuestionSet(2);

        // Confirm ready
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ConfirmReady();
        }
    }

    public void SelectTrapRoom(int roomIndex)
    {
        if (gameManager == null) return;
        Debug.Log($"[SetupUI] Key input -> SelectTrapRoom({roomIndex})");
        gameManager.SelectTrapRoom(roomIndex);
    }

    public void SelectQuestionSet(int setIndex)
    {
        if (gameManager == null) return;
        Debug.Log($"[SetupUI] Key input -> SelectQuestionSet({setIndex})");
        gameManager.SelectQuestionSet(setIndex);
    }

    public void ConfirmReady()
    {
        if (gameManager == null) return;
        Debug.Log("[SetupUI] Key input -> ConfirmReady()");
        gameManager.ConfirmReady();
    }
}
