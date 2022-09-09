using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExampleScriptTakenFromMyFightingGame : MonoBehaviour//this script used to has name Enemy
{
    public int maxHealth = 150;
    public static int s_currentHealth;
    public Transform playerPos;
    public static GameObject s_punchHand;
    public GameObject myPunchHand;
    public static GameObject s_uppercutHand;
    public GameObject myUppercutHand;
    public static GameObject s_ultimatePunchHand;
    public GameObject myUltimatePunchHand;
    public static GameObject s_kickLeg;
    public GameObject myKickLeg;
    public Transform punchAttackPointTr;
    public static Vector2 s_punchAttackPointV;
    public LayerMask punchAttackLayerMask;
    public static LayerMask s_punchAttackLayerMask2;
    public delegate void playerTakeDmgDel(int damage);
    public static event playerTakeDmgDel OnPlayerTakeDmg;
    public static bool s_isHandReachedPlayer = false;
    public Transform punchTopAttack;
    public Transform kickTopAttack;
    public Transform uppercutPointTransform;
    public Transform shootTransform;
    public static Transform s_shootTransform;
    public GameObject topPartSquare;
    public static GameObject s_topPartSquare;
    public static bool s_isGrounded = false;
    public bool isEnemyDied = false;
    public EnemyHealthBar enemyHealthBar;
    public static AudioManager s_audioManager;
    public GameObject audioManagerGO;
    bool isHurtingNow = false;
    int hurtBotIndex = 1;
    //float attackRange = .5f;
    //SpriteRenderer[] spriteRenderers;
    private void Awake()
    {
        s_audioManager = audioManagerGO.GetComponent<AudioManager>();
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.buildIndex)
        {
            case 2:
                maxHealth = 150;
                break;
            case 5:
                maxHealth = 175;
                break;
            case 8:
                maxHealth = 200;
                break;
            case 11:
                maxHealth = 225;
                break;
            default:
                Debug.LogWarning("Enemy health = " + maxHealth + "something wrong with scene indexes");
                break;
        }
    }
    void Start()
    {
        s_currentHealth = maxHealth;
        enemyHealthBar.SetBotMaxHealth(maxHealth);
        s_punchHand = myPunchHand;
        s_uppercutHand = myUppercutHand;
        s_kickLeg = myKickLeg;
        s_ultimatePunchHand = myUltimatePunchHand;
        s_punchAttackPointV = punchAttackPointTr.position;
        s_punchAttackLayerMask2 = punchAttackLayerMask;
        s_shootTransform = shootTransform;
        s_topPartSquare = topPartSquare;
        EnemyWalkingState.s_isFacingLeft = true;// <==== it gives right orientation to bullets
        //spriteRenderers = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
    }
    private void OnEnable()
    {
        PlayerCombat.OnPunch += TakeDamage;
        PlayerCombat.OnKick += TakeDamage;
        BulletScript.OnShoot += TakeDamage;
        PlayerCombat.OnUppercut += TakeDamage;
    }
    private void OnDisable()
    {
        PlayerCombat.OnPunch -= TakeDamage;
        PlayerCombat.OnKick -= TakeDamage;
        BulletScript.OnShoot -= TakeDamage;
        PlayerCombat.OnUppercut -= TakeDamage;
    }
    public void TakeDamage(int damage)
    {
        s_audioManager.Play("Damage");
        isHurtingNow = true;
        s_currentHealth -= damage;
        enemyHealthBar.SetBotHealth(s_currentHealth);
        StartCoroutine(PlayHurtAnimation(.4f));// hurt animation
        if (s_currentHealth <= 0)
        {
            Die();
        }
    }
    private void Update()//update needs only to turn off all other pictures except "hurt" picture
    {
        if (!isHurtingNow)
        {
            return;
        }
        for (int i = 0; i < EnemyPictures.s_pictures.Length; i++)
        {
            if (i == 1)
            {
                EnemyPictures.s_pictures[i].SetActive(true);
            }
            else
            {
                EnemyPictures.s_pictures[i].SetActive(false);
            }
        }
    }
    IEnumerator PlayHurtAnimation(float delay)// hurt animation
    {
        foreach (GameObject picture in EnemyPictures.s_pictures)
        {
            picture.SetActive(false);
        }
        EnemyPictures.s_pictures[hurtBotIndex].SetActive(true);
        yield return new WaitForSeconds(delay);
        isHurtingNow = false;
    }
    void Die()
    {
        s_audioManager.Play("Death");
        isEnemyDied = true;
    }
    public static void GiveDamage(int attackType)
    {
        switch (attackType)
        {
            case 0:
                OnPlayerTakeDmg(EnemyPunchState.punchDamage);
                break;
            case 1:
                OnPlayerTakeDmg(EnemyKickState.kickDamage);
                break;
            case 2:
                OnPlayerTakeDmg(EnemyShootState.shootDamage);
                break;
            case 3:
                OnPlayerTakeDmg(EnemyUppercutState.uppercutDamage);
                break;
            case 4:
                OnPlayerTakeDmg(EnemyUltimatePunchState.ultimatePunchDamage);
                break;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            s_isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Ground")
        {
            s_isGrounded = false;
        }
    }
    //private void OnDrawGizmosSelected() // this is to visualise radius of damage hitbox
    //{
    //    if (punchTopAttack == null)
    //    {
    //        return;
    //    }
    //    Gizmos.DrawWireSphere(punchTopAttack.position, attackRange);
    //    Gizmos.DrawWireSphere(kickTopAttack.position, attackRange);
    //    Gizmos.DrawWireSphere(uppercutPointTransform.position, attackRange);
    //}
}

