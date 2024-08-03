using UnityEngine;

namespace Base
{
	public class DescriptionBaseSO : SerializableScriptableObject
	{
		[TextArea] public string Description;
	}
}