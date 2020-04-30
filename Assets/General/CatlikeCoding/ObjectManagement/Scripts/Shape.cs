using UnityEngine;

public class Shape : PersistableObject
{
    MeshRenderer meshRenderer;

    int shapeId = int.MinValue;
    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            if(shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
        }
    }

    public int MaterialId { get; private set; }

    public void SetMaterial(Material material, int materialId)
    {
        meshRenderer.material = material;
        MaterialId = materialId;
    }

    Color color;
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    public void SetColor(Color color)
    {
        this.color = color;
        if(sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(sharedPropertyBlock);
    }

    public Vector3 AngularVelocity { get; set; }

    private void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
    }

    // void FixedUpdate() //개별적으로 FixedUpdate 호출은 성능에 매우 악영향
    public void GameUpdate()
    {
        this.transform.Rotate(AngularVelocity * Time.deltaTime);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
        writer.Write(AngularVelocity);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
    }




}