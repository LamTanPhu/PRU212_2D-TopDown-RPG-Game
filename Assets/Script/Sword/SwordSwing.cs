using System.Collections;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    public Animator swordAnimator; // Assign in Inspector
    public Transform sword; // Assign the Sword GameObject (Make sure it's inactive at start)
    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        sword.gameObject.SetActive(true); // Show the sword

        // Get direction based on cursor position
        Vector2 direction = GetCursorDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sword.rotation = Quaternion.Euler(0, 0, angle); // Set sword rotation

        string animationName = GetSwordAnimation(angle);
        swordAnimator.Play(animationName);

        yield return new WaitForSeconds(swordAnimator.GetCurrentAnimatorStateInfo(0).length); // Wait for animation

        sword.gameObject.SetActive(false); // Hide the sword
        isAttacking = false;
    }

    Vector2 GetCursorDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePos - transform.position).normalized;
    }

    string GetSwordAnimation(float angle)
    {
        Debug.Log("Attack Angle: " + angle);

        if (angle >= -22.5f && angle < 22.5f)
            return "SwingRight";
        else if (angle >= 22.5f && angle < 67.5f)
            return "SwingUpRight";
        else if (angle >= 67.5f && angle < 112.5f)
            return "SwingUp";
        else if (angle >= 112.5f && angle < 157.5f)
            return "SwingUpLeft";
        else if (angle >= 157.5f || angle < -157.5f)
            return "SwingLeft";
        else if (angle >= -157.5f && angle < -112.5f)
            return "SwingDownLeft";
        else if (angle >= -112.5f && angle < -67.5f)
            return "SwingDown";
        else if (angle >= -67.5f && angle < -22.5f)
            return "SwingDownRight";

        return "SwingDown"; // Default fallback
    }
}
