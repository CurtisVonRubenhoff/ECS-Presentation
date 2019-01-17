using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms2D;

namespace ParallaxVisions
{
  public struct PlayerInput: IComponentData {
    public float Move;
    public float Rotate;
    public float2 Shoot;
    public float FireCooldown;

    public bool Fire => (FireCooldown <= 0.0) && (math.length(Shoot) > 0.5f);
  }

  public struct Shot: IComponentData {
    public float TimeToLive;
    public float Energy;
  }

  public struct ShotSpawnData: IComponentData {
    public Shot Shot;
    public Position2D Position;
    public Heading2D Heading;
  }

  public struct EnemyRank: IComponentData {
    public int Value;
  }

  public struct EnemySplitData: IComponentData {
    public Position2D Position;
    public Heading2D Heading;
    public EnemyRank Rank;
  }

  public struct Health: IComponentData {
    public float Value;
  }

  public struct Enemy: IComponentData {}
  public struct PlayerShot: IComponentData {}
  public struct EnemyShot: IComponentData {}

  public struct EnemySpawnCooldown : IComponentData {
      public float Value;
  }

  public struct EnemySpawnSystemState : IComponentData {
      public int SpawnedEnemyCount;
      public Random.State RandomState;
  }
}
