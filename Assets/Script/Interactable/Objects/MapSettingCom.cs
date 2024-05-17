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
		// ��ȣ�ۿ� �� �ؽ�Ʈ �Է� �ޱ�
		if (isInteracting)
		{
			promptMessage = string.Empty;

			foreach (char c in Input.inputString)
			{
				if (c == '\b') // �齺���̽� Ű �Է� ��
				{
					if (inputText.Length != 0)
					{
						inputText = inputText.Substring(0, inputText.Length - 1);
					}
				}
				else if (c == '\n' || c == '\r') // ���� Ű �Է� �� ��ȣ�ۿ� ����
				{
					EndInteraction();
				}
				else
				{
					inputText += c;
				}
			}

			// TextMeshPro �ؽ�Ʈ ������Ʈ
			textInputObject.text = inputText;
		}
		else
		{
			promptMessage = "�� ����";
		}
	}

	void EndInteraction()
	{
		isInteracting = false;
		SceneMapLoadManager.Instance.SetCurrentMap(inputText, interactUser);  // �Էµ� �ؽ�Ʈ�� ����
		interactUser.GetComponent<PlayerInputAction>().canMove = true;
		interactUser = null;
		inputText = "";          // �Է� �ʵ� �ʱ�ȭ
		textInputObject.text = "";   // TextMeshPro �ؽ�Ʈ �ʱ�ȭ
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
