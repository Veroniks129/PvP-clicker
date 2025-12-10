using UnityEngine;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
    int count;
    public TMP_Text visible_counter;

    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        ++count;
        visible_counter.text = count.ToString();
    }
}
