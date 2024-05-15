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

	// 애니메이션
	Animator animator;
	private bool hasAnimator; // animator 컴포넌트를 가지고 있는지 판단
	public float animBlendX; // 블렌드트리 좌우 이동값
	public float animBlendY; // 블렌드트리 전후 이동값
	public AudioClip[] footStepSources; // 걸음소리
	public AudioClip landSource; // 착지소리
	public float stepVolume; // 소리 볼륨

	// 플레이어 이동
    public float walkSpeed = 3.0f;
    public float runSpeed = 5.0f;
	public float rotaionSpeed = 1.0f;
	public float speedChangeRate = 10.0f;

	// 플레이어 점프
    public float jumpForce = 5.0f;
    public float gravityForce = -9.8f;
    public float jumpTimeout = 0.3f;

	// 낙하 / 점프 제한시간
	public float jumpTimeOut = 0.5f; // 점프 제한 시간
	[SerializeField]
	private float jumpDeltaTime;
	public float fallTimeOut = 0.15f; // 낙하 제한 시간
	[SerializeField]
	private float fallDeltaTime;

	// 땅에 닿아있는지 체크
	public bool isGrounded = true;
    public float groundedOffset = -0.14f;
    public float groundedRadious = 0.5f;
    public LayerMask groundedLayer;

	// 카메라
    public GameObject _mainCam;
	public GameObject camTarget;
	private float cinemachineTargetPitch;
	public const float threshold = 0.01f;
    public float topClamp = 90.0f;
    public float bottomClamp = -90.0f;

	//포톤
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
		// 이동속도 변경
		float currentSpeed = action.sprint ? runSpeed : walkSpeed;

		// 입력이 없을 시 이동속도 0
		if(action.move.magnitude <= 0) { currentSpeed = 0; }

		condition.RunStaminaUpdate(action.sprint && action.move.magnitude > 0);

		if (action.sprint == true && condition.staminaAmount <= 0)
		{
			action.sprint = false;
		}

		// 플레이어의 수평 이동에 대한 참조
		float currentHorizontalSpeed =
			new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

		// 이동 시 변위차에 대한 값
		float speedOffset = 0.1f;

		// 플레이어의 수평 이동속도가 목표 속도로 가속/감속
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

		// 애니메이션 블렌드트리를 위한 값
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
		if (isGrounded) //땅에 닿은 상태일때
		{
			fallDeltaTime = fallTimeOut; // 땅에 닿으면 낙하 시간 초기화

			if(hasAnimator)
			{
				animator.SetBool("Jump", false);
				animator.SetBool("Fall", false);
			}

			// 지면에 닿으면 _verticalVelocity 수치가 떨어지는걸 막음
			if (_verticalVelocity < 0f)
			{
				_verticalVelocity = -2f;
			}

			// jump키가 눌리고 jumpDeltaTime이 0이거나 낮을때(땅에 닿은 상태일때)
			if(action.jump && jumpDeltaTime <= 0.0f)
			{
				// 점프하기 위한 높이로 jumpForce * -2f * gravityForce값의 제곱근값
				_verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravityForce);

				if (hasAnimator)
				{
					animator.SetBool("Jump", true);
				}
			}

			// jumpDeltaTime 카운트 시작
			if (jumpDeltaTime > 0f)
			{
				jumpDeltaTime -= Time.deltaTime * 2f;
			}
		}
		else // 땅에 닿지 않은 상태일때
		{
			jumpDeltaTime = jumpTimeout; // 점프 시간 초기화

			// fallDeltaTime 카운트 시작
			if (fallDeltaTime >= 0f)
			{
				fallDeltaTime -= Time.deltaTime;
			}
			else // 닿았을때
			{
				if (hasAnimator)
				{
					animator.SetBool("Fall", true);
				}
			}

			// _terminalVelocity값보다 낮으면 선형적으로 _verticalVelocity값을 증가
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += gravityForce * Time.deltaTime;
			}
		}
	}

	private void CameraRotaion()
	{
		if(action.look.sqrMagnitude >= threshold) // 마우스의 입력이 들어왔을때
		{
			cinemachineTargetPitch += action.look.y * rotaionSpeed * 1.0f;
			_rotationVelocity = action.look.x * rotaionSpeed * 1.0f;

			cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

			camTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0f, 0f);

			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	// 카메라가 -360도 미만일때와 360도 초과일때, 위치를 조정해준다.
	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}


	private void OnFootStep(AnimationEvent _event) // 발소리 재생 이벤트
	{
		if(_event.animatorClipInfo.weight > 0.5f) // event의 가중치가 0.5f 이상이면
		{
			if (footStepSources.Length > 0)
			{
				var index = Random.Range(0, footStepSources.Length);
				// 로컬 좌표축 기준으로 계산한 characterController.center값을 월드 좌표축 기준으로 변경하고 랜덤한 발소리 재생
				AudioSource.PlayClipAtPoint(
					footStepSources[index], transform.TransformPoint(characterController.center), stepVolume);
			}
		}
	}

	private void OnLand(AnimationEvent _event) // 착지 시 소리 재생 이벤트
	{
		if(_event.animatorClipInfo.weight > 0.5f)
		{
			// 로컬 좌표축 기준으로 계산한 characterController.center값을 월드 좌표축 기준으로 변경하고 착지 소리 재생
			AudioSource.PlayClipAtPoint(landSource, transform.TransformPoint(characterController.center), stepVolume);
		}
	}
}
