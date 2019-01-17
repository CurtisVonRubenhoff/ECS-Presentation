using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Transforms2D;
using Unity.Mathematics;

namespace ParallaxVisions {
    public class SplitSpawnSystem: ComponentSystem {
      public struct Data {
        public readonly int Length;
        public EntityArray SpawnedEntities;
        [ReadOnly] public ComponentDataArray<EnemySplitData> SpawnData;
      }

      [Inject] private Data m_Data;

      protected override void OnUpdate() {
        var em = PostUpdateCommands;

        for (int i = 0; i < m_Data.Length; ++i) {
          var sd = m_Data.SpawnData[i];
          var splitEntity = m_Data.SpawnedEntities[i];

          SpawnEnemy(sd.Position.Value, sd.Heading.Value, sd.Rank.Value, true);
          SpawnEnemy(sd.Position.Value, sd.Heading.Value, sd.Rank.Value, false);
          em.RemoveComponent<EnemySplitData>(splitEntity);
        }
      }

      private void SpawnEnemy(float2 position, float2 heading, int rank,  bool isFirst) {
        var em = PostUpdateCommands;
        heading = heading * ((isFirst) ? 1.0f : -1.0f);

        em.CreateEntity(AsteroidsBootstrap.BasicEnemyArchetype);
        em.SetComponent(new Position2D{Value = position});
        em.SetComponent(new Heading2D{Value = heading});
        em.SetComponent(default(Enemy));
        em.SetComponent(new Health{Value = AsteroidsBootstrap.Settings.enemyInitialHealth});
        em.SetComponent(new MoveSpeed{speed = AsteroidsBootstrap.Settings.enemySpeed});
        em.SetComponent(new EnemyRank{Value = rank});
        em.AddSharedComponent(AsteroidsBootstrap.EnemyLook);
      }
    }
}
