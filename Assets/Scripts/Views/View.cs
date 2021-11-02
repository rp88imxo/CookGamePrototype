using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CookingPrototype.Kitchen.Views {
public abstract class View : MonoBehaviour
{
	[Foldout("View")]
	[SerializeField]
	private GameObject _rootObject;

	public event Action Shown = () => { };
	public event Action Hidden = () => { };
	
	[PublicAPI]
	public virtual void Show()
	{
		SetActiveState(true);
	}

	[PublicAPI]
	public virtual void Hide()
	{
		SetActiveState(false);
	}

	[PublicAPI]
	public virtual void Toggle(bool state)
	{
		SetActiveState(state);
	}

	private void SetActiveState(bool state, bool throwEvent = true)
	{
		if(_rootObject == null)
			_rootObject = gameObject;
            
		_rootObject.SetActive(state);
            
		if(!throwEvent) return;
            
		if(state) 
			Shown?.Invoke();
		else
			Hidden?.Invoke();
	}

	#if UNITY_EDITOR
	[Button("Show")]
	[UsedImplicitly]
	public void EDITOR_SHOW()
	{
		SetActiveState(true, false);
	}
        
	[Button("Hide")]
	[UsedImplicitly]
	public void EDITOR_HIDE()
	{
		SetActiveState(false, false);
	}
        
	#endif
}

}
