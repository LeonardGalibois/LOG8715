using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct InputData : INetworkSerializable
{
    public int tick;
    public Vector2 direction;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref direction);
    }
}

public struct PositionData : INetworkSerializable
{
    public int tick;
    public Vector2 position;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
    }
}

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


    private NetworkVariable<PositionData> m_Position = new NetworkVariable<PositionData>();
    private PositionData LastHandledPosition;
    public Vector2 Position => m_Position.Value.position;
    public Vector2 LocalPosition => m_LocalPositionBuffer[BufferPosition].position;

    private InputData[] m_InputBuffer;
    private PositionData[] m_LocalPositionBuffer;

    private float m_Timer;
    private int m_CurrentTick;
    private float m_TimeBetweenTicks;
    private int m_TickOffset;

    private int BufferPosition => m_CurrentTick % m_InputBuffer.Length;

    private Queue<InputData> m_InputQueue = new Queue<InputData>();

    private void Awake()
    {
        m_GameState = FindObjectOfType<GameState>();
    }

    private void Start()
    {
        m_TimeBetweenTicks = 1.0f / NetworkUtility.GetLocalTickRate();

        int bufferLength = 1024;
        m_InputBuffer = new InputData[bufferLength];
        m_LocalPositionBuffer = new PositionData[bufferLength];
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
        
        while(m_Timer > m_TimeBetweenTicks)
        {
            m_Timer -= m_TimeBetweenTicks;
            m_CurrentTick++;
            HandleTick();
        }
    }

    private void HandleTick()
    {
        // Seul le serveur met à jour la position de l'entite.
        if (IsServer)
        {
            UpdatePositionServer();
        }

        // Seul le client qui possede cette entite peut envoyer ses inputs. 
        if (IsClient && IsOwner)
        {
            Reconciliate();

            UpdateInputClient();

            Vector2 direction = m_InputBuffer[BufferPosition].direction;
            Vector2 lastPosition = m_LocalPositionBuffer[(m_CurrentTick - 1) % m_InputBuffer.Length].position;
            PositionData data = new PositionData
            {
                tick = m_CurrentTick,
                position = UpdatePosition(direction, lastPosition)
            };
            m_LocalPositionBuffer[BufferPosition] = data;
        }
    }

    private void UpdatePositionServer()
    {
        // Mise a jour de la position selon dernier input reçu, puis consommation de l'input
        if (m_InputQueue.Count > 0)
        {
            var input = m_InputQueue.Dequeue();
            PositionData data = new PositionData
            {
                tick = input.tick,
                position = UpdatePosition(input.direction, m_Position.Value.position)
            };
            m_Position.Value = data;
        }
    }

    private void UpdateInputClient()
    {
        Vector2 inputDirection = new Vector2(0, 0);
        if (!GameState.IsStunned && !GameState.IsLocalStunned)
        {
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
        }

        InputData data = new InputData { direction = inputDirection.normalized, tick = m_CurrentTick };
        if(data.direction != Vector2.zero) SendInputServerRpc(data);
        
        m_InputBuffer[BufferPosition] = data;
    }

    private Vector2 UpdatePosition(Vector2 input, Vector2 previousPosition)
    {
        if (GameState.IsStunned || GameState.IsLocalStunned) return previousPosition;
        Vector2 newPosition = previousPosition + input * m_Velocity * m_TimeBetweenTicks;

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
    private void SendInputServerRpc(InputData input)
    {
        // On utilise une file pour les inputs pour les cas ou on en recoit plusieurs en meme temps.
        m_InputQueue.Enqueue(input);
    }

    void Reconciliate()
    {
        if (m_Position.Value.tick == LastHandledPosition.tick) return;


        PositionData data = m_Position.Value;
        LastHandledPosition = data;

        double distanceError = Vector2.Distance(data.position, m_LocalPositionBuffer[data.tick % m_LocalPositionBuffer.Length].position);

        if (distanceError > 0.1)
        {
            Debug.Log($"Server: {data.tick}, Client: {m_CurrentTick}");
            Debug.Log($"Error: {distanceError}");

            m_LocalPositionBuffer[data.tick % m_LocalPositionBuffer.Length] = data;
            for(int tick = data.tick + 1; tick < m_CurrentTick; tick++)
            {
                int currentBufferPosition = tick % m_LocalPositionBuffer.Length;
                int lastBufferPosition = (tick - 1) % m_InputBuffer.Length;

                m_LocalPositionBuffer[currentBufferPosition].position = UpdatePosition(m_InputBuffer[currentBufferPosition].direction, m_LocalPositionBuffer[lastBufferPosition].position);
            }
        }
    }


}
