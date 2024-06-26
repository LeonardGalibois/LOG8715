using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StunInputManager : NetworkBehaviour
{
    [SerializeField]
    private GameState m_GameState;

    private void Update()
    {
        // Seuls les clients peuvent envoyer des inputs.
        if (IsClient)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ActivateStunServerRpc();
                m_GameState.LocalStun();
                Debug.Log($"Stun: {m_GameState.IsLocalStunned}");

            }
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void ActivateStunServerRpc()
    {
        m_GameState.Stun();
    }
}
