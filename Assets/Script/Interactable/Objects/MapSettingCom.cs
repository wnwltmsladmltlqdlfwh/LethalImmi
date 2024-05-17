using System.Collections;
using TMPro;
using UnityEngine;

public class MapSettingCom : Interactable
{
    [SerializeField]
    TextMeshPro textInputObject;

	string inputText;
    bool isInteracting = false;
	GameObject interactUser = null;

    private void Start()
    {
		needLongPush = false;
    }

    private void Update()
    {
		// 상호작용 중 텍스트 입력 받기
		if (isInteracting)
		{
			promptMessage = string.Empty;

			foreach (char c in Input.inputString)
			{
				if (c == '\b') // 백스페이스 키 입력 시
				{
					if (inputText.Length != 0)
					{
						inputText = inputText.Substring(0, inputText.Length - 1);
					}
				}
				else if (c == '\n' || c == '\r') // 엔터 키 입력 시 상호작용 종료
				{
					EndInteraction();
				}
				else
				{
					inputText += c;
				}
			}

			// TextMeshPro 텍스트 업데이트
			textInputObject.text = inputText;
		}
		else
		{
			promptMessage = "맵 세팅";
		}
	}

	void EndInteraction()
	{
		isInteracting = false;
		SceneMapLoadManager.Instance.SetCurrentMap(inputText, interactUser);  // 입력된 텍스트를 저장
		interactUser.GetComponent<PlayerInputAction>().canMove = true;
		interactUser = null;
		inputText = "";          // 입력 필드 초기화
		textInputObject.text = "";   // TextMeshPro 텍스트 초기화
	}

	protected override void Interact(GameObject player)
    {
        base.Interact(player);

		interactUser = player;
		interactUser.GetComponent<PlayerInputAction>().canMove = false;
		isInteracting = true;
    }

	IEnumerator ComputerInteract(GameObject player)
	{
		yield return new WaitWhile(() => isInteracting == true);
	}
}
