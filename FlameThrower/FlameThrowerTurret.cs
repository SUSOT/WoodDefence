using UnityEngine;

namespace _01_Works.CM._01_Scripts.FlameThrower
{
    public class FlameThrowerTurret : DefTurret, IFuelable, IHealable
    {
        private int _maxFuel;
        private int _attackFuel;

        private int _currentFuel;

        private ParticleSystem _flame;

        private bool _isAttacking;

        private int _haveFuelHash = Animator.StringToHash("HaveFuel");

        [SerializeField] private Vector2 _attackSize, offset;

        public override void Awake()
        {
            base.Awake();
            _flame = GetComponentInChildren<ParticleSystem>();
        }

        private void Start()
        {
            _flame.transform.position = muzzle.position;
        }

        protected override void SetUpTurretSOData()
        {
            base.SetUpTurretSOData();
            _maxFuel = _turretInfo.maxFuel;
            _currentFuel = _turretInfo.maxFuel;
            _attackFuel = _turretInfo.attackFuel;
        }

        private void Update()
        {
            BurntAnimation();

            if (_lastAttackTime + _attackDelay < Time.time)
            {
                _lastAttackTime = Time.time;
                Attack();
            }
        }

        private void Attack()
        {
            if (!isInstalled) return;
            int n = Physics2D.OverlapBox((Vector2)transform.position + offset * transform.right, _attackSize, 0, _filter,
                _targets);

            if (n != 0 && RedutionFuel(_attackFuel))
            {
                _flame.Play();
                for (int i = 0; i < n; i++)
                {
                    if (_targets[i].TryGetComponent(out Enemy enemy))
                        enemy.HealthCompo.TakeDamage(_attack);
                }
            }
            else
            {
                _flame.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        private void BurntAnimation()
        {
            if (_currentFuel > 0)
                _animator.SetBool(_haveFuelHash, true);
            else
                _animator.SetBool(_haveFuelHash, false);
        }

        public bool GetIsMaxFuel()
            => _currentFuel >= _maxFuel;

        public int GetMaxFuel()
            => _maxFuel;

        public int GetFuel()
            => _currentFuel;

        public int FillFuel(int value)
        {
            OnFuelChange?.Invoke();
            return value + _currentFuel - (_currentFuel = Mathf.Min(_currentFuel + value, _maxFuel));
        }

        public bool RedutionFuel(int redutionValue)
        {
            if (_currentFuel < redutionValue) return false;
            int newFuel = _currentFuel - redutionValue;
            _currentFuel = Mathf.Max(0, newFuel);
            OnFuelChange?.Invoke();
            return newFuel >= 0;
        }

        public void TakeHeal(int value)
            => HealthComp.TakeHeal(value);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position + offset * transform.right, _attackSize);
        }
#endif
    }
}