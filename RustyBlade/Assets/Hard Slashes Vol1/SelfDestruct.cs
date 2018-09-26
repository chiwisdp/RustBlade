using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HardSlashes
{

	public class SelfDestruct : MonoBehaviour {
		public float selfdestruct_in = 4; // Setting this to 0 means no selfdestruct.

		void OnEnable () {
			if ( selfdestruct_in != 0){ 
			 StartCoroutine(	TurnOff ( selfdestruct_in));
			}
		}

		IEnumerator TurnOff(float waitTime){
			yield return new WaitForSeconds(waitTime);
			gameObject.SetActive(false);
		}
	}

}

