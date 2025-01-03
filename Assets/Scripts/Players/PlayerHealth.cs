using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public bool isdead { get; private set; }

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;
    [SerializeField] private Slider healthSlider;

    public int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private Movement playerMovement;
    private Animator animator;
    private MapManager mapManager;

    public VectorValue startingPosition; // ��˹����˹��Դ����
    public AudioClip takedamage;
    public AudioSource source;

    const string TOWN_TEXT = "main";
    readonly int DEATH_TRIGGER_HASH = Animator.StringToHash("Death");
    readonly int IDLE_TRIGGER_HASH = Animator.StringToHash("Idle");

    private void Start()
    {
        isdead = false;
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        playerMovement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
        mapManager = GetComponent<MapManager>();

        currentHealth = maxHealth;

        if (healthSlider == null)
        {
            Debug.LogError("HealthSlider reference not set in the Inspector.");
            return;
        }

        UpdateHealthSlider();
    }

    public void UseHealth(int amount)
    {
        if (currentHealth >= amount)
        {
            currentHealth -= amount;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == TOWN_TEXT)
        {
            // ����ͧ�����آ�Ҿ ���駤�ҵ��˹���������͡�Ѻ������ҡ
            transform.position = startingPosition.initialValue;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var enemy = other.gameObject.GetComponent<EnemyAI>() as MonoBehaviour ??
                    other.gameObject.GetComponent<SkeletonAi>() as MonoBehaviour;

        if (enemy != null && canTakeDamage)
        {
            var damageSource = enemy.GetComponent<DamageSource>();
            int damageAmount = (damageSource != null) ? damageSource.damageAmount : 1;
            TakeDamage(damageAmount);
            knockback.GetKnockedBack(other.gameObject.transform, knockBackThrustAmount);
            StartCoroutine(flash.FlashRoutine());
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    public void HealPlayer(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Health recovered: " + amount);
        UpdateHealthSlider();
    }

    public void ResetPlayerHealth()
    {
        currentHealth = maxHealth;
        isdead = false;
        canTakeDamage = true;

        // ��駤�ҵ��˹��Դ����
        transform.position = startingPosition.initialValue;

        // Reset triggers when reviving
        if (animator != null)
        {
            animator.ResetTrigger("Death");
            animator.SetTrigger("Idle"); // Trigger Idle state
            Debug.Log("Player is idle!2");

        }

        // Enable movement and reset to walking animation
        if (playerMovement != null)
        {
            playerMovement.ChangeState(PlayerState.walk);
        }

        UpdateHealthSlider();
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return;

        canTakeDamage = false;
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }

        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();
        source.PlayOneShot(takedamage);
    }

    private void CheckIfPlayerDeath()
    {
        if (currentHealth <= 0 && !isdead)
        {
            isdead = true;
            currentHealth = 0;

            Debug.Log("Setting Death Trigger in Animator");

            animator.SetTrigger("Death");

            // Stop player movement when the player dies
            if (playerMovement != null)
            {
                playerMovement.ChangeState(PlayerState.dead); // Set player's state to dead
            }

            // Wait for death animation to finish before resetting everything
            StartCoroutine(WaitForDeathAndReset());
        }
    }

    private IEnumerator WaitForDeathAndReset()
    {
        // Wait for the death animation to finish
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.IsName("Death") && stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;  // Wait until the next frame
        }

        // After the death animation, start resetting
        StartCoroutine(ResetAfterDeath());
    }

    private IEnumerator ResetAfterDeath()
    {
        // Wait for a short delay to simulate the reset process
        yield return new WaitForSeconds(2f);

        // Reset the player's health, movement, and everything else
        ResetPlayerHealth();

        // Trigger Idle animation only after everything is reset
        if (animator != null)
        {
            animator.SetTrigger("Idle"); // Trigger Idle state after reset
            Debug.Log("Player is idle!1");
        }

        // Reload the scene after everything is reset
        SceneManager.LoadScene(TOWN_TEXT); // Load the scene after death
        GameManager.instance.mapManager.SetFarmOn(true);
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}
