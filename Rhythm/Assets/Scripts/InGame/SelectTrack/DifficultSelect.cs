using UnityEngine;
using UnityEngine.InputSystem;

namespace TrackSelect
{
    public class DifficultSelect : MonoBehaviour
    {
        public int difficult;
        private RectTransform selectPos;
        public Vector3[] difficultPos;

        void Awake()
        {
            selectPos = GetComponent<RectTransform>();
        }

        void Update()
        {
            selectPos.anchoredPosition = Vector2.Lerp(selectPos.anchoredPosition, difficultPos[difficult], Time.deltaTime / 0.1f);
        }
    }
}
