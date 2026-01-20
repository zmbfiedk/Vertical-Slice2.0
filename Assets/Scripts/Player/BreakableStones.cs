using UnityEngine;

public class BreakableStones : MonoBehaviour
{

    public enum states
    {
        Intact,
        Cracked,
        Broken
    }
    public states currentState = states.Intact;

    [SerializeField] private int durability = 2;

    [SerializeField] private Sprite[] stateSprites;


    void Start()
    {
        for (int i = 0; i < stateSprites.Length; i++)
        {
            if (i == (int)currentState)
            {
                GetComponent<SpriteRenderer>().sprite = stateSprites[i];
                break;
            }
        }
    }



    void Update()
    {

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (currentState! == states.Broken)
        {
            return;
        }
        else if (currentState == states.Intact || currentState == states.Cracked)
        {
            if (collision.gameObject.CompareTag("PlayerWeapon"))
            {
                durability -= 1;

                if (durability <= 0)
                {
                    currentState = states.Broken;

                    
                }
                else
                {
                    currentState = states.Cracked;
                    // Play cracking animation or effect here
                }
            }
        }
    }
}
