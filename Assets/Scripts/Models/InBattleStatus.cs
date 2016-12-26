using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class InBattleStatus : IInBattleStatus {

    public int CurrentHealth { get { return currentHealth; } }
    public int MaxHealth { get { return maxHealth; } }
    public int BaseDamage { get { return damage; } }
    public int Damage { 
        get { return (int) temporalEffects.CalculateEffectResult(DAMAGE_MODIFIER_KEY, (double) damage, onExchange); }
    }
    public bool IsDead { get { return isDead; } }

    [Inject]
    public ResetBattleSignal resetBattleSignal { get; set; }
    [Inject]
    public OneExchangeDoneSignal oneExchangeDoneSignal { get; set; }

    protected int currentHealth = 100;
    protected int maxHealth = 100;
    protected int damage = 5;

    protected bool isDead = false;

    // Must be commutative and associative, otherwise the result is wrong
    private delegate double Operation(double first, double second);

    private Dictionary<int, OneCombo> equippedComboMap = new Dictionary<int, OneCombo>();
    private TemporalEffects temporalEffects = new TemporalEffects();
    private int onExchange = 0;
    private bool postConstructRan = false;

    private string DAMAGE_MODIFIER_KEY = "dmgModifier";
    private string TAKE_DAMAGE_MODIFIER_KEY = "takeDmgModifier";

    public InBattleStatus() {
        temporalEffects.CreateAffectedVariable(
                            TAKE_DAMAGE_MODIFIER_KEY
                            , (double x, double y) => { return x * y; }
                            , TemporalEffects.Category.BUFF
                            , TemporalEffects.Nature.OVERRIDDING
                            , TemporalEffects.PriorityOrder.ASC);
        temporalEffects.CreateAffectedVariable(
                            TAKE_DAMAGE_MODIFIER_KEY
                            , (double x, double y) => { return x * y; }
                            , TemporalEffects.Category.DEBUFF
                            , TemporalEffects.Nature.OVERRIDDING
                            , TemporalEffects.PriorityOrder.DESC);
        temporalEffects.CreateAffectedVariable(
                            DAMAGE_MODIFIER_KEY
                            , (double x, double y) => { return x * y; }
                            , TemporalEffects.Category.BUFF
                            , TemporalEffects.Nature.OVERRIDDING
                            , TemporalEffects.PriorityOrder.DESC);
        temporalEffects.CreateAffectedVariable(
                            DAMAGE_MODIFIER_KEY
                            , (double x, double y) => { return x * y; }
                            , TemporalEffects.Category.DEBUFF
                            , TemporalEffects.Nature.OVERRIDDING
                            , TemporalEffects.PriorityOrder.ASC);

        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 5, 1.3, TemporalEffects.Category.BUFF);
        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 5, 1.3, TemporalEffects.Category.BUFF);
        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 10, 1.5, TemporalEffects.Category.BUFF);
        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 5, 1.3, TemporalEffects.Category.BUFF);
        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 7, 1.7, TemporalEffects.Category.BUFF);
        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 15, 1.1, TemporalEffects.Category.BUFF);
        // temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, 12, 1.2, TemporalEffects.Category.BUFF);
    }

    [PostConstruct]
    public void PostConstruct() {
        // This ran once check is necessary, as the enemy instance of this class
        // will be binded to two different enemy interfaces, thus causing
        // this PostConstruct function to be called twice for enemy instance.
        // If the check is not there, everything will be binded twice
        // and stuff gets messed up
        if (!postConstructRan) {
            postConstructRan = true;
            resetBattleSignal.AddListener(OnResetBattle);
            oneExchangeDoneSignal.AddListener(OnOneExchangeDone);

            BindSignals();
        }
    }

    public void ReceiveDmg(int dmg) {
        currentHealth -= (int) (dmg * 1); // temp 1 for now
        if (currentHealth <= 0) {
            isDead = true;
        }
        FireHealthUpdatedSignal();
    }
    
    public void ResetHealth() {
        currentHealth = maxHealth;
        isDead = false;
        FireHealthUpdatedSignal();
    }

    public void AddToHealth(int addAmount) {
        currentHealth += addAmount;
        if (currentHealth >= maxHealth) {
            currentHealth = maxHealth;
        }
        FireHealthUpdatedSignal();
    }

    public void UpdateReceiveDamageModifier(double modifier, int inEffectExchangeCount) {
        TemporalEffects.Category cate = modifier > 1.0 ? TemporalEffects.Category.DEBUFF : TemporalEffects.Category.BUFF;
        temporalEffects.AddEffect(TAKE_DAMAGE_MODIFIER_KEY, onExchange + inEffectExchangeCount + 1, modifier, cate);
    }

    public void UpdateDealDamageModifier(double modifier, int inEffectExchangeCount) {
        TemporalEffects.Category cate = modifier > 1.0 ? TemporalEffects.Category.BUFF : TemporalEffects.Category.DEBUFF;
        temporalEffects.AddEffect(DAMAGE_MODIFIER_KEY, onExchange + inEffectExchangeCount + 1, modifier, cate);
    }

    public Dictionary<int, OneCombo> GetEquippedCombos() {
        return equippedComboMap;
    }

    public void OnOneExchangeDone(Enum winner, int winningTile) {
        ++onExchange;
    }

    public void UpdateEquipWeapon(List<Weapon> equippedWeaponList) {
        equippedComboMap.Clear();
        ComboListFetcher fetcher = ComboListFetcher.GetInstance();
        foreach(Weapon w in equippedWeaponList) {
            List<int> comboIdList = w.GetComboIdList();
            foreach(int id in comboIdList) {
                equippedComboMap[id] = fetcher.GetComboById(id);
            }
        }
        FireEquipComboUpdatedSignal();
    }

    abstract protected void FireHealthUpdatedSignal();
    abstract protected void FireEquipComboUpdatedSignal();
    abstract protected void BindSignals();

    private void OnResetBattle() {
        ResetHealth();
        temporalEffects.ClearAllEffects();
    }

    private class TemporalEffects {
        public enum Category { BUFF, DEBUFF }
        public enum Nature { OVERRIDDING, CULMULATIVE }
        public enum PriorityOrder { UNORDERED, ASC, DESC }

        private Dictionary<string, EffectsList> buffEffects = new Dictionary<string, EffectsList>();
        private Dictionary<string, EffectsList> debuffEffects = new Dictionary<string, EffectsList>();

        public void CreateAffectedVariable(
                        string varName
                        , Operation operation
                        , Category cate
                        , Nature nat
                        , PriorityOrder order = PriorityOrder.UNORDERED) {
            if (cate == Category.BUFF) {
                buffEffects.Add(varName, new EffectsList(nat, operation, order));
            } else if (cate == Category.DEBUFF){
                debuffEffects.Add(varName, new EffectsList(nat, operation, order));
            }
        }

        public void AddEffect(
                        string varName
                        , int endsOnExchange
                        , double effectValue
                        , Category cate) {
            if (cate == Category.BUFF) {
                buffEffects[varName].Add(new Effect(endsOnExchange, effectValue));
            } else if (cate == Category.DEBUFF){
                debuffEffects[varName].Add(new Effect(endsOnExchange, effectValue));
            }
        }

        public double CalculateEffectResult(
                        string varName
                        , double baseValue
                        , int onExchange
                        ) {
            // Calculate buffs first, then calculate debuffs
            double result = baseValue;

            EffectsList efList;
            if (buffEffects.TryGetValue(varName, out efList)) {
                result = efList.Calculate(result, onExchange);
            }
            if (debuffEffects.TryGetValue(varName, out efList)) {
                result = efList.Calculate(result, onExchange);
            }

            return result;
        }

        public void ClearAllEffects() {
            foreach(KeyValuePair<string, EffectsList> kvp in buffEffects) {
                kvp.Value.Clear();
            }
            foreach(KeyValuePair<string, EffectsList> kvp in debuffEffects) {
                kvp.Value.Clear();
            }
        }

        private class EffectsList {
            private Nature effectNature = Nature.CULMULATIVE;
            private PriorityOrder order = PriorityOrder.UNORDERED;
            private List<Effect> culmulativeList;
            private BinaryHeap<Effect> overridingHeap;
            private IComparer<Effect> comparer = null;
            private Operation op;

            public EffectsList(
                    Nature nat
                    , Operation operation
                    , PriorityOrder order = PriorityOrder.UNORDERED) {
                op = operation;
                effectNature = nat;
                if (nat == Nature.OVERRIDDING && order == PriorityOrder.UNORDERED) {
                    throw new ArgumentException("Effects order cannot be UNORDERED if the nature of effects is OVERRIDDING");
                }
                this.order = order;

                if (nat == Nature.OVERRIDDING) {
                    if (order == PriorityOrder.DESC) {
                        comparer = new DescEffectComparer();
                    } else {
                        comparer = new AscEffectComparer();
                    }
                    overridingHeap = new BinaryHeap<Effect>(comparer);
                } else if (nat == Nature.CULMULATIVE) {
                    culmulativeList = new List<Effect>();
                }
            }

            public void Add(Effect ef) {
                if (effectNature == Nature.CULMULATIVE) {
                    culmulativeList.Add(ef);
                } else if (effectNature == Nature.OVERRIDDING) {
                    overridingHeap.Insert(ef);
                }
            }

            public double Calculate(double baseValue, int onExchange) {

                double result = baseValue;

                if (effectNature == Nature.OVERRIDDING) {

                    Effect ef = overridingHeap.Peek();
                    while (ef != null && ef.EndsOnExchange <= onExchange) {
                        overridingHeap.Pop();
                        ef = overridingHeap.Peek();
                    }

                    if (ef != null) {
                        result = op(result, ef.EffectValue);
                    }

                } else if (effectNature == Nature.CULMULATIVE) {
                    List<int> effectsIndicesToBeRemoved = new List<int>();
                    for (int i = 0; i < culmulativeList.Count; ++i) {
                        if (culmulativeList[i].EndsOnExchange <= onExchange) {
                            effectsIndicesToBeRemoved.Add(i);
                        } else {
                            result = op(result, culmulativeList[i].EffectValue);
                        }
                    }

                    for (int j = effectsIndicesToBeRemoved.Count - 1; j >= 0; --j) {
                        // Remove the items in reversed order so that the indices
                        // won't be affected when an item got removed
                        culmulativeList.RemoveAt(effectsIndicesToBeRemoved[j]);
                    }
                }

                return result;
            }

            public void Clear() {
                if (effectNature == Nature.OVERRIDDING) {
                    overridingHeap.Clear();
                } else if (effectNature == Nature.CULMULATIVE) {
                    culmulativeList.Clear();
                }
            }

            public PriorityOrder GetOrder() { return order; }
        }

        private class Effect : IComparable<Effect> {
            private int m_endsOnExchange;
            public int EndsOnExchange { get { return m_endsOnExchange; } }
            private double m_effectValue;
            public double EffectValue { get { return m_effectValue; } }
            public Effect(int endsOnExchange, double effectValue) {
                m_endsOnExchange = endsOnExchange;
                m_effectValue = effectValue;
            }

            public int CompareTo(Effect other) {
                if (other == null) { throw new ArgumentException("CompareTo received null argument!"); }
                return m_effectValue.CompareTo(other.m_effectValue);
            }

            public int CompareEndingExchangeTo(Effect other) {
                if (other == null) { throw new ArgumentException("CompareTo received null argument!"); }
                return m_endsOnExchange.CompareTo(other.m_endsOnExchange);
            }
        }

        private class DescEffectComparer : IComparer<Effect>  {
            public int Compare(Effect ef1, Effect ef2) {
                int result = ef1.CompareTo(ef2);
                return result == 0 ? ef1.CompareEndingExchangeTo(ef2) : result * -1; // Revert the sign meaning the order is reverted
            }
        }

        private class AscEffectComparer : IComparer<Effect> {
            public int Compare(Effect ef1, Effect ef2) {
                int result = ef1.CompareTo(ef2);
                return result == 0 ? ef1.CompareEndingExchangeTo(ef2) : result;
            }
        }
    }
}
