using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : PersistableObject
{
    public static Game Instance { get; private set; }

    [SerializeField] ShapeFactory[] shapeFactories;
    [SerializeField] PersistentStorage storage;

    [SerializeField] KeyCode createKey = KeyCode.C;
    [SerializeField] KeyCode newGameKey = KeyCode.N;
    [SerializeField] KeyCode saveKey = KeyCode.S;
    [SerializeField] KeyCode loadKey = KeyCode.L;
    [SerializeField] KeyCode destroyKey = KeyCode.X;

    //public SpawnZone spawnZoneOfLevel { get; set; } //이제 게임이 spawnzone을 가질 필요 없음. Gamelevel이 spawnpoint를 대리하므로
    [SerializeField] float CreationSpeed { get; set; }
    [SerializeField] float DestructionSpeed { get; set; }
    float creationProgress;
    float destructionProgress;
    [SerializeField] Slider creationSpeedSlider;
    [SerializeField] Slider destructionSpeedSlider;
    Random.State mainRandomState;
    [SerializeField] bool reseedOnLoad; //게임을 시작할때 seed를 새로 할당할 것인지, 재사용할 것인지를 optional로 둠

    bool inGameUpdateLoop;

    [SerializeField] int levelCount;
    int loadLevelBuildIndex;

    List<Shape> shapes;
    List<ShapeInstance> killList, markAsDyingList;
    int dyingShapeCount; //죽어가고있는(?) shape을 앞에, 나머지 shape을 뒤에 두려 함
    [SerializeField] float destroyDuration;

    const int saveVersion = 6;

    private void OnEnable()
    {
        Instance = this;
        if(shapeFactories[0].FactoryId != 0)
        {
            for (int i = 0; i < shapeFactories.Length; i++)
            {
                shapeFactories[i].FactoryId = i;
            }
        }
    }

    private void Start()
    {
        //새 게임을 시작하면 완전히 새로운 random sequence를 사용하도록
        mainRandomState = Random.state;

        shapes = new List<Shape>();
        killList = new List<ShapeInstance>();
        markAsDyingList = new List<ShapeInstance>();

        if (Application.isEditor)
        {
            for(int i=0;i<SceneManager.sceneCount;i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if(loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        Debug.Log("Loading Scene " + levelBuildIndex.ToString());
        this.enabled = false;
        if(loadLevelBuildIndex > 0)
        {
            Debug.Log("Unload Scene " + loadLevelBuildIndex.ToString());
            yield return SceneManager.UnloadSceneAsync(loadLevelBuildIndex);
        }
        
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadLevelBuildIndex = levelBuildIndex;
        this.enabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            GameLevel.Current.SpawnShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(loadLevelBuildIndex));
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        inGameUpdateLoop = true;
        // update 해야 할 object들이 fully reference되어 있을때는 할 만한 작업
        for (int i=0;i<shapes.Count;i++)
        {
            shapes[i].GameUpdate();
        }
        inGameUpdateLoop = false;

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            GameLevel.Current.SpawnShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }

        int limit = GameLevel.Current.PopulationLimit;
        if(limit > 0)
        {
            while(shapes.Count - dyingShapeCount > limit) //죽는 중인 shape들은 limit count에서 제외
            {
                DestroyShape();
            }
        }
        
        //미뤄놓았던 kill의 처리
        if(killList.Count > 0)
        {
            for(int i=0;i<killList.Count;i++)
            {
                if(killList[i].IsValid)
                {
                    KillImmediately(killList[i].Shape);
                }
                
            }
            killList.Clear();
        }

        if(markAsDyingList.Count > 0)
        {
            for(int i=0;i<markAsDyingList.Count;i++)
            {
                if(markAsDyingList[i].IsValid)
                {
                    MarkAsDyingImmediately(markAsDyingList[i].Shape);
                }
            }
            markAsDyingList.Clear();
        }
    }

    private void BeginNewGame()
    {
        
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledDeltaTime; //bitwise or과 time을 사용해 랜덤 강화
        mainRandomState = Random.state;
        Random.InitState(seed);

        //slider에 적용은 매뉴얼하게 해 주어야 함
        creationSpeedSlider.value = CreationSpeed = 0f;
        destructionSpeedSlider.value = DestructionSpeed = 0f;

        for(int i=0;i<shapes.Count;i++)
        {
            shapes[i].Recycle();
        }
        shapes.Clear();

        dyingShapeCount = 0;
    }

    public void AddShape(Shape shape)
    {
        shape.SaveIndex = shapes.Count;
        shapes.Add(shape);
    }

    void DestroyShape()
    {
        if(shapes.Count - dyingShapeCount > 0)
        {
            Shape shape = shapes[Random.Range(dyingShapeCount, shapes.Count)];
            if(destroyDuration <= 0f)
            {
                KillImmediately(shape);
            }
            else
            {
                shape.AddBehavior<DyingShapeBehavior>().Initialize(shape, destroyDuration);
            }
        }
        
    }

    public void Kill(Shape shape)
    {
        if (inGameUpdateLoop)
        {
            killList.Add(shape); //update도중에 kill이 일어나면 순서가 바뀌므로, kill 할것은 따로 list에 보관했다가 나중에 처리
        }
        else
        {
            KillImmediately(shape);
        }
        
    }

    private void KillImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        if(index < dyingShapeCount && index < --dyingShapeCount)
        {
            shapes[dyingShapeCount].SaveIndex = index;
            shapes[index] = shapes[dyingShapeCount];
            index = dyingShapeCount;
        }

        shape.Recycle();
        int lastIndex = shapes.Count - 1;
        if(index < lastIndex)
        {
            shapes[lastIndex].SaveIndex = index;
            shapes[index] = shapes[lastIndex];
        }
        shapes.RemoveAt(lastIndex);
    }

    void MarkAsDyingImmediately(Shape shape)
    {
        int index = shape.SaveIndex;
        if(index < dyingShapeCount) //이미 죽어가는 중이라면, 추가적인 작업 안함
        {
            return;
        }
        shapes[dyingShapeCount].SaveIndex = index;
        shapes[index] = shapes[dyingShapeCount];
        shape.SaveIndex = dyingShapeCount;
        shapes[dyingShapeCount++] = shape;
    }

    public void MarkAsDying(Shape shape)
    {
        if(inGameUpdateLoop)
        {
            markAsDyingList.Add(shape);
        }
        else
        {
            MarkAsDyingImmediately(shape);
        }
    }

    public bool IsMarkedAsDying(Shape shape)
    {
        return shape.SaveIndex < dyingShapeCount;
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(Random.state); //Random State를 함께 저장
        writer.Write(CreationSpeed);
        writer.Write(creationProgress); //creation과 destruction도 저장
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadLevelBuildIndex);
        GameLevel.Current.Save(writer);
        for(int i=0;i<shapes.Count;i++)
        {
            writer.Write(shapes[i].OriginFactory.FactoryId);
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }
        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                Random.state = state; //reseed를 하지 않는다는 이야기는 저장된 seed를 사용하겠다는 뜻
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();

        }

        //loadlevel이 끝난 뒤 shape들이 불러와지도록 yield return 활용
        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if(version >= 3)
        {
            GameLevel.Current.Load(reader);
        }

        for (int i = 0; i < count; i++)
        {
            int factoryId = version >= 5 ? reader.ReadInt() : 0;
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactories[factoryId].Get(shapeId, materialId);
            instance.Load(reader);
        }

        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].ResolveShapeInstances();
        }
    }

    public Shape GetShape(int index)
    {
        return shapes[index];
    }
}
