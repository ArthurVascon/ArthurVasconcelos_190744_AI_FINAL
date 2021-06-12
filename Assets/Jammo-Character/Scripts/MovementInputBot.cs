
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class MovementInputBot : MonoBehaviour {

    public float Velocity;
    [Space]

	public float InputX;
	public float InputZ;
	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed = 0.4f;
	public float allowPlayerRotation = 0.1f;
	public bool isGrounded;

	public int health = 3;

	public GameObject player;

	NavMeshAgent navmesh;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    public float verticalVel;
    private Vector3 moveVector;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		navmesh = GetComponent<NavMeshAgent>();
		health = FindObjectOfType<GameController>().healthRobots;
	}
	
	// Update is called once per frame
	void Update () {
		Speed = 0.4f;
			anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
    }

    void PlayerMoveAndRotation() {

		var forward = transform.forward;
		var right = transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize ();
		right.Normalize ();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
		}
	}

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {

        var camera = Camera.main;
        var forward = transform.forward;
        var right = transform.right;

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

	void InputMagnitude() {

        //Physically move player

		if (Speed > allowPlayerRotation) {
			anim.SetFloat ("Blend", Speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation ();
		} else if (Speed < allowPlayerRotation) {
			anim.SetFloat ("Blend", Speed, StopAnimTime, Time.deltaTime);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		//Colisão do tiro para diminuir a vida.
		if (col.gameObject.tag == "bullet")
		{
			health -= 1;
			Debug.Log(health);
			if(health == 0)
            {
				Debug.Log("Robô morreu.");
				GameController.Instance.RobotDeath();
				GameController.Instance.Win();
            }
		}
	}

	//Panda
	[Task]
	public void PickRandomDestination()
	{
		//Vetor 3 aleatório de posição
		Vector3 dest = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
		//NavMesh para posição aleatória
		navmesh.SetDestination(dest);
		//Panda - Succeed
		Task.current.Succeed();
	}

	[Task]
	public void MoveToDestination()
	{
		//Debug para a movimentação através do tempo
		if (Task.isInspected)
			Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
		//Verificação de chegada no ponto
		if (navmesh.remainingDistance <= navmesh.stoppingDistance && !navmesh.pathPending)
		{
			Task.current.Succeed();
		}
	}

	//Padrão de morte

	[Task]
	public bool IsHealthLessThan(float health)
	{
		return this.health < health;
	}

	[Task]
	public bool Explode()
	{
		//Destroy(healthBar.gameObject);
		this.gameObject.SetActive(false);
		return true;
	}

	//Fuga para Waypoint
	[Task]
	public void FleeToSafePlace()
    {

    }

	//FOV of Enemy
	[Task]
	public bool FindPlayerInSight()
    {
		RaycastHit hit;
		Vector3 rayDirection = player.transform.position - transform.position;
		float fieldOfViewDegrees = 135f;
		float visibilityDistance = 30f;

		if ((Vector3.Angle(rayDirection, transform.forward)) <= fieldOfViewDegrees * 0.5f)
		{
			// Detect if player is within the field of view
			if (Physics.Raycast(transform.position, rayDirection, out hit, visibilityDistance))
			{
				Debug.Log("Hitou");
				return (hit.transform.CompareTag("Player"));
			}
		}

		return false;
	}

	[Task]
	bool SeePlayer()
	{
		Vector3 distance = player.transform.position - this.transform.position;
		RaycastHit hit;
		bool seeWall = false;
		float visibleRange = 10f;
		Debug.DrawRay(this.transform.position, distance, Color.red);
		if (Physics.Raycast(this.transform.position, distance, out hit))
		{
			if (hit.collider.gameObject.tag == "wall")
			{
				seeWall = true;
			}
		}
		if (Task.isInspected)
			Task.current.debugInfo = string.Format("wall={0}", seeWall);
		if (distance.magnitude < visibleRange && !seeWall)
			return true;
		else
			return false;
	}
}
