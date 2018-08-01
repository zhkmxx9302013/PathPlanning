using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

	// Use this for initialization
	float speed = 10;
	public GameObject m_Player;
	private Vector3 pos;
	private Vector3 offset = new Vector3(-8,15,-18);//相机相对于玩家的位置
	private Transform cam_target;
	void Start () {
		 cam_target = m_Player.transform;
	}
	
	// Update is called once per frame
	private void Update() {
		pos = cam_target.position + offset;
        this.transform.position = Vector3.Lerp(this.transform.position, pos, speed*Time.deltaTime);//调整相机与玩家之间的距离
        Quaternion angel = Quaternion.LookRotation(cam_target.position - this.transform.position);//获取旋转角度
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angel, speed * Time.deltaTime);
	}
}
