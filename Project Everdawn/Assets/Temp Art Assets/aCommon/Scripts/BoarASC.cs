public class BoarASC : AnimatorStateController
{
    protected override void Update()
    {
        base.Update();

        animator.SetBool("IsRolling", unitStats.ChargeStats.IsCharging);
    }
}
