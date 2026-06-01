using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private InputReader _input;
    [SerializeField] private Transform _bulletSpawnPosition;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private RectTransform _aim;
    [SerializeField] private float _shootForce;
    [SerializeField] private float _shootPeriod;
    [SerializeField] private float _xOffset;
    [SerializeField] private float _yOffset;

    private bool _isShooting;
    private bool _isReadyToShoot;

    private float _timer;

    private Transform _mainCamera;

    private Ray _ray;

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }

    private void OnEnable()
    {
        _input.Fire += OnFire;
    }

    private void Start()
    {
        _isReadyToShoot = true;
    }

    private void OnDisable()
    {
        _input.Fire -= OnFire;
    }

    private void Update()
    {
        Vector2 crosshaierPos = _aim.anchoredPosition;
        crosshaierPos += new Vector2(Screen.width / _xOffset, Screen.height / _yOffset);
        _ray = Camera.main.ScreenPointToRay(crosshaierPos);

        if (_isShooting)
        {
            Shoot(_ray.direction);
        }
    }

    private void Shoot(Vector3 direction)
    {
        CheckReadiness();

        if (_isReadyToShoot)
        {
            Bullet bullet = Instantiate(_bulletPrefab, _bulletSpawnPosition.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(direction * _shootForce, ForceMode.VelocityChange);
            _isReadyToShoot = false;
        }
    }

    private void CheckReadiness()
    {
        _timer += Time.deltaTime;

        if (_timer >= _shootPeriod)
        {
            _isReadyToShoot = true;
            _timer = 0;
        }
    }

    private void OnFire(bool fire)
    {
        _isShooting = fire;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, _ray.direction * 100);
    }
}
