using UnityEditor;

[CustomEditor(typeof(Interactable),true)]
public class InteractableEditor : Editor
{
	public override void OnInspectorGUI() // 편집기(Inspector)에서 변경할 때마다 이 함수가 호출된다.
	{
		Interactable interactable = (Interactable)target; // Interactable로 캐스팅한 target

		if (interactable.GetType() == typeof(EventOnlyInteractable)) // 이벤트로만 상호작용되는 오브젝트
		{
			// base.OnInspectorGUI(); 기본함수가 없어 구성요소가 완전히 비어있고 프롬포트 메세지를 수동으로 생성해야함
			interactable.promptMessage = EditorGUILayout.TextField("Prompt Message", interactable.promptMessage);
			EditorGUILayout.HelpBox("EventOnlyInteractable 클래스는 UnityEvent만으로 상호작용됩니다.", MessageType.Info);
			if (interactable.gameObject.GetComponent<InteractionEvent>() == null)   //Event를 사용할 상호작용 오브젝트
			{
				interactable.useEvents = true;
				interactable.gameObject.AddComponent<InteractionEvent>();
			}
		}
		else
		{
			base.OnInspectorGUI();
			if (interactable.useEvents) // UnityEvent를 사용할 interactable의 상태 확인
			{
				if (interactable.gameObject.GetComponent<InteractionEvent>() == null)   //Event를 사용할 상호작용 오브젝트
					interactable.gameObject.AddComponent<InteractionEvent>();
			}
			else
			{
				if (interactable.gameObject.GetComponent<InteractionEvent>() != null)   //Event를 사용하지 않는 상호작용 오브젝트
					DestroyImmediate(interactable.GetComponent<InteractionEvent>());
			}
		}
	}
}
