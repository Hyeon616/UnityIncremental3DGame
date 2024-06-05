using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private Text damageText;
    [SerializeField] private float disappearSpeed = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;

    private Color textColor;

    private void Awake()
    {
        textColor = damageText.color;
    }

    public void Setup(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
        textColor = damageText.color;
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        textColor.a -= fadeSpeed * Time.deltaTime;
        damageText.color = textColor;

        if (textColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
