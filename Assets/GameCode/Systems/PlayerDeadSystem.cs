using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

namespace ParallaxVisions {
  public class RemoveDeadBarrier: BarrierSystem {}

  public class PlayerDeadSystem: JobComponentSystem {
    struct Data {
      [ReadOnly] public EntityArray Entity;
      [ReadOnly] public ComponentDataArray<Health> Health;
    }

    struct PlayerCheck {
      [ReadOnly] public ComponentDataArray<PlayerInput> PlayerInput;
    }

    [Inject] private Data m_Data;
    [Inject] private PlayerCheck m_PlayerCheck;
    [Inject] private RemoveDeadBarrier m_RemoveDeadBarrier;

    [BurstCompile]
    struct RemoveReadJob: IJob {
      public bool playerDead;
      [ReadOnly] public EntityArray Entity;
      [ReadOnly] public ComponentDataArray<Health> Health;
      public EntityCommandBuffer Commands;

      public void Execute() {
        for (int i = 0; i < Entity.Length; ++i) {
          if (Health[i].Value < 1.0f || playerDead) {
            Commands.DestroyEntity(Entity[i]);
          }
        }
      }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
      return new RemoveReadJob {
        playerDead = m_PlayerCheck.PlayerInput.Length == 0,
        Entity = m_Data.Entity,
        Health = m_Data.Health,
        Commands = m_RemoveDeadBarrier.CreateCommandBuffer(),
      }.Schedule(inputDeps);
    }
  }
}
