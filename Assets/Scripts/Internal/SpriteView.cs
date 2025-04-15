using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SpriteView : MonoBehaviour {
    public TextMeshProUGUI label;

    public Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void Apply(string newLabel, Sprite sprite) {
        label.text   = newLabel;
        image.sprite = sprite;
    }
}