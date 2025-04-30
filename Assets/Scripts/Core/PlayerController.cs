using CMPM.DamageSystem;
using CMPM.Level;
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

        public void StartLevel(int wave) {
            Spellcaster = new SpellCaster(125, 8, Hittable.Team.PLAYER);
            StartCoroutine(Spellcaster.ManaRegeneration());

            Hp         =  new Hittable(100, Hittable.Team.PLAYER, gameObject);
            Hp.OnDeath += Die;
            Hp.team    =  Hittable.Team.PLAYER;

            //to set the players heath, dmg, mana, power, and speed each completed wave
            SetPlayerScale(wave);

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

        public void SetPlayerScale(int wave) {
            Spellcaster.Hp = RPN.Evaluate("95 wave 5 * +", new Dictionary<string, int> { { "wave", wave } });
            Spellcaster.MaxMana = RPN.Evaluate("90 wave 10 * +", new Dictionary<string, int> { { "wave", wave } });
            Spellcaster.ManaReg = RPN.Evaluate("10 wave +", new Dictionary<string, int> { { "wave", wave } });
            Spellcaster.SpellPower = RPN.Evaluate("wave 10 *", new Dictionary<string, int> { { "wave", wave } });
            speed = 5;
        }

        void OnMove(InputValue value) {
            if (GameManager.INSTANCE.State == GameManager.GameState.PREGAME
             || GameManager.INSTANCE.State == GameManager.GameState.GAMEOVER) return;
            unit.movement = value.Get<Vector2>() * speed;
        }

        void Die() {
            GameManager.INSTANCE.SetGameOver();
        }
    }
}