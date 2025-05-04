using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectorUI : MonoBehaviour
{
    private CharacterSelectManager manager = CharacterSelectManager.Instance;
    private int playerIndex;

    public void Init(CharacterSelectManager managerRef, int index)
    {
        manager = managerRef;
        playerIndex = index;
    }

    public void OnSubmit(InputAction.CallbackContext ctx)
    {
        //if (!ctx.performed) return;
        //if (playerIndex == 0)
        //    manager.OnPlayer1Confirmed();
        //else if (playerIndex == 1)
        //    manager.OnPlayer2Confirmed();
    }
}
