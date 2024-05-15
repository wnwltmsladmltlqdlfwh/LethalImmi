using UnityEditor;

[CustomEditor(typeof(Interactable),true)]
public class InteractableEditor : Editor
{
	public override void OnInspectorGUI() // ������(Inspector)���� ������ ������ �� �Լ��� ȣ��ȴ�.
	{
		Interactable interactable = (Interactable)target; // Interactable�� ĳ������ target

		if (interactable.GetType() == typeof(EventOnlyInteractable)) // �̺�Ʈ�θ� ��ȣ�ۿ�Ǵ� ������Ʈ
		{
			// base.OnInspectorGUI(); �⺻�Լ��� ���� ������Ұ� ������ ����ְ� ������Ʈ �޼����� �������� �����ؾ���
			interactable.promptMessage = EditorGUILayout.TextField("Prompt Message", interactable.promptMessage);
			EditorGUILayout.HelpBox("EventOnlyInteractable Ŭ������ UnityEvent������ ��ȣ�ۿ�˴ϴ�.", MessageType.Info);
			if (interactable.gameObject.GetComponent<InteractionEvent>() == null)   //Event�� ����� ��ȣ�ۿ� ������Ʈ
			{
				interactable.useEvents = true;
				interactable.gameObject.AddComponent<InteractionEvent>();
			}
		}
		else
		{
			base.OnInspectorGUI();
			if (interactable.useEvents) // UnityEvent�� ����� interactable�� ���� Ȯ��
			{
				if (interactable.gameObject.GetComponent<InteractionEvent>() == null)   //Event�� ����� ��ȣ�ۿ� ������Ʈ
					interactable.gameObject.AddComponent<InteractionEvent>();
			}
			else
			{
				if (interactable.gameObject.GetComponent<InteractionEvent>() != null)   //Event�� ������� �ʴ� ��ȣ�ۿ� ������Ʈ
					DestroyImmediate(interactable.GetComponent<InteractionEvent>());
			}
		}
	}
}
