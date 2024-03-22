using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerManager
{
    Character _myPlayer;
    Dictionary<int, Character> _players = new Dictionary<int, Character>();
    GameManager gameManager;
    public static PlayerManager Instance { get; } = new PlayerManager();

    public PlayerManager()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
    }

    public void Add(S_PlayerList packet)
    {
        foreach (S_PlayerList.Player p in packet.players)
        {
            if (p.isSelf)
            {
                //MyPlayer myPlayer = go.AddComponent<MyPlayer>();
                //myPlayer.PlayerId = p.playerId;
                //myPlayer.transform.position = new Vector2(p.posX, p.posY);

                _myPlayer = gameManager.MakeMyPlayer(new Vector2(p.posX, p.posY), p.playerId);
                Camera.main.GetComponent<CameraMove>().SetPlayerTransfrom(_myPlayer.Transform);

                gameManager.enemy.SetTarget(_myPlayer);
            }
            else
            {
                //Player player = go.AddComponent<Player>();
                //player.PlayerId = p.playerId;
                //player.transform.position = new Vector2(p.posX, p.posY);

                var player = gameManager.MakeMyPlayer(new Vector2(p.posX, p.posY), p.playerId);
                _players.Add(p.playerId, player);
                gameManager.enemy.SetTarget(player);
            }
        }
    }

    public void Move(S_BroadcastMove packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            _myPlayer.transform.position = new Vector2(packet.posX, packet.posY);
        }
        else
        {
            Character player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                player.transform.position = new Vector2(packet.posX, packet.posY);
            }
        }
    }

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myPlayer.PlayerId)
            return;
        Object obj = Resources.Load("Player");
        GameObject go = Object.Instantiate(obj) as GameObject;

        Character player = go.AddComponent<Character>();
        player.transform.position = new Vector2(packet.posX, packet.posY);
        _players.Add(packet.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
        }
        else
        {
            Character player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }

}
