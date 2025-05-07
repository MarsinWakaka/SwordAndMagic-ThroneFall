using UnityEngine;

namespace GameLogic.MyCamera 
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = .5f;
        [SerializeField] private float edgeScrollThreshold = 0.05f; // Screen % for edge scrolling
        [SerializeField] private bool edgeScrollingEnabled = true;

        [Header("Zoom Settings")] 
        [SerializeField] private float scrollSpeed = 100f;
        [SerializeField] private float minZoom = 1f;
        [SerializeField] private float maxZoom = 20f;

        [Header("Bounds")]
        // [SerializeField] private Vector2 mapSize = new Vector2(100f, 100f);
        // [SerializeField] private bool clampCamera = true;

        private Camera mainCamera;
        private float targetZoom;
        private Vector3 dragOrigin;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            targetZoom = mainCamera.orthographic ? mainCamera.orthographicSize : mainCamera.fieldOfView;
        }

        private void Update()
        {
            HandleMovement();
            HandleZoom();
        }

        private void HandleMovement()
        {
            var moveDirection = Vector3.zero;

            // Keyboard input
            moveDirection.x = Input.GetAxisRaw("Horizontal");
            moveDirection.y = Input.GetAxisRaw("Vertical");

            // Edge scrolling (optional)
            // if(edgeScrollingEnabled)
            // {
            //     Vector2 mousePos = Input.mousePosition;
            //     
            //     if(mousePos.x < Screen.width * edgeScrollThreshold)
            //         moveDirection.x = -1;
            //     else if(mousePos.x > Screen.width * (1 - edgeScrollThreshold))
            //         moveDirection.x = 1;
            //         
            //     if(mousePos.y < Screen.height * edgeScrollThreshold)
            //         moveDirection.z = -1;
            //     else if(mousePos.y > Screen.height * (1 - edgeScrollThreshold))
            //         moveDirection.z = 1;
            // }

            // Normalize diagonal movement
            if(moveDirection.magnitude > 1)
                moveDirection.Normalize();

            // Apply movement
            Vector3 moveAmount = moveDirection * (moveSpeed * Time.deltaTime);
            if(mainCamera.orthographic) moveAmount *= targetZoom; // Scale speed by zoom level

            Vector3 newPosition = transform.position + moveAmount;

            // Camera bounds clamping
            // if(clampCamera)
            // {
            //     float vertExtent = mainCamera.orthographic 
            //         ? mainCamera.orthographicSize 
            //         : targetZoom * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            //     
            //     float horzExtent = vertExtent * Screen.width / Screen.height;
            //
            //     newPosition.x = Mathf.Clamp(newPosition.x, -mapSize.x + horzExtent, mapSize.x - horzExtent);
            //     newPosition.z = Mathf.Clamp(newPosition.z, -mapSize.y + vertExtent, mapSize.y - vertExtent);
            // }

            transform.position = newPosition;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if(scroll == 0) return;

            targetZoom -= scroll * scrollSpeed * Time.deltaTime;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            if(mainCamera.orthographic)
            {
                mainCamera.orthographicSize = targetZoom;
            }
            else
            {
                mainCamera.fieldOfView = targetZoom;
            }
        }

        // Draw camera bounds in editor
        // private void OnDrawGizmosSelected()
        // {
        //     Gizmos.color = Color.cyan;
        //     Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x * 2, 1, mapSize.y * 2));
        // }
    }
}