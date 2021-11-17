using UnityEngine;
using UnityEngine.EventSystems;

namespace Menus.UntrimmedVisualizer.Component
{
    public class HideChildrenTExtOnHover : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        // Start is called before the first frame update
        void Start()
        {
            OnPointerExit(null);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Transform p = gameObject.transform.parent;
            transform.SetParent(null,worldPositionStays:false);
            transform.SetParent(p,worldPositionStays:false);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        
        }
    }
}
