using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatedFromCorpseComponent : MonoBehaviour
{
  void Start()
  {
    BotUnitChannel bot_channel = GetComponent<BotUnitChannel>();
    if ( bot_channel != null )
    {
      BotUnitEvent bot_unit_evt = ScriptableObject.CreateInstance<BotUnitEvent>();
      bot_unit_evt.m_Type = BotUnitEvent.Type.Resurrect;
      bot_channel.RaiseUnitEvent( bot_unit_evt );
    }
  }
}
