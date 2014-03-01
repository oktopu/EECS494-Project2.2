using UnityEngine;
using System.Collections;

public class cube_script : MonoBehaviour {
	public enum key_config {up =0, left = 1, down = 2, right = 3}

	public KeyCode inspect = KeyCode.A;
	public KeyCode inspect2 = KeyCode.A;
	public ConsistentKeyboard.Keyboard temp;
	public ConsistentKeyboard.Keyboard temp2;
	// Use this for initialization
	void Start () {
		temp = new ConsistentKeyboard.Keyboard(ConsistentKeyboard.Keyboard.Keyboard_Type.QWERTY, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
		temp2 = new ConsistentKeyboard.Keyboard(temp, KeyCode.P);
	}
	
	// Update is called once per frame
	void Update () {
		inspect = temp.get_key_assignment((int) key_config.up);
		inspect2 = temp2.get_key_assignment((int) key_config.up);

		Vector3 vel = Vector3.zero;

		if(temp.get_key((int) key_config.up)){
			print("UP");
			vel.y += 5;
		}
		if(temp2.get_key((int) key_config.up)){
			print("UP2");
			vel.y += 5;
		}

		if(temp.get_key((int) key_config.down)){
			print("DOWN");
			vel.y -= 5;
		}
		if(temp2.get_key((int) key_config.down)){
			print("DOWN2");
			vel.y -= 5;
		}

		if(temp.get_key((int) key_config.right)){
			print("RIGHT");
			vel.x += 5;
		}
		if(temp2.get_key((int) key_config.right)){
			print("RIGHT2");
			vel.x += 5;
		}

		if(temp.get_key((int) key_config.left)){
			print("LEFT");
			vel.x -= 5;

		}
		if(temp2.get_key((int) key_config.left)){
			print("LEFT2");
			vel.x -= 5;
		}

		rigidbody.velocity = vel;
		if(Input.GetKeyDown(KeyCode.LeftShift)){
			if(temp.current_keyboard() == ConsistentKeyboard.Keyboard.Keyboard_Type.QWERTY){
				temp.keyboard_switch(ConsistentKeyboard.Keyboard.Keyboard_Type.AZERTY);
				temp2.keyboard_switch(ConsistentKeyboard.Keyboard.Keyboard_Type.AZERTY);
			}
			else{
				temp.keyboard_switch(ConsistentKeyboard.Keyboard.Keyboard_Type.QWERTY);
				temp2.keyboard_switch(ConsistentKeyboard.Keyboard.Keyboard_Type.QWERTY);
			}
		}

	}
}
