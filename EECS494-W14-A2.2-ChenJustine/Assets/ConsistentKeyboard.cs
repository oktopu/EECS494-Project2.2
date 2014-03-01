using UnityEngine;
using System.Collections;
using System;
using System.IO;


namespace ConsistentKeyboard{
	public class Keyboard{
		
		private int num_rows = 0;							// number of rows in the key_map
		private int[] row_size;								// array containing the size of each row. Rows are numbered from bottom (0) to top
		private KeyCode[][] key_map;						// contains the key code in corresponding spot
		private Vector2 base_config_key_loc;				// contains the base key of the configuration
		private Vector2[] key_configuration_location;		// contains the key in the configuration
		
		public enum Keyboard_Type {none , QWERTY, Dvorak_US, AZERTY}

		private Keyboard_Type current_keyboard_type = Keyboard_Type.none;	//current keyboard type

		Hashtable key_hash = new Hashtable(); 			// contains hash table of each key, maps key code to location in key map
														// key: keycode. Value: location
														// location is Vector2(-1, -1) if not valid in the map

		private bool base_key_set = false;				// important in checking bounds, tells whether or not the location of the key is
														// set yet or not.

/*
		//-----------------------------------Constructors----------------------------------//

		// throws an exception. Needs arguments
		public Keyboard();

		// pass the type of keyboard initially used and the keys and configuration of the keys that you wish to use
		// e.g. pass Keyboard_Type US because using QWERTY US Keyboard
		//		pass WASD keycode because using that configuration of key code
		public Keyboard(Keyboard_Type type, params KeyCode[] key_config);

		// will return a keyboard with the same key configuration and base key will be located based on other_config's current keyboard type
		public Keyboard(Keyboard other_config, KeyCode base_key);


		//------------------------------------User functions------------------------------//

		// returns the current keyboard type
		public Keyboard_Type current_keyboard();

		//returns the current key configuration
		public Vector2[] configuration();

		// switches from one keyboard type to the other
		public void keyboard_switch( Keyboard_Type type);

		public bool get_key(int key_in_config);				// returns Input.GetKey
		public bool get_key_down(int key_in_config);		// returns Input.GetKeyDown
		public bool get_key_up(int key_in_config);			// returns Input.GetKeyUp

		/*
		 *Note to all Users: 
		 *	You must track which key you use when you put in the key configurations, for example, 
		 * 	if you put in 'WASD' in the key_config of the constructor, you must track the second key to be A
		 * 
		 * 	Recomended to use Enum to track this. e.g.
		 * 	public enum key_config {up =0, left = 1, down = 2, right = 3}
		 * 
		 * ConsistentKeyboard.Keyboard temp = new ConsistentKeyboard.Keyboard(ConsistentKeyboard.Keyboard.Keyboard_Type.QWERTY, Keycode.W, Keycode.A, Keycode.S, Keycode.D);	
		 * 
		 * // This will return the corresponding "W" button from the QWERTY keyboard on the Dvorak Keyboard
		 * temp.keyboard_switch(ConsistentKeyboard.Keyboard.keyboard_Type.Dvorak_US);
		 * 
		 * if(temp.get_key(key_config.up)){
		 * 		// DO STUFF
		 * }
		 * 
		 * Please look at the example included in zip for further details.
		 * 
		 * If you wish to add more keyboards, there are 3 places in which you must modify:
		 * 1) Keyboard_Type Enum
		 * 		Declare the kind of keyboard
		 * 2) First switch case in keyboard_switch()
		 * 		Declare the size and parameters of the keyboard
		 * 3) Second switch case in keyboard_switch()
		 * 		Declare the official mappings. See availible key codes here:
		 * 		https://docs.unity3d.com/Documentation/ScriptReference/KeyCode.html
		 * 
		 * Private functions not included in this readme
		 * 
		 */

//================================== CONSTRUCTORS ==============================================//

		public Keyboard() {
			throw new Exception("Error: Not allowed to have an empty constructor");
		} 


		public Keyboard(Keyboard_Type type, params KeyCode[] key_config) {
			constructor_defaults(type);
			if (setup_configuration(key_config) == false){

			}

			//checks to make sure it's not off keyboard, THROW EXCEPTION
			if(configuration_location_check(num_rows, row_size) == false){
				throw new Exception("ERROR: Invalid key configuration given in constructor");
			}
		} 

		public Keyboard(Keyboard other_config, KeyCode base_key){

			constructor_defaults(other_config.current_keyboard());
			key_configuration_location = other_config.configuration();
			base_config_key_loc = (Vector2) key_hash[base_key];

			//checks to make sure it's not off keyboard, THROW EXCEPTION
			if(configuration_location_check(num_rows, row_size) == false){
				throw new Exception("ERROR: Invalid key configuration given in constructor");
			}
				
		}

//================================ USER\PUBLIC FUNCTIONS ==============================================//

		public Keyboard_Type current_keyboard(){
			return current_keyboard_type;
		}
		
		//mostly here for functionality use
		public Vector2[] configuration(){
			return key_configuration_location;
		}

		// here for debugging use.
		public KeyCode get_key_assignment(int key_in_config){
			Vector2 location = key_configuration_location[key_in_config];
			location.x += base_config_key_loc.x;
			location.y += base_config_key_loc.y;
			return key_map[(int)location.x][(int)location.y];
		}
		
		public bool get_key(int key_in_config){
			Vector2 location = key_configuration_location[key_in_config];
			location.x += base_config_key_loc.x;
			location.y += base_config_key_loc.y;
			return Input.GetKey(key_map[(int)location.x][(int)location.y]);
		}
		public bool get_key_down(int key_in_config){
			Vector2 location = key_configuration_location[key_in_config];
			location.x += base_config_key_loc.x;
			location.y += base_config_key_loc.y;
			return Input.GetKeyDown(key_map[(int)location.x][(int)location.y]);
		}
		public bool get_key_up(int key_in_config){
			Vector2 location = key_configuration_location[key_in_config];
			location.x += base_config_key_loc.x;
			location.y += base_config_key_loc.y;
			return Input.GetKeyUp(key_map[(int)location.x][(int)location.y]);
		}

		public bool keyboard_switch( Keyboard_Type type){
			
			// set all keys to a location outside of the map
			//TODO: make this more efficient by going through the keyboard and setting those to -1, -1
			
			int rows = 0;
			int[] size;
			
			switch(type){
			case Keyboard_Type.QWERTY:
				/* 
				 * QWERTYUIOP[]
				 * ASDFGHJKL;'
				 * ZXCVBNM,./
				 */
				rows = 3;
				size = new int[] {10,11,12};
				break;
			case Keyboard_Type.Dvorak_US:
				/* 
				 * ,,.PYFGCRL/=
				 * AOEUIDHTNS-
				 * ;QJKXBMWVZ
				 */
				rows = 3;
				size = new int[] {10,11,12};
				break;
				
			case Keyboard_Type.AZERTY:
				//french keyboard
				/* 
				 * There are technically 12 keys on the second row, but not sure if Unity supports it
				 * 
				 * AZERTYUIOP^$
				 * QSDFGHJKLM
				 * <WXCVBN,;:!
				 */
				rows = 3;
				size = new int[] {11,10,12};
				break;
			default:
				throw new Exception("Invalid Keyboard Type. No changes were applied");
				return false;
				break;
				
			}
			
			//Makes sure the keyboard contains the bound of the location (there is a key that is not off board)
			// not the case firt time setting this up
			
			if(base_key_set){
				if(configuration_location_check(rows, size) == false){
					throw new Exception("ERROR: Can not convert to new keyboard configuration");
					return false;
				}
			}
			else{
				base_key_set = true;
			}
			
			num_rows = rows;
			row_size = size;
			
			key_map = new KeyCode[num_rows][];
			for(int i = 0; i < num_rows; i++){
				key_map[i] = new KeyCode[row_size[i]];
			}
			
			current_keyboard_type = type;
			
			switch(type){
			case Keyboard_Type.QWERTY:
				// map the keys in the key map.
				// US keyboard: 3 rows, 10, 11, 12
				/* 
				 * QWERTYUIOP[]
				 * ASDFGHJKL;'
				 * ZXCVBNM,./
				 */
				key_map[2][0] = KeyCode.Q;
				key_map[2][1] = KeyCode.W;
				key_map[2][2] = KeyCode.E;
				key_map[2][3] = KeyCode.R;
				key_map[2][4] = KeyCode.T;
				key_map[2][5] = KeyCode.Y;
				key_map[2][6] = KeyCode.U;
				key_map[2][7] = KeyCode.I;
				key_map[2][8] = KeyCode.O;
				key_map[2][9] = KeyCode.P;
				key_map[2][10] = KeyCode.LeftBracket;
				key_map[2][11] = KeyCode.RightBracket;
				
				key_map[1][0] = KeyCode.A;
				key_map[1][1] = KeyCode.S;
				key_map[1][2] = KeyCode.D;
				key_map[1][3] = KeyCode.F;
				key_map[1][4] = KeyCode.G;
				key_map[1][5] = KeyCode.H;
				key_map[1][6] = KeyCode.J;
				key_map[1][7] = KeyCode.K;
				key_map[1][8] = KeyCode.L;
				key_map[1][9] = KeyCode.Semicolon;
				key_map[1][10] = KeyCode.Quote;
				
				key_map[0][0] = KeyCode.Z;
				key_map[0][1] = KeyCode.X;
				key_map[0][2] = KeyCode.C;
				key_map[0][3] = KeyCode.V;
				key_map[0][4] = KeyCode.B;
				key_map[0][5] = KeyCode.N;
				key_map[0][6] = KeyCode.M;
				key_map[0][7] = KeyCode.Comma;
				key_map[0][8] = KeyCode.Period;
				key_map[0][9] = KeyCode.Slash;
				
				break;
				
			case Keyboard_Type.Dvorak_US:
				/* 
				 * ',.PYFGCRL/=
				 * AOEUIDHTNS-
				 * ;QJKXBMWVZ
				 */
				key_map[2][0] = KeyCode.Quote;
				key_map[2][1] = KeyCode.Comma;
				key_map[2][2] = KeyCode.Period;
				key_map[2][3] = KeyCode.P;
				key_map[2][4] = KeyCode.Y;
				key_map[2][5] = KeyCode.F;
				key_map[2][6] = KeyCode.G;
				key_map[2][7] = KeyCode.C;
				key_map[2][8] = KeyCode.R;
				key_map[2][9] = KeyCode.L;
				key_map[2][10] = KeyCode.Slash;
				key_map[2][11] = KeyCode.Equals;
				
				key_map[1][0] = KeyCode.A;
				key_map[1][1] = KeyCode.S;
				key_map[1][2] = KeyCode.E;
				key_map[1][3] = KeyCode.U;
				key_map[1][4] = KeyCode.I;
				key_map[1][5] = KeyCode.D;
				key_map[1][6] = KeyCode.H;
				key_map[1][7] = KeyCode.T;
				key_map[1][8] = KeyCode.N;
				key_map[1][9] = KeyCode.S;
				key_map[1][10] = KeyCode.Minus;
				
				key_map[0][0] = KeyCode.Semicolon;
				key_map[0][1] = KeyCode.Q;
				key_map[0][2] = KeyCode.J;
				key_map[0][3] = KeyCode.K;
				key_map[0][4] = KeyCode.X;
				key_map[0][5] = KeyCode.B;
				key_map[0][6] = KeyCode.M;
				key_map[0][7] = KeyCode.W;
				key_map[0][8] = KeyCode.V;
				key_map[0][9] = KeyCode.Z;
				
				break;
				
			case Keyboard_Type.AZERTY:
				//french keyboard
				/* 
				 * AZERTYUIOP^$
				 * QSDFGHJKLM
				 * <WXCVBN,;:!
				 */
				
				key_map[2][0] = KeyCode.A;
				key_map[2][1] = KeyCode.Z;
				key_map[2][2] = KeyCode.E;
				key_map[2][3] = KeyCode.R;
				key_map[2][4] = KeyCode.T;
				key_map[2][5] = KeyCode.Y;
				key_map[2][6] = KeyCode.U;
				key_map[2][7] = KeyCode.I;
				key_map[2][8] = KeyCode.O;
				key_map[2][9] = KeyCode.P;
				key_map[2][10] = KeyCode.Caret;
				key_map[2][11] = KeyCode.Dollar;
				
				key_map[1][0] = KeyCode.Q;
				key_map[1][1] = KeyCode.S;
				key_map[1][2] = KeyCode.D;
				key_map[1][3] = KeyCode.F;
				key_map[1][4] = KeyCode.G;
				key_map[1][5] = KeyCode.H;
				key_map[1][6] = KeyCode.J;
				key_map[1][7] = KeyCode.K;
				key_map[1][8] = KeyCode.L;
				key_map[1][9] = KeyCode.M;
				
				key_map[0][0] = KeyCode.Less;
				key_map[0][1] = KeyCode.W;
				key_map[0][2] = KeyCode.X;
				key_map[0][3] = KeyCode.C;
				key_map[0][4] = KeyCode.V;
				key_map[0][5] = KeyCode.B;
				key_map[0][6] = KeyCode.N;
				key_map[0][7] = KeyCode.Comma;
				key_map[0][8] = KeyCode.Semicolon;
				key_map[0][9] = KeyCode.Colon;
				key_map[0][10] = KeyCode.Equals;
				break;
			}
			return true;
			
		}

//================================== Private Functionality Functions ==============================================//

		//Under the hood stuff really. There's no need for you to be here unless I REALLY screwed up.

		private void constructor_defaults(Keyboard_Type type){
			foreach(KeyCode key in (KeyCode[]) Enum.GetValues(typeof(KeyCode))){
				if(key_hash.ContainsKey(key) == false){
					key_hash.Add (key, new Vector2(-1,-1));
				}
			}
			clear_key_locations();
			keyboard_switch (type);
			
			// assigning key_has values using key_map values
			for (int i = 0; i < num_rows; i ++){
				for (int j = 0; j < row_size[i]; j++){
					key_hash[key_map[i][j]] = new Vector2(i,j);
				}
			}
			
		}

			
		private bool setup_configuration( KeyCode [] keys){

			//set up location of the base key
			base_config_key_loc = (Vector2) key_hash[keys[0]];
			key_configuration_location = new Vector2[keys.Length];
			for (int i = 0; i < keys.Length; i++){
				Vector2 input_key_loc = (Vector2) key_hash[keys[i]];
				if(input_key_loc.x == -1 || input_key_loc.y == -1){
					throw new Exception("ERROR: Key configuration error, invalid keycode for current keyboard type");
					return false;
				}
				key_configuration_location[i] = new Vector2(input_key_loc.x - base_config_key_loc.x, 
				                                        input_key_loc.y - base_config_key_loc.y);
			}
			return true;
			
		}

		private void clear_key_locations(){
			foreach(KeyCode key in (KeyCode[]) Enum.GetValues(typeof(KeyCode))){
				key_hash[key] = new Vector2(-1,-1);
			}
		}

		private bool configuration_location_check(int rows, int[] size){
			for(int i = 0; i < key_configuration_location.Length ; i ++){
				int row_loc = (int) key_configuration_location[i].x + (int) base_config_key_loc.x;
				int col_loc = (int) key_configuration_location[i].y + (int) base_config_key_loc.y;
				if(	row_loc < 0 || row_loc >= rows){ 
					//throw new System.ArgumentException("Parameter cannot be null", "original");
					return false;
				}
				if(	col_loc < 0 || col_loc >= size[row_loc]){
					return false;
				}
			}
			return true;
		}

	}
}