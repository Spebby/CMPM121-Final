using CMPM.Level;
using CMPM.Relics;
using CMPM.Relics.Effects;
using CMPM.Relics.Expires;
using CMPM.Relics.Triggers;
using CMPM.Spells;
using CMPM.Spells.Modifiers;
using CMPM.Status;
using UnityEditor;
using UnityEngine;


// Mr. Gippity recommends moving this into an "Editor" folder. I'm unsure if I want to do that necessarily. I don't really
// care if this is compiled into a production build, just as long as it's not run.

// The purpose of this script is to force some code into memory so a Debugger can actually use breakpoints on it.
// C# doesn't actually load any of the code until it is referenced, so non-loaded code gets a "no associated module" error.
// This is *ONLY* necessary for non-monobehaviours, as Unity automatically loads all MonoBheaviours at start time.
#if DEBUG || DEVELOPMENT_BUILD || UNITY_EDITOR
namespace CMPM.Utils.Debug {
    [InitializeOnLoad]
    public static class DebugAssemblyWarmupEntry {
        static DebugAssemblyWarmupEntry() {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state) {
            if (state != PlayModeStateChange.EnteredPlayMode) return;
            GameObject go = new("DebugAssemblyWarmup") {
                hideFlags = HideFlags.HideAndDontSave
            };
            go.AddComponent<DebugAssemblyWarmup>();
            Object.DontDestroyOnLoad(go);
        } 
    } 
    
    public class DebugAssemblyWarmup : MonoBehaviour {
        void Awake() {
            #region Relics
            _ = typeof(RelicBuilder);
            _ = typeof(Relic);
            _ = typeof(RelicData);
            _ = typeof(RelicRegistry);
            
                #region Relic Effects
                _ = typeof(IRelicEffect);
                _ = typeof(GainRandomBuff);
                _ = typeof(GainStatEffect);
                #endregion
            
                #region Relic Expires
                _ = typeof(RelicExpire);
                _ = typeof(RelicTimerExpire);
                #endregion
            
                #region Relic Triggers
                _ = typeof(RelicTrigger);
                _ = typeof(RelicStandstill);
                _ = typeof(RelicTimer);
                #endregion
            #endregion

            #region Spells
            _ = typeof(SpellBuilder);
            _ = typeof(SpellCaster);
            _ = typeof(Spell);
            _ = typeof(SpellModifier);
            _ = typeof(SpellModifierData);
            _ = typeof(SpellRegistry);
            _ = typeof(SpellModifierRegistry);
            _ = typeof(SpellModifierDataRegistry);
            
                #region Base Spells
                _ = typeof(ArcaneBolt);
                _ = typeof(ArcaneBlast);
                _ = typeof(ArcaneSpray);
                _ = typeof(MagicMissile);
                _ = typeof(MysticRiver);
                #endregion
                
                #region Spell Modifiers
                _ = typeof(ISpellModifier);
                _ = typeof(SpellModifier);
                _ = typeof(SpellProjectileModifier);
                _ = typeof(SpellRepeaterModifier);
                _ = typeof(SpellSplitModifier);
                _ = typeof(SpellStatModifier);
                #endregion
            #endregion
            
            #region Enemys
            _ = typeof(EnemySpawner);
            _ = typeof(EnemyData);
            #endregion
            
            #region Status
            _ = typeof(IStatusEffect);
            _ = typeof(AbstractStatus);
            _ = typeof(DOTStatus);
            _ = typeof(SlowStatus);
            #endregion
        }
    }
}
#endif
