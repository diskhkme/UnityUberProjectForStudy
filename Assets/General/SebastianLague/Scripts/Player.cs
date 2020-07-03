using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5f;
    public Crosshair crosshair;

    PlayerController controller;
    GunController gunController;
    Camera viewCamera;

    private void Awake()
    {
        //OnNewWave의 호출도 Start에서 이루어지기 때문에, 미리 등록을 확실히 하기 위해 Awake로 옮김
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    protected override void Start()
    {
        base.Start();
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(Random.Range(0, gunController.allGuns.Length));
    }

    void Update()
    {
        //Movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //Look
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);
            
            
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
            //print((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).magnitude);
        }

        //Weapon
        if(Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("PlayerDeath", transform.position);
        base.Die();
    }
    
}
