
using System.Collections;
using System.Collections.Generic;
//using System.Threading.Tasks;//多线程可以不写
using UnityEngine;
using UnityEngine.UI;

public class article : MonoBehaviour
{

    public float FontMoveSpeed = 15;





    // Start is called before the first frame update
    void Awake()
    {
        this.transform.localPosition = new Vector3(0, -3500, 0);
    }

    // Update is called once per frame
    void Update()
    {
        FontMoveUp();
    }
    private void FontMoveUp()
    {

            float y = this.transform.localPosition.y;
            this.transform.localPosition = new Vector3(0, y + FontMoveSpeed * Time.deltaTime, 0);
  

    }

}