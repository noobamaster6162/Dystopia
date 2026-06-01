
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Assets.Scripts
{
    public class WeaponHandling : MonoBehaviour
    {
        private bool _isEquipped;

        [Header("Hold & ADS")]
        [SerializeField] private Vector3 holdOffset;
        [SerializeField] private Vector3 holdRotation;
        [SerializeField] private Vector3 aimPos;
        [SerializeField] private Vector3 aimRotation;
        [SerializeField] private float aimSpeed = 10f;

        [Header("Recoil")]
        [SerializeField] private float recoilX = 3f;
        [SerializeField] private float recoilY = 3f;
        [SerializeField] private float recoilZ = 3f;
        [SerializeField] private float snappiness = 6f;
        [SerializeField] private float returnSpeed = 4f;

        [Header("Camera Shake")]
        [SerializeField] private Camera mainCamera;
        [Range(0.1f, 0.5f)] [SerializeField] private float cameraShakeIntensity = 0.143f;

        private Vector3 _targetPosition;
        private Vector3 _targetAdsRotation;
        private Vector3 _recoilTarget;
        public Vector3 recoilCurrent;

        [Header("Shooting")]
        private bool _isShooting;
        [SerializeField] private int magSize;
        [SerializeField] private int spawnedAmmo;
        [SerializeField] private Transform muzzle;
        [Range(1, 30)] private float _range = 30;

        private int _currentAmmo;
        private float _fireRate;
        private float _barrelTemp;

        [SerializeField] private float fireRate = 0.1f;
        private float _nextFireTime;

        //Show ammunition, weapon health, barrel temp etc
        [SerializeField] private TextMeshProUGUI gunTxt;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletSpeed = 100f;
        [SerializeField] private float bulletLife = 4f;

        void Update()
        {

            Vector3 tarPos;
            Vector3 tarRot;

            if (!_isEquipped) return;

            // FIRING
            if (Mouse.current.leftButton.isPressed && Time.time >= _nextFireTime && _currentAmmo > 0)
            {
                _nextFireTime = fireRate + Time.time;
                Fire();
                ShootBullets();

                _isShooting = true;
            }
            else
            {
                _isShooting = false;

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    Reload(magSize);
                    spawnedAmmo -= _currentAmmo;
                }
            }

            if (spawnedAmmo == 0)
            {
                _isShooting = false;
            }

            // ADS targeting
            if (Mouse.current.rightButton.isPressed)
            {
                tarPos = aimPos;
                tarRot = aimRotation;

                float targetFov = 55f;
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFov, 3f);

            }
            else
            {
                tarPos = holdOffset;
                tarRot = holdRotation;

                float currentFov = 55f;
                mainCamera.fieldOfView = Mathf.Lerp(currentFov, mainCamera.fieldOfView, 3f);
            }

            if (_isShooting)
            {
                _barrelTemp += 10f * Time.deltaTime;
                if (_barrelTemp >= 100f)
                    gunTxt.SetText($"! {_barrelTemp:F0}°C");
            }
            else
            {
                _barrelTemp = Mathf.Max(0f, _barrelTemp - 5f * Time.deltaTime);
            }


            //Recoil Calculations
            float t = 1f - Mathf.Exp(-aimSpeed * Time.deltaTime);
            _targetPosition = Vector3.Lerp(_targetPosition, tarPos, t);
            _targetAdsRotation = Vector3.Lerp(_targetAdsRotation, tarRot, t);

            _recoilTarget = Vector3.Lerp(_recoilTarget, Vector3.zero, returnSpeed * Time.deltaTime);
            recoilCurrent = Vector3.Lerp(recoilCurrent, _recoilTarget, snappiness * Time.deltaTime);

            transform.localPosition = _targetPosition;
            transform.localRotation = Quaternion.Euler(_targetAdsRotation + recoilCurrent);

            UpdateUI();
        }

        public void Fire()
        {
            float multiplier = 2f;
            _recoilTarget.x += Random.Range(-recoilX, recoilX) * multiplier;
            _recoilTarget.y += Random.Range(-recoilY, recoilY) * multiplier;
            _recoilTarget.z += recoilZ * multiplier;

            CameraShake();
        }

        public void SetupWeaponBindings()
        {
            _targetPosition = holdOffset;
            _targetAdsRotation = holdRotation;
            _recoilTarget = Vector3.zero;
            recoilCurrent = Vector3.zero;

            transform.localPosition = holdOffset;
            transform.localRotation = Quaternion.Euler(holdRotation);
            _isEquipped = true;
        }

        public void ClearWeaponBindings()
        {
            _isEquipped = false;
            _recoilTarget = Vector3.zero;
            recoilCurrent = Vector3.zero;
        }

        public void CameraShake()
        {
            if (mainCamera == null) return;
            Vector3 camOffset = recoilCurrent * cameraShakeIntensity;
            mainCamera.transform.localRotation = Quaternion.Euler(camOffset);
        }

        public void ShootBullets()
        {

            _currentAmmo--;

            GameObject newBullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = bulletLife * transform.forward;
            }

            Destroy(newBullet, bulletLife);

        }

        public void Reload(int magSize)
        {
            if (_currentAmmo < magSize)
            {
                _currentAmmo = magSize;
            }
        }

        private void UpdateUI()
        {

            if (gunTxt == null) return;

            if (!transform.gameObject.CompareTag("Gun"))
            {
                gunTxt.SetText("");
            }

            else
            {
                if (_currentAmmo == 0)
                {
                    gunTxt.SetText("RELOAD");
                    gunTxt.color = Color.darkRed;
                } else if (_currentAmmo <= 15)
                {
                    gunTxt.SetText($"{_currentAmmo} (Low Ammo)");
                    gunTxt.color = Color.limeGreen;
                } if (spawnedAmmo == 0)
                {
                    gunTxt.SetText("Ammunition over. Find more ammo");
                }

                else if (_barrelTemp >= 100f)
                {
                    gunTxt.SetText($"! {_barrelTemp:F0}°C");
                    gunTxt.color = Color.yellow;
                }
                else
                {
                    // Displays how many bullets are left
                    gunTxt.SetText($"{_currentAmmo} / {spawnedAmmo}");
                    gunTxt.color = Color.white;
                }
            }

        }

    }
}
