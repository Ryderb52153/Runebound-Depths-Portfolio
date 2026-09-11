using UnityEngine;


// Skill Sword throw removed. 
// Not much do to with it and new player assets dont support.

public class Skill_SwordThrow : Skill_Base
{
    [Header("Regular Sword Upgrades")]
    //[SerializeField] private Skill_Object_Sword swordPrefab;
    [Range(0,10)]
    //[SerializeField] private float regularThrowPower = 5;

    [Header("Pierce Sword Upgrades")]
    //[SerializeField] private Skill_Object_Sword pierceSwordPrefab;
    public int pieceSwordCount = 3;
    [Range(0, 10)]
    //[SerializeField] private float pierceThrowPower = 5;

    [Header("Spin Sword Upgrades")]
    //[SerializeField] private Skill_Object_Sword spinSwordPrefab;
    public int maxDistance = 5;
    public float attacksPerSecond = 2;
    public float maxSpinDuration = 3f;
    [Range(0, 10)]
    //[SerializeField] private float spinThrowPower = 5;

    [Header("Bounce Sowrd Upgrades")]
    //[SerializeField] private Skill_Object_Sword bounceSwordPrefab;
    public int bounceCount = 3;
    public float bounceSpeed = 12;
    [Range(0, 10)]
    //[SerializeField] private float bounceThrowPower = 5;

    [Header("Trajectory prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [SerializeField] private float spaceBetweenDots = 0.05f;

    private float swordGravity = 3.5f;
    private Transform[] dots;
    private Vector2 confirmedDirection;
    private Skill_Object_Sword currentSword;
    private float currentThrowPower;

    protected override void Awake()
    {
        base.Awake();
        //swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
        //skillType = Skill_Type.SwordThrow;
    }

    public void ThrowSword()
    {
        Skill_Object_Sword prefab = GetSwordPrefab();
        Skill_Object_Sword sword = ObjectPoolManager.SpawnObject<Skill_Object_Sword>(prefab, dots[1].position, Quaternion.identity);
        currentSword = sword;
        currentSword.SetupSword(this, GetThrowPower());
        UseSkill();
    }

    private Skill_Object_Sword GetSwordPrefab()
    {
/*        if(Unlocked(Skill_Upgrade_Type.SwordThrow))
            return swordPrefab;
        else if(Unlocked(Skill_Upgrade_Type.SwordThrow_Pierce))
            return pierceSwordPrefab;
        else if(Unlocked(Skill_Upgrade_Type.SwordThrow_Spin))
            return spinSwordPrefab;
        else if(Unlocked(Skill_Upgrade_Type.SwordThrow_Bounce))
            return bounceSwordPrefab;*/

        Debug.Log("Sword prefab not found for upgrade type: " + upgradeType);
        return null;
    }

    public override bool CanUseSkill()
    {
        UpdateThrowPower();

        if (currentSword != null)
        {
            currentSword.ReturnSwordToPlayer();
            currentSword = null;
            return false;
        }

        return base.CanUseSkill();
    }

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            float t = i * spaceBetweenDots;
            dots[i].position = GetTrajectoryPoint(direction, t);
        }
    }

    public void ConfirmTrajector(Vector2 direction) => confirmedDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach (Transform transform in dots)
            transform.gameObject.SetActive(enable);
    }

    private Vector2 GetThrowPower() => confirmedDirection * (currentThrowPower * 10);

    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowPower * 10;
        Vector2 initialVelocity = direction * scaledThrowPower;

        // Gravity pulls the sword down over time. The longer it's in the air, the more it drops.
        Vector2 gravityEFfect = .5f * Physics.gravity * swordGravity * t * t;

        Vector2 predictedPoint = initialVelocity * t + gravityEFfect;
        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;
            newDots[i].gameObject.SetActive(false);
        }

        return newDots;
    }

    private void UpdateThrowPower()
    {
/*        switch (upgradeType)
        {
            case Skill_Upgrade_Type.SwordThrow:
                currentThrowPower = regularThrowPower;
                break;
            case Skill_Upgrade_Type.SwordThrow_Pierce:
                currentThrowPower = pierceThrowPower;
                break;
            case Skill_Upgrade_Type.SwordThrow_Spin:
                currentThrowPower = spinThrowPower;
                break;
            case Skill_Upgrade_Type.SwordThrow_Bounce:
                currentThrowPower = bounceThrowPower;
                break;
            default:
                currentThrowPower = regularThrowPower;
                break;
        }*/
    }
}
