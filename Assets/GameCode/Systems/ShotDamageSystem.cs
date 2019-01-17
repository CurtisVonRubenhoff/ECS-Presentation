using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms2D;

namespace ParallaxVisions {
  class ShotDamageSystem: JobComponentSystem {
    struct Players {
        public readonly int Length;
        public ComponentDataArray<Health> Health;
        [ReadOnly] public ComponentDataArray<Position2D> Position;
        [ReadOnly] public ComponentDataArray<PlayerInput> PlayerMarker;
    }

    [Inject] Players m_Players;

    struct Enemies {
      public readonly int Length;
      public ComponentDataArray<Health> Health;
      [ReadOnly] public ComponentDataArray<Position2D> Position;
      [ReadOnly] public ComponentDataArray<Enemy> EnemyMarker;
    }

    [Inject] Enemies m_Enemies;

    struct PlayerShotData {
      public readonly int Length;
      public ComponentDataArray<Shot> Shot;
      [ReadOnly] public ComponentDataArray<Position2D> Position;
      [ReadOnly] public ComponentDataArray<PlayerShot> PlayerShotMarker;
    }

    [Inject] PlayerShotData m_PlayerShots;

    struct EnemyShotData {
      public readonly int Length;
      public ComponentDataArray<Shot> Shot;
      [ReadOnly] public ComponentDataArray<Position2D> Position;
      [ReadOnly] public ComponentDataArray<EnemyShot> EnemyShotMarker;
    }

    [Inject] EnemyShotData m_EnemyShots;

    [BurstCompile]
    struct ShotCollisionJob: IJobParallelFor {
      public float CollisionRadiusSquared;

      public ComponentDataArray<Health> health;
      [ReadOnly] public ComponentDataArray<Position2D> Positions;

      [NativeDisableParallelForRestriction]
      public ComponentDataArray<Shot> Shots;

      [NativeDisableParallelForRestriction]
      [ReadOnly] public ComponentDataArray<Position2D> ShotPositions;

      public void Execute(int index) {
        float damage = 0.0f;

        float2 reveiverPos = Positions[index].Value;

        for (int si = 0; si < Shots.Length; ++si) {
          float2 shotPos = ShotPositions[si].Value;
          float2 delta = shotPos - reveiverPos;
          float distSquared = math.dot(delta, delta);

          if (distSquared <= CollisionRadiusSquared) {
            var shot = Shots[si];

            damage += shot.Energy;
            shot.TimeToLive = 0.0f;
            Shots[si] = shot;

            var h = health[index];
            h.Value = math.max(h.Value - damage, 0.0f);
            health[index] = h;
          }
        }
      }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
      var settings = AsteroidsBootstrap.Settings;

      if (settings == null)
        return inputDeps;

      var EnemiesVsPlayers =  new ShotCollisionJob {
        CollisionRadiusSquared = settings.playerCollisionRadius * settings.playerCollisionRadius,
        health = m_Players.Health,
        Positions = m_Players.Position,
        Shots = m_EnemyShots.Shot,
        ShotPositions = m_EnemyShots.Position
      }.Schedule(m_Players.Length, 1, inputDeps);

      var playersVsEnemies = new ShotCollisionJob {
        CollisionRadiusSquared = settings.enemyCollisionRadius * settings.enemyCollisionRadius,
        health = m_Enemies.Health,
        Positions = m_Enemies.Position,
        Shots = m_PlayerShots.Shot,
        ShotPositions = m_PlayerShots.Position
      }.Schedule(m_Enemies.Length, 1, EnemiesVsPlayers);

      return playersVsEnemies;
    }
  }    
}
