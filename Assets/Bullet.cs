using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    public static GameObject hitEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(hitEffect == null)
        {
            hitEffect = Resources.Load<GameObject>("prefabs/effects/Holy hit");
        }
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        GameObject sb = Instantiate(hitEffect, transform.position, Quaternion.identity);
        sb.transform.localScale = Vector3.one*.1f;
        Destroy(sb, 1f);
        rb.useGravity = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
