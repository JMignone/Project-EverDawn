using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPreview : MonoBehaviour
{
    private GameConstants.HEIGHT_ATTACKABLE heightAttackable;
    public GameConstants.HEIGHT_ATTACKABLE HeightAttackable { get { return heightAttackable; } set { heightAttackable = value; } }
    private GameConstants.TYPE_ATTACKABLE typeAttackable;
    public GameConstants.TYPE_ATTACKABLE TypeAttackable { get { return typeAttackable; } set { typeAttackable = value; } }
    private List<GameObject> targets = new List<GameObject>();
    public List<GameObject> Targets { get { return targets; } }

    public void ReduceHighlightIndicator() {
        foreach(GameObject go in targets) {
            IDamageable unit = (go.GetComponent(typeof(IDamageable)) as IDamageable);
            unit.Stats.DecIndicatorNum();
        }
        targets = new List<GameObject>();
    }

    public void OnDestroy() {
        ReduceHighlightIndicator();
    }
}
