using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerInputAction))]
public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    PlayerInputAction action;
	PlayerInput input;
	PlayerCondition condition;

	// �ִϸ��̼�
	Animator animator;
	private bool hasAnimator; // animator ������Ʈ�� ������ �ִ��� �Ǵ�
	public float animBlendX; // ����Ʈ�� �¿� �̵���
	public float animBlendY; // ����Ʈ�� ���� �̵���
	public AudioClip[] footStepSources; // �����Ҹ�
	public AudioClip landSource; // �����Ҹ�
	public float stepVolume; // �Ҹ� ����

	// �÷��̾� �̵�
    public float walkSpeed = 3.0f;
    public float runSpeed = 5.0f;
	public float rotaionSpeed = 1.0f;
	public float speedChangeRate = 10.0f;

	// �÷��̾� ����
    public float jumpForce = 5.0f;
    public float gravityForce = -9.8f;
    public float jumpTimeout = 0.3f;

	// ���� / ���� ���ѽð�
	public float jumpTimeOut = 0.5f; // ���� ���� �ð�
	[SerializeField]
	private float jumpDeltaTime;
	public float fallTimeOut = 0.15f; // ���� ���� �ð�
	[SerializeField]
	private float fallDeltaTime;

	// ���� ����ִ��� üũ
	public bool isGrounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadious = 0.5f;
    public LayerMask groundedLayer;

	// ī�޶�
    public GameObject _mainCam;
	public GameObject camTarget;
	private float cinemachineTargetPitch;
	public const float threshold = 0.01f;
    public float topClamp = 90.0f;
    public float bottomClamp = -90.0f;

	//����
	private PhotonView mPhotonView;

	[SerializeField]
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	private void Awake()
	{
		if( _mainCam == null ) 
		{
			_mainCam = GameObject.FindGameObjectWithTag("MainCamera");
		}
		mPhotonView = GetComponent<PhotonView>();
	}

	private void Start()
	{
		hasAnimator = TryGetComponent(out animator);
		characterController = GetComponent<CharacterController>();
		action = GetComponent<PlayerInputAction>();
		input = GetComponent<PlayerInput>();
		condition = GetComponent<PlayerCondition>();

		jumpDeltaTime = jumpTimeout;
		fallDeltaTime = fallTimeOut;
	}

	private void Update()
	{
		if (mPhotonView.IsMine == false)	{ return; }

		hasAnimator = TryGetComponent(out animator);

		JumpAndGravity();
		GroundedCheck();
		Move();
	}

	private void LateUpdate()
	{
		if (mPhotonView.IsMine == false) { return; }
		CameraRotaion();
	}

	private void GroundedCheck()
	{
		Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
		isGrounded = Physics.CheckSphere(spherePos, groundedRadious, groundedLayer, QueryTriggerInteraction.Ignore);

		if (hasAnimator)
		{
			animator.SetBool("Grounded", isGrounded);
		}
	}

	private void Move()
	{
		// �̵��ӵ� ����
		float currentSpeed = action.sprint ? runSpeed : walkSpeed;

		// �Է��� ���� �� �̵��ӵ� 0
		if(action.move.magnitude <= 0) { currentSpeed = 0; }

		condition.RunStaminaUpdate(action.sprint && action.move.magnitude > 0);

		if (action.sprint == true && condition.staminaAmount <= 0)
		{
			action.sprint = false;
		}

		// �÷��̾��� ���� �̵��� ���� ����
		float currentHorizontalSpeed =
			new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

		// �̵� �� �������� ���� ��
		float speedOffset = 0.1f;

		// �÷��̾��� ���� �̵��ӵ��� ��ǥ �ӵ��� ����/����
		if (currentHorizontalSpeed < currentSpeed - speedOffset || currentHorizontalSpeed > currentSpeed + speedOffset)
		{
			_speed = Mathf.Lerp(currentHorizontalSpeed, currentSpeed, Time.deltaTime * speedChangeRate);
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = currentSpeed;
		}

		Vector3 inputDir = new Vector3(action.move.x, 0f, action.move.y).normalized;

		if(action.move != Vector2.zero)
		{
			inputDir = transform.right * action.move.x + transform.forward * action.move.y;
		}

		// �ִϸ��̼� ����Ʈ���� ���� ��
		animBlendX = Mathf.Lerp(animBlendX, currentSpeed * action.move.x, Time.deltaTime * speedChangeRate);
		animBlendY = Mathf.Lerp(animBlendY, currentSpeed * action.move.y, Time.deltaTime * speedChangeRate);
		
		if(action.move == Vector2.zero)
		{ 
			animBlendX = 0f;
			animBlendY = 0f;
		}

		characterController.Move(inputDir * (_speed * Time.deltaTime) + new Vector3(0, _verticalVelocity, 0) * Time.deltaTime);

		if (hasAnimator)
		{
			animator.SetFloat("XInput", animBlendX);
			animator.SetFloat("YInput", animBlendY);
			animator.SetFloat("MotionSpeed", 1f);
		}
	}

	private void JumpAndGravity()
	{
		if (isGrounded) //���� ���� �����϶�
		{
			fallDeltaTime = fallTimeOut; // ���� ������ ���� �ð� �ʱ�ȭ

			if(hasAnimator)
			{
				animator.SetBool("Jump", false);
				animator.SetBool("Fall", false);
			}

			// ���鿡 ������ _verticalVelocity ��ġ�� �������°� ����
			if (_verticalVelocity < 0f)
			{
				_verticalVelocity = -2f;
			}

			// jumpŰ�� ������ jumpDeltaTime�� 0�̰ų� ������(���� ���� �����϶�)
			if(action.jump && jumpDeltaTime <= 0.0f)
			{
				// �����ϱ� ���� ���̷� jumpForce * -2f * gravityForce���� �����ٰ�
				_verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravityForce);

				if (hasAnimator)
				{
					animator.SetBool("Jump", true);
				}
			}

			// jumpDeltaTime ī��Ʈ ����
			if (jumpDeltaTime > 0f)
			{
				jumpDeltaTime -= Time.deltaTime * 2f;
			}
		}
		else // ���� ���� ���� �����϶�
		{
			jumpDeltaTime = jumpTimeout; // ���� �ð� �ʱ�ȭ

			// fallDeltaTime ī��Ʈ ����
			if (fallDeltaTime >= 0f)
			{
				fallDeltaTime -= Time.deltaTime;
			}
			else // �������
			{
				if (hasAnimator)
				{
					animator.SetBool("Fall", true);
				}
			}

			// _terminalVelocity������ ������ ���������� _verticalVelocity���� ����
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += gravityForce * Time.deltaTime;
			}
		}
	}

	private void CameraRotaion()
	{
		if(action.look.sqrMagnitude >= threshold) // ���콺�� �Է��� ��������
		{
			cinemachineTargetPitch += action.look.y * rotaionSpeed * 1.0f;
			_rotationVelocity = action.look.x * rotaionSpeed * 1.0f;

			cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

			camTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);

			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	// ī�޶� -360�� �̸��϶��� 360�� �ʰ��϶�, ��ġ�� �������ش�.
	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}


	private void OnFootStep(AnimationEvent _event) // �߼Ҹ� ��� �̺�Ʈ
	{
		if(_event.animatorClipInfo.weight > 0.5f) // event�� ����ġ�� 0.5f �̻��̸�
		{
			if (footStepSources.Length > 0)
			{
				var index = Random.Range(0, footStepSources.Length);
				// ���� ��ǥ�� �������� ����� characterController.center���� ���� ��ǥ�� �������� �����ϰ� ������ �߼Ҹ� ���
				AudioSource.PlayClipAtPoint(
					footStepSources[index], transform.TransformPoint(characterController.center), stepVolume);
			}
		}
	}

	private void OnLand(AnimationEvent _event) // ���� �� �Ҹ� ��� �̺�Ʈ
	{
		if(_event.animatorClipInfo.weight > 0.5f)
		{
			// ���� ��ǥ�� �������� ����� characterController.center���� ���� ��ǥ�� �������� �����ϰ� ���� �Ҹ� ���
			AudioSource.PlayClipAtPoint(landSource, transform.TransformPoint(characterController.center), stepVolume);
		}
	}
}
