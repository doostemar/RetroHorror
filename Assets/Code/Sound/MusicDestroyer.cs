using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDestroyer : MonoBehaviour
{
  void OnDestroy()
  {
    Destroy( GameObject.FindGameObjectWithTag( "MusicManager" ) );
  }
}
