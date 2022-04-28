public class WolfASC : AnimatorStateController
{
    private bool previousIsDashing = false;

    protected override void Update()
    {
        base.Update();

        // check if casting ability state as changed
        if (previousIsDashing != unitStats.DashStats.IsDashing)
        {
            // !previousIsDashing is a faster way of checking unitStats.DashStats.IsDashing in this case
            if (!previousIsDashing == true)
            {
                animator.SetTrigger("Ability");
            }

            previousIsDashing = unitStats.DashStats.IsDashing;
        }
    }
}
