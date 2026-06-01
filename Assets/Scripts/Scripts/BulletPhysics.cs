
using UnityEngine;
using DefaultNamespace;

public class BulletPhysics : MonoBehaviour
{

    [SerializeField] private BulletType bulletType;
    private float _mass;
    private float _gravity = -9.81f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BulletDrop()
    {

        if (bulletType == BulletType.SevenPointSixTwo)
        {
            _mass = 0.0008f;

            if (transform.gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(Vector3.down * _mass * _gravity, ForceMode.Acceleration);
            }
        }

        if (bulletType == BulletType.FivePointFourFive)
        {
            _mass = 0.0004f;

            if (transform.gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(Vector3.down * _mass * _gravity, ForceMode.Acceleration);
            }
        }

        if (bulletType == BulletType.FiftyCal)
        {
            _mass = 0.046f;

            if (transform.gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(Vector3.down * _mass * _gravity, ForceMode.Acceleration);
            }
        }

        if (bulletType == BulletType.TwelveGauge)
        {
            _mass = 0.03f;

            if (transform.gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(Vector3.down * _mass * _gravity, ForceMode.Acceleration);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
