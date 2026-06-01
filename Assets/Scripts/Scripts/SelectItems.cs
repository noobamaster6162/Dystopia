
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.Scripts
{
    public class SelectItems : MonoBehaviour
    {

        [SerializeField] private Transform hand;
        [SerializeField] private Camera mainCam;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float maxDistance = 5f;
        [SerializeField] private float throwForce = 5f;

        //private LayerMask _gunMask;
        private GameObject _heldObject;

        void Update()
        {
            DisplayItemName();

            if (Keyboard.current.eKey.wasPressedThisFrame)
                PickUp();

            if (Keyboard.current.gKey.wasPressedThisFrame)
                Drop();
        }

        private void PickUp()
        {
            if (_heldObject != null) return;

            if (!Physics.Raycast(mainCam.transform.position, mainCam.transform.forward,
                                 out RaycastHit hit, maxDistance)) return;

            GameObject target = hit.transform.gameObject;

            if (!target.CompareTag("Gun") && !target.CompareTag("Pickable")) return;

            _heldObject = target;

            if (_heldObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (_heldObject.TryGetComponent(out Collider col))
                col.enabled = false;

            _heldObject.transform.SetParent(hand, false);

            _heldObject.transform.localPosition = Vector3.zero;
            _heldObject.transform.localRotation = Quaternion.identity;

            // Step 4: Verify it stuck
            //Debug.Log($"After reset: localPos={_heldObject.transform.localPosition}, parent={_heldObject.transform.parent.name}");

            // Step 5: Setup weapon
            if (_heldObject.TryGetComponent(out WeaponHandling handling))
                handling.SetupWeaponBindings();
        }

        private void Drop()
        {
            if (_heldObject == null) return;

            if (_heldObject.TryGetComponent(out WeaponHandling handling))
                handling.ClearWeaponBindings();

            _heldObject.transform.SetParent(null);
            _heldObject.transform.position = mainCam.transform.position + mainCam.transform.forward * 0.5f;

            if (_heldObject.TryGetComponent(out Collider col))
                col.enabled = true;

            if (_heldObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.AddForce(_heldObject.transform.forward * throwForce, ForceMode.Acceleration);
            }

            _heldObject = null;
        }

        private void DisplayItemName()
        {

            bool isLooking = Physics.Raycast(mainCam.transform.position, mainCam.transform.forward,
                out RaycastHit hit, maxDistance);

            if (isLooking && hit.transform.gameObject.CompareTag("Pickable") &&
                hit.transform.gameObject.CompareTag("Gun"))

                text.SetText(hit.transform.gameObject.name);
            else
                text.SetText("");
        }
    }
}
