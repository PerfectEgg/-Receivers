using Assets.Script.AStartPathfinder;
using AStarPathfind;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = System.Random;
using State = Character.State;

namespace Assets
{
    public class EnemyAI
    {
        [SerializeField]
        private float speed = 3.0f;

        [SerializeField]
        private float nextBehaviorTime = 0.5f;

        [SerializeField]
        private float TrackingDistane = 3.0f;

        private GameObject gameObject;

        private Character[] players;

        private Character target;

        private Vector2Int targetPos;

        private float delayTime;
        private Stopwatch sw;
        private float lastTime;
        private State state;

        private Dictionary<State, Action<float>> dicFSM;

        private readonly float oneFlame;

        private Random random;

        private string mapName;

        private List<Node> path;
        private Vector2Int CurrentVector2IntPos => new Vector2Int((int)gameObject.transform.position.x, (int)gameObject.transform.position.y);

        public EnemyAI()
        {
            players = new Character[2];
            target = null;

            targetPos = new Vector2Int();

            delayTime = 0.0f;
            sw.Start();
            lastTime = 0.0f;

            oneFlame = (float)Math.Round((1.0f / 60.0f), 2);
            state = State.Idle;

            random = new Random();
            path = new List<Node>();

            dicFSM = new Dictionary<State, Action<float>>();
            dicFSM.Add(State.Idle, Idle);
            dicFSM.Add(State.Run, Run);
            dicFSM.Add(State.Tracking, Tracking);
        }

        public void SetGameObject(GameObject _gameObject)
        {
            this.gameObject = _gameObject;
        }

        public void SetMapKey(string  key)
        {
            mapName = key;
        }

        public void SetPlayers(Character[] _players)
        {
            players = _players;
        }

        private void Update()
        {
            float curTime = (sw.ElapsedMilliseconds / 1000.0f);
            float dt = curTime - lastTime;

            if (dt <= oneFlame)
                return;

            // TODO
            dicFSM[state]?.Invoke(dt);

            lastTime = curTime;
        }

        private void Idle(float dt)
        {
            delayTime += dt;

            foreach (Character c in players)
            {
                if (Distance(c.Vector2IntPosition, CurrentVector2IntPos) <= TrackingDistane)
                {
                    target = c;

                    state = State.Tracking;

                    return;
                }
            }

            if(delayTime >= nextBehaviorTime)
            {
                AStarPathfinderManager.Instance.GetMapMinMaxPos(mapName, out var minX, out var minY, out var maxX, out var maxY);

                int randomX = random.Next(minX, maxX);
                int randomY = random.Next(minY, maxY);

                targetPos.Set(randomX, randomY);

                state = State.Run;
            }
        }

        private void Run(float dt)
        {
            delayTime = 0.0f;
            path.Clear();
            if(!AStarPathfinderManager.Instance.Pathfind(mapName, CurrentVector2IntPos, targetPos, ref path))
            {
                state = State.Idle;
                return;
            }

            if(path.Count == 0)
            {
                state = State.Idle;
                return;
            }
            var dest = path.First();
            gameObject.transform.Translate(new Vector3(dest.x, dest.y , gameObject.transform.position.z) * speed * Time.deltaTime);

        }

        private void Tracking(float dt)
        {
            delayTime = 0.0f;
            path.Clear();
            if (!AStarPathfinderManager.Instance.Pathfind(mapName, CurrentVector2IntPos, target.Vector2IntPosition, ref path))
            {
                state = State.Idle;
                return;
            }

            if (path.Count == 0)
            {
                state = State.Idle;
                return;
            }
            var dest = path.First();
            gameObject.transform.Translate(new Vector3(dest.x, dest.y, gameObject.transform.position.z) * speed * Time.deltaTime);
        }

        public float Distance(Vector2Int a, Vector2Int b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            return (float)Math.Sqrt(num * num + num2 * num2);
        }
    }
}
