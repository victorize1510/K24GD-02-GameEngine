using UnityEngine;

public class FullHeal : MonoBehaviour
{
    void OnEnable()
    {
        var pc = GetComponent<PlayerController>();
        if (pc) pc.FullHeal();            // gọi hàm full heal ở Player
    }
}
