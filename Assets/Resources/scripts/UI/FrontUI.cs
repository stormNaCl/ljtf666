using TMPro;
using UnityEngine;

public class FrontUI : UIbase
{
    public TMP_Text time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = transform.Find("jsq").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
