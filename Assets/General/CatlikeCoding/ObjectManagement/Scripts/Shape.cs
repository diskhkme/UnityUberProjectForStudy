using UnityEngine;

public class Shape : PersistableObject
{
    MeshRenderer meshRenderer;
    private void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
    }


    //read-only라면 좋지만, 어디선가 할당은 필요함
    //inspector에서 설정하게 할 수도 있지만(serialize field) prefab별이 아닌 instance별로 할당이 필요할 수도 있으므로 유연성이 필요
    int shapeId = int.MinValue;

    //프로퍼티로 만듬
    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            //readonly면 값 할당을 default인 0으로밖에 못함. 아니면 생성자에서 해 주어야 하는데, persistableObject는 monobehavior이므로 생성자 사용 불가
            if(shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
        }
    }

    public int MaterialId { get; private set; }

    //Material과 그 Id를 페어로 저장하기 위해서 별도의 set method 구현
    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    Color color;
    //property block은 property가 바뀔 때, 새로운 material을 생성하지 않음.(https://thomasmountainborn.com/2016/05/25/materialpropertyblocks/)
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    public void SetColor(Color color)
    {
        this.color = color;
        //meshRenderer.material.color = color; //새로운 color를 할당할때마다 새로운 Material을 만들게 되어서 비효율적. (sharedmaterial이 아님.)
        if(sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(sharedPropertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
    }




}