using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField]
    private float m_Velocity;

    [SerializeField]
    private float m_Size = 1;

    private GameState m_GameState;

    // GameState peut etre nul si l'entite joueur est instanciee avant de charger MainScene
    private GameState GameState
    {
        get
        {
            if (m_GameState == null)
            {
                m_GameState = FindObjectOfType<GameState>();
            }
            return m_GameState;
        }
    }


    private NetworkVariable<Vector2> m_Position = new NetworkVariable<Vector2>();
    
    private Vector2[] m_InputBuffer;
    private Vector2[] m_LocalPositionBuffer;

    public Vector2 ServerPosition => m_Position.Value;
    public Vector2 LocalPosition => m_LocalPositionBuffer[BufferPosition];
    private int BufferPosition => ClientTick % m_InputBuffer.Length;
    private int LastBufferPosition => BufferPosition == 0 ? m_LocalPositionBuffer.Length - 1: BufferPosition - 1;

    private float m_ClientTime;
    private int ServerTick => (int)(m_GameState.ServerTime.Value * (1.0f / Time.fixedDeltaTime));
    private int ClientTick => (int)(m_ClientTime * (1.0f / Time.fixedDeltaTime)); 

    private Queue<Vector2> m_InputQueue = new Queue<Vector2>();

    private void Awake()
    {
        m_GameState = FindObjectOfType<GameState>();
        m_ClientTime = m_GameState.ServerTime.Value;

        int bufferLength = 1000;//(int)Math.Ceiling( (1.0 / Time.fixedDeltaTime));
        m_InputBuffer = new Vector2[bufferLength];
        m_LocalPositionBuffer = new Vector2[bufferLength];

        Debug.Log($"CurrentRTT: {m_GameState.CurrentRTT}");
        Debug.Log($"fixedDeltaTime: {Time.fixedDeltaTime}");
        Debug.Log($"BufferLength: {bufferLength}");
    }

    private void FixedUpdate()
    {
        m_ClientTime += Time.deltaTime;
        Reconciliate();

        // Si le stun est active, rien n'est mis a jour.
        if (GameState == null || GameState.IsStunned)
        {
            m_LocalPositionBuffer[BufferPosition] = m_LocalPositionBuffer[LastBufferPosition];
            m_InputBuffer[BufferPosition] = Vector2.zero;
            return;
        }

        // Seul le serveur met à jour la position de l'entite.
        if (IsServer)
        {
            UpdatePositionServer();
        }

        // Seul le client qui possede cette entite peut envoyer ses inputs. 
        if (IsClient && IsOwner)
        {
            Debug.Log($"Server Tick: {ServerTick}  Client Tick: {ClientTick}");

            UpdateInputClient();
            m_LocalPositionBuffer[BufferPosition] = UpdatePosition(m_InputBuffer[BufferPosition], m_LocalPositionBuffer[LastBufferPosition]);
        }
    }

    private void UpdatePositionServer()
    {
        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        if (m_InputQueue.Count > 0)
        {
            var input = m_InputQueue.Dequeue();
            m_Position.Value = UpdatePosition(input, m_Position.Value); ;
        }
    }

    private void UpdateInputClient()
    {
        Vector2 inputDirection = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += Vector2.right;
        }
        SendInputServerRpc(inputDirection.normalized);
        m_InputBuffer[BufferPosition] = inputDirection.normalized;
    }

    private Vector2 UpdatePosition(Vector2 input, Vector2 previousPosition)
    {
        Vector2 newPosition = previousPosition + input * m_Velocity * Time.deltaTime;

        var size = GameState.GameSize;
        if (newPosition.x - m_Size < -size.x)
        {
            newPosition = new Vector2(-size.x + m_Size, newPosition.y);
        }
        else if (newPosition.x + m_Size > size.x)
        {
            newPosition = new Vector2(size.x - m_Size, newPosition.y);
        }

        if (newPosition.y + m_Size > size.y)
        {
            newPosition = new Vector2(newPosition.x, size.y - m_Size);
        }
        else if (newPosition.y - m_Size < -size.y)
        {
            newPosition = new Vector2(newPosition.x, -size.y + m_Size);
        }

        return newPosition;
    }

    [ServerRpc]
    private void SendInputServerRpc(Vector2 input)
    {
        // On utilise une file pour les inputs pour les cas ou on en recoit plusieurs en meme temps.
        m_InputQueue.Enqueue(input);
    }

    void Reconciliate()
    {
        int bufferPosition = ServerTick % m_LocalPositionBuffer.Length;
        double distanceError = Vector2.Distance(m_Position.Value, m_LocalPositionBuffer[bufferPosition]);
        
        Debug.Log($"Error: {distanceError}");

        if (distanceError > 0.1)
        {  
            m_LocalPositionBuffer[bufferPosition] = m_Position.Value;
            for(int tick = ServerTick + 1; tick < ClientTick; tick++)
            {
                bufferPosition = tick % m_LocalPositionBuffer.Length;
                m_LocalPositionBuffer[bufferPosition] = UpdatePosition(m_InputBuffer[bufferPosition], m_LocalPositionBuffer[bufferPosition == 0 ? m_LocalPositionBuffer.Length - 1 : bufferPosition - 1]);
            }
        }
    }


}
