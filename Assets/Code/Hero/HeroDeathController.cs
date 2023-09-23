using UnityEngine;

public class HeroDeathController : MonoBehaviour
{
  public GameObject m_GameOverUI;

  private void Start()
  {
    HealthChannel health_channel = GetComponent<HealthChannel>();
    health_channel.OnHealthEvent += OnHealthEvent;
  }

  private void OnHealthEvent( HealthEvent health_evt )
  {
    if ( health_evt.m_Type == HealthEvent.Type.Dead )
    {
      GetComponent<HeroMovementShadow>().enabled = false;
      GetComponent<HeroResurrect>().enabled = false;
      Game.GetGameController().GetComponent<ResurrectionSystem>().enabled = false;
      Game.GetGameController().GetComponent<UnitSelectionSystem>().enabled = false;
      m_GameOverUI.SetActive( true );
    }
  }
}
