using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

namespace ParallaxVisions {
  class EnemySpawnSystem: ComponentSystem {
    struct State {
      public readonly int Length;
      public ComponentDataArray<EnemySpawnCooldown> Cooldown;
      public ComponentDataArray<EnemySpawnSystemState> S;
    }

    [Inject] State m_State;

    public static void SetupComponentData(EntityManager entityManager) {
      var arch = entityManager.CreateArchetype(
        typeof(EnemySpawnCooldown),
        typeof(EnemySpawnSystemState)
      );
      var stateEntity = entityManager.CreateEntity(arch);
      var oldState = Random.state;

      Random.InitState(0xaf77);
      entityManager.SetComponentData(stateEntity, new EnemySpawnCooldown{Value = 0.0f});
      entityManager.SetComponentData(stateEntity, new EnemySpawnSystemState{
        SpawnedEnemyCount = 0,
        RandomState = Random.state
      });
      Random.state = oldState;
    }

    protected override void OnUpdate() {
      float cooldown = m_State.Cooldown[0].Value;

      cooldown = Mathf.Max(0.0f, m_State.Cooldown[0].Value - Time.deltaTime);
      bool spawn = cooldown <= 0.0f;

      if (spawn) {
        cooldown = ComputeCooldown();
      }

      m_State.Cooldown[0] = new EnemySpawnCooldown{Value = cooldown};

      if (spawn) {
        SpawnEnemy();
      }
    }

    void SpawnEnemy() {
      var state = m_State.S[0];
      var oldState = Random.state;
      Random.state = state.RandomState;

      float2 spawnPosition = ComputeSpawnLocation();
      float2 spawnHeading = ComputeSpawnHeading();
      int spawnRank = ComputeRank();
      state.SpawnedEnemyCount++;

      PostUpdateCommands.CreateEntity(AsteroidsBootstrap.BasicEnemyArchetype);
      PostUpdateCommands.SetComponent(new Position2D{Value = spawnPosition});
      PostUpdateCommands.SetComponent(new Heading2D{Value = spawnHeading});
      PostUpdateCommands.SetComponent(default(Enemy));
      PostUpdateCommands.SetComponent(new EnemyRank{Value = spawnRank});
      PostUpdateCommands.SetComponent(new Health{Value = 1.0f});
      PostUpdateCommands.SetComponent(new MoveSpeed{speed = AsteroidsBootstrap.Settings.enemySpeed});
      PostUpdateCommands.SetComponent(new EnemyShot());
      PostUpdateCommands.SetComponent(new Shot {
        TimeToLive = 40f,
        Energy = 1.0f,
      });
      PostUpdateCommands.AddSharedComponent(AsteroidsBootstrap.EnemyLook);

      state.RandomState = Random.state;

      m_State.S[0] = state;
      Random.state = oldState;
    }

    float ComputeCooldown() {
      return 1f;
    }

    int ComputeRank() {
      return Mathf.CeilToInt(Random.value * 3);
    }

    float2 ComputeSpawnLocation() {
      var settings = AsteroidsBootstrap.Settings;

      float r = Random.value;
      float x0 = settings.playfield.xMin;
      float x1 = settings.playfield.xMax;
      float x = (r > 0.5f) ? x1 : x0;
      var y = UnityEngine.Random.Range(settings.playfield.yMin, settings.playfield.yMax);

      return new float2(x, y);
    }

    float2 ComputeSpawnHeading() {
      var settings = AsteroidsBootstrap.Settings;

      float r1 = UnityEngine.Random.Range(-1.0f, 1.0f);
      float r2 = UnityEngine.Random.Range(-1.0f, 1.0f);

      return new float2(r1, r2);
    }
  }
    
}
