using CMPM.DamageSystem;
using CMPM.Movement;
using CMPM.Spells;
using CMPM.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace CMPM.Core {
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
            unit                        = GetComponent<Unit>();
            GameManager.INSTANCE.Player = gameObject;
        }

        public void StartLevel() {
            Spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
            StartCoroutine(Spellcaster.ManaRegeneration());

            Hp         =  new Hittable(100, Hittable.Team.PLAYER, gameObject);
            Hp.OnDeath += Die;
            Hp.team    =  Hittable.Team.PLAYER;

            // tell UI elements what to show
            healthUI.SetHealth(Hp);
            manaUI.SetSpellCaster(Spellcaster);
            spellui.SetSpell(Spellcaster.Spell);
        }

        void OnAttack(InputValue value) {
            if (GameManager.INSTANCE.State == GameManager.GameState.PREGAME
             || GameManager.INSTANCE.State == GameManager.GameState.GAMEOVER) return;
            Vector2 mouseScreen = Mouse.current.position.value;
            Vector3 mouseWorld  = Camera.main!.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0;
            StartCoroutine(Spellcaster.Cast(transform.position, mouseWorld));
        }

        void OnMove(InputValue value) {
            if (GameManager.INSTANCE.State == GameManager.GameState.PREGAME
             || GameManager.INSTANCE.State == GameManager.GameState.GAMEOVER) return;
            unit.movement = value.Get<Vector2>() * speed;
        }

        void Die() {
            Debug.Log("You Lost");
        }
    }
}