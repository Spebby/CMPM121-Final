using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SpellUI : MonoBehaviour {
    public GameObject icon;
    public RectTransform cooldown;
    public TextMeshProUGUI manacost;
    public TextMeshProUGUI damage;
    public GameObject highlight;
    public Spell Spell;
    float _lastTextUpdate;
    const float UPDATE_DELAY = 1;
    public GameObject dropbutton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        _lastTextUpdate = 0;
    }

    public void SetSpell(Spell spell) {
        this.Spell = spell;
        GameManager.Instance.SpellIconManager.PlaceSprite(spell.GetIcon(), icon.GetComponent<Image>());
    }

    // Update is called once per frame
    void Update() {
        if (Spell == null) return;
        if (Time.time > _lastTextUpdate + UPDATE_DELAY) {
            manacost.text   = Spell.GetManaCost().ToString();
            damage.text     = Spell.GetDamage().ToString();
            _lastTextUpdate = Time.time;
        }

        float sinceLast = Time.time - Spell.LastCast;
        float ratio     = sinceLast > Spell.GetCooldown() ? 0 : 1 - sinceLast / Spell.GetCooldown();

        cooldown.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 48 * ratio);
    }
}