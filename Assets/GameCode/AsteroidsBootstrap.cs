using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine.SceneManagement;

namespace ParallaxVisions {
  public sealed class AsteroidsBootstrap {
    public static EntityArchetype PlayerArchetype;
    public static EntityArchetype BasicEnemyArchetype;
    public static EntityArchetype ShotSpawnArchetype;
    public static EntityArchetype SplitEnemyArchetype;

    public static MeshInstanceRenderer PlayerLook;
    public static MeshInstanceRenderer PlayerShotLook;
    public static MeshInstanceRenderer EnemyLook;

    public static AsteroidsSettings Settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize() {
      //create archetypes for entities we'll spawn
      var entityManager = World.Active.GetOrCreateManager<EntityManager>();

      PlayerArchetype = entityManager.CreateArchetype(
        typeof(Position2D),
        typeof(Heading2D),
        typeof(PlayerInput),
        typeof(Health),
        typeof(TransformMatrix)
      );

      ShotSpawnArchetype = entityManager.CreateArchetype(typeof(ShotSpawnData));

      SplitEnemyArchetype = entityManager.CreateArchetype(
        typeof(EnemySplitData)  
      );

      BasicEnemyArchetype = entityManager.CreateArchetype(
        typeof(Enemy),
        typeof(Health),
        typeof(Position2D),
        typeof(Heading2D),
        typeof(TransformMatrix),
        typeof(MoveSpeed),
        typeof(MoveForward),
        typeof(EnemyRank),
        typeof(EnemyShot),
        typeof(Shot)
      );
    }

    public static void NewGame() {
      var entityManager = World.Active.GetOrCreateManager<EntityManager>();

      Entity player = entityManager.CreateEntity(PlayerArchetype);

      entityManager.SetComponentData(player, new Position2D {Value = new float2(0.0f, 0.0f)});
      entityManager.SetComponentData(player, new Heading2D {Value = new float2(0.0f, 1.0f)});
      entityManager.SetComponentData(player, new Health {Value = Settings.playerInitialHealth});

      entityManager.AddSharedComponentData(player, PlayerLook);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterSceneLoad() {
      var settingsGO = GameObject.Find("Settings");

      if (settingsGO == null) {
          SceneManager.sceneLoaded += OnSceneLoaded;
          return;
      }
      
      InitializeWithScene();
    }
    
    public static void InitializeWithScene() {
      var settingsGO = GameObject.Find("Settings");

      if (settingsGO == null) {
          SceneManager.sceneLoaded += OnSceneLoaded;
          return;
      }
      Settings = settingsGO?.GetComponent<AsteroidsSettings>();
      if (!Settings)
          return;

      PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
      PlayerShotLook = GetLookFromPrototype("PlayerShotRenderPrototype");
      EnemyLook = GetLookFromPrototype("EnemyRenderPrototype");

      EnemySpawnSystem.SetupComponentData(World.Active.GetOrCreateManager<EntityManager>());

      World.Active.GetOrCreateManager<UpdatePlayerHUD>().SetupGameObjects();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
      InitializeWithScene();
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName) {
      var proto = GameObject.Find(protoName);
      var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;

      Object.Destroy(proto);
      return result;
    }
  }
}
