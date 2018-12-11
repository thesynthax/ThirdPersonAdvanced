/**
*    Copyright (c) 2018-3018 Silvius Inc.
*/

using UnityEngine;

/* About CharacterStates
* -> Contains all variables related to the states of character.
*/

[CreateAssetMenu(fileName = "Character States")]
public class CharacterStates : ScriptableObject 
{
	#region Variables and References
	public int curState = 0;
	public bool onGround;
	#endregion

	#region Methods
	#endregion
}
