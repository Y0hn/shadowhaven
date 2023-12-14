public class EnemyWeaponStats : EnemyStats
{
    public EnemyStats main;
    protected override void Start() {}
    protected override void Die() {}
    public override void TakeDamage(int damage) {}
    private void Awake()
    {
        this.damage.AddMod(main.DoDamage());
    }
}