using UnityEngine;
using UnityEngine.UI;
#if TMP_YG2
using TMPro;
#endif

namespace YG
{
    public class GetPlayerYG2 : MonoBehaviour
    {
        public Text textPlayerName;
#if TMP_YG2
        public TMP_Text TMPPlayerName;
#endif
        public ImageLoadYG imageLoadPlayerPhoto;
    }
}
