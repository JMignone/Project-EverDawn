public class AbilityASC : AnimatorStateController
{
    protected bool previousIsCasting = false;

    protected override void Update()
    {
        base.Update();

        // check if casting ability state as changed
        if (previousIsCasting != unitStats.Stats.IsCastingAbility)
        {
            // !previousIsCasting is a faster way of checking unitStats.Stats.IsCastingAbility in this case
            if (!previousIsCasting == true)
            {
                animator.SetTrigger("Ability");
            }

            previousIsCasting = unitStats.Stats.IsCastingAbility;
        }
    }
}
