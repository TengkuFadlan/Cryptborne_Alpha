using UnityEngine;

public class CharacterSoundSystem : EntitySystem
{
  [Header("Audio Clips")]
  public AudioClip basicAttackSound;
  public AudioClip primarySkillSound;
  public AudioClip secondarySkillSound;
  public AudioClip ultimateSound;
  public AudioClip dodgeSound;
  public AudioClip movementSound;
  public AudioClip hurtSound;

  private AudioSource audioSource;
  private bool isMoving = false;

  protected override void Awake()
  {
    base.Awake();
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
      Debug.LogError("AudioSource component is missing from the GameObject.");
    }
  }

  void OnEnable()
  {
    mainEntity.OnBasicAttackCast += PlayBasicAttackSound;
    mainEntity.OnPrimarySkillCast += PlayPrimarySkillSound;
    mainEntity.OnSecondarySkillCast += PlaySecondarySkillSound;
    mainEntity.OnUltimateCast += PlayUltimateSound;
    mainEntity.OnDodgeCast += PlayDodgeSound;
    mainEntity.OnMoved += HandleMovementSound;
    mainEntity.OnMovementStopped += StopMovementSound;
    mainEntity.OnDeath += StopMovementSound;
    mainEntity.OnHealthDamaged += PlayHurtSound;
  }

  void OnDisable()
  {
    mainEntity.OnBasicAttackCast -= PlayBasicAttackSound;
    mainEntity.OnPrimarySkillCast -= PlayPrimarySkillSound;
    mainEntity.OnSecondarySkillCast -= PlaySecondarySkillSound;
    mainEntity.OnUltimateCast -= PlayUltimateSound;
    mainEntity.OnDodgeCast -= PlayDodgeSound;
    mainEntity.OnMoved -= HandleMovementSound;
    mainEntity.OnMovementStopped -= StopMovementSound;
    mainEntity.OnDeath -= StopMovementSound;
    mainEntity.OnHealthDamaged -= PlayHurtSound;
  }

  private void PlayBasicAttackSound()
  {
    if (audioSource != null && basicAttackSound != null)
    {
      audioSource.PlayOneShot(basicAttackSound);
    }
  }

  private void PlayPrimarySkillSound()
  {
    if (audioSource != null && primarySkillSound != null)
    {
      audioSource.PlayOneShot(primarySkillSound);
    }
  }

  private void PlaySecondarySkillSound()
  {
    if (audioSource != null && secondarySkillSound != null)
    {
      audioSource.PlayOneShot(secondarySkillSound);
    }
  }

  private void PlayUltimateSound()
  {
    if (audioSource != null && ultimateSound != null)
    {
      audioSource.PlayOneShot(ultimateSound);
    }
  }

  private void PlayDodgeSound()
  {
    if (audioSource != null && dodgeSound != null)
    {
      audioSource.PlayOneShot(dodgeSound);
    }
  }

  private void HandleMovementSound(Vector3 movementDirection)
  {
    if (audioSource != null && movementSound != null && !isMoving)
    {
      audioSource.clip = movementSound;
      audioSource.loop = true;
      audioSource.Play();
      isMoving = true;
    }
  }

  private void StopMovementSound()
  {
    if (audioSource != null && isMoving)
    {
      audioSource.Stop();
      isMoving = false;
    }
  }

  // New: Method to play the hurt sound
  private void PlayHurtSound(float damage)
  {
    if (audioSource != null && hurtSound != null)
    {
      audioSource.PlayOneShot(hurtSound);
    }
  }
}