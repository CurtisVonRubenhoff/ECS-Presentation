using UnityEngine;

public class AsteroidsSettings: MonoBehaviour {
  public float playerMoveSpeed = 15.0f;
  public float bulletMoveSpeed = 30.0f;
  public float bulletTimeToLive = 2.0f;
  public float playerFireCoolDown = 0.5f;
  public float enemySpeed = 6.0f;
  public float playerInitialHealth = 3.0f;
  public float enemyInitialHealth = 1.0f;
  public float playerShotEnergy = 1.0f;
  public float enemyShotEnergy = 1.0f;
  public float playerCollisionRadius = 1.0f;
  public float enemyCollisionRadius = 1.0f;
  public float playerTurnSpeed = 0f;
  public Rect playfield = new Rect {x = -30.0f, y = -30.0f, width = 60.0f, height = 60.0f};
}
