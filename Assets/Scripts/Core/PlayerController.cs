using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour {
    public Hittable Hp;
    [FormerlySerializedAs("healthui")] public HealthBar healthUI;
    [FormerlySerializedAs("manaui")] public ManaBar manaUI;

    public SpellCaster Spellcaster;
    public SpellUI spellui;

    public int speed;

    public Unit unit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        unit                        = this.GetComponent<Unit>();
        GameManager.Instance.Player = gameObject;
    }

    public void StartLevel() {
        Spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
        this.StartCoroutine(Spellcaster.ManaRegeneration());

        Hp         =  new Hittable(100, Hittable.Team.PLAYER, gameObject);
        Hp.OnDeath += this.Die;
        Hp.team    =  Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthUI.SetHealth(Hp);
        manaUI.SetSpellCaster(Spellcaster);
        spellui.SetSpell(Spellcaster.Spell);
    }

    // Update is called once per frame
    void Update() { }

    void OnAttack(InputValue value) {
        if (GameManager.Instance.State == GameManager.GameState.PREGAME
         || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld  = Camera.main!.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        this.StartCoroutine(Spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value) {
        if (GameManager.Instance.State == GameManager.GameState.PREGAME
         || GameManager.Instance.State == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>() * speed;
    }

    void Die() {
        Debug.Log("You Lost");
    }
}