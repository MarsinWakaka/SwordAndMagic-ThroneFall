// using UnityEngine;
//
// namespace GameLogic.Grid.Style
// {
//     public class FloatBehaviour : MonoBehaviour
//     {
//         [SerializeField] private Vector3 floatValue = new Vector3(0, 0.1f, 0);
//         [SerializeField] private float floatSpeed = 1f;
//         
//         private Vector3 _startPosition;
//         private Transform _transform;
//         
//         private void Start()
//         {
//             _transform = transform;
//             _startPosition = _transform.localPosition;
//         }
//         
//         private void Update()
//         {
//             var rate = (Mathf.Sin(Time.time * floatSpeed) + 1) / 2;
//             _transform.localPosition = _startPosition + floatValue * rate; 
//         }
//     }
// }