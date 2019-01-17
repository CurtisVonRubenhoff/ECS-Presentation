using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms2D;

namespace ParallaxVisions {
  class EntityMovingSystem: JobComponentSystem {
    public struct EntityMoveJob: IJobProcessComponentData<Position2D, Health> {
      public float MinY;
      public float MaxY;
      public float MinX;
      public float MaxX;
    
      public void Execute([ReadOnly] ref Position2D pos, ref Health health) {
        if (pos.Value.y > MaxY) {
          pos.Value.y = MinY;
        } else if (pos.Value.y < MinY) {
          pos.Value.y = MaxY;
        }

        if (pos.Value.x > MaxX) {
          pos.Value.x = MinX;
        } else if (pos.Value.x < MinX) {
          pos.Value.x = MaxX;
        }
      }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
      if (AsteroidsBootstrap.Settings == null)
        return inputDeps;

      var entityMoveJob = new EntityMoveJob {
        MinY = AsteroidsBootstrap.Settings.playfield.yMin,
        MaxY = AsteroidsBootstrap.Settings.playfield.yMax,
        MinX = AsteroidsBootstrap.Settings.playfield.xMin,
        MaxX = AsteroidsBootstrap.Settings.playfield.xMax
      };

      return entityMoveJob.Schedule(this, 64, inputDeps);
    }
  }
}
