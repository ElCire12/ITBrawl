using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectorUI : MonoBehaviour
{
    private CharacterSelectionManager manager = CharacterSelectionManager.Instance;
    private int playerIndex;

    public void Init(CharacterSelectionManager managerRef, int index)
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
