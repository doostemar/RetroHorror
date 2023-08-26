using UnityEngine;

public class DebugCommon
{
  static GameObject m_DebugObj;

  public static Transform GetDebugObject()
  {
    if ( m_DebugObj == null )
    {
      m_DebugObj = new GameObject( "Debug" );
    }

    return m_DebugObj.transform;
  }
}
