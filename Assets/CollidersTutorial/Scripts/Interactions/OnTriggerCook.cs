using UnityEngine;

namespace HumbleGCO.CollidersTutorial
{
    public class OnTriggerCook : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other) 
        {
            if(other.gameObject.name == "Snail")
            {
                other.gameObject.GetComponent<Snail>().Cook();
            }
        }
    }
}
