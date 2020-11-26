using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public int move_speed = 50;
	//电脑测试用
	private float mousex = 0;
	private float mousey = 0;
	public float mouseMoveSpeed = 5;
    public float riseSpeed = 1;
    public bool isAutoRotate = false;
    public Transform centerObj;
	public GameObject mirror;

    Vector3 deltaMouse = Vector3.zero;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAutoRotate)
        {
            if (centerObj)
            {
                transform.RotateAround(centerObj.position, Vector3.up, 10f * Time.deltaTime);
                transform.LookAt(centerObj);
            }
            return;
        }
        //抬升/下降高度
        if (Input.GetKey(KeyCode.Z) ) 
		{
            transform.position = new Vector3(transform.position.x, transform.position.y + riseSpeed * Time.deltaTime, transform.position.z);
		}
		if (Input.GetKey(KeyCode.X) )
		{
            transform.position = new Vector3(transform.position.x, transform.position.y - riseSpeed * Time.deltaTime, transform.position.z);
		}

        //前进
        if (Input.GetKey(KeyCode.Keypad8)) 
		{
			this.gameObject.transform.Translate(new Vector3(0, 0, move_speed * Time.deltaTime));
		}
		//后退
		if (Input.GetKey(KeyCode.Keypad2))
		{
			this.gameObject.transform.Translate(new Vector3(0, 0, -1*move_speed * Time.deltaTime));
		}
		//左移
		if (Input.GetKey(KeyCode.Keypad4))
		{
			this.gameObject.transform.Translate(new Vector3(-1* move_speed * Time.deltaTime, 0, 0));
		}
		//右移
		if (Input.GetKey(KeyCode.Keypad6))
		{
			this.gameObject.transform.Translate(new Vector3(move_speed * Time.deltaTime, 0, 0));
		}

		// r f 控制镜子翻转
		if (Input.GetKey(KeyCode.R))
		{
			mirror.transform.Rotate(new Vector3(move_speed * Time.deltaTime, 0, 0));
		}
		if (Input.GetKey(KeyCode.F))
		{
			mirror.transform.Rotate(new Vector3(-move_speed * Time.deltaTime, 0, 0));
		}

		//旋转视角
		Cursor.visible = false;
        mousex += Input.GetAxis("Mouse X") * mouseMoveSpeed;
        mousey += Input.GetAxis("Mouse Y") * mouseMoveSpeed;
        this.transform.rotation = Quaternion.Euler(-mousey, mousex, 0);
    }
}
