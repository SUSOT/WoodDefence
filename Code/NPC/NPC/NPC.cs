using System.Collections.Generic;
using _01_Works.CM._01_Scripts.NPC.States;
using DG.Tweening;
using ObjectPooling;
using UnityEngine;

namespace _01_Works.CM._01_Scripts.NPC.NPC
{
    public abstract class Npc : PoolableMono
    {
        private static int nextSortingOrder = 3;

        private readonly Dictionary<NpcStateType, INpcState> _states = new Dictionary<NpcStateType, INpcState>();

        private INpcState _currentState;
        private Tween _gaugePulseTween;
        private Vector3 _gaugeRootBaseScale;
        private bool _isStateMachineInitialized;
        private bool _hasStarted;

        public NpcFlipComponent FlipCompo { get; private set; }
        public Rigidbody2D RigidbodyCompo { get; private set; }
        public NpcAnimationComponent AnimationCompo { get; private set; }
        public Transform StoragePos { get; protected set; }
        public NpcStateType CurrentState { get; private set; }
        public NpcAction CurrentAction { get; private set; }
        public abstract float MoveSpeed { get; }

        protected SpriteRenderer SpriteRendererCompo { get; private set; }
        protected INpcRole Role { get; set; }

        [field: SerializeField] public Transform GaugeBar { get; private set; }
        [field: SerializeField] public LayerMask WhatIsStorage { get; private set; }

        [SerializeField] private GameObject gaugeBarRoot;
        [SerializeField] private float gaugePulseDuration = 0.5f;
        [SerializeField] private float gaugePulseScale = 0.3f;

        protected virtual void Awake()
        {
            RigidbodyCompo = GetComponent<Rigidbody2D>();
            FlipCompo = GetComponentInChildren<NpcFlipComponent>();
            AnimationCompo = GetComponentInChildren<NpcAnimationComponent>();
            SpriteRendererCompo = GetComponentInChildren<SpriteRenderer>();

            if (gaugeBarRoot != null)
            {
                _gaugeRootBaseScale = gaugeBarRoot.transform.localScale;
            }

            _states[NpcStateType.Idle] = new NpcIdleState(this);
            _states[NpcStateType.Move] = new NpcMoveState(this);
            _states[NpcStateType.Work] = new NpcWorkState(this);
        }

        protected virtual void Start()
        {
            _hasStarted = true;
            InitializeNpc();
        }

        protected virtual void OnEnable()
        {
            if (SpriteRendererCompo != null)
            {
                SpriteRendererCompo.sortingOrder = nextSortingOrder++;
            }

            if (_hasStarted)
            {
                InitializeNpc();
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();

            if (_gaugePulseTween != null && _gaugePulseTween.IsActive())
            {
                _gaugePulseTween.Kill();
            }

            if (gaugeBarRoot != null)
            {
                gaugeBarRoot.transform.localScale = _gaugeRootBaseScale;
            }

            CurrentAction = null;
            _currentState = null;
            _isStateMachineInitialized = false;
        }

        protected abstract void InitializeNpc();

        protected void ResetNpcRuntime()
        {
            StopAllCoroutines();

            if (_gaugePulseTween != null && _gaugePulseTween.IsActive())
            {
                _gaugePulseTween.Kill();
            }

            if (gaugeBarRoot != null)
            {
                gaugeBarRoot.transform.localScale = _gaugeRootBaseScale;
            }

            CurrentAction = null;
            _currentState = null;
            _isStateMachineInitialized = false;
        }

        protected void EnterInitialState(NpcStateType stateType)
        {
            CurrentState = stateType;
            _currentState = _states[stateType];
            _isStateMachineInitialized = true;
            _currentState.Enter();
        }

        public void TransitionState(NpcStateType nextState)
        {
            if (_isStateMachineInitialized == false)
            {
                EnterInitialState(nextState);
                return;
            }

            if (CurrentState == nextState)
            {
                return;
            }

            _currentState.Exit();
            CurrentState = nextState;
            _currentState = _states[nextState];
            _currentState.Enter();
        }

        private void Update()
        {
            if (_isStateMachineInitialized)
            {
                _currentState.Update();
            }
        }

        private void FixedUpdate()
        {
            if (_isStateMachineInitialized)
            {
                _currentState.FixedUpdate();
            }
        }

        public bool TryAcquireNextAction()
        {
            if (CurrentAction != null || Role == null)
            {
                return false;
            }

            if (Role.TryGetNextAction(this, out NpcAction nextAction) == false || nextAction == null)
            {
                return false;
            }

            CurrentAction = nextAction;
            CurrentAction.OnAssigned(this);
            TransitionState(CurrentAction.RequiresMovement ? NpcStateType.Move : NpcStateType.Work);
            return true;
        }

        public void CompleteCurrentAction()
        {
            CurrentAction = null;
            TransitionState(NpcStateType.Idle);
        }

        public void CancelCurrentAction()
        {
            if (CurrentAction != null)
            {
                CurrentAction.OnCancelled(this);
            }

            CurrentAction = null;
            TransitionState(NpcStateType.Idle);
        }

        public Vector2 GetDirectionTo(Vector3 destination)
        {
            Vector2 direction = destination - transform.position;
            return direction.sqrMagnitude <= 0.0001f ? Vector2.zero : direction.normalized;
        }

        public void MoveHorizontally(Vector2 direction, float moveSpeed)
        {
            if (direction.sqrMagnitude > 0.0001f)
            {
                FlipCompo.FaceDirection(direction);
            }

            RigidbodyCompo.linearVelocity = new Vector2(direction.x * moveSpeed, RigidbodyCompo.linearVelocity.y);
        }

        public void StopMovement()
        {
            RigidbodyCompo.linearVelocity = Vector2.zero;
        }

        public void ShowGauge()
        {
            if (gaugeBarRoot != null)
            {
                gaugeBarRoot.SetActive(true);
            }
        }

        public void HideGauge()
        {
            if (gaugeBarRoot != null)
            {
                gaugeBarRoot.SetActive(false);
            }
        }

        protected void SetGaugeValue(float normalizedValue)
        {
            if (!GaugeBar)
            {
                return;
            }

            Vector3 scale = GaugeBar.localScale;
            scale.x = Mathf.Clamp01(normalizedValue);
            GaugeBar.localScale = scale;
        }
    }
}