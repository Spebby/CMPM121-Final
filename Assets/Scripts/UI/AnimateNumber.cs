using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


namespace CMPM.UI {
    public class AnimateNumber : MonoBehaviour {
        [FormerlySerializedAs("color_i")] public Color colorI;
        [FormerlySerializedAs("color_f")] public Color colorF;
        public float fadeDuration;
        Vector3 _initialPosition, _finalPosition; //position to drift to, relative to the gameObject's local origin
        float _fadeStartTime;
        int _fontI;
        int _fontF;
        string _dmg;
        float _timeOffset;

        public void Setup(
            string dmg, Vector3 initPos, Vector3 finPos, int fontI, int fontF, Color colorI,
            Color colorF,
            float duration) {
            _dmg             = dmg;
            _fontI           = fontI;
            _fontF           = fontF;
            _initialPosition = initPos;
            _finalPosition   = finPos;
            this.colorI      = colorI;
            this.colorF      = colorF;
            fadeDuration     = duration;
        }

        void Start() {
            _fadeStartTime                = Time.time;
            GetComponent<TMP_Text>().text = _dmg;
            _timeOffset                   = Random.value;
        }

        void Update() {
            float progress = (Time.time - _fadeStartTime) / fadeDuration;
            if (progress <= 1) {
                transform.position = Vector3.Lerp(_initialPosition, _finalPosition, progress)
                                   + new Vector3(Mathf.Sin((Time.time + _timeOffset) * 10) / 3, 0, 0);
                GetComponent<TMP_Text>().fontSize = Mathf.RoundToInt(progress * _fontF + (1 - progress) * _fontI);
                GetComponent<TMP_Text>().fontMaterial.color = Color.Lerp(colorI, colorF, progress);
            } else Destroy(gameObject);
        }
    }
}