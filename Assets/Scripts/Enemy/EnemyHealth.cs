using UnityEngine;
using UnityEngine.Rendering;

public class EnemyHealth : MonoBehaviour
{
    private int _health = 100;

    enum State
    {
        Alive,
        Dead
    }
    void Start()
    {
        State state = State.Alive;
        Damage.damageDealt += TakeDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            State state = State.Dead;
            Destroy(gameObject);
        }
    }
}
