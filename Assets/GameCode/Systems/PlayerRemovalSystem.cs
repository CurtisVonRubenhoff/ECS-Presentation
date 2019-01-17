using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms2D;

namespace ParallaxVisions {
  class PlayerRemovalSystem: JobComponentSystem {
    public struct BoundaryKillJob: IJobProcessComponentData<Position2D, PlayerInput> {
      public float MinY;
      public float MaxY;
      public float MinX;
      public float MaxX;
    
      public void Execute(
        [ReadOnly] ref Position2D pos,
        [ReadOnly] ref PlayerInput playerInput
      ) {
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

      var boundaryKillJob = new BoundaryKillJob {
        MinY = AsteroidsBootstrap.Settings.playfield.yMin,
        MaxY = AsteroidsBootstrap.Settings.playfield.yMax,
        MinX = AsteroidsBootstrap.Settings.playfield.xMin,
        MaxX = AsteroidsBootstrap.Settings.playfield.xMax
      };

      return boundaryKillJob.Schedule(this, 64, inputDeps);
    }
  }
}
