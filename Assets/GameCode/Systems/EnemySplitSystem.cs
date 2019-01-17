using Unity.Collections;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine;

namespace ParallaxVisions {
  public class EnemySplitBarrier: BarrierSystem {}

  public class EnemySplitSystem: ComponentSystem {
    struct Data {
      [ReadOnly] public EntityArray Entity;
      [ReadOnly] public ComponentDataArray<EnemyRank> EnemyRank;
      [ReadOnly] public ComponentDataArray<Health> Health;
      [ReadOnly] public ComponentDataArray<Position2D> Position;
      [ReadOnly] public ComponentDataArray<Heading2D> Heading;
    }

    [Inject] private Data m_Enemies;
    [Inject] private EnemySplitBarrier m_EnemySplitBarrier;


    protected override void OnUpdate() {
      for (int i = 0; i < m_Enemies.Entity.Length; ++i) {
        if ((m_Enemies.Health[i].Value < 1.0f)) {
          var rank = m_Enemies.EnemyRank[i].Value;

          if (rank > 1) {
            PostUpdateCommands.CreateEntity(AsteroidsBootstrap.SplitEnemyArchetype);
            PostUpdateCommands.SetComponent(new EnemySplitData {
              Position = new Position2D{Value = m_Enemies.Position[i].Value},
              Heading = new Heading2D{Value = m_Enemies.Heading[i].Value},
              Rank = new EnemyRank {Value = --rank}
            });
          }

          PostUpdateCommands.DestroyEntity(m_Enemies.Entity[i]);
        }
      }
    }
  }
}
