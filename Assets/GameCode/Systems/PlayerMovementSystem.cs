using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms2D;
using UnityEngine.Experimental.PlayerLoop;

namespace ParallaxVisions
{
    [UpdateBefore(typeof(FixedUpdate))]
    public class PlayerMovementSystem: ComponentSystem {
      public struct Data {
        public readonly int Length;
        public ComponentDataArray<Position2D> Position;
        public ComponentDataArray<Heading2D> Heading;
        public ComponentDataArray<PlayerInput> Input;
      }

      [Inject] private Data m_Data;

      protected override void OnUpdate() {
        var settings = AsteroidsBootstrap.Settings;

        float dt = Time.deltaTime;

        for (int i = 0; i < m_Data.Length; ++i) {
          var position = m_Data.Position[i].Value;
          var playerInput = m_Data.Input[i];
          var heading = CalculateHeading(m_Data.Heading[i].Value, playerInput.Rotate);

          position += dt * (heading * playerInput.Move) * settings.playerMoveSpeed;

          if (math.length(playerInput.Shoot) > 0.5f) {
            playerInput.FireCooldown = settings.playerFireCoolDown;

            PostUpdateCommands.CreateEntity(AsteroidsBootstrap.ShotSpawnArchetype);
            PostUpdateCommands.SetComponent(new ShotSpawnData {
              Shot = new Shot {
                TimeToLive = settings.bulletTimeToLive,
                Energy = settings.playerShotEnergy,
              },
              Position = new Position2D{Value = position},
              Heading = new Heading2D{Value = heading},
            });
          }

          m_Data.Position[i] = new Position2D{Value = position};
          m_Data.Heading[i] = new Heading2D{Value = heading};
          m_Data.Input[i] = playerInput;
        }
      }

      private float2 CalculateHeading(float2 old, float input) {
        var settings = AsteroidsBootstrap.Settings;

        if ((input < 0.01f) && (input > -0.01f))
          return old;

        var angle = math.atan2(old.y, old.x)*180/Mathf.PI;

        Debug.Log(string.Format("vector: {0} angle: {1}", old, angle));

        angle += ((input * -1) * settings.playerTurnSpeed);
        var rads = angle * (Mathf.PI/180);

        Debug.Log(angle);

        return new float2 {x = math.cos(rads), y = math.sin(rads)};
    }
    }
}
  
